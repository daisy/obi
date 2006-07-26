using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Events.Project
{
    public delegate void CommandCreatedHandler(object sender, CommandCreatedEventArgs e);

    /// <summary>
    /// This event indicates that the state of the project has changed.
    /// The project may have been opened, saved, modified, etc.
    /// </summary>
    public class CommandCreatedEventArgs : EventArgs
    {
        private Command mCommand; 

        public Command Command
        {
            get
            {
                return mCommand;
            }
        }

        public CommandCreatedEventArgs(Command command)
        {
            mCommand = command;
        }
    }
}
