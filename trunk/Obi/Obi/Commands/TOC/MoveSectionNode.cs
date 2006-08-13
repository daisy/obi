using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class MoveSectionNodeDown : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_down_command_label");
            }
        }

        public MoveSectionNodeDown(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.MoveSectionNodeDown(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mProject.UndoMoveSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }

    class MoveSectionNodeUp : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_up_command_label");
            }
        }

        public MoveSectionNodeUp(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mProject.MoveSectionNodeUp(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mProject.UndoMoveSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }

    class MoveSectionNodeDownLinear : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_down_linear_command_label");
            }
        }

        public MoveSectionNodeDownLinear(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
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
            mProject.UndoMoveSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }

    class MoveSectionNodeUpLinear : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public override string Label
        {
            get
            {
                return Localizer.Message("move_section_up_linear_command_label");
            }
        }

        public MoveSectionNodeUpLinear(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
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
            mProject.UndoMoveSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }
}
