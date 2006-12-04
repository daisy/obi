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
   
        public UndeleteSubtree(Project project, CoreNode parent)
        {
            mProject = project;
            mParent = parent;
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
            mParent = (CoreNode)node.getParent();
        }

        public bool preVisit(ICoreNode node)
        {
            if (node.GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                mProject.AddExistingSectionNode((SectionNode)node, mParent, null);
            }
            //md 20060816
            //the phrase node already has a parent "node", so we can't re-add it
            //just give an event to notify the displays that they should update
            else if (node.GetType() == System.Type.GetType("Obi.PhraseNode"))
            {
                mProject.ReconstructPhraseNodeInView((PhraseNode)node);
            }
            return true;
        }

        #endregion
    }
}
