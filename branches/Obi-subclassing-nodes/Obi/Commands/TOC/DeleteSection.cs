using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DeleteSectionNode : Command
    {
        protected SectionNode mNode;    // the deleted section node
        protected SectionNode mParent;  // parent of the deleted node
        protected int mIndex;           // index of the deleted node

        public override string Label
        {
            get { return Localizer.Message("delete_section_command_label"); }
        }

        public DeleteSectionNode(SectionNode node)
        {
            mNode = node;
            mParent = node.ParentSection;
            mIndex = node.SectionIndex;
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.RemoveNode(mNode.Project, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndeleteSectionNode(mNode, mParent, mIndex);
        }
    }
}
