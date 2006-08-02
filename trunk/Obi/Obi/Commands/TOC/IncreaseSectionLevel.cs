using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Commands.TOC
{
    class IncreaseSectionLevel : Command
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
                return Localizer.Message("increase_section_level_command_label");
            }
        }

        public IncreaseSectionLevel(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.IncreaseNodeLevel(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoIncreaseNodeLevel(mNode, mParent, mIndex, mPosition);
        }
    }
}
