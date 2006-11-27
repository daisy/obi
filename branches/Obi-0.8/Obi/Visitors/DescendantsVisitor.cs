using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Visitors
{
    /// <summary>
    /// Visitor building a flat list of all descendant nodes (including the root.)
    /// </summary>
    public class DescendantsVisitor: ICoreNodeVisitor
    {
        private List<CoreNode> mNodes;

        public List<CoreNode> Nodes
        {
            get
            {
                return mNodes;
            }
        }

        public DescendantsVisitor()
        {
            mNodes = new List<CoreNode>();
        }

        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
        }

        public bool preVisit(ICoreNode node)
        {
            mNodes.Add((CoreNode)node);
            return true;
        }

        #endregion
    }
}
