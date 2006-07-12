using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void BeginEditingSectionHeadingLabelHandler(object sender, 
    BeginEditingSectionHeadingLabelEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to let the user begin editing the label
    /// of a section heading.
    /// </summary>
    class BeginEditingSectionHeadingLabelEventArgs : EventArgs
    {
        private CoreNode mContextNode;  // the node whose label is to be edited

        public CoreNode ContextNode
        {
            get
            {
                return mContextNode;
            }
        }

        public BeginEditingSectionHeadingLabelEventArgs(CoreNode contextNode)
        {
            mContextNode = contextNode;
        }
    }
}
