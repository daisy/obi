using System;
using System.Collections;
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
    /// This control implements the CoreTreeView interface so that it can be synchronized with the core tree.
    /// </summary>
    public partial class TOCPanel : UserControl, IControlWithSelection
    {
        private ProjectPanel mProjectPanel; //the parent of this control


        #region IControlWithSelection Members

        public NodeSelection Selection
        {
            get
            {
                return null;
                /*TreeNode selected = mTocTree.SelectedNode;
                return selected == null ? null : (ObiNode)selected.Tag;*/
            }
            set
            {
                /*if ((mTocTree.SelectedNode == null && value != null) ||
                    (mTocTree.SelectedNode != null && value != mTocTree.SelectedNode.Tag))
                {
                    mTocTree.SelectedNode = value == null ? null : FindTreeNodeFromSectionNode((SectionNode)value);
                }*/
            }
        }

        #endregion

        /// <summary>
        /// True if a section is selected and it can be cut, copied or deleted.
        /// </summary>
        public bool CanCutCopyDelete
        {
            get
            {
                SectionNode selected = mProjectPanel.CurrentSelectedSection;
                return selected != null &&
                    (selected.Used || (selected.ParentAs<SectionNode>() == null || selected.ParentAs<SectionNode>().Used));
            }
        }

        /// <summary>
        /// True if there is a section in the clipboard that can be pasted in the selected section in the TOC panel.
        /// </summary>
        public bool CanPaste
        {
            get
            {
                SectionNode selected = mProjectPanel.CurrentSelectedSection;
                return selected != null ; //&& selected.Used && mProjectPanel.Project.Clipboard.Section != null;
            }
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

        public TOCPanel()
        {
            InitializeComponent();
            InitializeToolTips();
        }

        /// <summary>
        /// Synchronize the tree view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ITreeNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(urakawa.core.TreeNode root)
        {
            this.Cursor = Cursors.WaitCursor;
            mTocTree.Nodes.Clear();
            mTocTree.SelectedNode = null;
            root.acceptDepthFirst(SectionNodeVisitor, delegate(urakawa.core.TreeNode node) { });
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Create a new tree node for every core node. Skip the root node, and attach the children of the root directly to the
        /// tree; the other children are attached to their parent node.
        /// Skip the phrase nodes as well (they do not have a text channel.)
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True </returns>
        public bool SectionNodeVisitor(urakawa.core.TreeNode node)
        {
            urakawa.core.TreeNode _node = (urakawa.core.TreeNode)node;
            if (_node.GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                string label = Project.GetTextMedia(_node).getText();
                TreeNode newTreeNode;
                if (_node.getParent().getParent() != null)
                {
                    TreeNode parentTreeNode = FindTreeNodeFromSectionNode((SectionNode)_node.getParent());
                    newTreeNode = parentTreeNode.Nodes.Add(_node.GetHashCode().ToString(), label);
                }
                else
                {
                    // top-level nodes
                    newTreeNode = mTocTree.Nodes.Add(_node.GetHashCode().ToString(), label);
                }
                newTreeNode.Tag = _node;
                newTreeNode.ForeColor = ((SectionNode)node).Used ? Colors.SectionUsed : Colors.SectionUnused;
                newTreeNode.ExpandAll();
                newTreeNode.EnsureVisible();
            }
            return true;
        }

        #region toc tree event handlers

        /// <summary>
        /// Suspend playback and disallow "delete" shortcut while editing.
        /// </summary>
        private void mTocTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            mProjectPanel.TransportBar.Enabled = false;
            // AllowDelete = false;
        }

        /// <summary>
        /// The user has edited a label in the tree, so an event is raised to rename the node.
        /// </summary>
        private void mTocTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
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
                else if (e.Label != Project.GetTextMedia((SectionNode)e.Node.Tag).getText())
                {
                    mProjectPanel.Project.RenameSectionNodeWithCommand((SectionNode)e.Node.Tag, e.Label);
                }
            }
            // AllowDelete = true;
            mProjectPanel.TransportBar.Enabled = true;
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

        /// <summary>
        /// Catch selection event on a node
        /// </summary>
        private void mTocTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*if (mProjectPanel.CurrentSelectionNode != e.Node.Tag ||
                (mProjectPanel.CurrentSelection != null && mProjectPanel.CurrentSelection.Control != this))
            {
                System.Diagnostics.Debug.Print("{0} changes toc tree selection from {1} to {2}.",
                    sender, mProjectPanel.CurrentSelectionNode, e.Node.Tag);
                mProjectPanel.CurrentSelection = new NodeSelection((SectionNode)e.Node.Tag, this);
            }*/
        }

        #endregion
       
      
        #region helper functions
        /// <summary>
        /// helper function to get a channel based on its name
        /// </summary>
        /// <param name="node">the node (points to its own presentation)</param>
        /// <param name="channelName">the channel name</param>
        /// <returns></returns>
        private urakawa.property.channel.Channel GetChannelByName(urakawa.core.TreeNode node, string channelName)
        {
            urakawa.property.channel.ChannelsProperty channelsProp = 
                (urakawa.property.channel.ChannelsProperty)node.getProperty(typeof(urakawa.property.channel.ChannelsProperty));
            urakawa.property.channel.Channel foundChannel = null;
            IList channelsList = channelsProp.getListOfUsedChannels();

            for (int i = 0; i < channelsList.Count; i++)
            {
                string name = ((urakawa.property.channel.Channel)channelsList[i]).getName();
                if (name == channelName)
                {
                    foundChannel = (urakawa.property.channel.Channel)channelsList[i];
                    break;
                }
            }

            return foundChannel;
        }

        /// <summary>
        /// A helper function to get the <see cref="System.Windows.Forms.TreeNode"/>, given a 
        /// <see cref="SectionNode"/>.  
        /// The <see cref="TOCPanel"/> puts the value of <see cref="SectionNode.GetHashCode()"/> 
        /// into the <see cref="System.Windows.Forms.TreeNode"/> as a key value when it adds a 
        /// new node to the tree.  This function searches the tree view based on key values, and
        /// assumes that when they were generated, they came from <see cref="SectionNode.GetHashCode()"/>.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private TreeNode FindTreeNodeFromSectionNode(SectionNode node)
        {
            TreeNode foundNode = FindTreeNodeWithoutLabel(node);
            if (foundNode.Text != Project.GetTextMedia(node).getText())
            //if (foundNode.Text != node.TextMedia.getText())
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
        private TreeNode FindTreeNodeWithoutLabel(SectionNode node)
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

        /// <summary>
        /// Enable or disable tooltip.
        /// </summary>
        public bool EnableTooltips
        {
            set { mToolTip.Active = value; }
        }

        //md 20061009
        private void InitializeToolTips()
        {
            this.mToolTip.SetToolTip(this, Localizer.Message("toc_view_tooltip"));
            this.mToolTip.SetToolTip(this.mTocTree, Localizer.Message("toc_view_tooltip"));
        }

        /// <summary>
        /// Deselect the current section when focus leaves the window
        /// </summary>
        private void mTocTree_Leave(object sender, EventArgs e)
        {
            if (mTocTree.SelectedNode != null)
            {
                mTocTree.SelectedNode = null;
                mProjectPanel.CurrentSelection = null;
            }
        }
    }
}
