using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Commands.TOC
{
    class AddSection: Command
    {
        private Project mProject;
        private CoreNode mNode;
        private int mPosition;
        private int mIndex;

        public AddSection(Project project, CoreNode node, int index, int position)
        {
            mProject = project;
            mNode = node;
            mIndex = index;
        }

        /// <summary>
        /// Do: add the node to the project; it will send the synchronization events.
        /// This is really redo, so the node exists and so does its parent.
        /// </summary>
        public override void Do()
        {
            mProject.RedoAddSection(mNode, mIndex, mPosition);
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
