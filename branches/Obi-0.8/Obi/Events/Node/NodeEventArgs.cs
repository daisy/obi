using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
   
    public class NodeEventArgs : EventArgs
    {
        private CoreNode mNode;  // the node on which the operation is performed
        private object mOrigin;  // the origin of the event (initial requester)

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public object Origin
        {
            get
            {
                return mOrigin;
            }
        }

        public NodeEventArgs(object origin, CoreNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
