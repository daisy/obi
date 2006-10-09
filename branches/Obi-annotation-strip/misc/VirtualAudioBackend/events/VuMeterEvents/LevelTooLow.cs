using System;
using System.Collections;
using System.Text;

namespace VirtualAudioBackend.events.VuMeterEvents
{
	public delegate void LevelTooLowHandler(object sender, LevelTooLow e);

	public class LevelTooLow : VuMeterEvent
	{
		private object mMeasureInfo;
		private double mBytePositionStartOfRange;
		private double mBytePositionEndOfRange;

		public LevelTooLow(object measureInfo, double startOfRange, double endOfRange)
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