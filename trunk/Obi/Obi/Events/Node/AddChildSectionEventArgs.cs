using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void AddChildSectionHandler(object sender, AddChildSectionEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to add a new child section node in the core tree.
    /// </summary>
    class AddChildSectionEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the parent node of the new child

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public AddChildSectionEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
