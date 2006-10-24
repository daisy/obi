using System;

namespace Obi.Commands
{
	/// <summary>
	/// The basic command class. All commands inherit from this one.
	/// </summary>
	public abstract class Command
	{
        /// <summary>
        /// The label of this command for the undo menu.
        /// </summary>
        public abstract string Label
        {
            get;
        }

        /// <summary>
        /// Do, or rather redo, the command from the initial state.
        /// </summary>
		public abstract void Do();
        
        /// <summary>
        /// Undo the command and bring everything back to the initial state.
        /// </summary>
        public abstract void Undo();
	}
}