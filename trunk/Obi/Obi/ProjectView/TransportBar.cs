using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
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

        private SectionNode mRecordingSection;       // Section in which we are recording
        private PhraseNode mRecordingPhrase;         // Phrase which we are recording in (after start, before end)
        private int mRecordingInitPhraseIndex;       // Phrase child in which we are recording
        private bool mIsSelectionMarked = false;     // this should probably go I think


        private string mPrevSectionAccessibleName;   // Normal accessible name for the previous section button ???
        private string mStopButtonAccessibleName;    // Normal accessible name for the stop button ???


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
            AddTransportBarAccessibleName();
        }


        /// <summary>
        /// Get the phrase currently playing (or paused) if playback is active; null otherwise.
        /// </summary>
        public PhraseNode PlaybackPhrase
        {
            get { return IsPlayerActive ? mCurrentPlaylist.CurrentPhrase : null; }
        }

        // Set the accessible name of previous section/stop buttons (???)
        private void AddTransportBarAccessibleName()
        {
            mPrevSectionAccessibleName = mPrevSectionButton.AccessibleName;
            mStopButtonAccessibleName = mStopButton.AccessibleName;
            mPrevSectionButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mPrevSectionAccessibleName);
            mStopButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mStopButtonAccessibleName);
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

        public bool CanFastForward { get { return Enabled && mRecordingSession == null; } }
        public bool CanMarkCustomClass { get { return Enabled && mView.CanMarkPhrase; } }
        public bool CanNavigateNextPage { get { return Enabled; } }
        public bool CanNavigateNextPhrase { get { return Enabled; } }
        public bool CanNavigateNextSection { get { return Enabled; } }
        public bool CanNavigatePrevPage { get { return Enabled && mRecordingSession == null; } }
        public bool CanNavigatePrevPhrase { get { return Enabled && mRecordingSession == null; } }
        public bool CanNavigatePrevSection { get { return Enabled && mRecordingSession == null; } }
        public bool CanPause { get { return Enabled && (mState == State.Playing || mState == State.Recording); } }
        public bool CanPlay { get { return Enabled && mState == State.Stopped; } }
        public bool CanRecord { get { return Enabled && mState == State.Stopped; } }
        public bool CanResumePlayback { get { return Enabled && mState == State.Paused; } }
        public bool CanResumeRecording { get { return Enabled && mState == State.Monitoring; } }
        public bool CanRewind { get { return Enabled && mRecordingSession == null; } }
        public bool CanStop { get { return Enabled && (mState != State.Stopped || mView.Selection != null); } }
        
        /// <summary>
        /// A phrase can be split if there is an audio selection, or when audio is playing or paused.
        /// </summary>
        public bool CanSplitPhrase { get { return HasAudioCursor || mView.Selection is AudioSelection; } }

        public bool HasAudioCursor
        {
            get
            {
                return mPlayer.State == Obi.Audio.AudioPlayerState.Paused ||
                    mPlayer.State == Obi.Audio.AudioPlayerState.Playing;
            }
        }

        /// <summary>
        /// Split time is either 
        /// </summary>
        public double SplitTime
        {
            get
            {
                return mPlayer.State == Obi.Audio.AudioPlayerState.Paused ||
                    mPlayer.State == Obi.Audio.AudioPlayerState.Playing ?
                    mCurrentPlaylist.CurrentTimeInAsset :
                    ((AudioSelection)mView.Selection).AudioRange.CursorTime;
            } 
        }

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
        /// Mark the begin time/cursor of a selection.
        /// </summary>
        public bool MarkSelectionBeginTime()
        {
            if (mPlayer.State == Obi.Audio.AudioPlayerState.Playing || mPlayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(mCurrentPlaylist.CurrentTimeInAsset));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mark the end time of a selection. If no begin time is set, add a cursor/begin time.
        /// If a selection is already set, make a new selection from the beginning of said
        /// selection and the current cursor position.
        /// </summary>
        public bool MarkSelectionEndTime()
        {
            if (mPlayer.State == Obi.Audio.AudioPlayerState.Playing || mPlayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                AudioSelection selection = mView.Selection as AudioSelection;
                double begin = 0.0;
                double end = 0.0;
                if (selection != null && selection.Node == mCurrentPlaylist.CurrentPhrase)
                {
                    double now = mCurrentPlaylist.CurrentTimeInAsset;
                    double cursor = selection.AudioRange.HasCursor ? selection.AudioRange.CursorTime :
                        selection.AudioRange.SelectionBeginTime;
                    begin = cursor < now ? cursor : now;
                    end = cursor > now ? cursor : now;
                }
                if (begin != end)
                {
                    mView.Selection = new AudioSelection((PhraseNode)selection.Node, selection.Control, new AudioRange(begin, end));
                    return true;
                }
                else
                {
                    // If nothing was set, behave as if we started a selection.
                    return MarkSelectionBeginTime();
                }
            }
            return false;
        }

        /// <summary>
        /// Mark a selection from the beginning of the waveform to the current cursor position.
        /// </summary>
        public bool MarkSelectionFromCursor()
        {
            if ((mPlayer.State == Obi.Audio.AudioPlayerState.Playing || mPlayer.State == Obi.Audio.AudioPlayerState.Paused) &&
                mCurrentPlaylist.CurrentTimeInAsset > 0.0)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, mCurrentPlaylist.CurrentTimeInAsset));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mark a selection from the current cursor position to the end of the cursor.
        /// </summary>
        public bool MarkSelectionToCursor()
        {
            if ((mPlayer.State == Obi.Audio.AudioPlayerState.Playing || mPlayer.State == Obi.Audio.AudioPlayerState.Paused) &&
                mCurrentPlaylist.CurrentTimeInAsset < mCurrentPlaylist.CurrentPhrase.Audio.getDuration().getTimeDeltaAsMillisecondFloat())
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(mCurrentPlaylist.CurrentTimeInAsset,
                        mCurrentPlaylist.CurrentPhrase.Audio.getDuration().getTimeDeltaAsMillisecondFloat()));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mark a selection for the whole phrase.
        /// </summary>
        public bool MarkSelectionWholePhrase()
        {
            if (mPlayer.State == Obi.Audio.AudioPlayerState.Playing || mPlayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, mCurrentPlaylist.CurrentPhrase.Audio.getDuration().getTimeDeltaAsMillisecondFloat()));
                return true;
            }
            else if (mState == State.Stopped && mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, mCurrentPlaylist.CurrentPhrase.Audio.getDuration().getTimeDeltaAsMillisecondFloat()));
                return true;
            }
            return false;
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
            mView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
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
                mView.SelectionChanged += new EventHandler(delegate(object sender, EventArgs e) { UpdateButtons(); });
            }
        }

        /// <summary>
        /// Get the recorder associated with the transport bar.
        /// </summary>
        public Audio.AudioRecorder Recorder { get { return mRecorder; } }

        /// <summary>
        /// Get the VU meter associated with the transport bar.
        /// </summary>
        public Audio.VuMeter VuMeter { get { return mVuMeter; } }


        // Initialize audio (player, recorder, VU meter.)
        private void InitAudio()
        {
            mPlayer = new Audio.AudioPlayer();
            mRecorder = new Obi.Audio.AudioRecorder();
            mRecorder.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler(Recorder_StateChanged);
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

        // Property set when the selection is an audio selection (range or cursor)
        private bool IsInPhraseSelectionMarked
        {
            get
            {
                return mView.Selection != null && mView.Selection is AudioSelection;
            }
        }

        // Synchronize accessible label of the the time display box.
        private void mDisplayBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
        }

        // Update the time display immediatly when the display mode changes.
        private void mDisplayBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        // Periodically update the time display and the audio cursor.
        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
            if (mPlayer.State == Obi.Audio.AudioPlayerState.Playing) mView.UpdateCursorPosition(mCurrentPlaylist.CurrentTimeInAsset);
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
        private void Presentation_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (mState != State.Stopped) Stop();
        }

        // Adapt to changes in used status.
        // At the moment, simply stop.
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (mState != State.Stopped) Stop();
        }

        // Update state from the recorder.
        private void Recorder_StateChanged(object sender, Obi.Events.Audio.Recorder.StateChangedEventArgs e)
        {
            mState = mRecorder.State == Obi.Audio.AudioRecorderState.Monitoring ? State.Monitoring :
                mRecorder.State == Obi.Audio.AudioRecorderState.Recording ? State.Recording : State.Stopped;
            UpdateButtons();
            UpdateTimeDisplay();
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
                mRecorder.State == Obi.Audio.AudioRecorderState.Monitoring ? "start_recording" : "start_monitoring"
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
                mDisplayBox.SelectedIndex = ELAPSED_INDEX;
            }
            else if (mState == State.Recording)
            {
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(mRecordingSession.AudioRecorder.TimeOfAsset);
                mDisplayBox.SelectedIndex = ELAPSED_INDEX;
            }
            else if (mState == State.Stopped)
            {
                mTimeDisplayBox.Text = ObiForm.FormatTime_hh_mm_ss(0.0);
            }
            else
            {
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
        /// Start from the current selection, or from the first phrase.
        /// </summary>
        public void PlayAll()
        {
            mCurrentPlaylist = mMasterPlaylist;
            PlayCurrentPlaylistFromSelection();
        }

        /// <summary>
        /// All-purpose play function for the play button.
        /// Play or resume if possible, otherwise do nothing.
        /// If there is a selection, play the selection; if there is no selection, play everything
        /// (depending on the relevent preference setting.)
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
                mLocalPlaylist = new Playlist(mPlayer, mView.Selection);
                SetPlaylistEvents(mLocalPlaylist);
                mCurrentPlaylist = mLocalPlaylist;
                PlayCurrentPlaylistFromSelection();
            }
        }

        // Play the current playlist from the current selection.
        private void PlayCurrentPlaylistFromSelection()
        {
            if (mView.Selection is AudioSelection
                && mView.Selection.Node is PhraseNode
                && (!((AudioSelection)mView.Selection).AudioRange.HasCursor || mIsSelectionMarked)
                && ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime > ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime)
            {
                // Play the current selection
                mCurrentPlaylist.CurrentPhrase = (PhraseNode)mView.Selection.Node;
                if (mCurrentPlaylist == mMasterPlaylist)
                {
                    // when "play all", ignore the end time
                    mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime);
                }
                else
                {
                    mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime, ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime);
                }
            }
            else if (mView.Selection is AudioSelection
                && mView.Selection.Node is PhraseNode
                && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
            {
                // Play from the cursor
                mCurrentPlaylist.CurrentPhrase = (PhraseNode)mView.Selection.Node;
                mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.CursorTime);
            }
            else if (mView.Selection is StripCursorSelection)
            {
                StripCursorSelection s = (StripCursorSelection)mView.Selection;
                if (s.Index < s.Section.PhraseChildCount)
                {
                    mCurrentPlaylist.CurrentPhrase = (PhraseNode)s.Section.PhraseChild(s.Index);
                }
                mCurrentPlaylist.Play();
            }
            else
            {
                mCurrentPlaylist.Play();
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

        private void mRecordButton_Click(object sender, EventArgs e) { Record(); }

        /// <summary>
        /// Start monitoring (if stopped) or recording (if monitoring)
        /// </summary>
        public void Record()
        {
            if (mState == State.Monitoring)
            {
                mRecordingSession.Stop();
                StartRecording();
            }
            else if (CanResumeRecording)
            {
                PrepareForRecording(true, mResumerecordingPhrase);
            }
            else
            {
                PrepareForRecording(false, null);
            }
        }

        // Prepare for recording and return the corresponding recording command.
        private void PrepareForRecording(bool startRecording, ObiNode selected)
        {
            urakawa.undo.CompositeCommand command = CreateRecordingCommand();
            selected = GetRecordingSection(selected, command);
            EmptyNode emptyNode = null;  // empty node to record in
            // TODO: record at the position in the block, or replace the waveform selection
            if (selected is SectionNode)
            {
                // Record a new node in an existing section
                mRecordingSection = (SectionNode)selected;
                mRecordingInitPhraseIndex = mRecordingSection.PhraseChildCount;
            }
            else if (selected is PhraseNode)
            {
                // Record after or inside the phrase node
                mRecordingSection = selected.ParentAs<SectionNode>();
                mRecordingInitPhraseIndex = 1 + selected.Index;
                if (mAllowOverwrite && (IsInPhraseSelectionMarked || mState == State.Paused))
                { //1
                    if (mState == State.Paused && !(mView.Selection is AudioSelection))
                    { // 2
                        command.append(new Commands.Node.SplitAudio(mView , SplitTime));
                    } //-2
                    else if (mView.Selection is AudioSelection)
                    { //2
                        AudioRange range = ((AudioSelection)mView.Selection).AudioRange;
                        if (range.HasCursor)
                        { //3
                            // Split the phrase at the cursor
                            command.append(new Commands.Node.SplitAudio(mView));
                        } //-3
                        else if (range.SelectionBeginTime == 0)
                        { //3
                            if (range.SelectionEndTime < ((PhraseNode)selected).Audio.getDuration().getTimeDeltaAsMillisecondFloat())
                            { //4
                                // Split at the end of the selection (if there is something after the end...)
                                command.append(new Commands.Node.SplitAudio(mView, range.SelectionEndTime));
                            } //-4
                            // ... and remove the first half.
                            command.append(new Commands.Node.Delete(mView, mView.Selection.Node));
                            // Now we must recorde *before* the selected node
                            --mRecordingInitPhraseIndex;
                        } //-3
                        else
                        { //3
                            if (range.SelectionEndTime < ((PhraseNode)selected).Audio.getDuration().getTimeDeltaAsMillisecondFloat())
                            { //4
                                // Split at the end if necessary (do it first so that times are correct)
                                command.append(new Commands.Node.SplitAudio(mView, range.SelectionEndTime));
                            } //-4
                            // Split at the beginning of the selection
                            command.append(new Commands.Node.SplitAudio(mView, range.SelectionBeginTime));
                            // ... and remove the split part.
                            command.append(new Commands.Node.DeleteWithOffset(mView, selected, 1));
                        } //-3
                    } //-2
                } //-1 overwrite if ends

                if (mCurrentPlaylist.State == Audio.AudioPlayerState.Paused) mCurrentPlaylist.Stop();
            }
            else if (selected is EmptyNode)
            {
                // Record inside the empty node
                mRecordingSection = selected.ParentAs<SectionNode>();
                mRecordingInitPhraseIndex = selected.Index;
                emptyNode = (EmptyNode)selected;
            }
            Settings settings = mView.ObiForm.Settings;
            mRecordingSession = new RecordingSession(mView.Presentation, mRecorder);
            mRecordingSession.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                delegate(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
                {
                    mView.Presentation.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    PhraseNode phrase = mView.Presentation.CreatePhraseNode(e.Audio);
                    mRecordingPhrase = phrase;
                    if (e.PhraseIndex > 0)
                    {
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                            mRecordingInitPhraseIndex + e.PhraseIndex));
                    }
                    else
                    {
                        command.append(new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                            mRecordingInitPhraseIndex + e.PhraseIndex));
                        if (emptyNode != null)
                        {
                            phrase.CopyKind(emptyNode);
                            phrase.Used = emptyNode.Used;
                            command.append(new Commands.Node.Delete(mView, emptyNode));
                            emptyNode = null;
                        }
                        mView.Presentation.getUndoRedoManager().execute(command);
                        mView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    }
                });
            mRecordingSession.FinishingPhrase += new Obi.Events.Audio.Recorder.FinishingPhraseHandler(
                delegate(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
                {
                    PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(e.PhraseIndex + mRecordingInitPhraseIndex);
                    phrase.SignalAudioChanged(this, e.Audio);
                    mRecordingPhrase = null;
                });
            mRecordingSession.FinishingPage += new Events.Audio.Recorder.FinishingPageHandler(
                delegate(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
                {
                    SetPageNumberWhileRecording(e);
                });
            if (startRecording)
            {
                StartRecording();
            }
            else
            {
                mRecordingSession.StartMonitoring();
            }
        }

        // Create a new recording command.
        private urakawa.undo.CompositeCommand CreateRecordingCommand()
        {
            urakawa.undo.CompositeCommand command = mView.Presentation.getCommandFactory().createCompositeCommand();
            command.setShortDescription(Localizer.Message("recording_command"));
            return command;
        }

        // Get the recording section from the initial selection argument.
        // If the argument is null, get the selection, otherwise add a new
        // top-level section to record in (so the recording command includes
        // creating the new section.)
        private ObiNode GetRecordingSection(ObiNode selected, urakawa.undo.CompositeCommand command)
        {
            mView.SelectInContentView();
            if (selected == null) selected = mView.SelectedNodeAs<ObiNode>();
            if (selected == null)
            {
                // create a new section node to record in
                SectionNode section = mView.Presentation.CreateSectionNode();
                Commands.Node.AddNode add = new Commands.Node.AddNode(mView, section, mView.Presentation.RootNode,
                    mView.Presentation.RootNode.SectionChildCount);
                add.UpdateSelection = false;
                command.append(add);
                selected = section;
            }
            return selected;
        }

        // Start recording
        void StartRecording()
        {
            mRecordingSession.Record();
            mDisplayTimer.Start();
        }


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
        /// If recording, create a new phrase to record in.
        /// If playing or paused,
        /// </summary>
        public void NextPhrase()
        {
            if (Enabled)
            {
                if (mState == State.Recording)
                {
                    // record into to next phrase.
                    mRecordingSession.NextPhrase();
                }
                else if (mState == State.Monitoring)
                {
                    // start recording

                }
                else if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        mCurrentPlaylist.CurrentPhrase = mView.Selection.Node as PhraseNode;
                        mCurrentPlaylist.NavigateToNextPhrase();
                    }
                    else
                    {
                        mView.SelectNextPhrase();
                    }
                }
                else
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


        // Rewind/Fast forward

        private void mRewindButton_Click(object sender, EventArgs e) { Rewind(); }
        private void mFastForwardButton_Click(object sender, EventArgs e) { FastForward(); }

        /// <summary>
        /// Play faster.
        /// </summary>
        public void FastForward()
        {
            if (Enabled && mRecordingSession == null)
            {
                if (mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped) PlayOrResume();
                mCurrentPlaylist.FastForward();
            }
        }

        /// <summary>
        /// Rewind (i.e. play faster backward)
        /// </summary>
        public void Rewind()
        {
            if (Enabled && mRecordingSession == null)
            {
                if (mCurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped) PlayOrResume();
                mCurrentPlaylist.Rewind();
            }
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

        private void mFastPlayRateComboBox_SelectionChangeCommitted(object sender, EventArgs e)
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
        /// Start recording directly without going through listening
        /// </summary>
        public void StartRecordingDirectly()
        {
            if (mRecordingSession == null && mCurrentPlaylist.Audioplayer.State != Obi.Audio.AudioPlayerState.Playing)
            {
                PrepareForRecording(true, null);
            }
        }

        private void SetPageNumberWhileRecording(Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            int PageNumber = mView.Presentation.PageNumberFor(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + e.PhraseIndex));
            urakawa.undo.ICommand cmd = new Commands.Node.SetPageNumber(mView, (EmptyNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + e.PhraseIndex + 1), PageNumber);
            cmd.execute();
        }




        // Stop recording
        private void StopRecording()
        {
            if (mRecordingSession != null &&
                (mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Monitoring ||
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
                    mRecordingSession.AudioRecorder.State == Obi.Audio.AudioRecorderState.Monitoring;
            }
        }

        public bool IsActive { get { return Enabled && ( IsPlayerActive || IsRecorderActive ); } }
        private bool IsPlaying { get { return mPlayer.State == Obi.Audio.AudioPlayerState.Playing; } }
        public bool IsPlayerActive { get { return IsPaused || IsPlaying; } }
        private bool IsPaused { get { return mPlayer.State == Obi.Audio.AudioPlayerState.Paused; } }
        public bool IsRecorderActive { get { return IsListening || IsRecording; } }

        private void mCustomClassMarkButton_Click(object sender, EventArgs e) { MarkCustomClass(); }

        /// <summary>
        /// Mark custom class on current block. Currently defaults to "TODO".
        /// If recording, create new phrase and mark custom class this new phrase block
        /// else mark on currently playing block; otherwise on selected block
        /// </summary>
        public void MarkCustomClass()
        {
            if (mView.CanMarkPhrase)
            {
                EmptyNode node;
                if (IsRecording)
                {
                    mRecordingSession.NextPhrase();
                    node = mRecordingPhrase;
                }
                else
                {
                    if (mPlayer.State == Obi.Audio.AudioPlayerState.Paused ||
                        mPlayer.State == Obi.Audio.AudioPlayerState.Playing)
                    {
                        node = mCurrentPlaylist.CurrentPhrase;
                    }
                    else
                    {
                        node = mView.SelectedNodeAs<EmptyNode>();
                    }
                }
                mView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mView, node,
                    mView.MarkRole, mView.MarkCustomRole));
            }
        }

        public void MarkTodoClass()
        {
            EmptyNode node;
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


        private void TransportBar_Leave(object sender, EventArgs e)
        {
            mPrevSectionButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mPrevSectionAccessibleName);
            mStopButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mStopButtonAccessibleName);
        }


        private void TransportBar_Enter(object sender, EventArgs e)
        {
            mPrevSectionButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mPrevSectionAccessibleName);
            mStopButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mStopButtonAccessibleName);
            Thread TrimAccessibleName = new Thread(new ThreadStart(TrimTransportBarAccessibleLabel));
            TrimAccessibleName.Start();
        }

        private void TrimTransportBarAccessibleLabel()
        {
            Thread.Sleep(750);
            mPrevSectionButton.AccessibleName = mPrevSectionAccessibleName;
            mStopButton.AccessibleName = mStopButtonAccessibleName;
        }

        /// <summary>
        ///  Process dialog key overridden to prevent tab  from  moving focus out of transportbar
        /// </summary>
        /// <param name="KeyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (KeyData == Keys.Tab
                &&     this.ActiveControl != null )
            {
                    Control c = this.ActiveControl;
                    this.SelectNextControl(c, true, true, true, true);
                    if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                        System.Media.SystemSounds.Beep.Play();

                    return true;
            }
            else if (KeyData == (Keys)(Keys.Shift | Keys.Tab)
                &&    this.ActiveControl != null)
            {
                    Control c = this.ActiveControl;
                    this.SelectNextControl(c, false, true, true, true);
                    if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                        System.Media.SystemSounds.Beep.Play();

                    return true;
            }
            else
                return base.ProcessDialogKey(KeyData);
        }

        

        


    }
}
