using urakawa.core;

namespace Obi.Visitors
{
    class UndeleteSubtree: ICoreNodeVisitor
    {
        private CoreNode mParent;  // parent of the node to readd
        private int mIndex;        // index of the node to readd

        public UndeleteSubtree(CoreNode parent, int index)
        {
            mParent = parent;
            mIndex = index;
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
            mParent = (CoreNode)node.getParent();
            mIndex = mParent.indexOf(node) + 1;
        }

        public bool preVisit(ICoreNode node)
        {
            if (node is SectionNode)
            {
                ((SectionNode)node).Project.AddExistingSectionNode((SectionNode)node, mParent, mIndex);
                mIndex = 0;
            }
            //md 20060816
            //the phrase node already has a parent "node", so we can't re-add it
            //just give an event to notify the displays that they should update
            else if (node is PhraseNode)
            {
                ((PhraseNode)node).Project.ReconstructPhraseNodeInView((PhraseNode)node);
            }
            return true;
        }

        #endregion
    }
}
