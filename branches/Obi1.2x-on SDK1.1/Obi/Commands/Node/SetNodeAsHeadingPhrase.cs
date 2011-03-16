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
            mNode.AncestorAs<SectionNode>().DidSetHeading(mNode);
        }

        public override void unExecute()
        {
            mNode.AncestorAs<SectionNode>().UnsetHeading(mNode);
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
            mNode.AncestorAs<SectionNode>().UnsetHeading(mNode);
        }

        public override void unExecute()
        {
            mNode.AncestorAs<SectionNode>().DidSetHeading(mNode);
        }

    }
}
