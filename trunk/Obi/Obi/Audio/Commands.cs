using System;
using System.Collections;
using Obi.Commands;
using urakawa.media.data;

namespace Obi.Audio
{
    /// <summary>
    /// This command undoes IAudioMediaAsset.AppendBytes()
    /// </summary>
    public class AppendAudioDataCommand : Command
    {
        /// <summary>
        /// Create a new command to append audio data to an audio asset.
        /// </summary>
        /// <param name="asset">The asset to append to.</param>
        /// <param name="data">The raw audio data, supposed to be in the correct format.</param>
        public AppendAudioDataCommand(ManagedAudioMedia asset, byte[] data)
        {
        }

        public override string Label { get { return "append audio data"; } }

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
    public class DeleteAudioDataCommand : Command
    {
        public override string Label
        {
            get
            {
                return "delete audio data";
            }
        }

        // member variables
        private ManagedAudioMedia mAudio;
        private ManagedAudioMedia mNewAudio;
        private long m_lBeginPosition;
        private long m_lEndPosition;
        private double m_dBeginTime;
        private double m_dEndTime;
        private bool m_boolTime;


        /// <summary>
        /// Create a new command to delete audio data from an asset.
        /// </summary>
        /// <param name="asset">The asset to delete from.</param>
        /// <param name="beginPosition">Begin position of data to delete in bytes.</param>
        /// <param name="endPosition">End position of data to delete in bytes.</param>
        public DeleteAudioDataCommand(ManagedAudioMedia audio, long beginPosition, long endPosition)
        {
            mAudio = audio;
            m_lBeginPosition = beginPosition;
            m_lEndPosition = endPosition;
            m_boolTime = false;
        }

        /// <summary>
        /// Create a new command to delete audio data from an asset.
        /// </summary>
        /// <param name="asset">The asset to delete from.</param>
        /// <param name="beginTime">Begin time of data to delete in milliseconds.</param>
        /// <param name="endTime">End time of data to delete in milliseconds.</param>
        public DeleteAudioDataCommand(ManagedAudioMedia audio, double beginTime, double endTime)
        {
            mAudio = audio;
            m_dBeginTime = beginTime;
            m_dEndTime = endTime;
            m_boolTime = true;
        }

        /// <summary>
        /// Delete the data.
        /// </summary>
        public override void Do()
        {
            if (m_boolTime == true)
                mNewAudio = mAudio.DeleteChunk(m_dBeginTime, m_dEndTime);
            else
                mNewAudio = mAudio.DeleteChunk(m_lBeginPosition, m_lEndPosition);

        }

        /// <summary>
        /// Get the deleted data back into the asset.
        /// </summary>
        public override void Undo()
        {
            if (m_boolTime == true)
                mAudio.InsertAsset(mNewAudio, m_dBeginTime);
            else
                mAudio.InsertAsset(mNewAudio, m_lBeginPosition);
        }
    }

    /// <summary>
    /// Delete an asset from the project. The asset is actually only removed;
    /// for an actual deletion, use directly the asset's delete function.
    /// </summary>
    public class DeleteAssetCommand : Command
    {
        private ManagedAudioMedia mAudio;   // deleted audio
        private MediaDataManager mManager;  // the data manager for this audio media

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
        public DeleteAssetCommand(ManagedAudioMedia audio)
        {
            mAudio = audio;
            mManager = audio.getMediaData().getMediaDataManager();
        }

        /// <summary>
        /// Remove the asset from the asset manager.
        /// </summary>
        public override void Do()
        {
            mAudio.Manager.RemoveAsset(mAudio);
        }

        /// <summary>
        /// Add the asset again
        /// </summary>
        public override void Undo()
        {
            mAssetManager.AddAsset(mAudio);
        }
    }

    /// <summary>
    /// Insert an audio asset into another one.
    /// </summary>
    public class InsertAudioAssetCommand : Command
    {
        public override string Label
        {
            get
            {
                return "insert audio data";
            }
        }

        // memger variables
        private ManagedAudioMedia mAudio;
        private ManagedAudioMedia mChunk;  // TODO this is probably wrong
        private long m_lInsertionPosition;
        private double m_dInsertionTime;
        private bool m_boolTime;

