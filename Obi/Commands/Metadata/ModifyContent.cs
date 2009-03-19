using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Metadata
{
    class ModifyContent: Command
    {
        private urakawa.metadata.Metadata mEntry;
        private string mPreviousContent;
        private string mNewContent;

        public ModifyContent(ProjectView.ProjectView view, urakawa.metadata.Metadata entry, string content)
            : base(view)
        {
            mEntry = entry;
            mPreviousContent = mEntry.getContent();
            mNewContent = content;
            Label = Localizer.Message("modify_metadata_content");
        }

        public override void execute()
        {
            View.Presentation.SetMetadataEntryContent(mEntry, mNewContent);
        }

        public override void unExecute()
        {
            View.Presentation.SetMetadataEntryContent(mEntry, mPreviousContent);
            base.unExecute();
        }
    }
}
