using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    class AddedPhraseNodeEventArgs: NodeEventArgs
    {
        public AddedPhraseNodeEventArgs(object origin, PhraseNode node)
            :base(origin, node)
        {}
    }
}
