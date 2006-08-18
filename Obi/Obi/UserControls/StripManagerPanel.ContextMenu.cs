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

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        //md
        public event Events.Node.RequestToShallowDeleteSectionNodeHandler RequestToShallowDeleteSectionNode;
        public event Events.Node.RequestToMoveSectionNodeDownLinearHandler RequestToMoveSectionNodeDownLinear;
        public event Events.Node.RequestToMoveSectionNodeUpLinearHandler RequestToMoveSectionNodeUpLinear;

        public event Events.Node.RequestToCutPhraseNodeHandler RequestToCutPhraseNode;
        public event Events.Node.RequestToCopyPhraseNodeHandler RequestToCopyPhraseNode;
        public event Events.Node.RequestToPastePhraseNodeHandler RequestToPastePhraseNode;

        public Events.Node.RequestToSetPageLabelHandler RequestToSetPageLabel;
        public Events.Node.RequestToRemovePageLabelHandler RequestToRemovePageLabel;
        
        /// <summary>
        /// Enable/disable items depending on what is currently available.
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            bool isStripSelected = mSelectedSection != null;
            bool canMoveUp = isStripSelected && mProjectPanel.Project.CanMoveSectionNodeUp(mSelectedSection);
            bool canMoveDown = isStripSelected && mProjectPanel.Project.CanMoveSectionNodeDown(mSelectedSection);
            bool isAudioBlockSelected = mSelectedPhrase != null;
            bool isAudioBlockLast = isAudioBlockSelected &&
                Project.GetPhraseIndex(mSelectedPhrase) == Project.GetPhrasesCount(mSelectedSection) - 1;
            bool isAudioBlockFirst = isAudioBlockSelected && Project.GetPhraseIndex(mSelectedPhrase) == 0;
            bool isBlockClipBoardSet = mProjectPanel.Project.BlockClipBoard != null;
            
            bool canSetPage = isAudioBlockSelected;  // an audio block must be selected and a heading must not be set.
            bool canRemovePage = isAudioBlockSelected;
            if (isAudioBlockSelected)
            {
                CoreNode page = Project.GetStructureNode(mSelectedPhrase);
                canRemovePage = page != null && Project.GetNodeType(page) == NodeType.Page;
            }

            mAddStripToolStripMenuItem.Enabled = true;
            mRenameStripToolStripMenuItem.Enabled = isStripSelected;
            mDeleteStripToolStripMenuItem.Enabled = isStripSelected;
            mMoveStripUpToolStripMenuItem.Enabled = canMoveUp;
            mMoveStripDownToolStripMenuItem.Enabled = canMoveDown;
            mMoveStripToolStripMenuItem.Enabled = canMoveUp || canMoveDown;

            mRecordAudioToolStripMenuItem.Enabled = isStripSelected;
            mImportAudioFileToolStripMenuItem.Enabled = isStripSelected;
            mEditAudioBlockLabelToolStripMenuItem.Enabled = isAudioBlockSelected;
            mSplitAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMergeWithNextAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mCutAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mCopyAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mPasteAudioBlockToolStripMenuItem.Enabled = isBlockClipBoardSet && isStripSelected;
            mDeleteAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mMoveAudioBlockForwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockLast;
            mMoveAudioBlockBackwardToolStripMenuItem.Enabled = isAudioBlockSelected && !isAudioBlockFirst;
            mMoveAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected && (!isAudioBlockFirst || !isAudioBlockLast);

            mPlayAudioBlockToolStripMenuItem.Enabled = isAudioBlockSelected;
            mShowInTOCViewToolStripMenuItem.Enabled = isStripSelected;

            mSetPageLabelToolStripMenuItem.Enabled = canSetPage;
            mRemovePageLabelToolStripMenuItem.Enabled = canRemovePage;
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
            AddSiblingSection(this, new Events.Node.NodeEventArgs(this, mSelectedSection));
            // InsertSiblingSection(this, new Events.Node.NodeEventArgs(this, mSelectedSection));
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
        /// Bring up the record dialog.
        /// </summary>
        internal void mRecordAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedSection != null)
            {
                Settings settings = ((ObiForm)ParentForm).Settings;
                Dialogs.Record dialog = new Dialogs.Record(settings.AudioChannels, settings.SampleRate, settings.BitDepth,
                    mProjectPanel.Project.AssetManager);
                int index = mSelectedPhrase == null ?
                    Project.GetPhrasesCount(mSelectedSection) : mSelectedSection.indexOf(mSelectedPhrase) + 1;
                dialog.StartingPhrase += new Events.Audio.Recorder.StartingPhraseHandler(
                    delegate(object _sender, Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.StartRecordingPhrase(_sender, _e, mSelectedSection, index);
                    });
                dialog.ContinuingPhrase += new Events.Audio.Recorder.ContinuingPhraseHandler(
                    delegate(object _sender, Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.ContinuingRecordingPhrase(_sender, _e, mSelectedSection, index);
                    });
                dialog.FinishingPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(
                    delegate(object _sender, Events.Audio.Recorder.PhraseEventArgs _e)
                    {
                        mProjectPanel.Project.FinishRecordingPhrase(_sender, _e, mSelectedSection, index);
                    });
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    ((ObiForm)ParentForm).UndoLast();
                    Audio.AudioRecorder.Instance.EmergencyStop();
                }
            }
        }

        /// <summary>
        /// Play the currently selected audio block.
        /// </summary>
        /// <remarks>JQ</remarks>
        internal void mPlayAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null) ((ObiForm)ParentForm).Play(mSelectedPhrase);
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
                    SplitAudioBlockRequested(this, new Events.Node.SplitNodeEventArgs(this, mSelectedPhrase, dialog.ResultAsset));
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
        internal void mDeleteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                DeleteBlockRequested(this, new Events.Node.NodeEventArgs(this, mSelectedPhrase));
            }
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
                ProjectPanel.TOCPanel.SetSelectedSection(mSelectedSection);
                //since the tree can be hidden:
                mProjectPanel.ShowTOCPanel();
                ProjectPanel.TOCPanel.Focus();
            }
        }

        //md 20060812
        //shallow-delete a section node
        //mg 20060813: made internal to allow obiform menu sync access
        internal void deleteStripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToShallowDeleteSectionNode(this, new Events.Node.NodeEventArgs(this, this.mSelectedSection));
        }

        //md 20060812
        //mg 20060813: made internal to allow obiform menu sync access
        internal void upToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionNodeUpLinear(this, new Events.Node.NodeEventArgs(this, this.mSelectedSection));
        }

        //md 20060812
        //mg 20060813: made internal to allow obiform menu sync access
        internal void downToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RequestToMoveSectionNodeDownLinear(this, new Events.Node.NodeEventArgs(this, this.mSelectedSection));
        }


        /// <summary>
        /// Cut the selected block and store it in the block clip board.
        /// </summary>
        //JQ 20060815
        internal void mCutBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                RequestToCutPhraseNode(this, new Events.Node.NodeEventArgs(sender, mSelectedPhrase));
            }
        }

        /// <summary>
        /// Copy the selected blockand store it in the block clip board.
        /// Actually nothing changes, but a command is still issued to undo (and retrieve the last value in the clipboard.)
        /// </summary>
        // JQ 20060816
        internal void mCopyAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                RequestToCopyPhraseNode(this, new Events.Node.NodeEventArgs(sender, mSelectedPhrase));
            }
        }

        /// <summary>
        /// Paste the audio block in the clip board.
        /// </summary>
        // JQ 20060815
        internal void mPasteAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.Project.BlockClipBoard != null && mSelectedSection != null)
            {
                // Paste after the currently selected block, or at the end of the
                // currently selected section if no block is selected.
                RequestToPastePhraseNode(this, new Events.Node.NodeEventArgs(sender,
                    mSelectedPhrase == null ? mSelectedSection : mSelectedPhrase));
            }
        }

        /// <summary>
        /// Add a page number to the synchronization strip.
        /// If there is already a page here, ask the user if she wants to replace it.
        /// </summary>
        // JQ 20060817
        internal void mSetPageLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                mPhraseNodeMap[mSelectedPhrase].StartEditingPageLabel();
            }
        }


        /// <summary>
        /// Remove a page number.
        /// </summary>
        internal void mRemovePageLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                CoreNode pageNode = Project.GetStructureNode(mSelectedPhrase);
                if (pageNode != null && Project.GetNodeType(pageNode) == NodeType.Page)
                {
                    RequestToRemovePageLabel(this, new Events.Node.NodeEventArgs(sender, mSelectedPhrase));
                }
            }
        }
    }
}
