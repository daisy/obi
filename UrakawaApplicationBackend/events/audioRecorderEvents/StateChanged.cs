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
		private AudioRecorderState mOldState;  // the previous state of the player. The current state of the player is readily available.

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
		public StateChanged(AudioRecorderState oldState)
		{
			mOldState = oldState;
		}
	}
}
