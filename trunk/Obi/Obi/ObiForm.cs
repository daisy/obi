using Obi.Commands;
using Obi.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;
using urakawa.core;

namespace Obi
{
    /// <summary>
    /// The main for of the application.
    /// The form consists mostly of a menu bar and a project panel.
    /// We also keep an undo stack (the command manager) and settings.
    /// </summary>
    public partial class ObiForm : Form
    {
        private Project mProject;                // the project currently being authored
        private Settings mSettings;              // application settings
        private CommandManager mCommandManager;  // the undo stack for this project (should it belong to the project?)
        private Audio.VuMeterForm mVuMeter;      // keep track of a single Vu meter form

        /// <summary>
        /// The VU meter form owned by the main form can be shown and hidden from a menu.
        /// </summary>
        public Audio.VuMeterForm VuMeterForm
        {
            get { return mVuMeter; }
        }

        /// <summary>
        /// Initialize a new form. No project is opened at creation time.
        /// </summary>
        /// <remarks>TODO: have an option to open a project automatically when the application launches.</remarks>
        public ObiForm()
        {
            InitializeComponent();
            mProject = null;
            mSettings = null;
            mCommandManager = new CommandManager();
            mVuMeter = new Audio.VuMeterForm(Audio.AudioPlayer.Instance.VuMeter);
            mVuMeter.VuMeter.SetEventHandlers();
            mVuMeter.Visible = false;
            InitializeSettings();
            StatusUpdateClosedProject();  // no project opened, same as if we closed a project.
        }

