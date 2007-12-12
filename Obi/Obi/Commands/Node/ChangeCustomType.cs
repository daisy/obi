using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class ChangeCustomType : Command
    {
        protected EmptyNode mNode;            // the concerned node
        private EmptyNode.Kind mNodeKind;     // the new node kind
        private string mCustomClass;          // the new custom class
        private EmptyNode.Kind mOldNodeKind;  // the old node kind 
        private string mOldCustomClass;       // the old custom class
        private int mOldPageNumber;           // old page number (if previous kind was page)

        /// <summary>
        /// Change the type (either regular kind or custom type) of a node.
        /// </summary>
        public ChangeCustomType(ProjectView.ProjectView view, EmptyNode node, EmptyNode.Kind nodeKind, string customClass)
            : base(view)
        {
            mNode = node;
            mNodeKind = nodeKind;
            mCustomClass = customClass;
            mOldNodeKind = mNode.NodeKind;
            mOldCustomClass = mNode.CustomClass;
            mOldPageNumber = mNode.PageNumber;
            Label = Localizer.Message("change_role");
        }

        /// <summary>
        /// Set a custom class on a node.
        /// </summary>
        public ChangeCustomType(ProjectView.ProjectView view, EmptyNode node, string customClass)
            : this(view, node, EmptyNode.Kind.Custom, customClass) {}

        /// <summary>
        /// Set a kind on the node.
        /// </summary>
        public ChangeCustomType(ProjectView.ProjectView view, EmptyNode node, EmptyNode.Kind nodeKind)
            : this(view, node, nodeKind, null) { }

        public override void execute()
        {
            mNode.SetKind(mNodeKind, mCustomClass);
            View.SelectedBlockNode = mNode;
            base.execute();
        }

        public override void unExecute()
        {
            if (mOldNodeKind == EmptyNode.Kind.Page) mNode.PageNumber = mOldPageNumber;
            mNode.SetKind(mOldNodeKind, mOldCustomClass);
            base.unExecute();
        }
    }

    public class SetPageNumber : ChangeCustomType
    {
        private int mPageNumber;

        public SetPageNumber(ProjectView.ProjectView view, EmptyNode node, int pageNumber)
            : base(view, node, EmptyNode.Kind.Page)
        {
            mPageNumber = pageNumber;
            Label = Localizer.Message("set_page_number");
        }

        public override void execute()
        {
            mNode.PageNumber = mPageNumber;
            base.execute();
        }
    }
}
