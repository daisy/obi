using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DecreaseSectionNodeLevel : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;
        private int mChildCount;

        public override string Label
        {
            get
            {
                return Localizer.Message("decrease_section_level_command_label");
            }
        }

        public DecreaseSectionNodeLevel(Project project, CoreNode node, CoreNode parent, int index, int position, int numChildren)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
            mChildCount = numChildren;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.DecreaseSectionNodeLevel(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoDecreaseSectionNodeLevel(mNode, mParent, mIndex, mPosition, mChildCount);
        }
    }

    class IncreaseSectionNodeLevel : Command
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

        public IncreaseSectionNodeLevel(Project project, CoreNode node, CoreNode parent, int index, int position)
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
            mProject.IncreaseSectionNodeLevel(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoIncreaseSectionNodeLevel(mNode, mParent, mIndex, mPosition);
        }
    }
}
