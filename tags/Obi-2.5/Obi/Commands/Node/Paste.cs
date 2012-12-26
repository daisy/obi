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
            mCopy = (ObiNode)view.Clipboard.Node.Copy(view.Clipboard.Deep, true);
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
            SetDescriptions(Localizer.Message(mCopy is EmptyNode ? "paste_phrase" : "paste_section"));
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

        private bool m_AllowRoleChangeAccordingToSurroundingSpecialNodes = true;
        /// <summary>
        /// <Allows the added phrase to change its role according to special roles surrounding it. Its true by default
        /// </summary>
        public bool AllowRoleChangeAccordingToSurroundingSpecialNodes
        {
            get { return m_AllowRoleChangeAccordingToSurroundingSpecialNodes; }
            set { m_AllowRoleChangeAccordingToSurroundingSpecialNodes = value; }
        }


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {

                if (mCopy != null && mCopy is PhraseNode)
                {
                    List<MediaData> mediaList = new List<MediaData>();
                    if (((PhraseNode)mCopy).Audio != null)
                        mediaList.Add(((PhraseNode)mCopy).Audio.MediaData);
                    return mediaList;
                }
                else if (mCopy != null && mCopy is SectionNode)
                {
                    return GetMediaDataListForSection((SectionNode)mCopy);
                }
                else
                    return new List<MediaData>();
            }
        }

        private List<MediaData> GetMediaDataListForSection ( SectionNode sNode )
            {
            List<MediaData> mediaList = new List<MediaData> ();

            sNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                    if (n != null && n is PhraseNode && ((PhraseNode)n).Audio != null )
                        {
                            mediaList.Add ( ((PhraseNode)n ).Audio.MediaData) ;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
    
                return mediaList;
            }

        public override bool CanExecute { get { return true; } }


        public override void Execute ()
        {
            mParent.Insert(mCopy, mIndex);
            if (AllowRoleChangeAccordingToSurroundingSpecialNodes) AssignRole.AssignRoleToEmptyNodeSurroundedByCustomRoles(mCopy);
            if (UpdateSelection) View.Selection = mSelection;
            if (mParent != null && mParent is SectionNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
        }

        public override void UnExecute()
        {
            mCopy.Detach();
            if (mParent != null && mParent is SectionNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
            base.UnExecute();
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
