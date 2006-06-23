using System;
using System.IO;

namespace VirtualAudioBackend
{
	/// <summary>
	/// A media asset is an atomic unit of a given media (so far sound clip, but later on text, image, video...)
	/// </summary>
	public abstract class MediaAsset: IMediaAsset
	{
		/// <summary>
		/// Name of the asset. The name must be unique within the whole project.
		/// </summary>
		public string Name
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// MIME type of the asset. For instance, WAV files are audio/x-wav (? check this)
		/// </summary>
		public MediaType MediaType
		{
			get
			{
				return MediaType.Other;
			}
		}

		 //<summary>
		/// The size in bytes of the asset.
		/// </summary>
		public long SizeInBytes
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		public abstract void Delete();
		
		/*
		/// <summary>
		/// Merge with the following asset.
		/// Throw an exception if the assets could not be merged (e.g. they are of different type.)
		/// </summary>
		/// <param name="next">The asset that follows (must be of the same type.)</param>
		/// <returns>The new asset resulting of the merger.</returns>
		public abstract IMediaAsset MergeWith(IMediaAsset next);
		*/
	}

}