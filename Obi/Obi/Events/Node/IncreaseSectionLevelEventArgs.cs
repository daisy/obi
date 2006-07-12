using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void IncreaseSectionLevelHandler(object sender,
    IncreaseSectionLevelEventArgs e);

    /// <summary>
    /// This event is fired to request to increase a section by one level depth
    /// </summary>
    class IncreaseSectionLevelEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node to be moved

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public IncreaseSectionLevelEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
