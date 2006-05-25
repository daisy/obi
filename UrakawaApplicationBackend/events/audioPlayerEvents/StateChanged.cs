using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events.audioPlayerEvents
{

public delegate void DStateChangedEvent( object sender,  StateChanged state) ;
	/// <summary>
	/// The state of the audio player changed.
	/// </summary>
    public class StateChanged : AudioPlayerEvent
    {



		private static readonly StateChanged mFromStopped = new StateChanged(AudioPlayerState.stopped);
		private static readonly StateChanged mFromPlaying = new StateChanged(AudioPlayerState.playing);
		private static readonly StateChanged mFromPaused = new StateChanged(AudioPlayerState.paused);

		private AudioPlayerState mOldState;  // the previous state of the player. The current state of the player is readily available.

		public StateChanged FromStopped { get { return mFromStopped; } }
		public StateChanged FromPlaying { get { return mFromPlaying; } }
		public StateChanged FromPaused { get { return mFromPaused; } }



		

		public AudioPlayerState OldState
		{
			get
			{
				return mOldState;
			}
		}
		
public event DStateChangedEvent StateChangedEvent ;
		/// <summary>
		/// Create a new StateChanged event.
		/// </summary>
		/// <param name="oldState">The state of the player before the change.</param>
		public StateChanged(AudioPlayerState oldState)
		{
			mOldState = oldState;

			
		}

		public void NotifyStateChanged (object sender, StateChanged state)
		{
StateChangedEvent ( sender, state ) ;
		}
    }
}
