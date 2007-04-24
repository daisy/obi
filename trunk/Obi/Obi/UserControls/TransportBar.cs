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
        private ProjectPanel mProjectPanel;  // project panel to which the transport bar belongs
        private Playlist mMasterPlaylist;    // master playlist (all phrases in the project)
        private Playlist mLocalPlaylist;     // local playlist (only selected; may be null)
        private Playlist mCurrentPlaylist;   // playlist currently playing
        private NodeSelection mPlayingFrom;  // selection before playback started
        private bool m_IsSerialPlaying = false ; // A  non fluctuating flag to be set till playing of assets in serial is continued

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
            mLocalPlaylist = null;
            mMasterPlaylist = new Playlist(Audio.AudioPlayer.Instance);
            SetPlaylistEvents(mMasterPlaylist);
            mCurrentPlaylist = mMasterPlaylist;
            mDisplayBox.SelectedIndex = ElapsedTotal;
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
            mProjectPanel = null;
        }


        #region selection

        public ObiNode CurrentSelectedNode
        {
            get { throw new Exception("Please don't ask me for a selection, I don't know anything about that stuff."); }
            set
            {
                if (IsSelectionRelevant(value) && value is PhraseNode) mCurrentPlaylist.CurrentPhrase = (PhraseNode)value;
            }
        }

        public bool IsSelectionRelevant(ObiNode node)
        {
            return ((mCurrentPlaylist.State == Audio.AudioPlayerState.Playing ||
                    mCurrentPlaylist.State == Audio.AudioPlayerState.Paused));
        }

        public bool CanSelectPhrase(ObiNode node)
        {
            return mCurrentPlaylist != null && mCurrentPlaylist.ContainsPhrase(node as PhraseNode);
        }

        #endregion


        #region properties

        public bool CanPlay
        {
            get { return Enabled && mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped; }
        }

        public bool CanRecord
        {
            get { return Enabled && mCurrentPlaylist.State == Audio.AudioPlayerState.Stopped; }
        }

        public bool CanResume
        {
            get { return Enabled && mCurrentPlaylist.State == Audio.AudioPlayerState.Paused; }
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
            mProjectPanel.CurrentSelection = mPlayingFrom;
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
            m_IsSerialPlaying = true;
            if (Enabled) mCurrentPlaylist.NavigateToPreviousSection();

            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                m_IsSerialPlaying = false;
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
            if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing)
                m_IsSerialPlaying = true;
            
            if (Enabled) mCurrentPlaylist.NavigateToPreviousPhrase();

            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                m_IsSerialPlaying = false;
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
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing) Pause();
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    m_IsSerialPlaying = true;
                    mCurrentPlaylist.Rewind();
                }
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
            if (CanPlay)
            {
                mPlayingFrom = mProjectPanel.CurrentSelection;
                mCurrentPlaylist = mMasterPlaylist;
                mCurrentPlaylist.CurrentPhrase = InitialPhrase;
                if (mCurrentPlaylist.CurrentPhrase != null)
                {
                    mVUMeterPanel.Enable = true;
                    mVUMeterPanel.PlayListObj = mCurrentPlaylist;
                    m_IsSerialPlaying = true;
                    mCurrentPlaylist.Play();
                }
            }
            else if (CanResume)
            {
                m_IsSerialPlaying = true;
                mCurrentPlaylist.Resume();
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void Play(ObiNode node)
        {
            if (CanPlay)
            {
                mPlayingFrom = mProjectPanel.CurrentSelection;
                LocalPlaylist = new Playlist(Audio.AudioPlayer.Instance, node);
                mCurrentPlaylist = mLocalPlaylist;
                mCurrentPlaylist.CurrentPhrase = mLocalPlaylist.FirstPhrase;
                mVUMeterPanel.Enable = true;
                mVUMeterPanel.PlayListObj = mCurrentPlaylist;

                m_IsSerialPlaying = true;
                mCurrentPlaylist.Play();
            }
            else if (CanResume)
            {
                m_IsSerialPlaying = true;
                mCurrentPlaylist.Resume();
            }
            else if (mCurrentPlaylist != mMasterPlaylist)
            {
                Stop();
                mProjectPanel.CurrentSelection = new NodeSelection(node, mProjectPanel.StripManager);
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
                RecordingSession session = new RecordingSession(mProjectPanel.Project, Audio.AudioRecorder.Instance,
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
                new Dialogs.TransportRecord(session).ShowDialog();

                // delete newly created section if nothing is recorded.
                if (session.RecordedAssets.Count == 0 && IsSectionCreated)
                    this.mProjectPanel.ParentObiForm.UndoLast();

                for (int i = 0; i < session.RecordedAssets.Count; ++i)
                {
                    mProjectPanel.StripManager.UpdateAssetForPhrase(section.PhraseChild(index + i), session.RecordedAssets[i]);                   
                }
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
            if (Enabled)
            {
                // Stopping again deselects everything
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    mProjectPanel.CurrentSelection = null;
                }
                else
                {
                    mCurrentPlaylist.Stop();
                    mProjectPanel.CurrentSelection = mPlayingFrom;
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
            if (Enabled)
            {
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing) Pause();
                if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    m_IsSerialPlaying = true;
                    mCurrentPlaylist.FastForward();
                }
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
            m_IsSerialPlaying = true;
            if (Enabled) mCurrentPlaylist.NavigateToNextPhrase();

            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
                m_IsSerialPlaying = false;

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
            m_IsSerialPlaying = true;
            if (Enabled) mCurrentPlaylist.NavigateToNextSection();

            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Playing)
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
                        Assets.MediaAsset.FormatTime_hh_mm_ss(mCurrentPlaylist.CurrentTimeInAsset) :
                    mDisplayBox.SelectedIndex == ElapsedTotal ?
                        Assets.MediaAsset.FormatTime_hh_mm_ss(mCurrentPlaylist.CurrentTime) :
                    mDisplayBox.SelectedIndex == Remain ?
                        Assets.MediaAsset.FormatTime_hh_mm_ss(mCurrentPlaylist.RemainingTimeInAsset) :
                        Assets.MediaAsset.FormatTime_hh_mm_ss(mCurrentPlaylist.RemainingTime);
            }
            else
            {
                mTimeDisplayBox.Text = Assets.MediaAsset.FormatTime_hh_mm_ss(0.0);
            }
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
                mProjectPanel.Project.AddedPhraseNode += new Obi.Events.PhraseNodeHandler(Project_AddedPhraseNode);
                mProjectPanel.Project.DeletedPhraseNode += new Obi.Events.PhraseNodeHandler(Project_DeletedPhraseNode);
                mProjectPanel.Project.ToggledNodeUsedState += new Obi.Events.ObiNodeHandler(Project_ToggledNodeUsedState);
                mProjectPanel.Project.MediaSet += new Obi.Events.SetMediaHandler(Project_MediaSet);
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
    }
}
