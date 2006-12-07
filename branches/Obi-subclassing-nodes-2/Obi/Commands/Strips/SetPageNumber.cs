using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SetNewPageNumber: Command
    {
        protected PhraseNode mNode;      // the phrase node
        protected PageProperty mPageProp;  // the page property

        public override string Label
        {
            get { return Localizer.Message("set_page_command_label"); }
        }

        public SetNewPageNumber(PhraseNode node)
        {
            mNode = node;
            mPageProp = node.PageProperty;
        }

        public override void Do()
        {
            mNode.PageProperty = mPageProp;
            mNode.Project.SetPageNumber(this, new Events.Node.PhraseNodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mNode.Project.RemovePage(this, mNode);
        }
    }

    public class SetPageNumber : SetNewPageNumber
    {
        private int mPrevPageNumber;  // previous page number
        private int mPageNumber;      // current page number

        public SetPageNumber(PhraseNode node, int prev)
            : base(node)
        {
            mPrevPageNumber = prev;
            mPageNumber = mPageProp.PageNumber;
        }

        public override void Do()
        {
            mPageProp.PageNumber = mPageNumber;
            mNode.Project.SetPageNumber(this, new Events.Node.PhraseNodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mPageProp.PageNumber = mPrevPageNumber;
            mNode.Project.SetPageNumber(this, new Events.Node.PhraseNodeEventArgs(this, mNode));
        }
    }
}
