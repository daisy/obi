using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class MoveSectionIn: Command
    {
        private SectionNode mSection;

        public MoveSectionIn(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
        }

        /// <summary>
        /// Check whether a (possibly null) section can be moved in.
        /// It must have a preceding sibling.
        /// </summary>
        public static bool CanMoveNode(SectionNode section)
        {
            return section != null && section.Index > 0;
        }

        /// <summary>
        /// Increase the level of the section.
        /// </summary>
        public static void Move(SectionNode section)
        {
            SectionNode sibling = section.PrecedingSibling;
            section.getParent().removeChild(section);
            sibling.AddChildSection(section, sibling.SectionChildCount);
            for (int i = 0; i < section.SectionChildCount; ++i)
            {
                sibling.AddChildSection((SectionNode)section.removeChild(0), sibling.SectionChildCount);
            }
        }

        public override string getShortDescription() { return Localizer.Message("move_section_in_command"); }

        public override void execute()
        {
            base.execute();
            Move(mSection);
            View.SelectInTOCView(mSection);
        }

        public override void unExecute()
        {
            MoveSectionOut.Move(mSection);
            base.unExecute();
        }
    }
}
