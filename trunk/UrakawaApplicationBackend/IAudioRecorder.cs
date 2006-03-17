using System;
using System.Collections;
using System.Text;
using System.Collections;

namespace urakawaApplication
{
    public interface IAudioRecorder
    {
        //parameter should be of type InputDevice
        //throws AudioRecorderException
        void setInputDevice(Object device); 

        //return value should be of type InputDevice
        Object getCurrentInputDevice();

        //throws AudioRecorderException
        //allowed values: 1, 2
        void setNumberOfChannels(int number);

        //throws AudioRecorderException
        //allowed values: 22050, 44100, 88200, 96000
        void setSampleRate(int rate);

        // throws AudioRecorderException 
        //allowed values: 16, 24, 32
        void setBitDepth(int depth); 

        //throws AudioRecorderException 
        void record(IAudioMediaAsset wave);
        void stopRecording();
        

        //returns states such as recording, idle, initializing
        //the return value type will be something like AudioRecorderStateType instead of Object
        Object getState();
    }
}
