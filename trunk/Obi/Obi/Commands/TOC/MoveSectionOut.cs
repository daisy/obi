using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class MoveSectionOut: Command
    {
        private SectionNode mSection;

        public MoveSectionOut(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
        }

        /// <summary>
        /// Check whether a (possibly null) section can be moved out.
        /// </summary>
        public static bool CanMoveNode(SectionNode section)
        {
            return section != null && section.ParentSection != null && section.SectionChildCount == 0;
        }

        /// <summary>
        /// Increase the level of the section.
        /// </summary>
        public static void Move(SectionNode section)
        {
            int index = section.Index + 1;
            SectionNode sibling = section.ParentSection;
            for (int i = index; i < sibling.SectionChildCount; ++i)
            {
                SectionNode child = sibling.SectionChild(index);
                child.Detach();
                section.Append(child);
            }
            section.Detach();
            sibling.Parent.InsertAfter(section, sibling);
        }

        public override string getShortDescription() { return Localizer.Message("move_section_out_command"); }

        public override void execute()
        {
            base.execute();
            Move(mSection);
            View.SelectInTOCView(mSection);
        }

        public override void unExecute()
        {
            MoveSectionIn.Move(mSection);
            base.unExecute();
        }
    }
}
