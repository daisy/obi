using urakawa.core;
using urakawa.media.data;

namespace Obi.Commands.Strips
{
    public class MergePhrases : Command
    {
        private PhraseNode mNode;
        private PhraseNode mNext;
        private ManagedAudioMedia mAudio;
        private ManagedAudioMedia mMergedAudio;

        public override string Label
        {
            get { return Localizer.Message("merge_phrases_command_label"); }
        }

        public MergePhrases(PhraseNode node, PhraseNode next)
        {
            mNode = node;
            mNext = next;
            mMergedAudio = mNode.Asset;
            mAudio = (AudioMediaAsset)mMergedAudio.Copy();
        }
    
        public override void  Do()
        {
            mNode.Project.SetAudioMediaAsset(mNode, mMergedAudio);
            mNode.Project.DeletePhraseNodeAndMedia(mNext);
        }

        public override void Undo()
        {
            mNode.Project.SetAudioMediaAsset(mNode, mAudio);
            mNode.Project.AddPhraseNodeAndAsset(mNext, mNode.ParentSection, mNode.Index + 1);
            //md 20061130 - this used to be .TouchPhraseNode(mNode), which is now unavailable
            //hope i've picked the right replacement
            mNode.Project.TouchNode(mNode);
        }
    }
}
