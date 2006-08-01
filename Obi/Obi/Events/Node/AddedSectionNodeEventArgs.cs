using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void AddedSectionNodeHandler(object sender, AddedSectionNodeEventArgs e);
    
    /// <summary>
    /// A section node was added.
    /// </summary>
    class AddedSectionNodeEventArgs: NodeEventArgs
    {
        private int mPosition;   // the position in the flat list of nodes
        private int mIndex;      // the index of the node in the parent's list of children

        public int Index
        {
            get
            {
                return mIndex;
            }
        }

    	public int Position
	    {
		    get
            {
                return mPosition;
            }
		}

        public AddedSectionNodeEventArgs(object origin, CoreNode node, int index, int position) : 
            base(origin, node)
        {
            
            mIndex = index;
            mPosition = position;
        }
    }
}
