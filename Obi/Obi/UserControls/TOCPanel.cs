using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media;
using System.Collections;

namespace Obi.UserControls
{
	/// <summary>
    /// The TOCPanel is a view of the tree that displays the table of contents of the book as a tree widget.
    /// The user can easily see the structure of the book and edit the table of contents (add, remove, move,
    /// change the label, etc. of headings.)
    /// This control implements the CoreTreeView interface so that it can be synchronized with the core tree.
    /// </summary>
    public partial class TOCPanel : UserControl, urakawa.core.ICoreNodeVisitor
    {
        private ProjectPanel mProjectPanel; //the parent of this control

        public event Events.Node.SelectedHandler SelectedTreeNode;  // raised when selection changes (JQ)

       
        #region properties
        /// <summary>
        /// Test whether a node is currently selected or not.
        /// </summary>
        public bool Selected
        {
            get
            {
                return mTocTree.SelectedNode != null;
            }
        }

        /// <summary>
        /// Get and set the parent ProjectPanel control 
        /// </summary>
        // mg 20060804
        internal ProjectPanel ProjectPanel
        {
            get
            {
                return mProjectPanel;
            }
            set
            {
                mProjectPanel = value;
            }
        }

        #endregion

        #region instantiators

        public TOCPanel()
        {
            InitializeComponent();
            SelectedTreeNode += new Obi.Events.Node.SelectedHandler(TOCPanel_SelectedTreeNode);
            InitializeToolTips();
        }

        #endregion

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
            root.acceptDepthFirst(this);

            //select the first node
            if (mTocTree.Nodes.Count > 0)
            {
                mTocTree.SelectedNode = mTocTree.Nodes[0];
                mTocTree.SelectedNode.EnsureVisible();
            }

            this.Cursor = Cursors.Default;
        }
        
        #region Synchronization visitor

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <param name="node">The node to do nothing with.</param>
        public void postVisit(urakawa.core.ICoreNode node)
        {
        }

        /// <summary>
        /// Create a new tree node for every core node. Skip the root node, and attach the children of the root directly to the
        /// tree; the other children are attached to their parent node.
        /// Skip the phrase nodes as well (they do not have a text channel.)
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True </returns>
        public bool preVisit(urakawa.core.ICoreNode node)
        {
            if (Project.GetNodeType((urakawa.core.CoreNode)node) == NodeType.Section)
            {
                string label = Project.GetTextMedia((urakawa.core.CoreNode)node).getText();
                TreeNode newTreeNode;
                if (node.getParent().getParent() != null)
                {
                    TreeNode parentTreeNode = FindTreeNodeFromCoreNode((urakawa.core.CoreNode)node.getParent());
                    newTreeNode = parentTreeNode.Nodes.Add(node.GetHashCode().ToString(), label);
                }
                else
                {
                    // top-level nodes
                    newTreeNode = mTocTree.Nodes.Add(node.GetHashCode().ToString(), label);
                }
                newTreeNode.Tag = node;
                newTreeNode.ExpandAll();
                newTreeNode.EnsureVisible();
            }
            return true;
        }

        #endregion



        //md: do we still want this function?
        public void LimitViewToDepthOfCurrentSection()
        {
        }

        /// <summary>
        /// Show all the sections in the tree view.
        /// </summary>
        //md: there is no end-user command which exposes this feature
        public void ExpandViewToShowAllSections()
        {
            mTocTree.ExpandAll();
        }

        /// <summary>
        /// Return the core node version of the selected tree node.
        /// </summary>
        /// <returns>The selected section, or null if no section is selected.</returns>
        public urakawa.core.CoreNode GetSelectedSection()
        {
            TreeNode selected = this.mTocTree.SelectedNode;
            return selected == null ? null : (urakawa.core.CoreNode)selected.Tag;
        }

