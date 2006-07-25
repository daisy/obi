using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void MovedNodeUpHandler(object sender, MovedNodeEventArgs e);
    public delegate void MovedNodeDownHandler(object sender, MovedNodeEventArgs e);

    class MovedNodeEventArgs
    {
        private object mOrigin;  // the origin of the event (initial requester)
        private CoreNode mNode;  // the moved node
       
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

        public MovedNodeEventArgs(object origin, CoreNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
