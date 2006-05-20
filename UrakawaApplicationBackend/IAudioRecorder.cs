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

		/// Get and set the current input device.

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
		
				
		/// <summary>
		/// Start audio recording. Record to a given asset; if it contains data already, the new data is appended.
		/// Throws an exception if the device is unset, or if the asset has the wrong format (no format conversion is done.)
		/// The state after StartRecording() is Recording if succesful, Idle or Initializing otherwise.
		/// </summary>
		/// <param name="asset">The asset in which the audio is recorded.</param>
		void StartRecording(bool StartRecording, string FileName);

		/// <summary>
		/// Stop recording. The state after Stop() is Idle. If the recorder was not recording, there is no effect.
		/// </summary>
		void StopRecording();

		        
	
	}
}
	
