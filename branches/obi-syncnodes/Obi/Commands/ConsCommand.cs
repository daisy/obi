using System;

namespace Obi.Commands
{
	/// <summary>
	/// A cons command looks like a regular command but is in fact a pair of commands in sequence.
	/// </summary>
	public class ConsCommand : Command
	{
		private string mLabel;  // the label that will be shown to the user
		private Command mCar;   // the first command
		private Command mCdr;   // the rest

		public override string Label { get { return mLabel; } }

		/// <summary>
		/// Create a new cons command from two existing commands, and specifiy the label to use for the resulting command.
		/// For the moment, the label has an ellipsis (...) appended to show that it is a ConsCommand.
		/// </summary>
		/// <param name="label">The label that will appear for this command.</param>
		/// <param name="car">The first command.</param>
		/// <param name="cdr">The second command.</param>
		public ConsCommand(string label, Command car, Command cdr)
		{
			mLabel = label + " (...)";  // for debugging purposes only
			mCar = car;
			mCdr = cdr;
		}

		/// <summary>
		/// Do the car command first, then the cdr.
		/// </summary>
		public override void Do()
		{
			mCar.Do();
			mCdr.Do();
		}

		/// <summary>
		/// Undo the cdr command first, then the car.
		/// </summary>
		public override void Undo()
		{
			mCdr.Undo();
			mCar.Undo();
		}
	}
}