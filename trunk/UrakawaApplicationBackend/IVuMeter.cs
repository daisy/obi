
using System;
using System.Collections;
using System.Text;

namespace urakawaApplication
{
    public interface IVuMeter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream">an incoming buffer of bytes to analyse and render</param>
        void streamIn(Object stream); 

        //throws VuMeterException
        void setGraphicalWidget(Object graphicalWidget);
        //throws VuMeterException
        void setTextualWidget(Object textualWidget);

        Object getGraphicalWidget();
        Object getTextualWidget();

        //throws VuMeterException
        void setPeakWarningLevel(int sampleLevel);
        int getPeakWarningLevel();

        //throws VuMeterException
        void setTooLowRmsWarningLevel(int rmsLevel);
        int getTooLowRmsWarningLevel();

        /// <summary>
        /// resets peak and rms counts
        /// </summary>
        void resetStatistics();
    }
}
