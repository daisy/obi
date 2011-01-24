using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Obi.ProjectView;

using urakawa.command;

namespace Obi.Dialogs
{
    public partial class EditRoles : Form
    {
        private ObiPresentation mPresentation;
        private ProjectView.ProjectView mProjectView;
        int mNumberOfCommandsSinceOpened;

        public EditRoles(ObiPresentation presentation, ProjectView.ProjectView projectView)
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
        if (!mCustomRolesList.Items.Contains ( role ))
            {
            mCustomRolesList.Items.Add ( role );
            Commands.AddCustomType cmd = new Obi.Commands.AddCustomType ( mProjectView, mPresentation, role );
            mPresentation.getUndoRedoManager ().execute ( cmd );
            mNumberOfCommandsSinceOpened++;
            }
            mNewCustomRole.Text = "";
        }

        private void RemoveRole(string role)
        {
        if (role != null &&  role != "" ) 
            {
                        mCustomRolesList.Items.Remove ( role );
            RemoveCustomTypeFromNodes ( role );
            }
        }

        /// <summary>
        /// Go through the tree and remove the given custom type from any node who has it
        /// </summary>
        private bool RemoveCustomTypeFromNodes(string customClass)
        {
            List<EmptyNode> nodes = mPresentation.NodesForCustomClass(customClass);
            if (nodes.Count == 0) return false;
            CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
            command.setShortDescription(Localizer.Message("remove_custom_class"));
            foreach (EmptyNode node in nodes)
                {
                if ( node != null )
                command.append ( new Commands.Node.AssignRole ( mProjectView, node, EmptyNode.Role.Plain ) );
                else
                command.append ( new Obi.Commands.RemoveCustomType ( mProjectView, mPresentation, customClass ) ) ;
                }
            mPresentation.getUndoRedoManager().execute(command);
            ++mNumberOfCommandsSinceOpened;
            return true;
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
        if (e.KeyCode == Keys.Enter)    AddNewRole ( mNewCustomRole.Text );
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
            for (int i = 0; i < mNumberOfCommandsSinceOpened; i++) mPresentation.getUndoRedoManager().undo();
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