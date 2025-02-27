using System;
using System.Collections.Generic;
using System.Collections;
using System.IO ;
using System.Windows.Forms;

using urakawa.media.data;
using  urakawa.media.data.audio ;
using urakawa.media.timing;
using AudioLib;

namespace Obi.Audio
{
    public class PhraseDetection
    {
        public static readonly double DEFAULT_GAP = AudioLib.PhraseDetection.DEFAULT_GAP;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = AudioLib.PhraseDetection.DEFAULT_LEADING_SILENCE;  // default leading silence
        public static readonly double DEFAULT_THRESHOLD = AudioLib.PhraseDetection.DEFAULT_THRESHOLD;


        public static bool CancelOperation
        {
            get { return AudioLib.PhraseDetection.CancelOperation; }
            set { AudioLib.PhraseDetection.CancelOperation = value; }
        }

        private static bool m_RetainSilenceInBeginningOfPhrase = true;
        /// <summary>
        /// Retains silence in beginning of phrase as the first phrase created after phrase detection
        /// </summary>
        public static     bool RetainSilenceInBeginningOfPhrase
        {
            get { return m_RetainSilenceInBeginningOfPhrase; }
            set { m_RetainSilenceInBeginningOfPhrase = value; }
        }

        // NewDetection
        /// <summary>
        /// Detects the maximum size of noise level in a silent sample file
        /// </summary>
        /// <param name="RefAsset"></param>
        /// <returns></returns>
        public static long GetSilenceAmplitude(ManagedAudioMedia RefAsset)
        {
            
            AudioLibPCMFormat audioPCMFormat = new AudioLibPCMFormat (RefAsset.AudioMediaData.PCMFormat.Data.NumberOfChannels, RefAsset.AudioMediaData.PCMFormat.Data.SampleRate, RefAsset.AudioMediaData.PCMFormat.Data.BitDepth ) ;
            return AudioLib.PhraseDetection.GetSilenceAmplitude(RefAsset.AudioMediaData.OpenPcmInputStream(), audioPCMFormat);
        }

        /// <summary>
        /// < Detects phrases, accepts timing parameters in milliseconds
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="threshold"></param>
        /// <param name="GapLength"></param>
        /// <param name="before"></param>
        /// <returns></returns>
        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, double GapLength, double before, bool DeleteSilenceFromEndOfSection = false)
        {

            AudioLibPCMFormat audioPCMFormat = new AudioLibPCMFormat(audio.AudioMediaData.PCMFormat.Data.NumberOfChannels, audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BitDepth);
            List<long> timingList;

            timingList = AudioLib.PhraseDetection.Apply(audio.AudioMediaData.OpenPcmInputStream(),
                audioPCMFormat,
                threshold,
                (long)GapLength * AudioLibPCMFormat.TIME_UNIT,
                (long)before * AudioLibPCMFormat.TIME_UNIT);

            List<ManagedAudioMedia> detectedAudioMediaList = new List<ManagedAudioMedia>();
            //Console.WriteLine("returned list count " + timingList.Count);
            if (timingList == null)
            {
                detectedAudioMediaList.Add(audio);
            }
            else
            {
                for (int i = timingList.Count - 1; i >= 0; i--)
                {
                    //Console.WriteLine("splitting " + timingList[i] + " asset time " + audio.Duration.AsLocalUnits);
                    ManagedAudioMedia splitAsset = audio.Split(new Time(Convert.ToInt64(timingList[i])));
                    //ManagedAsset.MediaData.getMediaDataManager().addMediaData(splitAsset.MediaData);
                    detectedAudioMediaList.Insert(0, splitAsset);
                    //MessageBox.Show(Convert.ToDouble(alPhrases[i]).ToString());
                }
                if (RetainSilenceInBeginningOfPhrase && audio.Duration.AsMilliseconds > 200) detectedAudioMediaList.Insert(0, audio);
            }

            return detectedAudioMediaList;
        }

