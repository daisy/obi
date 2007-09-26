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
        private TreeNode mParent;   // parent of the new section (section or root node)
        private int mIndex;         // index of the new section
        private SectionNode mNode;  // the node for the new section

        /// <summary>
        /// Create a new add section command to add a new section after the context node
        /// and at the same level (or as last child of the root if the context node is null)
        /// The node is selected and the user can start renaming it.
        /// </summary>
        /// <param name="view">The view in which the command is to be executed.</param>
        /// <param name="asChild">If true, add a subsection of the selected section.</param>
        public AddNewSection(ProjectView.ProjectView view, TreeNode contextNode, bool asChild)
            : base(view)
        {
            if (asChild)
            {
                if (contextNode == null) throw new Exception("Cannot add as child with no parent node!");
                mParent = contextNode;
                mIndex = contextNode.getChildCount();
            }
            else
            {
                mParent = contextNode == null ? View.Project.RootNode : contextNode.getParent();
                mIndex = contextNode == null ? View.Project.RootNode.getChildCount() : mParent.indexOf(contextNode) + 1;
            }
            mNode = View.Project.NewSectionNode();
            view.SelectAndRenameNodeInTOCView(mNode);
        }
        
        /// <summary>
        /// Create a new add section command to add a new section after the context node
        /// and at the same level (or as last child of the root if the context node is null)
        /// The node is selected and the user can start renaming it.
        /// </summary>
        /// <param name="view">The view in which the command is to be executed.</param>
        public AddNewSection(ProjectView.ProjectView view, TreeNode contextNode)
            : this(view, contextNode, false)
        {
        }

        public override string getShortDescription() { return Localizer.Message("add_new_section_command"); }

        /// <summary>
        /// Add or readd the new section node then restore this as the selection.
        /// </summary>
        public override void execute()
        {
            base.execute();
            mParent.insert(mNode, mIndex);
            View.SelectInTOCView(mNode);
        }

        /// <summary>
        /// Remove the section node.
        /// </summary>
        public override void unExecute()
        {
            mParent.removeChild(mIndex);
            base.unExecute();
        }
    }
}
