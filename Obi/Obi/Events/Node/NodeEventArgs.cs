using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void RequestToAddSiblingNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToAddChildNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToIncreaseNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDecreaseNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveNodeUpHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveNodeDownHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCutNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopyNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPasteNodeHandler(object sender, NodeEventArgs e);
  
    public delegate void DecreasedNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void DeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void ShallowDeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void CutNodeHandler(object sender, NodeEventArgs e);
    public delegate void CopiedNodeHandler(object sender, NodeEventArgs e);
    public delegate void UndidPasteNodeHandler(object sender, NodeEventArgs e);
    
    public delegate void ImportedAssetHandler(object sender, NodeEventArgs e);
    public delegate void DeletedBlockHandler(object sender, NodeEventArgs e);

    public delegate void BlockChangedTimeHandler(object sender, NodeEventArgs e);

    public class NodeEventArgs : EventArgs
    {
        private CoreNode mNode;  // the node on which the operation is performed
        private object mOrigin;  // the origin of the event (initial requester)

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public object Origin
        {
            get
            {
                return mOrigin;
            }
        }

        public NodeEventArgs(object origin, CoreNode node)
        {
            mOrigin = origin;
            mNode = node;
        }
    }
}
