using System;
using System.Collections;
using System.Text;

namespace Obi.Events.Audio.VuMeter
{
	public delegate void LevelTooLowHandler(object sender, LevelTooLowEventArgs e);

	public class LevelTooLowEventArgs: EventArgs
	{
		private object mMeasureInfo;
		private double mBytePositionStartOfRange;
		private double mBytePositionEndOfRange;

		public LevelTooLowEventArgs(object measureInfo, double startOfRange, double endOfRange)
		{
			mMeasureInfo = measureInfo;
			mBytePositionStartOfRange = startOfRange;
			mBytePositionEndOfRange = endOfRange;
		}

		public object MeasureInfo
		{
			get
			{
				return mMeasureInfo;
			}
		}

		public double BytePositionStartOfRange
		{
			get
			{
				return mBytePositionStartOfRange;
			}
		}

		public double BytePositionEndOfRange
		{
			get
			{
				return mBytePositionEndOfRange;
			}
		}
	}
}