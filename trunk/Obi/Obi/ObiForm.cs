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


            /// <summary>
            /// Initialize a new form and open the last project if set in the preferences.
            /// </summary>
            public ObiForm()
            {
                mShowWelcomWindow = true;
                InitializeObi();

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
                    mSettings.AllowOverwrite = value;
                }
            }

            /// <summary>
            /// Global audio scale for waveforms.
            /// </summary>
            public float AudioScale
            {
                get { return mSettings.AudioScale; }
                set
                {
                    if (value > 0.002f && value < 0.1f)
                    {
                        mSettings.AudioScale = value;
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

            // True if the user has chosen the "open last project" option, and there is a last project to open.
            private bool ShouldOpenLastProject
            {
                get { return mSettings != null && mSettings.OpenLastProject && mSettings.LastOpenProject != "" && mShowWelcomWindow; }
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

                m_RestoreFromBackupToolStripMenuItem.Enabled = mSession.HasProject;
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
                    mProjectView.TransportBar.Stop();
                if (DidCloseProject())
                {
                    Dialogs.NewProject dialog = new Dialogs.NewProject(
                        mSettings.DefaultPath,
                        Localizer.Message("default_project_filename"),
                        Localizer.Message("obi_filter"),
                        Localizer.Message("default_project_title"),
                        mSettings.NewProjectDialogSize,
                        mSettings.AudioChannels,
                        mSettings.AudioSampleRate);
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
                    mProjectView.TransportBar.Stop();
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
                        title = ImportExport.ImportStructure.GrabTitle(new Uri(path));
                    else if (strExtension == ".xml")
                        title = ImportExport.DAISY3_ObiImport.getTitleFromDtBookFile(path);

                    dialog = new Dialogs.NewProject(
                        mSettings.DefaultPath,
                        Localizer.Message("default_project_filename"),
                        Localizer.Message("obi_filter"),
                        title,
                        mSettings.NewProjectDialogSize,
                        mSettings.AudioChannels,
                        mSettings.AudioSampleRate);
                    dialog.DisableAutoTitleCheckbox();
                    dialog.Text = Localizer.Message("create_new_project_from_import");
                    if (!string.IsNullOrEmpty(dtbUid)) dialog.ID = dtbUid;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(dialog.Path)
                            &&
                            MessageBox.Show(Localizer.Message("ImportProject_OverwriteProjectMsg"),
                                            Localizer.Message("Caption_Warning"),
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                            MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            return false;
                        }

                        mSettings.NewProjectDialogSize = dialog.Size;
                        //CreateNewProject ( dialog.Path, dialog.Title, false, dialog.ID );
                        ImportExport.DAISY3_ObiImport import = null;
                        bool isProjectCreated = false;

                        if (strExtension == ".opf" || strExtension == ".xml")
                        {
                            isProjectCreated = ImportProjectFromDTB(dialog.Path, dialog.Title, false, dialog.ID, path, dialog.AudioChannels, dialog.AudioSampleRate);
                        }
                        else
                        {
                            //CreateNewProject(dialog.Path, dialog.Title, false, dialog.ID);
                            //(new Obi.ImportExport.ImportStructure()).ImportFromXHTML(path, mSession.Presentation);
                            isProjectCreated = ImportStructureFromXHtml(dialog.Path, dialog.Title, dialog.ID, path, dialog.AudioChannels, dialog.AudioSampleRate);
                        }
                        if (!isProjectCreated) return false;

                        mSession.ForceSave();
                        AddRecentProject(mSession.Path);
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

            private bool ImportProjectFromDTB(string outputPath, string title, bool createTitleSection, string id,
                                              string importDTBPath, int audioChannels, int audioSampleRate)
            {

                importDTBPath = System.IO.Path.GetFullPath(importDTBPath);

                importDTBPath = System.IO.Path.GetFullPath(importDTBPath);
                mSession.CreateNewPresentationInBackend(outputPath, title, createTitleSection, id, mSettings, true, audioChannels,audioSampleRate);
                ImportExport.DAISY3_ObiImport import = new Obi.ImportExport.DAISY3_ObiImport(mSession, mSettings,
                                                                                             importDTBPath,
                                                                                             System.IO.Path.
                                                                                                 GetDirectoryName(
                                                                                                     outputPath), false,
                                                                                                     audioSampleRate == 44100? AudioLib.SampleRate.Hz44100: audioSampleRate == 22050?AudioLib.SampleRate.Hz22050: AudioLib.SampleRate.Hz11025 ,
                                                                                             audioChannels==
                                                                                             2);
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
                
                                
                ProgressDialog progress = new ProgressDialog(Localizer.Message("import_progress_dialog_title"),
                                                             delegate(ProgressDialog progress1)
                                                                 {
                                                                     import.DoWork();
                                                                     import.CorrectExternalAudioMedia();
                                                                     ImportStructureFromXHtml_ThreadSafe(path, title, id,
                                                                                                         xhtmlPath, audioChannels,audioSampleRate);
                                                                 });
                progress.OperationCancelled += new OperationCancelledHandler(delegate(object sender, EventArgs e)
                                                                                 {
                                                                                     if (import != null) import.RequestCancellation = true;
                                                                                         
                                                                                 });
                import.ProgressChangedEvent +=
                    new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                progress.ShowDialog();
                if (progress.Exception != null) throw progress.Exception;
                
                Dialogs.ReportDialog reportDialog = new ReportDialog(Localizer.Message("Report_for_import"),
                    import.RequestCancellation ? Localizer.Message("import_cancelled")
                                                                     : String.Format(
                                                                         Localizer.Message("import_output_path"),
                                                                         import != null && import.ErrorsList.Count > 0 ? Localizer.Message("ImportErrorCorrectionText") : "",
                                                                         path),
                                                                 import != null ? import.ErrorsList : null);
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
                if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive)
                    mProjectView.TransportBar.Stop();
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("obi_filter");
                dialog.InitialDirectory = mSettings.DefaultPath;
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
            public static bool CheckProjectDirectory(string path, bool checkEmpty)
            {
                return Directory.Exists(path) ? CheckEmpty(path, checkEmpty) : DidCreateDirectory(path, true);
            }

            /// <summary>
            /// Check that a directory can host a project or export. Safe version.
            /// </summary>
            public static bool CheckProjectDirectory_Safe(string path, bool checkEmpty)
            {
                bool check = false;
                try
                {
                    check = CheckProjectDirectory(path, checkEmpty);
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
                        mProjectView.TransportBar.Stop();

                    // Freeze restore should return only if the function do not return null
                    if (FreezeChangesFromProjectRestore() != null)
                    {
                        m_IsSaveActive = false;
                        return;
                    }

                    mSession.Save();


                    mStatusLabel.Text = Localizer.Message("Status_ProjectSaved");
                    // reset the  auto save timer
                    mAutoSaveTimer.Stop();
                    if (mSettings.AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();
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

                    if (mProjectView.TransportBar.IsRecorderActive) mProjectView.TransportBar.Stop();
                    DateTime currentDateTime= DateTime.Now ;
                    string postFix = currentDateTime.Year.ToString() + "-"
                        + (currentDateTime.Month.ToString().Length > 1 ? currentDateTime.Month.ToString() : "0" + currentDateTime.Month.ToString())  + "-"
                        + (currentDateTime.Day.ToString().Length > 1 ? currentDateTime.Day.ToString() : "0" + currentDateTime.Day.ToString()) 
                        + "-" + currentDateTime.Hour.ToString() + "hr";
                    string backUpFileCopyAtInterval = Path.Combine(Path.GetDirectoryName(mSession.BackUpPath), postFix + Path.GetFileName(mSession.BackUpPath) );
                        
                    Console.WriteLine(backUpFileCopyAtInterval);
                    if (File.Exists(mSession.BackUpPath) && !File.Exists(backUpFileCopyAtInterval))
                    {
                        try
                        {
                            File.Move(mSession.BackUpPath, backUpFileCopyAtInterval);
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
                    if (mSettings.AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();

                    mFile_SaveProjectMenuItem.Enabled = true;
                    mFile_SaveProjectAsMenuItem.Enabled = true;
                    m_IsSaveActive = false;
                }
            }


            // Save the current project under a different name; ask for a new path first.
            private void SaveAs()
            {
                mProjectView.TransportBar.Stop();
                string path_original = mSession.Path;
                SaveProjectAsDialog dialog = new SaveProjectAsDialog(path_original);
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
                                                                         });
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

                if (!(FreezeChangesFromProjectRestore() ?? true)) return false;
                if (!SaveProjectAndBookmarkOptionally()) return false;
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
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
                                  mProjectView.Selection.Node), !mSession.CanClose);
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

            // Clean unwanted audio from the project.
            // Before continuing, the user is given the choice to save or cancel.
            private void CleanProject()
            {
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
                    try
                    {


                        string dataFolderPath = mSession.Presentation.DataProviderManager.DataFileDirectoryFullPath;

                        string deletedDataFolderPath = Path.Combine(dataFolderPath,
                                                                    "__DELETED" + Path.DirectorySeparatorChar);
                        if (!Directory.Exists(deletedDataFolderPath))
                        {
                            Directory.CreateDirectory(deletedDataFolderPath);
                        }

                        Cleaner cleaner = new Cleaner(mSession.Presentation, deletedDataFolderPath);
                        Dialogs.ProgressDialog progress = new ProgressDialog(Localizer.Message("cleaning_up"),
                                                                             delegate()
                                                                                 {
                                                                                     m_CanAutoSave = false;
                                                                                     //mSession.Presentation.cleanup();
                                                                                     try
                                                                                     {
                                                                                         cleaner.Cleanup();
                                                                                     }
                                                                                     catch (Exception e)
                                                                                     {
                                                                                         MessageBox.Show(e.ToString());
                                                                                     }
                                                                                     List<string>
                                                                                         listOfDataProviderFiles =
                                                                                             new List<string>();
                                                                                     foreach (
                                                                                         DataProvider dataProvider in
                                                                                             mSession.Presentation.
                                                                                                 DataProviderManager.
                                                                                                 ManagedObjects.
                                                                                                 ContentsAs_Enumerable)
                                                                                     {
                                                                                         FileDataProvider
                                                                                             fileDataProvider =
                                                                                                 dataProvider as
                                                                                                 FileDataProvider;
                                                                                         if (fileDataProvider == null)
                                                                                             continue;

                                                                                         listOfDataProviderFiles.Add(
                                                                                             Path.GetFileName(
                                                                                                 fileDataProvider.
                                                                                                     DataFileRelativePath));
                                                                                     }


                                                                                     //foreach (urakawa.media.data.MediaData m in mProjectView.Presentation.MediaDataManager.ManagedObjects.ContentsAs_Enumerable)
                                                                                     //{
                                                                                     //if ( listOfDataProviderFiles.Contains ( m.UsedDataProviders.l

                                                                                     //}

                                                                                     bool folderIsShowing = false;
                                                                                     if (
                                                                                         Directory.GetFiles(
                                                                                             deletedDataFolderPath).
                                                                                             Length != 0)
                                                                                     {
                                                                                         folderIsShowing = true;

                                                                                         //m_ShellView.ExecuteShellProcess(deletedDataFolderPath);
                                                                                     }

                                                                                     foreach (
                                                                                         string filePath in
                                                                                             Directory.GetFiles(
                                                                                                 dataFolderPath))
                                                                                     {
                                                                                         string fileName =
                                                                                             Path.GetFileName(filePath);
                                                                                         if (
                                                                                             !listOfDataProviderFiles.
                                                                                                  Contains(fileName))
                                                                                         {
                                                                                             string filePathDest =
                                                                                                 Path.Combine(
                                                                                                     deletedDataFolderPath,
                                                                                                     fileName);
                                                                                             Debug.Assert(
                                                                                                 !File.Exists(
                                                                                                     filePathDest));
                                                                                             if (
                                                                                                 !File.Exists(
                                                                                                     filePathDest))
                                                                                             {
                                                                                                 File.Move(filePath,
                                                                                                           filePathDest);
                                                                                                 Console.WriteLine(
                                                                                                     filePath);
                                                                                             }
                                                                                         }
                                                                                     }

                                                                                     if (
                                                                                         Directory.GetFiles(
                                                                                             deletedDataFolderPath).
                                                                                             Length != 0)
                                                                                     {
                                                                                         if (
                                                                                             mSettings.
                                                                                                 Project_AutomaticallyDeleteUnusedFilesAfterCleanup ||
                                                                                             MessageBox.Show(
                                                                                                 Localizer.Message(
                                                                                                     "clean_up_ask_for_delete_project"),
                                                                                                 Localizer.Message(
                                                                                                     "Delete_unused_data_caption"),
                                                                                                 MessageBoxButtons.YesNo,
                                                                                                 MessageBoxIcon.Question) ==
                                                                                             DialogResult.Yes)
                                                                                         {

                                                                                             if (true)
                                                                                                 //delete definitively
                                                                                             {
                                                                                                 ProjectView.ProjectView.WriteToLogFile_Static ("Clean up operation: deleting files");
                                                                                                 foreach (
                                                                                                     string filePath in
                                                                                                         Directory.
                                                                                                             GetFiles(
                                                                                                                 deletedDataFolderPath)
                                                                                                     )
                                                                                                 {
                                                                                                     File.Delete(
                                                                                                         filePath);
                                                                                                 }
                                                                                             }

                                                                                             if (
                                                                                                 Directory.Exists(
                                                                                                     deletedDataFolderPath))
                                                                                             {
                                                                                                 Directory.Delete(
                                                                                                     deletedDataFolderPath);
                                                                                             }
                                                                                         }
                                                                                         else // show the delete folder
                                                                                         {
                                                                                             System.Diagnostics.Process.
                                                                                                 Start(
                                                                                                     deletedDataFolderPath);
                                                                                         }
                                                                                     }


                                                                                 });
                        if (cleaner != null)
                            cleaner.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
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

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("clean_failed_text"), e.Message),
                                        Localizer.Message("clean_failed_caption"), MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        m_CanAutoSave = true;
                    }
                    mProjectView.WaveformRendering_PauseOrResume(false);
                }
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
            private void ShowHelpFile()
            {
                try
                {
                    System.Diagnostics.Process.Start(
                        (new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                                              Localizer.Message("CHMhelp_file_name")))).ToString());
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
                System.Diagnostics.Process.Start(url.ToString());
            }

            // Help > About
            private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause();

                (new Dialogs.About()).ShowDialog();
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
                if (ShouldOpenLastProject) OpenProject_Safe(mSettings.LastOpenProject, null);
                if (!ShouldOpenLastProject && mShowWelcomWindow) ShowWelcomeDialog();

                UpdateKeyboardFocusForSelection();
                if (mSettings.ShowGraphicalPeakMeterAtStartup) ShowPeakMeter();
                
                if (mSettings.Project_CheckForUpdates) CheckForNewRelease(true);
                
            }


            private void CheckSystemSupportForMemoryOptimization()
            {
                if (!mSettings.OptimizeMemory) return;
                //System.Diagnostics.Stopwatch stopWatch = new Stopwatch();
                //stopWatch.Start();
                try
                {
                    if (!System.Diagnostics.PerformanceCounterCategory.CounterExists("Available MBytes", "Memory"))
                    {
                        
                        mSettings.OptimizeMemory = false;
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
                    mSettings.OptimizeMemory = false;

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
                        Dialogs.UserRegistration registrationDialog = new UserRegistration(m_Settings_Permanent);
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
                Dialogs.WelcomeDialog ObiWelcome = new WelcomeDialog(mSettings.LastOpenProject != "");
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
                if (mPeakMeter != null && !m_ShowingPeakMeter) 
                {
                    mSettings.ObiFormSize = Size;
                    
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
                        RemoveCustomClassFromMenu(customClass);
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
                mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
                mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
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
                mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
                mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
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
                mAddSectionToolStripMenuItem.Enabled = mProjectView.CanAddSection;
                mAddSubsectionToolStripMenuItem.Enabled = mProjectView.CanAddSubsection;
                mInsertSectionToolStripMenuItem.Enabled = mProjectView.CanInsertSection;
                mRenameSectionToolStripMenuItem.Enabled = mProjectView.CanRenameSection;
                mDecreaseSectionLevelToolStripMenuItem.Enabled = mProjectView.CanDecreaseLevel;
                mIncreaseSectionLevelToolStripMenuItem.Enabled = mProjectView.CanIncreaseLevel;
                mSplitSectionToolStripMenuItem.Enabled = mProjectView.CanSplitStrip;
                mMergeSectionWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeStripWithNext;
                mSectionIsUsedToolStripMenuItem.Enabled = mProjectView.CanSetSectionUsedStatus;
                mSectionIsUsedToolStripMenuItem.CheckedChanged -=
                    new System.EventHandler(mSectionIsUsedToolStripMenuItem_CheckedChanged);
                mSectionIsUsedToolStripMenuItem.Checked = mProjectView.CanMarkSectionUnused ||
                                                          mProjectView.CanMarkStripUnused;
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
                mAddEmptyPagesToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock &&
                                                          !mProjectView.TransportBar.IsRecorderActive;
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
                mPhraseDetectionToolStripMenuItem.Enabled = mSession.HasProject;
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
                mSkippableNoteToolStripMenuItem.Enabled = mSession.Presentation != null && !mProjectView.IsZoomWaveformActive;
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
                mPhrases_RenumberPagesMenuItem.Enabled = mProjectView.Presentation != null;
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
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = name;
                item.Click += new EventHandler(delegate(object sender, EventArgs e)
                                                   { mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Custom, name);
                                                   });
                items.Insert(index, item);
            }

            // Update the custom class menu to remove this class
            private void Presentation_CustomClassRemoved(object sender, CustomClassEventArgs e)
            {
                RemoveCustomClassFromMenu(e.CustomClass);
            }


            private void RemoveCustomClassFromMenu(string customClassName)
            {
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
                allEmptyPagesToolStripMenuItem.Enabled = mProjectView.CanGenerateSpeechForAllEmptyPages;
                selectedPageToolStripMenuItem.Enabled = mProjectView.CanGenerateSpeechForPage;
                mGenerateSpeechToolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mFastPlaytoolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
                mRecordToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.TransportBar.CanRecord;
                mStartRecordingDirectlyToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive;
                m_DeletePhrasesWhileRecordingtoolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive && mSettings.AllowOverwrite && mProjectView.TransportBar.CanRecord && !mProjectView.TransportBar.IsListening ;
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

            }

            private void mPlayAllToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (!mProjectView.IsZoomWaveformActive)//@zoomwaveform: if zoom waveform is not active, start play all else start play selection
                {
                    mProjectView.TransportBar.PlayAll();
                }
                else
                {
                    mProjectView.TransportBar.PlayOrResume();
                }
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
                mProjectView.TransportBar.StartRecordingDirectly();
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
                mTools_ExportSelectedAudioMenuItem.Enabled = mProjectView.CanExportSelectedNodeAudio;
                mTools_ExportAsDAISYMenuItem.Enabled = mSession.HasProject;
                mTools_CleanUnreferencedAudioMenuItem.Enabled = mSession.HasProject &&
                                                                !mProjectView.TransportBar.IsRecorderActive;
                mTools_PreferencesMenuItem.Enabled = !mProjectView.TransportBar.IsRecorderActive;
                PipelineMenuItemsEnabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
            }

            // Open the preferences dialog
            private void mTools_PreferencesMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause();
                bool isLeftAlignPhrasesInContentView = mSettings.LeftAlignPhrasesInContentView;
                bool showWaveform = mSettings.Project_ShowWaveformInContentView;
                bool enableEmptySectionColorInTOC = mSettings.Project_BackgroundColorForEmptySection;
                Dialogs.Preferences prefs = new Dialogs.Preferences(this, mSettings, mSession.Presentation,
                                                                    mProjectView.TransportBar, m_DefaultSettings);
                prefs.ShowDialog();
                if (prefs.IsColorChanged)
                    UpdateColors();
                Ready();
                mProjectView.TransportBar.UpdateButtons();
                //if (mProjectView.Presentation != null && mProjectView.Presentation.FirstSection != null && enableEmptySectionColorInTOC != mSettings.Project_BackgroundColorForEmptySection)
                //{
                //    mProjectView.UpdateTOCBackColorForEmptySection((SectionNode)mProjectView.Presentation.FirstSection);
                //}
                mProjectView.TransportBar.ResetFastPlayForPreferencesChange();
                if (isLeftAlignPhrasesInContentView != mSettings.LeftAlignPhrasesInContentView) UpdateZoomFactor();
                mSession.EnableFreeDiskSpaceCheck = mSettings.Project_EnableFreeDiskSpaceCheck;
                if (showWaveform != mSettings.Project_ShowWaveformInContentView)
                {
                    mProjectView.RecreateStrip();
                }
            }

            private void mTools_ExportAsDAISYMenuItem_Click(object sender, EventArgs e)
            {
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

                string exportDirectoryDAISY3 = "";
                string exportDirectoryDAISY202 = "";

                Dialogs.chooseDaisy3orDaisy202 chooseDialog = new chooseDaisy3orDaisy202();
                chooseDialog.bothOptionEnabled = true;
                if (chooseDialog.ShowDialog() == DialogResult.OK)
                {
                    if (chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.DAISY3_0 || chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202)
                    {
                        exportDirectoryDAISY3 = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(Localizer.Message("default_export_dirname"), "")));
                    }
                    if (chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.DAISY2_02 || chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202)
                    {
                        exportDirectoryDAISY202 = Path.Combine(Directory.GetParent(mSession.Path).FullName,
                                                       Program.SafeName(
                                                           string.Format(
                                                               Localizer.Message("Default_DAISY2_02export_dirname"), "")));
                    }
                }
                if ( string.IsNullOrEmpty(exportDirectoryDAISY3 ) && string.IsNullOrEmpty(exportDirectoryDAISY202))
                {
                    return;
                }

                mProjectView.TransportBar.Enabled = false;

                List<string> navList = new List<string>();
                navList.Add(EmptyNode.Annotation);
                navList.Add(EmptyNode.EndNote);
                navList.Add(EmptyNode.Footnote);
                navList.Add(EmptyNode.Note);
                navList.Add(EmptyNode.Sidebar);
                navList.Add(EmptyNode.ProducerNote);
                if (CheckedPageNumbers() && CheckedForEmptySectionsAndAnchoredReferences())
                {
                    //Dialogs.ExportDirectory dialog =
                    //new ExportDirectory(Path.Combine(Directory.GetParent(mSession.Path).FullName,
                    //Program.SafeName(string.Format(Localizer.Message("default_export_dirname"),
                    //"" ) ) ), mSession.Path ); // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline

                    Dialogs.ExportDirectory ExportDialogDAISY3 = null;
                    Dialogs.ExportDirectory ExportDialogDAISY202 = null;

                    if (chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.DAISY3_0 || chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202)
                    {
                        ExportDialogDAISY3 =
                            new ExportDirectory(exportDirectoryDAISY3,
                                                mSession.Path, mSettings.Export_EncodeToMP3, mSettings.Export_BitRateMP3,
                                                mSettings.Export_AppendSectionNameToAudioFile);
                        // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                        ExportDialogDAISY3.AdditionalTextForTitle = "DAISY 3";
                        ExportDialogDAISY3.LimitLengthOfAudioFileNames = mSettings.Export_LimitAudioFilesLength &&
                                                             mSettings.Export_AppendSectionNameToAudioFile;
                        ExportDialogDAISY3.AudioFileNameCharsLimit = Settings.Export_AudioFilesNamesLengthLimit >= 0 ? Settings.Export_AudioFilesNamesLengthLimit : 8;
                        if (ExportDialogDAISY3.ShowDialog() != DialogResult.OK) ExportDialogDAISY3  = null;
                    }
                    

                    if (chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.DAISY2_02 || chooseDialog.chooseOption == Obi.ImportExport.ExportFormat.Both_DAISY3_DAISY202)
                    {
                        ExportDialogDAISY202 =
                            new ExportDirectory(exportDirectoryDAISY202,
                                                mSession.Path, mSettings.Export_EncodeToMP3, mSettings.Export_BitRateMP3,
                                                mSettings.Export_AppendSectionNameToAudioFile);
                        // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                        ExportDialogDAISY202.AdditionalTextForTitle = "DAISY 2.02";
                        ExportDialogDAISY202.LimitLengthOfAudioFileNames = mSettings.Export_LimitAudioFilesLength &&
                                                             mSettings.Export_AppendSectionNameToAudioFile;
                        ExportDialogDAISY202.AudioFileNameCharsLimit = Settings.Export_AudioFilesNamesLengthLimit >= 0 ? Settings.Export_AudioFilesNamesLengthLimit : 8;
                        if (ExportDialogDAISY202.ShowDialog() != DialogResult.OK) ExportDialogDAISY202 = null;
                    }

                    if (ExportDialogDAISY3 != null ||  ExportDialogDAISY202 != null )
                    {
                        try
                        {
                            // Need the trailing slash, otherwise exported data ends up in a folder one level
                            // higher than our selection.
                            string exportPathDAISY3 = ExportDialogDAISY3 != null?  ExportDialogDAISY3.DirectoryPath: null;
                            string exportPathDAISY202 = ExportDialogDAISY202!= null?  ExportDialogDAISY202.DirectoryPath: null;


                            Dialogs.ExportDirectory dialog = ExportDialogDAISY3 != null ? ExportDialogDAISY3 : ExportDialogDAISY202;
                            mSettings.Export_EncodeToMP3 = dialog.EncodeToMP3;
                            mSettings.Export_BitRateMP3 = dialog.BitRate;
                            mSettings.Export_AppendSectionNameToAudioFile = dialog.AppendSectionNameToAudioFileName;
                            mSettings.Export_LimitAudioFilesLength = dialog.AppendSectionNameToAudioFileName &&
                                                                     dialog.LimitLengthOfAudioFileNames;
                            mSettings.Export_AudioFilesNamesLengthLimit = dialog.AudioFileNameCharsLimit;

                            if (!string.IsNullOrEmpty(exportPathDAISY3) &&  !exportPathDAISY3.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            {
                                exportPathDAISY3 += Path.DirectorySeparatorChar;
                            }

                            if (!string.IsNullOrEmpty(exportPathDAISY202) &&  !exportPathDAISY202.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            {
                                exportPathDAISY202 += Path.DirectorySeparatorChar;
                            }

                            urakawa.daisy.export.Daisy3_Export DAISY3Export = null;
                            urakawa.daisy.export.Daisy3_Export DAISY202Export = null;

                            if (ExportDialogDAISY3 != null)
                            {
                                DAISY3Export = new Obi.ImportExport.DAISY3_ObiExport(
                                    mSession.Presentation, exportPathDAISY3, null, ExportDialogDAISY3.EncodeToMP3, (ushort) ExportDialogDAISY3.BitRate,
                                    AudioLib.SampleRate.Hz44100,
                                    mProjectView.Presentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels == 2,
                                    false, ExportDialogDAISY3.LevelSelection);

                                DAISY3Export.AddSectionNameToAudioFile = ExportDialogDAISY3.AppendSectionNameToAudioFileName;
                                DAISY3Export.AudioFileNameCharsLimit = ExportDialogDAISY3.AudioFileNameCharsLimit;
                                if(ExportDialogDAISY3.EnabledAdvancedParameters)  DAISY3Export.SetAdditionalMp3EncodingParameters(ExportDialogDAISY3.Mp3ChannelMode, ExportDialogDAISY3.Mp3ReSample, ExportDialogDAISY3.Mp3RePlayGain);
                            }
                            if (ExportDialogDAISY202 != null)
                            {
                                DAISY202Export = new Obi.ImportExport.DAISY202Export(
                                    mSession.Presentation, exportPathDAISY202, ExportDialogDAISY202.EncodeToMP3, (ushort) ExportDialogDAISY202.BitRate,
                                    AudioLib.SampleRate.Hz44100, mSettings.AudioChannels == 2,
                                    ExportDialogDAISY202.LevelSelection);

                                DAISY202Export.AddSectionNameToAudioFile = ExportDialogDAISY202.AppendSectionNameToAudioFileName;
                                DAISY202Export.AudioFileNameCharsLimit = ExportDialogDAISY202.AudioFileNameCharsLimit;
                                if(ExportDialogDAISY202.EnabledAdvancedParameters)  DAISY202Export.SetAdditionalMp3EncodingParameters(ExportDialogDAISY202.Mp3ChannelMode, ExportDialogDAISY202.Mp3ReSample, ExportDialogDAISY202.Mp3RePlayGain);
                                ((Obi.ImportExport.DAISY202Export) DAISY202Export).AlwaysIgnoreIndentation = mSettings.Export_AlwaysIgnoreIndentation;
                            }
                            //DAISYExport.BitRate_Mp3 = dialog.BitRate;
                            

                            
                            //DAISYExport.EnableExplicitGarbageCollection = Settings.OptimizeMemory;
                            Status(String.Format(Localizer.Message("ObiFormStatusMsg_ExportingProject"), exportPathDAISY3));

                            ProgressDialog progress =
                                new ProgressDialog(Localizer.Message("export_progress_dialog_title"),
                                                   delegate(ProgressDialog progress1)
                                                       {
                                                           
                                                               if (DAISY3Export != null)
                                                               {
                                                                   
                                                                   mSession.Presentation.ExportToZ(exportPathDAISY3, mSession.Path,
                                                                                                   DAISY3Export);
                                                               }
                                                               if (DAISY202Export != null)
                                                               {
                                                                   mSession.Presentation.ExportToZ(exportPathDAISY202, mSession.Path,
                                                                                               DAISY202Export);
                                                               }
                                                           
                                                       });

                            progress.OperationCancelled +=
                                new Obi.Dialogs.OperationCancelledHandler(
                                    delegate(object sender, EventArgs e) { DAISY3Export.RequestCancellation = true; });
                            if(DAISY3Export!= null)   DAISY3Export.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            if(DAISY202Export!= null)   DAISY202Export.ProgressChangedEvent +=
                                new System.ComponentModel.ProgressChangedEventHandler(progress.UpdateProgressBar);
                            progress.ShowDialog();
                            if (progress.Exception != null) throw progress.Exception;

                            if ((DAISY3Export != null && DAISY3Export.RequestCancellation)
                                || (DAISY202Export != null && DAISY202Export.RequestCancellation))
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
                            mSession.ForceSave();

                            string displayPath = (exportPathDAISY3 != null && exportPathDAISY202 != null ) ? exportPathDAISY3 + "\n" + exportPathDAISY202:
                                exportPathDAISY3 != null? exportPathDAISY3: exportPathDAISY202;
                            MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"),displayPath),
                                            Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }
                        catch (Exception e)
                        {
                            string displayPath = (ExportDialogDAISY3 != null && ExportDialogDAISY202 != null) ? ExportDialogDAISY3.DirectoryPath + "\n" + ExportDialogDAISY202.DirectoryPath :
                                ExportDialogDAISY3 != null ? ExportDialogDAISY3.DirectoryPath : ExportDialogDAISY202.DirectoryPath;
                            MessageBox.Show(
                                String.Format(Localizer.Message("didnt_save_as_daisy_text"), displayPath,
                                              e.Message),
                                Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            Status(Localizer.Message("didnt_save_as_daisy_caption"));
                        }
                    }
                }
                mProjectView.TransportBar.Enabled = true;
                Ready();
            }

            private void mTools_CleanUnreferencedAudioMenuItem_Click(object sender, EventArgs e)
            {
                CleanProject();
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
                                    Dialogs.EmptySection dialog = new Dialogs.EmptySection(s.Label);
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
                    if (mSettings.AutoSaveTimeIntervalEnabled) mAutoSaveTimer.Start();
                    m_CanAutoSave = true; //@singleSection
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
                    if (Directory.Exists(mSettings.PipelineScriptsPath))
                    {
                        mPipelineInfo = new PipelineInterface.PipelineInfo(mSettings.PipelineScriptsPath);
                        PopulatePipelineScriptsInToolsMenu();
                    }
                    else
                    {
                        if (!ExtractPipelineLite())
                            MessageBox.Show(string.Format(Localizer.Message("ObiForm_PipelineNotFound"),
                                                          mSettings.PipelineScriptsPath));
                    }
                    Ready();
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
                mAutoSaveTimer.Interval = mSettings.AutoSaveTimeInterval;
                mAutoSaveTimer.Tick += new EventHandler(mAutoSaveTimer_Tick);
            }

            private void mAutoSaveTimer_Tick(object sender, EventArgs e)
            {
                if (!mSettings.AutoSaveTimeIntervalEnabled) return;

                if (!m_CanAutoSave
                    && mSession.CanSave) //@singleSection
                {
                    //keep on checking after interval of 5 seconds if CanAutoSave is true
                    mAutoSaveTimer.Interval = 5000;
                    return;
                }
                else
                {
                    mAutoSaveTimer.Interval = mSettings.AutoSaveTimeInterval;
                }

                if (mSession != null && mSession.Presentation != null
                    && mSettings.AutoSaveTimeIntervalEnabled && m_CanAutoSave && mSession.CanSave)
                {
                    //if (mProjectView.TransportBar.CurrentState != Obi.ProjectView.TransportBar.State.Recording)
                    //{
                    m_IsAutoSaveActive = true;
                    SaveToBackup();
                    m_IsAutoSaveActive = false;
                    Console.WriteLine("auto save executed ");
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
                if (mSettings.AutoSaveTimeIntervalEnabled)
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
                        System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(path));
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

                                                                                         });
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
                    if (backupFilePaths.Length > 23 || 
                        (backupFilePaths.Length > 1 &&  allExceptPrimaryBackup))
                    {
                        List<string> backUpFilesList = new List<string>();
                        for (int i = 0; i < backupFilePaths.Length; i++) backUpFilesList.Add(backupFilePaths[i]);
                        backUpFilesList.Sort();
                        if (backUpFilesList.Contains(Path.GetFullPath(mSession.BackUpPath)))
                            backUpFilesList.Remove(Path.GetFullPath(mSession.BackUpPath));

                        int filesCountToDelete = allExceptPrimaryBackup ? backUpFilesList.Count : 
                            backUpFilesList.Count > 16? backUpFilesList.Count - 16 : 0;
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
                            mSettings.RecordingToolBarIncrementVal =
                              Obi.UserControls.RecordingToolBarForm.
                                   NetSizeIncrementOfButtons;
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
                    Obi.UserControls.RecordingToolBarForm.NetSizeIncrementOfButtons =
                            mSettings.RecordingToolBarIncrementVal;
                    mRecordingToolBarForm.EnlargeButtonSize();




                    mRecordingToolBarForm.Location = new System.Drawing.Point(this.Location.X,
                                                                              (this.Location.Y + this.Size.Height) -
                                                                              mRecordingToolBarForm.Size.Height);
                    this.WindowState = FormWindowState.Minimized;
                    mView_RecordingToolBarMenuItem.Checked = true;
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
            private void ShowPeakMeter()
            {
                if (mPeakMeter == null)
                {
                    mPeakMeter = new Obi.Audio.PeakMeterForm();
                    if (mSettings.PeakmeterSize.Width == 0 || mSettings.PeakmeterSize.Height == 0 || mSettings.PeakmeterSize.Height < mSettings.GraphicalPeakMeterContolSize.Height)
                    {
                        mPeakMeter.Size = new Size(170, 640);
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
                                                                                mSettings.PeakmeterSize =
                                                                                    mPeakMeter.Size;
                                                                                mPeakMeter.GraphicalPeakMeterSaveSettings(mSettings);
                                                                                mPeakMeter = null;
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

            // Undo
            private void Undo()
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
                    Text = mSession.HasProject
                               ? String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                                               (mSession.CanSave ? "*" : ""), mSession.Path, Localizer.Message("obi"))
                               : Localizer.Message("obi");
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
                if (mProjectView != null && mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
                if (DidCloseProject())
                {
                    mSettings.ShowGraphicalPeakMeterAtStartup = mPeakMeter != null;
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
                    mProjectView.TransportBar.AudioPlayer.SetOutputDevice(this, mSettings.LastOutputDevice);
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
                    mProjectView.TransportBar.Recorder.SetInputDevice(mSettings.LastInputDevice);
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
                
                if (mSettings.ObiFormSize.Width == 0 || mSettings.ObiFormSize.Height == 0)
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
                AllowOverwrite = mSettings.AllowOverwrite;
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
                    MessageBox.Show(
                        Localizer.Message("KeyboardShortcuts_ErrorInLoadingConfiguredKeys") + "\n" + ex.ToString(),
                        Localizer.Message("Caption_Error"));
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

            private void UpdateColors()
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
            public static bool CheckEmpty(string path, bool checkEmpty)
            {
                if (checkEmpty &&
                    (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0))
                {
                    DialogResult result = MessageBox.Show(
                        String.Format(Localizer.Message("really_use_directory_text"), path),
                        Localizer.Message("really_use_directory_caption"),
                        // MessageBoxButtons.YesNoCancel,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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
                ZoomFactor = 1.0f;
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
                ProjectView.Strip strip = mProjectView.StripForSelection;
                if (strip == null)
                {
                    AudioScale *= AUDIO_SCALE_INCREMENT;
                }
                else if (strip.AudioScale < 0.1f)
                {
                    strip.AudioScale *= AUDIO_SCALE_INCREMENT;
                }
                mView_AudioZoomInMenuItem.Enabled = mSession.HasProject;
            }

            private void mView_AudioZoomOutMenuItem_Click(object sender, EventArgs e)
            {
                if (mProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                    mProjectView.TransportBar.Pause();
                if (!CheckAndAlertForMemoryUsage()) return;

                mView_AudioZoomOutMenuItem.Enabled = false;
                ProjectView.Strip strip = mProjectView.StripForSelection;
                if (strip == null)
                {
                    AudioScale /= AUDIO_SCALE_INCREMENT;
                }
                else if (strip.AudioScale > 0.002f)
                {
                    strip.AudioScale /= AUDIO_SCALE_INCREMENT;
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
                if (Settings == null || !Settings.OptimizeMemory) return true;
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
                Dialogs.chooseDaisy3orDaisy202 rdfrm = new Dialogs.chooseDaisy3orDaisy202();
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

                //AudioScale = AudioScale;
                AudioScale = 0.01f;
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
                            Directory.GetParent(mSession.Path).FullName);
                    ProgressDialog progress = new ProgressDialog(((ToolStripMenuItem) sender).Text,
                                                                 delegate() { pipeline.RunScript(); });

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
                mProjectView.GoToPageOrPhrase();
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
                ((EmptyNode) mProjectView.Selection.Node).Role_ = EmptyNode.Role.Anchor;
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
                mProjectView.TransportBar.RecordWithDeleteFollowing();
            }



            private void mRecordToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
            {
                m_DeletePhrasesWhileRecordingtoolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive && mSettings.AllowOverwrite && mProjectView.TransportBar.CanRecord && !mProjectView.TransportBar.IsListening;
                if (mSettings.AllowOverwrite)
                {
                    
                    mAllowOverwriteToolStripMenuItem.Checked = true;
                }
                else
                {
                    
                    mAllowOverwriteToolStripMenuItem.Checked = false;
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
                        EmptyNode startNode = mProjectView.Selection != null ? (mProjectView.Selection.Node is EmptyNode? (EmptyNode)mProjectView.Selection.Node : mProjectView.Selection.Node is SectionNode && ((SectionNode)mProjectView.Selection.Node).PhraseChildCount > 0? (EmptyNode)((SectionNode)mProjectView.Selection.Node).FirstLeaf:null):
                                        mProjectView.Presentation.FirstSection != null ? (EmptyNode)mProjectView.Presentation.FirstSection.FirstLeaf :
                                        null;

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
                        Dialogs.SetPageNumber dialog = new Dialogs.SetPageNumber(num, false, false);
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





        }
    }