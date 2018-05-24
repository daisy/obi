using System;
using System.Collections.Generic;
using System.Text;
using urakawa.metadata;

namespace Obi.Commands.Metadata
{
    class AddEntry: Command
    {
        private urakawa.metadata.Metadata mEntry;

        public AddEntry(ProjectView.ProjectView view)
            : base(view)
        {
            mEntry = view.Presentation.MetadataFactory.CreateMetadata();
            mEntry.NameContentAttribute = new MetadataAttribute();
            SetDescriptions(Localizer.Message("add_metadata_entry"));
        }

        public AddEntry(ProjectView.ProjectView view, string name)
            : this(view)
        {
            mEntry.NameContentAttribute.Name = name;
        }

        public urakawa.metadata.Metadata Entry { get { return mEntry; } }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            View.Presentation.AddMetadata(mEntry);
        }

        public override void UnExecute()
        {
            View.Presentation.DeleteMetadata(mEntry);
            base.UnExecute();
        }
    }
}
