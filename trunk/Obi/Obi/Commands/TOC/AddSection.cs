using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class AddSectionNode: SectionNodeCommand
    {
        private string mOriginalLabel;

        public override string Label
        {
            get
            {
                return Localizer.Message("add_section_command_label");
            }
        }

        public AddSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position)
        : base(project, node, parent, index, position)
        {
            mOriginalLabel = Project.GetTextMedia(node).getText();
        }

        /// <summary>
        /// Do: add the node to the project; it will send the synchronization events.
        /// This is really redo, so the node exists and so does its parent.
        /// </summary>
        public override void Do()
        {
            Project.AddExistingSectionNode(Node, Parent, Index, Position, mOriginalLabel);
        }

        /// <summary>
        /// Undo: remove the node from the project; it will send the synchronization events.
        /// This node has no descendant when we undo.
        /// </summary>
        public override void Undo()
        {
            Project.RemoveNode(Project, Node);
        }
    }
}
