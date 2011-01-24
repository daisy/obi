using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class MoveSectionOut: Command
    {
        private SectionNode mSection;         // section that changes level

        public MoveSectionOut(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            SetDescriptions(Localizer.Message("move_section_out"));
            mSection = section;
        }

        /// <summary>
        /// Check whether a (possibly null) section can be moved out.
        /// </summary>
        public static bool CanMoveNode(SectionNode section)
        {
            return section != null && section.ParentAs<SectionNode>() != null && section.SectionChildCount == 0;
        }

        /// <summary>
        /// Increase the level of the section.
        /// </summary>
        public static void Move(SectionNode section)
        {
            int index = section.Index + 1;
            SectionNode sibling = section.ParentAs<SectionNode>();
            int children = sibling.SectionChildCount;
            for (int i = index; i < children; ++i)
            {
                SectionNode child = sibling.SectionChild(index);
                child.Detach();
                section.AppendChild(child);
            }
            section.Detach();
            sibling.ParentAs<ObiNode>().InsertAfter(section, sibling);
        }

        //sdk2
        //public override string getShortDescription() { return SetDescriptions(Localizer.Message("move_section_out"));; }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            Move(mSection);
            View.SelectedSectionNode = mSection;
        }

        public override void UnExecute()
        {
            MoveSectionIn.Move(mSection);
            for (int i = 0; i < mSection.SectionChildCount; ++i) MoveSectionIn.Move(mSection.SectionChild(i));
            base.UnExecute();
        }
    }
}
