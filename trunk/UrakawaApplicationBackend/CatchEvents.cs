using System;
using System.Windows.Forms ;
using UrakawaApplicationBackend.events.audioPlayerEvents ;
using UrakawaApplicationBackend.events.assetManagerEvents ;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for CatchEvents.
	/// </summary>
	public class CatchEvents
	{
		
		public void CatchStateChangedEvent ( object sender, StateChanged   state)
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
