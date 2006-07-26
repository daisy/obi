using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Commands.TOC
{
    class Rename: Command
    {
        private Project mProject;
        private CoreNode mNode;
        private string mOldName;
        private string mNewName;

        public override string Label
        {
            get
            {
                return Localizer.Message("rename_command_label");
            }
        }

        public Rename(Project project, CoreNode node, string oldName, string newName)
        {
            mProject = project;
            mNode = node;
            mOldName = oldName;
            mNewName = newName;
        }

        public override void Do()
        {
            mProject.RenameNode(mProject, mNode, mNewName);
        }

        public override void Undo()
        {
            mProject.RenameNode(mProject, mNode, mOldName);
        }
    }
}
