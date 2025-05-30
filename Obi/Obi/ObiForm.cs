using System.Drawing;
using Obi.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using urakawa.core;
using urakawa.data;
using PipelineInterface;
using Jaime.Olivares;
using System.Security;
using Obi.ImportExport;
using System.Globalization;


namespace Obi
    {
        /// <summary>
        /// The main form of the application.
        /// This is basically the shell of the project view along with menu bars.
        /// </summary>
        public partial class ObiForm : Form
        {
            #region Members and initializers

            private float mBaseFontSize; // base font size
            private Audio.PeakMeterForm mPeakMeter; // maintain a single "peak meter" form
            private Obi.UserControls.RecordingToolBarForm mRecordingToolBarForm;
            private Session mSession; // current work session
            private Settings mSettings; // application settings
            private Settings m_DefaultSettings;
            private Settings_Permanent m_Settings_Permanent;
            private KeyboardShortcuts_Settings m_KeyboardShortcuts; // keyboard shortcuts used by application
            private Dialogs.ShowSource mSourceView; // maintain a single "source view" dialog
            private PipelineInterface.PipelineInfo mPipelineInfo; // instance for easy access to pipeline information
            private bool mShowWelcomWindow; // flag for controlling showing of welcome window
            private Timer mAutoSaveTimer;
            private bool m_IsSaveActive;
            private bool m_IsAutoSaveActive;
            private bool m_CanAutoSave = true;

            private bool m_IsStatusBarEnabled;
                         //@singleSection: allow disabling status bar for things like long operations

            private bool m_InputDeviceFound = true;
            private bool m_OutputDevicefound = true;

            private static readonly float ZOOM_FACTOR_INCREMENT = 1.4f; // zoom factor increment (zoom in/out)
            private static readonly float DEFAULT_ZOOM_FACTOR_HC = 1.2f; // default zoom factor (high contrast mode)
            private static readonly float AUDIO_SCALE_INCREMENT = 1.4f; // audio scale increment (audio zoom in/out)

            private string m_RestoreProjectFilePath = null;
            private string m_OriginalPath = null;
            private bool flagMaxWindow = false;
            private bool m_flag = false;// used for controling  the execution of certain statements during resize of obi window
            private static bool m_flagForPeakMeterHeight = false;
            private int m_tempHeightMin=0; // Stores minimum height of peak meter
            private bool m_FlagWindowState = false; //indicates whether window is in max or min state
            private int m_DiffPeakMeterGraphicalPeakMeter=0; // stores the difference between the hight of the peak meter and graphical peak  
            private bool m_NormalAfterMax = false;    //Used to revert back to the window of the original size from max obi window
            private bool m_FlagLangUpdate = false;
            private string m_ObiFont;//@fontconfig
            private int m_TotalTimeIntervalSinceLastBackup = 0;
            private bool m_IsSkippableNoteInProject = false;


            /// <summary>
            /// Initialize a new form and open the last project if set in the preferences.
            /// </summary>
            public ObiForm()
            {
                mShowWelcomWindow = true;
                InitializeObi();
                m_PreventSettingsUpdateOnClosing = false;
                m_IsSaveActive = false;
                m_DefaultSettings = Settings.GetDefaultSettings();
            }

            /// <summary>
            /// Initialize a new form and open a project from the path given as parameter.
            /// </summary>
            public ObiForm(string path)
            {
                mShowWelcomWindow = false;
                InitializeObi();
                OpenProject_Safe(path, null);
                m_IsSaveActive = false;
                m_DefaultSettings = Settings.GetDefaultSettings();
            }

            #endregion


            #region Properties

            private bool AllowOverwrite
            {
                set
                {
                    mAllowOverwriteToolStripMenuItem.Checked = value;
                    mSettings.Audio_AllowOverwrite = value;
                }
            }

            public string ObiFontName //@fontconfig
            {
                set
                {
                    m_ObiFont = value;
                }
                get
                {
                    return m_ObiFont;
                }
            }

            /// <summary>
            /// Global audio scale for waveforms.
            /// </summary>
            public float AudioScale
            {
                get { return mSettings.Audio_AudioScale; }
                set
                {
                    if (value > 0.002f && value < 0.1f)
                    {
                        mSettings.Audio_AudioScale = value;
                        mProjectView.AudioScale = value;
                    }
                }
            }

            public ColorSettings ColorSettings
            {
                get
                {
                    ColorSettings settings = SystemInformation.HighContrast
                                                 ? (mSettings == null
                                                        ? ColorSettings.DefaultColorSettingsHC()
                                                        : mSettings.ColorSettingsHC)
                                                 : (mSettings == null
                                                        ? ColorSettings.DefaultColorSettings()
                                                        : mSettings.ColorSettings);
                    settings.CreateBrushesAndPens();
                    return settings;
                }
            }

            /// <summary>
            /// Application settings.
            /// </summary>
            public Settings Settings
            {
                get { return mSettings; }
            }

            /// <summary>
            /// Values of configurable keyboard shortcuts
            /// </summary>
            public KeyboardShortcuts_Settings KeyboardShortcuts
            {
                get { return m_KeyboardShortcuts; }
            }

            public Settings_Permanent Settings_Permanent
            {
                get { return m_Settings_Permanent; }
            }

            private bool m_PreventSettingsUpdateOnClosing = false;
            public bool PreventSettingsUpdateOnClosing
            {
                get { return m_PreventSettingsUpdateOnClosing; }
                set
                {
                    m_PreventSettingsUpdateOnClosing = value;
                }
            }

            // True if the user has chosen the "open last project" option, and there is a last project to open.
            private bool ShouldOpenLastProject
            {
                get { return mSettings != null && mSettings.Project_OpenLastProject && mSettings.LastOpenProject != "" && mShowWelcomWindow; }
            }

            /// Set view synchronization and update the menu and settings accordingly.
            private bool SynchronizeViews
            {
                set
                {
                    mSettings.SynchronizeViews = value;
                    mSynchronizeViewsToolStripMenuItem.Checked = value;
                    mProjectView.SynchronizeViews = value;
                }
            }

            // Set content wrapping in strips
            private bool WrapStripContents
            {
                set
                {
                    // Switch comments to disable
                    // mSettings.WrapStrips = false;
                    // mProjectView.WrapStrips = false;
                    mSettings.WrapStripContents = value;
                    // mWrappingInContentViewToolStripMenuItem.Checked = value;
                    mProjectView.WrapStripContents = value;
                }
            }

            /// <summary>
            /// Global zoom factor for all controls in the form.
            /// </summary>
            public float ZoomFactor
            {
                get { return mSettings.ZoomFactor; }
                set
                {
                    if (value > 0.5f && value <= 4.0f)
                    {
                        mSettings.ZoomFactor = value;
                        UpdateZoomFactor();
                        mView_NormalSizeMenuItem.Enabled = value != 1.0;
                    }
                }
            }
            /// <summary>
            /// It is used to fix TOC Splitter in ProjectView.
            /// </summary>
            public bool FixTOCViewWidth
            {
                set
                {
                    mProjectView.FixTocViewWidth = value;
                }
            }          
            /// <summary>
            /// It is used to save TOCView width when checkbox in preferences is checked
            /// </summary>
            public void TOCViewWidth()
            {
                mSettings.TOCViewWidth = mProjectView.TOCViewWidth;
            }
            // Update the zoom factor for the form itself after it was set.
            private void UpdateZoomFactor()
            {
                float z = mSettings.ZoomFactor*(SystemInformation.HighContrast ? DEFAULT_ZOOM_FACTOR_HC : 1.0f);
                mStatusLabel.Font = new System.Drawing.Font(mStatusLabel.Font.FontFamily, mBaseFontSize*z);
                mProjectView.ZoomFactor = z;
            }

            #endregion


            #region File menu

            private void UpdateFileMenu()
            {
                mFile_NewProjectMenuItem.Enabled = true;
                mFile_NewProjectFromImportMenuItem.Enabled = true;
                mFile_OpenProjectMenuItem.Enabled = true;
                mFile_CloseProjectMenuItem.Enabled = mSession.HasProject;
                // MessageBox.Show(mSession.HasProject.ToString());
                if (m_OriginalPath == null)
                {
                    m_RestoreFromOriginalProjectToolStripMenuItem.Visible = false;
                    m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = false;
                    m_RestoreFromBackupToolStripMenuItem.Visible = true;
                    m_RestoreFromBackupToolStripMenuItem.Enabled = true;
                }
                else
                {
                    m_RestoreFromOriginalProjectToolStripMenuItem.Visible = true;
                    m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = true;
                    m_RestoreFromBackupToolStripMenuItem.Visible = false;
                    m_RestoreFromBackupToolStripMenuItem.Enabled = false;
                }

                m_RestoreFromBackupToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive ;
                mFile_SaveProjectMenuItem.Enabled = mSession.CanSave;
                mFile_SaveProjectAsMenuItem.Enabled = mSession.HasProject;
                mFile_RecentProjectMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
                mFile_ClearListMenuItem.Enabled = true;
                mFile_ExitMenuItem.Enabled = true;
                mFile_MergeProjectMenuItem.Enabled = mProjectView.CanMergeProject;
            }

            private void File_NewProjectMenuItem_Click(object sender, EventArgs e)
            {
                NewProject();
            }

            private void File_NewProjectFromImportMenuItem_Click(object sender, EventArgs e)
            {
                NewProjectFromImport();
            }

            private void File_OpenProjectMenuItem_Click(object sender, EventArgs e)
            {
                Open();
            }

            private void File_CloseProjectMenuItem_Click(object sender, EventArgs e)
            {
                DidCloseProject();
            }

            private void File_SaveProjectMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@monitorContinuously
                Save();
            }

            private void File_SaveProjectAsMenuItem_Click(object sender, EventArgs e)
            {
                SaveAs();
            }

            private void File_ClearListMenuItem_Click(object sender, EventArgs e)
            {
                ClearRecentProjectsList();
            }

            private void File_ExitMenuItem_Click(object sender, EventArgs e)
            {
                Close();
            }

            // Create a new project by asking initial information through a dialog.
            private void NewProject()
            {
                m_IsStatusBarEnabled = true;
                if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive)
                {
                    if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                    mProjectView.TransportBar.Stop();
                }
                if (DidCloseProject())
                {
                    Dialogs.NewProject dialog = new Dialogs.NewProject(
                        mSettings.Project_DefaultPath,
                        Localizer.Message("default_project_filename"),
                        Localizer.Message("obi_filter"),
                        Localizer.Message("default_project_title"),
                        mSettings.NewProjectDialogSize,
                        mSettings.Audio_Channels,
                        mSettings.Audio_SampleRate,mSettings);
                    dialog.CreateTitleSection = mSettings.CreateTitleSection;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection, dialog.ID, dialog.AudioChannels, dialog.AudioSampleRate);
                        AddRecentProject(mSession.Path);
                    }
                    mSettings.CreateTitleSection = dialog.CreateTitleSection;
                    mSettings.NewProjectDialogSize = dialog.Size;
                }
            }

            // Create a new project by importing an XHTML file.
            // Prompt the user for the location of the file through a dialog.
            private void NewProjectFromImport()
            {
                if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive)
                {
                    if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                    mProjectView.TransportBar.Stop();
                }
                if (DidCloseProject())
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = Localizer.Message("choose_import_file");
                    dialog.Filter = Localizer.Message("filter");
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        if (!NewProjectFromImport(dialog.FileName))
                        {
                            try
                            {
                                if (mSession.Path != null) RemoveRecentProject(mSession.Path);

                                mSession.CleanupAfterFailure();
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(string.Format(Localizer.Message("could_not_clean_up"), e.Message),
                                                Localizer.Message("could_not_clean_up_caption"),
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }

            // Create a new project by importing an XHTML file at the given path.
            // Return success status.
            private bool NewProjectFromImport(string path)
            {
                Dialogs.NewProject dialog = null;
                ImportExport.ConfigurationFileParser configurationInstance = GetObiConfigurationFileInstance(path);
                try
                {
                    string title = "";
                    string dtbUid = null;

                    string strExtension = System.IO.Path.GetExtension(path).ToLower();
                    if (strExtension == ".opf")
                    {
                        Obi.ImportExport.DAISY3_ObiImport.getTitleFromOpfFile(path, ref title, ref dtbUid);
                    }
                    if (strExtension == ".xhtml" || strExtension == ".html" || strExtension == ".htm")
                    {
                        title = ImportExport.ImportStructureFromXhtml.GrabTitle(new Uri(path));
                        dtbUid = ImportExport.DAISY202Import.GrabIdentifier(path);
                    }
                    else if (strExtension == ".xml")
                    {
                        ImportExport.DAISY3_ObiImport.getTitleFromDtBookFile(path, ref title, ref dtbUid);
                    }
                    else if (strExtension == ".epub")
                    {
                        string opfFullPath = unzipEPubAndGetOpfPath(path);
                        if (opfFullPath != null && File.Exists(opfFullPath)) ImportExport.DAISY3_ObiImport.getTitleFromOpfFile(opfFullPath, ref title, ref dtbUid);
                    }

                    if (string.IsNullOrEmpty(title))
                    {
                        title = Localizer.Message("default_project_title");
                    }

                    this.Activate(); //place focus back to Obi form to ensure that keyboard focus from here on is not lost
                    if (configurationInstance == null)
                    {
                        dialog = new Dialogs.NewProject(
                            mSettings.Project_DefaultPath,
                            Localizer.Message("default_project_filename"),
                            Localizer.Message("obi_filter"),
                            title,
                            mSettings.NewProjectDialogSize,
                            mSettings.Audio_Channels,
                            mSettings.Audio_SampleRate,mSettings);
                        dialog.DisableAutoTitleCheckbox();
                        dialog.Text = Localizer.Message("create_new_project_from_import");
                        if (!string.IsNullOrEmpty(dtbUid)) dialog.ID = dtbUid;
                    }

                    if (configurationInstance != null
                        || dialog.ShowDialog() == DialogResult.OK)
                    {
                        string obiProjectPath = configurationInstance != null ? Path.Combine(configurationInstance.ObiProjectDirectoryPath, "project.obi") :
                            dialog.Path;

                        if (File.Exists(obiProjectPath)
                            &&
                            MessageBox.Show(Localizer.Message("ImportProject_OverwriteProjectMsg"),
                                            Localizer.Message("Caption_Warning"),
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                            MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return false;
                        }

                        if (dialog != null) mSettings.NewProjectDialogSize = dialog.Size;
                        //CreateNewProject ( dialog.Path, dialog.Title, false, dialog.ID );
                        ImportExport.DAISY3_ObiImport import = null;
                        bool isProjectCreated = false;
                        // create directory from config file if it does not exist
                        if (configurationInstance != null && !Directory.Exists(configurationInstance.ObiProjectDirectoryPath))
                        {
                            Directory.CreateDirectory(configurationInstance.ObiProjectDirectoryPath);
                        }
                        if (dialog != null) title = dialog.Title;
                        Console.WriteLine("title : " + title);
                        string uniqueIdentifier = dialog != null ? dialog.ID :
                            !string.IsNullOrEmpty(dtbUid) ? dtbUid :
                            Guid.NewGuid().ToString();
                        Console.WriteLine("UID : " + uniqueIdentifier);
                        int audioChannels = configurationInstance != null ? configurationInstance.ImportChannels :
                            dialog.AudioChannels;
                        Console.WriteLine("channels : " + audioChannels);
                        int audioSampleRate = configurationInstance != null ? configurationInstance.ImportSampleRate :
                            dialog.AudioSampleRate;
                        Console.WriteLine("Samle rate : " + audioSampleRate);
                        if (strExtension == ".opf" || strExtension == ".xml" || strExtension == ".epub")
                        {
                            isProjectCreated = ImportProjectFromDTBOrEPUB(obiProjectPath, title, false, uniqueIdentifier, path, audioChannels, audioSampleRate);
                        }
                        else
                        {
                            //CreateNewProject(dialog.Path, dialog.Title, false, dialog.ID);
                            //(new Obi.ImportExport.ImportStructure()).ImportFromXHTML(path, mSession.Presentation);
                            isProjectCreated = ImportStructureFromXHtml(obiProjectPath, title, uniqueIdentifier, path, audioChannels, audioSampleRate);
                        }
                        if (!isProjectCreated) return false;

                        mSession.ForceSave();
                        AddRecentProject(mSession.Path);

                        //copy the configuration file in Obi project directory
                        if (configurationInstance != null && File.Exists(configurationInstance.ConfigurationFilePath))
                        {
                            mSession.Presentation.ConfigurationsImportExport = configurationInstance;
                            string preservedConfigFilePath = Path.Combine(Path.GetDirectoryName(obiProjectPath), mSettings.Project_ObiConfigFileName);
                            File.Copy(configurationInstance.ConfigurationFilePath, preservedConfigFilePath, true);
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    
                    MessageBox.Show(string.Format(Localizer.Message("import_failed"), e.Message),
                                    Localizer.Message("import_failed_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            private string unzipEPubAndGetOpfPath(string path)
            {
                try
                {
                    return ImportExport.EPUB3_ObiImport.unzipEPubAndGetOpfPath(path);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                return null;
            }

            private ImportExport.ConfigurationFileParser GetObiConfigurationFileInstance(string sourceProjectFile)
            {
                ImportExport.ConfigurationFileParser configInstance = null ;
                try
                {
                    string configFilePath = Path.Combine(Path.GetDirectoryName(sourceProjectFile),
                        mSettings.Project_ObiConfigFileName);
                    configInstance = ImportExport.ConfigurationFileParser.GetConfigurationFileInstance(configFilePath);

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                return configInstance;
            }

            private bool ImportProjectFromDTBOrEPUB(string outputPath, string title, bool createTitleSection, string id,
                                              string importDTBPath, int audioChannels, int audioSampleRate)
            {

                importDTBPath = System.IO.Path.GetFullPath(importDTBPath);

                importDTBPath = System.IO.Path.GetFullPath(importDTBPath);
                mSession.CreateNewPresentationInBackend(outputPath, title, createTitleSection, id, mSettings, true, audioChannels,audioSampleRate);

                ImportExport.DAISY3_ObiImport import = null;
                if (ImportExport.DAISY3_ObiImport.IsEPUBPublication(importDTBPath))
                {
                    import = new Obi.ImportExport.EPUB3_ObiImport(mSession, mSettings,
                                                                             importDTBPath,
                                                                             System.IO.Path.
                                                                                 GetDirectoryName(
                                                                                     outputPath), false,
                                                                   audioSampleRate == 96000 ? AudioLib.SampleRate.Hz96000 : audioSampleRate == 48000? AudioLib.SampleRate.Hz48000 :  audioSampleRate == 44100 ? AudioLib.SampleRate.Hz44100 : audioSampleRate == 22050 ? AudioLib.SampleRate.Hz22050 : AudioLib.SampleRate.Hz11025,
                                                                             audioChannels ==
                                                                             2);
                }
                else
                {
                    import = new Obi.ImportExport.DAISY3_ObiImport(mSession, mSettings,
                                                                                                 importDTBPath,
                                                                                                 System.IO.Path.
                                                                                                     GetDirectoryName(
                                                                                                         outputPath), false,
                                                                    audioSampleRate == 96000 ? AudioLib.SampleRate.Hz96000 : audioSampleRate == 48000 ? AudioLib.SampleRate.Hz48000 : audioSampleRate == 44100 ? AudioLib.SampleRate.Hz44100 : audioSampleRate == 22050 ? AudioLib.SampleRate.Hz22050 : AudioLib.SampleRate.Hz11025,
                                                                                                 audioChannels ==
                                                                                                 2);
                }
                ProgressDialog progress = new ProgressDialog(Localizer.Message("import_progress_dialog_title"),
                                                             delegate(ProgressDialog progress1)
                                                                 {
                                                                     import.DoWork();
                                                                     if (import.RequestCancellation)
                                                                     {
                                                                         //mSession.pro = null;
                                                                         return;
                                                                     }
                                                                     mSession.Presentation.
                                                                         CheckAndCreateDefaultMetadataItems(
                                                                             mSettings.UserProfile);
                                                                     import.CorrectExternalAudioMedia();
                                                                     mSession.Save(mSession.Path);
                                                                 });
                progress.OperationCancelled += new OperationCancelledHandler(delegate(object sender, EventArgs e)
                                                                                 {
                                                                                     if (import != null)
                                                                                         import.RequestCancellation =
                                                                                             true;
                                                                                 });
                import.ProgressChangedEvent +=
                    new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                progress.ShowDialog();
                if (progress.Exception != null) throw progress.Exception;
                if (import.RequestCancellation) return false;
                if (!import.RequestCancellation) mSession.NotifyProjectCreated();
                
                Dialogs.ReportDialog reportDialog = new ReportDialog(Localizer.Message("Report_for_import"),
                                                                     import.RequestCancellation
                                                                         ? Localizer.Message("import_cancelled")
                                                                         : String.Format(
                                                                             Localizer.Message("import_output_path"),
                                                                             import != null && import.ErrorsList.Count >0?Localizer.Message("ImportErrorCorrectionText"): "",
                                                                             outputPath),
                                                                     import != null ? import.ErrorsList : null);
                reportDialog.ShowDialog();
                return !import.RequestCancellation;
            }

            private bool ImportStructureFromXHtml(string path, string title, string id, string xhtmlPath, int audioChannels, int audioSampleRate)
            {
                mSession.CreateNewPresentationInBackend(path, title, false, id, mSettings, true, audioChannels, audioSampleRate);
                ImportExport.DAISY202Import import = new Obi.ImportExport.DAISY202Import(xhtmlPath, mSession.Presentation, mSettings);
                List<string> audioFilePaths = new List<string>();
                string audioFilesNotImportedDuringCSVImport = string.Empty;
                
                                
                ProgressDialog progress = new ProgressDialog(Localizer.Message("import_progress_dialog_title"),
                                                             delegate(ProgressDialog progress1)
                                                                 {
                                                                     if (Path.GetExtension(xhtmlPath).ToLower() == ".csv"
                                                                         || Path.GetExtension(xhtmlPath).ToLower() == ".txt")
                                                                     {
                                                                         ImportExport.ImportStructureFromCSV csvImport = new Obi.ImportExport.ImportStructureFromCSV();
                                                                         csvImport.ImportFromCSVFile(xhtmlPath, mSession.Presentation, mProjectView);
                                                                         audioFilePaths = csvImport.AudioFilePaths;
                                                                         audioFilesNotImportedDuringCSVImport = csvImport.AudioFilesNotImported;
                                                                         string DirectoryName = Path.GetDirectoryName(xhtmlPath);
                                                                         string metaDataFilePath = DirectoryName + "\\metadata.csv"; 
                                                                         if(File.Exists(metaDataFilePath))
                                                                         {
                                                                             ImportExport.ImportMetadata metadataImport = new ImportMetadata();
                                                                             metadataImport.ImportFromCSVFile(metaDataFilePath, mSession.Presentation, mProjectView, true);
                                                                         }
                                                                     }
                                                                     else
                                                                     {
                                                                         import.DoWork();
                                                                         mSession.Presentation.CheckAndCreateDefaultMetadataItems(
                                                                             mSettings.UserProfile);
                                                                         import.CorrectExternalAudioMedia();
                                                                         
                                                                     }
                                                                     ImportStructureFromXHtml_ThreadSafe(path, title, id,
                                                                                                             xhtmlPath, audioChannels, audioSampleRate);
                                                                 });
                progress.OperationCancelled += new OperationCancelledHandler(delegate(object sender, EventArgs e)
                                                                                 {
                                                                                     if (import != null) import.RequestCancellation = true;
                                                                                         
                                                                                 });
                import.ProgressChangedEvent +=
                    new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                progress.ShowDialog();
                if (progress.Exception != null) throw progress.Exception;

                Dialogs.ReportDialog reportDialog;
                if (audioFilesNotImportedDuringCSVImport != string.Empty)
                {
                    reportDialog = new ReportDialog(Localizer.Message("Report_for_import"),
                           import.RequestCancellation ? Localizer.Message("import_cancelled")
                                                                            : String.Format(
                                                                                Localizer.Message("ImportOfCSVStatus"),
                                                                                import != null && import.ErrorsList.Count > 0 ? Localizer.Message("ImportErrorCorrectionText") : "",
                                                                                path, string.Format(Localizer.Message("FilesNotImportedDuringCSVImport"), audioFilesNotImportedDuringCSVImport)),
                                                                        import != null ? import.ErrorsList : null);
                    
                }
                else
                {
                    reportDialog = new ReportDialog(Localizer.Message("Report_for_import"),
                           import.RequestCancellation ? Localizer.Message("import_cancelled")
                                                                            : String.Format(
                                                                                Localizer.Message("ImportOfCSVStatus"),
                                                                                import != null && import.ErrorsList.Count > 0 ? Localizer.Message("ImportErrorCorrectionText") : "",
                                                                                path,string.Empty),
                                                                        import != null ? import.ErrorsList : null);
                    
                }

                reportDialog.ShowDialog();
                return !import.RequestCancellation; 
            }

            private delegate void ImportStructureFromXHtml_Delegate(
                string path, string title, string id, string xhtmlPath, int audioChannels, int audioSampleRate);

            private void ImportStructureFromXHtml_ThreadSafe(string path, string title, string id, string xhtmlPath, int audioChannels, int audioSampleRate)
            {
                if (InvokeRequired)
                {
                    Invoke(new ImportStructureFromXHtml_Delegate(ImportStructureFromXHtml_ThreadSafe), path, title, id,
                           xhtmlPath, audioChannels, audioSampleRate);
                }
                else
                {
                    // comment old xhtml structure import code
                    //CreateNewProject(path, title, false, id, audioChannels,audioSampleRate);
                    //(new Obi.ImportExport.ImportStructure()).ImportFromXHTML(xhtmlPath, mSession.Presentation);

                    mSession.NotifyProjectCreated();

                }
            }

            // Open a new project after showing a file open dialog.
            private void Open()
            {
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive)
                {
                    mProjectView.TransportBar.Stop();
                }
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("obi_filter");
                dialog.InitialDirectory = mSettings.Project_DefaultPath;
                if (dialog.ShowDialog() == DialogResult.OK && DidCloseProject())
                    OpenProject_Safe(dialog.FileName, null);
            }

            // Clear the list of recently opened files (prompt the user first.)
            private void ClearRecentProjectsList()
            {
                if (MessageBox.Show(Localizer.Message("clear_recent_text"),
                                    Localizer.Message("clear_recent_caption"),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    while (mFile_RecentProjectMenuItem.DropDownItems.Count > 2)
                    {
                        mFile_RecentProjectMenuItem.DropDownItems.RemoveAt(0);
                    }
                    mSettings.RecentProjects.Clear();
                    mFile_RecentProjectMenuItem.Enabled = false;
                }
            }

            /// <summary>
            /// Check that a project path is correct (directory is usable; extension is OK with user.)
            /// If createDir is set, try to create a directory to save to.
            /// This is the safe version that does not throw exceptions.
            /// </summary>
            public static bool CheckProjectPath_Safe(string path, bool createDir)
            {
                bool check = false;
                try
                {
                    check = CheckProjectPath(path, createDir);
                }
                catch (Exception)
                {
                }
                return check;
            }

            /// <summary>
            /// Check that a project path is correct (directory is usable; extension is OK with user.)
            /// If createDir is set, try to create a directory to save to.
            /// This is the safe version that does not throw exceptions.
            /// </summary>
            public static bool CheckProjectPath(string path, bool createDir)
            {
                // Check that a URI can be built from this path because that's what will happen in the end.
                if (!Path.IsPathRooted(path))
                    throw new Exception(string.Format(Localizer.Message("path_not_rooted"), path));
                try
                {
                    Uri uri = new Uri(path);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("path_not_recognized", path, e.Message));
                }
                return CheckProjectDirectory(Path.GetDirectoryName(path), true) && CheckExtension(path);
            }

            /// <summary>
            /// Check the extension of a project file. If it is not .obi, ask the user if they really want to
            /// use the path (it may be a mistake on their part.)
            /// </summary>
            public static bool CheckExtension(string path)
            {
                return Path.GetExtension(path) == ".obi" ||
                       MessageBox.Show(string.Format(Localizer.Message("extension_warning"), path),
                                       Localizer.Message("extension_warning_caption"),
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
            }

            /// <summary>
            /// Check that a directory can host a project or export.
            /// </summary>
            public static bool CheckProjectDirectory(string path, bool checkEmpty, bool showMessageBox = true)
            {
                return Directory.Exists(path) ? CheckEmpty(path, checkEmpty,showMessageBox) : DidCreateDirectory(path, true);
            }

            /// <summary>
            /// Check that a directory can host a project or export. Safe version.
            /// </summary>
            public static bool CheckProjectDirectory_Safe(string path, bool checkEmpty, bool showMessageBox = true)
            {
                bool check = false;
                try
                {
                    check = CheckProjectDirectory(path, checkEmpty, showMessageBox);
                }
                catch (Exception)
                {
                }
                return check;
            }

            // Save the current project
            public void Save()
            {
                if (mSession != null && mSession.CanSave
                    && !m_IsSaveActive)
                {
                    m_IsSaveActive = true;
                    
                    if (mProjectView.TransportBar.IsPlayerActive || mProjectView.TransportBar.IsRecorderActive)
                    {
                        mProjectView.TransportBar.Stop();
                    }

                    // Freeze restore should return only if the function do not return null
                    if (FreezeChangesFromProjectRestore() != null)
                    {
                        m_IsSaveActive = false;
                        return;
                    }

                    mSession.Save();


                    mStatusLabel.Text = Localizer.Message("Status_ProjectSaved");
                    // reset the  auto save timer
                  //  mAutoSaveTimer.Stop(); // Resetting of Timer disabled for making forced backups.
                    if (mSettings.Project_AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();
                    m_IsSaveActive = false;
                }
            }

            /// <summary>
            /// Saves project to a backup .obi file
            /// </summary>
            public void SaveToBackup()
            {
                if (mSession != null && mSession.CanSave
                    && !m_IsSaveActive)
                {
                    m_IsSaveActive = true;
                    mFile_SaveProjectMenuItem.Enabled = false;
                    mFile_SaveProjectAsMenuItem.Enabled = false;

                    if (mProjectView.TransportBar.IsRecorderActive)
                    {
                        mProjectView.TransportBar.Stop();
                    }
                    //DateTime currentDateTime= DateTime.Now ;
                    DateTime currentDateTime = File.GetLastWriteTime(mSession.BackUpPath); 
                    string postFix = currentDateTime.Year.ToString() + "-"
                        + (currentDateTime.Month.ToString().Length > 1 ? currentDateTime.Month.ToString() : "0" + currentDateTime.Month.ToString())  + "-"
                        + (currentDateTime.Day.ToString().Length > 1 ? currentDateTime.Day.ToString() : "0" + currentDateTime.Day.ToString())
                        + "-" + currentDateTime.Hour.ToString() + "hr" + currentDateTime.Minute.ToString() + "mins"; 
                    string backUpFileCopyAtInterval = Path.Combine(Path.GetDirectoryName(mSession.BackUpPath), postFix + Path.GetFileName(mSession.BackUpPath) );
                        
                    Console.WriteLine(backUpFileCopyAtInterval);
                    if (File.Exists(mSession.BackUpPath) && !File.Exists(backUpFileCopyAtInterval) && m_TotalTimeIntervalSinceLastBackup >= 1800000)
                    {
                        try
                        {
                            File.Move(mSession.BackUpPath, backUpFileCopyAtInterval);
                            m_TotalTimeIntervalSinceLastBackup = 0;
                        }
                        catch (System.Exception ex)
                        {
                            mProjectView.WriteToLogFile(ex.ToString());
                        }
                    }
                    mSession.SaveToBackup();
                    //mStatusLabel.Text = Localizer.Message ( "Status_ProjectSaved" );
                    // reset the  auto save timer
                    mAutoSaveTimer.Stop();
                    if (mSettings.Project_AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();

                    mFile_SaveProjectMenuItem.Enabled = true;
                    mFile_SaveProjectAsMenuItem.Enabled = true;
                    m_IsSaveActive = false;
                }
            }

            public void CopyToBackup()
            {
                bool isSaveProjectEnabled = false;
                bool isSaveProjectAsEnabled = false;
                if (mFile_SaveProjectMenuItem.Enabled)
                {
                    isSaveProjectEnabled = true;
                    mFile_SaveProjectMenuItem.Enabled = false;
                }
                if (mFile_SaveProjectAsMenuItem.Enabled)
                {
                    isSaveProjectAsEnabled = true;
                    mFile_SaveProjectAsMenuItem.Enabled = false;
                }

                DateTime currentDateTime = File.GetLastWriteTime(mSession.Path); 
                string postFix = currentDateTime.Year.ToString() + "-"
                    + (currentDateTime.Month.ToString().Length > 1 ? currentDateTime.Month.ToString() : "0" + currentDateTime.Month.ToString()) + "-"
                    + (currentDateTime.Day.ToString().Length > 1 ? currentDateTime.Day.ToString() : "0" + currentDateTime.Day.ToString())
                    + "-" + currentDateTime.Hour.ToString() + "hr" + currentDateTime.Minute.ToString() + "mins";
                string backUpFileCopyAtInterval = Path.Combine(Path.GetDirectoryName(mSession.BackUpPath), postFix + Path.GetFileName(mSession.BackUpPath));
             //   string projectDirPath = Directory.GetParent(mSession.Path).FullName;
                if (File.Exists(mSession.Path) && !File.Exists(backUpFileCopyAtInterval))
                {
                    try
                    {
                        File.Copy(mSession.Path, backUpFileCopyAtInterval);
                        m_TotalTimeIntervalSinceLastBackup = 0;
                    }
                    catch (System.Exception ex)
                    {
                        mProjectView.WriteToLogFile(ex.ToString());
                    }
                }
                else
                {
                    m_TotalTimeIntervalSinceLastBackup = 0;
                }
                if(isSaveProjectEnabled)
                mFile_SaveProjectMenuItem.Enabled = true;
                if(isSaveProjectAsEnabled)
                mFile_SaveProjectAsMenuItem.Enabled = true;
            }

            // Save the current project under a different name; ask for a new path first.
            private void SaveAs()
            {
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                mProjectView.TransportBar.Stop();
                string path_original = mSession.Path;
                SaveProjectAsDialog dialog = new SaveProjectAsDialog(path_original, mSettings); //@fontconfig
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    m_IsSaveActive = true;
                    string path_new = dialog.NewProjectPath;
                    try
                    {
                        if (mSession.CanSave &&
                            MessageBox.Show(Localizer.Message("save_before_save_as"),
                                            Localizer.Message("save_before_save_as_caption"),
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            mSession.ForceSave();
                        }
                        ProgressDialog progress = new ProgressDialog(Localizer.Message("save_as_progress_dialog_title"),
                                                                     delegate()
                                                                         {
                                                                             DirectoryInfo dir_original =
                                                                                 new DirectoryInfo(
                                                                                     Path.GetDirectoryName(path_original));
                                                                             DirectoryInfo dir_new =
                                                                                 new DirectoryInfo(
                                                                                     Path.GetDirectoryName(path_new));
                                                                             ShallowCopyFilesInDirectory(
                                                                                 dir_original.FullName, dir_new.FullName);
                                                                             mSession.RemoveLock_Additional_safe(
                                                                                 path_new);
                                                                             DirectoryInfo[] dirs =
                                                                                 dir_original.GetDirectories("*.*",
                                                                                                             SearchOption
                                                                                                                 .
                                                                                                                 AllDirectories);
                                                                             foreach (DirectoryInfo d in dirs)
                                                                             {
                                                                                 string dest = dir_new.FullName +
                                                                                               d.FullName.Replace(
                                                                                                   dir_original.FullName,
                                                                                                   "");
                                                                                 if (!Directory.Exists(dest))
                                                                                 {
                                                                                     Directory.CreateDirectory(dest);
                                                                                     // copy files in each directory
                                                                                     ShallowCopyFilesInDirectory(
                                                                                         d.FullName, dest);
                                                                                 }
                                                                             }
                                                                             //Uri prevUri = mSession.Presentation.RootUri;
                                                                             //mSession.Presentation.setRootUri ( new Uri ( path_new ) );

                                                                             Uri oldUri = mSession.Presentation.RootUri;
                                                                             string oldDataDir =
                                                                                 mSession.Presentation.
                                                                                     DataProviderManager.
                                                                                     DataFileDirectory;

                                                                             string dirPath =
                                                                                 System.IO.Path.GetDirectoryName(
                                                                                     path_new);
                                                                             string prefix =
                                                                                 System.IO.Path.GetFileName(path_new);

                                                                             //TODO: it would be good for Obi to separate Data folder based on project file name,
                                                                             //TODO: otherwise collision of Data folder may happen if several project files are in same directory.
                                                                             //mSession.Presentation.DataProviderManager.SetDataFileDirectoryWithPrefix(prefix);
                                                                             mSession.Presentation.RootUri =
                                                                                 new Uri(
                                                                                     dirPath +
                                                                                     System.IO.Path.
                                                                                         DirectorySeparatorChar,
                                                                                     UriKind.Absolute);

                                                                             mSession.Save(path_new);

                                                                             //mSession.Presentation.setRootUri ( prevUri );

                                                                             mSession.Presentation.RootUri = oldUri;
                                                                             mSession.Presentation.DataProviderManager.
                                                                                 DataFileDirectory = oldDataDir;

                                                                             // delete the original copied project file ( .obi file ) from new directory in case the new project has different project file name
                                                                             if (Path.GetFileName(oldUri.LocalPath) !=
                                                                                 Path.GetFileName(path_new)
                                                                                 && File.Exists(path_new))
                                                                             {
                                                                                 string copiedProjectFilePath =
                                                                                     Path.Combine(dir_new.FullName,
                                                                                                  Path.GetFileName(
                                                                                                      oldUri.LocalPath));

                                                                                 if (File.Exists(copiedProjectFilePath))
                                                                                     File.Delete(copiedProjectFilePath);

                                                                                 if (
                                                                                     File.Exists(copiedProjectFilePath +
                                                                                                 ".lock"))
                                                                                     File.Delete(copiedProjectFilePath +
                                                                                                 ".lock");
                                                                             }
                                                                         }, mSettings);  //@fontconfig
                        progress.ShowDialog();
                        if (progress.Exception != null) throw progress.Exception;
                        this.Cursor = Cursors.WaitCursor;
                        if (dialog.SwitchToNewProject) CloseAndOpenProject(path_new);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format(Localizer.Message("save_as_failed"), path_new, e.Message),
                                        Localizer.Message("save_as_failed_caption"),
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    this.Cursor = Cursors.Default;
                    m_IsSaveActive = false;
                }
                else
                {
                    Ready();
                }
            }

            // Copy files from one directory to another.
            // May throw exception when things go wrong.
            private void ShallowCopyFilesInDirectory(string source, string dest)
            {
                string[] FilesList = Directory.GetFiles(source, "*.*", SearchOption.TopDirectoryOnly);
                FileInfo FInfo = null;
                foreach (string f in FilesList)
                {
                    FInfo = new FileInfo(f);
                    FInfo.CopyTo(Path.Combine(dest, FInfo.Name));
                }
            }

            // Return whether the project can be closed or not.
            // If a project is open and unsaved, ask about what to do.
            private bool DidCloseProject()
            {

                if (mProjectView.IsAudioProcessingPerformed)
                {
                    if (MessageBox.Show(Localizer.Message("CleanUpAfterAudioProcessing"), Localizer.Message("Caption_Information"),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        CleanProject(false);
                    }
                    mProjectView.IsAudioProcessingPerformed = false;
                }

                if (!(FreezeChangesFromProjectRestore() ?? true)) return false;
                if (!SaveProjectAndBookmarkOptionally()) return false;

                /// if Monitor continuously is active, it should be surely disabled, no matter if transportbar is active or not
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false;//@Monitorcontinuously
                if (mProjectView.TransportBar.IsActive)
                {
                    mProjectView.TransportBar.Stop();
                    
                }

                /*  if (!mSession.CanClose)
                 {
                      if (m_ShouldBookmark)
                          mSession.ForceSave();
                      else
                      {
                          DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                              Localizer.Message("closed_project_caption"),
                              MessageBoxButtons.YesNoCancel,
                              MessageBoxIcon.Question);
                          if (result == DialogResult.Cancel) return false;
                          if (result == DialogResult.Yes) mSession.Save();
                      }
                      
                  }*/


                mSession.Close();
                return true;
            }

            /// <summary>
            /// presents multiple option dialog box for providing various options for saving project and bookmark.
            /// returns false if process is cancelled by user
            /// </summary>
            /// <returns></returns>
            private bool SaveProjectAndBookmarkOptionally()
            {
                //if (mProjectView.Presentation != null && mProjectView.Selection != null && !((((ObiRootNode)mProjectView.Presentation.RootNode).BookmarkNode) == mProjectView.Selection.Node))
                if (mProjectView.Presentation != null && mProjectView.Selection != null)
                {
                    if (!mSession.CanClose)
                    {
                        /*DialogResult resultBookmark = MessageBox.Show(Localizer.Message("SaveSelectedNodeAsBookmark"), Localizer.Message("bookmark_closed_project_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resultBookmark == DialogResult.Yes)
                    {
                        CheckForSelectedNodeInBookmark();
                        m_ShouldBookmark = true;
                    }
                    if (resultBookmark == DialogResult.No)
                        m_ShouldBookmark = false;
                     */
                        if (mProjectView.TransportBar.IsActive)
                            mProjectView.TransportBar.Stop();
                        Dialogs.MultipleOptionDialog resultBookmark =
                            new MultipleOptionDialog(
                                !((((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode) ==
                                  mProjectView.Selection.Node), !mSession.CanClose, mSettings);  //@fontconfig
                        resultBookmark.ShowDialog();
                        if (resultBookmark.DialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                        if (resultBookmark.IsSaveBothChecked)
                        {
                            CheckForSelectedNodeInBookmark();
                            mSession.ForceSave();
                        }
                        else if (resultBookmark.IsSaveProjectChecked)
                        {
                            mSession.Save();
                        }
                        else if (resultBookmark.IsDiscardBothChecked)
                        {
                        }
                    }
                    else
                        CheckForSelectedNodeInBookmark();
                }
                return true;
            }

            private const string m_CleanUpDeleteDirectoryName = "__DELETED";
            private const string m_CleanUpFileNamesMapFile = "CleanupRollBackFilesMap.txt";

            // Clean unwanted audio from the project.
            // Before continuing, the user is given the choice to save or cancel.
            private void CleanProject(bool skipCleanedUpDataProvider)
            {
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                if (!string.IsNullOrEmpty(mSession.Path) && mSession.Path.Length > 220
                    && MessageBox.Show(Localizer.Message("CleanUp_LongFilePath"), Localizer.Message("Caption_Error"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }
                //if (mProjectView.IsWaveformRendering
                //&& MessageBox.Show(Localizer.Message ("Cleanup_WaveformLoadingWarning"), 
                //Localizer.Message("Caption_Warning") , 
                //MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No)
                //{
                //return;
                //}
                mProjectView.WaveformRendering_PauseOrResume(true);

                //return;
                DialogResult result = MessageBox.Show(Localizer.Message("clean_save_text"),
                                                      Localizer.Message("clean_save_caption"),
                                                      MessageBoxButtons.OKCancel,
                                                      MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    StreamWriter writer = null;
                    bool isDeleteFolderOpenned = false ;

                    try
                    {
                        mSession.Save () ;

                        string dataFolderPath = mSession.Presentation.DataProviderManager.DataFileDirectoryFullPath;

                        string deletedDataFolderPath = Path.Combine(dataFolderPath,
                                                                    m_CleanUpDeleteDirectoryName + Path.DirectorySeparatorChar);
                        if (Directory.Exists(deletedDataFolderPath))
                        {
                            Directory.Delete(deletedDataFolderPath, true);
                        }
                        Directory.CreateDirectory(deletedDataFolderPath);

                        // copy obi project file also
                        string destinationProjectPath = Path.Combine(deletedDataFolderPath, Path.GetFileName(mSession.Path));
                        Console.WriteLine("Destination path of Obi project for backup : " + destinationProjectPath);
                        if (File.Exists(destinationProjectPath)) File.Delete(destinationProjectPath);
                        
                        File.Copy(mSession.Path,
                        destinationProjectPath,
                        true);
                        // create text file for cleanup rol back

                        mSession.Presentation.UndoRedoManager.FlushCommands();
                        mProjectView.Clipboard = null;
                        string CleanupRollBackFilesmapPath = Path.Combine(deletedDataFolderPath, m_CleanUpFileNamesMapFile);
                        if (File.Exists(CleanupRollBackFilesmapPath)) File.Delete(CleanupRollBackFilesmapPath);
                        File.CreateText(CleanupRollBackFilesmapPath).Close();

                        Cleaner cleaner = new Cleaner(mSession.Presentation, deletedDataFolderPath, mSettings.Audio_CleanupMaxFileSizeInMB, skipCleanedUpDataProvider);
                        Dialogs.ProgressDialog progress = new ProgressDialog(Localizer.Message("cleaning_up"),
                                                                             delegate()
                                                                             {
                                                                               m_CanAutoSave = false;
                                                                             //mSession.Presentation.cleanup();
                                                                                                                                                                                                                       
                                                                              cleaner.Cleanup();

                                                                              // save cleanup mapping for roll back
                                                                                 writer = new StreamWriter(File.OpenWrite(CleanupRollBackFilesmapPath));
                                                                                 foreach (Cleaner.OriginalRenamedFilenameTuple tup in cleaner.GetListOfRenamedFiles())
                                                                                 {
                                                                                     string line = tup.m_original + "=" + tup.m_renamed;
                                                                                     writer.WriteLine(line);
                                                                                 }
                                                                                 writer.Close();
                                                                                 writer = null;

                                                                                 List<string> listOfDataProviderFiles = new List<string>();
                                                                                 foreach (
                                                                                     DataProvider dataProvider in
                                                                                         mSession.Presentation.
                                                                                             DataProviderManager.
                                                                                             ManagedObjects.
                                                                                             ContentsAs_Enumerable)
                                                                                 {
                                                                                     FileDataProvider fileDataProvider =dataProvider as FileDataProvider;
                                                                                     if (fileDataProvider == null)
                                                                                         continue;

                                                                                     listOfDataProviderFiles.Add(Path.GetFileName(fileDataProvider.DataFileRelativePath));
                                                                                 }
                                                                                                                                                                

                                                                                 //foreach (urakawa.media.data.MediaData m in mProjectView.Presentation.MediaDataManager.ManagedObjects.ContentsAs_Enumerable)
                                                                                 //{
                                                                                 //if ( listOfDataProviderFiles.Contains ( m.UsedDataProviders.l

                                                                                 //}

                                                                                 bool folderIsShowing = false;
                                                                                 if (Directory.GetFiles(deletedDataFolderPath).Length != 0)
                                                                                 {
                                                                                     folderIsShowing = true;
                                                                                     //m_ShellView.ExecuteShellProcess(deletedDataFolderPath);
                                                                                 }

                                                                                 foreach (string filePath in Directory.GetFiles(dataFolderPath))
                                                                                 {
                                                                                     string fileName = Path.GetFileName(filePath);
                                                                                     if (!listOfDataProviderFiles.Contains(fileName))
                                                                                     {
                                                                                         string filePathDest =Path.Combine(deletedDataFolderPath,fileName);
                                                                                         Debug.Assert(
                                                                                             !File.Exists(
                                                                                                 filePathDest));
                                                                                         if (!File.Exists(filePathDest))
                                                                                         {
                                                                                             File.Move(filePath,filePathDest);
                                                                                             Console.WriteLine(filePath);
                                                                                         }
                                                                                     }
                                                                                 }

                                                                                 
                                                                                 if (
                                                                                     Directory.GetFiles(deletedDataFolderPath).Length != 0)
                                                                                 {
                                                                                     if (mSettings.Project_DisableRollBackForCleanUp 
                                                                                         ||
                                                                                         (MessageBox.Show(Localizer.Message("clean_up_ask_for_delete_project"),
                                                                                             Localizer.Message("Delete_unused_data_caption"),
                                                                                             MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) ==DialogResult.Yes
                                                                                         &&
                                                                                         MessageBox.Show(Localizer.Message("CleanUp_AreYouSure"),
                                                                                             Localizer.Message("Caption_Warning"),
                                                                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes))
                                                                                     {

                                                                                         
                                                                                             if(true)
                                                                                         //delete definitively
                                                                                         {
                                                                                             ProjectView.ProjectView.WriteToLogFile_Static("Clean up operation: deleting files");
                                                                                             foreach (string filePath in Directory.GetFiles(deletedDataFolderPath))
                                                                                             {
                                                                                                 File.Delete(filePath);
                                                                                             }
                                                                                         }

                                                                                         if (Directory.Exists(deletedDataFolderPath))
                                                                                         {
                                                                                             Directory.Delete(deletedDataFolderPath);
                                                                                         }
                                                                                     }
                                                                                     else // show the delete folder
                                                                                     {
                                                                                         if(MessageBox.Show(Localizer.Message("Cleanup_Complete_ShowDeleteDirectory"),
                                                                                             Localizer.Message("Caption_Information"),
                                                                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                                                                         {
                                                                                             System.Diagnostics.Process.
                                                                                             Start("explorer.exe",deletedDataFolderPath);

                                                                                         }
                                                                                         isDeleteFolderOpenned = true;
                                                                                     }
                                                                                 }
                                                                                                                                                               

                                                       }, mSettings); //@fontconfig
                        if (cleaner != null)
                            cleaner.ProgressChangedEvent +=new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                        progress.ShowDialog();
                        m_CanAutoSave = true;
                        if (progress.Exception != null) throw progress.Exception;

                        if (!m_IsAutoSaveActive)
                        {
                            if (!mSession.CanSave) mSession.PresentationHasChanged(1);
                            DeleteExtraBackupFiles(true);
                            m_IsAutoSaveActive = true;
                            SaveToBackup();
                            m_IsAutoSaveActive = false;
                        }
                        mSession.ForceSave();
                        if (!isDeleteFolderOpenned)
                        {
                            MessageBox.Show(Localizer.Message("CleanUp_Complete"),
                            Localizer.Message("Caption_Information"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("clean_failed_text"), e.Message),
                                        Localizer.Message("clean_failed_caption"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        m_CanAutoSave = true;
                    }
                    finally
                    {
                        if (writer != null) writer.Close();
                    }
                    mProjectView.WaveformRendering_PauseOrResume(false);
                }
            }

            public void CleanUpRollBack()
            {
                if (!Directory.Exists(mSession.Presentation.DataProviderManager.DataFileDirectoryFullPath))
                {       
                    MessageBox.Show(Localizer.Message("Rollback_DataDirectory_Missing"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (mSettings.Project_DisableRollBackForCleanUp)
                    {
                        MessageBox.Show(Localizer.Message("Rollback_Disabled"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return ;
                    }

                string deleteDirectoryPath = Path.Combine(mSession.Presentation.DataProviderManager.DataFileDirectoryFullPath,
                    m_CleanUpDeleteDirectoryName);
                Console.WriteLine("delete directory " + deleteDirectoryPath);
                if (!Directory.Exists(deleteDirectoryPath))
                {
                  MessageBox.Show(Localizer.Message("Rollback_No_DataFound"), Localizer.Message("Caption_Information"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string MappingFilePath = Path.Combine(deleteDirectoryPath, m_CleanUpFileNamesMapFile);
                Console.WriteLine("Mapping file path: " + MappingFilePath);
                if (!File.Exists(MappingFilePath)) 
                {
                    MessageBox.Show(Localizer.Message("Rollback_MappingFile_Missing"),
                        Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string[] fileNames = Directory.GetFiles(deleteDirectoryPath, "*.obi");
                if (fileNames.Length == 0) 
                {
                    MessageBox.Show(Localizer.Message("Rollback_ProjectFile_Missing"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                if (MessageBox.Show(Localizer.Message("CleanupRollBack_AskToProceed"),
                                                                                             Localizer.Message("Caption_Information"),
                                                                                             MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }

                string ProjectPathInDeleteDirectory = fileNames[0];
                Console.WriteLine("cleande up project path: " + ProjectPathInDeleteDirectory) ;

                string currentProjectPath = mSession.Path;
                string currentDataDirectoryPath = mSession.Presentation.DataProviderManager.DataFileDirectoryFullPath;
                DidCloseProject();
                // start roll back
                StreamReader reader = null;
                try
                {
                    // create the filenames mapping dictionary
                    reader = File.OpenText(MappingFilePath);
                    Dictionary<string, string> fileMappingOriginalToRenamed = new Dictionary<string, string>();
                    string line = "";
                    Console.WriteLine("populating cleanup mapping dictionary");
                    while (line != null)
                    {
                        line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] lineArray = line.Split('=');
                            fileMappingOriginalToRenamed.Add(lineArray[0], lineArray[1]);
                            Console.WriteLine(lineArray[0]);
                            Console.WriteLine(lineArray[1]);
                            Console.WriteLine("");
                        }
                    }
                    reader.Close();
                    reader = null;

                    // rename the project file name
                    string backupProjectFilePath = currentProjectPath + "_Before_Rollback";
                    if (File.Exists(backupProjectFilePath)) File.Delete(backupProjectFilePath);

                    File.Move(currentProjectPath, backupProjectFilePath);
                    Console.WriteLine("Project file renamed");
                    // move the cleaned up project file from delete folder to project directory.
                    if (File.Exists(currentProjectPath)) File.Delete(currentProjectPath);
                    Console.WriteLine("Old project file path: " + ProjectPathInDeleteDirectory);
                    Console.WriteLine("Destination location : " + currentProjectPath);
                    File.Move(ProjectPathInDeleteDirectory, currentProjectPath);
                    Console.WriteLine("Project before clean up is restored");


                    Dialogs.ProgressDialog progress = new ProgressDialog(Localizer.Message("CleanUp_Rollback_Progress"),
                                                                             delegate()
                                                                             {
                                                                                 // move the files from delete directory to data directory.
                                                                                 Console.WriteLine("Moving back the deleted files");
                                                                                 string[] deletedFileNames = Directory.GetFiles(deleteDirectoryPath, "*.*");
                                                                                 for (int i = 0; i < deletedFileNames.Length; i++)
                                                                                 {
                                                                                     string sourcePath = deletedFileNames[i];
                                                                                     string destinationPath = Path.Combine(currentDataDirectoryPath,
                                                                                         Path.GetFileName(deletedFileNames[i]));
                                                                                     Console.WriteLine("Deleted file name : " + deletedFileNames[i]);
                                                                                     // take precaution for the mapping text file because an old file with same name can exist..
                                                                                     if (Path.GetFileName( deletedFileNames[i]) == m_CleanUpFileNamesMapFile
                                                                                         && File.Exists(destinationPath))
                                                                                     {
                                                                                         File.Delete(destinationPath);
                                                                                     }
                                                                                     Console.WriteLine("Restored file : " + destinationPath);
                                                                                     File.Move(sourcePath, destinationPath);

                                                                                 }

                                                                                 // now rename the files to the original names.
                                                                                 Console.WriteLine("Renaming files in data directory to original namespace");
                                                                                 foreach (string originalName in fileMappingOriginalToRenamed.Keys)
                                                                                 {
                                                                                     string sourcePath = Path.Combine(currentDataDirectoryPath,
                                                                                         fileMappingOriginalToRenamed[originalName]);
                                                                                     string destinationPath = Path.Combine(currentDataDirectoryPath,
                                                                                         originalName);
                                                                                     File.Move(sourcePath, destinationPath);
                                                                                     Console.WriteLine(sourcePath);
                                                                                     Console.WriteLine(destinationPath);
                                                                                     Console.WriteLine("");
                                                                                 }
                                                                             }, mSettings);
                    progress.ShowDialog();
                    if (progress.Exception != null) throw progress.Exception;

                    MessageBox.Show(Localizer.Message("Rollback_Complete"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(String.Format(Localizer.Message("Rollback_Failed") + ex.ToString()), Localizer.Message("Caption_Rollback_Failed"), MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
                OpenProject(currentProjectPath, "");
            }


            //sdk2
            // Delete extra files in the data directory (or directories)
            //private void DeleteExtraFiles ()
            //  {
            //Dictionary<string, Dictionary<string, bool>> dirs = new Dictionary<string, Dictionary<string, bool>> ();
            //// Get the list of files used by the asset manager
            //foreach (urakawa.media.data.FileDataProvider provider in
            //        ((urakawa.media.data.FileDataProviderManager)mSession.Presentation.getDataProviderManager ()).
            //    getListOfManagedFileDataProviders ())
            //    {
            //    string path = provider.getDataFileFullPath ();
            //    string dir = Path.GetDirectoryName ( path );
            //    if (!dirs.ContainsKey ( dir )) dirs.Add ( dir, new Dictionary<string, bool> () );
            //    dirs[dir].Add (Path.GetFullPath( path), true );
            //    }
            //// Go through each directory and remove files not used by the data manager
            //// TODO at the moment, this removes everything; if we have other files that we need
            //// (e.g. images of waveforms?) we need to be careful not to throw them away.
            //foreach (string dir in dirs.Keys)
            //    {
            //    System.Diagnostics.Debug.Print ( "--- Cleaning up in {0}", dir );
            //    foreach (string path in Directory.GetFiles ( dir ))
            //        {
            //        if (dirs[dir].ContainsKey (Path.GetFullPath( path )))
            //            {
            //            System.Diagnostics.Debug.Print ( "=== Keeping {0}", path );
            //            }
            //        else
            //            {
            //            System.Diagnostics.Debug.Print ( "--- Deleting {0}", path );
            //            File.Delete ( path );
            //            }
            //        }
            //    }
            //}

            #endregion

            #region Help menu

            // Help > Contents (F1)
            private void mHelp_ContentsMenuItem_Click(object sender, EventArgs e)
            {
                ShowHelpFile();
            }

            // View the help file in our own browser window.
            private void ShowHelpFile(bool IsEnglishVersionSelected = false)
            {
                try
                {
                    if (!IsEnglishVersionSelected)
                        System.Diagnostics.Process.Start("explorer.exe",
                            (new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                                                  Localizer.Message("CHMhelp_file_name")))).ToString());
                    else
                        System.Diagnostics.Process.Start("explorer.exe",
                        (new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                                              "Obi Help.chm"))).ToString());
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(Localizer.Message("ObiFormMsg_FileLoadingFail") + "\n\n" +
                                                         ex.ToString()); //@Messagecorrected
                    return;
                }
            }

            // Help > View help in external browser (Shift+F1)
            private void mHelp_ViewHelpInExternalBrowserMenuItem_Click(object sender, EventArgs e)
            {
                System.Diagnostics.Process.Start(
                    (new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                                          Localizer.Message("help_file_name")))).ToString());
            }

            // Help > Report bug (Ctrl+Alt+R)
            private void mHelp_ReportBugMenuItem_Click(object sender, EventArgs e)
            {
                Uri url = new Uri(Localizer.Message("Obi_Wiki_Url"));
                System.Diagnostics.Process.Start("explorer.exe",url.ToString());
            }

            // Help > About
            private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause();

                (new Dialogs.About(mSettings)).ShowDialog(); //@fontconfig
            }

            #endregion


            #region Event handlers

            // Initialize event handlers from the project view
            private void InitializeEventHandlers()
            {
                mProjectView.TransportBar.StateChanged +=
                    new AudioLib.AudioPlayer.StateChangedHandler(TransportBar_StateChanged);
                mProjectView.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
                mProjectView.TransportBar.Recorder.StateChanged +=
                    new AudioLib.AudioRecorder.StateChangedHandler(TransportBar_StateChanged);
                mProjectView.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(Progress_Changed);
            }

            private void ObiForm_commandDone(object sender, urakawa.events.undo.DoneEventArgs e)
            {
                CanAutoSave = true; //@singleSection
                ProjectHasChanged(1);
                if (!IsStatusBarEnabled) IsStatusBarEnabled = true; //@singleSection

                ShowLimitedPhrasesShownStatusMessage();
            }

            private void ObiForm_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e)
            {
                CanAutoSave = true; //@singleSection
                ProjectHasChanged(-1);
                if (!IsStatusBarEnabled) IsStatusBarEnabled = true; //@singleSection

                ShowLimitedPhrasesShownStatusMessage();
            }

            private void ObiForm_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e)
            {
                CanAutoSave = true; //@singleSection
                ProjectHasChanged(1);
                if (!IsStatusBarEnabled) IsStatusBarEnabled = true; //@singleSection

                ShowLimitedPhrasesShownStatusMessage();
            }

            private void ObiForm_BeforeCommandExecuted(object sender, urakawa.events.command.CommandEventArgs e)
                //@singleSection
            {
                CanAutoSave = false; //@singleSection
                if (e.SourceCommand is urakawa.command.CompositeCommand)
                {
                    if (!string.IsNullOrEmpty(e.SourceCommand.ShortDescription))
                    {
                        Status(string.Format(Localizer.Message("StatusBar_CommandExecuting"),
                                             e.SourceCommand.ShortDescription));
                    }
                    IsStatusBarEnabled = false;
                }
            }

            private delegate void ShowLimitedPhrasesShownStatusMessage_Delegate();

            private void ShowLimitedPhrasesShownStatusMessage_Safe()
            {
                if (InvokeRequired)
                {
                    Invoke(new ShowLimitedPhrasesShownStatusMessage_Delegate(ShowLimitedPhrasesShownStatusMessage_Safe));
                }
                else
                {
                    ShowLimitedPhrasesShownStatusMessage();
                }
            }

            private void ShowLimitedPhrasesShownStatusMessage()
            {
                if (mProjectView.IsLimitedPhraseBlocksCreatedAfterCommand())
                {
                    string selectionString = mProjectView.Selection != null
                                                 ? mProjectView.Selection.Node.ToString()
                                                 : "";
                    Status(string.Format(Localizer.Message("StatusBar_LimitedPhrasesShown"), selectionString));
                }
            }

            // Show welcome dialog first, unless the user has chosen
            private void ObiForm_Load(object sender, EventArgs e)
            {
                if (!m_InputDeviceFound && !m_OutputDevicefound) this.Close();
                CheckSystemSupportForMemoryOptimization();
                UploadUsersInfo(); //can be commentted for test releases
                UploadObiConfigurations();
                if (ShouldOpenLastProject) OpenProject_Safe(mSettings.LastOpenProject, null);
                if (!ShouldOpenLastProject && mShowWelcomWindow) ShowWelcomeDialog();

                UpdateKeyboardFocusForSelection();
                if (mSettings.ShowGraphicalPeakMeterAtStartup)
                {
                    ShowPeakMeter();
                }
                else if (mSettings.ShowGraphicalPeakMeterInsideObiAtStartup)
                {
                    mShowPeakMeterInsideObiMenuItem.Checked = true;
                    mProjectView.ShowPeakMeterInsideObi(true);
                }
                if (mSettings.Project_RecordingToolbarOpenInPreviousSession) ShowRecordingToolBar();
                if (mSettings.Project_CheckForUpdates) CheckForNewRelease(true);
                if (mSettings.Project_MaximizeObi)
                {
                    this.WindowState = FormWindowState.Maximized;

                }
                if (mSettings != null && mProjectView != null && mSettings.Project_SaveTOCViewWidth && mSettings.TOCViewWidth != 0 && mSession.Presentation != null)
                {
                    mProjectView.TOCViewWidth = mSettings.TOCViewWidth;
                    this.FixTOCViewWidth = true;
                }
            }


            private void CheckSystemSupportForMemoryOptimization()
            {
                if (!mSettings.Project_OptimizeMemory) return;
                //System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();
                try
                {
                    if (!System.Diagnostics.PerformanceCounterCategory.CounterExists("Available MBytes", "Memory"))
                    {
                        
                        mSettings.Project_OptimizeMemory = false;
                        return;
                    }
                    System.Diagnostics.PerformanceCounter ramPerformanceCounter =
                        new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
                    ramPerformanceCounter.NextValue();
                    ramPerformanceCounter.Close();
                    System.GC.GetTotalMemory(true);
                }
                catch (System.Exception)
                {   
                    mSettings.Project_OptimizeMemory = false;

                }
                //stopWatch.Stop();
                //Console.WriteLine("stop watch performance counter: " + stopWatch.ElapsedMilliseconds);
            }

            private void UploadUsersInfo()
            {
                //if (m_Settings_Permanent.UsersInfoToUpload != Dialogs.UserRegistration.Registered && m_Settings_Permanent.UploadAttemptsCount <= Dialogs.UserRegistration.MaxUploadAttemptsAllowed)
                if (!m_Settings_Permanent.RegistrationComplete && m_Settings_Permanent.UploadAttemptsCount <= Dialogs.UserRegistration.MaxUploadAttemptsAllowed)
                {
                    //Console.WriteLine(mSettings.UsersInfoToUpload);
                    if (string.IsNullOrEmpty(m_Settings_Permanent.UsersInfoToUpload) || m_Settings_Permanent.UsersInfoToUpload == Dialogs.UserRegistration.NoInfo)
                    {
                        Dialogs.UserRegistration registrationDialog = new UserRegistration(m_Settings_Permanent, mSettings); //@fontconfig
                        registrationDialog.ShowDialog();
                    }
                    //Console.WriteLine("bypassed dialog");
                    if (!string.IsNullOrEmpty(m_Settings_Permanent.UsersInfoToUpload) && m_Settings_Permanent.UsersInfoToUpload != Dialogs.UserRegistration.NoInfo && !m_Settings_Permanent.RegistrationComplete)
                    {
                        Console.WriteLine("Upload attempts: " + m_Settings_Permanent.UploadAttemptsCount);
                        // if attempts are less than max allowed attempts then try uploading 
                        // but if attempts count are equal to maximum attempts allowed then send email
                        if (m_Settings_Permanent.UploadAttemptsCount < Dialogs.UserRegistration.MaxUploadAttemptsAllowed)
                        {
                            Console.WriteLine("uploading");
                            Dialogs.UserRegistration.UploadUserInformation(m_Settings_Permanent);
                        }
                        else if (MessageBox.Show(string.Format(Localizer.Message("UserRegistration_SendEmailMsg"), m_Settings_Permanent.UploadAttemptsCount),
                             Localizer.Message("Caption_Information"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            Dialogs.UserRegistration.OpenEmailToSend(m_Settings_Permanent);
                        }
                    }
                }
            }


            private void UploadObiConfigurations()
            {
                if (!mSettings.IsObiConfigurationDone)
                {
                    Dialogs.ObiConfiguration
                        obiConfigDialog = new ObiConfiguration(this, mProjectView, mSettings); //@fontconfig
                    obiConfigDialog.ShowDialog();
                }

            }



            // workaround to move keyboard focus to selection 
            //as it do not synchronize with selection when selection is assigned while loading of obi form.
            private void UpdateKeyboardFocusForSelection()
            {
                if (mProjectView != null && mProjectView.Selection != null)
                {
                    System.ComponentModel.BackgroundWorker moveFocusToSelectionBkWorker =
                        new System.ComponentModel.BackgroundWorker();
                    moveFocusToSelectionBkWorker.RunWorkerCompleted +=
                        new System.ComponentModel.RunWorkerCompletedEventHandler(
                            moveFocusToSelectionBkWorker_RunWorkerCompleted);

                    moveFocusToSelectionBkWorker.RunWorkerAsync();
                }
            }

            public void moveFocusToSelectionBkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (InvokeRequired)
                {
                    Invoke(new RunWorkerCompletedEventHandler(moveFocusToSelectionBkWorker_RunWorkerCompleted), sender,
                           e);
                }
                else
                {
                    bool statusOfSelectionChangedPlaybackEnabled =
                        mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                    mProjectView.Selection = new NodeSelection(mProjectView.Selection.Node,
                                                               mProjectView.Selection.Control);
                    mProjectView.TransportBar.SelectionChangedPlaybackEnabled = statusOfSelectionChangedPlaybackEnabled;
                }
            }

            // Show the welcome dialog
            private void ShowWelcomeDialog()
            {
                Dialogs.WelcomeDialog ObiWelcome = new WelcomeDialog(mSettings.LastOpenProject != "", mSettings); //@fontconfig
                ObiWelcome.ShowDialog();
                switch (ObiWelcome.Result)
                {
                    case WelcomeDialog.Option.NewProject:
                        NewProject();
                        break;
                    case WelcomeDialog.Option.NewProjectFromImport:
                        NewProjectFromImport();
                        break;
                    case WelcomeDialog.Option.OpenProject:
                        Open();
                        break;
                    case WelcomeDialog.Option.OpenLastProject:
                        OpenProject_Safe(mSettings.LastOpenProject, null);
                        break;
                    case WelcomeDialog.Option.ViewHelp:
                        ShowHelpFile();
                        break;
                }
            }

            // Remember the form size in the settings.
            private void ObiForm_ResizeEnd(object sender, EventArgs e)
            {
                if ((mPeakMeter != null && !m_ShowingPeakMeter)|| mSettings.Project_SaveObiLocationAndSize)
                {
                    mSettings.ObiFormSize = Size;                    
                }
                if (mSettings.Project_PeakMeterChangeLocation && mPeakMeter != null)
                {
                    mPeakMeter.Top = this.Top;
                    mPeakMeter.Left = this.Right;
                }
                m_ShowingPeakMeter = false;
            }

            private void ProjectView_SelectionChanged(object sender, EventArgs e)
            {
                UpdateMenus();
                ShowSelectionInStatusBar();
            }

            private void ProjectView_BlocksVisibilityChanged(object sender, EventArgs e)
            {
                if (mProjectView.IsContentViewScrollActive)
                {
                    Status(Localizer.Message("Scroll_LoadingScreen"));
                }
                else if (Localizer.Message("Scroll_LoadingScreen") == mStatusLabel.Text)
                {
                    //Status ("" );
                    ShowSelectionInStatusBar();
                }
            }

            private void Session_ProjectCreated(object sender, EventArgs e)
            {
                GotNewPresentation();

            }

            private void Session_ProjectClosed(object sender, ProjectClosedEventArgs e)
            {
                if (e.ClosedPresentation != null && ((ObiPresentation) e.ClosedPresentation).Initialized
                    && mProjectView.Presentation != null)
                {
                    foreach (string customClass in mProjectView.Presentation.CustomClasses)
                        RemoveCustomClassFromMenu(customClass, true);
                    mProjectView.Presentation.Changed -=
                        new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    mProjectView.Presentation.BeforeCommandExecuted -=
                        new EventHandler<urakawa.events.command.CommandEventArgs>(ObiForm_BeforeCommandExecuted);
                        //@singleSection
                    Status(String.Format(Localizer.Message("closed_project"),
                                         ((ObiPresentation) e.ClosedPresentation).Title));
                }
                mAutoSaveTimer.Stop();
                mProjectView.Selection = null;
                mProjectView.Presentation = null;
                UpdateObi();
                if (mRecordingToolBarForm != null) mRecordingToolBarForm.UpdateForChangeInObi();
                if (mSourceView != null) mSourceView.Close();
            }

            private void Session_ProjectOpened(object sender, EventArgs e)
            {
                GotNewPresentation();
                //Status ( String.Format ( Localizer.Message ( "opened_project" ), mSession.Presentation.Title ) );
                Status_Safe(String.Format(Localizer.Message("opened_project"), mSession.Presentation.Title));
            }


            private void Session_ProjectSaved(object sender, EventArgs e)
            {
                UpdateObi();
                AddRecentProject(mSession.Path);
                Status(String.Format(Localizer.Message("saved_project"), mSession.Path));
            }

            // Add a project to the list of recent projects.
            // If the project was already in the list, promote it to the top of the list.
            private void AddRecentProject(string path)
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    // the item was in the list so bump it up
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    mFile_RecentProjectMenuItem.DropDownItems.RemoveAt(i);
                }
                AddRecentProjectsItem(path);
                mSettings.RecentProjects.Insert(0, path);
                mSettings.LastOpenProject = path;
            }

            // Add an item in the recent projects list.
            private bool AddRecentProjectsItem(string path)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = path;
                item.Click +=
                    new System.EventHandler(delegate(object sender, EventArgs e) { CloseAndOpenProject(path); });
                mFile_RecentProjectMenuItem.DropDownItems.Insert(0, item);
                return true;
            }

            #endregion







            #region Edit menu

            /// <summary>
            /// Explicitly update the find in text menu items
            /// TODO: this should be handled by an event.
            /// </summary>
            public void UpdateFindInTextMenuItems()
            {
                mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText && mProjectView.FindInTextVisible;
                mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText && mProjectView.FindInTextVisible;
            }

            private void UpdateHelpMenu()
            {
                checkForUpdatesToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive;
                aboutObiToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive;
            }
            // Update the edit menu
            private void UpdateEditMenu()
            {
                mUndoToolStripMenuItem.Enabled = mSession.CanUndo && !mProjectView.TransportBar.IsRecorderActive;
                mUndoToolStripMenuItem.Text = mSession.CanUndo && !mProjectView.TransportBar.IsRecorderActive
                                                  ? String.Format(Localizer.Message("undo_label"),
                                                                  Localizer.Message("undo"), mSession.UndoLabel)
                                                  : String.Format(Localizer.Message("undo_label"),
                                                                  Localizer.Message("undo"), "");
                    //avn: the "cannot" text create confusion in keyboardshortcut preferences
                //Localizer.Message ( "cannot_undo" );
                mUndoToolStripMenuItem.AccessibleName = mUndoToolStripMenuItem.Text.Replace("&", "");
                ;

                mRedoToolStripMenuItem.Enabled = mSession.CanRedo && !mProjectView.TransportBar.IsRecorderActive;
                mRedoToolStripMenuItem.Text = mSession.CanRedo && !mProjectView.TransportBar.IsRecorderActive
                                                  ? String.Format(Localizer.Message("redo_label"),
                                                                  Localizer.Message("redo"), mSession.RedoLabel)
                                                  : String.Format(Localizer.Message("redo_label"),
                                                                  Localizer.Message("redo"), "");
                    //avn: the "cannot" text create confusion in keyboardshortcut preferences
                //Localizer.Message ( "cannot_redo" );
                mRedoToolStripMenuItem.AccessibleName = mRedoToolStripMenuItem.Text.Replace("&", "");

                mCutToolStripMenuItem.Enabled = mProjectView.CanCut;
                mCopyToolStripMenuItem.Enabled = mProjectView.CanCopy;
                mPasteToolStripMenuItem.Enabled = mProjectView.CanPaste;
                mPasteBeforeToolStripMenuItem.Enabled = mProjectView.CanPasteBefore;
                mPasteInsideToolStripMenuItem.Enabled = mProjectView.CanPasteInside;
                mDeleteToolStripMenuItem.Enabled = mProjectView.CanDelete;
                mSelectNothingToolStripMenuItem.Enabled = mProjectView.CanDeselect;
                mEdit_DeleteUnusedDataMenuItem.Enabled = mSession.HasProject &&
                                                         !mProjectView.TransportBar.IsRecorderActive;
                mFindInTextToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.CanFindFirstTime &&
                                                       !mProjectView.TransportBar.IsRecorderActive;
                mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText && mProjectView.FindInTextVisible;
                mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText && mProjectView.FindInTextVisible;
                mEdit_BookmarkToolStripMenuItem.Enabled = mProjectView.Presentation != null &&
                                                          !mProjectView.TransportBar.IsRecorderActive;
                mEdit_AssignBookmarkToolStripMenuItem.Enabled = mProjectView.Selection != null;
                mEdit_GotoBookmarkToolStripMenuItem.Enabled = mProjectView.Presentation != null &&
                                                              ((ObiRootNode) mProjectView.Presentation.RootNode).
                                                                  BookmarkNode != null &&
                                                              ((ObiRootNode) mProjectView.Presentation.RootNode).
                                                                  BookmarkNode.IsRooted &&
                                                              !mProjectView.TransportBar.IsRecorderActive;
            }

            private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Undo();
            }

            private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Redo();
            }

            private void mCutToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.Cut();
            }

            private void mCopyToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.Copy();
            }

            private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.Paste();
            }

            private void mPasteBeforeToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.PasteBefore();
            }

            private void mPasteInsideToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.PasteInside();
            }

            private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.Delete();
            }

            private void mSelectNothingToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SelectNothing();
            }

            private void mEdit_DeleteUnusedDataMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();

                mProjectView.DeleteUnused();
            }

            private void mFindInTextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.FindInText();
            }

            #endregion

            #region View menu

            private void UpdateViewMenu()
            {
                mShowTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
                mShowMetadataViewToolStripMenuItem.Enabled = mSession.HasProject;
                mShowMetadataViewToolStripMenuItem.CheckedChanged -=
                    new System.EventHandler(mShowMetadataViewToolStripMenuItem_CheckedChanged);
                mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible;
                mShowMetadataViewToolStripMenuItem.CheckedChanged +=
                    new System.EventHandler(mShowMetadataViewToolStripMenuItem_CheckedChanged);
                mShowTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
                mShowStatusBarToolStripMenuItem.Enabled = true;
                mFocusOnTOCViewToolStripMenuItem.Enabled = (mProjectView.CanFocusOnTOCView ||
                                                            mProjectView.CanToggleFocusToContentsView) &&
                                                           mSession.HasProject;
                mFocusOnStripsViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnContentView &&
                                                              mProjectView.CanToggleFocusToContentsView &&
                                                              mSession.HasProject;
                mFocusOnTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
                mSynchronizeViewsToolStripMenuItem.Enabled = mSession.HasProject;
                // mWrappingInContentViewToolStripMenuItem.Enabled = mSession.HasProject;
                // mShowSectionContentsToolStripMenuItem.Enabled = mProjectView.CanShowSectionContents;
                // mShowSingleSectionToolStripItem.Enabled = mSession.HasProject && mProjectView.Selection != null;
                mShowPeakMeterMenuItem.Enabled = mSession.HasProject;
                mShowPeakMeterInsideObiMenuItem.Enabled = mSession.HasProject;
                mView_RecordingToolBarMenuItem.Enabled = mSession.HasProject;
                mShowSourceToolStripMenuItem.Enabled = mSession.HasProject;
                mView_PhrasePropertiesMenuItem.Visible =
                    mView_PhrasePropertiesMenuItem.Enabled = mProjectView.CanShowPhrasePropertiesDialog;
                mView_SectionPropertiesMenuItem.Visible =
                    mView_SectionPropertiesMenuItem.Enabled = mProjectView.CanShowSectionPropertiesDialog;
                mView_ProjectPropertiesMenuItem.Enabled = mProjectView.CanShowProjectPropertiesDialog;
                mView_ProjectPropertiesMenuItem.Visible =
                    mProjectView.CanShowProjectPropertiesDialog || !mSession.HasProject;
                mView_AudioZoomInMenuItem.Enabled = mSession.HasProject;
                mView_AudioZoomOutMenuItem.Enabled = mSession.HasProject;
                mView_NormalSizeMenuItem.Enabled = mSession.HasProject;
                mView_ZoomInMenuItem.Enabled = mSession.HasProject;
                mView_ZoomOutMenuItem.Enabled = mSession.HasProject;
                mView_ResetAudioSizeMenuItem.Enabled = mSession.HasProject;
            }

            private void mShowTOCViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mProjectView.TOCViewVisible = mShowTOCViewToolStripMenuItem.Checked;
            }

            private void mShowMetadataViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mProjectView.MetadataViewVisible = mShowMetadataViewToolStripMenuItem.Checked;
            }

            private void mShowTransportBarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mProjectView.TransportBarVisible = mShowTransportBarToolStripMenuItem.Checked;
            }

            private void mShowStatusBarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mStatusStrip.Visible = mShowStatusBarToolStripMenuItem.Checked;
            }

            private void mFocusOnTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView != null) mProjectView.ToggleFocusBTWTOCViewAndContentsView();
            }

            private void mFocusOnStripsViewToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView != null)
                    mProjectView.ToggleFocusBTWTOCViewAndContentsView();
            }

            private void mFocusOnTransportBarToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.Enabled) mProjectView.TransportBar.Focus();
            }

            private void mSynchronizeViewsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                SynchronizeViews = mSynchronizeViewsToolStripMenuItem.Checked;
            }

            // Check/uncheck "Wrapping in content view"
            /* private void mWrappingInContentViewToolStripMenuItem_CheckedChanged ( object sender, EventArgs e )
             {
             WrapStripContents = mWrappingInContentViewToolStripMenuItem.Checked;
             }
         */

            private void mShowSourceToolStripMenuItem_Click(object sender, EventArgs e)
            {
                ShowSource();
            }

            #endregion

            #region Sections menu

            private void UpdateSectionsMenu()
            {
                mAddSectionToolStripMenuItem.Enabled = mProjectView.CanAddSection && !mProjectView.TransportBar.IsListening && !this.Settings.Project_ReadOnlyMode;
                mAddSubsectionToolStripMenuItem.Enabled = mProjectView.CanAddSubsection && !mProjectView.TransportBar.IsListening && !this.Settings.Project_ReadOnlyMode;
                mInsertSectionToolStripMenuItem.Enabled = mProjectView.CanInsertSection && !this.Settings.Project_ReadOnlyMode;
                mRenameSectionToolStripMenuItem.Enabled = mProjectView.CanRenameSection && !this.Settings.Project_ReadOnlyMode;
                mDecreaseSectionLevelToolStripMenuItem.Enabled = mProjectView.CanDecreaseLevel && !this.Settings.Project_ReadOnlyMode;
                mIncreaseSectionLevelToolStripMenuItem.Enabled = mProjectView.CanIncreaseLevel && !this.Settings.Project_ReadOnlyMode;
                mSplitSectionToolStripMenuItem.Enabled = mProjectView.CanSplitStrip && !this.Settings.Project_ReadOnlyMode;
               // mMergeSectionWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeStripWithNext;
                mMergeWithNextSectionToolStripMenuItem.Enabled = mProjectView.EnableMultiSectionOperation && !this.Settings.Project_ReadOnlyMode;
                mMultiSectionOperations.Enabled = mProjectView.EnableMultiSectionOperation && !this.Settings.Project_ReadOnlyMode;
                mSectionIsUsedToolStripMenuItem.Enabled = mProjectView.CanSetSectionUsedStatus && !this.Settings.Project_ReadOnlyMode;
                mImportTOCMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive && !this.Settings.Project_ReadOnlyMode;
                m_ImportMetadataToolStripMenuItem.Enabled = mProjectView.CanAddMetadataEntry() && !this.Settings.Project_ReadOnlyMode;
                mTrimSilenceFromSectionEnd.Enabled = mDeleteSilenceFromEndOfSectionToolStripMenuItem.Enabled =
                    mRetainSilenceInLastPhraseOfSectionToolStripMenuItem.Enabled = mProjectView.CanDeleteSilenceFromEndOfSection && !this.Settings.Project_ReadOnlyMode;
                mSectionIsUsedToolStripMenuItem.CheckedChanged -=
                    new System.EventHandler(mSectionIsUsedToolStripMenuItem_CheckedChanged);
                mSectionIsUsedToolStripMenuItem.Checked = (mProjectView.CanMarkSectionUnused ||
                                                          mProjectView.CanMarkStripUnused) && !this.Settings.Project_ReadOnlyMode;
                mSectionIsUsedToolStripMenuItem.CheckedChanged +=
                    new System.EventHandler(mSectionIsUsedToolStripMenuItem_CheckedChanged);
            }

            private void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AddSection();
            }

            private void mAddSubsectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AddSubSection();
            }

            private void mInsertSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.InsertSection();
            }

            private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.StartRenamingSelectedSection();
            }

            private void mDecreaseSectionLevelToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.DecreaseSelectedSectionLevel();
            }

            private void mIncreaseSectionLevelToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.IncreaseSelectedSectionLevel();
            }

            private void mSplitSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SplitStrip();
            }

            private void mMergeSectionWithNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
// mProjectView.MergeStrips ();
            }

            private void mSectionIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mProjectView.SetSelectedNodeUsedStatus(mSectionIsUsedToolStripMenuItem.Checked);
            }

            #endregion

            #region Phrases menu

            // Update the status of the blocks menu item with the current selection and tree.
            private void UpdatePhrasesMenu()
            {
                mAddBlankPhraseToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock &&
                                                           !mProjectView.TransportBar.IsRecorderActive;
                mAddEmptyPagesToolStripMenuItem.Enabled = mProjectView.CanAddEmptyPage
                                                          && !mProjectView.TransportBar.IsRecorderActive;
                m_AutoFillMissingPagesMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mImportAudioFileToolStripMenuItem.Enabled = mProjectView.CanImportPhrases;
                mSplitPhraseToolStripMenuItem.Enabled = mProjectView.CanSplitPhrase &&
                                                        !mProjectView.TransportBar.IsRecorderActive;
                mMergeToolStripMenuItem.Enabled = mProjectView.Presentation != null && mProjectView.Selection != null &&
                                                  mProjectView.Selection.Node is EmptyNode &&
                                                  mProjectView.GetSelectedPhraseSection != null &&
                                                  mProjectView.GetSelectedPhraseSection.PhraseChildCount > 1 &&
                                                  !mProjectView.TransportBar.IsRecorderActive;
                mMergePhraseWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeBlockWithNext &&
                                                                !mProjectView.TransportBar.IsRecorderActive;
                mMergePhraseWithFollowingPhrasesToolStripMenuItem.Enabled =
                    mProjectView.CanMergePhraseWithFollowingPhrasesInSection;
                mMergePhraseWithPrecedingPhrasesToolStripMenuItem.Enabled =
                    mProjectView.CanMergeWithPhrasesBeforeInSection;
                mDeleteFollowingPhrasesToolStripMenuItem.Enabled = mProjectView.CanDeleteFollowingPhrasesInSection;
                mPhraseDetectionToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mApplyPhraseDetectionInProjectToolStripMenuItem.Enabled = mSession.HasProject &&
                                                                          mProjectView.
                                                                              CanApplyPhraseDetectionInWholeProject;
                mPhrases_AssignRole_PageMenuItem.Enabled = mProjectView.CanSetPageNumber;
                mPhrases_EditRolesMenuItem.Enabled = mSession.HasProject;
                mPhrases_ClearRoleMenuItem.Enabled = mProjectView.CanAssignPlainRole;
                mPhrases_ApplyPhraseDetectionMenuItem.Enabled = mSession.HasProject &&
                                                                mProjectView.CanApplyPhraseDetection;
                mCropAudiotoolStripMenuItem.Enabled = mProjectView.CanCropPhrase;
                mGoToToolStripMenuItem.Enabled = mSession.Presentation != null &&
                                                 !mProjectView.TransportBar.IsRecorderActive;
                mPhraseIsUsedToolStripMenuItem.Enabled = mProjectView.CanSetBlockUsedStatus;
                mPhraseIsUsedToolStripMenuItem.CheckedChanged -=
                    new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
                mPhraseIsUsedToolStripMenuItem.Checked = mProjectView.IsBlockUsed;
                mPhraseIsUsedToolStripMenuItem.CheckedChanged +=
                    new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
                mPhrases_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus;
                mPhrases_PhraseIsTODOMenuItem.CheckedChanged -=
                    new System.EventHandler(mPhrases_PhraseIsTODOMenuItem_CheckedChanged);
                mPhrases_PhraseIsTODOMenuItem.Checked = mProjectView.IsCurrentBlockTODO;
                mPhrases_PhraseIsTODOMenuItem.CheckedChanged +=
                    new System.EventHandler(mPhrases_PhraseIsTODOMenuItem_CheckedChanged);
                mPhrases_AssignRoleMenuItem.Enabled = mProjectView.CanAssignARole;
                mPhrases_AssignRole_PlainMenuItem.Enabled = mProjectView.CanAssignPlainRole;
                mPhrases_AssignRole_HeadingMenuItem.Enabled = mProjectView.CanAssignHeadingRole;
                mPhrases_AssignRole_PageMenuItem.Enabled = mProjectView.CanAssignARole;
                mPhrases_AssignRole_SilenceMenuItem.Enabled = mProjectView.CanAssignSilenceRole;
                mPhrases_AssignRole_NewCustomRoleMenuItem.Enabled = mProjectView.CanAssignARole;
                mPhrases_AssignRole_AnchorMenuItem.Enabled = mProjectView.CanAssignAnchorRole &&
                                                             !mProjectView.TransportBar.IsRecorderActive;
                    //@AssociateNode
                m_GoToPageToolStrip.Enabled = mSession.Presentation != null &&
                                              !mProjectView.TransportBar.IsRecorderActive;
                mSkippableNoteToolStripMenuItem.Enabled = mSession.Presentation != null && !mProjectView.IsZoomWaveformActive && !mProjectView.TransportBar.IsRecorderActive;
                mSkippableBeginNoteToolStripMenuItem.Enabled = mProjectView.CanBeginSpecialNote; //@AssociateNode
                mSkippableEndNoteToolStripMenuItem.Enabled = mProjectView.CanEndSpecialNote; //@AssociateNode
                mSkippableGotoToolStripMenuItem.Enabled = mProjectView.CanGotoSkippableNote; //@AssociateNode           
                //     mSkippableMoveToStartNoteToolStripMenuItem.Enabled = mProjectView.Selection != null && mProjectView.Selection.Node.IsRooted  && mProjectView.Selection.Node is EmptyNode && mProjectView.Selection.Node.Index > 0;
                mSkippableMoveToEndNoteToolStripMenuItem.Enabled = mProjectView.CanMoveToEndNote;
                mSkippableRemoveReferenceToolStripMenuItem.Enabled = mProjectView.CanRemoveSkippableNode;
                mSkippableMoveToStartNoteToolStripMenuItem.Enabled = mProjectView.CanMoveToStartNote;
                //   mSkippableMoveToEndNoteToolStripMenuItem.Enabled = mProjectView.Selection != null && mProjectView.Selection.Node is EmptyNode && ((EmptyNode)mProjectView.Selection.Node).Role_ == EmptyNode.Role.Custom;
                mSkippableAddReferenceToolStripMenuItem.Enabled = mProjectView.CanAssociateNode;
                mSkippableClearRoleFromNoteToolStripMenuItem.Enabled = mProjectView.CanClearSkippableRole;
                UpdateAudioSelectionBlockMenuItems();
                settingsFromSilencePhraseToolStripMenuItem.Enabled = mProjectView.CanUpdatePhraseDetectionSettingsFromSilencePhrase;
                mCheckForPhrasesWithImproperAudioMenuItem.Enabled = mProjectView.CanReplacePhrasesWithimproperAudioWithEmptyNodes;
                splitAndMergeWithNextToolStripMenuItem.Enabled = mProjectView.CanSplitPhrase;
                splitAndMergeWithPreviousToolStripMenuItem.Enabled = mProjectView.CanSplitPhrase;
                mPhrases_RenumberPagesMenuItem.Enabled = mProjectView.Presentation != null && !mProjectView.TransportBar.IsRecorderActive;
                beginMarkToolStripMenuItem.Enabled = mProjectView.CanBeginSpecialNote;
                endMarkToolStripMenuItem.Enabled = mProjectView.CanEndSpecialNote;
                mAutoPageGenerationMenuItem.Enabled = mProjectView.CanAddEmptyPage
                                                          && !mProjectView.TransportBar.IsRecorderActive;
                m_pasteMultiplePhrasesToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive &&  mProjectView.CanPasteMultiplePhrases;
                m_CommentToolStripMenuItem.Enabled = m_AddViewCommentToolStripMenuItem.Enabled = m_ClearCommentToolStripMenuItem.Enabled = mProjectView.IsBlockSelected
                                                                                                                                          ||mProjectView.TransportBar.IsPlayerActive ;                
            }

            private void UpdateAudioSelectionBlockMenuItems()
            {
                mPhrases_AudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
                mPhrases_AudioSelection_BeginAudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
                mPhrases_AudioSelection_EndAudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionEnd;
            }

            private void mAddBlankPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AddEmptyBlock();
            }

            private void mAddEmptyPagesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AddEmptyPages();
            }

            private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ImportPhrases();
            }

            private void mSplitPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SplitPhrase();
            }

            private void mMergePhraseWithNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergeBlockWithNext();
            }

            private void mPhrases_PhraseIsTODOMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                IsStatusBarEnabled = false;
                mProjectView.ToggleTODOForPhrase();
            }


            private void mPhraseIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                mProjectView.SetSelectedNodeUsedStatus(mPhraseIsUsedToolStripMenuItem.Checked);
            }


            // Update the custom class menu with the classes from the new project
            private void UpdateCustomClassMenu()
            {
                foreach (string customClass in mSession.Presentation.CustomClasses) AddCustomRoleToMenus(customClass);
                mSession.Presentation.CustomClassAddded += new CustomClassEventHandler(Presentation_CustomClassAddded);
                mSession.Presentation.CustomClassRemoved += new CustomClassEventHandler(Presentation_CustomClassRemoved);
            }

            // Update the custom class menu
            private void Presentation_CustomClassAddded(object sender, CustomClassEventArgs e)
            {
                AddCustomRoleToMenus(e.CustomClass);
            }

            private void AddCustomRoleToMenus(string name)
            {
                AddCustomRoleToMenu(name, mPhrases_AssignRoleMenuItem.DropDownItems,
                                    mPhrases_AssignRole_NewCustomRoleMenuItem);
                mProjectView.AddCustomRoleToContextMenu(name, this);
            }

            /// <summary>
            /// Add a new custom role menu item to a menu given a context item.
            /// </summary>
            public void AddCustomRoleToMenu(string name, ToolStripItemCollection items, ToolStripMenuItem contextItem)
            {
                int index = items.IndexOf(contextItem);
                int startIndex = items.IndexOf(mCustomRoleToolStripSeparator);
                
                if (startIndex < index && startIndex > 0 && index < items.Count)
                {
                    for (int i = startIndex; i < index; i++) { if (items[i].Text == name) return; }
                }
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = name;
                item.Click += new EventHandler(delegate(object sender, EventArgs e)
                                                   {
                                                       if (mProjectView.Selection != null && mProjectView.CanAssignARole)
                                                       mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Custom, name);
                                                   });
                items.Insert(index, item);
            }

            // Update the custom class menu to remove this class
            private void Presentation_CustomClassRemoved(object sender, CustomClassEventArgs e)
            {
                RemoveCustomClassFromMenu(e.CustomClass, false);
            }


            private void RemoveCustomClassFromMenu(string customClassName, bool isProjectClosing)
            {
                if (!isProjectClosing &&  !string.IsNullOrEmpty(customClassName) && EmptyNode.SkippableNamesList.Contains(customClassName)) return;
                ToolStripItemCollection items = mPhrases_AssignRoleMenuItem.DropDownItems;
                int index;
                for (index = items.IndexOf(mCustomRoleToolStripSeparator);
                     index < items.IndexOf(mPhrases_AssignRole_NewCustomRoleMenuItem) &&
                     items[index].Text != customClassName;
                     ++index) ;
                if (index < items.IndexOf(mPhrases_AssignRole_NewCustomRoleMenuItem))
                {
                    mProjectView.RemoveCustomRoleFromContextMenu(items[index].Text, this);
                    items.RemoveAt(index);
                }
            }


            private void mPhrases_AssignRole_PlainMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.CanAssignPlainRole) mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Plain, null);
            }

            private void mPhrases_AssignRole_HeadingMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.CanAssignHeadingRole)
                    mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Heading, null);
            }

            private void mPhrases_AssignRole_PageMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.CanAssignARole) mProjectView.SetPageNumberOnSelection();
            }

            private void mPhrases_AssignRole_SilenceMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.CanAssignSilenceRole) mProjectView.SetSilenceRoleForSelectedPhrase();
            }

            private void mPhrases_EditRolesMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive) mProjectView.TransportBar.Pause();
                if (mProjectView.TransportBar.IsRecorderActive) mProjectView.TransportBar.Stop();
                EditRoles dialog = new EditRoles(mSession.Presentation, mProjectView);
                dialog.ShowDialog();
            }

            #endregion

            #region Transport menu

            // Update the transport manu
            private void UpdateTransportMenu()
            {
                mPlayToolStripMenuItem.Enabled = mProjectView.CanPlay || mProjectView.CanPlaySelection ||
                                                 mProjectView.CanResume;
                mPlayAllToolStripMenuItem.Enabled = mProjectView.CanPlay || mProjectView.CanResume;
                mPlaySelectionToolStripMenuItem.Enabled = mProjectView.CanPlaySelection || mProjectView.CanResume;
                if (mProjectView.CanResume)
                {
                    mPauseToolStripMenuItem.Visible = false;
                    mResumeToolStripMenuItem.Visible = true;
                }
                else
                {
                    mPauseToolStripMenuItem.Visible = true;
                    mPauseToolStripMenuItem.Enabled = mProjectView.CanPause;
                    mResumeToolStripMenuItem.Visible = false;
                }
                
                    
                
                mStopToolStripMenuItem.Enabled = mProjectView.CanStop;
                mPreviewToolStripMenuItem.Enabled = mProjectView.CanPreview || mProjectView.CanPreviewAudioSelection;
                mPreviewFromToolStripMenuItem.Enabled = mProjectView.CanPreview;
                mPreviewUpToToolStripMenuItem.Enabled = mProjectView.CanPreview;
                mPreviewSelectedToolStripMenuItem.Enabled = mProjectView.CanPreviewAudioSelection;
                mFineNavigationToolStripMenuItem.Enabled = mProjectView.TransportBar.FineNavigationModeForPhrase ||
                                                           mProjectView.TransportBar.CanEnterFineNavigationMode;
                mFineNavigationToolStripMenuItem.Checked = mProjectView.TransportBar.FineNavigationModeForPhrase;
                mPreviousSectionToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevSection;
                mPreviousPageToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevPage;
                mPreviousPhraseToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevPhrase;
                mNextPhraseToolStripMenuItem.Enabled = mProjectView.CanNavigateNextPhrase;
                mNextPageToolStripMenuItem.Enabled = mProjectView.CanNavigateNextPage;
                mNextSectionToolStripMenuItem.Enabled = mProjectView.CanNavigateNextSection;
                mFastForwardToolStripMenuItem.Enabled = mProjectView.CanFastForward;
                mRewindToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.CanRewind;
                navigationToolStripMenuItem.Enabled = mSession.HasProject;
                allEmptyPagesToolStripMenuItem.Enabled = allEmptyPagesAudioFileToolStripMenuItem.Enabled = mProjectView.CanGenerateSpeechForAllEmptyPages;
                m_TextToSpeechToolStripMenuItem.Enabled = mProjectView.CanImportPhrases && !(mProjectView.Selection is StripIndexSelection);
                selectedPageToolStripMenuItem.Enabled = selectedPageAudioFileToolStripMenuItem.Enabled = mProjectView.CanGenerateSpeechForPage;
                mGenerateSpeechToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mFastPlaytoolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mRecordToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.TransportBar.CanRecord;
                mStartRecordingDirectlyToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive;
                m_DeletePhrasesWhileRecordingtoolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive && mSettings.Audio_AllowOverwrite && mProjectView.TransportBar.CanRecord && !mProjectView.TransportBar.IsListening ;
                if (mProjectView.TransportBar.IsListening)
                {
                    mStartMonitoringToolStripMenuItem.Visible = false;
                    mStartRecordingToolStripMenuItem.Visible = true;
                    mStartRecordingToolStripMenuItem.Enabled = true;
                    mStartRecordingDirectlyToolStripMenuItem.Enabled = false;
                }
                else if (mProjectView.TransportBar.IsRecorderActive) // actual recording is going on
                {
                    mStartRecordingToolStripMenuItem.Visible = false;
                    mStartMonitoringToolStripMenuItem.Visible = true;
                    mStartMonitoringToolStripMenuItem.Enabled = false;
                    mStartRecordingDirectlyToolStripMenuItem.Enabled = false;
                }
                else // neither listening nor actual recording is going on
                {
                    mStartMonitoringToolStripMenuItem.Visible = true;
                    mStartMonitoringToolStripMenuItem.Enabled = mProjectView.TransportBar.Enabled;
                    mStartRecordingToolStripMenuItem.Visible = false;
                    mStartRecordingDirectlyToolStripMenuItem.Enabled = mProjectView.TransportBar.Enabled;
                }
                if (mProjectView.TransportBar.IsPreviewBeforeRecordingEnabled)
                {
                    m_PreviewBeforeRecordingToolStripMenuItem.Enabled = true;
                }
                else
                {
                    m_PreviewBeforeRecordingToolStripMenuItem.Enabled = false;
                }
                m_PlayHeadingToolStripMenuItem.Enabled = mProjectView.CanPlaySelection || mProjectView.CanResume;
                m_PlaySectionToolStripMenuItem.Enabled = mProjectView.CanPlaySelection || mProjectView.CanResume;
                if (mProjectView.Selection != null && mProjectView.Selection.Node != null)
                {
                    mBackwardElapseToolStripMenuItem.Enabled = mProjectView.Selection.Node is PhraseNode;
                    mForwardElapseToolStripMenuItem.Enabled = mProjectView.Selection.Node is PhraseNode;
                }
            }

            private void mPlayAllToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PlayAllSections();
            }

            private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PlayOrResume();
            }

            private void mPauseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Pause();
            }

            private void mResumeToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.CanResume) mProjectView.TransportBar.PlayOrResume();
            }

            private void mStopToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Stop();
            }

            private void mStartRecordingToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Record();
            }

            private void mStartMonitoringToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Record();
            }

            private void mStartRecordingDirectlyToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.StartRecordingDirectly(this.Settings != null &&  this.Settings.Audio_Recording_PreviewBeforeStarting);
            }


            private void PreviewFromtoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Preview(ProjectView.TransportBar.From, ProjectView.TransportBar.UseAudioCursor);
            }

            private void PreviewUptotoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Preview(ProjectView.TransportBar.Upto, ProjectView.TransportBar.UseAudioCursor);
            }

            private void PreviewSelectedAudiotoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PreviewAudioSelection();
            }

            private void previousSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PrevSection();
            }

            private void previousPageToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PrevPage();
            }

            private void previousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PrevPhrase();
            }

            private void nextPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.NextPhrase();
            }

            private void nextPageToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.NextPage();
            }

            private void nextSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.NextSection();
            }

            private void rewindToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.Rewind();
            }

            private void fastForwardToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastForward();
            }


















            private void NormalSpeedtoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastPlayRateNormalise();
            }

            private void SpeedUptoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastPlayRateStepUp();
            }

            private void SpeedDowntoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastPlayRateStepDown();
            }

            private void ElapseBacktoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastPlayNormaliseWithLapseBack();
            }


            private void mPhrases_ApplyPhraseDetectionMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ApplyPhraseDetection();
            }

            private void mPhrases_AudioSelection_BeginAudioSelectionMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.MarkSelectionBeginTime();
                UpdateAudioSelectionBlockMenuItems();
            }

            private void mPhrases_AudioSelection_EndAudioSelectionMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.MarkSelectionEndTime();
                UpdateAudioSelectionBlockMenuItems();
            }

            #endregion

            #region Tools menu

            private void UpdateToolsMenu()
            {
                mTools_ExportSelectedAudioMenuItem.Enabled = mProjectView.CanExportSelectedNodeAudio && !mProjectView.TransportBar.IsRecorderActive;
                mTools_ExportAsDAISYMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                m_EPUB3ValidatorToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive; 
                mTools_CleanUnreferencedAudioMenuItem.Enabled = mSession.HasProject &&
                                                                !mProjectView.TransportBar.IsRecorderActive;
                mTools_PreferencesMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive;
                PipelineMenuItemsEnabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                m_ToolsLangPack.Enabled = !mProjectView.TransportBar.IsRecorderActive;
                m_ChangeVolumeToolStripMenuItem.Enabled = m_SpeechRateToolStripMenuItem.Enabled = mProjectView.CanExportSelectedNodeAudio;
                m_NormalizeToolStripMenuItem.Enabled = m_NoiseReductionToolStripMenuItem.Enabled = m_NoiseReductionRnnToolStripMenuItem.Enabled = mProjectView.CanShowNormalizeNoiseReductionDialog && (mProjectView.CanExportSelectedNodeAudio || mProjectView.Selection == null);
                m_FadeInToolStripMenuItem.Enabled = m_FadeOutToolStripMenuItem.Enabled = m_AudioMixerToolStripMenuItem.Enabled =  mProjectView.CanShowFadeInFadeOutDialog;
                m_Tools_QuickCleanupToolStripMenuItem.Enabled = mSettings.Audio_EnableFileDataProviderPreservation;
                mTools_AudioProcessingNew.Enabled = mProjectView.CanShowProjectPropertiesDialog;
            }


            protected override bool ProcessCmdKey(ref Message msg, Keys key)   
            {
                if (key == (Keys.Control | Keys.C) && mProjectView != null && mProjectView.IsTOCViewInEditMode)
                {
                    return false;
                }

                return base.ProcessCmdKey(ref msg, key);
            }


            // Open the preferences dialog
            private void mTools_PreferencesMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause();
                if (mProjectView.IsZoomWaveformActive)
                {
                    mProjectView.ZoomPanelClose();
                }
                bool isLeftAlignPhrasesInContentView = mSettings.Project_LeftAlignPhrasesInContentView;
                bool showWaveform = mSettings.Project_ShowWaveformInContentView;
                bool enableEmptySectionColorInTOC = mSettings.Project_BackgroundColorForEmptySection;
                Dialogs.Preferences prefs = new Dialogs.Preferences(this, mSettings, mSession.Presentation,
                                                                    mProjectView.TransportBar, m_DefaultSettings);
                prefs.ShowDialog();
                if (prefs.IsColorChanged)
                    UpdateColors();
                if (mRecordingToolBarForm != null) mRecordingToolBarForm.UpdateForChangeInObi();
                Ready();
                mProjectView.TransportBar.UpdateButtons();
                mProjectView.ZoomPanelToolTipInit();
                if (mProjectView.Presentation != null && mProjectView.Presentation.FirstSection != null && ((enableEmptySectionColorInTOC != mSettings.Project_BackgroundColorForEmptySection) || (prefs.UpdateBackgroundColorRequired)))// @emptysectioncolor
                {
                    mProjectView.UpdateTOCBackColorForEmptySection((SectionNode)mProjectView.Presentation.FirstSection);// @emptysectioncolor
                }
                mProjectView.TransportBar.ResetFastPlayForPreferencesChange();
                if (isLeftAlignPhrasesInContentView != mSettings.Project_LeftAlignPhrasesInContentView) UpdateZoomFactor();
                mSession.EnableFreeDiskSpaceCheck = mSettings.Project_EnableFreeDiskSpaceCheck;
                if (showWaveform != mSettings.Project_ShowWaveformInContentView)
                {
                    mProjectView.RecreateStrip();
                }
            }

            private void mTools_ExportAsDAISYMenuItem_Click(object sender, EventArgs e)
            {
                
                if (mProjectView.IsZoomWaveformActive)
                {
                    mProjectView.ZoomPanelClose();
                }
                m_IsSkippableNoteInProject = false;
                ExportProject();

            }

            // Export the project as DAISY 3.
            private void ExportProject()
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();

                // returns if project is empty
                if (((ObiRootNode) mProjectView.Presentation.RootNode).SectionCount == 0)
                {
                    MessageBox.Show(Localizer.Message("ExportError_EmptyProject"), Localizer.Message("Caption_Error"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    mProjectView.Selection = null; // done for precaution 
                    return;
                }

                string exportPathDAISY3 = "";
                string exportPathDAISY202 = "";
                string exportPathEPUB3 = "";
                string exportPathXHTML = "";
                string exportPathMegaVoice = "";
                string exportPathMegaVoiceFinal = "";
                string exportPathWPAudioBook = "";

                urakawa.daisy.export.Daisy3_Export DAISY3ExportInstance = null;
                urakawa.daisy.export.Daisy3_Export DAISY202ExportInstance = null;
                urakawa.daisy.export.Daisy3_Export EPUB3_ExportInstance = null;
                ImportExport.ExportStructure XHTML_ExportInstance = null;
                ImportExport.MegaVoiceExport MegaVoice_ExportInstance = null;
                ImportExport.WPAudioBook_ObiExport WPAudioBook_ExportInstance = null;
                

                

                List<string> navList = new List<string>();
                navList.Add(EmptyNode.Annotation);
                navList.Add(EmptyNode.EndNote);
                navList.Add(EmptyNode.Footnote);
                navList.Add(EmptyNode.Note);
                navList.Add(EmptyNode.Sidebar);
                navList.Add(EmptyNode.ProducerNote);
                if (CheckedPageNumbers() && CheckedForEmptySectionsAndAnchoredReferences())
                {
                    
                    
                            //DAISYExport.EnableExplicitGarbageCollection = Settings.OptimizeMemory;
                            Status(String.Format(Localizer.Message("ObiFormStatusMsg_ExportingProject"), exportPathDAISY3));

                            
                        try
                        {
                            if (mSession.Presentation.ConfigurationsImportExport != null)
                            {
                                if (ConfigureExportWithProjectConfigurationFile(ref DAISY3ExportInstance,
                                ref exportPathDAISY3,
                                ref DAISY202ExportInstance,
                                ref exportPathDAISY202,
                                ref EPUB3_ExportInstance,
                                ref exportPathEPUB3) == false)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                if (ConfigureExportWithUserInterface(ref DAISY3ExportInstance,
                                    ref exportPathDAISY3,
                                    ref DAISY202ExportInstance,
                                    ref exportPathDAISY202,
                                    ref EPUB3_ExportInstance,
                                    ref exportPathEPUB3,
                                    ref XHTML_ExportInstance,
                                    ref exportPathXHTML,
                                    ref MegaVoice_ExportInstance,
                                    ref exportPathMegaVoice,
                                    ref exportPathMegaVoiceFinal,
                                    ref WPAudioBook_ExportInstance,
                                    ref exportPathWPAudioBook) == false)
                                {
                                    return;
                                }
                            }


                            mProjectView.TransportBar.Enabled = false;

                            ProgressDialog progress =
                                new ProgressDialog(Localizer.Message("export_progress_dialog_title"),
                                                   delegate(ProgressDialog progress1)
                                                   {                                                       
                                                       if (DAISY3ExportInstance != null)
                                                       {
                                                           mSession.Presentation.ExportToZ(exportPathDAISY3, mSession.Path,
                                                                                           DAISY3ExportInstance);
                                                       }
                                                       if (DAISY202ExportInstance != null)
                                                       {
                                                           mSession.Presentation.ExportToZ(exportPathDAISY202, mSession.Path,
                                                                                       DAISY202ExportInstance);
                                                       }

                                                       if (EPUB3_ExportInstance != null)
                                                       {

                                                           mSession.Presentation.ExportToZ(exportPathEPUB3, mSession.Path,
                                                                                           EPUB3_ExportInstance);
                                                       }
                                                       if (XHTML_ExportInstance != null)
                                                       {
                                                           XHTML_ExportInstance.CreateFileSet();
                                                       }
                                                       if (MegaVoice_ExportInstance != null)
                                                       {
                                                           mSession.Presentation.ExportToZ(exportPathMegaVoice, mSession.Path,
                                                                                           MegaVoice_ExportInstance);
                                                       }
                                                       if (WPAudioBook_ExportInstance != null)
                                                       {
                                                           mSession.Presentation.ExportToZ(exportPathWPAudioBook, mSession.Path, WPAudioBook_ExportInstance);
                                                       }
                                                   });

                            progress.OperationCancelled +=
                                new Obi.Dialogs.OperationCancelledHandler(
                                    delegate(object sender, EventArgs e) 
                                    { 
                                        if(DAISY3ExportInstance != null )  DAISY3ExportInstance.RequestCancellation = true;
                                        if (DAISY202ExportInstance != null) DAISY202ExportInstance.RequestCancellation = true;
                                        if (EPUB3_ExportInstance != null) EPUB3_ExportInstance.RequestCancellation = true;
                                        if (MegaVoice_ExportInstance != null) MegaVoice_ExportInstance.RequestCancellation = true;
                                        if (WPAudioBook_ExportInstance != null) WPAudioBook_ExportInstance.RequestCancellation = true;
                                    });
                            if (DAISY3ExportInstance != null) DAISY3ExportInstance.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            if (DAISY202ExportInstance != null) DAISY202ExportInstance.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            if (EPUB3_ExportInstance != null) EPUB3_ExportInstance.ProgressChangedEvent +=
                                 new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            if (MegaVoice_ExportInstance != null) MegaVoice_ExportInstance.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            if (WPAudioBook_ExportInstance != null) WPAudioBook_ExportInstance.ProgressChangedEvent +=
                                 new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            progress.ShowDialog();
                            
                            if (progress.Exception != null) throw progress.Exception;

                            if ((DAISY3ExportInstance != null && DAISY3ExportInstance.RequestCancellation)
                                || (DAISY202ExportInstance != null && DAISY202ExportInstance.RequestCancellation)
                                || (EPUB3_ExportInstance != null && EPUB3_ExportInstance.RequestCancellation)
                                || (MegaVoice_ExportInstance != null && MegaVoice_ExportInstance.RequestCancellation)
                                || WPAudioBook_ExportInstance != null && WPAudioBook_ExportInstance.RequestCancellation)
                            {
                                mProjectView.TransportBar.Enabled = true;
                                return;
                            }
                            
                            if (exportPathDAISY3 != null)
                                mProjectView.SetExportPathMetadata(ImportExport.ExportFormat.DAISY3_0,
                                                                   exportPathDAISY3,
                                                                   Directory.GetParent(mSession.Path).FullName);

                            if (exportPathDAISY202 != null)
                                mProjectView.SetExportPathMetadata(ImportExport.ExportFormat.DAISY2_02,
                                                                   exportPathDAISY202,
                                                                   Directory.GetParent(mSession.Path).FullName);

                            if (exportPathEPUB3 != null)
                                mProjectView.SetExportPathMetadata(ImportExport.ExportFormat.EPUB3,
                                                                   exportPathEPUB3,
                                                                   Directory.GetParent(mSession.Path).FullName);
                            
                            mSession.ForceSave();

                            string displayPath = (exportPathDAISY3 != null && exportPathDAISY202 != null) ? exportPathDAISY3 + "\n" + exportPathDAISY202 :
                                exportPathDAISY3 != null ? exportPathDAISY3 : exportPathDAISY202;
                            if (exportPathMegaVoiceFinal != null)
                            {
                                if (string.IsNullOrEmpty(displayPath))
                                {
                                    displayPath = exportPathMegaVoiceFinal;
                                }
                                else
                                {
                                    displayPath += "\n" + exportPathMegaVoiceFinal;
                                }
                            }
                            if (exportPathEPUB3 != null)
                            {
                                if (string.IsNullOrEmpty(displayPath) )
                                {
                                displayPath = exportPathEPUB3;
                                }
                                else
                                {
                                    displayPath = displayPath+ "\n" +   exportPathEPUB3;
                                }
                            }
                            if (exportPathXHTML != null)
                            {
                                if (string.IsNullOrEmpty(displayPath))
                                {
                                    displayPath = exportPathXHTML;
                                }
                                else
                                {
                                    displayPath = displayPath + "\n" + exportPathXHTML;
                                }
                            }
                            if (exportPathWPAudioBook != null)
                            {
                                if (string.IsNullOrEmpty(displayPath))
                                {
                                    displayPath = exportPathWPAudioBook;
                                }
                                else
                                {
                                    displayPath += "\n" + exportPathWPAudioBook;
                                }
                            }
                            MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), displayPath),
                                            Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }
                        catch (Exception e)
                        {
                            
                            string displayPath = (exportPathDAISY3 != null && exportPathDAISY202 != null) ? exportPathDAISY3 + "\n" + exportPathDAISY202 :
                                exportPathDAISY3 != null ? exportPathDAISY3 : 
                                exportPathDAISY202 != null? exportPathDAISY202: 
                                exportPathEPUB3;

                            string errorMsg = " ";
                            if (e != null) errorMsg = e.Message;

                            MessageBox.Show(
                                String.Format(Localizer.Message("didnt_save_as_daisy_text"), displayPath,
                                              errorMsg),
                                Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Status(Localizer.Message("didnt_save_as_daisy_caption"));
                        }
                    }
                //}
                mProjectView.TransportBar.Enabled = true;
                Ready();

                try
                {
                    if (exportPathMegaVoice != null)
                    {
                        if (Directory.Exists(exportPathMegaVoice))
                        {
                            Directory.Delete(exportPathMegaVoice, true);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Localizer.Message("DidnotDeleteTempMegaVoiceFolder"), e.Message, exportPathMegaVoice), Localizer.Message("Caption_Information"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }

            private bool ConfigureExportWithUserInterface(ref urakawa.daisy.export.Daisy3_Export DAISY3ExportInstance,
ref string exportDirectoryDAISY3,
                ref urakawa.daisy.export.Daisy3_Export DAISY202ExportInstance,
                ref string exportDirectoryDAISY202,
                ref urakawa.daisy.export.Daisy3_Export EPUB3_ExportInstance,
                ref string exportDirectoryEPUB3,
                ref  ImportExport.ExportStructure XHTML_ExportInstance, 
                ref  string exportDirectoryXHTML,
                ref ImportExport.MegaVoiceExport MegaVoice_ExportInstance,
                ref string exportDirectoryMegaVoice,
                ref string MegavoiceFinalExportPath,
                ref ImportExport.WPAudioBook_ObiExport WPAudioBooks_ExportInstance,
                ref string exportDirectoryWPAudioBook)
            {
                Dialogs.chooseDaisy3orDaisy202 chooseDialog = new chooseDaisy3orDaisy202(this.mSettings);
                if (chooseDialog.ShowDialog() == DialogResult.OK)
                {
                    if (chooseDialog.ExportDaisy3)
                    {
                        exportDirectoryDAISY3 = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(Localizer.Message("default_export_dirname"), "")));
                    }
                    if (chooseDialog.ExportDaisy2)
                    {
                        exportDirectoryDAISY202 = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(
                                                               Localizer.Message("Default_DAISY2_02export_dirname"), "")));
                    }

                    if (chooseDialog.ExportEpub3)
                    {
                        exportDirectoryEPUB3 = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(Localizer.Message("default_EpubExport_dirname"), "")));
                    }
                    if (chooseDialog.ExportXhtml)
                    {
                        exportDirectoryXHTML = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(Localizer.Message("default_XHTMLExport_dirname"), "")));
                    }
                    if (chooseDialog.ExportMegaVoice)
                    {
                        exportDirectoryMegaVoice = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                        Program.SafeName(
                                                            string.Format(Localizer.Message("default_MegaVoiceExport_dirname"), "")));
                    }
                    if (chooseDialog.ExportWPAudioBook)
                    {
                        exportDirectoryWPAudioBook = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                        Program.SafeName(
                                                            string.Format(Localizer.Message("default_WPAudioBookExport_dirname"), "")));
                    }
                }
                if (string.IsNullOrEmpty(exportDirectoryDAISY3) && string.IsNullOrEmpty(exportDirectoryDAISY202) && string.IsNullOrEmpty(exportDirectoryEPUB3) && 
                    string.IsNullOrEmpty(exportDirectoryXHTML) &&  string.IsNullOrEmpty(exportDirectoryMegaVoice) && string.IsNullOrEmpty(exportDirectoryWPAudioBook))
                {
                    return false;
                }

                Dialogs.ExportDirectory ExportDialogDAISY3 = null;
                Dialogs.ExportDirectory ExportDialogDAISY202 = null;
                Dialogs.ExportDirectory ExportDialogEPUB3 = null;
                Dialogs.ExportDirectory ExportDialogXhtml = null;
                Dialogs.ExportDirectory ExportDialogMegaVoice = null;
                Dialogs.ExportDirectory ExportDialogWPAudioBook = null;

                if (chooseDialog.ExportDaisy3)
                {
                    ExportDialogDAISY3 =
                        new ExportDirectory(exportDirectoryDAISY3,
                                            mSession.Path, mSettings.Export_EncodeAudioFiles, (mSettings.ExportEncodingBitRate),
                                            mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat, this.mSettings); //@fontconfig
                    // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogDAISY3.AdditionalTextForTitle = "DAISY 3";
                    ExportDialogDAISY3.LimitLengthOfAudioFileNames = mSettings.Export_LimitAudioFilesLength &&
                                                         mSettings.Export_AppendSectionNameToAudioFile;
                    ExportDialogDAISY3.CreateCSVForCuesEnabled = true;
                    ExportDialogDAISY3.AddCuePointsInAudioEnabled = true;
                    ExportDialogDAISY3.AudioFileNameCharsLimit = Settings.Export_AudioFilesNamesLengthLimit >= 0 ? Settings.Export_AudioFilesNamesLengthLimit : 8;
                    if (ExportDialogDAISY3.ShowDialog() != DialogResult.OK) ExportDialogDAISY3 = null;
                }

                if (chooseDialog.ExportDaisy2)
                {
                    ExportDialogDAISY202 =
                        new ExportDirectory(exportDirectoryDAISY202,
                                            mSession.Path, mSettings.Export_EncodeAudioFiles, (mSettings.ExportEncodingBitRate),
                                            mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat, this.mSettings); //@fontconfig
                    // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogDAISY202.AdditionalTextForTitle = "DAISY 2.02";
                    ExportDialogDAISY202.LimitLengthOfAudioFileNames = mSettings.Export_LimitAudioFilesLength &&
                                                         mSettings.Export_AppendSectionNameToAudioFile;
                    ExportDialogDAISY202.CreateCSVForCuesEnabled = true;
                    ExportDialogDAISY202.AddCuePointsInAudioEnabled = true;
                    ExportDialogDAISY202.AudioFileNameCharsLimit = Settings.Export_AudioFilesNamesLengthLimit >= 0 ? Settings.Export_AudioFilesNamesLengthLimit : 8;
                    if (ExportDialogDAISY202.ShowDialog() != DialogResult.OK) ExportDialogDAISY202 = null;
                }

                if (chooseDialog.ExportEpub3)
                {
                    ExportDialogEPUB3 =
                        new ExportDirectory(exportDirectoryEPUB3,
                                            mSession.Path, true, (mSettings.ExportEncodingBitRate),
                                            mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat, this.mSettings); //@fontconfig
                    //   null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogEPUB3.EpubLengthCheckboxEnabled = true;
                    ExportDialogEPUB3.CreateDummyTextCheckboxEnabled = true;
                    ExportDialogEPUB3.CreateMediaOverlaysForNavigationDocChecked = true;
                    ExportDialogEPUB3.EPUB_CreateDummyTextInHtml = mSettings.Export_EPUBCreateDummyText;
                    ExportDialogEPUB3.AdditionalTextForTitle = "Epub 3";
                    ExportDialogEPUB3.LimitLengthOfAudioFileNames = mSettings.Export_LimitAudioFilesLength &&
                                                         mSettings.Export_AppendSectionNameToAudioFile;
                    ExportDialogEPUB3.EPUBFileLength = mSettings.Export_EPUBFileNameLengthLimit;
                    ExportDialogEPUB3.AudioFileNameCharsLimit = Settings.Export_AudioFilesNamesLengthLimit >= 0 ? Settings.Export_AudioFilesNamesLengthLimit : 8;
                    if (ExportDialogEPUB3.ShowDialog() != DialogResult.OK) ExportDialogEPUB3 = null;

                }
                if (chooseDialog.ExportXhtml)
                {
                    //ExportDialogXhtml =
                    //    new ExportDirectory(exportDirectoryDAISY202,
                    //                        mSession.Path, false, (mSettings.ExportEncodingBitRate),
                    //                        mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat);
                    ExportDialogXhtml = new ExportDirectory(exportDirectoryXHTML, mSession.Path, false, (mSettings.ExportEncodingBitRate), false, string.Empty, this.mSettings); //@fontconfig
                        //   null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogXhtml.XhtmlElmentsEnabled = false;
                    if (mSettings.Project_VAXhtmlExport)
                    {
                        ExportDialogXhtml.AdditionalTextForTitle = "Xhtml (VA profile)";
                    }
                    else
                    {
                        ExportDialogXhtml.AdditionalTextForTitle = "Xhtml structure";
                    }
                    if (ExportDialogXhtml.ShowDialog() != DialogResult.OK) ExportDialogXhtml = null;

                }

                if (chooseDialog.ExportMegaVoice)
                {
                    string FileName = Path.GetFileName(Path.GetDirectoryName(mSession.Path));
                    if (FileName.Length > 58)
                    {
                        FileName = FileName.Substring(0, 58);
                    }
                    ExportDialogMegaVoice =
                        new ExportDirectory(exportDirectoryMegaVoice,
                                            mSession.Path, mSettings.Export_EncodeAudioFiles, (mSettings.ExportEncodingBitRate),
                                            mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat, this.mSettings,true,FileName); //@fontconfig
                    // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogMegaVoice.LevelSelection = 1;
                    ExportDialogMegaVoice.AppendSectionNameToAudioFileName = true;
                    ExportDialogMegaVoice.EncodeAudioFiles = true;

                    ExportDialogMegaVoice.AdditionalTextForTitle = Localizer.Message("ExportDialogTitleForMegaVoiceExport");
                    ExportDialogMegaVoice.LimitLengthOfAudioFileNames = true;
                    ExportDialogMegaVoice.AudioFileNameCharsLimit = 100;
                    if (ExportDialogMegaVoice.ShowDialog() != DialogResult.OK) ExportDialogMegaVoice = null;
                }

                if (chooseDialog.ExportWPAudioBook)
                {
                    string FileName = Path.GetFileName(Path.GetDirectoryName(mSession.Path));
                    if (FileName.Length > 58)
                    {
                        FileName = FileName.Substring(0, 58);
                    }
                    ExportDialogWPAudioBook =
                        new ExportDirectory(exportDirectoryWPAudioBook,
                                            mSession.Path, mSettings.Export_EncodeAudioFiles, (mSettings.ExportEncodingBitRate),
                                            mSettings.Export_AppendSectionNameToAudioFile, mSettings.EncodingFileFormat, this.mSettings); //@fontconfig
                    // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                    ExportDialogWPAudioBook.LevelSelection = 0;
                    ExportDialogWPAudioBook.SelectLevelForAudioLevelsEnabled = false;
                    //ExportDialogWPAudioBook.AppendSectionNameToAudioFileName = true;
                    ExportDialogWPAudioBook.EncodeAudioFiles = true;

                    ExportDialogWPAudioBook.AdditionalTextForTitle = Localizer.Message("ExportDialogTitleForWPAudioBookExport");
                    //ExportDialogWPAudioBook.LimitLengthOfAudioFileNames = true;
                    //ExportDialogWPAudioBook.AudioFileNameCharsLimit = 100;
                    if (ExportDialogWPAudioBook.ShowDialog() != DialogResult.OK) ExportDialogWPAudioBook = null;
                    
                }

                if (ExportDialogDAISY3 != null || ExportDialogDAISY202 != null || ExportDialogEPUB3 != null || ExportDialogXhtml != null || ExportDialogMegaVoice != null || ExportDialogWPAudioBook != null)
                {

                    // Need the trailing slash, otherwise exported data ends up in a folder one level
                    // higher than our selection.
                    string exportPathDAISY3 = ExportDialogDAISY3 != null ? ExportDialogDAISY3.DirectoryPath : null;
                    string exportPathDAISY202 = ExportDialogDAISY202 != null ? ExportDialogDAISY202.DirectoryPath : null;
                    string exportPathEPUB3 = ExportDialogEPUB3 != null ? ExportDialogEPUB3.DirectoryPath : null;
                    string exportPathXhtml = ExportDialogXhtml != null ? ExportDialogXhtml.DirectoryPath : null;
                    string exportPathMegaVoice = ExportDialogMegaVoice != null ? exportDirectoryMegaVoice : null;
                    string exportPathWPAudioBook = ExportDialogWPAudioBook != null ? ExportDialogWPAudioBook.DirectoryPath : null;
                    
                    Dialogs.ExportDirectory dialog = ExportDialogDAISY3 != null ? ExportDialogDAISY3 : 
                        ExportDialogDAISY202 != null ? ExportDialogDAISY202 : 
                        ExportDialogEPUB3 != null? ExportDialogEPUB3: ExportDialogXhtml != null? ExportDialogXhtml :
                        ExportDialogMegaVoice != null ? ExportDialogMegaVoice : ExportDialogWPAudioBook;

                    if (dialog != ExportDialogXhtml)
                    {
                        mSettings.Export_EncodeAudioFiles = dialog.EncodeAudioFiles;
                        mSettings.ExportEncodingBitRate = dialog.BitRate;
                        mSettings.EncodingFileFormat = dialog.EncodingFileFormat.ToString();
                        //mSettings.Encoding_SelectedIndex = dialog.EncodingFileFormat;
                        mSettings.Export_AppendSectionNameToAudioFile = dialog.AppendSectionNameToAudioFileName;
                        mSettings.Export_LimitAudioFilesLength = dialog.AppendSectionNameToAudioFileName &&
                                                                 dialog.LimitLengthOfAudioFileNames;
                        mSettings.Export_AudioFilesNamesLengthLimit = dialog.AudioFileNameCharsLimit;
                    }
                    if (ExportDialogEPUB3 != null && ExportDialogEPUB3.EpubLengthCheckboxEnabled) mSettings.Export_EPUBFileNameLengthLimit = ExportDialogEPUB3.EPUBFileLength;
                    if (ExportDialogEPUB3 != null) mSettings.Export_EPUBCreateDummyText = ExportDialogEPUB3.EPUB_CreateDummyTextInHtml;
                    if (!string.IsNullOrEmpty(exportPathDAISY3) && !exportPathDAISY3.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        exportPathDAISY3 += Path.DirectorySeparatorChar;
                    }

                    if (!string.IsNullOrEmpty(exportPathDAISY202) && !exportPathDAISY202.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        exportPathDAISY202 += Path.DirectorySeparatorChar;
                    }

                    if (!string.IsNullOrEmpty(exportPathXhtml) && !exportPathXhtml.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        exportPathXhtml  += Path.DirectorySeparatorChar;
                    }

                    if (!string.IsNullOrEmpty(exportPathMegaVoice) && !exportPathMegaVoice.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        exportPathMegaVoice += Path.DirectorySeparatorChar;
                    }

                    if (!string.IsNullOrEmpty(exportPathWPAudioBook) && !exportPathWPAudioBook.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        exportPathWPAudioBook += Path.DirectorySeparatorChar;
                    }

                    if (ExportDialogDAISY3 != null)
                    {
                        DAISY3ExportInstance = new Obi.ImportExport.DAISY3_ObiExport(
                            mSession.Presentation, exportPathDAISY3, null, ExportDialogDAISY3.EncodeAudioFiles, ExportDialogDAISY3.BitRate,
                            AudioLib.SampleRate.Hz44100,
                            mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2,
                            false, ExportDialogDAISY3.LevelSelection, ExportDialogDAISY3.CreateCSVForCues, ExportDialogDAISY3.AddCuePoints, ExportDialogDAISY3.IsInsertCuePoints);

                        DAISY3ExportInstance.AddSectionNameToAudioFile = ExportDialogDAISY3.AppendSectionNameToAudioFileName;
                        DAISY3ExportInstance.AudioFileNameCharsLimit = ExportDialogDAISY3.AudioFileNameCharsLimit;
                        if (ExportDialogDAISY3.EnabledAdvancedParameters) DAISY3ExportInstance.SetAdditionalMp3EncodingParameters(ExportDialogDAISY3.Mp3ChannelMode, ExportDialogDAISY3.Mp3ReSample, ExportDialogDAISY3.Mp3RePlayGain);
                        ((Obi.ImportExport.DAISY3_ObiExport)DAISY3ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                        DAISY3ExportInstance.EncodingFileFormat = ExportDialogDAISY3.EncodingFileFormat;
                    }
                    if (ExportDialogDAISY202 != null)
                    {
                        DAISY202ExportInstance = new Obi.ImportExport.DAISY202Export(
                            mSession.Presentation, exportPathDAISY202, ExportDialogDAISY202.EncodeAudioFiles, ExportDialogDAISY202.BitRate,
                            AudioLib.SampleRate.Hz44100, mSettings.Audio_Channels == 2,
                            ExportDialogDAISY202.LevelSelection, mSettings.Audio_RemoveAccentsFromDaisy2ExportFileNames, ExportDialogDAISY202.CreateCSVForCues, ExportDialogDAISY202.AddCuePoints, ExportDialogDAISY202.IsInsertCuePoints);

                        DAISY202ExportInstance.AddSectionNameToAudioFile = ExportDialogDAISY202.AppendSectionNameToAudioFileName;
                        DAISY202ExportInstance.AudioFileNameCharsLimit = ExportDialogDAISY202.AudioFileNameCharsLimit;
                        if (ExportDialogDAISY202.EnabledAdvancedParameters) DAISY202ExportInstance.SetAdditionalMp3EncodingParameters(ExportDialogDAISY202.Mp3ChannelMode, ExportDialogDAISY202.Mp3ReSample, ExportDialogDAISY202.Mp3RePlayGain);
                        ((Obi.ImportExport.DAISY202Export)DAISY202ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                        DAISY202ExportInstance.EncodingFileFormat = ExportDialogDAISY202.EncodingFileFormat;

                        // check for custom metadata
                        List<string> DAISYMetadataNames = Metadata.DAISY3MetadataNames;
                        string customMetadata = null;
                        foreach (urakawa.metadata.Metadata m in mProjectView.Presentation.Metadatas.ContentsAs_Enumerable)
                        {
                            if (!DAISYMetadataNames.Contains(m.NameContentAttribute.Name))
                            {
                                customMetadata += "\n";
                                customMetadata += m.NameContentAttribute.Name;
                            }   
                        }
                        if (customMetadata != null) MessageBox.Show(Localizer.Message("CustomMetadataD202_NotIncluded") + '\n' + customMetadata, Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (m_IsSkippableNoteInProject)
                        {
                            MessageBox.Show(Localizer.Message("SkippableNotExportedInDaisy202"), Localizer.Message("Caption_Information"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                        }
                    }

                    if (ExportDialogEPUB3 != null)
                    {
                        EPUB3_ExportInstance = new Obi.ImportExport.Epub3_ObiExport(
                            mSession.Presentation, exportPathEPUB3, null, ExportDialogEPUB3.EncodeAudioFiles, ExportDialogEPUB3.BitRate,
                            AudioLib.SampleRate.Hz44100,
                            mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2,
                            false, ExportDialogEPUB3.LevelSelection,
                            ExportDialogEPUB3.EpubLengthCheckboxEnabled ? mSettings.Export_EPUBFileNameLengthLimit : 0,
                            ExportDialogEPUB3.CreateDummyTextCheckboxEnabled, ExportDialogEPUB3.CreateMediaOverlaysForNavigationDocChecked);

                        EPUB3_ExportInstance.AddSectionNameToAudioFile = ExportDialogEPUB3.AppendSectionNameToAudioFileName;
                        EPUB3_ExportInstance.AudioFileNameCharsLimit = ExportDialogEPUB3.AudioFileNameCharsLimit;

                        if (ExportDialogEPUB3.EnabledAdvancedParameters) EPUB3_ExportInstance.SetAdditionalMp3EncodingParameters(ExportDialogEPUB3.Mp3ChannelMode, ExportDialogEPUB3.Mp3ReSample, ExportDialogEPUB3.Mp3RePlayGain);
                        ((Obi.ImportExport.Epub3_ObiExport)EPUB3_ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                        EPUB3_ExportInstance.EncodingFileFormat = ExportDialogEPUB3.EncodingFileFormat;
                    }
                    if (ExportDialogXhtml != null)
                    {   
                        XHTML_ExportInstance = new Obi.ImportExport.ExportStructure(mProjectView.Presentation, exportPathXhtml);
                        if (mSettings.Project_VAXhtmlExport) XHTML_ExportInstance.Profile_VA = true;
                        
                    }

                    if (ExportDialogMegaVoice != null)
                    {
                        string FileName = Path.GetFileName(Path.GetDirectoryName(mSession.Path));
                        if (FileName.Length > 58)
                        {
                          FileName = FileName.Substring(0, 58);
                        }
                        MegavoiceFinalExportPath = ExportDialogMegaVoice.DirectoryPath + "\\" +  FileName;
                        MegaVoice_ExportInstance = new Obi.ImportExport.MegaVoiceExport(
                            mSession.Presentation, exportPathMegaVoice, null, ExportDialogMegaVoice.EncodeAudioFiles, ExportDialogMegaVoice.BitRate,
                            AudioLib.SampleRate.Hz44100,
                            mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2,
                            false, ExportDialogMegaVoice.LevelSelection, MegavoiceFinalExportPath);

                        MegaVoice_ExportInstance.AddSectionNameToAudioFile = ExportDialogMegaVoice.AppendSectionNameToAudioFileName;
                        MegaVoice_ExportInstance.AudioFileNameCharsLimit = ExportDialogMegaVoice.AudioFileNameCharsLimit;
                        if (ExportDialogMegaVoice.EnabledAdvancedParameters) MegaVoice_ExportInstance.SetAdditionalMp3EncodingParameters(ExportDialogMegaVoice.Mp3ChannelMode, ExportDialogMegaVoice.Mp3ReSample, ExportDialogMegaVoice.Mp3RePlayGain);
                        ((Obi.ImportExport.DAISY3_ObiExport)MegaVoice_ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                        MegaVoice_ExportInstance.EncodingFileFormat = ExportDialogMegaVoice.EncodingFileFormat;
                    }
                    if (ExportDialogWPAudioBook != null)
                    {
                        WPAudioBooks_ExportInstance = new Obi.ImportExport.WPAudioBook_ObiExport(
                            mSession.Presentation, exportPathWPAudioBook, null, ExportDialogWPAudioBook.EncodeAudioFiles, ExportDialogWPAudioBook.BitRate,
                            AudioLib.SampleRate.Hz44100,
                            mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2,
                            false, ExportDialogWPAudioBook.LevelSelection);

                        WPAudioBooks_ExportInstance.AddSectionNameToAudioFile = ExportDialogWPAudioBook.AppendSectionNameToAudioFileName;
                        WPAudioBooks_ExportInstance.AudioFileNameCharsLimit = ExportDialogWPAudioBook.AudioFileNameCharsLimit;
                        if (ExportDialogWPAudioBook.EnabledAdvancedParameters) WPAudioBooks_ExportInstance.SetAdditionalMp3EncodingParameters(ExportDialogWPAudioBook.Mp3ChannelMode, ExportDialogWPAudioBook.Mp3ReSample, ExportDialogWPAudioBook.Mp3RePlayGain);
                        ((Obi.ImportExport.WPAudioBook_ObiExport)WPAudioBooks_ExportInstance).AlwaysIgnoreIndentation = false;
                        WPAudioBooks_ExportInstance.EncodingFileFormat = ExportDialogWPAudioBook.EncodingFileFormat;
                    }
                    exportDirectoryDAISY202 = exportPathDAISY202;
                    exportDirectoryDAISY3 = exportPathDAISY3;
                    exportDirectoryEPUB3 = exportPathEPUB3;
                    exportDirectoryXHTML = exportPathXhtml;
                    exportDirectoryMegaVoice = exportPathMegaVoice;
                    exportDirectoryWPAudioBook = exportPathWPAudioBook;
                    return true;
                }
                //else if (ExportDialogXhtml != null)
                //{
                    //exportDirectoryXHTML = ExportDialogXhtml.DirectoryPath;
                    //ImportExport.ExportStructure stru = new Obi.ImportExport.ExportStructure(mProjectView.Presentation, exportDirectoryXHTML);
                    //if (mSettings.SettingsName.Contains("Profile-1-VA")) stru.Profile_VA = true;
                    //stru.CreateFileSet();
                    //return true;
                //}
                else
                {
                    return false;
                }
            }

            private bool ConfigureExportWithProjectConfigurationFile (ref urakawa.daisy.export.Daisy3_Export DAISY3ExportInstance,
ref string exportDirectoryDAISY3,
                ref urakawa.daisy.export.Daisy3_Export DAISY202ExportInstance,
ref string exportDirectoryDAISY202,
                ref urakawa.daisy.export.Daisy3_Export EPUB3_ExportInstance,
ref string exportDirectoryEPUB3)
            {
                if (mSession.Presentation == null || mSession.Presentation.ConfigurationsImportExport == null) return false;

                ImportExport.ConfigurationFileParser configInstance = mSession.Presentation.ConfigurationsImportExport;
                
                
                if (configInstance.DAISY3ExportParameters != null)
                {
                    // Delete the export directory if it already exists
                    if (Directory.Exists(configInstance.DAISY3ExportParameters.ExportDirectory))
                    {
                        Directory.Delete(configInstance.DAISY3ExportParameters.ExportDirectory, true);
                    }
                    DAISY3ExportInstance = new Obi.ImportExport.DAISY3_ObiExport(
                            mSession.Presentation, configInstance.DAISY3ExportParameters.ExportDirectory, null,
                            configInstance.DAISY3ExportParameters.EncodeExportedAudioFiles,
                            configInstance.DAISY3ExportParameters.ExportEncodingBitrate,
                            configInstance.DAISY3ExportParameters.ExportSampleRate,
                            configInstance.DAISY3ExportParameters.ExportChannels == 2,
                            false, 100);

                    DAISY3ExportInstance.AddSectionNameToAudioFile = false;
                    DAISY3ExportInstance.AudioFileNameCharsLimit = 100;
                    if (configInstance.DAISY3ExportParameters.EncodingAudioFileFormat != AudioLib.AudioFileFormats.MP3) 
                        DAISY3ExportInstance.EncodingFileFormat = configInstance.DAISY3ExportParameters.EncodingAudioFileFormat;

                    ((Obi.ImportExport.DAISY3_ObiExport)DAISY3ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                    exportDirectoryDAISY3 = configInstance.DAISY3ExportParameters.ExportDirectory;
                    //exportDirectoryEPUB3 = exportDirectoryDAISY202 = null;
                }

                if (configInstance.DAISY202ExportParameters != null)
                {
                    // Delete the export directory if it already exists
                    if (Directory.Exists(configInstance.DAISY202ExportParameters.ExportDirectory))
                    {
                        Directory.Delete(configInstance.DAISY202ExportParameters.ExportDirectory, true);
                    }
                    DAISY202ExportInstance = new Obi.ImportExport.DAISY202Export(
                        mSession.Presentation, configInstance.DAISY202ExportParameters.ExportDirectory,
                    configInstance.DAISY202ExportParameters.EncodeExportedAudioFiles ,
                    configInstance.DAISY202ExportParameters.ExportEncodingBitrate,
                        configInstance.DAISY202ExportParameters.ExportSampleRate , 
                    configInstance.DAISY202ExportParameters.ExportChannels == 2,
                        100,mSettings.Audio_RemoveAccentsFromDaisy2ExportFileNames);

                    DAISY202ExportInstance.AddSectionNameToAudioFile = false;
                    DAISY202ExportInstance.AudioFileNameCharsLimit = 100;
                    if (configInstance.DAISY202ExportParameters.EncodingAudioFileFormat != AudioLib.AudioFileFormats.MP3) 
                        DAISY202ExportInstance.EncodingFileFormat = configInstance.DAISY202ExportParameters.EncodingAudioFileFormat;

                    ((Obi.ImportExport.DAISY202Export)DAISY202ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                    exportDirectoryDAISY202 = configInstance.DAISY202ExportParameters.ExportDirectory;
                    //exportDirectoryDAISY3 = exportDirectoryEPUB3 = null;
                }

                if (configInstance.EPUB3ExportParameters != null)
                {
                    // Delete the export directory if it already exists
                    if (Directory.Exists(configInstance.EPUB3ExportParameters.ExportDirectory))
                    {
                        Directory.Delete(configInstance.EPUB3ExportParameters.ExportDirectory, true);
                    }
                    EPUB3_ExportInstance = new Obi.ImportExport.Epub3_ObiExport(
                            mSession.Presentation, configInstance.EPUB3ExportParameters.ExportDirectory, null, 
                            configInstance.EPUB3ExportParameters.EncodeExportedAudioFiles,
                            configInstance.EPUB3ExportParameters.ExportEncodingBitrate,
                            configInstance.EPUB3ExportParameters.ExportSampleRate,
                            configInstance.EPUB3ExportParameters.ExportChannels == 2,
                            false, 100,
                            0,
                            true,
                            false);

                    EPUB3_ExportInstance.AddSectionNameToAudioFile = false;
                    EPUB3_ExportInstance.AudioFileNameCharsLimit = 100;
                    if (configInstance.EPUB3ExportParameters.EncodingAudioFileFormat != AudioLib.AudioFileFormats.MP3) 
                        EPUB3_ExportInstance.EncodingFileFormat = configInstance.EPUB3ExportParameters.EncodingAudioFileFormat;

                    ((Obi.ImportExport.Epub3_ObiExport)EPUB3_ExportInstance).AlwaysIgnoreIndentation = mSettings.Project_Export_AlwaysIgnoreIndentation;
                    exportDirectoryEPUB3 = configInstance.EPUB3ExportParameters.ExportDirectory;
                    //exportDirectoryDAISY202 = exportDirectoryDAISY3 = null;
                }

                if (exportDirectoryDAISY202 != null && !Directory.Exists(exportDirectoryDAISY202)) exportDirectoryDAISY202 = null;
                if (exportDirectoryDAISY3 != null && !Directory.Exists(exportDirectoryDAISY3)) exportDirectoryDAISY3  = null;
                if (exportDirectoryEPUB3 != null && !Directory.Exists(exportDirectoryEPUB3)) exportDirectoryEPUB3 = null;

                return true;
            }

         
            // Check that page numbers are valid before exporting and return true if they are.
            // If they're not, the user is presented with the possibility to cancel export (return false)
            // or automatically renumber, in which case we also return true.
            // Only normal and front pages are considered, and we skip empty blocks since they're not exported.
            private bool CheckedPageNumbers()
            {
                bool retVal = false;
                try
                {
                    retVal = CheckPageNumbers(PageKind.Front) && CheckPageNumbers(PageKind.Normal);
                }
                catch (System.Exception ex)
                {
                    retVal = false;
                    MessageBox.Show(Localizer.Message("Operation_Cancelled") + "\n\n" + ex.ToString());
                        //@Messagecorrected
                }

                return retVal;
            }

            /// <summary>
            /// Check page numbers of a given kind and give the option to renumber from the first duplicate value if
            /// a duplicate is found.
            /// </summary>
            private bool CheckPageNumbers(PageKind kind)
            {
                Dictionary<int, EmptyNode> pages = new Dictionary<int, EmptyNode>();
                EmptyNode renumberFrom = null;
                PageNumber renumberNumber = null;
                mSession.Presentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                        {
                            EmptyNode phrase = n as EmptyNode;
                            if (phrase != null && phrase.Role_ == EmptyNode.Role.Page && phrase.PageNumber.Kind == kind)
                            {
                                if (pages.ContainsKey(phrase.PageNumber.Number))
                                {
                                    if (renumberFrom == null)
                                    {
                                        renumberFrom = phrase;
                                        renumberNumber = phrase.PageNumber.NextPageNumber();
                                        while (pages.ContainsKey(renumberNumber.Number))
                                            renumberNumber = renumberNumber.NextPageNumber();
                                    }
                                }
                                else
                                {
                                    pages.Add(phrase.PageNumber.Number, phrase);
                                }
                            }
                            if (phrase != null && phrase.Role_ == EmptyNode.Role.Custom)
                            {
                                m_IsSkippableNoteInProject = true;
                            }
                            return true;
                        },
                    delegate(urakawa.core.TreeNode n) { });
                if (renumberFrom != null)
                {
                    // There are duplicates, so ask if we should renumber and continue, or stop here.
                    if (MessageBox.Show(
                        string.Format(Localizer.Message("renumber_before_export"), renumberFrom.PageNumber.ToString()),
                        Localizer.Message("renumber_before_export_caption"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel) return false;
                    //mSession.Presentation.getUndoRedoManager().execute(renumberFrom.RenumberCommand(mProjectView, renumberNumber));
                    mSession.Presentation.UndoRedoManager.Execute(mProjectView.GetRenumberPageKindCommand(renumberFrom,
                                                                                                          renumberNumber));
                }
                return true;
            }

            // Look for sections which will not be exported and warn the user.
            // If there are empty sections, issue a warning and ask whether to continue.
            // Return true if there are no empty sections, or the user chose to continue.
            private bool CheckedForEmptySectionsAndAnchoredReferences()
            {
                bool cont = true;
                bool keepWarning = true;
                List<string> anchorErrors = new List<string>();
                ;
                try
                {
                    mSession.Presentation.RootNode.AcceptDepthFirst(
                        delegate(urakawa.core.TreeNode n)
                            {
                                SectionNode s = n as SectionNode;
                                if (s != null && s.Used && s.FirstUsedPhrase == null && keepWarning)
                                {
                                    Dialogs.EmptySection dialog = new Dialogs.EmptySection(s.Label, mSettings); //@fontconfig
                                    cont = cont && dialog.ShowDialog() == DialogResult.OK;
                                    keepWarning = dialog.KeepWarning;
                                    return false;
                                }
                                if (n is EmptyNode)
                                {
                                    EmptyNode eNode = (EmptyNode) n;
                                    if ((eNode.Role_ == EmptyNode.Role.Anchor && eNode.AssociatedNode != null &&
                                         !(eNode.AssociatedNode is PhraseNode)) ||
                                        (eNode.Role_ == EmptyNode.Role.Anchor && !(eNode is PhraseNode)))
                                    {
                                        anchorErrors.Add(string.Format(Localizer.Message("Export_AnchorErrors"),
                                                                       eNode.ParentAs<SectionNode>().Label,
                                                                       eNode.ToString()));
                                    }
                                }
                                return true;
                            },
                        delegate(urakawa.core.TreeNode n) { });
                    if (anchorErrors != null && anchorErrors.Count > 0)
                    {
                        Dialogs.ReportDialog reportDialog = new ReportDialog(Localizer.Message("Report_for_export"),
                                                                             Localizer.Message("Anchor_node_error"),
                                                                             anchorErrors);
                        if (reportDialog.ShowDialog() == DialogResult.OK)
                        {
                        }
                        else return false;
                    }
                    return cont;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(Localizer.Message("Operation_Cancelled") + "\n\n" + ex.ToString());
                        //@Messagecorrected
                    return false;
                }


            }

            #endregion


            private delegate void Status_Delegate(string message);

            public void Status_Safe(string message)
            {
                if (InvokeRequired)
                {
                    Invoke(new Status_Delegate(Status_Safe), message);
                }
                else
                {
                    Status(message);
                }

            }


            /// <summary>
            /// Display a message in the status bar.
            /// </summary>
            public void Status(string message)
            {
                if (IsStatusBarEnabled) mStatusLabel.Text = message;
            }

            /// <summary>
            /// Status bar can be disabled for issues like long operations
            /// </summary>
            public bool IsStatusBarEnabled
            {
                get { return m_IsStatusBarEnabled; }
                set
                {
                    m_IsStatusBarEnabled = value;
                    if (m_IsStatusBarEnabled) ShowSelectionInStatusBar();
                }
            }

            // Update the status bar to say "Ready."
            public void Ready()
            {
                Status(Localizer.Message("ready"));
            }

            // Utility functions




            /// <summary>
            /// Try to open a project from a XUK file.
            /// Actually open it only if a possible current project could be closed properly.
            /// </summary>
            private void CloseAndOpenProject(string path)
            {
                if (DidCloseProject()) OpenProject_Safe(path, null);
            }

            // Try to create a new project with the given title at the given path.
            private void CreateNewProject(string path, string title, bool createTitleSection, string id, int audioChannels, int audioSampleRate)
            {
                try
                {
                    path = Path.GetFullPath(path);
                    // let's see if we can actually write the file that the user chose (bug #1679175)
                    FileStream file = File.Create(path);
                    file.Close();
                    mSession.NewPresentation(path, title, createTitleSection, id, mSettings, audioChannels, audioSampleRate);
                    UpdateMenus();
                }
                catch (Exception e)
                {
                    mProjectView.WriteToLogFile(e.ToString());
                    MessageBox.Show(
                        String.Format(Localizer.Message("cannot_create_file_text"), path, e.Message),
                        Localizer.Message("cannot_create_file_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            private delegate void GotNewPresentationDelegate();

            // A new presentation was loaded or created.
            private void GotNewPresentation()
            {
                if (InvokeRequired)
                {
                    Invoke(new GotNewPresentationDelegate(GotNewPresentation));
                }
                else
                {
                    System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
                    Stopwatch.StartNew();
                    mProjectView.Presentation = mSession.Presentation;
                    UpdateObi();
                    mSession.Presentation.UndoRedoManager.CommandDone +=
                        new EventHandler<urakawa.events.undo.DoneEventArgs>(ObiForm_commandDone);
                    mSession.Presentation.UndoRedoManager.CommandReDone +=
                        new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ObiForm_commandReDone);
                    mSession.Presentation.UndoRedoManager.CommandUnDone +=
                        new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ObiForm_commandUnDone);
                    UpdateCustomClassMenu();
                    mProjectView.Presentation.Changed +=
                        new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_Changed);
                    mProjectView.Presentation.BeforeCommandExecuted +=
                        new EventHandler<urakawa.events.command.CommandEventArgs>(ObiForm_BeforeCommandExecuted);
                        //@singleSection
                    mProjectView.WriteToLogFile("Opened new project: " + mSession.Presentation.Title);
                    if (mSettings.Project_AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();
                    m_CanAutoSave = true; //@singleSection
                    if (mRecordingToolBarForm != null) mRecordingToolBarForm.UpdateForChangeInObi();
                    Status(String.Format(Localizer.Message("created_new_project"), mSession.Presentation.Title));
                    stopWatch.Stop();
                    Console.WriteLine("Time taken to create section and phrase blocks (in milliseconds) " +
                                      stopWatch.ElapsedMilliseconds);
                }
            }


            // Catch problems with initialization and report them.
            private void InitializeObi()
            {
                try
                {
                    InitializeComponent();
                   
                    m_IsStatusBarEnabled = true;
                    mProjectView.ObiForm = this;
                    mProjectView.SelectionChanged += new EventHandler(ProjectView_SelectionChanged);
                    mProjectView.BlocksVisibilityChanged += new EventHandler(ProjectView_BlocksVisibilityChanged);
                        //@singleSection: commented
                    mSession = new Session();
                    mSession.ProjectOpened += new EventHandler(Session_ProjectOpened);
                    mSession.ProjectCreated += new EventHandler(Session_ProjectCreated);
                    mSession.ProjectClosed += new ProjectClosedEventHandler(Session_ProjectClosed);
                    mSession.ProjectSaved += new EventHandler(Session_ProjectSaved);
                    mSourceView = null;
                    InitializeSettings();
                    if (mSettings.Project_SaveObiLocationAndSize)
                    {
                        this.StartPosition = FormStartPosition.Manual;
                        Rectangle screenArea = SystemInformation.WorkingArea;
                        if ((mSettings.ObiLastLocation.X > (screenArea.Width - this.Width / 4) || mSettings.ObiLastLocation.Y > (screenArea.Height - this.Height / 4)) ||
                             ((mSettings.ObiLastLocation.X + this.Width) < (this.Width / 4) || (mSettings.ObiLastLocation.Y + this.Height) < (this.Height / 4)))
                        {

                            mSettings.ObiLastLocation = this.Location;
                        }
                        else
                        {
                            this.Location = mSettings.ObiLastLocation;
                        }
                    }
                    InitializeKeyboardShortcuts(true);
                    InitializeEventHandlers();
                    UpdateMenus();
 
                    // these should be stored in settings
                    mShowTOCViewToolStripMenuItem.Checked = mProjectView.TOCViewVisible = true;
                    mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible = false;
                    mShowTransportBarToolStripMenuItem.Checked = mProjectView.TransportBarVisible = true;
                    mShowStatusBarToolStripMenuItem.Checked = mStatusStrip.Visible = true;
                    mBaseFontSize = mStatusLabel.Font.SizeInPoints;
                    InitializeColorSettings();
                    InitializeAutoSaveTimer();
                    if (!mSettings.Project_EnableFreeDiskSpaceCheck) mSession.EnableFreeDiskSpaceCheck = mSettings.Project_EnableFreeDiskSpaceCheck;
                    if (Directory.Exists(mSettings.Project_PipelineScriptsPath))
                    {
                        mPipelineInfo = new PipelineInterface.PipelineInfo(mSettings.Project_PipelineScriptsPath);
                        PopulatePipelineScriptsInToolsMenu();
                    }
                    else
                    {
                        if (!ExtractPipelineLite())
                            MessageBox.Show(string.Format(Localizer.Message("ObiForm_PipelineNotFound"),
                                                          mSettings.Project_PipelineScriptsPath));
                    }
                    
                    Ready();
                    if (CultureInfo.CurrentCulture.Name.Contains("fr") || CultureInfo.CurrentCulture.Name.Contains("de"))
                    {
                        mHelp_ContentsEnglishMenuItem.Visible = true;
                    }
                   if (mSettings.ObiFont != this.Font.Name)
                    {
                        mProjectView.SetFont(); //@fontconfig
                        mMenuStrip.Font = new Font(mSettings.ObiFont, this.mMenuStrip.Font.Size, FontStyle.Regular);//@fontconfig    
                        mStatusLabel.Font = new Font(mSettings.ObiFont, this.mStatusLabel.Font.Size, FontStyle.Regular);//@fontconfig    
                    }


                    
                    //CheckSystemSupportForMemoryOptimization();
                 }
                catch (Exception e)
                {
                    if (mProjectView != null)
                        mProjectView.WriteToLogFile(e.ToString());
                    string path = Path.Combine(Application.StartupPath, "obi_startup_error.txt");
                    System.IO.StreamWriter tmpErrorLogStream = System.IO.File.CreateText(path);
                    tmpErrorLogStream.WriteLine(e.ToString());
                    tmpErrorLogStream.Close();
                    MessageBox.Show(String.Format(Localizer.Message("init_error_text"), path, e.ToString()),
                                    Localizer.Message("init_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void InitializeAutoSaveTimer()
            {
                mAutoSaveTimer = new Timer();
                mAutoSaveTimer.Enabled = false;
                mAutoSaveTimer.Interval = mSettings.Project_AutoSaveTimeInterval;
                mAutoSaveTimer.Tick += new EventHandler(mAutoSaveTimer_Tick);
            }

            private void mAutoSaveTimer_Tick(object sender, EventArgs e)
            {
                if (!mSettings.Project_AutoSaveTimeIntervalEnabled) return;

                if (!m_CanAutoSave
                    && mSession.CanSave) //@singleSection
                {
                    //keep on checking after interval of 5 seconds if CanAutoSave is true
                    mAutoSaveTimer.Interval = 5000;
                    return;
                }
                else
                {
                    mAutoSaveTimer.Interval = mSettings.Project_AutoSaveTimeInterval;
                }
                m_TotalTimeIntervalSinceLastBackup += mAutoSaveTimer.Interval;

                if (mSession != null && mSession.Presentation != null
                    && mSettings.Project_AutoSaveTimeIntervalEnabled && m_CanAutoSave)
                {
                    //if (mProjectView.TransportBar.CurrentState != Obi.ProjectView.TransportBar.State.Recording)
                    //{
                    m_IsAutoSaveActive = true;
                    if (mSession.CanSave)
                    {
                        SaveToBackup();
                    }
                    if (m_TotalTimeIntervalSinceLastBackup >= 1800000)
                    {
                        CopyToBackup();
                    }
                    m_IsAutoSaveActive = false;
                    //Console.WriteLine("auto save executed ");
                    //}
                    //else
                    //{
                    //mProjectView.TransportBar.AutoSaveOnNextRecordingEnd = true;
                    //}
                }
            }

            public bool CanAutoSave
            {
                set { m_CanAutoSave = value == true ? !mProjectView.TransportBar.IsRecorderActive : value; }
            }

//@singleSection
            public bool IsAutoSaveActive
            {
                get { return m_IsAutoSaveActive; }
            }

            public int SetAutoSaverInterval
            {
                set { if (mAutoSaveTimer != null) mAutoSaveTimer.Interval = value; }
            }

            public void StartAutoSaveTimeInterval()
            {
                if (mSettings.Project_AutoSaveTimeIntervalEnabled)
                {
                    if (mAutoSaveTimer.Enabled) mAutoSaveTimer.Stop();
                    mAutoSaveTimer.Start();
                }
            }

            private bool ExtractPipelineLite()
            {

                string directoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
                string pipelineCabFile = "Pipeline-lite.cab";

                if (File.Exists(Path.Combine(directoryPath, pipelineCabFile))
                    && File.Exists(Path.Combine(directoryPath, "CABARC.EXE"))
                    && File.Exists(Path.Combine(directoryPath, "CABINET.DLL")))
                {
                    try
                    {
                        string pipelineLitePath = Path.Combine(directoryPath, "Pipeline-lite");
                        if (Directory.Exists(pipelineLitePath)) Directory.Delete(pipelineLitePath, true);

                        Process extractProcess = new Process();
                        if (System.Environment.OSVersion.Version.Major >= 6)
                        {
                            extractProcess.StartInfo.Verb = "runas";
                        }

                        extractProcess.StartInfo.WorkingDirectory = directoryPath;
                        extractProcess.StartInfo.FileName = Path.Combine(directoryPath, "CABARC.EXE");
                        extractProcess.StartInfo.Arguments = "-p x " + pipelineCabFile;
                        extractProcess.StartInfo.UseShellExecute = false;
                        extractProcess.StartInfo.CreateNoWindow = true;
                        extractProcess.Start();
                        extractProcess.WaitForExit();

                        if (Directory.Exists(pipelineLitePath) && Directory.GetFiles(pipelineLitePath, "*.*").Length > 0)
                            return true;
                    }
                    catch (System.Exception ex)
                    {
                        mProjectView.WriteToLogFile(ex.ToString());
                        MessageBox.Show(Localizer.Message("ObiFormMsg_PipelineExtractionFail") + "\n\n" + ex.ToString());
                            //@Messagecorrected
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Pipeline-lite could not be installed. Please install by yourself after installation is complete.",
                        "Warning");
                }
                return false;
            }

            private void PopulatePipelineScriptsInToolsMenu()
            {
                ToolStripMenuItem PipelineMenuItem = null;
                foreach (KeyValuePair<string, FileInfo> k in mPipelineInfo.ScriptsInfo)
                {
                    PipelineMenuItem = new ToolStripMenuItem();
                    PipelineMenuItem.Tag = k.Key;
                    PipelineMenuItem.Text = mPipelineInfo.TaskScriptNameToNiceNameMap[k.Key];
                    PipelineMenuItem.AccessibleName = mPipelineInfo.TaskScriptNameToNiceNameMap[k.Key];
                    PipelineMenuItem.Name = "PipelineMenu";
                    PipelineMenuItem.Enabled = mSession.HasProject;
                    mMenuStrip.Items.Add(PipelineMenuItem);
                    mToolsToolStripMenuItem.DropDown.Items.Add(PipelineMenuItem);
                    PipelineMenuItem.Click += new EventHandler(PipelineToolStripItems_Click);
                }
            }

            private bool PipelineMenuItemsEnabled
            {
                set
                {
                    ToolStripItem[] ItemList = mMenuStrip.Items.Find("PipelineMenu", true);
                    if (ItemList != null)
                    {
                        foreach (ToolStripMenuItem m in ItemList) m.Enabled = value;
                    }
                }
            }

            // Open the project at the given path; warn the user on error.
            private void OpenProject_Safe(string path, string progressTitle)
            {

                try
                {

                    if (ProjectUpgrader.IsObi1XProject(path))
                    {
                        if (MessageBox.Show(Localizer.Message("upgrade_obi_to_new_version"),
                                            Localizer.Message("Caption_Information"),
                                            MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            ProjectUpgrader upgrader = new ProjectUpgrader(path, null);
                            ProgressDialog progress = new ProgressDialog(Localizer.Message("upgrading_obi"),
                                                                         delegate(ProgressDialog progress1)
                                                                             {

                                                                                 upgrader.UpgradeProject();
                                                                             });
                            progress.OperationCancelled +=
                                new OperationCancelledHandler(
                                    delegate(object sender, EventArgs e) { upgrader.RequestCancellation = true; });
                            upgrader.ProgressChanged +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            progress.ShowDialog();
                            if (progress.Exception != null) throw progress.Exception;

                            if (upgrader.RequestCancellation) return;
                        }
                        else
                        {
                            return;
                        }
                    }

                    OpenProject(path, progressTitle);
                    if (mProjectView.Presentation != null) ShowLimitedPhrasesShownStatusMessage_Safe();
                }
                catch (Exception e)
                {
                    mProjectView.WriteToLogFile(e.ToString());
                    // if opening failed, no project is open and we don't try to open it again next time.
                    MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (e.Message.StartsWith(Localizer.Message("project_locked").Substring(0, 120)))
                    {
                    System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(path));
                }
                    else
                    {
                        RemoveRecentProject(path);
                        mSettings.LastOpenProject = "";
                    }
                    mSession.Close();
                }
            }

            // Unsafe version of open project
            private void OpenProject(string path, string progressTitle)
            {
                m_IsStatusBarEnabled = true;
                Status_Safe(string.Format( Localizer.Message("opening_project"), path));

                if (string.IsNullOrEmpty(progressTitle))
                    progressTitle = Localizer.Message("OpenProject_progress_dialog_title");
                this.Cursor = Cursors.WaitCursor;
                Obi.Dialogs.ProgressDialog progress = new Obi.Dialogs.ProgressDialog(progressTitle,
                                                                                     delegate()
                                                                                         {

                                                                                             mSession.Open(path);
                                                                                             if (mSession.ErrorsInOpeningProject) mProjectView.ReplacePhrasesWithImproperAudioWithEmptyPhrases((ObiNode) mProjectView.Presentation.RootNode,false);
                                                                                             DeleteExtraBackupFiles(false);
                                                                                             if (mSession.Presentation != null) mSession.Presentation.ConfigurationsImportExport = GetObiConfigurationFileInstance(mSession.Path);

                                                                                         }, mSettings); //@fontconfig
                progress.ShowDialog();
                if (progress.Exception != null)
                {
                    Status(Localizer.Message("error_in_opening")); //todo localize
                    throw progress.Exception;
                }
                AddRecentProject(path);
                // stores primary export path in metadata if it is stored in Obi 1.0 way
                UpdateExportMetadataFromPrimaryExportPath();

                this.Cursor = Cursors.Default;
            }

            private void DeleteExtraBackupFiles(bool allExceptPrimaryBackup)
            {
                try
                {
                    // if backup files are above 50, delete extra files
                    string[] backupFilePaths = Directory.GetFiles(Path.GetDirectoryName(mSession.BackUpPath), "*.obi", SearchOption.AllDirectories);
                    if (backupFilePaths.Length > 50 || 
                        (backupFilePaths.Length > 1 &&  allExceptPrimaryBackup))
                    {
                        List<string> backUpFilesList = new List<string>();
                        for (int i = 0; i < backupFilePaths.Length; i++) backUpFilesList.Add(backupFilePaths[i]);
                        backUpFilesList.Sort();
                        if (backUpFilesList.Contains(Path.GetFullPath(mSession.BackUpPath)))
                            backUpFilesList.Remove(Path.GetFullPath(mSession.BackUpPath));

                        int filesCountToDelete = allExceptPrimaryBackup ? backUpFilesList.Count : 
                            backUpFilesList.Count > 50? backUpFilesList.Count - 50 : 0;
                        for (int i = 0; 
                            i < backUpFilesList.Count && i <= filesCountToDelete; 
                            i++)
                        {
                            Console.WriteLine(backUpFilesList[i]);
                            if (File.Exists(backUpFilesList[i])) File.Delete(backUpFilesList[i]);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                }
            }

            // temporary function to get export path, which was stored in msession.primaryExportPath in Obi 1.0
            // and store it in metadata
            private void UpdateExportMetadataFromPrimaryExportPath()
            {
                if (mSession != null && mProjectView != null
                    &&
                    mProjectView.GetDAISYExportPath(ImportExport.ExportFormat.DAISY3_0,
                                                    Path.GetDirectoryName(mSession.Path)) == null
                    && Path.IsPathRooted(mSession.PrimaryExportPath))
                {
                    mProjectView.SetExportPathMetadata(Obi.ImportExport.ExportFormat.DAISY3_0,
                                                       mSession.PrimaryExportPath,
                                                       Path.GetDirectoryName(mSession.Path));
                }
            }


            // The project was modified.
            private void ProjectHasChanged(int change)
            {
                mSession.PresentationHasChanged(change);
                UpdateObi();
            }

            // Redo
            private void Redo()
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                bool PlayOnSelectionStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                mProjectView.SuspendLayout_All();
                try
                {
                    if (mSession.CanRedo)
                    {
                        CanAutoSave = false; //@singleSection
                        IsStatusBarEnabled = false; //@singleSection
                        mSession.Presentation.UndoRedoManager.Redo();
                    }
                }
                catch (System.Exception ex)
                {
                    mProjectView.WriteToLogFile(ex.ToString());
                    MessageBox.Show(Localizer.Message("ObiFormMsg_RedoFail") + "\n\n" + ex.ToString());
                        //@Messagecorrected
                }
                if (!IsStatusBarEnabled) IsStatusBarEnabled = true;
                CanAutoSave = true; //@singleSection
                mProjectView.ResumeLayout_All();
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
            }

            // Show a new source view window or give focus back to the previously opened one.
            private void ShowSource()
            {
                if (mSession.HasProject)
                {
                    if (mSourceView == null)
                    {
                        if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();

                        mSourceView = new Dialogs.ShowSource(mProjectView);
                        mSourceView.FormClosed +=
                            new FormClosedEventHandler(
                                delegate(object sender, FormClosedEventArgs e) { mSourceView = null; });
                        mSourceView.Show();
                    }
                    else
                    {
                        mSourceView.Focus();
                    }
                    Ready();
                }
            }

            private void ShowRecordingToolBar()
            {

                if (mRecordingToolBarForm == null)
                {
                    //    mRecordingToolBarForm = new Obi.UserControls.RecordingToolBarForm(mProjectView.TransportBar);
                    mRecordingToolBarForm = new Obi.UserControls.RecordingToolBarForm(mProjectView);
                    mRecordingToolBarForm.FormClosed +=
                        new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
                        {
                            mSettings.RecordingToolBarIncrementVal = mRecordingToolBarForm.NetSizeIncrementOfButtons;
                            mSettings.RecordingToolBarLastLocation = mRecordingToolBarForm.Location;
                            
                            if (mPeakMeter != null)
                            {
                                mPeakMeter.TopMost = false;
                            }
                            mRecordingToolBarForm = null;
                            mView_RecordingToolBarMenuItem.Checked = false;

                        });

                    // if selection is in TocView, move it to content view.
                    if (mProjectView.Selection != null && mProjectView.Selection.Control is ProjectView.TOCView)
                        mProjectView.FocusOnContentView();
                    mRecordingToolBarForm.TopMost = true;
                    if (mPeakMeter == null) ShowPeakMeter();
                    if (mPeakMeter != null)
                    {
                        mPeakMeter.TopMost = true;
                    }
                    mRecordingToolBarForm.Show();
                    mRecordingToolBarForm.NetSizeIncrementOfButtons = mSettings.RecordingToolBarIncrementVal;
                    mRecordingToolBarForm.EnlargeButtonSize();
                    mRecordingToolBarForm.Location = mSettings.RecordingToolBarLastLocation;
                    Rectangle screenArea = SystemInformation.WorkingArea;
                    if ((mRecordingToolBarForm.Location.X > screenArea.Width || mRecordingToolBarForm.Location.Y  > screenArea.Height) || 
                         ((mRecordingToolBarForm.Location.X + mRecordingToolBarForm.Width)  < (mRecordingToolBarForm.Width/4) || (mRecordingToolBarForm.Location.Y + mRecordingToolBarForm.Height) <(mRecordingToolBarForm.Height/4)) ||
                         (mRecordingToolBarForm.Location.X == 0 && mRecordingToolBarForm.Location.Y == 0))
                    {
                        mRecordingToolBarForm.Location = new System.Drawing.Point(this.Location.X,
                                                                                  (this.Location.Y + this.Size.Height) -
                                                                                  mRecordingToolBarForm.Size.Height);
                    }
                    if (!mSettings.Project_MinimizeObi)
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }
                    mView_RecordingToolBarMenuItem.Checked = true;
                    mRecordingToolBarForm.Focus();
                }
            }

            private bool m_ShowingPeakMeter = false;
            // Show a new peak meter form or give focus back to the previously opned one
            //private void ShowPeakMeter ()
            //    {
            //    if (mPeakMeter == null)
            //        {
            //        mPeakMeter = new Obi.Audio.PeakMeterForm ();
            //        mPeakMeter.SourceVuMeter = mProjectView.TransportBar.VuMeter;
            //        mPeakMeter.FormClosed += new FormClosedEventHandler ( delegate ( object sender, FormClosedEventArgs e )
            //            {
            //                mPeakMeter = null;
            //                mShowPeakMeterMenuItem.Checked = false;
            //                if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Minimized) this.Size = mSettings.ObiFormSize;
            //            } );
            //        if (this.WindowState != FormWindowState.Minimized)
            //            {
            //            //Make sure the Peak meter is displayed on the right of the Obi form.
            //            if (this.WindowState == FormWindowState.Maximized)
            //                {
            //                System.Drawing.Point newLoc = this.Location;
            //                newLoc.X += SystemInformation.HorizontalResizeBorderThickness;
            //                newLoc.Y += SystemInformation.VerticalResizeBorderThickness;
            //                System.Drawing.Size newSize = this.Size;
            //                newSize.Width -= 2 * SystemInformation.HorizontalResizeBorderThickness;
            //                newSize.Height -= 2 * SystemInformation.VerticalResizeBorderThickness;
            //                this.WindowState = FormWindowState.Normal;   //Commented  to stop resizing

            //                this.Location = newLoc; //Commented  to stop resizing
            //                m_ShowingPeakMeter = true;
            //                this.Size = newSize; //Commented  to stop resizing
            //                 //m_ShowingPeakMeter = false; this is made false in resize event
            //                }
            //          //  this.Width -= mPeakMeter.Width; //Commented to stop resizing

            //            if (this.WindowState == FormWindowState.Normal)
            //            {
            //                //this.Width -= mPeakMeter.Width; //Commented by rohit
            //                mPeakMeter.Left = this.Right;
            //            }
            //            else
            //            {   
            //                this.Width -= mPeakMeter.Width; //Commented to stop resizing
            //               // mPeakMeter.Left = this.Right;
            //                mPeakMeter.Left = this.Right - mPeakMeter.Width;
            //            }

            //            mPeakMeter.Top = this.Top;
            //            // mPeakMeter.Left = this.Right;  //Commented by rohit to stop resizing
            //            mPeakMeter.Height = this.Height;
            //            mPeakMeter.StartPosition = FormStartPosition.Manual;
            //            }
            //        mPeakMeter.Show ();
            //        mShowPeakMeterMenuItem.Checked = true;
            //        }
            //    else
            //        {
            //        this.Width = this.Width + mPeakMeter.Width; //Commented by rohit to stop resizing
            //        mPeakMeter.Close ();
            //        mPeakMeter = null;
            //        mShowPeakMeterMenuItem.Checked = false;
            //        }
            //    this.Ready ();
            //    }
            public void ShowPeakMeter()
            {
                if (mProjectView.IsPeakMeterInsideObiActive)
                {
                    mProjectView.RemovePeakMeterInsideObi();
                    mShowPeakMeterInsideObiMenuItem.Checked = mProjectView.IsPeakMeterInsideObiActive;
                }
                if (mPeakMeter == null)
                {
                    mPeakMeter = new Obi.Audio.PeakMeterForm();
                    if (mSettings.PeakmeterSize.Width == 0 || mSettings.PeakmeterSize.Height == 0 || mSettings.PeakmeterSize.Height < mSettings.GraphicalPeakMeterContolSize.Height || (mSettings.PeakmeterSize.Height-mSettings.GraphicalPeakMeterContolSize.Height<(mPeakMeter.MinimumDiff)))
                    {                       
                        mPeakMeter.PeakMeterInit();
                    }
                    else
                    {
                        mPeakMeter.Size = mSettings.PeakmeterSize;
                        mPeakMeter.GraphicaPeakMeterSizeSet(mSettings);
                        mPeakMeter.flagFirstTimeInit = false;
                    }
                    mPeakMeter.SourceVuMeter = mProjectView.TransportBar.VuMeter;
                    mPeakMeter.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
                                                                            {
                                                                                if (mPeakMeter != null)
                                                                                {
                                                                                    mSettings.PeakmeterSize =
                                                                                        mPeakMeter.Size;
                                                                                    mPeakMeter.GraphicalPeakMeterSaveSettings(mSettings);
                                                                                    mPeakMeter = null;
                                                                                }
                                                                                mShowPeakMeterMenuItem.Checked = false;
                                                                                if ((this.WindowState ==
                                                                                    FormWindowState.Normal ||
                                                                                    this.WindowState ==
                                                                                    FormWindowState.Minimized) && m_NormalAfterMax!=false)
                                                                                {
                                                                                    this.Size = mSettings.ObiFormSize;
                                                                                }
                                                                                if(flagMaxWindow==true)
                                                                                    {
                                                                                       this.WindowState=FormWindowState.Maximized;
                                                                                    }
                                                                                
                                                                            });
                    
                    if (this.WindowState != FormWindowState.Minimized)
                    {
                        flagMaxWindow = false;
                       m_flag = false;
                       
                        //Make sure the Peak meter is displayed on the right of the Obi form.
                        if (this.WindowState == FormWindowState.Maximized)
                        {
                            System.Drawing.Point newLoc = this.Location;
                            newLoc.X += SystemInformation.HorizontalResizeBorderThickness;
                            newLoc.Y += SystemInformation.VerticalResizeBorderThickness;
                            System.Drawing.Size newSize = this.Size;
                            newSize.Width -= 2*SystemInformation.HorizontalResizeBorderThickness;
                            newSize.Height -= 2*SystemInformation.VerticalResizeBorderThickness;
                            m_flag = true;
                            this.WindowState = FormWindowState.Normal;
                            m_NormalAfterMax = true;
                            this.Location = newLoc;
                            m_ShowingPeakMeter = true;
                            this.Size = newSize;
                            flagMaxWindow = true;
                            //m_ShowingPeakMeter = false; this is made false in resize event
                        }
                        if (flagMaxWindow == true)
                        {
                            this.Width -= mPeakMeter.Width;
                            mPeakMeter.Left = this.Right;
                            //  this.WindowState=FormWindowState.Maximized;
                        }
                        else
                        {
                            //mPeakMeter.Left = this.Right - mPeakMeter.Width;
                            mPeakMeter.Left = this.Right;
                        }

                      

                        mPeakMeter.Top = this.Top;
                        //  mPeakMeter.Left = this.Right;
                       // mPeakMeter.Height = this.Height;
                       // m_DiffPeakMeterGraphicalPeakMeter = mPeakMeter.Height - mPeakMeter.PeakMeterHeight;//peakmeter ref
                        mPeakMeter.StartPosition = FormStartPosition.Manual;
                    }
                    mPeakMeter.Show();
                    mShowPeakMeterMenuItem.Checked = true;
                    this.Activate();
                        m_tempHeightMin = mPeakMeter.Height;
                        m_flagForPeakMeterHeight = true;
                    
                }
                else
                {
                    if (this.WindowState != FormWindowState.Normal)
                    {
                        this.Width = this.Width + mPeakMeter.Width;
                    }
                    mPeakMeter.Close();
                    mPeakMeter = null;
                    mShowPeakMeterMenuItem.Checked = false;
                }
                this.Ready();
            }

            public void ShowPeakMeterInsideObi(bool IsChecked)
            {
                if (IsChecked)
                {
                    if (mPeakMeter != null)
                    {
                        mPeakMeter.Close();
                    }

                    mProjectView.ShowPeakMeterInsideObi(IsChecked);
                    mShowPeakMeterInsideObiMenuItem.Checked = true;
                }
                else
                {
                    if (mProjectView.IsPeakMeterInsideObiActive)
                    {
                        ShowPeakMeter();
                        mShowPeakMeterMenuItem.Checked = true;
                    }
                }
            }

            // Undo
            public void Undo()
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                bool PlayOnSelectionStatus = mProjectView.TransportBar.SelectionChangedPlaybackEnabled;
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = false;
                mProjectView.SuspendLayout_All();
                try
                {
                    if (mSession.CanUndo && !(mProjectView.Selection is TextSelection))
                        //&& (!(mProjectView.Selection is TextSelection)    ||   mProjectView.Selection.Control is ProjectView.ContentView))//@singleSection: allow undo for text selection in content view, no complexity like toc view there
                    {
                        //  if (mProjectView.Selection is TextSelection) mProjectView.SelectedSectionNode = (SectionNode) mProjectView.Selection.Node;
                        CanAutoSave = false; //@singleSection
                        IsStatusBarEnabled = false; //@singleSection
                        mSession.Presentation.UndoRedoManager.Undo();
                    }
                }
                catch (System.Exception ex)
                {
                    mProjectView.WriteToLogFile(ex.ToString());
                    MessageBox.Show(Localizer.Message("ObiFormMsg_UndoFail") + "\n\n" + ex.ToString());
                        //@Messagecorrected
                }
                if (!IsStatusBarEnabled) IsStatusBarEnabled = true; //@singleSection
                CanAutoSave = true; //@singleSection
                mProjectView.ResumeLayout_All();
                mProjectView.TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
            }

            /// <summary>
            /// Show the current selection in the status bar.
            /// </summary>
            private void ShowSelectionInStatusBar()
            {
                string strRecordingInfo = mProjectView.TransportBar.CurrentState ==
                                          Obi.ProjectView.TransportBar.State.Monitoring
                                              ? Localizer.Message("monitoring_short") + "..."
                                              : mProjectView.TransportBar.IsRecorderActive &&
                                                mProjectView.TransportBar.RecordingPhrase != null
                                                    ? string.Format(Localizer.Message("StatusBar_RecordingInPhrase"),
                                                                    (mProjectView.Selection != null &&
                                                                     mProjectView.Selection.Node !=
                                                                     mProjectView.TransportBar.RecordingPhrase
                                                                         ? mProjectView.TransportBar.RecordingPhrase.
                                                                               ToString() + ".."
                                                                         : ""))
                                                    : "";
                //if (IsStatusBarEnabled) Status ( mProjectView.Selection != null ? mProjectView.Selection.ToString () : Localizer.Message ( "StatusBar_NothingSelected" ) + mProjectView.TransportBar.RecordingPhraseToString );
                string limitedBlocksShownMsg = IsStatusBarEnabled && mProjectView.Selection != null &&
                                               mProjectView.IsLimitedPhraseBlocksCreatedAfterCommand()
                                                   ? string.Format(Localizer.Message("StatusBar_LimitedPhrasesShown"),
                                                                   " ")
                                                   : "";
                string strFineNavigation = mProjectView.TransportBar.FineNavigationStatusMsg;
                if (IsStatusBarEnabled)
                    Status(strFineNavigation + strRecordingInfo +
                           (mProjectView.Selection != null
                                ? mProjectView.Selection.ToString()
                                : Localizer.Message("StatusBar_NothingSelected")) + limitedBlocksShownMsg);
 
            }

            // Update all of Obi.
            private void UpdateObi()
            {
                UpdateTitleAndStatusBar();
                UpdateMenus();
            }

            // Update all menu items.
            private void UpdateMenus()
            {
                UpdateFileMenu();
                UpdateEditMenu();
                UpdateViewMenu();
                UpdateSectionsMenu();
                UpdatePhrasesMenu();
                UpdateTransportMenu();
                UpdateToolsMenu();
                UpdateHelpMenu();
                mProjectView.UpdateContextMenus();
            }

            private delegate void UpdateTitleAndStatusBarDelegate();

            // Update the title and status bars to show the name of the project, and if it has unsaved changes
            private void UpdateTitleAndStatusBar()
            {
                if (InvokeRequired)
                {
                    Invoke(new UpdateTitleAndStatusBarDelegate(UpdateTitleAndStatusBar));
                }
                else
                {
                    UpdateTitle();
                }
            }

            //Also used to update Title during profile change 
            public void UpdateTitle()
            {
                string tempPath = mSession.Path;
                if (tempPath != null && tempPath.Length > 90)
                {
                    string[] tempPathSplit = mSession.Path.Split('\\');
                    Console.WriteLine(tempPathSplit[0]);
                    if (tempPathSplit.Length > 3)
                    {
                        if (tempPathSplit[tempPathSplit.Length - 2].Length < 70)
                        {
                            tempPath = tempPathSplit[0] + tempPathSplit[1] + "\\.....\\" + tempPathSplit[tempPathSplit.Length - 2] + "\\" + tempPathSplit[tempPathSplit.Length - 1];
                        }
                        else
                        {
                            tempPath = tempPathSplit[0] + tempPathSplit[1] + "\\.....\\" + tempPathSplit[tempPathSplit.Length - 2] + "\\.. .obi";
                        }
                    }
                }
                if (mSettings != null)
                {
                    string[] str = mSettings.SettingsNameForManipulation.Split(new string[] { "   " }, StringSplitOptions.None);
                    string tempSettingsName = str[0];
                    if (str.Length > 1)
                    {
                        tempSettingsName = str[1] + " " + str[0];
                    }
                    if (!mSettings.Project_ReadOnlyMode)
                    {
                        Text = mSession.HasProject
                                       ? String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                                                       (mSession.CanSave ? "*" : ""), tempPath, Localizer.Message("obi"), tempSettingsName)
                                       : Localizer.Message("obi");
                    }
                    else
                    {
                        Text = mSession.HasProject
                                       ? String.Format(Localizer.Message("title_bar_ReadOnly"), mSession.Presentation.Title,
                                                       (mSession.CanSave ? "*" : ""), tempPath, Localizer.Message("obi"), Localizer.Message("ReadOnlyString"), tempSettingsName)
                                       : Localizer.Message("obi");
                    }

                }
            }


            /// <summary>
            /// Show the state of the transport bar in the status bar.
            /// </summary>
            private void TransportBar_StateChanged(object sender, EventArgs e)
            {
                TransportBar_StateChanged();
            }

            private delegate void TransportBar_StateChanged_Delegate();

            private void TransportBar_StateChanged()
            {

                if (this.InvokeRequired)
                {
                    this.Invoke(new TransportBar_StateChanged_Delegate(TransportBar_StateChanged));
                    return;
                }
                CanAutoSave = !mProjectView.TransportBar.IsRecorderActive;
                string additionalTransportbarOperationInfo = mProjectView.TransportBar.IsPlayerActive &&
                                                             mProjectView.TransportBar.PlaybackPhrase != null &&
                                                             mProjectView.TransportBar.PlaybackPhrase.IsRooted
                                                                 ? (mProjectView.Selection != null &&
                                                                    mProjectView.Selection is AudioSelection &&
                                                                    ((AudioSelection) mProjectView.Selection).AudioRange !=
                                                                    null &&
                                                                    !((AudioSelection) mProjectView.Selection).
                                                                         AudioRange.HasCursor
                                                                        ? mProjectView.Selection.ToString()
                                                                        : mProjectView.TransportBar.PlaybackPhrase.
                                                                              ToString()) + "--" +
                                                                   mProjectView.TransportBar.PlaybackPhrase.ParentAs
                                                                       <SectionNode>().Label.Replace("\n",string.Empty)
                                                                 : (mProjectView.TransportBar.IsRecorderActive &&
                                                                    mProjectView.TransportBar.RecordingPhrase != null
                                                                        ? mProjectView.TransportBar.RecordingPhrase.
                                                                              ToString()
                                                                        : ""); 
                Status(mProjectView.TransportBar.FineNavigationStatusMsg +
                       Localizer.Message(mProjectView.TransportBar.CurrentState.ToString()) + " " +
                       additionalTransportbarOperationInfo);
                UpdatePhrasesMenu();
                UpdateTransportMenu();
                UpdateEditMenu();
                UpdateToolsMenu();
                mProjectView.UpdateContextMenus();
            }

            private void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
            {
                if (mProjectView.Selection != null && mProjectView.Selection.Node.PhraseChildCount < 1)
                {
                }
                else if (mProjectView.TransportBar.PlaybackPhrase != null)
                    Status(String.Format(Localizer.Message("playback_rate"),
                                         mProjectView.TransportBar.CurrentPlaylist.PlaybackRate,
                                         mProjectView.TransportBar.PlaybackPhrase.Parent));
            }







            /// <summary>
            /// Handle errors when closing a project.
            /// </summary>
            /// <param name="message">The error message.</param>
            private void ReportDeleteError(string path, string message)
            {
                MessageBox.Show(String.Format(Localizer.Message("report_delete_error"), path, message));
            }

            /// <summary>
            /// Save the settings when closing.
            /// </summary>
            /// <remarks>Warn when closing while playing?</remarks>
            private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (ObiFontName != null) //@fontconfig
                {
                    mSettings.ObiFont = ObiFontName; //@fontconfig
                }
                
                if (mProjectView != null && mProjectView.TransportBar.IsActive)
                {   
                    mProjectView.TransportBar.Stop();
                    if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@MonitorContinuously
                }
                if ( mProjectView != null && mSettings != null && mSettings.Project_SaveTOCViewWidth)
                {
                    mSettings.TOCViewWidth = mProjectView.TOCViewWidth;
                }
                if (mProjectView.IsAudioProcessingPerformed)
                {
                    if (MessageBox.Show(Localizer.Message("CleanUpAfterAudioProcessing"), Localizer.Message("Caption_Information"), 
                        MessageBoxButtons.YesNo,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
                    {
                        CleanProject(false);
                    }
                    mProjectView.IsAudioProcessingPerformed = false;
                }
               if (DidCloseProject())
                {
                    if (!PreventSettingsUpdateOnClosing)
                    {
                        if (mSettings.Project_SaveObiLocationAndSize) mSettings.ObiLastLocation = this.Location;
                        mSettings.ShowGraphicalPeakMeterAtStartup = mPeakMeter != null;
                        mSettings.ShowGraphicalPeakMeterInsideObiAtStartup = mProjectView.IsPeakMeterInsideObiActive;
                        if (mRecordingToolBarForm != null)
                        {
                            mSettings.RecordingToolBarIncrementVal = mRecordingToolBarForm.NetSizeIncrementOfButtons;
                            mSettings.RecordingToolBarLastLocation = mRecordingToolBarForm.Location;
                        }

                        mSettings.Project_RecordingToolbarOpenInPreviousSession = (mRecordingToolBarForm != null && mRecordingToolBarForm.IsHandleCreated);
                    }
                    
                    try
                    {
                        mSettings.SaveSettings();
                        if (m_KeyboardShortcuts != null) m_KeyboardShortcuts.SaveSettings();
                    }
                    catch (Exception x)
                    {
                        mProjectView.WriteToLogFile(x.ToString());
                        MessageBox.Show(String.Format(Localizer.Message("save_settings_error_text"), x.Message),
                                        Localizer.Message("save_settings_error_caption"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    mProjectView.SelectionChanged -= new EventHandler(ProjectView_SelectionChanged);
                    mProjectView.BlocksVisibilityChanged -= new EventHandler(ProjectView_BlocksVisibilityChanged);
                        //@singleSection: commented
                    mProjectView.RenameLogFileAfterSession();
                    Application.Exit();

                }
                else
                {
                    e.Cancel = true;
                    Ready();
                }
            }





            /// <summary>
            /// Initialize the application settings: get the settings from the saved user settings or the system
            /// and add the list of recent projects (at least those that actually exist) to the recent project menu.
            /// </summary>
            private void InitializeSettings()
            {
                mSettings = Settings.GetSettings();
                m_Settings_Permanent = Settings_Permanent.GetSettings();
                for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
                {
                    if (!AddRecentProjectsItem((string) mSettings.RecentProjects[i]))
                        mSettings.RecentProjects.RemoveAt(i);
                }
                try
                {
                    mProjectView.TransportBar.AudioPlayer.SetOutputDevice(this, mSettings.Audio_LastOutputDevice);
                }
                catch (Exception e)
                {
                    mProjectView.WriteToLogFile(e.ToString());
                    m_OutputDevicefound = false;
                    MessageBox.Show(Localizer.Message("no_output_device_text"),
                                    Localizer.Message("no_output_device_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                try
                {
                    mProjectView.TransportBar.Recorder.SetInputDevice(mSettings.Audio_LastInputDevice);
                }
                catch (Exception ex)
                {
                    mProjectView.WriteToLogFile(ex.ToString());
                    m_InputDeviceFound = false;
                    MessageBox.Show(Localizer.Message("no_input_device_text"),
                                    Localizer.Message("no_input_device_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                if (!m_InputDeviceFound && !m_OutputDevicefound)
                {
                    MessageBox.Show(Localizer.Message("no_device_found_text"),
                                    Localizer.Message("no_device_found_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
                if (mSettings.ObiFormSize.Width == 0 || mSettings.ObiFormSize.Height == 0 || !mSettings.Project_SaveObiLocationAndSize)
                {
                    mSettings.ObiFormSize = Size;
                }
                else
                {
                    Size = mSettings.ObiFormSize;
                }
                // Synchronize views
                SynchronizeViews = mSettings.SynchronizeViews;
                WrapStripContents = mSettings.WrapStripContents;
                // Transport bar settings
                AllowOverwrite = mSettings.Audio_AllowOverwrite;
                mPlayOnNavigateToolStripMenuItem.Checked = mSettings.PlayOnNavigate;
                mFineNavigationToolStripMenuItem.Checked = mProjectView.TransportBar.FineNavigationModeForPhrase;
                // Colors
                mSettings.ColorSettings.CreateBrushesAndPens();
                mProjectView.TransportBar.UpdateButtons();
            }

            internal void InitializeKeyboardShortcuts(bool isFirstTime)
            {
                try
                {
                    m_KeyboardShortcuts = KeyboardShortcuts_Settings.GetKeyboardShortcuts_Settings();
                }
                catch (System.Exception ex)
                {
                    mProjectView.WriteToLogFile(ex.ToString());
                //MessageBox.Show(
                //    Localizer.Message("KeyboardShortcuts_ErrorInLoadingConfiguredKeys") + "\n" + ex.ToString(),
                //    Localizer.Message("Caption_Error"));
                MessageBox.Show(Localizer.Message("KeyboardShortcuts_LoadingDefaults"),Localizer.Message("Caption_Information"));
                m_KeyboardShortcuts = KeyboardShortcuts_Settings.GetDefaultKeyboardShortcuts_Settings();
                }
                mProjectView.InitializeShortcutKeys();

                foreach (ToolStripItem m in mMenuStrip.Items)
                {
                    if (m is ToolStripMenuItem)
                    {
                        AssignMenuShortcuts((ToolStripMenuItem) m, isFirstTime);

                    }
                }
                mProjectView.InitializeShortcutToContentViewContextMenu();
            }

            internal void LoadDefaultKeyboardShortcuts()
            {
                m_KeyboardShortcuts = null;
                m_KeyboardShortcuts = KeyboardShortcuts_Settings.GetDefaultKeyboardShortcuts_Settings();
                mProjectView.InitializeShortcutKeys();

                foreach (ToolStripItem m in mMenuStrip.Items)
                {
                    if (m is ToolStripMenuItem)
                    {
                        AssignMenuShortcuts((ToolStripMenuItem) m, false);
                    }
                }
                mProjectView.InitializeShortcutToContentViewContextMenu();
            }


            private void AssignMenuShortcuts(ToolStripMenuItem m, bool isFirstTime)
            {

                if (m.HasDropDownItems)
                {
                    foreach (ToolStripItem n in m.DropDownItems)
                    {
                        if (n is ToolStripMenuItem) AssignMenuShortcuts((ToolStripMenuItem) n, isFirstTime);

                    }
                }
                else
                {
                    //if (m != mFocusOnStripsViewToolStripMenuItem && m != mFocusOnTOCViewToolStripMenuItem)
                    {
                        string accessibleString = m.Text.Replace("&", "");
                        if (isFirstTime && !string.IsNullOrEmpty(m.Name) && m.Name != "PipelineMenu")
                            KeyboardShortcuts_Settings.AddDefaultMenuShortcut(m.Name, m.ShortcutKeys);
                        if (KeyboardShortcuts.MenuNameDictionary.ContainsKey(m.Name)
                            &&
                            (KeyboardShortcuts.MenuNameDictionary[m.Name].Value != Keys.None ||
                             (KeyboardShortcuts_Settings.MenuNameDefaultShortcutDictionary != null &&
                              KeyboardShortcuts_Settings.MenuNameDefaultShortcutDictionary.ContainsKey(m.Name) &&
                              KeyboardShortcuts_Settings.MenuNameDefaultShortcutDictionary[m.Name].Value != Keys.None))
                            //skip if both dictionary has none shortcut
                            && m.Name != "PipelineMenu")
                        {
                            m.ShortcutKeys = KeyboardShortcuts.MenuNameDictionary[m.Name].Value;
                            KeyboardShortcuts.AddMenuShortcut(m.Name, m.Text.Replace("&", ""), m.ShortcutKeys);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(m.Name) && m.Name != "PipelineMenu")
                            {
                                KeyboardShortcuts.AddMenuShortcut(m.Name, m.Text.Replace("&", ""), m.ShortcutKeys);
                            }
                            else
                            {
                                //MessageBox.Show(m.Text);
                            }
                        }
                        if (m.ShortcutKeys != Keys.None)
                        {
                            string shortcutString = m.ShortcutKeys.ToString();

                            accessibleString = accessibleString + " " +
                                               ObiForm.RefineKeyboardShortcutStringForAccessibleName(shortcutString);
                        }
                        m.AccessibleName = accessibleString;
                    }
                }
            }

            public static string RefineKeyboardShortcutStringForAccessibleName(string shortcutString)
            {
                string[] arrayStrings = shortcutString.Split(',');

                if (arrayStrings != null && arrayStrings.Length > 0)
                {
                    shortcutString = "";
                    for (int i = arrayStrings.Length - 1; i >= 0; --i)
                    {
                        shortcutString = shortcutString + arrayStrings[i] + (i > 0 ? "+" : "");
                    }
                }

                return shortcutString;

            }


            // Various utility functions


            private void InitializeColorSettings()
            {
                Microsoft.Win32.SystemEvents.UserPreferenceChanged
                    += new Microsoft.Win32.UserPreferenceChangedEventHandler(
                        delegate(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) { UpdateColors(); });
                UpdateColors();
            }

            public void UpdateColors()
            {
                UpdateZoomFactor();
                mProjectView.ColorSettings = SystemInformation.HighContrast
                                                 ? mSettings.ColorSettingsHC
                                                 : mSettings.ColorSettings;
            }


            /// <summary>
            /// Remove a project from the recent projects list.
            /// This is required when import fails halfway through, or when a project is no longer available.
            /// Also unset the last open project path if it was pointing to this project path.
            /// </summary>
            private void RemoveRecentProject(String path)
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    mFile_RecentProjectMenuItem.DropDownItems.RemoveAt(i);
                    if (mSettings.LastOpenProject == path) mSettings.LastOpenProject = "";
                }
            }






            /// <summary>
            /// Check if a directory is empty or not; ask the user to confirm
            /// that they mean this directory even though it is not empty.
            /// </summary>
            public static bool CheckEmpty(string path, bool checkEmpty,bool showMessagebox = true)
            {
                if (checkEmpty &&
                    (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0))
                {
                    DialogResult result = DialogResult.Yes;
                    if (showMessagebox)
                    {
                        result = MessageBox.Show(
                            String.Format(Localizer.Message("really_use_directory_text"), path),
                            Localizer.Message("really_use_directory_caption"),
                            // MessageBoxButtons.YesNoCancel,
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            if (Path.GetFullPath(path) != Path.GetPathRoot(path))
                            {
                                foreach (string f in Directory.GetFiles(path)) File.Delete(f);
                                foreach (string d in Directory.GetDirectories(path)) Directory.Delete(d, true);
                            }
                            else
                                MessageBox.Show(Localizer.Message("CannotDeleteAtRoot"),
                                                Localizer.Message("Caption_Error"));
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(string.Format(Localizer.Message("cannot_empty_directory"), path, e.Message),
                                            Localizer.Message("cannot_empty_directory_caption"),
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    //  return result != DialogResult.Cancel;
                    return true;                                        
                }
                else
                {
                    return true; // the directory was empty or we didn't need to check
                }
            }

            /// <summary>
            /// Ask the user whether she wants to create a directory,
            /// and try to create it if she does.
            /// </summary>
            /// <param name="path">Path to the non-existing directory.</param>
            /// <returns>True if the directory was created.</returns>
            private static bool DidCreateDirectory(string path, bool alwaysCreate)
            {
                if (alwaysCreate || MessageBox.Show(
                    String.Format(Localizer.Message("create_directory_text"), path),
                    Localizer.Message("create_directory_caption"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                        return true; // did create the directory
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(
                            String.Format(Localizer.Message("cannot_create_directory_text"), path, e.Message),
                            Localizer.Message("cannot_create_directory_caption"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false; // couldn't create the directory
                    }
                }
                else
                {
                    return false; // didn't want to create the directory
                }
            }



            private void mShowPeakMeterMenuItem_Click(object sender, EventArgs e)
            {
                ShowPeakMeter();
            }

            private void mFindNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.FindNextInText();
            }

            private void mFindPreviousToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.FindPreviousInText();
            }

            private void mAllowOverwriteToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                AllowOverwrite = mAllowOverwriteToolStripMenuItem.Checked;
            }

            private void mPlayOnNavigateToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
            {
                if (mSettings.PlayOnNavigate != mPlayOnNavigateToolStripMenuItem.Checked)
                {
                    mSettings.PlayOnNavigate = mPlayOnNavigateToolStripMenuItem.Checked;
                }
            }

            private void mNextTODOPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SelectNextTODOPhrase();
            }

            private void mPreviousTODOPhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SelectPreviousTODOPhrase();
            }


            // View > Zoom in (Ctrl+Alt++)
            private void mView_ZoomInMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();
                mView_ZoomInMenuItem.Enabled = false;
                ZoomFactor = ZoomFactor*ZOOM_FACTOR_INCREMENT;
                mView_ZoomInMenuItem.Enabled = mSession.HasProject;
            }

            // View > Zoom out (Ctrl+Alt+-)
            private void mView_ZoomOutMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();

                mView_ZoomOutMenuItem.Enabled = false;
                ZoomFactor = ZoomFactor/ZOOM_FACTOR_INCREMENT;
                if (mProjectView.TransportBar.IsRecorderActive &&
                    mProjectView.IsLimitedPhraseBlocksCreatedAfterCommand())
                {
                    string selectionString = mProjectView.Selection != null
                                                 ? mProjectView.Selection.Node.ToString()
                                                 : "";
                    Status(string.Format(Localizer.Message("StatusBar_LimitedPhrasesShown"), selectionString));
                }
                mView_ZoomOutMenuItem.Enabled = mSession.HasProject;
            }

            // View > Normal size (Ctrl+Alt+0)
            private void mView_NormalSizeMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();
                mView_NormalSizeMenuItem.Enabled = false;
                float previousZoomFactor = ZoomFactor;
                if (previousZoomFactor != 1.0f)
                {
                    ZoomFactor = 1.0f;
                }
                mView_NormalSizeMenuItem.Enabled = mSession.HasProject;

                if (previousZoomFactor > 1.0f && mProjectView.TransportBar.IsRecorderActive &&
                    mProjectView.IsLimitedPhraseBlocksCreatedAfterCommand())
                {
                    string selectionString = mProjectView.Selection != null
                                                 ? mProjectView.Selection.Node.ToString()
                                                 : "";
                    Status(string.Format(Localizer.Message("StatusBar_LimitedPhrasesShown"), selectionString));
                }
            }

            // View > Project properties (Alt+Enter)
            private void mView_ProjectPropertiesMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ShowProjectPropertiesDialog();
            }

            // View > Phrase properties (Alt+Enter)
            private void mView_PhrasePropertiesMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ShowPhrasePropertiesDialog(false);
            }

            // View > Section properties (Alt+Enter)
            private void mView_SectionPropertiesMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ShowSectionPropertiesDialog();
            }

            private void mCropAudiotoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.CropPhrase();
            }

            private void mView_AudioZoomInMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();
                if (!CheckAndAlertForMemoryUsage()) return;

                mView_AudioZoomInMenuItem.Enabled = false;
                if (!mProjectView.IsZoomWaveformActive)
                {
                    ProjectView.Strip strip = mProjectView.StripForSelection;
                    if (strip == null)
                    {
                        if (mProjectView.ActiveStrip != null)
                        {
                            strip = mProjectView.ActiveStrip;
                        }
                        else
                        {
                            AudioScale *= AUDIO_SCALE_INCREMENT;
                        }
                    }
                    if (strip.AudioScale < 0.1f)
                    {
                        strip.AudioScale *= AUDIO_SCALE_INCREMENT;
                        if (mSettings.Audio_SaveAudioZoom)
                        {
                            mSettings.Audio_AudioScale = strip.AudioScale;
                        }                   
                    }
                }
                else
                {
                    mProjectView.ZoomPanelZoomIn();
                }
                mView_AudioZoomInMenuItem.Enabled = mSession.HasProject;
            }

            private void mView_AudioZoomOutMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();
                if (!CheckAndAlertForMemoryUsage()) return;

                mView_AudioZoomOutMenuItem.Enabled = false;
                if (!mProjectView.IsZoomWaveformActive)
                {
                    ProjectView.Strip strip = mProjectView.StripForSelection;
                    if (strip == null)
                    {
                        if (mProjectView.ActiveStrip != null)
                        {
                            strip = mProjectView.ActiveStrip;
                        }
                        else
                        {
                            AudioScale /= AUDIO_SCALE_INCREMENT;
                        }
                    }
                    if (strip.AudioScale > 0.002f)
                    {
                        strip.AudioScale /= AUDIO_SCALE_INCREMENT;
                        if (mSettings.Audio_SaveAudioZoom)
                        {
                            mSettings.Audio_AudioScale = strip.AudioScale;
                        }            
                    }
                }
                else
                {
                    mProjectView.ZoomPanelZoomOut();
                }
                mView_AudioZoomOutMenuItem.Enabled = mSession.HasProject;
            }

            //@singleSection
            /// <summary>
            /// Check if RAM is near over flow and warns the user
            /// </summary>
            /// <returns></returns>
            private bool CheckAndAlertForMemoryUsage()
            {
                if (Settings == null || !Settings.Project_OptimizeMemory) return true;
                System.Diagnostics.PerformanceCounter ramPerformanceCounter =
                    new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
                if (ramPerformanceCounter.NextValue() < 100)
                {
                    if (MessageBox.Show(Localizer.Message("PerformanceCheck_RAMNearOverflowMsg"),
                                        Localizer.Message("Caption_Warning"),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) ==
                        DialogResult.No)
                    {
                        ramPerformanceCounter.Close();
                        return false;
                    }
                }
                ramPerformanceCounter.Close();
                return true;
            }


            private string chooseDaisyType(string toolStripText)
            {
                string exportDaisy202Path = mProjectView.GetDAISYExportPath(Obi.ImportExport.ExportFormat.DAISY2_02,
                                                                            Path.GetDirectoryName(mSession.Path));
                string exportDaisy3Path = mProjectView.GetDAISYExportPath(Obi.ImportExport.ExportFormat.DAISY3_0,
                                                                          Path.GetDirectoryName(mSession.Path));
                string newDirPath = null;
                Dialogs.chooseDaisy3orDaisy202 rdfrm = new Dialogs.chooseDaisy3orDaisy202(this.mSettings);
                rdfrm.RestrictToSingleDAISYChoice = true; 
                if (toolStripText == "DTBAudioEncoder" || toolStripText == "FilesetRenamer")
                {
                    if (rdfrm.ShowDialog() == DialogResult.OK)
                    {
                        if (rdfrm.chooseOption == Obi.ImportExport.ExportFormat.DAISY3_0)
                        {
                            if (exportDaisy3Path != null)
                            {
                                newDirPath = Path.Combine(exportDaisy3Path, "package.opf");
                            }
                            else
                            {
                                newDirPath = "";
                            }
                        }

                        if (rdfrm.chooseOption == Obi.ImportExport.ExportFormat.DAISY2_02)
                        {
                            if (exportDaisy202Path != null)
                            {
                                newDirPath = Path.Combine(exportDaisy202Path, "ncc.html");
                            }
                            else
                            {
                                newDirPath = "";
                            }
                        }
                    }
                    else
                        return null;
                }
                else
                {
                    if (toolStripText == "Daisy202DTBValidator")
                    {
                        if (exportDaisy202Path != null)
                            newDirPath = Path.Combine(exportDaisy202Path, "ncc.html");
                        else
                            newDirPath = "";
                    }
                    if (toolStripText == "Z3986DTBValidator")
                    {
                        if (exportDaisy3Path != null)
                            newDirPath = Path.Combine(exportDaisy3Path, "package.opf");
                        else
                            newDirPath = "";
                    }
                }

                return newDirPath;
            }

            private void mView_ResetAudioSizeMenuItem_Click(object sender, EventArgs e)
            {
                if (!CheckAndAlertForMemoryUsage()) return;
                if (!mProjectView.IsZoomWaveformActive)
                {
                    //AudioScale = AudioScale;
                    AudioScale = 0.01f;
                }
                else
                {
                    mProjectView.ZoomPanelReset();
                }
            }

            private void PipelineToolStripItems_Click(object sender, EventArgs e)
            {
                string exportFilePath = null;
                mProjectView.TransportBar.Enabled = false;
                ToolStripMenuItem clickedItem = ((ToolStripMenuItem) sender);
                if (clickedItem.Tag != null && clickedItem.Tag is string)
                {
                    //exportFilePath = chooseDaisyType ( ((ToolStripMenuItem)sender).Text );
                    exportFilePath = chooseDaisyType(((string) clickedItem.Tag));
                }
                if (exportFilePath == null)
                {
                    mProjectView.TransportBar.Enabled = true;
                    return;
                }


                try
                {
                    //PipelineInterface.PipelineInterfaceForm pipeline = new PipelineInterface.PipelineInterfaceForm(mPipelineInfo.ScriptsInfo[((ToolStripMenuItem)sender).Text].FullName, 
                    PipelineInterface.PipelineInterfaceForm pipeline =
                        new PipelineInterface.PipelineInterfaceForm(
                            mPipelineInfo.ScriptsInfo[(string) clickedItem.Tag].FullName,
                            exportFilePath,
                            Directory.GetParent(mSession.Path).FullName,mSettings.ObiFont);
                    ProgressDialog progress = new ProgressDialog(((ToolStripMenuItem) sender).Text,
                                                                 delegate() { pipeline.RunScript(); }, mSettings); //@fontconfig

                    if (pipeline.ShowDialog() == DialogResult.OK) progress.Show();
                    if (progress.Exception != null) throw progress.Exception;
                }
                catch (Exception x)
                {
                    mProjectView.WriteToLogFile(x.ToString());
                    MessageBox.Show(string.Format(Localizer.Message("dtb_encode_error"), x.Message),
                                    Localizer.Message("dtb_encode_error_caption"),
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                mProjectView.TransportBar.Enabled = true;
            }

            //@singleSection
            private void Progress_Changed(object sender, System.ComponentModel.ProgressChangedEventArgs e)
            {
                if (!mHasProgressBar
                    ||
                    (mHasProgressBar &&
                     ((string) mStatusProgressBar.Tag) == ProjectView.ProjectView.ProgressBar_Waveform))
                {
                    if (mHasProgressBar) BackgroundOperation_Done();
                    ShowProgressBar();
                    mStatusProgressBar.Style = ProgressBarStyle.Continuous;
                    mStatusProgressBar.Minimum = 0;
                    mStatusProgressBar.Maximum = 100;
                    mStatusProgressBar.Tag = "";
                }

                if (mHasProgressBar && e.ProgressPercentage != mStatusProgressBar.Value)
                {
                    if (mStatusProgressBar.Maximum != 100) mStatusProgressBar.Maximum = 100;
                    mStatusProgressBar.Value = e.ProgressPercentage;
                    if (mStatusProgressBar.Value == 100) BackgroundOperation_Done(); //closes progressbar
                }
                Console.WriteLine("obi form progressbar val " + e.ProgressPercentage);
            }

            private bool mHasProgressBar;

            private void ShowProgressBar()
            {
                if (!mHasProgressBar)
                {
                    mStatusProgressBar.Minimum = 0;
                    mStatusProgressBar.Maximum = 0;
                    mStatusProgressBar.Step = 1;
                    mStatusProgressBar.Value = 0;
                    mStatusProgressBar.Visible = true;
                    mHasProgressBar = true;
                }
            }

            private void ShowProgressBar_Background()
            {
                ShowProgressBar();
                mStatusProgressBar.Tag = ProjectView.ProjectView.ProgressBar_Waveform;
                mStatusProgressBar.Style = ProgressBarStyle.Blocks;
            }

            public void BackgroundOperation_AddItem()
            {
                if (mStatusProgressBar.ProgressBar != null)
                {
                    ShowProgressBar_Background();
                    ++mStatusProgressBar.Maximum;
                }
            }

            public void BackgroundOperation_Step()
            {
                if (mStatusProgressBar.ProgressBar != null)
                {
                    ShowProgressBar_Background();
                    if (mStatusProgressBar.Value < mStatusProgressBar.Maximum/3)
                        ++mStatusProgressBar.Value; //@singleSection: experiment
                }
            }

            public void BackgroundOperation_Done()
            {
                if (mStatusProgressBar.ProgressBar != null)
                {
                    mStatusProgressBar.Visible = false;
                    mHasProgressBar = false;
                }
            }

            private void mPhrases_AssignRole_NewCustomRoleMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ShowPhrasePropertiesDialog(true);
            }

            private void mPhrases_ClearRoleMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ClearRoleOfSelectedPhrase();
            }

            private void Presentation_Changed(object sender, urakawa.events.DataModelChangedEventArgs e)
            {
                if (mProjectView.Presentation != null && mProjectView.Selection != null &&
                    !mProjectView.TransportBar.IsActive)
                    ShowSelectionInStatusBar();
            }

            private void mShowSectionContentsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.Selection != null) mProjectView.ShowSelectedSectionContents();
            }


            private void mShowSingleSectionToolStripItem_Click(object sender, System.EventArgs e)
            {
                //if (mSession.HasProject) mProjectView.ShowOnlySelectedSection = mShowSingleSectionToolStripItem.Checked; //  //@ShowSingleSection
            }

            private void nextPageToolStripMenuItem_Click_1(object sender, EventArgs e)
            {
                try
                {
                    mProjectView.GoToPageOrPhrase();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            private void mMergePhraseWithFollowingPhrasesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergePhraseWithFollowingPhrasesInSection();
            }

            private void mMergePhraseWithPrecedingPhrasesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergeWithPhrasesBeforeInSection();
            }

            private void splitAndMergeWithNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SplitAndMerge(true);
            }

            private void splitAndMergeWithPreviousToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.SplitAndMerge(false);
            }
            private void mDeleteFollowingPhrasesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.DeleteFollowingPhrasesInSection();
            }

            private void mApplyPhraseDetectionInProjectToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ApplyPhraseDetectionInWholeProject();
            }

            private void mView_RecordingToolBarMenuItem_Click(object sender, EventArgs e)
            {
                if (mView_RecordingToolBarMenuItem.Checked)
                {
                    if (mRecordingToolBarForm != null)
                    {
                        mRecordingToolBarForm.Close();
                    }
                }
                else
                {
                    ShowRecordingToolBar();

                }
            }

            private void mergeMultipleSectionsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergeMultipleSections();
            }

            private void mergeSectionWithNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergeStrips();
            }

            public string ExportAudioDirectory
            {
                get { return Path.Combine(Path.GetDirectoryName(mSession.Path), "Exported_Audio_Files"); }
            }

            private void mtools_ExportSelectedAudioMenuItem_Click(object sender, EventArgs e)
            {
                //String directoryPath = Path.GetDirectoryName(mSession.Path);
                //if (mProjectView.IsZoomWaveformActive)
                //{
                    //mProjectView.ZoomPanelClose();
                //}
                mProjectView.ExportAudioOfSelectedNode();
            }

            private void CheckForSelectedNodeInBookmark()
            {
                if (mProjectView.Presentation == null) return;
                ObiNode newBookMarkedNode = null;
                if (mProjectView.Selection is StripIndexSelection)
                    newBookMarkedNode = mProjectView.Selection.EmptyNodeForSelection;
                else if (mProjectView.Selection.Node is SectionNode || mProjectView.Selection.Node is EmptyNode)
                    newBookMarkedNode = mProjectView.Selection.Node;
                if (newBookMarkedNode == null) return;

                if (newBookMarkedNode != ((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode)
                {
                    ((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode = newBookMarkedNode;

                    if (mSession.CanSave == false)
                    {
                        if (!mSession.ErrorsInOpeningProject)
                        {
                            mSession.PresentationHasChanged(1);
                            mSession.ForceSave();
                        }
                    }
                    else
                        mSession.PresentationHasChanged(1);

                    //   mSession.PresentationHasChanged(1);
                    UpdateMenus();
                    UpdateTitleAndStatusBar();

                }
            }

            private void mView_RefreshContentViewMenuItem_Click(object sender, EventArgs e)
            {
                UpdateZoomFactor();
            }

            private void mHelp_newFeaturestoolStripMenuItem_Click(object sender, EventArgs e)
            {
                //Uri url = new Uri(Localizer.Message("Obi_NewFeaturesWebpage"));
                Uri url =
                // new Uri(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                  //                       "Obi 2.0 Alpha-New features help.htm"));
                new Uri("http://www.daisy.org/obi/obi-2.6-test-releases");
                System.Diagnostics.Process.Start(url.ToString());
            }

            private void mEdit_GotoBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode != null)
                {
                    if (((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode is EmptyNode)
                    {
                        mProjectView.SelectedBlockNode =
                            (EmptyNode) ((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode;
                    }
                    else if (((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode is SectionNode)
                    {
                        mProjectView.SelectedStripNode =
                            (SectionNode) ((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode;
                    }
                    else if (((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode is StripIndexSelection)
                    {
                        mProjectView.SelectedBlockNode =
                            (EmptyNode) ((ObiRootNode) mProjectView.Presentation.RootNode).BookmarkNode;
                    }
                }
            }

            private void mEdit_AssignBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
            {
                CheckForSelectedNodeInBookmark();
            }

            private void m_RestoreFromBackupToolStripMenuItem_Click(object sender, EventArgs e)
            {
                RestoreProjectFromBackup();
            }

            private void m_RestoreFromOriginalProjectToolStripMenuItem_Click(object sender, EventArgs e)
            {
                RestoreProjectFromMainProject();
            }

            private void RestoreProjectFromBackup()
            {

                if (
                    (MessageBox.Show(Localizer.Message("open_from_backup_file"),
                                     Localizer.Message("information_caption"), MessageBoxButtons.YesNo) ==
                     DialogResult.Yes))
                {
                    if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@monitorContinuously
                    if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                    m_OriginalPath = mSession.Path;
                    string backupPath = mSession.BackUpPath;
                    if (string.IsNullOrEmpty(backupPath) ||
                        !File.Exists(backupPath))
                    {
                        MessageBox.Show((Localizer.Message("backup_file_missing") + "\n" + backupPath));
                        return;
                    }
                    m_RestoreProjectFilePath = Path.Combine(Directory.GetParent(mSession.Path).ToString(),
                                                            Localizer.Message("restored_project_name"));
                    if (File.Exists(m_RestoreProjectFilePath))
                        File.Delete(m_RestoreProjectFilePath);
                    File.Copy(backupPath, m_RestoreProjectFilePath);

                    if (DidCloseProject())
                    {
                        OpenProject_Safe(m_RestoreProjectFilePath, Localizer.Message("Open_Backup_file"));
                        ProjectHasChanged(1);
                        m_RestoreFromOriginalProjectToolStripMenuItem.Visible = true;
                        m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = true;
                        m_RestoreFromBackupToolStripMenuItem.Visible = false;
                        m_RestoreFromBackupToolStripMenuItem.Enabled = false;
                        mTools_CleanUnreferencedAudioMenuItem.Enabled = false;
                    }
                    else
                    {
                        //if existing project could not be closed then delete the restore file just created.
                        if (File.Exists(m_RestoreProjectFilePath)) File.Delete(m_RestoreProjectFilePath);
                    }
                }
                else
                    return;
            }

            private void RestoreProjectFromMainProject()
            {
                if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@monitorContinuously
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                mSession.Close();
                OpenProject_Safe(m_OriginalPath, Localizer.Message("Load_Original_Project"));
                if (File.Exists(m_RestoreProjectFilePath)) File.Delete(m_RestoreProjectFilePath);

                m_RestoreProjectFilePath = null;
                m_OriginalPath = null;
                mTools_CleanUnreferencedAudioMenuItem.Enabled = true;
                m_RestoreFromOriginalProjectToolStripMenuItem.Visible = false;
                m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = false;
                m_RestoreFromBackupToolStripMenuItem.Visible = true;
                m_RestoreFromBackupToolStripMenuItem.Enabled = true;
            }

            private bool? FreezeChangesFromProjectRestore()
            {
                
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();

                if (!String.IsNullOrEmpty(m_RestoreProjectFilePath)
                    && mSession.Path == m_RestoreProjectFilePath)
                {
                    if (
                        MessageBox.Show(Localizer.Message("save_current_state"), Localizer.Message("Save_current_state_caption"),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (mProjectView.TransportBar.MonitorContinuously) mProjectView.TransportBar.MonitorContinuously = false; //@monitorContinuously
                        mSession.Save(m_OriginalPath);
                        mSession.Close();
                        OpenProject_Safe(m_OriginalPath, Localizer.Message("Save_Restore_Project"));

                        m_OriginalPath = null;
                        if (File.Exists(m_RestoreProjectFilePath)) File.Delete(m_RestoreProjectFilePath);

                        m_RestoreFromOriginalProjectToolStripMenuItem.Visible = false;
                        m_RestoreFromOriginalProjectToolStripMenuItem.Enabled = false;
                        m_RestoreFromBackupToolStripMenuItem.Visible = true;
                        m_RestoreFromBackupToolStripMenuItem.Enabled = true;
                        m_RestoreProjectFilePath = null;
                        mTools_CleanUnreferencedAudioMenuItem.Enabled = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return null;
            }

            private void m_GoToCollectSpecialPhrasesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ShowSpecialPhraseList();
            }

            private void mSkippableBeginNoteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MarkBeginNote();
            }

            private void mSkippableEndNoteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MarkEndNote();
                mProjectView.AssignRoleToMarkedContinuousNodes();
            }

            private void mSkippableAddReferenceToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AssociateNodeToSpecialNode();
            }

            private void mSkippableGotoToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (((EmptyNode) mProjectView.Selection.Node).AssociatedNode == null)
                    MessageBox.Show(Localizer.Message("Anchor_node_not_associated"));
                if (mProjectView.Selection.Node is EmptyNode &&
                    ((EmptyNode) mProjectView.Selection.Node).AssociatedNode != null)
                    mProjectView.SelectedBlockNode = ((EmptyNode) mProjectView.Selection.Node).AssociatedNode;

            }

            private void mSkippableRemoveReferenceToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.DeassociateSpecialNode();
            }

            private void mSkippableMoveToStartNoteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.GotoSkippableNoteEnds(true);
            }

            private void mSkippableMoveToEndNoteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.GotoSkippableNoteEnds(false);
            }

            private void mPhrases_AssignRole_AnchorMenuItem_Click(object sender, EventArgs e)
            {
                //((EmptyNode) mProjectView.Selection.Node).Role_ = EmptyNode.Role.Anchor;
                mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Anchor, null);
            }

            private void mFineNavigationToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FineNavigationModeForPhrase =
                    !mProjectView.TransportBar.FineNavigationModeForPhrase;
            }

            private void mHelp_WhatsNewMenuItem_Click(object sender, EventArgs e)
            {
                try
                {
                    //System.Diagnostics.Process.Start(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                      //                                            "New-Features-Obi-2.0-alpha.htm"));
                    System.Diagnostics.Process.Start("http://www.daisy.org/obi/obi-2.6-test-releases");
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(Localizer.Message("ObiFormMsg_FileLoadingFail") + "\n\n" +
                                                         ex.ToString()); //@Messagecorrected
                }
            }

            private void mSkippableClearRoleFromNoteToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ClearSkippableChunk();
            }

            public long CheckDiskSpace()
            {
                if (mSession != null && mSession.HasProject && mSettings.Project_EnableFreeDiskSpaceCheck)
                {
                    
                        long space = mSession.CheckDiskSpace();
                        if (space == long.MaxValue)
                        {
                            mSettings.Project_EnableFreeDiskSpaceCheck = false;
                            mSession.EnableFreeDiskSpaceCheck = false;
                        }
                        return space;
                    
                }

                return long.MaxValue;
            }

            private void ObiForm_Resize(object sender, EventArgs e)
            {

                if (mPeakMeter != null && mRecordingToolBarForm == null)
                {

                  PeakMeterResize();

                }
                else if (mPeakMeter != null && mRecordingToolBarForm != null && (this.WindowState == FormWindowState.Maximized || this.WindowState == FormWindowState.Normal))
                {
                    if (this.WindowState != FormWindowState.Minimized)
                    {

                        PeakMeterResize();

                    }
                }
                if (mSettings != null  && mProjectView != null && mSettings.Project_SaveTOCViewWidth && mSettings.TOCViewWidth != 0 && mSession.Presentation != null)
                {
                    mProjectView.TOCViewWidth = mSettings.TOCViewWidth;
                }
            }
            public FormWindowState ObiformWindowsState
            {
                get
                {
                    return this.WindowState;
                }
            }

            private void PeakMeterResize()
            {
                if ((this.WindowState == FormWindowState.Maximized) && m_flag != true)
                {
                    mPeakMeter.WindowState=FormWindowState.Normal;
                    m_tempHeightMin = mPeakMeter.Height;
                    m_flagForPeakMeterHeight = true;

                    
                    System.Drawing.Point newLoc = this.Location;
                    newLoc.X += SystemInformation.HorizontalResizeBorderThickness;
                    newLoc.Y += SystemInformation.VerticalResizeBorderThickness;
                    System.Drawing.Size newSize = this.Size;
                    newSize.Width -= 2 * SystemInformation.HorizontalResizeBorderThickness;
                    newSize.Height -= 2 * SystemInformation.VerticalResizeBorderThickness;

                    this.WindowState = FormWindowState.Normal;


                    this.Location = newLoc;
                    m_ShowingPeakMeter = true;
                    this.Size = newSize;
                    flagMaxWindow = true;

                    this.Width -= mPeakMeter.Width;

                    //   int temphgt = mPeakMeter.Height - mPeakMeter.PeakMeterHeight;//peakmeter ref


                    //   mPeakMeter.AnchorStyle = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);//peakmeter ref

                 //   mPeakMeter.Height = this.Height;
                    mPeakMeter.Top = this.Top;
                      //  mPeakMeter.PeakMeterTop = mPeakMeter.Top;  //peakmeter ref
                       // mPeakMeter.PeakMeterHeight = mPeakMeter.PeakMeterHeight + temphgt - m_DiffPeakMeterGraphicalPeakMeter;//peakmeter ref




                    m_FlagWindowState = true;
                    m_flag = true;
                    m_NormalAfterMax = true;
                   
                }


                else if (this.WindowState == FormWindowState.Normal && m_flag != true)
                {
                    if (this.Height >= m_tempHeightMin)
                    {

                        if (m_FlagWindowState == true)
                        {
                           // mPeakMeter.AnchorStyle = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right);//peakmeter ref
                           // mPeakMeter.Height = this.Height;//peakmeter ref
                            m_FlagWindowState = false;
                        }
                        else
                        {
                          //  mPeakMeter.AnchorStyle = AnchorStyles.None; //peakmeter ref
                           // mPeakMeter.Height = this.Height; //peakmeter ref
                        }
                        m_NormalAfterMax = false;

                    }
                    m_flag = false;

                }
                
                if (mPeakMeter.WindowState == FormWindowState.Minimized && this.WindowState == FormWindowState.Normal)
                {
                    mPeakMeter.WindowState = FormWindowState.Normal;
                   // mPeakMeter.Height = this.Height;
                }
                mPeakMeter.Left = this.Right;
                
                //mPeakMeter.Height = this.Height;

                // mPeakMeter.Top = this.Top;




            }

            private void selectedPageToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.GenerateSpeechForPage(false);
            }

            private void allEmptyPagesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.GenerateSpeechForPage(true);
            }

            private void settingsFromSilencePhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.UpdatePhraseDetectionSettingsFromSilencePhrase();
            }

            private void CheckForNewRelease(bool isAutomaticUpdate)
            {
                Dialogs.CheckUpdates check = new CheckUpdates(mSettings, isAutomaticUpdate);
                check.CheckForAvailableUpdate();
            }

            private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                CheckForNewRelease(false);
            }

            private void mCheckForPhrasesWithImproperAudioMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.ReplacePhrasesWithImproperAudioWithEmptyPhrases((ObiNode)mProjectView.Presentation.RootNode, true);
            }

            private void m_DeletePhrasesWhileRecordingtoolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.RecordOverSubsequentPhrases();
            }



            private void mRecordToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
            {
                m_DeletePhrasesWhileRecordingtoolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive && mSettings.Audio_AllowOverwrite && mProjectView.TransportBar.CanRecord && !mProjectView.TransportBar.IsListening;
                if (mSettings.Audio_AllowOverwrite)
                {
                    
                    mAllowOverwriteToolStripMenuItem.Checked = true;
                }
                else
                {
                    
                    mAllowOverwriteToolStripMenuItem.Checked = false;
                }
                if (mProjectView.TransportBar.IsPreviewBeforeRecordingEnabled)
                {
                    m_PreviewBeforeRecordingToolStripMenuItem.Enabled = true;
                }
                else
                {
                    m_PreviewBeforeRecordingToolStripMenuItem.Enabled = false;
                }
                if (mProjectView.TransportBar.MonitorContinuously != m_MonitorContinuouslyToolStripMenuItem.Checked) m_MonitorContinuouslyToolStripMenuItem.Checked = mProjectView.TransportBar.MonitorContinuously;
                if (!mRecordToolStripMenuItem.Enabled)
                {
                    m_MonitorContinuouslyToolStripMenuItem.Enabled = false;
                    mStartMonitoringToolStripMenuItem.Enabled = false;
                    mAllowOverwriteToolStripMenuItem.Enabled = false;
                    mStartRecordingDirectlyToolStripMenuItem.Enabled = false;
                }
                else
                {
                    if (!mProjectView.TransportBar.IsPlayerActive && mSettings.Audio_RecordDirectlyWithRecordButton && !mProjectView.IsZoomWaveformActive)
                    {
                        m_MonitorContinuouslyToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        m_MonitorContinuouslyToolStripMenuItem.Enabled = false;
                    }
                    if (!mProjectView.IsZoomWaveformActive)
                    {
                        mStartMonitoringToolStripMenuItem.Enabled = true;
                        mAllowOverwriteToolStripMenuItem.Enabled = true;
                        if (mProjectView.TransportBar.IsListening)
                        {
                            mStartRecordingDirectlyToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            mStartRecordingDirectlyToolStripMenuItem.Enabled = true;
                        }
                    }
                    else
                    {

                        mStartMonitoringToolStripMenuItem.Enabled = false;
                        mAllowOverwriteToolStripMenuItem.Enabled = false;
                        mStartRecordingDirectlyToolStripMenuItem.Enabled = false;
                    }
                }


            }

            private void mFile_MergeProjectMenuItem_Click(object sender, EventArgs e)
            {               
                   CanAutoSave = false;
                   mProjectView.MergeProject(mSession);
                   CanAutoSave = true;                
            }

            private void mPhrases_RenumberPagesMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.Presentation != null)
                {
                    try
                    {
                        EmptyNode startNode = mProjectView.Selection != null ? (mProjectView.Selection.Node is EmptyNode ? (EmptyNode)mProjectView.Selection.Node : mProjectView.Selection.Node is SectionNode && ((SectionNode)mProjectView.Selection.Node).PhraseChildCount > 0 ? (EmptyNode)((SectionNode)mProjectView.Selection.Node).FirstLeaf : null) :
                                        mProjectView.Presentation.FirstSection != null && ((mProjectView.Presentation.FirstSection).PhraseChildCount > 0) ? 
                                        (EmptyNode)mProjectView.Presentation.FirstSection.FirstLeaf : null;

                        if (startNode == null) return;

                        PageNumber num = null;
                        for (ObiNode n = startNode.PrecedingNode; n != null; n = n.PrecedingNode)
                        {
                            if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                            {
                                num = ((EmptyNode)n).PageNumber;
                                break;
                            }
                        }
                        Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber(num, false, false, mSettings); //@fontconfig
                        dialog.AutoFillPagesEnable = false;
                        dialog.IsRenumberChecked = true;
                        dialog.EnableRenumberCheckBox = false;
                        dialog.Text = Localizer.Message("RenumberPages");
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            PageNumber pgNumber = new PageNumber(dialog.Number.Number - 1, dialog.Number.Kind);
                            urakawa.command.CompositeCommand k = mProjectView.GetPageRenumberCommand(startNode, pgNumber, "renumber command",false);
                            mProjectView.Presentation.Do(k);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        mProjectView.WriteToLogFile(ex.ToString());
                        MessageBox.Show(ex.ToString());
                    }

                }
            }

            private void m_ToolsLangPack_Click(object sender, EventArgs e)
            {
                if (m_FlagLangUpdate == false)
                {
                    MessageBox.Show(Localizer.Message("Language_Pack_Select"), Localizer.Message("Language_Pack_Select_Caption"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                OpenFileDialog select_File = new OpenFileDialog();
                select_File.Filter = "(*.zip)|*.zip";
                if (select_File.ShowDialog() == DialogResult.OK)
                {
                    string fileName = select_File.FileName;
                    string nameOfFile = System.IO.Path.GetFileName(fileName);
                    string destinationPath = Application.StartupPath;
                    string installationPath = System.IO.Path.GetTempPath();
                    installationPath += "Obi_Language_Pack";
                   // destinationPath = @"C:\Program Files\The Urakawa Project\Obi 3.5 alpha";
                    bool isExists = System.IO.Directory.Exists(installationPath);
                    if (!isExists)
                    {
                        System.IO.Directory.CreateDirectory(installationPath);
                    }

                    ZipStorer zip = ZipStorer.Open(fileName, FileAccess.Read);
                    List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                    bool isObiResourcesDll = false;

                    foreach (ZipStorer.ZipFileEntry entry in dir)
                    {
                        
                        if (Path.GetFileName(entry.FilenameInZip) == "Obi.resources.dll")
                        {
                            string tempStr = Directory.GetParent(entry.ToString()).Name;
                            Console.WriteLine("zip entry : " + entry.ToString() + " : " + tempStr);
                            
                            zip.ExtractFile(entry, installationPath + @"\" + tempStr + @"\Obi.resources.dll");
                            isObiResourcesDll = true;
                        }
                        else if (Path.GetFileName(entry.FilenameInZip) == "PipelineInterface.resources.dll")
                        {
                            string tempStr = Directory.GetParent(entry.ToString()).Name;
                            zip.ExtractFile(entry, installationPath + @"\" + tempStr + @"\PipelineInterface.resources.dll");
                        }

                    }
                    if (!isObiResourcesDll)
                    {
                        MessageBox.Show(Localizer.Message("Language_Pack_WrongFile"), Localizer.Message("Language_Pack_Caption"));
                        m_FlagLangUpdate = true;
                        m_ToolsLangPack_Click(sender, e);
                        return;
                    }
                  
                    zip.Close();

                    bool isUnauthorizedException = false;
                    try
                    {
                        foreach (string d in Directory.GetDirectories(installationPath))
                        {
                            string NameOfDirectory = Path.GetFileName(d);

                            foreach (string f in Directory.GetFiles(d))
                            {
                                string source = System.IO.Path.Combine(installationPath, f);
                                
                                string NameOfTheFile = Path.GetFileName(f);
                                string destination = System.IO.Path.Combine(destinationPath, NameOfDirectory);
                                isExists = System.IO.Directory.Exists(destination);
                                if (!isExists)
                                {
                                    System.IO.Directory.CreateDirectory(destination);
                                }
                                destination = System.IO.Path.Combine(destination, NameOfTheFile);
                                File.Copy(source, destination,true);
                                Console.WriteLine("Source {0}", source);
                            }
                        }
                    //    foreach(File file
                        MessageBox.Show(Localizer.Message("Language_Pack_Complete"), Localizer.Message("Language_Pack_Complete_Caption"));
                       
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show(Localizer.Message("NoAdministrativePrivilege"));
                        isUnauthorizedException = true;
                    }

                    if (isUnauthorizedException)
                    {
                        
                        Process.Start("explorer.exe", installationPath);
                    }
                }
                else
                {
                    m_FlagLangUpdate = false;
                }
            }

            private void m_PlaySectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PlaySection();
            }

            private void m_PlayHeadingToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PlayHeading();
            }

            private void m_PreviewBeforeRecordingToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.PreviewBeforeRecording();
            }

            private void m_MonitorContinuouslyToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (m_MonitorContinuouslyToolStripMenuItem.Checked)
                {
                    mProjectView.TransportBar.MonitorContinuously = true;
                }
                else
                {
                    mProjectView.TransportBar.MonitorContinuously = false;
                }
            }

            private void mBackwardElapseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.FastPlayNormaliseWithLapseBack();
            }

            private void mForwardElapseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.TransportBar.StepForward();
            }

            private void mSectionsToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
            {
                UpdateSectionsMenu();
            }

            private bool ValidateWithEpubCheck(string epub3Export, out string strMessage_ErrorsWarnings, out string strMessage_Status)
            {
                
                
                string strErrors = null;
                string strOutput = null;
                string strStatus = null;
                try
                {
                    string pipelineDirectoryPath = Path.GetDirectoryName(mSettings.Project_PipelineScriptsPath);
                    string epubCheckFullPath = Path.Combine(pipelineDirectoryPath, "epubcheck\\epubcheck.jar");
                    if (!File.Exists(epubCheckFullPath))
                    {
                        MessageBox.Show(Localizer.Message("obi_EPUB3ValidatorNotInstalled"));
                        strMessage_ErrorsWarnings = Localizer.Message("obi_EPUB3CheckNotFound");
                        strMessage_Status = Localizer.Message("OperationFailed");
                        return false ;
                    }
                    //string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                    //string obiSettingsDir = System.IO.Path.Combine(appDataDir, "Obi");
                    //if (!System.IO.Directory.Exists(obiSettingsDir)) System.IO.Directory.CreateDirectory(obiSettingsDir);
                    //string epubCheckOutput = System.IO.Path.Combine(obiSettingsDir, "epubcheckoutput.txt" );
                    //epubCheckOutput = "epubcheckoutput.txt" ;
                    
                    Process epubCheckProcess = new Process();
                    epubCheckProcess.StartInfo.CreateNoWindow = true;
                    epubCheckProcess.StartInfo.ErrorDialog = true;
                    epubCheckProcess.StartInfo.UseShellExecute = false;
                    
                    epubCheckProcess.StartInfo.FileName = "java.exe";
                    //if (IntPtr.Size == 8) 
                    //{
                        //string java32bitPath = Path.Combine (Directory.GetParent (System.Environment.SystemDirectory).FullName, "SysWOW64\\java.exe");
                        //if (File.Exists(java32bitPath))
                        //{
                            //epubCheckProcess.StartInfo.FileName = java32bitPath;
                            //Console.WriteLine("EPUBCheck invoked in 32 bit: " + epubCheckProcess.StartInfo.FileName);
                        //}
                //}
                    //
                    string strMode =Path.GetExtension(epub3Export).ToLower() == ".epub"? "" : "\" -mode exp -v 3.0";
                    
                    string strArguments = "-jar \"" +
                        epubCheckFullPath + "\" \"" +
                        epub3Export + strMode;
                        //-save > \"" + epubCheckOutput + "\" 2>&1";
                    epubCheckProcess.StartInfo.Arguments = strArguments;
                    Console.WriteLine("process " + epubCheckProcess.StartInfo.FileName + " " + epubCheckProcess.StartInfo.Arguments);
                    
                    epubCheckProcess.StartInfo.RedirectStandardOutput = true;
                    epubCheckProcess.StartInfo.RedirectStandardError = true;
                    epubCheckProcess.StartInfo.WorkingDirectory =Directory.GetParent ( Directory.GetParent (Directory.GetParent(epubCheckFullPath).FullName).FullName).FullName;
                    

                    epubCheckProcess.Start();
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    if (mSettings.Project_EPUBCheckTimeOutEnabled)
                    {
                        epubCheckProcess.WaitForExit(180000);
                        if (!epubCheckProcess.HasExited)
                        {
                            epubCheckProcess.CloseMainWindow();
                            stopWatch.Stop();
                            MessageBox.Show(Localizer.Message("ValidatorTimeOutMsg"));
                            strStatus = Localizer.Message("obi_EPUB3ValidationFailed");
                        }
                        else
                        {
                            strErrors = epubCheckProcess.StandardError.ReadToEnd();
                            strOutput = epubCheckProcess.StandardOutput.ReadToEnd();
                            stopWatch.Stop();
                        }
                    }
                    else
                    {
                        epubCheckProcess.WaitForExit();
                        strErrors = epubCheckProcess.StandardError.ReadToEnd();
                        strOutput = epubCheckProcess.StandardOutput.ReadToEnd();
                        stopWatch.Stop();
                    }
                    if(stopWatch != null )  Console.WriteLine("EPUB Check execution time: " + stopWatch.ElapsedMilliseconds);
                    //File.WriteAllText(epubCheckOutput, strArguments);
                    
                }
                catch (System.Exception ex)
                {
                    ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                    MessageBox.Show(ex.ToString());
                }
                
                bool isSuccessful = false ;
                if (strOutput == null ||  strErrors == null )
                {
                    // operation failed, no results should be displayed
                    strErrors = "failed" ;
                    isSuccessful = false ;
                }
                else
                {
                    if (strOutput.ToLower().Contains("no errors or warnings detected"))
                    {
                        isSuccessful = true;
                        strStatus = Localizer.Message("obi_EPUB3ValidationSuccessfull");
                    }
                    else if (strErrors.ToLower().Contains ("error:"))
                    {
                        isSuccessful = false ;
                        strStatus = Localizer.Message("obi_EPUB3ValidationFailed");
                    }
                    else if (strErrors.ToLower().Contains("warning:"))
                    {
                        isSuccessful = true;
                        strStatus = Localizer.Message("obi_EPUB3ValidationSuccessfullWithWarnings");
                    }
                    else
                    {
                        
                        isSuccessful = false;
                        strStatus = Localizer.Message("obi_EPUB3ValidationFailed");
                    }
                }
                strMessage_ErrorsWarnings = strOutput + strErrors;
                strMessage_Status = strStatus;
                // messagebox for debugging
                
                return isSuccessful;
            }

            private void m_EPUB3ValidatorToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    mProjectView.TransportBar.Stop();
                }
                Dialogs.Epub3Validator epubValidator = new Dialogs.Epub3Validator(Directory.GetParent(mSession.Path).FullName, this.mSettings); //@fontconfig
                epubValidator.ShowEpubValidatorDialog = true;
                epubValidator.ShowResultDialog = false;

                string exportDirectoryEPUB3 = mProjectView.GetDAISYExportPath(ImportExport.ExportFormat.EPUB3,Path.GetDirectoryName(mSession.Path));
                string epubcheckFileFolder="";
                String[] file=null;
                if(exportDirectoryEPUB3!=null && Directory.Exists(exportDirectoryEPUB3))
                file = System.IO.Directory.GetFiles(exportDirectoryEPUB3, "*.epub");
                if (Directory.Exists(exportDirectoryEPUB3) && file!=null && file.Length!=0)
                {                      
                        exportDirectoryEPUB3 = file[0];
                        epubValidator.InputEpubPath = exportDirectoryEPUB3;
                        epubcheckFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Obi");
                        if (!Directory.Exists(epubcheckFileFolder))
                        {
                            Directory.CreateDirectory(epubcheckFileFolder);
                        }
                        //epubcheckFileFolder = Path.Combine(epubcheckFileFolder, "epubcheckoutput.txt");
                        //epubValidator.OutputValidatorReportFilePath = epubcheckFileFolder;
                    
                }
                else
                {
                    MessageBox.Show(Localizer.Message("no_primary_export_directory"), Localizer.Message("no_primary_export_directory_Caption"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                if (epubValidator.ShowDialog() == DialogResult.OK)
                {
                  
                    epubValidator.ShowEpubValidatorDialog = false;
                    epubValidator.ShowResultDialog = true;
                    
                    string str = "";
                    string epubDirectoryPath = epubValidator.InputEpubPath  ;
                    if (epubDirectoryPath.EndsWith(".opf"))
                    {
                        epubDirectoryPath = Directory.GetParent( Directory.GetParent (epubDirectoryPath).FullName).FullName ;
                        
                    }
                    bool isSuccessful=false;
                    string strStatus = null;
                    ProgressDialog progress = new ProgressDialog(Localizer.Message("progress_EpubValidating"),
                                                                        delegate(ProgressDialog progress1)
                                                                        {

                                                                           isSuccessful = ValidateWithEpubCheck(epubDirectoryPath, out str,out strStatus);
                                                                        });

                    progress.ShowDialog();

                    //if (isSuccessful)
                    //{
                        //epubValidator.CompletionStatus = Localizer.Message("obi_EPUB3ValidationSuccessfull");
                    //}
                    //else
                    //{
                        //epubValidator.CompletionStatus = Localizer.Message("obi_EPUB3ValidationFailed");
                    //}
                    epubValidator.CompletionStatus = strStatus;
                    //string text = File.ReadAllText(epubcheckFileFolder);
                    epubValidator.EpubCheckOutputText = str;
                    epubValidator.ShowDialog();
                }

            }

            private void mTools_AudioProcessing_Click(object sender, EventArgs e)
            {
                //if (mProjectView.TransportBar.IsPlayerActive)
                //{
                //    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                //    mProjectView.TransportBar.Stop();
                //}
                //if (mProjectView.CanExportSelectedNodeAudio)
                //{
                //    mProjectView.ProcessAudio();
                //}
            }

            private void mMergeWithNextSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MergeStrips();
            }

            private void mMultiSectionOperations_Click(object sender, EventArgs e)
            {
                mProjectView.MergeMultipleSections();
            }

            public void UpdateRecordingToolBarButtons()
            {
                if (mRecordingToolBarForm != null)
                {
                    mRecordingToolBarForm.UpdateForChangeInObi();
                }
            }

            private void beginMarkToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MarkBeginNote();
            }
            
            private void endMarkToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.MarkEndNote();
                mProjectView.AssignRoleToMarkedContinuousNodes();
            }

            private void m_AutoFillMissingPagesMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.FillEmptyPagesForMissingPagesInCompleteProject();
            }

            private void m_Tools_CompleteCleanupToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.IsZoomWaveformActive)
                {
                    mProjectView.ZoomPanelClose();
                }
                CleanProject(false);
            }

            private void m_Tools_QuickCleanupToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.IsZoomWaveformActive) mProjectView.ZoomPanelClose();
                CleanProject(true);
            }

            private void mTools_CleanUnreferencedAudioMenuItem_DropDownOpening(object sender, EventArgs e)
            {
                if (Settings != null)
                {
                    m_Tools_QuickCleanupToolStripMenuItem.Enabled = Settings.Audio_EnableFileDataProviderPreservation;
                }
            }

            private void rollbackToolStripMenuItem_Click(object sender, EventArgs e)
            {
                CleanUpRollBack();
            }

            private void detectSilencePhraseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.DetectSilenceErrors();
            }

            private void navigationToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
            {
                if (mSettings.PlayOnNavigate)
                {
                    mPlayOnNavigateToolStripMenuItem.Checked = true;
                }
                else
                {
                    mPlayOnNavigateToolStripMenuItem.Checked = false;
                }
            }

            private void mAutoPageGenerationMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.AutoPageGeneration();
            }

            private void mImportTOCMenuItem_Click(object sender, EventArgs e)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = Localizer.Message("choose_import_file");
                    dialog.Filter = Localizer.Message("FilterImportTOC");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string strExtension = System.IO.Path.GetExtension(dialog.FileName).ToLower();
                    ImportExport.ImportTOC importTOC = new Obi.ImportExport.ImportTOC();
                    bool tempResult = true;
                    if (strExtension == ".xhtml" || strExtension == ".html" || strExtension == ".htm")
                    {
                        importTOC.ImportFromXHTML(dialog.FileName);
                    }
                    else if (strExtension == ".csv" || strExtension == ".text" || strExtension == ".txt")
                    {

                        tempResult = importTOC.ImportFromCSVFile(dialog.FileName);
                    }

                    if (importTOC.SectionNamesOfImportedTocList.Count != importTOC.LevelsListOfImportedTocList.Count || !tempResult || importTOC.SectionNamesOfImportedTocList.Count == 0)
                    {
                        MessageBox.Show(Localizer.Message("WrongFormat"), Localizer.Message("Caption_Error"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    List<SectionNode> listOfSectionNodes = new List<SectionNode>();
                        if (mSession != null && mSession.Presentation != null &&
                            mSession.Presentation.FirstSection != null)
                        {
                            for (SectionNode tempNode = (SectionNode) mSession.Presentation.FirstSection;
                                tempNode != null;
                                tempNode = (SectionNode) tempNode.FollowingSection)
                            {
                                listOfSectionNodes.Add(tempNode);
                            }
                        }
                        else
                        {
                            if (listOfSectionNodes.Count == 0)
                            {
                                MessageBox.Show(Localizer.Message("NoSectionsInBook"),
                                    Localizer.Message("Caption_Information"),MessageBoxButtons.OK,MessageBoxIcon.Information);
                            }

                            return;
                        }


                        if (listOfSectionNodes.Count < importTOC.SectionNamesOfImportedTocList.Count)
                        {
                            MessageBox.Show(Localizer.Message("LessSectionsInBook"),
                                Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        int index = 0;
                        urakawa.command.CompositeCommand renameCommands = mProjectView.Presentation.CreateCompositeCommand("Overwrite TOC with Imported TOC");
                        foreach (SectionNode tempNode in listOfSectionNodes)
                        {
                            if (index < importTOC.SectionNamesOfImportedTocList.Count &&
                                importTOC.SectionNamesOfImportedTocList[index] != null)
                            {
                              //  mSession.Presentation.RenameSectionNode(tempNode,importTOC.SectionNamesOfImportedTocList[index]);

//                                mProjectView.RenameSectionNode(tempNode, importTOC.SectionNamesOfImportedTocList[index]);
                                renameCommands.ChildCommands.Insert (renameCommands.ChildCommands.Count,
                                new Commands.Node.RenameSection(mProjectView, tempNode, importTOC.SectionNamesOfImportedTocList[index]));
                                //   mSession.Presentation.
                                if (tempNode.Level != importTOC.LevelsListOfImportedTocList[index])
                                {
                                    ChangeLevelForTOCImport(tempNode, importTOC.LevelsListOfImportedTocList[index]);
                                }

                                index++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (renameCommands.ChildCommands.Count > 0)
                        {
                            mProjectView.Presentation.Do(renameCommands);
                        }
                    }
                }
            

            private void ChangeLevelForTOCImport(SectionNode tempSectionNode, int requiredLevelOfSection)
            {
                if (tempSectionNode.Level < requiredLevelOfSection)
                {
                    while (tempSectionNode.Level < requiredLevelOfSection)
                    {
                        if (Commands.TOC.MoveSectionIn.CanMoveNode(tempSectionNode))
                        {

                            mSession.Presentation.Do(new Commands.TOC.MoveSectionIn(mProjectView, tempSectionNode));
                        }
                       
                        else
                            break;
                  
                    }
                }
                else if (tempSectionNode.Level > requiredLevelOfSection)
                {
                    while (tempSectionNode.Level > requiredLevelOfSection)
                    {
                        if (Commands.TOC.MoveSectionOut.CanMoveNode(tempSectionNode))
                        {
                            mSession.Presentation.Do(new Commands.TOC.MoveSectionOut(mProjectView, tempSectionNode));
                        }
                        else if (tempSectionNode.SectionChildCount > 0)
                        {
                            SectionNode tempSecNode = tempSectionNode;

                            while (tempSecNode.SectionChildCount > 0)
                            {
                                tempSecNode = FindLastSectionChild(tempSecNode);
                            }                           

                            while (tempSecNode != tempSectionNode)
                            {
                                while (tempSecNode.Level > requiredLevelOfSection)
                                {
                                    if (Commands.TOC.MoveSectionOut.CanMoveNode(tempSecNode))
                                    {
                                        mSession.Presentation.Do(new Commands.TOC.MoveSectionOut(mProjectView, tempSecNode));
                                    }
                                    else
                                        break;
                                }
                                if (tempSecNode != null && tempSecNode.PrecedingSection != null)
                                {
                                    tempSecNode = tempSecNode.PrecedingSection;
                                }
                                else
                                    break;
                            }
                        }
                        else
                            break;

                    }
                }

                
               
            }
            
            private SectionNode FindLastSectionChild(SectionNode sectionNode)
            {
                SectionNode tempSecNode = sectionNode;
              
                for (int index = 0; index < sectionNode.SectionChildCount; index++)
                {
                    if (tempSecNode != null && tempSecNode.FollowingSection != null)
                    {
                        tempSecNode = tempSecNode.FollowingSection;                                            
                    }
                    else
                        break;
                }
                return tempSecNode;
            }

            private void m_pasteMultiplePhrasesToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.PasteMultiplePhrases();
            }


            private void m_AddViewCommentToolStripMenuItem_Click(object sender, EventArgs e) // @Comment-todo
            {
                mProjectView.ShowEditLabelToAddComment();
            }

            private void m_ClearCommentToolStripMenuItem_Click(object sender, EventArgs e) // @Comment-todo
            {
                    mProjectView.ClearComment();               

            }

            private void mShowPeakMeterInsideObiMenuItem_Click(object sender, EventArgs e)
            {
                if (mPeakMeter != null)
                {
                    mPeakMeter.Close();
                }
                mProjectView.ShowPeakMeterInsideObi(mShowPeakMeterInsideObiMenuItem.Checked);
            }

            private void transportToolStripMenuItem_Click(object sender, EventArgs e)
            {

            }

            private void m_ChangeVolumeToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio)
                mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.Amplify);
            }

            private void m_FadeInToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio)
                mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.FadeIn);
            }

            private void m_FadeOutToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio)
                mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.FadeOut);
            }

            private void m_NormalizeToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio || mProjectView.Selection == null)
                mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.Normalize);
            }

            
            private void m_SpeechRateToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio)
                    mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch);
            }

            private void m_NoiseReductionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio || mProjectView.Selection == null)
                    mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.NoiseReduction);
            }
            private void m_NoiseReductionRnnToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio || mProjectView.Selection == null)
                    mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.NoiseReductionRnn);
            }

            private void m_AudioMixerToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                if (mProjectView.CanExportSelectedNodeAudio)
                    mProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.AudioMixing);

            }

            private void selectedPageAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                ChoosePageAudio pageAudioDialog = new ChoosePageAudio();
                NodeSelection tempSelection = mProjectView.Selection;
                if (pageAudioDialog.ShowDialog() == DialogResult.OK)
                {
                    mProjectView.Selection = tempSelection;
                    mProjectView.GenerateSpeechForPage(false, pageAudioDialog.RecordedAudioPath);
                }
            }

            private void allEmptyPagesAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsPlayerActive)
                {
                    if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) mProjectView.TransportBar.Pause();
                    mProjectView.TransportBar.Stop();
                }
                ChoosePageAudio pageAudioDialog = new ChoosePageAudio();
                if (pageAudioDialog.ShowDialog() == DialogResult.OK)
                {
                    mProjectView.GenerateSpeechForPage(true, pageAudioDialog.RecordedAudioPath);
                }
            }

        private void m_TextToSpeechToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TextToSpeech();
            //GenerateSpeech generateSpeech = new GenerateSpeech();
            //generateSpeech.ShowDialog();
        }

            private void mHelp_ContentsEnglishMenuItem_Click(object sender, EventArgs e)
            {
                ShowHelpFile(true);
            }

            private void m_ImportMetadataToolStripMenuItem_Click(object sender, EventArgs e)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = Localizer.Message("ChooseMetadataImportFile");
                dialog.Filter = Localizer.Message("FilterMetadataCSV");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ImportExport.ImportMetadata metadataImport = new ImportMetadata();
                    metadataImport.ImportFromCSVFile(dialog.FileName, mSession.Presentation, mProjectView, false);
                }
            }

            private void mDeleteSilenceFromEndOfSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.RemoveSilenceFromEndOfSection(true);
            }

            private void mRetainSilenceInLastPhraseOfSectionToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.RemoveSilenceFromEndOfSection(false, true);
            }

            private void mDeleteSilenceFromEndOfAllSectionsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.RemoveSilenceFromEndOfSection(true, false, true);
            }

            private void mRetainSilenceInLastPhraseOfAllSectionsToolStripMenuItem_Click(object sender, EventArgs e)
            {
                mProjectView.RemoveSilenceFromEndOfSection(false, true, true);
            }



 
        }
    }
