using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events.audioRecorderEvents
{
	/// <summary>
	/// The state of the audio recorder changed.
	/// </summary>
	public class StateChanged : AudioRecorderEvent
	{
		private static readonly StateChanged mFromInitializing = new StateChanged(AudioRecorderState.Initializing);
		private static readonly StateChanged mFromIdle = new StateChanged(AudioRecorderState.Idle);
		private static readonly StateChanged mFromRecording = new StateChanged(AudioRecorderState.Recording);

		private AudioRecorderState mOldState;  // the previous state of the recorder. The current state of the player is readily available.

		public StateChanged FromInitializing { get { return mFromInitializing; } }
		public StateChanged FromIdle { get { return mFromIdle; } }
		public StateChanged FromRecording { get { return mFromRecording; } }

		public AudioRecorderState OldState
		{
			get
			{
				return mOldState;
			}
		}
		
		/// <summary>
		/// Create a new StateChanged event.
		/// </summary>
		/// <param name="oldState">The state of the recorder before the change.</param>
		private StateChanged(AudioRecorderState oldState)
		{
			mOldState = oldState;
		}
	}
}
