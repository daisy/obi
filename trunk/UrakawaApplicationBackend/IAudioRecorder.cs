using System;
using System.Collections;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace UrakawaApplicationBackend
{
	public interface IAudioRecorder
	{
		// get the capture devices 
		// this will return the capture devices list 
		ArrayList GetInputDevice();
		
		// return the capture device guid for a selected device
		Guid SetInputDeviceGuid();

		// get the wave formats based on the sample rate and bit depth
		// pass the index  and ref WaveFormat as the parameters
		//allowed values: 1 and 2 for the channels
		//allowed values for the sample rates: 11025, 22050, 44100
		//allowed value for the bit depth are 8 and 16
		void GetWaveFormatFromIndex(int Index, ref WaveFormat format);

		// returns the array list of the formats based on the sample rate, channel  and the bit depth
		ArrayList GetFormatList();

		
		//throws AudioRecorderException 
		//		void record(IAudioMediaAsset wave);
		//		void stopRecording();
		//void setAudioRecorderListener(eventListeners.IAudioRecorderEventListener listener);

		//return value should be of type Collection<IAudioRecorderListener>
		//ArrayList getAudioRecorderListeners();

		//eventListeners.IAudioPlayerEventListener getAudioRecorderListener(int i);        

		//returns states such as recording, idle, initializing
		//the return value type will be something like AudioRecorderStateType instead of Object
		//		Object getState();
	}
}
	