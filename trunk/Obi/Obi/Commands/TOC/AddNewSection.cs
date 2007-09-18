using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.undo;

namespace Obi.Commands.TOC
{
    public class AddNewSection: Command
    {
        private TreeNode mParent;
        private int mIndex;
        private SectionNode mNode;
        
        public AddNewSection(ProjectView.ProjectView view, TreeNode contextNode): base(view)
        {
            mParent = contextNode == null ? View.Project.RootNode : contextNode.getParent();
            mIndex = contextNode == null ? View.Project.RootNode.getChildCount() : mParent.indexOf(contextNode) + 1;
            mNode = View.Project.NewSectionNode();
        }

        public override void execute()
        {
            base.execute();
            mParent.insert(mNode, mIndex);
        }

        public override string getShortDescription() { return Localizer.Message("add_new_section_command"); }

        public override void unExecute()
        {
            mParent.removeChild(mIndex);
            base.unExecute();
        }
    }
}
