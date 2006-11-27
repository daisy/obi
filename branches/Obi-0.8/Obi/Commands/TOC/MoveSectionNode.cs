using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class MoveSectionNodeDown : SectionNodeCommand
    {
        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_down_command_label");
            }
        }

        public MoveSectionNodeDown(Project project, CoreNode node, CoreNode parent, int index, int position)
            : base(project, node, parent, index, position)
        {
           
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            Project.MoveSectionNodeDown(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            Project.UndoMoveSectionNode(Node, Parent, Index, Position);
        }
    }

    class MoveSectionNodeUp : SectionNodeCommand
    {
        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_up_command_label");
            }
        }

        public MoveSectionNodeUp(Project project, CoreNode node, CoreNode parent, int index, int position)
            : base(project, node, parent, index, position)
        {
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            Project.MoveSectionNodeUp(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            Project.UndoMoveSectionNode(Node, Parent, Index, Position);
        }
    }

    class MoveSectionNodeDownLinear : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mSwapNode;
      
        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_down_linear_command_label");
            }
        }

        public MoveSectionNodeDownLinear(Project project, CoreNode node, CoreNode swapNode)
        {
            mProject = project;
            mNode = node;
            mSwapNode = swapNode;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.MoveSectionNodeDownLinear(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mProject.UndoShallowSwapNodes(mSwapNode, mNode);
        }
    }

    class MoveSectionNodeUpLinear : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mSwapNode;

        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_up_linear_command_label");
            }
        }

        public MoveSectionNodeUpLinear(Project project, CoreNode node, CoreNode swapNode)
        {
            mProject = project;
            mNode = node;
            mSwapNode = swapNode;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.MoveSectionNodeUpLinear(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mProject.UndoShallowSwapNodes(mSwapNode, mNode);
        }
    }
}
