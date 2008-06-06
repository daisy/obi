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
        /// <summary>
        /// Initialize a new form for the application.
        /// </summary>
        public BobiForm()
        {
            InitializeComponent();
            this.projectView.Paint += new PaintEventHandler(projectView_Paint);
            Project = new Project();
        }


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
            if (CanClose() && dialog.ShowDialog() == DialogResult.OK) Project = new Project(new Uri(dialog.FileName));
        }

        private void projectView_Paint(object sender, PaintEventArgs e)
        {
            System.Diagnostics.Debug.Print("Paint from project view; initialized project ? {0}.",
                Project != null && Project.Initialized ? "yes" : "no");
            if (Project != null && !Project.Initialized)
            {
                Cursor c = Cursor;
                Cursor = Cursors.WaitCursor;
                this.projectView.Paint -= new PaintEventHandler(projectView_Paint);
                Invalidate(true);
                Update();
                this.projectView.Paint += new PaintEventHandler(projectView_Paint);
                this.projectView.SuspendLayout();
                try
                {
                    Project.Open();
                    this.statusLabel.Text = string.Format("Opened project \"{0}\".", Project.Path);
                }
                catch (Exception x)
                {
                    MessageBox.Show(string.Format("Error opening file {0}: {1}", Project.Path, x.Message),
                        "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Project = null;
                }
                finally
                {
                    Cursor = c;
                    this.projectView.ResumeLayout();
                }
            }
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

        }

        // &Edit > Select &nothing (Ctrl+Shift+A)
        private void edit_SelectNothingMenuItem_Click(object sender, EventArgs e)
        {

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
            if (Project != null) Project.NewTrack();
        }

        // &Audio > &Import audio (Ctrl+I)
        private void audio_ImportAudioMenuItem_Click(object sender, EventArgs e)
        {
            if (Project != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Import audio";
                dialog.Filter = "WAV files (*.wav)|*.wav|Any file|*.*";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK) { }
            }
        }


        // Check whether the project can be closed. If there is a project
        // with unsaved changes, then ask whether the user wants to save.
        private bool CanClose()
        {
            bool can = Project == null || !Project.HasChanges;
            if (!can)
            {
                DialogResult results = MessageBox.Show("Do you want to save your changes before closing? Press \"Yes\" to save changes, \"No\" to close without saving, and \"Cancel\" to not close at all.",
                    "Save before closing?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                can = results == DialogResult.Yes ? SavedProject() : results == DialogResult.No;
            }
            return can;
        }

        // Set a new or null project in the current view.
        private Project Project
        {
            get { return this.projectView.Project; }
            set
            {
                this.projectView.Project = value;
                Text = "Bobi";
                if (value == null)
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
                    value.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(project_changed);
                    if (value.getNumberOfPresentations() == 0)
                    {
                        value.presentationAdded +=
                            new EventHandler<urakawa.events.project.PresentationAddedEventArgs>(project_presentationAdded);
                    }
                    else
                    {
                        SetPresentationEvents(value.getPresentation(0));
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
        }

        void project_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (Project.Initialized)
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
            }
        }

        private void project_commandDone(object sender, urakawa.events.undo.DoneEventArgs e)
        {
            statusLabel.Text = e.DoneCommand.getLongDescription();
        }

        private void project_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e)
        {
            statusLabel.Text = e.ReDoneCommand.getLongDescription();
        }

        private void project_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e)
        {
            statusLabel.Text = string.Format("Undid {0}", e.UnDoneCommand.getShortDescription());
        }

        private void project_presentationAdded(object sender, urakawa.events.project.PresentationAddedEventArgs e)
        {
            SetPresentationEvents(e.AddedPresentation);
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

        private void BobiForm_Paint(object sender, PaintEventArgs e)
        {
            System.Diagnostics.Debug.Print("Paint from {0} = {1}", sender, e.ClipRectangle);
        }
    }
}