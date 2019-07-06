using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public class PhraseNodeEventArgs
    {
        private PhraseNode mNode;  // the node on which the operation is performed
        private object mOrigin;  // the origin of the event (initial requester)

        public PhraseNode Node
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

        public PhraseNodeEventArgs(object origin, PhraseNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
