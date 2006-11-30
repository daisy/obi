using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    
    /// <summary>
    /// This event is fired when a view wants to rename a section.
    /// </summary>
    public class RenameSectionNodeEventArgs : SectionNodeEventArgs
    {
        private string mLabel;    // the new text label of the node

        public string Label
        {
            get
            {
                return mLabel;
            }
        }

        public RenameSectionNodeEventArgs(object origin, SectionNode node, string label) : 
            base(origin, node)
        {
            mLabel = label;
        }
    }
}
