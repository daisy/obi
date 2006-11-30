using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    class PhraseNodeEventArgs: NodeEventArgs
    {
        public PhraseNodeEventArgs(object origin, PhraseNode node)
            :base(origin, node)
        {}
    }
}
