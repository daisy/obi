using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using urakawa.core;
using urakawa.command;
using urakawa.media.timing;
using System.Drawing;
using System.Resources;

namespace Obi.ProjectView
{
    /// <summary>
    /// The transport bar: transport buttons, scrubbing slide, time display, text vu meter display.
    /// </summary>
    public partial class TransportBar : UserControl
    {
        private ProjectView mView;                   // the parent project view
        private bool m_IsProjectEmpty; // is true if project has no sections

        private AudioLib.AudioPlayer mPlayer;           // the audio player
        private AudioLib.AudioRecorder mRecorder;       // the audio recorder
        private AudioLib.VuMeter mVuMeter;              // VU meter
        //private bool m_AutoSaveOnNextRecordingEnd ; //flag to auto save whenever recording stops or pauses next time//@singleSection:commented

        private RecordingSession mRecordingSession;  // current recording session
        private SectionNode mRecordingSection;       // Section in which we are recording
        private PhraseNode mResumeRecordingPhrase;   // last phrase recorded (?)
        private PhraseNode mRecordingPhrase;         // Phrase which we are recording in (after start, before end)
        private int mRecordingInitPhraseIndex;       // Phrase child in which we are recording
        
        private State mState;                        // transport bar state (composite of player/recorder states)
        private bool m_CanMoveSelectionToPlaybackPhrase = true;//@singleSection
        private List<string> m_RecordingElapsedRemainingList = new List<string>();
        private List<string> m_PlayingElapsedRemainingList = new List<string>();
        // Playlists
        private Playlist mCurrentPlaylist;           // playlist currently playing, null when not playing
        private Playlist mMasterPlaylist;            // master playlist (all phrases in the project)
        private Playlist mQAMasterPlaylist;          // QA master playlist (all used phrases in the project)
        private Playlist mLocalPlaylist;             // local playlist (only selected; may be null) TO BE REMOVED

        Bitmap m_monitorButtonImage;
        Bitmap m_recordButtonImage;

        #region CAN WE REMOVE THIS?

        private bool mPlayQAPlaylist = false; // this should be set from UI
        private bool mSelectionChangedPlayEnable; // flag for enabling / disabling playback on change of selection
        private Mutex m_PlayOnSelectionChangedMutex ;

        private string mPrevSectionAccessibleName;   // Normal accessible name for the previous section button ???
        private string mStopButtonAccessibleName;    // Normal accessible name for the stop button ???

        // Set the accessible name of previous section/stop buttons (???)
        private void AddTransportBarAccessibleName()
        {
            mPrevSectionAccessibleName = mPrevSectionButton.AccessibleName;
            mStopButtonAccessibleName = mStopButton.AccessibleName;
            mPrevSectionButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mPrevSectionAccessibleName);
            mStopButton.AccessibleName = string.Format("{0} {1}", Localizer.Message("transport_bar"), mStopButtonAccessibleName);
        }


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

        public bool SelectionChangedPlaybackEnabled
        {
            get { return mSelectionChangedPlayEnable; }
            set { mSelectionChangedPlayEnable = value; }
        }

        #endregion


        // Constants from the display combo box
        private static readonly int ELAPSED_INDEX = 0;
        private static readonly int ELAPSED_SECTION = 1;
       // private static readonly int ELAPSED_SELECTION = 2;
        private static int ELAPSED_TOTAL_INDEX = 2;
        private static int ELAPSED_TOTAL_RECORDING_INDEX = 2;
        private static int REMAINING_IN_SECTION = 4;
        private static readonly int REMAIN_INDEX = 3;
        private readonly List<string> m_DisplayComboBoxItems;


        // Pass the state change and playback rate change events from the playlist
        public event AudioLib.AudioPlayer.StateChangedHandler StateChanged;
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
            m_RecordingElapsedRemainingList.Add("elapsed");
            m_RecordingElapsedRemainingList.Add("elapsed in section");
            m_RecordingElapsedRemainingList.Add("elapsed in project");
            m_PlayingElapsedRemainingList.Add("elapsed in phrase");
            m_PlayingElapsedRemainingList.Add("elapsed in section");
            m_PlayingElapsedRemainingList.Add("elapsed in selection");
            m_PlayingElapsedRemainingList.Add("remaining in phrase");
            m_PlayingElapsedRemainingList.Add("remaining in section");
            m_PlayingElapsedRemainingList.Add("remaining in selection");
            mDisplayBox.Items.AddRange(m_PlayingElapsedRemainingList.ToArray ());
            mDisplayBox.SelectedIndex = 0;
            mTimeDisplayBox.AccessibleName = mDisplayBox.SelectedItem.ToString();
            mFastPlayRateCombobox.SelectedIndex = 0;
            mState = State.Stopped;
            mSelectionChangedPlayEnable = true;
            AddTransportBarAccessibleName();
            m_PlayOnSelectionChangedMutex = new Mutex ();
            m_DisplayComboBoxItems = new List<string>();
            for (int i = 0; i < mDisplayBox.Items.Count; i++) m_DisplayComboBoxItems.Add(mDisplayBox.Items[i].ToString());
            
