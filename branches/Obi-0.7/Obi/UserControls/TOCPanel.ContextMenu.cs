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
    public partial class TOCPanel
    {
        public event Events.Node.RequestToAddSiblingNodeHandler RequestToAddSiblingSection;
        public event Events.Node.RequestToAddChildSectionNodeHandler RequestToAddChildSectionNode;
        public event Events.Node.RequestToDecreaseSectionNodeLevelHandler RequestToDecreaseSectionNodeLevel;
        public event Events.Node.RequestToIncreaseSectionNodeLevelHandler RequestToIncreaseSectionNodeLevel;
        public event Events.Node.RequestToMoveSectionNodeDownHandler RequestToMoveSectionNodeDown;
        public event Events.Node.RequestToMoveSectionNodeUpHandler RequestToMoveSectionNodeUp;
        public event Events.Node.RequestToRenameNodeHandler RequestToRenameSectionNode;
        public event Events.Node.RequestToDeleteNodeHandler RequestToDeleteSectionNode;

        //md: clipboard events
        public event Events.Node.RequestToCutSectionNodeHandler RequestToCutSectionNode;
        public event Events.Node.RequestToCopySectionNodeHandler RequestToCopySectionNode;
        public event Events.Node.RequestToPasteSectionNodeHandler RequestToPasteSectionNode;

        /*
         * ***************************************
         * These functions "...ToolStripMenuItem_Click" are triggered
         * by the TOC panel's context menu
         */

        // These are internal so that the main menu can also link to them once the project is open.

        /// <summary>
        /// Triggered by the "add sibling section" menu item.
        /// </summary>
        internal void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            RequestToAddSiblingSection(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
            
        }

        /// <summary>
        /// Triggered by the "add sub-section" menu item.
        /// </summary>
        internal void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            RequestToAddChildSectionNode(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
           
        }

        /// <summary>
        /// Triggered by the "move section up" menu item.
        /// </summary>
        internal void mMoveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            RequestToMoveSectionNodeUp(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
            
        }

        /// <summary>
        /// Triggered by the "delete section" menu item.
        /// </summary>
        private void mDeleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedSection();
        }

        public void DeleteSelectedSection()
        {
            RequestToDeleteSectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            sel.BeginEdit();
        }

        internal void mMoveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionNodeDown(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToIncreaseSectionNodeLevel(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToDecreaseSectionNodeLevel(this,
                new Events.Node.NodeEventArgs(this, GetSelectedSection()));
       }

     
        /// <summary>
        /// If a node is selected, set focus on that node in the Strip view.
        /// </summary>
        //  mg20060804
        internal void mShowInStripViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mg 20060804
            if (GetSelectedSection() != null)
            {
                ProjectPanel.StripManager.SelectedSectionNode = GetSelectedSection();
                if (ProjectPanel.StripManager.SelectedSectionNode != null)
                    ProjectPanel.StripManager.SelectedSectionStrip.Focus();
            }
        }

        private void mCutSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedSection();
        }

        /// <summary>
        /// Cut the selected section node.
        /// </summary>
        public void CutSelectedSection()
        {
            RequestToCutSectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        //md 20060810
        private void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedSection();
        }

        public void CopySelectedSection()
        {
            RequestToCopySectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToPasteSectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool isNodeSelected = false;
            bool canMoveUp = false;
            bool canMoveDown = false;
            bool canMoveIn = false;
            bool canMoveOut = false;

            urakawa.core.CoreNode selectedSection = null;
            if (mTocTree.SelectedNode != null)
            {
                isNodeSelected = true;
                selectedSection = GetSelectedSection();
            }


            mAddSubSectionToolStripMenuItem.Enabled = isNodeSelected;
            mDeleteSectionToolStripMenuItem.Enabled = isNodeSelected;
            mEditLabelToolStripMenuItem.Enabled = isNodeSelected;

            if (isNodeSelected == true)
            {
                canMoveUp = mProjectPanel.Project.CanMoveSectionNodeUp(selectedSection);
                canMoveDown = mProjectPanel.Project.CanMoveSectionNodeDown(selectedSection);
                canMoveIn = mProjectPanel.Project.CanMoveSectionNodeIn(selectedSection);
                canMoveOut = mProjectPanel.Project.CanMoveSectionNodeOut(selectedSection);
            }
            
           // mMoveToolStripMenuItem.Enabled = canMoveUp || canMoveDown || canMoveIn || canMoveOut;
           // mMoveUpToolStripMenuItem.Enabled = canMoveUp;
           // mMoveDownToolStripMenuItem.Enabled = canMoveDown;
            mMoveInToolStripMenuItem.Enabled = canMoveIn;
            mMoveOutToolStripMenuItem.Enabled = canMoveOut;

            mShowInStripViewToolStripMenuItem.Enabled = isNodeSelected;
            mCutSectionToolStripMenuItem.Enabled = isNodeSelected;
            mCopySectionToolStripMenuItem.Enabled = isNodeSelected;

            // when closing, the project can be null but an event may still be generated
            // so be careful of checking the the project is not null in order to check
            // for its clipboard. (JQ)
            // also, it doesn't matter if a node is selected since we can paste under the root node.
            mPasteSectionToolStripMenuItem.Enabled =
                (mProjectPanel.Project != null) && (mProjectPanel.Project.Clipboard != null);
        }
    }
}
