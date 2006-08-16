using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace Obi.Assets
{
    public class AudioMediaAsset : MediaAsset
    {
        // member variables
        //ArrayList is to be finally changed to internal
        // changed to mClips--see below (JQ)
        //public ArrayList m_alClipList = new ArrayList();
        private int m_Channels;
        private int m_BitDepth;
        private int m_SamplingRate;
        private int m_FrameSize;
        private long m_lAudioLengthInBytes = 0;
        private double m_dAudioLengthInTime = 0;

        public int SampleRate
        {
            get { return m_SamplingRate; }
        }

        public int Channels
        {
            get { return m_Channels; }
        }

        public int BitDepth
        {
            get { return m_BitDepth; }
        }

        internal int FrameSize
        {
            get { return m_FrameSize; }
        }

        public double LengthInMilliseconds
        {
            get { return m_dAudioLengthInTime; }
        }

        public string LengthInSeconds
        {
            get { return (Math.Round(m_dAudioLengthInTime / 1000.0)).ToString() + "s"; }
        }

        public long AudioLengthInBytes
        {
            get { return m_lAudioLengthInBytes; }
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
                m_Channels = channels;
                m_BitDepth = bitDepth;
                m_SamplingRate = sampleRate;
                m_FrameSize = (m_BitDepth / 8) * m_Channels;
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
                m_Channels = ob_AudioClip.Channels;
                m_BitDepth = ob_AudioClip.BitDepth;
                m_SamplingRate = ob_AudioClip.SampleRate;
                m_FrameSize = ob_AudioClip.FrameSize;
                //m_alClipList = clips;
                mClips = clips;
                m_dAudioLengthInTime = 0;

                // compute total time ofasset from clip lengths
                for (int i = 0; i < clips.Count; i++)
                {

                    ob_AudioClip = clips[i] as AudioClip;

                    if (m_Channels == ob_AudioClip.Channels && m_BitDepth == ob_AudioClip.BitDepth && m_SamplingRate == ob_AudioClip.SampleRate)
                        m_dAudioLengthInTime = m_dAudioLengthInTime + ob_AudioClip.LengthInTime;
                    else
                        throw new Exception("Clip format do not match Asset format");
                }

                m_lAudioLengthInBytes = Audio.CalculationFunctions.ConvertTimeToByte(m_dAudioLengthInTime, m_SamplingRate, m_FrameSize);
                mSizeInBytes = m_lAudioLengthInBytes;
            }
            else
                throw new Exception("No AudioMediaAsset can be created as clip list is empty");
        }

        /// <summary>
        /// Make a copy of the asset, sharing the same format and data.
        /// </summary>
        /// <returns>The new, identical asset.</returns>
        public override Assets.MediaAsset Copy()
        {
            AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset(this.Channels, this.BitDepth, this.SampleRate);
            ob_AudioMediaAsset.mMediaType = mMediaType;
            //if (this.Name != null) 

            // The copy is not managed yet. It keeps the same name as the original and has no assigned manager.
            // JQ 20060816
            ob_AudioMediaAsset.Name = mName;
            ob_AudioMediaAsset.mAssManager = null;
            //ob_AudioMediaAsset.mAssManager = mAssManager;

            // Add clips to clip list of new asset
            //for (int i = 0; i < this.m_alClipList.Count; i++)
            //    ob_AudioMediaAsset.m_alClipList.Add(this.m_alClipList[i]);
            foreach (AudioClip clip in mClips) ob_AudioMediaAsset.mClips.Add(clip);

            ob_AudioMediaAsset.m_FrameSize = m_FrameSize;
            ob_AudioMediaAsset.m_dAudioLengthInTime = m_dAudioLengthInTime;
            ob_AudioMediaAsset.m_lAudioLengthInBytes = m_lAudioLengthInBytes;
            ob_AudioMediaAsset.mSizeInBytes = mSizeInBytes;
            return ob_AudioMediaAsset;
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
            double dBeginTime = Audio.CalculationFunctions.ConvertByteToTime(beginPosition, m_SamplingRate, m_FrameSize);
            double dEndTime = Audio.CalculationFunctions.ConvertByteToTime(endPosition, m_SamplingRate, m_FrameSize);
            return GetChunk(dBeginTime, dEndTime);
        }

        public Assets.AudioMediaAsset GetChunk(double beginTime, double endTime)
        {
            // checks if the input parameters are in bounds of asset and in  order
            if (beginTime >= 0 && beginTime < endTime && endTime <= m_dAudioLengthInTime)
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
            double dPosition = Audio.CalculationFunctions.ConvertByteToTime(position, m_SamplingRate, m_FrameSize);
            InsertAsset(chunk, dPosition);
        }

        public void InsertAsset(Assets.AudioMediaAsset chunk, double time)
        {
            // checks if audio formats of original asset and chunk asset are of same formats
            if (CompareAudioAssetFormat(this, chunk) == true && time <= m_dAudioLengthInTime && time >= 0)
            {
                // creates the temporary blank asset
                AudioMediaAsset ob1 = new AudioMediaAsset(this.Channels, this.BitDepth, this.SampleRate);

                // if Chunk is to be inserted somewhere in between of original asset
                if (time > -0 && time < m_dAudioLengthInTime)
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
                m_dAudioLengthInTime = ob1.LengthInMilliseconds;
                m_lAudioLengthInBytes = ob1.AudioLengthInBytes;
                mSizeInBytes = ob1.SizeInBytes;

                // if Chunk is to be appended to original asset
                if (time == m_dAudioLengthInTime)
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
            double dBeginTime = Audio.CalculationFunctions.ConvertByteToTime(beginPosition, m_SamplingRate, m_FrameSize);
            double dEndTime = Audio.CalculationFunctions.ConvertByteToTime(endPosition, m_SamplingRate, m_FrameSize);
            return DeleteChunk(dBeginTime, dEndTime);
        }

        public Assets.AudioMediaAsset DeleteChunk(double beginTime, double endTime)
        {
            // checks if beginTime and EndTime is within bounds of asset and are in order
            if (beginTime >= 0 && beginTime < endTime && endTime <= m_dAudioLengthInTime)
            {
                // create new asset from original asset from part which has to be deleted and keep for returning back
                AudioMediaAsset ob_NewAsset = GetChunk(beginTime, endTime) as AudioMediaAsset;

                // create two temp assets for holding clips in front of BeginTime and a asset to hold  Clips after endTime
                AudioMediaAsset ob_FromtAsset = new AudioMediaAsset(m_Channels, m_BitDepth, m_SamplingRate);
                AudioMediaAsset ob_RearAsset = new AudioMediaAsset(m_Channels, m_BitDepth, m_SamplingRate);

                // if deletion part lies somewhere in between body of asset
                if (beginTime != 0 && endTime != m_dAudioLengthInTime)
                {
                    // Copy respective  clips  to Front and Rear Assets and merge them
                    ob_FromtAsset = GetChunk(0, beginTime) as AudioMediaAsset;

                    ob_RearAsset = GetChunk(endTime, m_dAudioLengthInTime) as AudioMediaAsset;

                    ob_FromtAsset.MergeWith(ob_RearAsset);
                }
                // if deletion is from in between to end of asset
                else if (beginTime != 0)
                {
                    // copies only front part of asset
                    ob_FromtAsset = GetChunk(0, beginTime) as AudioMediaAsset;
                }
                // if Deletion is in front including start
                else if (endTime != m_dAudioLengthInTime)
                {
                    // copies end part of asset to front asset
                    ob_FromtAsset = GetChunk(endTime, m_dAudioLengthInTime) as AudioMediaAsset;
                }

                // replaces clip list of original asset with clip list of front asset
                // m_alClipList = ob_FromtAsset.m_alClipList;
                mClips = ob_FromtAsset.Clips;
                m_dAudioLengthInTime = ob_FromtAsset.LengthInMilliseconds;
                m_lAudioLengthInBytes = ob_FromtAsset.AudioLengthInBytes;
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
            if (Time <= m_dAudioLengthInTime && Time >= 0)
            {

                //AudioClip ob_AudioClip = m_alClipList[0] as AudioClip;
                AudioClip ob_AudioClip = mClips[0];

                // Add the length in time of each clip and add untill time parameter is close enough upto atmost one clip distance
                double TimeSum = 0;
                int Count = 0;
                while (TimeSum <= Time && TimeSum < m_dAudioLengthInTime)
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


        public override void MergeWith(Assets.MediaAsset next)
        {

            AudioMediaAsset ob_AudioMediaAsset = next as AudioMediaAsset;
            // checks if the formats of both clips is same
            if (CompareAudioAssetFormat(this, ob_AudioMediaAsset) == true)
            {
                // append clips of next asset to clip list of original asset
                //for (int i = 0; i < ob_AudioMediaAsset.m_alClipList.Count; i++)
                //{
                //    m_alClipList.Add(ob_AudioMediaAsset.m_alClipList[i]);
                //}
                foreach (AudioClip clip in ob_AudioMediaAsset.Clips) mClips.Add(clip);
                m_dAudioLengthInTime = m_dAudioLengthInTime + ob_AudioMediaAsset.LengthInMilliseconds;
                m_lAudioLengthInBytes = m_lAudioLengthInBytes + ob_AudioMediaAsset.AudioLengthInBytes;
                mSizeInBytes = mSizeInBytes + ob_AudioMediaAsset.SizeInBytes;
                next = null;
            }
            else
                throw new Exception("Cannot merge assets: incompatible format");
        }

        public Assets.AudioMediaAsset Split(long position)
        {
            double dTime = Audio.CalculationFunctions.ConvertByteToTime(position, m_SamplingRate, m_FrameSize);
            return Split(dTime);
        }

        public Assets.AudioMediaAsset Split(double time)
        {
            // checks if time parameter is in bounds of asset
            if (time >= 0 && time <= m_dAudioLengthInTime)
            {
                // create new asset for clips after time specified in parameter

                AudioMediaAsset ob_AudioMediaAsset = GetChunk(time, m_dAudioLengthInTime) as AudioMediaAsset;

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

                m_dAudioLengthInTime = m_dAudioLengthInTime - ob_AudioMediaAsset.LengthInMilliseconds;
                m_lAudioLengthInBytes = m_lAudioLengthInBytes - ob_AudioMediaAsset.AudioLengthInBytes;
                mSizeInBytes = m_lAudioLengthInBytes;

                return ob_AudioMediaAsset;
            }
            else
                throw new Exception("Cannot split: parameter value out of bound of asset");
        }

        public long GetSilenceAmplitude(Assets.AudioMediaAsset silenceRef)
        {
            AudioMediaAsset ob_AudioMediaSilenceRef = silenceRef as AudioMediaAsset;
            //AudioClip Ref = ob_AudioMediaSilenceRef.m_alClipList[0] as AudioClip;
            AudioClip Ref = ob_AudioMediaSilenceRef.Clips[0];
            return Ref.GetClipSilenceAmplitude();

        }

        public ArrayList ApplyPhraseDetection(long threshold, long length, long before)
        {
            double dLength = Audio.CalculationFunctions.ConvertByteToTime(length, m_SamplingRate, m_FrameSize);
            double dBefore = Audio.CalculationFunctions.ConvertByteToTime(before, m_SamplingRate, m_FrameSize);

            return ApplyPhraseDetection(threshold, dLength, dBefore);
        }

        public ArrayList ApplyPhraseDetection(long threshold, double length, double before)
        {
            //			 convert input parameters from time to byte
            long lLength = Audio.CalculationFunctions.ConvertTimeToByte(length, m_SamplingRate, m_FrameSize);
            long lBefore = Audio.CalculationFunctions.ConvertTimeToByte(before, m_SamplingRate, m_FrameSize);


            AudioClip ob_Clip;
            // AssetList is list of assets returned by phrase detector
            ArrayList alAssetList = new ArrayList();
            // clipList is clip list for each return asset
            ArrayList alClipList;
            AudioMediaAsset ob_Asset = new AudioMediaAsset(m_Channels, m_BitDepth, m_SamplingRate);


            // apply phrase detection on each clip in clip list of this asset
            //for (int i = 0; i < m_alClipList.Count; i++)
            //{
            //    ob_Clip = m_alClipList[i] as AudioClip;
            for (int i = 0; i < mClips.Count; ++i)
            {
                ob_Clip = mClips[i];
                alClipList = ob_Clip.DetectPhrases(threshold, lLength, lBefore);
                //MessageBox.Show (alClipList.Count.ToString () + "Clip Count") ;
                if (Convert.ToBoolean(alClipList[0]) == false)
                {

                    //MessageBox.Show ("bool is False") ;
                    ob_Asset.AddClip(alClipList[1] as AudioClip);

                    // if (i == m_alClipList.Count - 1 && ob_Asset.m_alClipList != null)
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                        //MessageBox.Show ("last Asset added") ;
                    }
                }
                else
                {
                    //MessageBox.Show ("bool is true") ;
                    if (ob_Clip.BeginTime + 3000 < (alClipList[1] as AudioClip).BeginTime)
                    {
                        ob_Asset.AddClip(ob_Clip.CopyClipPart(0, (alClipList[1] as AudioClip).BeginTime - ob_Clip.BeginTime));
                        if (i == 0)
                            alAssetList.Add(ob_Asset);
                    }
                    //ob_Asset.AddClip (alClipList [1] as AudioClip) ;
                    if (i != 0)
                        alAssetList.Add(ob_Asset);
                    //MessageBox.Show ("Asset Added before loop") ;

                    for (int j = 1; j < alClipList.Count - 1; j++)
                    {
                        ob_Asset = new AudioMediaAsset(m_Channels, m_BitDepth, m_SamplingRate);
                        ob_Asset.AddClip(alClipList[j] as AudioClip);
                        alAssetList.Add(ob_Asset);
                        //MessageBox.Show ("Asset added inside loop") ;
                    }
                    ob_Asset = new AudioMediaAsset(m_Channels, m_BitDepth, m_SamplingRate);

                    if (alClipList.Count > 2)
                        ob_Asset.AddClip(alClipList[alClipList.Count - 1] as AudioClip);

                    // if (i == m_alClipList.Count - 1 && ob_Asset.m_alClipList != null)
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                        //MessageBox.Show ("last Asset added") ;
                    }
                } // bool if ends

            }


            return alAssetList;

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
            if (m_Channels == Clip.Channels && m_BitDepth == Clip.BitDepth && m_SamplingRate == Clip.SampleRate)
            {
                // m_alClipList.Add(Clip);
                mClips.Add(Clip);
                m_dAudioLengthInTime = m_dAudioLengthInTime + Clip.LengthInTime;
                m_lAudioLengthInBytes = Audio.CalculationFunctions.ConvertTimeToByte(m_dAudioLengthInTime, m_SamplingRate, m_FrameSize);
                mSizeInBytes = m_lAudioLengthInBytes;
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
    }// end of class
}