using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void ShallowSwappedSectionNodesHandler(object sender, ShallowSwappedSectionNodesEventArgs e);

    class ShallowSwappedSectionNodesEventArgs : NodeEventArgs
    {
        private CoreNode mSwappedNode;
        private int mNodePos;
        private int mSwappedNodePos;
        private int mNodeSectionIndex;
        private int mSwappedNodeSectionIndex;

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

        /// <summary>
        /// the position, wrt section nodes, of <see cref="mNode"/> before the swap
        /// </summary>
        public int SectionNodeIndex
        {
            get
            {
                return mNodeSectionIndex;
            }
        }

        /// <summary>
        /// the position, wrt section nodes, of <see cref="mSwappedNode"/> before the swap
        /// </summary>
        public int SwappedSectionNodeIndex
        {
            get
            {
                return mSwappedNodeSectionIndex;
            }
        }

        public ShallowSwappedSectionNodesEventArgs(object origin, CoreNode node, CoreNode swappedNode, int nodePos, int swappedNodePos, int sectionNodeIndex, int swappedSectionNodeIndex)
            :
            base(origin, node)
        {
            mSwappedNode = swappedNode;
            mNodePos = nodePos;
            mSwappedNodePos = swappedNodePos;
            mNodeSectionIndex = sectionNodeIndex;
            mSwappedNodeSectionIndex = swappedSectionNodeIndex;
        }

    }
}
