using System;
using Commands;

namespace VirtualAudioBackend
{
	/// <summary>
	/// This command undoes IAudioMediaAsset.AppendBytes()
	/// </summary>
	public class AppendAudioDataCommand: Command
	{
		public override string Label
		{
			get
			{
				return "append audio data";
			}
		}
 
		/// <summary>
		/// Create a new command to append audio data to an audio asset.
		/// </summary>
		/// <param name="asset">The asset to append to.</param>
		/// <param name="data">The raw audio data, supposed to be in the correct format.</param>
		public AppendAudioDataCommand(IAudioMediaAsset asset, byte[] data)
		{
		}

		/// <summary>
		/// Append the data to the asset.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Delete the data from the end of the asset.
		/// </summary>
		public override void Undo()
		{
		}
	}

	/// <summary>
	/// Command wrapper for IAudioMediaAsset.DeleteChunk()
	/// </summary>
	public class DeleteAudioDataCommand: Command
	{
		public override string Label
		{
			get
			{
				return "delete audio data";
			}
		}

		/// <summary>
		/// Create a new command to delete audio data from an asset.
		/// </summary>
		/// <param name="asset">The asset to delete from.</param>
		/// <param name="beginPosition">Begin position of data to delete in bytes.</param>
		/// <param name="endPosition">End position of data to delete in bytes.</param>
		public DeleteAudioDataCommand(IAudioMediaAsset asset, long beginPosition, long endPosition)
		{
		}

		/// <summary>
		/// Create a new command to delete audio data from an asset.
		/// </summary>
		/// <param name="asset">The asset to delete from.</param>
		/// <param name="beginTime">Begin time of data to delete in milliseconds.</param>
		/// <param name="endTime">End time of data to delete in milliseconds.</param>
		public DeleteAudioDataCommand(IAudioMediaAsset asset, double beginTime, long endTime)
		{
		}

		/// <summary>
		/// Delete the data.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Get the deleted data back into the asset.
		/// </summary>
		public override void Undo()
		{
		}
	}

	/// <summary>
	/// Delete an asset from the project. The asset is actually only removed;
	/// for an actual deletion, use directly the asset's delete function.
	/// </summary>
	public class DeleteAssetCommand: Command
	{
		private IMediaAsset mAsset;           // the deleted asset
		private IAssetManager mAssetManager;  // the asset manager

		public override string Label
		{
			get
			{
				return "delete asset";
			}
		}
		
		/// <summary>
		/// Create a new command to delete an asset.
		/// </summary>
		/// <param name="asset">The asset to delete.</param>
		public DeleteAssetCommand(IMediaAsset asset)
		{
			mAsset = asset;
			mAssetManager = mAsset.Manager;
		}

		/// <summary>
		/// Remove the asset from the asset manager.
		/// </summary>
		public override void Do()
		{
			mAsset.Manager.RemoveAsset(mAsset);
		}
		
		/// <summary>
		/// Add the asset again
		/// </summary>
		public override void Undo()
		{
			mAssetManager.AddAsset(mAsset);
		}
	}

	/// <summary>
	/// Insert an audio asset into another one.
	/// </summary>
	public class InsertAudioAssetCommand: Command
	{
		public override string Label
		{
			get
			{
				return "insert audio data";
			}
		}

		/// <summary>
		/// Create a new command to insert audio data into an asset.
		/// </summary>
		/// <param name="asset">The asset to insert into.</param>
		/// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
		/// <param name="position">The insertion position, in bytes.</param>
		public InsertAudioAssetCommand(IAudioMediaAsset asset, IAudioMediaAsset chunk, long position)
		{
		}

		/// <summary>
		/// Create a new command to insert audio data into an asset.
		/// </summary>
		/// <param name="asset">The asset to insert into.</param>
		/// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
		/// <param name="time">The insertion time, in milliseconds.</param>
		public InsertAudioAssetCommand(IAudioMediaAsset asset, IAudioMediaAsset chunk, double time)
		{
		}

		/// <summary>
		/// Insert the chunk into the asset.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Delete the chunk from the asset.
		/// </summary>
		public override void Undo()
		{
		}
	}

