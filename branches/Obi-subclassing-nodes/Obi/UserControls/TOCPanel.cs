using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.media;

namespace Obi.UserControls
{
	/// <summary>
    /// The TOCPanel is a view of the tree that displays the table of contents of the book as a tree widget.
    /// The user can easily see the structure of the book and edit the table of contents (add, remove, move,
    /// change the label, etc. of headings.)
    /// </summary>
    public partial class TOCPanel : UserControl
    {
        private ProjectPanel mProjectPanel; //the parent of this control

        public event Events.Node.SelectedHandler SelectedTreeNode;  // raised when selection changes (JQ)

        #region properties

        /// <summary>
        /// Test whether a node is currently selected or not.
        /// </summary>
        public bool Selected
        {
            get { return mTocTree.SelectedNode != null; }
        }

        /// <summary>
        /// Get and set the parent ProjectPanel control 
        /// </summary>
        // mg 20060804
        internal ProjectPanel ProjectPanel
        {
            get { return mProjectPanel; }
            set { mProjectPanel = value; }
        }

        #endregion

        /// <summary>
        /// Create a new TOCPanel and initialize the event listener for node selection.
        /// </summary>
        public TOCPanel()
        {
            InitializeComponent();
            SelectedTreeNode += new Obi.Events.Node.SelectedHandler(TOCPanel_SelectedTreeNode);
        }

        /// <summary>
        /// Handle node selection events.
        /// </summary>
        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mDeleteSectionToolStripMenuItem.Enabled = e.Selected;
            mEditLabelToolStripMenuItem.Enabled = e.Selected;

            //md: logic for these "canMove's" needs to come from Obi.Project
            mMoveToolStripMenuItem.Enabled = e.CanMoveUp || e.CanMoveDown || e.CanMoveIn || e.CanMoveOut;
            mMoveUpToolStripMenuItem.Enabled = e.CanMoveUp;
            mMoveDownToolStripMenuItem.Enabled = e.CanMoveDown;
            mMoveInToolStripMenuItem.Enabled = e.CanMoveIn;
            mMoveOutToolStripMenuItem.Enabled = e.CanMoveOut;

            mShowInStripViewToolStripMenuItem.Enabled = e.Selected;
            mCutSectionToolStripMenuItem.Enabled = e.Selected;
            mCopySectionToolStripMenuItem.Enabled = e.Selected;
            // when closing, the project can be null but an event may still be generated
            // so be careful of checking the the project is not null in order to check
            // for its clipboard. (JQ)
            mPasteSectionToolStripMenuItem.Enabled = e.Selected &&
                (mProjectPanel.Project != null) && (mProjectPanel.Project.TOCClipboard != null);
        }

