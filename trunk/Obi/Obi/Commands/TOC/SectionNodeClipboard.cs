using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class CutSectionNode : ListCommand
    {
        private object mPrevious;
        private SectionNode mNode;

        public override string Label
        {
            get { return Localizer.Message("cut_section_command_label"); }
        }

        public CutSectionNode(SectionNode node)
        {
            mPrevious = node.Project.Clipboard.Data;
            mNode = node;
        }

        /// <summary>
        /// Redo: uncut the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.Clipboard.Section = mNode;
            base.Do();
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.Clipboard.Data = mPrevious;
            base.Undo();
        }
    }

    class CopySectionNode : Command__OLD__
    {
        private object mPrevious;   // previous clipboard data
        private SectionNode mNode;  // the node to copy

        public override string Label
        {
            get { return Localizer.Message("copy_section_command_label"); }
        }

        public CopySectionNode(SectionNode node)
        {
            mNode = node;
            mPrevious = mNode.Project.Clipboard.Data;
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

    class PasteSectionNode : Command__OLD__
    {
        private SectionNode mNode;
        private TreeNode mContext;

        public override string Label
        {
            get { return Localizer.Message("paste_section_command_label"); }
        }

        public PasteSectionNode(SectionNode node, TreeNode parent)
        {
            mNode = node;
            mContext = parent;
        }

        public override void Do()
        {
            mNode = mNode.Project.PasteCopyOfSectionNode(mNode.Project.Clipboard.Section, mContext);
        }

        public override void Undo()
        {
            mNode.Project.RemoveSectionNode(mNode);
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

        public ShallowCopySectionNode(SectionNode node)
            : base(node)
        {
        }
    }
}
