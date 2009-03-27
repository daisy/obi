using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Node
{
    class ObiNodeEventArgs: EventArgs
    {
        private ObiNode mNode;

        public ObiNode Node
        {
            get { return mNode; }
        }

        public ObiNodeEventArgs(ObiNode node)
        {
            mNode = node;
        }
    }
}
