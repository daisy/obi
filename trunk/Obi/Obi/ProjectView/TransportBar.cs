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
        private State mState;                        // transport bar state (composite of player/recorder states)
        
        private bool mAllowOverwrite;                // if true, recording can overwrite data
        private bool mPlayIfNoSelection;             // play all when no selection if true; play nothing otherwise

        private int mPreviewDuration;                // duration of preview playback in milliseconds (from the settings)
        private PhraseNode mResumerecordingPhrase;   // last phrase recorded (?)

        private SectionNode mCurrentPlayingSection;  // holds section currently being played for highlighting it in TOC view while playing
        private bool mIsSelectionMarked = false;     // this should probably go I think


        // Constants from the display combo box
        private static readonly int ELAPSED_INDEX = 0;
        private static readonly int ELAPSED_TOTAL_INDEX = 1;
        private static readonly int REMAIN_INDEX = 2;


        // Pass the state change and playback rate change events from the playlist
        public event Events.Audio.Player.StateChangedHandler StateChanged;
        public event EventHandler PlaybackRateChanged;


        // States of the transport bar:
        // * Monitoring: recording is paused;
        // * Paused: playback is paused;
        // * Playing: playback is in progress;
        // * Recording: recording is in progress;
        // * Stopped: stopped.
        public enum State { Monitoring, Paused, Playing, Recording, Stopped };

        /// <summary>
        /// Initialize the transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            mView = null;
            InitAudio();
            InitPlaylists();
            mDisplayBox.SelectedIndex = 0;
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
            mFastPlayRateCombobox.SelectedIndex = 0;
            mAllowOverwrite = true;
            mPlayIfNoSelection = true;
            mState = State.Stopped;
        }


        /// <summary>
        /// Flag to enable/disable overwrite during recording.
        /// Set from an Obi preference.
        /// </summary>
        public bool AllowOverwrite { set { mAllowOverwrite = value; } }

        /// <summary>
        /// Get the audio player used by the transport bar.
        /// </summary>
        public Audio.AudioPlayer AudioPlayer { get { return mPlayer; } }

        public bool CanFastForward { get { return CanPlay || (Enabled && mState == State.Playing); } }
        public bool CanMarkCustomClass { get { return Enabled && mState == State.Recording; } }
        public bool CanNavigateNextPage { get { return true; } }
        public bool CanNavigateNextPhrase { get { return true; } }
        public bool CanNavigateNextSection { get { return true; } }
        public bool CanNavigatePrevPage { get { return true; } }
        public bool CanNavigatePrevPhrase { get { return true; } }
        public bool CanNavigatePrevSection { get { return true; } }
        public bool CanPause { get { return Enabled && (mState == State.Playing || mState == State.Recording); } }
        public bool CanPlay { get { return Enabled && mState == State.Stopped; } }
        public bool CanRecord { get { return Enabled && mState == State.Stopped; } }
        public bool CanResumePlayback { get { return Enabled && mState == State.Paused; } }
        public bool CanResumeRecording { get { return Enabled && mState == State.Monitoring; } }
        public bool CanRewind { get { return CanPlay || (Enabled && mState == State.Playing); } } 
        public bool CanStop { get { return Enabled && (mState != State.Stopped || mView.Selection != null); } }

        /// <summary>
        /// Get the current playlist.
        /// </summary>
        public Playlist CurrentPlaylist { get { return mCurrentPlaylist; } }

        /// <summary>
        /// Get the current composite state of the transport bar.
        /// </summary>
        public State CurrentState { get { return mState; } }

        /// <summary>
        /// The transport bar as a whole can be enabled/disabled when necessary.
        /// Disabling the transport bar will also stop playback.
        /// </summary>
        public new bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                if (base.Enabled && !value && IsActive) Stop();
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Enable/disable tooltips.
        /// </summary>
        public bool EnableTooltips { set { mTransportBarTooltip.Active = value; } }

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
        /// Get the master playlist (automatically maintained.)
        /// </summary>
        public Playlist MasterPlaylist { get { return mMasterPlaylist; } }

        /// <summary>
        /// The presentation in the project view has changed, so update playlists and event handlers accordingly.
        /// </summary>
        public void NewPresentation()
        {
            mMasterPlaylist.Presentation = mView.Presentation;
            mView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
            UpdateButtons();
        }

        /// <summary>
        /// If true, play all when there is no selection; otherwise, play nothing.
        /// </summary>
        public bool PlayIfNoSelection { set { mPlayIfNoSelection = value; } }

        /// <summary>
        /// Set preview duration.
        /// </summary>
        public int PreviewDuration { set { mPreviewDuration = value; } }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
                UpdateButtons();
            }
        }

        /// <summary>
        /// Get the recorder associated with the transport bar.
        /// </summary>
        public Audio.AudioRecorder Recorder { get { return mRecorder; } }

        /// <summary>
        /// Get the recording session associated with the transport bar.
        /// </summary>
        public RecordingSession Recordingsession { get { return mRecordingSession; } }

        /// <summary>
        /// Get the VU meter associated with the transport bar.
        /// </summary>
        public Audio.VuMeter VuMeter { get { return mVuMeter; } }


        // Initialize audio (player, recorder, VU meter.)
        private void InitAudio()
        {
            mPlayer = new Audio.AudioPlayer();
            mRecorder = new Obi.Audio.AudioRecorder();
            mVuMeter = new Obi.Audio.VuMeter(mPlayer, mRecorder);
            mVUMeterPanel.VuMeter = mVuMeter;
        }

        // Initialize playlists
        private void InitPlaylists()
        {
            mMasterPlaylist = new Playlist(mPlayer);
            mLocalPlaylist = null;
            mCurrentPlaylist = mMasterPlaylist;
            SetPlaylistEvents(mMasterPlaylist);
        }

        // Move the audio cursor to the phrase currently playing.
        private void Playlist_MovedToPhrase(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mView.PlaybackPhrase = e.Node;
            UpdateTimeDisplay();
        }

        // Update the transport bar according to the player state.
        private void Playlist_PlayerStateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            mState = mPlayer.State == Obi.Audio.AudioPlayerState.Paused ? State.Paused :
                mPlayer.State == Obi.Audio.AudioPlayerState.Playing ? State.Playing : State.Stopped;
            if (mState == State.Playing || mState == State.Recording)
            {
                mDisplayTimer.Start();
            }
            else if (mState == State.Stopped)
            {
                mDisplayTimer.Stop();
            }
            if (StateChanged != null) StateChanged(this, e);
            UpdateTimeDisplay();
            UpdateButtons();
        }

        // Simply pass the playback rate change event.
        private void Playlist_PlaybackRateChanged(object sender, EventArgs e)
        {
            if (PlaybackRateChanged != null) PlaybackRateChanged(sender, e);
        }

        // Update the transport bar once the player has stopped.
        private void Playlist_PlayerStopped(object sender, EventArgs e) { mView.PlaybackPhrase = null; }

        // Adapt to changes in the presentation.
        // At the moment, simply stop.
        private void Presentation_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (mState != State.Stopped) Stop();
        }

        // Adapt to changes in used status.
        // At the moment, simply stop.
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (mState != State.Stopped) Stop();
        }

        // Initialize events for a new playlist.
        private void SetPlaylistEvents(Playlist playlist)
        {
            playlist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Playlist_MovedToPhrase);
            playlist.StateChanged += new Events.Audio.Player.StateChangedHandler(Playlist_PlayerStateChanged);
            playlist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Playlist_PlayerStopped);
            playlist.PlaybackRateChanged += new Playlist.PlaybackRateChangedHandler(Playlist_PlaybackRateChanged);
        }

        // Update visibility and enabledness of buttons depending on the state of the recorder
        private void UpdateButtons()
        {
            mPrevSectionButton.Enabled = CanNavigatePrevSection;
            mPreviousPageButton.Enabled = CanNavigatePrevPage;
            mPrevPhraseButton.Enabled = CanNavigatePrevPhrase;
            mRewindButton.Enabled = CanRewind;
            mPauseButton.Visible = CanPause;
            mPlayButton.Visible = !mPauseButton.Visible;
            mPlayButton.Enabled = CanPlay || CanResumePlayback;
            mRecordButton.Enabled = CanRecord || CanResumeRecording;
            mRecordButton.AccessibleName = Localizer.Message(
                mRecorder.State == Obi.Audio.AudioRecorderState.Listening ? "start_recording" : "start_monitoring"
            );
            mStopButton.Enabled = CanStop;
            mFastForwardButton.Enabled = CanFastForward;
            mNextPhrase.Enabled = CanNavigateNextPhrase;
            mNextPageButton.Enabled = CanNavigateNextPage;
            mNextSectionButton.Enabled = CanNavigateNextSection;
            mCustomClassMarkButton.Enabled = CanMarkCustomClass;
        }

        /// <summary>
        /// Update the time display to show current time. Depends on the what kind of timing is selected.
        /// </summary>
        private void UpdateTimeDisplay()
        {
            if (mState == State.Monitoring)
            {
                mTimeDisplayBox.Text = "--:--:--";
            }
            else if (mState == State.Recording)
            {
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(mRecordingSession.AudioRecorder.TimeOfAsset);
            }
            else if (mState == State.Stopped)
            {
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(0.0);
            }
            else
            {
                mDisplayTime = mCurrentPlaylist.CurrentTimeInAsset;
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(
                    mDisplayBox.SelectedIndex == ELAPSED_INDEX ?
                        mCurrentPlaylist.CurrentTimeInAsset :
                    mDisplayBox.SelectedIndex == ELAPSED_TOTAL_INDEX ?
                        mCurrentPlaylist.CurrentTime :
                    mDisplayBox.SelectedIndex == REMAIN_INDEX ?
                        mCurrentPlaylist.RemainingTimeInAsset :
                        mCurrentPlaylist.RemainingTime);
            }
        }


        // Play/Resume playback

        private void mPlayButton_Click(object sender, EventArgs e) { PlayOrResume(); }

        /// <summary>
        /// Play all in the project. (Used when nothing is selected, or from the play all menu item.)
        /// </summary>
        public void PlayAll()
        {
            mCurrentPlaylist = mMasterPlaylist;
            mCurrentPlaylist.CurrentPhrase = mCurrentPlaylist.FirstPhrase;
            mCurrentPlaylist.Play();
        }

        /// <summary>
        /// All-purpose play function for the play button.
        /// Play or resume if possible, otherwise do nothing.
        /// If there is a selection, play the selection; if there is no selection, play everything
        /// </summary>
        public void PlayOrResume()
        {
            if (CanPlay)
            {
                PlayOrResume(mView.Selection == null ? null : mView.Selection.Node);
            }
            else if (CanResumePlayback)
            {
                mCurrentPlaylist.Resume();
            }
        }

        /// <summary>
        /// Play a single node (phrase or section), or everything if the node is null
        /// (and the mPlayIfNoSelection flag is set.)
        /// </summary>
        public void PlayOrResume(ObiNode node)
        {
            if (node == null && mPlayIfNoSelection)
            {
                PlayAll();
            }
            else if (node != null)
            {
                // we need the selection to tell between a strip and a section
                // maybe a deep flag would be better
                mLocalPlaylist = new Playlist(mPlayer, mView.Selection);
                SetPlaylistEvents(mLocalPlaylist);
                mCurrentPlaylist = mLocalPlaylist;
                if (mView.Selection is AudioSelection
                    && (!((AudioSelection)mView.Selection).AudioRange.HasCursor || mIsSelectionMarked)
                    && ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime > ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime)
                {
                    mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime, ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime);
                }
                else if (mView.Selection is AudioSelection
                    && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
                {
                    mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.CursorTime);
                }
                else
                {
                    mCurrentPlaylist.Play();
                }
            }
        }


        // Pause

        private void mPauseButton_Click(object sender, EventArgs e) { Pause(); }

        /// <summary>
        /// Pause playback or recording
        /// </summary>
        public void Pause()
        {
            if (CanPause)
            {
                mDisplayTimer.Stop();
                if (mRecorder.State == Obi.Audio.AudioRecorderState.Recording)
                {
                    PauseRecording();
                }
                else if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing)
                {
                    mCurrentPlaylist.Pause();
                }
                UpdateButtons();
            }
        }

        // Pause recording
        private void PauseRecording()
        {
            mRecordingSession.Stop();
            for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
            {
                mView.Presentation.UpdateAudioForPhrase(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i),
                    mRecordingSession.RecordedAudio[i]);
            }
            mResumerecordingPhrase = (PhraseNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1);
            mRecordingSession = null;
            UpdateTimeDisplay();
        }


        // Stop

        private void mStopButton_Click(object sender, EventArgs e) { Stop(); }

        /// <summary>
        /// The stop button. Stopping twice deselects all.
        /// </summary>
        public void Stop()
        {
            if (Enabled)
            {
                if (IsRecorderActive)
                {
                    StopRecording();
                }
                else
                {
                    // Stopping again deselects everything
                    if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                    {
                        mView.Selection = null;
                    }
                    else
                    {
                        mCurrentPlaylist.Stop();
                        mView.PlaybackPhrase = null;
                    }
                }
            }
        }


        // Record

        // Navigation

        private void mPrevSectionButton_Click(object sender, EventArgs e) { PrevSection(); }
        private void mPreviousPageButton_Click(object sender, EventArgs e) { PrevPage(); }
        private void mPrevPhraseButton_Click(object sender, EventArgs e) { PrevPhrase(); }
        private void mNextPhrase_Click(object sender, EventArgs e) { NextPhrase(); }
        private void mNextPageButton_Click(object sender, EventArgs e) { NextPage(); }
        private void mNextSectionButton_Click(object sender, EventArgs e) { NextSection(); }

        /// <summary>
        /// Move to or play the previous page.
        /// </summary>
        public void PrevPage()
        {
            if (Enabled && mRecordingSession == null)
            {
                mCurrentPlaylist.NavigateToPreviousPage();
            }
        }

        /// <summary>
        /// Move to or play the previous phrase.
        /// </summary>
        public void PrevPhrase()
        {
            if (Enabled && mRecordingSession == null)
            {
                mCurrentPlaylist.NavigateToPreviousPhrase();
            }
        }

        /// <summary>
        /// Move to the previous section (i.e. first phrase of the previous section.)
        /// </summary>
        public void PrevSection()
        {
            if (Enabled && mRecordingSession == null)
            {
                mCurrentPlaylist.NavigateToPreviousSection();
            }
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
        public void NextPage()
        {
            if (Enabled)
            {
                if (mState == State.Recording)
                {
                    mRecordingSession.MarkPage();
                }
                else if (mState != State.Monitoring)
                {
                    mCurrentPlaylist.NavigateToNextPage();
                }
            }
        }

        /// <summary>
        /// Go to the next phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (Enabled)
            {
                if (mState == State.Recording)
                {
                    mRecordingSession.NextPhrase();
                }
                else if (mState != State.Monitoring)
                {
                    mCurrentPlaylist.NavigateToNextPhrase();
                }
            }
        }

        /// <summary>
        /// Move to the next section (i.e. the first phrase of the next section)
        /// </summary>
        public void NextSection()
        {
            if (Enabled)
            {
                if (mState == State.Recording)
                {
                    // mark section
                    PauseRecording();
                    mView.AddSection();
                    PrepareForRecording(true, null);
                }
                else
                {
                    mCurrentPlaylist.NavigateToNextSection();
                }
            }
        }

















        #region buttons





        private void mRewindButton_Click(object sender, EventArgs e) { Rewind(); }

        private void mRecordButton_Click(object sender, EventArgs e) { Record(); }
                
        private void mFastForwardButton_Click(object sender, EventArgs e) { FastForward(); }



        /// <summary>
        /// Rewind (i.e. play faster backward)
        /// </summary>
        public void Rewind()
        {
            if (Enabled && mRecordingSession == null)
            {
                    if (mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped)
                        PlayOrResume();
                    mCurrentPlaylist.Rewind();
                }
                    }

        /// <summary>
        /// checks if play selection should be initialised from PlayAll function
                /// </summary>
        private bool IsPlaySelection
        {
            get
            {
                if (mView.Selection != null // if nothing is selected
&&
((mView.Selection.Control is TOCView) || (mView.Selection.Control is StripsView && ((StripsView)mView.Selection.Control).IsStripCursorSelected == false)) // if keyboard is in TOC view or if strip cursor is selected
                                        &&
(mView.Selection.Node is EmptyNode 
                    || mView.Selection.Node is SectionNode
                    || !(mView.Selection is AudioSelection))) // if time cursor is not selected
                    return true ;
                else
                    return false;
            }
        }


        /// <summary>
        /// Play the master playlist, starting from the selected phrase, or the first phrase of
        /// the selected section or the beginning of the project.
        /// </summary>
        public void __OLD__Play()
        {
                            //Avn: for instantly playing MasterPlaylist, check if current playlist is local
                // and stop if this LocalPlaylist not in stop state
                //if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped && mCurrentPlaylist == mLocalPlaylist) StopInternal();
                if (CanPlay)
                {
                    //if (IsPlaySelection)
                    //{
                        //Play(mView.Selection.Node);
                    //}
                    //else
                    //{
                        PlayMasterPlaylist();
                    //}
                }
                else if (CanResumePlayback)
                {

                    if (mView.Selection is AudioSelection)
                        mCurrentPlaylist.CurrentTimeInAsset = ((AudioSelection)mView.Selection).AudioRange.CursorTime;

                    mCurrentPlaylist.Resume();
                    mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
                    }

        private void PlayMasterPlaylist()
        {
            if (CanPlay)
            {
                mCurrentPlaylist = mMasterPlaylist;
                mCurrentPlaylist.CurrentPhrase = InitialPhrase;
                if (mCurrentPlaylist.CurrentPhrase != null)
                {
                    if (mView.Selection is AudioSelection
                        && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
                        mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.CursorTime);
                    else
                        mCurrentPlaylist.Play();

                    mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
            }
        }

        /// <summary>
        /// Play a single node (phrase or section).
        /// </summary>
        public void __OLD__Play(ObiNode node)
        {
            // Avn: For instantly playing LocalPlaylist check if current playlist is MasterPlaylist
            // and if this MasterPlaylist is not in stopped state, stop it.
            if (mCurrentPlaylist.State != Obi.Audio.AudioPlayerState.Stopped
                && mCurrentPlaylist == mMasterPlaylist)
            {
                // Avn: if keyboard focus is in toc panel, assign node to section as PlaySelection
                // command in TOC panel plays a section
                // if (mProjectPanel.TOCPanel.ContainsFocus) node = mCurrentPlayingSection;
                mCurrentPlaylist.Stop();
            }
                if (CanPlay)
                {
                    //mPlayingFrom = mView.Selection;
                    LocalPlaylist = new Playlist(mPlayer, mView.Selection);
                    mCurrentPlaylist = mLocalPlaylist;
                    mCurrentPlaylist.CurrentPhrase = InitialPhrase;
                    // Avn: condition added on 13 may 2007
                    //if (mCurrentPlaylist.PhraseList.Count > 1) mIsSerialPlaying = true;
                    
                    if (mView.Selection is AudioSelection
                        && ( !((AudioSelection)mView.Selection).AudioRange.HasCursor || mIsSelectionMarked )
                        && ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime > ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime)
                        mCurrentPlaylist.PreviewSelectedFragment(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime, ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime);
                    else if (mView.Selection is AudioSelection
                        && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
                        mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.CursorTime);
                    else
                        mCurrentPlaylist.Play();

                    mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
                else if (CanResumePlayback)
                {
                    // Avn: condition added on 13 may 2007
                    //if (mCurrentPlaylist.PhraseList.Count > 1) mIsSerialPlaying = true;

                    if (mView.Selection is AudioSelection)
                        mCurrentPlaylist.CurrentTimeInAsset = ((AudioSelection)mView.Selection).AudioRange.CursorTime;

                    mCurrentPlaylist.Resume();
                    mCurrentPlayingSection = mCurrentPlaylist.CurrentSection;
                }
                else if (mCurrentPlaylist != mMasterPlaylist)
                {
                    Stop();
                    // Avn: following line replaced by next line as this function also work in toc panel
                    //mProjectPanel.CurrentSelection = new NodeSelection(node, mProjectPanel.StripManager);
                    node = mView.Selection.Node;
                    PlayOrResume(node);
                }
                    }


        // Find the phrase to play from from the selected one in the project panel.
        // When there is a selection, this is the first phrase of the selection
        // (or after the selection in the case of the strip cursor selection);
        // when there is none, this is the first phrase of the master playlist.
        private PhraseNode InitialPhrase
        {
            get
            {
                if (mView == null) return null;
                if (mView.Selection is StripCursorSelection)
                {
                    // TODO this doesn't handle unused nodes/end of the book well.
                    return FirstPhraseNodeAfter((SectionNode)mView.Selection.Node, ((StripCursorSelection)mView.Selection).Index);
                }
                else if (mView.Selection != null && mView.Selection.Node.Used)
                {
                    return mView.Selection.Node.FirstUsedPhrase;
                }
                else
                {
                    return mMasterPlaylist.FirstPhrase;
                }
            }
        }

        // Get the first phrase node after the given index in the given section.
        // This can be a phrase of the section, or the first used phrase after the following section.
        private PhraseNode FirstPhraseNodeAfter(SectionNode section, int index)
        {
            ObiNode from = index < section.PhraseChildCount ? (ObiNode)section.PhraseChild(index) :
                section.PhraseChildCount > 0 ? section.PhraseChild(section.PhraseChildCount - 1).FollowingNode :
                section.SectionChildCount > 0 ? section.SectionChild(0).FollowingNode :
                section.FollowingNode;
            while (from != null && !(from is PhraseNode)) from = from.FollowingNode;
            return from as PhraseNode;
        }

        int mRecordingInitPhraseIndex ;
        SectionNode mRecordingSection; // Section in which we are recording

        private void DisablePlaybackButtonsForRecording ()
        {
            mPlayButton.Enabled = false;
            mPrevPhraseButton.Enabled = false;
            mPrevSectionButton.Enabled = false;
            mPreviousPageButton.Enabled = false;
            mFastForwardButton.Enabled = false;
            mRewindButton.Enabled = false;
            mFastPlayRateCombobox.Enabled = false;

        }



        public void UpdateInlineRecordingState()
        {
            mPrevPhraseButton.Enabled = this.Enabled;
            mPrevSectionButton.Enabled = this.Enabled;
            mRewindButton.Enabled = this.Enabled;
            mFastForwardButton.Enabled = this.Enabled;
            mPlayButton.Enabled = CanPlay;
            mNextPhrase.Enabled = this.Enabled;
            mNextSectionButton.Enabled = this.Enabled;
            mPauseButton.Enabled = this.Enabled;
        }



        /// <summary>
        /// Play faster.
        /// </summary>
        public void FastForward()
        {
            if (Enabled && mRecordingSession == null )
            {
                if ( mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped )
                    PlayOrResume();

                mCurrentPlaylist.FastForward();
            }
        }



        #endregion

        /// <summary>
        /// Periodically update the time display.
        /// </summary>
        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
            if (mPlayer.State == Obi.Audio.AudioPlayerState.Playing) mView.UpdateCursorPosition(mCurrentPlaylist.CurrentTimeInAsset);
        }

        private double mDisplayTime;

        /// <summary>
        /// Update the time display immediatly when the display mode changes.
        /// </summary>
        private void mDisplayBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (mRecordingSession != null)
                mDisplayBox.SelectedIndex = mDisplayBox.Items.Count - 1;
                            else if ( mDisplayBox.SelectedIndex == mDisplayBox.Items.Count - 1 )
            {
                mDisplayBox.SelectedIndex = mDisplayBox.Items.Count - 2;
                                            }

            UpdateTimeDisplay();
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
        }

        public void FocusTimeDisplay()
        {
            mTimeDisplayBox.Focus();
        }


        public bool FastPlayRateStepUp()
        {
            if (mFastPlayRateCombobox.SelectedIndex < mFastPlayRateCombobox.Items.Count - 1)
            {
                mFastPlayRateCombobox.SelectedIndex = mFastPlayRateCombobox.SelectedIndex + 1;
                mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
                return true;
            }
            return false;
        }

        public bool  FastPlayRateStepDown()
        {
            if (mFastPlayRateCombobox.SelectedIndex > 0)
            {
                mFastPlayRateCombobox.SelectedIndex = mFastPlayRateCombobox.SelectedIndex - 1;
                mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
                return true;
            }
            return false;
        }

        public bool FastPlayRateNormalise()
        {
            mFastPlayRateCombobox.SelectedIndex = 0;
            mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
            return true;
        }

        private void ComboFastPlateRate_SelectionChangeCommitted(object sender, EventArgs e)
        {
            mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
        }

        public bool FastPlayNormaliseWithLapseBack()
        {
            mCurrentPlaylist.FastPlayNormaliseWithLapseBack(1500);
            mFastPlayRateCombobox.SelectedIndex = 0;
            return true;
        }


        // preview playback functions

        private bool IsInPhraseSelectionMarked
        {
            get
            {
                return mView.Selection != null
                    && mView.Selection is AudioSelection;
                    //&& !((AudioSelection)mView.Selection).AudioRange.HasCursor;
            }
        }
        
        public bool MarkSelectionBeginTime ()
        {
            if (IsInPhraseSelectionMarked)
            {
                ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime = ((AudioSelection)mView.Selection).AudioRange.CursorTime ;
                return true;
            }
            return false;
        }

        public bool MarkSelectionEndTime()
        {
            if ( IsInPhraseSelectionMarked
                                && mCurrentPlaylist.CurrentTimeInAsset > ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime )
            {
                ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime = ((AudioSelection)mView.Selection).AudioRange.CursorTime ;
                mIsSelectionMarked = true;
                                                return true;
            }
            return false;
        }


        /// <summary>
        /// Preview from the current position.
        /// </summary>
        public bool PlayPreviewFromCurrentPosition()
        {
            if (IsInPhraseSelectionMarked)
            {
                mCurrentPlaylist.PreviewFromCurrentPosition(((AudioSelection)mView.Selection).AudioRange.CursorTime, mPreviewDuration);
                return true;
            }
            return false;
        }

        public bool PlayPreviewSelectedFragment()
        {
                                                if ( IsInPhraseSelectionMarked
                                                                    &&    ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime < ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime )
            {
                mCurrentPlaylist.PreviewSelectedFragment(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime, ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Preview up to the current position.
        /// </summary>
        public bool PlayPreviewUptoCurrentPosition()
        {
            if (IsInPhraseSelectionMarked)
            {
                mCurrentPlaylist.PreviewUptoCurrentPosition(((AudioSelection)mView.Selection).AudioRange.CursorTime, mPreviewDuration);
                return true;
            }
            return false;
        }


        #region undoable recording

        /// <summary>
        /// Start listening/recording.
        /// </summary>
        public void Record()
        {

            if (mRecordingSession != null)
            {
                if (mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Listening)
                {
                    mRecordingSession.Stop();
                    StartRecording();
                }
                            }
                            //else if (m_ResumerecordingPhrase != null)
                            else if ( CanResumeRecording)
                            {
                                PrepareForRecording(true, mResumerecordingPhrase);
                                                            }
            else
            {
                                                PrepareForRecording(false , null );

            }
        }


        /// <summary>
        /// Start recording directly without going through listening
                /// </summary>
        public void StartRecordingDirectly()
        {
            if (mRecordingSession == null && mCurrentPlaylist.Audioplayer.State != Obi.Audio.AudioPlayerState.Playing)
            {
                PrepareForRecording(true, null);
            }
        }


        private urakawa.undo.CompositeCommand CreateRecordingCommand()
        {
            urakawa.undo.CompositeCommand command = mView.Presentation.getCommandFactory().createCompositeCommand();
            command.setShortDescription(Localizer.Message("recording_command"));
            return command;
        }

        urakawa.undo.CompositeCommand mRecordingCommand;

        
        private EmptyNode mRecordingEmptyNode = null;

        // Prepare for recording and return the corresponding recording command.
        private void PrepareForRecording(bool startRecording, ObiNode selected)
        {
            if (!CanRecord) return;
            mRecordingCommand = CreateRecordingCommand();
            mRecordingEmptyNode = null;

            if (selected == null)
                selected = mView.SelectedNodeAs<ObiNode>();

            // If nothing is selected, create a new section to record in.
            if (selected == null)
            {
                // create a new section node to record in
                SectionNode section = mView.Presentation.CreateSectionNode();
                Commands.Node.AddNode add = new Commands.Node.AddNode(mView, section, mView.Presentation.RootNode,
                    mView.Presentation.RootNode.SectionChildCount);
                add.UpdateSelection = false;
                mRecordingCommand.append(add);
                selected = section;
            }
            // Now we should always have a selection.
            System.Diagnostics.Debug.Assert(selected != null, "No selection for recording.");
            // TODO: record at the position in the block, or replace the waveform selection
            if (selected is SectionNode)
            {
                mRecordingSection = (SectionNode)selected;
                mRecordingInitPhraseIndex = mRecordingSection.PhraseChildCount;
            }
            else if (selected is PhraseNode)
            {
                mRecordingSection = selected.ParentAs<SectionNode>();

                if (mAllowOverwrite  && IsInPhraseSelectionMarked)
                {
                    if (((AudioSelection)mView.Selection).AudioRange.SelectionEndTime != 0
                        && ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime < ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime)
                    {
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.SplitAudioSelection(mView));
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.Delete(mView, mView.Selection.Node));
                    }
                    else
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.SplitAudio(mView));

                }
                if (mCurrentPlaylist.State == Audio.AudioPlayerState.Paused)
                    mCurrentPlaylist.Stop();

                mRecordingInitPhraseIndex = 1 + selected.Index;
            }
            else if (selected is EmptyNode)
            {
                EmptyNode ENode = (EmptyNode)selected;
                mRecordingSection = ENode.ParentAs<SectionNode>();
                mRecordingInitPhraseIndex = selected.Index;
                mRecordingEmptyNode = ENode;
            }



            Settings settings = mView.ObiForm.Settings;
            mRecordingSession = new RecordingSession(mView.Presentation, mRecorder);
            mRecordingSession.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                {
                    PhraseNode phrase = mView.Presentation.CreatePhraseNode(_e.Audio);
                    if (_e.PhraseIndex > 0)
                    {
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                            mRecordingInitPhraseIndex + _e.PhraseIndex));
                    }
                    else
                    {
                        mRecordingCommand.append(new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                                                        mRecordingInitPhraseIndex + _e.PhraseIndex));
                        if (mRecordingEmptyNode != null)
                        {
                            phrase.CopyKind(mRecordingEmptyNode);
                            phrase.Used = mRecordingEmptyNode.Used;
                            mRecordingCommand.append(new Commands.Node.Delete(mView, mRecordingEmptyNode));
                            mRecordingEmptyNode = null;
                        }
                        mView.Presentation.getUndoRedoManager().execute(mRecordingCommand);
                    }
                });
            mRecordingSession.FinishingPhrase += new Obi.Events.Audio.Recorder.FinishingPhraseHandler(
                delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                {
                    PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(_e.PhraseIndex + mRecordingInitPhraseIndex);
                    phrase.SignalAudioChanged(this, _e.Audio);
                });
            mRecordingSession.FinishingPage += new Events.Audio.Recorder.FinishingPageHandler(
                delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                {
                    SetPageNumberWhileRecording(_e);
                });
            if (startRecording)
            {
                StartRecording();
            }
            else
            {
                StartListening();
            }
            SetTimeDisplayForRecording();
        }
        
        void mRecordingSession_FinishingPhrase(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void SetPageNumberWhileRecording( Obi.Events.Audio.Recorder.PhraseEventArgs  e )
        {
                        int PageNumber = mView.Presentation.PageNumberFor(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + e.PhraseIndex)) ;
                                                urakawa.undo.ICommand cmd = new Commands.Node.SetPageNumber(mView,(EmptyNode)  mRecordingSection.PhraseChild (mRecordingInitPhraseIndex + e.PhraseIndex + 1 ) ,  PageNumber );
                                                cmd.execute();
                                }

        private void SetTimeDisplayForRecording()
        {
                        mDisplayBox.SelectedIndex = mDisplayBox.Items.Count - 1;
                        mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString () ;
                        mDisplayTimer.Start();
        }


        // Start listening
        private void StartListening()
        {
            if (mCurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
                DisablePlaybackButtonsForRecording();
                mRecordButton.AccessibleName = Localizer.Message("start_recording");
                mRecordingSession.Listen();
            }
        }

        // Start recording
        void StartRecording()
        {
            if (mRecordingSession != null &&
                (mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Listening ||
                mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Idle))
            {
                DisablePlaybackButtonsForRecording();
                mRecordingSession.Record();
                mRecordButton.Enabled = false;

                SetTimeDisplayForRecording();
            }
        }

        // Stop recording
        private void StopRecording()
        {
            if (mRecordingSession != null &&
                (mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Listening ||
                mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Recording))
            {
                //ResetTimeDisplayForFinishedRecording();
                                                                

                mRecordingSession.Stop();
                // update phrases with audio assets
                for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
                {
                    mView.Presentation.UpdateAudioForPhrase(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i),
                        mRecordingSession.RecordedAudio[i]);
                }
                mRecordButton.Enabled = true;
                mRecordButton.AccessibleName = Localizer.Message("record");
                mRecordingSession = null;
                mResumerecordingPhrase = null;

                // enable playback controls
                mPlayButton.Enabled = true;
                mPrevPhraseButton.Enabled = true;
                mPrevSectionButton.Enabled = true;
                mPreviousPageButton.Enabled = true;
                mFastForwardButton.Enabled = true;
                mRewindButton.Enabled = true;
                mFastPlayRateCombobox.Enabled = true;
            }
        }



        private bool IsRecording
        {
            get
            {
                return mRecordingSession != null &&
                    mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Recording;
            }
        }

        public bool IsListening
        {
            get
            {
                return mRecordingSession != null &&
                    mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Listening;
            }
        }

        public bool IsActive { get { return IsPlayerActive || IsRecorderActive; } }
        private bool IsPlaying { get { return mPlayer.State == Obi.Audio.AudioPlayerState.Playing; } }
        private bool IsPlayerActive { get { return IsPaused || IsPlaying; } }
        private bool IsPaused { get { return mPlayer.State == Obi.Audio.AudioPlayerState.Paused; } }
        public bool IsRecorderActive { get { return IsListening || IsRecording; } }

        private void mCustomClassMarkButton_Click(object sender, EventArgs e) { MarkCustomClass(); }

        /// <summary>
        /// Mark custom class on current block with default name as "Custom"
        /// If recording, create new phrase and mark custom class this new phrase block
        /// else mark on currently selected block
        /// </summary>
        public void MarkCustomClass()
        {
            if (mView.Selection != null)
            {
                EmptyNode node;
                if (IsRecording)
                {
                    NextPhrase();
                    node = mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1);
                }
                else
                {
                    node = mView.SelectedNodeAs<EmptyNode>();
                }
                mView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mView, node,
                    EmptyNode.Kind.Custom, Localizer.Message("default_custom_class_name")));
            }
        }

        public void MarkTodoClass()
        {
            EmptyNode node;
            if (IsPlaying)
                Pause();

            if (IsRecording)
            {
                node = mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1);
                mView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mView, node,
                EmptyNode.Kind.TODO));
                NextPhrase();
            }
            else
            {
                node = mView.SelectedNodeAs<EmptyNode>();

                if (node != null)
                {
                    mView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mView, node,
                        EmptyNode.Kind.TODO));
                }
            }
        }

        #endregion
    }
}
