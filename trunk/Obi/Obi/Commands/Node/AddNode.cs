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
        private NodeSelection mSelection;

        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index): base(view, "")
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
            mSelection = view.Selection == null ? null : new NodeSelection(mNode, view.Selection.Control);
        }

        public AddNode(ProjectView.ProjectView view, ObiNode node)
            : this(view, node, node.ParentAs<ObiNode>(), node.Index) {}

        public override void execute()
        {
            mParent.Insert(mNode, mIndex);
            if (UpdateSelection) View.Selection = mSelection;
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
            Label = Localizer.Message("add_empty_block");
        }

        public override void execute()
        {
            base.execute();
            View.Selection = new NodeSelection(mNode, mControl);
        }
    }
}