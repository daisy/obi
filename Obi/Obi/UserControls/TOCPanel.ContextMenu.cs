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
        internal void mDeleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void tESTShallowDeleteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SyncShallowDeletedSectionNode(this,
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

        //md 20060810
        internal void cutSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToCutSectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        //md 20060810
        internal void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToCopySectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToPasteSectionNode(this, new Events.Node.NodeEventArgs(this, GetSelectedSection()));
        }
    }
}
