using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    public class DecreaseSectionNodeLevel : MoveSectionNode
    {
        private int mNumChildren;

        public override string Label
        {
            get { return Localizer.Message("decrease_section_level_command_label"); }
        }

        public DecreaseSectionNodeLevel(SectionNode node, TreeNode parent)
            : base(node, parent)
        {
            mNumChildren = node.SectionChildCount;
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.DecreaseSectionNodeLevel(mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndecreaseSectionNodeLevel(mNode, mParent, mNumChildren);
        }
    }

    public class IncreaseSectionNodeLevel : MoveSectionNode
    {
        public override string Label
        {
            get { return Localizer.Message("increase_section_level_command_label"); }
        }

        public IncreaseSectionNodeLevel(SectionNode node, TreeNode parent)
            : base(node, parent)
        {
        }

        /// <summary>
        /// ReDo: move the node
        /// </summary>
        public override void Do()
        {
            mNode.Project.IncreaseSectionNodeLevel(mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UnincreaseSectionNodeLevel(mNode, mParent, mIndex);
        }
    }
}
