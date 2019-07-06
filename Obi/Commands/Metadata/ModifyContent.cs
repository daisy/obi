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
            //mPreviousContent = mEntry.getContent();
            mPreviousContent = mEntry.NameContentAttribute.Value;//sdk2
            mNewContent = content;
            SetDescriptions(Localizer.Message("modify_metadata_content"));
        }
        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Presentation.SetMetadataEntryContent(mEntry, mNewContent);
        }

        public override void UnExecute()
        {
            View.Presentation.SetMetadataEntryContent(mEntry, mPreviousContent);
            base.UnExecute();
        }
    }
}
