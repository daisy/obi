using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void RenamedNodeHandler(object sender, RenamedNodeEventArgs e);

    /// <summary>
    /// This event is fired when a node has been renamed.
    /// </summary>
    public class RenamedNodeEventArgs : EventArgs
    {
        private CoreNode mNode;  // the renamed node
        private string mLabel;   // its new label (easier to find this way :))

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public string Label
        {
            get
            {
                return mLabel;
            }
        }

        public RenamedNodeEventArgs(CoreNode node, string label)
        {
            mNode = node;
            mLabel = label;
        }
    }
}
