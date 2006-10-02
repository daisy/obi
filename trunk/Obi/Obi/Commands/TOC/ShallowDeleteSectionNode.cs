using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class ShallowDeleteSectionNode : SectionNodeListCommand
    {
        private int mChildCount;
        //private ArrayList mSubCommands;
       
        public override string Label
        {
            get
            {
                return Localizer.Message("shallow_delete_section_command_label");
            }
        }

        public ShallowDeleteSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position, int numChildren, List<Command> commands)
        : base(project, node, parent, index, position, commands)
        {
            mChildCount = numChildren;

            //mSubCommands = new ArrayList();
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            Project.ShallowDeleteSectionNode(Project, Node);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
    /*   public override void Undo()
        {
            for (int i = mSubCommands.Count-1; i >=0; i--)
            {
                if (mSubCommands[i] != null)
                    ((Command)mSubCommands[i]).Undo();
            }
        }*/

        //md
        //needed to add this to make it easier to keep track of composite actions
       /* public void addSubCommand(Command cmd)
        {
            mSubCommands.Add(cmd);
        }*/

       /* public ArrayList getSubCommands()
        {
        }*/
    }
}
