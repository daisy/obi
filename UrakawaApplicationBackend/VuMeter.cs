using System;
using System.Collections;
using System.Text;

namespace urakawaApplication
{
    
    //has all the methods from IVuMeter
    //when sending an event, uses;
    //EventController.getInstance().notify(new SomeEvent())
    class VuMeter : IVuMeter
    {
        public void streamIn(object stream)
        {
            
        }

        public void setGraphicalWidget(object graphicalWidget)
        {
   
        }

        public void setTextualWidget(object textualWidget)
        {
           
        }

        public Object getGraphicalWidget()
        {
            return null;

        }

        public Object getTextualWidget()
        {
            return null;
        }

        public void setPeakWarningLevel(int sampleLevel)
        {

        }

        public int getPeakWarningLevel()
        {
            return 0;
        }

        public void setTooLowRmsWarningLevel(int rmsLevel)
        {
   
        }

        public int getTooLowRmsWarningLevel()
        {
            return 0;
        }

        public void resetStatistics()
        {
            
        }
    }
}
