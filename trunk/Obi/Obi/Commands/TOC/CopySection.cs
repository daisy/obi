using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class CopySection : Command
    {
        private Project mProject;
        private CoreNode mNode;

        public override string Label
        {
            get
            {
                return Localizer.Message("copy_section_command_label");
            }
        }

        public CopySection(Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
        }

        /// <summary>
        /// ReDo: uncut the node
        /// </summary>
        public override void Do()
        {
            mProject.CopyTOCNode(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoCopyTOCNode(mNode);
        }
    }
}
