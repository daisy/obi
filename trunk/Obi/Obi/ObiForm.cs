using Obi.Commands;
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

        private int STATUS_TIMER_DELAY_IN_SECONDS = 30;  // amount of time status messages stay on

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
        }


        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings { get { return mSettings; } }

        /// <summary>
        /// Display a message in the status bar.
        /// </summary>
        public void Status(string message) { mStatusLabel.Text = message; }


        #region File menu

        private void UpdateFileMenu()
        {
            mNewProjectToolStripMenuItem.Enabled = true;
            mNewProjectFromImportToolStripMenuItem.Enabled = false;  // currently disabled
            mOpenProjectToolStripMenuItem.Enabled = true;
            mOpenRecentProjectToolStripMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            mClearListToolStripMenuItem.Enabled = true;
            mSaveProjectToolStripMenuItem.Enabled = mSession.CanSave;
            mSaveProjectAsToolStripMenuItem.Enabled = false;  // mSession.HasProject;
            mDiscardChangesToolStripMenuItem.Enabled = false;  // mSession.CanSave;
            mCloseProjectToolStripMenuItem.Enabled = mSession.HasProject;
            mCleanProjectToolStripMenuItem.Enabled = false; // currently disabled
            mExportAsDAISYToolStripMenuItem.Enabled = mSession.HasProject;  // currently disabled
            mExitToolStripMenuItem.Enabled = true;
        }

        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e) { NewProject(); }
        private void mNewProjectFromImportToolStripMenuItem_Click(object sender, EventArgs e) { NewProjectFromImport(); }
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e) { Open(); }
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e) { ClearRecentProjectsList(); }
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e) { Save(); }
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e) { SaveAs(); }
        private void mDiscardChangesToolStripMenuItem_Click(object sender, EventArgs e) { DiscardChanges(); }
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e) { DidCloseProject(); }
        private void mCleanProjectToolStripMenuItem_Click(object sender, EventArgs e) { Cleanup(); }
        private void mExportAsDAISYToolStripMenuItem_Click(object sender, EventArgs e) { Export(); }
        private void mExitToolStripMenuItem_Click(object sender, EventArgs e) { Close(); }

        // Revert the project back to its last saved state.
        private void DiscardChanges()
        {
            if (mSession.CanSave &&
                MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
            {
                OpenProject(mSession.Path);
            }
        }

        // Create a new project by importing an XHTML file
        private void NewProjectFromImport()
        {
            /*
            mProjectView.TransportBar.Enabled = false;
            if (!DidCloseProject())
            {
                mProjectView.TransportBar.Enabled = true;
                Ready();
                return;
            }

            //select a file for import
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Choose a file for import";
            openFile.Filter = "HTML | *.html";

            if (openFile.ShowDialog() != DialogResult.OK) return;

            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_project_extension"),
                ImportStructure.grabTitle(new Uri(openFile.FileName)));
            dialog.MakeAutoTitleCheckboxInvisible();
            dialog.Text = "Create a new project starting from XHTML import";
            if (dialog.ShowDialog() != DialogResult.OK) return;

            // let's see if we can actually write the file that the user chose (bug #1679175)
            try
            {
                FileStream file = File.Create(dialog.Path);
                file.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show(String.Format(Localizer.Message("cannot_create_file_text"), dialog.Path, x.Message),
                    Localizer.Message("cannot_create_file_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            CreateNewProject(dialog.Path, dialog.Title, false);
            try
            {
                ImportStructure importer = new ImportStructure();
                //importer.ImportFromXHTML(openFile.FileName, mProject);
            }
            catch (Exception ex)
            {
                //report failure and undo the creation of a new project
                MessageBox.Show("Import failed: " + ex.Message);
                //mProject.Close();
                File.Delete(dialog.Path);
                mProjectView.TransportBar.Enabled = false;
                RemoveRecentProject(dialog.Path);
                return;
            }

            Ready();
            mProjectView.TransportBar.Enabled = true;
             * */
        }

        // Save the current project under a different name; ask for a new path first.
        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = Localizer.Message("obi_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mSession.SaveAs(dialog.FileName);
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }
        }

        #endregion

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
            mDeleteToolStripMenuItem.Enabled = mProjectView.CanDelete;
            mSelectNothingToolStripMenuItem.Enabled = mProjectView.CanDeselect;
            mFindInTextToolStripMenuItem.Enabled = mSession.HasProject;
            mFindNextToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
            mFindPreviousToolStripMenuItem.Enabled = mProjectView.CanFindNextPreviousText;
        }

        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e) { Undo(); }
        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e) { Redo(); }
        private void mCutToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Cut(); }
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Copy(); }
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Paste(); }
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Delete(); }
        private void mSelectNothingToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SelectNothing(); }
        private void mFindInTextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.FindInText(); }

        #endregion

        #region View menu

        private void UpdateViewMenu()
        {
            mShowTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowMetadataViewToolStripMenuItem.Enabled = mSession.HasProject;
            mShowSearchBarToolStripMenuItem.Enabled = mSession.HasProject;
            mShowTransportBarToolStripMenuItem.Enabled = mSession.HasProject;
            mShowStatusBarToolStripMenuItem.Enabled = true;
            mFocusOnStripsViewToolStripMenuItem.Enabled = mSession.HasProject;
            mFocusOnTOCViewToolStripMenuItem.Enabled = mSession.HasProject;
            mSynchronizeViewsToolStripMenuItem.Enabled = mSession.HasProject;
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

        private void mShowSearchBarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.FindInTextVisible = mShowSearchBarToolStripMenuItem.Checked;
        }

        private void mShowTransportBarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.TransportBarVisible = mShowTransportBarToolStripMenuItem.Checked;
        }

        private void mShowStatusBarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mStatusStrip.Visible = mShowStatusBarToolStripMenuItem.Checked;
        }

        private void mFocusOnStripsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSelectedSectionInStripsView();
        }

        private void mFocusOnTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSelectedSectionInTOCView();
        }

        private void mSynchronizeViewsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SynchronizeViews = mSynchronizeViewsToolStripMenuItem.Checked;
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

        // A new presentation was loaded or created.
        private void GotNewPresentation()
        {
            UpdateObi();
            mProjectView.Presentation = mSession.Presentation;
            mSession.Presentation.CommandExecuted += new UndoRedoEventHandler(Presentation_CommandExecuted);
            mSession.Presentation.CommandUnexecuted += new UndoRedoEventHandler(Presentation_CommandUnexecuted);
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
                mProjectView.FindInTextVisibilityChanged += new EventHandler(ProjectView_FindInTextVisibilityChanged);
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
                mStatusTimer.Interval = STATUS_TIMER_DELAY_IN_SECONDS * 1000;
                mStatusTimer.Tick += new EventHandler(delegate(object sender, EventArgs e) { Ready(); });
                // these should be stored in settings
                mShowTOCViewToolStripMenuItem.Checked = mProjectView.TOCViewVisible = true;
                mShowMetadataViewToolStripMenuItem.Checked = mProjectView.MetadataViewVisible = true;
                mShowSearchBarToolStripMenuItem.Checked = mProjectView.FindInTextVisible = false;
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
                MessageBox.Show(String.Format(Localizer.Message("init_error"), path, e.ToString()),
                    Localizer.Message("init_error_title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Initialize event handlers from the project view
        private void InitializeEventHandlers()
        {
            mProjectView.TransportBar.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
            mProjectView.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
            mProjectView.ImportingFile += new Obi.ProjectView.ImportingFileEventHandler(mProjectView_ImportingFile);
            mProjectView.FinishedImportingFiles += new EventHandler(mProjectView_FinishedImportingFiles);
        }

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void NewProject()
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(
                mSettings.DefaultPath,
                Localizer.Message("default_project_filename"),
                Localizer.Message("obi_project_extension"),
                Localizer.Message("default_project_title"));
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection);
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
                mSettings.LastOpenProject = "";
                mSession.Close();
            }
        }

        // The project was modified.
        private void ProjectHasChanged()
        {
            mSession.PresentationHasChanged();
            UpdateObi();
        }

        // Update the status bar to say "Ready."
        private void Ready()
        {
            mStatusTimer.Enabled = false;
            Status(Localizer.Message("ready"));
        }

        // Redo
        private void Redo()
        {
            if (mSession.CanRedo) mSession.Presentation.UndoRedoManager.redo();
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
            }
            else
            {
                mPeakMeter.Focus();
            }
            this.Ready();
        }

        // Save the current project
        private void Save()
        {
            if (mSession.CanSave)
            {
                mSession.Save();
                AddRecentProject(mSession.Path);
            }
        }

        // Undo
        private void Undo()
        {
            if (mSession.CanUndo) mSession.Presentation.UndoRedoManager.undo();
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
            UpdateTOCMenu();
            UpdateStripsMenu();
            UpdateBlocksMenu();
        }

        // Update the title and status bars to show the name of the project, and if it has unsaved changes
        private void UpdateTitleAndStatusBar()
        {
            Text = mSession.HasProject ?
                String.Format(Localizer.Message("title_bar"), mSession.Presentation.Title,
                    (mSession.CanSave ? "*" : ""), Localizer.Message("obi")) :
                Localizer.Message("obi");
            if (mStatusTimer.Enabled) mStatusTimer.Enabled = false;
            mStatusTimer.Enabled = true;
        }


        #region Event handlers

        private void Presentation_CommandUnexecuted(object sender, UndoRedoEventArgs e) { ProjectHasChanged(); }
        private void Presentation_CommandExecuted(object sender, UndoRedoEventArgs e) { ProjectHasChanged(); }
        private void ProjectView_SelectionChanged(object sender, EventArgs e) { UpdateMenus(); }

        private void ProjectView_FindInTextVisibilityChanged(object sender, EventArgs e)
        {
            mShowSearchBarToolStripMenuItem.Checked = mProjectView.FindInTextVisible;
        }

        private void Session_ProjectCreated(object sender, EventArgs e)
        {
            GotNewPresentation();
            Status(String.Format(Localizer.Message("created_new_project"), mSession.Presentation.Title));
        }

        private void Session_ProjectClosed(object sender, ProjectClosedEventArgs e)
        {
            UpdateObi();
            if (e.ClosedPresentation != null && e.ClosedPresentation.Initialized)
            {
                Status(String.Format(Localizer.Message("closed_project"), e.ClosedPresentation.Title));
            }
            mProjectView.Presentation = null;
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
            Status(String.Format(Localizer.Message("saved_project"), mSession.Path));
        }

        #endregion



        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            Status(Localizer.Message(mProjectView.TransportBar.CurrentPlaylist.State.ToString()));
        }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectView.TransportBar.CurrentPlaylist.PlaybackRate));
        }


        #region File menu event handlers


        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>





        private void Cleanup()
        {            /*
            if (mProject != null)
            {
                mProjectView.TransportBar.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                try
                {
                    mProject.CleanProjectAssets();
                }
                catch (Exception x)
                {
                    //report an error and exit the function
                    MessageBox.Show(String.Format(Localizer.Message("didnt_clean_project_text"), x.Message),
                            Localizer.Message("didnt_clean_project_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.Cursor = Cursors.Default;
                    mProjectView.TransportBar.Enabled = true;
                    return;
                }

                //report success
                MessageBox.Show(Localizer.Message("cleaned_project_text"), Localizer.Message("cleaned_project_caption"),
                         MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Cursor = Cursors.Default;
                mProjectView.TransportBar.Enabled = true;
            }
             */

        }

        /// <summary>
        /// Export makes an in-memory copy of the project tree and export it as a Daisy book
        /// </summary>
        private void Export()
        {
            mProjectView.TransportBar.Enabled = false;
            
            //should we prompt the user to save first before exporting?  we used to do this.

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = Localizer.Message("export_choose_folder");
            dialog.SelectedPath = mSettings.DefaultExportPath;
            if (dialog.ShowDialog() == DialogResult.OK && IsExportDirectoryReady(dialog.SelectedPath))
            {
                try
                {
                    //need the trailing slash -- otherwise exported data ends up in a folder one level higher than our selection
                    string path = dialog.SelectedPath;
                    if (!path.EndsWith("/")) path += "/";
                    mSession.Presentation.ExportToZed(new System.Uri(path));
                    MessageBox.Show(String.Format(Localizer.Message("saved_as_daisy_text"), dialog.SelectedPath),
                    Localizer.Message("saved_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception x)
                {
                    MessageBox.Show(String.Format(Localizer.Message("didnt_save_as_daisy_text"), dialog.SelectedPath, x.Message),
                    Localizer.Message("didnt_save_as_daisy_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Ready();
            }
            mProjectView.TransportBar.Enabled = true;
        }

        /// <summary>
        /// The export directory is ready if it doesn't exist and can be created, or exists
        /// and is empty or can be emptied (or the user decided not to empty it.)
        /// </summary>
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

        #endregion


        /// <summary>
        /// Handle errors when closing a project.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void ReportDeleteError(string path, string message)
        {
            MessageBox.Show(String.Format(Localizer.Message("report_delete_error"), path, message));
        }

        /// <summary>
        /// Edit the user profile through the user profile dialog.
        /// </summary>
        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.UserProfile dialog = new Dialogs.UserProfile(mSettings.UserProfile);
            dialog.ShowDialog();
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Project tab. (JQ)
        /// </summary>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mSession.Presentation, mProjectView.TransportBar);
            dialog.SelectProjectTab();
            ShowPreferencesDialog(dialog);
        }

        private void ShowPreferencesDialog(Dialogs.Preferences dialog)
        {
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Audio tab. (JQ)
        /// </summary>
        private void mAudioPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mSession.Presentation, mProjectView.TransportBar);
            dialog.SelectAudioTab();
            ShowPreferencesDialog(dialog);
        }

        /// <summary>
        /// Update the settings after the user has made some changes in the preferrences dialog. (JQ)
        /// </summary>
        private void UpdateSettings(Dialogs.Preferences dialog)
        {
            if (dialog.IdTemplate.Contains("#")) mSettings.IdTemplate = dialog.IdTemplate;
            if (Directory.Exists(dialog.DefaultXUKDirectory)) mSettings.DefaultPath = dialog.DefaultXUKDirectory;
            if (Directory.Exists(dialog.DefaultDAISYDirectory)) mSettings.DefaultExportPath = dialog.DefaultDAISYDirectory;
            mSettings.OpenLastProject = dialog.OpenLastProject;
            mSettings.LastOutputDevice = dialog.OutputDevice.Name;
            mProjectView.TransportBar.AudioPlayer.SetDevice(this, dialog.OutputDevice);
            mSettings.LastInputDevice = dialog.InputDevice.Name;
            mProjectView.TransportBar.Recorder.InputDevice = dialog.InputDevice;
            mSettings.AudioChannels = dialog.AudioChannels;
            mSettings.SampleRate = dialog.SampleRate;
            mSettings.BitDepth = dialog.BitDepth;
            // tooltips
            mSettings.EnableTooltips = dialog.EnableTooltips;
            mProjectView.EnableTooltips = dialog.EnableTooltips;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        /// <remarks>Warn when closing while playing?</remarks>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
                mProjectView.TransportBar.Stop();
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
        /// Update the TOC menu when a tree node is (de)selected.
        /// </summary>
        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mRenameSectionToolStripMenuItem.Enabled = e.Selected;
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
        }

        /// <summary>
        /// Update the title and status bar when the project is closed.
        /// </summary>
        private void StatusUpdateClosedProject()
        {
            /*this.Text = Localizer.Message("obi");
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                mStatusLabel.Text = String.Format(Localizer.Message("closed_project"), mProject.Title);
                mProjectView.Project = null;
                EnableItemsProjectClosed();
            }*/
        }

        private void EnableItemsProjectClosed()
        {
        }

        /// <summary>
        /// Update the form (title and status bar) when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            /*this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("opened_project"), mProject.XUKPath));*/
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            /*this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("saved_project"), mProject.LastPath));*/
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            /*this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title + "*");
            Ready();*/
        }














        /// <summary>
        /// Update the visibility and actual label of transport items.
        /// </summary>
        private void mTransportToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTransportMenu();
        }

        private void UpdateEnabledItemsForTransportMenu()
        {
            /*bool isProjectOpen = mProject != null;
            bool isNodeSelected = isProjectOpen && mProjectView.Selection != null;

            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message((mVuMeterForm != null && mVuMeterForm.Visible) ? "hide_vu_meter" : "show_vu_meter");

            if (mProjectView.TransportBar.IsInlineRecording)
            {
                mPlayAllToolStripMenuItem.Enabled = true;
                mPlaySelectionToolStripMenuItem.Enabled = true;
                mStopToolStripMenuItem.Enabled = true;
            }
            else
            {

                if (mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    mPlayAllToolStripMenuItem.Enabled = isProjectOpen;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    mPlaySelectionToolStripMenuItem.Enabled = isNodeSelected;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = isNodeSelected;
                }
                else if (mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.NotReady)
                {
                    mPlayAllToolStripMenuItem.Enabled = false;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    mPlaySelectionToolStripMenuItem.Enabled = false;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = false;
                }
                else if (mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    // Avn: changed to allowdirect change of Playback mode
                    mPlayAllToolStripMenuItem.Enabled = true; // mProjectView.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                    // Avn: changed to allowdirect change of Playback mode
                    mPlaySelectionToolStripMenuItem.Enabled = (mProjectView.Selection != null);  // !mProjectView.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                    mStopToolStripMenuItem.Enabled = true;
                }
                else // playing
                {
                    // Avn: changed to allowdirect change of Playback mode
                    mPlayAllToolStripMenuItem.Enabled = true; //mProjectView.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlayAllToolStripMenuItem.Text = Localizer.Message("pause_all");
                    // Avn: changed to allowdirect change of Playback mode
                    mPlaySelectionToolStripMenuItem.Enabled = (mProjectView.Selection != null);  // !mProjectView.TransportBar._CurrentPlaylist.WholeBook;
                    //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("pause");
                    mStopToolStripMenuItem.Enabled = true;
                }
                mRecordToolStripMenuItem.Enabled = mProjectView.TransportBar.CanRecord;
            }*/
        }

        internal void UndoLast()
        {
            /*if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                //UpdateEnabledItemsForUndoRedo();
            }*/
        }

        // Transport bar stuff

        #region transport bar

        /// <summary>
        /// Play the whole book from the selected node, or from the beginning.
        /// If already playing, pause.
        /// </summary>
        private void mPlayAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Play();
        }

        /// <summary>
        /// Play the current selection (phrase or section.)
        /// </summary>
        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Play(mProjectView.SelectionNode);
        }

        /// <summary>
        /// Play a single phrase node using the transport bar.
        /// </summary>
        private void Play(ObiNode node)
        {
            mProjectView.TransportBar.Play(node);
        }

        private void mPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Pause();
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void mStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_Stop();
        }

        private void TransportBar_Stop()
        {
            mProjectView.TransportBar.Stop();
        }

        /// <summary>
        /// Record new assets.
        /// </summary>
        private void mRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Record();
        }

        #endregion

        private void mRewindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Rewind();
        }

        private void mFastForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.FastForward();
        }

        private void mPreviousSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.PrevSection();
        }

        private void mPreviousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_PreviousPhrase();
        }

        private void TransportBar_PreviousPhrase()
        {
            if (mProjectView.TransportBar.CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectView.TransportBar.PrevPhrase();
            }
        }

        private void mNextPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_NextPhrase();
        }

        private void TransportBar_NextPhrase()
        {
            if (mProjectView.TransportBar.CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectView.TransportBar.NextPhrase();
            }
        }

        private void mNextSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.NextSection();
        }

        // Various utility functions


        private void mViewHelpInExternalBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                Localizer.Message("help_html")))).ToString());
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
        ///  move keyboard focus amung TOC view, Strip view, Transport Bar
        /// <see cref=""/>
        /// </summary>
        /// <param name="Clockwise">
        ///  true for clockwise movement
        /// </param>
        private void MoveToNextPanel(bool Clockwise)
        {

            /*
                        mProjectView.TransportBar.PlayOnFocusEnabled = false;
                        if (mProjectView.CurrentSelection != null)
                        {
                            if (mProjectView.TOCPanel.ContainsFocus)
                            {
                                if (Clockwise)
                                {
                                    NodeSelection TempnodeSelection = mProjectView.CurrentSelection;
                                    mProjectView.StripManager.Focus();
                                    mProjectView.CurrentSelection = new NodeSelection(TempnodeSelection.Node, mProjectView.StripManager);
                                }
                                else
                                    mProjectView.TransportBar.Focus();
                            }
                            else if (mProjectView.StripManager.ContainsFocus)
                            {
                                if (Clockwise)
                                    mProjectView.TransportBar.Focus();
                                else
                                    FocusTOCPanel();
                            }
                            else if (mProjectView.TransportBar.ContainsFocus)
                            {
                                if (Clockwise)
                                    FocusTOCPanel();
                                else
                                {
                                    NodeSelection TempnodeSelection = mProjectView.CurrentSelection;
                                    mProjectView .StripManager.Focus();
                                    mProjectView.CurrentSelection = new NodeSelection(TempnodeSelection.Node , mProjectView.StripManager);
                                                                    }
                            }
                        }
                        else
                            mProjectView.TOCPanel.Focus();
            mProjectView.TransportBar.PlayOnFocusEnabled = true;
             * */
        }


        /// <summary>
        ///  convenience function to be used in MoveToNextPanel ()
        /// <see cref=""/>
        /// </summary>
        private void FocusTOCPanel()
        {
            /*
            if (mProjectView.CurrentSelectionNode.GetType().Name == "PhraseNode")
            {
                PhraseNode TempPhraseNode = mProjectView.CurrentSelectionNode as PhraseNode;
                mProjectView.CurrentSelection = new NodeSelection(TempPhraseNode.ParentAs<SectionNode>(), mProjectView.TOCPanel);
                mProjectView.TOCPanel.Focus();
            }
            else
                mProjectView.StripManager.ShowInTOCPanel();
            */
        }

        //added by med june 4 2007
        /// <summary>
        /// Import an XHTML file and build the project structure from it.
        /// The requirements for the file to import are:
        /// 1. it is well-formed
        /// 2. the headings are ordered properly (i.e. h2 comes between h1 and h3)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>



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
            if (checkEmpty && Directory.GetFiles(path).Length > 0)
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
                String.Format(Localizer.Message("create_directory_query"), path),
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
                        String.Format(Localizer.Message("create_directory_failure"), path, e.Message),
                        Localizer.Message("error"),
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


        // TOC menu

        private void UpdateTOCMenu()
        {
            mAddSectionToolStripMenuItem.Enabled = mProjectView.CanAddSection;
            mAddSubSectionToolStripMenuItem.Enabled = mProjectView.CanAddSubSection;
            mRenameSectionToolStripMenuItem.Enabled = mProjectView.CanRenameSection;
            mMoveOutToolStripMenuItem.Enabled = mProjectView.CanMoveSectionOut;
            mMoveInToolStripMenuItem.Enabled = mProjectView.CanMoveSectionIn;
            mSectionIsUsedToolStripMenuItem.Enabled = mProjectView.CanToggleSectionUsed;
            mSectionIsUsedToolStripMenuItem.Checked = mProjectView.CanMarkSectionUnused;
            mListenToSectionToolStripMenuItem.Enabled = mProjectView.CanListenToSection;
        }

        private void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewSection(); }
        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewSubSection(); }
        private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.StartRenamingSelectedSection(); }
        private void mMoveOutToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MoveSelectedSectionOut(); }
        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MoveSelectedSectionIn(); }
        private void mSectionIsUsedToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.MarkSectionUsed(mSectionIsUsedToolStripMenuItem.Checked);
        }
        private void mListenToSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ListenToSelection(); }

        // Strips menu

        private void UpdateStripsMenu()
        {
            mInsertStripToolStripMenuItem.Enabled = mProjectView.CanAddStrip;
            mRenameStripToolStripMenuItem.Enabled = mProjectView.CanRenameStrip;
            mSplitStripToolStripMenuItem.Enabled = mProjectView.CanSplitStrip;
            mMergeWithNextStripToolStripMenuItem.Enabled = mProjectView.CanMergeStripWithNext;
            mListenToStripToolStripMenuItem.Enabled = mProjectView.CanListenToStrip;
        }

        private void mInsertStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewStrip(); }
        private void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.StartRenamingSelectedStrip(); }
        private void mSplitStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitStrip(); }
        private void mMergeWithNextStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }
        private void mListenToStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ListenToSelection(); }

        // Blocks menu

        private void UpdateBlocksMenu()
        {
            mAddEmptyBlockToolStripMenuItem.Enabled = mProjectView.CanAddEmptyBlock;
            mImportAudioFileToolStripMenuItem.Enabled = mProjectView.CanImportPhrases;
            mSplitBlockToolStripMenuItem.Enabled = mProjectView.CanSplitBlock;
            mMergeBlockWithNextToolStripMenuItem.Enabled = mProjectView.CanMergeBlockWithNext;
            mListenToBlockToolStripMenuItem.Enabled = (mProjectView.SelectedBlockNode != null);
            mAssignRoleToolStripMenuItem.Enabled = (mProjectView.SelectedBlockNode != null);
            mPageToolStripMenuItem.Enabled = mProjectView.CanSetPageNumber;
            mEditRolesToolStripMenuItem.Enabled = mSession.Presentation != null;
            mClearRoleToolStripMenuItem.Enabled = (mProjectView.SelectedBlockNode != null && mProjectView.SelectedBlockNode.NodeKind != EmptyNode.Kind.Plain);
        }

        private void mAddEmptyBlockToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddEmptyBlock(); }
        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ImportPhrases(); }
        private void mSplitBlockToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.SplitBlock(); }
        private void mMergeBlockWithNextToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeBlockWithNext(); }

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
                Dialogs.SetPageNumber dialog = new SetPageNumber(mProjectView.NextPageNumber, false);
                if (dialog.ShowDialog() == DialogResult.OK) mProjectView.SetPageNumberOnSelectedBock(dialog.Number, dialog.Renumber);
            }
        }


        #region OLD

        // TODO: merge full and simple metadata editing into a single dialog with two tabs
        private void mFullMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*FullMetadata dialog = new FullMetadata(mProject);
            List<urakawa.metadata.Metadata> affected = new List<urakawa.metadata.Metadata>();
            foreach (object o in mProject.getPresentation().getMetadataList())
            {
                urakawa.metadata.Metadata meta = (urakawa.metadata.Metadata)o;
                if (MetadataEntryDescription.GetDAISYEntries().Find(delegate(MetadataEntryDescription entry)
                    { return entry.Name == meta.getName(); }) != null)
                {
                    affected.Add(meta);
                    dialog.AddPanel(meta.getName(), meta.getContent());
                }
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (urakawa.metadata.Metadata m in affected) mProject.getPresentation().deleteMetadata(m.getName());
                foreach (UserControls.MetadataPanel p in dialog.MetadataPanels)
                {
                    if (p.CanSetName)
                    {
                        urakawa.metadata.Metadata m = (urakawa.metadata.Metadata)mProject.getPresentation().getMetadataFactory().createMetadata();
                        m.setName(p.EntryName);
                        m.setContent(p.EntryContent);
                        mProject.getPresentation().appendMetadata(m);
                    }
                    else
                    {
                        MessageBox.Show(String.Format(Localizer.Message("error_metadata_name_message"), p.EntryName),
                            Localizer.Message("error_metadata_name_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                mProject.Touch();
            }*/
        }
        #endregion

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
                blocksToolStripMenuItem.DropDown.Close();
                mAddRoleToolStripTextBox.Text = Localizer.Message("add_role");
            }
        }
        private void mMakeBlockIntoContainerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MakeBlockIntoContainer();
        }

        private void mRemoveContainerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.RemoveContainer();
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
    }
}
