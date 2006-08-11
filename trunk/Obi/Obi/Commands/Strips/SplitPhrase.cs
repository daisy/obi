using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SplitPhrase : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mNewNode;
        private Assets.AudioMediaAsset mAsset;  // save the asset from the next phrase

        public override string Label
        {
            get { return Localizer.Message("split_phrase_command_label"); }
        }

        public SplitPhrase(Project project, CoreNode node, CoreNode newNode)
        {
            mProject = project;
            mNode = node;
            mNewNode = newNode;
            mAsset = Project.GetAudioMediaAsset(mNewNode);
        }
    
        public override void  Do()
        {
            mProject.SplitAssetRequested(this, new Events.Node.SplitNodeEventArgs(this, mNode,
                Project.GetAudioMediaAsset(mNewNode)));
        }

        public override void Undo()
        {
            mProject.MergeNodesRequested(this, new Events.Node.MergeNodesEventArgs(this, mNode, mNewNode));
        }
    }
}
