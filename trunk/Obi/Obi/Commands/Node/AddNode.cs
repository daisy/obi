using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Command adding an existing node.
    /// </summary>
    public class AddNode: Command
    {
        protected ObiNode mNode;
        private ObiNode mParent;
        private int mIndex;

        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index): base(view, "")
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
        }

        public AddNode(ProjectView.ProjectView view, ObiNode node)
            : this(view, node, node.ParentAs<ObiNode>(), node.Index) {}

        public override void execute()
        {
            base.execute();
            mParent.Insert(mNode, mIndex);
        }

        public override void unExecute()
        {
            mNode.Detach();
            base.unExecute();
        }
    }

    public class AddEmptyNode : AddNode
    {
        private IControlWithSelection mControl;

        public AddEmptyNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index)
            : base(view, node, parent, index)
        {
            mControl = view.Selection.Control;
        }

        public override string getShortDescription() { return Localizer.Message("add_empty_block_command"); }

        public override void execute()
        {
            base.execute();
            View.Selection = new NodeSelection(mNode, mControl);
        }
    }
}
