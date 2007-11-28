using System;
using System.Collections.Generic;
using System.Text;
using urakawa.undo;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Add a custom type
    /// </summary>
    class AddCustomType : Command
    {
        Presentation mPresentation;
        string mCustomType;

        public AddCustomType(ProjectView.ProjectView view, Presentation presentation, string customType) : base(view)
        {
            mPresentation = presentation;
            mCustomType = customType;
        }

        public override void execute()
        {
            mPresentation.AddCustomType(mCustomType);
        }
        public override void unExecute()
        {
            mPresentation.RemoveCustomType(mCustomType);
        }

    }

    /// <summary>
    /// This removes a custom type from the list of custom types.
    /// </summary>
    class RemoveCustomType : Command
    {
        Presentation mPresentation;
        string mCustomType;

        public RemoveCustomType(ProjectView.ProjectView view, Presentation presentation, string customType) : base(view)
        {
            mPresentation = presentation;
            mCustomType = customType;
        }

        public override void execute()
        {
            mPresentation.RemoveCustomType(mCustomType);
        }
        public override void unExecute()
        {
            mPresentation.AddCustomType(mCustomType);
        }

    }
}
