using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Assign the given node as its section's audio block
    /// </summary>
    class SetNodeAsHeadingPhrase : Command
    {
        private PhraseNode mNode;

        public SetNodeAsHeadingPhrase(ProjectView.ProjectView view, PhraseNode node) : base(view, "")
        {
            mNode = node;
        }
        public override void execute()
        {
            if (mNode == null) return;
            mNode.AncestorAs<SectionNode>().Heading = mNode; 
        }

        public override void unExecute()
        {
            if (mNode == null) return;
            mNode.AncestorAs<SectionNode>().Heading = null;
        } 
    }

    /// <summary>
    /// Take away the given node's role of being its section's audio block
    /// </summary>
    class UnsetNodeAsHeadingPhrase : Command
    {
        private PhraseNode mNode;
        
        public UnsetNodeAsHeadingPhrase(ProjectView.ProjectView view, PhraseNode node)
            : base(view, "")
        {
            mNode = node;
        }
        public override void execute()
        {
            if (mNode == null) return;
            if (mNode.AncestorAs<SectionNode>().Heading == mNode) mNode.AncestorAs<SectionNode>().Heading = null;
            else throw new Exception("UnsetNodeAsHeading: node was not a heading!");
        }

        public override void unExecute()
        {
            if (mNode == null) return;
            mNode.AncestorAs<SectionNode>().Heading = mNode;
        }

    }
}
