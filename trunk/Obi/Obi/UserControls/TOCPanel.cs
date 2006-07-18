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
    public partial class TOCPanel : UserControl, ICoreTreeView, ICoreNodeVisitor
    {
        public event Events.Node.AddSiblingSectionHandler AddSiblingSection;
        public event Events.Node.AddChildSectionHandler AddChildSection;
        public event Events.Node.DeleteSectionHandler DeleteSection;
        public event Events.Node.BeginEditingSectionHeadingLabelHandler BeginEditingLabel;
        public event Events.Node.DecreaseSectionLevelHandler DecreaseSectionLevel;
        public event Events.Node.IncreaseSectionLevelHandler IncreaseSectionLevel;
        public event Events.Node.LimitViewToSectionDepthHandler LimitDepthOfView;
        public event Events.Node.MoveSectionDownHandler MoveSectionDown;
        public event Events.Node.MoveSectionUpHandler MoveSectionUp;

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
        /// Remove all nodes from the tree.
        /// </summary>
        public void Clear()
        {
            tocTree.Nodes.Clear();
            tocTree.SelectedNode = null;
        }

        /// <summary>
        /// Synchronize the tree view with the core tree.
        /// </summary>
        public void SynchronizeWithCoreTree(CoreNode root)
        {
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
                string label = GetTextMedia((CoreNode)node).getText();
                System.Windows.Forms.TreeNode newTreeNode;
                if (node.getParent().getParent() != null)
                {
                    System.Windows.Forms.TreeNode parentTreeNode = FindTreeNodeFromCoreNode((CoreNode)node.getParent());
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
        public void AddNewSiblingSection(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode newTreeNode;
            string label = GetTextMedia(newNode).getText();
            System.Windows.Forms.TreeNode relTreeNode = FindTreeNodeFromCoreNode(relNode);
            if (relTreeNode.Parent != null)
            {
                newTreeNode = relTreeNode.Parent.Nodes.Insert(relTreeNode.Index + 1, newNode.GetHashCode().ToString(), label);
            }
            else
            {
                newTreeNode = tocTree.Nodes.Add(newNode.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = newNode;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            tocTree.SelectedNode = newTreeNode;
        }

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
        public void AddNewChildSection(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode newTreeNode;
            string label = GetTextMedia(newNode).getText();
            if (relNode != null)
            {
                System.Windows.Forms.TreeNode relTreeNode = FindTreeNodeFromCoreNode(relNode);
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
        }

        /// <summary>
        /// Delete a section from the table contents. The core node was removed from the core tree.
        /// </summary>
        /// <param name="node">The core node that was removed.</param>
        public void DeleteSectionNode(CoreNode node)
        {
            if (node != null)
            {
                System.Windows.Forms.TreeNode treeNode = FindTreeNodeFromCoreNode(node);
                treeNode.Remove();
            }
        }

        /// <summary>
        /// Begin editing the label (activate the edit cursor) for the currently
        /// selected section heading node.
        /// </summary>
        public void BeginEditingNodeLabel(CoreNode node)
        {
            System.Windows.Forms.TreeNode treeNode = FindTreeNodeFromCoreNode(node);
            tocTree.SelectedNode = treeNode;
            treeNode.EnsureVisible();
            treeNode.BeginEdit();
        }


        /*
         * you might move left if you go up and down
         * you won't move right
         */
        public void MoveCurrentSectionUp()
        {
        }

        public void MoveCurrentSectionDown()
        {
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
            System.Windows.Forms.TreeNode sel = FindTreeNodeFromCoreNode(node);

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

        #region Event handlers

        /// <summary>
        /// Triggered by the "add child section" menu item.
        /// </summary>
        public void addChildSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildSection(this, new Events.Node.AddChildSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "add sibling section" menu item.
        /// </summary>
        public void addSectionAtSameLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSection(this, new Events.Node.AddSiblingSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Triggered by the "delete section" menu item.
        /// </summary>
        public void deleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSection(this, new Events.Node.DeleteSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// Update the text of the core node when we have edited a label in the tree.
        /// </summary>
        private void tocTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                CoreNode node = (CoreNode)e.Node.Tag;
                GetTextMedia(node).setText(e.Label);
            }
        }

        #endregion

        private void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            sel.BeginEdit();
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;
            MoveSectionUp(this, new Events.Node.MoveSectionUpEventArgs(selectedCoreNode));
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;
            MoveSectionDown(this, new Events.Node.MoveSectionDownEventArgs(selectedCoreNode));
        }

        private void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;
            IncreaseSectionLevel(this, new Events.Node.IncreaseSectionLevelEventArgs(selectedCoreNode));
        }

        private void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;
            DecreaseSectionLevel(this, new Events.Node.DecreaseSectionLevelEventArgs(selectedCoreNode));
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
        private System.Windows.Forms.TreeNode FindTreeNodeFromCoreNode(CoreNode node)
        {
            System.Windows.Forms.TreeNode foundNode = null;
            System.Windows.Forms.TreeNode[] treeNodes 
                = tocTree.Nodes.Find(node.GetHashCode().ToString(), true);
            // Make sure that we found the right node, especially since the hash value may not be unique.
            for (int i = 0; i < treeNodes.GetLength(0); i++)
            {
                //check the tag field and the text label
                if (treeNodes[i].Tag == node && treeNodes[i].Text == GetTextMedia(node).getText())
                {
                    foundNode = treeNodes[i];
                    break;
                }
            }
            // The node must be found, so raise an exception if it couldn't
            if (foundNode == null)
            {
                throw new Exception(String.Format("Could not find tree node matching core node #{0} with label \"{1}\".",
                    node.GetHashCode(), GetTextMedia(node).getText()));
            }
            return foundNode;
        }

        /// <summary>
        /// Get the text media of a core node. The result can then be used to get or set the text of a node.
        /// </summary>
        /// <param name="node">The node which text media we are interested in.</param>
        private static TextMedia GetTextMedia(CoreNode node)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel textChannel;
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();
                if (channelName == Project.TEXT_CHANNEL)
                {
                    textChannel = (Channel)channelsList[i];
                    return (TextMedia)channelsProp.getMedia(textChannel);
                }
            }
            return null;
        }
    }
}
