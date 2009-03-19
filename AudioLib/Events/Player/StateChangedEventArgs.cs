using System;
using System.Collections;
using System.Text;

namespace AudioLib.Events.Player
{
	public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

	/// <summary>
	/// The state of the audio player changed.
	/// </summary>
	public class StateChangedEventArgs : EventArgs
	{
		private AudioLib.AudioPlayerState mOldState;  // the previous state of the player. The current state of the player is readily available.

		public AudioLib.AudioPlayerState OldState
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
		public StateChangedEventArgs(AudioLib.AudioPlayerState oldState)
		{
			mOldState = oldState;
		}
	}
}
