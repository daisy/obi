using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace Obi.Commands
{
    /// <summary>
    /// Add a custom type to the presentation
    /// </summary>
    class AddCustomType : Command
    {
        private ObiPresentation mPresentation;
        private string mCustomType;

        public AddCustomType(ProjectView.ProjectView view, ObiPresentation presentation, string customType)
            : base(view)
        {
            mPresentation = presentation;
            mCustomType = customType;
        }

        public override void Execute()
        {
            mPresentation.AddCustomClass(mCustomType, null);
        }

        public override bool CanExecute { get { return true; } }

        public override void UnExecute()
        {
            mPresentation.RemoveCustomClass(mCustomType, null);
        }
    }

    /// <summary>
    /// This removes a custom type from the list of custom types in the presentation.
    /// </summary>
    class RemoveCustomType : Command
    {
        ObiPresentation mPresentation;
        string mCustomType;

        public RemoveCustomType(ProjectView.ProjectView view, ObiPresentation presentation, string customType) : base(view)
        {
            mPresentation = presentation;
            mCustomType = customType;
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            mPresentation.RemoveCustomClass(mCustomType, null);
        }

        public override void UnExecute()
        {
            mPresentation.AddCustomClass(mCustomType, null);
        }
    }
}
