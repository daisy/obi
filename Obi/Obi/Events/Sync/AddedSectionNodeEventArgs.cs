using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void AddedSectionNodeHandler(object sender, AddedSectionNodeEventArgs e);
 
    /// <summary>
    /// A section node was added.
    /// </summary>
    class AddedSectionNodeEventArgs: EventArgs
    {
        private object mOrigin;  // the origin of the event (initial requester)
        private CoreNode mNode;  // the added node
        private int mPosition;   // the position in the flat list of nodes
        private int mIndex;      // the index of the node in the parent's list of children

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

        public AddedSectionNodeEventArgs(object origin, CoreNode node, int index, int position)
        {
            mOrigin = origin;
            mNode = node;
            mIndex = index;
            mPosition = position;
        }
    }
}
