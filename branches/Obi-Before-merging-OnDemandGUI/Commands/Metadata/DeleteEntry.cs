using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Metadata
{
    class DeleteEntry: Command
    {
        private urakawa.metadata.Metadata mEntry;   // the entry to delete
        private MetadataSelection mSelectionAfter;  // selection after the deletion

        public DeleteEntry(ProjectView.ProjectView view)
            : base(view)
        {
            mEntry = ((MetadataSelection)view.Selection).Item.Entry;
            mSelectionAfter = null;
            Label = Localizer.Message("delete_metadata_entry");
        }

        public override void execute()
        {
            View.Presentation.DeleteMetadata(mEntry);
            View.Selection = mSelectionAfter;
        }

        public override void unExecute()
        {
            View.Presentation.AddMetadata(mEntry);
            base.unExecute();
        }
    }
}
