using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Obi.ProjectView;

namespace Obi.Dialogs
{
    public partial class EditRoles : Form
    {
        private Presentation mPresentation;
        private ProjectView.ProjectView mProjectView;
        int mNumberOfCommandsSinceOpened;

        public EditRoles(Presentation presentation, ProjectView.ProjectView projectView)
        {
            if (presentation == null) throw new Exception("Invalid presentation for custom types dialog");
            if (projectView == null) throw new Exception("Invalid project view for custom types dialog");
            InitializeComponent();
            mNumberOfCommandsSinceOpened = 0;
            mPresentation = presentation;
            mProjectView = projectView;
         }

        private void AddNewRole(string role)
        {
            if (!mCustomRolesList.Items.Contains(role)) mCustomRolesList.Items.Add(role);
            Commands.AddCustomType cmd = new Obi.Commands.AddCustomType(mProjectView, mPresentation, role);
            mPresentation.UndoRedoManager.execute(cmd);
            mNumberOfCommandsSinceOpened++;
        }

        private void RemoveRole(string role)
        {
            mCustomRolesList.Items.Remove(role);
            RemoveCustomTypeFromNodes(role);
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
                    if (n is EmptyNode && ((EmptyNode)n).CustomClass == removedType)
                    {
                        foundNodeWithThisType = true;
                        Commands.Node.ChangeCustomType cmd = new Commands.Node.ChangeCustomType(mProjectView, (EmptyNode)n, EmptyNode.Kind.Plain);
                        command.append(cmd);
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(urakawa.core.TreeNode n) { }
            );
            Commands.RemoveCustomType otherCmd = new Commands.RemoveCustomType(mProjectView, mPresentation, removedType);
            command.append(otherCmd);
            mPresentation.UndoRedoManager.execute(command);
            mNumberOfCommandsSinceOpened++;
            return foundNodeWithThisType;
        }

        /// <summary>
        /// Press delete to remove a role from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCustomRolesList_KeyUp(object sender, KeyEventArgs e)
        {
            //Remove the custom type from the list and also from all blocks
            if (e.KeyCode == Keys.Delete) RemoveRole((string)mCustomRolesList.SelectedItem);
        }
        
        private void mRemove_Click(object sender, EventArgs e)
        {
            RemoveRole((string)mCustomRolesList.SelectedItem);
        }

        /// <summary>
        /// Press enter after typing to add a role to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mNewCustomRole_KeyUp(object sender, KeyEventArgs e)
        {
            //Add to the list of roles
            if (e.KeyCode == Keys.Enter) AddNewRole(mNewCustomRole.Text);
        }

        private void mAdd_Click(object sender, EventArgs e)
        {
            AddNewRole(mNewCustomRole.Text);
        }

        private void mOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void mCancel_Click(object sender, EventArgs e)
        {
            //undo anything we've done since this dialog has been open
            for (int i = 0; i < mNumberOfCommandsSinceOpened; i++) mPresentation.UndoRedoManager.undo();
            //end the dialog
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CustomRoles_Load(object sender, EventArgs e)
        {
            if (mPresentation.CustomClasses == null || mPresentation.CustomClasses.Count == 0) return;
            foreach (string customType in mPresentation.CustomClasses) mCustomRolesList.Items.Add(customType);
            mNewCustomRole.Focus();
        }
    }
}