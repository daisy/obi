using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void DecreaseSectionLevelHandler(object sender, 
    DecreaseSectionLevelEventArgs e);

    /// <summary>
    /// This event is fired to request to decrease a section by one level depth
    /// </summary>
    class DecreaseSectionLevelEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node to be moved

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public DecreaseSectionLevelEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
