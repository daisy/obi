using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class AssignRole : Command
    {
        protected EmptyNode mNode;          // the concerned node
        private EmptyNode.Role mRole;       // the new node kind
        private string mCustomRole;         // the new custom class
        private EmptyNode.Role mOldRole;    // the old node kind 
        private string mOldCustomRole;      // the old custom class
        private PageNumber mOldPageNumber;  // old page number (if previous kind was page)

        /// <summary>
        /// Change the type (either regular kind or custom type) of a node.
        /// </summary>
        public AssignRole(ProjectView.ProjectView view, EmptyNode node, EmptyNode.Role role, string customRole)
            : base(view)
        {
            mNode = node;
            mRole = role;
            mCustomRole = customRole;
            mOldRole = mNode.Role_;
            mOldCustomRole = mNode.CustomRole;
            mOldPageNumber = mNode.PageNumber;
            SetDescriptions(Localizer.Message("assign_role"));
        }

        /// <summary>
        /// Set a custom class on a node.
        /// </summary>
        public AssignRole(ProjectView.ProjectView view, EmptyNode node, string customRole)
            : this(view, node, EmptyNode.Role.Custom, customRole) {}

        /// <summary>
        /// Set a kind on the node.
        /// </summary>
        public AssignRole(ProjectView.ProjectView view, EmptyNode node, EmptyNode.Role role)
            : this(view, node, role, null) { }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            mNode.SetRole(mRole, mCustomRole);
            if (mOldRole == EmptyNode.Role.Custom) View.Presentation.RemoveCustomClass(mOldCustomRole, mNode);
            if (mRole == EmptyNode.Role.Custom) View.Presentation.AddCustomClass(mCustomRole, mNode);
            if (UpdateSelection) View.SelectedBlockNode = mNode;
        }

        public override void UnExecute()
        {
            if (mOldRole == EmptyNode.Role.Page) mNode.PageNumber = mOldPageNumber;
            if (mRole == EmptyNode.Role.Custom) View.Presentation.RemoveCustomClass(mCustomRole, mNode);
            mNode.SetRole(mOldRole, mOldCustomRole);
            base.UnExecute();
        }
    }

    public class SetPageNumber : AssignRole
    {
        private PageNumber mPageNumber;

        public SetPageNumber(ProjectView.ProjectView view, EmptyNode node, PageNumber pageNumber)
            : base(view, node, EmptyNode.Role.Page)
        {
            mPageNumber = pageNumber;
            SetDescriptions(Localizer.Message("set_page_number_"));
        }

        public override void Execute()
        {
            mNode.PageNumber = mPageNumber;
            base.Execute();
        }
    }
}
