using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class CopyPhrase: Command
    {
        private Project mProject;    // current project
        private CoreNode mNode;      // current node
        private CoreNode mPrevNode;  // previous node in the clipboard

        public override string Label
        {
            get { return Localizer.Message("copy_phrase_command_label"); }
        }

        public CopyPhrase(Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPrevNode = mProject.BlockClipBoard;
        }

        public override void Do()
        {
            mProject.BlockClipBoard = mNode;
        }

        public override void Undo()
        {
            mProject.BlockClipBoard = mPrevNode;
        }
    }
}
