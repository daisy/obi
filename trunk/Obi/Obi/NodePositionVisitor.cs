using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi
{
    /// <summary>
    /// Find the position of a node in the TOC tree, as seen in a flat list.
    /// </summary>
    public class NodePositionVisitor: ICoreNodeVisitor
    {
        private CoreNode mTarget;
        private int mCounter;
        private int mPosition;

        public int Position
        {
            get
            {
                return mPosition - 1;
            }
        }

        public NodePositionVisitor(CoreNode target)
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
            if (node == mTarget) mPosition = mCounter;
            ++mCounter;
            return true;
        }

        #endregion
    }
}
