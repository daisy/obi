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
        private Audio.VuMeter m_Vumeter ; // VuMeterForm is to be initialised again and again so this instance is required as member

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
            try
            {
                InitializeComponent();
                mProject = null;
                mSettings = null;
                mCommandManager = new CommandManager();
                InitializeVuMeter();
                InitializeSettings();
                InitialiseHighContrastSettings();
                mProjectView.TransportBar.StateChanged +=
                    new Obi.Events.Audio.Player.StateChangedHandler(TransportBar_StateChanged);
                mProjectView.TransportBar.PlaybackRateChanged += new EventHandler(TransportBar_PlaybackRateChanged);
                StatusUpdateClosedProject();
            }
            catch (Exception eAnyStartupException)
            {
                System.IO.StreamWriter tmpErrorLogStream = System.IO.File.CreateText(Application.StartupPath + Path.DirectorySeparatorChar + "ObiStartupError.txt");
                tmpErrorLogStream.WriteLine(eAnyStartupException.ToString());
                tmpErrorLogStream.Close();
                System.Windows.Forms.MessageBox.Show("An error occured while initializing Obi.\nPlease Submit a bug report, including the contents of " + Application.StartupPath + Path.DirectorySeparatorChar + "ObiStartupError.txt\nError text:\n" + eAnyStartupException.ToString(), "Obi initialization error");
            }
        }
        
        /// <summary>
        /// Set up the VU meter form.
        /// </summary>
        private void InitializeVuMeter()
        {
            m_Vumeter = mProjectView.TransportBar.VuMeter;
        }

        // setup a VuMeter form and show it
        private void ShowVuMeterForm ()
        {
            mVuMeterForm = new Audio.VuMeterForm(m_Vumeter );
            mVuMeterForm.MagnificationFactor = 1.5;
            // Kludgy
            mVuMeterForm.Show();
            //mVuMeterForm.Visible = false;
        }

        /// <summary>
        /// Show the state of the transport bar in the status bar.
        /// </summary>
        void TransportBar_StateChanged(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e)
        {
            Status(Localizer.Message(mProjectView.TransportBar._CurrentPlaylist.State.ToString()));
        }

        void TransportBar_PlaybackRateChanged(object sender, EventArgs e)
        {
            Status(String.Format(Localizer.Message("playback_rate"), mProjectView.TransportBar._CurrentPlaylist.PlaybackRate));
        }


        #region File menu event handlers

        private void mFileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            UpdateEnabledItemsForFileMenu();
        }

        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e) { NewProject(); }


        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DidCloseProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = Localizer.Message("obi_filter");
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
            mProjectView.TransportBar.Stop();
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
                mProjectView.TransportBar.Stop();
                mProject.Save();
                mCommandManager.Clear();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.TransportBar.Stop();
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
                mProjectView.TransportBar.Stop();
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
            if (DidCloseProject())
            {
                mProjectView.Selection = null;
                mProject = null;
                mCommandManager.Clear();
            }
        }
        /// <summary>
        /// Clean the assets of a project
        /// </summary>
        private void mCleanProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        }
        /// <summary>
        /// Export the project to DAISY 3.
        /// </summary>
        private void mExportAsDAISYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectView.TransportBar.Enabled = false;
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
                if (dialog.ShowDialog() == DialogResult.OK && IsExportDirectoryReady(dialog.SelectedPath))
                {
                    try
                    {
                        mProject.ExportToZed(dialog.SelectedPath);
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
                this.Cursor = Cursors.Default;
                mProjectView.TransportBar.Enabled = true;
            }
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

        private void mExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
                                    Close();
        }

        #endregion


        #region Edit menu event handlers

        private void mEditToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //UpdateEnabledItemsForEditMenu();
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
            if (mProjectView != null) mProjectView.Cut();
        }

        /// <summary>
        /// Copy depends on what is selected.
        /// </summary>
        private void mCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView != null) mProjectView.Copy();
        }

        /// <summary>
        /// Paste what's in the clipboard in/before what is selected.
        /// </summary>
        private void mPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null) mProjectView.Paste();
        }

        /// <summary>
        /// Delete depens on what is selected.
        /// </summary>
        private void mDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView != null) mProjectView.Delete();
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void mMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                mProjectView.TransportBar.Enabled = false;
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject);
                // TODO replace this: if (mProject != null && dialog.ShowDialog() == DialogResult.OK) mProject.Modified();
                Ready();
                mProjectView.TransportBar.Enabled = true;
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
                mProjectView.TransportBar.Enabled = false;
                if (!mCommandManager.HasUndo) mProject.Touch();
                mProjectView.SynchronizeWithCoreTree();
                mProjectView.TransportBar.Enabled = true;
            }
        }

        #endregion


        #region TOC menu event handlers


        #endregion


        #region Strips menu event handlers

        private void mImportAudioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ImportPhrases();
        }

        private void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SplitBlock();
        }

        private void mQuickSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.QuickSplitBlock();
        }

        private void mApplyPhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ApplyPhraseDetection();
        }

        private void mMergeWithPreviousAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MergeBlocks();
        }

        private void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MoveBlock(PhraseNode.Direction.Forward);
        }

        private void mMoveAudioBlockBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MoveBlock(PhraseNode.Direction.Backward);
        }

        private void mMarkAudioBlockAsSectionHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.MarkSelectedAudioBlockAsHeading();
        }

        private void mEditAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.EditAnnotationForSelectedAudioBlock();
        }

        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.RemoveAnnotationForAudioBlock();
        }

        private void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.SetPageNumber();
        }

        private void mRemovePageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.RemovePageNumber();
        }

        private void mFocusOnAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.FocusOnAnnotation();
        }

        private void mGoToPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.GoToPage();
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
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mProject, mProjectView.TransportBar);
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
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings, mProject, mProjectView.TransportBar);
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
                mProjectView.SelectionChanged -= new EventHandler(mProjectView_SelectionChanged);
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
            this.Text = Localizer.Message("obi");
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                mToolStripStatusLabel.Text = String.Format(Localizer.Message("closed_project"), mProject.Title);
                mProjectView.Project = null;
                EnableItemsProjectClosed();
            }
        }

        private void EnableItemsProjectClosed()
        {
        }

        /// <summary>
        /// Update the form (title and status bar) when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("opened_project"), mProject.XUKPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title);
            Status(String.Format(Localizer.Message("saved_project"), mProject.LastPath));
        }

        /// <summary>
        /// Update the form (title and status bar) when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format(Localizer.Message("title_bar"), mProject.Title + "*");
            Ready();
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
            bool isNodeSelected = isProjectOpen && mProjectView.Selection != null;

            mShowHideVUMeterToolStripMenuItem.Text = Localizer.Message(( mVuMeterForm != null && mVuMeterForm.Visible) ? "hide_vu_meter" : "show_vu_meter");

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
            }
        }

        internal void UndoLast()
        {
            if (mCommandManager.HasUndo)
            {
                mCommandManager.Undo();
                //UpdateEnabledItemsForUndoRedo();
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

                //mVuMeterForm.Hide();
                mVuMeterForm.Close();
            }
            else
            {
                ShowVuMeterForm();
            }
        }

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

        private void TransportBar_Stop ()
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
            if (mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
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
            if (mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Stopped)
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

        /// <summary>
        /// Toggle strip used/unused.
        /// </summary>
        private void mMarkStripAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ToggleSelectedStripUsed();
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
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        /// <remarks>TODO: have a progress bar, and hide the panel while opening.</remarks>
        private void DoOpenProject(string path)
        {
            try
            {
                mProject = new Project(path);
                // TODO extract this (same as create new project)
                mProjectView.CommandExecuted += new UndoRedoEventHandler(mProjectView_CommandExecuted);
                mProjectView.CommandUnexecuted += new UndoRedoEventHandler(mProjectView_CommandUnexecuted);
                mProjectView.SelectionChanged += new EventHandler(mProjectView_SelectionChanged);
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
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
            if (DidCloseProject())
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
            //UpdateEnabledItemsForEditMenu();
            UpdateEnabledItemsForTOCMenu();
            UpdateEnabledItemsForStripsMenu();
            UpdateEnabledItemsForTransportMenu();
        }

        /// <summary>
        /// Update the enabled items of the File menu.
        /// </summary>
        private void UpdateEnabledItemsForFileMenu()
        {
            bool isProjectOpen = mProject != null;
            bool isProjectModified = isProjectOpen && mProject.Unsaved;
            bool isPlayingOrRecording = mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectView.TransportBar.IsInlineRecording;

            // mNewProjectToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mOpenProjectToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mOpenRecentProjectToolStripMenuItem.Enabled = !isPlayingOrRecording && mSettings.RecentProjects.Count > 0;
            mClearListToolStripMenuItem.Enabled = !isPlayingOrRecording;
            mSaveProjectToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectModified;
            mSaveProjectAsToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectOpen;
            mDiscardChangesToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectModified;
            mCloseProjectToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
            mExportAsDAISYToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
            mCleanProjectToolStripMenuItem.Enabled = isProjectOpen && !isPlayingOrRecording;
        }

        /// <summary>
        /// Update the enabled items of the Edit menu.
        /// </summary>
        private void UpdateEnabledItemsForTOCMenu()
        {
            bool isPlayingOrRecording = mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing ||mProjectView.TransportBar.IsInlineRecording;
            bool isProjectOpen = mProject != null;
            bool noNodeSelected = isProjectOpen && mProjectView.Selection == null;
            bool isSectionNodeSelected = isProjectOpen && mProjectView.SelectedSection != null;
            bool isSectionNodeUsed = isSectionNodeSelected && mProjectView.SelectedSection.Used;
            bool isParentUsed = isSectionNodeSelected ?
                mProjectView.SelectedSection.ParentSection == null ||
                mProjectView.SelectedSection.ParentSection.Used : false;

            mAddSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && (noNodeSelected || isSectionNodeUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed &&
                mProjectView.Project.CanMoveSectionNodeOut(mProjectView.SelectionNode as SectionNode);
            mMoveInToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeUsed &&
                mProjectView.Project.CanMoveSectionNodeIn(mProjectView.SelectionNode as SectionNode);
            
            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlayingOrRecording && isSectionNodeSelected && isParentUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"),
                Localizer.Message(!isSectionNodeSelected || isSectionNodeUsed ? "unused" : "used"));
        }

        private void UpdateEnabledItemsForStripsMenu()
        {
            bool isPlayingOrRecording = mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectView.TransportBar.IsInlineRecording;
            bool isPaused = mProjectView.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused;
            bool isProjectOpen = mProject != null;
            bool isStripSelected = isProjectOpen && mProjectView.SelectedStripNode != null;
            bool isAudioBlockSelected = isProjectOpen && mProjectView.SelectedBlockNode != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                mProjectView.SelectedBlockNode.Index ==
                mProjectView.SelectedBlockNode.ParentSection.PhraseChildCount - 1;
            bool isAudioBlockFirst = isAudioBlockSelected &&
                mProjectView.SelectedBlockNode.Index == 0;
            bool isBlockClipBoardSet = isProjectOpen && mProject.Clipboard.Phrase != null;
            bool canMerge = isProjectOpen && mProjectView.CanMergeBlocks;

            bool canInsertPhrase = !isPlayingOrRecording && isProjectOpen && mProjectView.CanInsertAudioBlock;
            mImportAudioFileToolStripMenuItem.Enabled = canInsertPhrase;

            mInsertStripToolStripMenuItem.Enabled = isProjectOpen;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;

            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (isPlayingOrRecording || isPaused);
            mApplyPhraseDetectionToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithPreviousAudioBlockToolStripMenuItem.Enabled = !isPlayingOrRecording && canMerge;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            bool canRemoveAnnotation = !isPlayingOrRecording && isAudioBlockSelected &&
                mProjectView.SelectedBlockNode.HasAnnotation;
            mEditAnnotationToolStripMenuItem.Enabled = !isPlayingOrRecording && isAudioBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;
            mFocusOnAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;

            mSetPageNumberToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectView.CanSetPageNumber;
            mRemovePageNumberToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectView.CanRemovePageNumber;
            mGoToPageToolStripMenuItem.Enabled = !isPlayingOrRecording && isProjectOpen && mProject.Pages > 0; 

            mMarkAudioBlockAsUnusedToolStripMenuItem.Enabled = mProjectView.CanToggleAudioBlockUsedFlag;
            mMarkAudioBlockAsUnusedToolStripMenuItem.Text = mProjectView.ToggleAudioBlockUsedFlagLabel;
            
            //mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled = isAudioBlockSelected &&
            //    !mProjectView.SelectedBlockNode.IsHeading && mProjectView.SelectedBlockNode.Used &&
            //    mProjectView.SelectedBlockNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat() > 0.0;
            //mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled = isAudioBlockSelected &&
            //    mProjectView.SelectedBlockNode.IsHeading;
            mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Visible = mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled;
            mMarkAudioBlockAsSectionHeadingToolStripMenuItem.Visible = !mUnmarkAudioBlockAsSectionHeadingToolStripMenuItem.Enabled;
        }

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

        private void mShowInTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSelectedStripInTOCView();
        }

        #region IMessageFilter Members

        private const UInt32 WM_KEYDOWN = 0x0100;
        private const UInt32 WM_SYSKEYDOWN = 0x0104;

        private void mProjectView_Load(object sender, EventArgs e)
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


        private void InitialiseHighContrastSettings()
        {
            // Associate  user preference system events
            Microsoft.Win32.SystemEvents.UserPreferenceChanged
                += new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            //UserControls.Colors.SetHighContrastColors(SystemInformation.HighContrast);
            //mProjectView.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            //BackColor = UserControls.Colors.ObiBackGround;
            
        }

        private void UserPreferenceChanged( object sender , EventArgs e )
        {
            UserControls.Colors.SetHighContrastColors( SystemInformation.HighContrast );
            //mProjectView.TransportBar.SetHighContrastColors(SystemInformation.HighContrast);
            BackColor = UserControls.Colors.ObiBackGround;
            mProject.Touch();
        }

/// <summary>
///  move keyboard focus amung TOC view, Strip view, Transport Bar
/// <see cref=""/>
/// </summary>
/// <param name="Clockwise">
///  true for clockwise movement
/// </param>
        private void MoveToNextPanel( bool Clockwise )
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
                mProjectView.CurrentSelection = new NodeSelection(TempPhraseNode.ParentSection, mProjectView.TOCPanel);
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
        private void mNewProjectFromImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                importer.ImportFromXHTML(openFile.FileName, mProject);
            }
            catch (Exception ex)
            {
                //report failure and undo the creation of a new project
                MessageBox.Show("Import failed: " + ex.Message);
                mProject.Close();
                File.Delete(dialog.Path);
                mProjectView.TransportBar.Enabled = false;
                RemoveRecentProject(dialog.Path);
                return;
            }
        
            Ready();
            mProjectView.TransportBar.Enabled = true;
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

        // TODO: merge full and simple metadata editing into a single dialog with two tabs
        private void mFullMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FullMetadata dialog = new FullMetadata(mProject);
            List<urakawa.metadata.Metadata> affected = new List<urakawa.metadata.Metadata>();
            foreach (object o in mProject.getPresentation ().getMetadataList())
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
                foreach (urakawa.metadata.Metadata m in affected) mProject.getPresentation ().deleteMetadata(m.getName());
                foreach (UserControls.MetadataPanel p in dialog.MetadataPanels)
                {
                    if (p.CanSetName)
                    {
                        urakawa.metadata.Metadata m = (urakawa.metadata.Metadata)mProject.getPresentation ().getMetadataFactory().createMetadata();
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
        /// Create a new project. The application listens to the project's state change and
        /// command created events.
        /// </summary>
        /// <param name="path">Path of the XUK file to the project.</param>
        /// <param name="title">Title of the project.</param>
        /// <param name="createTitleSection">If true, a title section is automatically created.</param>
        private void CreateNewProject(string path, string title, bool createTitleSection)
        {
            mProject = new Project(path);
            mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
            mProject.Initialize(title, mSettings.GeneratedID, mSettings.UserProfile, createTitleSection);
            AddRecentProject(mProject.XUKPath);
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about what to do.
        /// Close the project if that is what the user wants to do or if it was unmodified.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool DidCloseProject()
        {
            mProjectView.TransportBar.Stop();
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"),
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.Save();
                        mProject.Close();
                        return true;
                    case DialogResult.No:
                        mProject.Close();
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                if (mProject != null) mProject.Close();
                return true;
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
                try
                {
                    // let's see if we can actually write the file that the user chose (bug #1679175)
                    FileStream file = File.Create(dialog.Path);
                    file.Close();
                    mSettings.CreateTitleSection = dialog.CreateTitleSection;
                    if (DidCloseProject())
                    {
                        CreateNewProject(dialog.Path, dialog.Title, dialog.CreateTitleSection);
                    }
                    else
                    {
                        Ready();
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(String.Format(Localizer.Message("cannot_create_file_text"), dialog.Path, x.Message),
                        Localizer.Message("cannot_create_file_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Ready();
                }
            }
            else
            {
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
                    mViewToolStripMenuItem.Enabled = false;
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    FormUpdateModifiedProject();
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    mProjectView.Project = mProject;
                    UpdateMenus();
                    FormUpdateOpenedProject();
                    mCommandManager.Clear();
                    mProjectView.SynchronizeWithCoreTree();
                    mProjectView.TOCViewVisibilityChanged += new EventHandler(mProjectView_TOCViewVisibilityChanged);
                    mProjectView.MetadataViewVisibilityChanged += new EventHandler(mProjectView_MetadataViewVisibilityChanged);
                    mViewToolStripMenuItem.Enabled = true;
                    mProjectView.TOCViewVisible = true;
                    mProjectView.MetadataViewVisible = true;
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
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

        private void mProjectView_CommandExecuted(object sender, UndoRedoEventArgs e) { UpdateUndoRedo(e); }
        private void mProjectView_CommandUnexecuted(object sender, UndoRedoEventArgs e) { UpdateUndoRedo(e); }

        private void UpdateUndoRedo(UndoRedoEventArgs e)
        {
            mUndoToolStripMenuItem.Enabled = e.Manager.canUndo();
            mUndoToolStripMenuItem.Text = e.Manager.canUndo() ?
                String.Format(Localizer.Message("undo_label"), Localizer.Message("undo"), e.Manager.getUndoShortDescription()) :
                Localizer.Message("cannot_undo");
            mRedoToolStripMenuItem.Enabled = e.Manager.canRedo();
            mRedoToolStripMenuItem.Text = e.Manager.canRedo() ?
                String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"), e.Manager.getRedoShortDescription()) :
                Localizer.Message("cannot_redo");
        }

        private void mProjectView_SelectionChanged(object sender, EventArgs e) { UpdateMenus(); }

        private void UpdateMenus()
        {
            showToolStripMenuItem.Enabled = mProjectView.CanShowInTOCView;
            showInStripsViewToolStripMenuItem.Enabled = mProjectView.CanShowInStripsView;
            mAddSectionToolStripMenuItem.Enabled = mProjectView.CanAddSection;
            mAddSubSectionToolStripMenuItem.Enabled = mProjectView.CanAddSubSection;
            mRenameSectionToolStripMenuItem.Enabled = mProjectView.CanRenameSection;
            mMoveOutToolStripMenuItem.Enabled = mProjectView.CanMoveSectionOut;
            mMoveInToolStripMenuItem.Enabled = mProjectView.CanMoveSectionIn;
            mMarkSectionAsUsedToolStripMenuItem.Visible = mProjectView.CanMarkSectionUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Visible = mProjectView.CanMarkSectionUnused;
            mMarkSectionAsUsedunusedToolStripMenuItem.Visible = !mProjectView.CanToggleSectionUsed;
            mInsertStripToolStripMenuItem.Enabled = mProjectView.CanAddStrip;
            mRenameStripToolStripMenuItem.Enabled = mProjectView.CanRenameStrip;
        }

        private void NEWundoToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Undo(); }
        private void NEWredoToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Redo(); }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Cut(); }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Copy(); }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Paste(); }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.Delete(); }

        private void mShowTOCViewToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TOCViewVisible = true; }
        private void mHideTOCViewToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.TOCViewVisible = false; }

        void mProjectView_TOCViewVisibilityChanged(object sender, EventArgs e)
        {
            mShowTOCViewToolStripMenuItem.Visible = !mProjectView.TOCViewVisible;
            mHideTOCViewToolStripMenuItem.Visible = mProjectView.TOCViewVisible;
        }

        private void mShowMetadataViewToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MetadataViewVisible = true; }
        private void mHideMetadataViewToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MetadataViewVisible = false; }

        private void mProjectView_MetadataViewVisibilityChanged(object sender, EventArgs e)
        {
            mShowMetadataViewToolStripMenuItem.Visible = !mProjectView.MetadataViewVisible;
            mHideMetadataViewToolStripMenuItem.Visible = mProjectView.MetadataViewVisible;
        }

        // Show a new source view window or give focus back to the previously opened one.
        private Dialogs.ShowSource mSourceView = null;
        private void showSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.Project != null)
            {
                if (mSourceView == null)
                {
                    mSourceView = new Dialogs.ShowSource(mProjectView);
                    mSourceView.FormClosed += new FormClosedEventHandler(delegate(object _sender, FormClosedEventArgs _e)
                    {
                        mSourceView = null;
                    });
                    mSourceView.Show();
                }
                else
                {
                    mSourceView.Focus();
                }
            }
        }

        // Show (select) the selected section in the TOC view 
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSectionInTOCView();
        }

        // Show (select) the selected section in the Strips view
        private void showInStripsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.ShowSelectedSectionInStripsView();
        }

        // Synchronize/desynchronize views
        private void mSynchronizeViewsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SynchronizeViews = !mSettings.SynchronizeViews;
        }

        private bool SynchronizeViews
        {
            set
            {
                mSettings.SynchronizeViews = value;
                mSynchronizeViewsToolStripMenuItem.Checked = value;
                mProjectView.SynchronizeViews = value;
            }
        }

        // TOC menu

        private void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewSection(); }
        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewSubSection(); }
        private void mRenameSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.StartRenamingSelectedSection(); }
        private void mMoveOutToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MoveSelectedSectionOut(); }
        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.MoveSelectedSectionIn(); }
        private void markSectionToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ToggleSectionUsed(); }
        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.ToggleSectionUsed(); }

        // Strips menu

        private void mInsertStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.AddNewStrip(); }
        private void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e) { mProjectView.StartRenamingSelectedStrip(); }
    }
}
