using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class Rename: Command__OLD__
    {
        private SectionNode mNode;
        private string mOldName;
        private string mNewName;

        public override string Label
        {
            get { return Localizer.Message("rename_command_label"); }
        }

        /// <summary>
        /// Issue the command *before* renaming so that the node still has its old name.
        /// </summary>
        public Rename(SectionNode node, string newName)
        {
            mNode = node;
            mOldName = node.Label;
            mNewName = newName;
        }

        /// <summary>
        /// Do: set the new name on the node.
        /// </summary>
        public override void Do()
        {
            mNode.Project.RenameSectionNode(mNode, mNewName);
        }

        /// <summary>
        /// Undo: set the old name on the node.
        /// </summary>
        public override void Undo()
        {
            mNode.Project.RenameSectionNode(mNode, mOldName);
        }
    }
}
