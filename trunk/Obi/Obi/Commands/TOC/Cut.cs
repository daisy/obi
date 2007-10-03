using System;

namespace Obi.Commands.TOC
{
    public class Cut: Command
    {
        private SectionNode mSection;
        private ObiNode mParent;
        private int mIndex;
        private ObiNode mClipboard;

        public Cut(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mParent = section.Parent;
            mIndex = section.Index;
            mClipboard = view.Clipboard;
        }

        public override string getShortDescription() { return Localizer.Message("cut_section_command"); }

        public override void execute()
        {
            base.execute();
            View.Clipboard = mSection;
            mSection.Detach();
            View.Selection = null;
        }

        public override void unExecute()
        {
            View.Clipboard = mClipboard;
            mParent.Insert(mSection, mIndex);
            base.unExecute();
        }
    }
}
