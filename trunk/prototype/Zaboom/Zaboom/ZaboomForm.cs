using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa.undo;

namespace Zaboom
{
    public partial class ZaboomForm : Form
    {
        public ZaboomForm()
        {
            InitializeComponent();
            Text = "Zaboom";
            Status = "Ready.";
            UpdateUndoRedoMenuItems();
            projectPanel.SelectionChanged += new UserControls.SelectionChangedHandler(projectPanel_SelectionChanged);
        }

        private bool CanDelete { get { return projectPanel.Project != null && projectPanel.Selected.Count > 0; } }

        /// <summary>
        /// Create a new, blank project.
        /// Bring up a dialog to choose a location and a title.
        /// </summary>
        private void CreateNewProject()
        {
            NewProjectDialog dialog = new NewProjectDialog(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "project.zab",
                "Zaboom project files|*.zab",
                "Untitled Zaboom Project");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                projectPanel.Project = new Project(dialog.Title, dialog.ProjectLocation);
                projectPanel.Project.StateChanged += new StateChangedHandler(Project_StateChanged);
                projectPanel.Project.Save();
            }
        }

        /// <summary>
        /// Delete the selected items.
        /// </summary>
        private void Delete()
        {
            if (CanDelete)
            {
                List<UserControls.Selectable> ss = new List<UserControls.Selectable>(projectPanel.Selected);
                foreach (UserControls.Selectable s in ss)
                {
                    Commands.DeleteTreeNode command = new Commands.DeleteTreeNode(s.Node);
                    projectPanel.CommandManager.AddAndExecute(command);
                }
            }
        }

        /// <summary>
        /// Import a new audio file into the project.
        /// Bring up a dialog to choose the file.
        /// </summary>
        private void ImportFile()
        {
            if (projectPanel.Project != null && projectPanel.Selected.Count > 0)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = ("WAV files|*.wav");
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Commands.Command command = projectPanel.Project.ImportAudioFileCommand(dialog.FileName,
                            projectPanel.Selected[0].Node);
                        projectPanel.CommandManager.AddAndExecute(command);
                    }
                    catch (Exception e_)
                    {
                        MessageBox.Show(
                            String.Format("Could not open WAV file {0}: {1}", dialog.FileName, e_.Message),
                            "Error opening WAV file",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Open a new project by choosing a XUK file from a dialog.
        /// </summary>
        private void OpenProject()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Zaboom project files|*.zab";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = false;
            dialog.SupportMultiDottedExtensions = true;
            dialog.Title = "Open a Zaboom project file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    projectPanel.Project = new Project(dialog.FileName);
                    projectPanel.Project.StateChanged += new StateChangedHandler(Project_StateChanged);
                    projectPanel.Project.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("An error occurred while opening the project file {0}: {1}",
                        dialog.FileName, ex.Message), "Error opening project file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Redo the first command.
        /// </summary>
        private void Redo()
        {
            if (projectPanel.CommandManager.canRedo())
            {
                projectPanel.CommandManager.redo();
                UpdateUndoRedoMenuItems();
            }
        }

        /// <summary>
        /// Save the project to a XUK file.
        /// </summary>
        private void SaveProject()
        {
            if (projectPanel.Project != null) projectPanel.Project.Save();
        }

        /// <summary>
        /// Display a message in the status bar.
        /// </summary>
        private string Status { set { statusLabel.Text = value; } }

        /// <summary>
        /// Undo the last command.
        /// </summary>
        private void Undo()
        {
            if (projectPanel.CommandManager.canUndo())
            {
                projectPanel.CommandManager.undo();
                UpdateUndoRedoMenuItems();
            }
        }

        /// <summary>
        /// Update the enabled status of undo and redo as well as their labels.
        /// </summary>
        private void UpdateUndoRedoMenuItems()
        {
            undoToolStripMenuItem.Enabled = projectPanel.CommandManager.canUndo();
            undoToolStripMenuItem.Text = projectPanel.CommandManager.canUndo() ?
                projectPanel.CommandManager.getUndoShortDescription() : "Undo";
            redoToolStripMenuItem.Enabled = projectPanel.CommandManager.canRedo();
            redoToolStripMenuItem.Text = projectPanel.CommandManager.canRedo() ?
                projectPanel.CommandManager.getRedoShortDescription() : "Redo";
        }

        /// <summary>
        /// Open a view source window.
        /// </summary>
        private void ViewSource()
        {
            if (projectPanel.Project != null)
            {
                SourceView dialog = new SourceView(projectPanel.Project);
                dialog.Show();
            }
        }

        /// <summary>
        /// Zoom in in the project panel.
        /// </summary>
        private void ZoomIn()
        {
            if (projectPanel.Project != null) projectPanel.PixelsPerSecond *= 2;
        }

        /// <summary>
        /// Zoom out in the project panel.
        /// </summary>
        private void ZoomOut()
        {
            if (projectPanel.Project != null) projectPanel.PixelsPerSecond /= 2;
        }


        #region menu event handlers

        // File
        private void newToolStripMenuItem_Click(object sender, EventArgs e) { CreateNewProject(); }
        private void openToolStripMenuItem_Click(object sender, EventArgs e) { OpenProject(); }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) { SaveProject(); }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }

        // Edit
        private void undoToolStripMenuItem_Click(object sender, EventArgs e) { Undo(); }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) { Redo(); }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) { Delete(); }

        // Audio
        private void importFileToolStripMenuItem_Click(object sender, EventArgs e) { ImportFile(); }
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e) { ZoomIn(); }
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e) { ZoomOut(); }

        // Tools
        private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e) { ViewSource(); }

        #endregion


        /// <summary>
        /// Update the title bar and the undo/redo menus when the project state has changed.
        /// </summary>
        private void Project_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.Change == StateChange.Closed)
            {
                Text = "Zaboom";
            }
            else if (e.Change == StateChange.Modified || e.Change == StateChange.Opened || e.Change == StateChange.Saved)
            {
                Text = "Zaboom - " + projectPanel.Project.TitleSaved;
            }

            bool hasProject = projectPanel.Project != null;
            saveToolStripMenuItem.Enabled = hasProject && projectPanel.Project.CanSave;

            deleteToolStripMenuItem.Enabled = CanDelete;

            bool hasProjectWithContents = projectPanel.Project != null && !projectPanel.Project.IsEmpty;
            importFileToolStripMenuItem.Enabled = hasProject && projectPanel.Selected.Count > 0;
            zoomInToolStripMenuItem.Enabled = hasProjectWithContents;
            zoomOutToolStripMenuItem.Enabled = hasProjectWithContents;
            
            UpdateUndoRedoMenuItems();
        }

        private void projectPanel_SelectionChanged(UserControls.ProjectPanel projectPanel, EventArgs e)
        {
            importFileToolStripMenuItem.Enabled = projectPanel.Selected.Count > 0;
        }
    }
}