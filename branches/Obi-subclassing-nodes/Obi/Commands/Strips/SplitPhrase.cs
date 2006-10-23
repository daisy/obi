using urakawa.core;
using Obi.Assets;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Command to split a phrase node. The new node has already been created and the audio asset was already split.
    /// </summary>
    public class SplitPhrase : Command
    {
        private PhraseNode mNode;                // the phrase node that is split
        private PhraseNode mNewNode;             // the new phrase node that was created
        private AudioMediaAsset mSplitAsset;     // the first part of the split asset
        private AudioMediaAsset mOriginalAsset;  // the original asset before the split
        
        /// <summary>
        /// Label for the undo menu item.
        /// </summary>
        public override string Label
        {
            get { return Localizer.Message("split_phrase_command_label"); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">The phrase node that was split.</param>
        /// <param name="newNode">The new phrase node resulting from the split.</param>
        public SplitPhrase(PhraseNode node, PhraseNode newNode)
        {
            mNode = node;
            mNewNode = newNode;
            mSplitAsset = node.Asset;
            // reconstruct the original asset by merging.
            mOriginalAsset = (AudioMediaAsset)newNode.Asset.Copy();
            mOriginalAsset.MergeWith(newNode.Asset);
        }

        /// <summary>
        /// Do: restore the split asset on the split node, and readd the new node.
        /// </summary>
        public override void  Do()
        {
            mNode.Asset = mSplitAsset;
            mNode.Project.AddPhraseNodeAndAsset(mNewNode, mNewNode.ParentSection, mNewNode.PhraseIndex);
        }

        /// <summary>
        /// Undo: restore the unsplit asset on the split node, and remove the new node.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.SetAudioMediaAsset(mNode, mOriginalAsset);
            mNode.Project.DeletePhraseNodeAndAsset(mNewNode);
            mNode.Project.TouchPhraseNode(mNode);      
        }
    }
}
