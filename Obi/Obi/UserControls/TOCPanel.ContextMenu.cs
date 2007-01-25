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
        // public event Events.SectionNodeHandler AddSiblingSectionRequested;
        // public event Events.SectionNodeHandler AddChildSectionNodeRequested;
        public event Events.SectionNodeHandler DecreaseSectionNodeLevelRequested;
        public event Events.SectionNodeHandler IncreaseSectionNodeLevelRequested;
        // public event Events.RenameSectionNodeHandler RenameSectionNodeRequested;
        // public event Events.SectionNodeHandler DeleteSectionNodeRequested;
        // public event Events.SectionNodeHandler CutSectionNodeRequested;
        // public event Events.SectionNodeHandler PasteSectionNodeRequested;

        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UpdateEnabledItemsForContextMenu();
        }

        private void mAddSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CreateSiblingSectionNode(SelectedSection);
        }

        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CreateChildSectionNode(SelectedSection);
        }

        private void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartRenamingSelectedSection();
        }

        public void StartRenamingSelectedSection()
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            if (sel != null) sel.BeginEdit();
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
            mProjectPanel.Project.DeleteSectionNode(SelectedSection);
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
            mProjectPanel.Project.CutSectionNode(SelectedSection);
        }

        private void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CopySectionNode(SelectedSection);
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.PasteSectionNode(SelectedSection);
        }

        public void UpdateEnabledItemsForContextMenu()
        {
            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;
            bool isSelected = SelectedSection != null;
            bool isSelectedUsed = isSelected && SelectedSection.Used;
            bool isParentUsed = isSelected ?
                SelectedSection.ParentSection == null || SelectedSection.ParentSection.Used :
                false;

            mAddSectionToolStripMenuItem.Enabled = !isPlaying && (!isSelected || isSelectedUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = isSelectedUsed;
            mRenameSectionToolStripMenuItem.Enabled = false;
            mMoveInToolStripMenuItem.Enabled = false;
            mMoveOutToolStripMenuItem.Enabled = false;

            bool canCutCopyDelete = !isPlaying && isSelected && CanCutCopyDelete;
            bool canPaste = !isPlaying && CanPaste;

            mCutSectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mCopySectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mPasteSectionToolStripMenuItem.Enabled = canPaste;
            mDeleteSectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = false;

            mShowInStripViewToolStripMenuItem.Enabled = isSelected;

            UpdateVisibleItemsForContextMenu();
        }

        private void UpdateVisibleItemsForContextMenu()
        {
            foreach (ToolStripItem item in mContextMenuStrip.Items)
            {
                item.Visible = item.Enabled;
            }
        }
    }
}
