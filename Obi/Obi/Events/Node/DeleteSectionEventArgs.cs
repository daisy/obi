using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void DeleteSectionHandler(object sender, DeleteSectionEventArgs e);

    public class DeleteSectionEventArgs : EventArgs
    {
        private CoreNode mNode;

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public DeleteSectionEventArgs(CoreNode node)
        {
            mNode = node;
        }
    }
}
