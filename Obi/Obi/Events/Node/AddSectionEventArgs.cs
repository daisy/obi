using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void AddSiblingSectionHandler(object sender, AddSectionEventArgs e);
    public delegate void AddChildSectionHandler(object sender, AddSectionEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to add a new sibling section node in the core tree.
    /// </summary>
    public class AddSectionEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node after which to add the new sibling

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public AddSectionEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
