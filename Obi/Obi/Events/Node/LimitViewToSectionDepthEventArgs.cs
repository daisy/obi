using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void LimitViewToSectionDepthHandler(object sender, 
    LimitViewToSectionDepthEventArgs e);

    /// <summary>
    /// This event is fired when the view should be limited to the depth of the given node
    /// </summary>
    class LimitViewToSectionDepthEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node whose depth will determine the max depth shown in the views

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public LimitViewToSectionDepthEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
