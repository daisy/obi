using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class MovePhrase: Command
    {
        private Project mProject;
        private CoreNode mNode;
        private Project.Direction mDirection;
        private Project.Direction mOtherDirection;

        public override string Label
        {
            get { return Localizer.Message("move_phrase_command_label"); }
        }

        public MovePhrase(Project project, CoreNode node, Project.Direction direction)
        {
            mProject = project;
            mNode = node;
            mDirection = direction;
            mOtherDirection = direction == Project.Direction.Backward ? Project.Direction.Forward : Project.Direction.Backward;
        }

        public override void Do()
        {
            mProject.MovePhraseNode(mNode, mDirection);
        }

        public override void Undo()
        {
            mProject.MovePhraseNode(mNode, mOtherDirection);
        }
    }
}
