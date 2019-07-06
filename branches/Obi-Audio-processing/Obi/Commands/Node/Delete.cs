using System;
using System.Collections.Generic;
using urakawa.media.data;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Delete an Obi node.
    /// For shallow delete, children should be moved first.
    /// </summary>
    public class Delete : Command
    {
        private ObiNode mNode;         // the node to remove
        private ObiNode mParent;       // its original parent node
        private NodeSelection mAfter;  // selection after deletion
        private int mIndex;            // its original index


        /// <summary>
        /// Create a new delete section command from a view.
        /// </summary>
        public Delete(ProjectView.ProjectView view, ObiNode node, string label)
            : this(view, node, node.ParentAs<ObiNode>(), node.Index, true)
        {
            SetDescriptions(label);
        }

        public Delete(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index, bool update): base(view)
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
            if (view.Selection != null && view.Selection.Node == node)
            {
                mAfter = GetPostDeleteSelection();
            }
            else
            {
                mAfter = view.Selection;
            }
        }

        /// <summary>
        /// Create a delete node command with no label.
        /// </summary>
        public Delete(ProjectView.ProjectView view, ObiNode node) : this(view, node, "") { }

        /// <summary>
        /// Create a delete node command with no label and update selection flag (normally to be set to false.)
        /// </summary>
        public Delete(ProjectView.ProjectView view, ObiNode node, bool update)
            : this(view, node, "")
        {
            UpdateSelection = update;
        }


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                if (mNode != null && mNode is PhraseNode)
                {
                    List<MediaData> mediaList = new List<MediaData>();
                    if (((PhraseNode)mNode).Audio != null)
                        mediaList.Add(((PhraseNode)mNode).Audio.MediaData);
                    return mediaList;
                }
                else if (mNode != null && mNode is SectionNode)
                {
                    return GetMediaDataListForSection((SectionNode)mNode);
                }
                else
                    return new List<MediaData>();
            }
        }

    private List<MediaData> GetMediaDataListForSection ( SectionNode sNode )
        {
        List<MediaData> mediaList = new List<MediaData> ();

        sNode.AcceptDepthFirst (
                delegate ( urakawa.core.TreeNode n )
                    {
                    if (n != null && n is PhraseNode)
                        {
                        if ( ((PhraseNode)n).Audio != null )
                        mediaList.Add ( ((PhraseNode)n).Audio.MediaData);
                        }
                    return true;
                    },
                delegate ( urakawa.core.TreeNode n ) { } );

        return mediaList;
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            mNode.Detach();
            if (UpdateSelection) View.Selection = mAfter;
            TriggerProgressChanged ();
        }

        public override void UnExecute()
        {
            mParent.Insert(mNode, mIndex);
            base.UnExecute();
        }

        // Determine what the selection will be after deletion
        private NodeSelection GetPostDeleteSelection()
        {
            ObiNode node = null;
            if (mNode is SectionNode)
            {
                if (View.Selection.Control is ProjectView.ContentView)
                {
                    // Select the next strip; if there is no next strip, select the previous one.
                    node = ((SectionNode)mNode).FollowingSection;
                    if (node == null) node = ((SectionNode)mNode).PrecedingSection;
                }
                else
                {
                    // TODO: review this.
                    ObiNode parent = mNode.ParentAs<ObiNode>();
                    int index = mNode.Index;
                    node = index < parent.SectionChildCount - 1 ?
                        (ObiNode)parent.SectionChild(index + 1) :
                        index > 0 ? (ObiNode)parent.SectionChild(index - 1) :
                        parent == View.Presentation.RootNode ? null : parent;
                }
            }
            else
            {
                SectionNode parent = mNode.ParentAs<SectionNode>();
                int index = mNode.Index;
                // Select the next sibling;
                // if last child, select the previous sibling;
                // if first child, select the parent.
                node = index < parent.PhraseChildCount - 1 ?
                    (ObiNode)parent.PhraseChild(index + 1) :
                    index > 0 ? (ObiNode)parent.PhraseChild(index - 1) :
                    (ObiNode)parent;
            }
            return node == null ? null : new NodeSelection(node, View.Selection.Control);
        }
    }

    /// <summary>
    /// Delete a node at an offset of the selected one.
    /// Use this command when the node to delete does not exists yet.
    /// </summary>
    public class DeleteWithOffset : Command
    {
        private ObiNode mNode;     // node relative to which we delete
        private ObiNode mParent;   // parent of both nodes
        private ObiNode mDeleted;  // the deleted node
        private int mIndex;        // index of the node that we actually want to delete

        public DeleteWithOffset(ProjectView.ProjectView view, ObiNode node, int offset)
            : base(view)
        {
            mNode = node;
            mParent = node.ParentAs<ObiNode>();
            mDeleted = null;
            mIndex = mParent.Children.IndexOf(mNode) + offset;
        }


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                ObiNode node = (ObiNode)mParent.Children.Get(mIndex);
                if (node != null && node is PhraseNode)
                {
                    List<MediaData> mediaList = new List<MediaData>();
                    if (((PhraseNode)node).Audio != null)
                        mediaList.Add(((PhraseNode)node).Audio.MediaData);
                    return mediaList;
                }
                else if (node != null && node is SectionNode)
                {
                    return GetMediaDataListForSection((SectionNode)node);
                }
                else
                    return new List<MediaData>();
            }
        }

    private List<MediaData> GetMediaDataListForSection ( SectionNode sNode )
        {
        List<MediaData> mediaList = new List<MediaData> ();

        sNode.AcceptDepthFirst (
                delegate ( urakawa.core.TreeNode n )
                    {
                    if (n != null && n is PhraseNode && ((PhraseNode)n).Audio != null)
                        {
                        mediaList.Add ( ((PhraseNode)n).Audio.MediaData );
                        }
                    return true;
                    },
                delegate ( urakawa.core.TreeNode n ) { } );

        return mediaList;
        }

        public override bool CanExecute { get { return true; } }



        public override void Execute()
        {
            mDeleted = (ObiNode)mParent.Children.Get(mIndex);
            mParent.RemoveChild(mIndex);
            TriggerProgressChanged ();
        }

        public override void UnExecute()
        {
            mParent.Insert(mDeleted, mIndex);
            base.UnExecute();
        }
    }
}
