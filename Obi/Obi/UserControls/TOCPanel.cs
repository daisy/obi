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
    public partial class TOCPanel : UserControl, ICoreTreeView
    {
        public event Events.Node.AddSiblingSectionHandler AddSiblingSection;
        public event Events.Node.AddChildSectionHandler AddChildSection;
        public event Events.Node.BeginEditingSectionHeadingLabelHandler BeginEditingLabel;
        public event Events.Node.DecreaseSectionLevelHandler DecreaseSectionLevel;
        public event Events.Node.IncreaseSectionLevelHandler IncreaseSectionLevel;
        public event Events.Node.LimitViewToSectionDepthHandler LimitDepthOfView;
        public event Events.Node.MoveSectionDownHandler MoveSectionDown;
        public event Events.Node.MoveSectionUpHandler MoveSectionUp;

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
            System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(relNode);
     
            string label = getCoreNodeText(newNode);

            //add as a sibling
            System.Windows.Forms.TreeNode newTreeNode = 
                relTreeNode.Parent.Nodes.Insert
                (relTreeNode.Index+1, newNode.GetHashCode().ToString(), label);

            newTreeNode.Tag = newNode;

            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
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
            string label = getCoreNodeText(newNode);
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
        }

        /// <summary>
        /// Begin editing the label (activate the edit cursor) for the currently
        /// selected section heading node.
        /// </summary>
        public void BeginEditingNodeLabel(CoreNode node)
        {
            System.Windows.Forms.TreeNode treeNode = findTreeNodeFromCoreNode(node);
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
        public void addSectionAtSameLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;
            AddSiblingSection(this, new Events.Node.AddSiblingSectionEventArgs(selectedCoreNode));
        }

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

        public void addSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildSection(this, new Events.Node.AddChildSectionEventArgs(GetSelectedSection()));
        }

        /// <summary>
        /// A helper function to get the text from the given <see cref="CoreNode"/>.
        /// The text channel which contains the desired text will be named so that we know 
        /// what its purpose is (ie, "DefaultText" or "PrimaryText")
        /// @todo
        /// Otherwise we should use the default, only, or randomly first text channel found.
        /// </summary>
        /// <param name="node">The node whose text is to be retrieved</param>
        /// <returns>The text label.</returns>
        private string getCoreNodeText(CoreNode node)
        {
            string textString = "";
            ChannelsProperty channelsProp = (ChannelsProperty) node.getProperty(typeof(ChannelsProperty));
            Channel textChannel;
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();
                if (channelName == Project.TEXT_CHANNEL)
                {
                    textChannel = (Channel)channelsList[i];
                    TextMedia nodeText = (TextMedia)channelsProp.getMedia(textChannel);
                    textString = nodeText.getText();
                    break;
                }
            }
            return textString;
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

            
            //since a key isn't unique and we get a list back from Nodes.Find,
            //try to be as sure as possible that it's the same node
            for (int i = 0; i < treeNodes.GetLength(0); i++)
            {
                //check the tag field and the text label
                if (treeNodes[i].Tag == node && treeNodes[i].Text == getCoreNodeText(node))
                {
                    foundNode = treeNodes[i];
                    break;
                }
            }

            return foundNode;
        }


    }



}
