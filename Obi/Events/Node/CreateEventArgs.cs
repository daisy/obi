using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Node
{
    public delegate void CreateHandler(object sender, CreateEventArgs e);

    /// <summary>
    /// This event indicates that a new core node was created.
    /// </summary>
    public class CreateEventArgs : EventArgs
    {
        private Obi.Project mProject;
        private CoreNode mNode;

        public Obi.Project Project
        {
            get
            {
                return mProject;
            }
        }

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        public CreateEventArgs(Obi.Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
        }
    }
}
