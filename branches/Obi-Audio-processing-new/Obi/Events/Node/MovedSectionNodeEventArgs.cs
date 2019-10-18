using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public class MovedSectionNodeEventArgs : SectionNodeEventArgs
    {
        private TreeNode mParent;

        public TreeNode Parent
        {
            get
            {
                return mParent;
            }
        }

        public MovedSectionNodeEventArgs(object origin, SectionNode node, TreeNode parent) : 
            base(origin, node)
        {
            mParent = parent;
        }

    }
}
