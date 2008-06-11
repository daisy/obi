using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    class ToggleNodeTo_Do : Command
    {
        private EmptyNode mNode;         // the empty node
        private bool mOriginalStatus;  // original used status of the node

        /// <summary>
        /// Change the used status of a single node.
        /// </summary>
        public ToggleNodeTo_Do (ProjectView.ProjectView view, EmptyNode node)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.IsTo_Do;
        }

        public override void execute()
        {
            mNode.AssignTo_DoMark ( !mOriginalStatus );
        }

        public override void unExecute()
        {
            mNode.AssignTo_DoMark(mOriginalStatus);
            base.unExecute();
        }


    }
}
