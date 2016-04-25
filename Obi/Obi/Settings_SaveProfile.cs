using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

namespace Obi
{
    public enum PreferenceProfiles { None, Project, Audio, UserProfile, KeyboardShortcuts, Colors, All }
    /// <summary>
    /// Saves and loads the user defined profiles to the settings.
    /// </summary>
    
   public partial class Settings
    {

        public static Settings GetSettingsFromSavedProfile (string profileFilePath)
        {
            Settings settingsInstance = new Settings();
            Settings.InitializeDefaultSettings(settingsInstance);
            FileStream fs = new FileStream(profileFilePath, FileMode.OpenOrCreate);
            SoapFormatter soap = new SoapFormatter();
            settingsInstance  = (Settings)soap.Deserialize(fs);
            fs.Close();

            return settingsInstance;
            
        }

       public Settings CopyPropertiesToExistingSettings(Settings newSettings, PreferenceProfiles prefProfiles, string profileName)
       {
           
           if (prefProfiles == PreferenceProfiles.Audio || prefProfiles == PreferenceProfiles.All)
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
               if (profileName != "Profile-SBS" && profileName != "VA-Insert" && profileName != "VA-Overwrite" &&
                   profileName != "Basic" && profileName != "Advance" && profileName != "Intermediate")
               {
                   newSettings.Audio_LastInputDevice = this.Audio_LastInputDevice;
                   newSettings.Audio_LastOutputDevice = this.Audio_LastOutputDevice;
               }
               newSettings.Audio_LevelComboBoxIndex = this.Audio_LevelComboBoxIndex;
               newSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = this.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
               newSettings.Audio_NoiseLevel = this.Audio_NoiseLevel;
               newSettings.Audio_NudgeTimeMs = this.Audio_NudgeTimeMs;
               newSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio = this.Audio_PreservePagesWhileRecordOverSubsequentAudio;
               newSettings.Audio_PreviewDuration = this.Audio_PreviewDuration;
               newSettings.Audio_RecordDirectlyWithRecordButton = this.Audio_RecordDirectlyWithRecordButton; ;
               newSettings.Audio_Recording_PreviewBeforeStarting = this.Audio_Recording_PreviewBeforeStarting;
               newSettings.Audio_Recording_ReplaceAfterCursor = this.Audio_Recording_ReplaceAfterCursor;
               newSettings.Audio_RetainInitialSilenceInPhraseDetection = this.Audio_RetainInitialSilenceInPhraseDetection;
               newSettings.Audio_SampleRate = this.Audio_SampleRate;
               newSettings.Audio_ShowLiveWaveformWhileRecording = this.Audio_ShowLiveWaveformWhileRecording;
               newSettings.Audio_TTSVoice = this.Audio_TTSVoice;
               newSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio = this.Audio_UseRecordBtnToRecordOverSubsequentAudio;
               newSettings.Audio_UseRecordingPauseShortcutForStopping = this.Audio_UseRecordingPauseShortcutForStopping;
               newSettings.Audio_AlwaysMonitorRecordingToolBar = this.Audio_AlwaysMonitorRecordingToolBar;
               newSettings.Audio_ColorFlickerPreviewBeforeRecording = this.Audio_ColorFlickerPreviewBeforeRecording;
               newSettings.Audio_PlaySectionUsingPlayBtn = this.Audio_PlaySectionUsingPlayBtn;
               newSettings.Audio_EnableFileDataProviderPreservation = this.Audio_EnableFileDataProviderPreservation;
               newSettings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording = this.Audio_EnsureCursorVisibilityInUndoOfSplitRecording;
               newSettings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording = this.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording;
               newSettings.Audio_PreventSplittingPages = this.Audio_PreventSplittingPages;
               newSettings.Audio_SaveAudioZoom = this.Audio_SaveAudioZoom;
               if (!string.IsNullOrEmpty(this.Audio_LocalRecordingDirectory) && System.IO.Directory.Exists(this.Audio_LocalRecordingDirectory))
               {
                   newSettings.Audio_LocalRecordingDirectory = this.Audio_LocalRecordingDirectory;
               }
           }

