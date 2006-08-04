using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend.events.assetManagerEvents
{
public delegate void DAssetRenamedEvent ( object sender , AssetRenamed Asset) ;

	/// <summary>
	/// An asset was renamed in the asset manager.
	/// </summary>
    public class AssetRenamed : AssetManagerEvent
    {

public event DAssetRenamedEvent AssetRenamedEvent ; 

		private IMediaAsset mAsset;  // the asset (can get its current name through Asset.Name)
		private string mOldName;     // the asset's name before renaming

		public IMediaAsset Asset
		{
			get
			{
				return mAsset;
			}
		}

		public string OldName
		{
			get
			{
				return mOldName;
			}
		}

		/// <summary>
		/// Create a new AssetRenamed event.
		/// </summary>
		/// <param name="asset">The renamed asset (with its new name.)</param>
		/// <param name="oldName">The name of the asset before renaming.</param>
		public AssetRenamed(IMediaAsset asset, string oldName)
		{
			mOldName = oldName;
			mAsset = asset;
		}

		public void NotifyAssetRenamed ( object sender , AssetRenamed Asset) 
		{
AssetRenamedEvent (sender , Asset) ;
		}
    }
}
