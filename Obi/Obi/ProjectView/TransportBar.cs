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
        private bool m_IsPlaySectionInspiteOfPhraseSelection = false;
        private bool m_MonitorContinuously = false;
        private string[] m_filePaths;
        private ToolStripMenuItem m_CurrentCheckedProfile;
        private Dictionary<string, ToolStripMenuItem> m_ListOfSwitchProfiles = new Dictionary<string, ToolStripMenuItem>();
        private bool m_PreviewBeforeRecordingActive = false;
        private bool m_IsElapsedInProjectSelectedBeforeStop = false;
        private double m_TimeElapsedInRecording = 0;
        //public variables
        //private bool IsPreviewBeforeRec = false;

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
        private double m_ElapseBackInterval;
        private double m_CursorTime=0.0;
        private double m_TotalCursorTime; // used for Total Cursor time to Update Time desplay during stop stage

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
            InitializeSwitchProfiles(); 

         
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
        public bool CanRecord { get { return Enabled &&( mState == State.Stopped || mState == State.Paused ||  mState == State.Monitoring  
            ||  (mView.ObiForm.Settings.Audio_UseRecordBtnToRecordOverSubsequentAudio&& CurrentState == State.Playing && mCurrentPlaylist.PlaybackRate == 0)) 
            &&  mView.IsPhraseCountWithinLimit && !mView.IsContentViewScrollActive && !mView.IsZoomWaveformActive; } } // @phraseLimit
        public bool CanResumePlayback { get { return Enabled && mState == State.Paused   &&   !mView.IsContentViewScrollActive; } }
        public bool CanResumeRecording { get { return Enabled && mResumeRecordingPhrase != null && mResumeRecordingPhrase.IsRooted    &&   (mState != State.Playing  ||   (mView.ObiForm.Settings.Audio_UseRecordBtnToRecordOverSubsequentAudio && CurrentState == State.Playing) )&& !mView.IsContentViewScrollActive && !mView.IsZoomWaveformActive; } }
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

        public bool CanEnterFineNavigationMode { get { return mView.Selection != null && mView.Selection.Node is PhraseNode && (IsPlayerActive || mView.Selection is AudioSelection) && !IsRecorderActive; } }

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

        //@deleterecording
        public bool CanRecordOnFollowingAudio
        {
            get { return mView.ObiForm.Settings.Audio_RevertOverwriteBehaviourForRecordOnSelection || (mView.Selection == null || !(mView.Selection is AudioSelection) || ((AudioSelection)mView.Selection).AudioRange.HasCursor); }
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
                if (base.Enabled && !value && IsActive)
                {
                    if (MonitorContinuously) MonitorContinuously = false; //@MonitorContinuously
                    Stop();
                }
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
                m_PlayerTimeComboIndex = mView.ObiForm.Settings.TransportBarCounterIndex;
                m_RecorderTimeComboIndex = mView.ObiForm.Settings.TransportBarCounterIndex;
                mDisplayBox.SelectedIndex = mView.ObiForm.Settings.TransportBarCounterIndex < mDisplayBox.Items.Count ? mView.ObiForm.Settings.TransportBarCounterIndex : 0;
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
        public int PreviewDuration { get { return mView.ObiForm.Settings.Audio_PreviewDuration; } }

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
                        mView.UpdateCursorPosition(time);
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
            mRecorder.CircularBufferNotificationTimerMessage += new AudioLib.AudioRecorder.CircularBufferNotificationTimerMessageHandler(LogRecorderMissedNotificationMsg);
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
            mTransportBarTooltip.SetToolTip(mDisplayBox, mDisplayBox.SelectedItem.ToString());
            // selected index should go in settings only when presentation is not null because it is assigned only when new presentation is set
            if (mView != null && mView.ObiForm.Settings != null && mView.Presentation != null) mView.ObiForm.Settings.TransportBarCounterIndex = mDisplayBox.SelectedIndex;
            m_IsElapsedInProjectSelectedBeforeStop = false;
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
                // if single cursor is enforced then the audioselection should be removed on starting playback
                if (mView.ObiForm.Settings.Audio_EnforceSingleCursor)
                {
                    if (CurrentState == State.Playing && !(mLocalPlaylist is PreviewPlaylist )
                        && mView.Selection != null && mView.Selection is AudioSelection 
                            && ((AudioSelection)mView.Selection).AudioRange != null && ((AudioSelection)mView.Selection).AudioRange.HasCursor )
                    {   
                        bool playbackOnSelection = this.SelectionChangedPlaybackEnabled;
                        this.SelectionChangedPlaybackEnabled = false;
                        mView.Selection = new NodeSelection (mView.Selection.Node, mView.Selection.Control ) ;
                        this.SelectionChangedPlaybackEnabled = playbackOnSelection ;
                }
                }

                if (StateChanged != null) StateChanged(this, e);

                if (IsPlayerActive && !(mCurrentPlaylist is PreviewPlaylist) && MonitorContinuously) MonitorContinuously = false; //@MonitorContinuously
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
                {
                    mView.UpdateCursorPosition(mAfterPreviewRestoreTime);
                }
                else
                {
                    mView.SetPlaybackPhraseAndTime(null, 0.0);
                    if (CurrentState == State.Stopped)//@masternewbehaviour
                    {
                        if (mView.ObiForm.Settings.Audio_SelectLastPhrasePlayed
                    && mCurrentPlaylist != null
                    && mCurrentPlaylist.LastPhrase != null
                    && mView.Selection != null
                            && mView.Selection.Node != mCurrentPlaylist.LastPhrase
                            && mView.Selection.Control is ContentView)
                        {
                            bool isPlayOnSelectionChange = SelectionChangedPlaybackEnabled;
                            SelectionChangedPlaybackEnabled = false;
                            mView.SelectFromTransportBar(mCurrentPlaylist.LastPhrase, null);
                            SelectionChangedPlaybackEnabled = isPlayOnSelectionChange;
                        }
                        
                        mCurrentPlaylist = mMasterPlaylist;
                        Console.WriteLine("Master Plalist is assigned aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                        PhraseNode currentPhrase = FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node);
                        if (currentPhrase != null) mCurrentPlaylist.CurrentPhrase = currentPhrase;
                        UpdateButtons();
                        mView.ObiForm.UpdateRecordingToolBarButtons();
                    //System.Media.SystemSounds.Asterisk.Play();
                    }
                }

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
            if (IsRecorderActive && mView.IsZoomWaveformActive) mView.ZoomPanelClose(); //@zoomwaveform
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
            Console.WriteLine("Events assigned :::::::::::::::::::::::::::::::::::::::::");
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
                mRecordButton.Enabled = (CanRecord || CanResumeRecording
                    || (CurrentState == State.Playing && (mView.ObiForm.Settings.Audio_UseRecordBtnToRecordOverSubsequentAudio || mView.ObiForm.Settings.Audio_Recording_PreviewBeforeStarting))) && !mView.IsZoomWaveformActive;
                if (IsPlaying || IsRecorderActive)
                {
                    m_btnPlayingOptions.Enabled = false;
                }
                else
                {
                    m_btnPlayingOptions.Enabled = true;
                }
                if (IsPlayerActive)
                {
                    mMonitorContinuouslyToolStripMenuItem.Enabled = false;
                }
                else
                {
                    mMonitorContinuouslyToolStripMenuItem.Enabled = true;
                }

                bool recordDirectly = (mView.ObiForm  != null && mView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton) ? true : false;

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
                    if (mView.ObiForm.Settings.Audio_AllowOverwrite)
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
                if (MonitorContinuously && mView.Selection == null)
                {
                    m_btnRecordingOptions.Enabled = mView.Selection == null;
                    m_RecordingtoolStripMenuItem.Enabled = false;
                }
                else
                {
                    m_RecordingtoolStripMenuItem.Enabled = true;
                }
                if (mView != null && mView.ObiForm != null && mView.ObiForm.Settings != null)
                {
                    if (mView.ObiForm.Settings.Audio_PlaySectionUsingPlayBtn)
                    {
                        m_PlayAlltoolStripMenuItem.Text = Localizer.Message("TransportBar_PlaySelection");
                    }
                    else
                    {
                        m_PlayAlltoolStripMenuItem.Text = Localizer.Message("TransportBar_PlayAll");
                    }
                }
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
                         if (mDisplayBox.Tag != null)
                         {
                             int index = -1;
                             int.TryParse(mDisplayBox.Tag.ToString(), out index);
                             if (index >= 0) selectedIndex = index;
                         }
                     }

                     if (mState == State.Monitoring)
                     {   
                         mTimeDisplayBox.Text = "--:--:--";
                         mDisplayBox.SelectedIndex = ELAPSED_INDEX;
                     }
                     else if (mState == State.Recording && mRecordingSession.AudioRecorder.RecordingPCMFormat != null)
                     {
                         if (mView.ObiForm.Settings.Audio_ShowSelectionTimeInTransportBar && m_IsElapsedInProjectSelectedBeforeStop)
                         {
                             mDisplayBox.SelectedIndex = selectedIndex = ELAPSED_TOTAL_INDEX;
                         }
                         if(mRecordingSession == null ) return;
                         //mRecordingSession.AudioRecorder.TimeOfAsset
                         double timeOfAssetMilliseconds =
                            (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
                            Time.TIME_UNIT;

                         //mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(timeOfAssetMilliseconds);
                         //mDisplayBox.SelectedIndex = ELAPSED_INDEX;
                         mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(
                             selectedIndex == ELAPSED_INDEX ?
                                 timeOfAssetMilliseconds :
                             selectedIndex == ELAPSED_SECTION ?
                             RecordingTimeElapsedSection :
                             selectedIndex == ELAPSED_TOTAL_RECORDING_INDEX ?
                               RecordingTimeElapsedTotal : 0.0);

                     }
                     else if (mState == State.Stopped)
                     {
                         if (mView != null && mView.Selection != null && mView.ObiForm.Settings.Audio_ShowSelectionTimeInTransportBar && (mView.Selection is AudioSelection || mView.Selection.Node is PhraseNode))
                         {
                             PhraseNode phraseNode = (PhraseNode)mView.Selection.Node;
                             m_TotalCursorTime = 0.0;
                             if (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange != null)
                             {
                                 if (selectedIndex == ELAPSED_INDEX || selectedIndex == ELAPSED_SECTION || selectedIndex == ELAPSED_TOTAL_INDEX)
                                 {
                                     if (((AudioSelection)mView.Selection).AudioRange.HasCursor)
                                     {
                                         m_TotalCursorTime += ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                                     }
                                     else
                                     {
                                         m_TotalCursorTime += ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime;
                                     }

                                 }

                                 else if (selectedIndex == REMAIN_INDEX)
                                 {
                                     if (((AudioSelection)mView.Selection).AudioRange.HasCursor)
                                     {
                                         m_TotalCursorTime = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                                     }
                                     else
                                     {
                                         m_TotalCursorTime += ((AudioSelection)mView.Selection).AudioRange.SelectionEndTime;
                                     }
                                     m_TotalCursorTime = mView.Selection.Node.Duration - m_TotalCursorTime;

                                 }
                             }

                             if (selectedIndex == ELAPSED_SECTION || selectedIndex == ELAPSED_TOTAL_INDEX)
                             {
                                 if (phraseNode.PrecedingNode != null && phraseNode.Parent == phraseNode.PrecedingNode.Parent)
                                 {
                                     if (phraseNode.PrecedingNode is PhraseNode)
                                     {
                                         CalculateCursorTime((PhraseNode)phraseNode.PrecedingNode);
                                     }
                                     else if (phraseNode.PrecedingNode is EmptyNode)
                                     {
                                         ObiNode tempNode = phraseNode.PrecedingNode;
                                         while (tempNode != null && !(tempNode is PhraseNode) && tempNode.Parent == tempNode.PrecedingNode.Parent)
                                         {
                                             tempNode = tempNode.PrecedingNode;
                                         }
                                         if (tempNode is PhraseNode)
                                         {
                                             CalculateCursorTime((PhraseNode)tempNode);
                                         }
                                     }
                                 }
                             }
                             if (selectedIndex == ELAPSED_TOTAL_INDEX)
                             {
                                 mDisplayBox.SelectedIndex = ELAPSED_SECTION;
                                 m_IsElapsedInProjectSelectedBeforeStop = true;
                             }

                             mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(m_TotalCursorTime);
                         }
                         //else if (mView.ObiForm.Settings.Project_ShowSelectionTimeInTransportBar && mView.Selection.Node is PhraseNode)
                         //{
                         //    PhraseNode phraseNode = (PhraseNode)mView.Selection.Node;
                         //    m_TotalCursorTime = 0.0;
                         //    if (selectedIndex == ELAPSED_SECTION || selectedIndex == ELAPSED_TOTAL_INDEX)
                         //    {
                         //        if (phraseNode.PrecedingNode != null && phraseNode.PrecedingNode is PhraseNode && phraseNode.Parent == phraseNode.PrecedingNode.Parent)
                         //        {
                         //            CalculateCursorTime((PhraseNode)phraseNode.PrecedingNode);
                         //        }
                         //    }

                         //    if (selectedIndex == ELAPSED_TOTAL_INDEX)
                         //    {
                         //        mDisplayBox.SelectedIndex = ELAPSED_SECTION;
                         //        m_IsElapsedInProjectSelectedBeforeStop = true;
                         //    }
                         //    mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(m_TotalCursorTime);
                         //}
                         else
                         {
                             mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(0.0);
                         }
                     }
                     else
                     {
                         if (mView.ObiForm.Settings.Audio_ShowSelectionTimeInTransportBar && m_IsElapsedInProjectSelectedBeforeStop)
                         {
                             mDisplayBox.SelectedIndex = selectedIndex = ELAPSED_TOTAL_INDEX;
                         }
                         mTimeDisplayBox.Text = FormatDuration_hh_mm_ss(
                             selectedIndex == ELAPSED_INDEX ?
                                 mCurrentPlaylist.CurrentTimeInAsset :
                             selectedIndex == ELAPSED_SECTION ?
                                 PlaybackTimeElapsedSection :
                             selectedIndex == ELAPSED_TOTAL_INDEX ?
                             //  mCurrentPlaylist.CurrentTime :
                               PlayingTimeElapsedTotal :
                             selectedIndex == REMAIN_INDEX ?
                             mCurrentPlaylist.RemainingTimeInAsset :
                             //  selectedIndex == REMAINING_IN_SECTION?                         
                             //  RemainingTimeInSection:
                             mCurrentPlaylist.RemainingTime
                                 );
                     }
                 
             }
         }

        private void CalculateCursorTime(PhraseNode phraseNode)
        {

            m_TotalCursorTime += phraseNode.Duration;

            if (phraseNode.PrecedingNode != null && (phraseNode.PrecedingNode.Parent == phraseNode.Parent))
            {
                if (phraseNode.PrecedingNode is PhraseNode)
                {
                    CalculateCursorTime((PhraseNode)phraseNode.PrecedingNode);
                }
                else if (phraseNode.PrecedingNode is EmptyNode)
                {
                    ObiNode tempNode = phraseNode.PrecedingNode;
                    while (tempNode != null && !(tempNode is PhraseNode) && tempNode.PrecedingNode.Parent == tempNode.Parent)
                    {
                        tempNode = tempNode.PrecedingNode;
                    }
                    if (tempNode is PhraseNode)
                    {
                        CalculateCursorTime((PhraseNode)tempNode);
                    }
                }
            }

        }
         private void CalculateSectionTime(SectionNode secNode)
         {

             m_TotalCursorTime += secNode.Duration;
             if (secNode.PrecedingSection != null && secNode.PrecedingSection is SectionNode)
             {
                 CalculateSectionTime((SectionNode)secNode.PrecedingSection);
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
            SectionNode section = CurrentState == State.Recording && mRecordingPhrase != null ? mRecordingPhrase.ParentAs<SectionNode>() :
                PlaybackPhrase != null ? PlaybackPhrase.ParentAs<SectionNode>() : null;
            if (section != null)
            {
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

        }

        // Play/Resume playback

        private void mPlayButton_Click(object sender, EventArgs e) 
        {
            if (mView.ObiForm.Settings.Audio_PlaySectionUsingPlayBtn)
            {
                PlaySection();
            }
            else
            {
                PlayOrResume();
            }
        }

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
                //if (IsPlaySection == false)
                //{
                     neglectSelection = mView.Selection == null
                        || (node is EmptyNode && mView.Selection.Node != node);
                //}
                //else
                //{
                    //neglectSelection = true;
                    //IsPlaySection = false;
                //}
                
                if (neglectSelection || m_IsPlaySectionInspiteOfPhraseSelection)
                {
                    if (m_IsPlaySectionInspiteOfPhraseSelection && mView.Selection != null && mView.Selection.Control is ContentView )
                    {
                        // play shallow, if focus is in the content view
                        mLocalPlaylist = new Playlist(mPlayer,  node, mPlayQAPlaylist, false);
                    }
                    else
                    {
                    mLocalPlaylist = new Playlist(mPlayer,  node, mPlayQAPlaylist);
                    }
                }
                else
                {
                    mLocalPlaylist = new Playlist(mPlayer, mView.Selection, mPlayQAPlaylist);
                }
                SetPlaylistEvents(mLocalPlaylist);
                if (mCurrentPlaylist is PreviewPlaylist && !((PreviewPlaylist)mCurrentPlaylist).IsPreviewComplete) ((PreviewPlaylist)mCurrentPlaylist).EnsureDisAssociationEvents(); //added on Oct 29, 2015, precautionary for beta release, will be reviewed after release 
                mCurrentPlaylist.EnsureDisAssociationEventsinPlaylist();
                mCurrentPlaylist = mLocalPlaylist;
                if (neglectSelection && !m_IsPlaySectionInspiteOfPhraseSelection)
                {
                    mCurrentPlaylist.Play () ;
                }
                else
                {
                PlayCurrentPlaylistFromSelection();
                }
                m_IsPlaySectionInspiteOfPhraseSelection = false;
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
                if (!((AudioSelection)mView.Selection).AudioRange.HasCursor && mCurrentPlaylist != mMasterPlaylist && m_IsPlaySectionInspiteOfPhraseSelection == false)
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
                if (s.Index < s.Section.PhraseChildCount) // if the focus is on last index then play has nothing to play
                {
                    mCurrentPlaylist.CurrentPhrase = FindPlaybackStartNode(s.Index < s.Section.PhraseChildCount ?
                        (ObiNode)s.Section.PhraseChild(s.Index) : (ObiNode)s.Section);
                    if (mCurrentPlaylist.State != AudioLib.AudioPlayer.State.Playing) mCurrentPlaylist.Play();
                }
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
                    bool isPlayOnSelectionChange = SelectionChangedPlaybackEnabled;
                    SelectionChangedPlaybackEnabled = false;
                    PauseRecording();
                    SelectionChangedPlaybackEnabled = isPlayOnSelectionChange;
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
            if (mRecordingSession != null)
            {
                bool wasMonitoring = mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring;
                mVUMeterPanel.BeepEnable = false;

                EmptyNode firstRecordedPage = null;
                List<PhraseNode> listOfRecordedPhrases = new List<PhraseNode>();
                try
                {
                    mRecordingSession.Stop();

                    // update recorded phrases with audio assets
                    if (mRecordingSection != null) ///@MonitorContinuously , if block inserted to bypass the procedure of assigning assets
                    {
                        UpdateRecordedPhrasesAlongWithPostRecordingOperations(listOfRecordedPhrases, ref firstRecordedPage);

                        //Workaround to force phrases to show if they become invisible on stopping recording
                        mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
                    }
                }
                catch (System.Exception ex)
                {
                    mView.WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }

                if (mRecordingSection != null)//@MonitorContinuously
                {
                    mResumeRecordingPhrase = (PhraseNode)mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1);
                    EmptyNode phraseNextToResumePhrase = null;
                    if (mResumeRecordingPhrase.FollowingNode != null && mResumeRecordingPhrase.FollowingNode is EmptyNode) phraseNextToResumePhrase = (EmptyNode)mResumeRecordingPhrase.FollowingNode;

                    bool playbackEnabledOnSelectionChange = SelectionChangedPlaybackEnabled;
                    SelectionChangedPlaybackEnabled = false;
                    try
                    {
                        int phraseChildCount = mRecordingSection.PhraseChildCount;
                        AdditionalPostRecordingOperations(firstRecordedPage, listOfRecordedPhrases);
                        if (phraseChildCount != mRecordingSection.PhraseChildCount)
                        {
                            if (phraseNextToResumePhrase != null && phraseNextToResumePhrase.IsRooted && phraseNextToResumePhrase.PrecedingNode is PhraseNode)
                                mResumeRecordingPhrase = (PhraseNode)phraseNextToResumePhrase.PrecedingNode;
                            else if (mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1) is PhraseNode)
                                mResumeRecordingPhrase = (PhraseNode)mRecordingSection.PhraseChild(mRecordingSection.PhraseChildCount - 1);

                        }
                    }
                    catch (System.Exception ex)
                    {
                        mView.WriteToLogFile(ex.ToString());
                        MessageBox.Show(ex.ToString());
                    }

                    if (!wasMonitoring && mResumeRecordingPhrase != null) mView.SelectFromTransportBar(mResumeRecordingPhrase, null);
                    SelectionChangedPlaybackEnabled = playbackEnabledOnSelectionChange;
                }// recording section check    
                mRecordingSession = null;
                UpdateTimeDisplay();

                // optionally save project
                //SaveWhenRecordingEnds ();//@singleSection

                // makes phrase blocks invisible if these exceed max. visible blocks count during recording
                //mView.MakeOldStripsBlocksInvisible ( true); // @phraseLimit :@singleSection: legagy code commented

                // missed recorder notifications messages are written to log, if any
                if (m_MissedNotificationMessages != null && m_MissedNotificationMessages.Length > 1)
                {
                    WriteLogMsgForRecorderMissedNotification();
                }

                //@MonitorContinuously
                if (MonitorContinuously)
                {
                    /// avoiding use of delay at this time to prevent possible bug. It will be restored after alpha.
                    //StartMonitorContinuouslyWithDelay();
                    StartMonitorContinuously();
                }
                m_TimeElapsedInRecording = 0;
            }
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
                    bool isPlayOnSelectionChange = SelectionChangedPlaybackEnabled;
                    SelectionChangedPlaybackEnabled = false;
                    StopRecording();
                    SelectionChangedPlaybackEnabled = isPlayOnSelectionChange;
                }
                else
                {
                    // Stopping again deselects everything
                    if (mState == State.Stopped && !mView.ObiForm.Settings.Audio_DisableDeselectionOnStop)
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

                if (!(mCurrentPlaylist is PreviewPlaylist))
                {
                    mCurrentPlaylist = mMasterPlaylist; //@masternewbehaviour
                    PhraseNode currentPhrase = FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node);
                    if (currentPhrase != null)
                    {
                        mCurrentPlaylist.CurrentPhrase = currentPhrase;
                    }
                }
                else
                {
                    // if preview playlist, reset the flickering colors
                    if (mView.ObiForm.Settings.Audio_ColorFlickerPreviewBeforeRecording) mView.ResetColorAfterColorFlickering();
                    }
                UpdateButtons();
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
            if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
            if (mView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton && CurrentState != State.Monitoring) //if monitoring go through the traditional way
            {
                if (mView.ObiForm.Settings.Audio_AllowOverwrite && CurrentState == State.Playing) Pause(); //@RecordFromPlayback
                if (mView.ObiForm.Settings.Audio_UseRecordBtnToRecordOverSubsequentAudio
                    && !mView.ObiForm.Settings.Audio_Recording_PreviewBeforeStarting)
                {
                    
                    RecordOverSubsequentPhrases();
                }
                else
                {
                    StartRecordingDirectly(mView.ObiForm.Settings.Audio_Recording_PreviewBeforeStarting);
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

            
            if (mView.Presentation != null
                        &&    !IsMetadataSelected && ( mView.Selection == null || !(mView.Selection is TextSelection)))
            {
                if (CurrentState == State.Playing)//@RecordFromPlayback    
                {
                    if (mView.ObiForm.Settings.Audio_AllowOverwrite)
                    {
                        Pause(); 
                    }
                    else
                    {
                        return;
                    }
                }

            try
                {
                if (mState == State.Monitoring)
                    {
                        if (!MonitorContinuously)
                        {
                            mRecordingSession.Stop();
                            StartRecording();
                        }
                        else //@MonitorContinuously
                        {
                            StopMonitorContinuously();//@MonitorContinuously
                            SetupRecording(Recording, false);
                        }
                    }
                else if (CanResumeRecording)
                    {
                        if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
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
            bool ShouldRecordingContinue =  mView.RecreateContentsWhileInitializingRecording ( mResumeRecordingPhrase);
            if (!ShouldRecordingContinue)
            {
                return;
            }
            // if record section from first empty phrase is checked, do accordingly.
            if (mView.Selection != null && mView.Selection.Node is SectionNode && mResumeRecordingPhrase == null)
            {
                SectionNode section = mView.GetSelectedPhraseSection;
                if (mView.ObiForm.Settings.Audio_RecordAtStartingOfSectionWithRecordSectionCommand
                    && section.PhraseChildCount > 0 && !(section.PhraseChild(0) is PhraseNode) && (section.PhraseChild(0).Role_ == EmptyNode.Role.Page))
                {
                    bool hasEmptyPages = true;
                    for (int i = 0; i < section.PhraseChildCount; i++)
                    {
                        if (section.PhraseChild(i) is PhraseNode || section.PhraseChild(i).Role_ == EmptyNode.Role.Plain)
                        {
                            hasEmptyPages = false;
                        }
                    }
                    if (hasEmptyPages)
                    {
                        mView.SelectFromTransportBar(section.PhraseChild(0), null);
                        EmptyNode newEmptyNode = mView.Presentation.TreeNodeFactory.Create<EmptyNode>();
                        mView.Presentation.Do(new Obi.Commands.Node.AddEmptyNode(mView, newEmptyNode, section, 0));
                    }
                }
                if (mView.ObiForm.Settings.Audio_RecordInFirstEmptyPhraseWithRecordSectionCommand
                    && section.PhraseChildCount > 0 && !(section.PhraseChild(0) is PhraseNode) && (section.PhraseChild(0).Role_ == EmptyNode.Role.Plain))
                {
                        mView.SelectFromTransportBar(mView.GetSelectedPhraseSection.PhraseChild(0), null);
                }
            }
            if (deleteFollowingPhrases && CurrentState == State.Paused && PlaybackPhrase != null
                && mView.Selection != null && mView.Selection.Control is TOCView)
            {
                mView.SelectFromTransportBar(PlaybackPhrase, null);
            }

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
                        
                        //MessageBox.Show("recording selection update");   
                        double replaceStartTime = IsPlayerActive ? CurrentPlaylist.CurrentTimeInAsset:
                            mView.Selection is AudioSelection?( ((AudioSelection)mView.Selection).AudioRange.HasCursor? ((AudioSelection)mView.Selection).AudioRange.CursorTime : ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime ): 
                            selectionNode.Duration;

                        // adding a command for updating selection after intermediate selection changes, in order to hide the temporary selections being done for achieving the required behaviour.
                        if (mView.ObiForm.Settings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording) mView.Selection = new AudioSelection((PhraseNode)selectionNode, mView.Selection.Control, new AudioRange(replaceStartTime));
                            command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(mView, new NodeSelection(selectionNode, mView.Selection.Control)));

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
            InitRecordingSectionAndPhraseIndex ( node, mView.ObiForm.Settings.Audio_AllowOverwrite, command , deleteFollowingPhrases);
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
            // Holding the value in recording session because it is dependent on selection before recording
            mRecordingSession.Audio_DeleteFollowingPhrasesOfSectionAfterRecording = mView.ObiForm.Settings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording && CanRecordOnFollowingAudio;
                //&& (mView.Selection == null || !(mView.Selection is AudioSelection) || ((AudioSelection)mView.Selection).AudioRange.HasCursor) ;

            // Actually start monitoring or recording
            if (recording)
                {
                StartRecording ();
                }
            else
                {
                                     mRecordingSession.StartMonitoring ();
                                    
                if (mView.ObiForm.Settings.Audio_AudioClues) mVUMeterPanel.BeepEnable = true;
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
        bool isPhraseSplit = false;

            if (node is SectionNode)
            {
                // Record at the end of the section, or after the cursor
                // in case of a cursor selection in the section.
                mRecordingSection = (SectionNode)node;
                mRecordingInitPhraseIndex = mView.Selection is StripIndexSelection ?
                    ((StripIndexSelection)mView.Selection).Index : mRecordingSection.PhraseChildCount;
                if (mView.Selection is StripIndexSelection && mView.Selection.Node != null)
                {
                    AddTheDeleteSubsequentPhrasesCommand(mRecordingSection, deleteFollowingPhrases, false,command);
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

                    // behaviour modified as per request of VA
                    // Split command should not transfer properties to new phrase if the subsequent phrases are deleted. The following line is replaced by conditional split commands
                    //Commands.Node.SplitAudio.GetSplitCommand ( mView );
                CompositeCommand SplitCommand = deleteFollowingPhrases?
                    Commands.Node.SplitAudio.GetSplitCommand ( mView,false ):
                    Commands.Node.SplitAudio.GetSplitCommand ( mView );
                if (SplitCommand != null)
                {
                    command.ChildCommands.Insert(command.ChildCommands.Count, SplitCommand);
                    isPhraseSplit = true;
                }
                else if (SplitCommand == null && mView.ObiForm.Settings.Audio_AllowOverwrite
                    && CurrentState == State.Paused && mCurrentPlaylist.CurrentTimeInAsset == 0)
                {
                    // split does not work if the pause position is at 0.0s.
                    // Therefore, if we are in overwrite recording mode,  we need to move index to the selected phrase position, to ensure that the effect is like splitting at 0 position
                    mRecordingInitPhraseIndex = node.Index;
                    Console.WriteLine("Pause position was at 0");
                }

                                    if (mView.Selection is AudioSelection && !((AudioSelection)mView.Selection).AudioRange.HasCursor && SplitCommand != null)
                                        {
                                        command.ChildCommands.Insert (command.ChildCommands.Count,  new Commands.Node.DeleteWithOffset ( mView, node, 1 ) );
                                        // copy the properties to the new middle node only if the following phrases are not deleted. (VA request)
                                        // i.e. If the following phrases are deleted then the original phrase should retain properties
                                        if (!deleteFollowingPhrases)
                                        {
                                        m_IsAfterRecordingSplitTransferEnabled = true;
                                        CopyPropertiesForTransfer((EmptyNode)node);
                                    }
                                        
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
            if ((deleteFollowingPhrases)
                && node is EmptyNode && ((EmptyNode)node).Index < ((EmptyNode)node).ParentAs<SectionNode>().PhraseChildCount-1)
            {
                AddTheDeleteSubsequentPhrasesCommand(node, deleteFollowingPhrases,isPhraseSplit,command);
            }
        if (IsPlayerActive) StopPlaylistPlayback (); // stop if split recording starts while playback is paused

        }

        private void AddTheDeleteSubsequentPhrasesCommand(ObiNode node, bool deleteFollowingPhrases, bool isPhraseSplit,CompositeCommand command)
        {
            if (deleteFollowingPhrases)
            {
                int phraseIndex =(node != null &&   node is EmptyNode)? ((EmptyNode)node).Index + 1:
                    (mView.Selection != null && mView.Selection is StripIndexSelection )? ((StripIndexSelection)mView.Selection).Index: -1 ;
                SectionNode section = node != null && node is EmptyNode? ((EmptyNode)node).ParentAs<SectionNode>():
                    mView.Selection != null && mView.Selection is StripIndexSelection? (SectionNode)mView.Selection.Node: null ;
                //MessageBox.Show(phraseIndex.ToString());
                if (section == null || phraseIndex < 0 || phraseIndex >= section.PhraseChildCount) return;

                if (mView.ObiForm.Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio
                    && isPhraseSplit && node is ObiNode && ((EmptyNode)node).Role_ == EmptyNode.Role.Page)
                {
                    //command.ChildCommands.Insert(command.ChildCommands.Count,
                        //new Commands.Node.Delete(mView, section.PhraseChild(phraseIndex)));
                    //phraseIndex++;
                    //if (phraseIndex >= section.PhraseChildCount) return;
                    
                }

                command.ChildCommands.Insert(command.ChildCommands.Count, 
                    mView.GetDeleteRangeOfPhrasesInSectionCommand(section, section.PhraseChild(phraseIndex), section.PhraseChild(section.PhraseChildCount - 1),
                    mView.ObiForm.Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio, PhraseNode.Role.Page));
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
                        PhraseNode destinationPhrase = mCurrentPlaylist.PrevSection(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node));
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
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
                        PhraseNode destinationPhrase = mCurrentPlaylist.PrevPage(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node));
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
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
                        // if strip index is selected then select the equivalent phrase block.
                        if (mView.Selection != null
                            && mView.Selection.Node is SectionNode
                            && mView.Selection is StripIndexSelection)
                        {
                            if (((StripIndexSelection)mView.Selection).Index >= ((SectionNode)mView.Selection.Node).PhraseChildCount)
                            {
                                SectionNode section = (SectionNode)mView.Selection.Node;
                                mView.SelectPhraseBlockOrStrip(section.PhraseChild(section.PhraseChildCount - 1));
                            }
                            else
                            {
                                mView.SelectPhraseBlockOrStrip (mView.Selection.EmptyNodeForSelection);
                            }
                        }
                            
                        // older code continues below
                        PhraseNode destinationPhrase = mCurrentPlaylist.PrevPhrase(
                                                    FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node));
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
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
                    try
                    {
                        //@MonitorContinuously
                        if (MonitorContinuously)
                        {
                            StopMonitorContinuously();
                            Record_Button();
                        }
                        else
                        {
                            mRecordingSession.Stop();

                            mVUMeterPanel.BeepEnable = false;
                            mRecordingSession.Record();
                            mDisplayTimer.Start();
                        }
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
                        // if strip index is selected then select the equivalent phrase block.
                        if (mView.Selection != null
                            && mView.Selection.Node is SectionNode
                            && mView.Selection is StripIndexSelection)
                        {
                            if (((StripIndexSelection)mView.Selection).Index >= ((SectionNode)mView.Selection.Node).PhraseChildCount)
                            {
                                SectionNode section = (SectionNode)mView.Selection.Node;
                                mView.SelectPhraseBlockOrStrip(section.PhraseChild(section.PhraseChildCount - 1));
                            }
                            else
                            {
                                mView.SelectPhraseBlockOrStrip(mView.Selection.EmptyNodeForSelection);
                            }
                        }

                        // older code continues below
                        PhraseNode startPhrase = FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node);
                        PhraseNode destinationPhrase = null;
                        if (mView.Selection != null && mView.Selection.Node is EmptyNode && !(mView.Selection.Node is PhraseNode))
                        {
                            destinationPhrase = startPhrase;
                        }
                        else
                        {
                            destinationPhrase = mCurrentPlaylist.NextPhrase(startPhrase);
                        }
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
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
                    if (mRecordingSection != null && mRecordingSection.PhraseChildCount < mView.MaxVisibleBlocksCount) // @phraseLimit
                    {
                        // check if creation of pages is disabled. If yes, then should proceed only if the next phrase is empty page.
                        if (mView.ObiForm.Settings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording )
                        {
                            if (mRecordingPhrase.FollowingNode == null) return false;
                            if (!(mRecordingPhrase.FollowingNode is EmptyNode)) return false;
                            // if next node is empty node then move to following 
                                EmptyNode nextEmptyNode = (EmptyNode)mRecordingPhrase.FollowingNode;
                                if (nextEmptyNode.Parent != mRecordingPhrase.Parent) return false;
                                if (nextEmptyNode.Duration > 0) return false;
                                if (nextEmptyNode.Role_ != EmptyNode.Role.Page) return false;
                            
                        }   

                        mRecordingSession.MarkPage();
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
                        mCurrentPlaylist.NavigateToNextPage();
                    }
                    else
                    {
                        PhraseNode destinationPhrase = mCurrentPlaylist.NextPage(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node));
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
                        // assignment to current playlist should happen before selection for calling update button function through selection change event. For now update button is explicitly called, but it will be corrected in next release
                        UpdateButtons();
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
                    // first check if creation of next heading is allowed. If not, then should proceed only if the next heading is empty
                    if (mView.ObiForm.Settings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording
                        && mRecordingSection.FollowingSection != null && mRecordingSection.FollowingSection.Duration > 0)
                    {
                        return false;
                    }
                    //mRecordingSession.AudioRecorder.TimeOfAsset
                    double timeOfAssetMilliseconds =
                   (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64( mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
                   Time.TIME_UNIT;

                    if (mRecordingPhrase != null && mRecordingSession != null
                        && timeOfAssetMilliseconds < 250) return false;
                    m_EnablePostRecordingPageRenumbering = false;
                    bool isPlayOnSelectionChange = SelectionChangedPlaybackEnabled;
                    SelectionChangedPlaybackEnabled = false;
                    PauseRecording();
                    SelectionChangedPlaybackEnabled = isPlayOnSelectionChange;
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
                            if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
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
                            if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
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
                        PhraseNode destinationPhrase = mCurrentPlaylist.NextSection(
                            FindPlaybackStartNode(mView.Selection == null ? null : mView.Selection.Node));
                        mView.SelectPhraseInContentView(destinationPhrase);
                        mCurrentPlaylist.CurrentPhrase = destinationPhrase; //@masternewbehaviour
                        // assignment to current playlist should happen before selection for calling update button function through selection change event. For now update button is explicitly called, but it will be corrected in next release
                        UpdateButtons();
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
            m_ElapseBackInterval = mView.ObiForm.Settings.Audio_ElapseBackTimeInMilliseconds;
            // work around to handle special nudge condition of preview: this should be implemented universally after 2.0 release
            if (mCurrentPlaylist != null && mView.Selection is AudioSelection && mCurrentPlaylist is PreviewPlaylist && CurrentState == State.Paused) Stop();
            if (IsPlayerActive)
            {
                // if current playback phrase is not selected, hthen select it.
                if (mView.Selection == null
                    || (CurrentPlaylist.CurrentPhrase != null &&  CurrentPlaylist.CurrentPhrase != mView.Selection.Node))
                {
                    mView.SelectFromTransportBar(mCurrentPlaylist.CurrentPhrase, null);
                }


                if (((IsPaused && mCurrentPlaylist.CurrentTimeInAsset <= 10) || (mView.ObiForm.Settings.PlayOnNavigate && CurrentState == State.Playing && mCurrentPlaylist.CurrentTimeInAsset <= 800))
                    && (mView.Selection.Node.PrecedingNode is PhraseNode || mView.Selection.Node.PrecedingNode is EmptyNode) && !mView.IsZoomWaveformActive)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate && CurrentState == State.Playing && mCurrentPlaylist.CurrentTimeInAsset <= 800) Pause();
                    LapseBackCursor();
                    return true;
                }
                else
                {
                    DetermineUseOfSoundTouch(1.0f);
                    mCurrentPlaylist.FastPlayNormaliseWithLapseBack(m_ElapseBackInterval);
                    mFastPlayRateCombobox.SelectedIndex = 0;
                    UpdateTimeDisplay();
                    if (mCurrentPlaylist.CurrentTimeInAsset <= 10)
                    {
                        this.Pause();
                        PhraseNode prevPhraseNode = mCurrentPlaylist.PrevPhrase(mCurrentPlaylist.CurrentPhrase);
                        if (mView.Selection != null && mView.Selection.Node is PhraseNode)
                        {
                            if (prevPhraseNode != null)
                            {
                                LapseBackCursor(); 
                                this.PlayOrResume();
                                return true;
                            }
                            else
                            {
                                LapseBackCursor();
                                return true;
                            }
                        }
                    }
  
                    if (CurrentPlaylist != null) mView.UpdateCursorPosition(mCurrentPlaylist.CurrentTimeInAsset);
                    if (mView.Selection is AudioSelection)
                    {
                        if (((AudioSelection)mView.Selection).AudioRange.HasCursor)
                        {
                            if (((AudioSelection)mView.Selection).AudioRange.CursorTime != mCurrentPlaylist.CurrentTimeInAsset)
                            {
                                ((AudioSelection)mView.Selection).AudioRange.CursorTime = mCurrentPlaylist.CurrentTimeInAsset;
                            }

                        }
                    }
                    return true;
                }
            }
            else if (CurrentState == State.Stopped && mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                LapseBackCursor();

                return true;
            }
            return false;
        }

        public bool StepForward()
        {
            m_ElapseBackInterval = mView.ObiForm.Settings.Audio_ElapseBackTimeInMilliseconds;
            // work around to handle special nudge condition of preview: this should be implemented universally after 2.0 release
            if (mCurrentPlaylist != null && mView.Selection is AudioSelection && mCurrentPlaylist is PreviewPlaylist && CurrentState == State.Paused) Stop();
            if (IsPlayerActive)
            {

                if ((IsPaused || (mView.ObiForm.Settings.PlayOnNavigate && CurrentState == State.Playing ))
                    && (mView.Selection.Node.FollowingNode is PhraseNode || mView.Selection.Node.FollowingNode is EmptyNode) && (mView.Selection.Node.Parent == mView.Selection.Node.FollowingNode.Parent) && (mCurrentPlaylist.CurrentTimeInAsset >= (mView.Selection.Node.Duration - m_ElapseBackInterval)) && !mView.IsZoomWaveformActive)
                {
                    if (mView.ObiForm.Settings.PlayOnNavigate && CurrentState == State.Playing) Pause();
                    StepForwardCursor();
                    return true;
                }
                else
                {
                    //DetermineUseOfSoundTouch(1.0f);
                    mCurrentPlaylist.StepForward(m_ElapseBackInterval, mCurrentPlaylist.CurrentPhrase != null ? mCurrentPlaylist.CurrentPhrase.Duration : mView.Selection.Node.Duration);
                    
                    UpdateTimeDisplay();
                    if (CurrentPlaylist != null)
                    {
                        mView.UpdateCursorPosition(mCurrentPlaylist.CurrentTimeInAsset);                       
                    }
                    if (mView.Selection is AudioSelection)
                    {
                        if (((AudioSelection)mView.Selection).AudioRange.HasCursor)
                        {
                            if (((AudioSelection)mView.Selection).AudioRange.CursorTime != mCurrentPlaylist.CurrentTimeInAsset)
                            {
                                ((AudioSelection)mView.Selection).AudioRange.CursorTime = mCurrentPlaylist.CurrentTimeInAsset;
                            }

                        }
                    }
                    return true;
                }
            }
            else if (CurrentState == State.Stopped && mView.Selection != null && mView.Selection.Node is PhraseNode)
            {
                StepForwardCursor();

                return true;
            }
            return false;
        }

        private void LapseBackCursor()
        {
            if (IsPaused)
            {
                double time = mCurrentPlaylist.CurrentTimeInAsset;
               
                if (mView.Selection.Node.PrecedingNode != null && mView.Selection.Node.PrecedingNode is PhraseNode)
                {
                    Stop();
                   // mView.ClearCursor();
                    mView.Selection = new NodeSelection(mView.Selection.Node.PrecedingNode, mView.Selection.Control);
                    
                    AudioRange range = new AudioRange(mView.Selection.Node.Duration);
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control, range);
                    if (mView.ObiForm.Settings.Audio_AudioClues)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }
            }
            if (mView.Selection is AudioSelection)
            {
                double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;

                if (time < 1 && !mView.IsZoomWaveformActive && ((mView.Selection.Node.PrecedingNode is PhraseNode) || (mView.Selection.Node.PrecedingNode is EmptyNode)))
                {
                    ObiNode preceedingNode = mView.Selection.Node.PrecedingNode;
                    mView.Selection = new NodeSelection(preceedingNode, mView.Selection.Control);
                    AudioRange range = new AudioRange(mView.Selection.Node.Duration);
                    while (mView.Selection.Node is EmptyNode && !(mView.Selection.Node is PhraseNode))
                    {
                        preceedingNode = mView.Selection.Node.PrecedingNode;
                        mView.Selection = new NodeSelection(preceedingNode, mView.Selection.Control);
                        range = new AudioRange(mView.Selection.Node.Duration);
                    }
                    Console.WriteLine("Current time in Asset {0}", mCurrentPlaylist.CurrentTimeInAsset);
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control, range);
                    time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                    if (mView.ObiForm.Settings.Audio_AudioClues)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }

                time = time - m_ElapseBackInterval >= 0 ? time - m_ElapseBackInterval : 0;

                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(time));
            }
            else if(!IsPaused)
            {
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(((PhraseNode)mView.Selection.EmptyNodeForSelection).Duration - m_ElapseBackInterval));
            }
        }
        private void StepForwardCursor()
        {
            bool flag = false;
            if (IsPaused)
            {
                double time = mCurrentPlaylist.CurrentTimeInAsset;

                if (mView.Selection.Node.FollowingNode != null && mView.Selection.Node.FollowingNode is PhraseNode)
                {
                    Stop();
                    // mView.ClearCursor();
                    double diff = mView.Selection.Node.Duration - time;
                    diff = m_ElapseBackInterval - diff;
                    mView.Selection = new NodeSelection(mView.Selection.Node.FollowingNode, mView.Selection.Control);

                    AudioRange range = new AudioRange(diff);
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control, range);
                    flag = true;
                    if (mView.ObiForm.Settings.Audio_AudioClues)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }
            }
            if (mView.Selection is AudioSelection)
            {
                double time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;

                if ((time + m_ElapseBackInterval) >= mView.Selection.Node.Duration && !mView.IsZoomWaveformActive && ((mView.Selection.Node.FollowingNode is PhraseNode) || (mView.Selection.Node.FollowingNode is EmptyNode)) && (mView.Selection.Node.Parent==mView.Selection.Node.FollowingNode.Parent))
                {
                    ObiNode followingNode = mView.Selection.Node.FollowingNode;
                    double diff = mView.Selection.Node.Duration - time;
                    diff = m_ElapseBackInterval - diff;
                    mView.Selection = new NodeSelection(followingNode, mView.Selection.Control);
                    AudioRange range = new AudioRange(diff);
                    while (mView.Selection.Node is EmptyNode && !(mView.Selection.Node is PhraseNode))
                    {
                        followingNode = mView.Selection.Node.FollowingNode;
                        mView.Selection = new NodeSelection(followingNode, mView.Selection.Control);
                        range = new AudioRange(diff);
                    }
                    mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control, range);
                    time = ((AudioSelection)mView.Selection).AudioRange.CursorTime;
                    if (time > mView.Selection.Node.Duration)
                    {
                        time = mView.Selection.Node.Duration;
                    }
                    //mView.UpdateCursorPosition(time);
                    if (mView.ObiForm.Settings.Audio_AudioClues)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }
                else if (((time + m_ElapseBackInterval) <= mView.Selection.Node.Duration) && !flag)
                {
                    time = time + m_ElapseBackInterval;
                }
                else if(!flag)
                {
                    time = mView.Selection.Node.Duration;
                }

                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(time));
            }
            else if (!IsPaused)
            {             
                mView.Selection = new AudioSelection((PhraseNode)mView.Selection.Node, mView.Selection.Control,
                    new AudioRange(((PhraseNode)mView.Selection.EmptyNodeForSelection).Duration));
            }
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
            double nudge = mView.ObiForm.Settings.Audio_NudgeTimeMs * (forward ? 1 : -1);
            
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

            double nudgeDuration = mView.ObiForm.Settings.Audio_NudgeTimeMs ;

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
                    

                    PhraseNode node = mCurrentPlaylist is PreviewPlaylist ? ((PreviewPlaylist)mCurrentPlaylist).RevertPhrase: 
                        mCurrentPlaylist.CurrentPhrase;
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
                if (!(mView.Selection is AudioSelection)) return false;

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
            //if (from < 0.0)
            //{
                //duration += from;
                //from = 0.0;
            //}
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
            // Aug 12, 2015 check added for kind of playlist. The pause here is to ensure that playback does not starts due to some event, when play on navigate is checked.
                if (mState == State.Playing && !(mCurrentPlaylist is PreviewPlaylist )) Pause();

                SelectionChangedPlaybackEnabled = playOnSelectionStatus ;
                        }


        #region undoable recording



        System.ComponentModel.BackgroundWorker m_PreviewBeforeRecordingWorker = null;
        /// <summary>
        /// Start recording directly without going through listening
        /// </summary>
        public void StartRecordingDirectly(bool isPreviewBeforeRecording)
        {
            if (isPreviewBeforeRecording && mView.ObiForm.Settings.Audio_AllowOverwrite
                && m_PreviewBeforeRecordingWorker != null && m_PreviewBeforeRecordingWorker.IsBusy)
            {
                return;
            }
            if (mView.IsZoomWaveformActive)
            {
                return;
            }

            if (isPreviewBeforeRecording && mCurrentPlaylist.Audioplayer.PlaybackFwdRwdRate != 0)
            {
                return;
            }
            if (mView.ObiForm.Settings.Audio_AllowOverwrite && CurrentState == State.Playing) //@recordFromPlay
            {
                Pause();
                if (isPreviewBeforeRecording)
                {
                if (mView.Selection == null || !(mView.Selection.Node is EmptyNode) || mView.Selection.Node != mCurrentPlaylist.CurrentPhrase) return;
            }
            }

            // special handling if the time is at 0 s. This code should be removed by doing adequate improvements in preview playlist.
            if (isPreviewBeforeRecording && mView.Selection != null
                && mView.ObiForm.Settings.Audio_AllowOverwrite)
            {
                double time = -1;

                time = IsPlayerActive ? mCurrentPlaylist.CurrentTimeInAsset :
                    mView.Selection is AudioSelection ?
                    (((AudioSelection)mView.Selection).AudioRange.HasCursor ? ((AudioSelection)mView.Selection).AudioRange.CursorTime : ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime) :
                    -1;
                // If the preceeding phrase is in the same section then place the cursor at end of preceeding phrase
                // if preceeding phrase is not appropriate then select the strip index, start recording and  and return
                if (time == 0)
                {
                    ObiNode proceedingNode = mView.Selection.Node.PrecedingNode;
                    if (proceedingNode != null 
                        && proceedingNode.Parent == mView.Selection.Node.Parent
                        && proceedingNode is EmptyNode && ((EmptyNode)proceedingNode).Duration > 0)
                    {
                        mView.Selection = new AudioSelection((PhraseNode)proceedingNode, mView.Selection.Control,
                           new AudioRange(proceedingNode.Duration));
                        
                    }
                    else
                    {
                        mView.Selection = new StripIndexSelection(mView.Selection.Node.ParentAs<SectionNode>(), mView.Selection.Control, mView.Selection.Node.Index);
                        StartRecordingDirectly_Internal(true);
                        return;
                    }
                }
            }

            bool Status_SelectionChangedPlaybackEnabled = SelectionChangedPlaybackEnabled;
            if (isPreviewBeforeRecording && mView.ObiForm.Settings.Audio_AllowOverwrite
               && ((CurrentState == State.Paused && !(mView.Selection is AudioSelection)) || (mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange.HasCursor)))
            {
                // first delete the subsequent phrases in the section
                try
                {
                    if (SelectionChangedPlaybackEnabled) SelectionChangedPlaybackEnabled = false;
                    EmptyNode selectedNode = mView.Selection != null && mView.Selection.Node is EmptyNode ? (EmptyNode)mView.Selection.Node : null;
                    NodeSelection currentSelection = mView.Selection;
                    double time = -1;
                    if (selectedNode != null)
                    {
                        time = IsPlayerActive ? mCurrentPlaylist.CurrentTimeInAsset :
                            mView.Selection is AudioSelection ?
                            (((AudioSelection)mView.Selection).AudioRange.HasCursor ? ((AudioSelection)mView.Selection).AudioRange.CursorTime : ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime) :
                            -1;
                        
                        // Specific request of SBS: restore blue audio selection even if in pause state, so that blue cursor is visible after undo.
                        if (time >= 0 && mView.ObiForm.Settings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording&& !(mView is AudioSelection))
                        {
                            mView.Selection = new AudioSelection((PhraseNode)selectedNode, currentSelection.Control,
                               new AudioRange(time));
                        }
                    }

                    if (selectedNode != null && selectedNode is PhraseNode && selectedNode.Index < selectedNode.ParentAs<SectionNode>().PhraseChildCount - 1
                        && mView.ObiForm.Settings.Audio_DeleteFollowingPhrasesWhilePreviewBeforeRecording)
                    {
                        SectionNode section = selectedNode.ParentAs<SectionNode>();
                        Command deleteFollowingCmd = mView.GetDeleteRangeOfPhrasesInSectionCommand(
                            section, section.PhraseChild(selectedNode.Index + 1), section.PhraseChild(section.PhraseChildCount - 1),
                            mView.ObiForm.Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio, PhraseNode.Role.Page);
                        mView.Presentation.Do(deleteFollowingCmd);
                    }
                    else
                    {
                        // selection is automatically saved if delete command is executed. But it should also be saved if delete command is bypassed.
                        Commands.UpdateSelection selectionUpdateCmd = new Obi.Commands.UpdateSelection(mView, mView.Selection);
                        mView.Presentation.Do(selectionUpdateCmd);
                    }
                    if (time >= 0)
                    {
                        mView.Selection = new AudioSelection((PhraseNode)selectedNode, currentSelection.Control,
                           new AudioRange(time));
                    }
                }
                catch (System.Exception ex)
                {
                    mView.WriteToLogFile(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
                NodeSelection prevSelection = mView.Selection;
                m_PreviewBeforeRecordingWorker = new System.ComponentModel.BackgroundWorker();
                m_PreviewBeforeRecordingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    m_PreviewBeforeRecordingActive = true;
                    
                    double time = IsPlayerActive ? mCurrentPlaylist.CurrentTimeInAsset :
                            mView.Selection is AudioSelection ?
                            (((AudioSelection)mView.Selection).AudioRange.HasCursor ? ((AudioSelection)mView.Selection).AudioRange.CursorTime : ((AudioSelection)mView.Selection).AudioRange.SelectionBeginTime) :
                            -1;
                    if (time > 0)
                    {
                        Preview(Upto, IsPlayerActive ? UseAudioCursor : UseSelection);
                        int interval = 50;
                        for (int i = 0; i < (PreviewDuration * 2) / interval; i++)
                        {
                            if (mCurrentPlaylist is PreviewPlaylist && ((PreviewPlaylist)mCurrentPlaylist).IsPreviewComplete)
                            {
                                //System.Media.SystemSounds.Asterisk.Play();
                                Console.WriteLine(i);
                                break;
                            }
                            Thread.Sleep(interval);
                        }
                    }
                    //if (CurrentState == State.Paused && mCurrentPlaylist is PreviewPlaylist && ((PreviewPlaylist)mCurrentPlaylist).RevertTime == 0
                        //&& ((PreviewPlaylist)mCurrentPlaylist).RevertPhrase != mCurrentPlaylist.CurrentPhrase)
                    //{
                        //mCurrentPlaylist.CurrentPhrase = ((PreviewPlaylist)mCurrentPlaylist).RevertPhrase;
                    //}
                });
                m_PreviewBeforeRecordingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
                {
                    m_PreviewBeforeRecordingActive = false;
                    if (CurrentState == State.Paused)
                    {
                        if (prevSelection.Node == mView.Selection.Node)
                        {
                            if (mResumeRecordingPhrase != null) mResumeRecordingPhrase = null;
                            if (mView.ObiForm.Settings.Audio_ColorFlickerPreviewBeforeRecording) mView.ResetColorAfterColorFlickering();

                            if (mView.ObiForm.Settings.Audio_DeleteFollowingPhrasesWhilePreviewBeforeRecording)
                                StartRecordingDirectly_Internal(true);
                            else
                                StartRecordingDirectly_Internal(false);
                        }
                        else
                        {
                            if (mView.ObiForm.Settings.Audio_ColorFlickerPreviewBeforeRecording) mView.ResetColorAfterColorFlickering();
                            if (CurrentState == State.Paused) Stop();
                            MessageBox.Show(Localizer.Message("PreviewBeforeRecording_SelectionChanged"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    if (SelectionChangedPlaybackEnabled != Status_SelectionChangedPlaybackEnabled)
                        SelectionChangedPlaybackEnabled = Status_SelectionChangedPlaybackEnabled;
                    
                });
                m_PreviewBeforeRecordingWorker.RunWorkerAsync();
                
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
                        if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
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
                bool IsDataMissingException = false;
                bool wasMonitoring = mRecordingSession.AudioRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring;
                mVUMeterPanel.BeepEnable = false;
                List<PhraseNode> listOfRecordedPhrases = new List<PhraseNode>();
                EmptyNode firstRecordedPage = null;
                try
                {
                    mRecordingSession.Stop();


                    if (mRecordingSection != null) ///@MonitorContinuously , if block inserted to bypass the procedure of assigning assets
                    {
                        // update phrases with audio assets
                        UpdateRecordedPhrasesAlongWithPostRecordingOperations(listOfRecordedPhrases, ref firstRecordedPage);

                        //Workaround to force phrases to show if they become invisible on stopping recording
                        mView.PostRecording_RecreateInvisibleRecordingPhrases(mRecordingSection, mRecordingInitPhraseIndex, mRecordingSession.RecordedAudio.Count);
                        EmptyNode lastRecordedPhrase = mRecordingSection.PhraseChildCount > 0 ? mRecordingSection.PhraseChild(mRecordingInitPhraseIndex + mRecordingSession.RecordedAudio.Count - 1) : null;
                        if (!wasMonitoring && lastRecordedPhrase != null && lastRecordedPhrase.IsRooted) mView.SelectFromTransportBar(lastRecordedPhrase, null);
                    }


                }
                catch (System.Exception ex)
                {                   
                    if (ex is urakawa.exception.DataMissingException || ex is System.IO.DirectoryNotFoundException)
                    {
                        IsDataMissingException = true;
                    }
                    MessageBox.Show(Localizer.Message("TransportBar_ErrorInStopRecording") + "\n\n" + ex.ToString(), Localizer.Message("Caption_Error"));
                }

                    if (IsDataMissingException)
                    {
                        mView.ReplacePhrasesWithImproperAudioWithEmptyPhrases((ObiNode)mView.Presentation.RootNode, true);
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
                if (mRecordingSection != null && mView.Selection != null && !mRecordingSection.IsRooted && mView.Selection.Node == mRecordingSection) mView.Selection = null;
            }
        else if (mResumeRecordingPhrase != null)
            {
            mRecordingSession = null;
            mResumeRecordingPhrase = null;

            }
            // missed recorder notification messages are written to log, if any
            if (m_MissedNotificationMessages != null && m_MissedNotificationMessages.Length > 1)
            {
                WriteLogMsgForRecorderMissedNotification();
            }

            if (MonitorContinuously)
            {
                /// avoiding use of delay at this time to prevent possible bug. It will be restored after alpha.
                //StartMonitorContinuouslyWithDelay();
                StartMonitorContinuously();
            }
            else if (mView.ObiForm.Settings.Audio_AutoPlayAfterRecordingStops)
            {
                // if monitoring is not enabled and auto play after recording is checked.
                PlayOrResume_Safe();
            }
            m_TimeElapsedInRecording = 0;
        }

        private void UpdateRecordedPhrasesAlongWithPostRecordingOperations(List<PhraseNode> listOfRecordedPhrases,ref EmptyNode firstRecordedPage)
        {
            if (mRecordingSession!=null && mRecordingSession.RecordedAudio != null && mRecordingSession.RecordedAudio.Count > 0)
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

            // delete the following phrases before going into more complex commands
            if (mRecordingSession.Audio_DeleteFollowingPhrasesOfSectionAfterRecording && listOfRecordedPhrases != null && listOfRecordedPhrases.Count > 0)
            {
                EmptyNode lastRecordedPhrase = listOfRecordedPhrases[listOfRecordedPhrases.Count - 1] ;
                SectionNode section = lastRecordedPhrase.ParentAs<SectionNode> () ;
                if ( lastRecordedPhrase.IsRooted && lastRecordedPhrase.Index < section.PhraseChildCount -1)
                {
                Command deleteFollowingCmd =  mView.GetDeleteRangeOfPhrasesInSectionCommand(
                    section, section.PhraseChild(lastRecordedPhrase.Index+1), section.PhraseChild(section.PhraseChildCount - 1),
                    mView.ObiForm.Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio, PhraseNode.Role.Page);
                
                mView.Presentation.Do(deleteFollowingCmd);
                }
            }

            // on the fly phrase detection
            if (mRecordingSession != null && mRecordingSession.PhraseMarksOnTheFly != null && mRecordingSession.PhraseMarksOnTheFly.Count > 0)
            {
                if (IsPlaying) Pause();

                EmptyNode lastPhrase = listOfRecordedPhrases.Count > 0 ? listOfRecordedPhrases[listOfRecordedPhrases.Count - 1] : null;//@AdvanceRecording
                EmptyNode nextToLastPhrase = null;
                if (lastPhrase != null)
                {
                    nextToLastPhrase = lastPhrase.Index < lastPhrase.ParentAs<SectionNode>().PhraseChildCount - 1 ? (EmptyNode)lastPhrase.FollowingNode :
                        null;//@AdvanceRecording
                }
                if (mView.ObiForm.Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetectionWhileRecording
                    && mRecordingSession.PhraseMarksOnTheFly.Count > 1)
                {
                    mRecordingSession.PhraseMarksOnTheFly.RemoveAt(0);
                    Console.WriteLine("Merging first 2 phrases aftre recording");
                }

                mView.Presentation.Do(GetSplitCommandForOnTheFlyDetectedPhrases(listOfRecordedPhrases, mRecordingSession.PhraseMarksOnTheFly, mView.ObiForm.Settings.Audio_PreventSplittingPages));

                if (!mView.ObiForm.Settings.Audio_PreventSplittingPages)
                {
                    CompositeCommand multipleMergePhraseCommand = GetMultiplePhrasesMergeCommand(listOfRecordedPhrases);
                    if (multipleMergePhraseCommand.ChildCommands.Count > 0)
                    {
                        mView.Presentation.Do(multipleMergePhraseCommand);
                    }
                }

                if (nextToLastPhrase != null && nextToLastPhrase.IsRooted && nextToLastPhrase.Index > 0)//@advanceRecording
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

        private CompositeCommand GetSplitCommandForOnTheFlyDetectedPhrases(List<PhraseNode> phrasesList, List<double> timingList, bool preventSplittingPages)
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

                if ((preventSplittingPages && phrase.Role_ == EmptyNode.Role.Page) || phrase.TODO || !phrase.Used) continue;

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
                    if ( splitTimeInPhrase <= 200 || splitTimeInPhrase>= (phrase.Duration - 200) ) continue ;
                    CompositeCommand split = Commands.Node.SplitAudio.GetSplitCommand(mView, phrase,splitTimeInPhrase );
                    multipleSplitCommand.ChildCommands.Insert(multipleSplitCommand.ChildCommands.Count, split);
                    newTimingList.RemoveAt(j);
                }

            }
            // mark to do to first phrase if retain initial silence is false
            if (!mView.ObiForm.Settings.Audio_RetainInitialSilenceInPhraseDetection
                && phrasesList.Count > 0
                && multipleSplitCommand.ChildCommands.Count > 0)
            {
                Commands.Node.ToggleNodeTODO todoMarkCmd = new Obi.Commands.Node.ToggleNodeTODO(mView, phrasesList[0]);
                todoMarkCmd.UpdateSelection = false;
                multipleSplitCommand.ChildCommands.Insert(multipleSplitCommand.ChildCommands.Count, todoMarkCmd);
            }
            return multipleSplitCommand;
        }

        private CompositeCommand GetMultiplePhrasesMergeCommand(List<PhraseNode> listOfRecordedPhrases)
        {
            CompositeCommand multipleMergePhraseCommand = mView.Presentation.CreateCompositeCommand("Merge multiple phrases");
            foreach (PhraseNode n in listOfRecordedPhrases)
            {
                ObiNode nextObiNode = n.FollowingNode;
                if (nextObiNode != null && nextObiNode is PhraseNode && !listOfRecordedPhrases.Contains((PhraseNode)nextObiNode) && nextObiNode.Parent == n.Parent)
                {
                    PhraseNode nextPhraseNode = (PhraseNode)nextObiNode;
                    Commands.Node.MergeAudio mergeCmd = new Obi.Commands.Node.MergeAudio(mView, n, nextPhraseNode);
                    mergeCmd.UpdateSelection = false;
                    multipleMergePhraseCommand.ChildCommands.Insert(multipleMergePhraseCommand.ChildCommands.Count,
                        mergeCmd);
                    Console.WriteLine(n.ToString());
                }
            }
            return multipleMergePhraseCommand;
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
        public void MarkTodo(bool IsCommentAdded = false)
        {
            EmptyNode node = null;
            if (IsRecording)
            {
                node = (EmptyNode)mRecordingPhrase;

                mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                double previousTimeElaped = m_TimeElapsedInRecording;
                m_TimeElapsedInRecording =
                            (double)mRecordingSession.AudioRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(mRecordingSession.AudioRecorder.CurrentDurationBytePosition)) /
                            Time.TIME_UNIT;
                double currentTimeElapsed = m_TimeElapsedInRecording - previousTimeElaped - 100;
                mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node, currentTimeElapsed));
                mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                if (!IsCommentAdded)
                NextPhrase();
            }
            else if (IsPlayerActive)
            {
                node = (EmptyNode)mCurrentPlaylist.CurrentPhrase;
                if (node != null)
                {

                    if (node.CommentText != null && node.CommentText != string.Empty)
                        mView.Presentation.UndoRedoManager.Execute(new Commands.Node.AddComment(mView, node, null));

                    mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    if (node is PhraseNode && node.Duration > 0 && mCurrentPlaylist != null)
                    {
                        mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node, mCurrentPlaylist.CurrentTimeInAsset));                       
                }
                else
                    {
                    mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node));
                }

                    mView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                }

            }
        else if (mView.Selection != null && mView.Selection.Node is EmptyNode)
            {
            node  = (EmptyNode)mView.Selection.Node;
            mView.Presentation.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
            if (node != null && node is PhraseNode && node.Duration > 0 && mView.Selection is AudioSelection)
            {
                AudioSelection selection = mView.Selection as AudioSelection;
                double todoTime = selection.AudioRange.HasCursor ? selection.AudioRange.CursorTime :
                    selection.AudioRange.SelectionBeginTime;
                if (todoTime > 0 && todoTime < node.Duration)
                {
                    if (node.CommentText != null && node.CommentText != string.Empty)
                        mView.Presentation.UndoRedoManager.Execute(new Commands.Node.AddComment(mView, node, null));

                    mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node, todoTime));
                }
            }
            else
            {
                if (node.CommentText != null && node.CommentText != string.Empty)
                    mView.Presentation.UndoRedoManager.Execute(new Commands.Node.AddComment(mView, node, null));

                mView.Presentation.UndoRedoManager.Execute(new Commands.Node.ToggleNodeTODO(mView, node));
            }
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
                if (mView.ObiForm.Settings.Audio_ShowSelectionTimeInTransportBar && mState == State.Stopped)
                {
                    UpdateTimeDisplay();
                }
                                        }// end of selection null check

        }

        public void PlayHeadingPhrase( SectionNode node     )
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
            if ( mView.ObiForm.Settings.Audio_AudioClues )
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
                        if (mView.ObiForm.Settings.Audio_AudioClues &&  System.IO.File.Exists(navigationOnClue))
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
                        if(mView.ObiForm.Settings.Audio_AudioClues)  System.Media.SystemSounds.Exclamation.Play();
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
                if (MonitorContinuously) StopMonitorContinuously(); //@MonitorContinuously
                SetupRecording(Monitoring, false); 
            }
        }

        private void RecordingOptions_RecordWithDeleteFollowing_Click(object sender, EventArgs e)
        {
            RecordOverSubsequentPhrases();
        }

        public void RecordOverSubsequentPhrases()
        {
            if (CanRecord )
            {
                if (mView.ObiForm.Settings.Audio_AllowOverwrite && CurrentState == State.Playing) Pause(); //@recordFromPlay
                //StartRecordingDirectly_Internal(true); //@deleterecording
                StartRecordingDirectly_Internal(CanRecordOnFollowingAudio);
            }
        }

        private void m_RecordingtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Record();
        }

        private void m_RecordingOptionsContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsPreviewBeforeRecordingEnabled)           
            {
                mPreviewBeforeRecToolStripMenuItem.Enabled = true;
            }
            else
            {
                mPreviewBeforeRecToolStripMenuItem.Enabled = false;
            }

            if (mView.ObiForm.Settings.Audio_AllowOverwrite)
            {
                m_DeletePhrasestoolStripMenuItem.Enabled = !IsListening;
            }
            else
            {
                m_DeletePhrasestoolStripMenuItem.Enabled = false;
            }

            mMonitorContinuouslyToolStripMenuItem.Checked = MonitorContinuously;//@MonitorContinuously

        }

        public string GetPredefinedProfilesDirectory()
        {
            string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string defaultProfilesDirectory = System.IO.Path.Combine(appDirectory, "profiles");
            return defaultProfilesDirectory;
        }

        public string GetCustomProfilesDirectory()
        {
            string ProfileDirectory = System.IO.Directory.GetParent(Settings_Permanent.GetSettingFilePath()).ToString();
            string customProfilesDirectory = System.IO.Path.Combine(ProfileDirectory, "profiles");
            return customProfilesDirectory;
        }
        public string[] ProfilesPaths
        {
            get
            {
                return m_filePaths;
            }
        }
        public void RemoveProfileFromSwitchProfile(string profileToRemove)
        {           
            if (m_ListOfSwitchProfiles.ContainsKey(profileToRemove))
            {
                m_SwitchProfileContextMenuStrip.Items.Remove(m_ListOfSwitchProfiles[profileToRemove]);
                m_ListOfSwitchProfiles.Remove(profileToRemove);
            } 
        }
        public void AddProfileToSwitchProfile(string profileToAdd)
        {
            ToolStripMenuItem SwitchProfile = new ToolStripMenuItem(profileToAdd, null, SwitchProfile_Click);
            m_SwitchProfileContextMenuStrip.Items.Add(SwitchProfile);
            m_ListOfSwitchProfiles.Add(profileToAdd, SwitchProfile);            
        }
        // To Initialize Switch Profile ToolStrip menu Items
        public void InitializeSwitchProfiles()
        {
            //string ProfileDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            //string defaultProfilesDirectory = System.IO.Path.Combine(ProfileDirectory, "profiles");
            m_ListOfSwitchProfiles.Clear();
            m_SwitchProfileContextMenuStrip.Items.Clear();
            m_CurrentCheckedProfile = null;
            string ProfileDirectory = GetPredefinedProfilesDirectory();
            m_filePaths = System.IO.Directory.GetFiles(ProfileDirectory, "*.xml");
            List<string> filePathsList = new List<string>();
            if (m_filePaths != null && m_filePaths.Length > 0)
            {
                //string[] profileFileNames = new string[m_filePaths.Length];
                for (int i = 0; i < m_filePaths.Length; i++)
                {
                    filePathsList.Add(System.IO.Path.GetFileNameWithoutExtension(m_filePaths[i]));
                    //   m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(m_filePaths[i]));
                }
                
                if (filePathsList.Contains("Basic"))
                {
                    int index = filePathsList.IndexOf("Basic");
                    ToolStripMenuItem SwitchProfile = new ToolStripMenuItem(filePathsList[index], null, SwitchProfile_Click);
                    m_SwitchProfileContextMenuStrip.Items.Add(SwitchProfile);
                    m_ListOfSwitchProfiles.Add(filePathsList[index], SwitchProfile);
                    filePathsList.RemoveAt(index);
                }
                if (filePathsList.Contains("Intermediate"))
                {
                    int index = filePathsList.IndexOf("Intermediate");
                    ToolStripMenuItem SwitchProfile = new ToolStripMenuItem(filePathsList[index], null, SwitchProfile_Click);
                    m_SwitchProfileContextMenuStrip.Items.Add(SwitchProfile);
                    m_ListOfSwitchProfiles.Add(filePathsList[index], SwitchProfile);
                    filePathsList.RemoveAt(index);
                }
                foreach (string file in filePathsList)
                {
                    ToolStripMenuItem SwitchProfile = new ToolStripMenuItem(file, null, SwitchProfile_Click);
                    m_SwitchProfileContextMenuStrip.Items.Add(SwitchProfile);
                    m_ListOfSwitchProfiles.Add(file, SwitchProfile);                  
                }

            }

            //ProfileDirectory = System.IO.Directory.GetParent(Settings_Permanent.GetSettingFilePath()).ToString();
            //ProfileDirectory = System.IO.Path.Combine(ProfileDirectory, "profiles");
            ProfileDirectory = GetCustomProfilesDirectory();
            if (System.IO.Directory.Exists(ProfileDirectory))
            {
                string[] temp =  System.IO.Directory.GetFiles(ProfileDirectory, "*.xml");
                string[] tempFilePaths = new string[m_filePaths.Length + temp.Length];
                m_filePaths.CopyTo(tempFilePaths, 0);
                temp.CopyTo(tempFilePaths, m_filePaths.Length);
                m_filePaths = tempFilePaths;


                if (temp != null && temp.Length > 0)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(temp[i]);
                        if (!m_ListOfSwitchProfiles.ContainsKey(filename))
                        {
                            ToolStripMenuItem SwitchProfile = new ToolStripMenuItem(filename, null, SwitchProfile_Click);
                            m_SwitchProfileContextMenuStrip.Items.Add(SwitchProfile);
                            m_ListOfSwitchProfiles.Add(filename, SwitchProfile);
                        }

                    }
                }
            }// directory exists check

            if (mView != null && mView.ObiForm != null && mView.ObiForm.Settings != null)
            {
                mView.ObiForm.UpdateTitle();
            }
        }
        // LoadProfile is used to Load Profile from RT toggle and Transport bar Switch profile button.
        public void LoadProfile(string profilePath, string ProfileName)
        {
            if (this.MonitorContinuously) this.MonitorContinuously = false;
            if (this.IsRecorderActive || this.IsPlayerActive) this.Stop();
            Settings saveProfile = Settings.GetSettingsFromSavedProfile(profilePath);
            if (saveProfile == null) return;
            saveProfile.CopyPropertiesToExistingSettings(mView.ObiForm.Settings, PreferenceProfiles.Audio,ProfileName);
            saveProfile.CopyPropertiesToExistingSettings(mView.ObiForm.Settings, PreferenceProfiles.Colors,ProfileName);
            saveProfile.SettingsName = ProfileName;
            string strLoadedProfiles = " ";
            if (saveProfile.Compare(mView.ObiForm.Settings, PreferenceProfiles.All))
            {
                strLoadedProfiles += "all";
            }
            else
            {
                if (saveProfile.Compare(mView.ObiForm.Settings, PreferenceProfiles.Project))
                {
                    strLoadedProfiles += "project, ";
                }
                if (saveProfile.Compare(mView.ObiForm.Settings, PreferenceProfiles.Audio))
                {
                    strLoadedProfiles += "audio, ";
                }
                if (saveProfile.Compare(mView.ObiForm.Settings, PreferenceProfiles.UserProfile))
                {
                    strLoadedProfiles += "users profile, ";
                }
                if (saveProfile.Compare(mView.ObiForm.Settings, PreferenceProfiles.Colors))
                {
                    strLoadedProfiles += "colors";
                }
            }
            if (string.IsNullOrEmpty(strLoadedProfiles))
            {
                mView.ObiForm.Settings.SettingsName = "customized";
            }
            else
            {
                if (strLoadedProfiles.EndsWith(",")) strLoadedProfiles = strLoadedProfiles.Remove(strLoadedProfiles.Length - 2);
            }
            string text = string.Format(Localizer.Message("Preferences_ProfilesStatus"), saveProfile.SettingsName, strLoadedProfiles);
            mView.ObiForm.Settings.SettingsNameForManipulation = saveProfile.SettingsName + "   " + Localizer.Message("Profile_Audio");
            mView.ObiForm.Settings.SettingsName = text;
            UpdateButtons();
            mTransportBarTooltip.SetToolTip(m_btnSwitchProfile, Localizer.Message("Transport_SwitchProfile") + "\n" + ProfileName + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString()) + ")");
            m_btnSwitchProfile.AccessibleName = Localizer.Message("Transport_SwitchProfile") + ProfileName + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString());

            if (m_CurrentCheckedProfile != null)
            {
                m_CurrentCheckedProfile.Checked = false;
            }
            if (m_ListOfSwitchProfiles.ContainsKey(ProfileName))
            {
                ToolStripMenuItem ProfileSelected = m_ListOfSwitchProfiles[ProfileName];
                ProfileSelected.Checked = true;
                m_CurrentCheckedProfile = ProfileSelected;
            }
               
            mView.ObiForm.UpdateRecordingToolBarButtons();
            mView.ObiForm.UpdateTitle();
            mView.ObiForm.UpdateColors();
            if (mView.ObiForm.Settings.ShowGraphicalPeakMeterInsideObiAtStartup)
            {

                mView.ObiForm.ShowPeakMeterInsideObi(true);
            }
            else
            {
                mView.ObiForm.ShowPeakMeterInsideObi(false);
            }
        }

    // Event is subscribed to ToolStripMenu items.
        void SwitchProfile_Click(object sender, EventArgs e)
        {

            string ProfileName =   sender.ToString();
           
            List<string> filePathsList = new List<string>();
            if (m_filePaths != null && m_filePaths.Length > 0)
            {
                for (int i = 0; i < m_filePaths.Length; i++)
                {
                    filePathsList.Add(System.IO.Path.GetFileNameWithoutExtension(m_filePaths[i]));
                }
            }
            if (filePathsList.Contains(ProfileName))
            {
                int index = filePathsList.IndexOf(ProfileName);

                LoadProfile(m_filePaths[index],ProfileName);
                List<string> installedTTSVoices = Obi.Audio.AudioFormatConverter.InstalledTTSVoices;
                if (installedTTSVoices != null && installedTTSVoices.Count > 0)
                {
                    if (!installedTTSVoices.Contains(mView.ObiForm.Settings.Audio_TTSVoice))
                    {
                        mView.ObiForm.Settings.Audio_TTSVoice = installedTTSVoices[0];
                    }
                }
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

            mTransportBarTooltip.SetToolTip(m_btnPlayingOptions, Localizer.Message("Transport_PlayingOptions") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandPlayOptions.Value.ToString()) + ")");
            m_btnPlayingOptions.AccessibleName = Localizer.Message("Transport_PlayingOptions") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandPlayOptions.Value.ToString());

            mTransportBarTooltip.SetToolTip(m_btnRecordingOptions, Localizer.Message("Transport_RecordingOptions") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandRecordOptions.Value.ToString()) + ")");
            m_btnRecordingOptions.AccessibleName = Localizer.Message("Transport_RecordingOptions") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandRecordOptions.Value.ToString());

            string tempSettingsName = mView.ObiForm.Settings.SettingsNameForManipulation;
            string[] str = tempSettingsName.Split(new string[] { "   " }, StringSplitOptions.None);
            mTransportBarTooltip.SetToolTip(m_btnSwitchProfile, Localizer.Message("Transport_SwitchProfile") + "\n" + str[0] + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString()) + ")");
            m_btnSwitchProfile.AccessibleName = Localizer.Message("Transport_SwitchProfile") + str[0] + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString());

            mTransportBarTooltip.SetToolTip(mDisplayBox, mDisplayBox.SelectedItem.ToString());
           
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
            PlaySection();           
            
        }
        public void PlaySection()
        {
            EmptyNode phrase = null;
            ObiNode nodeSelect = null;
            if (mView != null && mView.Selection != null)
            {


                if (mView.Selection.Node is EmptyNode)
                {
                    phrase = (EmptyNode)mView.Selection.Node;
                    nodeSelect = phrase.ParentAs<SectionNode>();
                }
                else if (mView.Selection.Node is SectionNode)
                {
                    nodeSelect = mView.Selection.Node;
                }
                if (nodeSelect != null)
                {

                    m_IsPlaySectionInspiteOfPhraseSelection = true;
                    double time = -1;
                    if (IsPlayerActive)
                    {
                        if (mCurrentPlaylist.CurrentPhrase == mView.Selection.Node) time = mCurrentPlaylist.CurrentTimeInAsset;
                    }
                    else if (mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange != null)
                    {
                        AudioSelection audioSel = (AudioSelection)mView.Selection;
                        if (audioSel.AudioRange.HasCursor) time = audioSel.AudioRange.CursorTime;
                        else time = audioSel.AudioRange.SelectionBeginTime;
                    }

                    if (IsPlayerActive) Stop();

                    if (time > 0  && mView.Selection.Node is PhraseNode) 
                    {
                        bool playOnNavigateStatus = SelectionChangedPlaybackEnabled;
                        SelectionChangedPlaybackEnabled = false;
                        mView.Selection = new AudioSelection((PhraseNode) mView.Selection.Node, mView.Selection.Control, new AudioRange(time));
                        SelectionChangedPlaybackEnabled = playOnNavigateStatus;
                        }

                    try
                    {
                        PlayOrResume(nodeSelect);
                    }
                    catch (System.Exception ex)
                    {
                        mView.WriteToLogFile(ex.ToString());
                        if (mCurrentPlaylist != null) mCurrentPlaylist.ForcedStopForError();
                        MessageBox.Show(string.Format(Localizer.Message("TransportBar_PlayerExceptionMsg"), "\n\n", ex.ToString()));
                    }
                }
            }
        }

        private void m_PlayAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mView.ObiForm.Settings.Audio_PlaySectionUsingPlayBtn)
            {
                PlayAllSections();
            }
            else
            {
                PlayOrResume();
            }

        }
        public void PlayAllSections()
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
            if (!mView.ObiForm.Settings.Audio_PlaySectionUsingPlayBtn)
            {
                m_PlayAlltoolStripMenuItem.Enabled = mView.CanPlay || mView.CanResume;
            }
            else
            {
                m_PlayAlltoolStripMenuItem.Enabled = mView.CanPlaySelection || mView.CanResume;
            }
            m_PlaySectiontoolStripMenuItem.Enabled = mView.CanPlaySelection || mView.CanResume;
            m_playHeadingToolStripMenuItem.Enabled = mView.CanPlaySelection || mView.CanResume;

            m_PreviewFromtoolStripMenuItem.Enabled = mView.CanPreview || mView.CanPreviewAudioSelection;
            m_PreviewUptotoolStripMenuItem.Enabled = mView.CanPreview || mView.CanPreviewAudioSelection;
            
        }

        private void mPreviewBeforeRecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreviewBeforeRecording();
        }
        public void PreviewBeforeRecording()
        {
            StartRecordingDirectly(true);
        }

        public bool PreviewBeforeRecordingActive
        {
            get { return m_PreviewBeforeRecordingActive; }
        }

        private void m_playHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayHeading();        
           
        }
        public void PlayHeading()
        {
            PhraseNode pharse = null;
            SectionNode nodeSelect = null;
            EmptyNode emptyNode = null;
            if (mView != null )
            {
                if (mView.Selection == null)
                {
                    nodeSelect = mView.Presentation.FirstSection;
                }
                if (mView.Selection.Node is EmptyNode)
                {
                    nodeSelect = mView.Selection.Node.ParentAs<SectionNode>();
                }
                else if (mView.Selection.Node is SectionNode)
                {
                    nodeSelect = (SectionNode)mView.Selection.Node;
                }
                
                if (nodeSelect != null)
                {
                    if (IsPlayerActive) Stop();
                    PlayHeadingPhrase(nodeSelect);
                }
            }
        }
        public bool ExpandPlayOptions()
        {
            if (m_btnPlayingOptions.Enabled)
            {
                Point pt = new Point(0, m_btnPlayingOptions.Height);
                pt = m_btnPlayingOptions.PointToScreen(pt);
                m_PlayingOptionsContextMenuStrip.Show(pt);
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public bool ExpandRecordOptions()
        {
            if (m_btnRecordingOptions.Enabled)
            {
                Point pt = new Point(0, m_btnRecordingOptions.Height);
                pt = m_btnRecordingOptions.PointToScreen(pt);
                m_RecordingOptionsContextMenuStrip.Show(pt);
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool ExpandSwitchProfile()
        {
            if (m_btnSwitchProfile.Enabled)
            {
                Point pt = new Point(0, m_btnSwitchProfile.Height);
                pt = m_btnSwitchProfile.PointToScreen(pt);
                m_SwitchProfileContextMenuStrip.Show(pt);
                ShowSwitchProfileContextMenu();
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool IsPreviewBeforeRecordingEnabled
        {
            get
            {
                if (mView != null && mView.ObiForm != null && mView.ObiForm.Settings != null && mView.Selection != null && CurrentState != State.Monitoring
&& mView.ObiForm.Settings.Audio_AllowOverwrite && ((CurrentState == State.Paused && !(mView.Selection is AudioSelection)) || (mView.Selection != null && mView.Selection is AudioSelection && ((AudioSelection)mView.Selection).AudioRange != null && ((AudioSelection)mView.Selection).AudioRange.HasCursor)))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        //@MonitorContinuously
        public bool MonitorContinuously
        {
            get { return m_MonitorContinuously; }
            set
            {
                Console.WriteLine("value of monitor continuously" + value);
                if (value && mView.ObiForm != null )
                {
                    
                    m_MonitorContinuously = value;
                    StartMonitorContinuously();
                }
                else
                {
                    StopMonitorContinuously();
                    m_MonitorContinuously = value;
                    // trigger state changed event again because the MonitorContinuesly goes false after the state changed event
                    if (StateChanged != null) StateChanged(this, new AudioLib.AudioPlayer.StateChangedEventArgs(mPlayer.CurrentState));
                }
                
            }
        }

        //@MonitorContinuously
        private void StartMonitorContinuously()
        {
            if (m_MonitorContinuously && mPlayer.CurrentState == AudioLib.AudioPlayer.State.Stopped && mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
            {
                mRecordingSession = new RecordingSession(mView.Presentation, mRecorder, mView.ObiForm.Settings);
                mRecordingSession.StartMonitoring();
                mVUMeterPanel.BeepEnable = true;
            }
        }

        //@MonitorContinuously
        private void StopMonitorContinuously()
        {
            if (m_MonitorContinuously && mRecordingSession != null && mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring)
            {
                mRecordingSession.Stop();
                mRecordingSession = null;
                mVUMeterPanel.BeepEnable = false;                 
            }
        }

        //@MonitorContinuously
        System.ComponentModel.BackgroundWorker m_MonitorContinuouslyWorker= null;
        private void StartMonitorContinuouslyWithDelay()
        {
            m_MonitorContinuouslyWorker = new System.ComponentModel.BackgroundWorker();
                m_MonitorContinuouslyWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
                {
                    Thread.Sleep(1500);
                 });

m_MonitorContinuouslyWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
                {
                    if (m_MonitorContinuously && CurrentState == State.Stopped)
                    {
                        StartMonitorContinuously();
                    }
                });
m_MonitorContinuouslyWorker.RunWorkerAsync();
                
        }

        private void mMonitorContinuouslyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mMonitorContinuouslyToolStripMenuItem.Checked)
            {
                MonitorContinuously = true;
            }
            else
            {
                MonitorContinuously = false;
            }
        }

        private void m_SwitchProfile_Click(object sender, EventArgs e)
        {
            Point pt = new Point(0, m_btnSwitchProfile.Height);
            pt = m_btnSwitchProfile.PointToScreen(pt);
            m_SwitchProfileContextMenuStrip.Show(pt);
            ShowSwitchProfileContextMenu();
        }
        public void ShowSwitchProfileContextMenu()
        {

            string[] str = mView.ObiForm.Settings.SettingsNameForManipulation.Split(new string[] { "   " }, StringSplitOptions.None);
            //     if (m_CurrentCheckedProfile == null || m_CurrentCheckedProfile.ToString() != str[0])

            if (m_ListOfSwitchProfiles.ContainsKey(str[0]))
            {
                if (m_CurrentCheckedProfile != null)
                {
                    m_CurrentCheckedProfile.Checked = false;
                }
                ToolStripMenuItem ProfileSelected = m_ListOfSwitchProfiles[str[0]];
                ProfileSelected.Checked = true;
                m_CurrentCheckedProfile = ProfileSelected;
                mTransportBarTooltip.SetToolTip(m_btnSwitchProfile, Localizer.Message("Transport_SwitchProfile") + "\n" + str[0] + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString()) + ")");
                m_btnSwitchProfile.AccessibleName = Localizer.Message("Transport_SwitchProfile") + str[0] + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ContentView_TransportBarExpandSwitchProfile.Value.ToString());
                mView.ObiForm.UpdateTitle();

            }

        }

        public void SetFont()//@fontconfig
        {
            mTransportBarTooltip.OwnerDraw = true;
            mTransportBarTooltip.IsBalloon = false;
            this.Font = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
            m_PlayingOptionsContextMenuStrip.Font = m_RecordingOptionsContextMenuStrip.Font = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
        }
        private void mTransportBarTooltip_Draw(object sender, DrawToolTipEventArgs e)//@fontconfig
        {
            Font tooltipFont = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size);
            Font tooltipTitleFont = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Bold);
            e.Graphics.Clear(Color.White);

            e.Graphics.DrawString(mTransportBarTooltip.ToolTipTitle, tooltipTitleFont, Brushes.Blue, new PointF(0, 0));
            e.Graphics.DrawString(" \n" + e.ToolTipText, tooltipFont, Brushes.Black, new PointF(0, 0));

        }

        private void mTransportBarTooltip_Popup(object sender, PopupEventArgs e)//@fontconfig
        {
            e.ToolTipSize = TextRenderer.MeasureText(mTransportBarTooltip.ToolTipTitle + "\n" + mTransportBarTooltip.GetToolTip(e.AssociatedControl), new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size));
        }

        private System.Text.StringBuilder m_MissedNotificationMessages = new System.Text.StringBuilder();
        private void LogRecorderMissedNotificationMsg(object sender, AudioLib.AudioRecorder.CircularBufferNotificationTimerMessageEventArgs e)
        {
            if (e != null && !string.IsNullOrEmpty(e.Msg))
            {
                m_MissedNotificationMessages.AppendLine(e.Msg);
                if (m_MissedNotificationMessages != null &&  m_MissedNotificationMessages.Length > 4000)
                {
                    WriteLogMsgForRecorderMissedNotification();
                }
            }
        }

        private void WriteLogMsgForRecorderMissedNotification ()
        {
            if (m_MissedNotificationMessages != null &&  m_MissedNotificationMessages.Length > 0)
            {
                m_MissedNotificationMessages.AppendLine("adding next set of missed notification messages");
                mView.WriteToLogFile(m_MissedNotificationMessages.ToString());
                m_MissedNotificationMessages = null;
                m_MissedNotificationMessages = new System.Text.StringBuilder();
            }
        }


    }
}
