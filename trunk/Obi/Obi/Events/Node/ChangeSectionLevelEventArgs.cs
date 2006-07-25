using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void IncreaseSectionLevelHandler(object sender,
    ChangeSectionLevelEventArgs e);
    public delegate void DecreaseSectionLevelHandler(object sender, 
    ChangeSectionLevelEventArgs e);

    /// <summary>
    /// This event is fired to request to increase a section by one level depth
    /// </summary>
    class ChangeSectionLevelEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node to be moved

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public ChangeSectionLevelEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
