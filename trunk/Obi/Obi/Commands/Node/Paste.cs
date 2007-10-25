using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class Paste: Command
    {
        private NodeSelection mSelection;  // the selection context
        private ObiNode mCopy;             // copy of the node to paste
        private string mLabel;             // command label
        
        public Paste(ProjectView.ProjectView view)
            : base(view)
        {
            mSelection = view.Selection;
            mCopy = view.Clipboard.Copy;
            mLabel = Localizer.Message(
                view.Selection.Control is ProjectView.TOCView ? "paste_section_command" :
                mCopy.SectionChildCount > 0 ? "paste_strips_command" : "paste_strip_command"
            );
        }

        /// <summary>
        /// The copy of the node to be pasted.
        /// </summary>
        public ObiNode Copy { get { return mCopy; } }

        public override string getShortDescription() { return mLabel; }

        public override void execute()
        {
            base.execute();
            mSelection.Node.ParentAs<ObiNode>().InsertAfter(mCopy, mSelection.Node);
            if (!mCopy.ParentAs<ObiNode>().Used) MakeUnused(mCopy);
            View.Selection = new NodeSelection(mCopy, mSelection.Control, false);
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
