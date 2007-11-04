using System;
using System.Collections.Generic;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.ProjectView
{
    /// <summary>
    /// The transport bar: transport buttons, scrubbing slide, time display, text vu meter display.
    /// </summary>
    public partial class TransportBar : UserControl
    {
        private ProjectView mView;                   // the parent project view
        private Audio.AudioPlayer mPlayer;           // the audio player
        private Audio.AudioRecorder mRecorder;       // the audio recorder
        private Audio.VuMeter mVuMeter;              // VU meter
        private Playlist mMasterPlaylist;            // master playlist (all phrases in the project)
        private Playlist mLocalPlaylist;             // local playlist (only selected; may be null)
        private Playlist mCurrentPlaylist;           // playlist currently playing
        private RecordingSession mRecordingSession;  // current recording session
        private NodeSelection mPlayingFrom;          // selection before playback started
        private SectionNode mCurrentPlayingSection;  // holds section currently being played for highlighting it in TOC view while playing

        // A  non fluctuating flag to be set till playing of assets in serial is continued
        // it is true while a series of assets are being played but false when only single asset is played
        // so it is true for play all command and true for play node command when node is section
        // for Playing playlist state of serial play of asset it is true while for pause and stop state during serial play, it is false
        private bool mIsSerialPlaying = false;
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
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        public void NewPresentation()
        {
            mPlayingFrom = null;
            mMasterPlaylist.Presentation = mView.Presentation;
        }

        /// <summary>
        /// Initialize the transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mView = null;
            mPlayer = new Audio.AudioPlayer();
            mRecorder = new Obi.Audio.AudioRecorder();
            mVuMeter = new Obi.Audio.VuMeter(mPlayer, mRecorder);
            mVuMeter.SetEventHandlers();
            mLocalPlaylist = null;
            mMasterPlaylist = new Playlist(mPlayer);
            SetPlaylistEvents(mMasterPlaylist);
            mCurrentPlaylist = mMasterPlaylist;
            mDisplayBox.SelectedIndex = ElapsedTotal;
            mRecordModeBox.SelectedIndex = 0; //First element will be the default selected one
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
            mVUMeterPanel.VuMeter = mVuMeter;

            ComboFastPlateRate.Items.Add("1.0");
            ComboFastPlateRate.Items.Add("1.125");
            ComboFastPlateRate.Items.Add("1.25");
            ComboFastPlateRate.Items.Add("1.5");
            ComboFastPlateRate.Items.Add("1.75");
            ComboFastPlateRate.Items.Add("2.0");
            ComboFastPlateRate.SelectedIndex = 0;
        }

        public Audio.AudioPlayer AudioPlayer { get { return mPlayer; } }

        public bool CanPlay
        {
            get { return Enabled && mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped && !IsInlineRecording; }
        }

        public bool CanRecord
        {
            get
            {
                return Enabled &&
                    (
                        (mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped && mRecordModeBox.SelectedIndex == 0)
                        ||
                        (!IsInlineRecording && mRecordModeBox.SelectedIndex > 0)
                    );
            }
        }

        public bool CanResume
        {
            get
            {
                return Enabled &&
                    mCurrentPlaylist.State == Audio.AudioPlayerState.Paused &&
                    !IsInlineRecording;
            }
        }

        public Playlist CurrentPlaylist { get { return mCurrentPlaylist; } }

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

        public bool EnableTooltips { set { mTransportBarTooltip.Active = value; } }
        public bool IsSeriallyPlaying { get { return mIsSerialPlaying; } }

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


        public Audio.AudioRecorder Recorder { get { return mRecorder; } }
        public Audio.VuMeter VuMeter { get { return mVuMeter; } }
        public RecordingSession Recordingsession { get { return mRecordingSession; } }

        /// <summary>
        /// The master playlist is automatically maintained and canot be modified.
        /// </summary>
        public Playlist MasterPlaylist { get { return mMasterPlaylist; } }




        #region selection

        public NodeSelection Selection
        {
            get {
                //LNN: removed this line, since it's higly annoying when working in the designer
                //throw new Exception("Please don't ask me for a selection, I don't know anything about that stuff."); 
                /*
                if (mCurrentPlaylist != null)
                    if (mCurrentPlaylist.CurrentPhrase != null)
                        return mCurrentPlaylist.CurrentPhrase;
                */
                //ok, I give up, this might cause you an error, so lay off asking me for the current selection!
                return null;
            }
            set
            {
                //if (IsSelectionRelevant(value) && value is PhraseNode) mCurrentPlaylist.CurrentPhrase = (PhraseNode)value;
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



        #region playlist events

        private void SetPlaylistEvents(Playlist playlist)
        {
            playlist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Play_MovedToPhrase);
            playlist.StateChanged += new Events.Audio.Player.StateChangedHandler(Play_PlayerStateChanged);
            playlist.PlaybackRateChanged += new Playlist.PlaybackRateChangedHandler(mPlaylist_PlaybackRateChanged);
            playlist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Play_PlayerStopped);
        }

        // Move the cursor to next phrase
        // Highlight in the section view if it has the focus
        private void Play_MovedToPhrase(object sender, Events.Node.PhraseNodeEventArgs e)
        {
        }

        // Update the transport bar according to the player state.
        private void Play_PlayerStateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {            
            if (mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped)
            {
                mDisplayTimer.Stop();
            }
            else if (mCurrentPlaylist.State == Audio.AudioPlayerState.Playing)
            {
                mDisplayTimer.Start();
            }
            if (StateChanged != null) StateChanged(this, e);
            UpdateTimeDisplay();
        }

        // Simply pass the playback rate change event.
        private void mPlaylist_PlaybackRateChanged(object sender, EventArgs e)
        {
            if (PlaybackRateChanged != null) PlaybackRateChanged(sender, e);
        }

        // Update the transport bar once the player has stopped.
        private void Play_PlayerStopped(object sender, EventArgs e)
        {
            /*
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
             */
        }

        #endregion

        #region buttons

        private void mPrevSectionButton_Click(object sender, EventArgs e) { PrevSection(); }
        private void mPrevPhraseButton_Click(object sender, EventArgs e) { PrevPhrase(); }
        private void mRewindButton_Click(object sender, EventArgs e) { Rewind(); }
        private void mPlayButton_Click(object sender, EventArgs e) { Play(); }
        private void mPauseButton_Click(object sender, EventArgs e) { Pause(); }
        private void mRecordButton_Click(object sender, EventArgs e) { Record(); }
        private void mStopButton_Click(object sender, EventArgs e) { Stop(); }
        private void mFastForwardButton_Click(object sender, EventArgs e) { FastForward(); }
        private void mNextPhrase_Click(object sender, EventArgs e) { NextPhrase(); }
        private void mNextSectionButton_Click(object sender, EventArgs e) { NextSection(); }


        /// <summary>
        /// Move to the previous section (i.e. first phrase of the previous section.)
        /// </summary>
        public void PrevSection()
        {
            if (!IsInlineRecording)
            {
                mIsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToPreviousSection();
                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing) mIsSerialPlaying = false;
            }
        }

        /// <summary>
        /// Move to or play the previous phrase.
        /// </summary>
        public void PrevPhrase()
        {
            if (!IsInlineRecording)
            {
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing) mIsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToPreviousPhrase();
                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing) mIsSerialPlaying = false;
            }
        }

        /// <summary>
        /// Rewind (i.e. play faster backward)
        /// </summary>
        public void Rewind()
        {
            if (Enabled)
            {
                mIsSerialPlaying = true;
                mCurrentPlaylist.Rewind();
            }
        }

        /// <summary>
        /// Play the master playlist, starting from the selected phrase, or the first phrase of
        /// the selected section or the beginning of the project.
        /// </summary>
        public void Play()
        {
            //Avn: for instantly playing MasterPlaylist, check if current playlist is local
            // and stop if this LocalPlaylist not in stop state
            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped && mCurrentPlaylist == mLocalPlaylist) StopInternal();
            if (CanPlay)
            {
                mPlayingFrom = mView.Selection;
                mCurrentPlaylist = mMasterPlaylist;
                mCurrentPlaylist.CurrentPhrase = InitialPhrase;
                if (mCurrentPlaylist.CurrentPhrase != null)
                {
                    mIsSerialPlaying = true;
                    mCurrentPlaylist.Play();
                    mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
            }
            else if (CanResume)
            {
                mIsSerialPlaying = true;
                mCurrentPlaylist.Resume();
                mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void Play(ObiNode node)
        {
            // Avn: For instantly playing LocalPlaylist check if current playlist is MasterPlaylist
            // and if this MasterPlaylist is not in stopped state, stop it.
            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped
                && mCurrentPlaylist == mMasterPlaylist)
            {
                // Avn: if keyboard focus is in toc panel, assign node to section as PlaySelection
                // command in TOC panel plays a section
                // if (mProjectPanel.TOCPanel.ContainsFocus) node = mCurrentPlayingSection;
                StopInternal();
            }
            if (CanPlay)
            {
                mPlayingFrom = mView.Selection;
                LocalPlaylist = new Playlist(mPlayer, node);
                mCurrentPlaylist = mLocalPlaylist;
                mCurrentPlaylist.CurrentPhrase = mLocalPlaylist.FirstPhrase;
                // Avn: condition added on 13 may 2007
                if (mCurrentPlaylist.PhraseList.Count > 1) mIsSerialPlaying = true;
                mCurrentPlaylist.Play();
                mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
            else if (CanResume)
            {
                // Avn: condition added on 13 may 2007
                if (mCurrentPlaylist.PhraseList.Count > 1) mIsSerialPlaying = true;
                mCurrentPlaylist.Resume();
                mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
            }
            else if (mCurrentPlaylist != mMasterPlaylist)
            {
                Stop();
                // Avn: following line replaced by next line as this function also work in toc panel
                //mProjectPanel.CurrentSelection = new NodeSelection(node, mProjectPanel.StripManager);
                node = mView.Selection.Node;
                if (mCurrentPlaylist.PhraseList.Count > 1) mIsSerialPlaying = true;
                Play(node);
            }
        }


        // Find the phrase to play from from the selected one in the project panel.
        private PhraseNode InitialPhrase
        {
            get
            {
                if (mView != null)
                {
                    if (mView.Selection != null && mView.Selection.Node != null && mView.Selection.Node.Used)
                    {
                        return mView.Selection.Node.FirstUsedPhrase;
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
        /// Pause playback.
        /// </summary>
        public void Pause()
        {
            if (Enabled)
            {
                mCurrentPlaylist.Pause();
                mIsSerialPlaying = false;
            }
        }


        /// <summary>
        /// Start listening/recording.
        /// </summary>
        public void Record()
        {
            if (CanRecord)
            {
                ObiNode selected = mView.Selection.Node;
                SectionNode section;  // section in which we are recording
                int index;            // index from which we add new phrases in the aforementioned section
                bool IsSectionCreated = false; // Flag to indicate creation of a section as to undo it if nothing is recorded.
                if (selected == null)
                {
                    // nothing selected: append a new section and start from 0
                    section = null; // mProjectPanel.Project.CreateSiblingSectionNode(null);
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
                    section = ((PhraseNode)selected).ParentAs<SectionNode>();
                    index = ((PhraseNode)selected).Index;
                }
                Settings settings = ((ObiForm)ParentForm).Settings;
                mRecordingSession = new RecordingSession(mView.Presentation, mRecorder,
                    settings.AudioChannels, settings.SampleRate, settings.BitDepth);
                // the following closures handle the various events sent during the recording session
                mRecordingSession.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        // mProjectPanel.Project.StartRecordingPhrase(_e, section, index + _e.PhraseIndex);
                    }
                );
                mRecordingSession.ContinuingPhrase += new Events.Audio.Recorder.ContinuingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        // mProjectPanel.Project.RecordingPhraseUpdate(_e, section, index + _e.PhraseIndex);
                    }
                );
                mRecordingSession.FinishingPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        // mProjectPanel.Project.RecordingPhraseUpdate(_e, section, index + _e.PhraseIndex);
                        mMasterPlaylist.UpdateTimeFrom(section.PhraseChild(index + _e.PhraseIndex).PreviousPhraseInProject);
                    }
                );
                mRecordingSession.FinishingPage += new Events.Audio.Recorder.FinishingPageHandler(
                    delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        // PhraseNode _node = section.PhraseChild(index + _e.PhraseIndex);
                        // mProjectPanel.Project.DidSetPageNumberOnPhrase(_node);
                    }
                );

                if (mRecordModeBox.SelectedIndex == 0) //recording using the dialog
                {
                    new Dialogs.TransportRecord(mRecordingSession , mVuMeter).ShowDialog();
                    // delete newly created section if nothing is recorded.
                    // if (mRecordingSession.RecordedAudio.Count == 0 && IsSectionCreated)
                    //    this.mProjectPanel.ParentObiForm.UndoLast();
                    for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
                    {
                        //mProjectPanel.StripManager.UpdateAudioForPhrase(section.PhraseChild(index + i), m_RecordingSession.RecordedAudio[i]);
                    }
                }
                else //recording using the transportbar buttons
                {
                    if (mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
                    {
                        // mProjectPanel.StripManager.QuickSplitBlock();
                        mCurrentPlaylist.Stop();
                        mIsSerialPlaying = false;
                        index++; //for "punch in", we want to record between the parts of the split
                    }
                    if (mRecordModeBox.SelectedIndex == 2 && section.PhraseChildCount>0) //we are recording in destructive mode
                    {
                        PhraseNode removeableNode = section.PhraseChild(index);
                        while (removeableNode != null)
                        {
                            // this.ProjectPanel.Project.DeletePhraseNode(removeableNode);
                            removeableNode = (index<section.PhraseChildCount)?section.PhraseChild(index):null;
                        }
                        // this.ProjectPanel.CurrentSelection = null;
                    }
                    mDidCreateSectionForRecording = IsSectionCreated;
                    mRecordingToSection = section;
                    mRecordingStartIndex = index;
                    inlineRecordingSession = mRecordingSession ;
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


        /// <summary>
        /// The stop button. Stopping twice deselects all.
        /// </summary>
        public void Stop()
        {
            if (IsInlineRecording)
            {
                inlineRecordingSession.Stop();
                //if(mDidCreateSectionForRecording && inlineRecordingSession.RecordedAudio.Count == 0)
                //    this.mProjectPanel.ParentObiForm.UndoLast();

                for (int i = 0; i < inlineRecordingSession.RecordedAudio.Count; ++i)
                {
                    // mProjectPanel.StripManager.UpdateAudioForPhrase(mRecordingToSection.PhraseChild(mRecordingStartIndex + i), inlineRecordingSession.RecordedAudio[i]);
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
                    mView.Selection = null;
                }
                else
                {
                    mCurrentPlaylist.Stop();
                    mView.Selection = mPlayingFrom;
                }
                mPlayingFrom = null;
                mIsSerialPlaying = false;
            }
        }


        /// <summary>
        /// Play faster.
        /// </summary>
        public void FastForward()
        {
            if (Enabled && !IsInlineRecording)
            {
                mIsSerialPlaying = true;
                mCurrentPlaylist.FastForward();
            }
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
                mIsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToNextPhrase();
                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing) mIsSerialPlaying = false;
            }
        }

        /// <summary>
        /// Move to the next section (i.e. the first phrase of the next section)
        /// </summary>
        public void NextSection()
        {
            if (!IsInlineRecording)
            {
                mIsSerialPlaying = true;
                if (Enabled) mCurrentPlaylist.NavigateToNextSection();
                if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing) mIsSerialPlaying = false;
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
            mIsSerialPlaying = false;
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

        private void mRecordModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void FastPlayRateStepUp()
        {
            if (ComboFastPlateRate.SelectedIndex < ComboFastPlateRate.Items.Count - 1)
            {
                ComboFastPlateRate.SelectedIndex = ComboFastPlateRate.SelectedIndex + 1;
                mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(ComboFastPlateRate.SelectedItem.ToString());
            }
        }

        public void FastPlayRateStepDown()
        {
            if (ComboFastPlateRate.SelectedIndex > 0)
            {
                ComboFastPlateRate.SelectedIndex = ComboFastPlateRate.SelectedIndex - 1;
                mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(ComboFastPlateRate.SelectedItem.ToString());
            }
        }

        public void FastPlayRateNormalise()
        {
            ComboFastPlateRate.SelectedIndex = 0;
            mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(ComboFastPlateRate.SelectedItem.ToString());
        }

        private void ComboFastPlateRate_SelectionChangeCommitted(object sender, EventArgs e)
        {
            mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(ComboFastPlateRate.SelectedItem.ToString());
        }

        public void FastPlayNormaliseWithLapseBack()
        {
            mCurrentPlaylist.FastPlayNormaliseWithLapseBack(1500);
            ComboFastPlateRate.SelectedIndex = 0;
        }
    }
}
