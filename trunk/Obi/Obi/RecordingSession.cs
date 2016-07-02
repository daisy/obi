using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AudioLib;
using Obi.Audio;
using Obi.Events.Audio.Recorder;
using urakawa.data;
using urakawa.media.data;
using urakawa.media.data.audio ;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;


namespace Obi
{
    /// <summary>
    /// A recording session during which new assets are recorded. Phrases, sections and pages are created.
    /// This replaces the Record dialog.
    /// </summary>
    public class RecordingSession
    {
        private ObiPresentation mPresentation;                     // presentation to record in
        private AudioRecorder mRecorder;                        // recorder for the session

        private ManagedAudioMedia mSessionMedia;                // session asset (?)
        private int mSessionOffset;                             // offset from end of last part of the session
        private List<double> mPhraseMarks ;                     // list of phrase marks
        private List<int> mSectionMarks;                        // list of section marks (necessary?)
        private List<ManagedAudioMedia> mAudioList;             // list of assets created
        private Timer mRecordingUpdateTimer;                    // timer to send regular "recording" messages
        private Settings m_Settings;
        
        private List<int> m_PhraseIndexesToDelete = new List<int>();
        private List<double[]> mDeletedTime = new List<double[]>();

        private double[] arrOfLocations;
        

        public event StartingPhraseHandler StartingPhrase;      // start recording a new phrase
        public event ContinuingPhraseHandler ContinuingPhrase;  // a new phrase is being recorded (time update)
        public event FinishingPhraseHandler FinishingPhrase;    // finishing a phrase
        public event FinishingPageHandler FinishingPage;        // finishing a page

        private void OnAudioRecordingFinished(object sender, AudioRecorder.AudioRecordingFinishEventArgs e)
        {
            mRecorder.AudioRecordingFinished -= OnAudioRecordingFinished;
            bool deleteAfterInsert = true;
                if (deleteAfterInsert)
                {
                    FileDataProvider dataProv = (FileDataProvider)mSessionMedia.Presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                    dataProv.InitByMovingExistingFile(e.RecordedFilePath);
                    mSessionMedia.AudioMediaData.AppendPcmData(dataProv);
                }
                else
                {
                    // TODO: progress ! (time consuming file copy)
                    mSessionMedia.AudioMediaData.AppendPcmData_RiffHeader(e.RecordedFilePath);
                }

                if (deleteAfterInsert && File.Exists(e.RecordedFilePath)) //check exist just in case file adopted by DataProviderManager
                {
                    File.Delete(e.RecordedFilePath);
                }
                m_PhDetectorBytesRecorded = 0;
        }


        /// <summary>
        /// Create a recording session for a project starting from a given node.
        /// </summary>
        /// <param name="project">The project in which we are recording.</param>
        /// <param name="recorder">The audio recorder from the project.</param>
        public RecordingSession(ObiPresentation presentation, AudioRecorder recorder, Settings settings)
        {
            mPresentation = presentation;
            mRecorder = recorder;
            if (!string.IsNullOrEmpty(settings.Audio_LocalRecordingDirectory))
            {
                mRecorder.RecordingDirectory = settings.Audio_LocalRecordingDirectory;
            }
            else
            {
                mRecorder.RecordingDirectory =
                    presentation.DataProviderManager.DataFileDirectoryFullPath;
            }
            if (!Directory.Exists(mRecorder.RecordingDirectory)) Directory.CreateDirectory(mRecorder.RecordingDirectory);
            mSessionOffset = 0;
            mPhraseMarks = null;
            mSectionMarks = null;
            mAudioList = new List<ManagedAudioMedia>();
            mRecordingUpdateTimer = new Timer();
            mRecordingUpdateTimer.Tick += new System.EventHandler(mRecordingUpdateTimer_tick);
            mRecordingUpdateTimer.Interval = 1000;
            m_Settings = settings;
            m_PhraseIndexesToDelete = new List<int>();
            mRecorder.PcmDataBufferAvailable += new AudioLib.AudioRecorder.PcmDataBufferAvailableHandler(DetectPhrasesOnTheFly);
        }


        public List<double[]> DeletedItemList { get { return mDeletedTime; } }
        public int PhraseMarksCount { get { return mPhraseMarks != null ? mPhraseMarks.Count : 0; } }

