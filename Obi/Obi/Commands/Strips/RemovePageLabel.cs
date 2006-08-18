using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class RemovePageLabel: Command
    {
        protected Obi.Project mProject;  // the current project
        protected CoreNode mNode;        // the phrase node
        protected CoreNode mPageNode;    // page node
        
        public override string Label
        {
            get { return Localizer.Message("remove_page_command_label"); }
        }

        public RemovePageLabel(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPageNode = Project.GetStructureNode(node);
        }

        public override void Do()
        {
            mProject.RemovePage(this, mNode);
        }

        public override void Undo()
        {
            mNode.appendChild(mPageNode);
            mProject.SetPageLabel(this, new Events.Node.NodeEventArgs(this, mNode));
        }
    }
}
