using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events.audioPlayerEvents
{
	/// <summary>
	/// The state of the audio player changed.
	/// </summary>
    public class StateChanged : AudioPlayerEvent
    {
		private AudioPlayerState mOldState;  // the previous state of the player. The current state of the player is readily available.

		public AudioPlayerState OldState
		{
			get
			{
				return mOldState;
			}
		}
		
		/// <summary>
		/// Create a new StateChanged event.
		/// </summary>
		/// <param name="oldState">The state of the player before the change.</param>
		public StateChanged(AudioPlayerState oldState)
		{
			mOldState = oldState;
		}
    }
}
