using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AudioLib;
using Obi.Audio;
using Obi.Events.Audio.Recorder;
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

        public event StartingPhraseHandler StartingPhrase;      // start recording a new phrase
        public event ContinuingPhraseHandler ContinuingPhrase;  // a new phrase is being recorded (time update)
        public event FinishingPhraseHandler FinishingPhrase;    // finishing a phrase
        public event FinishingPageHandler FinishingPage;        // finishing a page


        /// <summary>
        /// Create a recording session for a project starting from a given node.
        /// </summary>
        /// <param name="project">The project in which we are recording.</param>
        /// <param name="recorder">The audio recorder from the project.</param>
        public RecordingSession(ObiPresentation presentation, AudioRecorder recorder)
        {
            mPresentation = presentation;
            mRecorder = recorder;
            mRecorder.RecordingDirectory =
                presentation.DataProviderManager.DataFileDirectoryFullPath;
            if (!Directory.Exists(mRecorder.RecordingDirectory)) Directory.CreateDirectory(mRecorder.RecordingDirectory);
            mSessionOffset = 0;
            mPhraseMarks = null;
            mSectionMarks = null;
            mAudioList = new List<ManagedAudioMedia>();
            mRecordingUpdateTimer = new Timer();
            mRecordingUpdateTimer.Tick += new System.EventHandler(mRecordingUpdateTimer_tick);
            mRecordingUpdateTimer.Interval = 1000;
        }


        /// <summary>
        /// The audio recorder used by the recording session.
        /// </summary>
        public AudioRecorder AudioRecorder { get { return mRecorder; } }

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
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition)/
                    Time.TIME_UNIT;

            // check for illegal time input
                if (mPhraseMarks != null && mPhraseMarks.Count > 0 && mPhraseMarks[mPhraseMarks.Count - 1] >= timeOfAssetMilliseconds)
                return;

                PhraseEventArgs e = FinishedPhrase();
                if (StartingPhrase != null)
                    StartingPhrase(this, new PhraseEventArgs(mSessionMedia, mPhraseMarks.Count, 0.0));
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
                    (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition) /
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
                AudioMediaData asset =
                    (AudioMediaData)mPresentation.MediaDataFactory.Create<WavAudioMediaData>();
                mSessionMedia = (ManagedAudioMedia)mPresentation.MediaFactory.CreateManagedAudioMedia();
                //mSessionMedia.setMediaData(asset);
                mSessionMedia.MediaData = asset;
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
            if (mRecorder.CurrentState == AudioLib.AudioRecorder.State.Monitoring || mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording)
            {
                bool wasRecording = mRecorder.CurrentState == AudioLib.AudioRecorder.State.Recording;
                if (wasRecording   &&   mPhraseMarks.Count > 0 ) FinishedPhrase();
                mRecorder.StopRecording();
                if (wasRecording)
                {
                    // Split the session asset into smaller assets starting from the end
                    // (to keep the split times correct) until the second one
                    for (int i = mPhraseMarks.Count - 2; i >= 0; --i)
                    {
                    if (mPhraseMarks[i] < mSessionMedia.Duration.AsTimeSpan.TotalMilliseconds)
                        {
                            ManagedAudioMedia split = mSessionMedia.Split(new Time(Convert.ToInt64(mPhraseMarks[i] * Time.TIME_UNIT)));
                        mAudioList.Insert ( mSessionOffset, split );
                        }
                    else
                        MessageBox.Show ( Localizer.Message ( "RecordingSession_SplitError" ) , Localizer.Message ("Caption_Warning"));
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
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition) /
                Time.TIME_UNIT;

            mPhraseMarks.Add(timeOfAssetMilliseconds);
            int last = mPhraseMarks.Count - 1;
            double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            length = length - (length % 100);
            PhraseEventArgs e = new PhraseEventArgs(mSessionMedia, mSessionOffset + last, length);
            if (FinishingPhrase != null) FinishingPhrase(this, e);
            return e;
        }

        // Send recording update
        private void mRecordingUpdateTimer_tick(object sender, EventArgs e)
        {
            //mRecorder.TimeOfAsset
            double timeOfAssetMilliseconds =
                (double)mRecorder.RecordingPCMFormat.ConvertBytesToTime(mRecorder.CurrentDurationBytePosition) /
                Time.TIME_UNIT;

            double time = timeOfAssetMilliseconds - (mPhraseMarks.Count > 0 ? mPhraseMarks[mPhraseMarks.Count - 1] : 0.0);
            time = time - (time % 100);
            if (ContinuingPhrase != null)
                ContinuingPhrase(this, new PhraseEventArgs(mSessionMedia, mSessionOffset + mPhraseMarks.Count, time));
        }
    }
}
