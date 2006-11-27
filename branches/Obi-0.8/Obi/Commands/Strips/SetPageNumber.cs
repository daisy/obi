using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SetNewPageNumber: Command
    {
        protected Obi.Project mProject;    // the current project
        protected CoreNode mNode;          // the phrase node
        protected PageProperty mPageProp;  // the page property

        public override string Label
        {
            get { return Localizer.Message("set_page_command_label"); }
        }

        public SetNewPageNumber(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPageProp = (PageProperty)mNode.getProperty(typeof(PageProperty));
        }

        public override void Do()
        {
            mNode.setProperty(mPageProp);
            mProject.SetPageNumber(this, new Events.Node.NodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mProject.RemovePage(this, mNode);
        }
    }

    public class SetPageNumber : SetNewPageNumber
    {
        private int mPrevPageNumber;  // previous page number
        private int mPageNumber;      // current page number

        public SetPageNumber(Obi.Project project, CoreNode node, int prev)
            : base(project, node)
        {
            mPrevPageNumber = prev;
            mPageNumber = mPageProp.PageNumber;
        }

        public override void Do()
        {
            mPageProp.PageNumber = mPageNumber;
            mProject.SetPageNumber(this, new Events.Node.NodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mPageProp.PageNumber = mPrevPageNumber;
            mProject.SetPageNumber(this, new Events.Node.NodeEventArgs(this, mNode));
        }
    }
}
