using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    class UndeleteSubtree: ICoreNodeVisitor
    {
        private SectionNode mParent;  // parent of the node to readd
        private int mIndex;           // index of the node to readd

        public UndeleteSubtree(SectionNode parent, int index)
        {
            mParent = parent;
            mIndex = index;
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
            mParent = (SectionNode)node.getParent();
            mIndex = mParent.indexOf(node) + 1;
        }

        public bool preVisit(ICoreNode node)
        {
            if (node.GetType() == typeof(SectionNode))
            {
                ((SectionNode)node).Project.AddExistingSectionNode((SectionNode)node, mParent, mIndex);
                mIndex = 0;
            }
            //md 20060816
            //the phrase node already has a parent "node", so we can't re-add it
            //just give an event to notify the displays that they should update
            else if (node.GetType() == typeof(PhraseNode))
            {
                ((PhraseNode)node).Project.ReconstructPhraseNodeInView((PhraseNode)node);
            }
            return true;
        }

        #endregion
    }
}
