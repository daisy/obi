using System;

using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        /// <summary>
        /// Add a new block for a phrase node that was just created.
        /// </summary>
        /// <param name="e">The node event with a pointer to the new phrase node.</param>
        internal void SyncCreateNewAudioBlock(object sender, Events.Node.NodeEventArgs e)
        {
            CoreNode parent = (CoreNode)e.Node.getParent();
            SectionStrip strip = mSectionNodeMap[parent];
            AudioBlock block = new AudioBlock();
            block.Manager = this;
            block.Node = e.Node;
            mPhraseNodeMap[e.Node] = block;
            block.Label = ((TextMedia)Project.GetMediaForChannel(e.Node, Project.AnnotationChannel)).getText();
            block.Time = Project.GetAudioMediaAsset(block.Node).LengthInSeconds;
            strip.InsertAudioBlock(block, Project.GetPhraseIndex(block.Node));
            // SelectedPhraseNode = block.Node;
            this.ReflowTabOrder(block);  // mg
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
    }
}