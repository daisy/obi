using System;

namespace Obi.Commands.Node
{
    public class ToggleNodeUsed: Command
    {
        private ObiNode mNode;         // the section node
        private bool mOriginalStatus;  // original used status of the node

        /// <summary>
        /// Change the used status of a single node.
        /// </summary>
        public ToggleNodeUsed(ProjectView.ProjectView view, ObiNode node)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.Used;
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            mNode.Used = !mOriginalStatus;
        }

        public override void UnExecute()
        {
            mNode.Used = mOriginalStatus;
            base.UnExecute();
        }
    }
}
