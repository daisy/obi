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
        private object mOrigin;  // the originator of the request
        private CoreNode mNode;  // the renamed node
        private string mLabel;   // its new label (easier to find this way :))

        public object Origin
        {
            get
            {
                return mOrigin;
            }
        }

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

        public RenamedNodeEventArgs(object origin, CoreNode node, string label)
        {
            mOrigin = origin;
            mNode = node;
            mLabel = label;
        }
    }
}
