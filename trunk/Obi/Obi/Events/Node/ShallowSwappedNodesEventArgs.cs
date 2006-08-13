using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void ShallowSwappedNodesHandler(object sender, ShallowSwappedNodesEventArgs e);

    class ShallowSwappedNodesEventArgs : NodeEventArgs
    {
        private CoreNode mSwappedNode;
        private int mNodePos;
        private int mSwappedNodePos;

        public CoreNode SwappedNode
        {
            get
            {
                return mSwappedNode;
            }
        }

        /// <summary>
        /// Represents the <see cref="mNode"/> position before the swap
        /// </summary>
        public int NodePosition
        {
            get
            {
                return mNodePos;
            }
        }

        /// <summary>
        /// Represents the <see cref="mSwappedNode"/> position before the swap
        /// </summary>
        public int SwappedNodePosition
        {
            get
            {
                return mSwappedNodePos;
            }
        }

        public ShallowSwappedNodesEventArgs(object origin, CoreNode node, CoreNode swappedNode, int nodePos, int swappedNodePos)
            :
            base(origin, node)
        {
            mSwappedNode = swappedNode;
            mNodePos = nodePos;
            mSwappedNodePos = swappedNodePos;
        }

    }
}