        /// <summary>
        /// Synchronize the tree view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(urakawa.core.CoreNode root)
        {
            this.Cursor = Cursors.WaitCursor;
            mTocTree.Nodes.Clear();
            mTocTree.SelectedNode = null;
            root.visitDepthFirst(MakeTreeNodeFromSectionNode, delegate(urakawa.core.ICoreNode n) {});            
            // select the first node if any
            if (mTocTree.Nodes.Count > 0)
            {
                mTocTree.SelectedNode = mTocTree.Nodes[0];
                mTocTree.SelectedNode.EnsureVisible();
            }
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Create a new tree node for every section node. Children of the root are created as "root nodes",
        /// other section nodes are attached to their parent widget node.
        /// This method is called by the syncrhonization visitor, going down the core tree.
        /// Note: we use the id of a section node as the key for the tree node; this id is unique.
        /// </summary>
        public bool MakeTreeNodeFromSectionNode(urakawa.core.ICoreNode node)
        {
            if (node.GetType() == typeof(SectionNode))
            {
                SectionNode _node = (SectionNode)node;
                TreeNode newTreeNode;
                if (node.getParent().getParent() != null)
                {
                    // not a child of the root, i.e. has a TreeNode parent
                    TreeNode parentTreeNode = FindTreeNodeFromSectionNode((SectionNode)node.getParent());
                    newTreeNode = parentTreeNode.Nodes.Add(_node.Id.ToString(), _node.Label);
                }
                else
                {
                    // root node (in the sense of widget tree nodes)
                    newTreeNode = mTocTree.Nodes.Add(_node.Id.ToString(), _node.Label);
                }
                newTreeNode.Tag = node;
                newTreeNode.ExpandAll();
                newTreeNode.EnsureVisible();
            }
            return true;
        }

        /// <summary>
        /// Find a tree node from a section node, making sure that labels match.
        /// </summary>
        private TreeNode FindTreeNodeFromSectionNode(SectionNode node)
        {
            TreeNode foundNode = FindTreeNodeWithoutLabel(node);
            if (foundNode.Text != node.Label)
            {
                throw new Exception(String.Format(
                    "Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\" instead.)",
                    node.Id, node.Label, foundNode.Text));
            }
            return foundNode;
        }

        /// <summary>
        /// Find a tree node for a section node, regardless of its label.
        /// The must be exactly one node for a given key (id.)
        /// </summary>
        private TreeNode FindTreeNodeWithoutLabel(SectionNode node)
        {
            TreeNode[] treeNodes = mTocTree.Nodes.Find(node.Id.ToString(), true);
            if (treeNodes.Length == 1)
            {
                return treeNodes[0];
            }
            else
            {
                throw new Exception(
                    String.Format("Could not find tree node matching sectoin node #{0} with label \"{1}\".",
                    node.Id.ToString(), node.Label));
            }
        }

        /// <summary>
        /// Return the core node version of the selected tree node.
        /// </summary>
        /// <returns>The selected section, or null if no section is selected.</returns>
        public SectionNode GetSelectedSection()
        {
            TreeNode selected = this.mTocTree.SelectedNode;
            return selected == null ? null : (SectionNode)selected.Tag;
        }

        /// <summary>
        /// Selects a node in the tree view.
        /// </summary>
        /// <param name="node">The core node version of the node to select.</param>
        /// <returns>true or false, depending on if the selection was successful</returns>
        public bool SetSelectedSection(SectionNode node)
        {
            TreeNode selected = FindTreeNodeFromSectionNode(node);
            if (selected != null)
            {
                mTocTree.SelectedNode = selected;
                SelectedTreeNode(this, new Obi.Events.Node.SelectedEventArgs(true));
                return true;
            }
            else
            {
                SelectedTreeNode(this, new Obi.Events.Node.SelectedEventArgs(false));
                return false;
            }
        }

        #region toc tree event handlers

        /// <summary>
        /// Using this event to assure that a node is selected. 
        /// </summary>
        //mg
        private void tocTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            this.mShowInStripViewToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// The user has edited a label in the tree, so an event is raised to rename the node.
        /// </summary>
        private void tocTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label == "")
                {
                    // Normally, the toolkit would cause an exception for an empty string;
                    // but it's easier to catch it here and cancel the event.
                    // In any case I am not sure that the behavior of the toolkit is good
                    // in this situation.
                    e.CancelEdit = true;
                    MessageBox.Show(Localizer.Message("empty_label_warning_text"),
                        Localizer.Message("empty_label_warning_caption"),
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    SectionNode node = (SectionNode)e.Node.Tag;
                    if (e.Label != node.Label)
                    {
                        RequestToRenameSectionNode(this, new Events.Node.Section.RenameEventArgs(this, node, e.Label));
                    }
                }
            }
        }

        /// <summary>
        /// select a node upon receiving a mouse-click (including right-clicks)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tocTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            mTocTree.SelectedNode = e.Node;
        }

        /// <summary>
        /// synchronize the highlight with the strip view on double-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>todo: this should override the default behavior (expand/collapse), 
        /// and, it's a bit weird to have the strip node rename-able upon select</remarks>
        //marisa added this 4 aug 06
        private void tocTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            this.mShowInStripViewToolStripMenuItem_Click(this, null);
        }

        /// <summary>
        /// synchronize the highlight with the strip view upon pressing enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //marisa added this 4 aug 06
        private void tocTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (mTocTree.SelectedNode != null)
                {
                    this.mShowInStripViewToolStripMenuItem_Click(this, null);
                }
            }

        }

        #endregion
    }
}
