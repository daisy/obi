using System;

namespace Obi.Commands.TOC
{
    public class MoveSectionIn: Command
    {
        private SectionNode mSection;

        public MoveSectionIn(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            SetDescriptions(Localizer.Message("move_section_in"));
            mSection = section;
        }

        /// <summary>
        /// Check whether a (possibly null) section can be moved in.
        /// It must have a preceding sibling.
        /// </summary>
        public static bool CanMoveNode(SectionNode section)
        {
            return section != null && section.IsRooted && section.Index > 0;
        }

        /// <summary>
        /// Increase the level of the section.
        /// </summary>
        public static void Move(SectionNode section)
        {
            SectionNode sibling = section.PrecedingSibling;
            section.Detach();
            sibling.AppendChild(section);
            int children = section.SectionChildCount;
            for (int i = 0; i < children; ++i)
            {
                SectionNode child = section.SectionChild(0);
                child.Detach();
                sibling.AppendChild(child);
            }
        }

        //sdk2
        //public override string getShortDescription() { return Localizer.Message("move_section_in"); }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            Move(mSection);
            View.SelectedSectionNode = mSection;
        }

        public override void UnExecute()
        {
            MoveSectionOut.Move(mSection);
            base.UnExecute();
        }
    }
}
