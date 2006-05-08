using System;
using System.Collections;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// The three states of the audio recorder.
	/// Initializing: the recorder is not yet ready to record.
	/// Idle: the recorder is ready to record.
	/// Recording: sound is currently being recorded.
	/// </summary>
	public enum AudioRecorderState { Initializing, Idle, Recording };

	public interface IAudioRecorder
	{
		/// <summary>
		/// Get and set the current input device.
		/// </summary>
		// I guess that Device is the actual audio device class
		// set may not be enough, so be free to replace it with SetInputDevice(...) if necessary.
		Device InputDevice
		{
			get;
			set;
		}

		/// <summary>
		/// Get and set the number of channels (mono = 1, stereo = 2) for recording.
		/// Throw an exception in case of illegal value.
		/// </summary>
		int Channels
		{
			get;
			set;
		}

		/// <summary>
		/// Get and set the sample rate for recording.
		/// Throw an exception in case of illegal value.
		/// </summary>
		int SampleRate
		{
			get;
			set;
		}

		/// <summary>
		/// Get and set the bit depth (8 or 16) for recording.
		/// Throw an exception in case of illegal value.
		/// </summary>
		int BitDepth
		{
			get;
			set;
		}

		/// <summary>
		/// Get the current recorder state (initializing, recording, or idle.)
		/// </summary>
		AudioRecorderState State
		{
			get;
		}

		// get the capture devices 
		// this will return the capture devices list
		// fixed a typo in the name
		ArrayList GetInputDevices();
		
		// return the capture device guid for a selected device
		// should it be in the interface?
		// Guid SetInputDeviceGuid();

		// get the wave formats based on the sample rate and bit depth
		// pass the index  and ref WaveFormat as the parameters
		//allowed values: 1 and 2 for the channels
		//allowed values for the sample rates: 11025, 22050, 44100
		//allowed value for the bit depth are 8 and 16
		// should it be in the interface?
		// void GetWaveFormatFromIndex(int Index, ref WaveFormat format);

		// returns the array list of the formats based on the sample rate, channel  and the bit depth
		// should it be in the interface?
		// ArrayList GetFormatList();
		
		/// <summary>
		/// Start audio recording. Record to a given asset; if it contains data already, the new data is appended.
		/// Throws an exception if the device is unset, or if the asset has the wrong format (no format conversion is done.)
		/// The state after Record() is Recording if succesful, Idle or Initializing otherwise.
		/// </summary>
		/// <param name="asset">The asset in which the audio is recorded.</param>
		void Record(IAudioMediaAsset asset);

		/// <summary>
		/// Stop recording. The state after Stop() is Idle. If the recorder was not recording, there is no effect.
		/// </summary>
		void Stop();

		// the event stuff should be unnecessary since we use .Net's native events.
		//void setAudioRecorderListener(eventListeners.IAudioRecorderEventListener listener);
		//return value should be of type Collection<IAudioRecorderListener>
		//ArrayList getAudioRecorderListeners();
		//eventListeners.IAudioPlayerEventListener getAudioRecorderListener(int i);        
	}
}
	
