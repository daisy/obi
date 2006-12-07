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
        private PhraseNode mNode;                      // the phrase node to move
        private PhraseNode.Direction mDirection;       // the direction in which to move it
        private PhraseNode.Direction mOtherDirection;  // the other direction (for undo)

        public override string Label
        {
            get { return Localizer.Message("move_phrase_command_label"); }
        }

        /// <summary>
        /// Create a new command.
        /// </summary>
        /// <param name="node">The phrase node to move.</param>
        /// <param name="direction">The direction in which to move it.</param>
        public MovePhrase(PhraseNode node, PhraseNode.Direction direction)
        {
            mNode = node;
            mDirection = direction;
            mOtherDirection = direction == PhraseNode.Direction.Backward ?
                PhraseNode.Direction.Forward : PhraseNode.Direction.Backward;
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
