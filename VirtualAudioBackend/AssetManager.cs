using System;
using System.Collections;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Implementation of the asset manager.
	/// </summary>
	public class AssetManager: IAssetManager
	{
		/// <summary>
		/// Create the asset manager taking as argument the project directory where the data should live.
		/// </summary>
		public AssetManager(string projectDirectory)
		{
		}

		/// <summary>
		/// Create a new empty AudioMediaAsset object with the given parameters and add it to the list of managed assets.
		/// </summary>
		/// <param name="channels">Number of channels</param>
		/// <param name="bitDepth">Bit depth</param>
		/// <param name="sampleRate">Sample rate</param>
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

		public Hashtable Assets
		{
			get
			{
				return null;
			}
		}

		public void AddAsset(IMediaAsset asset)
		{
		}

		public Hashtable GetAssets(VirtualAudioBackend.MediaType assetType)
		{
			return null;
		}

		public IMediaAsset GetAsset(string assetName)
		{
			return null;
		}

		public IMediaAsset NewAsset(VirtualAudioBackend.MediaType assetType)
		{
			return null;
		}

		public void DeleteAsset(IMediaAsset assetToDelete)
		{
		}

		public void RemoveAsset(IMediaAsset assetToRemove)
		{
		}

		public IMediaAsset CopyAsset(IMediaAsset asset)
		{
			return null;
		}

		public string RenameAsset(IMediaAsset asset, String newName)
		{
			return null;
		}

		#endregion

	}

}