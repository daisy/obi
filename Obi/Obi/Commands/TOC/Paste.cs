using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class Paste: Command
    {
        private SectionNode mSection;    // the section after which to paste
        private SectionNode mCopy;       // copy of the node that actually gets pasted
        private SectionNode mClipboard;  // the section in the clipboard

        public Paste(ProjectView.ProjectView view, SectionNode section)
            : base(view)
        {
            mSection = section;
            mClipboard = (SectionNode)view.Clipboard;
            mCopy = (SectionNode)mClipboard.copy(true, true);
        }

        public override string getShortDescription() { return Localizer.Message("paste_section_command"); }

        public override void execute()
        {
            base.execute();
            mSection.Parent.InsertAfter(mCopy, mSection);
            View.SelectInTOCView(mCopy);
        }

        public override void unExecute()
        {
            mCopy.Detach();
            base.unExecute();
        }
    }
}
