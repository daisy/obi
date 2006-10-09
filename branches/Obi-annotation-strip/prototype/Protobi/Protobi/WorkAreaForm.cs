using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class WorkAreaForm : Form
    {
        private StripManager mStripManager;  // the strip manager for the main layout
        private UndoRedoStack mUndoStack;    // the application undo/redo stack
        private Metadata mMetadata;          // should be an actuall Book object

        private string undo_label;  // keep track of the original labels
        private string redo_label;  // for undo and redo

        public WorkAreaForm()
        {
            InitializeComponent();
            mStripManager = new StripManager(stripLayout);
            mUndoStack = new UndoRedoStack();
            mMetadata = new Metadata();
            metadataInfoPanel1.MetadataController = mMetadata;
            undo_label = undoToolStripMenuItem.Text;
            redo_label = redoToolStripMenuItem.Text;
            openFileDialog1.Filter = "Wave file|*.wav|Any file|*.*";
        }

        // Exit cleanly. This should ask for confirmation when there is unsaved work.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Push a command in the undo stack and modify the undo/redo menu items accordingly.
        /// </summary>
        /// <param name="command">The command to push.</param>
        public void PushUndo(Command command)
        {
            mUndoStack.Push(command);
            // we can undo now
            undoToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Text = String.Format(Localizer.GetString("undo"), undo_label, mUndoStack.UndoLabel);
            // and we cannot redo anymore
            redoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Text = redo_label;
        }

        // Undo the last command and update the undo/redo menu items. Nothing happens if the stack is empty
        // (normally this command is never called when there is nothing to undo.)
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mUndoStack.UndoLabel != null)
            {
                mUndoStack.Undo();
                string label = mUndoStack.UndoLabel;
                if (label == null)
                {
                    // nothing left to undo, disable the undo menu and restore its label
                    undoToolStripMenuItem.Enabled = false;
                    undoToolStripMenuItem.Text = undo_label;
                }
                else
                {
                    // update the undo menu with the next label
                    undoToolStripMenuItem.Text = String.Format(Localizer.GetString("undo"), undo_label, label);
                }
                // we can redo now
                redoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Text = String.Format(Localizer.GetString("redo"), redo_label, mUndoStack.RedoLabel);
            }
        }

        // Redo the last command that was undone and update the undo/redo menu items.
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mUndoStack.Redo();
            string label = mUndoStack.RedoLabel;
            if (label == null)
            {
                redoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Text = redo_label;
            }
            else
            {
                redoToolStripMenuItem.Text = String.Format(Localizer.GetString("redo"), redo_label, label);
            }
            undoToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Text = String.Format(Localizer.GetString("undo"), undo_label, mUndoStack.UndoLabel);
        }

        // Append a new container strip in the work area.
        private void appendStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AppendParStripCommand append = new AppendParStripCommand(mStripManager);
            append.Do();
            PushUndo(append);
        }

        // Clicking in the work area deselects any currently selected item.
        private void stripLayout_MouseClick(object sender, MouseEventArgs e)
        {
            Deselect();
        }

        // Deselect any currently selected item.
        private void deselectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deselect();
        }

        // Update the menus when something is selected.
        public void EnableSelect()
        {
            deselectToolStripMenuItem.Enabled = true;     // we can now deselect
            renameStripToolStripMenuItem.Enabled = true;  // the selected item can be renamed
            editHeadingToolStripMenuItem.Enabled = true;
            addAudioStripToolStripMenuItem.Enabled = mStripManager.Selected == null ?
                false : mStripManager.Selected.CanAddAudioStrip;
            loadSoundFileToolStripMenuItem.Enabled = mStripManager.Selected == null ?
                false : mStripManager.Selected.CanAddAudioFile;
        }

        // Update the menus when deselecting.
        private void Deselect()
        {
            mStripManager.Select(null);
            deselectToolStripMenuItem.Enabled = false;     // cannot deselect anymore
            renameStripToolStripMenuItem.Enabled = false;  // nothing to rename at the moment
            editHeadingToolStripMenuItem.Enabled = false;
            addAudioStripToolStripMenuItem.Enabled = false;
            loadSoundFileToolStripMenuItem.Enabled = false;
        }

        // Rename the currently selected item. This brings a text entry window in which the user can type the new name.
        // Nothing happens if there is no selected item at the moment.
        private void renameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Strip strip = mStripManager.Selected;
            if (strip != null)
            {
                RenameStripBox box = new RenameStripBox(strip.Label);
                box.ShowDialog();
                if (box.DialogResult == DialogResult.OK)
                {
                    // Create the rename command. If the new label is too long for the strip, then resize it as well and issue
                    // a cons command so that undoing will restore the previous size of the strip.
                    ResizeStripCommand resize = new ResizeStripCommand(strip, strip.Size);
                    RenameStripCommand rename = new RenameStripCommand(strip, box.Label);
                    rename.Do();
                    PushUndo(new ConsCommand(rename.Label, resize, rename));
                }
            }
        }

        private void bibliographicMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMetadata.Edit();
        }

        private void stripLayout_SizeChanged(object sender, EventArgs e)
        {
            metadataInfoPanel1.Width = Width;
        }

        private void editHeadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Strip strip = mStripManager.Selected;
            if (strip.GetType() == typeof(StructureStrip))
            {
                ((StructureStripUserControl)((StructureStrip)strip).UserControl).EditHeading();
            }
            else if (strip.GetType() == typeof(ParStrip))
            {
                ((StructureStripUserControl)((ParStrip)strip).StructureStrip.UserControl).EditHeading();
            }
        }

        private void addAudioStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ParStrip)(mStripManager.Selected)).AddAudioStrip();
        }

        private void loadSoundFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WaveFile wavefile = new WaveFile(openFileDialog1.FileName);
                    ((AudioStrip)(mStripManager.Selected)).LoadFile(wavefile);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, Localizer.GetString("error_loading_wav"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}