using System;
using System.Windows.Forms ;
using UrakawaApplicationBackend.events.audioPlayerEvents ;
using UrakawaApplicationBackend.events.assetManagerEvents ;
using UrakawaApplicationBackend.events.audioRecorderEvents;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for CatchEvents.
	/// </summary>
	public class CatchEvents
	{
		
		public void CatchStateChangedEvent ( object sender, UrakawaApplicationBackend.events.audioPlayerEvents.StateChanged state)
		{
MessageBox.Show (  state.OldState.ToString () ) ;
		}

		//Audiorecorder State 
		public void CatchOnStateChangedEvent(Object sender, UrakawaApplicationBackend.events.audioRecorderEvents.StateChanged state)
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
