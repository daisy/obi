using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SplitPhrase : Command
    {
        private Project mProject;                       // the current project
        private CoreNode mNode;                         // the node that is split
        private CoreNode mNewNode;                      // the new node that was created
        private Assets.AudioMediaAsset mSplitAsset;     // the first part of the split asset
        private Assets.AudioMediaAsset mOriginalAsset;  // the original asset before the split
        private int mNewIndex;                          // index of the new node

        public override string Label
        {
            get { return Localizer.Message("split_phrase_command_label"); }
        }

        public SplitPhrase(Project project, CoreNode node, CoreNode newNode)
        {
            mProject = project;
            mNode = node;
            mNewNode = newNode;
            mSplitAsset = Project.GetAudioMediaAsset(mNode);
            mOriginalAsset = (Assets.AudioMediaAsset)mSplitAsset.Copy();
            mOriginalAsset.MergeWith(Project.GetAudioMediaAsset(mNewNode));
            mNewIndex = ((CoreNode)mNewNode.getParent()).indexOf(mNewNode);
        }
    
        public override void  Do()
        {
            mProject.SetAudioMediaAsset(mNode, mSplitAsset);
            mProject.AddPhraseNodeAndAsset(mNewNode, (CoreNode)mNode.getParent(), mNewIndex);
        }

        public override void Undo()
        {
            mProject.SetAudioMediaAsset(mNode, mOriginalAsset);
            mProject.DeletePhraseNodeAndAsset(mNewNode);
            mProject.TouchNode(mNode);
        }
    }
}