            ResourceManager resourceManager = new ResourceManager("Obi.ProjectView.TransportBar", GetType().Assembly);
            m_monitorButtonImage = (Bitmap)resourceManager.GetObject("media-monitor.png");
            m_recordButtonImage = (Bitmap)resourceManager.GetObject("mRecordButton.Image");
        }

        /// <summary>
        /// Get the audio player used by the transport bar.
        /// </summary>
        public AudioLib.AudioPlayer AudioPlayer { get { return mPlayer; } }

        public bool CanFastForward { get { return Enabled && (IsPlayerActive || CanPlay) ; } }
        public bool CanMarkCustomClass { get { return Enabled && mView.CanMarkPhrase; } }
        public bool CanNavigatePrevPage { get { return Enabled && !m_IsProjectEmpty && ( IsPlayerActive || CanPlay ) ; } }
        public bool CanNavigatePrevSection { get { return Enabled && !m_IsProjectEmpty && (IsPlayerActive || CanPlay) ; } }
        public bool CanPause { get { return Enabled && (mState == State.Playing || mState == State.Recording) ; } }
        public bool CanPausePlayback { get { return Enabled && mState == State.Playing; } }
        public bool CanPlay { get { return Enabled && mState == State.Stopped && !m_IsProjectEmpty && !mView.IsContentViewScrollActive; } }
        public bool CanRecord { get { return Enabled &&( mState == State.Stopped || mState == State.Paused ||  mState == State.Monitoring  ||  (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing 
            && mCurrentPlaylist.PlaybackRate == 0)) &&  mView.IsPhraseCountWithinLimit && !mView.IsContentViewScrollActive; } } // @phraseLimit
        public bool CanResumePlayback { get { return Enabled && mState == State.Paused   &&   !mView.IsContentViewScrollActive; } }
        public bool CanResumeRecording { get { return Enabled && mResumeRecordingPhrase != null && mResumeRecordingPhrase.IsRooted    &&   (mState != State.Playing  ||   (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing) )&& !mView.IsContentViewScrollActive; } }
        public bool CanRewind { get { return Enabled && (IsPlayerActive || CanPlay) ; } }
        public bool CanStop { get { return Enabled && (mState != State.Stopped || mView.Selection != null); } }

        public bool CanNavigatePrevPhrase
        {
            get
            {
                return (!m_IsProjectEmpty && IsPlayerActive && mCurrentPlaylist.CanNavigatePrevPhrase) || CanPlay;
            }
        }

        public bool CanNavigateNextPhrase
        {
            get
            {
                return IsRecorderActive ||
                    (IsPlayerActive && mCurrentPlaylist.CanNavigateNextPhrase) ||
                    CanPlay;
            }
        }

        public bool CanNavigateNextPage
        {
            get
            {
                return IsRecorderActive ||
                    (IsPlayerActive && mCurrentPlaylist.CanNavigateNextPage) ||
                    CanPlay;
            }
        }

        public bool CanNavigateNextSection
        {
            get
            {
                return IsRecorderActive ||
                    (IsPlayerActive && mCurrentPlaylist.CanNavigateNextSection) ||
                    CanPlay;
            }
        }

        public bool CanPreview
        {
            get
            {
                return Enabled && (IsPlayerActive 
                    || (mView.Selection != null && mView.Selection is AudioSelection 
                    && !IsRecorderActive));
            }
        }
        
        public bool CanPreviewAudioSelection
        {
            get
            {
                return Enabled && mView.Selection != null &&  mView.Selection is AudioSelection
                    && ((AudioSelection)mView.Selection).AudioRange != null && !((AudioSelection)mView.Selection).AudioRange.HasCursor && !IsRecorderActive;
            }
        }
        
        /// <summary>
        /// A phrase can be split if there is an audio selection, or when audio is playing or paused.
        /// </summary>
        public bool CanSplitPhrase { get { return (IsPlayerActive 
            || ( mView.Selection != null  &&   mView.Selection is AudioSelection   &&   ((AudioSelection)mView.Selection).AudioRange != null))    
            &&    IsPhraseCountWithinLimit; } } // @phraseLimit

        //@singleSection
        public bool CanMoveSelectionToPlaybackPhrase
            {
            get { return m_CanMoveSelectionToPlaybackPhrase; }
            set { m_CanMoveSelectionToPlaybackPhrase = value; }
        }

        // @phraseLimit
        /// <summary>
        ///  Returns true if phrase count of  selected section is below below max visible phrase blocks count
        /// it also consider active node w.r.t. recording / playback instead of selection if recording or playback is active
                /// </summary>
        public bool IsPhraseCountWithinLimit
            {
            get
                {
                if (IsRecorderActive && mRecordingSection != null && mRecordingSection.PhraseChildCount < mView.MaxVisibleBlocksCount)
                    return true;
                else if (IsPlayerActive && mCurrentPlaylist.CurrentPhrase != null && mCurrentPlaylist.CurrentPhrase.IsRooted  && mCurrentPlaylist.CurrentPhrase.ParentAs<SectionNode>().PhraseChildCount < mView.MaxVisibleBlocksCount)
                    return true;
                else
                    return mView.IsPhraseCountWithinLimit;
                }
            }

        public double ElapsedTimeInSection
        {
            get 
            {
                PhraseNode phrNode = mCurrentPlaylist.CurrentPhrase;
                ObiNode nodeSel = phrNode.ParentAs<SectionNode>();
                double time = 0;
                for (int i = 0; i < nodeSel.PhraseChildCount; i++)
                {
                    if (nodeSel.PhraseChild(i) != mCurrentPlaylist.CurrentPhrase && nodeSel.PhraseChild(i) is PhraseNode)
                        time = nodeSel.PhraseChild(i).Duration + time;
                    else
                        break;                    
                }
                return time + mCurrentPlaylist.CurrentTimeInAsset;
            }
        }

        public double RemainingTimeInSection
        {
            get { return mCurrentPlaylist.CurrentSection.Duration - ElapsedTimeInSection;  }
        }

        /// <summary>
        /// Set color settings for the transport bar.
        /// </summary>
        public ColorSettings ColorSettings
        {
            set
            {
                if (value != null)
                {
                    BackColor = value.TransportBarBackColor;
                    mTransportBarTooltip.ForeColor = value.ToolTipForeColor;
                    mTimeDisplayBox.BackColor = value.TransportBarLabelBackColor;
                    mTimeDisplayBox.ForeColor = value.TransportBarLabelForeColor;
                }
            }
        }

        /// <summary>
        /// Get the phrase currently playing (or paused) if playback is active; null otherwise.
        /// </summary>
        public PhraseNode PlaybackPhrase
        {
            get { return IsPlayerActive ? mCurrentPlaylist.CurrentPhrase : null; }
        }

        /// <summary>
        /// Split time is either the current playback position, or when stopped, the selection position.
        /// Will return 0.0 in case something goes wrong (which may be the actual split time, but we don't
        /// want to split then anyway do we?)
        /// </summary>
        public double SplitBeginTime
        {
            get
            {
                return IsPlayerActive
                    && !(mView.Selection is AudioSelection && (((AudioSelection)mView.Selection).AudioRange != null   && !((AudioSelection)mView.Selection).AudioRange.HasCursor) ) ?
                    mCurrentPlaylist.CurrentTimeInAsset :
                    mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange  != null?
                        ((AudioSelection)mView.Selection).AudioRange.HasCursor ?
                            ((AudioSelection)mView.Selection).AudioRange.CursorTime :
                            ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime :
                        0.0;
            }
        }

        /// <summary>
        /// Get the end time for splitting; this is only valid for audio selections when playback is not
        /// underway. In all other cases return 0.0.
        /// </summary>
        public double SplitEndTime
        {
            get
            {
                return mPlayer.CurrentState != AudioLib.AudioPlayer.State.Playing &&
                    mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange != null
                    && !((AudioSelection)mView.Selection).AudioRange.HasCursor ?
                    ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime : 0.0;
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
            if (mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(mCurrentPlaylist.CurrentTimeInAsset));

                                PlayAudioClue (AudioCluesSelection.SelectionBegin ) ;
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
            if (mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused)
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
                    
                    PlayAudioClue ( AudioCluesSelection.SelectionEnd) ;
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
            if ((mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused) &&
                mCurrentPlaylist.CurrentTimeInAsset > 0.0)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, mCurrentPlaylist.CurrentTimeInAsset));
                return true;
            }
            else if (CurrentState == State.Stopped && mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
            {
                double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, time));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mark a selection from the current cursor position to the end of the cursor.
        /// </summary>
        public bool MarkSelectionToCursor()
        {
            if ((mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused) &&
                mCurrentPlaylist.CurrentTimeInAsset < mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsTimeSpan.TotalMilliseconds)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(mCurrentPlaylist.CurrentTimeInAsset,
                        mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsTimeSpan.TotalMilliseconds));
                return true;
            }
            else if (CurrentState == State.Stopped && mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
            {   
                double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(time ,
                        ((PhraseNode)mView.Selection.Node).Audio.Duration.AsTimeSpan.TotalMilliseconds));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Mark a selection for the whole phrase.
        /// </summary>
        public bool MarkSelectionWholePhrase()
        {
            if (mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsTimeSpan.TotalMilliseconds));
                return true;
            }
            else if (mState == State.Stopped && mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, ((PhraseNode)mView.Selection.Node).Audio.Duration.AsTimeSpan.TotalMilliseconds));
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
            mCurrentPlaylist = mMasterPlaylist;
            mResumeRecordingPhrase = null;
            mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
            m_IsProjectEmpty = mView.Presentation.FirstSection == null;

            //m_AutoSaveOnNextRecordingEnd = false;
            UpdateButtons();
        }

        /// <summary>
        /// If true, play all when there is no selection; otherwise, play nothing.
        /// </summary>
        public bool PlayIfNoSelection { get { return mView.ObiForm.Settings.PlayIfNoSelection; } }
        /*/// <summary>//@singleSection: commented
        /// Auto save whenever recording pauses or stops next
        ///</summary>
        public bool AutoSaveOnNextRecordingEnd 
            { get { return m_AutoSaveOnNextRecordingEnd; }
            set { m_AutoSaveOnNextRecordingEnd = value; }
            }
        */
        /// <summary>
        /// Set preview duration.
        /// </summary>
        public int PreviewDuration { get { return mView.ObiForm.Settings.PreviewDuration; } }

        /// <summary>
        ///  Empty node in which recording is taking place
                /// </summary>
        public EmptyNode RecordingPhrase { get { return mRecordingPhrase; } }

        /// <summary>
        /// Section in which recording is going on
        /// </summary>
        public SectionNode RecordingSection { get { return mRecordingSection; } }
        public string RecordingPhraseToString { get { 
            return ( mState == State.Recording && mRecordingPhrase != null && mView.Selection != null && mView.Selection.Node == mRecordingPhrase )?
            Localizer.Message("Selected_RecordingPhrase"): "" ; } }


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
                mView.BlocksVisibilityChanged += new EventHandler(mView_BlocksVisibilityChanged);
                mView.SelectionChanged += new EventHandler(delegate(object sender, EventArgs e) { 
                    
                    UpdateButtons();
                    //if (Enabled && mSelectionChangedPlayEnable &&  mView.ObiForm.Settings.PlayOnNavigate)   PlaybackOnSelectionChange();
                    if (Enabled && mSelectionChangedPlayEnable ) PlaybackOnSelectionChange_Safe ();
                });
            }
        }

        /// <summary>
        /// Get the recorder associated with the transport bar.
        /// </summary>
        public AudioLib.AudioRecorder Recorder { get { return mRecorder; } }

        /// <summary>
        /// Get the VU meter associated with the transport bar.
        /// </summary>
        public AudioLib.VuMeter VuMeter { get { return mVuMeter; } }


        // Initialize audio (player, recorder, VU meter.)
        private void InitAudio()
        {
            mPlayer = new AudioLib.AudioPlayer(false);
            //mPlayer.AllowBackToBackPlayback = true;
            mRecorder = new AudioLib.AudioRecorder();
            mRecorder.StateChanged += new AudioLib.AudioRecorder.StateChangedHandler(Recorder_StateChanged);
            mVuMeter = new AudioLib.VuMeter(mPlayer, mRecorder);
            mVUMeterPanel.VuMeter = mVuMeter;
        }

        // Initialize playlists
        private void InitPlaylists()
        {
            mMasterPlaylist = new Playlist(mPlayer , true);
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
            mDisplayBox.Tag = null;
            UpdateTimeDisplay();
            
        }

        // Periodically update the time display and the audio cursor.
        private void mDisplayTimer_Tick(object sender, EventArgs e)
        {
            UpdateTimeDisplay();
            if (mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing)
                {
                mView.UpdateCursorPosition ( mCurrentPlaylist.CurrentTimeInAsset );
                                                }
        }

        // Move the audio cursor to the phrase currently playing.

        private delegate void Playlist_MovedToPhrase_Delegate(object sender, Events.Node.PhraseNodeEventArgs e);
        private void Playlist_MovedToPhrase(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Playlist_MovedToPhrase_Delegate(Playlist_MovedToPhrase), sender, e);
            }
            else
            {
                mView.SetPlaybackPhraseAndTime(e.Node, mCurrentPlaylist.CurrentTimeInAsset);
                UpdateTimeDisplay();
            }
        }
        
        // Update the transport bar according to the player state.

        private delegate void Playlist_PlayerStateChanged_Delegate(object sender, AudioLib.AudioPlayer.StateChangedEventArgs e);
        private void Playlist_PlayerStateChanged(object sender, AudioLib.AudioPlayer.StateChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Playlist_PlayerStateChanged_Delegate(Playlist_PlayerStateChanged), sender, e);
            }
            else
            {
                mState = mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused ? State.Paused :
                   mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing ? State.Playing : State.Stopped;

                m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase = -1;
                if (mState == State.Playing || mState == State.Recording)
                {
                    mDisplayTimer.Start();
                    if (mState == State.Playing || mState == State.Paused) mView.SetPlaybackBlockIfRequired();
                }
                else if (mState == State.Stopped)
                {
                    mDisplayTimer.Stop();
                    if (!(mCurrentPlaylist is PreviewPlaylist)) mView.SetPlaybackPhraseAndTime(null, 0);//added on 31 july ,2010 
                }
                UpdateTimeDisplay();
                UpdateButtons();

                if (StateChanged != null) StateChanged(this, e);

                if (m_IsPreviewing)
                {
                    if (mState == State.Paused) mView.UpdateCursorPosition(((PreviewPlaylist)mCurrentPlaylist).RevertTime);
                    PostPreviewRestore();
                }
            }
        }

        // Simply pass the playback rate change event.

        private delegate void Playlist_PlaybackRateChanged_Delegate(object sender, EventArgs e);
        private void Playlist_PlaybackRateChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Playlist_PlaybackRateChanged_Delegate(Playlist_PlaybackRateChanged), sender, e);
            }
            else
            {
                if (PlaybackRateChanged != null) PlaybackRateChanged(sender, e);
            }
        }


        // Update the transport bar once the player has stopped.

        private delegate void Playlist_PlayerStopped_Delegate(object sender, EventArgs e);
        private void Playlist_PlayerStopped(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Playlist_PlayerStopped_Delegate(Playlist_PlayerStopped), sender, e);
            }
            else
            {
                if (mCurrentPlaylist != null && mCurrentPlaylist is PreviewPlaylist)
                    mView.UpdateCursorPosition(mAfterPreviewRestoreTime);
                else
                    mView.SetPlaybackPhraseAndTime(null, 0.0); 
            }
        }

        // Adapt to changes in the presentation.
        // At the moment, simply stop.
        private void Presentation_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            // set project empty flag depending on zero sections in presentation
        if (mView.Presentation != null)
            {
                m_IsProjectEmpty = mView.Presentation.FirstSection == null ;
            }
        if (mState != State.Stopped)
            {
            if (IsPlayerActive && mView.ObiForm.IsAutoSaveActive)
                {
                // do not stop, auto save should not disturb playback
                }
            else
                {
                Stop ();
                }
            
            }
        }
        
        // Adapt to changes in used status.
        // At the moment, simply stop.
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (mState != State.Stopped) Stop();
        }

        private delegate void Recorder_StateChanged_Delegate();
        // Update state from the recorder.
        private void Recorder_StateChanged(object sender, AudioLib.AudioRecorder.StateChangedEventArgs e)
        {
            Recorder_StateChanged();
        }

        private int m_RecorderTimeComboIndex = 0;
        private int m_PlayerTimeComboIndex = 0;
        private void Recorder_StateChanged()
        {

    if (this.InvokeRequired)
            {
                this.Invoke(new Recorder_StateChanged_Delegate(Recorder_StateChanged));
                return;
            }
            mState = mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring ? State.Monitoring :
                mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording ? State.Recording : State.Stopped;
            UpdateButtons();
            
            //int selectedIndex = mDisplayBox.SelectedIndex;
            if (mDisplayBox.Items.Count == m_RecordingElapsedRemainingList.Count)
            {
                m_RecorderTimeComboIndex = mDisplayBox.SelectedIndex;
            }
            else if (mDisplayBox.Items.Count == m_PlayingElapsedRemainingList.Count)
            {
                // following string indicate monitoring: rapid fix
                if ( mTimeDisplayBox.Text != "--:--:--") m_PlayerTimeComboIndex = mDisplayBox.SelectedIndex;
            }
            if (mRecorder.CurrentState != AudioLib.AudioRecorder.State.Recording)
            {
                m_RecordingElapsedTime_Book = -1;
                m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase = -1;
   
                mDisplayBox.Items.Clear () ;
                mDisplayBox.Items.AddRange(m_PlayingElapsedRemainingList.ToArray ());
              //  for (int i = 0; i < m_DisplayComboBoxItems.Count; i++) mDisplayBox.Items.Add(m_DisplayComboBoxItems[i]);
                //mDisplayBox.SelectedIndex = selectedIndex > -1 ? selectedIndex: 0;
                mDisplayBox.SelectedIndex = m_PlayerTimeComboIndex > -1 && m_PlayerTimeComboIndex < mDisplayBox.Items.Count ? m_PlayerTimeComboIndex : 0;
            }
            else
            {
                
                mDisplayBox.Items.Clear();
                mDisplayBox.Items.AddRange(m_RecordingElapsedRemainingList.ToArray () );
               // for (int i = 0; i < 2; i++ ) mDisplayBox.Items.Add(m_DisplayComboBoxItems[i]);
                //mDisplayBox.SelectedIndex = (selectedIndex < mDisplayBox.Items.Count) ? selectedIndex
                mDisplayBox.SelectedIndex = (m_RecorderTimeComboIndex>= 0 &&  m_RecorderTimeComboIndex < mDisplayBox.Items.Count) ? m_RecorderTimeComboIndex
                    : 0;    
            }
            
            UpdateTimeDisplay();
        }

        // Initialize events for a new playlist.
        private void SetPlaylistEvents(Playlist playlist)
        {
            playlist.MovedToPhrase += new Playlist.MovedToPhraseHandler(Playlist_MovedToPhrase);
            playlist.StateChanged += new AudioLib.AudioPlayer.StateChangedHandler(Playlist_PlayerStateChanged);
            playlist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler(Playlist_PlayerStopped);
            playlist.PlaybackRateChanged += new Playlist.PlaybackRateChangedHandler(Playlist_PlaybackRateChanged);
        }

        // Update visibility and enabledness of buttons depending on the state of the recorder
        
        private delegate void UpdateButtons_Delegate();
        public void UpdateButtons()
        {         
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateButtons_Delegate(UpdateButtons));
            }
            else
            {
                mPrevSectionButton.Enabled = CanNavigatePrevSection;
                mPreviousPageButton.Enabled = CanNavigatePrevPage;
                mPrevPhraseButton.Enabled = CanNavigatePrevPhrase;
                mRewindButton.Enabled = CanRewind;
                mPauseButton.Visible = CanPause;
                mPlayButton.Visible = !mPauseButton.Visible;
                mPlayButton.Enabled = CanPlay || CanResumePlayback;
                mFastPlayRateCombobox.Enabled = !IsRecorderActive;
                mRecordButton.Enabled = CanRecord || CanResumeRecording;
                bool recordDirectly = (mView.ObiForm  != null && mView.ObiForm.Settings.RecordDirectlyWithRecordButton) ? true : false;
                if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring || recordDirectly || mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording || CanResumeRecording)
                {
                    mRecordButton.Image = m_recordButtonImage;
                    mRecordButton.Invalidate();
                }
                else
                {
                    mRecordButton.Image = m_monitorButtonImage;
                    mRecordButton.Invalidate();
                }
                mRecordButton.AccessibleName = Localizer.Message(
                    (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring || (recordDirectly && CurrentState != State.Recording))
                        ? "start_recording"
                        : "start_monitoring"
                    );
                mStopButton.Enabled = CanStop;
                mFastForwardButton.Enabled = CanFastForward;
                mNextPhrase.Enabled = CanNavigateNextPhrase;
                mNextPageButton.Enabled = CanNavigateNextPage;
                mNextSectionButton.Enabled = CanNavigateNextSection;
                mToDo_CustomClassMarkButton.Enabled = mView.CanSetTODOStatus;
            }
        }

        private static string FormatDuration_hh_mm_ss(double durationMs)
        {
            double seconds = durationMs / 1000.0;
            int minutes = (int)Math.Floor(seconds / 60.0);
            int seconds_ = (int)Math.Floor(seconds - minutes * 60.0);
            return string.Format(Localizer.Message("duration_hh_mm_ss"), minutes / 60, minutes % 60, seconds_);
        }

        /// <summary>
        /// Update the time display to show current time. Depends on the what kind of timing is selected.
        /// </summary>

        private delegate void UpdateTimeDisplay_Delegate();
        private void UpdateTimeDisplay()
         {
             if (this.InvokeRequired)
             {
                 this.Invoke(new UpdateTimeDisplay_Delegate(UpdateTimeDisplay));
             }
             else
             {
                 int selectedIndex = mDisplayBox.SelectedIndex;
                 if (mDisplayBox.DroppedDown)
                 {
                     if ( mDisplayBox.Tag != null ) 
                     {
                         int index = -1;
                     int.TryParse ( mDisplayBox.Tag.ToString (),out index ) ;
                         if ( index >= 0) selectedIndex = index ;
                     }
                 }
                 
                 if (mState == State.Monitoring)
                 {
                     mTimeDisplayBox.Text = "--:--:--";
                     mDisplayBox.SelectedIndex = ELAPSED_INDEX;
                 }
                 else if (mState == State.Recording)
                 {
                     //mRecordingSession.AudioRecorder.TimeOfAsset
                     double timeOfAssetMilliseconds =
                        (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecordingSession.AudioRecorder.CurrentDurationBytePosition) /
                        Time.TIME_UNIT;

                     //mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(timeOfAssetMilliseconds);
                     //mDisplayBox.SelectedIndex = ELAPSED_INDEX;
                     mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(
                         selectedIndex == ELAPSED_INDEX ?
                             timeOfAssetMilliseconds :
                         selectedIndex == ELAPSED_SECTION?
                         RecordingTimeElapsedSection:
                         selectedIndex  == ELAPSED_TOTAL_RECORDING_INDEX ?
                           RecordingTimeElapsedTotal  : 0.0);

                 }
                 else if (mState == State.Stopped)
                 {
                     mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(0.0);
                 }
                 else
                 {
                     mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(
                         selectedIndex  == ELAPSED_INDEX ?
                             mCurrentPlaylist.CurrentTimeInAsset :
                         selectedIndex  == ELAPSED_SECTION ?
                             PlaybackTimeElapsedSection :
                         selectedIndex  == ELAPSED_TOTAL_INDEX ?
                             mCurrentPlaylist.CurrentTime :
                         selectedIndex  == REMAIN_INDEX ?
                             mCurrentPlaylist.RemainingTimeInAsset :
                         selectedIndex == REMAINING_IN_SECTION?                         
                         RemainingTimeInSection:
                         mCurrentPlaylist.RemainingTime
                             );
                 }
             }
         }

         private double m_RecordingElapsedTime_Book = -1;
         public double RecordingTimeElapsedTotal
         {
             get
            {
                if (m_RecordingElapsedTime_Book < 0) CalculateTimeElapsed();

                return m_RecordingElapsedTime_Book +  (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition) /
                          Time.TIME_UNIT;
            }
        }

             private void CalculateTimeElapsed()
        {
                 if ( mRecordingPhrase == null ) return ;
            mView.Presentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is PhraseNode && n.Children.Count == 0)
                        {
                            m_RecordingElapsedTime_Book += ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                        }
                        if (n == mRecordingPhrase)
                            return false;
                        else
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
        }

        private double m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase = -1;
        public double RecordingTimeElapsedSection
        {
            get
            {
                if (m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase < 0) CalculateTimeElapsedInSection();

                return m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase + (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition) /
                          Time.TIME_UNIT;
            }
        }

        public double PlaybackTimeElapsedSection
        {
            get
            {
                if (m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase < 0) CalculateTimeElapsedInSection();

                return m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase + mCurrentPlaylist.CurrentTimeInAsset;
            }
        }


        private void CalculateTimeElapsedInSection()
        {
            if (CurrentState == State.Recording && mRecordingPhrase == null) return;
            if (IsPlayerActive && PlaybackPhrase == null) return;

            SectionNode section = CurrentState == State.Recording ? mRecordingPhrase.ParentAs<SectionNode>() :
                PlaybackPhrase.ParentAs<SectionNode>();
            section.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is PhraseNode && n.Children.Count == 0)
                        {
                            m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase += ((PhraseNode)n).Audio.Duration.AsTimeSpan.TotalMilliseconds;
                        }
                        if ((CurrentState == State.Recording &&  n == mRecordingPhrase)
                            || (IsPlayerActive && n == PlaybackPhrase ))
                            return false;
                        else
                            return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
        }

        // Play/Resume playback

        private void mPlayButton_Click(object sender, EventArgs e) { PlayOrResume(); }

        /// <summary>
        /// Play all in the project. (Used when nothing is selected, or from the play all menu item.)
        /// Start from the current selection, or from the first phrase.
        /// </summary>
        public void PlayAll()
            {
            try
                {
                                                                PlayAll_safe ();
                }
            catch (System.Exception ex)
                {
                    if (mCurrentPlaylist != null) mCurrentPlaylist.ForcedStopForError();
                MessageBox.Show ( string.Format ( Localizer.Message ( "TransportBar_PlayerExceptionMsg" ), "\n\n", ex.ToString () ) );
                }
            }

        private void PlayAll_safe ()
        {
                        if (CanPlay)
            {
                //mCurrentPlaylist = mMasterPlaylist;
                //PlayCurrentPlaylistFromSelection();
            PlayMaster ();
            }
            else if (CanResumePlayback)
            {
                if (mCurrentPlaylist != mMasterPlaylist) // if this is local playlist, start playing master playlist from the point where local playlist has paused
                {
                PlayMasterFromPlaylistTransition ();
                }
                else
                    mCurrentPlaylist.Resume();
            }
                    }

        

        private void PlayMaster ()
            {
            if (mPlayQAPlaylist)
                {
                CreateQAPlaylist ();
                }
            else
                {
                mCurrentPlaylist = mMasterPlaylist;
                }
            PlayCurrentPlaylistFromSelection ();
            }

        private void CreateQAPlaylist ()
            {
                                                mQAMasterPlaylist = new Playlist ( mPlayer, true);
                                    mQAMasterPlaylist.Presentation = mView.Presentation;
                        mCurrentPlaylist = mQAMasterPlaylist;
            SetPlaylistEvents (  mQAMasterPlaylist);
            }

        /// <summary>
        /// if playback is paused and current playlist is not master playlist, starts playing master playlist from paused position
                /// </summary>
        private void PlayMasterFromPlaylistTransition ()
            {
            if (mState == State.Paused && mCurrentPlaylist != null)
                {
                PhraseNode transitionPhrase = mCurrentPlaylist.CurrentPhrase;
                double transitionTime = mCurrentPlaylist.CurrentTimeInAsset;
                if (mCurrentPlaylist is PreviewPlaylist) transitionTime = ((PreviewPlaylist)mCurrentPlaylist).RevertTime;

                Stop ();

                if (mPlayQAPlaylist)
                    {
                    CreateQAPlaylist ();
                    }
                else
                    {
                    mCurrentPlaylist = mMasterPlaylist;
                    }
                PlayCurrentPlaylistFrom ( transitionPhrase, transitionTime );
                }
            }



        /// <summary>
        /// All-purpose play function for the play button.
        /// Play or resume if possible, otherwise do nothing.
        /// If there is a selection, play the selection; if there is no selection, play everything
        /// (depending on the relevent preference setting.)
        /// </summary>
        public void PlayOrResume()
            {
            try
                {
                                                PlayOrResume_Safe ();
                }
            catch ( System.Exception ex )
                {
                    if (mCurrentPlaylist != null) mCurrentPlaylist.ForcedStopForError();
                MessageBox.Show ( string.Format ( Localizer.Message ( "TransportBar_PlayerExceptionMsg" ), "\n\n", ex.ToString () ) );
                }
            }

        private void PlayOrResume_Safe ()
        {
                if (CanPlay || CanPreviewAudioSelection)
            {
                    // for play selection stop existing playlist if active
            if (mState == State.Paused && CanPreviewAudioSelection) Stop ();

                PlayOrResume(mView.Selection == null ? null : mView.Selection.Node);
            }
            else if (CanResumePlayback)
            {
                if (mCurrentPlaylist == mMasterPlaylist || mCurrentPlaylist is PreviewPlaylist)
                {
                    // if this is master playlist or preview playlist, start playing local playlist from the point where master or preview playlist is paused
                                    PlayLocalPlaylistFromPlaylistTransition ();
                }
                else
                {
                    mCurrentPlaylist.Resume();
                }
            }
        }


        /// <summary>
        /// Play a single node (phrase or section), or everything if the node is null
        /// (and the mPlayIfNoSelection flag is set.)
        /// </summary>
        public void PlayOrResume(ObiNode node)
        {
            if (node == null && PlayIfNoSelection)
            {
                PlayAll();
            }
            else if (node != null)
            {
                if (!node.IsRooted) return;
                mLocalPlaylist = new Playlist(mPlayer, mView.Selection , mPlayQAPlaylist);
                SetPlaylistEvents(mLocalPlaylist);
                mCurrentPlaylist = mLocalPlaylist;
                PlayCurrentPlaylistFromSelection();
            }
        }

        /// <summary>
        /// If playback is paused and current playlist is not local playlist, Plays local playlist from paused position
                /// </summary>
        private void PlayLocalPlaylistFromPlaylistTransition ()
            {
            if (mState == State.Paused && mCurrentPlaylist != null)
                {
                PhraseNode transitionPhrase = mCurrentPlaylist.CurrentPhrase;
                double transitionTime = mCurrentPlaylist.CurrentTimeInAsset;
                //if (mCurrentPlaylist is PreviewPlaylist && !(mView.Selection is AudioSelection) ) transitionTime = ((PreviewPlaylist)mCurrentPlaylist).RevertTime;
                if (mCurrentPlaylist is PreviewPlaylist)
                {
                    if (mView.Selection is AudioSelection)
                    {
                        transitionTime = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                    }
                    else
                    {
                        transitionTime = ((PreviewPlaylist)mCurrentPlaylist).RevertTime;
                    }
                    Console.WriteLine("TransportBar: " + "transition in process " + transitionTime);
                }
                Stop ();
                
                mLocalPlaylist = new Playlist ( mPlayer, transitionPhrase , mPlayQAPlaylist );
                SetPlaylistEvents ( mLocalPlaylist );
                mCurrentPlaylist = mLocalPlaylist;
                PlayCurrentPlaylistFrom ( transitionPhrase , transitionTime );
                }

            }


        // Find the node to start playback from.
        private PhraseNode FindPlaybackStartNode(ObiNode node)
        {
            ObiNode n;
            // start from this node (or the first leaf for a section)
            // and go through every node to find the first one in the playlist.
            for (n = node is SectionNode ? node.FirstLeaf : node;
                n != null && !mCurrentPlaylist.ContainsPhrase(n as PhraseNode);
                n = n.FollowingNode) { }
            return n as PhraseNode;
        }

        // Play the current playlist from the current selection.
        private void PlayCurrentPlaylistFromSelection()
        {
            if (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange != null )
            {
                if (!((AudioSelection)mView.Selection).AudioRange.HasCursor && mCurrentPlaylist != mMasterPlaylist)
                {
                    // Play the audio selection (only for local playlist; play all ignores the end of the selection.)
                    mCurrentPlaylist.CurrentPhrase = (PhraseNode)mView.Selection.Node;
                    if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing)  mCurrentPlaylist.Play(((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime,
                        ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime);
                }
                else 
                {
                    mCurrentPlaylist.CurrentPhrase = FindPlaybackStartNode(mView.Selection.Node);
                    if (mCurrentPlaylist.CurrentPhrase == mView.Selection.Node)
                    {
                    // The selected node is in the playlist so play from the cursor
                    if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing)
                        {
                        // First disable scrol to avoid jumping scrolling of screen on starting playback @AudioScrol
                        mView.DisableScrollingInContentsView ();
                        mCurrentPlaylist.Play ( ((AudioSelection)mView.Selection).AudioRange.CursorTime );
                        }
                    }
                    else
                    {
                    if (mAfterPreviewRestoreTime > 0)
                        {
                        if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play ( mAfterPreviewRestoreTime );
                        mAfterPreviewRestoreTime = 0;
                        }
                    else
                        {
                        // The selected node is not in the playlist so play from the beginning
                        if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play ();
                        }
                    }
                }
            }
            else if (mView.Selection is StripIndexSelection)
            {
                // Play from the first phrase in the playlist following the strip cursor,
                // or the beginning of the strip.
                StripIndexSelection s = (StripIndexSelection)mView.Selection;
                mCurrentPlaylist.CurrentPhrase = FindPlaybackStartNode(s.Index < s.Section.PhraseChildCount ?
                    (ObiNode)s.Section.PhraseChild(s.Index) : (ObiNode)s.Section);
                if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play ();
            }
            else if (mView.Selection is NodeSelection)
            {
                mCurrentPlaylist.CurrentPhrase = FindPlaybackStartNode(mView.Selection.Node);
                if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play();
            }
            else
            {
                if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play();
            }
        }

        /// <summary>
        ///  Plays current playlist from given phrase and given time if this given phrase lie in current playlist
                /// </summary>
        /// <param name="startNode"></param>
        /// <param name="time"></param>
        private  void    PlayCurrentPlaylistFrom ( PhraseNode startNode , double time )
        {
        if (mCurrentPlaylist != null && (mState == State.Stopped || mState == State.Paused))
            {
            if (startNode != null &&  mCurrentPlaylist.ContainsPhrase ( startNode ))
                {
                mCurrentPlaylist.CurrentPhrase = startNode;
                if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play ( time );
                }
            else
                {
                PlayCurrentPlaylistFromSelection ();
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
            if (m_PreviewBeforeRecordingWorker != null && m_PreviewBeforeRecordingWorker.IsBusy) return ;

            if (CanPause)
            {
                            if (m_IsPreviewing)
                {
                                    mAfterPreviewRestoreTime = mCurrentPlaylist.CurrentTimeInAsset;
                                        //Stop();
                                    }

                mDisplayTimer.Stop();
                if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording
                    || mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring)
                {
                    PauseRecording();                    
                }
                else if (mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Playing)
                {
                                    mCurrentPlaylist.Pause();
                                    MoveSelectionToPlaybackPhrase ();
                }
                UpdateButtons();
            }
        }

        // Pause recording
        private void PauseRecording()
        {
            bool wasMonitoring = mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring;
        mVUMeterPanel.BeepEnable = false;

        try
            {
            mRecordingSession.Stop ();
            }
        catch (System.Exception ex)
            {
            MessageBox.Show ( ex.ToString () );
            }

            for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
            {
                mView.Presentation.UpdateAudioForPhrase(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i),
                    mRecordingSession.RecordedAudio[i]);
                if (!mRecordingSection.Used) mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i).Used = false;
            }
            //Workaround to force phrases to show if they become invisible on stopping recording
            mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
            mResumeRecordingPhrase = (PhraseNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1);
            if(!wasMonitoring &&  mResumeRecordingPhrase != null )  mView.SelectFromTransportBar ( mResumeRecordingPhrase, null );
            mRecordingSession = null;
            UpdateTimeDisplay();

            // optionally save project
            //SaveWhenRecordingEnds ();//@singleSection

            // makes phrase blocks invisible if these exceed max. visible blocks count during recording
            //mView.MakeOldStripsBlocksInvisible ( true); // @phraseLimit :@singleSection: legagy code commented
        }


        /// <summary>
        /// move selection to current phrase in playlist
                /// </summary>
        public void MoveSelectionToPlaybackPhrase ()
            {
            if (!CanMoveSelectionToPlaybackPhrase) return;//@singleSection
            if ( IsPlayerActive )
                {
                bool SelectionChangedPlaybackStatus = mSelectionChangedPlayEnable;
                mSelectionChangedPlayEnable = false;
                if (mView.Selection == null )
                    mView.SelectFromTransportBar ( mCurrentPlaylist.CurrentPhrase, null );
                else if (mCurrentPlaylist != null &&
                    (mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Paused)
                    && mView.Selection.Node != mCurrentPlaylist.CurrentPhrase)
                    {
                    mView.SelectFromTransportBar ( mCurrentPlaylist.CurrentPhrase, mView.Selection.Control );
                    }
                mSelectionChangedPlayEnable = SelectionChangedPlaybackStatus;
                }

            }


        // Stop

        private void mStopButton_Click(object sender, EventArgs e) { Stop(); }

        /// <summary>
        /// The stop button. Stopping twice deselects all.
        /// </summary>
        public void Stop()
        {
            if (CanStop)
            {
                if ((IsRecorderActive || CanResumeRecording) && !IsPlayerActive )
                {
                    StopRecording();
                }
                else
                {
                    // Stopping again deselects everything
                    if (mState == State.Stopped)
                    {
                    mView.SetPlaybackPhraseAndTime ( null, 0.0 );
                        mView.Selection = null;
                    }
                    else
                    {
                    StopPlaylistPlayback ();
                    }
                }
            }
        }

        private void StopPlaylistPlayback ()
            {
            if (mCurrentPlaylist != null && mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Stopped)
                {
                bool PlaybackOnSelectionStatus = SelectionChangedPlaybackEnabled;
                SelectionChangedPlaybackEnabled = false;
                mCurrentPlaylist.Stop ();
                mView.SetPlaybackPhraseAndTime ( null, 0.0 );
                SelectionChangedPlaybackEnabled = PlaybackOnSelectionStatus;
                }
            }



        // Record

        private void mRecordButton_Click(object sender, EventArgs e) { Record_Button(); }

        /// <summary>
        /// allows direct recording when record directly preferences is checked else it goes through monitoring first
        /// </summary>
        public void Record_Button()
        {
            if (mView.ObiForm.Settings.RecordDirectlyWithRecordButton && CurrentState != State.Monitoring) //if monitoring go through the traditional way
            {
                StartRecordingDirectly();
            }
            else
            {
                Record();
            }
        }

        /// <summary>
        /// Start monitoring (if stopped) or recording (if monitoring)
        /// </summary>
        public void Record()
        {
            if (mView.Selection is TextSelection || IsMetadataSelected)
                return;

            if (mView.Presentation != null&& mState != State.Playing
                        &&    !IsMetadataSelected && ( mView.Selection == null || !(mView.Selection is TextSelection)))

                        if (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing) Pause();
            {
            try
                {
                if (mState == State.Monitoring)
                    {
                    mRecordingSession.Stop ();
                    StartRecording ();
                    }
                else if (CanResumeRecording)
                    {
                    SetupRecording ( Recording );
                    }
                else if (!IsRecorderActive)
                    {
                    SetupRecording ( Monitoring );
                    }
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString () , Localizer.Message ( "Caption_Error" ) );
                if (mState == State.Monitoring || mState == State.Recording ) Stop ();
                }
            } // presentation check ends
        }


        // Parameters for StartRecordingOrMonitoring
        private static readonly bool Recording = true;
        private static readonly bool Monitoring = false;

        private void SetupRecording(bool recording) { SetupRecording(recording, null); }

        // Setup recording and start recording or monitoring
        private void SetupRecording(bool recording, SectionNode afterSection)
        {
            
        if (mRecorder != null && mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
                        {
            urakawa.command.CompositeCommand command = CreateRecordingCommand ();

            // assign selection to null if metadata is selected.
            // : this may be removed now as recording is skipped if metadata is selected
            if (mView.Selection != null && mView.Selection is MetadataSelection)
                mView.Selection = null;

            // warning message while resuming recording
            if ((mResumeRecordingPhrase != null && mResumeRecordingPhrase.IsRooted) &&
                mView.Selection != null && mView.Selection.Node != mResumeRecordingPhrase &&
                MessageBox.Show ( Localizer.Message ( "recording_resume_check" ),
                    Localizer.Message ( "recording_resume_check_caption" ),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question ) == DialogResult.No)
                {
                mResumeRecordingPhrase = null;
                mRecordingPhrase = null;
                }
// if recording phrase is not rooted, make it to null
            // doing separately to reduce complexity of above if block
            if (mResumeRecordingPhrase != null && !mResumeRecordingPhrase.IsRooted)
                {
                mResumeRecordingPhrase = null;
                mRecordingPhrase = null;
                MessageBox.Show ( Localizer.Message ( "RecordingResumePhrasesDeleted" ), Localizer.Message ( "Caption_Information" ), MessageBoxButtons.OK, MessageBoxIcon.Information );
                                }

            //if selection is in TOC view save it also
                                if (mView.Selection != null && mView.Selection.Control is TOCView)
                                {
                                    command.ChildCommands.Insert(command.ChildCommands.Count , new Commands.UpdateSelection(mView, new NodeSelection(mView.Selection.Node , mView.Selection.Control)));
                                }
            //@singleSection: if phrases till recording phrases are hidden, remove existing phrases to enable content view start from phrases near to recording phrase
            mView.RecreateContentsWhileInitializingRecording ( mResumeRecordingPhrase);
            
            // save the selection before starting recording
            ObiNode selectionNode = mResumeRecordingPhrase != null ? mResumeRecordingPhrase :
                mView.GetSelectedPhraseSection != null ? (mView.Selection is StripIndexSelection && ( (StripIndexSelection)mView.Selection).EmptyNodeForSelection != null ? ((StripIndexSelection)mView.Selection).EmptyNodeForSelection :mView.Selection.Node ): null;
            if (selectionNode != null && mView.GetSelectedPhraseSection != null)
                {
                if (mResumeRecordingPhrase != null && mResumeRecordingPhrase == selectionNode && !(mView.Selection.Control is ContentView))
                    {
                    mView.SelectPhraseInContentView ( mResumeRecordingPhrase );
                    }
                    
                    if (mView.ObiForm.Settings.Recording_ReplaceAfterCursor
                        &&    ( (CurrentState == State.Paused  && mView.Selection.Node is EmptyNode)    ||    (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor )))
                    {
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(mView, new NodeSelection(selectionNode, mView.Selection.Control)));
                        //MessageBox.Show("recording selection update");   
                        double replaceStartTime = (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor )? ((AudioSelection)mView.Selection).AudioRange.CursorTime :
                            CurrentPlaylist.CurrentTimeInAsset;
                        mView.Selection = new AudioSelection((PhraseNode) selectionNode, mView.Selection.Control, new AudioRange(replaceStartTime, selectionNode.Duration) );
                        
                    }
                    else
                    {
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(mView, new NodeSelection(selectionNode, mView.Selection.Control)));
                    }
                }
                else if (selectionNode == null && mView.GetSelectedPhraseSection == null)//also saving null selection
                {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(mView, null));
                }
                if (mResumeRecordingPhrase == null) mRecordingPhrase = null;

            ObiNode node = GetRecordingNode ( command, afterSection );
            InitRecordingSectionAndPhraseIndex ( node, mView.ObiForm.Settings.AllowOverwrite, command );
            if (mView.Selection == null && node is SectionNode) mView.SelectFromTransportBar(node, null);// if nothing is selected, new section is created, select it in content view
            // Set events
            mRecordingSession = new RecordingSession ( mView.Presentation, mRecorder );
            mRecordingSession.StartingPhrase += new Obi.Events.Audio.Recorder.StartingPhraseHandler (
                delegate ( object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e )
                    {
                    RecordingPhraseStarted ( e, command, (EmptyNode)(node.GetType () == typeof ( EmptyNode ) ? node : null) );
                    } );
            mRecordingSession.FinishingPhrase += new Obi.Events.Audio.Recorder.FinishingPhraseHandler (
                delegate ( object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e ) { RecordingPhraseEnded ( e ); } );
            mRecordingSession.FinishingPage += new Events.Audio.Recorder.FinishingPageHandler (
                delegate ( object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e ) { RecordingPage ( e ); } );

            // resume recording should be null to indicate that recoreding is in process and cannot be resumed.
            mResumeRecordingPhrase = null;

            // Actually start monitoring or recording
            if (recording)
                {
                StartRecording ();
                }
            else
                {
                                     mRecordingSession.StartMonitoring ();
                                    
                if (mView.ObiForm.Settings.AudioClues) mVUMeterPanel.BeepEnable = true;
                }
            }
        }

        // Create a new recording command.
        private CompositeCommand CreateRecordingCommand()
        {
            urakawa.command.CompositeCommand command = mView.Presentation.CommandFactory.CreateCompositeCommand();
            command.ShortDescription = (Localizer.Message("recording_command"));//sdk2
            return command;
        }

        // member variables to transfer role of recording phrase in case of 2 point split, not a good way but it is safest at this last moment
        private bool m_IsAfterRecordingSplitTransferEnabled;
                private EmptyNode m_TempNodeForPropertiesTransfer = null;

        // Initialize recording section/phrase index depending on the
        // context node for recording and the settings.
        private void InitRecordingSectionAndPhraseIndex(ObiNode node, bool overwrite, urakawa.command.CompositeCommand  command)
        {
        m_IsAfterRecordingSplitTransferEnabled = false;
        m_TempNodeForPropertiesTransfer = null;

            if (node is SectionNode)
            {
                // Record at the end of the section, or after the cursor
                // in case of a cursor selection in the section.
                mRecordingSection = (SectionNode)node;
                mRecordingInitPhraseIndex = mView.Selection is StripIndexSelection ?
                    ((StripIndexSelection)mView.Selection).Index : mRecordingSection.PhraseChildCount;
            }
            else if (node is PhraseNode)
            {
                // Record in or after the phrase node, depending on overwrite settings.
                mRecordingSection = node.AncestorAs<SectionNode>();
                mRecordingInitPhraseIndex = 1 + node.Index;
                if (overwrite && (mState == State.Paused ||
                    mView.Selection is AudioSelection ))
                {
                    //MessageBox.Show(SplitBeginTime.ToString () + " , selection time"+ ((mView.Selection != null && mView.Selection is AudioSelection)? ((AudioSelection)mView.Selection).AudioRange.CursorTime.ToString () : ""  ));
                    // TODO: we cannot record from pause at the moment; maybe that's not so bad actually.
                CompositeCommand SplitCommand = Commands.Node.SplitAudio.GetSplitCommand ( mView );
                                    if ( SplitCommand != null )  command.ChildCommands.Insert(command.ChildCommands.Count, SplitCommand);

                                    if (mView.Selection is AudioSelection && !((AudioSelection)mView.Selection).AudioRange.HasCursor && SplitCommand != null)
                                        {
                                        command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.DeleteWithOffset ( mView, node, 1 ) );
                                        m_IsAfterRecordingSplitTransferEnabled = true;
                                                                                CopyPropertiesForTransfer ( (EmptyNode)node );
                                        }
                }
            }
            else if (node is EmptyNode)
            {
                // Record inside the empty node
                mRecordingSection = node.AncestorAs<SectionNode>();
                mRecordingInitPhraseIndex = node.Index;
            }
        if (IsPlayerActive) StopPlaylistPlayback (); // stop if split recording starts while playback is paused

        }

        // Start recording a phrase, possibly replacing an empty node (only for the first one.)
        private void RecordingPhraseStarted(Obi.Events.Audio.Recorder.PhraseEventArgs e,
            urakawa.command.CompositeCommand command, EmptyNode emptyNode)
        {
            // Suspend presentation change handler so that we don't stop when new nodes are added.
            mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
            mView.Presentation.UsedStatusChanged -= new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
            PhraseNode phrase = mView.Presentation.CreatePhraseNode(e.Audio);
            mRecordingPhrase = phrase;
            Commands.Node.AddNode add = new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                mRecordingInitPhraseIndex + e.PhraseIndex);
            add.SetDescriptions(command.ShortDescription);

            // transfer properties if 2 point split is being performed
            if (m_IsAfterRecordingSplitTransferEnabled && m_TempNodeForPropertiesTransfer != null )
                {
                m_IsAfterRecordingSplitTransferEnabled = false;
                 CopyPropertiesToRecordingNode ( (EmptyNode) phrase );
                                }

            //add.UpdateSelection = false;
            if (e.PhraseIndex == 0)
            {
                if (emptyNode != null && e.PhraseIndex == 0)
                {
                    phrase.CopyAttributes(emptyNode);
                    phrase.Used = emptyNode.Used;
                    Commands.UpdateSelection updateSelection = new Commands.UpdateSelection(mView,new NodeSelection (emptyNode, mView.Selection.Control));
                    updateSelection.RefreshSelectionForUnexecute = true;
                    command.ChildCommands.Insert(command.ChildCommands.Count, updateSelection);
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(mView, emptyNode));
                    command.ChildCommands.Insert(command.ChildCommands.Count, add);
                }
                else
                    command.ChildCommands.Insert(command.ChildCommands.Count, add);

                mView.Presentation.UndoRedoManager.Execute(command);
            }
            else
            {
                mView.Presentation.UndoRedoManager.Execute(add);
            }
                mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
                mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
            if (mRecordingPhrase != null &&  mView.Selection != null && mView.Selection.Control.GetType() == typeof(ContentView) && !this.ContainsFocus)
                mView.Selection = new NodeSelection(mRecordingPhrase, mView.Selection.Control);
        }


        private void CopyPropertiesForTransfer ( EmptyNode node )
            {
            mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
            mView.Presentation.UsedStatusChanged -= new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
            m_TempNodeForPropertiesTransfer = mView.Presentation.TreeNodeFactory.Create<EmptyNode>();
            m_TempNodeForPropertiesTransfer.CopyAttributes ( (EmptyNode)node );
            m_TempNodeForPropertiesTransfer.Used = ((EmptyNode)node).Used;
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode> ( Presentation_UsedStatusChanged );
            mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
            }


        private void CopyPropertiesToRecordingNode ( EmptyNode recordingNode )
            {//1
            if (m_TempNodeForPropertiesTransfer != null)
                {//2
                try
                    {//3
                    if (m_TempNodeForPropertiesTransfer.Role_ == EmptyNode.Role.Page)
                        {//4
                        recordingNode.TODO = m_TempNodeForPropertiesTransfer.TODO;
                        recordingNode.Used = m_TempNodeForPropertiesTransfer.Used;
                        recordingNode.Role_ = m_TempNodeForPropertiesTransfer.Role_;
                        recordingNode.PageNumber = m_TempNodeForPropertiesTransfer.PageNumber.Clone ();
                        }//-4
                    else
                        {//4
                        recordingNode.CopyAttributes ( m_TempNodeForPropertiesTransfer );
                        recordingNode.Used = m_TempNodeForPropertiesTransfer.Used;
                        }//-4
                    m_TempNodeForPropertiesTransfer = null;
                    }//-3
                catch (System.Exception)
                    {//3
                    m_TempNodeForPropertiesTransfer = null;
                    }//-3
                }//-2

            }


        // Stop recording a phrase
        private void RecordingPhraseEnded(Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(e.PhraseIndex + mRecordingInitPhraseIndex);
            phrase.SignalAudioChanged(this, e.Audio);
            mRecordingPhrase = null;
        }

        // Start recording a new page, set the right page number
        private void RecordingPage(Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(e.PhraseIndex + mRecordingInitPhraseIndex + 1);
            phrase.PageNumber = mView.Presentation.PageNumberFollowing(phrase);
        }

        // Get a node to record in. If we are resuming, this is the node to resume from;
        // otherwise the selected node (section or phrase) for node selection, audio selection
        // or strip cursor selection. If there is no node, add to the recording command a
        // command to create a new section to record in.
        public ObiNode GetRecordingNode(urakawa.command.CompositeCommand command, SectionNode afterSection)
        {
            ObiNode node = afterSection != null ? null :
                (mResumeRecordingPhrase == null || !mResumeRecordingPhrase.IsRooted) ?
                mState == State.Paused && !(mView.Selection is AudioSelection && !((AudioSelection)mView.Selection).AudioRange.HasCursor) ? mCurrentPlaylist.CurrentPhrase :
                mView.Selection is NodeSelection || mView.Selection is AudioSelection || mView.Selection is StripIndexSelection ?
                    mView.Selection.Node : null :
                mResumeRecordingPhrase;
            if (node == null)
            {
            
                SectionNode section = mView.Presentation.CreateSectionNode();
                urakawa.command.Command add = null;
                if (afterSection == null)
                {
                    add = new Commands.Node.AddNode(mView, section, (ObiNode)mView.Presentation.RootNode,
                        ((ObiRootNode)mView.Presentation.RootNode).SectionChildCount);
                    ((Commands.Node.AddNode)add).UpdateSelection = false;
                }
                else
                {
                    Commands.Node.AddNode addSection =
                        new Commands.Node.AddNode(mView, section, afterSection.ParentAs<ObiNode>(), afterSection.Index + 1);
                    addSection.UpdateSelection = false;
                    add = mView.Presentation.CreateCompositeCommand(addSection.ShortDescription);
                    ((CompositeCommand)add).ChildCommands.Insert(((CompositeCommand)add).ChildCommands.Count, addSection);
                    for (int i = afterSection.SectionChildCount - 1; i >= 0; --i)
                    {
                        SectionNode child = afterSection.SectionChild(i);
                        Commands.Node.Delete delete = new Commands.Node.Delete(mView, child);
                        delete.UpdateSelection = false;
                        ((CompositeCommand)add).ChildCommands.Insert(((CompositeCommand)add).ChildCommands.Count, delete);
                        Commands.Node.AddNode readd = new Commands.Node.AddNode(mView, child, section, 0);
                        readd.UpdateSelection = false;
                        ((CompositeCommand)add).ChildCommands.Insert(((CompositeCommand)add).ChildCommands.Count,  readd);
                    }
                }
                command.ChildCommands.Insert(command.ChildCommands.Count, add);
                node = section;
            }
            return node;
        }

        // Start recording
        void StartRecording()
        {
        mVUMeterPanel.BeepEnable = false;
            try
            {
                mRecordingSession.Record();
                mDisplayTimer.Start ();
            }
            catch (System.Exception ex)
            {
            MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString (), Localizer.Message ( "Caption_Error" ) );
            if (mState == State.Monitoring || mState == State.Recording) Stop ();
            }
                    }


        // Navigation

        private void mPrevSectionButton_Click(object sender, EventArgs e) { PrevSection(); }
        private void mPreviousPageButton_Click(object sender, EventArgs e) { PrevPage(); }
        private void mPrevPhraseButton_Click(object sender, EventArgs e) { PrevPhrase(); }
        private void mNextPhrase_Click(object sender, EventArgs e) { NextPhrase(); }
        private void mNextPageButton_Click(object sender, EventArgs e) { NextPage(); }
        private void mNextSectionButton_Click(object sender, EventArgs e) { NextSection(); }

        /// <summary>
        /// Move to the previous section (i.e. first phrase of the previous section.)
        /// </summary>
        public bool PrevSection()
        {
            if (CanNavigatePrevSection)
            {
                if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToPreviousSection();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.PrevSection(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }
                else
                {
                    mCurrentPlaylist.NavigateToPreviousSection();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to or play the previous page.
        /// </summary>
        public bool PrevPage()
        {
            if (CanNavigatePrevPage)
            {
                if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToPreviousPage();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.PrevPage(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }
                else
                {
                    mCurrentPlaylist.NavigateToPreviousPage();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to or play the previous phrase.
        /// </summary>
        public bool PrevPhrase()
        {
            if (CanNavigatePrevPhrase)
            {
                if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToPreviousPhrase();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.PrevPhrase(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }
                else
                {
                    mCurrentPlaylist.NavigateToPreviousPhrase();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Go to the next phrase.
        /// If recording, create a new phrase to record in.
        /// If playing or paused,
        /// </summary>
        public bool NextPhrase()
        {
            if (CanNavigateNextPhrase)
            {
                if (mState == State.Recording)
                {
                    //mRecordingSession.AudioRecorder.TimeOfAsset
                    double timeOfAssetMilliseconds =
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecordingSession.AudioRecorder.CurrentDurationBytePosition) /
                   Time.TIME_UNIT;

                    if (mRecordingPhrase != null && mRecordingSession != null
                        && timeOfAssetMilliseconds < 250) return false;
                    // record into to next phrase.
                    // check if phrase count of section is less than max limit
                    if ( mRecordingSection != null && mRecordingSection.PhraseChildCount < mView.MaxVisibleBlocksCount ) // @phraseLimit
                    mRecordingSession.NextPhrase();
                }
                else if (mState == State.Monitoring)
                {
                    // start recording
                    mRecordingSession.Stop();
                    
                    mVUMeterPanel.BeepEnable = false;
                    try
                    {
                        mRecordingSession.Record();
                        mDisplayTimer.Start();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToNextPhrase();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.NextPhrase(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }
                else
                {
                    mCurrentPlaylist.NavigateToNextPhrase();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Go to the next page.
        /// </summary>
        public bool NextPage()
        {
            if (CanNavigateNextPage)
            {
                if (mState == State.Recording)
                {
                    //mRecordingSession.AudioRecorder.TimeOfAsset
                    double timeOfAssetMilliseconds =
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecordingSession.AudioRecorder.CurrentDurationBytePosition) /
                   Time.TIME_UNIT;

                    if (mRecordingPhrase != null && mRecordingSession != null
                        && timeOfAssetMilliseconds < 250) return false;
                    // check if phrase limit for section is not over
                    if ( mRecordingSection != null && mRecordingSection.PhraseChildCount < mView.MaxVisibleBlocksCount ) // @phraseLimit
                    mRecordingSession.MarkPage();
                }
                else if (mState == State.Monitoring)
                {
                    return false;
                }
                else if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToNextPage();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.NextPage(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }

                else if (mState != State.Monitoring)
                {
                    mCurrentPlaylist.NavigateToNextPage();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Move to the next section (i.e. the first phrase of the next section)
        /// </summary>
        public bool NextSection()
        {
            if (CanNavigateNextSection)
            {
                if (mState == State.Recording)
                {
                    //mRecordingSession.AudioRecorder.TimeOfAsset
                    double timeOfAssetMilliseconds =
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecordingSession.AudioRecorder.CurrentDurationBytePosition) /
                   Time.TIME_UNIT;

                    if (mRecordingPhrase != null && mRecordingSession != null
                        && timeOfAssetMilliseconds < 250) return false;
                    PauseRecording();
                    mResumeRecordingPhrase = null;

                    if (mRecordingSection.FollowingSection != null && mRecordingSection.FollowingSection.Duration == 0)
                    {
                        //focus to next section and start recording again
                        if ( mView.Selection != null )
                        mView.Selection = new NodeSelection(mRecordingSection.FollowingSection, mView.Selection.Control);
                        else if ( mRecordingSection != null )
                        mView.SelectFromTransportBar ( mRecordingSection , null ) ;

                    try
                        {
                        SetupRecording ( Recording );
                        }
                    catch (System.Exception ex)
                        {
                        MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString (), Localizer.Message ( "Caption_Error" ) );
                        if (mState == State.Monitoring || mState == State.Recording ) Stop ();
                        }
                    }
                    else
                    {
                        // mView.AddSection(ProjectView.WithoutRename);

                    try
                        {
                        SetupRecording ( Recording, mRecordingSection );
                        }
                    catch (System.Exception ex)
                        {
                        MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString (), Localizer.Message ( "Caption_Error" ) );
                        if (mState == State.Monitoring || mState == State.Recording) Stop ();
                        }
                    }
                }
                else if (mState == State.Monitoring)
                {
                    return false;
                }
                else if (mState == State.Stopped)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate)
                    {
                        PlayCurrentPlaylistFromSelection();
                        mCurrentPlaylist.NavigateToNextSection();
                    }
                    else
                    {
                        mView.SelectPhraseInContentView(mCurrentPlaylist.NextSection(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node)));
                    }
                }
                else
                {
                    mCurrentPlaylist.NavigateToNextSection();
                }
                return true;
            }
            return false;
        }


        // Rewind/Fast forward

        private void mRewindButton_Click(object sender, EventArgs e) { Rewind(); }
        private void mFastForwardButton_Click(object sender, EventArgs e) { FastForward(); }

        /// <summary>
        /// Play faster.
        /// </summary>
        public void FastForward()
        {
            if (CanFastForward)
            {
                if (mState == State.Stopped || mState == State.Paused) PlayOrResume();
                if (mCurrentPlaylist.CurrentPhrase != null) mCurrentPlaylist.FastForward(); // explicit care required for zero phrase playlist to prevent setting of fwd/rwd rate without playback

                if (mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Playing)
                    mDisplayTimer.Start ();
            }
        }

        /// <summary>
        /// Rewind (i.e. play faster backward)
        /// </summary>
        public void Rewind()
        {
            if (CanRewind)
            {
                if (mState == State.Stopped || mState == State.Paused) PlayOrResume();
                if (mCurrentPlaylist.CurrentPhrase != null) mCurrentPlaylist.Rewind(); // explicit care required for zero phrase playlist because it will set the fwd/rwd rate

                if (mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Playing)
                    mDisplayTimer.Start ();
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
            double elapseBackInterval = mView.ObiForm.Settings.ElapseBackTimeInMilliseconds;
            // work around to handle special nudge condition of preview: this should be implemented universally after 2.0 release
            if (mCurrentPlaylist != null && mView.Selection is AudioSelection && mCurrentPlaylist is PreviewPlaylist && CurrentState == State.Paused) Stop();
            if (IsPlayerActive)
            {
                mCurrentPlaylist.FastPlayNormaliseWithLapseBack(elapseBackInterval);
                mFastPlayRateCombobox.SelectedIndex = 0;
                UpdateTimeDisplay();
                if (CurrentPlaylist != null) mView.UpdateCursorPosition(mCurrentPlaylist.CurrentTimeInAsset);
                return true;
            }
            else if (CurrentState == State.Stopped &&  mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                if (mView.Selection is AudioSelection)
                {
                    double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                    time = time - elapseBackInterval >= 0 ? time - elapseBackInterval : 0;
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                        new AudioRange(time));
                }
                else
                {
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                        new AudioRange(((PhraseNode)mView.Selection.EmptyNodeForSelection).Duration - elapseBackInterval));
                }
                return true;
            }
            return false;
        }


        // nudge selection
        public static readonly bool Forward = true;         // nudge forward
        public static readonly bool Backward = false;       // nudge backward

        // Nudge selection forward or backward.
        public bool Nudge(bool forward)
        {
        bool PlaybackOnSelectionEnabledStatus = SelectionChangedPlaybackEnabled;
        SelectionChangedPlaybackEnabled = false;
            double nudge = mView.ObiForm.Settings.NudgeTimeMs * (forward ? 1 : -1);
            
            if (!IsRecorderActive && (mState == State.Paused || m_IsPreviewing))
            {
                                double time = mCurrentPlaylist.CurrentTimeInAsset + nudge;
                if (m_IsPreviewing )
                    time = ((PreviewPlaylist)mCurrentPlaylist).RevertTime + nudge;

                if (time >= 0.0 && time < mCurrentPlaylist.CurrentPhrase.Duration)
                {
                    // Move selection to audio cursor, stop, and nudge the selection.
                    PhraseNode currentlyPlayingNode = mCurrentPlaylist.CurrentPhrase;
                    Stop();

                    if (mCurrentPlaylist is PreviewPlaylist)
                        ((PreviewPlaylist)mCurrentPlaylist).TriggerEndOfPreviewPlaylist ( time);

                    mView.SelectedBlockNode = currentlyPlayingNode;
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                        new AudioRange(time));

                    if (mView.ObiForm.Settings.PlayOnNavigate) Preview(true, false);
                    
                    SelectionChangedPlaybackEnabled = PlaybackOnSelectionEnabledStatus;
                    return true;
                }
            }
            else if (mState == State.Stopped)
            {
                AudioSelection s = mView.Selection as AudioSelection;
                if (s != null)
                {
                    double time = (s.AudioRange.HasCursor ? s.AudioRange.CursorTime : s.AudioRange.SelectionBeginTime) + nudge;
                    if (time >= 0.0 && time < ((PhraseNode)s.Node).Duration)
                    {
                        mView.Selection = new AudioSelection((PhraseNode)s.Node, mView.Selection.Control, new AudioRange(time));
                        SelectionChangedPlaybackEnabled = PlaybackOnSelectionEnabledStatus;
                        return true;
                    }
                }
            }
        SelectionChangedPlaybackEnabled = PlaybackOnSelectionEnabledStatus ;
            return false;
        }

        // preview playback functions
        public static readonly bool From = true;
        public static readonly bool Upto = false;
        public static readonly bool UseSelection = true;
        public static readonly bool UseAudioCursor = false;

        /// <summary>
        /// Preview from or upt the current position; use the audio cursor, the selection cursor,
        /// or the beginning position of a selection. If the useSelection flag is set, use the
        /// selection position; otherwise, use the audio cursor position (if set.)
        /// </summary>
        public bool Preview(bool from, bool useSelection)
        {
            if (!IsRecorderActive && PreviewDuration > 0)
            {
                if ((mState == State.Paused || mState == State.Playing) && !useSelection )
                {
                    

                    PhraseNode node = mCurrentPlaylist.CurrentPhrase;
                    double time = mCurrentPlaylist is PreviewPlaylist ?
                        ((PreviewPlaylist)mCurrentPlaylist).RevertTime : mCurrentPlaylist.CurrentTimeInAsset;
                    if (mState == State.Playing ||  mState == State.Paused) Stop ();
                    CreateLocalPlaylistForPreview( node , time, useSelection);
                    mCurrentPlaylist.CurrentTimeInAsset = time;
                    PlayPreview(mCurrentPlaylist.CurrentPhrase, time - (from ? 0.0 : PreviewDuration), PreviewDuration, from);
                    return true;
                }
                else if (mView.Selection is AudioSelection)
                {
                if (mState == State.Playing) Pause ();
                
                                    AudioSelection s = (AudioSelection)mView.Selection;
                                    if (mState == State.Stopped && s.AudioRange != null && !s.AudioRange.HasCursor && !useSelection) 
                                        return false ;

                                    double time = (s.AudioRange.HasCursor || !useSelection ? s.AudioRange.CursorTime : s.AudioRange.SelectionBeginTime);
                        


                    if (mState == State.Playing || mState == State.Paused ) Stop ();
                    CreateLocalPlaylistForPreview ( (PhraseNode)s.Node, time, true );

                    time = from ? (s.AudioRange.HasCursor || !useSelection ? s.AudioRange.CursorTime : s.AudioRange.SelectionBeginTime) :
                        (s.AudioRange.HasCursor ? s.AudioRange.CursorTime : s.AudioRange.SelectionEndTime) - PreviewDuration;
                    PlayPreview((PhraseNode)s.Node, time, PreviewDuration, from);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Preview the audio selection.
        /// </summary>
        public bool PreviewAudioSelection()
        {
            if (CanPreviewAudioSelection)
            {
                AudioSelection s = (AudioSelection)mView.Selection;
                
                if (mState == State.Playing  || mState == State.Paused) Stop ();
                CreateLocalPlaylistForPreview( (PhraseNode)s.Node , s.AudioRange.SelectionBeginTime, true);
                PlayPreview((PhraseNode)s.Node, s.AudioRange.SelectionBeginTime,
                    s.AudioRange.SelectionEndTime - s.AudioRange.SelectionBeginTime, true);
                return true;
            }
            return false;
        }

        private PhraseNode m_PreviewPhraseNode;
        private double mAfterPreviewRestoreTime;
        private double m_AfterPreviewRestoreEndTime;
        private bool m_IsPreviewing;


        // Preview from a given time for a given duration inside a phrase.
        private void PlayPreview(PhraseNode phrase, double from, double duration, bool forward)
        {
            urakawa.media.data.audio.AudioMediaData audioData = phrase.Audio.AudioMediaData;
            if (from < 0.0)
            {
                duration += from;
                from = 0.0;
            }
            double end = from + duration;
            if (end > audioData.AudioDuration.AsTimeSpan.TotalMilliseconds)
                end = audioData.AudioDuration.AsTimeSpan.TotalMilliseconds;

            if (from >= end) return;
            //mPlayer.PlayPreview(audioData, from, end, forward ? from : end);
            m_PreviewPhraseNode = mCurrentPlaylist.CurrentPhrase;
            m_AfterPreviewRestoreEndTime =  mAfterPreviewRestoreTime = forward ? from : end;
            
            if (mView.Selection != null && mView.Selection is AudioSelection &&   !((AudioSelection)mView.Selection).AudioRange.HasCursor)
                m_AfterPreviewRestoreEndTime = ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime;
            if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) 
                        mCurrentPlaylist.Play(from, end);
            
            m_IsPreviewing = true;
        }
        

        private void PostPreviewRestore()
        {
        bool playOnSelectionStatus = SelectionChangedPlaybackEnabled;
        SelectionChangedPlaybackEnabled = false;
                                        if (mView.Selection != null &&
                    mView.Selection.Node == m_PreviewPhraseNode)
                {
                if (mAfterPreviewRestoreTime == m_AfterPreviewRestoreEndTime)
                    {
                    mView.UpdateCursorPosition ( mAfterPreviewRestoreTime );
                    //mView.Selection = new AudioSelection(m_PreviewPhraseNode, mView.Selection.Control, new AudioRange(m_AfterPreviewRestoreTime));
                    }
                else
                    {
                                        //mView.Selection = new AudioSelection ( m_PreviewPhraseNode, mView.Selection.Control, new AudioRange ( mAfterPreviewRestoreTime, m_AfterPreviewRestoreEndTime ) );
                    }
                                                                                                                                                            }
                m_IsPreviewing = false;
                if (mState == State.Playing) Pause();

                SelectionChangedPlaybackEnabled = playOnSelectionStatus ;
                        }


        #region undoable recording



        System.ComponentModel.BackgroundWorker m_PreviewBeforeRecordingWorker = null;
        /// <summary>
        /// Start recording directly without going through listening
        /// </summary>
        public void StartRecordingDirectly()
        {
            if (mView.ObiForm.Settings.Recording_PreviewBeforeStarting && mView.ObiForm.Settings.AllowOverwrite
                && m_PreviewBeforeRecordingWorker != null && m_PreviewBeforeRecordingWorker.IsBusy)
            {
                return;
            }

            if (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing)
            {
                Pause();
                if (mView.Selection == null || !(mView.Selection.Node is EmptyNode) || mView.Selection.Node != mCurrentPlaylist.CurrentPhrase) return;
            }

            if (mView.ObiForm.Settings.Recording_PreviewBeforeStarting && mView.ObiForm.Settings.AllowOverwrite
                && ((CurrentState == State.Paused &&  !(mView.Selection is AudioSelection)) || (mView.Selection!= null && mView.Selection is AudioSelection  &&  ((AudioSelection)mView.Selection).AudioRange.HasCursor )))
            {
                
                m_PreviewBeforeRecordingWorker = new System.ComponentModel.BackgroundWorker();
                m_PreviewBeforeRecordingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    Preview(Upto, UseAudioCursor);
                    int interval = 50;
                    for (int i = 0; i < (PreviewDuration * 2) / interval; i++)
                    {
                        if (mCurrentPlaylist is PreviewPlaylist && mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Paused)
                        {
                            //System.Media.SystemSounds.Asterisk.Play();
                            Console.WriteLine(i);
                            break;
                        }
                        Thread.Sleep(interval);
                    }
                });
                m_PreviewBeforeRecordingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
                {
                    if ( CurrentState == State.Paused )  StartRecordingDirectly_Internal();
                });
                m_PreviewBeforeRecordingWorker.RunWorkerAsync();
            }
            else
            {
                StartRecordingDirectly_Internal();
            }
        }

        private void StartRecordingDirectly_Internal()
    {
            if (mRecordingSession == null
                && mCurrentPlaylist.Audioplayer.CurrentState != AudioLib.AudioPlayer.State.Playing
                &&    !IsMetadataSelected &&  ( mView.Selection == null || !(mView.Selection is TextSelection)))
                {
                try
                    {
                    SetupRecording ( Recording );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString (), Localizer.Message ( "Caption_Error" ) );
                    if (mState == State.Monitoring || mState == State.Recording) Stop ();
                    }
                
                }
        }




        // Stop recording
        private void StopRecording()
        {
            if (mRecordingSession != null && 
                (mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring ||
                mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording))
            {
                bool wasMonitoring = mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring;
                mVUMeterPanel.BeepEnable = false;

                try
                    {
                    mRecordingSession.Stop ();


                    // update phrases with audio assets
                    if (mRecordingSession.RecordedAudio != null && mRecordingSession.RecordedAudio.Count > 0)
                        {
                        for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
                            {
                            mView.Presentation.UpdateAudioForPhrase ( mRecordingSection.PhraseChild ( mRecordingInitPhraseIndex + i ),
                                mRecordingSession.RecordedAudio[i] );
                            if (!mRecordingSection.Used) mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i).Used = false;
                            }
                            //Workaround to force phrases to show if they become invisible on stopping recording
                            mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
                        }
                    EmptyNode lastRecordedPhrase = mRecordingSection.PhraseChildCount >0? mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1):null;
                    if (!wasMonitoring && lastRecordedPhrase != null && lastRecordedPhrase.IsRooted) mView.SelectFromTransportBar ( lastRecordedPhrase, null );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( Localizer.Message ("TransportBar_ErrorInStopRecording") + "\n\n" +   ex.ToString ()  , Localizer.Message ("Caption_Error"));
                    }
                
UpdateButtons();
                mRecordingSession = null;
                mResumeRecordingPhrase = null;

                
            }
        else if (mResumeRecordingPhrase != null)
            {
            mRecordingSession = null;
            mResumeRecordingPhrase = null;

            }
        }



        private bool IsRecording
        {
            get
            {
                return mRecordingSession != null &&
                    mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording;
            }
        }

        public bool IsListening
        {
            get
            {
                return mRecordingSession != null &&
                    mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring;
            }
        }

        public bool IsActive { get { return Enabled && ( IsPlayerActive || IsRecorderActive ); } }
        private bool IsPlaying { get { return mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing; } }
        public bool IsPlayerActive { get { return IsPaused || IsPlaying; } }
        private bool IsPaused { get { return mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused; } }
        public bool IsRecorderActive { get { return IsListening || IsRecording; } }
        private bool IsMetadataSelected { get { return mView.Selection != null && mView.Selection.Control is MetadataView  ; } }

        private void mToDoMarkButton_Click ( object sender, EventArgs e ) 
        {
                    MarkTodo();
        }

        /// <summary>
        /// Toggle TODO on the currently playing/recording phrase.
        /// </summary>
        public void MarkTodo()
        {
            EmptyNode node = null;
            if (IsRecording)
            {
                node = (EmptyNode)mRecordingPhrase;

                mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node));
                mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                NextPhrase();
            }
            else if (IsPlayerActive)
            {
                node = (EmptyNode)mCurrentPlaylist.CurrentPhrase;
                if (node != null)
                {
                    mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node));
                    mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                }
            }
        else if (mView.Selection != null && mView.Selection.Node is EmptyNode)
            {
            node  = (EmptyNode)mView.Selection.Node;
            mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
            mView.Presentation.UndoRedoManager.Execute ( new Commands.Node.ToggleNodeTODO ( mView, node ) );
            mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
            }
        }


        private void PlaybackOnSelectionChange_Safe ()
            {
            if (mView.Selection != null && !(mView.Selection is TextSelection)
                &&mView.Selection.Node is ObiNode)
                {
                try
                    {
                    m_PlayOnSelectionChangedMutex.WaitOne ();
                    PlaybackOnSelectionChange ();
                    m_PlayOnSelectionChangedMutex.ReleaseMutex ();
                    }
                catch (System.Exception)
                    {
                    m_PlayOnSelectionChangedMutex.ReleaseMutex ();
                    return;
                    }
                }
            }

        public bool SkipPlayOnNavigateForSection { set { m_SkipPlayOnNavigateForSection = value; } } 
        private bool m_SkipPlayOnNavigateForSection = false;
        private void PlaybackOnSelectionChange ()
        {
            if (IsRecorderActive) return;
            if (m_SkipPlayOnNavigateForSection && (mView.Selection == null || mView.Selection.Node is SectionNode))
            {
                m_SkipPlayOnNavigateForSection = false;
                return;
            }
            if (mView.Selection != null && !(mView.Selection is StripIndexSelection))
            {
                if (mState == State.Playing || mState == State.Paused)
                {
                    ObiNode node = mView.Selection.Node;
                    PhraseNode PNode = null;

                    if (node is PhraseNode)
                        PNode = (PhraseNode)node;
                    else if (node is SectionNode)
                    {
                        if (((SectionNode)node).PhraseChildCount > 0 )
                            {
                            if ( mCurrentPlaylist != mMasterPlaylist &&  ((SectionNode)node).PhraseChild(0) is PhraseNode)
                            PNode = (PhraseNode)((SectionNode)node).PhraseChild(0);
                            else // start finding required phrase node in this section
                                {
                                for (int PIndex = 0; PIndex < node.PhraseChildCount; PIndex++)
                                    {
                                    if (node.PhraseChild ( PIndex ) is PhraseNode && node.PhraseChild ( PIndex ).Used)
                                        {
                                        PNode = (PhraseNode)((SectionNode)node).PhraseChild ( PIndex );
                                        break;
                                        }
                                    } // for loop ends

                                }
                            }
                    }

                if (PNode != null && mCurrentPlaylist.ContainsPhrase ( PNode ))
                    {// 1
                    if (PNode != mCurrentPlaylist.CurrentPhrase) // if selected node is not currently playing phrase
                        { //2
                        if (mView.Selection.Control.GetType () != typeof ( TOCView )
                            || mCurrentPlaylist.CurrentPhrase.ParentAs<SectionNode> () != PNode.ParentAs<SectionNode> ()) // bypass if selection is in TOC and playing section is same as selected section
                            { //3
                            if (mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused) mCurrentPlaylist.Stop ();

                            mCurrentPlaylist.CurrentPhrase = PNode;
                            if (mView.Selection is AudioSelection) mCurrentPlaylist.CurrentTimeInAsset = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                            } //-3
                        } //-2
                    else if (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor) // clicked on the same phrase
                        mCurrentPlaylist.CurrentTimeInAsset = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                    } //-1
                else if (mCurrentPlaylist == mMasterPlaylist) // newly selected phrase is not in master paylist
                    Stop ();

                    if (mCurrentPlaylist != mMasterPlaylist
                        && !mCurrentPlaylist.ContainsPhrase(PNode))
                    {
                        if (mState == State.Playing)
                        {
                            mView.SetPlaybackPhraseAndTime(null, 0.0);
                            mCurrentPlaylist.Stop();
                            //if (mView.Selection.Node is PhraseNode)
                                //PlayOrResume();
                            if (mView.Selection.Node is SectionNode && mView.ObiForm.Settings.PlayOnNavigate)
                                PlayHeadingPhrase(mView.SelectedNodeAs<SectionNode>());
                        }
                        else
                        {
                            mView.SetPlaybackPhraseAndTime(null, 0.0);
                            mCurrentPlaylist.Stop();
                        }
                    }
                }
                if (mView.ObiForm.Settings.PlayOnNavigate
                    &&
                    (mState == State.Paused || mState == State.Stopped))
                {
                    if (mView.Selection.Node is SectionNode)
                        PlayHeadingPhrase(mView.SelectedNodeAs<SectionNode>());
                    else if (mView.Selection.Node is PhraseNode)
                        PlayOrResume();
                }
                                        }// end of selection null check

        }

        private void PlayHeadingPhrase( SectionNode node     )
        {
            if ( node != null  && node.PhraseChildCount > 0  )
            {
                EmptyNode ENode = node.PhraseChild(0);
                    
                for (int i = 0; i < node.PhraseChildCount ; i++)
                {
                    if (((EmptyNode)node.PhraseChild(i)).Role_ == EmptyNode.Role.Heading)
                    {
                        ENode = node.PhraseChild(i);
                                                break;
                    }
                                    }

                                    if (ENode is PhraseNode)
                                    {
                                        mLocalPlaylist = new Playlist(mPlayer, ENode ,mPlayQAPlaylist);
                                        SetPlaylistEvents(mLocalPlaylist);
                                        mCurrentPlaylist = mLocalPlaylist;
                                        mCurrentPlaylist.CurrentPhrase = (PhraseNode) ENode ;
                                        if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play();
                                                                                //PlayOrResume(node.PhraseChild(0));
                                    }
            }
        }


        private enum AudioCluesSelection { SelectionBegin, SelectionEnd } ;

        private void PlayAudioClue(AudioCluesSelection Clue)
        {
            if ( mView.ObiForm.Settings.AudioClues )
            {
            if (Clue == AudioCluesSelection.SelectionBegin)
            {
                string FilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SelectionBegin.wav");
                if ( System.IO.File.Exists (FilePath))
                                    new System.Media.SoundPlayer(FilePath).Play ()  ;
                                                }
            else if (Clue == AudioCluesSelection.SelectionEnd)
            {
                string FilePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SelectionEnd.wav");
                if (System.IO.File.Exists(FilePath))
                                    new System.Media.SoundPlayer(FilePath).Play();
            }
            }
                    }


        #endregion

        public bool FocusOnTimeDisplay()
        {
            return mTimeDisplayBox.Focus();
        }

        /// <summary>
        ///  Process dialog key overridden to prevent tab  from  moving focus out of transportbar
        /// </summary>
        /// <param name="KeyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (KeyData == Keys.Tab
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, true, true);
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();
                return true;
            }
            else if (KeyData == (Keys)(Keys.Shift | Keys.Tab)
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, true, true);
                if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else
            {
                return base.ProcessDialogKey(KeyData);
            }
        }

        // Create the preview playlist
        private void CreateLocalPlaylistForPreview(PhraseNode node,  double revertTime, bool useSelection)
        {
            PreviewPlaylist playlist = useSelection ? new PreviewPlaylist(mPlayer, mView.Selection, revertTime) :
                new PreviewPlaylist(mPlayer, node , revertTime);
            SetPlaylistEvents(playlist);
            mCurrentPlaylist = playlist;
        }

        /*/// <summary>//@singleSection: commented
        /// optionally  saves the project when recording ends depending on autoSave recording ends flag
                /// </summary>
        private void SaveWhenRecordingEnds ()
            {
            if ( mView.ObiForm.Settings.AutoSave_RecordingEnd || m_AutoSaveOnNextRecordingEnd )
                {
                mView.ObiForm.SaveToBackup();
                m_AutoSaveOnNextRecordingEnd = false;
                }
            }*/

        public bool CanUsePlaybackSelection { get { return Enabled && IsPlayerActive && mView.ObiForm.Settings.PlayOnNavigate; }}

        public void mView_BlocksVisibilityChanged ( object sender, EventArgs e )
            {
            UpdateButtons ();
            }

        private void mDisplayBox_DropDown(object sender, EventArgs e)
        {
            mDisplayBox.Tag = mDisplayBox.SelectedIndex;
        }


    }
}
