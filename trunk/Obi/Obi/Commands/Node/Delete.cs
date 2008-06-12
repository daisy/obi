using System;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Delete an Obi node.
    /// For shallow delete, children should be moved first.
    /// </summary>
    public class Delete : Command
    {
        private ObiNode mNode;         // the node to remove
        private ObiNode mParent;       // its original parent node
        private NodeSelection mAfter;  // selection after deletion
        private int mIndex;            // its original index


        /// <summary>
        /// Create a new delete section command from a view.
        /// </summary>
        public Delete(ProjectView.ProjectView view, ObiNode node, string label)
            : base(view)
        {
            mNode = node;
            mParent = node.ParentAs<ObiNode>();
            mIndex = mNode.Index;
            if (view.Selection != null && view.Selection.Node == node)
                mAfter = GetPostDeleteSelection();
            else
                mAfter = view.Selection;
            Label = label;
        }

        /// <summary>
        /// Create a delete section command with no label.
        /// </summary>
        public Delete(ProjectView.ProjectView view, ObiNode node) : this(view, node, "") { }


        public override void execute()
        {
            mNode.Detach();
            if (UpdateSelection) View.Selection = mAfter;
            if (mNode is EmptyNode ) View.UpdateBlocksLabelInStrip((SectionNode) mParent);
        }

        public override void unExecute()
        {
            mParent.Insert(mNode, mIndex);
            if (mNode is EmptyNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
            base.unExecute();
        }

        // Determine what the selection will be after deletion
        private NodeSelection GetPostDeleteSelection()
        {
            ObiNode node = null;
            if (mNode is SectionNode)
            {
                if (View.Selection.Control is ProjectView.StripsView)
                {
                    // Select the next strip; if there is no next strip, select the previous one.
                    node = ((SectionNode)mNode).FollowingSection;
                    if (node == null) node = ((SectionNode)mNode).PrecedingSection;
                }
                else
                {
                    // TODO: review this.
                    ObiNode parent = mNode.ParentAs<ObiNode>();
                    int index = mNode.Index;
                    node = index < parent.SectionChildCount - 1 ?
                        (ObiNode)parent.SectionChild(index + 1) :
                        index > 0 ? (ObiNode)parent.SectionChild(index - 1) :
                        parent is RootNode ? null : parent;
                }
            }
            else
            {
                SectionNode parent = mNode.ParentAs<SectionNode>();
                int index = mNode.Index;
                // Select the next sibling;
                // if last child, select the previous sibling;
                // if first child, select the parent.
                node = index < parent.PhraseChildCount - 1 ?
                    (ObiNode)parent.PhraseChild(index + 1) :
                    index > 0 ? (ObiNode)parent.PhraseChild(index - 1) :
                    (ObiNode)parent;
            }
            return node == null ? null : new NodeSelection(node, View.Selection.Control);
        }
    }

    /// <summary>
    /// Delete a node at an offset of the selected one.
    /// Use this command when the node to delete does not exists yet.
    /// </summary>
    public class DeleteWithOffset : Command
    {
        private ObiNode mNode;     // node relative to which we delete
        private ObiNode mParent;   // parent of both nodes
        private ObiNode mDeleted;  // the deleted node
        private int mIndex;        // index of the node that we actually want to delete

        public DeleteWithOffset(ProjectView.ProjectView view, ObiNode node, int offset)
            : base(view)
        {
            mNode = node;
            mParent = node.ParentAs<ObiNode>();
            mDeleted = null;
            mIndex = mParent.indexOf(mNode) + offset;
        }

        public override void execute()
        {
            mDeleted = (ObiNode)mParent.getChild(mIndex);
            mParent.removeChild(mIndex);
        }

        public override void unExecute()
        {
            mParent.Insert(mDeleted, mIndex);
            base.unExecute();
        }
    }
}
