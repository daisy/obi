using System;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// Delete a section (and all of its subsections)
    /// </summary>
    public class Delete : Command
    {
        private SectionNode mSection;  // the deleted section
        private ObiNode mParent;       // parent of the deleted section (section or root node)
        private int mIndex;            // index of the deleted section

        public Delete(ProjectView.ProjectView view, SectionNode section): base(view)
        {
            mSection = section;
            mParent = section.Parent;
            mIndex = section.Index;
        }

        public override string getShortDescription() { return Localizer.Message("delete_section_command"); }

        /// <summary>
        /// Remove the section node.
        /// </summary>
        public override void execute()
        {
            base.execute();
            mSection.Detach();
            View.Selection = null;
        }

        /// <summary>
        /// Readd the section node.
        /// </summary>
        public override void unExecute()
        {
            mParent.Insert(mSection, mIndex);
            base.unExecute();
        }
    }
}
