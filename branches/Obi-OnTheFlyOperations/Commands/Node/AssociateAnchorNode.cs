using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class AssociateAnchorNode : Command
    {
        private EmptyNode m_Node;
        private EmptyNode m_NodeToBeAssociated;
        private EmptyNode m_PreviouslyAssociatedNode;
        private EmptyNode.Role m_PreviouslyAssignedRoleToAnchor;
        private string m_PreviousCustomClass;
        private PageNumber m_PreviousPageNumber;

        public AssociateAnchorNode(ProjectView.ProjectView view, EmptyNode anchor, EmptyNode nodeToBeAssociated):base ( view )
        {
            m_Node = anchor;
            m_NodeToBeAssociated = nodeToBeAssociated;
            m_PreviouslyAssociatedNode = anchor.AssociatedNode;
            m_PreviouslyAssignedRoleToAnchor = anchor.Role_;
            if (anchor.Role_ == EmptyNode.Role.Page) m_PreviousPageNumber = anchor.PageNumber;
            if (anchor.Role_ == EmptyNode.Role.Custom) m_PreviousCustomClass = anchor.CustomRole;
        }

        public AssociateAnchorNode(ProjectView.ProjectView view, EmptyNode nodeToBeAssociated): 
            this     ( view, (EmptyNode)view.Selection.Node, nodeToBeAssociated )
        { }
            
        
        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            if (m_Node.Role_ != EmptyNode.Role.Anchor) m_Node.SetRole(EmptyNode.Role.Anchor, null);
            m_Node.AssociatedNode = m_NodeToBeAssociated ;
            
            if (UpdateSelection) View.SelectedBlockNode = m_Node;
        }

        public override void UnExecute()
        {   
            m_Node.AssociatedNode = m_PreviouslyAssociatedNode;
            m_Node.SetRole(m_PreviouslyAssignedRoleToAnchor, m_PreviouslyAssignedRoleToAnchor == EmptyNode.Role.Custom ? m_PreviousCustomClass : null);
            if (m_Node.Role_ == EmptyNode.Role.Page) m_Node.PageNumber = m_PreviousPageNumber;
                        base.UnExecute();
        }
    


    }
}
