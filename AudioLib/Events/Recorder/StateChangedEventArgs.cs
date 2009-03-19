using System;
using System.Collections;
using System.Text;	

namespace AudioLib.Events.Recorder
{
	public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

	/// <summary>
	/// The state of the audio recorder changed.
	/// </summary>
	public class StateChangedEventArgs: EventArgs
	{
		private AudioLib.AudioRecorderState mOldState;  // the previous state of the recorder. The current state of the player is readily available.

        public AudioLib.AudioRecorderState OldState
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
		public StateChangedEventArgs(AudioLib.AudioRecorderState oldState)
		{
			mOldState = oldState;
		}
	}
}
