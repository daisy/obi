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
            mPreviousName = mEntry.NameContentAttribute.Name;
            mNewName = name;
            SetDescriptions(Localizer.Message("modify_metadata_name"));
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Presentation.SetMetadataEntryName(mEntry, mNewName);
        }

        public override void UnExecute()
        {
            View.Presentation.SetMetadataEntryName(mEntry, mPreviousName);
            base.UnExecute();
        }
    }
}
