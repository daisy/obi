using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    class MovedSectionNodeEventArgs : NodeEventArgs
    {
        private SectionNode mParent;

        public SectionNode Parent
        {
            get
            {
                return mParent;
            }
        }

        public MovedSectionNodeEventArgs(object origin, SectionNode node, SectionNode parent) : 
            base(origin, node)
        {
            mParent = parent;
        }

    }
}
