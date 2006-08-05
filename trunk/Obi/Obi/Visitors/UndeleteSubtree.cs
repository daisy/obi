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
        private int mPosition;     // position in the list

        public UndeleteSubtree(Project project, CoreNode parent, int index, int position)
        {
            mProject = project;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
            mParent = (CoreNode)node.getParent();
            mIndex = mParent.indexOf(node) + 1;
        }

        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.Section)
            {
                mProject.AddExistingSection((CoreNode)node, mParent, mIndex, mPosition, null);
                mIndex = 0;
                ++mPosition;
            }
            return true;
        }

        #endregion
    }
}
