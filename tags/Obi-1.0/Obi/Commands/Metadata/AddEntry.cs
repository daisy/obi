using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Metadata
{
    class AddEntry: Command
    {
        private urakawa.metadata.Metadata mEntry;

        public AddEntry(ProjectView.ProjectView view)
            : base(view)
        {
            mEntry = view.Presentation.getMetadataFactory().createMetadata();
            Label = Localizer.Message("add_metadata_entry");
        }

        public AddEntry(ProjectView.ProjectView view, string name)
            : this(view)
        {
            mEntry.setName(name);
        }

        public urakawa.metadata.Metadata Entry { get { return mEntry; } }

        public override void execute()
        {
            View.Presentation.AddMetadata(mEntry);
        }

        public override void unExecute()
        {
            View.Presentation.DeleteMetadata(mEntry);
            base.unExecute();
        }
    }
}
