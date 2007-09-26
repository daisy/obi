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
        }

        public override string getShortDescription() { return Localizer.Message("rename_section_command"); }

        public override void execute()
        {
            base.execute();
            View.Project.RenameSectionNode(mNode, mNewLabel);
        }

        public override void unExecute()
        {
            View.Project.RenameSectionNode(mNode, mOldLabel);
            base.unExecute();
        }
    }
}
