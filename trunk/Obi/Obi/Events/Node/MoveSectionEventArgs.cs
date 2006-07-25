using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void MoveSectionUpHandler(object sender, MoveSectionEventArgs e);
    public delegate void MoveSectionDownHandler(object sender, MoveSectionEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to move a node up (reorder) in the core tree.
    /// </summary>
    public class MoveSectionEventArgs : EventArgs
    {
        private CoreNode mNode;  // the node that will move up

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public MoveSectionEventArgs(CoreNode node)
        {
            mNode = node;
        }
    }
}
