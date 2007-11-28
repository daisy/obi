using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class ChangeCustomType : Command
    {
        //the node whose type is being changed
        private PhraseNode mNode;
        private string mCustomKind;
        private string mOldCustomKind;

        //the node plus the information about the change that we're about to make
        public ChangeCustomType(ProjectView.ProjectView view, PhraseNode node, string customType) : base(view)
        {
            mNode = null;
            if (node != null)
            {
                mNode = node;
                mCustomKind = customType;
                mOldCustomKind = node.CustomKind;
            }
        }
        public override void execute()
        {
            if (mNode == null) return;
            mNode.CustomKind = mCustomKind;
            if (mCustomKind == "") mNode.PhraseKind = PhraseNode.Kind.Plain;
            else mNode.PhraseKind = PhraseNode.Kind.Custom;
        }

        public override void unExecute()
        {
            if (mNode == null) return;
            mNode.CustomKind = mOldCustomKind;
            if (mOldCustomKind == "") mNode.PhraseKind = PhraseNode.Kind.Plain;
            else mNode.PhraseKind = PhraseNode.Kind.Custom;
        }
    }
}
