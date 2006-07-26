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
        public event Events.Node.RequestToAddSiblingNodeHandler RequestToAddSiblingSection;
        public event Events.Node.RequestToAddChildNodeHandler RequestToAddChildSection;
        public event Events.Node.RequestToDecreaseNodeLevelHandler RequestToDecreaseSectionLevel;
        public event Events.Node.RequestToIncreaseNodeLevelHandler RequestToIncreaseSectionLevel;
        public event Events.Node.RequestToMoveNodeDownHandler RequestToMoveSectionDown;
        public event Events.Node.RequestToMoveNodeUpHandler RequestToMoveSectionUp;
        public event Events.Node.RequestToRenameNodeHandler RequestToRenameSection;
        public event Events.Node.RequestToDeleteNodeHandler RequestToDeleteSection;
      
		/// <summary>
        /// Test whether a node is currently selected or not.
        /// </summary>
        public bool Selected
        {
            get
            {
                return tocTree.SelectedNode != null;
            }
        }

        /// <summary>
        /// Get the context menu strip of the tree view so that we can replicate it in the form.
        /// </summary>
        public ContextMenuStrip TocTreeContextMenuStrip
        {
            get
            {
                return tocTree.ContextMenuStrip;
            }
        }

        /// <summary>
        /// Synchronize the tree view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(urakawa.core.CoreNode root)
        {
            tocTree.Nodes.Clear();
            tocTree.SelectedNode = null;
            root.acceptDepthFirst(this);
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
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True </returns>
        public bool preVisit(urakawa.core.ICoreNode node)
        {
            if (node.getParent() != null)
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
                    newTreeNode = tocTree.Nodes.Add(node.GetHashCode().ToString(), label);
                }
                newTreeNode.Tag = node;
                newTreeNode.ExpandAll();
                newTreeNode.EnsureVisible();
            }
            return true;
        }

        #endregion

        /*
         * Some discussion points we made:
            1. Expand new nodes by default
            2. No image list
            3. Use a right click menu
            4. Use enter or double-click to load location
         *  5. if no node is selected, assume the last one is selected
         *  6. right-click should focus on the node under it
         *  7. if the tree is empty, the command text is "add heading"
         */
        public TOCPanel()
        {
            InitializeComponent();
        }

        public void LimitViewToDepthOfCurrentSection()
        {
        }

        /// <summary>
        /// Show all the sections in the tree view.
        /// </summary>
        public void ExpandViewToShowAllSections()
        {
            tocTree.ExpandAll();
        }

        /// <summary>
        /// Return the core node version of the selected tree node.
        /// </summary>
        /// <returns>The selected section, or null if no section is selected.</returns>
        public urakawa.core.CoreNode GetSelectedSection()
        {
            TreeNode selected = this.tocTree.SelectedNode;
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
                tocTree.SelectedNode = sel;
                return true;
            }
            else
            {
                return false;
            }
        }

        #region context menu handlers
        /*
         * ***************************************
         * These functions "...ToolStripMenuItem_Click" are triggered
         * by the TOC panel's context menu
         */

        // These are internal so that the main menu can also link to them once the project is open.

        /// <summary>
        /// Triggered by the "add sibling section" menu item.
        /// </summary>
        internal void addSectionAtSameLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToAddSiblingSection(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "add sub-section" menu item.
        /// </summary>
        internal void addSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToAddChildSection(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "move section up" menu item.
        /// </summary>
        internal void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionUp(this, 
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "delete section" menu item.
        /// </summary>
        internal void deleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToDeleteSection(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode sel = this.tocTree.SelectedNode;
            sel.BeginEdit();
        }

        internal void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionDown(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToIncreaseSectionLevel(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	RequestToDecreaseSectionLevel(this, 
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }
        #endregion

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
                    RequestToRenameSection(this, new Events.Node.RenameNodeEventArgs(this, (urakawa.core.CoreNode)e.Node.Tag, e.Label));
                }
            }
        }

       
        //trying to figure out which event to call when the tree gets right-clicked
        //it's really annoying to not have the node get selected when you right click it
        //because then, menu actions are applied to whichever node *is* selected
        //however, i'm not sure which function to use
        //and i'm not online to look it up right now
        private void tocTree_Click(object sender, EventArgs e)
        {

        }

        #region Sync event handlers

        /// <summary>
        /// Add a section to the tree view. If we were the ones to request its addition, 
        /// also start editing its label right now.
        ///
        /// The new heading has already been created as a <see cref="CoreNode"/>.  
        /// It is in its correct place in the core tree.  
        /// Now we need to add it as a <see cref="TreeNode"/> so 
        /// it shows up in the tree view. Internally, the new <see cref="TreeNode"/>
        /// is given the key of its <see cref="CoreNode"/> object's hash code.
        /// This makes it faster to find a <see cref="TreeNode"/> 
        /// based on a given <see cref="CoreNode"/>.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the new heading to add to the tree</param>
        
        internal void SyncAddedSectionNode(object sender, Events.Node.AddedSectionNodeEventArgs e)
        {
            TreeNode newTreeNode;
            string label = Project.GetTextMedia(e.Node).getText();
            if (e.Node.getParent().getParent() != null)
            {
                TreeNode relTreeNode = FindTreeNodeFromCoreNode((urakawa.core.CoreNode)e.Node.getParent());
                newTreeNode = relTreeNode.Nodes.Insert(e.Index, e.Node.GetHashCode().ToString(), label);
            }
            else
            {
                newTreeNode = tocTree.Nodes.Insert(e.Index, e.Node.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = e.Node;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
            //start editing if the request to add a node happened in the tree view
            if (e.Origin.Equals(this))
            {
                newTreeNode.BeginEdit();
            }
        }

        /// <summary>
        /// Change the label of the tree view node.
        /// This is in response to external renames (i.e. those not originating from within the tree view itself)
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the tree node being renamed.</param>
        internal void SyncRenamedNode(object sender, Events.Node.RenameNodeEventArgs e)
        {
            if (e.Origin != this)
            {
                TreeNode treeNode = FindTreeNodeWithoutLabel(e.Node);
                treeNode.Text = e.Label;
            }
        }

        /// <summary>
        /// Remove a node from the tree view.
        /// This will remove the whole subtree.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the node to be removed.</param>
        internal void SyncDeletedNode(object sender, Events.Node.NodeEventArgs e)
        {
            if (e.Node != null)
            {
                TreeNode treeNode = FindTreeNodeFromCoreNode(e.Node);
                treeNode.Remove();
            }    
        }

        /// <summary>
        /// This function deletes a node and promotes its children to be one level shallower
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncShallowDeletedNode(object sender, Events.Node.NodeEventArgs e)
        {
        }

        /// <summary>
        /// Move a tree node up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncMovedNodeUp(object sender, Events.Node.NodeEventArgs e)
        {
            System.Windows.Forms.TreeNode selected = FindTreeNodeFromCoreNode(e.Node);

            TreeNode clone = (TreeNode)selected.Clone();
            TreeNodeCollection siblingCollection = null;

            int newIndex = 0;

            if (selected.Parent != null)
            {
                siblingCollection = selected.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }

            //it is the first node in its list
            //change its level and move it to be the previous sibling of its parent
            if (selected.Index == 0)
            {
                if (selected.Parent != null)
                {
                    newIndex = selected.Parent.Index;

                    //it will be a sibling of its parent (soon to be former parent)
                    if (selected.Parent.Parent != null)
                    {
                        siblingCollection = selected.Parent.Parent.Nodes;
                    }
                    //it is moving to the outermost level
                    else
                    {
                        siblingCollection = tocTree.Nodes;
                    }
                }
                //else it has index = 0 and no parent 
                //so it's the first node in the tree and can't move up
            }
            else
            {
                newIndex = selected.Index - 1;
            }

            if (siblingCollection != null)
            {
                //insert the clone at one above the node to be moved
                siblingCollection.Insert(newIndex, clone);

                //remove the node which was just moved
                selected.Remove();

                tocTree.SelectedNode = clone;

                clone.Expand();
            }

        }

        /// <summary>
        /// Move a tree node down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncMovedNodeDown(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);
            TreeNode clone = (TreeNode)
                selected.Clone();
            TreeNodeCollection siblingCollection = null;

            int newIndex = 0;

            //get the set of sibling nodes that surround the selected node
            if (selected.Parent != null)
            {
                siblingCollection = selected.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }

            //if this is the last node in its collection
            if (selected.Index >= siblingCollection.Count - 1)
            {
                if (selected.Parent != null)
                {
                    newIndex = selected.Parent.Index + 1;

                    //move it out a level
                    if (selected.Parent.Parent != null)
                    {
                        siblingCollection = selected.Parent.Parent.Nodes;
                    }
                    else
                    {
                        siblingCollection = tocTree.Nodes;
                    }
                }
            }
            else
            {
                newIndex = selected.Index + 2;
            }

            if (siblingCollection != null)
            {
                //insert the clone at one above the node to be moved
                siblingCollection.Insert(newIndex, clone);

                //remove the node which was just moved
                selected.Remove();

                clone.ExpandAll();
                clone.EnsureVisible();
                tocTree.SelectedNode = clone;
            }
        }

        internal void SyncIncreasedNodeLevel(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);

            //you can always increase a node's level if it has an older sibling
            if (selected.Index == 0)
            {
                return;
            }

            TreeNode clone = (TreeNode)selected.Clone();
            TreeNodeCollection siblingCollection = null;

            TreeNode newParent = null;

            if (selected.Parent != null)
            {
                siblingCollection = selected.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }

            newParent = siblingCollection[selected.Index - 1];

            if (newParent != null)
            {
                //insert the clone as another child of its new parent
                newParent.Nodes.Add(clone);

                //remove the node which was just moved
                selected.Remove();

                tocTree.SelectedNode = clone;

                clone.ExpandAll();
                clone.EnsureVisible();
                tocTree.SelectedNode = clone;
            }
        }

        /// <summary>
        /// Decrease the node level.
        /// When a node becomes shallower, it adopts its former younger siblings.
        /// We'll have to get feedback on how users like this behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncDecreasedNodeLevel(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);

            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (selected.Parent == null)
            {
                return;
            }

            ArrayList futureChildren = new ArrayList();

            int idx = 0;
            //make copies of our future children, and remove them from the tree
            foreach (TreeNode node in selected.Parent.Nodes)
            {
                if (node.Index > selected.Index)
                {
                    futureChildren.Add(node.Clone());
                    node.Remove();
                    idx++;
                }
            }

            TreeNodeCollection siblingCollection = null;

            //move it out a level
            if (selected.Parent.Parent != null)
            {
                siblingCollection = selected.Parent.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }

            int newIndex = selected.Parent.Index + 1;

            TreeNode clone = 
                (TreeNode)selected.Clone();
            selected.Remove();

            siblingCollection.Insert(newIndex, clone);

            foreach (object node in futureChildren)
            {
                clone.Nodes.Add((TreeNode)node);
            }

            clone.ExpandAll();
            clone.EnsureVisible();
            tocTree.SelectedNode = clone;
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
                = tocTree.Nodes.Find(node.GetHashCode().ToString(), true);
          
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
    }
}
