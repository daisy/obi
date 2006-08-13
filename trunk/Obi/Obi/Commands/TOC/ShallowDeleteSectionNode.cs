using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using urakawa.core;

namespace Obi.Commands.TOC
{
    class ShallowDeleteSectionNode : Command
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;
        private int mChildCount;
        private ArrayList mSubCommands;
       
        public override string Label
        {
            get
            {
                return Localizer.Message("shallow_delete_section_command_label");
            }
        }

        public ShallowDeleteSectionNode(Project project, CoreNode node, CoreNode parent, int index, int position, int numChildren)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
            mChildCount = numChildren;

            mSubCommands = new ArrayList();
        }

        /// <summary>
        /// Do: delete the node from the project.
        /// </summary>
        public override void Do()
        {
            mProject.ShallowDeleteSectionNode(mProject, mNode);
        }

        /// <summary>
        /// Undo: restore the node and its descendants.
        /// </summary>
        public override void Undo()
        {
            for (int i = mSubCommands.Count-1; i >=0; i--)
            {
                ((Command)mSubCommands[i]).Undo();
            }
        }

        //md
        //needed to add this to make it easier to keep track of composite actions
        public void addSubCommand(Command cmd)
        {
            mSubCommands.Add(cmd);
        }

       /* public ArrayList getSubCommands()
        {
        }*/
    }
}
