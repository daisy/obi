using System;

namespace VirtualAudioBackend.events.AudioPlayerEvents
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

	public delegate void DEndOfAudioAssetEvent (object sender, EndOfAudioData EndData) ;
	/// <summary>
	/// The end of the asset currently playing has been reached.
	/// </summary>
	public class EndOfAudioAsset: EndOfAudioData
	{
		public event DEndOfAudioAssetEvent  EndOfAudioAssetEvent  ;

		public void NotifyEndOfAudioAsset ( object sender, EndOfAudioData EndData) 
		{
			EndOfAudioAssetEvent   ( sender, EndData ) ;
		}
	}

	public delegate void DEndOfAudioBufferEvent ( object sender , EndOfAudioData EndData) ;
	/// <summary>
	/// The end of the buffer currently playing has been reached.
	/// </summary>
	public class EndOfAudioBuffer: EndOfAudioData
	{

		public event DEndOfAudioBufferEvent  EndOfAudioBufferEvent  ;
		
		public void NotifyEndOfAudioBuffer (object sender , EndOfAudioData EndData ) 
		{
			EndOfAudioBufferEvent ( sender, EndData ) ;
		}
		
	}
}

