using System;

namespace VirtualAudioBackend.events.AudioPlayerEvents
{
	public delegate void EndOfAudioAssetHandler(object sender, EndOfAudioAsset e);    // JQ
	public delegate void EndOfAudioBufferHandler(object sender, EndOfAudioBuffer e);  // JQ

	/// <summary>
	/// The end of the audio data currently playing has been reached.
	/// </summary>
	public abstract class EndOfAudioData: AudioPlayerEvent
	{
		public EndOfAudioData()
		{
		}
	}

	public class EndOfAudioAsset: EndOfAudioData
	{
	}

	public class EndOfAudioBuffer: EndOfAudioData
	{
	}

	/* To be deleted (JQ)
	 
	public delegate void DEndOfAudioAssetEvent (object sender, EndOfAudioData EndData) ;

	/// <summary>
	/// The end of the asset currently playing has been reached.
	/// </summary>
	public class EndOfAudioAsset: EndOfAudioData
	{
		public event DEndOfAudioAssetEvent  EndOfAudioAssetEvent  ;

		public void NotifyEndOfAudioAsset ( object sender, EndOfAudioData EndData) 
		{
			if (EndOfAudioAssetEvent   != null)
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
		
	} */
}

