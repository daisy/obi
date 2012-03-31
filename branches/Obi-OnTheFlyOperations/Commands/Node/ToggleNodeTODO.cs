using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    class ToggleNodeTODO : Command
    {
        private EmptyNode mNode;         // the empty node
        private bool mOriginalStatus;  // original used status of the node

        /// <summary>
        /// Change the used status of a single node.
        /// </summary>
        public ToggleNodeTODO (ProjectView.ProjectView view, EmptyNode node)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.TODO;
            SetDescriptions(Localizer.Message("toggle_TODO"));
        }

        public override bool CanExecute { get { return true; } }
        
        
        public override void Execute()
        {
            mNode.SetTODO ( !mOriginalStatus );
        }

        public override void UnExecute()
        {
            mNode.SetTODO(mOriginalStatus);
            base.UnExecute();
        }


    }
}
