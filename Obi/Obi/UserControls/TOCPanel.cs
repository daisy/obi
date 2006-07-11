using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.UserControls
{
    public partial class TOCPanel : UserControl
    {
        public event Events.Node.AddSiblingSectionHandler addSiblingSection;


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
        public void addNewSiblingHeading(CoreNode newNode, CoreNode relNode)
        {
            System.Windows.Forms.TreeNode relTreeNode = findTreeNodeFromCoreNode(relNode);

            System.Windows.Forms.TreeNode newTreeNode = new System.Windows.Forms.TreeNode(newNode
            
            string label = getCoreNodeText(newNode);

            relTreeNode.Parent.Nodes.Add(relNode.getProperty(typeof(urakawa.core.ChannelsProperty)).
        }

        public void addNewChildHeading(CoreNode newNode, CoreNode relNode)
        {
        }

        public void makeLabelEditableForHeadingInFocus()
        {
        }


        /*
         * you might move left if you go up and down
         * you won't move right
         */
        public void moveUp(urakawa.core.CoreNode node);
        public void moveDown(urakawa.core.CoreNode node);
        
        //always allowed until level 1
        public void decreaseLevel();
        //allowed if you have a previous sibling
        public void increaseLevel();
        public void limitView();
        public void expandView();

        /*
         * ***************************************
         * These functions "...ToolStripMenuItem_Click" are triggered
         * by the TOC panel's context menu
         */
        private void addSectionAtSameLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            urakawa.core.CoreNode selectedCoreNode;

            TreeNode sel = this.tocTree.SelectedNode;

            selectedCoreNode = (urakawa.core.CoreNode)sel.Tag;

            addSiblingSection(this, new Events.Node.AddSiblingSectionEventArgs(selectedCoreNode));
        }

        private void editLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        /********
         * helper functions
         */

        private string getCoreNodeText(CoreNode node)
        {
            Presentation pres;
            Channel stuff;
            
            ChannelsProperty channelsProp = node.getProperty(typeof(ChannelsProperty));

           //todo
            //get the channel that corresponds to the text label
        }

        private System.Windows.Forms.TreeNode getTreeNodeFromCoreNode(CoreNode node)
        {
        }

    }

  

}
