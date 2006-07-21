using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void AddedSiblingNodeHandler(object sender, AddedSiblingNodeEventArgs e);

    class AddedSiblingNodeEventArgs : EventArgs
    {
        private object mOrigin;         // the originator of the request
        private CoreNode mNode;         // the node that was added
        private CoreNode mContextNode;  // the sibling node that we added to
        private int mPosition;          // position of the node in the flat list of sections

        public object Origin
        {
            get
            {
                return mOrigin;
            }
        }

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public int Position
        {
            get
            {
                return mPosition;
            }
        }

        public AddedSiblingNodeEventArgs(object origin, CoreNode node, CoreNode contextNode, int position)
        {
            mOrigin = origin;
            mNode = node;
            mContextNode = contextNode;
            mPosition = position;
        }
    }
}
