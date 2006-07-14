using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void AddSiblingSectionHandler(object sender, AddSiblingSectionEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to add a new sibling section node in the core tree.
    /// </summary>
    public class AddSiblingSectionEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node after which to add the new sibling

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public AddSiblingSectionEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
