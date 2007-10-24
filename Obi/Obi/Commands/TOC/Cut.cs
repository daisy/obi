using System;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// Cut a section node.
    /// </summary>
    // TODO: this should be generic
    public class Cut: Command
    {
        private SectionNode mSection;  // the cut section
        private ObiNode mParent;       // its original parent
        private int mIndex;            // its original index
        private ObiNode mClipboard;    // the previous contents of the clipboard

        public Cut(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mParent = section.ParentAs<ObiNode>();
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
