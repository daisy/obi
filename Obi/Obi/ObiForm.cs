using Obi.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using urakawa.core;


namespace Obi
{
    /// <summary>
    /// The main form of the application.
    /// This is basically the shell of the project view along with menu bars.
    /// </summary>
    public partial class ObiForm : Form
    {
        #region Members and initializers

        private float mBaseFontSize;             // base font size
        private Audio.PeakMeterForm mPeakMeter;  // maintain a single "peak meter" form
        private Session mSession;                // current work session
        private Settings mSettings;              // application settings
        private Dialogs.ShowSource mSourceView;  // maintain a single "source view" dialog
        private PipelineInterface.PipelineInfo mPipelineInfo; // instance for easy access to pipeline information
        private bool mShowWelcomWindow; // flag for controlling showing of welcome window
        private Timer mAutoSaveTimer;

        private static readonly float ZOOM_FACTOR_INCREMENT = 1.2f;   // zoom factor increment (zoom in/out)
        private static readonly float DEFAULT_ZOOM_FACTOR_HC = 1.2f;  // default zoom factor (high contrast mode)
        private static readonly float AUDIO_SCALE_INCREMENT = 1.2f;   // audio scale increment (audio zoom in/out)

        /// <summary>
        /// Initialize a new form and open the last project if set in the preferences.
        /// </summary>
        public ObiForm()
        {
            mShowWelcomWindow = true;
            InitializeObi();
            if (ShouldOpenLastProject) OpenProject_Safe(mSettings.LastOpenProject);
                    }

        /// <summary>
        /// Initialize a new form and open a project from the path given as parameter.
        /// </summary>
        public ObiForm(string path)
        {
            mShowWelcomWindow = false;
            InitializeObi();
            OpenProject_Safe(path);
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
                if (value > 0.0)
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
                ColorSettings settings = SystemInformation.HighContrast ?
                    (mSettings == null ? ColorSettings.DefaultColorSettingsHC() : mSettings.ColorSettingsHC) :
                    (mSettings == null ? ColorSettings.DefaultColorSettings() : mSettings.ColorSettings);
                settings.CreateBrushesAndPens();
                return settings;
            }
        }

        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings { get { return mSettings; } }

        // True if the user has chosen the "open last project" option, and there is a last project to open.
        private bool ShouldOpenLastProject
        {
            get
            {
                return mSettings != null && mSettings.OpenLastProject && mSettings.LastOpenProject != "";
            }
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
                mWrappingInContentViewToolStripMenuItem.Checked = value;
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
                if (value > 0.0)
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
            float z = mSettings.ZoomFactor * (SystemInformation.HighContrast ? DEFAULT_ZOOM_FACTOR_HC : 1.0f);
            mStatusLabel.Font = new System.Drawing.Font(mStatusLabel.Font.FontFamily, mBaseFontSize * z);
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
            mFile_SaveProjectMenuItem.Enabled = mSession.CanSave;
            mFile_SaveProjectAsMenuItem.Enabled = mSession.HasProject;
            mFile_RecentProjectMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            mFile_ClearListMenuItem.Enabled = true;
            mFile_ExitMenuItem.Enabled = true;
        }

        private void File_NewProjectMenuItem_Click(object sender, EventArgs e) { NewProject(); }
        private void File_NewProjectFromImportMenuItem_Click(object sender, EventArgs e) { NewProjectFromImport(); }
        private void File_OpenProjectMenuItem_Click(object sender, EventArgs e) { Open(); }
        private void File_CloseProjectMenuItem_Click(object sender, EventArgs e) { DidCloseProject(); }
        private void File_SaveProjectMenuItem_Click(object sender, EventArgs e) { Save(); }
        private void File_SaveProjectAsMenuItem_Click(object sender, EventArgs e) { SaveAs(); }
        private void File_ClearListMenuItem_Click(object sender, EventArgs e) { ClearRecentProjectsList(); }
        private void File_ExitMenuItem_Click(object sender, EventArgs e) { Close(); }

