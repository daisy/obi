using System;
using System.Windows.Forms ;
using VirtualAudioBackend.events.AudioPlayerEvents;
using VirtualAudioBackend.events.AssetManagerEvents ;
using VirtualAudioBackend.events.AudioRecorderEvents;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Summary description for CatchEvents.
	/// </summary>
	public class CatchEvents
	{
		
		public void CatchStateChangedEvent ( object sender, VirtualAudioBackend.events.AudioPlayerEvents.StateChanged state)
		{
			MessageBox.Show (  state.OldState.ToString () ) ;
		}

		//Audiorecorder State 
		public void CatchOnStateChangedEvent(Object sender, VirtualAudioBackend.events.AudioRecorderEvents.StateChanged state)
		{
			MessageBox.Show (  state.OldState.ToString () ) ;
		}
		public void CatchEndOfAudioEvent ( object sender , EndOfAudioData Data )
		{
			MessageBox.Show ("Audio ends") ;
		}
			
		public void CatchAssetDeletedEvent ( object sender , AssetDeleted Asset)		
		{
			MessageBox.Show (Asset.Asset.Name.ToString ()) ;
		}

		public void CatchAssetRenamedEvent (object sender , AssetRenamed Asset)
		{
			MessageBox.Show (Asset.OldName.ToString ()) ;
		}
	}
}