        /// <summary>
        /// Create a new command to insert audio data into an asset.
        /// </summary>
        /// <param name="asset">The asset to insert into.</param>
        /// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
        /// <param name="position">The insertion position, in bytes.</param>
        public InsertAudioAssetCommand(ManagedAudioMedia audio, ManagedAudioMedia chunk, long position)
        {
            mAudio = audio;
            mChunk = chunk;
            m_lInsertionPosition = position;
            m_boolTime = false;
        }

        /// <summary>
        /// Create a new command to insert audio data into an asset.
        /// </summary>
        /// <param name="asset">The asset to insert into.</param>
        /// <param name="chunk">The chunk of data (as an asset itself) to insert into the asset.</param>
        /// <param name="time">The insertion time, in milliseconds.</param>
        public InsertAudioAssetCommand(ManagedAudioMedia audio, ManagedAudioMedia chunk, double time)
        {
            mAudio = audio;
            mChunk = chunk;
            m_dInsertionTime = time;
            m_boolTime = true;
        }

        /// <summary>
        /// Insert the chunk into the asset.
        /// </summary>
        public override void Do()
        {
            if (m_boolTime == true)
                mAudio.InsertAsset(mChunk, m_dInsertionTime);
            else
                mAudio.InsertAsset(mChunk, m_lInsertionPosition);

        }

        /// <summary>
        /// Delete the chunk from the asset.
        /// </summary>
        public override void Undo()
        {
            if (m_boolTime == true)
                mAudio.DeleteChunk(m_dInsertionTime, m_dInsertionTime + mChunk.LengthInMilliseconds);
            else
                mAudio.DeleteChunk(m_lInsertionPosition, m_lInsertionPosition + mChunk.AudioLengthInBytes);
        }
    }

    /// <summary>
    /// Command to merge two audio assets.
    /// </summary>
    public class MergeAudioAssetsCommand : Command
    {
        public override string Label
        {
            get
            {
                return "merge audio assets";
            }
        }

        // member variables
        private ManagedAudioMedia mFirstAudio;
        private ManagedAudioMedia mSecondAudio;
        private double m_dUndoSplitTime;

        /// <summary>
        /// Create a new command to merge two audio assets.
        /// </summary>
        /// <param name="first">The first asset.</param>
        /// <param name="second">The second asset.</param>
        public MergeAudioAssetsCommand(ManagedAudioMedia first, ManagedAudioMedia second)
        {
            mFirstAudio = first;
            mSecondAudio = second;
            m_dUndoSplitTime = mFirstAudio.LengthInMilliseconds;
        }

        /// <summary>
        /// Merge the assets.
        /// </summary>
        public override void Do()
        {
            mFirstAudio.MergeWith(mSecondAudio);
        }

        /// <summary>
        /// Split the asset back again.
        /// </summary>
        public override void Undo()
        {
            mSecondAudio = mFirstAudio.Split(m_dUndoSplitTime);
        }
    }

