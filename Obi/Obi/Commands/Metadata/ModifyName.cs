using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Metadata
{
    class ModifyName: Command
    {
        private urakawa.metadata.Metadata mEntry;
        private string mPreviousName;
        private string mNewName;

        public ModifyName(ProjectView.ProjectView view, urakawa.metadata.Metadata entry, string name)
            : base(view)
        {
            mEntry = entry;
            mPreviousName = mEntry.getName();
            mNewName = name;
            Label = Localizer.Message("modify_metadata_name");
        }

        public override void execute()
        {
            View.Presentation.SetMetadataEntryName(mEntry, mNewName);
        }

        public override void unExecute()
        {
            View.Presentation.SetMetadataEntryName(mEntry, mPreviousName);
            base.unExecute();
        }
    }
}
