using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class MoveSectionIn: Command
    {
        private SectionNode mSection;         // the section moving in
        private List<SectionNode> mChildren;  // its children

        public MoveSectionIn(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mChildren = new List<SectionNode>(mSection.SectionChildCount);
            for (int i = 0; i < mSection.SectionChildCount; ++i) mChildren.Add(mSection.SectionChild(i));
        }

        public override string getShortDescription() { return Localizer.Message("move_section_in_command"); }

        public override void execute()
        {
            base.execute();
            foreach (SectionNode child in mChildren) mSection.removeChild(child);
            SectionNode sibling = mSection.ParentSection.SectionChild(mSection.Index - 1);
            mSection.ParentSection.removeChild(mSection);
            sibling.AppendChildSection(mSection);
            foreach (SectionNode child in mChildren) sibling.AppendChildSection(child);
            View.SelectInTOCView(mSection);
        }

        public override void unExecute()
        {
            base.unExecute();
        }
    }
}
