using System;
using System.Collections.Generic;
using System.Collections;
using System.IO ;

using urakawa.media.data;
using  urakawa.media.data.audio ;


namespace Obi.Audio
{
    public class PhraseDetection
    {
        public static readonly double DEFAULT_GAP = 500.0;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = 100.0;  // default leading silence

        private static  AudioMediaData m_AudioAsset;

/*
        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, long length, long before)
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
 
            //return new List<ManagedAudioMedia>();
        }
        */


        
        /*
        /// <summary>
        /// Get the maximum amplitude of silence in a given "silent" asset.
        /// </summary>
        /// <returns>The amplitude.</returns>
        public static long GetSilenceAmplitude(ManagedAudioMedia audio)
        {
            long max = 0;
            foreach (AudioClip clip in mClips)
            {
                long amplitude = clip.GetClipSilenceAmplitude();
                if (amplitude > max) max = amplitude;
            }
            max = max + 10;
            return max;
        }
        */
        // NewDetection

        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, double GapLength, double before)
        {
            long lGapLength = CalculationFunctions.ConvertTimeToByte(GapLength, (int)audio.getMediaData().getPCMFormat().getSampleRate(), audio.getMediaData().getPCMFormat().getBlockAlign());
            long lBefore = CalculationFunctions.ConvertTimeToByte(before, (int)audio.getMediaData().getPCMFormat().getSampleRate(), audio.getMediaData().getPCMFormat().getBlockAlign());
            return ApplyPhraseDetection(audio, threshold, lGapLength, lBefore);
        }



        // Detecs the maximum size of noise level in a silent sample file
        public static long GetSilenceAmplitude (ManagedAudioMedia RefAsset)
        {

            BinaryReader brRef = new BinaryReader(RefAsset.getMediaData ().getAudioData ()  );

            // creates counter of size equal to clip size
            long lSize = RefAsset.getMediaData ().getPCMLength();

            // Block size of audio chunck which is least count of detection
            int Block;

            // determine the Block  size
            if (RefAsset.getMediaData ().getPCMFormat ().getSampleRate ()  > 22500)
            {
                Block = 192;
            }
            else
            {
                Block = 96;
            }

            //set reading position after the header
            
            long lLargest = 0;
            long lBlockSum;

            // adjust the  lSize to avoid reading beyond file length
            lSize = ((lSize / Block) * Block) - 4;
            
            // Experiment starts here
            double BlockTime = 25;

            long Iterations = Convert.ToInt64(RefAsset.getMediaData ().getAudioDuration ().getTimeDeltaAsMillisecondFloat () / BlockTime);
            long SampleCount = Convert.ToInt64((int)RefAsset.getMediaData ().getPCMFormat ().getSampleRate ()  / (1000 / BlockTime));

            long lCurrentSum = 0;
            long lSumPrev = 0;


            for (long j = 0; j < Iterations - 1; j++)
            {
                //  BlockSum is function to retrieve average amplitude in  Block
                //lCurrentSum  = GetAverageSampleValue(brRef, SampleCount)  ;
                lCurrentSum =  GetAvragePeakValue(brRef, SampleCount);
                lBlockSum = Convert.ToInt64((lCurrentSum + lSumPrev) / 2);
                lSumPrev = lCurrentSum;

                if (lLargest < lBlockSum)
                {
                    lLargest = lBlockSum;
                }
            }
            long SilVal = Convert.ToInt64(lLargest);

            // experiment ends here

            brRef.Close();

            return SilVal;

        }




