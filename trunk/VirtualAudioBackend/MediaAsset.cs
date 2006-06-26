using System;
using System.IO;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Virtual edits implementation of IMediaAsset.
	/// </summary>
	public abstract class MediaAsset: IMediaAsset
	{
		#region IMediaAsset Members

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

		public MediaType MediaType
		{
			get
			{
				return MediaType.Other;
			}
		}

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

		public IAssetManager AssetManager
		{
			get
			{
				return null;
			}
		}

		public void Add(IAssetManager manager)
		{
		}

		public void Remove()
		{
		}

		public abstract void Delete();
		
		public abstract void MergeWith(IMediaAsset next);

		#endregion
	}
}