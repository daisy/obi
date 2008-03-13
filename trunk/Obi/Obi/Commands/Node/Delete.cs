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
            NodeSelection AfterDeleteSelection = PostDeleteSelection();
            mNode.Detach();
            //if (UpdateSelection) View.Selection = null;
            if (UpdateSelection) View.Selection = AfterDeleteSelection;
        }

        public override void unExecute()
        {
            mParent.Insert(mNode, mIndex);
            base.unExecute();
        }

        // Avn: function to determine selection after delete.
        private NodeSelection PostDeleteSelection()
        {
            ObiNode AfterDeleteSelectionNode = null;
            if (mNode is SectionNode) // if selected node is section select appropriate section after delete
            {
                SectionNode node = (SectionNode)mNode;
                AfterDeleteSelectionNode = node.NextSection;

                if (AfterDeleteSelectionNode == null)
                    AfterDeleteSelectionNode = node.PrecedingSection; // bugg in proceeding section property, sometimes triggeres out of range index exception
            }
            else // else selected node is empty node or phrase node so after delete select appropriate  empty/phrase node or parent section if there are no children left.
            {
                EmptyNode ENode = (EmptyNode)mNode;
                SectionNode Parent = ENode.ParentAs<SectionNode>();
                int PhraseIndex = ENode.Index;
                if (Parent.PhraseChildCount >= PhraseIndex + 2) // atleast one node is  there after deleted node so select it
                    AfterDeleteSelectionNode = Parent.PhraseChild(PhraseIndex + 1);
                else if (Parent.PhraseChildCount > 1) // no node after deleted node but there are some nodes before deleted node so select previous phrase
                {
                    AfterDeleteSelectionNode = Parent.PhraseChild(PhraseIndex - 1);
                }
                else // there is no phrase in section, move selection to parent section
                {
                    AfterDeleteSelectionNode = Parent;
                }
            }
            if (AfterDeleteSelectionNode != null)
                return new NodeSelection(AfterDeleteSelectionNode, View.Selection.Control);
            else
                return null;
        }

    }
}
