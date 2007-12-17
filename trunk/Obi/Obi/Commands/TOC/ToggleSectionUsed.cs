using System;

namespace Obi.Commands.TOC
{
    public class ToggleSectionUsed: Command
    {
        private SectionNode mNode;     // the section node
        private bool mOriginalStatus;  // original used status of the node

        /// <summary>
        /// Create a command to toggle the used status of a single section.
        /// </summary>
        public ToggleSectionUsed(ProjectView.ProjectView view, SectionNode node)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.Used;
        }

        public override string getShortDescription()
        {
            return String.Format(Localizer.Message("toggle_section_used_command"),
                Localizer.Message(mOriginalStatus ? "unused" : "used"));
        }

        public override void execute()
        {
            mNode.Used = !mOriginalStatus;
            View.SelectInTOCView(mNode);
            base.execute();
        }

        public override void unExecute()
        {
            mNode.Used = mOriginalStatus;
            base.unExecute();
        }

        // Change the used status of a section node and all of its descendants
        private void ChangeUsedStatusDeep(SectionNode section, bool used)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) ChangeUsedStatusDeep(section.SectionChild(i), used);
            section.Used = used;
        }
    }
}
