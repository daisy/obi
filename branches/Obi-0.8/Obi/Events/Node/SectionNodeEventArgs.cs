using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
   
    /// <summary>
    /// Communicate events about section nodes
    /// </summary>
    public class SectionNodeEventArgs
    {
       private SectionNode mNode;  // the node on which the operation is performed
        private object mOrigin;  // the origin of the event (initial requester)

        public SectionNode Node
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

        public SectionNodeEventArgs(object origin, SectionNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
