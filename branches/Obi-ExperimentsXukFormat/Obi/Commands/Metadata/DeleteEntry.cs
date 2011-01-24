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
            SetDescriptions(Localizer.Message("delete_metadata_entry"));
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Presentation.DeleteMetadata(mEntry);
            View.Selection = mSelectionAfter;
        }

        public override void UnExecute()
        {
            View.Presentation.AddMetadata(mEntry);
            base.UnExecute();
        }
    }
}
