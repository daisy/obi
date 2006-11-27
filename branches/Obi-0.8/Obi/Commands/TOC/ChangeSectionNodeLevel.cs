using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DecreaseSectionNodeLevel : SectionNodeCommand
    {
        private int mChildCount;

        public override string Label
        {
            get
            {
                return Localizer.Message("decrease_section_level_command_label");
            }
        }

        public DecreaseSectionNodeLevel(Project project, CoreNode node, CoreNode parent, int index, int position, int numChildren)
            : base(project, node, parent, index, position)
        {
            mChildCount = numChildren;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            Project.DecreaseSectionNodeLevel(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            Project.UndoDecreaseSectionNodeLevel(Node, Parent, Index, Position, mChildCount);
        }
    }

    class IncreaseSectionNodeLevel : SectionNodeCommand
    {
        public override string Label
        {
            get
            {
                return Localizer.Message("increase_section_level_command_label");
            }
        }

        public IncreaseSectionNodeLevel(Project project, CoreNode node, CoreNode parent, int index, int position)
        : base(project, node, parent, index, position)
        {
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            Project.IncreaseSectionNodeLevel(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            Project.UndoIncreaseSectionNodeLevel(Node, Parent, Index, Position);
        }
    }
}
