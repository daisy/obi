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
        private PhraseNode mNode;      // the phrase node to move
        private int mIndexBeforeMove;  // the original index of the node
        private int mIndexAfterMove;   // the new index after the move

        public override string Label
        {
            get { return Localizer.Message("move_phrase_command_label"); }
        }

        /// <summary>
        /// Create a new command *before* the move is done.
        /// </summary>
        /// <param name="node">The phrase node to move.</param>
        /// <param name="direction">The direction in which it was moved.</param>
        public MovePhrase(PhraseNode node, int indexAfterMove)
        {
            mNode = node;
            mIndexBeforeMove = mNode.Index;
            mIndexAfterMove = indexAfterMove;
        }

        /// <summary>
        /// Do: move the node in the requested direction.
        /// </summary>
        public override void Do()
        {
            mNode.Project.MovePhraseNodeTo(mNode, mIndexAfterMove);
        }

        /// <summary>
        /// Undo: move the node in the opposite direction.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.MovePhraseNodeTo(mNode, mIndexBeforeMove);
        }
    }
}
