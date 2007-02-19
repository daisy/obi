using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SetPageNumber: Command
    {
        protected PhraseNode mNode;  // the phrase node

        public override string Label
        {
            get { return Localizer.Message("set_page_command_label"); }
        }

        /// <summary>
        /// Issue a page number command after the page was set.
        /// </summary>
        /// <param name="node">The node on which the page was set.</param>
        public SetPageNumber(PhraseNode node)
        {
            mNode = node;
        }

        /// <summary>
        /// Set the page number again on the phrase.
        /// </summary>
        public override void Do()
        {
            mNode.Project.DidSetPageNumberOnPhrase(mNode);
        }

        /// <summary>
        /// Remove the page number from the phrase.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.DidRemovePageNumberFromPhrase(mNode);
        }
    }
}
