using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.audioPlayerEvents
{
	//these are just some suggestions for states
	enum AudioPlayerState {Playing, Idle, Initializing, Stopped};

    class StateChanged : AudioPlayerEvent
    {
		private AudioPlayerState mState;

		public StateChanged(AudioPlayerState newState)
		{
			mState = newState;
		}

		public AudioPlayerState State
		{
			get
			{
				return mState;
			}
		}
    }
}
