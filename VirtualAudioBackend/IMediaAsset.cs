using System;
using System.IO;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Only Audio is known at the moment. Anything else is "other" and out of scope.
	/// </summary>
	public enum MediaType { Audio, Other };

	/// <summary>
	/// A media asset is an atomic unit of a given media (so far sound clip, but later on text, image, video...)
	/// </summary>
	public interface IMediaAsset
	{
		/// <summary>
		/// Name of the asset. The name must be unique within the whole project.
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Which type of media (as understood by the project) is this asset.
		/// When we introduce new media types, we may need the MIME type as well.
		/// </summary>
		MediaType MediaType
		{
			get;
		}

		///<summary>
		/// The total size in bytes of the asset, including headers, etc.
		/// </summary>
		long SizeInBytes
		{
			get;
			set;
		}

		/// <summary>
		/// The asset manager that manages this asset, or null if unmanaged.
		/// </summary>
		IAssetManager AssetManager
		{
			get;
		}

		/// <summary>
		/// Add self to an asset manager.
		/// </summary>
		void Add(IAssetManager manager);

		/// <summary>
		/// Remove self from the project. The data is not deleted.
		/// </summary>
		void Remove();

		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		void Delete();

		/// <summary>
		/// Merge with another asset of the same kind.
		/// The asset is modified in place; the next one should be discarded.
		/// Throw an exception if the assets could not be merged (e.g. they are of different type.)
		/// </summary>
		/// <param name="next">The asset to merge with (must be of the same type.)</param>
		void MergeWith(IMediaAsset next);
	}
}