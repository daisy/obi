using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Commands.TOC
{
    class DeleteSection : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public override string Label
        {
            get
            {
                return Localizer.Message("delete_section_command_label");
            }
        }

        public DeleteSection(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mProject.RemoveNode(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndeleteSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }
}
