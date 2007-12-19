using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class Paste: Command
    {
        private NodeSelection mSelection;  // the selection context
        private ObiNode mCopy;             // copy of the node to paste
        private ObiNode mLastNode;         // last node copied (for selection)
        private ObiNode mParent;           // parent for the copy
        private int mIndex;                // index for the copy
        
        public Paste(ProjectView.ProjectView view)
            : base(view)
        {
            mSelection = view.Selection;
            mCopy = view.Clipboard.Copy;
            mLastNode = mCopy.LastDescendant;
            mParent = mSelection.ParentForNewNode(mCopy);
            mIndex = mSelection.IndexForNewNode(mCopy);
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

        public override void execute()
        {
            mParent.Insert(mCopy, mIndex);
            if (!mParent.Used) MakeUnused(mCopy);
            View.Selection = new NodeSelection(mLastNode, mSelection.Control);
        }

        private void MakeUnused(ObiNode node)
        {
            if (node != null && node.Used)
            {
                node.Used = false;
                for (int i = 0; i < node.getChildCount(); ++i) MakeUnused(node.getChild(i) as ObiNode);
            }
        }

        public override void unExecute()
        {
            mCopy.Detach();
            base.unExecute();
        }
    }
}
