using urakawa.core;
using Obi.Assets;

namespace Obi.Commands.Strips
{
    public class MergePhrases : Command
    {
        private PhraseNode mNode;
        private PhraseNode mNext;
        private AudioMediaAsset mAsset;
        private AudioMediaAsset mMergedAsset;

        public override string Label
        {
            get { return Localizer.Message("merge_phrases_command_label"); }
        }

        public MergePhrases(PhraseNode node, PhraseNode next)
        {
            mNode = node;
            mNext = next;
            mMergedAsset = mNode.Asset;
            mAsset = (AudioMediaAsset)mMergedAsset.Copy();
        }
    
        public override void  Do()
        {
            mNode.Project.SetAudioMediaAsset(mNode, mMergedAsset);
            mNode.Project.DeletePhraseNodeAndAsset(mNext);
        }

        public override void Undo()
        {
            mNode.Project.SetAudioMediaAsset(mNode, mAsset);
            mNode.Project.AddPhraseNodeAndAsset(mNext, mNode.ParentSection, mNode.Index + 1);
            //md 20061130 - this used to be .TouchPhraseNode(mNode), which is now unavailable
            //hope i've picked the right replacement
            mNode.Project.TouchNode(mNode);
        }
    }
}
