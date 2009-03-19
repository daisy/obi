using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLib.Events.VuMeter
{
    public delegate void UpdatePeakMeterHandler(object sender, UpdatePeakMeter e);

    public class UpdatePeakMeter
    {
        private double[] mValues ;
        public UpdatePeakMeter(double[] Values)
        {
            mValues = Values ;
        }

        public double[] PeakValues
        {
            get
            {
                return mValues ;
            }
        }
    }
}
