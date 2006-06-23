using System;
using System.IO;

namespace VirtualAudioBackend
{
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
		/// MIME type of the asset. For instance, WAV files are audio/x-wav (? check this)
		/// </summary>
		MediaType MediaType
		{
			get;
		}

		 //<summary>
		/// The size in bytes of the asset.
		/// </summary>
		long SizeInBytes
		{
			get;
			set;
		}

		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		void Delete();

		/*
		/// <summary>
		/// Merge with the following asset.
		/// Throw an exception if the assets could not be merged (e.g. they are of different type.)
		/// </summary>
		/// <param name="next">The asset that follows (must be of the same type.)</param>
		/// <returns>The new asset resulting of the merger.</returns>
		IMediaAsset MergeWith(IMediaAsset next);
		*/
	}

}