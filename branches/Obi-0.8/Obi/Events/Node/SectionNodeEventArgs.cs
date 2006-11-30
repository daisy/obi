using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
   
    /// <summary>
    /// Communicate events about section nodes
    /// </summary>
    public class SectionNodeEventArgs: NodeEventArgs
    {
       public SectionNodeEventArgs(object origin, SectionNode node) : 
            base(origin, node)
        {
        }
    }
}
