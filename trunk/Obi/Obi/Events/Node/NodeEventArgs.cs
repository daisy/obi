using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void RequestToAddSiblingNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToAddChildSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToIncreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDecreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeUpHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeDownHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopySectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPasteSectionNodeHandler(object sender, NodeEventArgs e);

    public delegate void DecreasedSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void DeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void ShallowDeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void CutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void CopiedSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void UndidPasteSectionNodeHandler(object sender, NodeEventArgs e);
    
    public delegate void ImportedAssetHandler(object sender, NodeEventArgs e);
    public delegate void DeletedBlockHandler(object sender, NodeEventArgs e);

    public delegate void TouchedNodeHandler(object sender, NodeEventArgs e);

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
