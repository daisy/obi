using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public abstract class AddNewSection: Command
    {
        private ObiNode mParent;
        private int mIndex;
        private SectionNode mNode;
        private NodeSelection mSelection;  // hack while we don't have working dummy

        /// <summary>
        /// Add a new section node in the context of the previous selection.
        /// </summary>
        public AddNewSection(ProjectView.ProjectView view) : this(view, view.Selection) { }

        public AddNewSection(ProjectView.ProjectView view, NodeSelection selection)
            : base(view)
        {
            mSelection = selection;
            ObiNode contextNode = mSelection.Node;
            if (mSelection.IsDummy)
            {
                mParent = contextNode;
                mIndex = 0;
            }
            else
            {
                mParent = contextNode.Parent;
                mIndex = contextNode.Index + 1;
            }
            mNode = View.Presentation.CreateSectionNode();
            mNode.Used = mParent.Used;
            View.SelectAndRenameSelection(new NodeSelection(mNode, mSelection.Control, false));
        }

        /// <summary>
        /// Add or readd the new section node then restore this as the selection.
        /// </summary>
        public override void execute()
        {
            base.execute();
            mParent.Insert(mNode, mIndex);
            View.Selection = new NodeSelection(mNode, mSelection.Control, false);
        }

        /// <summary>
        /// Remove the section node.
        /// </summary>
        public override void unExecute()
        {
            mNode.Detach();
            base.unExecute();
        }
    }
}
