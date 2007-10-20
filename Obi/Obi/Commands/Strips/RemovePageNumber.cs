using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class RemovePageNumber: Command__OLD__
    {
        protected PhraseNode mNode;  // the phrase node
        
        public override string Label
        {
            get { return Localizer.Message("remove_page_command_label"); }
        }

        public RemovePageNumber(PhraseNode node)
        {
            mNode = node;
        }

        public override void Do()
        {
            //mNode.Project.DidRemovePageNumberFromPhrase(mNode);
        }

        public override void Undo()
        {
            //mNode.Project.DidSetPageNumberOnPhrase(mNode);
        }
    }
}
