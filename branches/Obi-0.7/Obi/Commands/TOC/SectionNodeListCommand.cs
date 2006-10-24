using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
    abstract class SectionNodeListCommand : ListCommand
    {
        private Project mProject;
        private CoreNode mNode;
        private CoreNode mParent;
        private int mIndex;
        private int mPosition;

        public Project Project
        {
            get { return mProject; }
        }
        public CoreNode Node
        {
            get { return mNode; }
        }
        public CoreNode Parent
        {
            get { return mParent; }
        }
        public int Index
        {
            get { return mIndex; }
        }
        public int Position
        {
            get { return mPosition; }
        }

        public SectionNodeListCommand(Project project, CoreNode node, CoreNode parent, int index, int position, List<Command> commands)
        : base("", commands)
        {
            mProject = project;
            mNode = node;
            mParent = parent;
            mIndex = index;
            mPosition = position;
        }
    }
}
