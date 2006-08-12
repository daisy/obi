using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class MergePhrases : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mNext;
        private Assets.AudioMediaAsset mAsset;
        private Assets.AudioMediaAsset mMergedAsset;
        private int mNextIndex;

        public override string Label
        {
            get { return Localizer.Message("merge_phrases_command_label"); }
        }

        public MergePhrases(Project project, CoreNode node, CoreNode next)
        {
            mProject = project;
            mNode = node;
            mNext = next;
            mMergedAsset = Project.GetAudioMediaAsset(mNode);
            mAsset = (Assets.AudioMediaAsset)mMergedAsset.Copy();
            mNextIndex = ((CoreNode)mNext.getParent()).indexOf(mNext);
        }
    
        public override void  Do()
        {
            mProject.SetAudioMediaAsset(mNode, mMergedAsset);
            mProject.DeletePhraseNodeAndAsset(mNext);
        }

        public override void Undo()
        {
            mProject.SetAudioMediaAsset(mNode, mAsset);
            mProject.AddPhraseNodeAndAsset(mNext, (CoreNode)mNode.getParent(), mNextIndex);
            mProject.TouchNode(mNode);
        }
    }
}