           if (prefProfiles == PreferenceProfiles.Project || prefProfiles == PreferenceProfiles.All)
           {
               newSettings.Project_DisableRollBackForCleanUp = this.Project_DisableRollBackForCleanUp;
               newSettings.Project_AutoSave_RecordingEnd = this.Project_AutoSave_RecordingEnd;
               newSettings.Project_AutoSaveTimeInterval = this.Project_AutoSaveTimeInterval;
               newSettings.Project_AutoSaveTimeIntervalEnabled = this.Project_AutoSaveTimeIntervalEnabled;
               newSettings.Project_BackgroundColorForEmptySection = this.Project_BackgroundColorForEmptySection;
               newSettings.Project_CheckForUpdates = this.Project_CheckForUpdates;
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
               newSettings.Project_SaveObiLocationAndSize = this.Project_SaveObiLocationAndSize;
               newSettings.Project_SaveProjectWhenRecordingEnds = this.Project_SaveProjectWhenRecordingEnds;
               newSettings.Project_ShowWaveformInContentView = this.Project_ShowWaveformInContentView;
               newSettings.Project_MinimizeObi = this.Project_MinimizeObi;
               newSettings.Project_EnableMouseScrolling = this.Project_EnableMouseScrolling;
               newSettings.Project_DisableTOCViewCollapse = this.Project_DisableTOCViewCollapse;
               newSettings.Project_MaximizeObi = this.Project_MaximizeObi;
               newSettings.Project_VAXhtmlExport = this.Project_VAXhtmlExport;
               newSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup =
                   this.Project_AutomaticallyDeleteUnusedFilesAfterCleanup;

               // checks for paths
               //if ( !string.IsNullOrEmpty(this.Project_DefaultPath) && Directory.Exists (this.Project_DefaultPath ))
               //newSettings.Project_DefaultPath = this.Project_DefaultPath;

               //if (!string.IsNullOrEmpty(this.Project_PipelineScriptsPath) && Directory.Exists(this.Project_PipelineScriptsPath))
               //newSettings.Project_PipelineScriptsPath = this.Project_PipelineScriptsPath;
           }
           if (prefProfiles == PreferenceProfiles.UserProfile || prefProfiles == PreferenceProfiles.All)
           {
               if (this.UserProfile != null)
               {
                   //newSettings.UserProfile.Name = this.UserProfile.Name;
                   //newSettings.UserProfile.Organization = this.UserProfile.Organization;
                   newSettings.UserProfile.Culture = this.UserProfile.Culture;
               }
           }
           //newSettings.RecentProjects = this.RecentProjects;
           //newSettings.BookMarkNodeHierarchy = this.BookMarkNodeHierarchy;
           if (prefProfiles == PreferenceProfiles.Colors || prefProfiles == PreferenceProfiles.All)
           {
               newSettings.ColorSettings.BlockBackColor_Custom = this.ColorSettings.BlockBackColor_Custom;
               newSettings.ColorSettings.BlockBackColor_Anchor = this.ColorSettings.BlockBackColor_Anchor;
               newSettings.ColorSettings.BlockBackColor_Empty = this.ColorSettings.BlockBackColor_Empty;
               newSettings.ColorSettings.BlockBackColor_Heading = this.ColorSettings.BlockBackColor_Heading;
               newSettings.ColorSettings.BlockBackColor_Page = this.ColorSettings.BlockBackColor_Page;
               newSettings.ColorSettings.BlockBackColor_Plain = this.ColorSettings.BlockBackColor_Plain;
               newSettings.ColorSettings.BlockBackColor_Selected = this.ColorSettings.BlockBackColor_Selected;
               newSettings.ColorSettings.BlockBackColor_Silence = this.ColorSettings.BlockBackColor_Silence;
               newSettings.ColorSettings.BlockBackColor_TODO = this.ColorSettings.BlockBackColor_TODO;
               newSettings.ColorSettings.BlockBackColor_Unused = this.ColorSettings.BlockBackColor_Unused;
               newSettings.ColorSettings.BlockForeColor_Anchor = this.ColorSettings.BlockForeColor_Anchor;
               newSettings.ColorSettings.BlockForeColor_Custom = this.ColorSettings.BlockForeColor_Custom;
               newSettings.ColorSettings.BlockForeColor_Empty = this.ColorSettings.BlockForeColor_Empty;
               newSettings.ColorSettings.BlockForeColor_Heading = this.ColorSettings.BlockForeColor_Heading;
               newSettings.ColorSettings.BlockForeColor_Page = this.ColorSettings.BlockForeColor_Page;
               newSettings.ColorSettings.BlockForeColor_Plain = this.ColorSettings.BlockForeColor_Plain;
               newSettings.ColorSettings.BlockForeColor_Selected = this.ColorSettings.BlockForeColor_Selected;
               newSettings.ColorSettings.BlockForeColor_Silence = this.ColorSettings.BlockForeColor_Silence;
               newSettings.ColorSettings.BlockForeColor_TODO = this.ColorSettings.BlockForeColor_TODO;
               newSettings.ColorSettings.BlockForeColor_Unused = this.ColorSettings.BlockForeColor_Unused;
               newSettings.ColorSettings.BlockLayoutSelectedColor = this.ColorSettings.BlockLayoutSelectedColor;
               newSettings.ColorSettings.ContentViewBackColor = this.ColorSettings.ContentViewBackColor;
               newSettings.ColorSettings.EditableLabelTextBackColor = this.ColorSettings.EditableLabelTextBackColor;
               newSettings.ColorSettings.ProjectViewBackColor = this.ColorSettings.ProjectViewBackColor;
               newSettings.ColorSettings.StripBackColor = this.ColorSettings.StripBackColor;
               newSettings.ColorSettings.StripCursorSelectedBackColor = this.ColorSettings.StripCursorSelectedBackColor;
               newSettings.ColorSettings.StripForeColor = this.ColorSettings.StripForeColor;
               newSettings.ColorSettings.StripSelectedBackColor = this.ColorSettings.StripSelectedBackColor;
               newSettings.ColorSettings.StripSelectedForeColor = this.ColorSettings.StripSelectedForeColor;
               newSettings.ColorSettings.StripUnusedBackColor = this.ColorSettings.StripUnusedBackColor;
               newSettings.ColorSettings.StripUnusedForeColor = this.ColorSettings.StripUnusedForeColor;
               newSettings.ColorSettings.StripWithoutPhrasesBackcolor = this.ColorSettings.StripWithoutPhrasesBackcolor;
               newSettings.ColorSettings.TOCViewBackColor = this.ColorSettings.TOCViewBackColor;
               newSettings.ColorSettings.TOCViewForeColor = this.ColorSettings.TOCViewForeColor;
               newSettings.ColorSettings.TOCViewUnusedColor = this.ColorSettings.TOCViewUnusedColor;
               newSettings.ColorSettings.ToolTipForeColor = this.ColorSettings.ToolTipForeColor;
               newSettings.ColorSettings.TransportBarBackColor = this.ColorSettings.TransportBarBackColor;
               newSettings.ColorSettings.TransportBarLabelBackColor = this.ColorSettings.TransportBarLabelBackColor;
               newSettings.ColorSettings.TransportBarLabelForeColor = this.ColorSettings.TransportBarLabelForeColor;
               newSettings.ColorSettings.WaveformBackColor = this.ColorSettings.WaveformBackColor;
               newSettings.ColorSettings.WaveformBaseLineColor = this.ColorSettings.WaveformBaseLineColor;
               newSettings.ColorSettings.WaveformHighlightedBackColor = this.ColorSettings.WaveformHighlightedBackColor;
               newSettings.ColorSettings.FineNavigationColor = this.ColorSettings.FineNavigationColor;
               newSettings.ColorSettings.RecordingHighlightPhraseColor = this.ColorSettings.RecordingHighlightPhraseColor;
               newSettings.ColorSettings.EmptySectionBackgroundColor = this.ColorSettings.EmptySectionBackgroundColor;
               newSettings.ColorSettings.HighlightedSectionNodeWithoutSelectionColor = this.ColorSettings.HighlightedSectionNodeWithoutSelectionColor;

               newSettings.ColorSettingsHC.BlockBackColor_Custom = this.ColorSettingsHC.BlockBackColor_Custom;
               newSettings.ColorSettingsHC.BlockBackColor_Anchor = this.ColorSettingsHC.BlockBackColor_Anchor;
               newSettings.ColorSettingsHC.BlockBackColor_Empty = this.ColorSettingsHC.BlockBackColor_Empty;
               newSettings.ColorSettingsHC.BlockBackColor_Heading = this.ColorSettingsHC.BlockBackColor_Heading;
               newSettings.ColorSettingsHC.BlockBackColor_Page = this.ColorSettingsHC.BlockBackColor_Page;
               newSettings.ColorSettingsHC.BlockBackColor_Plain = this.ColorSettingsHC.BlockBackColor_Plain;
               newSettings.ColorSettingsHC.BlockBackColor_Selected = this.ColorSettingsHC.BlockBackColor_Selected;
               newSettings.ColorSettingsHC.BlockBackColor_Silence = this.ColorSettingsHC.BlockBackColor_Silence;
               newSettings.ColorSettingsHC.BlockBackColor_TODO = this.ColorSettingsHC.BlockBackColor_TODO;
               newSettings.ColorSettingsHC.BlockBackColor_Unused = this.ColorSettingsHC.BlockBackColor_Unused;
               newSettings.ColorSettingsHC.BlockForeColor_Anchor = this.ColorSettingsHC.BlockForeColor_Anchor;
               newSettings.ColorSettingsHC.BlockForeColor_Custom = this.ColorSettingsHC.BlockForeColor_Custom;
               newSettings.ColorSettingsHC.BlockForeColor_Empty = this.ColorSettingsHC.BlockForeColor_Empty;
               newSettings.ColorSettingsHC.BlockForeColor_Heading = this.ColorSettingsHC.BlockForeColor_Heading;
               newSettings.ColorSettingsHC.BlockForeColor_Page = this.ColorSettingsHC.BlockForeColor_Page;
               newSettings.ColorSettingsHC.BlockForeColor_Plain = this.ColorSettingsHC.BlockForeColor_Plain;
               newSettings.ColorSettingsHC.BlockForeColor_Selected = this.ColorSettingsHC.BlockForeColor_Selected;
               newSettings.ColorSettingsHC.BlockForeColor_Silence = this.ColorSettingsHC.BlockForeColor_Silence;
               newSettings.ColorSettingsHC.BlockForeColor_TODO = this.ColorSettingsHC.BlockForeColor_TODO;
               newSettings.ColorSettingsHC.BlockForeColor_Unused = this.ColorSettingsHC.BlockForeColor_Unused;
               newSettings.ColorSettingsHC.BlockLayoutSelectedColor = this.ColorSettingsHC.BlockLayoutSelectedColor;
               newSettings.ColorSettingsHC.ContentViewBackColor = this.ColorSettingsHC.ContentViewBackColor;
               newSettings.ColorSettingsHC.EditableLabelTextBackColor = this.ColorSettingsHC.EditableLabelTextBackColor;
               newSettings.ColorSettingsHC.ProjectViewBackColor = this.ColorSettingsHC.ProjectViewBackColor;
               newSettings.ColorSettingsHC.StripBackColor = this.ColorSettingsHC.StripBackColor;
               newSettings.ColorSettingsHC.StripCursorSelectedBackColor = this.ColorSettingsHC.StripCursorSelectedBackColor;
               newSettings.ColorSettingsHC.StripForeColor = this.ColorSettingsHC.StripForeColor;
               newSettings.ColorSettingsHC.StripSelectedBackColor = this.ColorSettingsHC.StripSelectedBackColor;
               newSettings.ColorSettingsHC.StripSelectedForeColor = this.ColorSettingsHC.StripSelectedForeColor;
               newSettings.ColorSettingsHC.StripUnusedBackColor = this.ColorSettingsHC.StripUnusedBackColor;
               newSettings.ColorSettingsHC.StripUnusedForeColor = this.ColorSettingsHC.StripUnusedForeColor;
               newSettings.ColorSettingsHC.StripWithoutPhrasesBackcolor = this.ColorSettingsHC.StripWithoutPhrasesBackcolor;
               newSettings.ColorSettingsHC.TOCViewBackColor = this.ColorSettingsHC.TOCViewBackColor;
               newSettings.ColorSettingsHC.TOCViewForeColor = this.ColorSettingsHC.TOCViewForeColor;
               newSettings.ColorSettingsHC.TOCViewUnusedColor = this.ColorSettingsHC.TOCViewUnusedColor;
               newSettings.ColorSettingsHC.ToolTipForeColor = this.ColorSettingsHC.ToolTipForeColor;
               newSettings.ColorSettingsHC.TransportBarBackColor = this.ColorSettingsHC.TransportBarBackColor;
               newSettings.ColorSettingsHC.TransportBarLabelBackColor = this.ColorSettingsHC.TransportBarLabelBackColor;
               newSettings.ColorSettingsHC.TransportBarLabelForeColor = this.ColorSettingsHC.TransportBarLabelForeColor;
               newSettings.ColorSettingsHC.WaveformBackColor = this.ColorSettingsHC.WaveformBackColor;
               newSettings.ColorSettingsHC.WaveformBaseLineColor = this.ColorSettingsHC.WaveformBaseLineColor;
               newSettings.ColorSettingsHC.WaveformHighlightedBackColor = this.ColorSettingsHC.WaveformHighlightedBackColor;
               newSettings.ColorSettingsHC.FineNavigationColor = this.ColorSettingsHC.FineNavigationColor;
               newSettings.ColorSettingsHC.RecordingHighlightPhraseColor = this.ColorSettingsHC.RecordingHighlightPhraseColor;
               newSettings.ColorSettingsHC.EmptySectionBackgroundColor = this.ColorSettingsHC.EmptySectionBackgroundColor;
               newSettings.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor = this.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor;
               //newSettings.ColorSettings = this.ColorSettings;
               //newSettings.ColorSettingsHC = this.ColorSettingsHC;
           }
           if (prefProfiles == PreferenceProfiles.All)
           {
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
               //newSettings.LastOpenProject = this.LastOpenProject; // no need to preserve last open project across settings
               newSettings.MaxAllowedPhraseDurationInMinutes = this.MaxAllowedPhraseDurationInMinutes;
               newSettings.MaxPhraseDurationMinutes = this.MaxPhraseDurationMinutes;
               newSettings.NewProjectDialogSize = this.NewProjectDialogSize;
               newSettings.ObiFormSize = this.ObiFormSize;
               //newSettings.ObiLastLocation = this.ObiLastLocation;
               newSettings.PeakmeterSize = this.PeakmeterSize;
               newSettings.PlayIfNoSelection = this.PlayIfNoSelection;
               newSettings.PlayOnNavigate = this.PlayOnNavigate;
               newSettings.RecordingToolBarIncrementVal = this.RecordingToolBarIncrementVal;
               //newSettings.ShowGraphicalPeakMeterAtStartup = this.ShowGraphicalPeakMeterAtStartup;
               newSettings.SplitPhrasesOnImport = this.SplitPhrasesOnImport;
               //newSettings.SynchronizeViews = this.SynchronizeViews;
               newSettings.TransportBarCounterIndex = this.TransportBarCounterIndex;
               //newSettings.UploadAttemptsCount = this.UploadAttemptsCount;
               
               //newSettings.UsersInfoToUpload = this.UsersInfoToUpload;
               //newSettings.WrapStripContents = this.WrapStripContents;
               newSettings.ZoomFactor = this.ZoomFactor;
           }
           return newSettings;
       }

