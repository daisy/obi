using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace Obi.ProjectView
{
  
    /// <summary>
    /// The view of the table of contents, mostly a wrapper over a tree view.
    /// </summary>
    public partial class TOCView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;         // the parent project view
        private NodeSelection mSelection;  // actual selection context

        /// <summary>
        /// Create a new TOC view as part of a project view.
        /// </summary>
        public TOCView() { InitializeComponent(); }


        /// <summary>
        /// Can add a section as long as the selection is not a text selection.
        /// </summary>
        public bool CanAddSection { get { return !(mSelection is TextSelection); } }

        /// <summary>
        /// An actual section must be selected to be copied (i.e. not the text of the section.)
        /// </summary>
        public bool CanCopySection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if there is a selected section and its level can be increased (it must not be the first child.)
        /// </summary>
        public bool CanIncreaseLevel
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionIn.CanMoveNode((SectionNode)mSelection.Node); }
        }

        /// <summary>
        /// True if there is a selected section and its level can be decreased (it must not be a top-level section.)
        /// </summary>
        public bool CanDecreaseLevel
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionOut.CanMoveNode((SectionNode)mSelection.Node); }
        }

        /// <summary>
        /// True if the contents of the clipboard can be pasted before the selected section.
        /// </summary>
        public bool CanPasteBefore(Clipboard clipboard)
        {
            return mSelection != null && clipboard != null && clipboard.Node is SectionNode;
        }

        /// <summary>
        /// True if the contents of the clipboard can be pasted inside the selected section.
        /// </summary>
        public bool CanPasteInside(Clipboard clipboard)
        {
            return mSelection != null && clipboard != null && clipboard.Node is SectionNode;
        }

        /// <summary>
        /// True if the selected node can be removed (deleted or cut)
        /// </summary>
        public bool CanRemoveSection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if the selected node can be renamed.
        /// </summary>
        public bool CanRenameSection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if the used state of the selected section can be changed
        /// (a section is selected and its parent is used.)
        /// </summary>
        public bool CanSetSectionUsedStatus
        {
            get { return IsSectionSelected && mSelection.Node.ParentAs<ObiNode>().Used; }
        }

        /// <summary>
        /// Make the tree node for this section visible.
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section) { FindTreeNode(section).EnsureVisible(); }

        /// <summary>
        /// Resynchronize strips and TOC views depending on which node is visible.
        /// </summary>
        public void ResyncViews()
        {
            foreach (TreeNode n in mTOCTree.Nodes) SetStripsVisibilityForNode(n, true);
        }

        /// <summary>
        /// Get or set the current selection.
        /// Only the project view can set the selection.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != value)
                {
                    mSelection = value;
                    TreeNode n = value == null ? null : FindTreeNode((SectionNode)mSelection.Node);
                    // Ignore the select event, since we were asked to change the selection;
                    // but allow the selection not coming from the user.
                    mTOCTree.AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                    mTOCTree.BeforeSelect -= new TreeViewCancelEventHandler(TOCTree_BeforeSelect);
                    mTOCTree.SelectedNode = n;
                    if (n != null) mView.MakeStripVisibleForSection(n.Tag as SectionNode);
                    mTOCTree.AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
                    mTOCTree.BeforeSelect += new TreeViewCancelEventHandler(TOCTree_BeforeSelect);
                }
            }
        }

        /// <summary>
        /// Select and start renaming a section node.
        /// </summary>
        public void SelectAndRename(ObiNode node)
        {
            SectionNode section = (SectionNode)node;
            DoToNewNode(section, delegate()
            {
                mView.Selection = new TextSelection(section, this, section.Label);
                FindTreeNode(section).BeginEdit();
            });
        }

        /// <summary>
        /// Set a new presentation to be displayed.
        /// </summary>
        public void SetNewPresentation()
        {
            mTOCTree.Nodes.Clear();
            CreateTreeNodeForSectionNode(mView.Presentation.RootNode);
            mView.Presentation.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
            mView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Set the parent project view.
        /// </summary>
        public ProjectView ProjectView { set { mView = value; } }


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

        // Change the color of a node to reflect its used status
        private void ChangeColorUsed(TreeNode n, bool used)
        {
            n.ForeColor = used ? Color.Black : Color.LightGray;
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

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree; if it's not, set an event to look out for
        // its addition. This is used for naming new sections for instance.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            if (IsInTree(section))
            {
                f();
            }
            else
            {
                EventHandler<urakawa.events.DataModelChangedEventArgs> h =
                    delegate(object sender, urakawa.events.DataModelChangedEventArgs e) { };
                h = delegate(object sender, urakawa.events.DataModelChangedEventArgs e)
                {
                    if (e is urakawa.events.core.ChildAddedEventArgs &&
                        ((urakawa.events.core.ChildAddedEventArgs)e).AddedChild == section)
                    {
                        f();
                        mView.Presentation.changed -= h;
                    }
                };
                mView.Presentation.changed += h;
            }
        }

        /// <summary>
        /// Find the tree node for a section node. The labels must also match.
        /// </summary>
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n == null) throw new TreeNodeNotFoundException(String.Format(
                "Could not find tree node for section with label \"{0}\"", section.Label));
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
            foreach (TreeNode n in nodes) if (n.Tag == section) return n;
            return null;
        }

        // Check whether a node is in the tree view
        private bool IsInTree(SectionNode section)
        {
            if (section != null)
            {
                TreeNode[] nodes = mTOCTree.Nodes.Find(section.GetHashCode().ToString(), true);
                foreach (TreeNode n in nodes) if (n.Tag == section && n.Text == section.Label) return true;
            }
            return false;
        }

        // True if a section (not dummy, or not its text) is selected.
        private bool IsSectionSelected
        {
            get { return mSelection != null && mSelection.GetType() == typeof(NodeSelection); }
        }

        // Reflect changes in the presentation (added or deleted nodes)
        private void Presentation_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.core.ChildAddedEventArgs)
            {
                TreeNodeAdded((urakawa.events.core.ChildAddedEventArgs)e);
            }
            else if (e is urakawa.events.core.ChildRemovedEventArgs)
            {
                TreeNodeRemoved((urakawa.events.core.ChildRemovedEventArgs)e);
            }
        }

        // When a node was renamed, show the new name in the tree.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Node.Label;
        }

        // Add new section nodes to the tree
        private void TreeNodeAdded(urakawa.events.core.ChildAddedEventArgs e)
        {
            if (e.AddedChild is SectionNode)
            {
                // ignore the selection of the new tree node
                mTOCTree.AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                CreateTreeNodeForSectionNode((SectionNode)e.AddedChild);
                mTOCTree.AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
            }
        }

        // Node used status changed
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode && IsInTree((SectionNode)e.Node)) ChangeColorUsed(FindTreeNode((SectionNode)e.Node), e.Node.Used);
        }

        // Remove deleted section nodes from the tree
        void TreeNodeRemoved(urakawa.events.core.ChildRemovedEventArgs e)
        {
            if (e.RemovedChild is SectionNode) mTOCTree.Nodes.Remove(FindTreeNode((SectionNode)e.RemovedChild));
        }

        // Set the strips visibility for the given tree node according to expandednessity
        private void SetStripsVisibilityForNode(TreeNode node, bool visible)
        {
            mView.SetStripVisibilityForSection((SectionNode)node.Tag, visible);
            foreach (TreeNode n in node.Nodes) SetStripsVisibilityForNode(n, visible && node.IsExpanded);
        }

        // When a node is collapsed, hide strips corresponding to the collapsed nodes.
        private void TOCTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // When a node is expanded, make the strips reappear
        private void TOCTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void TOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag != null && e.Label != null && e.Label != "")
            {
                mView.RenameSectionNode((SectionNode)e.Node.Tag, e.Label);
            }
            else
            {
                e.CancelEdit = true;
                mView.Selection = new NodeSelection((SectionNode)e.Node.Tag, this);
            }
        }

        // Pass a new selection to the main view.
        // Do not act on reselection of the same item to avoid infinite loops.
        private void TOCTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            NodeSelection s = new NodeSelection((SectionNode)e.Node.Tag, this);
            if (s != mView.Selection) mView.Selection = s;
        }

        // Make a text selection in the view.
        private void TOCTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is SectionNode))
            {
                e.CancelEdit = true;
            }
            else if (mSelection != null)
            {
                mView.Selection = new TextSelection((SectionNode)e.Node.Tag, this, e.Node.Text);
            }
        }

        // Filter out unwanted tree selections (not caused by the user clicking, expanding, collapsing or keyboarding.)
        private void TOCTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) e.Cancel = true;
        }

        
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (KeyData == Keys.Tab
                && this.ActiveControl != null)
            {
                                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, true, true);
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else if (KeyData == (Keys)(Keys.Shift | Keys.Tab)
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, true, true);
                if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else
                return base.ProcessDialogKey(KeyData);
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