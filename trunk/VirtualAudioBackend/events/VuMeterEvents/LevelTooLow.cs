using System;
using System.Collections;
using System.Text;

namespace VirtualAudioBackend.events.VuMeterEvents
{
	class LevelTooLow : VuMeterEvent
	{
		private Object mMeasureInfo;
		private double mBytePositionStartOfRange;
		private double mBytePositionEndOfRange;

		public LevelTooLow(Object measureInfo, double startOfRange, double endOfRange)
		{
			mMeasureInfo = measureInfo;
			mBytePositionStartOfRange = startOfRange;
			mBytePositionEndOfRange = endOfRange;
		}

		public Object MeasureInfo
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
