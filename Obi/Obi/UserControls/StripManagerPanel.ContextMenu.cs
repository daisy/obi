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
            SectionNode node = mProjectPanel.CurrentSelectedStrip == null ?
                mProjectPanel.Project.CreateChildSectionNode(mProjectPanel.Project.RootNode) :
                mProjectPanel.Project.CreateSiblingSectionNode(mProjectPanel.CurrentSelectedStrip);
            mProjectPanel.CurrentSelection = new NodeSelection(node, this);
            StartRenamingSelectedStrip();
        }

        /// <summary>
        /// Handle both "rename strip" and "edit annotation" (sharing the same shortcut.)
        /// </summary>
        private void RenameStripOrEditAnnotation(object sender, EventArgs e)
        {
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                mSectionNodeMap[mProjectPanel.CurrentSelectedStrip].Renaming = true;
            }
            else if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                mPhraseNodeMap[mProjectPanel.CurrentSelectedAudioBlock].AnnotationBlock.Renaming = true;
            }
        }

        /// <summary>
        /// Start renaming the strip currently selected (if any.)
        /// </summary>
        public void StartRenamingSelectedStrip()
        {
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                mSectionNodeMap[mProjectPanel.CurrentSelectedStrip].Renaming = true;
            }
        }

        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        public void UpdateEnabledItemsForContextMenu()
        {
            bool isPlaying = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Playing;
            bool isPaused = mProjectPanel.TransportBar._CurrentPlaylist.State == Obi.Audio.AudioPlayerState.Paused;
            bool isStripSelected = SelectedSectionNode != null;
            bool isStripUsed = isStripSelected && SelectedSectionNode != null;
            bool isParentUsed = isStripSelected &&
                (SelectedSectionNode.ParentSection == null || SelectedSectionNode.ParentSection.Used);
            bool noSelection = mProjectPanel.CurrentSelection == null || mProjectPanel.CurrentSelection.Control != this;

            mInsertStripToolStripMenuItem.Enabled = !isPlaying && (noSelection || isStripUsed || isParentUsed);
            mRenameStripToolStripMenuItem.Enabled = !isPlaying && isStripUsed;

            bool canCutCopyDeleteSection = !isPlaying && isStripSelected && mProjectPanel.CurrentSelectionNode != null;
            bool canPasteSection = !isPlaying && mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Section);

            mCutStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mCopyStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mPasteStripToolStripMenuItem.Enabled = canPasteSection;
            mDeleteStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mMarkStripAsUnusedToolStripMenuItem.Enabled = !isPlaying && isStripSelected;
            mShowInTOCViewToolStripMenuItem.Enabled = !isPlaying && isStripSelected;

            bool isBlockSelected = SelectedPhraseNode != null;
            bool canCutCopyDeletePhrase = !isPlaying && isBlockSelected && mProjectPanel.CurrentSelectionNode != null;
            bool canPastePhrase = !isPlaying && mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Phrase);
            bool canMoveForward = !isPlaying &&
                mProjectPanel.Project.CanMovePhraseNode(mProjectPanel.CurrentSelectedAudioBlock, PhraseNode.Direction.Forward);
            bool canMoveBackward = !isPlaying &&
                mProjectPanel.Project.CanMovePhraseNode(mProjectPanel.CurrentSelectedAudioBlock, PhraseNode.Direction.Backward);

            mImportAudioFileToolStripMenuItem.Enabled = !isPlaying && !noSelection;
            mCutAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
            mCopyAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
            mPasteAudioBlockToolStripMenuItem.Enabled = canPastePhrase;
            mDeleteAudioBlockToolStripMenuItem.Enabled = canCutCopyDeletePhrase;
            mMarkPhraseAsUnusedToolStripMenuItem.Enabled = mProjectPanel.CanToggleAudioBlock;
            mMarkPhraseAsUnusedToolStripMenuItem.Text = mProjectPanel.ToggleAudioBlockString;
            mSplitAudioBlockToolStripMenuItem.Enabled = isBlockSelected;
            mQuickSplitAudioBlockToolStripMenuItem.Enabled = isBlockSelected && (isPlaying || isPaused);
            mApplyPhraseDetectionToolStripMenuItem.Enabled = !isPlaying && isBlockSelected;
            mMergeWithPreviousAudioBlockToolStripMenuItem.Enabled = !isPlaying && CanMerge;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = canMoveForward;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = canMoveBackward;
            mMoveAudioBlockToolStripMenuItem.Enabled = mMoveAudioBlockForwardToolStripMenuItem.Enabled ||
                mMoveAudioBlockBackwardToolStripMenuItem.Enabled;

            bool canRemoveAnnotation = !isPlaying && isBlockSelected && mProjectPanel.CurrentSelectedAudioBlock.HasAnnotation;
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
            // For some reason marking section is impossible if marking phrase is disabled :(
            if (mMarkStripAsUnusedToolStripMenuItem.Enabled) mMarkPhraseAsUnusedToolStripMenuItem.Enabled = true;
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
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                PhraseNode silence = mProjectPanel.Project.FindFirstPhrase();
                if (mProjectPanel.CurrentSelectedAudioBlock != silence)
                {
                    Dialogs.SentenceDetection dialog = new Dialogs.SentenceDetection(silence);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        mProjectPanel.Project.ApplyPhraseDetection(mProjectPanel.CurrentSelectedAudioBlock, dialog.Threshold, dialog.Gap,
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
        /// Edit the annotation for the selected audio block.
        /// </summary>
        /// <remarks>Start renaming process; the rest is handled by the annotation block itself.</remarks>
        public void EditAnnotationForSelectedAudioBlock()
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                mPhraseNodeMap[mProjectPanel.CurrentSelectedAudioBlock].AnnotationBlock.Renaming = true;
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
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                mProjectPanel.CurrentSelectedAudioBlock.Project.EditAnnotationPhraseNode(mProjectPanel.CurrentSelectedAudioBlock, "");
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
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                mPhraseNodeMap[mProjectPanel.CurrentSelectedAudioBlock].AnnotationBlock.FocusOnAnnotation();
            }
        }

        /// <summary>
        /// Move a block forward one spot in the strip, if it is not the last one.
        /// </summary>
        private void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null) 
            {
                mProjectPanel.Project.MovePhraseNode(mProjectPanel.CurrentSelectedAudioBlock, PhraseNode.Direction.Forward);
            }
        }

        /// <summary>
        /// Move a block backward one spot in the strip, if it is not the first one.
        /// </summary>
        //mg 20060813: made internal to allow obiform menu sync access
        internal void mMoveAudioBlockBackwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                mProjectPanel.Project.MovePhraseNode(mProjectPanel.CurrentSelectedAudioBlock, PhraseNode.Direction.Backward);
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
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                ProjectPanel.CurrentSelection = new NodeSelection(mProjectPanel.CurrentSelection.Node, mProjectPanel.TOCPanel);
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                ProjectPanel.TOCPanel.Focus();
            }
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

        /// <summary>
        /// Toggle the "used" status of the selected phrase.
        /// Do nothing if no phrase is selected.
        /// </summary>
        internal void ToggleSelectedAudioBlockUsed()
        {
            mProjectPanel.Project.ToggleNodeUsedWithCommand(SelectedPhraseNode, false);
        }

        private void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.CurrentSelectedAudioBlock != null) mProjectPanel.Project.SetPageNumberOnPhraseWithUndo(mProjectPanel.CurrentSelectedAudioBlock);
        }

        private void mGoTopageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.Project.Pages > 0) GoToPage();
        }

        #region ambiguous click event handlers

        /// <summary>
        /// Cut either the selected strip or the selected audio block.
        /// </summary>
        private void CutStripOrAudioBlockHandler(object sender, EventArgs e)
        {
            mProjectPanel.Cut();
        }

        /// <summary>
        /// Copy either the selected strip or the selected audio block.
        /// </summary>
        private void CopyStripOrAudioBlockHandler(object sender, EventArgs e)
        {
            mProjectPanel.Copy();
        }

        /// <summary>
        /// Paste the node in the clipboard in the selected strip or before the selected audio block.
        /// </summary>
        private void PasteStripOrAudioBlockHandler(object sender, EventArgs e)
        {
            mProjectPanel.Paste();
        }

        /// <summary>
        /// Delete either the selected strip or audio block.
        /// </summary>
        private void DeleteStripOrAudioBlockHandler(object sender, EventArgs e)
        {
            mProjectPanel.Delete();
        }

        /// <summary>
        /// Toggle the used state of either the selected strip or audio block.
        /// </summary>
        private void ToggleUsedStripOrAudioBlockHandler(object sender, EventArgs e)
        {
            if (mProjectPanel.CurrentSelectedStrip != null)
            {
                ToggleSelectedStripUsed();
            }
            else if (mProjectPanel.CurrentSelectedAudioBlock != null)
            {
                ToggleSelectedAudioBlockUsed();
            }
        }

        #endregion
    }
}
