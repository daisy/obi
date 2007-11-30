using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    class AddContainer : Command
    {
        private int mIndex;
        private ObiNode mParent;
        private PhraseNode mCreatedNode;

        public AddContainer(ProjectView.ProjectView view, ObiNode parent, int index): base(view)
        {
            mIndex = index;
            mParent = parent;
            mCreatedNode = View.Presentation.CreatePhraseNode();
        }

        //get the newly-related container
        public PhraseNode Container { get { return mCreatedNode; } }

        //this creates a container node and puts the phrase inside it
        public override void execute()
        {
            mParent.Insert(mCreatedNode, mIndex);   
        }

        public override void unExecute()
        {
            mParent.RemoveChild(mCreatedNode);
        }
    }
}
