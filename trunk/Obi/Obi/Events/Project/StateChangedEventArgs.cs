using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Events.Project
{
    /// <summary>
    ///  Various things that can happen to the project.
    /// </summary>
    public enum StateChange { Closed, Modified, Opened, Saved };

    public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

    /// <summary>
    /// This event indicates that the state of the project has changed.
    /// The project may have been opened, saved, modified, etc.
    /// </summary>
    public class StateChangedEventArgs: EventArgs
    {
        private StateChange mChange;

        public StateChange Change
        {
            get
            {
                return mChange;
            }
        }

        public StateChangedEventArgs(StateChange change)
        {
            mChange = change;
        }
    }
}
