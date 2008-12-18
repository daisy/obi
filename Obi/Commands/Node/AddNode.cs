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

        /// <summary>
        /// Add an existing node to a parent node at the given index.
        /// </summary>
        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index): base(view, "")
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
            mSelection = view.Selection != null && view.Selection.Control is ProjectView.ContentView ?
                new NodeSelection(mNode, view.Selection.Control) : view.Selection;
        }

        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index, bool update)
            : this(view, node, parent, index)
        {
            UpdateSelection = update;
        }

        /// <summary>
        /// Add an existing node to its parent at its index.
        /// </summary>
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
            if (mNode is EmptyNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
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
            Label = Localizer.Message("add_blank_phrase");
        }

        public override void execute()
        {
            base.execute();
            View.Selection = new NodeSelection(mNode, mControl);
        }
    }
}