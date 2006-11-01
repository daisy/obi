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
            TextMedia media = (TextMedia)block.Node.getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            media.setText(newName);
            Events.Node.SetMediaEventArgs e =
                new Events.Node.SetMediaEventArgs(this, block.Node, Project.AnnotationChannel, media);
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
        /// Add a new block from a phrase node and select it.
        /// </summary>
        internal void SyncAddedPhraseNode(object sender, Events.Node.AddedPhraseNodeEventArgs e)
        {
            System.Diagnostics.Debug.Assert(e.Node != null);
            SectionStrip strip = mSectionNodeMap[(CoreNode)e.Node.getParent()];
            AudioBlock block = SetupAudioBlockFromPhraseNode(e.Node);
            strip.InsertAudioBlock(block, e.Index);
            this.ReflowTabOrder(block);
            SelectedPhraseNode = e.Node;
        }

        /// <summary>
        /// Setup a new audio block from a phrase node.
        /// </summary>
        /// <param name="node">The phrase node.</param>
        /// <returns>The new audio block.</returns>
        private AudioBlock SetupAudioBlockFromPhraseNode(CoreNode node)
        {
            AudioBlock block = new AudioBlock();
            block.Manager = this;
            block.Node = node;
            mPhraseNodeMap[node] = block;
            TextMedia annotation = (TextMedia)Project.GetMediaForChannel(node, Project.AnnotationChannel);
            if (annotation != null) block.AnnotationBlock.Label = annotation.getText();
            Assets.AudioMediaAsset asset = Project.GetAudioMediaAsset(node);
            block.Label = asset.Name;
            PageProperty pageProp = node.getProperty(typeof(PageProperty)) as PageProperty;
            block.Page = pageProp == null ? "" : pageProp.PageNumber.ToString();
            block.Time = asset.LengthInSeconds;
            return block;
        }

        /// <summary>
        /// Delete the block of a phrase node.
        /// </summary>
        /// <param name="e">The node event with a pointer to the deleted phrase node.</param>
        internal void SyncDeleteAudioBlock(object sender, Events.Node.NodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[(CoreNode)e.Node.getParent()];
            if (SelectedPhraseNode == e.Node) SelectedPhraseNode = null;
            strip.RemoveAudioBlock(mPhraseNodeMap[e.Node]);
            mPhraseNodeMap.Remove(e.Node);
            // reflow?
        }

        /// <summary>
        /// Changed a media object on a node.
        /// </summary>
        internal void SyncMediaSet(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (Project.GetNodeType(e.Node) == NodeType.Phrase)
            {
                SectionStrip strip = mSectionNodeMap[(CoreNode)e.Node.getParent()];
                if (e.Channel == Project.AnnotationChannel)
                {
                    // the label of an audio block has changed
                    strip.SetAnnotationBlock(mPhraseNodeMap[e.Node], ((TextMedia)e.Media).getText());
                }
                else if (e.Channel == Project.AudioChannel)
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
            switch (Project.GetNodeType(e.Node))
            {
                case NodeType.Phrase:
                    SelectedPhraseNode = e.Node;
                    break;
                case NodeType.Section:
                    SelectedSectionNode = e.Node;
                    break;
                default:
                    break;
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
        internal void SyncSetPageNumber(object sender, Events.Node.NodeEventArgs e)
        {
            PageProperty pageProp = (PageProperty)e.Node.getProperty(typeof(PageProperty));
            mPhraseNodeMap[e.Node].Page = pageProp.PageNumber.ToString();
        }

        /// <summary>
        /// The page label was removed.
        /// </summary>
        internal void SyncRemovedPageNumber(object sender, Events.Node.NodeEventArgs e)
        {
            mPhraseNodeMap[e.Node].Page = "";
        }
    }
}
