using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Metadata
{
    class DeleteEntry: Command
    {
        private urakawa.metadata.Metadata mEntry;

        public DeleteEntry(ProjectView.ProjectView view)
            : base(view)
        {
            mEntry = ((MetadataSelection)view.Selection).Panel.Entry;
            Label = Localizer.Message("delete_metadata_entry");
        }

        public override void execute()
        {
            View.Presentation.DeleteMetadata(mEntry);
        }

        public override void unExecute()
        {
            View.Presentation.AddMetadata(mEntry);
            base.unExecute();
        }
    }
}