        public static double RemoveSilenceFromEnd(ManagedAudioMedia audio, long threshold, double GapLength, double before)
        {
            AudioLibPCMFormat audioPCMFormat = new AudioLibPCMFormat(audio.AudioMediaData.PCMFormat.Data.NumberOfChannels, audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BitDepth);
            long Silencetiming = AudioLib.PhraseDetection.RemoveSilenceFromEnd(audio.AudioMediaData.OpenPcmInputStream(),
                  audioPCMFormat,
                  threshold,
                  (long)GapLength * AudioLibPCMFormat.TIME_UNIT,
                  (long)before * AudioLibPCMFormat.TIME_UNIT);
            if (Silencetiming != 0)
            {
                double timingListInMS = 0;

                Time temptime = new Time(Convert.ToInt64(Silencetiming));
                timingListInMS = temptime.AsMilliseconds;
                return timingListInMS;
            }
            return 0;
        }
        /*
         
         public static readonly double DEFAULT_GAP = 300.0;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = 50.0;  // default leading silence
        public static readonly double DEFAULT_THRESHOLD = 280.0;

        private static  AudioMediaData m_AudioAsset;
        private static  readonly int m_FrequencyDivisor = 2000; // frequency inin hz to observe.
        
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

            long Iterations = Convert.ToInt64(RefAsset.AudioMediaData.AudioDuration.AsMilliseconds/ BlockTime);
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
            long lGapLength = ObiCalculationFunctions.ConvertTimeToByte(GapLength, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            long lBefore = ObiCalculationFunctions.ConvertTimeToByte(before, (int)audio.AudioMediaData.PCMFormat.Data.SampleRate, audio.AudioMediaData.PCMFormat.Data.BlockAlign);
            return ApplyPhraseDetection(audio, threshold, lGapLength, lBefore);
        }


        private static  List<ManagedAudioMedia> ApplyPhraseDetection(ManagedAudioMedia ManagedAsset, long threshold, long GapLength, long before)
        {
            m_AudioAsset = ManagedAsset.AudioMediaData;
            GapLength = ObiCalculationFunctions.AdaptToFrame(GapLength, m_AudioAsset.PCMFormat.Data.BlockAlign);
            before = ObiCalculationFunctions.AdaptToFrame(before , m_AudioAsset.PCMFormat.Data.BlockAlign);

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
            list    <long>  alPhrases = new list    <long>   ();
            long lCheck = 0;

            // flags to indicate phrases and silence
            bool boolPhraseDetected = false;
            bool boolBeginPhraseDetected = false;


            double BlockTime = 25; // milliseconds
            double BeforePhraseInMS = ObiCalculationFunctions.ConvertByteToTime(before , (int) m_AudioAsset.PCMFormat.Data.SampleRate, m_AudioAsset.PCMFormat.Data.BlockAlign);

            lCountSilGap = Convert.ToInt64(ObiCalculationFunctions.ConvertByteToTime(GapLength , (int) m_AudioAsset.PCMFormat.Data.SampleRate, m_AudioAsset.PCMFormat.Data.BlockAlign) / BlockTime);

            long Iterations = Convert.ToInt64(m_AudioAsset.AudioDuration.AsMilliseconds/ BlockTime);
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
                        double phraseMarkTime = ObiCalculationFunctions.ConvertByteToTime (Convert.ToInt64(errorCompensatingCoefficient  * (j - Counter)) * SampleCount * m_AudioAsset.PCMFormat.Data.BlockAlign,
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
                    ManagedAudioMedia splitAsset = ManagedAsset.Split(new Time(Convert.ToInt64(alPhrases[i]) * Time.TIME_UNIT));
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
*/

         //Diagnosis code for detecting silence gaps 
        public static List<double> GetErrorSilencePositionInAsset(ManagedAudioMedia RefAsset)
        {
            
            AudioLibPCMFormat audioPCMFormat = new AudioLibPCMFormat(RefAsset.AudioMediaData.PCMFormat.Data.NumberOfChannels, RefAsset.AudioMediaData.PCMFormat.Data.SampleRate, RefAsset.AudioMediaData.PCMFormat.Data.BitDepth);
            
            List<long> errorPositionsBytesList = null;
            Stream stream = null;
            try
            {
                stream  = RefAsset.AudioMediaData.OpenPcmInputStream();
                errorPositionsBytesList = GetErrorSilencePosition(audioPCMFormat, stream);
            }
            finally
            {
                if(stream != null)  stream.Close();
            }
            List<double> errorPositionTimesList = new List<double>();
            foreach (long positionBytes in errorPositionsBytesList)
            {
                double positionTime = Convert.ToDouble(positionBytes / audioPCMFormat.ByteRate) * 1000 ;
                errorPositionTimesList.Add(positionTime);
                Console.WriteLine("Position time: " + positionTime);
            }
            return errorPositionTimesList;
        }

        public static List<long> GetErrorSilencePosition(AudioLibPCMFormat audioPCMFormat, Stream stream)
        {
            List<long> errorPositionsList = new List<long>();
            // check in chunks of 100 ms
            double chunkSizeMS = 100 ;
            int chunkSizeBytes = Convert.ToInt32( chunkSizeMS * audioPCMFormat.ByteRate / 1000 );
            chunkSizeBytes =(int) audioPCMFormat.AdjustByteToBlockAlignFrameSize (chunkSizeBytes ) ;
            Console.WriteLine ("Chunk size bytes: " + chunkSizeBytes ) ;

            if (stream.Length < chunkSizeBytes)
            {
                Console.WriteLine("The stream is too short to check silence errors");
                return errorPositionsList;
            }
            Console.WriteLine("Stream length: " + stream.Length);

            int iterations = Convert.ToInt32(stream.Length / chunkSizeBytes);
            byte[] chunkArray = new byte[chunkSizeBytes];
            int referenceVal = 0;
            int count = 0;

                        for (int i = 0; i < iterations; i++)
            {
                int startPos = Convert.ToInt32(i * chunkSizeBytes);
                //Console.WriteLine("Stream offset: " + startPos );
                stream.Read(chunkArray , 
                    0, chunkSizeBytes);

                long threshold =  AudioLib.PhraseDetection.GetSilenceAmplitude(
                    new MemoryStream(chunkArray), 
                    audioPCMFormat);
                
                if (threshold <= referenceVal && count >= 7 )
                {
                    long position = chunkSizeBytes * (i-7) ;
                    if (position < 0) position = 0;
                    Console.WriteLine("Silence error found at: " + position);
                    errorPositionsList.Add(position);
                }
                count++;
                if (threshold > referenceVal) 
                {
                    if (errorPositionsList.Count > 0
                        && (count > 9 || errorPositionsList[errorPositionsList.Count - 1] <= chunkSizeBytes ))
                    {
                        Console.WriteLine("Removing last value from list");
                        errorPositionsList.RemoveAt (errorPositionsList.Count - 1 );
                    }
                    count = 0;
                    }
                
            }


            return errorPositionsList;
        }


    }
}
