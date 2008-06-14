using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi
{
    public partial class BobiForm : Form
    {
        private Settings settings;
        private ColorSettings colorScheme;

        /// <summary>
        /// Initialize a new form for the application.
        /// </summary>
        public BobiForm()
        {
            InitializeComponent();
            this.settings = new Settings();
            SetColorScheme(SystemInformation.HighContrast ? this.settings.ColorScheme_HighContrast : this.settings.ColorScheme);
            Microsoft.Win32.SystemEvents.UserPreferenceChanged +=
                new Microsoft.Win32.UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            this.projectView.SelectionSet += new SelectionSetEventHandler(projectView_SelectionSet);
            HideStatusProgressBar();
            Project = new Project();
        }


        /// <summary>
        /// Current color scheme for the whole application.
        /// </summary>
        public ColorSettings ColorSettings { get { return this.colorScheme; } }

        /// <summary>
        /// Current settings.
        /// </summary>
        public Settings Settings { get { return this.settings; } }

        // &File > &New (Ctrl+N)
        private void file_NewMenuItem_Click(object sender, EventArgs e)
        {
            if (CanClose()) Project = new Project();
        }

        // &File > &Open (Ctrl+O)
        private void file_OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XUK files (*.xuk)|*.xuk";
            if (CanClose() && dialog.ShowDialog() == DialogResult.OK) Open(dialog.FileName);
        }

        // &File > &Close (Ctrl+W)
        private void file_CloseMenuItem_Click(object sender, EventArgs e)
        {
            if (CanClose()) Project = null;
        }

        // &File > &Save (Ctrl+S)
        private void file_SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (Project != null) SavedProject();
        }

        // &File > E&xit (Alt+F4)
        private void file_ExitMenuItem_Click(object sender, EventArgs e)
        {
            if (CanClose())
            {
                Project = null;
                Close();
            }
        }

        // Closing
        private void BobiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CanClose();
        }


        // &Edit > &Undo (Ctrl+Z)
        private void edit_UndoMenuItem_Click(object sender, EventArgs e)
        {
            Project.Undo();
        }

        // &Edit > &Redo (Ctrl+Y)
        private void edit_RedoMenuItem_Click(object sender, EventArgs e)
        {
            Project.Redo();
        }

        // &Edit > Cu&t (Ctrl+X)
        private void edit_CutMenuItem_Click(object sender, EventArgs e)
        {

        }

        // &Edit > &Copy (Ctrl+C)
        private void edit_CopyMenuItem_Click(object sender, EventArgs e)
        {

        }

        // &Edit > &Paste (Ctrl+V)
        private void edit_PasteMenuItem_Click(object sender, EventArgs e)
        {

        }

        // &Edit > &Delete (Del)
        private void edit_DeleteMenuItem_Click(object sender, EventArgs e)
        {

        }

        // &Edit > Select &all (Ctrl+A)
        private void edit_SelectAllMenuItem_Click(object sender, EventArgs e)
        {
            this.projectView.SelectAllFromAbove();
            UpdateStatusForNewSelection(this.projectView.Selection);
        }

        // &Edit > Select &nothing (Ctrl+Shift+A)
        private void edit_SelectNothingMenuItem_Click(object sender, EventArgs e)
        {
            this.projectView.SelectFromAbove(null);
            UpdateStatusForNewSelection(null);
        }


        // &View > Zoom &in (Ctrl++)
        private void view_ZoomInMenuItem_Click(object sender, EventArgs e)
        {
            this.projectView.Zoom *= 1.25f; 
        }

        // &View > Zoom &out (Ctrl+-)
        private void view_ZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            this.projectView.Zoom /= 1.25f;
        }

        // &View > &Normal size (Ctrl+0)
        private void view_NormalSizeMenuItem_Click(object sender, EventArgs e) 
        {
            this.projectView.Zoom = 1.0f; 
        }


        // &Audio > New &track (Ctrl+T)
        private void audio_NewTrackMenuItem_Click(object sender, EventArgs e)
        {
            if (Project != null) NewTrack();
        }

        // &Audio > &Import audio (Ctrl+I)
        private void audio_ImportAudioMenuItem_Click(object sender, EventArgs e)
        {
            if (this.projectView.Selection is NodeSelection && 
                ((NodeSelection)this.projectView.Selection).ItemsInSelection == 1)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Import audio";
                dialog.Filter = "WAV files (*.wav)|*.wav|Any file|*.*";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK) foreach (string filename in dialog.FileNames) ImportAudio(filename);
            }
        }


        // Check whether the project can be closed. If there is a project
        // with unsaved changes, then ask whether the user wants to save.
        private bool CanClose()
        {
            bool can = Project == null || !Project.HasChanges;
            if (!can)
            {
                DialogResult results = MessageBox.Show(
                    "Do you want to save your changes before closing? Press \"Yes\" to save changes, \"No\" to close without saving, and \"Cancel\" to not close at all.",
                    "Save before closing?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                can = results == DialogResult.Yes ? SavedProject() : results == DialogResult.No;
            }
            return can;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Microsoft.Win32.SystemEvents.UserPreferenceChanged -=
                new Microsoft.Win32.UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            base.Dispose(disposing);
        }

        /// <summary>
        /// Update status and selection after a command was executed (done or redone.)
        /// </summary>
        private void ExecutedCommand(urakawa.undo.ICommand command)
        {
            statusLabel.Text = command.getLongDescription();
            if (command is Commands.Command && ((Commands.Command)command).UpdateSelection)
            {
                this.projectView.SelectFromAbove(((Commands.Command)command).SelectionAfter);
                UpdateStatusForNewSelection(this.projectView.Selection);
            }
        }

        // Hide the status progress bar
        private void HideStatusProgressBar()
        {
            this.statusProgressBar.Style = ProgressBarStyle.Blocks;
            this.statusProgressBar.Value = 0;
            this.statusProgressBar.Visible = false;
        }

        // Append a new track to the project.
        private void NewTrack()
        {
            Project.getPresentation(0).getUndoRedoManager().execute(new Commands.NewTrack(this.projectView));
        }

        // Import an audio file in the selected track
        private void ImportAudio(string filename)
        {
            Project.getPresentation(0).getUndoRedoManager().execute(new Commands.ImportAudio(this.projectView,
                ((NodeSelection)this.projectView.Selection).SingleNode, filename));
        }

        // Open a file while showing progress.
        // If something goes wrong catch the exception and show an error message.
        private void Open(string path)
        {
            bool opened = false;
            Project = new Project(new Uri(path));
            Cursor c = Cursor;
            Cursor = Cursors.WaitCursor;
            Invalidate(true);
            Update();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(delegate(object _sender, DoWorkEventArgs _e)
            {
                try
                {
                    Project.Open();
                    opened = !worker.CancellationPending;
                }
                catch (Exception x)
                {
                    MessageBox.Show(string.Format("Error opening file {0}: {1}", path, x.Message),
                        "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object _sender, RunWorkerCompletedEventArgs _e)
            {
                if (!opened) Project = null;
                UpdateStatus();
                HideStatusProgressBar();
                if (opened) this.statusLabel.Text = string.Format("Opened project \"{0}\".", path);
                this.projectView.ResumeLayout();
                Cursor = c;
            });
            worker.WorkerReportsProgress = false;
            worker.WorkerSupportsCancellation = true;
            this.statusLabel.Text = string.Format("Opening project \"{0}\"...", path);
            StartStatusProgressBar();
            this.projectView.SuspendLayout();
            worker.RunWorkerAsync();
        }

        // Set a new or null project in the current view.
        private Project Project
        {
            get { return this.projectView.Project; }
            set
            {
                this.projectView.Project = value;
                Text = "Bobi";
                UpdateStatus();
            }
        }

        // Set the color scheme for the project view
        private void SetColorScheme(ColorSettings scheme)
        {
            this.colorScheme = scheme;
            this.projectView.Colors = scheme;
        }

        void project_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (Project != null && Project.Initialized)
            {
                Text = string.Format("Bobi{0}", Project.HasChanges ? "*" : "");
                this.file_SaveMenuItem.Enabled = Project.HasChanges;
                // Undo and redo displays the command that can be un/redone
                urakawa.undo.UndoRedoManager undo = Project.getPresentation(0).getUndoRedoManager();
                this.edit_UndoMenuItem.Enabled = undo.canUndo();
                this.edit_UndoMenuItem.Text = string.Format("&Undo{0}",
                    this.edit_UndoMenuItem.Enabled ? " " + undo.getUndoShortDescription() : "");
                this.edit_RedoMenuItem.Enabled = Project.getPresentation(0).getUndoRedoManager().canRedo();
                this.edit_RedoMenuItem.Text = string.Format("&Redo{0}",
                    this.edit_RedoMenuItem.Enabled ? " " + undo.getRedoShortDescription() : "");
                this.edit_SelectAllMenuItem.Enabled = Project.NumberOfTracks > 0;
                this.edit_SelectNothingMenuItem.Enabled = this.projectView.Selection != null;
            }
        }

        private void project_commandDone(object sender, urakawa.events.undo.DoneEventArgs e)
        {
            ExecutedCommand(e.DoneCommand);
        }

        private void project_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e)
        {
            ExecutedCommand(e.ReDoneCommand);
        }

        private void project_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e)
        {
            statusLabel.Text = string.Format("Undid {0}", e.UnDoneCommand.getShortDescription());
            if (e.UnDoneCommand is Commands.Command)
            {
                this.projectView.SelectFromAbove(((Commands.Command)e.UnDoneCommand).SelectionBefore);
                UpdateStatusForNewSelection(this.projectView.Selection);
            }
        }

        private void project_presentationAdded(object sender, urakawa.events.project.PresentationAddedEventArgs e)
        {
            SetPresentationEvents(e.AddedPresentation);
        }

        private void projectView_SelectionSet(object sender, SelectionSetEventArgs e)
        {
            UpdateStatusForNewSelection(e.Selection);
        }

        // Save the project to its current path, or try a different path.
        // Return true on success.
        private bool SavedProject()
        {
            if (Project.Path == null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.AddExtension = true;
                dialog.Filter = "XUK files (*.xuk)|*.xuk";
                if (dialog.ShowDialog() == DialogResult.OK) Project.Path = new Uri(dialog.FileName);
            }
            if (Project.Path != null)
            {
                try
                {
                    Project.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not save to " + Project.Path + ": " + ex.Message);
                    Project.Path = null;
                }
            }
            return false;
        }

        // Set events once a presentation is added to the project
        // (after creating an empty presentation or opening a XUK file.)
        private void SetPresentationEvents(urakawa.Presentation presentation)
        {
            urakawa.undo.UndoRedoManager undo = presentation.getUndoRedoManager();
            undo.commandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(project_commandDone);
            undo.commandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(project_commandUnDone);
            undo.commandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(project_commandReDone);
        }

        // Show and start the status progress bar animation
        private void StartStatusProgressBar()
        {
            this.statusProgressBar.Visible = true;
            this.statusProgressBar.Style = ProgressBarStyle.Marquee;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            SetColorScheme(SystemInformation.HighContrast ? this.settings.ColorScheme_HighContrast : this.settings.ColorScheme);
        }

        // Update the status of the application (mostly menus)
        private void UpdateStatus()
        {
            if (Project == null || !Project.Initialized)
            {
                this.statusLabel.Text = "No project.";
                this.file_CloseMenuItem.Enabled = false;
                this.file_SaveMenuItem.Enabled = false;
                this.edit_UndoMenuItem.Enabled = false;
                this.edit_RedoMenuItem.Enabled = false;
                this.edit_CutMenuItem.Enabled = false;
                this.edit_CopyMenuItem.Enabled = false;
                this.edit_PasteMenuItem.Enabled = false;
                this.edit_DeleteMenuItem.Enabled = false;
                this.edit_SelectAllMenuItem.Enabled = false;
                this.edit_SelectNothingMenuItem.Enabled = false;
                this.view_ZoomInMenuItem.Enabled = false;
                this.view_ZoomOutMenuItem.Enabled = false;
                this.view_NormalSizeMenuItem.Enabled = false;
                this.audio_NewTrackMenuItem.Enabled = false;
                this.audio_ImportAudioMenuItem.Enabled = false;
            }
            else
            {
                Project.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(project_changed);
                if (Project.getNumberOfPresentations() == 0)
                {
                    Project.presentationAdded +=
                        new EventHandler<urakawa.events.project.PresentationAddedEventArgs>(project_presentationAdded);
                }
                else
                {
                    SetPresentationEvents(Project.getPresentation(0));
                }
                this.statusLabel.Text = "New project.";
                this.file_CloseMenuItem.Enabled = true;
                this.file_SaveMenuItem.Enabled = false;
                this.edit_UndoMenuItem.Enabled = false;
                this.edit_RedoMenuItem.Enabled = false;
                this.edit_CutMenuItem.Enabled = false;
                this.edit_CopyMenuItem.Enabled = false;
                this.edit_PasteMenuItem.Enabled = false;
                this.edit_DeleteMenuItem.Enabled = false;
                this.edit_SelectAllMenuItem.Enabled = false;
                this.edit_SelectNothingMenuItem.Enabled = false;
                this.view_ZoomInMenuItem.Enabled = true;
                this.view_ZoomOutMenuItem.Enabled = true;
                this.view_NormalSizeMenuItem.Enabled = true;
                this.audio_NewTrackMenuItem.Enabled = true;
                this.audio_ImportAudioMenuItem.Enabled = false;
            }
        }

        // Update status when the selection has changed
        private void UpdateStatusForNewSelection(Selection selection)
        {
            this.edit_CutMenuItem.Enabled = selection != null;
            this.edit_CopyMenuItem.Enabled = selection != null;
            this.edit_PasteMenuItem.Enabled = this.projectView.Clipboard != null && this.projectView.Clipboard.CanPaste(selection);
            this.edit_DeleteMenuItem.Enabled = selection != null;
            this.edit_SelectAllMenuItem.Enabled = this.projectView.HasUnselectedTrack;
            this.edit_SelectNothingMenuItem.Enabled = selection != null;
            this.audio_ImportAudioMenuItem.Enabled = selection is NodeSelection && selection.ItemsInSelection == 1;
        }
    }
}