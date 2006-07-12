using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void MoveSectionDownHandler(object sender, MoveSectionDownEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to move a node down (reorder) in the core tree.
    /// </summary>
    class MoveSectionDownEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node that will move down

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public MoveSectionDownEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
