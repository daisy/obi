using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class ShallowDeleteSectionNode : Command__OLD__
    {
        protected SectionNode mNode;         // the node being deleted
        protected ListCommand mSubCommands;  // successive commands for redo

        public override string Label
        {
            get { return Localizer.Message("shallow_delete_section_command_label"); }
        }

        public ShallowDeleteSectionNode(SectionNode node)
        {
            mNode = node;
            mSubCommands = new ListCommand();
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mNode.Project.ShallowDeleteSectionNode(this, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            mSubCommands.Undo();
        }

        //md
        //needed to add this to make it easier to keep track of composite actions
        public void AddCommand(Command__OLD__ cmd)
        {
            mSubCommands.AddCommand(cmd);
        }
    }
}