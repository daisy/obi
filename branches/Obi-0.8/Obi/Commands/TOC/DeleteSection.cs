using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class DeleteSectionNode : SectionNodeCommand
    {
        public override string Label
        {
            get
            {
                return Localizer.Message("delete_section_command_label");
            }
        }

        public DeleteSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position)
        : base(project, node, parent, index, position)
        {
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            Project.RemoveNode(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            Project.UndeleteSectionNode(Node, Parent, Index, Position);
        }
    }
}
