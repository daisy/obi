using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    class CopyPhrase: Command
    {
        private object mPrevious;  // the previous data in the clipboard
        private PhraseNode mNode;  // the phrase node that was copied

        public override string Label
        {
            get { return Localizer.Message("copy_phrase_command_label"); }
        }

        /// <summary>
        /// Create the command *before* updating the clipboard.
        /// </summary>
        /// <param name="node">Copy of the phrase node to be stored in the clipboard.</param>
        public CopyPhrase(PhraseNode node)
        {
            mNode = node;
            mPrevious = mNode.Project.Clipboard.Data;
        }

        /// <summary>
        /// Do: put the copy of the phrase in the clipboard.
        /// </summary>
        public override void Do()
        {
            mNode.Project.Clipboard.Phrase = mNode;
        }

        /// <summary>
        /// Undo: restore the previous data in the clipboard.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.Clipboard.Data = mPrevious;
        }
    }
}