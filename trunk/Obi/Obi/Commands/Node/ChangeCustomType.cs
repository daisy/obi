using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class ChangeCustomType : Command
    {
        //the node whose type is being changed
        private PhraseNode mNode;
        private string mCustomName;
        private string mOldCustomName;
        private EmptyNode.Kind mNodeKind;
        private EmptyNode.Kind mOldNodeKind;

        //the node plus the information about the change that we're about to make
        public ChangeCustomType(ProjectView.ProjectView view, PhraseNode node, string customName, EmptyNode.Kind nodeKind) : base(view)
        {
            mNode = null;
            if (node != null)
            {
                mNode = node;
                mCustomName = customName;
                mNodeKind = nodeKind;
                mOldCustomName = node.CustomClass;
                mOldNodeKind = node.NodeKind;

            }
        }
        public override void execute()
        {
            if (mNode == null) return;
            mNode.CustomClass = mCustomName;
            mNode.NodeKind = mNodeKind;

        }

        public override void unExecute()
        {
            if (mNode == null) return;
            mNode.CustomClass = mOldCustomName;
            mNode.NodeKind = mOldNodeKind;
        }
    }
}
