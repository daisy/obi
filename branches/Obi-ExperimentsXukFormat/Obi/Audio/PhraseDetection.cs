using System;
using System.Collections.Generic;
using System.Collections;
using System.IO ;
using System.Windows.Forms;

using urakawa.media.data;
using  urakawa.media.data.audio ;


namespace Obi.Audio
{
    public class PhraseDetection
    {
        public static readonly double DEFAULT_GAP = 300.0;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = 50.0;  // default leading silence
        public static readonly double DEFAULT_THRESHOLD = 280.0;

        private static  AudioMediaData m_AudioAsset;
        private static  readonly int m_FrequencyDivisor = 2000; // frequency inin hz to observe.
        
        

        // NewDetection

        // Detecs the maximum size of noise level in a silent sample file
        public static long GetSilenceAmplitude (ManagedAudioMedia RefAsset)
        {
            m_AudioAsset = RefAsset.AudioMediaData;
            BinaryReader brRef = new BinaryReader(RefAsset.AudioMediaData.OpenPcmInputStream ());

            // creates counter of size equal to clip size
            long lSize = RefAsset.AudioMediaData.PCMFormat.Data.ConvertTimeToBytes(RefAsset.AudioMediaData.AudioDuration.AsLocalUnits);

            // Block size of audio chunck which is least count of detection
            int Block;

            // determine the Block  size
            if (RefAsset.AudioMediaData.PCMFormat.Data.SampleRate> 22500)
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

            long Iterations = Convert.ToInt64(RefAsset.AudioMediaData.AudioDuration.AsTimeSpan.TotalMilliseconds/ BlockTime);
            long SampleCount = Convert.ToInt64((int)RefAsset.AudioMediaData.PCMFormat.Data.SampleRate/ (1000 / BlockTime));

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


        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, double GapLength, double before)
        {
            long lGapLength = CalculationFunctions.ConvertTimeToByte(GapLength, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            long lBefore = CalculationFunctions.ConvertTimeToByte(before, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            return ApplyPhraseDetection(audio, threshold, lGapLength, lBefore);
        }


        private static  List<ManagedAudioMedia> ApplyPhraseDetection(ManagedAudioMedia ManagedAsset, long threshold, long GapLength, long before)
        {
            m_AudioAsset = ManagedAsset.AudioMediaData;
            GapLength = CalculationFunctions.AdaptToFrame(GapLength, m_AudioAsset.PCMFormat.Data.BlockAlign);
            before = CalculationFunctions.AdaptToFrame(before , m_AudioAsset.PCMFormat.Data.BlockAlign);

            int Block = 0;

            // determine the Block  size
            if ( m_AudioAsset.PCMFormat.Data.SampleRate> 22500)
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
            double BeforePhraseInMS = CalculationFunctions.ConvertByteToTime(before , (int) m_AudioAsset.PCMFormat.Data.SampleRate, m_AudioAsset.PCMFormat.Data.BlockAlign);

            lCountSilGap = Convert.ToInt64(CalculationFunctions.ConvertByteToTime(GapLength , (int) m_AudioAsset.PCMFormat.Data.SampleRate, m_AudioAsset.PCMFormat.Data.BlockAlign) / BlockTime);

            long Iterations = Convert.ToInt64(m_AudioAsset.AudioDuration.AsTimeSpan.TotalMilliseconds/ BlockTime);
            long SampleCount = Convert.ToInt64(m_AudioAsset.PCMFormat.Data.SampleRate/ (1000 / BlockTime));
            double errorCompensatingCoefficient  = GetErrorCompensatingConstant ( SampleCount );
            long SpeechBlockCount = 0;

            long lCurrentSum = 0;
            long lSumPrev = 0;

            BinaryReader br = new BinaryReader( m_AudioAsset.OpenPcmInputStream());

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

                        // changing following time calculations to reduce concatination of rounding off errors 
                        //alPhrases.Add(((j - Counter) * BlockTime) - BeforePhraseInMS);
                        double phraseMarkTime = CalculationFunctions.ConvertByteToTime (Convert.ToInt64(errorCompensatingCoefficient  * (j - Counter)) * SampleCount * m_AudioAsset.PCMFormat.Data.BlockAlign,
                            (int) m_AudioAsset.PCMFormat.Data.SampleRate,
                            (int) m_AudioAsset.PCMFormat.Data.BlockAlign);
                        alPhrases.Add ( phraseMarkTime - BeforePhraseInMS );

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
                    ManagedAudioMedia splitAsset = ManagedAsset.Split(new urakawa.media.timing.Time(Convert.ToInt64(alPhrases[i]) * 1000));
                                        //ManagedAsset.MediaData.getMediaDataManager().addMediaData(splitAsset.MediaData);
                    ReturnList.Insert(0, splitAsset);
                    //MessageBox.Show(Convert.ToDouble(alPhrases[i]).ToString());
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
                        long PeakCount  = Convert.ToInt64 (  m_AudioAsset.PCMFormat.Data.SampleRate/ m_FrequencyDivisor) ;

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
                                    if ( m_AudioAsset.PCMFormat.Data.BitDepth == 16 )                    
            {
                    SampleValue1 = SampleValue1 + (br.ReadByte() * 256);

                    if (SampleValue1 > 32768)
                        SampleValue1 = SampleValue1 - 65536;

        }
        if ( m_AudioAsset.PCMFormat.Data.NumberOfChannels == 2)
        {
            SampleValue2 = br.ReadByte();
            if ( m_AudioAsset.PCMFormat.Data.BitDepth== 16)
            {
                SampleValue2 = SampleValue2 + (br.ReadByte() * 256);

                if (SampleValue2 > 32768)
                    SampleValue2 = SampleValue2 - 65536;

            }
            SampleValue1 = (SampleValue1 + SampleValue2) / 2;
        }


            return SampleValue1 ;

        }

        /// <summary>
        /// computes multiplying factor to compensate errors due to rounding off in average peak calculation functions
        /// </summary>
        /// <param name="SampleCount"></param>
        /// <returns></returns>
        private static double GetErrorCompensatingConstant ( long SampleCount )
            {
            // number of samples from which peak is selected
            long PeakCount = Convert.ToInt64 ( m_AudioAsset.PCMFormat.Data.SampleRate/ m_FrequencyDivisor );

            // number of blocks iterated
            long AverageCount = Convert.ToInt64 ( SampleCount / PeakCount );
            
            double roundedOffSampleCount = AverageCount * PeakCount;
            
            double errorCoeff = roundedOffSampleCount  / SampleCount;

            if (errorCoeff < 0.90 || errorCoeff  > 1.1)
                {
                errorCoeff  = 1.0;
                }
            return errorCoeff;
            }


    }
}
