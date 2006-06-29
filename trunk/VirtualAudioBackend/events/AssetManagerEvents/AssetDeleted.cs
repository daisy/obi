using System;
using System.Collections;
using System.Text;


namespace VirtualAudioBackend.events.AssetManagerEvents
{
	public delegate void DAssetDeletedEvent ( object sender, AssetDeleted Asset ) ;
	/// <summary>
	/// An asset was deleted in the asset manager.
	/// </summary>
	public class AssetDeleted : AssetManagerEvent
	{

		public event DAssetDeletedEvent AssetDeletedEvent ;
		private IMediaAsset mAsset;  // the asset that was deleted

		public IMediaAsset Asset
		{
			get
			{
				return mAsset;
			}
		}

		/// <summary>
		/// Create a new AssetDeleted event.
		/// </summary>
		/// <param name="asset">The asset that was deleted.</param>
		public AssetDeleted(IMediaAsset asset)
		{
			mAsset = asset;
		}

		public void NotifyAssetDeleted ( object sender , AssetDeleted Asset) 
		{
			AssetDeletedEvent (sender , Asset) ;
		}
	}
}
