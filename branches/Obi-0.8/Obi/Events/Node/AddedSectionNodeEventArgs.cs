using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
   
    /// <summary>
    /// A section node was added.
    /// </summary>
    class AddedSectionNodeEventArgs: NodeEventArgs
    {
       public AddedSectionNodeEventArgs(object origin, SectionNode node) : 
            base(origin, node)
        {
        }
    }
}
