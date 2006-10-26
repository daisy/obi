using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    public abstract class MoveSectionNode : Command
    {
        protected SectionNode mNode;
        protected CoreNode mParent;
        protected int mIndex;
        protected int mPosition;

        public MoveSectionNode(SectionNode node, CoreNode parent)
        {
            mNode = node;
            mParent = parent;
            mIndex = node.Index;
            mPosition = node.Position;
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndoMoveSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }

    public class MoveSectionNodeDown : MoveSectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("move_section_down_command_label"); }
        }

        public MoveSectionNodeDown(SectionNode node, CoreNode parent)
            : base(node, parent)
        {
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.MoveSectionNodeDown(mNode.Project, mNode);
        }
    }

    public class MoveSectionNodeUp : MoveSectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("move_section_up_command_label"); }
        }

        public MoveSectionNodeUp(SectionNode node, CoreNode parent)
            : base(node, parent)
        {
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.MoveSectionNodeUp(mNode.Project, mNode);
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