        /// <summary>
        /// Selects a node in the tree view.
        /// </summary>
        /// <param name="node">The core node version of the node to select.</param>
        /// <returns>true or false, depending on if the selection was successful</returns>
        public bool SetSelectedSection(urakawa.core.CoreNode node)
        {
            TreeNode sel = FindTreeNodeFromCoreNode(node);
            if (sel != null)
            {
                mTocTree.SelectedNode = sel;
                // set can move up, down, etc.
                Obi.Events.Node.SelectedEventArgs e = new Obi.Events.Node.SelectedEventArgs(true);
                SelectedTreeNode(this, e);
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
                else if (e.Label != Project.GetTextMedia((urakawa.core.CoreNode)e.Node.Tag).getText())
                {
                    RequestToRenameSectionNode(this, new Events.Node.RenameNodeEventArgs(this, (urakawa.core.CoreNode)e.Node.Tag, e.Label));
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

        // Added comments to remove warnings, should go away completely

        /// <summary>
        /// A new selection is made so the context menu is updated.
        /// </summary>
        private void mTocTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //mdXXX
            //return;

            //Events.Node.SelectedEventArgs _event = new Events.Node.SelectedEventArgs(true);
            // should set CanMoveUp, etc. here
            //SelectedTreeNode(this, _event);
        }

        /// <summary>
        /// When leaving the TOC tree, there is no selection anymore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTocTree_Leave(object sender, EventArgs e)
        {
            //mdXXX
            //return;

            //SelectedTreeNode(this, new Events.Node.SelectedEventArgs(false));
        }

#endregion
       
      
        #region helper functions
        /// <summary>
        /// helper function to get a channel based on its name
        /// </summary>
        /// <param name="node">the node (points to its own presentation)</param>
        /// <param name="channelName">the channel name</param>
        /// <returns></returns>
        private urakawa.core.Channel GetChannelByName(urakawa.core.CoreNode node, string channelName)
        {
            urakawa.core.ChannelsProperty channelsProp = 
                (urakawa.core.ChannelsProperty)node.getProperty(typeof(urakawa.core.ChannelsProperty));
            urakawa.core.Channel foundChannel = null;
            IList channelsList = channelsProp.getListOfUsedChannels();

            for (int i = 0; i < channelsList.Count; i++)
            {
                string name = ((urakawa.core.IChannel)channelsList[i]).getName();
                if (name == channelName)
                {
                    foundChannel = (urakawa.core.Channel)channelsList[i];
                    break;
                }
            }

            return foundChannel;
        }

        /// <summary>
        /// A helper function to get the <see cref="System.Windows.Forms.TreeNode"/>, given a 
        /// <see cref="CoreNode"/>.  
        /// The <see cref="TOCPanel"/> puts the value of <see cref="CoreNode.GetHashCode()"/> 
        /// into the <see cref="System.Windows.Forms.TreeNode"/> as a key value when it adds a 
        /// new node to the tree.  This function searches the tree view based on key values, and
        /// assumes that when they were generated, they came from <see cref="CoreNode.GetHashCode()"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private TreeNode FindTreeNodeFromCoreNode(urakawa.core.CoreNode node)
        {
            TreeNode foundNode = FindTreeNodeWithoutLabel(node);
            if (foundNode.Text != Project.GetTextMedia(node).getText())
            {
                throw new Exception(String.Format("Found tree node matching core node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
                    node.GetHashCode(), Project.GetTextMedia(node).getText(), foundNode.Text));
            }
            return foundNode;
        }

        /// <summary>
        /// Find a tree node for a core node, regardless of its label (used by rename.)
        /// </summary>
        /// <param name="node">The node to find.</param>
        /// <returns>The corresponding tree node.</returns>
        private TreeNode FindTreeNodeWithoutLabel(urakawa.core.CoreNode node)
        {
            TreeNode foundNode = null;
            TreeNode[] treeNodes
                = mTocTree.Nodes.Find(node.GetHashCode().ToString(), true);
          
            //since a key isn't unique and we get a list back from Nodes.Find,
            //try to be as sure as possible that it's the same node
            //however, this is questionably valuable as it will get more complicated
            //as text support improves and as multiple labels are supported on TOC items
            for (int i = 0; i < treeNodes.GetLength(0); i++)
            {
                //check the tag field only
                if (treeNodes[i].Tag == node)
                {
                    foundNode = treeNodes[i];
                    break;
                }
            }
            // The node must be found, so raise an exception if it couldn't
            if (foundNode == null)
            {
                throw new Exception(String.Format("Could not find tree node matching core node #{0} with label \"{1}\".",
                    node.GetHashCode(), Project.GetTextMedia(node).getText()));
            }
            return foundNode;
        }
        #endregion

        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mDeleteSectionToolStripMenuItem.Enabled = e.Selected;
            mEditLabelToolStripMenuItem.Enabled = e.Selected;

            //md: logic for these "canMove's" needs to come from Obi.Project
            //mMoveToolStripMenuItem.Enabled = e.CanMoveUp || e.CanMoveDown || e.CanMoveIn || e.CanMoveOut;
           // mMoveUpToolStripMenuItem.Enabled = e.CanMoveUp;
           // mMoveDownToolStripMenuItem.Enabled = e.CanMoveDown;
            mMoveInToolStripMenuItem.Enabled = e.CanMoveIn;
            mMoveOutToolStripMenuItem.Enabled = e.CanMoveOut;

            mShowInStripViewToolStripMenuItem.Enabled = e.Selected;
            mCutSectionToolStripMenuItem.Enabled = e.Selected;
            mCopySectionToolStripMenuItem.Enabled = e.Selected;
            // when closing, the project can be null but an event may still be generated
            // so be careful of checking the the project is not null in order to check
            // for its clipboard. (JQ)
            mPasteSectionToolStripMenuItem.Enabled = e.Selected &&
                (mProjectPanel.Project != null) && (mProjectPanel.Project.Clipboard != null);
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("toc_view_tooltip"));
            this.mToolTip.SetToolTip(this.mTocTree, Localizer.Message("toc_view_tooltip"));
        }
    }
}