	/// <summary>
	/// Command to merge two audio assets.
	/// </summary>
	public class MergeAudioAssetsCommand: Command
	{
		public override string Label
		{
			get
			{
				return "merge audio assets";
			}
		}

		/// <summary>
		/// Create a new command to merge two audio assets.
		/// </summary>
		/// <param name="first">The first asset.</param>
		/// <param name="second">The second asset.</param>
		public MergeAudioAssetsCommand(IAudioMediaAsset first, IAudioMediaAsset second)
		{
		}

		/// <summary>
		/// Merge the assets.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Split the asset back again.
		/// </summary>
		public override void Undo()
		{
		}
	}

	/// <summary>
	/// Apply phrase detection to an audio asset.
	/// </summary>
	public class PhraseDetectionCommand: Command
	{
		public override string Label
		{
			get
			{
				return "apply phrase detection";
			}
		}

		/// <summary>
		/// Create a new command to apply phrase detection.
		/// </summary>
		/// <param name="asset">The asset to split into phrases.</param>
		/// <param name="threshold">The silence threshold.</param>
		/// <param name="length">The length of silence between sentences (in bytes.)</param>
		/// <param name="before">The legnth of leading silence for a sentece (in bytes.)</param>
		public PhraseDetectionCommand(IAudioMediaAsset asset, long threshold, long length, long before)
		{
		}

		/// <summary>
		/// Create a new command to apply phrase detection.
		/// </summary>
		/// <param name="asset">The asset to split into phrases.</param>
		/// <param name="threshold">The silence threshold.</param>
		/// <param name="length">The length of silence between sentences (in milliseconds.)</param>
		/// <param name="before">The legnth of leading silence for a sentece (in milliseconds.)</param>
		public PhraseDetectionCommand(IAudioMediaAsset asset, long threshold, double length, double before)
		{
		}

		/// <summary>
		/// Apply phrase detection and replace the asset by its phrases.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Merge back the sentences into the original asset.
		/// </summary>
		public override void Undo()
		{
		}
	}

	/// <summary>
	/// Rename an asset.
	/// </summary>
	public class RenameAssetCommand: Command
	{
		private IMediaAsset mAsset;  // the asset to rename
		private string mName;        // the new name of the asset

		public override string Label
		{
			get
			{
				return "rename asset";
			}
		}

		/// <summary>
		/// Create a new command to rename an asset in the asset manager.
		/// </summary>
		/// <param name="asset">The asset to rename.</param>
		/// <param name="name">The new name of the asset.</param>
		public RenameAssetCommand(IMediaAsset asset, string name)
		{
			mAsset = asset;
			mName = name;
		}

		/// <summary>
		/// Change the name of the asset to its new name.
		/// </summary>
		public override void Do()
		{
			mName = mAsset.Manager.RenameAsset(mAsset, mName);
		}

		/// <summary>
		/// Revert to the old name of the asset. (This is actually the same as Do()!)
		/// </summary>
		public override void Undo()
		{
			mName = mAsset.Manager.RenameAsset(mAsset, mName);
		}
	}

	/// <summary>
	/// Command to split an audio asset.
	/// </summary>
	public class SplitAudioAssetCommand: Command
	{
		public override string Label
		{
			get
			{
				return "split audio asset";
			}
		}

		/// <summary>
		/// Create a new command to split an audio asset.
		/// </summary>
		/// <param name="asset">The asset to split.</param>
		/// <param name="position">The split position, in bytes.</param>
		public SplitAudioAssetCommand(IAudioMediaAsset asset, long position)
		{
		}

		/// <summary>
		/// Create a new command to split an audio asset.
		/// </summary>
		/// <param name="asset">The asset to split.</param>
		/// <param name="time">The split time, in milliseconds.</param>
		public SplitAudioAssetCommand(IAudioMediaAsset asset, IAudioMediaAsset chunk, double time)
		{
		}

		/// <summary>
		/// Split the asset.
		/// </summary>
		public override void Do()
		{
		}

		/// <summary>
		/// Merge the two parts of the split asset back again.
		/// </summary>
		public override void Undo()
		{
		}
	}
}