        /// <summary>
        /// The audio recorder used by the recording session.
        /// </summary>
        public AudioRecorder AudioRecorder { get { return mRecorder; } }

        public List<int> PhraseIndexesToDelete { get { return m_PhraseIndexesToDelete; } }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// The phrase that was just finished receives a page number as well (auto-generated.)
        /// </summary>
        public void MarkPage()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording)
            {
                //recorder.TimeOfAsset
                double timeOfAssetMilliseconds =
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition))/
                    Time.TIME_UNIT;

            // check for illegal time input
                if (mPhraseMarks != null && mPhraseMarks.Count > 0 && mPhraseMarks[mPhraseMarks.Count - 1] >= timeOfAssetMilliseconds)
                return;

                PhraseEventArgs e = FinishedPhrase();
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mPhraseMarks.Count, 0.0, e.TimeFromBeginning, true));
                if (FinishingPage != null) FinishingPage(this, e);
            }
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording)
            {
                //mRecorder.TimeOfAsset
                double timeOfAssetMilliseconds =
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                    Time.TIME_UNIT;

                // check for illegal time input
            if (mPhraseMarks != null && mPhraseMarks.Count > 0    &&    mPhraseMarks[mPhraseMarks.Count - 1] >=
                timeOfAssetMilliseconds
                )
                return;

                FinishedPhrase();
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Start recording. Stop monitoring before starting recording.
        /// </summary>
        public void Record()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
            {
                mSessionOffset = mAudioList.Count;
                mPhraseMarks = new List<double>();
                mSectionMarks = new List<int>();
                mDeletedTime.Clear();
                m_PhraseIndexesToDelete.Clear();
                AudioMediaData asset =
                    (AudioMediaData)mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
                mSessionMedia = (ManagedAudioMedia)mPresentation.MediaFactory.CreateManagedAudioMedia();
                //mSessionMedia.setMediaData(asset);
                mSessionMedia.MediaData = asset;
                mRecorder.AudioRecordingFinished += OnAudioRecordingFinished;
                mRecorder.StartRecording(asset.PCMFormat.Data);
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset, 0.0));
                mRecordingUpdateTimer.Enabled = true;
            }
        }

        /// <summary>
        /// The list of recorded asset, in the order in which they were recorded during the session.
        /// </summary>
        public List<ManagedAudioMedia> RecordedAudio { get { return mAudioList; } }

        /// <summary>
        /// Start monitoring the audio input.
        /// This may happen at the beginning of the session,
        /// or when recording is paused.
        /// Create a new asset to "record" in (it gets discarded anyway.)
        /// </summary>
        public void StartMonitoring()
        {
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Stopped)
            {
                AudioMediaData asset =
                    (AudioMediaData)mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
                mRecorder.StartMonitoring(asset.PCMFormat.Data);
            }
        }

        /// <summary>
        /// Stop recording or monitoring.
        /// </summary>
        public void Stop()
        {
            bool wasRecording = mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording;
            ApplyPhraseDetectionOnTheFly(null); //@onTheFly: before stopping last chunk of memory stream is passed into phrase detection
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring
                || wasRecording)
            {
                if (wasRecording   &&   mPhraseMarks.Count > 0 ) FinishedPhrase();
                mRecorder.StopRecording();
                if (wasRecording)
                {
                    for (int i = m_PhraseMarksOnTheFly.Count - 2; i >= 0; --i)
                    {
                        if (i != 0 && i < m_PhraseMarksOnTheFly.Count 
                            && (m_PhraseMarksOnTheFly[i] - m_PhraseMarksOnTheFly[i - 1]) <= 250)
                        {
                            m_PhraseMarksOnTheFly.Remove(m_PhraseMarksOnTheFly[i]);
                            i++;
                        }
                        else if (i == 0 && i < m_PhraseMarksOnTheFly.Count
                            && m_PhraseMarksOnTheFly[i] <= 250)
                        {
                            m_PhraseMarksOnTheFly.Remove(m_PhraseMarksOnTheFly[i]);
                            i++;
                        }
                    }
                   
                    for (int i = mPhraseMarks.Count - 2; i >= 0; --i)
                    {
                        if (mPhraseMarks[i] < mSessionMedia.Duration.AsMilliseconds && mSessionMedia.Duration.AsMilliseconds > 200)
                        {
                            ManagedAudioMedia split = mSessionMedia.Split(new Time(Convert.ToInt64(mPhraseMarks[i] * Time.TIME_UNIT)));
                            mAudioList.Insert(mSessionOffset, split);
                        }
                        else
                        {
                            MessageBox.Show(Localizer.Message("RecordingSession_SplitError"), Localizer.Message("Caption_Warning"));
                        }
                    }
                    // The first asset is what remains of the session asset
                    mAudioList.Insert(mSessionOffset, mSessionMedia);
                }
                mRecordingUpdateTimer.Enabled = false;
            }
        }


        // Finish recording of the current phrase.
        private PhraseEventArgs FinishedPhrase()
        {
            //mRecorder.TimeOfAsset
            double timeOfAssetMilliseconds =
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                Time.TIME_UNIT;

            mPhraseMarks.Add(timeOfAssetMilliseconds);
            int last = mPhraseMarks.Count - 1;
            double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            length = length - (length % 100);
            PhraseEventArgs e = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length, timeOfAssetMilliseconds);
            if (FinishingPhrase != null) FinishingPhrase(this, e);
            
            return e;
        }

        // Send recording update
        private void mRecordingUpdateTimer_tick(object sender, EventArgs e)
        {
            //mRecorder.TimeOfAsset
            double timeOfAssetMilliseconds =
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (mRecorder.CurrentDurationBytePosition)) /
                Time.TIME_UNIT;

            double time = timeOfAssetMilliseconds - (mPhraseMarks.Count > 0 ? mPhraseMarks[mPhraseMarks.Count - 1] : 0.0);
            time = time - (time % 100);
            if (ContinuingPhrase != null)
                ContinuingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, time, timeOfAssetMilliseconds));
        }

        private byte[] m_MemStreamArray;
        private int m_PhDetectionMemStreamPosition;
        private long m_PhDetectorBytesRecorded;
        private List<double> m_PhraseMarksOnTheFly = new List<double>();
        /// <summary>
        /// List of timings for phrases marked with on the fly phrase detection  
        /// </summary>
        public List<double> PhraseMarksOnTheFly { get { return m_PhraseMarksOnTheFly; } }

        private void DetectPhrasesOnTheFly(object sender, AudioLib.AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            ApplyPhraseDetectionOnTheFly(e);
        }

        public int UpdatePhraseTimeList(double time, bool isPage)
        {
            
            if (mPhraseMarks == null)
                return -1;

            int phraseIndex = 0;
            if (mPhraseMarks.Count == 0)
            {
                mPhraseMarks.Add(time);
                phraseIndex = 0;
            }
            else if (mPhraseMarks.Count == 1)
            {
                if (time < mPhraseMarks[0])
                {
                    mPhraseMarks.Insert(0, time);
                    phraseIndex = 0;
                }
                else
                {
                    mPhraseMarks.Add(time);
                    phraseIndex = 1;
                }
            }
            else if (time < mPhraseMarks[0])
            {
                mPhraseMarks.Insert(0, time);
                phraseIndex = 0;
            }
            else if (time > mPhraseMarks[mPhraseMarks.Count - 1])
            {
                mPhraseMarks.Add(time);
                phraseIndex = mPhraseMarks.Count - 1;
            }
            else
            {
                for (int i = 0; i < mPhraseMarks.Count - 1; i++)
                {
                    if (time > mPhraseMarks[i] && time < mPhraseMarks[i + 1])
                    {
                        mPhraseMarks.Insert(i + 1, time);
                        phraseIndex = i + 1;
                        break;
                    }
                }
            }

            int last = mPhraseMarks.Count - 1;
            double length = 0;
            if(last > 0)
               length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            length = length - (length % 100);
            PhraseEventArgs eArg = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length, time);

            
            if (FinishingPhrase != null) FinishingPhrase(this, eArg);
            if (StartingPhrase != null)
                StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, 0.0));

            if (!isPage)
            {
            }
            else
            {
                eArg = new PhraseEventArgs(mSessionMedia, mSessionOffset + phraseIndex, length, time);
               if (FinishingPage != null) FinishingPage(this, eArg);
            }
            //Console.WriteLine("Last index:" + last + "    phrase index:" + phraseIndex);
            return phraseIndex;
        }
        
        public void UpdateDeletedTimeList(double startTime, double endTime)
        {
            arrOfLocations = new double[2];
            arrOfLocations[0] = startTime;
            arrOfLocations[1] = endTime;
            if (mDeletedTime.Count == 0 || mDeletedTime[mDeletedTime.Count - 1][0] < arrOfLocations[0])
            {
                mDeletedTime.Add(arrOfLocations);
            }
            else if (mDeletedTime[0][0] > arrOfLocations[0])
            {
                mDeletedTime.Insert(0, arrOfLocations);
            }
            else
            {
                for (int i = mDeletedTime.Count - 1; i > 0; i--)
                {
                    if (mDeletedTime[i - 1][0] < arrOfLocations[0] && mDeletedTime[i][0] > arrOfLocations[0])
                    {
                        mDeletedTime.Insert(i, arrOfLocations);
                        break;
                    }
                }
            }
            UpdatePhraseTimeList(startTime, false);
            int phraseIndex = UpdatePhraseTimeList(endTime, false);
            if (phraseIndex >= 0 && !m_PhraseIndexesToDelete.Contains(phraseIndex))
            {
                m_PhraseIndexesToDelete.Add(phraseIndex);
                m_PhraseIndexesToDelete.Sort();
            }
        }

        private void ApplyPhraseDetectionOnTheFly(AudioLib.AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            //m_Settings.Audio_EnableLivePhraseDetection = true;
            if (!m_Settings.Audio_EnableLivePhraseDetection ) return;//letsverify with on the fly phrase detection first
            // todo : associate this function to recorder VuMeter events
            int overlapLength = 0;
            int msLentth = Convert.ToInt32(mRecorder.RecordingPCMFormat.SampleRate * mRecorder.RecordingPCMFormat.BlockAlign * 40); // 40 seconds
            if (mRecorder.RecordingPCMFormat != null )
            {
                //@overlap: overlapLength = Convert.ToInt32(0.250 * (mRecorder.RecordingPCMFormat.BlockAlign * mRecorder.RecordingPCMFormat.SampleRate));
                //@overlap: if (e != null) overlapLength = (Math.Abs(overlapLength / e.PcmDataBufferLength) * e.PcmDataBufferLength);
                //msLentth = e != null? Convert.ToInt32( e.PcmDataBufferLength * 480): 
                    //m_MemStreamArray != null? m_MemStreamArray.Length: 0;
                
            }

            if ((m_MemStreamArray == null || e == null || m_PhDetectionMemStreamPosition > (msLentth - e.PcmDataBufferLength)) && mRecorder.RecordingPCMFormat != null)
            {//1

                if (m_MemStreamArray != null)
                {//2
                    //m_PhDetectionMemoryStream.ReadByte();
                    
                    long threshold = (long)m_Settings.Audio_DefaultThreshold;
                    long GapLength = (long)m_Settings.Audio_DefaultGap;
                    long before = (long)m_Settings.Audio_DefaultLeadingSilence;
                    //Console.WriteLine("on the fly ph detection parameters " + threshold + " : " + GapLength);
                    AudioLib.AudioLibPCMFormat audioPCMFormat = new AudioLib.AudioLibPCMFormat(mRecorder.RecordingPCMFormat.NumberOfChannels, mRecorder.RecordingPCMFormat.SampleRate, mRecorder.RecordingPCMFormat.BitDepth);
                    List<long> timingList = AudioLib.PhraseDetection.Apply(new System.IO.MemoryStream(m_MemStreamArray),
                        audioPCMFormat,
                        threshold,
                        (long)GapLength * AudioLib.AudioLibPCMFormat.TIME_UNIT,
                        (long)before * AudioLib.AudioLibPCMFormat.TIME_UNIT);
                    if (timingList != null)
                    {//3
                        //Console.WriteLine("timingList " + timingList.Count);
                        double overlapTime = mRecorder.RecordingPCMFormat.ConvertBytesToTime(overlapLength);
                        //System.Media.SystemSounds.Asterisk.Play();
                        foreach (double d in timingList)
                        {//4
                            //Console.WriteLine("Overlap time and list time " + (overlapTime / AudioLibPCMFormat.TIME_UNIT) + " : " + (d / AudioLibPCMFormat.TIME_UNIT));
                            if (d >= overlapTime )
                            {//5
                                double phraseTime = d - overlapTime;
                                double timeInSession = (mRecorder.RecordingPCMFormat.ConvertBytesToTime(m_PhDetectorBytesRecorded - msLentth) + d) / AudioLib.AudioLibPCMFormat.TIME_UNIT;
                                //Console.WriteLine("phrase time: " + phraseTime + " : " + timeInSession);
                                //@event: if (PhraseCreatedEvent != null) PhraseCreatedEvent(this, new Audio.PhraseDetectedEventArgs(timeInSession));
                                if (phraseTime != 0)
                                {
                                    m_PhraseMarksOnTheFly.Add(timeInSession);
                                }
                                //@event: int last = mPhraseMarks.Count - 1;
                                //@event: double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
                                //@event: length = length - (length % 100);
                                //@event: PhraseEventArgs eArg = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length, timeInSession);

                                //@event: if (FinishingPhrase != null) FinishingPhrase(this, eArg);
                                //@event: if (StartingPhrase != null)
                                    //@event: StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, 0.0));

                            }//-5
                        }//-4
                    }//-3
                    else
                    {//3
                        //Console.WriteLine("timing list is null ");
                    }//-3
                    
                }//-2
                if (e == null) return;
                byte[] overlapData = null;
                if (m_MemStreamArray != null)
                {//2
                    //@overlap: overlapData = new byte[overlapLength];
                    //@overlap: Array.Copy(m_MemStreamArray, m_MemStreamArray.Length - overlapLength - 1, overlapData, 0, overlapData.Length);
                    m_MemStreamArray = new byte[msLentth + overlapLength];
                    //@overlap: overlapData.CopyTo(m_MemStreamArray, 0);
                }//-2
                else
                {//2
                    overlapLength = 0;
                    m_MemStreamArray = new byte[msLentth];
                }//-2
                //Console.WriteLine("newMemStream length  " + m_MemStreamArray.Length);
                m_PhDetectionMemStreamPosition = overlapLength;
                m_PhDetectorBytesRecorded += msLentth;

                
                    //e.PcmDataBuffer.CopyTo(m_MemStreamArray, m_PhDetectionMemStreamPosition);
                Array.Copy(e.PcmDataBuffer, 0, m_MemStreamArray, m_PhDetectionMemStreamPosition, e.PcmDataBufferLength);
                    m_PhDetectionMemStreamPosition += e.PcmDataBufferLength;
                
                //m_PhDetectionMemoryStream.Write(e.PcmDataBuffer, (int)m_PhDetectionMemStreamPosition, e.b);
                //m_PhDetectionMemStreamPosition = m_PhDetectionMemoryStream.Position;
                //Console.WriteLine("first writing of recorder buffer " + m_PhDetectionMemStreamPosition);
            }//-1
            else if (m_MemStreamArray != null && e.PcmDataBuffer != null)
            {//1
                int leftOverLength = Convert.ToInt32(msLentth - m_PhDetectionMemStreamPosition);
                if (leftOverLength > e.PcmDataBufferLength) leftOverLength = e.PcmDataBufferLength ;
                //Console.WriteLine("length:position:leftOver " + m_MemStreamArray.Length + " : " + m_PhDetectionMemStreamPosition + " : " + leftOverLength + " : " + e.PcmDataBuffer.Length);
                //m_PhDetectionMemoryStream.Write(e.PcmDataBuffer,0, leftOverLength);
                //m_PhDetectionMemStreamPosition = m_PhDetectionMemoryStream.Position;
                //m_MemStreamArray = new byte[msLentth];
                //m_PhDetectionMemoryStream.ToArray().CopyTo(m_MemStreamArray, 0); ;
                if (m_MemStreamArray.Length - m_PhDetectionMemStreamPosition > e.PcmDataBufferLength)
                {
                    //e.PcmDataBuffer.CopyTo(m_MemStreamArray, m_PhDetectionMemStreamPosition);
                    Array.Copy (e.PcmDataBuffer, 0, m_MemStreamArray, m_PhDetectionMemStreamPosition, e.PcmDataBufferLength);
                    m_PhDetectionMemStreamPosition += e.PcmDataBufferLength;
                }
                else
                {
                    m_PhDetectionMemStreamPosition = m_MemStreamArray.Length - 1;
                }
                    
                
                //Console.WriteLine("writing recorder buffer " + m_PhDetectionMemStreamPosition);

            }//-1

        }

        
    }
}
