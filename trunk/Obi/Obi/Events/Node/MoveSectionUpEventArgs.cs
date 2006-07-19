using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void MoveSectionUpHandler(object sender, MoveSectionUpEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to move a node up (reorder) in the core tree.
    /// </summary>
    public class MoveSectionUpEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node that will move up

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public MoveSectionUpEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
