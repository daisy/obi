using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DeleteSectionNode : Command
    {
        private SectionNode mNode;  // the deleted node
        private CoreNode mParent;   // parent of the deleted node
        private int mIndex;         // index of the deleted node

        public override string Label
        {
            get { return Localizer.Message("delete_section_command_label"); }
        }

        public DeleteSectionNode(SectionNode node, CoreNode parent, int index)
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
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
