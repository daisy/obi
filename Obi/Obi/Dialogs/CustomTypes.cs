using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Obi.ProjectView;

namespace Obi.Dialogs
{
    public partial class CustomTypes : Form
    {
        private Presentation mPresentation;
        private ProjectView.ProjectView mProjectView;
        int mNumberOfCommandsSinceOpened;

        public CustomTypes(Presentation presentation, ProjectView.ProjectView projectView)
        {
            if (presentation == null) throw new Exception("Invalid presentation for custom types dialog");
            if (projectView == null) throw new Exception("Invalid project view for custom types dialog");
            InitializeComponent();
            mNumberOfCommandsSinceOpened = 0;
            mPresentation = presentation;
            mProjectView = projectView;
         }

        /// <summary>
        /// Keypress in the list box.
        /// Enter: apply the selected type to the selected block
        /// Delete: remove the selected type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCustomTypesList_KeyUp(object sender, KeyEventArgs e)
        {
            //Close the dialog and apply the change
            if (e.KeyCode == Keys.Enter)
            {
                ApplyAndClose();
            }
            //Remove the custom type from the list and also from all blocks
            //do not close the dialog
            if (e.KeyCode == Keys.Delete)
            {
                string removedType = (string)mCustomTypesList.SelectedItem;
                mCustomTypesList.Items.RemoveAt(mCustomTypesList.SelectedIndex);
                bool res = RemoveCustomTypeFromNodes(removedType);
            }
        }
        /// <summary>
        /// Keypress in the text field.
        /// Enter: apply the selected type to the selected block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mNewCustomType_KeyUp(object sender, KeyEventArgs e)
        {
            //Close the dialog and apply the change
            if (e.KeyCode == Keys.Enter)
            {
                ApplyAndClose();
            }

        }
        /// <summary>
        /// If there is text in the text box, apply it to the block
        /// </summary>
        private void ApplyAndClose()
        {
            if (mNewCustomType.Text != "")
            {
                urakawa.undo.CompositeCommand command = new urakawa.undo.CompositeCommand();
                //this will filter duplicates
                Commands.Node.AddCustomType cmd = new Obi.Commands.Node.AddCustomType(mProjectView, mPresentation, mNewCustomType.Text);
                Commands.Node.ChangeCustomType otherCmd = new Obi.Commands.Node.ChangeCustomType(mProjectView, mProjectView.SelectedBlockNode, mNewCustomType.Text);
                command.append(cmd);
                command.append(otherCmd);
                mPresentation.UndoRedoManager.execute(command);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Go through the tree and remove the given custom type from any node who has it
        /// </summary>
        /// <param name="removedType"></param>
        /// <returns></returns>
        private bool RemoveCustomTypeFromNodes(string removedType)
        {
            urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
            command.setShortDescription(Localizer.Message("remove_custom_type"));
            bool foundNodeWithThisType = false;
            mPresentation.RootNode.acceptDepthFirst
            (
                // Remove the custom kind, phrase kind, and custom kind label for any phrase with
                // a custom kind label that no longer exists
                delegate(urakawa.core.TreeNode n)
                {
                    //is this a custom type of phrase?
                    if (n is PhraseNode && ((PhraseNode)n).PhraseKind == PhraseNode.Kind.Custom)
                    {
                        string customKind = ((PhraseNode)n).CustomKind;
                        if (customKind == removedType)
                        {
                            foundNodeWithThisType = true;
                            Commands.Node.ChangeCustomType cmd = new Commands.Node.ChangeCustomType(mProjectView, (PhraseNode)n, "");
                            command.append(cmd);
                        }
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(urakawa.core.TreeNode n) { }
            );
            Commands.Node.RemoveCustomType otherCmd = new Commands.Node.RemoveCustomType(mProjectView, mPresentation, removedType);
            command.append(otherCmd);
            mPresentation.UndoRedoManager.execute(command);
            mNumberOfCommandsSinceOpened++;
            return foundNodeWithThisType;
        }

        private void mCustomTypesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            mNewCustomType.Text = (string)mCustomTypesList.SelectedItem;
        }

        private void mOk_Click(object sender, EventArgs e)
        {
            ApplyAndClose();
        }

        private void mCancel_Click(object sender, EventArgs e)
        {
            //undo anything we've done since this dialog has been open
            for (int i = 0; i < mNumberOfCommandsSinceOpened; i++) mPresentation.UndoRedoManager.undo();
            //end the dialog
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CustomTypes_Load(object sender, EventArgs e)
        {
            if (mPresentation.CustomTypes == null || mPresentation.CustomTypes.Count == 0) return;
            foreach (string customType in mPresentation.CustomTypes)
            {
                mCustomTypesList.Items.Add(customType);
            }
            mNewCustomType.Focus();
        }
    }
}