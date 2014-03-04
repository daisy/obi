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
        private bool IsPlaySection = false;
        private bool mPreviewBeforeRec = false;

        Bitmap m_monitorButtonImage;
        Bitmap m_recordButtonImage;
        Bitmap m_recordOptionsButtonImage;

        #region CAN WE REMOVE THIS?

        private bool mPlayQAPlaylist = false; // this should be set from UI
        private bool mSelectionChangedPlayEnable; // flag for enabling / disabling playback on change of selection
        private Mutex m_PlayOnSelectionChangedMutex ;

        private string mPrevSectionAccessibleName;   // Normal accessible name for the previous section button ???
        private string mStopButtonAccessibleName;    // Normal accessible name for the stop button ???
        private KeyboardShortcuts_Settings keyboardShortcuts=null;

        //private ContextMenuStrip m_RecordingOptionsContextMenuStrip;
        //private ToolStripMenuItem m_MoniteringtoolStripMenuItem;
        //private ToolStripMenuItem m_DeletePhrasestoolStripMenuItem;
        //private Button m_btnRecordingOptions;


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
       // private static int REMAINING_IN_SECTION = 4;
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
            m_RecordingElapsedRemainingList.Add(Localizer.Message("Elapsed"));
            m_RecordingElapsedRemainingList.Add(Localizer.Message("ElapsedInSection"));
            m_RecordingElapsedRemainingList.Add(Localizer.Message("ElapsedInProject"));
            m_PlayingElapsedRemainingList.Add(Localizer.Message("ElapsedInPhrase"));
            m_PlayingElapsedRemainingList.Add(Localizer.Message("ElapsedInSection"));
            m_PlayingElapsedRemainingList.Add(Localizer.Message("ElapsedInProject"));
            m_PlayingElapsedRemainingList.Add(Localizer.Message("RemainingInPhrase"));
          // m_PlayingElapsedRemainingList.Add("remaining in section");
            m_PlayingElapsedRemainingList.Add(Localizer.Message("RemainingInSelection"));
            mDisplayBox.Items.AddRange(m_PlayingElapsedRemainingList.ToArray ());
            mDisplayBox.SelectedIndex = 0 ;
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
        
        public bool CanPause { get { return Enabled && (mState == State.Playing || mState == State.Recording) ; } }
        public bool CanPausePlayback { get { return Enabled && mState == State.Playing; } }
        public bool CanPlay { get { return Enabled && mState == State.Stopped && !m_IsProjectEmpty && !mView.IsContentViewScrollActive; } }
        public bool CanRecord { get { return Enabled &&( mState == State.Stopped || mState == State.Paused ||  mState == State.Monitoring  ||  (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing 
            && mCurrentPlaylist.PlaybackRate == 0)) &&  mView.IsPhraseCountWithinLimit && !mView.IsContentViewScrollActive && !mView.IsZoomWaveformActive; } } // @phraseLimit
        public bool CanResumePlayback { get { return Enabled && mState == State.Paused   &&   !mView.IsContentViewScrollActive; } }
        public bool CanResumeRecording { get { return Enabled && mResumeRecordingPhrase != null && mResumeRecordingPhrase.IsRooted    &&   (mState != State.Playing  ||   (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing) )&& !mView.IsContentViewScrollActive; } }
        public bool CanRewind { get { return Enabled && (IsPlayerActive || CanPlay) ; } }
        public bool CanStop { get { return Enabled && (mState != State.Stopped || mView.Selection != null); } }


        public bool CanNavigatePrevPhrase
        {
            get
            {
                return (!m_IsProjectEmpty && IsPlayerActive && mCurrentPlaylist.CanNavigatePrevPhrase && !IsRecorderActive) || CanPlay ;
            }
        }

        public bool CanNavigateNextPhrase
        {
            get
            {
                return IsRecorderActive ||
                    (mCurrentPlaylist != null && mCurrentPlaylist.CanNavigateNextPhrase) ;
            }
        }

        
        
        public bool CanNavigatePrevPage 
        { 
            get 
            { return Enabled && !m_IsProjectEmpty && (mCurrentPlaylist != null && mCurrentPlaylist.CanNavigatePrevPage) && !IsRecorderActive; } 
        }
        public bool CanNavigateNextPage
        {
            get
            {
                return IsRecorderActive ||
                   (mCurrentPlaylist != null && mCurrentPlaylist.CanNavigateNextPage) ;
            }
        }

        public bool CanNavigatePrevSection { get { return Enabled && !m_IsProjectEmpty && (mCurrentPlaylist != null && mCurrentPlaylist.CanNavigatePrevSection) && !IsRecorderActive; } }

        public bool CanNavigateNextSection
        {
            get
            {
                return IsRecorderActive ||
                    (mCurrentPlaylist != null &&  mCurrentPlaylist.CanNavigateNextSection) ;
            }
        }

        public bool CanEnterFineNavigationMode { get { return mView.Selection != null && mView.Selection.Node is PhraseNode && (IsPlayerActive || mView.Selection is AudioSelection); } }

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
                if (IsPlayerActive && mCurrentPlaylist is PreviewPlaylist && !(mView.Selection is AudioSelection)) return ((PreviewPlaylist)mCurrentPlaylist).RevertTime;
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
        public bool MarkSelectionFromBeginningToTheCursor()
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
        public bool MarkSelectionFromCursorToTheEnd()
        {
            if ((mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing
                || mPlayer.CurrentState == AudioLib.AudioPlayer.State.Paused) &&
                mCurrentPlaylist.CurrentTimeInAsset < mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsMilliseconds)
            {
                mView.SelectedBlockNode = mCurrentPlaylist.CurrentPhrase;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(mCurrentPlaylist.CurrentTimeInAsset,
                        mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsMilliseconds));
                return true;
            }
            else if (CurrentState == State.Stopped && mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)
            {   
                double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(time ,
                        ((PhraseNode)mView.Selection.Node).Audio.Duration.AsMilliseconds));
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
                    new AudioRange(0.0, mCurrentPlaylist.CurrentPhrase.Audio.Duration.AsMilliseconds));
                return true;
            }
            else if (mState == State.Stopped && mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(0.0, ((PhraseNode)mView.Selection.Node).Audio.Duration.AsMilliseconds));
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
            if (mView.ObiForm.Settings != null)
            {
                m_PlayerTimeComboIndex = mView.ObiForm.Settings.Audio_TransportBarCounterIndex;
                m_RecorderTimeComboIndex = mView.ObiForm.Settings.Audio_TransportBarCounterIndex;
                mDisplayBox.SelectedIndex = mView.ObiForm.Settings.Audio_TransportBarCounterIndex < mDisplayBox.Items.Count ? mView.ObiForm.Settings.Audio_TransportBarCounterIndex : 0;
                ResetFastPlayForPreferencesChange();
            }
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

        public int RecordingInitPhraseIndex { get { return mRecordingInitPhraseIndex; } }

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
                    if(mView.Selection == null || mView.Selection.Node != m_FineNavigationPhrase ) FineNavigationModeForPhrase = false;

                    //@enforce single cursor
                    if (mView.ObiForm != null && mView.ObiForm.Settings.Audio_EnforceSingleCursor
                        && CurrentState == State.Paused &&  mView.Selection != null && mView.Selection is AudioSelection
                    && ((AudioSelection)mView.Selection).AudioRange.HasCursor )
                    {
                        double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime  ;
                        mView.UpdateCursorPosition (time) ;
                        mCurrentPlaylist.CurrentTimeInAsset = time;
                    }

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

        public RecordingSession RecordingSession { get { return mRecordingSession; } }

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
            // selected index should go in settings only when presentation is not null because it is assigned only when new presentation is set
            if (mView != null && mView.ObiForm.Settings != null && mView.Presentation != null) mView.ObiForm.Settings.Audio_TransportBarCounterIndex = mDisplayBox.SelectedIndex;
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
                m_ElapsedTime_Book = -1;
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
                if (mState == State.Stopped && mView != null && mCurrentPlaylist != null && !mCurrentPlaylist.CanNavigateNextPhrase) mView.UpdateCursorPosition(0.0); // audio coursor returns to 0 position if the single phrase is being played

                if (StateChanged != null) StateChanged(this, e);

                if (m_IsPreviewing && mCurrentPlaylist is PreviewPlaylist)
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
            if (FineNavigationModeForPhrase) FineNavigationModeForPhrase = false;
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
                m_ElapsedTime_Book = -1;
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
                if (IsPlaying || IsRecorderActive)
                {
                    m_btnPlayingOptions.Enabled = false;
                }
                else
                {
                    m_btnPlayingOptions.Enabled = true;
                }
                if (mView != null && mView.ObiForm != null && mView.ObiForm.Settings != null && mView.Selection != null && CurrentState!=State.Monitoring
    && mView.ObiForm.Settings.AllowOverwrite && ((CurrentState == State.Paused && !(mView.Selection is AudioSelection)) || (mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)))
                {
                    mPreviewBeforeRecToolStripMenuItem.Enabled = true;
                }
                else
                {
                    mPreviewBeforeRecToolStripMenuItem.Enabled = false;
                }
                bool recordDirectly = (mView.ObiForm  != null && mView.ObiForm.Settings.RecordDirectlyWithRecordButton) ? true : false;

                if (recordDirectly)
                {
                    m_btnRecordingOptions.Enabled = CanRecord || CanResumeRecording;                   
                    if (this.IsListening)
                    {
                        m_MonitoringtoolStripMenuItem.Visible = false;
                        m_RecordingtoolStripMenuItem.Visible = true;
                    }

                    else
                    {
                        m_MonitoringtoolStripMenuItem.Visible = true;                     
                        m_RecordingtoolStripMenuItem.Visible = false;                     

                    }                    
                    if (mView.ObiForm.Settings.AllowOverwrite)
                    {
                        m_DeletePhrasestoolStripMenuItem.Enabled = !this.IsListening;
                    }
                    else
                    {
                        m_DeletePhrasestoolStripMenuItem.Enabled = false;
                    }
                }
                else
                {
                    m_btnRecordingOptions.Enabled = false;
                }

               

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
                if (keyboardShortcuts != null)
                {
                    mRecordButton.AccessibleName = Localizer.Message(
                        (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring || (recordDirectly && CurrentState != State.Recording))
                            ? "start_recording"
                            : "start_monitoring"
                        ) + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mStartMonitoringToolStripMenuItem"].Value.ToString()) + ")";
                }
                else
                {
                    mRecordButton.AccessibleName = Localizer.Message(
                                           (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring || (recordDirectly && CurrentState != State.Recording))
                                               ? "start_recording"
                                               : "start_monitoring"
                                           );
                }
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
                 else if (mState == State.Recording && mRecordingSession.AudioRecorder.RecordingPCMFormat != null)
                 {
                     //mRecordingSession.AudioRecorder.TimeOfAsset
                     double timeOfAssetMilliseconds =
                        (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
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
                           //  mCurrentPlaylist.CurrentTime :
                           PlayingTimeElapsedTotal:
                         selectedIndex  == REMAIN_INDEX ?
                         mCurrentPlaylist.RemainingTimeInAsset: 
                       //  selectedIndex == REMAINING_IN_SECTION?                         
                       //  RemainingTimeInSection:
                         mCurrentPlaylist.RemainingTime
                             );
                 }
             }
         }

         private double m_ElapsedTime_Book = -1;
         public double RecordingTimeElapsedTotal
         {
             get
            {
                if (m_ElapsedTime_Book < 0) CalculateTimeElapsed_Book();

                return m_ElapsedTime_Book +  (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64( mRecorder.CurrentDurationBytePosition)) /
                          Time.TIME_UNIT;
            }
        }

        public double PlayingTimeElapsedTotal
        {
            get
            {
                if (m_ElapsedTime_Book < 0) CalculateTimeElapsed_Book();
                return m_ElapsedTime_Book + mCurrentPlaylist.CurrentTimeInAsset;
            }
        }


             private void CalculateTimeElapsed_Book()
        {
                 if (CurrentState == State.Recording &&   mRecordingPhrase == null ) return ;
                 if (IsPlayerActive && PlaybackPhrase == null) return;
                 m_ElapsedTime_Book = 0;

                 bool foundPhrase = false;
            mView.Presentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if ((CurrentState == State.Recording &&  n == mRecordingPhrase )
                            ||    ( IsPlayerActive && n == PlaybackPhrase)
                            || foundPhrase)
                        {
                            foundPhrase = true;
                            return false;
                        }
                        if (n is PhraseNode && n.Children.Count == 0)
                        {
                            m_ElapsedTime_Book += ((PhraseNode)n).Audio.Duration.AsMilliseconds;
                        }
                        
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

                return m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase + (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64( mRecorder.CurrentDurationBytePosition)) /
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
            
            m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase = 0;
            SectionNode section = CurrentState == State.Recording ? mRecordingPhrase.ParentAs<SectionNode>() :
                PlaybackPhrase.ParentAs<SectionNode>();

            for (int i = 0; i < section.PhraseChildCount; ++i)
            {
                EmptyNode n = section.PhraseChild(i);
                if ((CurrentState == State.Recording && n == mRecordingPhrase)
                    || (IsPlayerActive && n == PlaybackPhrase))
                {
                    return;
                }
                if (n is PhraseNode && n.Children.Count == 0)
                {
                    m_ElapsedTime_FromSectionToFirstRecordingPhraseOrPlaybackPhrase += ((PhraseNode)n).Audio.Duration.AsMilliseconds;
                }
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
            try
                {
                                                                PlayAll_safe ();
                }
            catch (System.Exception ex)
                {
                mView.WriteToLogFile(ex.ToString());
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
                mView.WriteToLogFile(ex.ToString());
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
                bool neglectSelection;
                if (IsPlaySection == false)
                {
                     neglectSelection = mView.Selection == null
                        || (node is EmptyNode && mView.Selection.Node != node);
                }
                else
                {
                    neglectSelection = true;
                    IsPlaySection = false;
                }
                
                if (neglectSelection)
                {
                    mLocalPlaylist = new Playlist(mPlayer,  node, mPlayQAPlaylist);
                }
                else
                {
                    mLocalPlaylist = new Playlist(mPlayer, mView.Selection, mPlayQAPlaylist);
                }
                SetPlaylistEvents(mLocalPlaylist);
                mCurrentPlaylist = mLocalPlaylist;
                if ( neglectSelection )
                {
                    mCurrentPlaylist.Play () ;
                }
                else
                {
                PlayCurrentPlaylistFromSelection();
                }
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
                n != null && n.IsRooted && !mCurrentPlaylist.ContainsPhrase(n as PhraseNode);
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

        EmptyNode firstRecordedPage = null;
        List<PhraseNode> listOfRecordedPhrases = new List<PhraseNode>();
        try
            {
            mRecordingSession.Stop ();
            
            // update recorded phrases with audio assets
            UpdateRecordedPhrasesAlongWithPostRecordingOperations(listOfRecordedPhrases, ref firstRecordedPage);
            
            //Workaround to force phrases to show if they become invisible on stopping recording
            mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
        }
        catch (System.Exception ex)
        {
            mView.WriteToLogFile(ex.ToString());
            MessageBox.Show(ex.ToString());
        }
            mResumeRecordingPhrase = (PhraseNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1);
            EmptyNode phraseNextToResumePhrase = null;
            if (mResumeRecordingPhrase.FollowingNode != null && mResumeRecordingPhrase.FollowingNode is EmptyNode) phraseNextToResumePhrase = (EmptyNode) mResumeRecordingPhrase.FollowingNode;

            bool playbackEnabledOnSelectionChange = SelectionChangedPlaybackEnabled;
            SelectionChangedPlaybackEnabled = false;
            try
            {
                int phraseChildCount = mRecordingSection.PhraseChildCount;
                AdditionalPostRecordingOperations(firstRecordedPage, listOfRecordedPhrases);
                if (phraseChildCount != mRecordingSection.PhraseChildCount)
                {
                    if (phraseNextToResumePhrase != null && phraseNextToResumePhrase.PrecedingNode is PhraseNode) 
                        mResumeRecordingPhrase =(PhraseNode) phraseNextToResumePhrase.PrecedingNode;
                    else if ( mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1) is PhraseNode )
                        mResumeRecordingPhrase =(PhraseNode)  mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1);

                }
            }
            catch (System.Exception ex)
            {
                mView.WriteToLogFile(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
            if (!wasMonitoring && mResumeRecordingPhrase != null) mView.SelectFromTransportBar(mResumeRecordingPhrase, null);
            SelectionChangedPlaybackEnabled = playbackEnabledOnSelectionChange;
            
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
        public bool Stop()
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
                    if (mView.IsZoomWaveformActive == false)
                    {
                        mView.Selection = null;
                    }
                    }
                    else
                    {
                    StopPlaylistPlayback ();
                    }
                }
                return true;
            }
            return false;
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
        public bool Record_Button()
        {
            if (mView.ObiForm.Settings.RecordDirectlyWithRecordButton && CurrentState != State.Monitoring) //if monitoring go through the traditional way
            {
                if (mView.ObiForm.Settings.Audio_UseRecordBtnToRecordOverSubsequentAudio)
                {
                    RecordOverSubsequentPhrases();
                }
                else
                {
                    StartRecordingDirectly();
                }
            }
            else
            {
                Record();
            }
            return true;
        }

        /// <summary>
        /// Start monitoring (if stopped) or recording (if monitoring)
        /// </summary>
        public void Record()
        {
            if (mView.Selection is TextSelection || IsMetadataSelected || mView.IsZoomWaveformActive)
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
                    SetupRecording ( Recording , false);
                    }
                else if (!IsRecorderActive)
                    {
                    SetupRecording ( Monitoring, false );
                    }
                }
            catch (System.Exception ex)
                {
                mView.WriteToLogFile(ex.ToString());
                MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString () , Localizer.Message ( "Caption_Error" ) );
                if (mState == State.Monitoring || mState == State.Recording ) Stop ();
                }
            } // presentation check ends
        }


        // Parameters for StartRecordingOrMonitoring
        private static readonly bool Recording = true;
        private static readonly bool Monitoring = false;

        private void SetupRecording(bool recording, bool deleteFollowingPhrases) { SetupRecording(recording, null, deleteFollowingPhrases); }

        // Setup recording and start recording or monitoring
        private void SetupRecording(bool recording, SectionNode afterSection, bool deleteFollowingPhrases)
        {

            if (mView.ObiForm.CheckDiskSpace() <= 100)
            {
                DialogResult result = MessageBox.Show(string.Format(Localizer.Message("LimitedDiskSpaceWarning"), 100), Localizer.Message("Memory_Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

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
                    
                    if (deleteFollowingPhrases 
                        && ( (CurrentState == State.Paused  && mView.Selection.Node is EmptyNode)    
                        ||    (mView.Selection is AudioSelection )))
                    {
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(mView, new NodeSelection(selectionNode, mView.Selection.Control)));
                        //MessageBox.Show("recording selection update");   
                        double replaceStartTime = IsPlayerActive ? CurrentPlaylist.CurrentTimeInAsset:
                            mView.Selection is AudioSelection?( ((AudioSelection)mView.Selection).AudioRange.HasCursor? ((AudioSelection)mView.Selection).AudioRange.CursorTime : ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime ): 
                            selectionNode.Duration;

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
            InitRecordingSectionAndPhraseIndex ( node, mView.ObiForm.Settings.AllowOverwrite, command , deleteFollowingPhrases);
            if (mView.Selection == null && node is SectionNode) mView.SelectFromTransportBar(node, null);// if nothing is selected, new section is created, select it in content view
            // Set events
            mRecordingSession = new RecordingSession ( mView.Presentation, mRecorder, mView.ObiForm.Settings );
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
        private void InitRecordingSectionAndPhraseIndex(ObiNode node, bool overwrite, urakawa.command.CompositeCommand  command, bool deleteFollowingPhrases)
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
                if (mView.Selection is StripIndexSelection && mView.Selection.Node != null)
                {
                    AddTheDeleteSubsequentPhrasesCommand(mRecordingSection, deleteFollowingPhrases, command);
                }
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
            // if audio after cursor has to be replaced, delete following phrases command should be used
            if ((mView.ObiForm.Settings.Recording_ReplaceAfterCursor || deleteFollowingPhrases)
                && node is EmptyNode && ((EmptyNode)node).Index < ((EmptyNode)node).ParentAs<SectionNode>().PhraseChildCount-1)
            {
                AddTheDeleteSubsequentPhrasesCommand(node, deleteFollowingPhrases, command);
            }
        if (IsPlayerActive) StopPlaylistPlayback (); // stop if split recording starts while playback is paused

        }

        private void AddTheDeleteSubsequentPhrasesCommand(ObiNode node, bool deleteFollowingPhrases, CompositeCommand command)
        {
            if (mView.ObiForm.Settings.Recording_ReplaceAfterCursor || deleteFollowingPhrases)
            {
                int phraseIndex =(node != null &&   node is EmptyNode)? ((EmptyNode)node).Index + 1:
                    (mView.Selection != null && mView.Selection is StripIndexSelection )? ((StripIndexSelection)mView.Selection).Index: -1 ;
                SectionNode section = node != null && node is EmptyNode? ((EmptyNode)node).ParentAs<SectionNode>():
                    mView.Selection != null && mView.Selection is StripIndexSelection? (SectionNode)mView.Selection.Node: null ;
                //MessageBox.Show(phraseIndex.ToString());
                if (section == null || phraseIndex < 0 || phraseIndex >= section.PhraseChildCount) return;

                command.ChildCommands.Insert(command.ChildCommands.Count, 
                    mView.GetDeleteRangeOfPhrasesInSectionCommand(section, section.PhraseChild(phraseIndex), section.PhraseChild(section.PhraseChildCount - 1)));
            }
        }

        private delegate void RecordingPhraseStarted_Delegate(Obi.Events.Audio.Recorder.PhraseEventArgs e,
            urakawa.command.CompositeCommand command, EmptyNode emptyNode);

        // Start recording a phrase, possibly replacing an empty node (only for the first one.)
        private void RecordingPhraseStarted(Obi.Events.Audio.Recorder.PhraseEventArgs e,
            urakawa.command.CompositeCommand command, EmptyNode emptyNode)
        {
            if (InvokeRequired)
            {
                Invoke(new RecordingPhraseStarted_Delegate(RecordingPhraseStarted), e, command, emptyNode);
            }
            else
            {

                // Suspend presentation change handler so that we don't stop when new nodes are added.
                mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                mView.Presentation.UsedStatusChanged -= new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
                PhraseNode phrase = mView.Presentation.CreatePhraseNode(e.Audio);
                mRecordingPhrase = phrase;
                Commands.Node.AddNode add = new Commands.Node.AddNode(mView, phrase, mRecordingSection,
                    mRecordingInitPhraseIndex + e.PhraseIndex);
                add.SetDescriptions(command.ShortDescription);

                // transfer properties if 2 point split is being performed
                if (m_IsAfterRecordingSplitTransferEnabled && m_TempNodeForPropertiesTransfer != null)
                {
                    m_IsAfterRecordingSplitTransferEnabled = false;
                    CopyPropertiesToRecordingNode((EmptyNode)phrase);
                }


                //add.UpdateSelection = false;
                if (e.PhraseIndex == 0)
                {
                    if (emptyNode != null && e.PhraseIndex == 0)
                    {
                        phrase.CopyAttributes(emptyNode);
                        phrase.Used = emptyNode.Used;
                        Commands.UpdateSelection updateSelection = new Commands.UpdateSelection(mView, new NodeSelection(emptyNode, mView.Selection.Control));
                        updateSelection.RefreshSelectionForUnexecute = true;
                        command.ChildCommands.Insert(command.ChildCommands.Count, updateSelection);
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(mView, emptyNode));
                        command.ChildCommands.Insert(command.ChildCommands.Count, add);
                    }
                    else
                    {
                        command.ChildCommands.Insert(command.ChildCommands.Count, add);
                    }


                    mView.Presentation.UndoRedoManager.Execute(command);
                }
                else
                {
                    // Check if the next phrase is empty page. if it is then record into it instead of creating new phrase
                    EmptyNode followingEmptyPage = null;
                    if (e.PhraseIndex > 0 && mRecordingSection.PhraseChildCount > mRecordingInitPhraseIndex + e.PhraseIndex && e.IsPage)
                    {

                        ObiNode followingObiNode = mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + e.PhraseIndex);
                        if (followingObiNode != null && !(followingObiNode is PhraseNode)
                            && ((EmptyNode)followingObiNode).Role_ == EmptyNode.Role.Page)
                        {

                            followingEmptyPage = (EmptyNode)followingObiNode;
                        }
                    }

                    if (followingEmptyPage != null)
                    {
                        urakawa.command.CompositeCommand recordInNextPageCommand = mView.Presentation.CreateCompositeCommand(Localizer.Message("Recording_RecordInNextExistingEmptyPage"));
                        phrase.CopyAttributes(followingEmptyPage);
                        recordInNextPageCommand.ChildCommands.Insert(recordInNextPageCommand.ChildCommands.Count, new Commands.Node.Delete(mView, followingEmptyPage));
                        recordInNextPageCommand.ChildCommands.Insert(recordInNextPageCommand.ChildCommands.Count, add);
                        mView.Presentation.UndoRedoManager.Execute(recordInNextPageCommand);

                    }
                    else
                    {
                        mView.Presentation.UndoRedoManager.Execute(add);
                    }
                }
                mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
                mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                if (mRecordingPhrase != null && mView.Selection != null && mView.Selection.Control.GetType() == typeof(ContentView) && !this.ContainsFocus)
                    mView.Selection = new NodeSelection(mRecordingPhrase, mView.Selection.Control);
            }
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
                catch (System.Exception ex)
                    {//3
                    mView.WriteToLogFile(ex.ToString());
                    m_TempNodeForPropertiesTransfer = null;
                    }//-3
                }//-2

            }


        // Stop recording a phrase
        private void RecordingPhraseEnded(Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            //if (!mView.ObiForm.Settings.Audio_EnableLivePhraseDetection) //@onTheFly: following code should be executed if live phrase detection is disabled
            //{
                PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(e.PhraseIndex + mRecordingInitPhraseIndex);
                phrase.SignalAudioChanged(this, e.Audio);
                mRecordingPhrase = null;
            //}
        }

        // Start recording a new page, set the right page number
        private void RecordingPage(Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            
                PhraseNode phrase = (PhraseNode)mRecordingSection.PhraseChild(e.PhraseIndex + mRecordingInitPhraseIndex + 1);
                Console.WriteLine ("Page mark indexes: " + phrase.Index + " : " + mRecordingSection.PhraseChildCount + ", session phrase count  : " + (mRecordingInitPhraseIndex+mRecordingSession.PhraseMarksCount)) ;
                Dictionary<PhraseNode, PageNumber> phraseToPageNumberMap = new Dictionary<PhraseNode, PageNumber>();
                phraseToPageNumberMap.Add(phrase, phrase.PageNumber);
            // if the phrase is page, make it plain phrase before assigning new page number
                if (phrase.Role_ == EmptyNode.Role.Page) phrase.Role_ = EmptyNode.Role.Plain;
                // page role is automatically assigned by assigning page number 
                phrase.PageNumber = mView.Presentation.PageNumberFollowing(phrase);
                
                for (int i = phrase.Index+1 ; 
                    i < mRecordingSection.PhraseChildCount && i<= mRecordingInitPhraseIndex + mRecordingSession.PhraseMarksCount; 
                    i++)
                {
                    EmptyNode empty = mRecordingSection.PhraseChild(i);
                    //Console.WriteLine("iterating phrase " + i);
                    phrase = (PhraseNode)empty;
                    
                    phraseToPageNumberMap.Add(phrase, phrase.PageNumber);

                    if (phraseToPageNumberMap.ContainsKey((PhraseNode)mRecordingSection.PhraseChild(i - 1)))
                    {
                        PageNumber number = phraseToPageNumberMap[(PhraseNode)mRecordingSection.PhraseChild(i - 1)];
                        if (number != null)
                        {
                            phrase.PageNumber = number;
                        }
                        else
                        {
                            phrase.Role_ = EmptyNode.Role.Plain;
                        }
                    }//key contains check ends
                }
            
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
            mView.WriteToLogFile(ex.ToString());
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
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
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
                        mView.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("TransportBar_ErrorInStartingRecording") + "\n\n" + ex.ToString());  //@Messagecorrected
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
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64( mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
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
        private bool m_EnablePostRecordingPageRenumbering = true;
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
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64( mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
                   Time.TIME_UNIT;

                    if (mRecordingPhrase != null && mRecordingSession != null
                        && timeOfAssetMilliseconds < 250) return false;
                    m_EnablePostRecordingPageRenumbering = false;
                    PauseRecording();
                    m_EnablePostRecordingPageRenumbering = true;
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
                        SetupRecording ( Recording , false);
                        }
                    catch (System.Exception ex)
                        {
                        mView.WriteToLogFile(ex.ToString());
                        MessageBox.Show ( Localizer.Message ( "TransportBar_ErrorInStartingRecording" ) + "\n\n" + ex.ToString (), Localizer.Message ( "Caption_Error" ) );
                        if (mState == State.Monitoring || mState == State.Recording ) Stop ();
                        }
                    }
                    else
                    {
                        // mView.AddSection(ProjectView.WithoutRename);

                    try
                        {
                        SetupRecording ( Recording, mRecordingSection, false );
                        }
                    catch (System.Exception ex)
                        {
                        mView.WriteToLogFile(ex.ToString());
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
                float fastPlayFactor = GetFastPlayFactorOfCombobox () ;
                DetermineUseOfSoundTouch(fastPlayFactor);
                mCurrentPlaylist.Audioplayer.FastPlayFactor = fastPlayFactor;
                //mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
                return true;
            }
            return false;
        }

        public bool  FastPlayRateStepDown()
        {
            if (mFastPlayRateCombobox.SelectedIndex > 0)
            {
                mFastPlayRateCombobox.SelectedIndex = mFastPlayRateCombobox.SelectedIndex - 1;

                float fastPlayFactor = GetFastPlayFactorOfCombobox ();
                DetermineUseOfSoundTouch(fastPlayFactor);
                mCurrentPlaylist.Audioplayer.FastPlayFactor = fastPlayFactor;
                //mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
                return true;
            }
            return false;
        }

        public bool FastPlayRateNormalise()
        {
            mFastPlayRateCombobox.SelectedIndex = 0;
            DetermineUseOfSoundTouch(1.0f);
            mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
            return true;
        }

        private void mFastPlayRateComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            float fastPlayFactor = GetFastPlayFactorOfCombobox ();
            DetermineUseOfSoundTouch(fastPlayFactor);
            mCurrentPlaylist.Audioplayer.FastPlayFactor = fastPlayFactor;
            //mCurrentPlaylist.Audioplayer.FastPlayFactor = (float)Convert.ToDouble(mFastPlayRateCombobox.SelectedItem.ToString());
        }

        private float GetFastPlayFactorOfCombobox()
        {
            float fastPlayFactor = 0;
            float.TryParse(mFastPlayRateCombobox.SelectedItem.ToString(), out fastPlayFactor);
            if (fastPlayFactor == 0)
            {
                fastPlayFactor = float.Parse(mFastPlayRateCombobox.SelectedItem.ToString(), System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));
            }
            return fastPlayFactor;
        }

        public bool FastPlayNormaliseWithLapseBack()
        {
            double elapseBackInterval = mView.ObiForm.Settings.ElapseBackTimeInMilliseconds;
            // work around to handle special nudge condition of preview: this should be implemented universally after 2.0 release
            if (mCurrentPlaylist != null && mView.Selection is AudioSelection && mCurrentPlaylist is PreviewPlaylist && CurrentState == State.Paused) Stop();
            if (IsPlayerActive)
            {
                DetermineUseOfSoundTouch(1.0f);
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

        /// <summary>
        /// sound touch is enabled if fast play is to be activated else it is restored to original DX
        /// </summary>
        /// <param name="fastPlayFactor"></param>
        private void DetermineUseOfSoundTouch(float fastPlayFactor)
        {
            if (mPlayer == null) return;
            //if (mView.Presentation != null &&  mView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 1
            if (mView.ObiForm != null
                && mView.ObiForm.Settings.Audio_FastPlayWithoutPitchChange
                && fastPlayFactor > 1.0f
                && !mPlayer.UseSoundTouch)
            {
                mPlayer.UseSoundTouch = true;
            }
            else if (mPlayer.UseSoundTouch 
                && (!mView.ObiForm.Settings.Audio_FastPlayWithoutPitchChange ||  fastPlayFactor <= 1.0f))
            {
                mPlayer.UseSoundTouch = false;
                Console.WriteLine("use sound touch " + mPlayer.UseSoundTouch);
            }
        }

        public void ResetFastPlayForPreferencesChange()
        {
            FastPlayRateNormalise();
            if (mFastPlayRateCombobox.Items.Count < 8)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportBar));
                resources.ApplyResources(this.mFastPlayRateCombobox, "mFastPlayRateCombobox");
                mFastPlayRateCombobox.Items.Clear();
                this.mFastPlayRateCombobox.Items.AddRange(new object[] {
            resources.GetString("mFastPlayRateCombobox.Items"),
            resources.GetString("mFastPlayRateCombobox.Items1"),
            resources.GetString("mFastPlayRateCombobox.Items2"),
            resources.GetString("mFastPlayRateCombobox.Items3"),
            resources.GetString("mFastPlayRateCombobox.Items4"),
                resources.GetString("mFastPlayRateCombobox.Items5"),
                resources.GetString("mFastPlayRateCombobox.Items6"),
            resources.GetString("mFastPlayRateCombobox.Items7")});
            }
            
                if (mFastPlayRateCombobox.Items.Count == 8 &&  mView.ObiForm.Settings.Audio_FastPlayWithoutPitchChange)
                {
                    mFastPlayRateCombobox.Items.RemoveAt(1);
                    mFastPlayRateCombobox.Items.RemoveAt(1);
                }
            
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
                        if (mView.ObiForm.Settings.PlayOnNavigate) Preview(true, false);
                        SelectionChangedPlaybackEnabled = PlaybackOnSelectionEnabledStatus;
                        return true;
                    }
                }
            }
        SelectionChangedPlaybackEnabled = PlaybackOnSelectionEnabledStatus ;
            return false;
        }

        public enum NudgeSelection { ExpandAtLeft, ContractAtLeft, ExpandAtRight, ContractAtRight } ;

        public bool NudgeSelectedAudio(NudgeSelection nudgeDirection)
        {
            if (mView.Selection == null || !(mView.Selection is AudioSelection)) return false;

            double beginTime = ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime;
            double endTime = ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime;
            PhraseNode phrase = (PhraseNode)mView.Selection.Node ;
            if (beginTime == 0 || endTime == phrase.Duration) return false;

            double nudgeDuration = mView.ObiForm.Settings.NudgeTimeMs ;

            if (nudgeDirection == NudgeSelection.ExpandAtLeft)
            {
                beginTime = beginTime - nudgeDuration;
                if (beginTime < 0) beginTime = 0;
            }
            else if (nudgeDirection == NudgeSelection.ContractAtLeft)
            {
                beginTime = beginTime + nudgeDuration;
            }
            else if (nudgeDirection == NudgeSelection.ExpandAtRight)
            {
                endTime = endTime + nudgeDuration;
                if (endTime > phrase.Duration) endTime = phrase.Duration;
            }
            else if (nudgeDirection == NudgeSelection.ContractAtRight)
            {
                endTime = endTime - nudgeDuration;
            }
            if (endTime < beginTime) return false;

            this.SelectionChangedPlaybackEnabled = false;
            mView.Selection = new AudioSelection (phrase,mView.Selection.Control, new AudioRange(beginTime,endTime)) ;
            SelectionChangedPlaybackEnabled = true;
            if (mView.ObiForm.Settings.PlayOnNavigate ) PreviewAudioSelection(); 
            return true ;
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
            if (end > audioData.AudioDuration.AsMilliseconds)
                end = audioData.AudioDuration.AsMilliseconds;

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
            if (mView.IsZoomWaveformActive)
            {
                return;
            }

            if (mView.ObiForm.Settings.Recording_ReplaceAfterCursor && CurrentState == State.Playing)
            {
                Pause();
                if (mView.Selection == null || !(mView.Selection.Node is EmptyNode) || mView.Selection.Node != mCurrentPlaylist.CurrentPhrase) return;
            }

            if ((mView.ObiForm.Settings.Recording_PreviewBeforeStarting || mPreviewBeforeRec) && mView.ObiForm.Settings.AllowOverwrite
               && ((CurrentState == State.Paused && !(mView.Selection is AudioSelection)) || (mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)))
            {
                
                m_PreviewBeforeRecordingWorker = new System.ComponentModel.BackgroundWorker();
                m_PreviewBeforeRecordingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    Preview(Upto, UseAudioCursor);
                    int interval = 50;
                    for (int i = 0; i < (PreviewDuration * 2) / interval; i++)
                    {
                        if (mCurrentPlaylist is PreviewPlaylist && mCurrentPlaylist.State == AudioLib.AudioPlayer.State.Paused && ((PreviewPlaylist)mCurrentPlaylist).IsPreviewComplete)
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
                    if (CurrentState == State.Paused)
                    {
                        if (mResumeRecordingPhrase != null) mResumeRecordingPhrase = null;
                        StartRecordingDirectly_Internal(false);
                    }
                });
                m_PreviewBeforeRecordingWorker.RunWorkerAsync();
                mPreviewBeforeRec = false;
            }
            else
            {
                StartRecordingDirectly_Internal(false);
            }
        }

        private void StartRecordingDirectly_Internal(bool deleteFollowingPhrases)
    {
            if (mRecordingSession == null
                && mCurrentPlaylist.Audioplayer.CurrentState != AudioLib.AudioPlayer.State.Playing
                &&    !IsMetadataSelected &&  ( mView.Selection == null || !(mView.Selection is TextSelection)))
                {
                try
                    {
                    SetupRecording ( Recording, deleteFollowingPhrases );
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
                List<PhraseNode> listOfRecordedPhrases = new List<PhraseNode>();
                EmptyNode firstRecordedPage = null;
                try
                    {
                    mRecordingSession.Stop ();
                    
                    

                    // update phrases with audio assets
                    UpdateRecordedPhrasesAlongWithPostRecordingOperations(listOfRecordedPhrases,ref firstRecordedPage);

                    //Workaround to force phrases to show if they become invisible on stopping recording
                    mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
                    EmptyNode lastRecordedPhrase = mRecordingSection.PhraseChildCount >0? mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1):null;
                    if (!wasMonitoring && lastRecordedPhrase != null && lastRecordedPhrase.IsRooted) mView.SelectFromTransportBar ( lastRecordedPhrase, null );

                    

                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( Localizer.Message ("TransportBar_ErrorInStopRecording") + "\n\n" +   ex.ToString ()  , Localizer.Message ("Caption_Error"));
                    }
                
UpdateButtons();
bool playbackEnabledOnSelectionChange = SelectionChangedPlaybackEnabled;
SelectionChangedPlaybackEnabled = false;
                try
                {
                    AdditionalPostRecordingOperations(firstRecordedPage, listOfRecordedPhrases);
                }
                catch (System.Exception ex)
                {
                    mView.WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
                SelectionChangedPlaybackEnabled = playbackEnabledOnSelectionChange;
                mRecordingSession = null;
                mResumeRecordingPhrase = null;
                // if a new unrooted section is created and is held in selection, clear the selection
                if (mRecordingSection != null && !mRecordingSection.IsRooted && mView.Selection.Node == mRecordingSection) mView.Selection = null;
            }
        else if (mResumeRecordingPhrase != null)
            {
            mRecordingSession = null;
            mResumeRecordingPhrase = null;

            }
        }

        private void UpdateRecordedPhrasesAlongWithPostRecordingOperations(List<PhraseNode> listOfRecordedPhrases,ref EmptyNode firstRecordedPage)
        {
            if (mRecordingSession.RecordedAudio != null && mRecordingSession.RecordedAudio.Count > 0)
            {
                for (int i = 0; i < mRecordingSession.RecordedAudio.Count; ++i)
                {
                    mView.Presentation.UpdateAudioForPhrase(mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i),
                        mRecordingSession.RecordedAudio[i]);
                    if (!mRecordingSection.Used) mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i).Used = false;
                    if (mRecordingSession.PhraseIndexesToDelete.Contains(i))
                    {
                        mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i).Used = false;
                    }
                    //check if a page was marked
                    if (firstRecordedPage == null && mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i).Role_ == EmptyNode.Role.Page) firstRecordedPage = mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i);

                    listOfRecordedPhrases.Add((PhraseNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + i));
                }
                
            }
        }

        private void AdditionalPostRecordingOperations(EmptyNode firstRecordedPage, List<PhraseNode> listOfRecordedPhrases)
        {
            // make sure that recordingsession is not null before calling this function
            bool isRecordingAtEnd = false;
            if (listOfRecordedPhrases.Count > 0 && listOfRecordedPhrases[listOfRecordedPhrases.Count - 1]== mView.Presentation.LastSection.LastLeaf) 
                isRecordingAtEnd = true;
            //Console.WriteLine("recording index :" + listOfRecordedPhrases[listOfRecordedPhrases.Count - 1].Index + " : " + (mRecordingSection.PhraseChildCount-1));
            if (mRecordingSession.PhraseMarksOnTheFly.Count > 0)
            {
                if (IsPlaying) Pause();

                EmptyNode lastPhrase = listOfRecordedPhrases.Count > 0 ? listOfRecordedPhrases[listOfRecordedPhrases.Count - 1] : null;//@AdvanceRecording
                EmptyNode nextToLastPhrase = null;
                if (lastPhrase != null)
                {
                    nextToLastPhrase = lastPhrase.Index < lastPhrase.ParentAs<SectionNode>().PhraseChildCount - 1 ? (EmptyNode)lastPhrase.FollowingNode :
                        null;//@AdvanceRecording
                }


                mView.Presentation.Do(GetSplitCommandForOnTheFlyDetectedPhrases(listOfRecordedPhrases, mRecordingSession.PhraseMarksOnTheFly));

                
                if (nextToLastPhrase != null && nextToLastPhrase.Index > 0)//@advanceRecording
                {
                    SectionNode section = nextToLastPhrase.ParentAs<SectionNode>();
                    mView.SelectFromTransportBar( section.PhraseChild(nextToLastPhrase.Index - 1), null);
                    
                }
                else if (lastPhrase != null)
                {
                    SectionNode section = lastPhrase.ParentAs<SectionNode>();
                    mView.SelectFromTransportBar(section.PhraseChild(section.PhraseChildCount-1), null);
                }

                if (mView.Selection != null)
                {
                    Commands.UpdateSelection updateSelectionCmd = updateSelectionCmd = new Obi.Commands.UpdateSelection(mView,
                        new NodeSelection(mView.Selection.Node, mView.Selection.Control));
                    mView.Presentation.Do(updateSelectionCmd);
                }
            }
            if (mView.ObiForm.Settings.Audio_EnablePostRecordingPageRenumbering &&  m_EnablePostRecordingPageRenumbering &&  firstRecordedPage != null)
            {//1
                int pageNum = firstRecordedPage.PageNumber.Number ;
                PageKind pageKind = firstRecordedPage.PageNumber.Kind ;
                bool renumber = false ;
                foreach (PhraseNode p in listOfRecordedPhrases)
                {//2
                    if ( p == firstRecordedPage ) continue ;
                    if (p.Role_ == EmptyNode.Role.Page && p.PageNumber != null && p.PageNumber.Kind == pageKind)
                    {//3
                        if (p.PageNumber.Number != pageNum + 1)
                        {//4
                            renumber = true;
                            break;
                        }//-4
                        pageNum = p.PageNumber.Number;
                    }//-3
                }//-2
                // check if the last recording phrase is also last phrase in section
                if (!isRecordingAtEnd) renumber = true;
                if(renumber )
                    //&& MessageBox.Show(Localizer.Message("TransportBar_RenumberPagesAfterRecording"), Localizer.Message("RenumberPagesCaption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (IsPlaying) Pause();
                mView.Presentation.Do(mView.GetPageRenumberCommand(firstRecordedPage, firstRecordedPage.PageNumber, Localizer.Message("RenumberPagesCaption").Replace("?", ""),true));
            }
            }
            if (mView != null && mView.ObiForm.Settings.Project_SaveProjectWhenRecordingEnds) mView.ObiForm.Save();
        }

        private CompositeCommand GetSplitCommandForOnTheFlyDetectedPhrases(List<PhraseNode> phrasesList, List<double> timingList)
        {
            CompositeCommand multipleSplitCommand = mView.Presentation.CreateCompositeCommand("Multiple split command");
            timingList.Sort();
            List<double> newTimingList = new List<double>();
            newTimingList.InsertRange(0, timingList);
            Console.WriteLine("On the fly: " + phrasesList.Count);
            for (int i = phrasesList.Count - 1; i >= 0; i--)
            {
                PhraseNode phrase = phrasesList[i];
                double referenceTimeForPhrase = 0;

                if (phrase.Role_ == EmptyNode.Role.Page || phrase.TODO || !phrase.Used) continue;

                //first calculate the reference time
                foreach (PhraseNode p in phrasesList)
                {
                    if (p == phrase) break;
                    referenceTimeForPhrase += p.Duration;
                }

                for (int j = newTimingList.Count-1; j >= 0; j--)
                {
                    if (newTimingList[j] < referenceTimeForPhrase) break;
                    //Commands.Node.SplitAudio split = new Commands.Node.SplitAudio(mView, phrase, newTimingList[j]);
                    double splitTimeInPhrase = newTimingList[j] - referenceTimeForPhrase;
                    //avoid invalid time and also too small phrases, less than 200ms
                    if ( splitTimeInPhrase <= 0 || splitTimeInPhrase>= (phrase.Duration - 200) ) continue ;
                    CompositeCommand split = Commands.Node.SplitAudio.GetSplitCommand(mView, phrase,splitTimeInPhrase );
                    multipleSplitCommand.ChildCommands.Insert(multipleSplitCommand.ChildCommands.Count, split);
                    newTimingList.RemoveAt(j);
                }

            }
            return multipleSplitCommand;
        }


        public bool IsRecording
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
        public bool IsPlaying { get { return mPlayer.CurrentState == AudioLib.AudioPlayer.State.Playing; } }
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


        public enum AudioCluesSelection { SelectionBegin, SelectionEnd } ;

        public void PlayAudioClue(AudioCluesSelection Clue)
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

        

        public bool CanUsePlaybackSelection { get { return Enabled && IsPlayerActive && mView.ObiForm.Settings.PlayOnNavigate; }}

        public void mView_BlocksVisibilityChanged ( object sender, EventArgs e )
            {
            UpdateButtons ();
            }

        private void mDisplayBox_DropDown(object sender, EventArgs e)
        {
            mDisplayBox.Tag = mDisplayBox.SelectedIndex;
        }

        private bool m_FineNavigationModeForPhrase = false ;
        private EmptyNode m_FineNavigationPhrase = null;
        public bool FineNavigationModeForPhrase
        {
            get
            {
                return m_FineNavigationModeForPhrase;
            }
            set
            {
                if (value != m_FineNavigationModeForPhrase)
                {
                    m_FineNavigationModeForPhrase = value;
                    if (m_FineNavigationModeForPhrase)
                    {
                        m_FineNavigationPhrase = mView.Selection.EmptyNodeForSelection;
                        mView.UpdateBlockForFindNavigation(m_FineNavigationPhrase, m_FineNavigationModeForPhrase);    
                        string navigationOnClue = System.IO.Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "FineNavigationOn.wav") ;
                        if (mView.ObiForm.Settings.AudioClues &&  System.IO.File.Exists(navigationOnClue))
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(navigationOnClue);
                            player.Play();
                        }
                        // add sound here
                    }
                    else
                    {
                        if (m_FineNavigationPhrase != null) mView.UpdateBlockForFindNavigation(m_FineNavigationPhrase, m_FineNavigationModeForPhrase);
                        m_FineNavigationPhrase = null;
                        if(mView.ObiForm.Settings.AudioClues)  System.Media.SystemSounds.Exclamation.Play();
                        // add sound here
                    }
                    

                    if (StateChanged != null) StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(mPlayer.CurrentState) );
                }
            }
        }

        public string FineNavigationStatusMsg
        {
            get { return FineNavigationModeForPhrase ? Localizer.Message ("StatusMsg_FineNavigation") : ""; }
        }

        public string GetHelpTopicPath()
        {
            if (mTimeDisplayBox.ContainsFocus || mDisplayBox.ContainsFocus)
            {
                return "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Time Display.htm";
            }
            else if (mVUMeterPanel.ContainsFocus)
            {
                return "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Textual peak meter.htm";
            }
            else if (mFastPlayRateCombobox.ContainsFocus)
            {
                return "HTML Files\\Creating a DTB\\Working with Audio\\Fast Play.htm";
            }
            else if (mToDo_CustomClassMarkButton.ContainsFocus)
            {
                return "HTML Files\\Creating a DTB\\Working with Phrases\\Changing the Todo or Used Status.htm";
            }
            else
            { return "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Audio and Navigation controls.htm"; }
        }


        private void m_btnRecordingOptions_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point ptLowerLeft = new Point(0, btn.Height);
            ptLowerLeft = btn.PointToScreen(ptLowerLeft);
            m_RecordingOptionsContextMenuStrip.Show(ptLowerLeft);  

        }

        private void RecordingOptions_Monitoring_Click(object sender, EventArgs e)
        {
            if (CanResumePlayback || mState == State.Stopped)
            {
                SetupRecording(Monitoring, false); 
            }
        }

        private void RecordingOptions_RecordWithDeleteFollowing_Click(object sender, EventArgs e)
        {
            RecordOverSubsequentPhrases();
        }

        public void RecordOverSubsequentPhrases()
        {
            if (CanRecord) StartRecordingDirectly_Internal(true);
        }

        private void m_RecordingtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Record();
        }

        private void m_RecordingOptionsContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mView != null && mView.ObiForm != null && mView.ObiForm.Settings != null && mView.Selection != null && CurrentState != State.Monitoring
    && mView.ObiForm.Settings.AllowOverwrite && ((CurrentState == State.Paused && !(mView.Selection is AudioSelection)) || (mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)))
           
            {
                mPreviewBeforeRecToolStripMenuItem.Enabled = true;
            }
            else
            {
                mPreviewBeforeRecToolStripMenuItem.Enabled = false;
            }

            if (mView.ObiForm.Settings.AllowOverwrite)
            {
                m_DeletePhrasestoolStripMenuItem.Enabled = !IsListening;
            }
            else
            {
                m_DeletePhrasestoolStripMenuItem.Enabled = false;
            }
        }

        public void InitializeTooltipsForTransportpar()
        {
            keyboardShortcuts = mView.ObiForm.KeyboardShortcuts;
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPreviousPhraseToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mPrevPhraseButton, Localizer.Message("Transport_PreviousPhrase") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousPhraseToolStripMenuItem"].Value.ToString()) + ")");
                mPrevPhraseButton.AccessibleName = Localizer.Message("Transport_PreviousPhraseAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousPhraseToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPreviousSectionToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mPrevSectionButton, Localizer.Message("Transport_PreviousSection") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousSectionToolStripMenuItem"].Value.ToString()) + ")");
                mPrevSectionButton.AccessibleName = Localizer.Message("Transport_PreviousSectionAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousSectionToolStripMenuItem"].Value.ToString());
            }

            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPreviousPageToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mPreviousPageButton, Localizer.Message("Transport_PreviousPage") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousPageToolStripMenuItem"].Value.ToString()) + ")");
                mPreviousPageButton.AccessibleName = Localizer.Message("Transport_PreviousPageAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPreviousPageToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mRewindToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mRewindButton, Localizer.Message("Transport_FastPlayBackward") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mRewindToolStripMenuItem"].Value.ToString()) + ")");
                mRewindButton.AccessibleName = Localizer.Message("Transport_FastPlayBackwardAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mRewindToolStripMenuItem"].Value.ToString());           
            }

           
            mTransportBarTooltip.SetToolTip(mPlayButton, Localizer.Message("Transport_StartPlayback") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ProjectView_PlayPauseUsingAudioCursor_Default.Value.ToString()) + ")");
            mTransportBarTooltip.SetToolTip(mPauseButton, Localizer.Message("Transport_StartPlayback") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ProjectView_PlayPauseUsingAudioCursor_Default.Value.ToString()) + ")");

            mPlayButton.AccessibleName = Localizer.Message("Transport_StartPlaybackAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ProjectView_PlayPauseUsingAudioCursor_Default.Value.ToString());
            mPauseButton.AccessibleName = Localizer.Message("Transport_StartPausebackAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ProjectView_PlayPauseUsingAudioCursor_Default.Value.ToString());

            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mStopToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mStopButton, Localizer.Message("Transport_StopPlayback") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mStopToolStripMenuItem"].Value.ToString()) + ")");
                mStopButton.AccessibleName = Localizer.Message("Transport_StopPlaybackAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mStopToolStripMenuItem"].Value.ToString());
            }
