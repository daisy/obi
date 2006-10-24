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
            Events.Node.SetMediaEventArgs e = new Events.Node.SetMediaEventArgs(Project.AnnotationChannelName, media);
            SetMediaRequested(this, (PhraseNode)block.Node, e);
            if (e.Cancel)
            {
                MessageBox.Show(String.Format(Localizer.Message("name_already_exists_text"), newName),
                    Localizer.Message("name_already_exists_caption"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                block.Label = newName;
            }
        }

        /// <summary>
        /// Add a new block from a phrase node and select it.
        /// </summary>
        internal void SyncAddedPhraseNode(object sender, PhraseNode node)
        {
            if (node != null)
            {
                SectionStrip strip = mSectionNodeMap[node.ParentSection];
                AudioBlock block = new AudioBlock();
                block.Manager = this;
                block.Node = node;
                mPhraseNodeMap[node] = block;
                TextMedia annotation = (TextMedia)Project.GetMediaForChannel(node, Project.AnnotationChannelName);
                block.Label = annotation.getText();
                block.Time = Project.GetAudioMediaAsset(node).LengthInSeconds;
                strip.InsertAudioBlock(block, node.Index);
                this.ReflowTabOrder(block);  // MG
                SelectedPhraseNode = node;
                PageProperty pageProp = node.getProperty(typeof(PageProperty)) as PageProperty;
                //if (pageProp != null && pageProp.getOwner() != null)
                if (pageProp != null) block.StructureBlock.Label = pageProp.PageNumber.ToString();
            }
        }

        /// <summary>
        /// Delete the block of a phrase node.
        /// </summary>
        internal void SyncDeleteAudioBlock(object sender, PhraseNode node)
        {
            SectionStrip strip = mSectionNodeMap[node.ParentSection];
            if (SelectedPhraseNode == node) SelectedPhraseNode = null;
            strip.RemoveAudioBlock(mPhraseNodeMap[node]);
            mPhraseNodeMap.Remove(node);
            // reflow?
        }

        /// <summary>
        /// Changed a media object on a node.
        /// </summary>
        internal void SyncMediaSet(object sender, Events.Node.Phrase.SetMediaEventArgs e)
        {
            if (Project.GetNodeType(e.Node) == NodeType.Phrase)
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)e.Node.getParent()];
                if (e.Channel == Project.AnnotationChannelName)
                {
                    // the label of an audio block has changed
                    strip.RenameAudioBlock(mPhraseNodeMap[e.Node], ((TextMedia)e.Media).getText());
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
        internal void SyncTouchedPhraseNode(object sender, PhraseNode node)
        {
            // update the node somehow?
            SelectedPhraseNode = node;
        }

        /// <summary>
        /// The time of the asset for a phrase has changed.
        /// </summary>
        internal void SyncUpdateAudioBlockTime(object sender, PhraseNode node, double time)
        {
            mPhraseNodeMap[node].Time = Math.Round(time / 1000).ToString() + "s";
        }

        internal void InterceptKeyDownFromChildControl(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// The page label has changed.
        /// </summary>
        internal void SyncSetPageNumber(object sender, PhraseNode node)
        {
            if (node.PageProperty != null)
            {
                mPhraseNodeMap[node].StructureBlock.Label = node.PageProperty.PageNumber.ToString();
            }
        }

        /// <summary>
        /// The page label was removed.
        /// </summary>
        internal void SyncRemovedPageNumber(object sender, PhraseNode node)
        {
            mPhraseNodeMap[node].StructureBlock.Label = "";
        }

        internal void SendRequestToSetPageNumber(StructureBlock structureBlock, PhraseNode phraseNode, int pageNumber)
        {
            RequestToSetPageNumber(structureBlock, phraseNode, pageNumber);
        }
    }
}
