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
		void DeleteAsset(IMediaAsset assetToDelete);

		/// <summary>
		/// Remove an asset from the asset manager. Do not delete the data.
		/// </summary>
		/// <param name="assetToDelete"></param>
		IMediaAsset RemoveAsset(IMediaAsset assetToRemove);

		/// <summary>
		/// Make a copy of an existing asset and give it a new name (e.g. "Foo (copy)".)
		/// Throw an exception if something went wrong (the asset could not be found, memory problem...)
		/// </summary>
		/// <param name="asset">The asset to copy.</param>
		/// <returns>A copy of the asset.</returns>
		//was: IMediaAsset copyAsset(IMediaAsset source, IMediaAsset dest, bool replaceIfExisting);
		IMediaAsset CopyAsset(IMediaAsset asset);

		/// <summary>
		/// Rename an asset.
		/// Throw an exception if something went wrong (the asset could not be found, an asset of the same name already exists...)
		/// </summary>
		/// <param name="asset">The asset to rename.</param>
		/// <param name="newName">The new name for the asset.</param>
		/// <returns>The renamed asset.</returns>
		IMediaAsset RenameAsset(IMediaAsset asset, String newName);

		/// <summary>
		/// Create and add a new media asset from its file path.
		/// </summary>
		/// <param name="assetType">The type of the asset to add.</param>
		/// <param name="assetPath">The path of the file for this asset.</param>
		/// <returns>The asset that was added.</returns>
		IMediaAsset AddAsset(MediaType assetType, string assetPath);
	}
}
