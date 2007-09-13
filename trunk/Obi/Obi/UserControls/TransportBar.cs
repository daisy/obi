using System;
using System.Collections.Generic;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// The transport bar: transport buttons, scrubbing slide, time display, text vu meter display.
    /// </summary>
    public partial class TransportBar : UserControl, IControlWithSelection
    {
        // TO BE REMOVED
        private ProjectPanel mProjectPanel;  // project panel to which the transport bar belongs

        private ProjectView.ProjectView mProjectView;  // the project view to which this transport bar belongs

        public ProjectView.ProjectView ProjectView
        {
            get { return mProjectView; }
            set { mProjectView = value; }
        }


        private Audio.AudioPlayer mPlayer;   // the player for this playlist
        private Audio.AudioRecorder m_Recorder; // AudioRecorder for this transport bar 
        private Audio.VuMeter m_VuMeter;    // VuMeter for this transport bar
        private Playlist mMasterPlaylist;    // master playlist (all phrases in the project)
        private Playlist mLocalPlaylist;     // local playlist (only selected; may be null)
        private Playlist mCurrentPlaylist;   // playlist currently playing
        private NodeSelection mPlayingFrom;  // selection before playback started
        
        private SectionNode m_CurrentPlayingSection;  // holds section currently being played for highlighting it in TOC view while playing

        // A  non fluctuating flag to be set till playing of assets in serial is continued
        // it is true while a series of assets are being played but false when only single asset is played
        // so it is true for play all command and true for play node command when node is section
        // for Playing playlist state of serial play of asset it is true while for pause and stop state during serial play, it is false
        private bool m_IsSerialPlaying = false;

        private bool m_PlayOnFocusEnabled = true;     // Avn: for controlling triggering of OnFocus playback.

        RecordingSession inlineRecordingSession = null; // LNN: hack for doing non-dialog recording.
        public bool IsInlineRecording
        { get { return (inlineRecordingSession != null); } }
        private bool mDidCreateSectionForRecording = false;
        private SectionNode mRecordingToSection = null;
        private int mRecordingStartIndex = 0;


        // constants from the display combo box
        private static readonly int Elapsed = 0;
        private static readonly int ElapsedTotal = 1;
        private static readonly int Remain = 2;
        // private static readonly int RemainTotal = 3;

        // Pass the state change and playback rate change events from the playlist
        public event Events.Audio.Player.StateChangedHandler StateChanged;
        public event EventHandler PlaybackRateChanged;


        /// <summary>
        /// Initialize the transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mPlayer = new Audio.AudioPlayer();
            m_Recorder = new Obi.Audio.AudioRecorder();
            m_VuMeter = new Obi.Audio.VuMeter(mPlayer, m_Recorder);
            mPlayer.VuMeter = m_VuMeter;
            m_Recorder.VuMeterObject = m_VuMeter;
            m_VuMeter.SetEventHandlers();
            mLocalPlaylist = null;
            mMasterPlaylist = new Playlist(mPlayer);
            SetPlaylistEvents(mMasterPlaylist);
            mCurrentPlaylist = mMasterPlaylist;
            mDisplayBox.SelectedIndex = ElapsedTotal;
            mRecordModeBox.SelectedIndex = 0; //First element will be the default selected one
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
            mProjectPanel = null;
        }


        /// <summary>
        /// The audio player used by the transport bar.
        /// </summary>
        public Audio.AudioPlayer AudioPlayer { get { return mPlayer; } }

        public Audio.AudioRecorder Recorder
        {
            get
            {
                return m_Recorder;
            }
        }

        public Audio.VuMeter VuMeter
        {
            get
            {
                return m_VuMeter;
            }
        }

        #region selection

        public ObiNode CurrentSelectedNode
        {
            get {
                //LNN: removed this line, since it's higly annoying when working in the designer
                //throw new Exception("Please don't ask me for a selection, I don't know anything about that stuff."); 

                if (mCurrentPlaylist != null)
                    if (mCurrentPlaylist.CurrentPhrase != null)
                        return mCurrentPlaylist.CurrentPhrase;

                //ok, I give up, this might cause you an error, so lay off asking me for the current selection!
                return null;
            }
            set
            {
                if (IsSelectionRelevant(value) && value is PhraseNode) mCurrentPlaylist.CurrentPhrase = (PhraseNode)value;
            }
        }

        public bool IsSelectionRelevant(ObiNode node)
        {
            //Avn: <> condition added for play on focus 13 May 2007
            if (IsSeriallyPlaying)
            {
                return ((mCurrentPlaylist.State == Audio.AudioPlayerState.Playing ||
                        mCurrentPlaylist.State == Audio.AudioPlayerState.Paused));
            }
            else
                return false;
        }

        public bool CanSelectPhrase(ObiNode node)
        {
            return mCurrentPlaylist != null && mCurrentPlaylist.ContainsPhrase(node as PhraseNode);
        }

        /// <summary>
        ///  Controls triggering of On Focus Playback
        /// <see cref=""/>
        /// </summary>
        public bool PlayOnFocusEnabled
        {
            get
            {
                return m_PlayOnFocusEnabled;
            }
            set
            {
                m_PlayOnFocusEnabled = value;
            }
        }


        #endregion


        #region properties

        public bool CanPlay
        {
            get { return Enabled && mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped && !IsInlineRecording; }
        }

        public bool CanRecord
        {
            get { 
                return Enabled && 
                    (
                        (mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped && mRecordModeBox.SelectedIndex==0)
                        || 
                        (!IsInlineRecording && mRecordModeBox.SelectedIndex>0)
                    ); 
            }
        }

        public bool CanResume
        {
            get 
            { 
                return Enabled   && mCurrentPlaylist.State == Audio.AudioPlayerState.Paused  
                  && !IsInlineRecording; 
            }
        }


        public bool IsSeriallyPlaying
        {
            get
            {
                return m_IsSerialPlaying;
            }
        }

        /// <summary>
        /// The playlist currently playing, or the master playlist by default.
        /// </summary>
        public Playlist _CurrentPlaylist
        {
            get { return mCurrentPlaylist; }
        }

        /// <summary>
        /// The transport bar as a whole can be enabled/disabled when necessary.
        /// Disabling the transport bar will also stop playback.
        /// </summary>
        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                if (base.Enabled && !value &&
                    (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing ||
                    mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused))
                {
                    Stop();
                }
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Enable or disable the tooltips for this component.
        /// </summary>
        public bool EnableTooltips
        {
            set { mTransportBarTooltip.Active = value; }
        }

        /// <summary>
        /// The local playlist allows to only play a selection.
        /// </summary>
        public Playlist LocalPlaylist
        {
            get { return mLocalPlaylist; }
            set
            {
                mLocalPlaylist = value;
                if (value != null) SetPlaylistEvents(mLocalPlaylist);
            }
        }

        /// <summary>
        /// The master playlist is automatically maintained and canot be modified.
        /// </summary>
        public Playlist MasterPlaylist
        {
            get { return mMasterPlaylist; }
        }

        /// <summary>
        /// Get/set the parent project panel.
        /// </summary>
        public ProjectPanel ProjectPanel
        {
            get { return mProjectPanel; }
            set { mProjectPanel = value; }
        }

        #endregion


        #region playlist events

        private void SetPlaylistEvents(Playlist playlist)
        {
            playlist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Play_MovedToPhrase);
            playlist.StateChanged += new Events.Audio.Player.StateChangedHandler(Play_PlayerStateChanged);
            playlist.PlaybackRateChanged += new Playlist.PlaybackRateChangedHandler(mPlaylist_PlaybackRateChanged);
            playlist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Play_PlayerStopped);
        }

        /// <summary>
        /// Highlight (i.e. select) the phrase currently playing.
        /// </summary>
        private void Play_MovedToPhrase(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            if (mProjectPanel.TOCPanel.ContainsFocus    &&    m_CurrentPlayingSection != mCurrentPlaylist.CurrentSection)
            {
                mProjectPanel.TOCPanel.CurrentSelectedNode = mCurrentPlaylist.CurrentSection;
                m_CurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                            }

            mProjectPanel.CurrentSelection = new NodeSelection(e.Node, mProjectPanel.StripManager);
        }

        /// <summary>
        /// Update the transport bar according to the player state.
        /// </summary>
        private void Play_PlayerStateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            if (mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped)
            {
                mDisplayTimer.Stop();
                //Play_PlayerStopped(this, null); // Avn: commented as was invoking EndOfPlaylist catch function which gave wrong indication of end of playlist
            }
            else if (mCurrentPlaylist.State == Audio.AudioPlayerState.Playing)
            {
                mDisplayTimer.Start();
            }
            if (StateChanged != null) StateChanged(this, e);
            UpdateTimeDisplay();
        }

        /// <summary>
        /// Simply pass the playback rate chang event.
        /// </summary>
        private void mPlaylist_PlaybackRateChanged(object sender, EventArgs e)
        {
            if (PlaybackRateChanged != null) PlaybackRateChanged(sender, e);
        }

        /// <summary>
        /// Update the transport bar once the player has stopped.
        /// </summary>
        private void Play_PlayerStopped(object sender, EventArgs e)
        {
            // Avn: condition added on 13 May 2007 
            //to prevent focus from returning to initial position  after playback if focus is not in same view
            if (IsSeriallyPlaying)
            {
                if (mPlayingFrom != null)
                {
                    if (mPlayingFrom.Control.GetType().FullName == "Obi.UserControls.TOCPanel"
        && mProjectPanel.TOCPanel.ContainsFocus)
                        mProjectPanel.CurrentSelection = mPlayingFrom;

                    else if (mPlayingFrom.Control.GetType().FullName == "Obi.UserControls.StripManagerPanel"
                    && mProjectPanel.StripManager.ContainsFocus)
                        mProjectPanel.CurrentSelection = mPlayingFrom;
                }
                                            }
            // Avn: statement added on 13 May 2007
        m_IsSerialPlaying = false;
        }

        #endregion

        #region buttons

        private void mPrevSectionButton_Click(object sender, EventArgs e)
        {
            PrevSection();
        }

        /// <summary>
        /// Move to the previous section (i.e. first phrase of the previous section.)
        /// </summary>
        public void PrevSection()
        {
            if (!IsInlineRecording)
            {
                m_IsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToPreviousSection();

                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                    m_IsSerialPlaying = false;
            }
        }

        private void mPrevPhraseButton_Click(object sender, EventArgs e)
        {
            PrevPhrase();
        }

        /// <summary>
        /// Move to or play the previous phrase.
        /// </summary>
        public void PrevPhrase()
        {
            if (!IsInlineRecording)
            {
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing)
                    m_IsSerialPlaying = true;

                if (Enabled) mCurrentPlaylist.NavigateToPreviousPhrase();

                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                    m_IsSerialPlaying = false;
            }
        }

        private void mRewindButton_Click(object sender, EventArgs e)
        {
            Rewind();
        }

        /// <summary>
        /// Rewind (i.e. play faster backward)
        /// </summary>
        public void Rewind()
        {
            if (Enabled)
            {
                    m_IsSerialPlaying = true;
                    mCurrentPlaylist.Rewind();
            }
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            Play();            
        }

        /// <summary>
        /// Find the phrase to play from from the selected one in the project panel.
        /// </summary>
        private PhraseNode InitialPhrase
        {
            get
            {
                if (mProjectPanel != null)
                {
                    if (mProjectPanel.CurrentSelection != null && mProjectPanel.CurrentSelectionNode.Used)
                    {
                        if (mProjectPanel.CurrentSelectionNode is PhraseNode)
                        {
                            return (PhraseNode)mProjectPanel.CurrentSelectionNode;
                        }
                        if (((SectionNode)mProjectPanel.CurrentSelectionNode).FirstUsedPhrase != null)
                        {
                            return ((SectionNode)mProjectPanel.CurrentSelectionNode).FirstUsedPhrase;
                        }
                    }
                    return mCurrentPlaylist.FirstPhrase;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Play the master playlist, starting from the selected phrase, or the first phrase of
        /// the selected section or the beginning of the project.
        /// </summary>
        public void Play()
        {
            //Avn:  for instantly playing MasterPlaylist, check if current playlist is local and stop if this LocalPlaylist not in stop state
            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped
                && mCurrentPlaylist == mLocalPlaylist)
                StopInternal();
            
            if (CanPlay)
            {
                mPlayingFrom = mProjectPanel.CurrentSelection;
                mCurrentPlaylist = mMasterPlaylist;
                mCurrentPlaylist.CurrentPhrase = InitialPhrase;
                if (mCurrentPlaylist.CurrentPhrase != null)
                {
                                                            m_IsSerialPlaying = true;
                    mCurrentPlaylist.Play();
                    m_CurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
            }
            else if (CanResume)
            {
                m_IsSerialPlaying = true;
                mCurrentPlaylist.Resume();
                m_CurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void Play(ObiNode node)
        {
            // Avn: For instantly playing LocalPlaylist check if current playlist is MasterPlaylist and if this MasterPlaylist is not in stopped state, stop it.
                                    if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped
                && mCurrentPlaylist == mMasterPlaylist)
            {
                                        // Avn: if keyboard focus is in toc panel, assign node to section as PlaySelection command in TOC panel plays a section
                if ( mProjectPanel.TOCPanel.ContainsFocus )
                node = m_CurrentPlayingSection;

                StopInternal();
                            }

                        if (CanPlay)
            {
                mPlayingFrom = mProjectPanel.CurrentSelection;
                LocalPlaylist = new Playlist(mPlayer, node);
                mCurrentPlaylist = mLocalPlaylist;
                mCurrentPlaylist.CurrentPhrase = mLocalPlaylist.FirstPhrase;
                                                
                // Avn: condition added on 13 may 2007
                if ( mCurrentPlaylist.PhraseList.Count > 1 )
                m_IsSerialPlaying = true;

                mCurrentPlaylist.Play();
                m_CurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
            else if (CanResume)
            {
                // Avn: condition added on 13 may 2007
                if ( mCurrentPlaylist.PhraseList.Count > 1 )
                m_IsSerialPlaying = true;

                mCurrentPlaylist.Resume();
                m_CurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
            else if (mCurrentPlaylist != mMasterPlaylist)
            {
                Stop();
                // Avn: following line replaced by next line as this function also work in toc panel
                                //mProjectPanel.CurrentSelection = new NodeSelection(node, mProjectPanel.StripManager);
                node = mProjectPanel.CurrentSelection.Node;

                if ( mCurrentPlaylist.PhraseList.Count > 1 )
                m_IsSerialPlaying = true;

                                Play(node);
            }
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// The pause button.
        /// </summary>
        public void Pause()
        {
            if (Enabled)
            {
                mCurrentPlaylist.Pause();
                m_IsSerialPlaying = false;
            }
        }

        private void mRecordButton_Click(object sender, EventArgs e)
        {
            Record();
        }

        /// <summary>
        /// Start listening/recording.
        /// </summary>
        public void Record()
        {
            if (CanRecord)
            {
                ObiNode selected = mProjectPanel.CurrentSelectionNode;
                SectionNode section;  // section in which we are recording
                int index;            // index from which we add new phrases in the aforementioned section
                bool IsSectionCreated = false; // Flag to indicate creation of a section as to undo it if nothing is recorded.
                if (selected == null)
                {
                    // nothing selected: append a new section and start from 0
                    section = mProjectPanel.Project.CreateSiblingSectionNode(null);
                    index = 0;
                    IsSectionCreated = true;
                }
                else if (selected is SectionNode)
                {
                    // a section is selected: append in the section
                    section = (SectionNode)selected;
                    index = section.PhraseChildCount;
                }
                else
                {
                    // a phrase is selected: inster before the phrase
                    section = ((PhraseNode)selected).ParentSection;
                    index = ((PhraseNode)selected).Index;
                }
                Settings settings = ((ObiForm)ParentForm).Settings;
                RecordingSession session = new RecordingSession(mProjectPanel.Project, m_Recorder ,
                    settings.AudioChannels, settings.SampleRate, settings.BitDepth);
                // the following closures handle the various events sent during the recording session
                session.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.StartRecordingPhrase(_e, section, index + _e.PhraseIndex);
                    }
                );
                session.ContinuingPhrase += new Events.Audio.Recorder.ContinuingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.RecordingPhraseUpdate(_e, section, index + _e.PhraseIndex);
                    }
                );
                session.FinishingPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.RecordingPhraseUpdate(_e, section, index + _e.PhraseIndex);
                        mMasterPlaylist.UpdateTimeFrom(section.PhraseChild(index + _e.PhraseIndex).PreviousPhraseInProject);
                    }
                );
                session.FinishingPage += new Events.Audio.Recorder.FinishingPageHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        PhraseNode _node = section.PhraseChild(index + _e.PhraseIndex);
                        mProjectPanel.Project.DidSetPageNumberOnPhrase(_node);
                    }
                );

                if (mRecordModeBox.SelectedIndex == 0) //recording using the dialog
                {
                    new Dialogs.TransportRecord(session , m_VuMeter ).ShowDialog ();

                    // delete newly created section if nothing is recorded.
                    if (session.RecordedAudio.Count == 0 && IsSectionCreated)
                        this.mProjectPanel.ParentObiForm.UndoLast();

                    for (int i = 0; i < session.RecordedAudio.Count; ++i)
                    {
                        mProjectPanel.StripManager.UpdateAudioForPhrase(section.PhraseChild(index + i), session.RecordedAudio[i]);
                                                                    }
                }
                else //recording using the transportbar buttons
                {
                    if (mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
                    {
                        mProjectPanel.StripManager.QuickSplitBlock();
                        mCurrentPlaylist.Stop();
                        m_IsSerialPlaying = false;
                        index++; //for "punch in", we want to record between the parts of the split
                    }
                    if (mRecordModeBox.SelectedIndex == 2 && section.PhraseChildCount>0) //we are recording in destructive mode
                    {
                        PhraseNode removeableNode = section.PhraseChild(index);
                        while (removeableNode != null)
                        {
                            this.ProjectPanel.Project.DeletePhraseNode(removeableNode);
                            removeableNode = (index<section.PhraseChildCount)?section.PhraseChild(index):null;
                        }
                        this.ProjectPanel.CurrentSelection = null;
                    }

                    mDidCreateSectionForRecording = IsSectionCreated;
                    mRecordingToSection = section;
                    mRecordingStartIndex = index;
                    inlineRecordingSession = session;
                    inlineRecordingSession.Record();
                    UpdateInlineRecordingState();
                }
            }
        }

        public void UpdateInlineRecordingState()
        {
            if (IsInlineRecording)
            {
                mPrevPhraseButton.Enabled = false;
                mPrevSectionButton.Enabled = false;
                mRewindButton.Enabled = false;
                mPlayButton.Enabled = false;
                mFastForwardButton.Enabled = false;
                mNextSectionButton.Enabled = false;
                mPauseButton.Enabled = false;
                mRecordModeBox.Enabled = false;
            }
            else
            {
                mPrevPhraseButton.Enabled = this.Enabled;
                mPrevSectionButton.Enabled = this.Enabled;
                mRewindButton.Enabled = this.Enabled;
                mFastForwardButton.Enabled = this.Enabled;
                mPlayButton.Enabled = CanPlay;
                mNextPhrase.Enabled = this.Enabled;
                mNextSectionButton.Enabled = this.Enabled;
                mPauseButton.Enabled = this.Enabled;
                mRecordModeBox.Enabled = this.Enabled;
            }
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
                Stop();
        }

        /// <summary>
        /// The stop button. Stopping twice deselects all.
        /// </summary>
        public void Stop()
        {
            if (IsInlineRecording)
            {
                inlineRecordingSession.Stop();
                if(mDidCreateSectionForRecording && inlineRecordingSession.RecordedAudio.Count == 0)
                    this.mProjectPanel.ParentObiForm.UndoLast();

                for (int i = 0; i < inlineRecordingSession.RecordedAudio.Count; ++i)
                {
                    mProjectPanel.StripManager.UpdateAudioForPhrase(mRecordingToSection.PhraseChild(mRecordingStartIndex + i), inlineRecordingSession.RecordedAudio[i]);
                }
                
                mDidCreateSectionForRecording = false;
                inlineRecordingSession = null;
                UpdateInlineRecordingState();
            }
            else if (Enabled)
            {
                // Stopping again deselects everything
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    mProjectPanel.CurrentSelection = null;
                }
                else
                {
                    mCurrentPlaylist.Stop();
                    mProjectView.Selection = mPlayingFrom;
                }
                mPlayingFrom = null;
                m_IsSerialPlaying = false;
            }
        }

        private void mFastForwardButton_Click(object sender, EventArgs e)
        {
            
            FastForward();
        }

        /// <summary>
        /// Play faster.
        /// </summary>
        public void FastForward()
        {
            if (Enabled && !IsInlineRecording)
            {
                    m_IsSerialPlaying = true;
                    mCurrentPlaylist.FastForward();
            }
        }

        private void mNextPhrase_Click(object sender, EventArgs e)
        {
            NextPhrase();
        }

        /// <summary>
        /// Go to the next phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (IsInlineRecording)
            {
                inlineRecordingSession.NextPhrase();
            }
            else
            {
                m_IsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToNextPhrase();

                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                    m_IsSerialPlaying = false;
            }

        }

        private void mNextSectionButton_Click(object sender, EventArgs e)
        {
            NextSection();

        }

        /// <summary>
        /// Move to the next section (i.e. the first phrase of the next section)
        /// </summary>
        public void NextSection()
        {
            if (!IsInlineRecording)
            {
                m_IsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToNextSection();

                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                    m_IsSerialPlaying = false;
            }
        }


        // <> function added for play on focus 12 May 2007
        /// <summary>
        ///  Plays a single phrase when keyboard focus arrives on a audio block
        /// <see cref=""/>
        /// </summary>
        /// <param name="node"></param>
        public void PlayPhraseOnFocus(ObiNode node)
        {
            
            if (mCurrentPlaylist != null
                && Enabled
                && PlayOnFocusEnabled )
            {
                //execute on checking if play all is not active in playing state
                if (!IsSeriallyPlaying )
                {
                    StopInternal();
                    Play(node);
                }// serial play  if ends
            }
        }

        /// <summary>
        /// Stops playlist without returning focus
        ///  to be used locally inside this class
        /// <see cref=""/>
        /// </summary>
        private void StopInternal()
        {
            mCurrentPlaylist.Stop();
            mPlayingFrom = null;
            m_IsSerialPlaying = false;
        }


        #endregion

        /// <summary>
        /// Periodically update the time display.
        /// </summary>
        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        /// <summary>
        /// Update the time display to show current time. Depends on the what kind of timing is selected.
        /// </summary>
        public void UpdateTimeDisplay()
        {
            if (Enabled && mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped)
            {
                mTimeDisplayBox.Text =
                    mDisplayBox.SelectedIndex == Elapsed ?
                        ObiForm.FormatTime_hh_mm_ss(mCurrentPlaylist.CurrentTimeInAsset) :
                    mDisplayBox.SelectedIndex == ElapsedTotal ?
                        ObiForm.FormatTime_hh_mm_ss(mCurrentPlaylist.CurrentTime) :
                    mDisplayBox.SelectedIndex == Remain ?
                        ObiForm.FormatTime_hh_mm_ss(mCurrentPlaylist.RemainingTimeInAsset) :
                        ObiForm.FormatTime_hh_mm_ss(mCurrentPlaylist.RemainingTime);
            }
            else
            {
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(0.0);
            }
            //LNN: needs handling of inline recording time
        }

        /// <summary>
        /// Update the time display immediatly when the display mode changes.
        /// </summary>
        private void mDisplayBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
        }

        public void FocusTimeDisplay()
        {
            mTimeDisplayBox.Focus();
        }

        /// <summary>
        /// The project panel has a new project; rebuild the master playlist.
        /// </summary>
        public void UpdatedProject()
        {
            Enabled = mProjectPanel.Project != null;
            mPlayingFrom = null;
            if (mProjectPanel.Project != null)
            {
                mMasterPlaylist.Project = mProjectPanel.Project;
                mCurrentPlaylist = mMasterPlaylist;
                mProjectPanel.Project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(Project_ToggledNodeUsedState);
                mProjectPanel.Project.MediaSet += new Obi.Events.SetMediaHandler(Project_MediaSet);
                mProjectPanel.Project.PastedSectionNode += new Obi.Events.SectionNodeHandler(Project_PastedSectionNode);
            }
        }

        void Project_AddedPhraseNode(object sender, Obi.Events.Node.PhraseNodeEventArgs e)
        {
            if (e.Node.Used) mMasterPlaylist.AddPhrase(e.Node);
                    }

        void Project_DeletedPhraseNode(object sender, Obi.Events.Node.PhraseNodeEventArgs e)
        {
            if (e.Node.Used) mMasterPlaylist.RemovePhrase(e.Node);
        }

        void Project_ToggledNodeUsedState(object sender, Obi.Events.Node.ObiNodeEventArgs e)
        {
            if (e.Node is PhraseNode)
            {
                if (e.Node.Used)
                {
                    mMasterPlaylist.AddPhrase((PhraseNode)e.Node);
                }
                else
                {
                    mMasterPlaylist.RemovePhrase((PhraseNode)e.Node);
                }
            }
        }

        void Project_MediaSet(object sender, Obi.Events.Node.SetMediaEventArgs e)
        {
            if (e.Node.Used) mMasterPlaylist.UpdateTimeFrom(e.Node);
        }


        void Project_PastedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            if (e.Node != null)
            {

                e.Node.acceptDepthFirst
    (
                    // add all used phrases under this section node to master playlist
                                        delegate(urakawa.core.TreeNode n)
                                        {
                                            if (n is PhraseNode && ((PhraseNode)n).Used)
                                            {
                                                mMasterPlaylist.AddPhrase((PhraseNode)n);
                                            }
                                            return true;
                                        },
                        delegate(urakawa.core.TreeNode n) { }
    );

            }
        }

        private void mRecordModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
