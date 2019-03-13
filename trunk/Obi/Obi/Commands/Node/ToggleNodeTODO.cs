using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    class ToggleNodeTODO : Command
    {
        private EmptyNode mNode;         // the empty node
        private bool mOriginalStatus;  // original used status of the node
        private double m_ToDoTime; // time in phrase at which Todo is marked

        /// <summary>
        /// Change the used status of a single node.
        /// </summary>
        public ToggleNodeTODO (ProjectView.ProjectView view, EmptyNode node)
            : this (view, node, 0)
            {
    }

        /// <summary>
        /// Change the used status of a single node.
        /// </summary>
        public ToggleNodeTODO (ProjectView.ProjectView view, EmptyNode node, double time)
            : base(view)
        {
            mNode = node;
            mOriginalStatus = node.TODO;
            m_ToDoTime = time;
            SetDescriptions(Localizer.Message("toggle_TODO"));
        }

        public override bool CanExecute { get { return true; } }
        
        
        public override void Execute()
        {
            mNode.SetTODO ( !mOriginalStatus, m_ToDoTime);
        }

        public override void UnExecute()
        {
            mNode.SetTODO(mOriginalStatus, m_ToDoTime);
            base.UnExecute();
        }


    }

    class AddComment: Command
    {
        private EmptyNode mNode;         // the empty node
        private string m_Comment ; // Comment on the node
        private string m_CommentPrevious;

        /// <summary>
        /// Add comment to a node.
        /// </summary>
        public AddComment(ProjectView.ProjectView view, EmptyNode node, string comment)
            :base(view)
        {
            mNode = node;
            m_CommentPrevious = node.CommentText;
            m_Comment = comment;
            //SetDescriptions(Localizer.Message("toggle_TODO"));
        }

        public override bool CanExecute { get { return true; } }


        public override void Execute()
        {
            mNode.CommentText = m_Comment ;
        }

        public override void UnExecute()
        {
            mNode.CommentText = m_CommentPrevious;
            base.UnExecute();
        }


    }

}
