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
        // private CoreNode mMerged;

        public override string Label
        {
            get { return Localizer.Message("merge_phrases_command_label"); }
        }

        public MergePhrases(Project project, CoreNode node, CoreNode next)
        {
            mProject = project;
            mNode = node;
            mNext = next;
            // mMerged = null;
        }
    
        public override void  Do()
        {
            mProject.MergeNodesRequested(this, new Events.Node.MergeNodesEventArgs(this, mNode, mNext));
        }

        public override void Undo()
        {
            /*Assets.AudioMediaAsset asset = Project.GetAudioMediaAsset(mNode);
            Assets.AudioMediaAsset mResultAsset =
                asset.Manager.SplitAudioMediaAsset(Project.GetAudioMediaAsset(mNode), mSplitTime);
            mProject.SplitAssetRequested(this, new Events.Node.SplitNodeEventArgs(this, mNode, mResultAsset));*/
            throw new Exception("CANNOT UNDO");
        }
    }
}
