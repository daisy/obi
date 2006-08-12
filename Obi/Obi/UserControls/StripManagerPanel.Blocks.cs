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
                block.Label = newName;
            }
        }

        /// <summary>
        /// Add a new block from a phrase node and select it.
        /// </summary>
        internal void SyncAddedPhraseNode(object sender, Events.Node.AddedPhraseNodeEventArgs e)
        {
            if (e.Node != null)
            {
                SectionStrip strip = mSectionNodeMap[(CoreNode)e.Node.getParent()];
                AudioBlock block = new AudioBlock();
                block.Manager = this;
                block.Node = e.Node;
                mPhraseNodeMap[e.Node] = block;
                TextMedia annotation = (TextMedia)Project.GetMediaForChannel(e.Node, Project.AnnotationChannel);
                block.Label = annotation.getText();
                block.Time = Project.GetAudioMediaAsset(e.Node).LengthInSeconds;
                strip.InsertAudioBlock(block, e.Index);
                this.ReflowTabOrder(block);  // MG
                SelectedPhraseNode = block.Node;
            }
        }

        /// <summary>
        /// Delete the block of a phrase node.
        /// </summary>
        /// <param name="e">The node event with a pointer to the deleted phrase node.</param>
        internal void SyncDeleteAudioBlock(object sender, Events.Node.NodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[(CoreNode)e.Node.getParent()];
            SelectedPhraseNode = null;
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
                    strip.RenameAudioBlock(mPhraseNodeMap[e.Node], ((TextMedia)e.Media).getText());
                }
                else if (e.Channel == Project.AudioChannel)
                {
                    // the audio asset of an audio block has changed
                    strip.UpdateAssetAudioBlock(mPhraseNodeMap[e.Node]);  
                }
            }
        }
    }
}