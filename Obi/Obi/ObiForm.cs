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
        private UndoRedoStack mUndoStack;

        private string undo_label;
        private string redo_label;

        private delegate void UndoStackChangedHandler(object sender, EventArgs e);
        private event UndoStackChangedHandler UndoStackChanged;

        public ObiForm()
        {
            InitializeComponent();
            mStrip.Resized += new EmptyStrip.ResizedHandler(mStrip_Resized);
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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