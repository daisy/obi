using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Obi.Assets
{
    public class AudioMediaAsset : MediaAsset
    {
        public static readonly double DefaultGap = 500.0;             // default gap for phrase detection
        public static readonly double DefaultLeadingSilence = 100.0;  // default leading silence

        /// <summary>
        /// The empty audio media for phrase nodes that do not have audio.
        /// </summary>
        public static AudioMediaAsset Empty
        {
            get { return mEmptyAsset; }
        }

        // The actual asset does not really matter much
        // Should be default project parameters though?
        private static readonly AudioMediaAsset mEmptyAsset = new AudioMediaAsset(1, 8, 8000);

        private int mChannels;
        private int mBitDepth;
        private int mSamplingRate;
        private int mFrameSize;
        private long mAudioLengthInBytes = 0;
        private double mAudioLengthInTime = 0;

        public int SampleRate
        {
            get { return mSamplingRate; }
        }

        public int Channels
        {
            get { return mChannels; }
        }

        public int BitDepth
        {
            get { return mBitDepth; }
        }

        internal int FrameSize
        {
            get { return mFrameSize; }
        }

        public double LengthInMilliseconds
        {
            get { return mAudioLengthInTime; }
        }

        public string LengthInSeconds
        {
            get { return (Math.Round(mAudioLengthInTime / 1000.0)).ToString() + "s"; }
        }

        public long AudioLengthInBytes
        {
            get { return mAudioLengthInBytes; }
        }

        private List<AudioClip> mClips;

        /// <summary>
        /// The list of audio clips for this asset.
        /// </summary>
        public List<AudioClip> Clips
        {
            get { return mClips; }
        }

        /// <summary>
        /// Constructor for an empty AudioMediaAsset. The format is specified by the arguments and there is no initial audio data.
        /// </summary>
        /// <param name="channels">Number of channels (1 or 2.)</param>
        /// <param name="bitDepth">Bit depth (8 or 16?)</param>
        /// <param name="sampleRate">Sample rate in Hz.</param>
        public AudioMediaAsset(int channels, int bitDepth, int sampleRate)
        {
            if (channels >= 1 && channels <= 2 && bitDepth >= 8 && bitDepth <= 16 && sampleRate >= 8000)
            {
                mChannels = channels;
                mBitDepth = bitDepth;
                mSamplingRate = sampleRate;
                mFrameSize = (mBitDepth / 8) * mChannels;
                mMediaType = MediaType.Audio;
                mClips = new List<AudioClip>();
            }
            else
                throw new Exception("Audio media of this format is not supported");
        }

        /// <summary>
        /// Constructor for an audio asset from existing clips.
        /// </summary>
        /// <param name="clips">The list of <see cref="AudioClip"/>s.</param>
        //public AudioMediaAsset(ArrayList clips)
        public AudioMediaAsset(List<AudioClip> clips)
        {
            if (clips != null)
            {
                mMediaType = MediaType.Audio;
                AudioClip ob_AudioClip = clips[0] as AudioClip;
                mChannels = ob_AudioClip.Channels;
                mBitDepth = ob_AudioClip.BitDepth;
                mSamplingRate = ob_AudioClip.SampleRate;
                mFrameSize = ob_AudioClip.FrameSize;
                //m_alClipList = clips;
                mClips = clips;
                mAudioLengthInTime = 0;

                // compute total time ofasset from clip lengths
                for (int i = 0; i < clips.Count; i++)
                {

                    ob_AudioClip = clips[i] as AudioClip;

                    if (mChannels == ob_AudioClip.Channels && mBitDepth == ob_AudioClip.BitDepth && mSamplingRate == ob_AudioClip.SampleRate)
                        mAudioLengthInTime = mAudioLengthInTime + ob_AudioClip.LengthInTime;
                    else
                        throw new Exception("Clip format do not match Asset format");
                }

                mAudioLengthInBytes = Audio.CalculationFunctions.ConvertTimeToByte(mAudioLengthInTime, mSamplingRate, mFrameSize);
                mSizeInBytes = mAudioLengthInBytes;
            }
            else
                throw new Exception("No AudioMediaAsset can be created as clip list is empty");
        }

        /// <summary>
        /// Make a copy of the asset, sharing the same format and data.
        /// The copy has the same name and is not managed (yet).
        /// </summary>
        /// <returns>The new, identical asset.</returns>
        public override Assets.MediaAsset Copy()
        {
            AudioMediaAsset copy = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
            copy.mAssManager = null;
            copy.mAudioLengthInTime = mAudioLengthInTime;
            copy.mAudioLengthInBytes = mAudioLengthInBytes;
            copy.mSizeInBytes = mSizeInBytes;
            copy.mFrameSize = mFrameSize;
            copy.mMediaType = mMediaType;
            copy.mName = mName;
            foreach (AudioClip clip in mClips) copy.mClips.Add(clip);
            return copy;
        }

        /// <summary>
        /// Remove the asset from the project, and actually delete all corresponding resources.
        /// Throw an exception if the asset could not be deleted.
        /// </summary>
        public override void Delete()
        {
            // clean up physical resources
            //AudioClip ob_Clip;

            //for (int i = 0; i < m_alClipList.Count; i++)
            //{
            //    ob_Clip = m_alClipList[i] as AudioClip;
            foreach (AudioClip clip in mClips)
            {
                clip.DeletePhysicalResource();
                // Remove clip from clip list hash table
                AudioClip.static_htClipExists.Remove(clip.Name);
            }// current clip ends

            //m_alClipList = null;
            mClips = null;
            mAssManager = null;
            mName = null;

        }

        public void AppendBytes(byte[] data)
        {
        }

        public Assets.AudioMediaAsset GetChunk(long beginPosition, long endPosition)
        {
            double dBeginTime = Audio.CalculationFunctions.ConvertByteToTime(beginPosition, mSamplingRate, mFrameSize);
            double dEndTime = Audio.CalculationFunctions.ConvertByteToTime(endPosition, mSamplingRate, mFrameSize);
            return GetChunk(dBeginTime, dEndTime);
        }

        public Assets.AudioMediaAsset GetChunk(double beginTime, double endTime)
        {
            // checks if the input parameters are in bounds of asset and in  order
            if (beginTime >= 0 && beginTime < endTime && endTime <= mAudioLengthInTime)
            {
                //ArrayList alNewClipList = new ArrayList();
                List<AudioClip> alNewClipList = new List<AudioClip>();

                // finds the data for chunk begin point including Clip index, local clip time etc from FindClipToProcess in form of ArrayList and copy it in an ArrayList active in this function
                ArrayList alBeginList = new ArrayList(FindClipToProcess(beginTime));
                //BeginClipIndex  is index of clip in Asset Clip list which is to be split at begin point
                int BeginClipIndex = Convert.ToInt32(alBeginList[0]);
                // dBeginTimeMark is the time marking in target clip  at which point split has to be made
                double dBeginTimeMark = Convert.ToDouble(alBeginList[1]);

                // All above steps are repeated for finding marking for EndTime of chunk
                ArrayList alEndList = new ArrayList(FindClipToProcess(endTime));
                int EndClipIndex = Convert.ToInt32(alEndList[0]);
                double dEndTimeMark = Convert.ToDouble(alEndList[1]);

                // transfer clip to process to separate object
                // AudioClip ob_BeginClip = m_alClipList[BeginClipIndex] as AudioClip;
                AudioClip ob_BeginClip = mClips[BeginClipIndex];

                // if begin time and end time lie in same clip then make a new clip from that clip create an asset for it and return
                if (BeginClipIndex == EndClipIndex)
                {
                    AudioClip ob_NewClip = ob_BeginClip.CopyClipPart(dBeginTimeMark, dEndTimeMark);
                    alNewClipList.Add(ob_NewClip);

                }
                else
                {
                    // Normalise EndClip from m_ClipList to original class
                    // AudioClip ob_EndClip = m_alClipList[EndClipIndex] as AudioClip;
                    AudioClip ob_EndClip = mClips[EndClipIndex];

                    // branch if BeginClip time mark is not end of target clip
                    if (dBeginTimeMark < ob_BeginClip.LengthInTime)
                    {
                        // derive new begin  clip from target clip
                        AudioClip ob_NewBeginClip = ob_BeginClip.CopyClipPart(dBeginTimeMark, ob_BeginClip.LengthInTime);
                        //if (ob_NewBeginClip.Equals (null)  ) 
                        //MessageBox.Show ("if (dBeginTimeMark <ob_BeginClip.LengthInTime )") ;
                        // Add new derived begin clip to clip list of return asset
                        alNewClipList.Add(ob_NewBeginClip);
                    }

                    // add clips between beginClip index and EndClip index to ClipList of return asset excluding begin and end clips
                    for (int i = BeginClipIndex + 1; i < EndClipIndex; i++)
                    {
                    //    alNewClipList.Add(m_alClipList[i]);
                        alNewClipList.Add(mClips[i]);
                    }

                    // if EndClip time mark is not at beginning of target clip then do following
                    if (dEndTimeMark > 0)
                    {
                        // Create new endClip to be added to Clip list of return asset from target end clip
                        AudioClip ob_NewEndClip = ob_EndClip.CopyClipPart(0, dEndTimeMark);

                        //if (ob_NewEndClip.Equals (null)  ) 
                        //MessageBox.Show ("if (dEndTimeMark > 0)") ;


                        alNewClipList.Add(ob_NewEndClip);
                    }

                }


                // create return AudioMediaAsset from new clip list
                AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset(alNewClipList);
                return ob_AudioMediaAsset;
            }
            else
                throw new Exception("Invalid input parameters");

        }

        public void InsertAsset(Assets.AudioMediaAsset chunk, long position)
        {
            double dPosition = Audio.CalculationFunctions.ConvertByteToTime(position, mSamplingRate, mFrameSize);
            InsertAsset(chunk, dPosition);
        }

        public void InsertAsset(Assets.AudioMediaAsset chunk, double time)
        {
            // checks if audio formats of original asset and chunk asset are of same formats
            if (CompareAudioAssetFormat(this, chunk) == true && time <= mAudioLengthInTime && time >= 0)
            {
                // creates the temporary blank asset
                AudioMediaAsset ob1 = new AudioMediaAsset(this.Channels, this.BitDepth, this.SampleRate);

                // if Chunk is to be inserted somewhere in between of original asset
                if (time > -0 && time < mAudioLengthInTime)
                {
                    // copies part of original asset before insertion time to temporary ob1 asset
                    ob1 = GetChunk(0, time) as AudioMediaAsset;
                    // merges the chunk to temp ob1 asset
                    ob1.MergeWith(chunk);
                    //					copies part of original assetafter insertion time to temporary ob2 asset
                    AudioMediaAsset ob2 = GetChunk(time, this.LengthInMilliseconds) as AudioMediaAsset;
                    // merge ob2 at back of ob1 so as to finalise ob1
                    ob1.MergeWith(ob2);


                }
                // if chunk asset is to be placed before original asset
                else if (time == 0)
                {
                    // points chunk to ob1 and merge original asset at back of ob1
                    ob1 = chunk as AudioMediaAsset;
                    ob1.MergeWith(this);
                }
                // clears clip list of original asset and copy clips in clip list of ob1 to it
                //m_alClipList.Clear();
                //for (int i = 0; i < ob1.m_alClipList.Count; i++)
                //{
                //    m_alClipList.Add(ob1.m_alClipList[i]);
                //}
                mClips.Clear();
                foreach (AudioClip clip in ob1.Clips) mClips.Add(clip);
                mAudioLengthInTime = ob1.LengthInMilliseconds;
                mAudioLengthInBytes = ob1.AudioLengthInBytes;
                mSizeInBytes = ob1.SizeInBytes;

                // if Chunk is to be appended to original asset
                if (time == mAudioLengthInTime)
                {
                    MergeWith(chunk);
                }

            } // end of main format check
            else
            {
                throw new Exception("Incompatible format or Insertion time not in asset range");
            }


            // end of insert chunk function
        }

        public Assets.AudioMediaAsset DeleteChunk(long beginPosition, long endPosition)
        {
            double dBeginTime = Audio.CalculationFunctions.ConvertByteToTime(beginPosition, mSamplingRate, mFrameSize);
            double dEndTime = Audio.CalculationFunctions.ConvertByteToTime(endPosition, mSamplingRate, mFrameSize);
            return DeleteChunk(dBeginTime, dEndTime);
        }

        public Assets.AudioMediaAsset DeleteChunk(double beginTime, double endTime)
        {
            // checks if beginTime and EndTime is within bounds of asset and are in order
            if (beginTime >= 0 && beginTime < endTime && endTime <= mAudioLengthInTime)
            {
                // create new asset from original asset from part which has to be deleted and keep for returning back
                AudioMediaAsset ob_NewAsset = GetChunk(beginTime, endTime) as AudioMediaAsset;

                // create two temp assets for holding clips in front of BeginTime and a asset to hold  Clips after endTime
                AudioMediaAsset ob_FromtAsset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
                AudioMediaAsset ob_RearAsset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);

                // if deletion part lies somewhere in between body of asset
                if (beginTime != 0 && endTime != mAudioLengthInTime)
                {
                    // Copy respective  clips  to Front and Rear Assets and merge them
                    ob_FromtAsset = GetChunk(0, beginTime) as AudioMediaAsset;

                    ob_RearAsset = GetChunk(endTime, mAudioLengthInTime) as AudioMediaAsset;

                    ob_FromtAsset.MergeWith(ob_RearAsset);
                }
                // if deletion is from in between to end of asset
                else if (beginTime != 0)
                {
                    // copies only front part of asset
                    ob_FromtAsset = GetChunk(0, beginTime) as AudioMediaAsset;
                }
                // if Deletion is in front including start
                else if (endTime != mAudioLengthInTime)
                {
                    // copies end part of asset to front asset
                    ob_FromtAsset = GetChunk(endTime, mAudioLengthInTime) as AudioMediaAsset;
                }

                // replaces clip list of original asset with clip list of front asset
                // m_alClipList = ob_FromtAsset.m_alClipList;
                mClips = ob_FromtAsset.Clips;
                mAudioLengthInTime = ob_FromtAsset.LengthInMilliseconds;
                mAudioLengthInBytes = ob_FromtAsset.AudioLengthInBytes;
                mSizeInBytes = ob_FromtAsset.SizeInBytes;

                ob_FromtAsset = null;

                return ob_NewAsset;
            }
            else
                throw new Exception("Invalid input parameters");


        }

        internal ArrayList FindClipToProcess(double Time)
        {
            // checks if the time taken as input parameter is within bounds of Asset
            if (Time <= mAudioLengthInTime && Time >= 0)
            {

                //AudioClip ob_AudioClip = m_alClipList[0] as AudioClip;
                AudioClip ob_AudioClip = mClips[0];

                // Add the length in time of each clip and add untill time parameter is close enough upto atmost one clip distance
                double TimeSum = 0;
                int Count = 0;
                while (TimeSum <= Time && TimeSum < mAudioLengthInTime)
                {
                    //if (Count < m_alClipList.Count)
                    //    ob_AudioClip = m_alClipList[Count] as AudioClip;
                    if (Count < mClips.Count)
                    {
                        ob_AudioClip = mClips[Count];
                    }
                    else
                    {

                        break;
                    }
                    TimeSum = TimeSum + ob_AudioClip.LengthInTime;
                    Count++;
                }
                // decrement count by one so as to            compensate one increment done to it in while loop
                Count--;

                // find localtime of target clip
                //ob_AudioClip = m_alClipList[Count] as AudioClip;
                ob_AudioClip = mClips[Count];
                double NewClipTime = TimeSum - Time;

                NewClipTime = ob_AudioClip.LengthInTime - NewClipTime;

                // create ArrayList to return at index one the " target clip index" and in second index "local clip time to process"
                ArrayList alReturnList = new ArrayList();
                alReturnList.Add(Count);

                alReturnList.Add(NewClipTime);

                return alReturnList;
            }
            else
                throw new Exception("find clip time is out of bound of Asset time");

        }

        /// <summary>
        /// Merge with another asset.
        /// The asset is modified in place, the "next" asset being appended.
        /// </summary>
        /// <param name="next">The next asset to merge with.</param>
        public override void MergeWith(Assets.MediaAsset next)
        {
            AudioMediaAsset _next = next as AudioMediaAsset;
            // checks if the formats of both clips is same
            if (!CompareAudioAssetFormat(this, _next))
            {
                throw new Exception("Cannot merge assets: incompatible format");
            }
            // append clips of next asset to clip list of original asset
            foreach (AudioClip clip in _next.Clips) mClips.Add(clip);
            mAudioLengthInTime = mAudioLengthInTime + _next.LengthInMilliseconds;
            mAudioLengthInBytes = mAudioLengthInBytes + _next.AudioLengthInBytes;
            mSizeInBytes = mSizeInBytes + _next.SizeInBytes;
        }

        public Assets.AudioMediaAsset Split(long position)
        {
            double dTime = Audio.CalculationFunctions.ConvertByteToTime(position, mSamplingRate, mFrameSize);
            return Split(dTime);
        }

        public Assets.AudioMediaAsset Split(double time)
        {
            // checks if time parameter is in bounds of asset
            if (time >= 0 && time <= mAudioLengthInTime)
            {
                // create new asset for clips after time specified in parameter

                AudioMediaAsset ob_AudioMediaAsset = GetChunk(time, mAudioLengthInTime) as AudioMediaAsset;

                //// modify original asset
                ArrayList alMarksList = new ArrayList(FindClipToProcess(time));
                int ClipIndex = Convert.ToInt32(alMarksList[0]);
                double dClipTimeMark = Convert.ToDouble(alMarksList[1]);


                // AudioClip ob_AudioClip = m_alClipList[ClipIndex] as AudioClip;
                AudioClip ob_AudioClip = mClips[ClipIndex];

                if (dClipTimeMark > 0 && dClipTimeMark < ob_AudioClip.LengthInTime)
                {
                    ob_AudioClip.Split(dClipTimeMark);
                }
                else if (dClipTimeMark == 0)
                {
                    ClipIndex--;
                }
                //MessageBox.Show (m_alClipList.Count.ToString () ) ;
                // Remove clips after clip index
                //m_alClipList.RemoveRange(ClipIndex + 1, (m_alClipList.Count - ClipIndex - 1));
                mClips.RemoveRange(ClipIndex + 1, (mClips.Count - ClipIndex - 1));

                mAudioLengthInTime = mAudioLengthInTime - ob_AudioMediaAsset.LengthInMilliseconds;
                mAudioLengthInBytes = mAudioLengthInBytes - ob_AudioMediaAsset.AudioLengthInBytes;
                mSizeInBytes = mAudioLengthInBytes;

                return ob_AudioMediaAsset;
            }
            else
                throw new Exception("Cannot split: parameter value out of bound of asset");
        }

        /// <summary>
        /// Get the maximum amplitude of silence in a given "silent" asset.
        /// </summary>
        /// <returns>The amplitude.</returns>
        public long GetSilenceAmplitude()
        {
            long max = 0;
            foreach (AudioClip clip in mClips)
            {
                long amplitude = clip.GetClipSilenceAmplitude();
                if (amplitude > max) max = amplitude;
            }
            max = max + 10 ;
            return max;
        }

        /// <summary>
        /// Apply phrase detection to a phrase.
        /// </summary>
        /// <param name="threshold">Silence threshold.</param>
        /// <param name="length">Minimum silence gap between phrases (in bytes).</param>
        /// <param name="before">Maximum leading silence before a phrase (in bytes).</param>
        /// <returns>The list of detected phrases.</returns>
        public List <AudioMediaAsset> ApplyPhraseDetection(long threshold, long length, long before)
        {
            AudioClip ob_Clip;
            // AssetList is list of assets returned by phrase detector
            ArrayList alAssetList = new ArrayList();
            // clipList is clip list for each return asset
            ArrayList alClipList;
            AudioMediaAsset ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);

            // apply phrase detection on each clip in clip list of this asset
            for (int i = 0; i < mClips.Count; ++i)
            {
                ob_Clip = mClips[i];
                alClipList = ob_Clip.DetectPhrases(threshold, length, before);
                if (Convert.ToBoolean(alClipList[0]) == false)
                {
                    ob_Asset.AddClip(alClipList[1] as AudioClip);
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                    }
                }
                else
                {
                    if (ob_Clip.BeginTime + 3000 < (alClipList[1] as AudioClip).BeginTime)
                    {
                        ob_Asset.AddClip(ob_Clip.CopyClipPart(0, (alClipList[1] as AudioClip).BeginTime - ob_Clip.BeginTime));
                        if (i == 0)
                            alAssetList.Add(ob_Asset);
                    }
                    if (i != 0)
                        alAssetList.Add(ob_Asset);
                    for (int j = 1; j < alClipList.Count - 1; j++)
                    {
                        ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
                        ob_Asset.AddClip(alClipList[j] as AudioClip);
                        alAssetList.Add(ob_Asset);
                    }
                    ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
                    if (alClipList.Count > 2)
                        ob_Asset.AddClip(alClipList[alClipList.Count - 1] as AudioClip);
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                    }
                }
            }
            List<AudioMediaAsset> RetList = new List<AudioMediaAsset>();
            for (int n = 0; n < alAssetList.Count; n++)
            {
                RetList.Add(alAssetList[n] as AudioMediaAsset);
            }
            return RetList;
        }

        /// <summary>
        /// Apply phrase detection to a phrase.
        /// </summary>
        /// <param name="threshold">Silence threshold.</param>
        /// <param name="length">Minimum silence gap between phrases (in milliseconds).</param>
        /// <param name="before">Maximum leading silence before a phrase (in milliseconds).</param>
        /// <returns>The list of detected phrases.</returns>
        public List<AudioMediaAsset> ApplyPhraseDetection(long threshold, double length, double before)
        {
            // convert input parameters from time to byte
            long lLength = Audio.CalculationFunctions.ConvertTimeToByte(length, mSamplingRate, mFrameSize);
            long lBefore = Audio.CalculationFunctions.ConvertTimeToByte(before, mSamplingRate, mFrameSize);
            return ApplyPhraseDetection(threshold, lLength, lBefore);
        }

        // function to compute the amplitude of a small chunck of samples

        long BlockSum(BinaryReader br, long Pos, int Block, int FrameSize, int
            Channels)
        {
            long sum = 0;
            long SubSum;
            for (int i = 0; i < Block; i = i + FrameSize)
            {
                br.BaseStream.Position = i + Pos;
                SubSum = 0;
                if (FrameSize == 1)
                {
                    SubSum = Convert.ToInt64((br.ReadByte()));

                    // FrameSize 1 ends
                }
                else if (FrameSize == 2)
                {
                    if (Channels == 1)
                    {
                        SubSum = Convert.ToInt64(br.ReadByte());
                        SubSum = SubSum + (Convert.ToInt64(br.ReadByte()) * 256); SubSum = (SubSum * 256) / 65792;
                    }
                    else if (Channels == 2)
                    {
                        SubSum = Convert.ToInt64(br.ReadByte());
                        SubSum = SubSum + Convert.ToInt64(br.ReadByte()); SubSum = SubSum / 2;
                    }
                    // FrameSize 2 ends
                }
                else if (FrameSize == 4)
                {
                    if (Channels == 1)
                    {
                        SubSum = Convert.ToInt64(br.ReadByte());
                        SubSum = SubSum +
                            (Convert.ToInt64(br.ReadByte()) * 256);
                        SubSum = SubSum +
                            (Convert.ToInt64(br.ReadByte()) * 256 * 256);
                        SubSum = SubSum +
                            (Convert.ToInt64(br.ReadByte()) * 256 * 256 * 256);
                    }
                    else if (Channels == 2)
                    {
                        SubSum = Convert.ToInt64(br.ReadByte());

                        SubSum = SubSum + (Convert.ToInt64(br.ReadByte()) * 256);

                        // second channel
                        SubSum = SubSum + Convert.ToInt64(br.ReadByte()); SubSum = SubSum + (Convert.ToInt64(br.ReadByte()) * 256);
                        SubSum = (SubSum * 256) / (65792 * 2);

                    }
                    // FrameSize 4 ends
                }
                sum = sum + SubSum;


                // Outer, For ends
            }



            sum = sum / (Block / FrameSize);

            //MessageBox.Show(sum.ToString()) ;
            return sum;
        }


        public void AddClip(AudioClip Clip)
        {
            if (mChannels == Clip.Channels && mBitDepth == Clip.BitDepth && mSamplingRate == Clip.SampleRate)
            {
                // m_alClipList.Add(Clip);
                mClips.Add(Clip);
                mAudioLengthInTime = mAudioLengthInTime + Clip.LengthInTime;
                mAudioLengthInBytes = Audio.CalculationFunctions.ConvertTimeToByte(mAudioLengthInTime, mSamplingRate, mFrameSize);
                mSizeInBytes = mAudioLengthInBytes;
            }
            else
                throw new Exception("Clip format do not match Asset format");
        }

        private bool CompareAudioAssetFormat(Assets.AudioMediaAsset asset1, Assets.AudioMediaAsset asset2)
        {
            return asset1.Channels == asset2.Channels &&
                asset2.SampleRate == asset2.SampleRate &&
                asset1.BitDepth == asset2.BitDepth;
        }

        /// <summary>
        /// Save the asset into a single file.
        /// </summary>
        /// <param name="path">Path of the file to save.</param>
        public void Export(string path)
        {

            if (mClips.Count != 0)
            {


                BinaryWriter bw = new BinaryWriter(File.Create(path));
                BinaryReader br;
                br = new BinaryReader(File.OpenRead(mClips[0].Path));

                for (int i = 0; i < 44; i++)
                {
                    bw.Write(br.ReadByte());
                }

                bw.BaseStream.Position = 44;

                long ByteLengthCount = 0;
                for (int i = 0; i < this.mClips.Count; i++)
                {
                    br = new BinaryReader(File.OpenRead(mClips[i].Path));
                    br.BaseStream.Position = mClips[i].BeginByte + 44 ;
                    for (long l = mClips[i].BeginByte; l < mClips[i].EndByte; l++)
                    {
                        bw.Write(br.ReadByte());
                        ByteLengthCount++;
                    }
                }
                UpdateLengthHeader(ByteLengthCount, bw);
                bw.Close();
                br.Close();
            }
            else
            {
                
                throw new Exception("No Clip in Asset");
            }
            
        }
        
        /// <summary>
        /// Export function for converting a list of audiomedia assets to a list of exportable AudioMediaAsset pointing to a single file
        /// <see cref=""/>
        /// </summary>
        /// <param name="AssetList"></param>
        /// <param name="FilePath"></param>
        /// <returns>
        /// list of AudioMediaAsset consisting of input Assets but changed clip list
        /// </returns>
        public static  List<AudioMediaAsset> ExportAssets (List<AudioMediaAsset> AssetList, string path )
        {
            
            // new clip which is required to replace existing list of clips in each AudioMediaAsset
            AudioClip ExportAudioClip;
            
            // binary writer for writing to export wave file
            BinaryWriter bw = new BinaryWriter(File.Create(path));
            BinaryReader br;

            // copy header of first audio clip file to export aubio file
            br = new BinaryReader(File.OpenRead(AssetList[0].mClips[0].Path));

            for (int i = 0; i < 44; i++)
            {
                bw.Write(br.ReadByte());
            }
            bw.BaseStream.Position = 44;

            // byte count variable for counting total bytes copied to export file
            long ByteLengthCount = 0;


            for (int AssetCount = 0; AssetCount < AssetList.Count; AssetCount++)
            {

                if (AssetList[AssetCount].mClips.Count != 0)
                {
                    for (int i = 0 ; i < AssetList[AssetCount].mClips.Count ; i++)
                    {
                        br = new BinaryReader(File.OpenRead(AssetList[AssetCount].mClips[i].Path));
                        br.BaseStream.Position = AssetList[AssetCount].mClips[i].BeginByte + 44;
                        for (long l = AssetList[AssetCount].mClips[i].BeginByte; l < AssetList[AssetCount].mClips[i].EndByte; l++)
                        {
                            bw.Write(br.ReadByte());
                            ByteLengthCount++;

                        }
                        
                    }
                }

            }
            
                AssetList[0].UpdateLengthHeader(ByteLengthCount + 44 , bw);
                bw.Close();
                br.Close();
                br = null;    

                double OutputAssetClipStartTime = 0;
            for ( int ICount = 0 ; ICount  < AssetList.Count ; ICount++  )
            {

                ExportAudioClip = new AudioClip( path, OutputAssetClipStartTime  , OutputAssetClipStartTime +  AssetList[ ICount ].LengthInMilliseconds);
                OutputAssetClipStartTime = OutputAssetClipStartTime + AssetList[ICount].LengthInMilliseconds;
                List<AudioClip> NewList = new List<AudioClip>();
                NewList.Add(ExportAudioClip);
                AssetList[ICount].mClips = NewList;
            }
            bw = null;
            return AssetList ;
        }


        private void UpdateLengthHeader(long Length, BinaryWriter bw)
        {
            long  AudioLengthBytes = Length - 44;
            
            // update length field (4 to 7 )in header
            for (int i = 0; i < 4; i++)
            {
                bw.BaseStream.Position = i + 4;
                bw.Write(Convert.ToByte(Audio.CalculationFunctions.ConvertFromDecimal(Length)[i]));

            }
            long TempLength = AudioLengthBytes;
            for (int i = 0; i < 4; i++)
            {
                bw.BaseStream.Position = i + 40;
                bw.Write( Convert.ToByte( Audio.CalculationFunctions.ConvertFromDecimal(TempLength)[i]));
                
            }

        }
    }
}