    /// <summary>
    /// Apply phrase detection to an audio asset.
    /// </summary>
    public class PhraseDetectionCommand : Command
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
                return m_alAssets;
            }
        }

        public ManagedAudioMedia Audio
        {
            get
            {
                return mAudio;
            }
        }
        // member variables
        private ManagedAudioMedia mAudio;
        private ArrayList m_alAssets;
        private MediaDataManager mManager;
        private long m_lThreshold;
        private long m_lPhraseLength;
        private long m_lBefore;
        private double m_dPhraseLength;
        private double m_dBefore;
        private bool m_boolTime;


        /// <summary>
        /// Create a new command to apply phrase detection.
        /// </summary>
        /// <param name="asset">The asset to split into phrases.</param>
        /// <param name="threshold">The silence threshold.</param>
        /// <param name="length">The length of silence between sentences (in bytes.)</param>
        /// <param name="before">The legnth of leading silence for a sentece (in bytes.)</param>
        public PhraseDetectionCommand(ManagedAudioMedia audio, long threshold, long length, long before)
        {
            mAudio = audio;
            mManager = audio.getMediaData().getMediaDataManager();
            m_lThreshold = threshold;
            m_lPhraseLength = length;
            m_lBefore = before;
            m_boolTime = false;
        }

        /// <summary>
        /// Create a new command to apply phrase detection.
        /// </summary>
        /// <param name="asset">The asset to split into phrases.</param>
        /// <param name="threshold">The silence threshold.</param>
        /// <param name="length">The length of silence between sentences (in milliseconds.)</param>
        /// <param name="before">The legnth of leading silence for a sentece (in milliseconds.)</param>
        public PhraseDetectionCommand(ManagedAudioMedia audio, long threshold, double length, double before)
        {
            mAudio = audio;
            mManager = audio.getMediaData().getMediaDataManager();
            m_lThreshold = threshold;
            m_dPhraseLength = length;
            m_dBefore = before;
            m_boolTime = true;
        }

        /// <summary>
        /// Apply phrase detection and replace the asset by its phrases.
        /// </summary>
        public override void Do()
        {
            if (m_alAssets == null)
            {
                if (m_boolTime == true)
                    m_alAssets = new ArrayList(mAudio.ApplyPhraseDetection(m_lThreshold, m_dPhraseLength, m_dBefore));
                else
                    m_alAssets = new ArrayList(mAudio.ApplyPhraseDetection(m_lThreshold, m_lPhraseLength, m_lBefore));
            }

            // Replace original asset in AssetManager with ArrayList assets
            mManager.RemoveAsset(mAudio);

            for (int n = 0; n < m_alAssets.Count; n++)
            {
                mManager.AddAsset(m_alAssets[n] as Assets.AudioMediaAsset);
            }

        }

        /// <summary>
        /// Merge back the sentences into the original asset.
        /// </summary>
        public override void Undo()
        {
            foreach (int n in m_alAssets)
            {
                mManager.RemoveAsset(m_alAssets[n] as Assets.AudioMediaAsset);
            }

            mManager.AddAsset(mAudio);
        }
    }

    /// <summary>
    /// Rename an asset.
    /// </summary>
    public class RenameAssetCommand : Command
    {
        private ManagedAudioMedia mAudio;  // the asset to rename
        private string mName;        // the new name of the asset
        private string m_sOldName;

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
        public RenameAssetCommand(ManagedAudioMedia audio, string name)
        {
            mAudio = audio;
            mName = name;
            m_sOldName = asset.Name;
        }

        /// <summary>
        /// Change the name of the asset to its new name.
        /// </summary>
        public override void Do()
        {
            mName = mAudio.Manager.RenameAsset(mAudio, mName);
        }

        /// <summary>
        /// Revert to the old name of the asset. (This is actually the same as Do()!)
        /// </summary>
        public override void Undo()
        {
            mName = mAudio.Manager.RenameAsset(mAudio, m_sOldName);
        }
    }

    /// <summary>
    /// Command to split an audio asset.
    /// </summary>
    public class SplitAudioAssetCommand : Command
    {
        public override string Label
        {
            get
            {
                return "split audio asset";
            }
        }

        // member variables
        private ManagedAudioMedia mAudio;
        private ManagedAudioMedia mRearAudio;
        private long m_lSplitPosition;
        private double m_dSplitTime;
        bool m_boolTime;

        /// <summary>
        /// Create a new command to split an audio asset.
        /// </summary>
        /// <param name="asset">The asset to split.</param>
        /// <param name="position">The split position, in bytes.</param>
        public SplitAudioAssetCommand(ManagedAudioMedia audio, long position)
        {
            mAudio = audio;
            m_lSplitPosition = position;
            m_boolTime = false;
        }

        /// <summary>
        /// Create a new command to split an audio asset.
        /// </summary>
        /// <param name="asset">The asset to split.</param>
        /// <param name="time">The split time, in milliseconds.</param>
        public SplitAudioAssetCommand(ManagedAudioMedia audio, double time)
        {
            mAudio = audio;
            m_dSplitTime = time;
            m_boolTime = true;
        }

        /// <summary>
        /// Split the asset.
        /// </summary>
        public override void Do()
        {
            if (m_boolTime == true)
                mRearAudio = mAudio.Split(m_dSplitTime);
            else
                mRearAudio = mAudio.Split(m_lSplitPosition);

        }

        /// <summary>
        /// Merge the two parts of the split asset back again.
        /// </summary>
        public override void Undo()
        {
            mAudio.MergeWith(mRearAudio);
        }
    }
}