using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    class UndeleteSubtree: ICoreNodeVisitor
    {
        private Project mProject;  // the project in which the nodes belong
        private CoreNode mParent;  // parent of the node to readd
        private int mIndex;        // index of the node to readd

        public UndeleteSubtree(Project project, CoreNode parent, int index)
        {
            mProject = project;
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
            if (node.GetType() == typeof(SectionNode))
            {
                mProject.AddExistingSectionNode((SectionNode)node, mParent, mIndex);
                mIndex = 0;
            }
            //md 20060816
            //the phrase node already has a parent "node", so we can't re-add it
            //just give an event to notify the displays that they should update
            else
            {
                mProject.ReconstructPhraseNodeInView((PhraseNode)node, mIndex);
            }
            return true;
        }

        #endregion
    }
}
