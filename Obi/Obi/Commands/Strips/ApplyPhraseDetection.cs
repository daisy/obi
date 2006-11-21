using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace Obi.Commands.Strips
{
    public class ApplyPhraseDetection: Command
    {
        private Obi.Project mProject;
        private CoreNode mNode;
        private List<CoreNode> mPhraseNodes;

        public override string Label
        {
            get { return Localizer.Message("apply_phrase_detection_command_label"); }
        }

        public ApplyPhraseDetection(Obi.Project project, CoreNode node, List<CoreNode> phraseNodes)
        {
            mProject = project;
            mNode = node;
            mPhraseNodes = phraseNodes;
        }

        public override void Do()
        {
            mProject.ReplaceNodeWithNodes(mNode, mPhraseNodes);
        }

        public override void Undo()
        {
            mProject.ReplaceNodesWithNode(mPhraseNodes, mNode);
        }
    }
}
