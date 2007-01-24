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
        private CommandManager mCommandManager;  // the undo stack for this project
        private Audio.VuMeterForm mVuMeterForm;  // keep track of a single VU meter form

        /// <summary>
        /// Application settings.
        /// </summary>
        public Settings Settings
        {
            get { return mSettings; }
        }

        /// <summary>
        /// The VU meter form owned by the main form can be shown and hidden from a menu.
        /// </summary>
        public Audio.VuMeterForm VuMeterForm
        {
            get { return mVuMeterForm; }
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
            InitializeVuMeter();
            InitializeSettings();
            mProjectPanel.TransportBar.StateChanged +=
                new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
            mProjectPanel.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
            if (mSettings.OpenLastProject && mSettings.LastOpenProject != "")
            {
                // open the last open project
                DoOpenProject(mSettings.LastOpenProject);
            }
            else
            {
                // no project opened, same as if we closed a project.
                StatusUpdateClosedProject();
            }
        }

        /// <summary>
        /// Set up the VU meter form.
        /// </summary>
        private void InitializeVuMeter()
        {
            Audio.VuMeter vumeter = new Obi.Audio.VuMeter();
            Audio.AudioPlayer.Instance.VuMeter = vumeter;
            Audio.AudioRecorder.Instance.VuMeterObject = vumeter;
            vumeter.SetEventHandlers();
            mVuMeterForm = new Audio.VuMeterForm(vumeter);
            mVuMeterForm.MagnificationFactor = 1.5;
            // Kludgy
            mVuMeterForm.Show();
            mVuMeterForm.Visible = false;
        }

        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            Status(Localizer.Message(mProjectPanel.TransportBar.State.ToString()));
        }

        /// <summary>
        /// TODO.
        /// </summary>
        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status("PLAYBACK RATE CHANGED");
        }

        #region File menu event handlers

        private void mFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForFileMenu();
        }

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Stop();
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Whether or not the project was created, the setting for
                // automatically creating a title section is saved.
                mSettings.CreateTitleSection = dialog.CreateTitleSection;
                if (ClosedProject())
                {
                    CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection);
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
            mProjectPanel.TransportBar.Stop();
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
        /// <remarks>In the future, do not clear the command manager (only after cleanup.)</remarks>
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProjectPanel.TransportBar.Stop();
                mProject.Save();
                mCommandManager.Clear();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Stop();
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
            if (mProject.Unsaved)
            {
                mProjectPanel.TransportBar.Stop();
                // Ask for confirmation (yes/no question)
                if (MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    DoOpenProject(mProject.XUKPath);
                }
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
        /// Export the project to DAISY 3.
        /// </summary>
        private void mExportAsDAISYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Stop();
                if (mProject.Unsaved)
                {
                    DialogResult result = MessageBox.Show(Localizer.Message("export_unsaved_text"),
                        Localizer.Message("export_unsaved_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel) return;
                }
                this.Cursor = Cursors.WaitCursor;
                // string xuk = System.IO.Path.GetFileNameWithoutExtension(mProject.XUKPath);
                // string path = mSettings.DefaultExportPath + "\\" + xuk + Project.DaisyOutputDirSuffix;
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = Localizer.Message("export_choose_folder");
                dialog.SelectedPath = mSettings.DefaultExportPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mProject.ExportToZed(dialog.SelectedPath);
                }
                else
                {
                    Ready();
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


        #region Edit menu

        private void mEditToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForEditMenu();
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
            if (mCommandManager.HasRedo) mCommandManager.Redo();
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
                    StopIfPaused();
                    mProject.CutPhraseNode(mProjectPanel.StripManager.SelectedPhraseNode);
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    StopIfPaused();
                    mProject.ShallowCutSectionNode(mProjectPanel.StripManager.SelectedSectionNode, true);
                }
                else if (mProjectPanel.TOCPanel.IsNodeSelected)
                {
                    StopIfPaused();
                    mProject.CutSectionNode(mProjectPanel.TOCPanel.SelectedSection);
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
                    StopIfPaused();
                    mProject.CopyPhraseNode(mProjectPanel.StripManager.SelectedPhraseNode);
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    StopIfPaused();
                    mProject.ShallowCopySectionNode(mProjectPanel.StripManager.SelectedSectionNode, true);
                }
                else if (mProjectPanel.TOCPanel.IsNodeSelected)
                {
                    StopIfPaused();
                    mProject.CopySectionNode(mProjectPanel.TOCPanel.SelectedSection);
                }
            }
        }

        /// <summary>
        /// Paste what's in the clipboard and what is selected.
        /// </summary>
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                if (mProject.Clipboard.Section != null)
                {
                    mProject.PasteSectionNode(mProjectPanel.SelectedNode);
                }
                else if (mProject.Clipboard.Phrase != null)
                {
                    StopIfPaused();
                    mProject.PastePhraseNode(mProject.Clipboard.Phrase, mProjectPanel.SelectedNode);
                }
            }
        }

        /// <summary>
        /// Delete depens on what is selected.
        /// </summary>
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                if (mProjectPanel.StripManager.SelectedPhraseNode != null)
                {
                    StopIfPaused();
                    mProject.DeletePhraseNode(mProjectPanel.StripManager.SelectedPhraseNode);
                }
                else if (mProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    mProjectPanel.StripManager.DeleteSelectedSection();
                }
                else if (mProjectPanel.TOCPanel.IsNodeSelected)
                {
                    StopIfPaused();
                    mProject.DeleteSectionNode(mProjectPanel.TOCPanel.SelectedSection);
                }
            }

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
        /// Edit the metadata for the project.
        /// </summary>
        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
            if (mProject != null && dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.Modified();
            }
            Ready();
        }

        /// <summary>
        /// Edit the full DAISY metadata for this project.
        /// </summary>
        private void mFullMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.FullMetadata dialog = new Dialogs.FullMetadata(new Metadata(mProject, Metadata.DaisyTemplates()));
            if (mProject != null && dialog.ShowDialog() == DialogResult.OK)
            {
                mProject.Modified();
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
                mProjectPanel.SynchronizeWithCoreTree();
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
        private void mAudioPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
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
            if (Directory.Exists(dialog.DefaultXUKDirectory)) mSettings.DefaultPath = dialog.DefaultXUKDirectory;
            if (Directory.Exists(dialog.DefaultDAISYDirectory)) mSettings.DefaultExportPath = dialog.DefaultDAISYDirectory;
            mSettings.OpenLastProject = dialog.OpenLastProject;
            mSettings.LastOutputDevice = dialog.OutputDevice.Name;
            Audio.AudioPlayer.Instance.SetDevice(this, dialog.OutputDevice);
            mSettings.LastInputDevice = dialog.InputDevice.Name;
            Audio.AudioRecorder.Instance.InputDevice = dialog.InputDevice;
            mSettings.AudioChannels = dialog.AudioChannels;
            mSettings.SampleRate = dialog.SampleRate;
            mSettings.BitDepth = dialog.BitDepth;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        /// <remarks>Warn when closing while playing?</remarks>
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

        /// <summary>
        /// Handle state change events from the project (closed, modified, opened, saved.)
        /// </summary>
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
                    mProjectPanel.SynchronizeWithCoreTree();
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
            UpdateEnabledItemsForUndoRedo();
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
        /// Update the form (title and status bar) when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Metadata.Title);
            Status(String.Format(Localizer.Message("opened_project"), mProject.XUKPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Metadata.Title);
            Status(String.Format(Localizer.Message("saved_project"), mProject.LastPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            mProjectPanel.TransportBar.Stop();
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Metadata.Title + "*");
            Ready();
        }

        private void dumpTreeDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.RootNode.acceptDepthFirst(new Visitors.DumpTree());
            mProject.DumpAssManager();
        }




        




        #region TOC menu

        private void mTocToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTOCMenu();
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForTOCMenu()
        {
            mShowhideTableOfContentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            mShowhideTableOfContentsToolStripMenuItem.Enabled = mProject != null;

            bool isNodeSelected = mProject != null && mProjectPanel.TOCPanelVisible && mProjectPanel.TOCPanel.IsNodeSelected;
            SectionNode selected = isNodeSelected ? mProjectPanel.TOCPanel.SelectedSection : null;
            bool isParentUsed = isNodeSelected ? selected.ParentSection == null || selected.ParentSection.Used : false;
            bool isNodeUsed = isNodeSelected && selected.Used;
            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;

            mAddSectionToolStripMenuItem.Enabled = !isPlaying && (isNodeUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlaying && isNodeUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlaying && isNodeUsed;
            mMoveInToolStripMenuItem.Enabled = !isPlaying && isNodeUsed && mProjectPanel.Project.CanMoveSectionNodeIn(selected);
            mMoveOutToolStripMenuItem.Enabled = !isPlaying && isNodeUsed && mProjectPanel.Project.CanMoveSectionNodeOut(selected);

            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlaying && isNodeSelected && isParentUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"),
                Localizer.Message(!isNodeSelected || isNodeUsed ? "unused" : "used"));
            mShowInStripviewToolStripMenuItem.Enabled = isNodeSelected;
        }

        #endregion

        #region Strips menu

        private void mStripsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForStripsMenu();
        }

        /// <summary>
        /// Update the enabled items of the Strips menu.
        /// </summary>
        private void UpdateEnabledItemsForStripsMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isStripSelected = isProjectOpen && mProjectPanel.StripManager.SelectedSectionNode != null;
            bool isAudioBlockSelected = isProjectOpen && mProjectPanel.StripManager.SelectedPhraseNode != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index ==
                mProjectPanel.StripManager.SelectedPhraseNode.ParentSection.PhraseChildCount - 1;
            bool isAudioBlockFirst = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index == 0;
            bool isBlockClipBoardSet = isProjectOpen && mProject.Clipboard.Phrase != null;
            bool canSetPage = isAudioBlockSelected;  // an audio block must be selected and a heading must not be set.
            bool canRemovePage = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.getProperty(typeof(PageProperty)) != null;

            bool canInsertPhrase = isProjectOpen && mProjectPanel.StripManager.CanInsertPhraseNode;
            mInsertEmptyAudioblockToolStripMenuItem.Enabled = canInsertPhrase;
            mImportAudioFileToolStripMenuItem.Enabled = canInsertPhrase;

            mAddStripToolStripMenuItem.Enabled = isProjectOpen;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;

            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
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

            mMarkStripAsUnusedToolStripMenuItem.Enabled = mProjectPanel.CanToggleStrip;
            mMarkStripAsUnusedToolStripMenuItem.Text = mProjectPanel.ToggleStripString;
            mMarkAudioBlockAsUnusedToolStripMenuItem.Enabled = mProjectPanel.CanToggleAudioBlock;
            mMarkAudioBlockAsUnusedToolStripMenuItem.Text = mProjectPanel.ToggleAudioBlockString;
        }


        #endregion


        /// <summary>
        /// Update the visibility and actual label of transport items.
        /// </summary>
        private void mTransportToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTransportMenu();
        }

        private void UpdateEnabledItemsForTransportMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isNodeSelected = isProjectOpen && mProjectPanel.SelectedNode != null;
            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message(mVuMeterForm.Visible ? "hide_vu_meter" : "show_vu_meter");
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
            mRecordToolStripMenuItem.Enabled = mProjectPanel.TransportBar.CanRecord;
        }

        /// <summary>
        /// Tools item are always enabled (except for the debug stuff.)
        /// </summary>
        private void mToolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForToolsMenu();
        }

        private void UpdateEnabledItemsForToolsMenu()
        {
            mDumpTreeDEBUGToolStripMenuItem.Enabled = mProject != null;
            mExportAssetDEBUGToolStripMenuItem.Enabled = mProjectPanel.StripManager.SelectedPhraseNode != null;
        }

        internal void UndoLast()
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                UpdateEnabledItemsForUndoRedo();
            }
        }

        // Transport bar stuff

        #region transport bar

        /// <summary>
        /// Show the VU meter form (creating it if necessary) or hide it.
        /// </summary>
        private void mShowHideVUMeterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mVuMeterForm != null && mVuMeterForm.Visible)
            {
                mVuMeterForm.Hide();
            }
            else
            {
                mVuMeterForm.Show();
            }
        }

        /// <summary>
        /// Play the whole book from the selected node, or from the beginning.
        /// If already playing, pause.
        /// </summary>
        private void mPlayAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TransportBar_PlayAll();
        }

        private void TransportBar_PlayAll ()
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
            TransportBar_PlaySelection();
        }

        private void TransportBar_PlaySelection ()
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
            TransportBar_Stop();
        }


        private void TransportBar_Stop ()
    {
            mProjectPanel.TransportBar.Stop();
        }

        /// <summary>
        /// Record new assets.
        /// </summary>
        private void mRecordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Record();
        }

        #endregion

        private void mRewindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Rewind();
        }

        private void mFastForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.FastForward();
        }

        private void mPreviousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_PreviousPhrase();
        }

        private void TransportBar_PreviousPhrase()
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
            TransportBar_NextPhrase();
        }

        private void TransportBar_NextPhrase()
        {
            if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.NextPhrase();
            }
        }

        /// <summary>
        /// Test the export asset function.
        /// </summary>
        private void mExportAssetDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.StripManager.SelectedPhraseNode != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = Localizer.Message("audio_file_filter");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mProjectPanel.StripManager.SelectedPhraseNode.Asset.Export(dialog.FileName);
                }
                else
                {
                    Ready();
                }

            }
        }

        /// <summary>
        /// Keys for the whole application.
        /// </summary>
        protected override bool ProcessDialogKey(Keys key)
        {
            // Make sure that the correct menu items are enabled for the keyboard shortcuts to work.
            UpdateEnabledItems();
            if (mProject != null) mProjectPanel.StripManager.UpdateEnabledItemsForContextMenu();
            switch (key)
            {
                case Keys.Control | Keys.Space:
                    if (mPlayAllToolStripMenuItem.Enabled)
                        TransportBar_PlayAll();
                    break;

                case Keys.Control | Keys.Shift | Keys.Space:
                    if (mPlaySelectionToolStripMenuItem.Enabled)
                        TransportBar_PlaySelection();
                    break;

// shorcut local to stripmanager
                case Keys.Space:
                    if (mProjectPanel.StripManager.ContainsFocus)
                    {
                        if (mPlaySelectionToolStripMenuItem.Enabled)
                            TransportBar_PlaySelection();
                    }
                    break;

                // shorcut local to stripmanager
                case Keys.Escape:
                    if (mProjectPanel.StripManager.ContainsFocus)
                    {
                        if (mStopToolStripMenuItem.Enabled)
                            TransportBar_Stop();
                    }
                    break;

                case Keys.Control | Keys.T:
                    if (mStopToolStripMenuItem.Enabled)
                        TransportBar_Stop();
                    break;

                case Keys.Control | Keys.R:
                    if (mRecordToolStripMenuItem.Enabled)
                        mProjectPanel.TransportBar.Record();
                    break;

                case Keys.Alt | Keys.Left:
                    if (mPreviousPhraseToolStripMenuItem.Enabled)
                        TransportBar_PreviousPhrase();
                    break;

                case Keys.Alt | Keys.Right:
                    if (mNextPhraseToolStripMenuItem.Enabled)
                        TransportBar_NextPhrase();
                    break;

                case Keys.Alt | Keys.Down:
                    if (mNextSectionToolStripMenuItem.Enabled)
                        mProjectPanel.TransportBar.NextSection();
                    break;

                case Keys.Alt | Keys.Up:
                    if (mPreviousSectionToolStripMenuItem.Enabled)
                        mProjectPanel.TransportBar.PrevSection();
                    break;


                case Keys.Alt | Keys.Shift | Keys.Right:
                    if (mFastForwardToolStripMenuItem.Enabled == true)
                        mProjectPanel.TransportBar.FastForward();
                    break;
                case Keys.Alt | Keys.Shift | Keys.Left:
                    if (mRewindToolStripMenuItem.Enabled == true)
                        mProjectPanel.TransportBar.Rewind();
                    break;

                case Keys.Control | Keys.Shift | Keys.T :
                    mProjectPanel.TransportBar.FocusTimeDisplay();
                    break;

            }
            return base.ProcessDialogKey(key);
        }

        /// <summary>
        /// Toggle section used/unsed.
        /// </summary>
        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.ToggleSelectedSectionUsed();
        }

        /// <summary>
        /// Toggle strip used/unused.
        /// </summary>
        private void mMarkStripAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedStripUsed();
        }

        /// <summary>
        /// Toggle audio block used/unused.
        /// </summary>
        private void mMarkAudioBlockAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedPhraseUsed();
        }

        private void mInsertEmptyAudioblockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.InsertEmptyAudioBlock();
        }


        // Various utility functions

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// Update the recent menu if necessary.
        /// </summary>
        /// <param name="path">The path of the project to add.</param>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Contains(path))
            {
                // the item was in the list so bump it up
                int i = mSettings.RecentProjects.IndexOf(path);
                mSettings.RecentProjects.RemoveAt(i);
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
            }
            if (AddRecentProjectsItem(path)) mSettings.RecentProjects.Insert(0, path);
        }

        /// <summary>
        /// Add an item in the recent projects list, if the file actually exists.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        /// <remarks>The file was in the preferences but may have disappeared since.</remarks>
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
        /// Stop playback, then
        /// check whether a project is currently open and not saved; prompt the user about what to do.
        /// Close the project if that is what the user wants to do or if it was unmodified.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            mProjectPanel.TransportBar.Stop();
            if (mProject != null && mProject.Unsaved)
            {
                // Unsaved project: ask the user if they want to save and close ("yes" option),
                // close without saving ("no" option) or not close at all ("cancel" option.)
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.Save();
                        DoCloseProject();
                        return true;
                    case DialogResult.No:
                        DoCloseProject();
                        return true;
                    // case DialogResult.Cancel:
                    default:
                        return false;
                }
            }
            else
            {
                // No project, or no changes, so just close.
                if (mProject != null) DoCloseProject();
                return true;
            }
        }

        /// <summary>
        /// Create a new project. The application listens to the project's state change and
        /// command created events.
        /// </summary>
        /// <param name="path">Path of the XUK file to the project.</param>
        /// <param name="title">Title of the project.</param>
        /// <param name="createTitleSection">If true, a title section is automatically created.</param>
        private void CreateNewProject(string path, string title, bool createTitleSection)
        {
            mProject = Project.BlankProject();
            mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
            mProject.Create(path, title, mSettings.IdTemplate, mSettings.UserProfile, createTitleSection);
            AddRecentProject(mProject.XUKPath);
        }

        /// <summary>
        /// Clean up and close the project.
        /// Cleaning up only occurs when the project is closed.
        /// </summary>
        private void DoCloseProject()
        {
            if (mProject.Unsaved)
            {
                // A bit kldugy but an easy way to rebuild the list of used files when discarding changes.
                string path = mProject.XUKPath;
                mProject = Project.BlankProject();
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(Program.Noop);
                mProject.Open(path);
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            }
            mProject.DeleteUnusedFiles(ReportDeleteError);
            mProject.Close();
        }

        /// <summary>
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        /// <remarks>TODO: have a progress bar, and hide the panel while opening.</remarks>
        private void DoOpenProject(string path)
        {
            try
            {
                mProject = Project.BlankProject();  // new Project();
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
                this.Cursor = Cursors.WaitCursor;
                mProject.Open(path);
                AddRecentProject(path);
                mSettings.LastOpenProject = path;
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
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            Status(Localizer.Message("ready"));
        }

        /// <summary>
        /// Display a message on the status bar.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void Status(string message)
        {
            mToolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Stop the player if it was paused before an edit operation can be made.
        /// </summary>
        private void StopIfPaused()
        {
            if (mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Paused)
                mProjectPanel.TransportBar.Stop();
        }

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
        /// Update the enabled items for all menus.
        /// </summary>
        /// <remarks>This is necessary to make sure that keyboard shortcuts work correctly.</remarks>
        private void UpdateEnabledItems()
        {
            UpdateEnabledItemsForFileMenu();
            UpdateEnabledItemsForEditMenu();
            UpdateEnabledItemsForTOCMenu();
            UpdateEnabledItemsForStripsMenu();
            UpdateEnabledItemsForTransportMenu();
            UpdateEnabledItemsForToolsMenu();
        }

        /// <summary>
        /// Update the enabled items of the File menu.
        /// </summary>
        private void UpdateEnabledItemsForFileMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isProjectModified = isProjectOpen && mProject.Unsaved;
            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;

            mNewProjectToolStripMenuItem.Enabled = !isPlaying;
            mOpenProjectToolStripMenuItem.Enabled = !isPlaying;
            mOpenRecentProjectToolStripMenuItem.Enabled = !isPlaying && mSettings.RecentProjects.Count > 0;
            mClearListToolStripMenuItem.Enabled = !isPlaying;
            mSaveProjectToolStripMenuItem.Enabled = !isPlaying && isProjectModified;
            mSaveProjectAsToolStripMenuItem.Enabled = !isPlaying && isProjectOpen;
            mDiscardChangesToolStripMenuItem.Enabled = !isPlaying && isProjectModified;
            mCloseProjectToolStripMenuItem.Enabled = isProjectOpen && !isPlaying;
            mExportAsDAISYToolStripMenuItem.Enabled = isProjectOpen && !isPlaying;
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForEditMenu()
        {
            UpdateEnabledItemsForUndoRedo();

            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;
            bool canCutCopyDelete = !isPlaying && mProjectPanel.CanCutCopyDeleteNode;
            string itemLabel = mProjectPanel.SelectedLabel;
            if (itemLabel != "") itemLabel = " " + itemLabel;
            ObiNode clipboardData = mProject == null ? null : mProject.Clipboard.Data as ObiNode;
            string pasteLabel = mProjectPanel.PasteLabel(clipboardData);
            if (pasteLabel != "") pasteLabel = " " + pasteLabel;

            mCutToolStripMenuItem.Enabled = canCutCopyDelete;
            mCutToolStripMenuItem.Text = String.Format(Localizer.Message("cut_menu_label"), itemLabel);
            mCopyToolStripMenuItem.Enabled = canCutCopyDelete;
            mCopyToolStripMenuItem.Text = String.Format(Localizer.Message("copy_menu_label"), itemLabel);
            mPasteToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.CanPaste(clipboardData);
            mPasteToolStripMenuItem.Text = String.Format(Localizer.Message("paste_menu_label"), pasteLabel);
            mDeleteToolStripMenuItem.Enabled = canCutCopyDelete;
            mDeleteToolStripMenuItem.Text = String.Format(Localizer.Message("delete_menu_label"), itemLabel);

            bool isProjectOpen = mProject != null;
            bool canTouch = !isPlaying && isProjectOpen;
            mMetadataToolStripMenuItem.Enabled = canTouch;
            mFullMetadataToolStripMenuItem.Enabled = canTouch;
            mTouchProjectToolStripMenuItem.Enabled = canTouch;
        }

        /// <summary>
        /// Update the label for undo and redo (and their availability) depending on what is in the command manager.
        /// </summary>
        private void UpdateEnabledItemsForUndoRedo()
        {
            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;
            if (mCommandManager.HasUndo)
            {
                mUndoToolStripMenuItem.Enabled = !isPlaying;
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
                mRedoToolStripMenuItem.Enabled = !isPlaying;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"),
                    mCommandManager.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = Localizer.Message("redo");
            }
        }

        private void mQuickSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.QuickSplit();
        }
    }
}
