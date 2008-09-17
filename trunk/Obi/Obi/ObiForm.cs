using Obi.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
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

        private static readonly float ZOOM_FACTOR_INCREMENT = 1.2f;   // zoom factor increment (zoom in/out)
        private static readonly float DEFAULT_ZOOM_FACTOR_HC = 1.2f;  // default zoom factor (high contrast mode)
        private static readonly float AUDIO_SCALE_INCREMENT = 1.2f;   // audio scale increment (audio zoom in/out)

        /// <summary>
        /// Initialize a new form and open the last project if set in the preferences.
        /// </summary>
        public ObiForm()
        {
            InitializeObi();
            if (ShouldOpenLastProject) OpenProject(mSettings.LastOpenProject);
        }

        /// <summary>
        /// Initialize a new form and open a project from the path given as parameter.
        /// </summary>
        public ObiForm(string path)
        {
            InitializeObi();
            OpenProject(path);
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

        // Set wrapping strips
        private bool WrapStrips
        {
            set
            {
                // Temporarily disabled
                mSettings.WrapStrips = false;
                mProjectView.WrapStrips = false;
                // mSettings.WrapStrips = value;
                // mWrappingInContentViewToolStripMenuItem.Checked = value;
                // mProjectView.WrapStrips = value;
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

        // Create a new project by importing an XHTML file.
        // Prompt the user for the location of the file through a dialog.
        private void NewProjectFromImport()
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
                        RemoveRecentProject(mSession.Path);
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = Localizer.Message("obi_filter");
            dialog.InitialDirectory = mSettings.DefaultPath;
            if (dialog.ShowDialog() == DialogResult.OK && DidCloseProject()) OpenProject(dialog.FileName);
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

        // Save the current project
        private void Save() { mSession.Save(); }

        // Save the current project under a different name; ask for a new path first.
        private void SaveAs()
        {
            SaveProjectAsDialog SaveDialog = new SaveProjectAsDialog(Directory.GetParent(mSession.Path).FullName);
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                if (mSession.CanSave &&
                    MessageBox.Show(Localizer.Message("SaveBeforeUsingSaveAs"),
                                    Localizer.Message("Caption_Warning"),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    mSession.ForceSave();
                }

                string OriginalProjectFilePath = mSession.Path;
                DirectoryInfo NewProjectDirectoryInfo = new DirectoryInfo(SaveDialog.NewProjectDirectoryPath);
                DirectoryInfo OriginalDirInfo = new DirectoryInfo(Directory.GetParent(mSession.Path).FullName);
                NewProjectDirectoryInfo.Create();
                ShallowCopyFilesInDirectory(OriginalDirInfo.FullName, NewProjectDirectoryInfo.FullName);

                FileInfo XukFileInfo = new FileInfo(mSession.Path);
                string XukPath = Path.Combine(NewProjectDirectoryInfo.FullName, XukFileInfo.Name);
                if (File.Exists(XukPath)) File.Delete(XukPath);
                mSession.SaveAs(XukPath);

                if (!SaveDialog.SavePrimaryDirectoriesOnly)
                {
                    DirectoryInfo[] DirList = OriginalDirInfo.GetDirectories("*.*", SearchOption.AllDirectories);

                    foreach (DirectoryInfo d in DirList)
                    {
                        string DestPath = NewProjectDirectoryInfo.FullName + d.FullName.Replace(OriginalDirInfo.FullName, "");
                        if (!Directory.Exists(DestPath))
                        {
                            Directory.CreateDirectory(DestPath);
                            // copy files in each directory
                            ShallowCopyFilesInDirectory(d.FullName, DestPath);
                        }
                    }

                }
                if (!SaveDialog.ActivateNewProject)
                {
                    mSession.Close();
                    OpenProject(OriginalProjectFilePath);
                }
            }
            else
            {
                Ready();
            }
        }

        // Copy files from one directory to another
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
            if (!mSession.CanClose)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return false;
                if (result == DialogResult.Yes) mSession.Save();
            }
            if (mProjectView.TransportBar.IsActive) mProjectView.TransportBar.Stop();
            mSession.Close();
            return true;
        }

        // Clean unwanted audio from the project.
        // Before continuing, the user is given the choice to save or cancel.
        private void CleanProject()
        {
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
            Dialogs.Help help = new Dialogs.Help();
            help.WebBrowser.Url = new Uri(Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_file_name")));
            help.Show();
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
            Uri url = new Uri("http://daisy-trac.cvsdude.com/obi/newticket");
            System.Diagnostics.Process.Start(url.ToString());
        }

        // Help > About
        private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e) { (new Dialogs.About()).ShowDialog(); }

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
            if (!ShouldOpenLastProject) ShowWelcomeDialog();
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
                    OpenProject(mSettings.LastOpenProject);
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

        private void Session_ProjectCreated(object sender, EventArgs e)
        {
            GotNewPresentation();
            Status(String.Format(Localizer.Message("created_new_project"), mSession.Presentation.Title));
        }

        private void Session_ProjectClosed(object sender, ProjectClosedEventArgs e)
        {
            if (e.ClosedPresentation != null && e.ClosedPresentation.Initialized)
            {
                Status(String.Format(Localizer.Message("closed_project"), e.ClosedPresentation.Title));
            }
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
        private void mEdit_DeleteUnusedDataMenuItem_Click(object sender, EventArgs e) { mProjectView.DeleteUnused(); }
        private void mFindInTextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.FindInText(); }

        #endregion

        #region View menu

        private void UpdateViewMenu()
        {
            mShowTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowMetadataViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mShowStatusBarToolStripMenuItem.Enabled = true;
            mFocusOnTOCViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnTOCView && !mProjectView.CanToggleFocusToContentsView;
            mFocusOnStripsViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnContentView && mProjectView.CanToggleFocusToContentsView;
            mFocusOnTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mSynchronizeViewsToolStripMenuItem.Enabled = mSession.HasProject;
            mShowOnlySelectedSectionToolStripMenuItem.Enabled = mProjectView.CanShowOnlySelectedSection;
            mWrappingInContentViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowPeakMeterMenuItem.Enabled = mSession.HasProject;
            mShowSourceToolStripMenuItem.Enabled = mSession.HasProject;

            mView_PhrasePropertiesMenuItem.Visible =
                mView_PhrasePropertiesMenuItem.Enabled = mProjectView.CanShowPhrasePropertiesDialog;
            mView_SectionPropertiesMenuItem.Visible =
                mView_SectionPropertiesMenuItem.Enabled = mProjectView.CanShowSectionPropertiesDialog;
            mView_ProjectPropertiesMenuItem.Enabled = mProjectView.CanShowProjectPropertiesDialog;
            mView_ProjectPropertiesMenuItem.Visible =
                mProjectView.CanShowProjectPropertiesDialog || !mSession.HasProject;
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

        // Check/uncheck "Show only selected section"
        private void mShowOnlySelectedSectionToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (mProjectView.CanShowOnlySelectedSection)
            {
                mProjectView.ShowOnlySelectedSection = mShowOnlySelectedSectionToolStripMenuItem.Checked;
            }
        }

        // Check/uncheck "Wrapping in content view"
        private void mWrappingInContentViewToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            WrapStrips = mWrappingInContentViewToolStripMenuItem.Checked;
        }

        private void mShowSourceToolStripMenuItem_Click(object sender, EventArgs e) { ShowSource(); }

        #endregion

        #region Sections menu

        private void UpdateSectionsMenu()
        {
            mAddSectionToolStripMenuItem.Enabled = mProjectView.CanAddSection;
            mAddSubsectionToolStripMenuItem.Enabled = mProjectView.CanAddSubSection;
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
        private void mIncreaseSectionLevelToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.IncreaseSelectedSectionNodeLevel(); }
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
            mAssignRoleToolStripMenuItem.Enabled = mProjectView.CanAssignRole;
            mPageToolStripMenuItem.Enabled = mProjectView.CanSetPageNumber;
            mPhrases_EditRolesMenuItem.Enabled = mSession.HasProject;
            mPhrases_AssignRole_PlainMenuItem.Enabled = mProjectView.CanClearRole;
            mPhraseDetectionToolStripMenuItem.Enabled = mProjectView.CanApplyPhraseDetection;
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
            UpdateAudioSelectionBlockMenuItems();
        }

        private void UpdateAudioSelectionBlockMenuItems()
        {
            //string AudioSelectionStatusMessage = "";
            if (mProjectView.Selection is AudioSelection)
            {
                mBeginInPhraseSelectionToolStripMenuItem.Enabled = true;

                if (((AudioSelection)mProjectView.Selection).AudioRange.HasCursor)
                {
                    mEndInPhraseSelectionToolStripMenuItem.Enabled = true;
                }

                if (((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime > 0)
                {
                    mDeselectInPhraseSelectionToolStripMenuItem.Enabled = true;
                    string BeginTime = Math.Round(((AudioSelection)mProjectView.Selection).AudioRange.SelectionBeginTime / 1000, 1).ToString();
                    string EndTime = Math.Round(((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime / 1000, 1).ToString();

                }
                else
                {
                    mDeselectInPhraseSelectionToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                mBeginInPhraseSelectionToolStripMenuItem.Enabled = false;
                mEndInPhraseSelectionToolStripMenuItem.Enabled = false;
                mDeselectInPhraseSelectionToolStripMenuItem.Enabled = false;
            }
        }

        private void mAddBlankPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyBlock(); }
        private void mAddEmptyPagesToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyPages(); }
        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ImportPhrases(); }
        private void mSplitPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitPhrase(); }
        private void mMergePhraseWithNextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeBlockWithNext(); }

        private void mPhrases_PhraseIsTODOMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (mProjectView.TransportBar.IsActive)
            {
                mProjectView.TransportBar.MarkTodoClass();
            }
            else
            {
                mProjectView.ToggleEmptyNodeTo_DoMark();
            }
        }

        private void mPhraseIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.SetSelectedNodeUsedStatus(mPhraseIsUsedToolStripMenuItem.Checked);
        }


        private void mPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanSetPageNumber)
            {
                Dialogs.SetPageNumber dialog = new SetPageNumber(mProjectView.CurrentOrNextPageNumber, false, false);
                if (dialog.ShowDialog() == DialogResult.OK) mProjectView.SetPageNumberOnSelectedBock(dialog.Number, dialog.Renumber);
            }
        }


        // Update the custom class menu with the classes from the new project
        private void UpdateCustomClassMenu()
        {
            foreach (string customClass in mSession.Presentation.CustomClasses) AddCustomClassToMenu(customClass);
            mSession.Presentation.CustomClassAddded += new CustomClassEventHandler(Presentation_CustomClassAddded);
            mSession.Presentation.CustomClassRemoved += new CustomClassEventHandler(Presentation_CustomClassRemoved);
        }

        private void AddCustomClassToMenu(string customClass)
        {
            ToolStripItemCollection items = mAssignRoleToolStripMenuItem.DropDownItems;
            int index = items.IndexOf(mAddRoleToolStripTextBox);
            // TODO find alphabetical spot for the new class
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = customClass;
            item.Click += new EventHandler(delegate(object sender, EventArgs e)
               { mProjectView.SetCustomTypeForSelectedBlock(EmptyNode.Kind.Custom, customClass); });
            items.Insert(index, item);
        }
        // Update the custom class menu
        private void Presentation_CustomClassAddded(object sender, CustomClassEventArgs e)
        {
            AddCustomClassToMenu(e.CustomClass);
        }

        // Update the custom class menu to remove this class
        void Presentation_CustomClassRemoved(object sender, CustomClassEventArgs e)
        {
            ToolStripItemCollection items = mAssignRoleToolStripMenuItem.DropDownItems;
            int index;
            for (index = items.IndexOf(mCustomRoleToolStripSeparator); index < items.IndexOf(mAddRoleToolStripTextBox) &&
                items[index].Text != e.CustomClass; ++index) ;
            if (index < items.IndexOf(mAddRoleToolStripTextBox)) items.RemoveAt(index);
        }

        private void mPhrases_AssignRole_PlainMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SetCustomTypeForSelectedBlock(EmptyNode.Kind.Plain, null);
        }

        private void mSetAsHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MakeSelectedBlockIntoHeadingPhrase();
        }

        private void mSilenceToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MakeSelectedBlockIntoSilencePhrase(); }

        private void mPhrases_EditRolesMenuItem_Click(object sender, EventArgs e)
        {
            EditRoles dialog = new EditRoles(mSession.Presentation, mProjectView);
            dialog.ShowDialog();
        }
        private void mAddRoleToolStripTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mProjectView.AddCustomTypeAndSetOnBlock(EmptyNode.Kind.Custom, mAddRoleToolStripTextBox.Text);
                mPhrasesToolStripMenuItem.DropDown.Close();
                mAddRoleToolStripTextBox.Text = Localizer.Message("add_role");
            }
        }

        private void mAssignNewCustomRoleToolStripMenuItem_click ( object sender, EventArgs e )
            {
            Dialogs.AssignNewCustomRole NewCustomRoleDialog = new AssignNewCustomRole ();

            if (NewCustomRoleDialog.ShowDialog () == DialogResult.OK && NewCustomRoleDialog.CustomClassName != "" )
                {
                mProjectView.AddCustomTypeAndSetOnBlock ( EmptyNode.Kind.Custom, NewCustomRoleDialog.CustomClassName );
                mPhrasesToolStripMenuItem.DropDown.Close ();
                mAddRoleToolStripTextBox.Text = Localizer.Message ( "add_role" );
                }
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
            mRewindToolStripMenuItem.Enabled = mProjectView.CanRewind;


            mFastPlaytoolStripMenuItem.Enabled = mProjectView.CanPlay;

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


        private void PhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ApplyPhraseDetection();
        }

        private void BeginInPhraseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.MarkSelectionBeginTime();
            UpdateAudioSelectionBlockMenuItems();
        }

        private void EndInPhraseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.MarkSelectionEndTime();
            UpdateAudioSelectionBlockMenuItems();
        }

        private void DeselectInPhraseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.Selection is AudioSelection)
            {
                ((AudioSelection)mProjectView.Selection).AudioRange.SelectionBeginTime = 0;
                ((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime = 0;
            }
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
            Dialogs.Preferences prefs = new Dialogs.Preferences(this, mSettings, mSession.Presentation, mProjectView.TransportBar);
            prefs.ShowDialog();
            Ready();
        }

        private void mTools_ExportAsDAISYMenuItem_Click(object sender, EventArgs e) { ExportProject(); }

        // Export the project as DAISY 3.
        private void ExportProject()
        {
            if (CheckedPageNumbers() && CheckedForEmptySections())
            {
                Dialogs.SelectDirectoryPath dialog =
                    new SelectDirectoryPath(Path.Combine(Directory.GetParent(mSession.Path).FullName,
                        Localizer.Message("default_export_dirname")));
                if (dialog.ShowDialog() == DialogResult.OK && IsExportDirectoryReady(dialog.DirectoryPath))
                {
                    try
                    {
                        // Need the trailing slash, otherwise exported data ends up in a folder one level
                        // higher than our selection.
                        string exportPath = dialog.DirectoryPath;
                        if (!exportPath.EndsWith("/")) exportPath += "/";
                        mSession.PrimaryExportPath = exportPath;
                        ProgressDialog progress = new ProgressDialog(Localizer.Message("export_progress_dialog_title"),
                            delegate() { mSession.Presentation.ExportToZ(exportPath, mSession.Path); });
                        progress.ShowDialog();
                        if (progress.Exception != null) throw progress.Exception;
                        mSession.ForceSave();
                        MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), dialog.DirectoryPath),
                            Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("didnt_save_as_daisy_text"), dialog.DirectoryPath, e.Message),
                            Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            Ready();
        }

        private void mTools_CleanUnreferencedAudioMenuItem_Click(object sender, EventArgs e) { CleanProject(); }



        // Check that page numbers are valid before exporting and return true if they are.
        // If they're not, the user is presented with the possibility to cancel export (return false)
        // or automatically renumber, in which case we also return true.
        // Only normal and front pages are considered, and we skip empty blocks since they're not exported.
        private bool CheckedPageNumbers()
        {
            return CheckPageNumbers(PageKind.Front) && CheckPageNumbers(PageKind.Normal);
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
                    if (phrase != null && phrase.NodeKind == EmptyNode.Kind.Page && phrase.PageNumber.Kind == kind)
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
                mSession.Presentation.getUndoRedoManager().execute(renumberFrom.RenumberCommand(mProjectView, renumberNumber));
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
            mSession.Presentation.RootNode.acceptDepthFirst(
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
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { });
            return cont;
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
        private void CloseAndOpenProject(string path) { if (DidCloseProject()) OpenProject(path); }

        // Try to create a new project with the given title at the given path.
        private void CreateNewProject(string path, string title, bool createTitleSection, string id)
        {
            try
            {
                // let's see if we can actually write the file that the user chose (bug #1679175)
                FileStream file = File.Create(path);
                file.Close();
                if (DidCloseProject())
                {
                    mSession.NewPresentation(path, title, createTitleSection, id, mSettings);
                }
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
            mShowOnlySelectedSectionToolStripMenuItem.Checked = false;
            UpdateCustomClassMenu();
        }


        // Catch problems with initialization and report them.
        private void InitializeObi()
        {
            try
            {
                InitializeComponent();
                mProjectView.ObiForm = this;
                mProjectView.SelectionChanged += new EventHandler(ProjectView_SelectionChanged);
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
        private void OpenProject(string path)
        {
            try
            {
                mSession.Open(path);
                AddRecentProject(path);
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

        // The project was modified.
        private void ProjectHasChanged(int change)
        {
            mSession.PresentationHasChanged(change);
            UpdateObi();
        }

        // Redo
        private void Redo()
        {
            if (mProjectView.TransportBar.IsActive)
                mProjectView.TransportBar.Stop();

            if (mSession.CanRedo) mSession.Presentation.getUndoRedoManager().redo();
        }

        // Show a new source view window or give focus back to the previously opened one.
        private void ShowSource()
        {
            if (mSession.HasProject)
            {
                if (mSourceView == null)
                {
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
                mPeakMeter.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e) { mPeakMeter = null; });
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
            if (mSession.CanUndo && !(mProjectView.Selection is TextSelection)) { mSession.Presentation.getUndoRedoManager().undo(); }
        }

        /// <summary>
        /// Show the current selection in the status bar.
        /// </summary>
        private void ShowSelectionInStatusBar()
        {
            if (mProjectView.Selection != null) Status(mProjectView.Selection.ToString());
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
        }

        // Update the title and status bars to show the name of the project, and if it has unsaved changes
        private void UpdateTitleAndStatusBar()
        {
            Text = mSession.HasProject ?
                String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                    (mSession.CanSave ? "*" : ""), Localizer.Message("obi")) :
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
        }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectView.TransportBar.CurrentPlaylist.PlaybackRate));
        }





        // The export directory is ready if it doesn't exist and can be created, or exists
        // and is empty or can be emptied (or the user decided not to empty it.)
        private bool IsExportDirectoryReady(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                if (files.Length > 0)
                {
                    DialogResult result = MessageBox.Show(String.Format(Localizer.Message("empty_directory_text"), path),
                        Localizer.Message("empty_directory_caption"), MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                    else if (result == DialogResult.Yes)
                    {
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (Exception e)
                            {
                                DialogResult dialog = MessageBox.Show(String.Format(Localizer.Message("cannot_delete_text"),
                                    file, e.Message),
                                    Localizer.Message("cannot_delete_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Localizer.Message("cannot_create_directory_text"), path, e.Message),
                        Localizer.Message("cannot_create_directory_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
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
            WrapStrips = mSettings.WrapStrips;
            // Transport bar settings
            mProjectView.TransportBar.PreviewDuration = mSettings.PreviewDuration;
            mProjectView.TransportBar.PlayIfNoSelection = mSettings.PlayIfNoSelection;
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
        /// Check if a string representation of a directory 
        /// exists as a directory on the filesystem,
        /// if not, try to create it, asking the user first.
        /// </summary>
        /// <param name="path">String representation of the directory to be checked/created</param>
        /// <param name="checkEmpty">Check for empty directories.</param>
        /// <returns>True if the is suitable, false otherwise.</returns>        
        public static bool CanUseDirectory(string path, bool checkEmpty)
        {
            return File.Exists(path) ? false :
                Directory.Exists(path) ? CheckEmpty(path, checkEmpty) : DidCreateDirectory(path);
        }

        /// <summary>
        /// Check if a directory is empty or not; ask the user to confirm
        /// that they mean this directory even though it is not empty.
        /// </summary>
        /// <param name="path">The directory to check.</param>
        /// <param name="checkEmpty">Actually check.</param>
        private static bool CheckEmpty(string path, bool checkEmpty)
        {
            if (checkEmpty &&
                (Directory.GetFiles(path).Length > 0 || Directory.GetDirectories(path).Length > 0))
            {
                DialogResult result = MessageBox.Show(
                    String.Format(Localizer.Message("really_use_directory_text"), path),
                    Localizer.Message("really_use_directory_caption"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                return result == DialogResult.Yes;
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
        private static bool DidCreateDirectory(string path)
        {
            if (MessageBox.Show(
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

        private void mAddRoleToolStripTextBox_Click(object sender, EventArgs e)
        {
            if (mAddRoleToolStripTextBox.SelectedText == "")
            {
                // A little bit convoluted but otherwise the selection doesn't work :(
                string text = mAddRoleToolStripTextBox.Text;
                mAddRoleToolStripTextBox.Text = "";
                mAddRoleToolStripTextBox.SelectedText = text;
                mAddRoleToolStripTextBox.SelectAll();
            }
        }


        private void mMarkDefaultCustomClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.TransportBar.Enabled) mProjectView.TransportBar.MarkCustomClass();
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
            mProjectView.ShowPhrasePropertiesDialog();
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
            try
            {
                PipelineInterface.PipelineInterfaceForm PipelineForm = new PipelineInterface.PipelineInterfaceForm(
                    mPipelineInfo.ScriptsInfo[((ToolStripMenuItem)sender).Text].FullName,
                    Path.Combine(mSession.PrimaryExportPath, "obi_dtb.opf"),
                    Directory.GetParent(mSession.Path).FullName);
                if (PipelineForm.ShowDialog() == DialogResult.OK) PipelineForm.RunScript();
            }
            catch (Exception x)
            {
                MessageBox.Show(string.Format(Localizer.Message("dtb_encode_error"), x.Message),
                                   Localizer.Message("dtb_encode_error_caption"),
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    }
}
