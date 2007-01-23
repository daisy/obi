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
            mNode.Project._CutSectionNode(mNode, false);
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
        private object mPrevious;   // previous clipboard data
        private SectionNode mNode;  // the node to copy

        public override string Label
        {
            get { return Localizer.Message("copy_section_command_label"); }
        }

        public CopySectionNode(object prev, SectionNode node)
        {
            mPrevious = prev;
            mNode = node;
        }

        /// <summary>
        /// Do: restore previous data
        /// </summary>
        public override void Do()
        {
            mNode.Project.Clipboard.Section = mNode;
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.Clipboard.Data = mPrevious;
        }
    }

    class PasteSectionNode : Command
    {
        private CoreNode mParent;
        private SectionNode mNode;

        public override string Label
        {
            get { return Localizer.Message("paste_section_command_label"); }
        }

        public PasteSectionNode(CoreNode parent, SectionNode node)
        {
            mParent = parent;
            mNode = node;
        }

        public override void Do()
        {
            mNode.Project.PasteSectionNode(mNode.Project, mParent);
        }

        public override void Undo()
        {
            mNode.Project.UndoPasteSectionNode(mNode);
        }
    }

    class ShallowCutSectionNode : ShallowDeleteSectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("cut_strip_command_label"); }
        }

        public ShallowCutSectionNode(SectionNode node)
            : base(node)
        {
        }

        /// <summary>
        /// ReDo: uncut the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.DoShallowCutSectionNode(mNode.Project, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndoShallowCutSectionNode();
            base.Undo();
        }
    }

    class ShallowCopySectionNode : CopySectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("copy_strip_command_label"); }
        }

        public ShallowCopySectionNode(object prev, SectionNode node)
            : base(prev, node)
        {
        }
    }
}
