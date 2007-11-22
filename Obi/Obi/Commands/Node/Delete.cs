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
        private string mDescription;  // description (for undo/redo menu)

        public Delete(ProjectView.ProjectView view, ObiNode node, string description)
            : base(view)
        {
            mNode = node;
            mParent = node.ParentAs<ObiNode>();
            mIndex = mNode.Index;
            mDescription = description;
        }

        public Delete(ProjectView.ProjectView view, ObiNode node) : this(view, node, "") { }

        public override string getShortDescription() { return mDescription; }

        public override void execute()
        {
            base.execute();
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
