using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void RenameSectionHandler(object sender, RenameSectionEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to rename a section.
    /// </summary>
    public class RenameSectionEventArgs : EventArgs
    {
        private CoreNode mNode;  // the node to rename
        private string mLabel;   // the new text label of the node

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

        public RenameSectionEventArgs(CoreNode node, string label)
        {
            mNode = node;
            mLabel = label;
        }
    }
}
