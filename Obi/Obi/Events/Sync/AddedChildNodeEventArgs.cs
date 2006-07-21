using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void AddedChildNodeHandler(object sender, AddedChildNodeEventArgs e);

    class AddedChildNodeEventArgs: EventArgs
    {
        private object mOrigin;   // the originator of the request
        private CoreNode mNode;   // the node that was added
        private int mPosition;    // position of the node in the flat list of sections

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

        public int Position
        {
            get
            {
                return mPosition;
            }
        }

        public AddedChildNodeEventArgs(object origin, CoreNode node, int position)
        {
            mOrigin = origin;
            mNode = node;
            mPosition = position;
        }
    }
}
