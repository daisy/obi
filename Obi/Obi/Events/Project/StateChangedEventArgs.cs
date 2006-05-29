using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Project
{
    public enum StateChange { Closed, Created, Modified, Opened, Reverted, Saved };

    public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

    public class StateChangedEventArgs: EventArgs
    {
        private Obi.Project mProject;
        private StateChange mChange;

        public Obi.Project Project { get { return mProject; } } 
        public StateChange Change { get { return mChange; } }

        public StateChangedEventArgs(Obi.Project project, StateChange change)
        {
            mProject = project;
            mChange = change;
        }
    }
}
