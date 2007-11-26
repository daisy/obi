using System;
using System.Drawing;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    /// <summary>
    /// The view of the table of contents, mostly a wrapper over a tree view.
    /// </summary>
    public partial class TOCView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;         // the parent project view
        private TreeNode mDummy;           // dummy section node (for selection; not used yet)
        private NodeSelection mSelection;  // actual selection context

        /// <summary>
        /// Create a new TOC view as part of a project view.
        /// </summary>
        public TOCView()
        {
            InitializeComponent();
            mDummy = null;
        }

        public ProjectView ProjectView { set { mView = value; } }

        // When we have a dummy section, then a section *must* be selected (?)
        public bool CanAddSection { get { return true; } }

        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionOut
        {
            get
            {
                return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode != mDummy &&
                    Commands.TOC.MoveSectionOut.CanMoveNode(mTOCTree.SelectedNode.Tag as SectionNode);
            }
        }

        /// <summary>
        /// True if there is a selected section and it can be moved out (i.e. decrease its level)
        /// </summary>
        public bool CanMoveSectionIn
        {
            get
            {
                return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode != mDummy &&
                    Commands.TOC.MoveSectionIn.CanMoveNode(mTOCTree.SelectedNode.Tag as SectionNode);
            }
        }

        /// <summary>
        /// True if the selected node can be removed (deleted or cut)
        /// </summary>
        public bool CanRemoveSection { get { return mSelection != null && mSelection.SectionOnly != null && !mSelection.IsDummy; } }

        /// <summary>
        /// True if the selected node can be renamed.
        /// </summary>
        public bool CanRenameSection { get { return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode != mDummy; } }

        /// <summary>
        /// True if the used state of the selected section can be changed
        /// </summary>
        public bool CanToggleSectionUsed
        {
            get
            {
                return mTOCTree.SelectedNode != null && mTOCTree.SelectedNode != mDummy &&
                    ((ObiNode)mTOCTree.SelectedNode.Tag).ParentAs<ObiNode>().Used;
            }
        }

        /// <summary>
        /// Make the tree node for this section visible.
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section)
        {
            FindTreeNode(section).EnsureVisible();
        }

        public void NewPresentation()
        {
            mTOCTree.Nodes.Clear();
            /*mDummy = new TreeNode(Localizer.Message("dummy_section"));
            mTOCTree.Nodes.Add(mDummy);
            mDummy.ForeColor = Color.LightGray;
            mDummy.Tag = mView.Project.RootNode;*/
            CreateTreeNodeForSectionNode(mView.Presentation.RootNode);
            mView.Presentation.treeNodeAdded += new TreeNodeAddedEventHandler(Presentation_treeNodeAdded);
            mView.Presentation.treeNodeRemoved += new TreeNodeRemovedEventHandler(Presentation_treeNodeRemoved);
            mView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Get or set the current selection.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != value)
                {
                    mSelection = value;
                    TreeNode n = value == null ? null : value.IsDummy ? mDummy : FindTreeNode((SectionNode)value.Node);
                    // ignore the select event, since we were asked to change the selection;
                    // but allow the selection not coming from the user
                    mTOCTree.AfterSelect -= new TreeViewEventHandler(mTOCTree_AfterSelect);
                    mTOCTree.BeforeSelect -= new TreeViewCancelEventHandler(mTOCTree_BeforeSelect);
                    // if (n != mDummy) UpdateDummyNode(n);  // TODO dummy node is not working yet
                    mTOCTree.SelectedNode = n;
                    // Show the strip for the selected section (use "as SectionNode" since the dummy under the root
                    // may be selected and there is no strip for it.)
                    if (n != null) mView.MakeStripVisibleForSection(n.Tag as SectionNode);
                    mTOCTree.AfterSelect += new TreeViewEventHandler(mTOCTree_AfterSelect);
                    mTOCTree.BeforeSelect += new TreeViewCancelEventHandler(mTOCTree_BeforeSelect);
                }
            }
        }

        /// <summary>
        /// Select a node in the TOC view.
        /// </summary>
        public void SelectNode(SectionNode section)
        {
            DoToNewNode(section, delegate()
            {
                mView.Selection = new NodeSelection(section, this, false);
            });
        }

        public void ResyncViews()
        {
            foreach (TreeNode n in mTOCTree.Nodes) ResyncViews(n);
        }

        private void ResyncViews(TreeNode n)
        {
            if (n.IsExpanded)
            {
                mView.SetStripsVisibilityForSection((SectionNode)n.Tag, true);
                foreach (TreeNode n_ in n.Nodes) ResyncViews(n_);
            }
            else
            {
                mView.SetStripsVisibilityForSection((SectionNode)n.Tag, false);
            }
        }

        // When a node was renamed, show the new name in the tree.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Node.Label;
            // not sure why this is necessary...
            if (mSelection != null) mSelection.Text = null;
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
            if (mSelection != null) mSelection.Text = null;
        }

        private void mTOCTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is SectionNode))
            {
                e.CancelEdit = true;
            }
            else if (mSelection != null)
            {
                mSelection.Text = e.Node.Text;
            }
        }

        // Pass a new selection to the main view.
        // Do not act on reselection of the same item to avoid infinite loops.
        private void mTOCTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            NodeSelection s = new NodeSelection((ObiNode)e.Node.Tag, this, e.Node == mDummy);
            if (s != mView.Selection) mView.Selection = s;
        }

        // Filter out unwanted tree selections (not caused by the user clicking, expanding, collapsing or keyboarding.)
        private void mTOCTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                System.Diagnostics.Debug.Print("!!! Cancelled selection in tree from `{0}'", sender);
                e.Cancel = true;
            }
        }

        // Show the dummy node under the selected section, or the root of the tree
        private void UpdateDummyNode(TreeNode n)
        {
            mDummy.Remove();
            if (n != null)
            {
                n.Nodes.Insert(0, mDummy);
            }
            else
            {
                mTOCTree.Nodes.Insert(0, mDummy);
            }
        }

        // Add new section nodes to the tree
        private void Presentation_treeNodeAdded(object o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                System.Diagnostics.Debug.Print("+++ new tree node!");
                // ignore the selection of the new tree node
                mTOCTree.AfterSelect -= new TreeViewEventHandler(mTOCTree_AfterSelect);
                CreateTreeNodeForSectionNode(e.getTreeNode() as SectionNode);
                mTOCTree.AfterSelect += new TreeViewEventHandler(mTOCTree_AfterSelect);
            }
        }

        // Create a new tree node for a section node and all of its descendants
        private void CreateTreeNodeForSectionNode(ObiNode node)
        {
            TreeNode n = AddSingleSectionNode(node);
            if (n != null)
            {
                n.EnsureVisible();
                n.ExpandAll();
                ChangeColorUsed(n, node.Used);
            }
            if (n != null || node is RootNode)
            {
                for (int i = 0; i < node.SectionChildCount; ++i) CreateTreeNodeForSectionNode(node.SectionChild(i));
            }
        }

        // Remove deleted section nodes from the tree
        void Presentation_treeNodeRemoved(object o, TreeNodeRemovedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null && IsInTree(section)) mTOCTree.Nodes.Remove(FindTreeNode(section));
        }

        // Check whether a node is in the tree view
        private bool IsInTree(SectionNode section)
        {
            if (section != null)
            {
                TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
                foreach (TreeNode n in nodes)
                {
                    if (n.Tag == section && n.Text == section.Label) return true;
                }
            }
            return false;
        }

        // Convenience method to add a new tree node for a section. Return the added tree node.
        private TreeNode AddSingleSectionNode(ObiNode node)
        {
            TreeNode n = null;
            if (node is SectionNode && node.IsRooted)
            {
                if (node.ParentAs<SectionNode>() != null)
                {
                    TreeNode p = FindTreeNode(node.ParentAs<SectionNode>());
                    n = p.Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                }
                else
                {
                    n = mTOCTree.Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                }
                n.Tag = node;
                ChangeColorUsed(n, node.Used);
            }
            return n;
        }

        // Node used status changed
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode && IsInTree((SectionNode)e.Node)) ChangeColorUsed(FindTreeNode((SectionNode)e.Node), e.Node.Used);
        }

        private void ChangeColorUsed(TreeNode n, bool used)
        {
            n.ForeColor = used ? Color.Black : Color.LightGray;
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
            return null;
            //throw new TreeNodeNotFoundException(
            //    String.Format("Could not find tree node matching section node #{0} with label \"{1}\".",
            //        section.GetHashCode(), Project.GetTextMedia(section).getText()));
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            if (IsInTree(section))
            {
                System.Diagnostics.Debug.Print("=== Doing to new node");
                f();
            }
            else
            {
                TreeNodeAddedEventHandler h = delegate(object o, TreeNodeAddedEventArgs e) { };
                h = delegate(object o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == section)
                    {
                        System.Diagnostics.Debug.Print("~~~ Doing to new node");
                        f();
                        mView.Presentation.treeNodeAdded -= h;
                    }
                };
                mView.Presentation.treeNodeAdded += h;
            }
        }

        private void mTOCTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            mView.SetStripsVisibilityForSection((SectionNode)e.Node.Tag, false);
        }

        private void mTOCTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            mView.SetStripsVisibilityForSection((SectionNode)e.Node.Tag, true);
        }

        #region IControlWithRenamableSelection Members

        public void SelectAndRename(ObiNode node)
        {
            DoToNewNode((SectionNode)node, delegate() { FindTreeNode((SectionNode)node).BeginEdit(); });
        }

        #endregion
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