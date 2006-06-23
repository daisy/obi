using System;
using System.Collections;

namespace VirtualAudioBackend
{
	/// <summary>
	/// This stub for the asset manager implements the singleton pattern. 
	/// </summary>
	public class AssetManager: IAssetManager
	{
		private static readonly AssetManager instance = new AssetManager();

		public static AssetManager Instance
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// The constructor is private so that it is only called to create the static instance.
		/// </summary>
		private AssetManager()
		{
		}

		/// <summary>
		/// Create a new empty AudioMediaAsset object with the given parameters and add it to the list of managed assets.
		/// </summary>
		/// <param name="channels"></param>
		/// <param name="bitDepth"></param>
		/// <param name="sampleRate"></param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(int channels, int bitDepth, int sampleRate)
		{
			return null;
		}

		/// <summary>
		/// Create a new AudioMediaAsset object from a list of clips and add it to the list of managed assets.
		/// </summary>
		/// <param name="clips">The array of <see cref="AudioClip"/>s.</param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(ArrayList clips)
		{
			return null;
		}

		#region IAssetManager Members

		public System.Collections.Hashtable Assets
		{
			get
			{
				// TODO:  Add AssetManager.Assets getter implementation
				return null;
			}
		}

		public System.Collections.Hashtable GetAssets(VirtualAudioBackend.MediaType assetType)
		{
			// TODO:  Add AssetManager.GetAssets implementation
			return null;
		}

		public IMediaAsset GetAsset(string assetName)
		{
			// TODO:  Add AssetManager.GetAsset implementation
			return null;
		}

		public IMediaAsset NewAsset(VirtualAudioBackend.MediaType assetType)
		{
			// TODO:  Add AssetManager.NewAsset implementation
			return null;
		}

		public void DeleteAsset(IMediaAsset assetToDelete)
		{
			// TODO:  Add AssetManager.DeleteAsset implementation
		}

		public IMediaAsset RemoveAsset(IMediaAsset assetToRemove)
		{
			// TODO:  Add AssetManager.RemoveAsset implementation
			return null;
		}

		public IMediaAsset CopyAsset(IMediaAsset asset)
		{
			// TODO:  Add AssetManager.CopyAsset implementation
			return null;
		}

		public IMediaAsset RenameAsset(IMediaAsset asset, String newName)
		{
			// TODO:  Add AssetManager.RenameAsset implementation
			return null;
		}

		public IMediaAsset AddAsset(VirtualAudioBackend.MediaType assetType, string assetPath)
		{
			// TODO:  Add AssetManager.AddAsset implementation
			return null;
		}

		#endregion
	}
}
