using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class RemovePageNumber: Command
    {
        protected PhraseNode mNode;          // the phrase node
        protected PageProperty mPageProp;  // the removed page property
        
        public override string Label
        {
            get { return Localizer.Message("remove_page_command_label"); }
        }

        public RemovePageNumber(PhraseNode node)
        {
            mNode = node;
            mPageProp = mNode.PageProperty;
        }

        public override void Do()
        {
            mNode.Project.RemovePage(this, mNode);
        }

        public override void Undo()
        {
            mNode.setProperty(mPageProp);
            mNode.Project.SetPageNumber(this, mNode);
        }
    }
}
