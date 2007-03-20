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
                SectionNode section = mProjectPanel.Project.CreateSiblingSectionNode(mProjectPanel.CurrentSelectedSection);
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
                SectionNode section = mProjectPanel.Project.CreateChildSectionNode(mProjectPanel.CurrentSelectedSection);
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
            mProjectPanel.Project.MoveSectionNodeOut(mProjectPanel.CurrentSelectedSection);
        }

        private void mMoveInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.MoveSectionNodeIn(mProjectPanel.CurrentSelectedSection);
        }

        private void mCutSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CutSectionNode(mProjectPanel.CurrentSelectedSection);
        }

        private void mCopySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CopySectionNode(mProjectPanel.CurrentSelectedSection);
        }

        internal void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.PasteSectionNode(mProjectPanel.CurrentSelectedSection);
        }

        private void mDeleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.DeleteSectionNode(mProjectPanel.CurrentSelectedSection);
        }

        private void mMarkSectionAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.ToggleNodeUsedWithCommand(mProjectPanel.CurrentSelectedSection, true);
        }

        private void mShowInStripViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSelectedSectionInStripView();
        }

        public void ShowSelectedSectionInStripView()
        {
            SectionNode selected = mProjectPanel.CurrentSelectedSection;
            if (selected != null)
            {
                mProjectPanel.CurrentSelection = new NodeSelection(selected, mProjectPanel.StripManager);
            }
        }

        public void StartRenamingSelectedSection()
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            if (sel != null) sel.BeginEdit();
        }

        public void UpdateEnabledItemsForContextMenu()
        {
            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
            bool isSelected = mProjectPanel.CurrentSelectedSection != null;
            bool isSelectedUsed = isSelected && mProjectPanel.CurrentSelectedSection.Used;
            bool isParentUsed = isSelected ?
                mProjectPanel.CurrentSelectedSection.ParentSection == null || mProjectPanel.CurrentSelectedSection.ParentSection.Used :
                false;

            mAddSectionToolStripMenuItem.Enabled = !isPlaying && (!isSelected || isSelectedUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlaying && isSelectedUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlaying && isSelectedUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.Project.CanMoveSectionNodeOut(mProjectPanel.CurrentSelectedSection);
            mMoveInToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.Project.CanMoveSectionNodeIn(mProjectPanel.CurrentSelectedSection);

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
