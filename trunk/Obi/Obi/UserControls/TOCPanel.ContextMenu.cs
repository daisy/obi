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
        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UpdateEnabledItemsForContextMenu();
        }

        private void mInsertSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertSection();
        }

        /// <summary>
        /// Insert a new section in the project before the selected section
        /// (or at the end if none is selected) and start renaming it.
        /// </summary>
        public void InsertSection()
        {
            if (mProjectPanel.Project != null)
            {
                SectionNode section = mProjectPanel.Project.CreateSiblingSectionNode(SelectedSection);
                mTocTree.SelectedNode = FindTreeNodeFromSectionNode(section);
                StartRenamingSelectedSection();
            }
        }

        private void mAddSubSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSubSection();
        }

        public void AddSubSection()
        {
            if (mProjectPanel.Project != null)
            {
                SectionNode section = mProjectPanel.Project.CreateChildSectionNode(SelectedSection);
                mTocTree.SelectedNode = FindTreeNodeFromSectionNode(section);
                StartRenamingSelectedSection();
            }
        }

        private void mRenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartRenamingSelectedSection();
        }

        private void mMoveOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.MoveSectionNodeOut(SelectedSection);
        }

        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.MoveSectionNodeIn(SelectedSection);
        }

        private void mCutSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CutSectionNode(SelectedSection);
        }

        private void mCopySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CopySectionNode(SelectedSection);
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.PasteSectionNode(SelectedSection);
        }

        private void mDeleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.DeleteSectionNode(SelectedSection);
        }

        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.ToggleNodeUsedWithCommand(SelectedSection, true);
        }

        private void mShowInStripViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSelectedSectionInStripView();
        }

        public void ShowSelectedSectionInStripView()
        {
            if (IsNodeSelected)
            {
                ProjectPanel.StripManager.SelectedNode = SelectedSection;
                if (ProjectPanel.StripManager.SelectedSectionNode != null)
                {
                    ProjectPanel.StripManager.SelectedSectionStrip.Focus();
                }
            }
        }

        public void StartRenamingSelectedSection()
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            if (sel != null) sel.BeginEdit();
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
            mAddSubSectionToolStripMenuItem.Enabled = !isPlaying && isSelectedUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlaying && isSelectedUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.Project.CanMoveSectionNodeOut(SelectedSection);
            mMoveInToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.Project.CanMoveSectionNodeIn(SelectedSection);

            bool canCutCopyDelete = !isPlaying && isSelected && CanCutCopyDelete;
            bool canPaste = !isPlaying && CanPaste;

            mCutSectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mCopySectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mPasteSectionToolStripMenuItem.Enabled = canPaste;
            mDeleteSectionToolStripMenuItem.Enabled = canCutCopyDelete;

            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlaying && isParentUsed;
            mMarkSectionAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("section"),
                Localizer.Message(!isSelected || isSelectedUsed ? "unused" : "used"));

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
