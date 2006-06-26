using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace VirtualAudioBackend
{
	/// <summary>
	/// The asset manager.
	/// </summary>
	public interface IAssetManager
	{
		/// <summary>
		/// Return all assets currently managed.
		/// </summary>
		Hashtable Assets
		{
			get;
		}

		/// <summary>
		/// Add an existing media asset to the manager. The asset is indexed by its name.
		/// </summary>
		/// <param name="asset">The asset to add.</param>
		void AddAsset(IMediaAsset asset);

		/// <summary>
		/// Return all the assets of a given type.
		/// </summary>
		/// <param name="assetType">The type of assets.</param>
		/// <returns>The hash of assets of this type.</returns>
		Hashtable GetAssets(MediaType assetType);

		/// <summary>
		/// Return an asset given its name.
		/// </summary>
		/// <param name="name">Name of the asset to find.</param>
		/// <returns>The asset of that name or null if no asset of that name could be found.</returns>
		IMediaAsset GetAsset(string assetName);

		/// <summary>
		/// Create an empty asset that conforms to the required type. The asset is automatically named and added to the manager.
		/// Throw an exception in case of trouble.
		/// </summary>
		/// <param name="assetType">Required asset type.</param>
		/// <returns>A new asset of the required type, or some derived type.</returns>
		IMediaAsset NewAsset(MediaType assetType);

		/// <summary>
		/// Delete an asset completely (remove it from the list and delete from the disk.)
		/// Throw an exception if something went wrong (the asset could not be found, or could not be deleted.)
		/// </summary>
		/// <param name="assetToDelete">The asset to delete.</param>
		/// <remarks>A <see cref="DeleteAssetCommand"/> must be used if the operation is to be undone.</remarks>
		void DeleteAsset(IMediaAsset assetToDelete);

		/// <summary>
		/// Remove an asset from the asset manager. Do not delete the data.
		/// </summary>
		/// <param name="assetToRemove">The asset to remove</param>
		void RemoveAsset(IMediaAsset assetToRemove);

		/// <summary>
		/// Make a copy of an existing asset and give it a new name (e.g. "Foo (copy)".)
		/// Throw an exception if something went wrong (the asset could not be found, memory problem...)
		/// </summary>
		/// <param name="asset">The asset to copy.</param>
		/// <returns>A copy of the asset.</returns>
		IMediaAsset CopyAsset(IMediaAsset asset);

		/// <summary>
		/// Rename an asset.
		/// Throw an exception if something went wrong (the asset could not be found, an asset of the same name already exists...)
		/// </summary>
		/// <param name="asset">The asset to rename.</param>
		/// <param name="newName">The new name for the asset.</param>
		/// <returns>The old name of the asset.</returns>
		/// <remarks>A <see cref="RenameAssetCommand"/> should be used for renaming assets.</remarks>
		string RenameAsset(IMediaAsset asset, String newName);
	}
}