using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media;
using System.Collections;

using Obi.Events.Node;


namespace Obi.UserControls
{
    public partial class TOCPanel
    {
        public event Events.SectionNodeHandler AddSiblingSectionRequested;
        public event Events.SectionNodeHandler AddChildSectionNodeRequested;
        public event Events.SectionNodeHandler DecreaseSectionNodeLevelRequested;
        public event Events.SectionNodeHandler IncreaseSectionNodeLevelRequested;
      
        public event Events.RenameSectionNodeHandler RenameSectionNodeRequested;
        public event Events.SectionNodeHandler DeleteSectionNodeRequested;
        public event Events.SectionNodeHandler CutSectionNodeRequested;
        public event Events.SectionNodeHandler CopySectionNodeRequested;
        public event Events.SectionNodeHandler PasteSectionNodeRequested;

        // These are internal so that the main menu can also link to them once the project is open.

        /// <summary>
        /// Triggered by the "add sibling section" menu item.
        /// </summary>
        internal void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSectionRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        /// <summary>
        /// Triggered by the "add sub-section" menu item.
        /// </summary>
        internal void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddChildSectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));  
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
            DeleteSectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        internal void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            sel.BeginEdit();
        }

        internal void increaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IncreaseSectionNodeLevelRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        internal void decreaseLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DecreaseSectionNodeLevelRequested(this, new SectionNodeEventArgs(this, SelectedSection));
       }

     
        /// <summary>
        /// If a node is selected, set focus on that node in the Strip view.
        /// </summary>
        //  mg20060804
        internal void mShowInStripViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mg 20060804
            if (IsNodeSelected)
            {
                ProjectPanel.StripManager.SelectedSectionNode = SelectedSection;
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
            CutSectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        //md 20060810
        private void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedSection();
        }

        public void CopySelectedSection()
        {
            CopySectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool isNodeSelected = false;
            bool canMoveIn = false;
            bool canMoveOut = false;

            SectionNode selectedSection = null;
            if (mTocTree.SelectedNode != null)
            {
                isNodeSelected = true;
                selectedSection = SelectedSection;
            }

            mAddSubSectionToolStripMenuItem.Enabled = isNodeSelected;
            mDeleteSectionToolStripMenuItem.Enabled = isNodeSelected;
            mEditLabelToolStripMenuItem.Enabled = isNodeSelected;

            if (isNodeSelected == true)
            {
                canMoveIn = mProjectPanel.Project.CanMoveSectionNodeIn(selectedSection);
                canMoveOut = mProjectPanel.Project.CanMoveSectionNodeOut(selectedSection);
            }
            
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
                (mProjectPanel.Project != null) && (mProjectPanel.Project.Clipboard.Section != null);
        }
    }
}
