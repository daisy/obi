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
    public partial class TOCView : UserControl, IControlWithSelection
    {
        private ProjectView mView;  // the parent project view
        private TreeNode mDummy;    // dummy tree node (used for selection)

        /// <summary>
        /// Create a new TOC view as part of a project view
        /// </summary>
        public TOCView(ProjectView view) : this()
        {
            mView = view;
            mDummy = null;
        }

        // Used by the designer
        public TOCView() { InitializeComponent(); }


        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionOut
        {
            get
            {
                return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode.Tag is SectionNode &&
                    ((SectionNode)mTOCTree.SelectedNode.Tag).ParentSection != null;
            }
        }

        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionIn
        {
            get
            {
                return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode.Tag is SectionNode &&
                    ((SectionNode)mTOCTree.SelectedNode.Tag).Index > 0;
            }
        }

        /// <summary>
        /// Set a new project for this view.
        /// </summary>
        public void NewProject()
        {
            mView.Project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(TOCView_treeNodeAdded);
            mView.Project.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(TOCView_treeNodeRemoved);
            mView.Project.RenamedSectionNode += new Obi.Events.RenameSectionNodeHandler(Project_RenamedSectionNode);
            mTOCTree.Nodes.Clear();
        }

        /// <summary>
        /// Select a node in the TOC view and start its renaming.
        /// </summary>
        public void SelectAndRenameNode(SectionNode section)
        {
            DoToNewNode(section, delegate()
            {
                TreeNode n = FindTreeNode(section);
                n.BeginEdit();
                mView.Selection = new NodeSelection(section, this);
            });
        }

        /// <summary>
        /// Get or set the current selection. Make sure that the node is indeed in the tree.
        /// </summary>
        public ObiNode Selection
        {
            get
            {
                TreeNode selected = mTOCTree.SelectedNode;
                return selected == null ?   null :
                       selected == mDummy ? (ObiNode)selected.Parent.Tag : 
                                            (ObiNode)selected.Tag;
            }
            set
            {
                if ((mTOCTree.SelectedNode == null && value != null) ||
                    (mTOCTree.SelectedNode != null && value != mTOCTree.SelectedNode.Tag))
                {
                    TreeNode n = value == null ? null : FindTreeNode((SectionNode)value);
                    // ShowDummyNode(n);
                    mTOCTree.SelectedNode = n;
                }
            }
        }

        /// <summary>
        /// Get the selected section, or null if nothing is selected.
        /// </summary>
        public SectionNode SelectedSection
        {
            get { return Selection as SectionNode; }
        }

        /// <summary>
        /// Select a node in the TOC view.
        /// </summary>
        public void SelectNode(SectionNode section)
        {
            DoToNewNode(section, delegate() { mView.Selection = new NodeSelection(section, this); });
        }


        // When a node was renamed, show the new name in the tree.
        private void Project_RenamedSectionNode(object sender, Obi.Events.Node.RenameSectionNodeEventArgs e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Label;
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void mTOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag != null && e.Label != null && e.Label != "")
            {
                mView.RenameSectionNode((SectionNode)e.Node.Tag, e.Label);
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        // Do not allow editing of the dummy node
        private void mTOCTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is SectionNode)) e.CancelEdit = true;
        }

        // Pass a new selection to the main view.
        // Do not act on reselection of the same item to avoid infinite loops.
        private void mTOCTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != mView.SelectedSection) mView.Selection = new NodeSelection((ObiNode)e.Node.Tag, this);
        }

        // Add new section nodes to the tree
        private void TOCView_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            CreateTreeNodeForSectionNode(e.getTreeNode() as SectionNode);
        }

        private void CreateTreeNodeForSectionNode(SectionNode section)
        {
            TreeNode n = AddSingleSectionNode(section);
            n.ExpandAll();
            n.EnsureVisible();
            for (int i = 0; i < section.SectionChildCount; ++i) CreateTreeNodeForSectionNode(section.SectionChild(i));
        }

        // Remove deleted section nodes from the tree
        void TOCView_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null) mTOCTree.Nodes.Remove(FindTreeNode(section));
        }


        // Convenience method to add a new tree node for a section. Return the added tree node.
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


        // Create and insert a dummy node under the selected section, or at the root of the tree if nothing is selected.
        private void ShowDummyNode(TreeNode parent)
        {
            RemoveDummyNode();
            mDummy = parent == null ?
                mTOCTree.Nodes.Insert(0, Localizer.Message("dummy_section")) :
                parent.Nodes.Insert(0, Localizer.Message("dummy_section"));
            mDummy.ForeColor = Color.LightGray;
            mDummy.Tag = null;
            mDummy.EnsureVisible();
        }

        // Remove the dummy node
        private void RemoveDummyNode()
        {
            if (mDummy != null) mDummy.Remove();
            mDummy = null;
        }

        /// <summary>
        /// Find the tree node for a section node. The labels must also match.
        /// </summary>
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n.Text != section.Label)
            {
                throw new TreeNodeFoundWithWrongLabelException(
                    String.Format("Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
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
            throw new TreeNodeNotFoundException(
                String.Format("Could not find tree node matching section node #{0} with label \"{1}\".",
                    section.GetHashCode(), Project.GetTextMedia(section).getText()));
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            try
            {
                f();
            }
            catch (TreeNodeNotFoundException _e)
            {
                TreeNodeAddedEventHandler h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e) { };
                h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == section)
                    {
                        f();
                        mView.Project.getPresentation().treeNodeAdded -= h;
                    }
                };
                mView.Project.getPresentation().treeNodeAdded += h;
            }
        }
    }

    /// <summary>
    /// Raised when a tree node could not be found.
    /// </summary>
    public class TreeNodeNotFoundException : Exception
    {
        public TreeNodeNotFoundException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Raised when a tree node is found but with the wrong label.
    /// </summary>
    public class TreeNodeFoundWithWrongLabelException: Exception
    {
        public TreeNodeFoundWithWrongLabelException(string msg) : base(msg) { }
    }
}