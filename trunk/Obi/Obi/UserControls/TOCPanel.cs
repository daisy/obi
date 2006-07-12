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
    public partial class TOCPanel : UserControl
    {
        public event Events.Node.AddSiblingSectionHandler AddSiblingSection;

        private string ObiTextChannelName = "Obi.Text";

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

        /*
         * ************************************
         * these functions are triggered from the outside
         * *************************************
        */
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
            //@todo
            //this adds it as the last child, so for us that's going to look like
            //the youngest sibling.  we need to add a middle sibling.
            System.Windows.Forms.TreeNode newTreeNode = 
                relTreeNode.Parent.Nodes.Add(newNode.GetHashCode().ToString(), label);
            
            newTreeNode.Tag = newNode;

            newTreeNode.ExpandAll();
        }

        /// <summary>
        /// Add a new heading as a child of the relative node.
        /// The new heading has already been created as a <see cref="CoreNode"/>.  Now we
        /// need to add it as a <see cref="System.Windows.Forms.TreeNode"/>.
        /// Internally, the new <see cref="System.Windows.Forms.TreeNode"/>
        /// is given the key of its <see cref="CoreNode"/> object's hash code.
        /// This makes it faster to find a <see cref="System.Windows.Forms.TreeNode"/> 
        /// based on a given <see cref="CoreNode"/>.
        /// </summary>
        /// <param name="newNode">The new heading to add to the tree</param>
        /// <param name="relNode">The parent node for the new heading</param>
        public void AddNewChildSection(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(relNode);
     
            string label = getCoreNodeText(newNode);

            //add as a child
            System.Windows.Forms.TreeNode newTreeNode = 
                relTreeNode.Nodes.Add(newNode.GetHashCode().ToString(), label);
            
            newTreeNode.Tag = newNode;

            newTreeNode.ExpandAll();
        }

        public void BeginEditingLabelForCurrentSectionHeading()
        {
            System.Windows.Forms.TreeNode sel = this.tocTree.SelectedNode;
            sel.BeginEdit();
        }


        /*
         * you might move left if you go up and down
         * you won't move right
         */
        public void MoveCurrentSectionUp(urakawa.core.CoreNode node)
        {
        }

        public void MoveCurrentSectionDown(urakawa.core.CoreNode node)
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

        public void ExpandViewToShowAllSections()
        {
            tocTree.ExpandAll();
        }

        /*
         * ***************************************
         * These functions "...ToolStripMenuItem_Click" are triggered
         * by the TOC panel's context menu
         */
        private void addSectionAtSameLevelToolStripMenuItem_Click(object sender, EventArgs e)
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

        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void addSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

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
            Channel textChannel;
            string textString = "";
            
            ChannelsProperty channelsProp = (ChannelsProperty)
                node.getProperty(typeof(ChannelsProperty));

            IList channelsList = channelsProp.getListOfUsedChannels();

            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();

                if (channelName == ObiTextChannelName)
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
