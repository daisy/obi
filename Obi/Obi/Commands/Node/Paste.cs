using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data ;

namespace Obi.Commands.Node
{
    public class Paste: Command
    {
        private ObiNode mCopy;              // copy of the node to paste
        protected ObiNode mParent;          // parent for the copy
        protected int mIndex;               // index for the copy
        private NodeSelection mSelection;   // the selection after pasting
        private bool mDeleteSelectedBlock;  // flag to require deletion of the previous block

        /// <summary>
        /// Paste a node.
        /// </summary>
        public Paste(ProjectView.ProjectView view)
            : base(view)
        {
            mCopy = (ObiNode)view.Clipboard.Node.copy(view.Clipboard.Deep, true);
            mParent = view.Selection.ParentForNewNode(mCopy);
            mIndex = view.Selection.IndexForNewNode(mCopy);
            // If we paste a phrase node "into" an emtpy node, the empty node must be deleted
            // and its attributes are oved to the pasted node.
            if (mCopy is PhraseNode && view.Selection.Node.GetType() == typeof(EmptyNode))
            {
                mDeleteSelectedBlock = true;
                ((PhraseNode)mCopy).CopyAttributes((EmptyNode)view.Selection.Node);
                mCopy.Used = view.Selection.Node.Used;
            }
            else
            {
                mDeleteSelectedBlock = false;
            }
            mSelection = new NodeSelection(mCopy.LastDescendant, view.Selection.Control);
            Label = Localizer.Message(mCopy is EmptyNode ? "paste_phrase" : "paste_section");
        }

        /// <summary>
        /// The copy of the node to be pasted.
        /// </summary>
        public ObiNode Copy { get { return mCopy; } }

        /// <summary>
        /// The (future) parent of the copy.
        /// </summary>
        public ObiNode CopyParent { get { return mParent; } }

        /// <summary>
        /// If true, deleted the selected block before pasting.
        /// </summary>
        public bool DeleteSelectedBlock { get { return mDeleteSelectedBlock; } }


        public override List<MediaData> getListOfUsedMediaData ()
            {
            if (mCopy != null && mCopy is PhraseNode)
                {
                List<MediaData> mediaList = new List<MediaData> ();
                mediaList.Add ( (MediaData)((PhraseNode)mCopy).Audio.getMediaData () );
                return mediaList;
                }
            else if ( mCopy != null && mCopy is SectionNode )
                {
                return GetMediaDataListForSection ( (SectionNode)mCopy );
                
                }
            else
                return new List<MediaData> () ;
            }

        private List<MediaData> GetMediaDataListForSection ( SectionNode sNode )
            {
            List<MediaData> mediaList = new List<MediaData> ();

            sNode.acceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n != null &&  n is PhraseNode )
                        {
                            mediaList.Add ( ((PhraseNode)n ).Audio.getMediaData () ) ;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
    
                return mediaList;
            }


        public override void execute ()
        {
            mParent.Insert(mCopy, mIndex);
            if (UpdateSelection) View.Selection = mSelection;
            if (mParent != null && mParent is SectionNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
        }

        public override void unExecute()
        {
            mCopy.Detach();
            if (mParent != null && mParent is SectionNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
            base.unExecute();
        }
    }

    public class PasteBefore : Paste
    {
        public PasteBefore(ProjectView.ProjectView view)
            : base(view)
        {
            mIndex = view.Selection.Node.Index;
        }
    }

    public class PasteInside : Paste
    {
        public PasteInside(ProjectView.ProjectView view)
            : base(view)
        {
            mParent = view.Selection.Node;
            mIndex = mParent.SectionChildCount;
        }
    }
}
