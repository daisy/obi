using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class Copy : Command
    {
        private SectionNode mSection;
        private ObiNode mClipboard;

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
