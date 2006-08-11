using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class CutSectionNode : Command
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
                return Localizer.Message("cut_section_command_label");
            }
        }

        public CutSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }

        /// <summary>
        /// ReDo: uncut the node
        /// </summary>
        public override void Do()
        {
            mProject.DoCutSectionNode(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoCutSectionNode(mNode, mParent, mIndex, mPosition);
        }
    }

    class CopySectionNode : Command
    {
        private Project mProject;
        private CoreNode mNode;

        public override string Label
        {
            get
            {
                return Localizer.Message("copy_section_command_label");
            }
        }

        public CopySectionNode(Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
        }

        /// <summary>
        /// ReDo: uncut the node
        /// </summary>
        public override void Do()
        {
            mProject.CopySectionNode(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mProject.UndoCopySectionNode(mNode);
        }
    }
    class PasteSectionNode : Command
    {
        private Project mProject;
        private CoreNode mParent;
        private CoreNode mNode;

        public override string Label
        {
            get
            {
                return Localizer.Message("paste_section_command_label");
            }
        }

        public PasteSectionNode(Project project, CoreNode node, CoreNode parent)
        {
            mProject = project;
            mParent = parent;
            mNode = node;
        }


        public override void Do()
        {
            mProject.PasteSectionNode(mProject, mParent);
        }

        public override void Undo()
        {
            mProject.RemoveNode(mProject, mNode);
        }
    }
}
