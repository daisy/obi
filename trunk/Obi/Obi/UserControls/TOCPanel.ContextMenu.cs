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
            mProjectPanel.Cut();
        }

        private void mCopySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Copy();
        }

        private void mPasteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Paste();
        }

        private void mDeleteSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Delete();
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
                //mProjectPanel.CurrentSelection = new NodeSelection(selected, mProjectPanel.StripManager);
            }
        }

        public void StartRenamingSelectedSection()
        {
            TreeNode sel = this.mTocTree.SelectedNode;
            if (sel != null) sel.BeginEdit();
        }

        public void UpdateEnabledItemsForContextMenu()
        {
            bool isPlayingOrRecording = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing || mProjectPanel.TransportBar.IsInlineRecording;
            bool isSelected = mProjectPanel.CurrentSelectedSection != null;
            bool isSelectedUsed = isSelected && mProjectPanel.CurrentSelectedSection.Used;
            bool isParentUsed = isSelected ?
                mProjectPanel.CurrentSelectedSection.ParentSection == null || mProjectPanel.CurrentSelectedSection.ParentSection.Used :
                false;

            mAddSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && (!isSelected || isSelectedUsed || isParentUsed);
            mAddSubSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSelectedUsed;
            mRenameSectionToolStripMenuItem.Enabled = !isPlayingOrRecording && isSelectedUsed;
            mMoveOutToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectPanel.Project.CanMoveSectionNodeOut(mProjectPanel.CurrentSelectedSection);
            mMoveInToolStripMenuItem.Enabled = !isPlayingOrRecording && mProjectPanel.Project.CanMoveSectionNodeIn(mProjectPanel.CurrentSelectedSection);

            bool canCutCopyDelete = !isPlayingOrRecording && isSelected && CanCutCopyDelete;
            bool canPaste = !isPlayingOrRecording && CanPaste;

            mCutSectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mCopySectionToolStripMenuItem.Enabled = canCutCopyDelete;
            mPasteSectionToolStripMenuItem.Enabled = canPaste;
            mDeleteSectionToolStripMenuItem.Enabled = canCutCopyDelete;

            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkSectionAsUnusedToolStripMenuItem.Enabled = !isPlayingOrRecording && isParentUsed;
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
