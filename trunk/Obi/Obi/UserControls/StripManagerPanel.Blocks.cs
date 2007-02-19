using System;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        /// <summary>
        /// The user has modified the label of an audio block, so the change has to be made in the project and shown on the block.
        /// However, if the name change could not be made, the event is cancelled and the block's label is left unchanged.
        /// </summary>
        /// <param name="block">The block (with its old label.)</param>
        /// <param name="newName">The new label for the block.</param>
        internal void EditedAudioBlockLabel(AudioBlock block, string newName)
        {
            //md 20061220 fixes exit crash
            if (mProjectPanel == null) return;

            TextMedia media = (TextMedia)block.Node.getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            media.setText(newName);
            Events.Node.SetMediaEventArgs e =
                new Events.Node.SetMediaEventArgs(this, block.Node, Project.AnnotationChannelName, media);
            SetMediaRequested(this, e);
            if (e.Cancel)
            {
                MessageBox.Show(String.Format(Localizer.Message("name_already_exists_text"), newName),
                    Localizer.Message("name_already_exists_caption"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                block.AnnotationBlock.Label = newName;
            }
        }

        /// <summary>
        /// Remove the annotation on an audio block.
        /// </summary>
        /// <param name="block">The block to remove the annotation on.</param>
        internal void RemovedAnnotation(AudioBlock block)
        {
            TextMedia media = (TextMedia)block.Node.getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            Events.Node.SetMediaEventArgs e =
                new Events.Node.SetMediaEventArgs(this, block.Node, Project.AnnotationChannelName, media);
            SetMediaRequested(this, e);
            block.AnnotationBlock.Label = "";
        }

        /// <summary>
        /// Add a new block from a phrase node and select it.
        /// </summary>
        internal void SyncAddedPhraseNode(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Node != null);
            if (e.Node.ParentSection != null)
            {
                SectionStrip strip = mSectionNodeMap[e.Node.ParentSection];
                AudioBlock block = SetupAudioBlockFromPhraseNode(e.Node);
                strip.InsertAudioBlock(block, e.Node.Index);
                if (e.Node.PageProperty != null) mProjectPanel.Project.RenumberPages();
                SelectedPhraseNode = e.Node;
            }
        }

        /// <summary>
        /// Setup a new audio block from a phrase node.
        /// </summary>
        /// <param name="node">The phrase node.</param>
        /// <returns>The new audio block.</returns>
        private AudioBlock SetupAudioBlockFromPhraseNode(PhraseNode node)
        {
            AudioBlock block = new AudioBlock();
            block.Manager = this;
            block.Node = node;
            mPhraseNodeMap[node] = block;
            TextMedia annotation = (TextMedia)Project.GetMediaForChannel(node, Project.AnnotationChannelName);
            if (annotation != null) block.AnnotationBlock.Label = annotation.getText();
            Assets.AudioMediaAsset asset = node.Asset;// Project.GetAudioMediaAsset(node);
            PageProperty pageProp = node.getProperty(typeof(PageProperty)) as PageProperty;
            // block.Label = pageProp == null ? "(no page)" : pageProp.PageNumber.ToString();
            // block.Time = Assets.MediaAsset.FormatTime(asset.LengthInMilliseconds);
            return block;
        }

        /// <summary>
        /// Delete the block of a phrase node.
        /// </summary>
        /// <param name="e">The node event with a pointer to the deleted phrase node.</param>
        internal void SyncDeleteAudioBlock(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Node != null);
            if (e.Node.ParentSection != null)
            {
                SectionStrip strip = mSectionNodeMap[e.Node.ParentSection];
                if (SelectedPhraseNode == e.Node) SelectedPhraseNode = null;
                strip.RemoveAudioBlock(mPhraseNodeMap[e.Node]);
                mPhraseNodeMap.Remove(e.Node);
                if (e.Node.PageProperty != null) mProjectPanel.Project.RenumberPagesExcluding(e.Node);
            }
        }

        /// <summary>
        /// Changed a media object on a node.
        /// </summary>
        internal void SyncMediaSet(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (e.Node.getParent().GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)e.Node.getParent()];
                if (e.Channel == Project.AnnotationChannelName)
                {
                    // the label of an audio block has changed
                    strip.SetAnnotationBlock(mPhraseNodeMap[e.Node], ((TextMedia)e.Media).getText());
                }
                else if (e.Channel == Project.AudioChannelName)
                {
                    // the audio asset of an audio block has changed
                    strip.UpdateAssetAudioBlock(mPhraseNodeMap[e.Node]);
                }
            }
        }

        /// <summary>
        /// The node was modified so we need to make sure that it gets selected.
        /// </summary>
        internal void SyncTouchedNode(object sender, Events.Node.NodeEventArgs e)
        {
            SelectedNode = e.Node as ObiNode;
        }

        /// <summary>
        /// The time of the asset for a phrase has changed.
        /// </summary>
        internal void SyncUpdateAudioBlockTime(object sender, Events.Strip.UpdateTimeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay();
        }

        internal void InterceptKeyDownFromChildControl(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// The page label has changed.
        /// </summary>
        internal void SyncSetPageNumber(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay();
        }

        /// <summary>
        /// The page label was removed.
        /// </summary>
        internal void SyncRemovedPageNumber(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].RefreshDisplay();
        }





        /// <summary>
        /// The insert point for a phrase node inside a section.
        /// </summary>
        private struct InsertPoint
        {
            public SectionNode node;  // the section node to add in
            public int index;         // the index at which to add
        }

        /// <summary>
        /// Get the current insertion point.
        /// If a section is selected, this is the end of the section.
        /// If a phrase is selected, this is the index of the phrase
        /// (so that insertion happens before.)
        /// </summary>
        private InsertPoint CurrentInsertPoint
        {
            get
            {
                InsertPoint insert = new InsertPoint();
                if (SelectedPhraseNode != null)
                {
                    insert.node = SelectedPhraseNode.ParentSection;
                    insert.index = SelectedPhraseNode.Index;
                }
                else if (SelectedSectionNode != null)
                {
                    insert.node = SelectedSectionNode;
                    insert.index = SelectedSectionNode.PhraseChildCount;
                }
                else
                {
                    insert.node = null;
                }
                return insert;
            }
        }

        /// <summary>
        /// True if there is a currently available insertion point.
        /// </summary>
        public bool CanInsertPhraseNode
        {
            get { return SelectedNode != null; }
        }

        /// <summary>
        /// True if there is a selected block currently in use that is preceded by another block in use as well.
        /// </summary>
        public bool CanMerge
        {
            get
            {
                return mSelectedPhrase != null && mSelectedPhrase.Used &&
                    mSelectedPhrase.PreviousPhrase != null &&
                    mSelectedPhrase.PreviousPhrase.Used;
            }
        }

        /// <summary>
        /// Import phrases from sound files at the end of a selected section or before a selected phrase.
        /// The files are chosen through a file browser. This can be cancelled through the file browser's
        /// cancel button, in which case no change is made.
        /// </summary>
        /// <remarks>One "import phrase" command will be created for every phrase that is inserted.</remarks>
        public void ImportPhrases()
        {
            if (CanInsertPhraseNode)
            {
                // we may lose the currently selected phrase when stopping
                InsertPoint insert = CurrentInsertPoint;
                mProjectPanel.TransportBar.Enabled = false;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = Localizer.Message("audio_file_filter");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string path in dialog.FileNames)
                    {
                        mProjectPanel.Project.AddPhraseFromFile(path, insert.node, insert.index);
                        ++insert.index;
                    }
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Split (with preview) of the selected block.
        /// </summary>
        public void SplitBlock()
        {
            if (mSelectedPhrase != null)
            {
                PhraseNode phrase = mSelectedPhrase;
                double time = mProjectPanel.TransportBar.Playlist.CurrentTimeInAsset;
                Audio.AudioPlayerState state = mProjectPanel.TransportBar.State;
                mProjectPanel.TransportBar.Enabled = false;
                Dialogs.Split dialog = new Dialogs.Split(phrase, time, state);
                if (dialog.ShowDialog() == DialogResult.OK) mProjectPanel.Project.Split(phrase, dialog.ResultAsset);
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Quick split (without preview) of the selected block.
        /// </summary>
        public void QuickSplitBlock()
        {
            if (mSelectedPhrase != null)
            {
                PhraseNode phrase = mSelectedPhrase;
                double time = ProjectPanel.TransportBar.Playlist.CurrentTimeInAsset;
                mProjectPanel.TransportBar.Enabled = false;
                Assets.AudioMediaAsset asset = phrase.Asset;
                if (time > 0 && time < asset.LengthInMilliseconds)
                {
                    Assets.AudioMediaAsset result = asset.Manager.SplitAudioMediaAsset(asset, time);
                    mProjectPanel.Project.Split(phrase, result);
                }
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Merge the selected block with the previous one in the strip.
        /// </summary>
        public void MergeBlocks()
        {
            if (CanMerge)
            {
                mProjectPanel.TransportBar.Enabled = false;
                mProjectPanel.Project.MergeNodes(mSelectedPhrase.PreviousPhrase, mSelectedPhrase);
                mProjectPanel.TransportBar.Enabled = true;
            }
        }

        /// <summary>
        /// Insert an empty audio block at the current insertion point.
        /// </summary>
        /// <remarks>Currently disabled</remarks>
        public void InsertEmptyAudioBlock()
        {
            InsertPoint insert = this.CurrentInsertPoint;
            if (insert.node != null) mProjectPanel.Project.AddEmptyPhraseNode(insert.node, insert.index);   
        }

        /// <summary>
        /// Move the selected audio block, if any, in the given direction.
        /// </summary>
        /// <param name="direction">Move the block backward or forward.</param>
        public void MoveBlock(PhraseNode.Direction direction)
        {
            if (mSelectedPhrase != null) mProjectPanel.Project.MovePhraseNode(mSelectedPhrase, direction);
        }

        /// <summary>
        /// Set a page number on the selected block.
        /// </summary>
        public void SetPageNumber()
        {
            if (mSelectedPhrase != null) mProjectPanel.Project.SetPageNumberOnPhraseWithUndo(mSelectedPhrase);
        }

        /// <summary>
        /// Remove a page number on the selected block.
        /// </summary>
        public void RemovePageNumber()
        {
            if (mSelectedPhrase != null) mProjectPanel.Project.RemovePageNumberFromPhraseWithUndo(mSelectedPhrase);
        }
    }
}
