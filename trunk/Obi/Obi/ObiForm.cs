using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Obi.UserControls;

namespace Obi
{
    public partial class ObiForm : Form
    {
        private Project mProject;          // the project currently being authored
        private UndoRedoStack mUndoStack;  // the undo stack for this project [should belong to the project; saved together]

        private event Events.Project.StateChangedHandler ProjectStateChanged;

        /// <summary>
        /// Initialize a new form. No project is opened at creation time.
        /// </summary>
        public ObiForm()
        {
            InitializeComponent();
            
            mProject = null;
            ProjectStateChanged += new Events.Project.StateChangedHandler(mProject_StateChanged);
            GUIUpdateNoProject();

            mUndoStack = new UndoRedoStack();
            undo_label = mUndoToolStripMenuItem.Text;
            redo_label = mRedoToolStripMenuItem.Text;
            UndoStackChanged += new UndoStackChangedHandler(mUndoStack_UndoStackChanged);
        }

        /// <summary>
        /// Create a new project if the correct one was closed properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject = new Project();
                OnProjectCreated();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Obi project file (*.xuk)|*.xuk";
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    mProject = new Project(dialog.FileName);
                    OnProjectOpened();
                    return;
                }
            }
            Ready();
        }

        /// <summary>
        /// Close the current project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject = null;
            }
        }

        /// <summary>
        /// Exit if and only if the currently open project was saved correctly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                Application.Exit();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about whatto do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("close_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    mProject.Save();
                    OnProjectSaved();
                }
                if (result == DialogResult.Cancel)
                {
                    Ready();
                    return false;
                }
                else
                {
                    OnProjectClosed();
                    return true;
                }
            }
            else
            {
                Ready();
                return true;
            }
        }

        /// <summary>
        /// Touch the project so that it seems that it was modified (used for debugging.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void touchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.Touch();
            OnProjectModified();
        }

        private void OnProjectClosed()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Closed));
        }

        private void OnProjectCreated()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Created));
        }

        private void OnProjectModified()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Modified));
        }

        private void OnProjectOpened()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Opened));
        }

        private void OnProjectSaved()
        {
            ProjectStateChanged(this, new Events.Project.StateChangedEventArgs(mProject, Obi.Events.Project.StateChange.Saved));
        }

        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    GUIUpdateNoProject();
                    break;
                case Obi.Events.Project.StateChange.Created:
                    GUIUpdateSavedProject();
                    Debug(Localizer.Message("debug_created_project"));
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    GUIUpdateUnsavedProject();
                    Debug(Localizer.Message("debug_touched_project"));
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    GUIUpdateSavedProject();
                    Debug(String.Format(Localizer.Message("debug_opened_project"), e.Project.XUKPath));
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    GUIUpdateSavedProject();
                    Debug(String.Format(Localizer.Message("debug_saved_project"), e.Project.XUKPath));
                    break;
                default:
                    Debug("******");
                    break;
            }
        }

        /// <summary>
        /// Utility method that prints to the console and outputs to the status bar.
        /// </summary>
        /// <param name="message">The message to print.</param>
        private void Debug(string message)
        {
            Console.WriteLine(message);
            toolStripStatusLabel1.Text = message;
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void GUIUpdateNoProject()
        {
            closeProjectToolStripMenuItem.Enabled = false;
            saveProjectToolStripMenuItem.Enabled = false;
            saveProjectasToolStripMenuItem.Enabled = false;
            touchProjectToolStripMenuItem.Enabled = false;
            revertToolStripMenuItem.Enabled = false;
            Ready();
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when the project is saved.
        /// </summary>
        private void GUIUpdateSavedProject()
        {
            closeProjectToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = false;
            saveProjectasToolStripMenuItem.Enabled = true;
            touchProjectToolStripMenuItem.Enabled = true;
            revertToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Update the GUI to reflect enabled/disabled actions when the project is unsaved.
        /// </summary>
        private void GUIUpdateUnsavedProject()
        {
            closeProjectToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = true;
            saveProjectasToolStripMenuItem.Enabled = true;
            touchProjectToolStripMenuItem.Enabled = true;
            revertToolStripMenuItem.Enabled = true;
            Ready();
        }

        private void Ready()
        {
            toolStripStatusLabel1.Text = Localizer.Message("ready");
        }


        



        private string undo_label;
        private string redo_label;

        private delegate void UndoStackChangedHandler(object sender, EventArgs e);
        private event UndoStackChangedHandler UndoStackChanged;

        private void mStrip_Resized(object sender, ResizedEventArgs e)
        {
            PushUndo(new ResizeStripCommand((EmptyStrip)sender, e.Before, e.After));
        }

        private void PushUndo(Command command)
        {
            mUndoStack.Push(command);
            UndoStackChanged(this, new EventArgs());
        }

        private void mUndoStack_UndoStackChanged(object sender, EventArgs e)
        {
            if (mUndoStack.CanUndo)
            {
                mUndoToolStripMenuItem.Enabled = true;
                mUndoToolStripMenuItem.Text = String.Format(Localizer.Message("undo_label"), undo_label, mUndoStack.UndoLabel);
            }
            else
            {
                mUndoToolStripMenuItem.Enabled = false;
                mUndoToolStripMenuItem.Text = undo_label;
            }
            if (mUndoStack.CanRedo)
            {
                mRedoToolStripMenuItem.Enabled = true;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), redo_label, mUndoStack.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = redo_label;
            }
        }

        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mUndoStack.CanUndo)
            {
                mUndoStack.Undo();
                UndoStackChanged(this, new EventArgs());
            }
        }

        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mUndoStack.CanRedo)
            {
                mUndoStack.Redo();
                UndoStackChanged(this, new EventArgs());
            }
        }
    }
}