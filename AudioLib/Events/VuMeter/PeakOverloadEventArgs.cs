using System;
using System.Collections;
using System.Text;

namespace AudioLib.Events.VuMeter
{
	public delegate void PeakOverloadHandler(object sender, PeakOverloadEventArgs e);

	/// <summary>
	/// Event raised by the Vu meter when the signal level overloads.
	/// </summary>
	public class PeakOverloadEventArgs: EventArgs 
	{
		private int mChannel;          // channel which overloaded
		private double mTimePosition;  // time when the overload happened (from start of recording)

		public int Channel
		{
			get
			{
				return mChannel;
			}
		}

		public double TimePosition
		{
			get
			{
				return mTimePosition;
			}
		}

		/// <summary>
		/// Create a new PeakOverload event at a given time.
		/// </summary>
		/// <param name="channel">The channel that overloaded.</param>
		/// <param name="timePosition">The time when the event occurred.</param>
		public PeakOverloadEventArgs(int channel, double timePosition)
		{
			mChannel = channel;
			mTimePosition = timePosition;
		}       
	}
}
