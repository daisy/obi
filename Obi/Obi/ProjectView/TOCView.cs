using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    public partial class TOCView : UserControl
    {
        private Project mProject;

        public TOCView()
        {
            InitializeComponent();
            mProject = null;
        }

        public Project Project
        {
            set
            {
                mProject = value;
                mProject.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(TOCView_treeNodeAdded);
                mProject.RenamedSectionNode += new Obi.Events.RenameSectionNodeHandler(mProject_RenamedSectionNode);
            }
        }

        private void mProject_RenamedSectionNode(object sender, Obi.Events.Node.RenameSectionNodeEventArgs e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Label;
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void mTOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null && e.Label != "")
            {
                mProject.RenameSectionNode(e.Node.Tag as SectionNode, e.Label);
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        public SectionNode SelectedSection
        {
            get { return mTOCTree.SelectedNode == null ? null : mTOCTree.SelectedNode.Tag as SectionNode; }
            set { mTOCTree.SelectedNode = value == null ? null : FindTreeNode(value); }
        }

        public void StartRenaming(SectionNode sectionNode)
        {
            FindTreeNode(sectionNode).BeginEdit();
        }

        private void TOCView_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null)
            {
                TreeNode n = AddSingleSectionNode(section);
                n.ExpandAll();
                n.EnsureVisible();
            }
        }

        /// <summary>
        /// Add a new tree node for an empty section.
        /// </summary>
        /// <returns>The tree node created.</returns>
        private TreeNode AddSingleSectionNode(SectionNode section)
        {
            TreeNode n;
            if (section.ParentSection != null)
            {
                TreeNode p = FindTreeNode(section.ParentSection);
                n = p.Nodes.Insert(section.Index, section.GetHashCode().ToString(), section.Label);
            }
            else
            {
                n = mTOCTree.Nodes.Insert(section.Index, section.GetHashCode().ToString(), section.Label);
            }
            n.Tag = section;
            return n;
        }


        /// <summary>
        /// Find the tree node for a section node. The labels must also match.
        /// </summary>
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n.Text != section.Label)
            {
                throw new Exception(String.Format("Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
                    section.GetHashCode(), section.Label, n.Text));
            }
            return n;
        }

        /// <summary>
        /// Find a tree node for a section node, regardless of its label.
        /// The node that we are looking for must be present, but its label
        /// may not be the same.
        /// </summary>
        private TreeNode FindTreeNodeWithoutLabel(SectionNode section)
        {
            TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
            foreach (TreeNode n in nodes)
            {
                if (n.Tag == section) return n;
            }
            throw new Exception(String.Format("Could not find tree node matching section node #{0} with label \"{1}\".",
                    section.GetHashCode(), Project.GetTextMedia(section).getText()));
        }
    }
}
