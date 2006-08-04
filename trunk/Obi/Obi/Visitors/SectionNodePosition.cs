using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    /// <summary>
    /// Find the position of a section node in the TOC tree, as seen in a flat list.
    /// </summary>
    public class SectionNodePosition: ICoreNodeVisitor
    {
        private CoreNode mTarget;
        private int mCounter;
        private int mPosition;

        public int Position
        {
            get
            {
                return mPosition;
            }
        }

        public SectionNodePosition(CoreNode target)
        {
            mTarget = target;
            mCounter = 0;
            mPosition = 0;
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
        }

        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.Section)
            {
                if (node == mTarget) mPosition = mCounter;
                ++mCounter;
            }
            return true;
        }

        #endregion
    }
}
