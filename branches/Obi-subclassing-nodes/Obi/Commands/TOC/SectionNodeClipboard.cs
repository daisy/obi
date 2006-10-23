using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class CutSectionNode : DeleteSectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("cut_section_command_label"); }
        }

        public CutSectionNode(SectionNode node)
            : base(node)
        {
        }

        /// <summary>
        /// ReDo: uncut the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.DoCutSectionNode(mNode.Project, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndoCutSectionNode(mNode, mParent, mIndex);
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
            mProject.UndoPasteSectionNode(mNode);
        }
    }
}
