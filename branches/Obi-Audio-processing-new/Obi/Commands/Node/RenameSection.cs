using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Create a new rename command before the node is actually renamed.
    /// On execute(), the event is broadcast from the project.
    /// </summary>
    class RenameSection: Command
    {
        private SectionNode mNode;  // the renamed node
        private string mOldLabel;   // the old label for the node
        private string mNewLabel;   // the new label for the node

        public RenameSection(ProjectView.ProjectView view, SectionNode section, string label)
            : base(view)
        {
            mNode = section;
            mOldLabel = mNode.Label;
            mNewLabel = label;

            SetDescriptions(Localizer.Message("rename_section"));
        }

        //sdk2
        //public override string getShortDescription() { return ; }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Presentation.RenameSectionNode(mNode, mNewLabel);
            if (SelectionBefore != null) View.Selection = new NodeSelection(mNode, SelectionBefore.Control);
        }

        public override void UnExecute()
        {
            View.Presentation.RenameSectionNode(mNode, mOldLabel);
            base.UnExecute();
            if (SelectionBefore != null  )View.Selection = new NodeSelection(mNode, SelectionBefore.Control);
        }
    }
}
