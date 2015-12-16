using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class DeAssociateAnchorNode: Command
    {
        private EmptyNode m_Node;
        private EmptyNode m_AssociatedNode;
        

        public DeAssociateAnchorNode(ProjectView.ProjectView view, EmptyNode anchor):base ( view )
        {
            m_Node = anchor;
            m_AssociatedNode = anchor.AssociatedNode;
            
        }

        public DeAssociateAnchorNode(ProjectView.ProjectView view)
            : 
            this     ( view, (EmptyNode)view.Selection.Node)
        { }
            
        
        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            m_Node.AssociatedNode = null;
            if (UpdateSelection) View.SelectedBlockNode = m_Node;
        }

        public override void UnExecute()
        {
            m_Node.AssociatedNode = m_AssociatedNode;
            base.UnExecute();
        }
    


    }
}
