using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core;
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
    public partial class TOCPanel : UserControl, ICoreNodeVisitor
    {
        public event Events.Node.AddSiblingSectionHandler AddSiblingSection;
        public event Events.Node.AddChildSectionHandler AddChildSection;
        public event Events.Node.BeginEditingSectionHeadingLabelHandler BeginEditingLabel;
        public event Events.Node.DecreaseSectionLevelHandler DecreaseSectionLevel;
        public event Events.Node.IncreaseSectionLevelHandler IncreaseSectionLevel;
        public event Events.Node.LimitViewToSectionDepthHandler LimitDepthOfView;
        public event Events.Node.MoveSectionDownHandler MoveSectionDown;
        public event Events.Node.MoveSectionUpHandler MoveSectionUp;
        public event Events.Node.RenameSectionHandler RenameSection;
        public event Events.Node.DeleteSectionHandler DeleteSection;

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
        /// Synchronize the tree view with the core tree.
        /// Since we need priviledged access to the class for synchronization,
        /// we make it implement ICoreNodeVisitor directly.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
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
        public void postVisit(ICoreNode node)
        {
        }

        /// <summary>
        /// Create a new tree node for every core node. Skip the root node, and attach the children of the root directly to the
        /// tree; the other children are attached to their parent node.
        /// </summary>
        /// <param name="node">The node to add to the tree.</param>
        /// <returns>True </returns>
        public bool preVisit(ICoreNode node)
        {
            if (node.getParent() != null)
            {
                string label = Project.GetTextMedia((CoreNode)node).getText();
                System.Windows.Forms.TreeNode newTreeNode;
                if (node.getParent().getParent() != null)
                {
                    System.Windows.Forms.TreeNode parentTreeNode = findTreeNodeFromCoreNode((CoreNode)node.getParent());
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

        /// <summary>
        /// Add a new heading as an immediate sibling of the relative node.
        /// The new heading has already been created as a <see cref="CoreNode"/>.  Now we
        /// need to add it as a <see cref="System.Windows.Forms.TreeNode"/>.
        /// Internally, the new <see cref="System.Windows.Forms.TreeNode"/>
        /// is given the key of its <see cref="CoreNode"/> object's hash code.
        /// This makes it faster to find a <see cref="System.Windows.Forms.TreeNode"/> 
        /// based on a given <see cref="CoreNode"/>.
        /// A <see cref="CoreNode"/> can always be found from a <see cref="System.Windows.Forms.TreeNode"/>
        /// because the <see cref="System.Windows.Forms.TreeNode.Tag"/> field contains a reference to the <see cref="CoreNode"/>
        /// </summary>
        /// <param name="newNode">The new heading to add to the tree</param>
        /// <param name="relNode">The relative sibling node</param>
        /*public void AddNewSiblingSection(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(relNode);
            string label = GetTextMedia(newNode).getText();
            //add as a sibling
            System.Windows.Forms.TreeNodeCollection siblingCollection = null;
            if (relTreeNode.Parent != null)
            {
                siblingCollection = relTreeNode.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }
            System.Windows.Forms.TreeNode newTreeNode = 
                siblingCollection.Insert
                (relTreeNode.Index+1, newNode.GetHashCode().ToString(), label);
            newTreeNode.Tag = newNode;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
        }*/

        /// <summary>
        /// Add a new heading as a child of the relative node.
        /// The new heading has already been created as a <see cref="CoreNode"/>.  Now we
        /// need to add it as a <see cref="System.Windows.Forms.TreeNode"/>.
        /// Internally, the new <see cref="System.Windows.Forms.TreeNode"/>
        /// is given the key of its <see cref="CoreNode"/> object's hash code.
        /// This makes it faster to find a <see cref="System.Windows.Forms.TreeNode"/> 
        /// based on a given <see cref="CoreNode"/>.
        /// If the relative node is null, then the new node is created as a child of the
        /// presentation root.
        /// </summary>
        /// <param name="newNode">The new heading to add to the tree</param>
        /// <param name="relNode">The parent node for the new heading</param>
        /*public void AddNewChildSection(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode newTreeNode;
            string label = GetTextMedia(newNode).getText();
            if (relNode != null)
            {
                System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(relNode);
                newTreeNode = relTreeNode.Nodes.Add(newNode.GetHashCode().ToString(), label);
            }
            else
            {
                newTreeNode = tocTree.Nodes.Add(newNode.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = newNode;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
        }*/

        /// <summary>
        /// Delete a section from the table contents. The core node was removed from the core tree.
        /// </summary>
        /// <param name="node">The core node that was removed.</param>
        /*public void DeleteSectionNode(CoreNode node)
        {
            if (node != null)
            {
                System.Windows.Forms.TreeNode treeNode = findTreeNodeFromCoreNode(node);
                treeNode.Remove();
            }
        }*/

        /// <summary>
        /// Begin editing the label (activate the edit cursor) for the currently
        /// selected section heading node.
        /// </summary>
        /*public void BeginEditingNodeLabel(CoreNode node)
        {
            System.Windows.Forms.TreeNode treeNode = findTreeNodeFromCoreNode(node);
            treeNode.EnsureVisible();
            treeNode.BeginEdit();
        }*/

        /*
         * you might move left if you go up and down
         * you won't move right
         */
        public void MoveCurrentSectionUp()
        {
            System.Windows.Forms.TreeNode selected = this.tocTree.SelectedNode;
            System.Windows.Forms.TreeNode clone = (System.Windows.Forms.TreeNode)
                selected.Clone();
            System.Windows.Forms.TreeNodeCollection siblingCollection = null;


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

            if (siblingCollection != null)
            {
                //insert the clone at one above the node to be moved
                siblingCollection.Insert(selected.Index - 1, clone);

                //remove the node which was just moved
                selected.Remove();

                tocTree.SelectedNode = clone;

                clone.Expand();
            }
            
        }

        public void MoveCurrentSectionDown()
        {
            System.Windows.Forms.TreeNode selected = this.tocTree.SelectedNode;
            System.Windows.Forms.TreeNode clone = (System.Windows.Forms.TreeNode)
                selected.Clone();
            System.Windows.Forms.TreeNodeCollection siblingCollection = null;

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

            if (siblingCollection != null)
            {
                //insert the clone at one above the node to be moved
                siblingCollection.Insert(selected.Index + 2, clone);

                //remove the node which was just moved
                selected.Remove();

                tocTree.SelectedNode = clone;

                clone.Expand();
            }
        }
        
        //always allowed until level 1
        public void DecreaseCurrentSectionLevel()
        {
        }

        //allowed if you have a previous sibling
        public void IncreaseCurrentSectionLevel()
        {
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
        public CoreNode GetSelectedSection()
        {
            System.Windows.Forms.TreeNode selected = this.tocTree.SelectedNode;
            return selected == null ? null : (urakawa.core.CoreNode)selected.Tag;
        }

        /// <summary>
        /// Selects a node in the tree view.
        /// </summary>
        /// <param name="node">The core node version of the node to select.</param>
        /// <returns>true or false, depending on if the selection was successful</returns>
        public bool SetSelectedSection(CoreNode node)
        {
            System.Windows.Forms.TreeNode sel = findTreeNodeFromCoreNode(node);

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
            System.Diagnostics.Debug.WriteLine("TOC panel click!\n");

            AddSiblingSection(this,
                new Events.Node.AddSiblingSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "add sub-section" menu item.
        /// </summary>
        internal void addSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildSection(this,
                new Events.Node.AddChildSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "move section up" menu item.
        /// </summary>
        internal void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSectionUp(this, 
                new Events.Node.MoveSectionUpEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "delete section" menu item.
        /// </summary>
        internal void deleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSection(this, new Events.Node.DeleteSectionEventArgs(GetSelectedSection()));
        }

        private void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            sel.BeginEdit();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //!! uncomment this when the event is handled by the ProjectPanel (or whomever)
            //otherwise it crashes
            /*MoveSectionDown(this,
                new Events.Node.MoveSectionDownEventArgs(GetSelectedSection()));*/
            //this line will get deleted.  it's just for testing.
            this.MoveCurrentSectionDown();
        }

        private void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncreaseSectionLevel(this,
                new Events.Node.IncreaseSectionLevelEventArgs(GetSelectedSection()));
        }

        private void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
          sel.BeginEdit();

            DecreaseSectionLevel(this, 
                new Events.Node.DecreaseSectionLevelEventArgs(GetSelectedSection()));
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
                else if (e.Label != Project.GetTextMedia((CoreNode)e.Node.Tag).getText())
                {
                    RenameSection(this, new Events.Node.RenameSectionEventArgs((CoreNode)e.Node.Tag, e.Label));
                }
            }
        }

        /// <summary>
        /// helper function to get a channel based on its name
        /// </summary>
        /// <param name="node">the node (points to its own presentation)</param>
        /// <param name="channelName">the channel name</param>
        /// <returns></returns>
        private Channel GetChannelByName(CoreNode node, string channelName)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel foundChannel = null;
            IList channelsList = channelsProp.getListOfUsedChannels();
            
            for (int i = 0; i < channelsList.Count; i++)
            {
                string name = ((IChannel)channelsList[i]).getName();
                if (name == channelName)
                {
                    foundChannel = (Channel)channelsList[i];
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
        private System.Windows.Forms.TreeNode findTreeNodeFromCoreNode(CoreNode node)
        {
            System.Windows.Forms.TreeNode foundNode = null;
            System.Windows.Forms.TreeNode[] treeNodes 
                = tocTree.Nodes.Find(node.GetHashCode().ToString(), true);            
            //(please try to enjoy this long comment:)
            //since a key isn't unique and we get a list back from Nodes.Find,
            //try to be as sure as possible that it's the same node
            //however, this is questionably valuable as it will get more complicated
            //as text support improves and as multiple labels are supported on TOC items
            for (int i = 0; i < treeNodes.GetLength(0); i++)
            {
                //check the tag field and the text label
                if (treeNodes[i].Tag == node && treeNodes[i].Text == Project.GetTextMedia(node).getText())
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
        
        //trying to figure out which event to call when the tree gets right-clicked
        //it's really annoying to not have the node get selected when you right click it
        //because then, menu actions are applied to whichever node *is* selected
        //however, i'm not sure which function to use
        //and i'm not online to look it up right now
        private void tocTree_Click(object sender, EventArgs e)
        {
            
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
           
        }

        #region Sync event handlers

        /// <summary>
        /// Show the child node that was added in the tree (if it is a section.)
        /// If we were the ones to request its addition, also start editing its label right now.
        /// </summary>
        internal void SyncAddedChildNode(object sender, Events.Sync.AddedChildNodeEventArgs e)
        {
            System.Windows.Forms.TreeNode newTreeNode;
            string label = Project.GetTextMedia(e.Node).getText();

            System.Diagnostics.Debug.WriteLine("TocPanel: SyncAddedChildNode -- " + label);

            if (e.Node.getParent().getParent() != null)
            {
                System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode((CoreNode)e.Node.getParent());
                newTreeNode = relTreeNode.Nodes.Add(e.Node.GetHashCode().ToString(), label);
            }
            else
            {
                newTreeNode = tocTree.Nodes.Add(e.Node.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = e.Node;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
            newTreeNode.BeginEdit();
        }

        internal void SyncAddedSiblingNode(object sender, Events.Sync.AddedSiblingNodeEventArgs e)
        {
            
            System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(e.ContextNode);
            string label = Project.GetTextMedia(e.Node).getText();

            System.Diagnostics.Debug.WriteLine("TocPanel: SyncAddedSiblingNode -- " + label);

            //add as a sibling
            System.Windows.Forms.TreeNodeCollection siblingCollection = null;
            if (relTreeNode.Parent != null)
            {
                siblingCollection = relTreeNode.Parent.Nodes;
            }
            else
            {
                siblingCollection = tocTree.Nodes;
            }
            System.Windows.Forms.TreeNode newTreeNode =
                siblingCollection.Insert
                (relTreeNode.Index + 1, e.Node.GetHashCode().ToString(), label);
            newTreeNode.Tag = e.Node;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
            newTreeNode.BeginEdit();
        }

        internal void SyncRenamedNode(object sender, Events.Sync.RenamedNodeEventArgs e)
        {
            if (e.Origin != this)
            {
                System.Windows.Forms.TreeNode treeNode = findTreeNodeFromCoreNode(e.Node);
                treeNode.Text = e.Label;
            }
        }

        internal void SyncDeletedNode(object sender, Events.Sync.DeletedNodeEventArgs e)
        {
            if (e.Node != null)
            {
                System.Windows.Forms.TreeNode treeNode = findTreeNodeFromCoreNode(e.Node);
                treeNode.Remove();
            }    
        }

        #endregion
    }
}
