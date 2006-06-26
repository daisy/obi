using System;
using System.Collections;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace VirtualAudioBackend
{
	/// <summary>
	/// The three states of the audio recorder.
	/// NotReady: the recorder is not ready to record, for whatever reason.
	/// Idle: the recorder is ready to record.
	/// Listening: the recording is listening but not writing any data.
	/// Recording: sound is currently being recorded.
	/// </summary>
	public enum AudioRecorderState { NotReady, Idle, Listening, Recording };

	public interface IAudioRecorder
	{
		/// <summary>
		/// Get and set the current input device.
		/// </summary>
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
		short BitDepth
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
		/// Start listening on the device but do not record yet. This allows the user to test the settings before actually recording.
		/// Although no audio is recorded, this method takes an audio asset as a parameter so that it can determine the parameters of
		/// the recording, such as channels, bit depth, frequency, etc. The asset is not modified and doesn't have to be the on that
		/// will eventually be recorded to.
		/// </summary>
		/// <param name="asset">The asset containing the required audio parameters.</param>
		void StartListening(IAudioMediaAsset asset);

		/// <summary>
		/// Start audio recording. Record to a given asset; if it contains data already, the new data is appended.
		/// Throws an exception if the device is unset, or if the asset has the wrong format (no format conversion is done.)
		/// The state after StartRecording() is Recording if succesful, Idle or Initializing otherwise.
		/// </summary>
		/// <param name="asset">The asset in which the audio is recorded.</param>
		void StartRecording(IAudioMediaAsset asset);

		/// <summary>
		/// Stop recording. The state after Stop() is Idle. If the recorder was not recording, there is no effect.
		/// </summary>
		void StopRecording();	
	}
}