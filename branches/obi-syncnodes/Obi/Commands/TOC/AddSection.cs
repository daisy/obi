using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class AddSectionNode: Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;
        private string mOriginalLabel;

        public override string Label
        {
            get
            {
                return Localizer.Message("add_section_command_label");
            }
        }

        public AddSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
            mOriginalLabel = Project.GetTextMedia(node).getText();
        }

        /// <summary>
        /// Do: add the node to the project; it will send the synchronization events.
        /// This is really redo, so the node exists and so does its parent.
        /// </summary>
        public override void Do()
        {
            mProject.AddExistingSectionNode(mNode, mParent, mIndex, mPosition, mOriginalLabel);
        }

        /// <summary>
        /// Undo: remove the node from the project; it will send the synchronization events.
        /// This node has no descendant when we undo.
        /// </summary>
        public override void Undo()
        {
            mProject.RemoveNode(mProject, mNode);
        }
    }
}