        #region GUI event handlers

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK && ClosedProject())
            {
                mProject = InitProject();
                mProject.Create(dialog.Path, dialog.Title, mSettings.IdTemplate, mSettings.UserProfile,
                    dialog.CreateTitleSection);
                mSettings.CreateTitleSection = dialog.CreateTitleSection;
                AddRecentProject(mProject.XUKPath);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Initialize a blank project and set some event handlers.
        /// </summary>
        /// <returns>The blank project thus initialized.</returns>
        private Project InitProject()
        {
            Project project = new Project();
            project.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            project.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
            return project;
        }

        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("xuk_filter");
                dialog.InitialDirectory = mSettings.DefaultPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    TryOpenProject(dialog.FileName);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Clear the list of recently opened files (prompt the user first.)
        /// </summary>
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Localizer.Message("clear_recent_text"),
                    Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                ClearRecentList();
            }
            Ready();
        }

        /// <summary>
        /// Save the current project under its current name, or ask for one if none is defined yet.
        /// </summary>
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProject.Save();
                mCommandManager.Clear();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = Localizer.Message("xuk_filter");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.SaveAs(dialog.FileName);
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Revert the project to its last saved state.
        /// </summary>
        private void mDiscardChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved &&
                MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                DoOpenProject(mProject.XUKPath);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Close and clean up the current project.
        /// </summary>
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject = null;
                mCommandManager.Clear();
            }
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
        /// Exit if and only if the currently open project was saved correctly.
        /// </summary>
        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
            if (mProject != null && dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.Touch();
            }
            Ready();
        }

        /// <summary>
        /// Touch the project so that it seems that it was modified.
        /// Also refresh the display.
        /// </summary>
        private void mTouchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                if (!mCommandManager.HasUndo) mProject.Touch();
                mProjectPanel.SynchronizeWithCoreTree(mProject.RootNode);
            }
        }

        /// <summary>
        /// Show or hide the NCX panel in the project panel.
        /// </summary>
        private void mShowhideTableOfContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TOCPanelVisible)
            {
                mProjectPanel.HideTOCPanel();
            }
            else
            {
                mProjectPanel.ShowTOCPanel();
            }
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
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings);
            dialog.SelectProjectTab();
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Audio tab. (JQ)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void audioPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings);
            dialog.SelectAudioTab();
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Update the settings after the user has made some changes in the preferrences dialog. (JQ)
        /// </summary>
        private void UpdateSettings(Dialogs.Preferences dialog)
        {
            if (dialog.IdTemplate.Contains("#")) mSettings.IdTemplate = dialog.IdTemplate;
            if (Directory.Exists(dialog.DefaultDir)) mSettings.DefaultPath = dialog.DefaultDir;
            // mSettings.LastOutputDevice = dialog.OutputDevice;
            // Audio.AudioPlayer.Instance.SetDevice(this, dialog.OutputDeviceIndex);
            mSettings.LastOutputDevice = dialog.OutputDevice.Name;
            Audio.AudioPlayer.Instance.SetDevice(this, dialog.OutputDevice);
            // mSettings.LastInputDevice = dialog.InputDevice;
            // Audio.AudioRecorder.Instance.SetInputDeviceForRecording(this, dialog.InputDeviceIndex);
            mSettings.LastInputDevice = dialog.InputDevice.Name;
            Audio.AudioRecorder.Instance.InputDevice = dialog.InputDevice;
            mSettings.AudioChannels = dialog.AudioChannels;
            mSettings.SampleRate = dialog.SampleRate;
            mSettings.BitDepth = dialog.BitDepth;
        }

        /// <summary>
        /// Save the settings when closing.
        /// TODO: check for closing project?
        /// </summary>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClosedProject())
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
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
                Ready();
            }
        }

        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    StatusUpdateClosedProject();
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    FormUpdateModifiedProject();
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    mProjectPanel.Project = mProject;
                    FormUpdateOpenedProject();
                    mCommandManager.Clear();
                    mProjectPanel.SynchronizeWithCoreTree(mProject.getPresentation().getRootNode());
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
            }
        }

        /// <summary>
        /// Add a new command to the command manager and update the status display and menu items.
        /// </summary>
        private void mProject_CommandCreated(object sender, Events.Project.CommandCreatedEventArgs e)
        {
            mCommandManager.Add(e.Command);
            FormUpdateUndoRedoLabels();
        }

        /// <summary>
        /// Setup the TOC and strip menus in the same way as the context menus for TOCPanel and StripManager.
        /// </summary>
        private void ObiForm_Load(object sender, EventArgs e)
        {
            // The TOC menu behaves like the context menu in the TOC view.
            mAddSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mAddSectionToolStripMenuItem_Click);
            mAddSubSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mAddSubSectionToolStripMenuItem_Click);
            mRenameSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mRenameToolStripMenuItem_Click);
            mMoveInToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.increaseLevelToolStripMenuItem_Click);
            mMoveOutToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.decreaseLevelToolStripMenuItem_Click);
            mShowInStripviewToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mShowInStripViewToolStripMenuItem_Click);

            // The strip menu behaves like the context menu in the strip view.
            mAddStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mAddStripToolStripMenuItem_Click);
            mRenameStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mRenameStripToolStripMenuItem_Click);
            mImportAudioFileToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mImportAudioToolStripMenuItem_Click);
            mSplitAudioBlockToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mSplitAudioBlockToolStripMenuItem_Click);
            mApplyPhraseDetectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mApplyPhraseDetectionToolStripMenuItem_Click);
            mEditAnnotationToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mEditAudioBlockLabelToolStripMenuItem_Click);
            //mg 20060813:
            mMoveAudioBlockForwardToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mMoveAudioBlockForwardToolStripMenuItem_Click); 
            mMoveAudioBlockBackwardToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mMoveAudioBlockBackwardToolStripMenuItem_Click); 
            //end mg 20060813    
            mShowInTOCViewToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mShowInTOCViewToolStripMenuItem_Click);

            mSetPageNumberToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mSetPageNumberToolStripMenuItem_Click);
            mRemovePageNumberToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mRemovePageNumberToolStripMenuItem_Click);
        }

        /// <summary>
        /// Update the TOC menu when a tree node is (de)selected.
        /// </summary>
        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mRenameSectionToolStripMenuItem.Enabled = e.Selected;
            mMoveInToolStripMenuItem.Enabled = e.CanMoveIn;
            mMoveOutToolStripMenuItem.Enabled = e.CanMoveOut;
            mShowInStripviewToolStripMenuItem.Enabled = e.Selected;
        }

        /// <summary>
        /// Show the HTML help page.
        /// </summary>
        private void mHelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Dialogs.Help help = new Dialogs.Help();
            // TODO: Make sure the file corresponds to the current language
            // help.WebBrowser.DocumentStream = GetType().Assembly.GetManifestResourceStream("Obi.help_en.html");
            help.WebBrowser.Url = new Uri(Path.GetDirectoryName(GetType().Assembly.Location) + "\\help_en.html");
            help.ShowDialog();
        }

        /// <summary>
        /// Show the help dialog.
        /// </summary>
        private void mAboutObiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Dialogs.About()).ShowDialog();
        }

        #endregion

        /// <summary>
        /// Try to open a project from a XUK file.
        /// Actually open it only if a possible current project could be closed properly.
        /// </summary>
        /// <param name="path">The path of the XUK file to open.</param>
        private void TryOpenProject(string path)
        {
            if (ClosedProject())
            {
                DoOpenProject(path);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        /// <remarks>TODO: have a progress bar, and hide the panel while opening.</remarks>
        private void DoOpenProject(string path)
        {
            mProject = InitProject();
            this.Cursor = Cursors.WaitCursor;
            try
            {
                mProject.Open(path);
                AddRecentProject(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mProject = null;
            }
            finally
            {
                this.Cursor = Cursors.Default;
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
                Audio.AudioPlayer.Instance.SetDevice(this, mSettings.LastOutputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_output_device_text"), Localizer.Message("no_output_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            try
            {
                Audio.AudioRecorder.Instance.SetDevice(this, mSettings.LastInputDevice);
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("no_input_device_text"), Localizer.Message("no_input_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            while (mOpenRecentProjectToolStripMenuItem.DropDownItems.Count > 2)
            {
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(0);
            }
            mSettings.RecentProjects.Clear();
        }

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// Update the recent menu if necessary.
        /// </summary>
        /// <param name="path">The path of the project to add.</param>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Count == 0)
            {
                mOpenRecentProjectToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }
            if (AddRecentProjectsItem(path)) mSettings.RecentProjects.Insert(0, path);
        }

        /// <summary>
        /// Add an item in the recent projects list, if the file actually exists.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        private bool AddRecentProjectsItem(string path)
        {
            if (File.Exists(path))
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(path);
                item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { TryOpenProject(path); });
                mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the title and status bar when the project is closed.
        /// </summary>
        private void StatusUpdateClosedProject()
        {
            this.Text = Localizer.Message("obi");
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                mToolStripStatusLabel.Text = String.Format(Localizer.Message("closed_project"), mProject.Metadata.Title);
                mProjectPanel.Project = null;
            }
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mToolStripStatusLabel.Text = String.Format(Localizer.Message("opened_project"), mProject.XUKPath);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mToolStripStatusLabel.Text = String.Format(Localizer.Message("saved_project"), mProject.LastPath);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format("{0}* - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            Ready();
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about what to do.
        /// Close the project if that is what the user wants to do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.Save();
                        DoClose();
                        return true;
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.No:
                        DoClose();
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                if (mProject != null) DoClose();
                return true;
            }
        }

        /// <summary>
        /// Clean up and close the project.
        /// Cleaning up only occurs when the project is closed.
        /// </summary>
        private void DoClose()
        {
            if (mProject.Unsaved)
            {
                // A bit kldugy but an easy way to rebuild the list of used files when discarding changes.
                string path = mProject.XUKPath;
                mProject = new Project();
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(
                    delegate(object sender, Obi.Events.Project.StateChangedEventArgs e) { }
                );    
                mProject.Open(path);
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            }
            mProject.DeleteUnusedFiles(ReportDeleteError);
            mProject.Close();
        }

        /// <summary>
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            mToolStripStatusLabel.Text = Localizer.Message("ready");
        }

        /// <summary>
        /// Handle the undo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                FormUpdateUndoRedoLabels();
                if (!mCommandManager.HasUndo) mProject.Reverted();
            }
        }

        /// <summary>
        /// Handle the redo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (mCommandManager.HasRedo)
            {
                mCommandManager.Redo();
                FormUpdateUndoRedoLabels();
            }
            
        }

        private void dumpTreeDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.RootNode.acceptDepthFirst(new Visitors.DumpTree());
            mProject.DumpAssManager();
        }

        #region enabling/disabling of main menu items

        /// <summary>
        /// Update the enabled items for all menus.
        /// </summary>
        /// <remarks>This is necessary to make sure that keyboard shortcuts work correctly.</remarks>
        private void UpdateEnabledItems()
        {
            UpdateEnabledItemsForFileMenu();
        }

        /// <summary>
        /// When the File menu opens, enable and disable its items according to the state of the project.
        /// </summary>
        private void mFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForFileMenu();
        }

        /// <summary>
        /// Update the enabled items of the File menu.
        /// </summary>
        private void UpdateEnabledItemsForFileMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isProjectModified = isProjectOpen && mProject.Unsaved;
            mOpenRecentProjectToolStripMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            mSaveProjectToolStripMenuItem.Enabled = isProjectModified;
            mSaveProjectasToolStripMenuItem.Enabled = isProjectOpen;
            mDiscardChangesToolStripMenuItem.Enabled = isProjectModified;
            mCloseProjectToolStripMenuItem.Enabled = isProjectOpen;
        }

        /// <summary>
        /// When the Edit menu opens, enable and disable its items according to what is selected.
        /// </summary>
        private void mEditToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            FormUpdateUndoRedoLabels();                // take care of undo and redo

            bool isProjectOpen = mProject != null;
            string phraseSectionLabel =
                isProjectOpen && mProjectPanel.StripManager.SelectedPhraseNode != null ?
                    Localizer.Message("phrase") :
                isProjectOpen && (mProjectPanel.StripManager.SelectedSectionNode != null || mProjectPanel.TOCPanel.Selected) ?
                    Localizer.Message("section") :
                    "";
            string pasteLabel =
                isProjectOpen && mProject.Clipboard.Phrase != null ?
                    Localizer.Message("phrase") :
                isProjectOpen && mProject.Clipboard.Section != null ?
                    Localizer.Message("section") :
                    "";

            mCutToolStripMenuItem.Enabled = phraseSectionLabel != "";
            mCutToolStripMenuItem.Text = String.Format(Localizer.Message("cut_menu_label"), phraseSectionLabel);
            mCopyToolStripMenuItem.Enabled = phraseSectionLabel != "";
            mCopyToolStripMenuItem.Text = String.Format(Localizer.Message("copy_menu_label"), phraseSectionLabel);
            mPasteToolStripMenuItem.Enabled = pasteLabel != "";
            mPasteToolStripMenuItem.Text = String.Format(Localizer.Message("paste_menu_label"), pasteLabel);
            mDeleteToolStripMenuItem.Enabled = phraseSectionLabel != "";
            mDeleteToolStripMenuItem.Text = String.Format(Localizer.Message("delete_menu_label"), phraseSectionLabel);

            mMetadataToolStripMenuItem.Enabled = mProject != null;
            mTouchProjectToolStripMenuItem.Enabled = mProject != null;
        }

        /// <summary>
        /// Cut depends on what is selected.
        /// </summary>
        private void mCutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                if (mProjectPanel.StripManager.SelectedPhraseNode != null)
                {
                    mProjectPanel.StripManager.CutSelectedPhrase();
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    mProjectPanel.StripManager.CutSelectedSection();
                }
                else if (mProjectPanel.TOCPanel.Selected)
                {
                    // check that there is actually something that looks selected
                    // from the user POV.
                    mProjectPanel.TOCPanel.CutSelectedSection();
                }
            }
        }

        /// <summary>
        /// Copy depends on what is selected.
        /// </summary>
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                if (mProjectPanel.StripManager.SelectedPhraseNode != null)
                {
                    mProjectPanel.StripManager.CopySelectedPhrase();
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    mProjectPanel.StripManager.CopySelectedSection();
                }
                else if (mProjectPanel.TOCPanel.Selected)
                {
                    // check that there is actually something that looks selected
                    // from the user POV.
                    mProjectPanel.TOCPanel.CopySelectedSection();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>TODO: make only one clipboard visible to the user.</remarks>
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {

            }
        }

        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                if (mProjectPanel.StripManager.SelectedPhraseNode != null)
                {
                    mProjectPanel.StripManager.DeleteSelectedPhrase();
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    mProjectPanel.StripManager.DeleteSelectedSection();
                }
                else if (mProjectPanel.TOCPanel.Selected)
                {
                    // check that there is actually something that looks selected
                    // from the user POV.
                    mProjectPanel.TOCPanel.DeleteSelectedSection();
                }
            }

        }

        /// <summary>
        /// Update the label for undo and redo (and their availability) depending on what is in the command manager.
        /// </summary>
        private void FormUpdateUndoRedoLabels()
        {
            if (mCommandManager.HasUndo)
            {
                mUndoToolStripMenuItem.Enabled = true;
                mUndoToolStripMenuItem.Text = String.Format(Localizer.Message("undo_label"), Localizer.Message("undo"),
                    mCommandManager.UndoLabel);
            }
            else
            {
                mUndoToolStripMenuItem.Enabled = false;
                mUndoToolStripMenuItem.Text = Localizer.Message("undo");
            }
            if (mCommandManager.HasRedo)
            {
                mRedoToolStripMenuItem.Enabled = true;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"),
                    mCommandManager.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = Localizer.Message("redo");
            }
        }

        /// <summary>
        /// The TOC menu is enabled only if the TOC view is visible.
        /// </summary>
        private void mTocToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (mProjectPanel.TOCPanelVisible)
            {
                bool isNodeSelected = false;
                bool canMoveUp = false;
                bool canMoveDown = false;
                bool canMoveIn = false;
                bool canMoveOut = false;

                urakawa.core.CoreNode selectedSection = null;
                if (mProjectPanel.TOCPanel.Selected)
                {
                    isNodeSelected = true;
                    selectedSection = this.mProjectPanel.TOCPanel.SelectedSection;
                }

                mAddSectionToolStripMenuItem.Enabled = mProject != null;
                mAddSubSectionToolStripMenuItem.Enabled = isNodeSelected;

                // JQ 20060818
                // be careful that project is not null when opening the menu...
                mRenameSectionToolStripMenuItem.Enabled = isNodeSelected;

                if (isNodeSelected == true)
                {
                    canMoveUp = mProjectPanel.Project.CanMoveSectionNodeUp(selectedSection);
                    canMoveDown = mProjectPanel.Project.CanMoveSectionNodeDown(selectedSection);
                    canMoveIn = mProjectPanel.Project.CanMoveSectionNodeIn(selectedSection);
                    canMoveOut = mProjectPanel.Project.CanMoveSectionNodeOut(selectedSection);
                }

                mMoveInToolStripMenuItem.Enabled = canMoveIn;
                mMoveOutToolStripMenuItem.Enabled = canMoveOut;

                mShowInStripviewToolStripMenuItem.Enabled = isNodeSelected;
            }
            else
            {
                foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems) item.Enabled = false;
            }
            // Show/hide table of contents
            mShowhideTableOfCOntentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            mShowhideTableOfCOntentsToolStripMenuItem.Enabled = mProject != null;
        }

        /// <summary>
        /// When the Strips menu opens, enable and disable its items according to what is selected.
        /// </summary>
        private void mStripsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool isProjectOpen = mProject != null;
            bool isStripSelected = isProjectOpen && mProjectPanel.StripManager.SelectedSectionNode != null;
            bool isAudioBlockSelected = isProjectOpen && mProjectPanel.StripManager.SelectedPhraseNode != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                Project.GetPhraseIndex(mProjectPanel.StripManager.SelectedPhraseNode) ==
                Project.GetPhrasesCount(mProjectPanel.StripManager.SelectedSectionNode) - 1;
            bool isAudioBlockFirst = isAudioBlockSelected &&
                Project.GetPhraseIndex(mProjectPanel.StripManager.SelectedPhraseNode) == 0;
            bool isBlockClipBoardSet = isProjectOpen && mProject.Clipboard.Phrase != null;
            bool canSetPage = isAudioBlockSelected;  // an audio block must be selected and a heading must not be set.
            bool canRemovePage = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.getProperty(typeof(PageProperty)) != null;

            mAddStripToolStripMenuItem.Enabled = isProjectOpen;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;

            mImportAudioFileToolStripMenuItem.Enabled = isStripSelected;
            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mApplyPhraseDetectionToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithNextAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            mEditAnnotationToolStripMenuItem.Enabled = isAudioBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = isAudioBlockSelected;

            mSetPageNumberToolStripMenuItem.Enabled = canSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = canRemovePage;

            mShowInTOCViewToolStripMenuItem.Enabled = isStripSelected;
        }

        /// <summary>
        /// Update the visibility and actual label of transport items.
        /// </summary>
        private void mTransportToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool isProjectOpen = mProject != null;
            bool isNodeSelected = isProjectOpen && mProjectPanel.SelectedNode != null;
            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message(mVuMeter.Visible ? "hide_vu_meter" : "show_vu_meter");
            if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Stopped)
            {
                mPlayAllToolStripMenuItem.Enabled = isProjectOpen;
                mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = isNodeSelected;
                mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = isNodeSelected;
            }
            else if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.NotReady)
            {
                mPlayAllToolStripMenuItem.Enabled = false;
                mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = false;
                mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = false;
            }
            else if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mPlayAllToolStripMenuItem.Enabled = mProjectPanel.TransportBar.Playlist.WholeBook;
                mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = !mProjectPanel.TransportBar.Playlist.WholeBook;
                mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = true;
            }
            else // playing
            {
                mPlayAllToolStripMenuItem.Enabled = mProjectPanel.TransportBar.Playlist.WholeBook;
                mPlayAllToolStripMenuItem.Text = Localizer.Message("pause_all");
                mPlaySelectionToolStripMenuItem.Enabled = !mProjectPanel.TransportBar.Playlist.WholeBook;
                mPlaySelectionToolStripMenuItem.Text = Localizer.Message("pause");
                mStopToolStripMenuItem.Enabled = true;
            }
            mRecordToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Tools item are always enabled (except for the debug stuff.)
        /// </summary>
        private void mToolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            mDumpTreeDEBUGToolStripMenuItem.Enabled = mProject != null;
        }

        /// <summary>
        /// Help items are always enabled, but we may do some fancy contextual stuff later so here it is for the sake
        /// of completeness.
        /// </summary>
        private void mHelpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        }

        #endregion

        internal void UndoLast()
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                FormUpdateUndoRedoLabels();
            }
        }

        // Transport bar stuff

        #region transport bar

        /// <summary>
        /// Show the VU meter form (creating it if necessary) or hide it.
        /// </summary>
        private void mShowHideVUMeterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mVuMeter != null && mVuMeter.Visible)
            {
                mVuMeter.Hide();
            }
            else
            {
                mVuMeter.Show();
            }
        }

        /// <summary>
        /// Play the whole book from the selected node, or from the beginning.
        /// If already playing, pause.
        /// </summary>
        private void mPlayAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TransportBar.Playlist != null &&
                mProjectPanel.TransportBar.Playlist.State == Audio.AudioPlayerState.Playing)
            {
                mProjectPanel.TransportBar.Pause();
            }
            else
            {
                mProjectPanel.TransportBar.Play();
            }
        }

        /// <summary>
        /// Play the current selection (phrase or section.)
        /// </summary>
        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TransportBar.Playlist != null && 
                mProjectPanel.TransportBar.Playlist.State == Audio.AudioPlayerState.Playing)
            {
                mProjectPanel.TransportBar.Pause();
            }
            else
            {
                Play(mProjectPanel.StripManager.SelectedNode);
            }
        }

        /// <summary>
        /// Play a single phrase node using the transport bar.
        /// </summary>
        internal void Play(urakawa.core.CoreNode node)
        {
            mProjectPanel.TransportBar.Play(node);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void mStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Stop();
        }

        /// <summary>
        /// Record new assets.
        /// </summary>
        private void mRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                CoreNode selected =
                    mProjectPanel.StripManager.SelectedPhraseNode != null ?
                        mProjectPanel.StripManager.SelectedPhraseNode :
                    mProjectPanel.StripManager.SelectedSectionNode != null ?
                        mProjectPanel.StripManager.SelectedSectionNode :
                        null;
                if (selected != null)
                {
                    // section in which we are recording
                    CoreNode section = Project.GetNodeType(selected) == NodeType.Section ?
                        selected :
                        (CoreNode)selected.getParent();
                    // index from which we add new phrases in the aforementioned section
                    int index = selected == section ? section.getChildCount() : section.indexOf(selected);
                    RecordingSession session = new RecordingSession(mProject, Audio.AudioRecorder.Instance, selected,
                        mSettings.AudioChannels, mSettings.SampleRate, mSettings.BitDepth);
                    // the following closures handle the various events sent during the recording session
                    session.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                        delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                        {
                            mProject.StartRecordingPhrase(_e, section, index);
                        }
                    );
                    session.ContinuingPhrase += new Events.Audio.Recorder.ContinuingPhraseHandler(
                        delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                        {
                            mProject.ContinuingRecordingPhrase(_e, section, index);
                        }
                    );
                    session.FinishingPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(
                        delegate(object _sender, Obi.Events.Audio.Recorder.PhraseEventArgs _e)
                        {
                            mProject.FinishRecordingPhrase(_e, section, index);
                        }
                    );
                    new Dialogs.TransportRecord(session).ShowDialog();
                }
            }
            Ready();
        }

        #endregion

        private void mPreviousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.PrevPhrase();
            }
        }

        private void mNextPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.NextPhrase();
            }
        }
    }
}