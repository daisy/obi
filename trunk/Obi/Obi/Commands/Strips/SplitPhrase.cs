using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Command to split a phrase node. The new node has already been created and the audio asset was already split.
    /// </summary>
    public class SplitPhrase : Command__OLD__
    {
        private PhraseNode mNode;                // the phrase node that is split
        private PhraseNode mNewNode;             // the new phrase node that was created
        private ManagedAudioMedia mSplitAudio;     // the first part of the split asset
        private ManagedAudioMedia mOriginalAudio;  // the original asset before the split

        /// <summary>
        /// Create the command once a node has been split in two from its two parts.
        /// </summary>
        /// <param name="node">The phrase node that was split.</param>
        /// <param name="newNode">The new phrase node resulting from the split.</param>
        public SplitPhrase(PhraseNode node, PhraseNode newNode)
        {
            mNode = node;
            mNewNode = newNode;
            mSplitAudio = node.Audio;

            // Avn: following two lines changed for updated toolkit
            //mOriginalAudio = node.Project.DataManager.CopyAndManage(node.Audio);
                        //ManagedAudioMedia newAudio = newNode.Project.DataManager.CopyAndManage(newNode.Audio);
            mOriginalAudio = node.Audio.copy();
            ManagedAudioMedia newAudio = newNode.Audio.copy();

            Audio.DataManager.MergeAndManage(mOriginalAudio, newAudio);
        }


        /// <summary>
        /// Label for the undo menu item.
        /// </summary>
        public override string Label { get { return Localizer.Message("split_phrase_command_label"); } }

        /// <summary>
        /// Do: restore the split asset on the split node, and readd the new node.
        /// </summary>
        public override void Do()
        {
            mNode.Audio = mSplitAudio;
            //mNode.Project.AddPhraseNodeWithAudio(mNewNode, mNode.ParentSection, mNode.Index + 1);
        }

        /// <summary>
        /// Undo: restore the unsplit asset on the split node, and remove the new node.
        /// </summary>
        public override void Undo()
        {
            //mNode.Project.DeletePhraseNodeAndMedia(mNewNode);            
            mNode.Audio = mOriginalAudio;
            //mNode.Project.TouchNode(mNode);
        }
    }
}
