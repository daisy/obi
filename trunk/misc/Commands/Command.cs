using System;

namespace Commands
{
	/// <summary>
	/// The basic command class. All commands inherit from this one.
	/// </summary>
	public abstract class Command
	{
		public virtual string Label { get { return null; } }  // get the label (for user interaction purposes)
		public abstract void Do();                            // actually perform the action
		public abstract void Undo();                          // go back to the state before the action was performed
	}
}