        public void Save(string profileFilePath)
        {
            try
            {
                //Settings.InitializeDefaultSettings(this);
                ///CopyPropertiesFromSettings(existingSettings);
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
       /*
       private void CopyPropertiesFromSettings(Settings existingSettings)
       {
           this.Audio_AllowOverwrite = existingSettings.Audio_AllowOverwrite;
           this.Audio_AudioClues = existingSettings.Audio_AudioClues;
           this.Audio_AudioScale = existingSettings.Audio_AudioScale;
           //this.Audio_BitDepth = existingSettings.Audio_BitDepth;
           this.Audio_Channels = existingSettings.Audio_Channels;
           this.Audio_CleanupMaxFileSizeInMB = existingSettings.Audio_CleanupMaxFileSizeInMB;
           this.Audio_DefaultGap = existingSettings.Audio_DefaultGap;
           this.Audio_DefaultLeadingSilence = existingSettings.Audio_DefaultLeadingSilence;
           this.Audio_DefaultThreshold = existingSettings.Audio_DefaultThreshold;
           this.Audio_DeleteFollowingPhrasesOfSectionAfterRecording = existingSettings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording;
           this.Audio_DisableDeselectionOnStop = existingSettings.Audio_DisableDeselectionOnStop;
           this.Audio_ElapseBackTimeInMilliseconds = existingSettings.Audio_ElapseBackTimeInMilliseconds;
           this.Audio_EnableLivePhraseDetection = existingSettings.Audio_EnableLivePhraseDetection;
           this.Audio_EnablePostRecordingPageRenumbering = existingSettings.Audio_EnablePostRecordingPageRenumbering;
           this.Audio_EnforceSingleCursor = existingSettings.Audio_EnforceSingleCursor;
           this.Audio_FastPlayWithoutPitchChange = existingSettings.Audio_FastPlayWithoutPitchChange;
           this.Audio_LastInputDevice = existingSettings.Audio_LastInputDevice;
           this.Audio_LastOutputDevice = existingSettings.Audio_LastOutputDevice;
           this.Audio_LevelComboBoxIndex = existingSettings.Audio_LevelComboBoxIndex;
           this.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = existingSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
           this.Audio_NoiseLevel = existingSettings.Audio_NoiseLevel;
           this.Audio_NudgeTimeMs = existingSettings.Audio_NudgeTimeMs;
           this.Audio_PreservePagesWhileRecordOverSubsequentAudio = existingSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio;
           this.Audio_PreviewDuration = existingSettings.Audio_PreviewDuration;
           this.Audio_RecordDirectlyWithRecordButton = existingSettings.Audio_RecordDirectlyWithRecordButton;
           this.Audio_Recording_PreviewBeforeStarting = existingSettings.Audio_Recording_PreviewBeforeStarting;
           this.Audio_Recording_ReplaceAfterCursor = existingSettings.Audio_Recording_ReplaceAfterCursor;
           this.Audio_RetainInitialSilenceInPhraseDetection = existingSettings.Audio_RetainInitialSilenceInPhraseDetection;
           this.Audio_SampleRate = existingSettings.Audio_SampleRate;
           this.Audio_ShowLiveWaveformWhileRecording = existingSettings.Audio_ShowLiveWaveformWhileRecording;
           this.Audio_TTSVoice = existingSettings.Audio_TTSVoice;
           this.Audio_UseRecordBtnToRecordOverSubsequentAudio = existingSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio;
           this.Audio_UseRecordingPauseShortcutForStopping = existingSettings.Audio_UseRecordingPauseShortcutForStopping;

           this.CreateTitleSection = existingSettings.CreateTitleSection;
           this.EncodingFileFormat = existingSettings.EncodingFileFormat;
           this.Export_AppendSectionNameToAudioFile = existingSettings.Export_AppendSectionNameToAudioFile;
           this.Export_AudioFilesNamesLengthLimit = existingSettings.Export_AudioFilesNamesLengthLimit;
           this.Export_EncodeAudioFiles = existingSettings.Export_EncodeAudioFiles;
           this.Export_EPUBCreateDummyText = existingSettings.Export_EPUBCreateDummyText;
           this.Export_EPUBFileNameLengthLimit = existingSettings.Export_EPUBFileNameLengthLimit;
           this.Export_LimitAudioFilesLength = existingSettings.Export_LimitAudioFilesLength;
           this.ExportEncodingBitRate = existingSettings.ExportEncodingBitRate;
           this.Font = existingSettings.Font;
           this.FontSize = existingSettings.FontSize;
           this.GraphicalPeakMeterContolSize = existingSettings.GraphicalPeakMeterContolSize;
           this.ImportAudioCreateSectionCheck = existingSettings.ImportAudioCreateSectionCheck;
           this.ImportCharCountToTruncateFromStart = existingSettings.ImportCharCountToTruncateFromStart;
           this.ImportCharsToReplaceWithSpaces = existingSettings.ImportCharsToReplaceWithSpaces;
           this.ImportPageIdentificationString = existingSettings.ImportPageIdentificationString;
           this.LastOpenProject = existingSettings.LastOpenProject;
           this.MaxAllowedPhraseDurationInMinutes = existingSettings.MaxAllowedPhraseDurationInMinutes;
           this.MaxPhraseDurationMinutes = existingSettings.MaxPhraseDurationMinutes;
           this.NewProjectDialogSize = existingSettings.NewProjectDialogSize;
           this.ObiFormSize = existingSettings.ObiFormSize;
           this.ObiLastLocation = existingSettings.ObiLastLocation;
           this.PeakmeterSize = existingSettings.PeakmeterSize;
           this.PlayIfNoSelection = existingSettings.PlayIfNoSelection;
           this.PlayOnNavigate = existingSettings.PlayOnNavigate;
           this.Project_AutomaticallyDeleteUnusedFilesAfterCleanup = existingSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup;
           this.Project_AutoSave_RecordingEnd = existingSettings.Project_AutoSave_RecordingEnd;
           this.Project_AutoSaveTimeInterval = existingSettings.Project_AutoSaveTimeInterval;
           this.Project_AutoSaveTimeIntervalEnabled = existingSettings.Project_AutoSaveTimeIntervalEnabled;
           this.Project_BackgroundColorForEmptySection = existingSettings.Project_BackgroundColorForEmptySection;
           this.Project_CheckForUpdates = existingSettings.Project_CheckForUpdates;
           this.Project_DefaultPath = existingSettings.Project_DefaultPath;
           this.Project_EnableFreeDiskSpaceCheck = existingSettings.Project_EnableFreeDiskSpaceCheck;
           this.Project_EPUBCheckTimeOutEnabled = existingSettings.Project_EPUBCheckTimeOutEnabled;
           this.Project_Export_AlwaysIgnoreIndentation = existingSettings.Project_Export_AlwaysIgnoreIndentation;
           this.Project_ImportToleranceForAudioInMs = existingSettings.Project_ImportToleranceForAudioInMs;
           this.Project_LatestVersionCheckedByUpdate = existingSettings.Project_LatestVersionCheckedByUpdate;
           this.Project_LeftAlignPhrasesInContentView = existingSettings.Project_LeftAlignPhrasesInContentView;
           this.Project_ObiConfigFileName = existingSettings.Project_ObiConfigFileName;
           this.Project_OpenBookmarkNodeOnReopeningProject = existingSettings.Project_OpenBookmarkNodeOnReopeningProject;
           this.Project_OpenLastProject = existingSettings.Project_OpenLastProject;
           this.Project_OptimizeMemory = existingSettings.Project_OptimizeMemory;
           this.Project_PeakMeterChangeLocation = existingSettings.Project_PeakMeterChangeLocation;
           this.Project_PipelineScriptsPath = existingSettings.Project_PipelineScriptsPath;
           this.Project_SaveObiLocationAndSize = existingSettings.Project_SaveObiLocationAndSize;
           this.Project_SaveProjectWhenRecordingEnds = existingSettings.Project_SaveProjectWhenRecordingEnds;
           this.Project_ShowWaveformInContentView = existingSettings.Project_ShowWaveformInContentView;
           this.Project_MinimizeObi = existingSettings.Project_MinimizeObi;

           this.RecordingToolBarIncrementVal = existingSettings.RecordingToolBarIncrementVal;
           this.ShowGraphicalPeakMeterAtStartup = existingSettings.ShowGraphicalPeakMeterAtStartup;
           this.SplitPhrasesOnImport = existingSettings.SplitPhrasesOnImport;
           this.SynchronizeViews = existingSettings.SynchronizeViews;
           this.TransportBarCounterIndex = existingSettings.TransportBarCounterIndex;
           //this.UploadAttemptsCount = existingSettings.UploadAttemptsCount;
           if (existingSettings.UserProfile != null)
           {
               this.UserProfile.Culture = existingSettings.UserProfile.Culture;
               this.UserProfile.Name = existingSettings.UserProfile.Name;
               this.UserProfile.Organization = existingSettings.UserProfile.Organization;
           }
           //this.UsersInfoToUpload = existingSettings.UsersInfoToUpload;
           this.WrapStripContents = existingSettings.WrapStripContents;
           this.ZoomFactor = existingSettings.ZoomFactor;
       }
       */
       public bool Compare(Settings settings,PreferenceProfiles selectedProfile)
       {
           // compare if all the relevant public members of the settings are  equal to the corresponding members of this class
           // relevant members means the members which can be changed from preferences dialog. It also excludes path related members.

           bool projectPreferencesMatch = false;
           bool audioPreferencesMatch = false;
           bool usersProfilePreferencesMatch = false;
           bool colorPreferenceMatch = false;


           if ((selectedProfile == PreferenceProfiles.Audio || selectedProfile == PreferenceProfiles.All)
               && this.Audio_AllowOverwrite == settings.Audio_AllowOverwrite
               && this.Audio_AudioClues == settings.Audio_AudioClues
               && this.Audio_AudioScale == settings.Audio_AudioScale
               && this.Audio_Channels == settings.Audio_Channels
               && this.Audio_CleanupMaxFileSizeInMB == settings.Audio_CleanupMaxFileSizeInMB
               && this.Audio_DefaultGap == settings.Audio_DefaultGap
               && this.Audio_DefaultLeadingSilence == settings.Audio_DefaultLeadingSilence
               && this.Audio_DefaultThreshold == settings.Audio_DefaultThreshold
               && this.Audio_DeleteFollowingPhrasesOfSectionAfterRecording == settings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording
               && this.Audio_DisableDeselectionOnStop == settings.Audio_DisableDeselectionOnStop
               && this.Audio_ElapseBackTimeInMilliseconds == settings.Audio_ElapseBackTimeInMilliseconds
               && this.Audio_EnableLivePhraseDetection == settings.Audio_EnableLivePhraseDetection
               && this.Audio_EnablePostRecordingPageRenumbering == settings.Audio_EnablePostRecordingPageRenumbering
               && this.Audio_EnforceSingleCursor == settings.Audio_EnforceSingleCursor
               && this.Audio_FastPlayWithoutPitchChange == settings.Audio_FastPlayWithoutPitchChange
               && this.Audio_LastInputDevice == settings.Audio_LastInputDevice
               && this.Audio_LastOutputDevice == settings.Audio_LastOutputDevice
               && this.Audio_LevelComboBoxIndex == settings.Audio_LevelComboBoxIndex
               && this.Audio_MergeFirstTwoPhrasesAfterPhraseDetection == settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection
               && this.Audio_NoiseLevel == settings.Audio_NoiseLevel
               && this.Audio_NudgeTimeMs == settings.Audio_NudgeTimeMs
               && this.Audio_PreservePagesWhileRecordOverSubsequentAudio == settings.Audio_PreservePagesWhileRecordOverSubsequentAudio
               && this.Audio_PreviewDuration == settings.Audio_PreviewDuration
               && this.Audio_RecordDirectlyWithRecordButton == settings.Audio_RecordDirectlyWithRecordButton
               && this.Audio_Recording_PreviewBeforeStarting == settings.Audio_Recording_PreviewBeforeStarting
               && this.Audio_Recording_ReplaceAfterCursor == settings.Audio_Recording_ReplaceAfterCursor
               && this.Audio_RetainInitialSilenceInPhraseDetection == settings.Audio_RetainInitialSilenceInPhraseDetection
               && this.Audio_SampleRate == settings.Audio_SampleRate
               && this.Audio_ShowLiveWaveformWhileRecording == settings.Audio_ShowLiveWaveformWhileRecording
               && this.Audio_TTSVoice == settings.Audio_TTSVoice
               && this.Audio_UseRecordBtnToRecordOverSubsequentAudio == settings.Audio_UseRecordBtnToRecordOverSubsequentAudio
               && this.Audio_UseRecordingPauseShortcutForStopping == settings.Audio_UseRecordingPauseShortcutForStopping
               && this.Audio_AlwaysMonitorRecordingToolBar == settings.Audio_AlwaysMonitorRecordingToolBar
               && this.Audio_ColorFlickerPreviewBeforeRecording == settings.Audio_ColorFlickerPreviewBeforeRecording
               && this.Audio_PlaySectionUsingPlayBtn == settings.Audio_PlaySectionUsingPlayBtn
               && this.Audio_EnableFileDataProviderPreservation == settings.Audio_EnableFileDataProviderPreservation
               && this.Audio_EnsureCursorVisibilityInUndoOfSplitRecording == settings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording
               && this.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording == settings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording
               && this.Audio_PreventSplittingPages == settings.Audio_PreventSplittingPages
               && this.Audio_SaveAudioZoom == settings.Audio_SaveAudioZoom)
           {
               audioPreferencesMatch = true ;
           }

               if ((selectedProfile == PreferenceProfiles.Project || selectedProfile == PreferenceProfiles.All)
                   && this.Project_DisableRollBackForCleanUp == settings.Project_DisableRollBackForCleanUp
               && this.Project_AutoSave_RecordingEnd == settings.Project_AutoSave_RecordingEnd
               && this.Project_AutoSaveTimeInterval == settings.Project_AutoSaveTimeInterval
               && this.Project_AutoSaveTimeIntervalEnabled == settings.Project_AutoSaveTimeIntervalEnabled
               && this.Project_BackgroundColorForEmptySection == settings.Project_BackgroundColorForEmptySection
               && this.Project_CheckForUpdates == settings.Project_CheckForUpdates
               && this.Project_EnableFreeDiskSpaceCheck == settings.Project_EnableFreeDiskSpaceCheck
               && this.Project_EPUBCheckTimeOutEnabled == settings.Project_EPUBCheckTimeOutEnabled
               && this.Project_Export_AlwaysIgnoreIndentation == settings.Project_Export_AlwaysIgnoreIndentation
               && this.Project_ImportToleranceForAudioInMs == settings.Project_ImportToleranceForAudioInMs
               && this.Project_LatestVersionCheckedByUpdate == settings.Project_LatestVersionCheckedByUpdate
               && this.Project_LeftAlignPhrasesInContentView == settings.Project_LeftAlignPhrasesInContentView
               && this.Project_MinimizeObi == settings.Project_MinimizeObi
               && this.Project_OpenBookmarkNodeOnReopeningProject == settings.Project_OpenBookmarkNodeOnReopeningProject
               && this.Project_OpenLastProject == settings.Project_OpenLastProject
               && this.Project_OptimizeMemory == settings.Project_OptimizeMemory
               && this.Project_PeakMeterChangeLocation == settings.Project_PeakMeterChangeLocation
               && this.Project_SaveObiLocationAndSize == settings.Project_SaveObiLocationAndSize
               && this.Project_SaveProjectWhenRecordingEnds == settings.Project_SaveProjectWhenRecordingEnds
               && this.Project_ShowWaveformInContentView == settings.Project_ShowWaveformInContentView
               && this.Project_EnableMouseScrolling == settings.Project_EnableMouseScrolling
               && this.Project_DisableTOCViewCollapse == settings.Project_DisableTOCViewCollapse
               && this.Project_MaximizeObi == settings.Project_MaximizeObi
               && this.Project_VAXhtmlExport == settings.Project_VAXhtmlExport)
               {
                   projectPreferencesMatch = true ;
               }

               if ((selectedProfile == PreferenceProfiles.Colors || selectedProfile == PreferenceProfiles.All)
                   && this.ColorSettings.BlockBackColor_Anchor == settings.ColorSettings.BlockBackColor_Anchor
               && this.ColorSettings.BlockBackColor_Custom == settings.ColorSettings.BlockBackColor_Custom
               && this.ColorSettings.BlockBackColor_Empty == settings.ColorSettings.BlockBackColor_Empty
               && this.ColorSettings.BlockBackColor_Heading == settings.ColorSettings.BlockBackColor_Heading
               && this.ColorSettings.BlockBackColor_Page == settings.ColorSettings.BlockBackColor_Page
               && this.ColorSettings.BlockBackColor_Plain == settings.ColorSettings.BlockBackColor_Plain
               && this.ColorSettings.BlockBackColor_Selected == settings.ColorSettings.BlockBackColor_Selected
               && this.ColorSettings.BlockBackColor_Silence == settings.ColorSettings.BlockBackColor_Silence
               && this.ColorSettings.BlockBackColor_TODO == settings.ColorSettings.BlockBackColor_TODO
               && this.ColorSettings.BlockBackColor_Unused == settings.ColorSettings.BlockBackColor_Unused
               && this.ColorSettings.BlockForeColor_Anchor == settings.ColorSettings.BlockForeColor_Anchor
               && this.ColorSettings.BlockForeColor_Custom == settings.ColorSettings.BlockForeColor_Custom
               && this.ColorSettings.BlockForeColor_Empty == settings.ColorSettings.BlockForeColor_Empty
               && this.ColorSettings.BlockForeColor_Heading == settings.ColorSettings.BlockForeColor_Heading
               && this.ColorSettings.BlockForeColor_Page == settings.ColorSettings.BlockForeColor_Page
               && this.ColorSettings.BlockForeColor_Plain == settings.ColorSettings.BlockForeColor_Plain
               && this.ColorSettings.BlockForeColor_Selected == settings.ColorSettings.BlockForeColor_Selected
               && this.ColorSettings.BlockForeColor_Silence == settings.ColorSettings.BlockForeColor_Silence
               && this.ColorSettings.BlockForeColor_TODO == settings.ColorSettings.BlockForeColor_TODO
               && this.ColorSettings.BlockForeColor_Unused == settings.ColorSettings.BlockForeColor_Unused             
               && this.ColorSettings.BlockLayoutSelectedColor == settings.ColorSettings.BlockLayoutSelectedColor
               && this.ColorSettings.ContentViewBackColor == settings.ColorSettings.ContentViewBackColor
               && this.ColorSettings.EditableLabelTextBackColor == settings.ColorSettings.EditableLabelTextBackColor
               && this.ColorSettings.ProjectViewBackColor == settings.ColorSettings.ProjectViewBackColor
               && this.ColorSettings.StripBackColor == settings.ColorSettings.StripBackColor
               && this.ColorSettings.StripCursorSelectedBackColor == settings.ColorSettings.StripCursorSelectedBackColor
               && this.ColorSettings.StripForeColor == settings.ColorSettings.StripForeColor
               && this.ColorSettings.StripSelectedBackColor == settings.ColorSettings.StripSelectedBackColor
               && this.ColorSettings.StripSelectedForeColor == settings.ColorSettings.StripSelectedForeColor
               && this.ColorSettings.StripUnusedBackColor == settings.ColorSettings.StripUnusedBackColor
               && this.ColorSettings.StripUnusedForeColor == settings.ColorSettings.StripUnusedForeColor
               && this.ColorSettings.StripWithoutPhrasesBackcolor == settings.ColorSettings.StripWithoutPhrasesBackcolor
               && this.ColorSettings.TOCViewBackColor == settings.ColorSettings.TOCViewBackColor
               && this.ColorSettings.TOCViewForeColor == settings.ColorSettings.TOCViewForeColor
               && this.ColorSettings.TOCViewUnusedColor == settings.ColorSettings.TOCViewUnusedColor
               && this.ColorSettings.ToolTipForeColor == settings.ColorSettings.ToolTipForeColor
               && this.ColorSettings.TransportBarBackColor == settings.ColorSettings.TransportBarBackColor
               && this.ColorSettings.TransportBarLabelBackColor == settings.ColorSettings.TransportBarLabelBackColor
               && this.ColorSettings.TransportBarLabelForeColor == settings.ColorSettings.TransportBarLabelForeColor
               && this.ColorSettings.WaveformBackColor == settings.ColorSettings.WaveformBackColor
               && this.ColorSettings.WaveformBaseLineColor == settings.ColorSettings.WaveformBaseLineColor
               && this.ColorSettings.WaveformHighlightedBackColor == settings.ColorSettings.WaveformHighlightedBackColor
               && this.ColorSettings.FineNavigationColor == settings.ColorSettings.FineNavigationColor
               && this.ColorSettings.RecordingHighlightPhraseColor == settings.ColorSettings.RecordingHighlightPhraseColor
               && this.ColorSettings.EmptySectionBackgroundColor == settings.ColorSettings.EmptySectionBackgroundColor
               && this.ColorSettings.HighlightedSectionNodeWithoutSelectionColor == settings.ColorSettings.HighlightedSectionNodeWithoutSelectionColor

               && this.ColorSettingsHC.BlockBackColor_Anchor == settings.ColorSettingsHC.BlockBackColor_Anchor
               && this.ColorSettingsHC.BlockBackColor_Custom == settings.ColorSettingsHC.BlockBackColor_Custom
               && this.ColorSettingsHC.BlockBackColor_Empty == settings.ColorSettingsHC.BlockBackColor_Empty
               && this.ColorSettingsHC.BlockBackColor_Heading == settings.ColorSettingsHC.BlockBackColor_Heading
               && this.ColorSettingsHC.BlockBackColor_Page == settings.ColorSettingsHC.BlockBackColor_Page
               && this.ColorSettingsHC.BlockBackColor_Plain == settings.ColorSettingsHC.BlockBackColor_Plain
               && this.ColorSettingsHC.BlockBackColor_Selected == settings.ColorSettingsHC.BlockBackColor_Selected
               && this.ColorSettingsHC.BlockBackColor_Silence == settings.ColorSettingsHC.BlockBackColor_Silence
               && this.ColorSettingsHC.BlockBackColor_TODO == settings.ColorSettingsHC.BlockBackColor_TODO
               && this.ColorSettingsHC.BlockBackColor_Unused == settings.ColorSettingsHC.BlockBackColor_Unused
               && this.ColorSettingsHC.BlockForeColor_Anchor == settings.ColorSettingsHC.BlockForeColor_Anchor
               && this.ColorSettingsHC.BlockForeColor_Custom == settings.ColorSettingsHC.BlockForeColor_Custom
               && this.ColorSettingsHC.BlockForeColor_Empty == settings.ColorSettingsHC.BlockForeColor_Empty
               && this.ColorSettingsHC.BlockForeColor_Heading == settings.ColorSettingsHC.BlockForeColor_Heading
               && this.ColorSettingsHC.BlockForeColor_Page == settings.ColorSettingsHC.BlockForeColor_Page
               && this.ColorSettingsHC.BlockForeColor_Plain == settings.ColorSettingsHC.BlockForeColor_Plain
               && this.ColorSettingsHC.BlockForeColor_Selected == settings.ColorSettingsHC.BlockForeColor_Selected
               && this.ColorSettingsHC.BlockForeColor_Silence == settings.ColorSettingsHC.BlockForeColor_Silence
               && this.ColorSettingsHC.BlockForeColor_TODO == settings.ColorSettingsHC.BlockForeColor_TODO
               && this.ColorSettingsHC.BlockForeColor_Unused == settings.ColorSettingsHC.BlockForeColor_Unused
               && this.ColorSettingsHC.BlockLayoutSelectedColor == settings.ColorSettingsHC.BlockLayoutSelectedColor
               && this.ColorSettingsHC.ContentViewBackColor == settings.ColorSettingsHC.ContentViewBackColor
               && this.ColorSettingsHC.EditableLabelTextBackColor == settings.ColorSettingsHC.EditableLabelTextBackColor
               && this.ColorSettingsHC.ProjectViewBackColor == settings.ColorSettingsHC.ProjectViewBackColor
               && this.ColorSettingsHC.StripBackColor == settings.ColorSettingsHC.StripBackColor
               && this.ColorSettingsHC.StripCursorSelectedBackColor == settings.ColorSettingsHC.StripCursorSelectedBackColor
               && this.ColorSettingsHC.StripForeColor == settings.ColorSettingsHC.StripForeColor
               && this.ColorSettingsHC.StripSelectedBackColor == settings.ColorSettingsHC.StripSelectedBackColor
               && this.ColorSettingsHC.StripSelectedForeColor == settings.ColorSettingsHC.StripSelectedForeColor
               && this.ColorSettingsHC.StripUnusedBackColor == settings.ColorSettingsHC.StripUnusedBackColor
               && this.ColorSettingsHC.StripUnusedForeColor == settings.ColorSettingsHC.StripUnusedForeColor
               && this.ColorSettingsHC.StripWithoutPhrasesBackcolor == settings.ColorSettingsHC.StripWithoutPhrasesBackcolor
               && this.ColorSettingsHC.TOCViewBackColor == settings.ColorSettingsHC.TOCViewBackColor
               && this.ColorSettingsHC.TOCViewForeColor == settings.ColorSettingsHC.TOCViewForeColor
               && this.ColorSettingsHC.TOCViewUnusedColor == settings.ColorSettingsHC.TOCViewUnusedColor
               && this.ColorSettingsHC.ToolTipForeColor == settings.ColorSettingsHC.ToolTipForeColor
               && this.ColorSettingsHC.TransportBarBackColor == settings.ColorSettingsHC.TransportBarBackColor
               && this.ColorSettingsHC.TransportBarLabelBackColor == settings.ColorSettingsHC.TransportBarLabelBackColor
               && this.ColorSettingsHC.TransportBarLabelForeColor == settings.ColorSettingsHC.TransportBarLabelForeColor
               && this.ColorSettingsHC.WaveformBackColor == settings.ColorSettingsHC.WaveformBackColor
               && this.ColorSettingsHC.WaveformBaseLineColor == settings.ColorSettingsHC.WaveformBaseLineColor
               && this.ColorSettingsHC.WaveformHighlightedBackColor == settings.ColorSettingsHC.WaveformHighlightedBackColor
               && this.ColorSettingsHC.FineNavigationColor == settings.ColorSettingsHC.FineNavigationColor
               && this.ColorSettingsHC.RecordingHighlightPhraseColor == settings.ColorSettingsHC.RecordingHighlightPhraseColor
               && this.ColorSettingsHC.EmptySectionBackgroundColor == settings.ColorSettingsHC.EmptySectionBackgroundColor
               && this.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor == settings.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor)
               {
                   colorPreferenceMatch = true ;
               }

               if ((selectedProfile == PreferenceProfiles.UserProfile || selectedProfile == PreferenceProfiles.All)
                   && this.UserProfile.Culture == settings.UserProfile.Culture)
           {
               usersProfilePreferencesMatch= true;
           }

           if (selectedProfile == PreferenceProfiles.Project) return projectPreferencesMatch;
           if (selectedProfile == PreferenceProfiles.Audio) return audioPreferencesMatch;
           if (selectedProfile == PreferenceProfiles.UserProfile) return usersProfilePreferencesMatch;
           if (selectedProfile == PreferenceProfiles.Colors) return colorPreferenceMatch;

           if (selectedProfile == PreferenceProfiles.All
               && projectPreferencesMatch
               && audioPreferencesMatch
               && usersProfilePreferencesMatch
               && colorPreferenceMatch)
           {
               return true;
           }

           return false;
       }

            }
}