if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mStartMonitoringToolStripMenuItem"))
{
            mTransportBarTooltip.SetToolTip(mRecordButton, Localizer.Message("Transport_StartRecording") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mStartMonitoringToolStripMenuItem"].Value.ToString()) + ")");
}
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mNextPhraseToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mNextPhrase, Localizer.Message("Transport_NextPhrase") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextPhraseToolStripMenuItem"].Value.ToString()) + ")");
                mNextPhrase.AccessibleName = Localizer.Message("Transport_NextPhrase") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextPhraseToolStripMenuItem"].Value.ToString());
            }

            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mNextPageToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mNextPageButton, Localizer.Message("Transport_NextPage") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextPageToolStripMenuItem"].Value.ToString()) + ")");
                mNextPageButton.AccessibleName = Localizer.Message("Transport_NextPage") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextPageToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mNextSectionToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mNextSectionButton, Localizer.Message("Transport_NextSection") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextSectionToolStripMenuItem"].Value.ToString()) + ")");
                mNextSectionButton.AccessibleName = Localizer.Message("Transport_NextSection") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mNextSectionToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPhrases_PhraseIsTODOMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mToDo_CustomClassMarkButton, Localizer.Message("Transport_AddTodo") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPhrases_PhraseIsTODOMenuItem"].Value.ToString()) + ")");
                mToDo_CustomClassMarkButton.AccessibleName = Localizer.Message("Transport_AddTodoAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPhrases_PhraseIsTODOMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mFastForwardToolStripMenuItem"))
            {
                mTransportBarTooltip.SetToolTip(mFastForwardButton, Localizer.Message("Transport_FastPlayForward") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mFastForwardToolStripMenuItem"].Value.ToString()) + ")");
                mFastForwardButton.AccessibleName = Localizer.Message("Transport_FastPlayForwardAcc") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mFastForwardToolStripMenuItem"].Value.ToString());  
            }           
            
                   
        }

        private void m_btnPlayingOptions_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Point ptLowerLeft = new Point(0, btn.Height);
            ptLowerLeft = btn.PointToScreen(ptLowerLeft);
            m_PlayingOptionsContextMenuStrip.Show(ptLowerLeft); 
        }

        private void m_PlaySectiontoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //PhraseNode phrNode = mCurrentPlaylist.CurrentPhrase;
            //ObiNode nodeSel = phrNode.ParentAs<SectionNode>();
            PhraseNode pharse=null;
            ObiNode nodeSelect = null;
            if (mView != null && mView.Selection != null)
            {


                if (mView.Selection.Node is PhraseNode)
                {
                    pharse = (PhraseNode)mView.Selection.Node;
                    nodeSelect = pharse.ParentAs<SectionNode>();
                }
                else if (mView.Selection.Node is SectionNode)
                {
                    nodeSelect = mView.Selection.Node;
                }
                if (nodeSelect != null)
                {

                    IsPlaySection = true;
                    PlayOrResume(nodeSelect);
                }
            }
        }

        private void m_PlayAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mView.IsZoomWaveformActive)//@zoomwaveform: if zoom waveform is not active, start play all else start play selection
            {
                PlayAll();
            }
            else
            {
                PlayOrResume();
            }
        }

        private void m_PreviewFromtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preview(From, UseAudioCursor);
        }

        private void m_PreviewUptotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preview(Upto, UseAudioCursor);
        }

        private void m_PlayingOptionsContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_PlayAlltoolStripMenuItem.Enabled = mView.CanPlay || mView.CanResume;
            m_PlaySectiontoolStripMenuItem.Enabled = mView.CanPlaySelection || mView.CanResume;
            m_playHeadingToolStripMenuItem.Enabled = mView.CanPlaySelection || mView.CanResume;

            m_PreviewFromtoolStripMenuItem.Enabled = mView.CanPreview || mView.CanPreviewAudioSelection;
            m_PreviewUptotoolStripMenuItem.Enabled = mView.CanPreview || mView.CanPreviewAudioSelection;
            
        }

        private void mPreviewBeforeRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mPreviewBeforeRec = true;
            StartRecordingDirectly();
        }

        private void m_playHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PhraseNode pharse=null;
            SectionNode nodeSelect = null;
            EmptyNode emptyNode = null;
            if (mView != null && mView.Selection != null)
            {


                if (mView.Selection.Node is PhraseNode)
                {
                    pharse = (PhraseNode)mView.Selection.Node;
                    nodeSelect = pharse.ParentAs<SectionNode>();
                }
                else if (mView.Selection.Node is SectionNode)
                {
                    nodeSelect = (SectionNode)mView.Selection.Node;
                }
                else if (mView.Selection.Node is EmptyNode)
                {
                    emptyNode = (EmptyNode)mView.Selection.Node;
                    nodeSelect = emptyNode.ParentAs<SectionNode>();
                }
                if (nodeSelect != null)
                {                    
                    PlayHeadingPhrase(nodeSelect);
                }
            }
           
        }

    }
}
