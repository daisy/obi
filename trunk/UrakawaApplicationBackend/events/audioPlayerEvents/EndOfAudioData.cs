using System;

namespace UrakawaApplicationBackend.events.audioPlayerEvents
{
	/// <summary>
	/// The end of the audio data currently playing has been reached.
	/// </summary>
	public abstract class EndOfAudioData: AudioPlayerEvent
	{
		public EndOfAudioData()
		{
		}
	}

	/// <summary>
	/// The end of the asset currently playing has been reached.
	/// </summary>
	public class EndOfAudioAsset: EndOfAudioData
	{
		public EndOfAudioAsset()
		{
		}
	}

	/// <summary>
	/// The end of the buffer currently playing has been reached.
	/// </summary>
	public class EndOfAudioBuffer: EndOfAudioData
	{
		public EndOfAudioBuffer()
		{
		}
	}
}