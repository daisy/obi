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
        /// <summary>
        /// Create a new project if the correct one was closed properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                ProjectManager.Instance.CreateProject();
            }
            else
            {
                Console.WriteLine(Localizer.Message("cancelled") + ".");
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
                    ProjectManager.Instance.OpenXUK(dialog.FileName);
                    return;
                }
            }
            Console.WriteLine(Localizer.Message("cancelled") + ".");    
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
                Console.WriteLine(Localizer.Message("cancelled") + ".");
            }
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about whatto do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (ProjectManager.Instance.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("close_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // save the project
                }
                return result != DialogResult.Cancel;
            }
            else
            {
                return true;
            }
        }


        
        
        
        private UndoRedoStack mUndoStack;

        private string undo_label;
        private string redo_label;

        private delegate void UndoStackChangedHandler(object sender, EventArgs e);
        private event UndoStackChangedHandler UndoStackChanged;

        public ObiForm()
        {
            InitializeComponent();
            //mStrip.Resized += new EmptyStrip.ResizedHandler(mStrip_Resized);
            UndoStackChanged += new UndoStackChangedHandler(mUndoStack_UndoStackChanged);
            mUndoStack = new UndoRedoStack();
            undo_label = mUndoToolStripMenuItem.Text;
            redo_label = mRedoToolStripMenuItem.Text;
        }

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