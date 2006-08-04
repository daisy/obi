using System;
using System.Collections;
using System.Text;

namespace VirtualAudioBackend.events.AssetManagerEvents
{
	public delegate void AssetRenamedHandler(object sender, AssetRenamed e);

	/// <summary>
	/// An asset was renamed in the asset manager.
	/// </summary>
	public class AssetRenamed : AssetManagerEvent
	{
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
	}
}
