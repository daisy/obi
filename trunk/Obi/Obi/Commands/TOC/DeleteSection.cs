using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    public class DeleteSectionNode : ListCommand
    {
        protected SectionNode mNode;  // the deleted section node
        protected TreeNode mParent;   // parent of the deleted node (root or section node)
        protected int mIndex;         // the index of where the node used to be
  
        public override string Label
        {
            get { return Localizer.Message("delete_section_command_label"); }
        }

        /// <summary>
        /// The command must be created *before* the section is actually deleted.
        /// </summary>
        /// <param name="node"></param>
        public DeleteSectionNode(SectionNode node)
        {
            mNode = node;
            mParent = (TreeNode)node.getParent();
            mIndex = node.Index;
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.RemoveSectionNode(mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.ReaddSectionNode(mNode, mParent, mIndex);
            base.Undo();
        }
    }
}
