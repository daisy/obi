using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class RemovePageNumber: Command
    {
        protected Obi.Project mProject;    // the current project
        protected CoreNode mNode;          // the phrase node
        protected PageProperty mPageProp;  // the removed page property
        
        public override string Label
        {
            get { return Localizer.Message("remove_page_command_label"); }
        }

        public RemovePageNumber(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPageProp = (PageProperty)mNode.getProperty(typeof(PageProperty));
        }

        public override void Do()
        {
            mProject.RemovePage(this, mNode);
        }

        public override void Undo()
        {
            mNode.setProperty(mPageProp);
            mProject.SetPageNumber(this, new Events.Node.NodeEventArgs(this, mNode));
        }
    }
}
