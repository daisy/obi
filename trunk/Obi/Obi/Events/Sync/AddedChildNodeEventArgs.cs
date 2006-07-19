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

        public AddedChildNodeEventArgs(object origin, CoreNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
