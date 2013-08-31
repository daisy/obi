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
    public class Settings
    {
        public bool AllowOverwrite;            // allow/disallow overwriting audio when recording
        public bool Recording_PreviewBeforeStarting; //plays a bit of audio before starting recording.
        public bool Recording_ReplaceAfterCursor; // replaces the audio after cursor position with new recording
        public bool RecordDirectlyWithRecordButton; // Directly start recording on clicking record button bypassing monitoring
        public int AudioChannels;              // number of channels for recording
        public bool AudioClues;                // use audio clues (or not.)
        public float AudioScale;               // scale of audio in waveform views
        public int AudioBitDepth;                   // sample bit depth
        public string Audio_TTSVoice;
        public int Audio_TransportBarCounterIndex;
        public bool Audio_FastPlayWithoutPitchChange;
        public ColorSettings ColorSettings;    // current color settings
        public ColorSettings ColorSettingsHC;  // current color settings for high contrast
        public bool CreateTitleSection;        // defaulf for "create title section" in new project
        public string DefaultPath;             // default location
        public bool Project_AutomaticallyDeleteUnusedFilesAfterCleanup; // deletes the unused files without user permission after cleanup
        public bool EnableTooltips;            // enable or disable tooltips
        public float FontSize;                 // global font size (all font sizes must be relative to this one)
        public string LastInputDevice;         // the name of the last input device selected by the user
        public string LastOpenProject;         // path to the last open project
        public string LastOutputDevice;        // the name of the last output device selected by the user
        public uint MaxPhraseDurationMinutes;  // maximum phrase duration in minutes for autosplitting during import
        public uint Audio_ImportCharCountToTruncateFromStart; // number of chars to truncate in section name while creating sections with imported file names
        public string Audio_ImportCharsToReplaceWithSpaces; // chars that should be replaced by white spaces in section names while creating sections from imported audio file names
        public string Audio_ImportPageIdentificationString; // string that indicates that the audio file is a page instead of being a section and it should be appended to previous section
        public uint MaxAllowedPhraseDurationInMinutes ; //Max size of phrase allowed in content view
        public bool Audio_ShowLiveWaveformWhileRecording;// Show Live Waveform While Recording
        public bool Audio_EnableLivePhraseDetection; // enables phrase detection while recording
        public Size NewProjectDialogSize;      // size of the new project dialog
        public AudioLib.VuMeter.NoiseLevelSelection NoiseLevel;  // noise level for low amplitude detection
        public double NudgeTimeMs;             // nudge time in milliseconds
        public Size ObiFormSize;               // size of the form (for future sessions)
        public Size PeakmeterSize;             // Size of the peak meter form(for future sessions)
        public Size GraphicalPeakMeterContolSize; //Size of the peak meter control
        public bool ShowGraphicalPeakMeterAtStartup; // displays the graphical peak meter when Obi initializes
        public bool OpenLastProject;           // open the last open project at startup
        public string PipelineScriptsPath;     // path to the pipeline script for the DTB audio encoder
        public bool Project_CheckForUpdates;
        public string Project_LatestVersionCheckedByUpdate;
        public bool PlayIfNoSelection;         // play all or nothing if no selection
        public bool PlayOnNavigate;            // start playback when navigating, or just change the selection
        public int PreviewDuration;            // playback preview duration in milliseconds
        public int ElapseBackTimeInMilliseconds; // elapse back time  interval in milliseconds
        public ArrayList RecentProjects;       // paths to projects recently opened
        public int AudioSampleRate;                 // sample rate in Hertz
        public bool SplitPhrasesOnImport;      // split phrases on import
        public bool SynchronizeViews;          // keep views synchronized
        public UserProfile UserProfile;        // the user profile
        public bool WrapStripContents;         // wrap strip contents in the content view
        public float ZoomFactor;               // global zoom factor
        public bool AutoSave_RecordingEnd ; // flag to audo save whenever recording ends
        public bool AutoSaveTimeIntervalEnabled;// enables / disables auto save after time interval contained in AutoSaveTimeInterval
        public int AutoSaveTimeInterval; // time interval after which project will be auto saved 
        public bool Project_SaveProjectWhenRecordingEnds; //save to main project file when recording stops or pauses
        public string UsersInfoToUpload; //users info is temporarily stored till it is uploaded or timed out
        public int UploadAttemptsCount; // number of times user info upload attempted
        public bool Export_EncodeToMP3;
        public int Export_BitRateMP3;
        public bool Export_AppendSectionNameToAudioFile;
        public bool Export_LimitAudioFilesLength; // decides if the length of audio file names has to be limited
        public int Export_AudioFilesNamesLengthLimit; // truncates exported audio file name from right side to limit the maximum no. of characters.
        public bool OpenBookmarkNodeOnReopeningProject;
        public int[] BookMarkNodeHierarchy;
        public bool RetainInitialSilenceInPhraseDetection;
        public bool Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
        public int ImportToleranceForAudioInMs;
        public bool RecordDirectly;
        public bool LeftAlignPhrasesInContentView;
        public bool OptimizeMemory;
        public bool Project_ShowWaveformInContentView;
        public string Font;
        public decimal DefaultGap;
        public decimal DefaultLeadingSilence;
        public decimal DefaultThreshold;
        public double RecordingToolBarIncrementVal;
        public bool Project_EnableFreeDiskSpaceCheck; // enables free disk space check
        public bool Audio_EnablePostRecordingPageRenumbering; //ask for renumber following pages as recording is stopped
        public bool Audio_MonitorContinuously; //start monitoring whenever transport bar is in stop state
        public bool ImportAudioCreateSectionCheck; //Cheked status for the Create section for each audio file in importfilesize dialog 

        private static readonly string SETTINGS_FILE_NAME = "obi_settings.xml";

        private static void InitializeDefaultSettings(Settings settings)
        {
            settings.AudioChannels = 1;
            settings.AudioClues = false;
            settings.Audio_TTSVoice = "";
            settings.Audio_TransportBarCounterIndex = 0;
            settings.Audio_FastPlayWithoutPitchChange = true;
            settings.AudioScale = 0.01f;
            settings.AllowOverwrite = false;
            settings.Recording_PreviewBeforeStarting = false;
            settings.Recording_ReplaceAfterCursor = false;
            settings.RecordDirectlyWithRecordButton = false;
            settings.AudioBitDepth = 16;
            settings.ColorSettings = ColorSettings.DefaultColorSettings();
            settings.ColorSettingsHC = ColorSettings.DefaultColorSettingsHC();
            settings.DefaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            settings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup = true;
            settings.EnableTooltips = true;
            settings.FontSize = 10.0f;
            settings.LastInputDevice = "";
            settings.LastOpenProject = "";
            settings.LastOutputDevice = "";
            settings.MaxPhraseDurationMinutes = 10;
            settings.Audio_ImportCharCountToTruncateFromStart = 4;
            settings.Audio_ImportCharsToReplaceWithSpaces = "_";
            settings.Audio_ImportPageIdentificationString = "page";
            settings.MaxAllowedPhraseDurationInMinutes = 50;
            settings.Audio_ShowLiveWaveformWhileRecording = false;
            settings.Audio_EnableLivePhraseDetection = false;
            settings.NewProjectDialogSize = new Size(0, 0);
            settings.CreateTitleSection = true;
            settings.NoiseLevel = AudioLib.VuMeter.NoiseLevelSelection.Medium;
            settings.NudgeTimeMs = 200.0;
            settings.ObiFormSize = new Size(0, 0);
            settings.PeakmeterSize = new Size(0, 0);
            settings.GraphicalPeakMeterContolSize = new Size(0, 0);
            settings.ShowGraphicalPeakMeterAtStartup = true ;
            settings.OpenLastProject = false;
            settings.PreviewDuration = 1500;
            settings.ElapseBackTimeInMilliseconds = 1500;
            settings.PipelineScriptsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                Path.Combine ( "Pipeline-lite", "scripts" ) );
            settings.Project_CheckForUpdates = true;
            settings.Project_LatestVersionCheckedByUpdate = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            settings.PlayIfNoSelection = true;
            settings.PlayOnNavigate = false;
            settings.RecentProjects = new ArrayList();
            settings.AudioSampleRate = 44100;
            settings.SplitPhrasesOnImport = false;
            settings.SynchronizeViews = true;
            settings.UserProfile = new UserProfile();
            settings.WrapStripContents = true;
            settings.ZoomFactor = 1.0f;
            settings.AutoSave_RecordingEnd = false;
            settings.AutoSaveTimeIntervalEnabled= true;
            settings.AutoSaveTimeInterval = 300000; // saving time interval in ms ( 5min)
            settings.Project_SaveProjectWhenRecordingEnds = true;
            settings.UsersInfoToUpload = "NoInfo" ;
            settings.UploadAttemptsCount = 0 ;
            settings.Export_EncodeToMP3 = false;
            settings.Export_BitRateMP3 = 64;
            settings.Export_AppendSectionNameToAudioFile = false;
            settings.Export_LimitAudioFilesLength = false;
            settings.Export_AudioFilesNamesLengthLimit = 8;
            settings.OpenBookmarkNodeOnReopeningProject = false;
            settings.RetainInitialSilenceInPhraseDetection = true;
            settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = false;
            settings.ImportToleranceForAudioInMs = 100;
            settings.RecordDirectly = false;
            settings.LeftAlignPhrasesInContentView = true;
            settings.OptimizeMemory = true;
            settings.Project_ShowWaveformInContentView = true;
            settings.Font = "Times New Roman";
            settings.DefaultGap = 300;
            settings.DefaultLeadingSilence = 50;
            settings.DefaultThreshold = 280;
            settings.RecordingToolBarIncrementVal = 0;
            settings.Project_EnableFreeDiskSpaceCheck = true;
            settings.Audio_EnablePostRecordingPageRenumbering = true;
            settings.Audio_MonitorContinuously = false;
            settings.ImportAudioCreateSectionCheck = true;
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
