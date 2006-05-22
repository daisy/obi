using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events.assetManagerEvents
{
	/// <summary>
	/// An asset was deleted in the asset manager.
	/// </summary>
	public class AssetDeleted : AssetManagerEvent
    {
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
	}
}
