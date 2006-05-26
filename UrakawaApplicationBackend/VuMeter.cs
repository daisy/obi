using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Stub for the VU Meter class
	/// </summary>
    public class VuMeter : IVuMeter
    {
		public int Channels
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public object Stream
		{
			set
			{
			}
		}
		
		public UserControl VisualControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public UserControl TextControl
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public double[] PeakValue
		{
			get
			{
				return null;
			}
		}

		public bool[] Overloaded
		{
			get
			{
				return null;
			}
		}

		public void Reset()
		{
		}
    }
}
