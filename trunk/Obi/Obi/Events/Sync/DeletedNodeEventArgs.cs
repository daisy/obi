using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Sync
{
    public delegate void DeletedNodeHandler(object sender, DeletedNodeEventArgs e);
    public delegate void ShallowDeletedNodeHandler(object sender, DeletedNodeEventArgs e);

    class DeletedNodeEventArgs : EventArgs
    {
        private object mOrigin;   // the originator of the request
        private CoreNode mNode;   // the node that was deleted

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

        public DeletedNodeEventArgs(object origin, CoreNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
