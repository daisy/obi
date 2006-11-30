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
        public event Events.SectionNodeHandler RequestToShallowDeleteSectionNode;
        public event Events.SectionNodeHandler RequestToMoveSectionNodeDownLinear;
        public event Events.SectionNodeHandler RequestToMoveSectionNodeUpLinear;

        public event Events.SectionNodeHandler RequestToCutSectionNode;
        public event Events.PhraseNodeHandler RequestToCutPhraseNode;
        public event Events.SectionNodeHandler RequestToCopySectionNode;
        public event Events.PhraseNodeHandler RequestToCopyPhraseNode;
        public event Events.SectionNodeHandler RequestToPasteSectionNode;
        public event Events.PhraseNodeHandler RequestToPastePhraseNode;

        public Events.RequestToSetPageNumberHandler RequestToSetPageNumber;
        public Events.RequestToRemovePageNumberHandler RequestToRemovePageNumber;

        public Events.RequestToApplyPhraseDetectionHandler RequestToApplyPhraseDetection;

        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool isStripSelected = mSelectedSection != null;
            bool isAudioBlockSelected = mSelectedPhrase != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                Project.GetPhraseIndex(mSelectedPhrase) == Project.GetPhrasesCount(mSelectedSection) - 1;
            bool isAudioBlockFirst = isAudioBlockSelected && Project.GetPhraseIndex(mSelectedPhrase) == 0;
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
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;
            mDeleteStripToolStripMenuItem.Enabled = isStripSelected;

            mImportAudioFileToolStripMenuItem.Enabled = isStripSelected;
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
                        Project.GetPhrasesCount(mSelectedSection) : mSelectedSection.indexOf(mSelectedPhrase) + 1;
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
                Dialogs.Split dialog = new Dialogs.Split(mSelectedPhrase, 0.0);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SplitAudioBlockRequested(this, new Events.Node.SplitPhraseNodeEventArgs(this, mSelectedPhrase, dialog.ResultAsset));
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
                CoreNode silence = mProjectPanel.Project.FindFirstPhrase();
                if (mSelectedPhrase != silence)
                {
                    Dialogs.SentenceDetection dialog = new Dialogs.SentenceDetection(silence);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        RequestToApplyPhraseDetection(this, new Events.Node.PhraseDetectionEventArgs(this, mSelectedPhrase,
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
            CoreNode next = Project.GetNextPhrase(mSelectedPhrase);
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
        /// Delete the currently selected audio block.
        /// </summary>
        private void mDeleteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) DeleteSelectedPhrase();
        }

        public void DeleteSelectedPhrase()
        {
            DeleteBlockRequested(this, new Events.Node.NodeEventArgs(this, mSelectedPhrase));
        }

        /// <summary>
        /// Move a block forward one spot in the strip, if it is not the last one.
        /// </summary>
        //mg 20060813: made internal to allow obiform menu sync access 
        internal void mMoveAudioBlockForwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                MoveAudioBlockForwardRequested(this, new Events.Node.NodeEventArgs(this, mSelectedPhrase));
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
                MoveAudioBlockBackwardRequested(this, new Events.Node.NodeEventArgs(this, mSelectedPhrase));
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
            RequestToShallowDeleteSectionNode(this, new SectionNodeEventArgs(this, this.mSelectedSection));
        }

        //md 20060812
        //mg 20060813: made internal to allow obiform menu sync access
        internal void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionNodeUpLinear(this, new SectionNodeEventArgs(this, this.mSelectedSection));
        }

        //md 20060812
        //mg 20060813: made internal to allow obiform menu sync access
        internal void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionNodeDownLinear(this, new SectionNodeEventArgs(this, this.mSelectedSection));
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
            RequestToCutPhraseNode(this, new PhraseNodeEventArgs(this, mSelectedPhrase));
        }

        /// <summary>
        /// Cut the section currently selected.
        /// </summary>
        public void CutSelectedSection()
        {
            RequestToCutSectionNode(this, new SectionNodeEventArgs(this, mSelectedSection));
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
            RequestToCopyPhraseNode(this, new PhraseNodeEventArgs(this, mSelectedPhrase));
        }

        /// <summary>
        /// Copy the section currently selected.
        /// </summary>
        public void CopySelectedSection()
        {
            RequestToCopySectionNode(this, new SectionNodeEventArgs(this, mSelectedSection));
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
            RequestToPastePhraseNode(this, new PhraseNodeEventArgs(this,
                mSelectedPhrase == null ? mSelectedSection : mSelectedPhrase));
        }

        /// <summary>
        /// Paste a section.
        /// </summary>
        /// <remarks>TODO: find the right context node when none is selected.</remarks>
        public void PasteSectionNode()
        {
            SectionNode contextNode = mSelectedSection;
            // if contextNode == null...
            RequestToPasteSectionNode(this, new SectionNodeEventArgs(this, contextNode));
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
                    RequestToRemovePageNumber(this, new Events.Node.NodeEventArgs(sender, mSelectedPhrase));
                }
            }
        }
    }
}
