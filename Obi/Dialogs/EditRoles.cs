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
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Editing Custom Roles.htm");
            if (mProjectView.ObiForm.Settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(mProjectView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
         }

        private void AddNewRole(string role)
        {
        if (!mCustomRolesList.Items.Contains ( role ))
            {
            mCustomRolesList.Items.Add ( role );
            Commands.AddCustomType cmd = new Obi.Commands.AddCustomType ( mProjectView, mPresentation, role );
            mPresentation.UndoRedoManager.Execute ( cmd );
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
            CompositeCommand command = mPresentation.CommandFactory.CreateCompositeCommand();
            command.ShortDescription = Localizer.Message("remove_custom_class");
            foreach (EmptyNode node in nodes)
                {
                if ( node != null )
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssignRole ( mProjectView, node, EmptyNode.Role.Plain ) );
                else
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Obi.Commands.RemoveCustomType(mProjectView, mPresentation, customClass));
                }
            mPresentation.UndoRedoManager.Execute(command);
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
            for (int i = 0; i < mNumberOfCommandsSinceOpened; i++) mPresentation.UndoRedoManager.Undo();
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

        private void mCustomRolesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCustomRolesList.SelectedItem != null)
            {
                if (EmptyNode.SkippableNamesList.Contains(mCustomRolesList.SelectedItem.ToString()))
                    mRemove.Enabled = false;
                else
                    mRemove.Enabled = true;
            }
        }       


    }
}