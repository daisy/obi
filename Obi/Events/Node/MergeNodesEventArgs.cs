using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public class MergeNodesEventArgs: PhraseNodeEventArgs
    {
        private PhraseNode mNext;

        public PhraseNode Next
        {
            get { return mNext; }
        }
	
        public MergeNodesEventArgs(object origin, PhraseNode node, PhraseNode next)
            : base(origin, node)
        {
            mNext = next;
        }
    }
}
