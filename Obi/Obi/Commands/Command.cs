using System;

namespace Obi.Commands
{
	/// <summary>
	/// The basic command class. All commands inherit from this one.
	/// </summary>
	public abstract class Command
	{
        /*
        private bool mUserVisible;                            // is this command visible to the user, or part of a complex command?

        public static readonly bool Visible = true;
        public static readonly bool Hidden = false;

        public bool UserVisible
        {
            get { return mUserVisible; }
        }

        internal Command(bool visible)
        {
            mUserVisible = visible;
        }
        */

		public virtual string Label { get { return null; } }  // get the label (for user interaction purposes)
		public abstract void Do();                            // actually perform the action
		public abstract void Undo();                          // go back to the state before the action was performed
	}
}