        public static  List<ManagedAudioMedia> ApplyPhraseDetection(ManagedAudioMedia ManagedAsset, long threshold, long GapLength, long before)
        {
            m_AudioAsset = ManagedAsset.getMediaData ()  ;
            GapLength = CalculationFunctions.AdaptToFrame(GapLength, m_AudioAsset.getPCMFormat().getBlockAlign());
            before = CalculationFunctions.AdaptToFrame(before , m_AudioAsset.getPCMFormat().getBlockAlign());

            int Block = 0;

            // determine the Block  size
            if ( m_AudioAsset.getPCMFormat ().getSampleRate ()  > 22500)
            {
                Block = 192;
            }
            else
            {
                Block = 96;
            }

            
            // count chunck of silence which trigger phrase detection
            long lCountSilGap = ( 2 * GapLength ) / Block; // multiplied by two because j counter is incremented by 2
            long lSum = 0;
            ArrayList alPhrases = new ArrayList();
            long lCheck = 0;

            // flags to indicate phrases and silence
            bool boolPhraseDetected = false;
            bool boolBeginPhraseDetected = false;


            double BlockTime = 25; // milliseconds
            double BeforePhraseInMS = CalculationFunctions.ConvertByteToTime(before , (int) m_AudioAsset.getPCMFormat ().getSampleRate () , m_AudioAsset.getPCMFormat ().getBlockAlign ()  );

            lCountSilGap = Convert.ToInt64(CalculationFunctions.ConvertByteToTime(GapLength , (int) m_AudioAsset.getPCMFormat ().getSampleRate ()  , m_AudioAsset.getPCMFormat ().getBlockAlign ()  ) / BlockTime);

            long Iterations = Convert.ToInt64(m_AudioAsset.getAudioDuration ().getTimeDeltaAsMillisecondFloat ()  / BlockTime);
            long SampleCount = Convert.ToInt64(m_AudioAsset.getPCMFormat ().getSampleRate ()  / (1000 / BlockTime));
            long SpeechBlockCount = 0;

            long lCurrentSum = 0;
            long lSumPrev = 0;

            BinaryReader br = new BinaryReader( m_AudioAsset.getAudioData ()  );

            bool PhraseNominated = false;
            long SpeechChunkSize = 5;
            long Counter = 0;
            for (long j = 0; j < Iterations - 1; j++)
            {
                // decodes audio chunck inside block
                //lCurrentSum = GetAverageSampleValue(br, SampleCount);
                lCurrentSum = GetAvragePeakValue(br, SampleCount);
                lSum = (lCurrentSum + lSumPrev) / 2;
                lSumPrev = lCurrentSum;

                // conditional triggering of phrase detection
                if (lSum < threshold )
                {
                    lCheck++;

                    SpeechBlockCount = 0;
                }
                else
                {
                    if (j < lCountSilGap && boolBeginPhraseDetected == false)
                    {
                        boolBeginPhraseDetected = true;
                        alPhrases.Add(Convert.ToInt64(0));
                        boolPhraseDetected = true;
                        lCheck = 0;
                    }


                    // checks the length of silence
                    if (lCheck > lCountSilGap)
                    {
                        PhraseNominated = true;
                        lCheck = 0;
                    }
                    if (PhraseNominated)
                        SpeechBlockCount++;

                    if (SpeechBlockCount >= SpeechChunkSize && Counter >= 4)
                    {
                        //sets the detection flag
                        boolPhraseDetected = true;

                        alPhrases.Add(((j - Counter) * BlockTime) - BeforePhraseInMS);


                        SpeechBlockCount = 0;
                        Counter = 0;
                        PhraseNominated = false;
                    }
                    lCheck = 0;
                }
                if (PhraseNominated)
                    Counter++;
                // end outer For
            }
            br.Close();

            List<ManagedAudioMedia> ReturnList = new List<ManagedAudioMedia>();

            if (boolPhraseDetected == false)
            {
                ReturnList.Add( ManagedAsset );
            }
            else
            {
                for (int i = alPhrases.Count-1   ; i >= 0 ; i-- )
                {
                    ManagedAudioMedia splitAsset  = ManagedAsset.split(new urakawa.media.timing.Time( Convert.ToDouble ( alPhrases[i] )) ) ;
                    ManagedAsset.getMediaData().getMediaDataManager().addMediaData(splitAsset.getMediaData());
                    ReturnList.Insert(0, splitAsset);
                }

            }



            return ReturnList ;
        }


        private static int GetAverageSampleValue(BinaryReader br, long SampleLength)
        {
            long AvgSampleValue = 0;

            for (long i = 0; i < SampleLength; i++)
            {
                AvgSampleValue = AvgSampleValue + GetSampleValue(br);
            }
            AvgSampleValue = AvgSampleValue / SampleLength;

            return Convert.ToInt32(AvgSampleValue);
        }


        private static  int GetAvragePeakValue(BinaryReader br, long SampleCount)
        {
                    // average value to return
            long AverageValue = 0;

            // number of samples from which peak is selected
                        long PeakCount  = Convert.ToInt64 (  m_AudioAsset.getPCMFormat ().getSampleRate () / 3000 ) ;

            // number of blocks iterated
            long AverageCount = Convert.ToInt64 ( SampleCount / PeakCount ) ;

                for (long i = 0; i < AverageCount; i++)
                {
                    AverageValue = AverageValue + GetPeak
                        (br, PeakCount);
                }
            
            AverageValue = AverageValue / AverageCount;

            return Convert.ToInt32 (  AverageValue  ) ;

        }
    
        
        private static  int GetPeak(BinaryReader br , long  UBound )
        {
            int Peak = 0;
            
            int CurrentValue = 0 ;
            for (long i = 0; i < UBound; i++)
            {
                CurrentValue = GetSampleValue (br)  ;
                if (CurrentValue > Peak)
                    Peak = CurrentValue;
            }
            return Peak ;
        }


        private static   int GetSampleValue(BinaryReader br)
        {
            int SampleValue1 =  0 ;
int SampleValue2 = 0 ;
                            
                
                                SampleValue1 =  br.ReadByte(); 
                                    if ( m_AudioAsset.getPCMFormat ().getBitDepth () == 16 )                    
            {
                    SampleValue1 = SampleValue1 + (br.ReadByte() * 256);

                    if (SampleValue1 > 32768)
                        SampleValue1 = SampleValue1 - 65536;

        }
        if ( m_AudioAsset.getPCMFormat ().getNumberOfChannels ()== 2)
        {
            SampleValue2 = br.ReadByte();
            if ( m_AudioAsset.getPCMFormat ().getBitDepth () == 16)
            {
                SampleValue2 = SampleValue2 + (br.ReadByte() * 256);

                if (SampleValue2 > 32768)
                    SampleValue2 = SampleValue2 - 65536;

            }
            SampleValue1 = (SampleValue1 + SampleValue2) / 2;
        }


            return SampleValue1 ;

        }


    }
}