        // Create a new project by asking initial information through a dialog.
        private void NewProject()
        {
            if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            if (DidCloseProject())
            {
                Dialogs.NewProject dialog = new Dialogs.NewProject(
                    mSettings.DefaultPath,
                    Localizer.Message("default_project_filename"),
                    Localizer.Message("obi_filter"),
                    Localizer.Message("default_project_title"),
                    mSettings.NewProjectDialogSize);
                dialog.CreateTitleSection = mSettings.CreateTitleSection;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection, dialog.ID);
                }
                mSettings.CreateTitleSection = dialog.CreateTitleSection;
                mSettings.NewProjectDialogSize = dialog.Size;
            }
        }

        // Create a new project by importing an XHTML file.
        // Prompt the user for the location of the file through a dialog.
        private void NewProjectFromImport()
        {
            if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            if (DidCloseProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = Localizer.Message("choose_import_file");
                dialog.Filter = Localizer.Message("xhtml_filter");
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
                string title = ImportStructure.GrabTitle(new Uri(path));
                dialog = new Dialogs.NewProject(
                    mSettings.DefaultPath,
                    Localizer.Message("default_project_filename"),
                    Localizer.Message("obi_filter"),
                    title,
                    mSettings.NewProjectDialogSize);
                dialog.DisableAutoTitleCheckbox();
                dialog.Text = Localizer.Message("create_new_project_from_import");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mSettings.NewProjectDialogSize = dialog.Size;
                    CreateNewProject(dialog.Path, dialog.Title, false, dialog.ID);
                    ProgressDialog progress = new ProgressDialog(Localizer.Message("import_progress_dialog_title"),
                        delegate() { (new ImportStructure()).ImportFromXHTML(path, mSession.Presentation); });
                    progress.ShowDialog();
                    if (progress.Exception != null) throw progress.Exception;
                    mSession.ForceSave();
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

        // Open a new project after showing a file open dialog.
        private void Open()
        {
            if (mProjectView.Presentation != null && mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Localizer.Message("obi_filter");
            dialog.InitialDirectory = mSettings.DefaultPath;
            if (dialog.ShowDialog() == DialogResult.OK && DidCloseProject()) OpenProject_Safe(dialog.FileName);
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
            catch (Exception) { }
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
            if (!Path.IsPathRooted(path)) throw new Exception(string.Format(Localizer.Message("path_not_rooted"), path));
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
                    MessageBoxIcon.Warning , MessageBoxDefaultButton.Button2) == DialogResult.Yes;
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
            catch (Exception) { }
            return check;
        }

        // Save the current project
        public void Save()
        {
        if (mSession != null && mSession.CanSave)
            {
            if (mProjectView.TransportBar.IsPlayerActive || mProjectView.TransportBar.IsRecorderActive) mProjectView.TransportBar.Stop ();

            mSession.Save ();

            mStatusLabel.Text = Localizer.Message ( "Status_ProjectSaved" );
            // reset the  auto save timer
            mAutoSaveTimer.Stop ();
            if ( mSettings.AutoSaveTimeIntervalEnabled )  mAutoSaveTimer.Start ();
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
                            DirectoryInfo dir_original = new DirectoryInfo(Path.GetDirectoryName(path_original));
                            DirectoryInfo dir_new = new DirectoryInfo(Path.GetDirectoryName(path_new));
                            ShallowCopyFilesInDirectory(dir_original.FullName, dir_new.FullName);
                            mSession.RemoveLock_Additional_safe (path_new);
                            DirectoryInfo[] dirs = dir_original.GetDirectories("*.*", SearchOption.AllDirectories);
                            foreach (DirectoryInfo d in dirs)
                            {
                                string dest = dir_new.FullName + d.FullName.Replace(dir_original.FullName, "");
                                if (!Directory.Exists(dest))
                                {
                                    Directory.CreateDirectory(dest);
                                    // copy files in each directory
                                    ShallowCopyFilesInDirectory(d.FullName, dest);
                                }
                            }
                            Uri prevUri = mSession.Presentation.getRootUri();
                            mSession.Presentation.setRootUri(new Uri(path_new));
                            mSession.Save(path_new);
                            mSession.Presentation.setRootUri(prevUri);
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
            if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            if (!mSession.CanClose)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return false;
                if (result == DialogResult.Yes) mSession.Save();
            }
            mSession.Close();
            return true;
        }

        // Clean unwanted audio from the project.
        // Before continuing, the user is given the choice to save or cancel.
        private void CleanProject()
        {
            if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            DialogResult result = MessageBox.Show(Localizer.Message("clean_save_text"),
                Localizer.Message("clean_save_caption"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                try
                {
                    Dialogs.ProgressDialog progress = new ProgressDialog(Localizer.Message("cleaning_up"),
                        delegate()
                        {
                            mSession.Presentation.cleanup();
                            DeleteExtraFiles();
                        });
                    progress.ShowDialog();
                    if (progress.Exception != null) throw progress.Exception;
                    mSession.ForceSave();
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Localizer.Message("clean_failed_text"), e.Message),
                        Localizer.Message("clean_failed_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Delete extra files in the data directory (or directories)
        private void DeleteExtraFiles()
        {
            Dictionary<string, Dictionary<string, bool>> dirs = new Dictionary<string, Dictionary<string, bool>>();
            // Get the list of files used by the asset manager
            foreach (urakawa.media.data.FileDataProvider provider in
                    ((urakawa.media.data.FileDataProviderManager)mSession.Presentation.getDataProviderManager()).
                getListOfManagedFileDataProviders())
            {
                string path = provider.getDataFileFullPath();
                string dir = Path.GetDirectoryName(path);
                if (!dirs.ContainsKey(dir)) dirs.Add(dir, new Dictionary<string, bool>());
                dirs[dir].Add(path, true);
            }
            // Go through each directory and remove files not used by the data manager
            // TODO at the moment, this removes everything; if we have other files that we need
            // (e.g. images of waveforms?) we need to be careful not to throw them away.
            foreach (string dir in dirs.Keys)
            {
                System.Diagnostics.Debug.Print("--- Cleaning up in {0}", dir);
                foreach (string path in Directory.GetFiles(dir))
                {
                    if (dirs[dir].ContainsKey(path))
                    {
                        System.Diagnostics.Debug.Print("=== Keeping {0}", path);
                    }
                    else
                    {
                        System.Diagnostics.Debug.Print("--- Deleting {0}", path);
                        File.Delete(path);
                    }
                }
            }
        }

        #endregion

        #region Help menu

        // Help > Contents (F1)
        private void mHelp_ContentsMenuItem_Click(object sender, EventArgs e) { ShowHelpFile(); }

        // View the help file in our own browser window.
        private void ShowHelpFile()
        {
                try
            {
            System.Diagnostics.Process.Start((new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message ( "CHMhelp_file_name" ) ) )).ToString () );
                        }
        catch (System.Exception ex)
            {
            System.Windows.Forms.MessageBox.Show ( ex.ToString () );
            return;
            }
        }

        // Help > View help in external browser (Shift+F1)
        private void mHelp_ViewHelpInExternalBrowserMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_file_name")))).ToString());
        }

        // Help > Report bug (Ctrl+Alt+R)
        private void mHelp_ReportBugMenuItem_Click(object sender, EventArgs e)
        {
        Uri url = new Uri (Localizer.Message( "Obi_Wiki_Url") );
            System.Diagnostics.Process.Start(url.ToString());
        }

        // Help > About
        private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e) 
            {
            if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause ();

            (new Dialogs.About()).ShowDialog(); 
            }

        #endregion


        #region Event handlers

        // Initialize event handlers from the project view
        private void InitializeEventHandlers()
        {
            mProjectView.TransportBar.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
            mProjectView.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
            mProjectView.TransportBar.Recorder.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler(TransportBar_StateChanged);
        }

        private void ObiForm_commandDone(object sender, urakawa.events.undo.DoneEventArgs e) { ProjectHasChanged(1); }
        private void ObiForm_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e) { ProjectHasChanged(-1); }
        private void ObiForm_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e) { ProjectHasChanged(1); }

        // Show welcome dialog first, unless the user has chosen
        private void ObiForm_Load(object sender, EventArgs e)
        {
            if (!ShouldOpenLastProject && mShowWelcomWindow) ShowWelcomeDialog();
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
                    OpenProject_Safe(mSettings.LastOpenProject);
                    break;
                case WelcomeDialog.Option.ViewHelp:
                    ShowHelpFile();
                    break;
            }
        }

        // Remember the form size in the settings.
        private void ObiForm_ResizeEnd(object sender, EventArgs e) { mSettings.ObiFormSize = Size; }

        private void ProjectView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateMenus();
            ShowSelectionInStatusBar();
        }

        private void ProjectView_BlocksVisibilityChanged ( object sender, EventArgs e )
            {
            ShowSelectionInStatusBar ();
            }

        private void Session_ProjectCreated(object sender, EventArgs e)
        {
            GotNewPresentation();
            Status(String.Format(Localizer.Message("created_new_project"), mSession.Presentation.Title));
        }

        private void Session_ProjectClosed(object sender, ProjectClosedEventArgs e)
        {
            if (e.ClosedPresentation != null && e.ClosedPresentation.Initialized)
            {
            foreach (string customClass in mProjectView.Presentation.CustomClasses)RemoveCustomClassFromMenu( customClass );
                mProjectView.Presentation.changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
                Status(String.Format(Localizer.Message("closed_project"), e.ClosedPresentation.Title));
            }
        mAutoSaveTimer.Stop ();
            mProjectView.Selection = null;
            mProjectView.Presentation = null;
            UpdateObi();
            if (mSourceView != null) mSourceView.Close();
        }

        private void Session_ProjectOpened(object sender, EventArgs e)
        {
            GotNewPresentation();
            Status(String.Format(Localizer.Message("opened_project"), mSession.Presentation.Title));
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
            item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { CloseAndOpenProject(path); });
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
            mUndoToolStripMenuItem.Enabled = mSession.CanUndo;
            mUndoToolStripMenuItem.Text = mSession.CanUndo ?
                String.Format(Localizer.Message("undo_label"), Localizer.Message("undo"), mSession.UndoLabel) :
                Localizer.Message("cannot_undo");
            mRedoToolStripMenuItem.Enabled = mSession.CanRedo;
            mRedoToolStripMenuItem.Text = mSession.CanRedo ?
                String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"), mSession.RedoLabel) :
                Localizer.Message("cannot_redo");
            mCutToolStripMenuItem.Enabled = mProjectView.CanCut;
            mCopyToolStripMenuItem.Enabled = mProjectView.CanCopy;
            mPasteToolStripMenuItem.Enabled = mProjectView.CanPaste;
            mPasteBeforeToolStripMenuItem.Enabled = mProjectView.CanPasteBefore;
            mPasteInsideToolStripMenuItem.Enabled = mProjectView.CanPasteInside;
            mDeleteToolStripMenuItem.Enabled = mProjectView.CanDelete;
            mSelectNothingToolStripMenuItem.Enabled = mProjectView.CanDeselect;
            mEdit_DeleteUnusedDataMenuItem.Enabled = mSession.HasProject;
            mFindInTextToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.CanFindFirstTime;
            mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
            mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
        }

        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e) { Undo(); }
        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e) { Redo(); }
        private void mCutToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Cut(); }
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Copy(); }
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Paste(); }
        private void mPasteBeforeToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteBefore(); }
        private void mPasteInsideToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteInside(); }
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Delete(); }
        private void mSelectNothingToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SelectNothing(); }

        private void mEdit_DeleteUnusedDataMenuItem_Click(object sender, EventArgs e) 
            {
            if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop ();

            mProjectView.DeleteUnused(); 
            }

        private void mFindInTextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.FindInText(); }

        #endregion

        #region View menu

        private void UpdateViewMenu()
        {
            mShowTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowMetadataViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowMetadataViewToolStripMenuItem.CheckedChanged -= new System.EventHandler ( mShowMetadataViewToolStripMenuItem_CheckedChanged );
            mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible;
            mShowMetadataViewToolStripMenuItem.CheckedChanged += new System.EventHandler ( mShowMetadataViewToolStripMenuItem_CheckedChanged );
            mShowTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mShowStatusBarToolStripMenuItem.Enabled = true;
            mFocusOnTOCViewToolStripMenuItem.Enabled =  mProjectView.CanFocusOnTOCView || mProjectView.CanToggleFocusToContentsView;
            mFocusOnStripsViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnContentView && mProjectView.CanToggleFocusToContentsView;
            mFocusOnTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mSynchronizeViewsToolStripMenuItem.Enabled = mSession.HasProject;
            mWrappingInContentViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowSectionContentsToolStripMenuItem.Enabled = mProjectView.CanShowSectionContents;
            mShowSingleSectionToolStripItem.Enabled = mSession.HasProject&& mProjectView.Selection != null ;
            mShowPeakMeterMenuItem.Enabled = mSession.HasProject;
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
        private void mWrappingInContentViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            WrapStripContents = mWrappingInContentViewToolStripMenuItem.Checked;
        }

        private void mShowSourceToolStripMenuItem_Click(object sender, EventArgs e) { ShowSource(); }

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
            mSectionIsUsedToolStripMenuItem.CheckedChanged -= new System.EventHandler(mSectionIsUsedToolStripMenuItem_CheckedChanged);
            mSectionIsUsedToolStripMenuItem.Checked = mProjectView.CanMarkSectionUnused;
            mSectionIsUsedToolStripMenuItem.CheckedChanged += new System.EventHandler(mSectionIsUsedToolStripMenuItem_CheckedChanged);
        }

        private void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddSection(); }
        private void mAddSubsectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddSubSection(); }
        private void mInsertSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.InsertSection(); }
        private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.StartRenamingSelectedSection(); }
        private void mDecreaseSectionLevelToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.DecreaseSelectedSectionLevel(); }
        private void mIncreaseSectionLevelToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.IncreaseSelectedSectionLevel(); }
        private void mSplitSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitStrip(); }
        private void mMergeSectionWithNextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }
        private void mSectionIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.SetSelectedNodeUsedStatus(mSectionIsUsedToolStripMenuItem.Checked);
        }

        #endregion

        #region Phrases menu

        // Update the status of the blocks menu item with the current selection and tree.
        private void UpdatePhrasesMenu()
        {
            mAddBlankPhraseToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            mAddEmptyPagesToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            mImportAudioFileToolStripMenuItem.Enabled = mProjectView.CanImportPhrases;
            mSplitPhraseToolStripMenuItem.Enabled = mProjectView.CanSplitPhrase;
            mMergePhraseWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeBlockWithNext;
            mPhrases_AssignRole_PageMenuItem.Enabled = mProjectView.CanSetPageNumber;
            mPhrases_EditRolesMenuItem.Enabled = mSession.HasProject;
            mPhrases_ClearRoleMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            mPhrases_ApplyPhraseDetectionMenuItem.Enabled = mProjectView.CanApplyPhraseDetection;
            mCropAudiotoolStripMenuItem.Enabled = mProjectView.CanCropPhrase;
            mGoToToolStripMenuItem.Enabled = mSession.Presentation != null;
            mPhraseIsUsedToolStripMenuItem.Enabled = mProjectView.CanSetBlockUsedStatus;
            mPhraseIsUsedToolStripMenuItem.CheckedChanged -= new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
            mPhraseIsUsedToolStripMenuItem.Checked = mProjectView.IsBlockUsed;
            mPhraseIsUsedToolStripMenuItem.CheckedChanged += new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
            mPhrases_PhraseIsTODOMenuItem.Enabled = mProjectView.CanSetTODOStatus;
            mPhrases_PhraseIsTODOMenuItem.CheckedChanged -= new System.EventHandler(mPhrases_PhraseIsTODOMenuItem_CheckedChanged);
            mPhrases_PhraseIsTODOMenuItem.Checked = mProjectView.IsCurrentBlockTODO;
            mPhrases_PhraseIsTODOMenuItem.CheckedChanged += new System.EventHandler(mPhrases_PhraseIsTODOMenuItem_CheckedChanged);
            mPhrases_AssignRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            mPhrases_AssignRole_PlainMenuItem.Enabled = mProjectView.CanAssignPlainRole;
            mPhrases_AssignRole_HeadingMenuItem.Enabled = mProjectView.CanAssignHeadingRole;
            mPhrases_AssignRole_PageMenuItem.Enabled = mProjectView.CanAssignARole;
            mPhrases_AssignRole_SilenceMenuItem.Enabled = mProjectView.CanAssignSilenceRole;
            mPhrases_AssignRole_NewCustomRoleMenuItem.Enabled = mProjectView.CanAssignARole;
            m_GoToPageToolStrip.Enabled = mSession.Presentation != null;
            UpdateAudioSelectionBlockMenuItems();
        }

        private void UpdateAudioSelectionBlockMenuItems()
        {
                    mPhrases_AudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
        mPhrases_AudioSelection_BeginAudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionBegin;
        mPhrases_AudioSelection_EndAudioSelectionMenuItem.Enabled = mProjectView.CanMarkSelectionEnd;
        }

        private void mAddBlankPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyBlock(); }
        private void mAddEmptyPagesToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyPages(); }
        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ImportPhrases(); }
        private void mSplitPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitPhrase(); }
        private void mMergePhraseWithNextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeBlockWithNext(); }
        private void mPhrases_PhraseIsTODOMenuItem_CheckedChanged(object sender, EventArgs e) { mProjectView.ToggleTODOForPhrase(); }


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
               { mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Custom, name); });
            items.Insert(index, item);
        }

        // Update the custom class menu to remove this class
        void Presentation_CustomClassRemoved(object sender, CustomClassEventArgs e)
        {
        RemoveCustomClassFromMenu ( e.CustomClass );
            }


         private void RemoveCustomClassFromMenu ( string customClassName )   
        {
            ToolStripItemCollection items = mPhrases_AssignRoleMenuItem.DropDownItems;
            int index;
            for (index = items.IndexOf(mCustomRoleToolStripSeparator);
                index < items.IndexOf(mPhrases_AssignRole_NewCustomRoleMenuItem) &&
                items[index].Text != customClassName; ++index) ;
            if (index < items.IndexOf(mPhrases_AssignRole_NewCustomRoleMenuItem)) 
                {
                mProjectView.RemoveCustomRoleFromContextMenu ( items[index].Text, this );
                items.RemoveAt(index);
                                                }
        }


        private void mPhrases_AssignRole_PlainMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignPlainRole) mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Plain, null);
        }

        private void mPhrases_AssignRole_HeadingMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanAssignHeadingRole) mProjectView.SetRoleForSelectedBlock(EmptyNode.Role.Heading, null);
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
            EditRoles dialog = new EditRoles(mSession.Presentation, mProjectView);
            dialog.ShowDialog();
        }

        #endregion

        #region Transport menu

        // Update the transport manu
        private void UpdateTransportMenu()
        {
            mPlayToolStripMenuItem.Enabled = mProjectView.CanPlay || mProjectView.CanPlaySelection || mProjectView.CanResume;
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
            mPreviousSectionToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevSection;
            mPreviousPageToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevPage;
            mPreviousPhraseToolStripMenuItem.Enabled = mProjectView.CanNavigatePrevPhrase;
            mNextPhraseToolStripMenuItem.Enabled = mProjectView.CanNavigateNextPhrase;
            mNextPageToolStripMenuItem.Enabled = mProjectView.CanNavigateNextPage;
            mNextSectionToolStripMenuItem.Enabled = mProjectView.CanNavigateNextSection;
            mFastForwardToolStripMenuItem.Enabled = mProjectView.CanFastForward;
            mRewindToolStripMenuItem.Enabled = mSession.HasProject &&  mProjectView.CanRewind;
            navigationToolStripMenuItem.Enabled = mSession.HasProject;


            mFastPlaytoolStripMenuItem.Enabled = mSession.HasProject && !mProjectView.TransportBar.IsRecorderActive;
            mRecordToolStripMenuItem.Enabled = mSession.HasProject && mProjectView.TransportBar.CanRecord; 
            mStartRecordingDirectlyToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive;
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

        private void mPlayAllToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.PlayAll(); }
        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanPlaySelection) mProjectView.TransportBar.PlayOrResume(mProjectView.Selection.Node);
        }
        private void mPauseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Pause(); }
        private void mResumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanResume) mProjectView.TransportBar.PlayOrResume();
        }
        private void mStopToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Stop(); }

        private void mStartRecordingToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Record(); }
        private void mStartMonitoringToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Record(); }
        private void mStartRecordingDirectlyToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.StartRecordingDirectly(); }


        private void PreviewFromtoolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Preview(ProjectView.TransportBar.From, ProjectView.TransportBar.UseAudioCursor); }
        private void PreviewUptotoolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Preview(ProjectView.TransportBar.Upto, ProjectView.TransportBar.UseSelection); }
        private void PreviewSelectedAudiotoolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.PreviewAudioSelection(); }
        private void previousSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.PrevSection(); }
        private void previousPageToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.PrevPage(); }
        private void previousPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.PrevPhrase(); }
        private void nextPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.NextPhrase(); }
        private void nextPageToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.NextPage(); }
        private void nextSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.NextSection(); }

        private void rewindToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.Rewind(); }
        private void fastForwardToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TransportBar.FastForward(); }


















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
            mTools_ExportAsDAISYMenuItem.Enabled = mSession.HasProject;
            mTools_CleanUnreferencedAudioMenuItem.Enabled = mSession.HasProject;
            PipelineMenuItemsEnabled = mSession.HasProject && mSession.PrimaryExportPath != "";
        }

        // Open the preferences dialog
        private void mTools_PreferencesMenuItem_Click(object sender, EventArgs e)
        {
        if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Pause ();

            Dialogs.Preferences prefs = new Dialogs.Preferences(this, mSettings, mSession.Presentation, mProjectView.TransportBar);
            prefs.ShowDialog();
            Ready();
        }

        private void mTools_ExportAsDAISYMenuItem_Click(object sender, EventArgs e) { ExportProject(); }

        // Export the project as DAISY 3.
        private void ExportProject()
        {
            mProjectView.TransportBar.Enabled = false ;

            // returns if project is empty
            if (mProjectView.Presentation.RootNode.SectionCount == 0)
                {
                MessageBox.Show ( Localizer.Message ( "ExportError_EmptyProject" ), Localizer.Message ( "Caption_Error" ),MessageBoxButtons.OK, MessageBoxIcon.Error);
                mProjectView.Selection = null ; // done for precaution 
                return ;
                }

            if (CheckedPageNumbers() && CheckedForEmptySections())
            {
                Dialogs.ExportDirectory dialog =
                    new ExportDirectory(Path.Combine(Directory.GetParent(mSession.Path).FullName,
                        Program.SafeName(string.Format(Localizer.Message("default_export_dirname"),
                            "" ) ) ), mSession.Path ); // null string temprorarily used instead of -mProjectView.Presentation.Title- to avoid unicode character problem in path for pipeline
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Need the trailing slash, otherwise exported data ends up in a folder one level
                        // higher than our selection.
                        string exportPath = dialog.DirectoryPath;
                        int audioFileSectionLevel = dialog.LevelSelection ;
                        
                        if (!exportPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                        {
                            exportPath += Path.DirectorySeparatorChar;
                        }
                        ProgressDialog progress = new ProgressDialog(Localizer.Message("export_progress_dialog_title"),
                            delegate() { mSession.Presentation.ExportToZ(
                                exportPath, mSession.Path ,dialog.ExportFormatSelected,audioFileSectionLevel ); });
                        progress.ShowDialog();
                        if (progress.Exception != null) throw progress.Exception;
                        mSession.PrimaryExportPath = exportPath;
                        mSession.ForceSave();
                        MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), exportPath),
                            Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("didnt_save_as_daisy_text"), dialog.DirectoryPath, e.Message),
                            Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        mProjectView.TransportBar.Enabled = true;
            Ready();
        }

        private void mTools_CleanUnreferencedAudioMenuItem_Click(object sender, EventArgs e) { CleanProject(); }



        // Check that page numbers are valid before exporting and return true if they are.
        // If they're not, the user is presented with the possibility to cancel export (return false)
        // or automatically renumber, in which case we also return true.
        // Only normal and front pages are considered, and we skip empty blocks since they're not exported.
        private bool CheckedPageNumbers()
        {
        bool retVal = false;
        try
            {
            retVal = CheckPageNumbers ( PageKind.Front ) && CheckPageNumbers ( PageKind.Normal );
            }
        catch (System.Exception ex)
            {
            retVal = false;
            MessageBox.Show ( ex.ToString () );
            }

            return retVal;
        }

        /// <summary>
        /// Check page numbers of a given kind and give the option to renumber from the first duplicate value if
        /// a duplicate is found.
        /// </summary>
        private bool CheckPageNumbers(PageKind kind)
        {
            Dictionary<int, PhraseNode> pages = new Dictionary<int, PhraseNode>();
            PhraseNode renumberFrom = null;
            PageNumber renumberNumber = null;
            mSession.Presentation.RootNode.acceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    PhraseNode phrase = n as PhraseNode;
                    if (phrase != null && phrase.Role_ == EmptyNode.Role.Page && phrase.PageNumber.Kind == kind)
                    {
                        if (pages.ContainsKey(phrase.PageNumber.Number))
                        {
                            if (renumberFrom == null)
                            {
                                renumberFrom = phrase;
                                renumberNumber = phrase.PageNumber.NextPageNumber();
                                while (pages.ContainsKey(renumberNumber.Number)) renumberNumber = renumberNumber.NextPageNumber();
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
                if (MessageBox.Show(string.Format(Localizer.Message("renumber_before_export"), renumberFrom.PageNumber.ToString()),
                    Localizer.Message("renumber_before_export_caption"),
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.Cancel) return false;
                //mSession.Presentation.getUndoRedoManager().execute(renumberFrom.RenumberCommand(mProjectView, renumberNumber));
                mSession.Presentation.getUndoRedoManager ().execute ( mProjectView.GetRenumberPageKindCommand ( renumberFrom, renumberNumber ) );
            }
            return true;
        }

        // Look for sections which will not be exported and warn the user.
        // If there are empty sections, issue a warning and ask whether to continue.
        // Return true if there are no empty sections, or the user chose to continue.
        private bool CheckedForEmptySections()
        {
            bool cont = true;
            bool keepWarning = true;
            try
                {
                mSession.Presentation.RootNode.acceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {
                        SectionNode s = n as SectionNode;
                        if (s != null && s.Used && s.FirstUsedPhrase == null && keepWarning)
                            {
                            Dialogs.EmptySection dialog = new Dialogs.EmptySection ( s.Label );
                            cont = cont && dialog.ShowDialog () == DialogResult.OK;
                            keepWarning = dialog.KeepWarning;
                            return false;
                            }
                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );
                return cont;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () ) ;
                return false ;
                }
        }

        #endregion





        /// <summary>
        /// Display a message in the status bar.
        /// </summary>
        public void Status(string message) { mStatusLabel.Text = message; }

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
        private void CloseAndOpenProject(string path) { if (DidCloseProject()) OpenProject_Safe(path); }

        // Try to create a new project with the given title at the given path.
        private void CreateNewProject(string path, string title, bool createTitleSection, string id)
        {
            try
            {
                // let's see if we can actually write the file that the user chose (bug #1679175)
                FileStream file = File.Create(path);
                file.Close();
                mSession.NewPresentation(path, title, createTitleSection, id, mSettings);
                UpdateMenus();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    String.Format(Localizer.Message("cannot_create_file_text"), path, e.Message),
                    Localizer.Message("cannot_create_file_caption"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // A new presentation was loaded or created.
        private void GotNewPresentation()
        {
            mProjectView.Presentation = mSession.Presentation;
            UpdateObi();
            mSession.Presentation.getUndoRedoManager().commandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(ObiForm_commandDone);
            mSession.Presentation.getUndoRedoManager().commandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ObiForm_commandReDone);
            mSession.Presentation.getUndoRedoManager().commandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ObiForm_commandUnDone);
            UpdateCustomClassMenu();
             mProjectView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs> ( Presentation_Changed );
             if (mSettings.AutoSaveTimeIntervalEnabled )  mAutoSaveTimer.Start ();
        }


        // Catch problems with initialization and report them.
        private void InitializeObi()
        {
            try
            {
                InitializeComponent();
                mProjectView.ObiForm = this;
                mProjectView.SelectionChanged += new EventHandler(ProjectView_SelectionChanged);
                mProjectView.BlocksVisibilityChanged += new EventHandler( ProjectView_BlocksVisibilityChanged ) ;
                mSession = new Session();
                mSession.ProjectOpened += new EventHandler(Session_ProjectOpened);
                mSession.ProjectCreated += new EventHandler(Session_ProjectCreated);
                mSession.ProjectClosed += new ProjectClosedEventHandler(Session_ProjectClosed);
                mSession.ProjectSaved += new EventHandler(Session_ProjectSaved);
                mSourceView = null;
                InitializeSettings();
                InitializeEventHandlers();
                UpdateMenus();
                // these should be stored in settings
                mShowTOCViewToolStripMenuItem.Checked = mProjectView.TOCViewVisible = true;
                mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible = false;
                mShowTransportBarToolStripMenuItem.Checked = mProjectView.TransportBarVisible = true;
                mShowStatusBarToolStripMenuItem.Checked = mStatusStrip.Visible = true;
                mBaseFontSize = mStatusLabel.Font.SizeInPoints;
                InitializeColorSettings();
                InitializeAutoSaveTimer ();

                if (Directory.Exists(mSettings.PipelineScriptsPath))
                {
                    mPipelineInfo = new Obi.PipelineInterface.PipelineInfo(mSettings.PipelineScriptsPath);
                    PopulatePipelineScriptsInToolsMenu();
                }
                else
                    MessageBox.Show(string.Format(Localizer.Message("ObiForm_PipelineNotFound"), mSettings.PipelineScriptsPath));
                Ready();
            }
            catch (Exception e)
            {
                string path = Path.Combine(Application.StartupPath, "obi_startup_error.txt");
                System.IO.StreamWriter tmpErrorLogStream = System.IO.File.CreateText(path);
                tmpErrorLogStream.WriteLine(e.ToString());
                tmpErrorLogStream.Close();
                MessageBox.Show(String.Format(Localizer.Message("init_error_text"), path, e.ToString()),
                    Localizer.Message("init_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeAutoSaveTimer ()
            {
            mAutoSaveTimer = new Timer ();
            mAutoSaveTimer.Enabled = false;
            mAutoSaveTimer.Interval = mSettings.AutoSaveTimeInterval;
                        mAutoSaveTimer.Tick += new EventHandler(mAutoSaveTimer_Tick);
            }

        private void mAutoSaveTimer_Tick ( object sender, EventArgs e )
            {
            if (mSession != null && mSession.Presentation != null && mSettings.AutoSaveTimeIntervalEnabled  && mSession.CanSave)
                {
                if (mProjectView.TransportBar.CurrentState != Obi.ProjectView.TransportBar.State.Recording)
                    Save ();
                else
                    mProjectView.TransportBar.AutoSaveOnNextRecordingEnd = true;
                                }
                                        }

        public int SetAutoSaverInterval { set { if (mAutoSaveTimer != null) mAutoSaveTimer.Interval = value; } }
        public void StartAutoSaveTimeInterval ()
            {
            if (mSettings.AutoSaveTimeIntervalEnabled)
                {
                if ( mAutoSaveTimer.Enabled ) mAutoSaveTimer.Stop ();
                mAutoSaveTimer.Start ();
                                }
            }

        private void PopulatePipelineScriptsInToolsMenu()
        {
            ToolStripMenuItem PipelineMenuItem = null;
            foreach (KeyValuePair<string, FileInfo> k in mPipelineInfo.ScriptsInfo)
            {
                PipelineMenuItem = new ToolStripMenuItem();
                PipelineMenuItem.Text = k.Key;
                PipelineMenuItem.AccessibleName = k.Key;
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
                foreach (ToolStripMenuItem m in ItemList) m.Enabled = value;
            }
        }


        // Open the project at the given path; warn the user on error.
        private void OpenProject_Safe(string path)
        {
            try
            {
                OpenProject(path);
            }
            catch (Exception e)
            {
                // if opening failed, no project is open and we don't try to open it again next time.
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                RemoveRecentProject(path);
                mSettings.LastOpenProject = "";
                mSession.Close();
            }
        }

        // Unsafe version of open project
        private void OpenProject(string path)
        {
        this.Cursor = Cursors.WaitCursor;
            mSession.Open(path);
            AddRecentProject(path);
            this.Cursor = Cursors.Default;
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
                if (mSession.CanRedo) mSession.Presentation.getUndoRedoManager().redo();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
                if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop ();

                    mSourceView = new Dialogs.ShowSource(mProjectView);
                    mSourceView.FormClosed +=
                        new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e) { mSourceView = null; });
                    mSourceView.Show();
                }
                else
                {
                    mSourceView.Focus();
                }
                Ready();
            }
        }

        // Show a new peak meter form or give focus back to the previously opned one
        private void ShowPeakMeter()
        {
            if (mPeakMeter == null)
            {
                mPeakMeter = new Obi.Audio.PeakMeterForm();
                mPeakMeter.SourceVuMeter = mProjectView.TransportBar.VuMeter;
                mPeakMeter.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e) 
                    { 
                    mPeakMeter = null;
                    mShowPeakMeterMenuItem.Checked = false;
                    });
                if (this.WindowState != FormWindowState.Minimized)
                {
                    //Make sure the Peak meter is displayed on the right of the Obi form.
                    if (this.WindowState == FormWindowState.Maximized)
                    {
                        System.Drawing.Point newLoc = this.Location;
                        newLoc.X += SystemInformation.HorizontalResizeBorderThickness;
                        newLoc.Y += SystemInformation.VerticalResizeBorderThickness;
                        System.Drawing.Size newSize = this.Size;
                        newSize.Width -= 2 * SystemInformation.HorizontalResizeBorderThickness;
                        newSize.Height -= 2 * SystemInformation.VerticalResizeBorderThickness;
                        this.WindowState = FormWindowState.Normal;
                        this.Location = newLoc;
                        this.Size = newSize;
                    }
                    this.Width -= mPeakMeter.Width;
                    mPeakMeter.Top = this.Top;
                    mPeakMeter.Left = this.Right;
                    mPeakMeter.Height = this.Height;
                    mPeakMeter.StartPosition = FormStartPosition.Manual;
                }
                mPeakMeter.Show();
                mShowPeakMeterMenuItem.Checked = true;
            }
            else
            {
            this.Width = this.Width + mPeakMeter.Width;
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
                {
                    mSession.Presentation.getUndoRedoManager().undo(); 
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            mProjectView.ResumeLayout_All();
            mProjectView.TransportBar.SelectionChangedPlaybackEnabled = PlayOnSelectionStatus;
        }

        /// <summary>
        /// Show the current selection in the status bar.
        /// </summary>
        private void ShowSelectionInStatusBar()
        {
        if (mProjectView.Selection != null) Status ( mProjectView.Selection.ToString () + mProjectView.TransportBar.RecordingPhraseToString + mProjectView.InvisibleSelectedStripString);
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

        // Update the title and status bars to show the name of the project, and if it has unsaved changes
        private void UpdateTitleAndStatusBar()
        {
            Text = mSession.HasProject ?
                String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                    (mSession.CanSave ? "*" : ""), mSession.Path, Localizer.Message("obi")) :
                Localizer.Message("obi");
        }



        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, EventArgs e)
        {
            Status(Localizer.Message(mProjectView.TransportBar.CurrentState.ToString()));
            UpdatePhrasesMenu();
            UpdateTransportMenu();
            UpdateEditMenu ();
            mProjectView.UpdateContextMenus ();
                    }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectView.TransportBar.CurrentPlaylist.PlaybackRate));
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

            if (mProjectView != null) mProjectView.TransportBar.Stop();

            if (DidCloseProject())
            {
                try
                {
                    mSettings.SaveSettings();
                }
                catch (Exception x)
                {
                    MessageBox.Show(String.Format(Localizer.Message("save_settings_error_text"), x.Message),
                        Localizer.Message("save_settings_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                mProjectView.SelectionChanged -= new EventHandler(ProjectView_SelectionChanged);
                mProjectView.BlocksVisibilityChanged -= new EventHandler ( ProjectView_BlocksVisibilityChanged );

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
            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                if (!AddRecentProjectsItem((string)mSettings.RecentProjects[i])) mSettings.RecentProjects.RemoveAt(i);
            }
            try
            {
                mProjectView.TransportBar.AudioPlayer.SetDevice(this, mSettings.LastOutputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_output_device_text"), Localizer.Message("no_output_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                mProjectView.TransportBar.Recorder.SetDevice(this, mSettings.LastInputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_input_device_text"), Localizer.Message("no_input_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
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
            // Colors
            mSettings.ColorSettings.CreateBrushesAndPens();
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
            mProjectView.ColorSettings = SystemInformation.HighContrast ?
                mSettings.ColorSettingsHC : mSettings.ColorSettings;
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
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                    if (Path.GetFullPath ( path ) != Path.GetPathRoot ( path ))
                        {
                        foreach (string f in Directory.GetFiles ( path )) File.Delete ( f );
                        foreach (string d in Directory.GetDirectories ( path )) Directory.Delete ( d, true );
                        }
                    else MessageBox.Show(Localizer.Message ("CannotDeleteAtRoot"), Localizer.Message("Caption_Error"));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format(Localizer.Message("cannot_empty_directory"), path, e.Message),
                            Localizer.Message("cannot_empty_directory_caption"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return result != DialogResult.Cancel;
            }
            else
            {
                return true;  // the directory was empty or we didn't need to check
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
                    return true;  // did create the directory
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        String.Format(Localizer.Message("cannot_create_directory_text"), path, e.Message),
                        Localizer.Message("cannot_create_directory_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;  // couldn't create the directory
                }
            }
            else
            {
                return false;  // didn't want to create the directory
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
            ZoomFactor = ZoomFactor * ZOOM_FACTOR_INCREMENT;
        }

        // View > Zoom out (Ctrl+Alt+-)
        private void mView_ZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            ZoomFactor = ZoomFactor / ZOOM_FACTOR_INCREMENT;
        }

        // View > Normal size (Ctrl+Alt+0)
        private void mView_NormalSizeMenuItem_Click(object sender, EventArgs e)
        {
            ZoomFactor = 1.0f;
        }

        // View > Project properties (Alt+Enter)
        private void mView_ProjectPropertiesMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowProjectPropertiesDialog();
        }

        // View > Phrase properties (Alt+Enter)
        private void mView_PhrasePropertiesMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowPhrasePropertiesDialog( false);
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
            ProjectView.Strip strip = mProjectView.StripForSelection;
            if (strip == null)
            {
                AudioScale *= AUDIO_SCALE_INCREMENT;
            }
            else
            {
                strip.AudioScale *= AUDIO_SCALE_INCREMENT;
            }
        }

        private void mView_AudioZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            ProjectView.Strip strip = mProjectView.StripForSelection;
            if (strip == null)
            {
                AudioScale /= AUDIO_SCALE_INCREMENT;
            }
            else
            {
                strip.AudioScale /= AUDIO_SCALE_INCREMENT;
            }
        }

        private void mView_ResetAudioSizeMenuItem_Click(object sender, EventArgs e)
        {
            AudioScale = AudioScale;
        }

        private void PipelineToolStripItems_Click(object sender, EventArgs e)
        {
        mProjectView.TransportBar.Enabled = false;
            try
            {
                PipelineInterface.PipelineInterfaceForm pipeline = new PipelineInterface.PipelineInterfaceForm(
                    mPipelineInfo.ScriptsInfo[((ToolStripMenuItem)sender).Text].FullName,
                    Path.Combine(mSession.PrimaryExportPath, "obi_dtb.opf"),
                    Directory.GetParent(mSession.Path).FullName);
                ProgressDialog progress = new ProgressDialog(((ToolStripMenuItem)sender).Text,
                    delegate() { pipeline.RunScript(); });
                if (pipeline.ShowDialog() == DialogResult.OK) progress.Show();
                if (progress.Exception != null) throw progress.Exception;
            }
            catch (Exception x)
            {
                MessageBox.Show(string.Format(Localizer.Message("dtb_encode_error"), x.Message),
                                   Localizer.Message("dtb_encode_error_caption"),
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        mProjectView.TransportBar.Enabled = true;
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

        public void BackgroundOperation_AddItem()
        {
            if (mStatusProgressBar.ProgressBar != null)
            {
                ShowProgressBar();
                ++mStatusProgressBar.Maximum;
            }
        }

        public void BackgroundOperation_Step()
        {
            if (mStatusProgressBar.ProgressBar != null)
            {
                ShowProgressBar();
                if (mStatusProgressBar.Value < mStatusProgressBar.Maximum) ++mStatusProgressBar.Value;
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
        mProjectView.ClearRoleOfSelectedPhrase ();
        }

        private void Presentation_Changed ( object sender, urakawa.events.DataModelChangedEventArgs e )
            {
            if ( mProjectView.Presentation != null && mProjectView.Selection != null  && !mProjectView.TransportBar.IsActive)
                ShowSelectionInStatusBar ();
                        }

        private void mShowSectionContentsToolStripMenuItem_Click ( object sender, EventArgs e )
            {
            if (mProjectView.Selection != null) mProjectView.ShowSelectedSectionContents ();
                        }


        void mShowSingleSectionToolStripItem_Click ( object sender, System.EventArgs e )
            {
            //if (mSession.HasProject) mProjectView.ShowOnlySelectedSection = mShowSingleSectionToolStripItem.Checked; //  //@ShowSingleSection
            }

        private void nextPageToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            mProjectView.GoToPage();            
        }
    }
}
