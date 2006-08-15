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
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Play the currently selected audio block.
        /// </summary>
        /// <remarks>JQ</remarks>
        internal void mPlayAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedPhrase != null)
            {
                Dialogs.Play dialog = new Dialogs.Play(mSelectedPhrase);
                dialog.ShowDialog();
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

      
    }
}
