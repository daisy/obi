using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.Strips
{
    public class SetNewPageLabel: Command
    {
        protected Obi.Project mProject;  // the current project
        protected CoreNode mNode;        // the phrase node
        protected CoreNode mPageNode;    // page node

        public override string Label
        {
            get { return Localizer.Message("set_page_command_label"); }
        }

        public SetNewPageLabel(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mPageNode = Project.GetStructureNode(node);
        }

        public override void Do()
        {
            mNode.appendChild(mPageNode);
            mProject.SetPageLabel(this, new Events.Node.NodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mProject.RemovePage(this, mNode);
        }
    }

    public class SetPageLabel : SetNewPageLabel
    {
        private string mPrevLabel;
        private string mLabel;

        public SetPageLabel(Obi.Project project, CoreNode node, string prev)
            : base(project, node)
        {
            mPrevLabel = prev;
            mLabel = Project.GetTextMedia(mPageNode).getText();
        }

        public override void Do()
        {
            Project.GetTextMedia(mPageNode).setText(mLabel);
            mProject.SetPageLabel(this, new Events.Node.NodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            Project.GetTextMedia(mPageNode).setText(mPrevLabel);
            mProject.SetPageLabel(this, new Events.Node.NodeEventArgs(this, mNode));
        }
    }
}
