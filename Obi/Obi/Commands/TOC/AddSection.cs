using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class AddSectionNode: Command
    {
        private SectionNode mNode;  // the newly added section node
        private CoreNode mParent;   // the parent to which it was added
        private int mIndex;         // the index at which it was added

        public override string Label
        {
            get { return Localizer.Message("add_section_command_label"); }
        }

        /// <summary>
        /// Create a new "add section" command from the new section node, the parent node and the index of the new section.
        /// The command is created once the section has actually been added (so that its parent was set.)
        /// </summary>
        public AddSectionNode(SectionNode node)
        {
            mNode = node;
            mParent = (CoreNode)node.getParent();
            mIndex = mNode.Index;
        }

        /// <summary>
        /// Do: readd the node to the project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.ReaddSectionNode(mNode, mParent, mIndex);
        }

        /// <summary>
        /// Undo: remove the node from the project.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.RemoveSectionNode(mNode);
        }
    }
}
