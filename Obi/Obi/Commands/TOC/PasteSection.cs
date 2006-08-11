using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class PasteSection : Command
    {
        private Project mProject;
        private CoreNode mParent;
        private CoreNode mNode;

        public override string Label
        {
            get
            {
                return Localizer.Message("paste_section_command_label");
            }
        }

        public PasteSection(Project project, CoreNode node, CoreNode parent)
        {
            mProject = project;
            mParent = parent;
            mNode = node;
        }

      
        public override void Do()
        {
            mProject.PasteTOCNode(mProject, mParent);
        }

        public override void Undo()
        {
            mProject.RemoveNode(mProject, mNode);
        }
    }
}
