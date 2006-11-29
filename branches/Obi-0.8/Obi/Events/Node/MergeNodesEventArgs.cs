using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public class MergeNodesEventArgs: NodeEventArgs
    {
        private CoreNode mNext;

        public CoreNode Next
        {
            get { return mNext; }
        }
	
        public MergeNodesEventArgs(object origin, CoreNode node, CoreNode next)
            : base(origin, node)
        {
            mNext = next;
        }
    }
}
