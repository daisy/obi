using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.core.visitor;

namespace Obi.Visitors
{
    class UndeleteSubtree: ITreeNodeVisitor
    {
        private Project mProject;  // the project in which the nodes belong
        private TreeNode mParent;  // parent of the node to readd
        private int mIndex;        // the future index of the subtree root, once it is re-integrated
   
        public UndeleteSubtree(Project project, TreeNode parent, int index)
        {
            mProject = project;
            mParent = parent;
            mIndex = index;
        }

        #region ITreeNodeVisitor Members

        public void postVisit(TreeNode node)
        {
            mParent = (TreeNode)node.getParent();
        }

        public bool preVisit(TreeNode node)
        {
            if (node.GetType() == System.Type.GetType("Obi.SectionNode"))
            {
                int index = node.getParent() == null ? mIndex : ((SectionNode)node).Index;
                //mProject.ReaddSectionNode((SectionNode)node, mParent, index);
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
