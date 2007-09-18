using System.Collections.Generic;

namespace Obi.Commands
{
    public class ListCommand : Command__OLD__
    {
        private string mLabel;            // the label that will be shown in the end
        private List<Command__OLD__> mCommands;  // list of actual commands

        public override string Label
        {
            get { return mLabel; }
        }

        /// <summary>
        /// Create a list of commands from, well, a list of commands.
        /// </summary>
        public ListCommand(string label, List<Command__OLD__> commands)
        //    : base(Command.Visible)
        {
            mLabel = label + "*";  // just for debugging
            mCommands = commands;
        }

        /// <summary>
        /// Create an empty list command.
        /// </summary>
        public ListCommand()
        {
            mLabel = "";
            mCommands = new List<Command__OLD__>();
        }

        /// <summary>
        /// Do all commands in order.
        /// </summary>
        public override void Do()
        {
            foreach (Command__OLD__ c in mCommands) c.Do();
        }

        /// <summary>
        /// Undo all commands starting from the last.
        /// </summary>
        public override void Undo()
        {
            mCommands.Reverse();
            foreach (Command__OLD__ c in mCommands) c.Undo();
            mCommands.Reverse();
        }

        /// <summary>
        /// Append a new command to the list.
        /// </summary>
        /// <param name="cmd">The command to append</param>
        public void AddCommand(Command__OLD__ cmd)
        {
            mCommands.Add(cmd);
        }

        /// <summary>
        /// Set the label for this command.
        /// </summary>
        /// <param name="label">New label for the command.</param>
        public void SetLabel(string label)
        {
            mLabel = label;
        }
    }
}