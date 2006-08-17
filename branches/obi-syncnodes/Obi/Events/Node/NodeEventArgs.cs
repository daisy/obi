using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    //requests for adding and deleting section nodes
    public delegate void RequestToAddSiblingNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToAddChildSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDeleteNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToShallowDeleteSectionNodeHandler(object sender, NodeEventArgs e);
   
    //requests for moving section nodes
    public delegate void RequestToIncreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToDecreaseSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeUpHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeDownHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeUpLinearHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveSectionNodeDownLinearHandler(object sender, NodeEventArgs e);

    //section node clipboard requests
    public delegate void RequestToCutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopySectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPasteSectionNodeHandler(object sender, NodeEventArgs e);

    // phrase node clipboard requests (JQ 20060816)
    public delegate void RequestToCutPhraseNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToCopyPhraseNodeHandler(object sender, NodeEventArgs e);
    public delegate void RequestToPastePhraseNodeHandler(object sender, NodeEventArgs e);

    public delegate void RequestToDeleteBlockHandler(object sender, NodeEventArgs e);
    public delegate void RequestToMoveBlockHandler(object sender, NodeEventArgs e);
    
    //section node handlers which communicate that requested actions have been done
    //most likely so that the views may update themselves accordingly
    public delegate void DecreasedSectionNodeLevelHandler(object sender, NodeEventArgs e);
    public delegate void DeletedNodeHandler(object sender, NodeEventArgs e);
    public delegate void ShallowDeletedSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void CutSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void CopiedSectionNodeHandler(object sender, NodeEventArgs e);
    public delegate void UndidPasteSectionNodeHandler(object sender, NodeEventArgs e);
    
    public delegate void ImportedAssetHandler(object sender, NodeEventArgs e);
    public delegate void DeletedBlockHandler(object sender, NodeEventArgs e);

    public delegate void TouchedNodeHandler(object sender, NodeEventArgs e);

    public delegate void RequestToRemovePageLabelHandler(object sender, NodeEventArgs e);
    public delegate void RemovedPageLabelHandler(object sender, NodeEventArgs e);

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
