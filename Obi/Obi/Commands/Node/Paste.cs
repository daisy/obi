using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class Paste: Command
    {
        private ObiNode mCopy;             // copy of the node to paste
        private ObiNode mParent;           // parent for the copy
        private int mIndex;                // index for the copy
        private NodeSelection mSelection;  // the selection after pasting
        
        /// <summary>
        /// Paste a node.
        /// </summary>
        public Paste(ProjectView.ProjectView view)
            : base(view)
        {
            mCopy = (ObiNode)view.Clipboard.Node.copy(view.Clipboard.Deep, true);
            mParent = view.Selection.ParentForNewNode(mCopy);
            mIndex = view.Selection.IndexForNewNode(mCopy);
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

        public override void execute()
        {
            mParent.Insert(mCopy, mIndex);
            View.Selection = mSelection;
        }

        public override void unExecute()
        {
            mCopy.Detach();
            base.unExecute();
        }
    }
}
