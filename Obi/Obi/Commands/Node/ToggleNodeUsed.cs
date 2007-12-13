using System;

namespace Obi.Commands.Node
{
    public class ToggleNodeUsed: Command
    {
        private ObiNode mNode;         // the section node
        private bool mOriginalStatus;  // original used status of the node

        public ToggleNodeUsed(ProjectView.ProjectView view, ObiNode node)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.Used;
        }

        public override void execute()
        {
            ChangeUsedStatusDeep(mNode, !mOriginalStatus);
            View.Selection = new NodeSelection(mNode, SelectionBefore.Control);
            base.execute();
        }

        public override void unExecute()
        {
            ChangeUsedStatusDeep(mNode, mOriginalStatus);
            base.unExecute();
        }

        // Change the used status of a section node and all of its descendants
        private void ChangeUsedStatusDeep(ObiNode node, bool used)
        {
            if (node.Used != used)
            {
                node.Used = used;
                for (int i = 0; i < node.getChildCount(); ++i) ChangeUsedStatusDeep((ObiNode)node.getChild(i), used);
            }
        }
    }
}
