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
            if (e.Node.getParent().GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)e.Node.getParent()];
                AudioBlock block = SetupAudioBlockFromPhraseNode(e.Node);
                strip.InsertAudioBlock(block, e.Node.Index);
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
            block.Page = pageProp == null ? "" : pageProp.PageNumber.ToString();
            block.Time = asset.LengthInSeconds;
            return block;
        }

        /// <summary>
        /// Delete the block of a phrase node.
        /// </summary>
        /// <param name="e">The node event with a pointer to the deleted phrase node.</param>
        internal void SyncDeleteAudioBlock(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            if (e.Node.getParent().GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)e.Node.getParent()];
                if (SelectedPhraseNode == e.Node) SelectedPhraseNode = null;
                strip.RemoveAudioBlock(mPhraseNodeMap[e.Node]);
                mPhraseNodeMap.Remove(e.Node);
                // reflow?
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
            if (e.Node.GetType() == Type.GetType("Obi.SectionNode"))
            {
                SelectedSectionNode = (SectionNode)e.Node;
            }
            else if (e.Node.GetType() == Type.GetType("Obi.PhraseNode"))
            {
                SelectedPhraseNode = (PhraseNode)e.Node;
            }
        }

        /// <summary>
        /// The time of the asset for a phrase has changed.
        /// </summary>
        internal void SyncUpdateAudioBlockTime(object sender, Events.Strip.UpdateTimeEventArgs e)
        {
            mPhraseNodeMap[e.Node].Time = Math.Round(e.Time / 1000).ToString() + "s";
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
            PageProperty pageProp = (PageProperty)e.Node.getProperty(typeof(PageProperty));
            mPhraseNodeMap[e.Node].Page = pageProp.PageNumber.ToString();
        }

        /// <summary>
        /// The page label was removed.
        /// </summary>
        internal void SyncRemovedPageNumber(object sender, Events.Node.PhraseNodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].Page = "";
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
        /// Insert an empty audio block at the current insertion point.
        /// </summary>
        internal void InsertEmptyAudioBlock()
        {
            InsertPoint insert = this.CurrentInsertPoint;
            if (insert.node != null) mProjectPanel.Project.AddEmptyPhraseNode(insert.node, insert.index);   
        }

        internal void QuickSplit()
        {
            if (mSelectedPhrase != null)
            {
                double time = ProjectPanel.TransportBar.Playlist.CurrentTimeInAsset;
                Assets.AudioMediaAsset asset = mSelectedPhrase.Asset;
                if (time > 0 && time < asset.LengthInMilliseconds)
                {
                    ProjectPanel.TransportBar.Playlist.Stop();
                    Assets.AudioMediaAsset result = asset.Manager.SplitAudioMediaAsset(asset, time);
                    SplitAudioBlockRequested(this, new Events.Node.SplitPhraseNodeEventArgs(this, mSelectedPhrase, result));
                }
            }
        }
    }
}
