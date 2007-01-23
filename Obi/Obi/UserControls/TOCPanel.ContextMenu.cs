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
        // Should change all these to direct calls to the project
        public event Events.SectionNodeHandler AddSiblingSectionRequested;
        public event Events.SectionNodeHandler AddChildSectionNodeRequested;
        public event Events.SectionNodeHandler DecreaseSectionNodeLevelRequested;
        public event Events.SectionNodeHandler IncreaseSectionNodeLevelRequested;
        public event Events.RenameSectionNodeHandler RenameSectionNodeRequested;
        public event Events.SectionNodeHandler DeleteSectionNodeRequested;
        // public event Events.SectionNodeHandler CutSectionNodeRequested;
        public event Events.SectionNodeHandler PasteSectionNodeRequested;

        // These are internal so that the main menu can also link to them once the project is open.
        // Actual they should be private and have another functions called by the main menu.

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
        /// Triggered by the "mark section as used/unused" menu item.
        /// </summary>
        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSelectedSectionUsed();
        }

        /// <summary>
        /// Toggle the used property of the selected section.
        /// </summary>
        public void ToggleSelectedSectionUsed()
        {
            if (mProjectPanel.CanToggleSection) SelectedSection.Project.ToggleNodeUsed(SelectedSection, this, true);
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
        internal void mShowInStripViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsNodeSelected)
            {
                ProjectPanel.StripManager.SelectedSectionNode = SelectedSection;
                if (ProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    ProjectPanel.StripManager.SelectedSectionStrip.Focus();
                }
            }
        }

        private void mCutSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project._CutSectionNode(SelectedSection, true);
        }

        //md 20060810
        private void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedSection();
        }

        public void CopySelectedSection()
        {
            mProjectPanel.Project.CopySectionNode(SelectedSection, true);
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSectionNodeRequested(this, new SectionNodeEventArgs(this, SelectedSection));
        }

        /// <summary>
        /// Mark all items in the context menu as enabled or not depending on whether there is a current node
        /// selected, in use, etc.
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool isNodeSelected = mTocTree.SelectedNode != null;
            bool isNodeUsed = isNodeSelected && SelectedSection.Used;
            // Add section (after the current section) is disallowed if this would add *inside*
            // an unused section. So it is possible to add after an unused section, as long as
            // its parent is used.
            mAddSectionAtSameLevelToolStripMenuItem.Enabled = !isNodeSelected || isNodeUsed ||
                (SelectedSection.ParentSection != null && SelectedSection.ParentSection.Used);
            mAddSubSectionToolStripMenuItem.Enabled = isNodeUsed;
            mRenameSectionToolStripMenuItem.Enabled = isNodeUsed;
            mMoveInToolStripMenuItem.Enabled = isNodeUsed && mProjectPanel.Project.CanMoveSectionNodeIn(SelectedSection);
            mMoveOutToolStripMenuItem.Enabled = isNodeUsed && mProjectPanel.Project.CanMoveSectionNodeOut(SelectedSection);
            mCutSectionToolStripMenuItem.Enabled = isNodeUsed;
            mCopySectionToolStripMenuItem.Enabled = isNodeUsed;
            // When closing, the project can be null but an event may still be generated
            // so be careful of checking the the project is not null in order to check
            // for its clipboard. (JQ)
            // Also, it doesn't matter if a node is selected since we can paste under the root node.
            // But if a node is selected it must be used.
            mPasteSectionToolStripMenuItem.Enabled = (!isNodeSelected || isNodeUsed) &&
                (mProjectPanel.Project != null) && (mProjectPanel.Project.Clipboard.Section != null);
            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = isNodeSelected;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"), Localizer.Message(!isNodeSelected || isNodeUsed ? "unused" : "used"));
            mDeleteSectionToolStripMenuItem.Enabled = isNodeUsed;
            mShowInStripViewToolStripMenuItem.Enabled = isNodeSelected;
        }
    }
}
