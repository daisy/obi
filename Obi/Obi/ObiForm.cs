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
        private Session mSession;                // current work session
        private Settings mSettings;              // application settings
        private Dialogs.ShowSource mSourceView;  // maintain a single "source view" dialog
		private Audio.PeakMeterForm mPeakMeter;  // maintain a single "peak meter" form

        /// <summary>
        /// Initialize a new form and open the last project if set in the preferences.
        /// </summary>
        public ObiForm()
        {
            InitializeObi();
            if (mSettings.OpenLastProject && mSettings.LastOpenProject != "") OpenProject(mSettings.LastOpenProject);
        }

        /// <summary>
        /// Initialize a new form with a XUK path given as parameter.
        /// </summary>
        public ObiForm(string path)
        {
            InitializeObi();
            OpenProject(path);

            // Avn: following line is temprorary till this goes into settings
            mProjectView.TransportBar.AllowOverwrite = mAllowOverwriteToolStripMenuItem.Checked;
        }


        #region File menu

        private void UpdateFileMenu()
        {
            mNewProjectToolStripMenuItem.Enabled = true;
            mNewProjectFromImportToolStripMenuItem.Enabled = true;
            mOpenProjectToolStripMenuItem.Enabled = true;
            mOpenRecentProjectToolStripMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            mClearListToolStripMenuItem.Enabled = true;
            mSaveProjectToolStripMenuItem.Enabled = mSession.CanSave;
            mSaveProjectAsToolStripMenuItem.Enabled = mSession.HasProject;
            mCloseProjectToolStripMenuItem.Enabled = mSession.HasProject;
            mCleanProjectToolStripMenuItem.Enabled = mSession.HasProject;
            mExportAsDAISYToolStripMenuItem.Enabled = mSession.HasProject;
            mExitToolStripMenuItem.Enabled = true;
        }

        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e) { NewProject(); }
        private void mNewProjectFromImportToolStripMenuItem_Click(object sender, EventArgs e) { NewProjectFromImport(); }
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e) { Open(); }
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e) { ClearRecentProjectsList(); }
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e) { Save(); }
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e) { SaveAs(); }
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e) { DidCloseProject(); }
        private void mCleanProjectToolStripMenuItem_Click(object sender, EventArgs e) { CleanProject(); }
        private void mExportAsDAISYToolStripMenuItem_Click(object sender, EventArgs e) { ExportProject(); }
        private void mExitToolStripMenuItem_Click(object sender, EventArgs e) { Close(); }

        // Create a new project if the current one was closed properly, or if none was open.
        private void NewProject()
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_filter"),
                Localizer.Message("default_project_title"));
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection);
            }
        }

        // Create a new project by importing an XHTML file
        private void NewProjectFromImport()
        {
            // Bring up a chooser for the import file
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = Localizer.Message("choose_import_file");
            openFile.Filter = Localizer.Message("xhtml_filter");
            if (openFile.ShowDialog() != DialogResult.OK) return;
            // Then bring up the create project dialog
            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_filter"),
                ImportStructure.GrabTitle(new Uri(openFile.FileName)));
            dialog.DisableAutoTitleCheckbox();
            dialog.Text = Localizer.Message("create_new_project_from_import");
            if (dialog.ShowDialog() != DialogResult.OK) return;
            CreateNewProject(dialog.Path, dialog.Title, false);
            try
            {
                (new ImportStructure()).ImportFromXHTML(openFile.FileName, mSession.Presentation);
            }
            catch (Exception e)
            {
                MessageBox.Show(Localizer.Message("import_failed") + e.Message);
                if (mSession.CanClose) mSession.Close();
                File.Delete(dialog.Path);
                return;
            }
        }

        // Open a new project from a file chosen by the user.
        private void Open()
        {
            if (DidCloseProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("obi_filter");
                dialog.InitialDirectory = mSettings.DefaultPath;
                if (dialog.ShowDialog() == DialogResult.OK) OpenProject(dialog.FileName);
            }
        }

        // Clear the list of recently opened files (prompt the user first.)
        private void ClearRecentProjectsList()
        {
            if (MessageBox.Show(Localizer.Message("clear_recent_text"),
                    Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                while (mOpenRecentProjectToolStripMenuItem.DropDownItems.Count > 2)
                {
                    mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(0);
                }
                mSettings.RecentProjects.Clear();
                mOpenRecentProjectToolStripMenuItem.Enabled = false;
            }
        }

        // Save the current project
        private void Save() { mSession.Save(); }

        // Save the current project under a different name; ask for a new path first.
        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(mSession.Path));
            dialog.FileName = Localizer.Message("default_project_filename");
            dialog.Filter = Localizer.Message("obi_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mSession.SaveAs(dialog.FileName);
            }
            else
            {
                Ready();
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
                    mSession.Presentation.cleanup();
                    DeleteExtraFiles();
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

        // Export the project as DAISY 3.
        private void ExportProject()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = Localizer.Message("export_choose_folder");
            dialog.SelectedPath = mSettings.DefaultExportPath;
            if (dialog.ShowDialog() == DialogResult.OK && IsExportDirectoryReady(dialog.SelectedPath))
            {
                try
                {
                    // Need the trailing slash, otherwise exported data ends up in a folder one level
                    // higher than our selection.
                    string exportPath = dialog.SelectedPath;
                    if (!exportPath.EndsWith("/")) exportPath += "/";
                    mSession.Presentation.ExportToZ(exportPath, mSession.Path);
                    MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), dialog.SelectedPath),
                       Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format(Localizer.Message("didnt_save_as_daisy_text"), dialog.SelectedPath, e.Message),
                        Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            Ready();
        }

        #endregion
        
        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings { get { return mSettings; } }

        /// <summary>
        /// Display a message in the status bar.
        /// </summary>
        public void Status(string message) { mStatusLabel.Text = message; }



        #region Edit menu

        /// <summary>
        /// Explicitly update the find in text menu items
        /// </summary>
        public void UpdateFindInTextMenuItems()
        {
            mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
            mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
        }

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
            mDeleteUnusedDataToolStripMenuItem.Enabled = mSession.HasProject;
            mFindInTextToolStripMenuItem.Enabled = mSession.HasProject;
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
        private void mDeleteUnusedDataToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.DeleteUnused(); }
        private void mFindInTextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.FindInText(); }
        
        #endregion

        #region View menu

        private void UpdateViewMenu()
        {
            mShowTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowMetadataViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mShowStatusBarToolStripMenuItem.Enabled = true;
            mFocusOnTOCViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnTOCView;
            mFocusOnStripsViewToolStripMenuItem.Enabled = mProjectView.CanFocusOnContentView;
            mFocusOnTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mSynchronizeViewsToolStripMenuItem.Enabled = mSession.HasProject;
            mShowOnlySelectedSectionToolStripMenuItem.Enabled = mProjectView.CanShowOnlySelectedSection;
            mWrappingInContentViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowPeakMeterMenuItem.Enabled = mSession.HasProject;
            mShowSourceToolStripMenuItem.Enabled = mSession.HasProject;
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
            mProjectView.FocusOnTOCView();
        }

        private void mFocusOnStripsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSelectedSectionInStripsView();
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


        // Utility functions


        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// </summary>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Contains(path))
            {
                // the item was in the list so bump it up
                int i = mSettings.RecentProjects.IndexOf(path);
                mSettings.RecentProjects.RemoveAt(i);
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            AddRecentProjectsItem(path);
            mSettings.RecentProjects.Insert(0, path);
            mSettings.LastOpenProject = path;
        }

        /// <summary>
        /// Add an item in the recent projects list.
        /// </summary>
        private bool AddRecentProjectsItem(string path)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text = path;
            item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { CloseAndOpenProject(path); });
            mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
            return true;
        }

        /// <summary>
        /// Try to open a project from a XUK file.
        /// Actually open it only if a possible current project could be closed properly.
        /// </summary>
        private void CloseAndOpenProject(string path) { if (DidCloseProject()) OpenProject(path); }

        // Try to create a new project with the given title at the given path.
        private void CreateNewProject(string path, string title, bool createTitleSection)
        {
            try
            {
                // let's see if we can actually write the file that the user chose (bug #1679175)
                FileStream file = File.Create(path);
                file.Close();
                mSettings.CreateTitleSection = createTitleSection;
                if (DidCloseProject())
                {
                    mSession.NewPresentation(path, title, createTitleSection, "(please set id)", mSettings);
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

        private void ObiForm_commandDone(object sender, urakawa.events.undo.DoneEventArgs e) { ProjectHasChanged(1); }
        private void ObiForm_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e) { ProjectHasChanged(-1); }
        private void ObiForm_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e) { ProjectHasChanged(1); }

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
                InitialiseHighContrastSettings();
                InitializeEventHandlers();
                UpdateMenus();
                // these should be stored in settings
                mShowTOCViewToolStripMenuItem.Checked = mProjectView.TOCViewVisible = true;
                mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible = false;
                mShowTransportBarToolStripMenuItem.Checked = mProjectView.TransportBarVisible = true;
                mShowStatusBarToolStripMenuItem.Checked = mStatusStrip.Visible = true;
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

        // Initialize event handlers from the project view
        private void InitializeEventHandlers()
        {
            mProjectView.TransportBar.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
            mProjectView.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
            mProjectView.TransportBar.Recorder.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler(TransportBar_StateChanged);
            mProjectView.ImportingFile += new Obi.ProjectView.ImportingFileEventHandler(mProjectView_ImportingFile);
            mProjectView.FinishedImportingFiles += new EventHandler(mProjectView_FinishedImportingFiles);
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

        // Update the status bar to say "Ready."
        private void Ready()
        {
            Status(Localizer.Message("ready"));
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
						newLoc.Y +=SystemInformation.VerticalResizeBorderThickness;
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
                mPeakMeter.Close();
                mPeakMeter = null;
                mShowPeakMeterMenuItem.Checked = false;
                            }
            this.Ready();
        }

        // Undo
        private void Undo()
        {
            if (mProjectView.TransportBar.IsActive)
                mProjectView.TransportBar.Stop();

            if (mSession.CanUndo) { mSession.Presentation.getUndoRedoManager().undo(); }
        }

        /// <summary>
        /// Set view synchronization and update the menu and settings accordingly.
        /// </summary>
        private bool SynchronizeViews
        {
            set
            {
                mSettings.SynchronizeViews = value;
                mSynchronizeViewsToolStripMenuItem.Checked = value;
                mProjectView.SynchronizeViews = value;
            }
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
        }

        // Update the title and status bars to show the name of the project, and if it has unsaved changes
        private void UpdateTitleAndStatusBar()
        {
            Text = mSession.HasProject ?
                String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                    (mSession.CanSave ? "*" : ""), Localizer.Message("obi")) :
                Localizer.Message("obi");
        }

        // Set wrapping strips
        private bool WrapStrips
        {
            set
            {
                mSettings.WrapStrips = value;
                mWrappingInContentViewToolStripMenuItem.Checked = value;
                mProjectView.WrapStrips = value;
            }
        }

        #region Event handlers

        private void ProjectView_SelectionChanged(object sender, EventArgs e) { UpdateMenus(); }

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

        #endregion



        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, EventArgs e )
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

        // Open the preferences dialog
        private void mPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences prefs = new Dialogs.Preferences(this, mSettings, mSession.Presentation, mProjectView.TransportBar);
            prefs.ShowDialog();
            Ready();
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        /// <remarks>Warn when closing while playing?</remarks>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if ( mProjectView != null )  mProjectView.TransportBar.Stop();

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

                // unhook User preferences system events 
                Microsoft.Win32.SystemEvents.UserPreferenceChanged
                -= new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            }
            else
            {
                e.Cancel = true;
                Ready();
            }
        }

        /// <summary>
        /// Show the HTML help page.
        /// </summary>
        private void mHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dialogs.Help help = new Dialogs.Help();
            help.WebBrowser.Url = new Uri(Path.Combine(
                Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_file_name")));
            help.ShowDialog();
        }

        /// <summary>
        /// Show the help dialog.
        /// </summary>
        private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Dialogs.About()).ShowDialog();
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
            // tooltips
            mProjectView.EnableTooltips = mSettings.EnableTooltips;
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
        }

        // Set the allow overwrite preference and the corresponding property in the transport bar.
        private bool AllowOverwrite
        {
            set
            {
                mProjectView.TransportBar.AllowOverwrite = value;
                mAllowOverwriteToolStripMenuItem.Checked = value;
                mSettings.AllowOverwrite = value;
            }
        }
        

        // Various utility functions

        private void mViewHelpInExternalBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_file_name")))).ToString());
        }

        private void mReportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uri url = new Uri("http://sourceforge.net/tracker/?func=add&group_id=149942&atid=776242");
            System.Diagnostics.Process.Start(url.ToString());
        }


        private void InitialiseHighContrastSettings()
        {
            // Associate  user preference system events
            Microsoft.Win32.SystemEvents.UserPreferenceChanged
                += new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            //UserControls.Colors.SetHighContrastColors(SystemInformation.HighContrast);
            //mProjectView.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            //BackColor = UserControls.Colors.ObiBackGround;

        }

        private void UserPreferenceChanged(object sender, EventArgs e)
        {
            // UserControls.Colors.SetHighContrastColors(SystemInformation.HighContrast);
            // mProjectView.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            // BackColor = UserControls.Colors.ObiBackGround;
            // mProject.Touch();
        }

        /// <summary>
        /// Remove a project from the recent projects list
        /// This is required when import fails halfway through
        /// </summary>
        /// <param name="p"></param>
        //added by med june 4 2007
        private void RemoveRecentProject(String path)
        {
            if (mSettings.RecentProjects.Contains(path))
            {
                int i = mSettings.RecentProjects.IndexOf(path);
                mSettings.RecentProjects.RemoveAt(i);
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
        }

        private void ObiForm_ResizeEnd(object sender, EventArgs e)
        {
            mSettings.ObiFormSize = Size;
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


        /// <summary>
        /// Format a time value. If less than a minute, display seconds and milliseconds.
        /// If less than an hour, display minutes and seconds. Otherwise show hh:mm:ss.
        /// </summary>
        /// <param name="time">The time to display (in milliseconds.)</param>
        /// <returns>The formatted string.</returns>
        public static string FormatTime(double time)
        {
            return time < 60000.0 ? FormatTime_ss_ms(time) :
                // time < 3600000.0 ? FormatTime_mm_ss(time) :
                FormatTime_hh_mm_ss(time);
        }

        /// <summary>
        /// Convenient function to format a milliseconds time into hh:mm:ss format.
        /// </summary>
        /// <param name="time">The time in milliseconds.</param>
        /// <returns>The time in hh:mm:ss format (fractions of seconds are discarded.)</returns>
        public static string FormatTime_hh_mm_ss(double time)
        {
            int s = Convert.ToInt32(time / 1000.0);
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(s / 60);
            str = (m % 60).ToString("00") + ":" + str;
            int h = m / 60;
            return h.ToString("00") + ":" + str;
        }

        private static string FormatTime_mm_ss(double time)
        {
            int s = Convert.ToInt32(Math.Floor(time / 1000.0));
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(Math.Floor(s / 60.0));
            return m.ToString("00") + ":" + str;
        }

        private static string FormatTime_ss_ms(double time)
        {
            time /= 1000.0;
            return time.ToString("0.00") + "s";
        }


        // Sections menu

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

        // Update the status of the blocks menu item with the current selection and tree.
        private void UpdatePhrasesMenu()
        {
            mAddBlankPhraseToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            mAddEmptyPagesToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            mImportAudioFileToolStripMenuItem.Enabled = mProjectView.CanImportPhrases;
            mSplitPhraseToolStripMenuItem.Enabled = mProjectView.CanSplitBlock;
            mMergePhraseWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeBlockWithNext;
            mPhraseIsUsedToolStripMenuItem.Enabled = mProjectView.CanSetBlockUsedStatus;
            mPhraseIsUsedToolStripMenuItem.CheckedChanged -= new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
            mPhraseIsUsedToolStripMenuItem.Checked = mProjectView.IsBlockUsed;
            mPhraseIsUsedToolStripMenuItem.CheckedChanged += new System.EventHandler(mPhraseIsUsedToolStripMenuItem_CheckedChanged);
            mAssignRoleToolStripMenuItem.Enabled = mProjectView.CanAssignRole;
            mPageToolStripMenuItem.Enabled = mProjectView.CanSetPageNumber;
            mEditRolesToolStripMenuItem.Enabled = mSession.Presentation != null;
            mClearRoleToolStripMenuItem.Enabled = mProjectView.CanClearRole;
            PhraseDetectionToolStripMenuItem.Enabled = mProjectView.CanApplyPhraseDetection;
            // mMarkDefaultCustomClassToolStripMenuItem.Enabled = mProjectView.CanMarkPhrase;
            mGoToToolStripMenuItem.Enabled = mSession.Presentation != null;
            UpdateAudioSelectionBlockMenuItems();
        }

        private void UpdateAudioSelectionBlockMenuItems()
        {
            string AudioSelectionStatusMessage = "";
            if (mProjectView.Selection is AudioSelection)
            {
                BeginInPhraseSelectionToolStripMenuItem.Enabled = true;

                if (((AudioSelection)mProjectView.Selection).AudioRange.HasCursor)
                {
                    EndInPhraseSelectionToolStripMenuItem.Enabled = true;
                }

                if (((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime > 0)
                {
                    DeselectInPhraseSelectionToolStripMenuItem.Enabled = true;
                    string BeginTime = Math.Round(((AudioSelection)mProjectView.Selection).AudioRange.SelectionBeginTime / 1000, 1).ToString();
                    string EndTime = Math.Round(((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime / 1000, 1).ToString();
                    //AudioSelectionStatusMessage = string.Concat(" Selected:", ((AudioSelection)mProjectView.Selection).AudioRange.SelectionBeginTime.ToString(), " - ", ((AudioSelection)mProjectView.Selection).AudioRange.SelectionEndTime.ToString());
                    AudioSelectionStatusMessage = string.Concat(Localizer.Message("AudioSelected"), BeginTime, " - ", EndTime, "s");

                }
                else
                {
                    DeselectInPhraseSelectionToolStripMenuItem.Enabled = false;
                    AudioSelectionStatusMessage = "";
                }
            }
            else
            {
                BeginInPhraseSelectionToolStripMenuItem.Enabled = false;
                EndInPhraseSelectionToolStripMenuItem.Enabled = false;
                DeselectInPhraseSelectionToolStripMenuItem.Enabled = false;
                AudioSelectionStatusMessage = "";
            }
            if (AudioSelectionStatusMessage != "")
                Status(Localizer.Message(mProjectView.TransportBar.CurrentPlaylist.State.ToString()) + AudioSelectionStatusMessage);
        }

        private void mAddBlankPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyBlock(); }
        private void mAddEmptyPagesToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyPages(); }
        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ImportPhrases(); }
        private void mSplitPhraseToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitBlock(); }
        private void mMergePhraseWithNextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeBlockWithNext(); }
        private void mPhraseIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.SetSelectedNodeUsedStatus(mPhraseIsUsedToolStripMenuItem.Checked);
        }


        private delegate void DisableCallback(bool disable);

        private void Disable(bool disable)
        {
            if (InvokeRequired)
            {
                Invoke(new DisableCallback(Disable), new object[] { disable });
            }
            else
            {
                Cursor = disable ? Cursors.WaitCursor : Cursors.Default;
                mMenuStrip.Enabled = !disable;
                mProjectView.Enabled = !disable;
            }
        }

        void mProjectView_ImportingFile(object sender, Obi.ProjectView.ImportingFileEventArgs e)
        {
            Disable(true);
            Status(String.Format(Localizer.Message("importing_file"), e.Path));
        }

        void mProjectView_FinishedImportingFiles(object sender, EventArgs e)
        {
            Disable(false);
        }

        private void mPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanSetPageNumber)
            {
                Dialogs.SetPageNumber dialog = new SetPageNumber(mProjectView.NextPageNumber, false, false);
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
                items[index].Text != e.CustomClass; ++index);
            if (index < items.IndexOf(mAddRoleToolStripTextBox)) items.RemoveAt(index);
        }

        private void mClearRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SetCustomTypeForSelectedBlock(EmptyNode.Kind.Plain, null);
        }

        private void mSetAsHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MakeSelectedBlockIntoHeadingPhrase();
        }

        private void mSilenceToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MakeSelectedBlockIntoSilencePhrase(); }

        private void mEditRolesToolStripMenuItem_Click(object sender, EventArgs e)
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

        // Update the transport manu
        private void UpdateTransportMenu()
        {
            mPlayAllToolStripMenuItem.Enabled = mProjectView.CanPlay;
            mPlaySelectionToolStripMenuItem.Enabled = mProjectView.CanPlaySelection;
            PlayPreviewtoolStripMenuItem.Enabled = mProjectView.CanPlay;
            FastPlaytoolStripMenuItem.Enabled = mProjectView.CanPlay; 

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

            // update recording menu items
                        mStartRecordingToolStripMenuItem.Enabled = !mProjectView.TransportBar.IsActive;
            if (mProjectView.TransportBar.IsRecorderActive)
            {
                mStartListeningToolStripMenuItem.Enabled = false;
            }
            else
            {
                mStartListeningToolStripMenuItem.Enabled = true;
                mStartRecordingToolStripMenuItem.Enabled = true;
            }
            mStartRecordingToolStripMenuItem.Enabled = mProjectView.TransportBar.Enabled;
            mStartListeningToolStripMenuItem.Enabled = mProjectView.TransportBar.Enabled;
        }

        private void mPlayAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanPlay) mProjectView.TransportBar.PlayAll();
        }

        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanPlaySelection) mProjectView.TransportBar.PlayOrResume(mProjectView.Selection.Node);
        }

        private void mPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanPause) mProjectView.TransportBar.Pause();
        }

        private void mResumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanResume) mProjectView.TransportBar.PlayOrResume();
        }

        private void mStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.CanStop) mProjectView.TransportBar.Stop();
        }

        private void mStartListeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Record();
        }

        private void mStartRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.StartRecordingDirectly();
        }


        private void rewindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Rewind();
        }

        private void fastForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.FastForward();
        }

        private void previousSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PrevSection();
        }

        private void previousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PrevPhrase();
        }

        private void previousPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PrevPage();
        }

        private void nextPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.NextPhrase();
        }

        private void nextSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.NextSection(); 
        }

        private void nextPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.NextPage();
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

        private void PreviewFromtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PlayPreviewFromCurrentPosition();
        }

        private void PreviewUptotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PlayPreviewUptoCurrentPosition() ;
        }

        private void PreviewSelectedAudiotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PlayPreviewSelectedFragment();
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

        private void mMarkDefaultCustomClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.TransportBar.Enabled) mProjectView.TransportBar.MarkCustomClass();
        }

        private void mTodoClasstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.TransportBar.Enabled
                && mProjectView.TransportBar.IsActive)
            {
                mProjectView.TransportBar.MarkTodoClass();
            }
            else
            {
                EmptyNode node = mProjectView.SelectedNodeAs<EmptyNode>();
                if (node != null)
                {
                    mProjectView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mProjectView, node,
                       EmptyNode.Kind.TODO));
                }
            }
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
    }
}