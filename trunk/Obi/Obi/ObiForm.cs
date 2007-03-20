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
    /// The main for of the application.
    /// The form consists mostly of a menu bar and a project panel.
    /// We also keep an undo stack (the command manager) and settings.
    /// </summary>
    public partial class ObiForm : Form, IMessageFilter
    {
        private Project mProject;                // the project currently being authored
        private Settings mSettings;              // application settings
        private CommandManager mCommandManager;  // the undo stack for this project
        private Audio.VuMeterForm mVuMeterForm;  // keep track of a single VU meter form

        public bool AllowDelete
        {
            set { mDeleteToolStripMenuItem.Enabled = value; }
        }

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
        /// Initialize a new form.
        /// </summary>
        public ObiForm()
        {
            InitializeObi();
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
        /// Initialize a new form with a project given as parameter.
        /// </summary>
        /// <param name="path">The project to open on startup.</param>
        public ObiForm(string path)
        {
            InitializeObi();
            DoOpenProject(path);
        }

        private void InitializeObi()
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
            Status(Localizer.Message(mProjectPanel.TransportBar._CurrentPlaylist.State.ToString()));
        }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectPanel.TransportBar._CurrentPlaylist.PlaybackRate));
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
            mProjectPanel.TransportBar.Enabled = false;
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
            mProjectPanel.TransportBar.Enabled = true;
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
                mProjectPanel.TransportBar.Enabled = false;
                if (mProject.Unsaved)
                {
                    DialogResult result = MessageBox.Show(Localizer.Message("export_unsaved_text"),
                        Localizer.Message("export_unsaved_caption"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Cancel) return;
                }
                this.Cursor = Cursors.WaitCursor;
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
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


        #region Edit menu event handlers

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
                mProjectPanel.TransportBar.Enabled = false;
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    mProject.CutPhraseNode(mProjectPanel.CurrentSelectedAudioBlock);
                    mProjectPanel.CurrentSelection = null;
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    mProject.ShallowCutSectionNode(mProjectPanel.CurrentSelectedStrip);
                    mProjectPanel.CurrentSelection = null;
                }
                else if (mProjectPanel.CurrentSelectedSection != null)
                {
                    mProject.CutSectionNode(mProjectPanel.CurrentSelectedSection);
                    mProjectPanel.CurrentSelection = null;
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Copy depends on what is selected.
        /// </summary>
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    mProject.CopyPhraseNode(mProjectPanel.CurrentSelectedAudioBlock);
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    mProject.ShallowCopySectionNode(mProjectPanel.CurrentSelectedStrip, true);
                }
                else if (mProjectPanel.CurrentSelectedSection != null)
                {
                    mProject.CopySectionNode(mProjectPanel.CurrentSelectedSection);
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Paste what's in the clipboard and what is selected.
        /// </summary>
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (mProject.Clipboard.Section != null)
                {
                    IControlWithSelection control = mProjectPanel.CurrentSelection.Control;
                    SectionNode pasted = mProject.PasteSectionNode(mProjectPanel.CurrentSelectionNode);
                    mProjectPanel.CurrentSelection = new NodeSelection(pasted, control);
                }
                else if (mProject.Clipboard.Phrase != null)
                {
                    PhraseNode pasted = mProject.PastePhraseNode(mProject.Clipboard.Phrase,
                        mProjectPanel.CurrentSelectionNode);
                    mProjectPanel.CurrentSelection = new NodeSelection(pasted, mProjectPanel.StripManager);
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Delete depens on what is selected.
        /// </summary>
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    mProject.DeletePhraseNode(mProjectPanel.CurrentSelectedAudioBlock);
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    // to review!
                    mProject.ShallowDeleteSectionNode(this, mProjectPanel.CurrentSelectedStrip);
                }
                else if (mProjectPanel.CurrentSelectedSection != null)
                {
                    mProject.DeleteSectionNode(mProjectPanel.CurrentSelectedSection);
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void mMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
                if (mProject != null && dialog.ShowDialog() == DialogResult.OK) mProject.Modified();
                Ready();
                mProjectPanel.TransportBar.Enabled = true;
            }
        }


        /// <summary>
        /// Touch the project so that it seems that it was modified.
        /// Also refresh the display.
        /// </summary>
        private void mTouchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectPanel.TransportBar.Enabled = false;
                if (!mCommandManager.HasUndo) mProject.Touch();
                mProjectPanel.SynchronizeWithCoreTree();
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        #endregion


        #region TOC menu event handlers

        private void mTocToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForTOCMenu();
        }

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

        private void mInsertSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.InsertSection();
        }

        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.AddSubSection();
        }

        private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.StartRenamingSelectedSection();
        }

        private void mMoveOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.MoveSectionNodeOut(mProjectPanel.CurrentSelectedSection);
        }

        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.MoveSectionNodeIn(mProjectPanel.CurrentSelectedSection);
        }

        private void mShowInStripviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TOCPanel.ShowSelectedSectionInStripView();
        }

        #endregion


        #region Strips menu event handlers

        private void mStripsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForStripsMenu();
        }

        private void mInsertStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.InsertStrip();
        }

        private void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.StartRenamingSelectedStrip();
        }

        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ImportPhrases();
        }

        private void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.SplitBlock();
        }

        private void mQuickSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.QuickSplitBlock();
        }

        private void mApplyPhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ApplyPhraseDetection();
        }

        private void mMergeWithPreviousAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MergeBlocks();
        }

        private void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MoveBlock(PhraseNode.Direction.Forward);
        }

        private void mMoveAudioBlockBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.MoveBlock(PhraseNode.Direction.Backward);
        }

        private void mMarkAudioBlockAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedAudioBlockUsed();
        }

        private void mEditAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.EditAnnotationForSelectedAudioBlock();
        }

        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.RemoveAnnotationForAudioBlock();
        }

        private void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.SetPageNumber();
        }

        private void mRemovePageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.RemovePageNumber();
        }

        private void mFocusOnAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.FocusOnAnnotation();
        }

        private void mGoToPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.GoToPage();
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
                mProjectPanel.TransportBar.Stop();
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
            mProjectPanel.TransportBar.Enabled = false;
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Metadata.Title + "*");
            Ready();
            mProjectPanel.TransportBar.Enabled = true;
        }

        private void dumpTreeDEBUGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.RootNode.acceptDepthFirst(new Visitors.DumpTree());
            mProject.DumpAssManager();
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
            bool isProjectOpen = mProject != null;
            bool isNodeSelected = isProjectOpen && mProjectPanel.CurrentSelection != null;

            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message(mVuMeterForm.Visible ? "hide_vu_meter" : "show_vu_meter");
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
                mPlayAllToolStripMenuItem.Enabled = isProjectOpen;
                //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = isNodeSelected;
                //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = isNodeSelected;
            }
            else if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.NotReady)
            {
                mPlayAllToolStripMenuItem.Enabled = false;
                //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = false;
                //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = false;
            }
            else if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mPlayAllToolStripMenuItem.Enabled = mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                //mPlayAllToolStripMenuItem.Text = Localizer.Message("play_all");
                mPlaySelectionToolStripMenuItem.Enabled = !mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("play");
                mStopToolStripMenuItem.Enabled = true;
            }
            else // playing
            {
                mPlayAllToolStripMenuItem.Enabled = mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                //mPlayAllToolStripMenuItem.Text = Localizer.Message("pause_all");
                mPlaySelectionToolStripMenuItem.Enabled = !mProjectPanel.TransportBar._CurrentPlaylist.WholeBook;
                //mPlaySelectionToolStripMenuItem.Text = Localizer.Message("pause");
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
            mProjectPanel.TransportBar.Play();
        }

        /// <summary>
        /// Play the current selection (phrase or section.)
        /// </summary>
        private void mPlaySelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Play(mProjectPanel.CurrentSelectionNode);
        }

        /// <summary>
        /// Play a single phrase node using the transport bar.
        /// </summary>
        private void Play(ObiNode node)
        {
            mProjectPanel.TransportBar.Play(node);
        }

        private void mPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.Pause();
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

        private void mPreviousSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.PrevSection();
        }

        private void mPreviousPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TransportBar_PreviousPhrase();
        }

        private void TransportBar_PreviousPhrase()
        {
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
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
            if (mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
            {
            }
            else
            {
                mProjectPanel.TransportBar.NextPhrase();
            }
        }

        private void mNextSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.TransportBar.NextSection();
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
        /// Toggle section used/unsed.
        /// </summary>
        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.ToggleNodeUsedWithCommand(mProjectPanel.CurrentSelectedSection, true);
        }

        /// <summary>
        /// Toggle strip used/unused.
        /// </summary>
        private void mMarkStripAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.ToggleSelectedStripUsed();
        }

        private void mInsertEmptyAudioblockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.StripManager.InsertEmptyAudioBlock();
        }


        // Various utility functions

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
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
        /// The path relative to the project directory is shown.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        /// <remarks>The file was in the preferences but may have disappeared since.</remarks>
        private bool AddRecentProjectsItem(string path)
        {
            if (File.Exists(path))
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetDirectoryName(path) == mSettings.DefaultPath ? Path.GetFileName(path) : path;
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
            if (mProject != null && mProject.Unsaved)
            {
                mProjectPanel.TransportBar.Enabled = false;
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
                        mProjectPanel.TransportBar.Enabled = true;
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
                // if opening failed, no project is open and we don't try to open it again next time.
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mProject = null;
                mSettings.LastOpenProject = "";
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
            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;

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

            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
            bool canCutCopyDelete = !isPlaying && mProjectPanel.CurrentSelectionNode != null;
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
            mTouchProjectToolStripMenuItem.Enabled = canTouch;
        }

        /// <summary>
        /// Update the label for undo and redo (and their availability) depending on what is in the command manager.
        /// </summary>
        private void UpdateEnabledItemsForUndoRedo()
        {
            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
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
            System.Diagnostics.Debug.Print("~~~ can{0} undo ~~~", mUndoToolStripMenuItem.Enabled ? "" : "NOT");
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForTOCMenu()
        {
            mShowhideTableOfContentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            mShowhideTableOfContentsToolStripMenuItem.Enabled = mProject != null;

            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
            bool isProjectOpen = mProject != null;
            bool noNodeSelected = isProjectOpen && mProjectPanel.CurrentSelection == null;
            bool isSectionNodeSelected = isProjectOpen && mProjectPanel.CurrentSelectedSection != null;
            bool isSectionNodeUsed = isSectionNodeSelected && mProjectPanel.CurrentSelectedSection.Used;
            bool isParentUsed = isSectionNodeSelected ?
                mProjectPanel.CurrentSelectedSection.ParentSection == null ||
                mProjectPanel.CurrentSelectedSection.ParentSection.Used : false;

            mAddSectionToolStripMenuItem.Enabled = !isPlaying && (noNodeSelected || isSectionNodeUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlaying && isSectionNodeUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlaying && isSectionNodeUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlaying && isSectionNodeUsed &&
                mProjectPanel.Project.CanMoveSectionNodeOut(mProjectPanel.CurrentSelectionNode as SectionNode);
            mMoveInToolStripMenuItem.Enabled = !isPlaying && isSectionNodeUsed &&
                mProjectPanel.Project.CanMoveSectionNodeIn(mProjectPanel.CurrentSelectionNode as SectionNode);
            
            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlaying && isSectionNodeSelected && isParentUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"),
                Localizer.Message(!isSectionNodeSelected || isSectionNodeUsed ? "unused" : "used"));
            mShowInStripviewToolStripMenuItem.Enabled = isSectionNodeSelected;
        }

        private void UpdateEnabledItemsForStripsMenu()
        {
            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
            bool isProjectOpen = mProject != null;
            bool isStripSelected = isProjectOpen && mProjectPanel.StripManager.SelectedSectionNode != null;
            bool isAudioBlockSelected = isProjectOpen && mProjectPanel.StripManager.SelectedPhraseNode != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index ==
                mProjectPanel.StripManager.SelectedPhraseNode.ParentSection.PhraseChildCount - 1;
            bool isAudioBlockFirst = isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.Index == 0;
            bool isBlockClipBoardSet = isProjectOpen && mProject.Clipboard.Phrase != null;
            bool canMerge = isProjectOpen && mProjectPanel.StripManager.CanMerge;

            bool canInsertPhrase = !isPlaying && isProjectOpen && mProjectPanel.StripManager.CanInsertPhraseNode;
            mImportAudioFileToolStripMenuItem.Enabled = canInsertPhrase;

            mInsertStripToolStripMenuItem.Enabled = isProjectOpen;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;

            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mApplyPhraseDetectionToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithPreviousAudioBlockToolStripMenuItem.Enabled = !isPlaying && canMerge;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            bool canRemoveAnnotation = !isPlaying && isAudioBlockSelected &&
                mProjectPanel.StripManager.SelectedPhraseNode.HasAnnotation;
            mEditAnnotationToolStripMenuItem.Enabled = !isPlaying && isAudioBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;
            mFocusOnAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;

            mSetPageNumberToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.StripManager.CanSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.StripManager.CanRemovePage;
            mGoToPageToolStripMenuItem.Enabled = !isPlaying && isProjectOpen && mProject.Pages > 0; 

            mShowInTOCViewToolStripMenuItem.Enabled = isStripSelected;

            mMarkAudioBlockAsUnusedToolStripMenuItem.Enabled = mProjectPanel.CanToggleAudioBlock;
            mMarkAudioBlockAsUnusedToolStripMenuItem.Text = mProjectPanel.ToggleAudioBlockString;
        }

        private void mViewHelpInExternalBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uri url = new Uri(Path.GetDirectoryName(GetType().Assembly.Location) + "\\help_en.html");
            System.Diagnostics.Process.Start(url.ToString());
        }

        private void mShowInTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // should be a project panel method?
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                mProjectPanel.CurrentSelection = new NodeSelection(mProjectPanel.CurrentSelection.Node, mProjectPanel.TOCPanel);
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                mProjectPanel.TOCPanel.Focus();
            }
        }

        #region IMessageFilter Members

        private const UInt32 WM_KEYDOWN = 0x0100;
        private const UInt32 WM_SYSKEYDOWN = 0x0104;

        private void mProjectPanel_Load(object sender, EventArgs e)
        {
            Application.AddMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)
            {
                System.Diagnostics.Debug.Print("*** Got WM_{0}KEYDOWN message ***", m.Msg == WM_SYSKEYDOWN ? "SYS" : "");
                UpdateEnabledItems();
            }
            return false;
        }

        #endregion
    }
}
