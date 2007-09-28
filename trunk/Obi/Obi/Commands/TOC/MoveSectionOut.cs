using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class MoveSectionOut: Command
    {
        private SectionNode mSection;
        private SectionNode mParent;
        private int mIndex;
        private List<SectionNode> mSiblings;

        public MoveSectionOut(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mParent = section.ParentSection;
            if (mParent == null) throw new Exception("Cannot move out top-level section");
            mIndex = mSection.Index;
            mSiblings = new List<SectionNode>(mParent.SectionChildCount - 1 - mIndex);
            for (int i = mIndex + 1; i < mParent.SectionChildCount; ++i) mSiblings.Add(mParent.SectionChild(i));
        }

        public override string getShortDescription() { return Localizer.Message("move_section_out_command"); }

        public override void execute()
        {
            base.execute();
            mParent.removeChild(mSection);
            foreach (SectionNode sibling in mSiblings) mParent.removeChild(sibling);
            urakawa.core.TreeNode parent = mParent.getParent();
            if (parent is SectionNode) ((SectionNode)parent).AddChildSection(mSection, mParent.Index + 1);
            else parent.insert(mSection, mParent.Index + 1);
            foreach (SectionNode sibling in mSiblings) mSection.AppendChildSection(sibling);
            View.SelectInTOCView(mSection);
        }

        public override void unExecute()
        {
            foreach (SectionNode sibling in mSiblings) mSection.removeChild(sibling);
            mParent.getParent().removeChild(mSection);
            mParent.AddChildSection(mSection, mIndex);
            foreach (SectionNode sibling in mSiblings) mParent.AppendChildSection(sibling);
            base.unExecute();
        }
    }
}
