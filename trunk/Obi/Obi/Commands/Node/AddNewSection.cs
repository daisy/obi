using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class AddNewSection: Command
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
            mNode = view.Presentation.CreateSectionNode();
            mIndex = selection.IndexForNewNode(mNode);
            mParent = selection.ParentForNewNode(mNode);
            mNode.Used = mParent.Used;
            View.SelectAndRenameSelection(new NodeSelection(mNode, mSelection.Control));
        }

        /// <summary>
        /// The new section node to be added.
        /// </summary>
        public SectionNode NewSection { get { return mNode; } }
        
        /// <summary>
        /// Add or readd the new section node then restore this as the selection.
        /// </summary>
        public override void execute()
        {
            base.execute();
            mParent.Insert(mNode, mIndex);
            View.Selection = new NodeSelection(mNode, mSelection.Control);
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
