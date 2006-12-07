using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    class MovedSectionNodeEventArgs : SectionNodeEventArgs
    {
        private CoreNode mParent;

        public CoreNode Parent
        {
            get
            {
                return mParent;
            }
        }

        public MovedSectionNodeEventArgs(object origin, SectionNode node, CoreNode parent) : 
            base(origin, node)
        {
            mParent = parent;
        }

    }
}
