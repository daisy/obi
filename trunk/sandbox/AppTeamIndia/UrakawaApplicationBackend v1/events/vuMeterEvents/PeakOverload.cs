using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.vuMeterEvents
{
    //risen by a VuMeter when the signal level
    //rises above the set peak threshold
    class PeakOverload : VuMeterEvent 
    {
		private Object mMeasureInfo;
		private double mBytePosition;
		private long mTimePosition;

		public PeakOverload(Object measureInfo, double bytePosition)
		{
			mMeasureInfo = measureInfo;
			mBytePosition = bytePosition;
			mTimePosition = 0;
		}

		public PeakOverload(Object measureInfo, long timePosition)
		{
			mMeasureInfo = measureInfo;
			mBytePosition = 0;
			mTimePosition = timePosition;
		}
       
		public Object MeasureInfo
		{
			get
			{
				return mMeasureInfo;
			}
		}

		public double BytePosition
		{
			get
			{
				return mBytePosition;
			}
		}

		public long TimePosition
		{
			get
			{
				return mTimePosition;
			}
		}

    }
}
