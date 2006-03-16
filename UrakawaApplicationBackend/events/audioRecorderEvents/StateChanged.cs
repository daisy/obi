using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.audioRecorderEvents
{
	//these are just some suggestions for states
	enum AudioRecorderState {Recording, Idle, Initializing, Stopped};

    class StateChanged : AudioRecorderEvent
    {
		private AudioRecorderState mState;

		public StateChanged(AudioRecorderState newState)
		{
			mState = newState;
		}

		public AudioRecorderState State
		{
			get
			{
				return mState;
			}
		}
    }
}
