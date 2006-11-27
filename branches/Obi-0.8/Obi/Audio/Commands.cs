using System;
using System.Collections;
using Obi.Commands;

namespace Obi.Audio
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
		public AppendAudioDataCommand(Assets.AudioMediaAsset asset, byte[] data)
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

// member variables
private Assets.AudioMediaAsset m_Asset ;
		private Assets.AudioMediaAsset m_NewAsset ;
private long m_lBeginPosition ;
private long m_lEndPosition ;
private double m_dBeginTime ;
private double m_dEndTime ;
		private bool m_boolTime ;


		/// <summary>
		/// Create a new command to delete audio data from an asset.
		/// </summary>
		/// <param name="asset">The asset to delete from.</param>
		/// <param name="beginPosition">Begin position of data to delete in bytes.</param>
		/// <param name="endPosition">End position of data to delete in bytes.</param>
		public DeleteAudioDataCommand(Assets.AudioMediaAsset asset, long beginPosition, long endPosition)
		{
m_Asset = asset ;
			m_lBeginPosition = beginPosition ;
m_lEndPosition = endPosition ;
m_boolTime = false ;
		}

		/// <summary>
		/// Create a new command to delete audio data from an asset.
		/// </summary>
		/// <param name="asset">The asset to delete from.</param>
		/// <param name="beginTime">Begin time of data to delete in milliseconds.</param>
		/// <param name="endTime">End time of data to delete in milliseconds.</param>
		public DeleteAudioDataCommand(Assets.AudioMediaAsset asset, double beginTime, double endTime)
		{
m_Asset = asset ;
m_dBeginTime = beginTime ;
m_dEndTime = endTime ;
m_boolTime = true ;
		}

		/// <summary>
		/// Delete the data.
		/// </summary>
		public override void Do()
		{
if (m_boolTime == true)
m_NewAsset = m_Asset.DeleteChunk (m_dBeginTime , m_dEndTime) ;
else
	m_NewAsset = m_Asset.DeleteChunk (m_lBeginPosition , m_lEndPosition ) ;

		}

		/// <summary>
		/// Get the deleted data back into the asset.
		/// </summary>
		public override void Undo()
		{
if (m_boolTime == true)
m_Asset.InsertAsset (m_NewAsset, m_dBeginTime ) ;
else
	m_Asset.InsertAsset (m_NewAsset , m_lBeginPosition ) ;
		}
	}

	/// <summary>
	/// Delete an asset from the project. The asset is actually only removed;
	/// for an actual deletion, use directly the asset's delete function.
	/// </summary>
	public class DeleteAssetCommand: Command
	{
		private Assets.MediaAsset mAsset;           // the deleted asset
		private Assets.AssetManager mAssetManager;  // the asset manager

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
		public DeleteAssetCommand(Assets.MediaAsset asset)
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

// memger variables
private Assets.AudioMediaAsset m_Asset ;
		private Assets.AudioMediaAsset m_AssetChunk ;
private long m_lInsertionPosition ;
		private double m_dInsertionTime ;
		private bool m_boolTime ;

		/// <summary>
		/// Create a new command to insert audio data into an asset.
		/// </summary>
		/// <param name="asset">The asset to insert into.</param>
		/// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
		/// <param name="position">The insertion position, in bytes.</param>
		public InsertAudioAssetCommand(Assets.AudioMediaAsset asset, Assets.AudioMediaAsset chunk, long position)
		{	
m_Asset = asset ;
	m_AssetChunk = chunk ;
m_lInsertionPosition = position ;
			m_boolTime = false ;
		}

		/// <summary>
		/// Create a new command to insert audio data into an asset.
		/// </summary>
		/// <param name="asset">The asset to insert into.</param>
		/// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
		/// <param name="time">The insertion time, in milliseconds.</param>
		public InsertAudioAssetCommand(Assets.AudioMediaAsset asset, Assets.AudioMediaAsset chunk, double time)
		{
			m_Asset = asset ;
			m_AssetChunk = chunk ;
			m_dInsertionTime = time ;
			m_boolTime = true ;
		}

		/// <summary>
		/// Insert the chunk into the asset.
		/// </summary>
		public override void Do()
		{
			if (m_boolTime == true)
				m_Asset.InsertAsset (m_AssetChunk , m_dInsertionTime) ;
				else
			m_Asset.InsertAsset (m_AssetChunk , m_lInsertionPosition ) ;

		}

		/// <summary>
		/// Delete the chunk from the asset.
		/// </summary>
		public override void Undo()
		{
if (m_boolTime == true)
m_Asset.DeleteChunk (m_dInsertionTime , m_dInsertionTime + m_AssetChunk.LengthInMilliseconds) ;
			else
m_Asset.DeleteChunk (m_lInsertionPosition , m_lInsertionPosition + m_AssetChunk.AudioLengthInBytes) ;
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

// member variables
		private Assets.AudioMediaAsset m_FirstAsset ;
private Assets.AudioMediaAsset m_SecondAsset ;
private double m_dUndoSplitTime ;

		/// <summary>
		/// Create a new command to merge two audio assets.
		/// </summary>
		/// <param name="first">The first asset.</param>
		/// <param name="second">The second asset.</param>
		public MergeAudioAssetsCommand(Assets.AudioMediaAsset first, Assets.AudioMediaAsset second)
		{
m_FirstAsset = first ;
			m_SecondAsset = second ;
			m_dUndoSplitTime  = m_FirstAsset.LengthInMilliseconds ;
		}

		/// <summary>
		/// Merge the assets.
		/// </summary>
		public override void Do()
		{
m_FirstAsset.MergeWith (m_SecondAsset) ;
		}

		/// <summary>
		/// Split the asset back again.
		/// </summary>
		public override void Undo()
		{
m_SecondAsset = m_FirstAsset.Split(m_dUndoSplitTime ) ;	 
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

		public ArrayList AssetList
		{
			get
			{
return m_alAssets ;
			}
		}

		public Assets.AudioMediaAsset Asset
		{
			get
			{
return m_Asset ;
			}
		}
// member variables
private Assets.AudioMediaAsset m_Asset ;
private ArrayList m_alAssets ;
private Assets.AssetManager m_Manager ;
private long m_lThreshold ;
		private long m_lPhraseLength ;
		private long m_lBefore ;
		private double m_dPhraseLength ;
		private double m_dBefore ;
		private bool m_boolTime ;


		/// <summary>
		/// Create a new command to apply phrase detection.
		/// </summary>
		/// <param name="asset">The asset to split into phrases.</param>
		/// <param name="threshold">The silence threshold.</param>
		/// <param name="length">The length of silence between sentences (in bytes.)</param>
		/// <param name="before">The legnth of leading silence for a sentece (in bytes.)</param>
		public PhraseDetectionCommand(Assets.AudioMediaAsset asset, long threshold, long length, long before)
		{
m_Asset = asset ;
m_Manager = asset.Manager ;
			m_lThreshold = threshold ;
			m_lPhraseLength = length ;
			m_lBefore = before ;
			m_boolTime = false ;
		}

		/// <summary>
		/// Create a new command to apply phrase detection.
		/// </summary>
		/// <param name="asset">The asset to split into phrases.</param>
		/// <param name="threshold">The silence threshold.</param>
		/// <param name="length">The length of silence between sentences (in milliseconds.)</param>
		/// <param name="before">The legnth of leading silence for a sentece (in milliseconds.)</param>
		public PhraseDetectionCommand(Assets.AudioMediaAsset asset, long threshold, double length, double before)
		{
			m_Asset = asset ;
			m_Manager = asset.Manager ;
			m_lThreshold = threshold ;
			m_dPhraseLength = length ;
			m_dBefore = before ;
			m_boolTime = true ;
		}

		/// <summary>
		/// Apply phrase detection and replace the asset by its phrases.
		/// </summary>
		public override void Do()
		{
			if (m_alAssets == null)
			{
				if (m_boolTime == true)
					m_alAssets = new ArrayList ( m_Asset.ApplyPhraseDetection ( m_lThreshold, m_dPhraseLength , m_dBefore)) ;
				else
					m_alAssets = new ArrayList ( m_Asset.ApplyPhraseDetection ( m_lThreshold, m_lPhraseLength , m_lBefore)) ;
			}

// Replace original asset in AssetManager with ArrayList assets
m_Manager.RemoveAsset (m_Asset ) ;

			for ( int n = 0 ;n < m_alAssets.Count ; n++)
			{
			m_Manager.AddAsset (m_alAssets [n] as Assets.AudioMediaAsset) ;
			}

		}

		/// <summary>
		/// Merge back the sentences into the original asset.
		/// </summary>
		public override void Undo()
		{
			foreach (int n in m_alAssets)
			{
                m_Manager.RemoveAsset (m_alAssets [n] as Assets.AudioMediaAsset) ;
			}

m_Manager.AddAsset (m_Asset) ;
		}
	}

	/// <summary>
	/// Rename an asset.
	/// </summary>
	public class RenameAssetCommand: Command
	{
		private Assets.MediaAsset mAsset;  // the asset to rename
		private string mName;        // the new name of the asset
		private string m_sOldName ;

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
		public RenameAssetCommand(Assets.MediaAsset asset, string name)
		{
			mAsset = asset;
			mName = name;
            m_sOldName = asset.Name ;
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
			mName = mAsset.Manager.RenameAsset(mAsset, m_sOldName);
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

// member variables
private Assets.AudioMediaAsset m_Asset ;
private Assets.AudioMediaAsset m_RearAsset ;
		private long m_lSplitPosition ;
private double m_dSplitTime ;
bool m_boolTime ;

		/// <summary>
		/// Create a new command to split an audio asset.
		/// </summary>
		/// <param name="asset">The asset to split.</param>
		/// <param name="position">The split position, in bytes.</param>
		public SplitAudioAssetCommand(Assets.AudioMediaAsset asset, long position)
		{
m_Asset = asset ;
			m_lSplitPosition = position ;
m_boolTime = false ;
		}

		/// <summary>
		/// Create a new command to split an audio asset.
		/// </summary>
		/// <param name="asset">The asset to split.</param>
		/// <param name="time">The split time, in milliseconds.</param>
		public SplitAudioAssetCommand(Assets.AudioMediaAsset asset, double time)
		{
			m_Asset = asset ;
			m_dSplitTime = time ;
			m_boolTime = true ;
		}

		/// <summary>
		/// Split the asset.
		/// </summary>
		public override void Do()
		{
			if (m_boolTime == true) 
m_RearAsset = m_Asset.Split (m_dSplitTime) ;
			else
				m_RearAsset = m_Asset.Split (m_lSplitPosition) ;

		}

		/// <summary>
		/// Merge the two parts of the split asset back again.
		/// </summary>
		public override void Undo()
		{
m_Asset.MergeWith (m_RearAsset) ;
		}
	}
}