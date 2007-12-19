using System;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Delete an Obi node.
    /// For shallow delete, children should be moved first.
    /// </summary>
    public class Delete : Command
    {
        private ObiNode mNode;        // the node to remove
        private ObiNode mParent;      // its original parent node
        private int mIndex;           // its original index

        public Delete(ProjectView.ProjectView view, ObiNode node, string label)
            : base(view)
        {
            mNode = node;
            mParent = node.ParentAs<ObiNode>();
            mIndex = mNode.Index;
            Label = label;
        }

        public Delete(ProjectView.ProjectView view, ObiNode node) : this(view, node, "") { }

        public override void execute()
        {
            mNode.Detach();
            View.Selection = null;
        }

        public override void unExecute()
        {
            mParent.Insert(mNode, mIndex);
            base.unExecute();
        }
    }
}
