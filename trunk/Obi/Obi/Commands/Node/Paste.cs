using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class Paste: Command
    {
        private ObiNode mCopy;              // copy of the node to paste
        private ObiNode mParent;            // parent for the copy
        private int mIndex;                 // index for the copy
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
                ((PhraseNode)mCopy).CopyKind((EmptyNode)view.Selection.Node);
                mCopy.Used = view.Selection.Node.Used;
            }
            else
            {
                mDeleteSelectedBlock = false;
            }
            mSelection = new NodeSelection(mCopy.LastDescendant, view.Selection.Control);
            Label = Localizer.Message(
                mCopy is EmptyNode ? "paste_block" :
                view.Selection.Control is ProjectView.TOCView ? "paste_section" :
                mCopy.SectionChildCount > 0 ? "paste_strips" : "paste_strip"
            );
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

        public override void execute()
        {
            mParent.Insert(mCopy, mIndex);
            if (UpdateSelection) View.Selection = mSelection;
        }

        public override void unExecute()
        {
            mCopy.Detach();
            base.unExecute();
        }
    }
}
