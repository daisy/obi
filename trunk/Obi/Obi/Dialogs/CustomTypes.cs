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

        public CustomTypes(Presentation presentation, ProjectView.ProjectView projectView)
        {
            if (presentation == null) throw new Exception("Invalid presentation for custom types dialog");
            if (projectView == null) throw new Exception("Invalid project view for custom types dialog");
            InitializeComponent();
            mPresentation = presentation;
            mProjectView = projectView;
            foreach (string customType in mPresentation.CustomTypes)
            {
                mCustomTypesList.Items.Add(customType);
            }
        }

        private void mAddButton_Click(object sender, EventArgs e)
        {
            if (mPresentation != null) mPresentation.AddCustomType(mNewCustomType.Text);
            mCustomTypesList.Items.Add(mNewCustomType.Text);
        }

        private void mCustomClassesList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                string removedClass = (string)mCustomTypesList.SelectedItem;
                mPresentation.RemoveCustomType((string)mCustomTypesList.SelectedItem);
                mCustomTypesList.Items.RemoveAt(mCustomTypesList.SelectedIndex);
                RemoveCustomClassFromNodes(removedClass);
            }
        }

        private void RemoveCustomClassFromNodes(object removedClass)
        {
            /*
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
                        if (customKind == removedClass)
                        {
                            ((PhraseNode)n).CustomKind = "";
                            ((PhraseNode)n).PhraseKind = PhraseNode.Kind.Plain;
                            mProjectView.RemoveCustomKindLabelForBlock((PhraseNode)n);
                        }
                    }
                    return true;
                },
                // nothing to do in post-visit
                delegate(urakawa.core.TreeNode n) { }
            );*/
        }
    }
}