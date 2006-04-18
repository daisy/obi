using System;
using System.IO;

namespace UrakawaApplicationBackend
{
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
		/// MIME type of the asset. For instance, WAV files are audio/x-wav (? check this)
		/// </summary>
		string MediaType
		{
			get;
		}

		/// <summary>
		/// The size in bytes of the asset.
		/// </summary>
		ulong SizeInBytes
		{
			get;
		}

		/// <summary>
		/// The file path for this asset.
		/// </summary>
		Path Path
		{
			get;
		}

		/// <summary>
		/// The file object for the file that contains this asset.
		/// </summary>
		File File
		{
			get;
		}

		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		void Delete();

		/// <summary>
		/// Validate the asset by performing an integrity check.
		/// </summary>
		/// <returns>True if the asset was found to be valid, false otherwise.</returns>
		bool Validate();    
	}
}
