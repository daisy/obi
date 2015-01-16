using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Saves and loads the user defined profiles to the settings.
    /// </summary>
    [Serializable()]
   public class Settings_SaveProfile : Settings
    {

        public static Settings_SaveProfile GetSettingsFromSavedProfile (string profileFilePath)
        {
            Settings_SaveProfile settingsInstance = new Settings_SaveProfile();
            Settings.InitializeDefaultSettings(settingsInstance);
            FileStream fs = new FileStream(profileFilePath, FileMode.OpenOrCreate);
            SoapFormatter soap = new SoapFormatter();
            settingsInstance  = (Settings_SaveProfile)soap.Deserialize(fs);
            fs.Close();

            return settingsInstance;
            
        }

       public Settings CopyPropertiesToExistingSettings(Settings newSettings)
       {
           

           newSettings.Audio_AllowOverwrite = this.Audio_AllowOverwrite;
           newSettings.Audio_AudioClues = this.Audio_AudioClues;
           newSettings.Audio_AudioScale = this.Audio_AudioScale;
           //newSettings.Audio_BitDepth = this.Audio_BitDepth;
           newSettings.Audio_Channels = this.Audio_Channels;
           newSettings.Audio_CleanupMaxFileSizeInMB = this.Audio_CleanupMaxFileSizeInMB;
           newSettings.Audio_DefaultGap = this.Audio_DefaultGap;
           newSettings.Audio_DefaultLeadingSilence = this.Audio_DefaultLeadingSilence;
           newSettings.Audio_DefaultThreshold = this.Audio_DefaultThreshold;
           newSettings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording = this.Audio_DeleteFollowingPhrasesOfSectionAfterRecording;
           newSettings.Audio_DisableDeselectionOnStop = this.Audio_DisableDeselectionOnStop;
           newSettings.Audio_ElapseBackTimeInMilliseconds = this.Audio_ElapseBackTimeInMilliseconds;
           newSettings.Audio_EnableLivePhraseDetection = this.Audio_EnableLivePhraseDetection;
           newSettings.Audio_EnablePostRecordingPageRenumbering = this.Audio_EnablePostRecordingPageRenumbering;
           newSettings.Audio_EnforceSingleCursor = this.Audio_EnforceSingleCursor;
           newSettings.Audio_FastPlayWithoutPitchChange = this.Audio_FastPlayWithoutPitchChange;
           //newSettings.Audio_LastInputDevice = this.Audio_LastInputDevice;
           newSettings.Audio_LastOutputDevice = this.Audio_LastOutputDevice;
           newSettings.Audio_LevelComboBoxIndex = this.Audio_LevelComboBoxIndex;
           newSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = this.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
           newSettings.Audio_NoiseLevel = this.Audio_NoiseLevel;
           newSettings.Audio_NudgeTimeMs = this.Audio_NudgeTimeMs;
           newSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio = this.Audio_PreservePagesWhileRecordOverSubsequentAudio;
           newSettings.Audio_PreviewDuration = this.Audio_PreviewDuration;
           newSettings.Audio_RecordDirectlyWithRecordButton = this.Audio_RecordDirectlyWithRecordButton;;
           newSettings.Audio_Recording_PreviewBeforeStarting = this.Audio_Recording_PreviewBeforeStarting;
           newSettings.Audio_Recording_ReplaceAfterCursor = this.Audio_Recording_ReplaceAfterCursor;
           newSettings.Audio_RetainInitialSilenceInPhraseDetection = this.Audio_RetainInitialSilenceInPhraseDetection;
           newSettings.Audio_SampleRate = this.Audio_SampleRate;
           newSettings.Audio_ShowLiveWaveformWhileRecording = this.Audio_ShowLiveWaveformWhileRecording;
           newSettings.Audio_TTSVoice = this.Audio_TTSVoice;
           newSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio = this.Audio_UseRecordBtnToRecordOverSubsequentAudio;
           newSettings.Audio_UseRecordingPauseShortcutForStopping = this.Audio_UseRecordingPauseShortcutForStopping;
           
           //newSettings.BookMarkNodeHierarchy = this.BookMarkNodeHierarchy;
           //newSettings.ColorSettings = this.ColorSettings;
           //newSettings.ColorSettingsHC = this.ColorSettingsHC;
           newSettings.CreateTitleSection = this.CreateTitleSection;
           newSettings.EncodingFileFormat = this.EncodingFileFormat;
           newSettings.Export_AppendSectionNameToAudioFile = this.Export_AppendSectionNameToAudioFile;
           newSettings.Export_AudioFilesNamesLengthLimit = this.Export_AudioFilesNamesLengthLimit;
           newSettings.Export_EncodeAudioFiles = this.Export_EncodeAudioFiles;
           newSettings.Export_EPUBCreateDummyText = this.Export_EPUBCreateDummyText;
           newSettings.Export_EPUBFileNameLengthLimit = this.Export_EPUBFileNameLengthLimit;
           newSettings.Export_LimitAudioFilesLength = this.Export_LimitAudioFilesLength;
           newSettings.ExportEncodingBitRate = this.ExportEncodingBitRate;
           newSettings.Font = this.Font;
           newSettings.FontSize = this.FontSize;
           newSettings.GraphicalPeakMeterContolSize = this.GraphicalPeakMeterContolSize;
           newSettings.ImportAudioCreateSectionCheck = this.ImportAudioCreateSectionCheck;
           newSettings.ImportCharCountToTruncateFromStart = this.ImportCharCountToTruncateFromStart;
           newSettings.ImportCharsToReplaceWithSpaces = this.ImportCharsToReplaceWithSpaces;
           newSettings.ImportPageIdentificationString = this.ImportPageIdentificationString;
           newSettings.LastOpenProject = this.LastOpenProject;
           newSettings.MaxAllowedPhraseDurationInMinutes = this.MaxAllowedPhraseDurationInMinutes;
           newSettings.MaxPhraseDurationMinutes = this.MaxPhraseDurationMinutes;
           newSettings.NewProjectDialogSize = this.NewProjectDialogSize;
           newSettings.ObiFormSize = this.ObiFormSize;
           newSettings.ObiLastLocation = this.ObiLastLocation;
           newSettings.PeakmeterSize = this.PeakmeterSize;
           newSettings.PlayIfNoSelection = this.PlayIfNoSelection;
           newSettings.PlayOnNavigate = this.PlayOnNavigate;
           
           newSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup = this.Project_AutomaticallyDeleteUnusedFilesAfterCleanup;
           newSettings.Project_AutoSave_RecordingEnd = this.Project_AutoSave_RecordingEnd;
           newSettings.Project_AutoSaveTimeInterval = this.Project_AutoSaveTimeInterval;
           newSettings.Project_AutoSaveTimeIntervalEnabled = this.Project_AutoSaveTimeIntervalEnabled;
           newSettings.Project_BackgroundColorForEmptySection = this.Project_BackgroundColorForEmptySection;
           newSettings.Project_CheckForUpdates = this.Project_CheckForUpdates;
           newSettings.Project_DefaultPath = this.Project_DefaultPath;
           newSettings.Project_EnableFreeDiskSpaceCheck = this.Project_EnableFreeDiskSpaceCheck;
           newSettings.Project_EPUBCheckTimeOutEnabled = this.Project_EPUBCheckTimeOutEnabled;
           newSettings.Project_Export_AlwaysIgnoreIndentation = this.Project_Export_AlwaysIgnoreIndentation;
           newSettings.Project_ImportToleranceForAudioInMs = this.Project_ImportToleranceForAudioInMs;
           newSettings.Project_LatestVersionCheckedByUpdate = this.Project_LatestVersionCheckedByUpdate;
           newSettings.Project_LeftAlignPhrasesInContentView = this.Project_LeftAlignPhrasesInContentView;
           newSettings.Project_ObiConfigFileName = this.Project_ObiConfigFileName;
           newSettings.Project_OpenBookmarkNodeOnReopeningProject = this.Project_OpenBookmarkNodeOnReopeningProject;
           newSettings.Project_OpenLastProject = this.Project_OpenLastProject;
           newSettings.Project_OptimizeMemory = this.Project_OptimizeMemory;
           newSettings.Project_PeakMeterChangeLocation = this.Project_PeakMeterChangeLocation;
           newSettings.Project_PipelineScriptsPath = this.Project_PipelineScriptsPath;
           newSettings.Project_SaveObiLocationAndSize = this.Project_SaveObiLocationAndSize;
           newSettings.Project_SaveProjectWhenRecordingEnds = this.Project_SaveProjectWhenRecordingEnds;
           newSettings.Project_ShowWaveformInContentView = this.Project_ShowWaveformInContentView;
           
           //newSettings.RecentProjects = this.RecentProjects;
           newSettings.RecordingToolBarIncrementVal = this.RecordingToolBarIncrementVal;
           newSettings.ShowGraphicalPeakMeterAtStartup = this.ShowGraphicalPeakMeterAtStartup;
           newSettings.SplitPhrasesOnImport = this.SplitPhrasesOnImport;
           newSettings.SynchronizeViews = this.SynchronizeViews;
           newSettings.TransportBarCounterIndex = this.TransportBarCounterIndex;
           //newSettings.UploadAttemptsCount = this.UploadAttemptsCount;
           newSettings.UserProfile.Name = this.UserProfile.Name;
           newSettings.UserProfile.Organization = this.UserProfile.Organization;
           newSettings.UserProfile.Culture = this.UserProfile.Culture;
           //newSettings.UsersInfoToUpload = this.UsersInfoToUpload;
           newSettings.WrapStripContents = this.WrapStripContents;
           newSettings.ZoomFactor = this.ZoomFactor;
           
           return newSettings;
       }

        public void Save(string profileFilePath, Settings existingSettings)
        {
            try
            {
                CopyPropertiesFromSettings(existingSettings);
                FileStream fs = new FileStream(profileFilePath, FileMode.OpenOrCreate);
                SoapFormatter soap = new SoapFormatter();
                soap.Serialize(fs, this);
                
                fs.Close();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

       private void CopyPropertiesFromSettings(Settings existingSettings)
       {
           this.Audio_AllowOverwrite = existingSettings.Audio_AllowOverwrite;
           this.Audio_AudioClues = existingSettings.Audio_AudioClues;
           this.Audio_AudioScale = existingSettings.Audio_AudioScale;
       }

            }
}
