using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void MovedNodeHandler(object sender, MovedNodeEventArgs e);
 
    class MovedNodeEventArgs : AddedSectionNodeEventArgs
    {
        private CoreNode mParent;

        public CoreNode Parent
        {
            get
            {
                return mParent;
            }
        }

        public MovedNodeEventArgs(object origin, CoreNode node, CoreNode parent, int index, int position) : 
            base(origin, node, index, position)
        {
            mParent = parent;
        }

    }
}
