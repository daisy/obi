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

        private void mInsertStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InsertStrip();
        }

        /// <summary>
        /// Insert a new strip for a section node, select it and rename it.
        /// </summary>
        public void InsertStrip()
        {
            if (mProjectPanel.Project != null)
            {
                SectionNode node = mProjectPanel.Project.CreateSiblingSectionNode(mSelectedSection);
                _SelectedSectionNode = node;
                StartRenamingSelectedStrip();
            }
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

            mInsertStripToolStripMenuItem.Enabled = !isPlaying && (noSelection || isStripUsed || isParentUsed);
            mRenameStripToolStripMenuItem.Enabled = !isPlaying && isStripUsed;

            bool canCutCopyDeleteSection = !isPlaying && isStripSelected && mProjectPanel.CanCutCopyDeleteNode;
            bool canPasteSection = !isPlaying && mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Section);

            mCutStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mCopyStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mPasteStripToolStripMenuItem.Enabled = canPasteSection;
            mDeleteStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
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
            mDeleteAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
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
            mFocusOnAnnotationToolStripMenuItem.Enabled = canRemoveAnnotation;

            mSetPageNumberToolStripMenuItem.Enabled = !isPlaying && CanSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = !isPlaying && CanRemovePage;
            mGoTopageToolStripMenuItem.Enabled = !isPlaying && mProjectPanel.Project.Pages > 0;

            UpdateVisibleItemsForContextMenu();
        }

        /// <summary>
        /// Make the context menu items visible only if they are enabled.
        /// Handle separators as well.
        /// </summary>
        private void UpdateVisibleItemsForContextMenu()
        {
            ToolStripItem lastVisible = null;
            foreach (ToolStripItem item in this.mContextMenuStrip.Items)
            {
                if (item is ToolStripSeparator)
                {
                    item.Enabled = lastVisible != null && !(lastVisible is ToolStripSeparator);
                }
                // I have no clue what's going on here... visible behaves strangely :(
                item.Visible = item.Enabled;
                if (item.Enabled) lastVisible = item;
            }
            if (lastVisible is ToolStripSeparator) lastVisible.Visible = false;
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

        private void mEditAudioBlockLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditAnnotationForAudioBlock();
        }

        /// <summary>
        /// Edit the annotation for the selected audio block.
        /// </summary>
        /// <remarks>Start renaming process; the rest is handled by the annotation block itself.</remarks>
        public void EditAnnotationForAudioBlock()
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].AnnotationBlock.Renaming = true;
            }
        }

        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAnnotationForAudioBlock();
        }

        /// <summary>
        /// Remove the annotation on a block.
        /// </summary>
        public void RemoveAnnotationForAudioBlock()
        {
            if (mSelectedPhrase != null)
            {
                mSelectedPhrase.Project.EditAnnotationPhraseNode(mSelectedPhrase, "");
            }
        }

        private void mFocusOnAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FocusOnAnnotation();
        }

        /// <summary>
        /// Focus on the (non-editable) text box of the annotation for the selected block, if it has one.
        /// </summary>
        public void FocusOnAnnotation()
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].AnnotationBlock.FocusOnAnnotation();
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

        private void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) mProjectPanel.Project.SetPageNumberOnPhraseWithUndo(mSelectedPhrase);
        }

        private void mGoTopageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.Project.Pages > 0) GoToPage();
        }
    }
}
