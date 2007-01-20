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
        //md
        public event Events.SectionNodeHandler ShallowDeleteSectionNodeRequested;
       
        // public event Events.SectionNodeHandler CutSectionNodeRequested;
        // public event Events.PhraseNodeHandler CutPhraseNodeRequested;
        public event Events.PhraseNodeHandler CopyPhraseNodeRequested;
        public event Events.SectionNodeHandler CopySectionNodeRequested;
        public event Events.SectionNodeHandler PasteSectionNodeRequested;
        public event Events.NodeEventHandler PastePhraseNodeRequested;

        public Events.RequestToSetPageNumberHandler SetPageNumberRequested;
        public Events.PhraseNodeHandler RemovePageNumberRequested;

        public Events.RequestToApplyPhraseDetectionHandler ApplyPhraseDetectionRequested;

        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            UpdateEnabledItemsForContextMenu();
        }

        public void UpdateEnabledItemsForContextMenu()
        {
            bool isStripSelected = mSelectedSection != null;
            bool isAudioBlockSelected = mSelectedPhrase != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                mSelectedPhrase.Index == mSelectedPhrase.ParentSection.PhraseChildCount - 1;
            bool isAudioBlockFirst = isAudioBlockSelected && mSelectedPhrase.Index == 0;
            bool isBlockClipBoardSet = mProjectPanel.Project.Clipboard.Data != null;

            bool canSetPage = isAudioBlockSelected;  // an audio block must be selected and a heading must not be set.
            bool canRemovePage = isAudioBlockSelected;
            if (canRemovePage)
            {
                PageProperty pageProp = mSelectedPhrase.getProperty(typeof(PageProperty)) as PageProperty;
                canRemovePage = pageProp != null && pageProp.getOwner() != null;
            }
            bool canRemoveAnnotation = isAudioBlockSelected;

            mAddStripToolStripMenuItem.Enabled = true;

            bool canCutCopyDeleteSection = isStripSelected;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;
            mDeleteStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mCutStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mCopyStripToolStripMenuItem.Enabled = canCutCopyDeleteSection;
            mPasteStripToolStripMenuItem.Enabled = mProjectPanel.CanPaste(mProjectPanel.Project.Clipboard.Section);

            bool canInsertAudioBlock = CanInsertPhraseNode;
            mImportAudioFileToolStripMenuItem.Enabled = canInsertAudioBlock;
            mInsertEmptyAudioblockToolStripMenuItem.Enabled = canInsertAudioBlock;

            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mApplyPhraseDetectionToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithNextAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mCutAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mCopyAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mPasteAudioBlockToolStripMenuItem.Enabled = isBlockClipBoardSet && isStripSelected;
            mDeleteAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            mEditAnnotationToolStripMenuItem.Enabled = isAudioBlockSelected;
            mRemoveAnnotationToolStripMenuItem.Enabled = isAudioBlockSelected;

            mSetPageNumberToolStripMenuItem.Enabled = canSetPage;
            mRemovePageNumberToolStripMenuItem.Enabled = canRemoveAnnotation;

            mShowInTOCViewToolStripMenuItem.Enabled = isStripSelected;

            // Mark section used/unused (by default, i.e. if disabled, "unused")
            mMarkStripAsUnusedToolStripMenuItem.Enabled = isStripSelected;
            mMarkStripAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("all_phrases"), Localizer.Message(isStripSelected && !SelectedNode.Used ? "used" : "unused"));
            mMarkPhraseAsUnusedToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMarkPhraseAsUnusedToolStripMenuItem.Text = String.Format(Localizer.Message("mark_x_as_y"),
                Localizer.Message("phrase"), Localizer.Message(isAudioBlockSelected && !SelectedNode.Used ? "used" : "unused"));

            MakeEnabledMenuItemsVisible();
        }

        //md 20061219 
        /// <summary>
        /// make the context menu items visible based on if they are enabled or greyed-out
        /// </summary>
        // this is probably redundant, and could just be done instead of "enabling" things first
        // but that is for later.  still trying things out now.
        private void MakeEnabledMenuItemsVisible()
        {
            foreach (ToolStripItem item in this.contextMenuStrip1.Items)
            {
                item.Visible = item.Enabled;
            }
         
            //this is the separator that appears before the audio block commands
            toolStripSeparator1.Visible = mImportAudioFileToolStripMenuItem.Enabled || mCutAudioBlockToolStripMenuItem.Enabled;
            //and this one comes before the annotation commands
            toolStripSeparator3.Visible = mEditAnnotationToolStripMenuItem.Enabled;
            //this one before the show in TOC view option
            toolStripSeparator2.Visible = mShowInTOCViewToolStripMenuItem.Enabled;
            //this one before the page number commands
            toolStripSeparator4.Visible = mRemovePageNumberToolStripMenuItem.Enabled;

        }

        /// <summary>
        /// TODO:
        /// Adding a strip from the strip manager adds a new sibling strip right after the selected strip
        /// and reattaches the selected strip's children to the new strip. In effet, the new strip appears
        /// just below the selected strip.
        /// When no strip is selected, just add a new strip at the top of the tree.
        /// </summary>
        internal void mAddStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSiblingSectionRequested(this, new SectionNodeEventArgs(this, mSelectedSection));
        }

        internal void mRenameStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                mSectionNodeMap[mSelectedSection].StartRenaming();
            }
        }

        /// <summary>
        /// Brings up a file dialog and import one or more audio assets from selected files.
        /// The project is requested to create new blocks in the selected section, after the
        /// currently selected block (or at the end if no block is selected.)
        /// </summary>
        internal void mImportAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = Localizer.Message("audio_file_filter");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    int index = mSelectedPhrase == null ?
                        mSelectedSection.PhraseChildCount : mSelectedSection.indexOf(mSelectedPhrase) + 1;
                    foreach (string path in dialog.FileNames)
                    {
                        ImportAudioAssetRequested(this, new Events.Strip.ImportAssetEventArgs(mSelectedSection, path, index));
                        ++index;
                    }
                }
            }
        }

        /// <summary>
        /// Split the currently selected audio block.
        /// </summary>
        /// <remarks>JQ</remarks>
        internal void mSplitAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                // save the phrase at it gets lost when we stop (?)
                PhraseNode phrase = mSelectedPhrase;
                Audio.AudioPlayerState State = this.mProjectPanel.TransportBar.Playlist.State;
                double time = this.mProjectPanel.TransportBar.Playlist.CurrentTimeInAsset ;
                this.mProjectPanel.TransportBar.Playlist.Stop();
                Dialogs.Split dialog = new Dialogs.Split(phrase, time , State );
                if (dialog.ShowDialog() == DialogResult.OK)
                {
					SplitAudioBlockRequested(this, new Events.Node.SplitPhraseNodeEventArgs(this, phrase, dialog.ResultAsset));
                }
            }
        }

        /// <summary>
        /// Apply sentence detection on the currently selected phrase (unless it is the silence phrase.)
        /// </summary>
        internal void mApplyPhraseDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                PhraseNode silence = mProjectPanel.Project.FindFirstPhrase();
                if (mSelectedPhrase != silence)
                {
                    Dialogs.SentenceDetection dialog = new Dialogs.SentenceDetection(silence);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ApplyPhraseDetectionRequested(this, new Events.Node.PhraseDetectionEventArgs(this, mSelectedPhrase,
                            dialog.Threshold, dialog.Gap, dialog.LeadingSilence));
                    }
                }
            }
        }

        /// <summary>
        /// Merge the currently selected block with the following one (if there is such a block.)
        /// </summary>
        private void mMergeWithNextAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PhraseNode next = Project.GetNextPhrase(mSelectedPhrase);
            if (next != null)
            {
                MergeNodes(this, new Events.Node.MergeNodesEventArgs(this, mSelectedPhrase, next));
            }
        }

        /// <summary>
        /// Rename the currently selected audio block (JQ)
        /// </summary>
        internal void mEditAudioBlockLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].StartRenaming();
            }
        }

        /// <summary>
        /// Remove the annotation on a block.
        /// </summary>
        private void mRemoveAnnotationToolStripMenuItem_Click(object sender, EventArgs e)
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
            if (mSelectedPhrase != null) DeleteSelectedPhrase();
        }

        public void DeleteSelectedPhrase()
        {
            DeleteBlockRequested(this, new Events.Node.PhraseNodeEventArgs(this, mSelectedPhrase));
        }

        /// <summary>
        /// Move a block forward one spot in the strip, if it is not the last one.
        /// </summary>
        //mg 20060813: made internal to allow obiform menu sync access 
        internal void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                MoveAudioBlockForwardRequested(this, new Events.Node.PhraseNodeEventArgs(this, mSelectedPhrase));
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
                MoveAudioBlockBackwardRequested(this, new Events.Node.PhraseNodeEventArgs(this, mSelectedPhrase));
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
            if (mSelectedPhrase != null) CutSelectedPhrase();
        }

        /// <summary>
        /// Cut the phrase currently selected.
        /// </summary>
        public void CutSelectedPhrase()
        {
            // CutPhraseNodeRequested(this, new PhraseNodeEventArgs(this, mSelectedPhrase));
            mProjectPanel.Project.CutPhraseNode(mSelectedPhrase);
        }

        /// <summary>
        /// Cut the section currently selected.
        /// </summary>
        public void CutSelectedSection()
        {
            // CutSectionNodeRequested(this, new SectionNodeEventArgs(this, mSelectedSection));
            mProjectPanel.Project.ShallowCutSectionNode(mSelectedSection, true);
        }

        /// <summary>
        /// Copy the selected blockand store it in the block clip board.
        /// Actually nothing changes, but a command is still issued to undo (and retrieve the last value in the clipboard.)
        /// </summary>
        // JQ 20060816
        private void mCopyAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) CopySelectedPhrase();
        }

        /// <summary>
        /// Copy the phrase currently selected.
        /// </summary>
        public void CopySelectedPhrase()
        {
            CopyPhraseNodeRequested(this, new PhraseNodeEventArgs(this, mSelectedPhrase));
        }

        /// <summary>
        /// Copy the section currently selected.
        /// </summary>
        public void CopySelectedSection()
        {
            CopySectionNodeRequested(this, new SectionNodeEventArgs(this, mSelectedSection));
        }

        /// <summary>
        /// Paste the audio block in the clip board.
        /// </summary>
        // JQ 20060815
        private void mPasteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.Project.Clipboard.Phrase != null && mSelectedSection != null)
            {
                PastePhraseNode();
            }
        }

        /// <summary>
        /// Paste after the currently selected block, or at the end of the
        /// currently selected section if no block is selected.
        /// </summary>
        public void PastePhraseNode()
        {
            PastePhraseNodeRequested(this, new NodeEventArgs(this,
                mSelectedPhrase == null ? (CoreNode)mSelectedSection : (CoreNode)mSelectedPhrase));
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
        /// Add a page number to the synchronization strip.
        /// If there is already a page here, ask the user if she wants to replace it.
        /// </summary>
        // JQ 20060817
        internal void mSetPageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) mPhraseNodeMap[mSelectedPhrase].StartEditingPageNumber();
        }


        /// <summary>
        /// Remove a page number.
        /// </summary>
        internal void mRemovePageNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                PageProperty pageProp = mSelectedPhrase.getProperty(typeof(PageProperty)) as PageProperty;
                if (pageProp != null)
                {
                    RemovePageNumberRequested(this, new Events.Node.PhraseNodeEventArgs(sender, mSelectedPhrase));
                }
            }
        }

        /// <summary>
        /// Toggle the "used" status of the selected strip.
        /// Do nothing if no strip is selected.
        /// </summary>
        internal void ToggleSelectedStripUsed()
        {
            if (mProjectPanel.CanToggleStrip) mSelectedSection.Project.ToggleNodeUsed(mSelectedSection, this, false);
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
            if (mProjectPanel.CanToggleAudioBlock) mSelectedPhrase.Project.ToggleNodeUsed(mSelectedPhrase, this, false);
        }

        private void mMarkPhraseAsUnusedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleSelectedPhraseUsed();
        }

        private void mCutStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutSelectedSection();
        }


        private void mCopyStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedSection();
        }

        private void mPasteStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSectionNode();
        }

        

    }
}
