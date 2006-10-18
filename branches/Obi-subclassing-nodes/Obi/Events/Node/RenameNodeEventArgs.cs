using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    // public delegate void RequestToRenameNodeHandler(object sender, RenameNodeEventArgs e);
    public delegate void RenamedNodeHandler(object sender, RenameNodeEventArgs e);
    public delegate void RequestToSetPageNumberHandler(object sender, SetPageEventArgs e);

    /// <summary>
    /// This event is fired when a view wants to rename a section.
    /// </summary>
    public class RenameNodeEventArgs : NodeEventArgs
    {
        private string mLabel;    // the new text label of the node

        public string Label
        {
            get { return mLabel; }
        }

        public RenameNodeEventArgs(object origin, CoreNode node, string label) : 
            base(origin, node)
        {
            mLabel = label;
        }
    }
}
