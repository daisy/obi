using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using urakawa.core;
using urakawa.media;

using Obi.Events.Node;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        public event Events.SectionNodeHandler ShallowDeleteSectionNodeRequested;
        public event Events.SectionNodeHandler PasteSectionNodeRequested;

        public Events.RequestToSetPageNumberHandler SetPageNumberRequested;
        public Events.PhraseNodeHandler RemovePageNumberRequested;

        private void mContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            UpdateEnabledItemsForContextMenu();
        }

        private void mAddStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SectionNode node = mProjectPanel.Project.CreateSiblingSectionNode(mSelectedSection);
            SelectedSectionNode = node;
            StartRenamingSelectedStrip();
        }

        private void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartRenamingSelectedStrip();
        }

        /// <summary>
        /// Start renaming the strip currently selected (if any.)
        /// </summary>
        public void StartRenamingSelectedStrip()
        {
            if (mSelectedSection != null)
            {
                mSectionNodeMap[mSelectedSection].Renaming = true;
            }
        }

        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        public void UpdateEnabledItemsForContextMenu()
        {
            bool isPlaying = mProjectPanel.TransportBar.State == Obi.Audio.AudioPlayerState.Playing;
            bool isStripSelected = SelectedSectionNode != null;
            bool isStripUsed = isStripSelected && SelectedSectionNode.Used;
            bool isParentUsed = isStripSelected &&
                (SelectedSectionNode.ParentSection == null || SelectedSectionNode.ParentSection.Used);
            bool noSelection = SelectedNode == null;

            mAddStripToolStripMenuItem.Enabled = !isPlaying && (noSelection || isStripUsed || isParentUsed);
            mRenameStripToolStripMenuItem.Enabled = !isPlaying && isStripUsed;

            bool canCutCopyDeleteSection = !isPlaying && isStripSelected && mProjectPanel.CanCutCopyDeleteNode;
            bool canPasteSection = !isPlaying && mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Section);

            mCutStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mCopyStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mPasteStripToolStripMenuItem.Enabled = canPasteSection;
            mDeleteStripToolStripMenuItem.Enabled = canCutCopyDeleteSection && !mProjectPanel.EditingText;
            mMarkStripAsUnusedToolStripMenuItem.Enabled = !isPlaying && isStripSelected;
            mShowInTOCViewToolStripMenuItem.Enabled = !isPlaying && isStripSelected;

            bool isBlockSelected = SelectedPhraseNode != null;
            bool canCutCopyDeletePhrase = !isPlaying && isBlockSelected && mProjectPanel.CanCutCopyDeleteNode;
            bool canPastePhrase = !isPlaying && mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Phrase);
            bool canMoveForward = !isPlaying &&
                mProjectPanel.Project.CanMovePhraseNode(mSelectedPhrase, PhraseNode.Direction.Forward);
            bool canMoveBackward = !isPlaying &&
                mProjectPanel.Project.CanMovePhraseNode(mSelectedPhrase, PhraseNode.Direction.Backward);

            mImportAudioFileToolStripMenuItem.Enabled = !isPlaying && !noSelection;
            mInsertEmptyAudioblockToolStripMenuItem.Enabled = false; // !isPlaying && !noSelection
            mCutAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
            mCopyAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
            mPasteAudioBlockToolStripMenuItem.Enabled = canPastePhrase;
            mDeleteAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase && !mProjectPanel.EditingText;
            mMarkPhraseAsUnusedToolStripMenuItem.Enabled = false;
            mSplitAudioBlockToolStripMenuItem.Enabled = isBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isBlockSelected;
            mApplyPhraseDetectionToolStripMenuItem.Enabled = !isPlaying && isBlockSelected;
            mMergeWithPreviousAudioBlockToolStripMenuItem.Enabled = !isPlaying && CanMerge;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = canMoveForward;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = canMoveBackward;
            mMoveAudioBlockToolStripMenuItem.Enabled = mMoveAudioBlockForwardToolStripMenuItem.Enabled ||
                mMoveAudioBlockBackwardToolStripMenuItem.Enabled;

            bool canRemoveAnnotation = !isPlaying && isBlockSelected && mSelectedPhrase.HasAnnotation;
            mEditAnnotationToolStripMenuItem.Enabled = !isPlaying && isBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;

            mSetPageNumberToolStripMenuItem.Enabled = !isPlaying && CanSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = !isPlaying && CanRemovePage;

            UpdateVisibleItemsForContextMenu();
        }

        /// <summary>
        /// Make the context menu items visible only if they are enabled.
        /// Handle separators as well.
        /// </summary>
        private void UpdateVisibleItemsForContextMenu()
        {
            foreach (ToolStripItem item in this.mContextMenuStrip.Items)
            {
                item.Visible = item.Enabled;
            }
            ToolStripItem prev = null;
            ToolStripItem prevprev = null;
            foreach (ToolStripItem item in mContextMenuStrip.Items)
            {
                if (prev is ToolStripSeparator)
                {
                    prev.Visible = prevprev != null && prevprev.Enabled &&
                        !(item is ToolStripSeparator) && item.Enabled;
                }
                if ((!(item is ToolStripSeparator) && item.Enabled && (prev == null || prev is ToolStripSeparator)) ||
                    (item is ToolStripSeparator && !(prev is ToolStripSeparator) && prev != null && prev.Enabled))
                {
                    prevprev = prev;
                    prev = item;
                }
            }
        }

        private void mImportAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportPhrases();
        }

        private void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SplitBlock();
        }

        private void mQuickSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuickSplitBlock();
        }

        /// <summary>
        /// Apply sentence detection on the currently selected phrase (unless it is the silence phrase.)
        /// </summary>
        private void mApplyPhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyPhraseDetection();
        }

        public void ApplyPhraseDetection()
        {
            if (mSelectedPhrase != null)
            {
                PhraseNode silence = mProjectPanel.Project.FindFirstPhrase();
                if (mSelectedPhrase != silence)
                {
                    Dialogs.SentenceDetection dialog = new Dialogs.SentenceDetection(silence);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        mProjectPanel.Project.ApplyPhraseDetection(mSelectedPhrase, dialog.Threshold, dialog.Gap,
                            dialog.LeadingSilence);
                    }
                }
            }
        }

        private void mMergeWithPreviousAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeBlocks();
        }

        /// <summary>
        /// Rename the currently selected audio block (JQ)
        /// </summary>
        private void mEditAudioBlockLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditAnnotationForAudioBlock();
        }

        public void EditAnnotationForAudioBlock()
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].StartEditingAnnotation();
            }
        }

        /// <summary>
        /// Remove the annotation on a block.
        /// </summary>
        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAnnotationForAudioBlock();
        }

        public void RemoveAnnotationForAudioBlock()
        {
            if (mSelectedPhrase != null)
            {
                RemovedAnnotation(mPhraseNodeMap[mSelectedPhrase]);
            }
        }

        /// <summary>
        /// Delete the currently selected audio block.
        /// </summary>
        private void mDeleteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.DeletePhraseNode(mSelectedPhrase);
        }

        /// <summary>
        /// Move a block forward one spot in the strip, if it is not the last one.
        /// </summary>
        private void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) 
            {
                mProjectPanel.Project.MovePhraseNode(mSelectedPhrase, PhraseNode.Direction.Forward);
            }
        }

        /// <summary>
        /// Move a block backward one spot in the strip, if it is not the first one.
        /// </summary>
        //mg 20060813: made internal to allow obiform menu sync access
        internal void mMoveAudioBlockBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                mProjectPanel.Project.MovePhraseNode(mSelectedPhrase, PhraseNode.Direction.Backward);
            }
        }

        /// <summary>
        /// If a node is selected, set focus on that node in the TreeView.
        /// If the selected node is not a section node, move back to the
        /// section before commiting the select
        /// </summary>
        //  mg20060804
        internal void mShowInTOCViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                ProjectPanel.TOCPanel.SelectedSection = mSelectedSection;
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                ProjectPanel.TOCPanel.Focus();
            }
        }

        //md 20060812
        //shallow-delete a section node
        //mg 20060813: made internal to allow obiform menu sync access
        private void mDeleteStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedSection();
        }

        public void DeleteSelectedSection()
        {
            ShallowDeleteSectionNodeRequested(this, new SectionNodeEventArgs(this, this.mSelectedSection));
        }

        /// <summary>
        /// Cut the selected block and store it in the block clip board.
        /// </summary>
        //JQ 20060815
        private void mCutBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.CutPhraseNode(mSelectedPhrase);
        }

        /// <summary>
        /// Copy the selected blockand store it in the block clip board.
        /// Actually nothing changes, but a command is still issued to undo (and retrieve the last value in the clipboard.)
        /// </summary>
        // JQ 20060816
        private void mCopyAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) mProjectPanel.Project.CopyPhraseNode(mSelectedPhrase);
        }

        /// <summary>
        /// Paste the audio block in the clip board.
        /// </summary>
        private void mPasteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.PastePhraseNode(mProjectPanel.Project.Clipboard.Phrase, SelectedNode);
        }

        /// <summary>
        /// Paste a section.
        /// </summary>
        /// <remarks>TODO: find the right context node when none is selected.</remarks>
        public void PasteSectionNode()
        {
            // SectionNode contextNode = mSelectedSection;
            PasteSectionNodeRequested(this, new SectionNodeEventArgs(this, mSelectedSection));
        }

        /// <summary>
        /// Remove a page number.
        /// </summary>
        internal void mRemovePageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemovePageNumber();
        }

        /// <summary>
        /// Toggle the "used" status of the selected strip.
        /// Do nothing if no strip is selected.
        /// </summary>
        internal void ToggleSelectedStripUsed()
        {
            mProjectPanel.Project.ToggleNodeUsedWithCommand(SelectedSectionNode, false);
        }

        private void mMarkStripAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSelectedStripUsed();
        }

        /// <summary>
        /// Toggle the "used" status of the selected phrase.
        /// Do nothing if no phrase is selected.
        /// </summary>
        internal void ToggleSelectedPhraseUsed()
        {
            mProjectPanel.Project.ToggleNodeUsedWithCommand(SelectedPhraseNode, false);
        }

        private void mMarkPhraseAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSelectedPhraseUsed();
        }

        private void mCutStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.ShallowCutSectionNode(mSelectedSection);
        }

        private void mCopyStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProjectPanel.Project.ShallowCopySectionNode(mSelectedSection, true);
        }

        private void mPasteStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSectionNode();
        }
    }
}
