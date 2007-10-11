using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.undo;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// Add a new section or subsection, then let the user rename it.
    /// </summary>
    public class AddNewSection: Command
    {
        private ObiNode mParent;    // parent of the new section (section or root node)
        private int mIndex;         // index of the new section
        private SectionNode mNode;  // the node for the new section

        /// <summary>
        /// Create a new add section command to add a new section after the context node
        /// and at the same level (or as last child of the root if the context node is null)
        /// The node is selected and the user can start renaming it.
        /// </summary>
        public AddNewSection(ProjectView.ProjectView view, NodeSelection selection)
            : base(view)
        {
            ObiNode contextNode = selection == null || !(selection.Node is SectionNode) ? null : selection.Node;
            if (selection != null && selection.IsDummy)
            {
                mParent = contextNode;
                mIndex = contextNode is SectionNode ? ((SectionNode)contextNode).SectionChildCount : contextNode.getChildCount();
            }
            else
            {
                mParent = contextNode == null ? View.Project.RootNode : contextNode.Parent;
                mIndex = contextNode == null ? View.Project.RootNode.getChildCount() : contextNode.Index + 1;
            }
            mNode = View.Project.NewSectionNode();
            View.SelectAndRenameNodeInTOCView(mNode);
        }

        public override string getShortDescription() { return Localizer.Message("add_new_section_command"); }

        /// <summary>
        /// Add or readd the new section node then restore this as the selection.
        /// </summary>
        public override void execute()
        {
            base.execute();
            mParent.Insert(mNode, mIndex);
            View.SelectInTOCView(mNode);
        }

        /// <summary>
        /// Remove the section node.
        /// </summary>
        public override void unExecute()
        {
            mNode.Detach();
            base.unExecute();
        }
    }
}