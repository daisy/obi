using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public partial class Settings
    {
        public bool Audio_AllowOverwrite;            // allow/disallow overwriting audio when recording
        public bool Audio_Recording_PreviewBeforeStarting; //plays a bit of audio before starting recording.
        public bool Audio_Recording_ReplaceAfterCursor; // replaces the audio after cursor position with new recording
        public bool Audio_RecordDirectlyWithRecordButton; // Directly start recording on clicking record button bypassing monitoring
        public int Audio_Channels;              // number of channels for recording
        public bool Audio_AudioClues;                // use audio clues (or not.)
        public float Audio_AudioScale;               // scale of audio in waveform views
        public int Audio_BitDepth;                   // sample bit depth
        public string Audio_TTSVoice;
        public int TransportBarCounterIndex;
        public bool Audio_FastPlayWithoutPitchChange;
        public ColorSettings ColorSettings;    // current color settings
        public ColorSettings ColorSettingsHC;  // current color settings for high contrast
        public bool CreateTitleSection;        // defaulf for "create title section" in new project
        public string Project_DefaultPath;             // default location
        public bool Project_AutomaticallyDeleteUnusedFilesAfterCleanup; // deletes the unused files without user permission after cleanup
        public float FontSize;                 // global font size (all font sizes must be relative to this one)
        public string Audio_LastInputDevice;         // the name of the last input device selected by the user
        public string LastOpenProject;         // path to the last open project
        public string Audio_LastOutputDevice;        // the name of the last output device selected by the user
        public uint MaxPhraseDurationMinutes;  // maximum phrase duration in minutes for autosplitting during import
        public uint ImportCharCountToTruncateFromStart; // number of chars to truncate in section name while creating sections with imported file names
        public string ImportCharsToReplaceWithSpaces; // chars that should be replaced by white spaces in section names while creating sections from imported audio file names
        public string ImportPageIdentificationString; // string that indicates that the audio file is a page instead of being a section and it should be appended to previous section
        public uint MaxAllowedPhraseDurationInMinutes ; //Max size of phrase allowed in content view
        public bool Audio_ShowLiveWaveformWhileRecording;// Show Live Waveform While Recording
        public bool Audio_EnableLivePhraseDetection; // enables phrase detection while recording
        public AudioLib.VuMeter.NoiseLevelSelection Audio_NoiseLevel;  // noise level for low amplitude detection
        public double Audio_NudgeTimeMs;             // nudge time in milliseconds
        public bool ShowGraphicalPeakMeterAtStartup; // displays the graphical peak meter when Obi initializes
        public bool Project_OpenLastProject;           // open the last open project at startup
        public string Project_PipelineScriptsPath;     // path to the pipeline script for the DTB audio encoder
        public bool Project_CheckForUpdates;
        public string Project_LatestVersionCheckedByUpdate;
        public bool PlayIfNoSelection;         // play all or nothing if no selection
        public bool PlayOnNavigate;            // start playback when navigating, or just change the selection
        public int Audio_PreviewDuration;            // playback preview duration in milliseconds
        public int Audio_ElapseBackTimeInMilliseconds; // elapse back time  interval in milliseconds
        public ArrayList RecentProjects;       // paths to projects recently opened
        public int Audio_SampleRate;                 // sample rate in Hertz
        public bool SplitPhrasesOnImport;      // split phrases on import
        public bool SynchronizeViews;          // keep views synchronized
        public UserProfile UserProfile;        // the user profile
        public bool WrapStripContents;         // wrap strip contents in the content view
        public float ZoomFactor;               // global zoom factor
        public bool Project_AutoSave_RecordingEnd ; // flag to audo save whenever recording ends
        public bool Project_AutoSaveTimeIntervalEnabled;// enables / disables auto save after time interval contained in AutoSaveTimeInterval
        public int Project_AutoSaveTimeInterval; // time interval after which project will be auto saved 
        public bool Project_SaveProjectWhenRecordingEnds; //save to main project file when recording stops or pauses
        public string UsersInfoToUpload; //users info is temporarily stored till it is uploaded or timed out
        public int UploadAttemptsCount; // number of times user info upload attempted
        public bool Export_EncodeAudioFiles;
        public double ExportEncodingBitRate;
        public bool Export_AppendSectionNameToAudioFile;
        public bool Export_LimitAudioFilesLength; // decides if the length of audio file names has to be limited
        public int Export_AudioFilesNamesLengthLimit; // truncates exported audio file name from right side to limit the maximum no. of characters.
        public bool Project_Export_AlwaysIgnoreIndentation;  // if true, it ensures that exported files have no line breaks, tab breaks etc.
        public int Export_EPUBFileNameLengthLimit; // truncates exported EPUB 3 file name from right side to limit the maximum no. of characters.
        public bool Export_EPUBCreateDummyText ; // create dummy text corresponding to each smil event in exported book.
        public bool Project_OpenBookmarkNodeOnReopeningProject;
        public int[] BookMarkNodeHierarchy;
        public bool Audio_RetainInitialSilenceInPhraseDetection;
        public bool Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
        public int Project_ImportToleranceForAudioInMs;
        //public bool RecordDirectly;
        public bool Project_LeftAlignPhrasesInContentView;
        public bool Project_OptimizeMemory;
        public bool Project_ShowWaveformInContentView;
        public string Font;
        public decimal Audio_DefaultGap;
        public decimal Audio_DefaultLeadingSilence;
        public decimal Audio_DefaultThreshold;
        public double RecordingToolBarIncrementVal;
        public bool Project_EnableFreeDiskSpaceCheck; // enables free disk space check
        public bool Audio_EnablePostRecordingPageRenumbering; //ask for renumber following pages as recording is stopped
        //public bool MonitorContinuously; //start monitoring whenever transport bar is in stop state
        public bool ImportAudioCreateSectionCheck; //Checked status for the Create section for each audio file in importfilesize dialog 
        public bool Audio_UseRecordingPauseShortcutForStopping;
        public bool Project_BackgroundColorForEmptySection;
        public int Audio_LevelComboBoxIndex;
        public bool Audio_UseRecordBtnToRecordOverSubsequentAudio;
        public bool Audio_PreservePagesWhileRecordOverSubsequentAudio;
        public bool Audio_EnforceSingleCursor;
        public bool Audio_DeleteFollowingPhrasesOfSectionAfterRecording; // delete the following phrases in section when recording stops, it is attempt to minimize the delay while recording.
        public bool Audio_DisableDeselectionOnStop;
        public bool Project_SaveObiLocationAndSize;// Saves Obi last location and Size
        public bool Project_EPUBCheckTimeOutEnabled ;
        public string Project_ObiConfigFileName;
        public bool Project_PeakMeterChangeLocation; // Changes Peak meter location wrt Obi Form
        public bool Project_MinimizeObi;
        public double Audio_CleanupMaxFileSizeInMB;
        public string EncodingFileFormat;
        [OptionalField]
        public bool Project_RecordingToolbarOpenInPreviousSession;
        
        // size and point types should be stored in arrays

        private int[] m_ObiLastLocation;
        public Point ObiLastLocation
        {
            get { return new Point(m_ObiLastLocation[0], m_ObiLastLocation[1]); }
            set
            {
                m_ObiLastLocation = new int[2];
                m_ObiLastLocation[0] = value.X;
                m_ObiLastLocation[1] = value.Y;
            }
        }

        private int[] m_NewProjectDialogSize;
        public Size NewProjectDialogSize      // size of the new project dialog
        {
            get { return new Size(m_NewProjectDialogSize[0], m_NewProjectDialogSize[1]); }
            set
            {
                m_NewProjectDialogSize = new int[2];
                m_NewProjectDialogSize[0] = value.Width;
                m_NewProjectDialogSize[1] = value.Height;
            }
        }

        private int[] m_ObiFormSize;
        public Size ObiFormSize               // size of the form (for future sessions)
        {
            get { return new Size(m_ObiFormSize[0], m_ObiFormSize[1]); }
            set
            {
                m_ObiFormSize = new int[2];
                m_ObiFormSize[0] = value.Width;
                m_ObiFormSize[1] = value.Height;
            }
        }

        private int[] m_PeakmeterSize;
        public Size PeakmeterSize             // Size of the peak meter form(for future sessions)
        {
            get { return new Size(m_PeakmeterSize[0], m_PeakmeterSize[1]); }
            set
            {
                m_PeakmeterSize = new int[2];
                m_PeakmeterSize[0] = value.Width;
                m_PeakmeterSize[1] = value.Height;
            }
        }

        private int[] m_GraphicalPeakMeterContolSize;
        public Size GraphicalPeakMeterContolSize //Size of the peak meter control
        {
            get { return new Size(m_GraphicalPeakMeterContolSize[0], m_GraphicalPeakMeterContolSize[1]); }
            set
            {
                m_GraphicalPeakMeterContolSize = new int[2];
                m_GraphicalPeakMeterContolSize[0] = value.Width;
                m_GraphicalPeakMeterContolSize[1] = value.Height;
            }
        }
        
        // For Obi 3.7
        [OptionalField]
        public bool Audio_AlwaysMonitorRecordingToolBar;

        [OptionalField]
        private int[] m_RecordingToolBarLastLocation;
        public Point RecordingToolBarLastLocation
        {
            get { return new Point(m_RecordingToolBarLastLocation[0], m_RecordingToolBarLastLocation[1]); }
            set
            {
                m_RecordingToolBarLastLocation = new int[2];
                m_RecordingToolBarLastLocation[0] = value.X;
                m_RecordingToolBarLastLocation[1] = value.Y;
            }
        }

        // For post Obi 3.7
        [OptionalField]
        public string Audio_RecordingToolbarProfile1;

        [OptionalField]
        public string Audio_RecordingToolbarProfile2;

        [OptionalField]
        public bool Audio_ColorFlickerPreviewBeforeRecording;

        [OptionalField]
        public bool Audio_PlayAllUsingPlayBtn;
        


        private static readonly string SETTINGS_FILE_NAME = "obi_settings.xml";


        [OptionalField]
        public string SettingsName;

        protected static void InitializeDefaultSettings(Settings settings)
        {
            settings.Audio_Channels = 1;
            settings.Audio_AudioClues = false;
            settings.Audio_TTSVoice = "";
            settings.TransportBarCounterIndex = 0;
            settings.Audio_FastPlayWithoutPitchChange = true;
            settings.Audio_AudioScale = 0.01f;
            settings.Audio_AllowOverwrite = false;
            settings.Audio_Recording_PreviewBeforeStarting = false;
            settings.Audio_Recording_ReplaceAfterCursor = false;
            settings.Audio_RecordDirectlyWithRecordButton = false;
            settings.Audio_BitDepth = 16;
            settings.ColorSettings = ColorSettings.DefaultColorSettings();
            settings.ColorSettingsHC = ColorSettings.DefaultColorSettingsHC();
            settings.Project_DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup = true;
            //settings.EnableTooltips = true;
            settings.FontSize = 10.0f;
            settings.Audio_LastInputDevice = "";
            settings.LastOpenProject = "";
            settings.Audio_LastOutputDevice = "";
            settings.MaxPhraseDurationMinutes = 10;
            settings.ImportCharCountToTruncateFromStart = 4;
            settings.ImportCharsToReplaceWithSpaces = "_";
            settings.ImportPageIdentificationString = "page";
            settings.MaxAllowedPhraseDurationInMinutes = 50;
            settings.Audio_ShowLiveWaveformWhileRecording = false;
            settings.Audio_EnableLivePhraseDetection = false;
            settings.NewProjectDialogSize = new Size(0, 0);
            settings.CreateTitleSection = true;
            settings.Audio_NoiseLevel = AudioLib.VuMeter.NoiseLevelSelection.Medium;
            settings.Audio_NudgeTimeMs = 200.0;
            settings.ObiFormSize = new Size(0, 0);
            settings.PeakmeterSize = new Size(0, 0);
            settings.GraphicalPeakMeterContolSize = new Size(0, 0);
            settings.ShowGraphicalPeakMeterAtStartup = true ;
            settings.Project_OpenLastProject = false;
            settings.Audio_PreviewDuration = 1500;
            settings.Audio_ElapseBackTimeInMilliseconds = 1500;
            settings.Project_PipelineScriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine ( "Pipeline-lite", "scripts" ) );
            settings.Project_CheckForUpdates = true;
            settings.Project_LatestVersionCheckedByUpdate = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            settings.PlayIfNoSelection = true;
            settings.PlayOnNavigate = false;
            settings.RecentProjects = new ArrayList();
            settings.Audio_SampleRate = 44100;
            settings.SplitPhrasesOnImport = false;
            settings.SynchronizeViews = true;
            settings.UserProfile = new UserProfile();
            settings.WrapStripContents = true;
            settings.ZoomFactor = 1.0f;
            settings.Project_AutoSave_RecordingEnd = false;
            settings.Project_AutoSaveTimeIntervalEnabled= true;
            settings.Project_AutoSaveTimeInterval = 300000; // saving time interval in ms ( 5min)
            settings.Project_SaveProjectWhenRecordingEnds = true;
            settings.UsersInfoToUpload = "NoInfo" ;
            settings.UploadAttemptsCount = 0 ;
            settings.Export_EncodeAudioFiles = false;
            settings.ExportEncodingBitRate = 64;
            settings.Export_AppendSectionNameToAudioFile = false;
            settings.Export_LimitAudioFilesLength = false;
            settings.Export_AudioFilesNamesLengthLimit = 8;
            settings.Project_Export_AlwaysIgnoreIndentation = false;
            settings.Export_EPUBFileNameLengthLimit = 12;
            settings.Export_EPUBCreateDummyText = true;
            settings.Project_OpenBookmarkNodeOnReopeningProject = false;
            settings.Audio_RetainInitialSilenceInPhraseDetection = true;
            settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = false;
            settings.Project_ImportToleranceForAudioInMs = 100;
            //settings.RecordDirectly = false;
            settings.Project_LeftAlignPhrasesInContentView = true;
            settings.Project_OptimizeMemory = true;
            settings.Project_ShowWaveformInContentView = true;
            settings.Font = "Times New Roman";
            settings.Audio_DefaultGap = 300;
            settings.Audio_DefaultLeadingSilence = 50;
            settings.Audio_DefaultThreshold = 280;
            settings.RecordingToolBarIncrementVal = 0;
            settings.Project_EnableFreeDiskSpaceCheck = true;
            settings.Audio_EnablePostRecordingPageRenumbering = true;
            //settings.MonitorContinuously = false;
            settings.ImportAudioCreateSectionCheck = true;
            settings.Audio_UseRecordingPauseShortcutForStopping = false;
            settings.Project_BackgroundColorForEmptySection = false;
            settings.Audio_LevelComboBoxIndex = 0;
            settings.Audio_UseRecordBtnToRecordOverSubsequentAudio = false;
            settings.Audio_PreservePagesWhileRecordOverSubsequentAudio = false;
            settings.Audio_EnforceSingleCursor = false;
            settings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording = false;
            settings.Audio_DisableDeselectionOnStop = false;
            settings.Project_SaveObiLocationAndSize = false;
            settings.Project_EPUBCheckTimeOutEnabled = true;
            settings.Project_ObiConfigFileName = "obiconfig.xml";
            settings.ObiLastLocation = new Point(0, 0);
            settings.Project_PeakMeterChangeLocation = false;
            settings.Project_MinimizeObi = false;
            settings.Audio_CleanupMaxFileSizeInMB = 100;
            settings.EncodingFileFormat = "MP3";
            settings.SettingsName = "Basic";
            settings.Project_RecordingToolbarOpenInPreviousSession = false;
            // For Obi 3.7
            settings.Audio_AlwaysMonitorRecordingToolBar = false;
            settings.RecordingToolBarLastLocation = new Point(0, 0);
            // For post Obi 3.7
            settings.Audio_RecordingToolbarProfile1 = "Basic";
            settings.Audio_RecordingToolbarProfile2 = "Advance";
            settings.Audio_ColorFlickerPreviewBeforeRecording = false;
            settings.Audio_PlayAllUsingPlayBtn = false;
        }

        /// <summary>
        /// Creates a settings object having default values
        /// </summary>
        /// <returns></returns>
        public static Settings GetDefaultSettings()
        {
            Settings settings = new Settings();
            InitializeDefaultSettings(settings);
            return settings;
        }

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            InitializeDefaultSettings(settings);

            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception) { }
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }


        public void ResetSettingsFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SETTINGS_FILE_NAME, FileMode.Create, FileAccess.Write, file);
            InitializeDefaultSettings(this);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

    }
}
