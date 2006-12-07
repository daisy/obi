using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace Obi.Commands.Strips
{
    public class ApplyPhraseDetection: Command
    {
        private Obi.Project mProject;
        private PhraseNode mNode;
        private List<PhraseNode> mPhraseNodes;

        public override string Label
        {
            get { return Localizer.Message("apply_phrase_detection_command_label"); }
        }

        public ApplyPhraseDetection(Obi.Project project, PhraseNode node, List<PhraseNode> phraseNodes)
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
