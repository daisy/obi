using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace VirtualAudioBackend
{
    public interface IVuMeter
    {
		/// <summary>
		/// Number of channels (1 or 2)
		/// </summary>
		int Channels
		{
			get;
			set;
		}

		/// <summary>
		/// Set the incoming buffer of bytes to analyze and render. Choose a better type than object.
		/// </summary>
		object Stream
		{
			set;
		}
		
		/// <summary>
		/// Get/set the control that is used to render the VU meter visually.
		/// </summary>
		UserControl VisualControl
		{
			get;
			set;
		}

		/// <summary>
		/// Get/set the control that is used to render the VU meter visually.
		/// </summary>
		UserControl TextControl
		{
			get;
			set;
		}

		/// <summary>
		/// Current peak values from the stream. There is a value for each channel.
		/// </summary>
		// type is changed to int from double which is more favourable for decoding wave files
		int [] PeakValue
		{
			get;
		}

		/// <summary>
		/// Flag that shows if the signal overloaded in each channel.
		/// </summary>
		bool[] Overloaded
		{
			get;
		}

        /// <summary>
        /// Resets statistics from the VU meter (peak values and overload.)
        /// </summary>
        void Reset();		
    }
}
