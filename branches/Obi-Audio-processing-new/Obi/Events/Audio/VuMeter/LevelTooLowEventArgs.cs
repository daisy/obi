using System;
using System.Collections;
using System.Text;

namespace Obi.Events.Audio.VuMeter
{
	public delegate void LevelTooLowHandler(object sender, LevelTooLowEventArgs e);

	public class LevelTooLowEventArgs: EventArgs
	{
		private object mMeasureInfo;
        private double m_LowLevelValue;
		private double mBytePositionStartOfRange;
		private double mBytePositionEndOfRange;

		public LevelTooLowEventArgs(object measureInfo, double LowLevelValue ,double startOfRange, double endOfRange)
		{
			mMeasureInfo = measureInfo;
            m_LowLevelValue = LowLevelValue;
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

        public double LowLevelValue
        {
            get
            {
                return m_LowLevelValue;
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