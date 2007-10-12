using System;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// Copy a section node.
    /// </summary>
    // TODO: this should be generic.
    public class Copy : Command
    {
        private SectionNode mSection;  // the copied section
        private ObiNode mClipboard;    // the previous contents of the clipboard

        public Copy(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mClipboard = view.Clipboard;
        }

        public override string getShortDescription() { return Localizer.Message("copy_section_command"); }

        public override void execute()
        {
            base.execute();
            View.Clipboard = mSection;
            View.SelectInTOCView(mSection);
        }

        public override void unExecute()
        {
            View.Clipboard = mClipboard;
            base.unExecute();
        }
    }
}