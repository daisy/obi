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
            NodeSelection after = PostDeleteSelection;
            mNode.Detach();
            if (UpdateSelection) View.Selection = after;
        }

        public override void unExecute()
        {
            mParent.Insert(mNode, mIndex);
            base.unExecute();
        }

        // Determine what the selection will be after deletion
        private NodeSelection PostDeleteSelection
        {
            get
            {
                ObiNode node = null;
                if (mNode is SectionNode)
                {
                    if (View.Selection.Control is ProjectView.StripsView && ((SectionNode)mNode).PhraseChildCount > 0)
                    {
                        node = mNode.PhraseChild(0);
                    }
                    else
                    {
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
                    node = index < parent.PhraseChildCount - 1 ?
                        (ObiNode)parent.PhraseChild(index + 1) :
                        index > 0 ? (ObiNode)parent.PhraseChild(index - 1) :
                        (ObiNode)parent;
                }
                return node == null ? null : new NodeSelection(node, View.Selection.Control);
            }
        }
    }
}