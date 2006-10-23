using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// Command to move a phrase node inside its strip in one direction or the other.
    /// </summary>
    class MovePhrase: Command
    {
        private PhraseNode mNode;                   // the phrase node to move
        private Project.Direction mDirection;       // the direction in which to move it
        private Project.Direction mOtherDirection;  // the other direction (for undo)

        /// <summary>
        /// The label of this command for the undo menu.
        /// </summary>
        public override string Label
        {
            get { return Localizer.Message("move_phrase_command_label"); }
        }

        /// <summary>
        /// Create a new command.
        /// </summary>
        /// <param name="node">The phrase node to move.</param>
        /// <param name="direction">The direction in which to move it.</param>
        public MovePhrase(PhraseNode node, Project.Direction direction)
        {
            mNode = node;
            mDirection = direction;
            mOtherDirection = direction == Project.Direction.Backward ? Project.Direction.Forward : Project.Direction.Backward;
        }

        /// <summary>
        /// Do: move the node in the requested direction.
        /// </summary>
        public override void Do()
        {
            mNode.Project.MovePhraseNode(mNode, mDirection);
        }

        /// <summary>
        /// Undo: move the node in the opposite direction.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.MovePhraseNode(mNode, mOtherDirection);
        }
    }
}
