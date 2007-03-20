using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Obi.Audio;
using Obi.Assets;
using Obi.Events.Audio.Recorder;

namespace Obi
{
    /// <summary>
    /// A recording session during which new assets are recorded. Phrases, sections and pages are created.
    /// This replaces the Record dialog.
    /// </summary>
    public class RecordingSession
    {
        private Project mProject;                           // project in which we are recording
        private AudioRecorder mRecorder;                    // the actual recorder

        private int mChannels;                              // number of channels of audio to record
        private int mSampleRate;                            // sample rate of audio to record
        private int mBitDepth;                              // bit depth of audio to record

        private AudioMediaAsset mSessionAsset;              // session asset (?)
        private int mSessionOffset;                         // offset from end of last part of the session
        private List<double> mPhraseMarks ;                 // list of phrase marks
        private List<int> mSectionMarks;                    // list of section marks (necessary?)
        private List<AudioMediaAsset> mAssetList;           // list of assets created
        private Timer mRecordingUpdateTimer = new Timer();  // timer to send regular "recording" messages

        // Record session events
        public event StartingPhraseHandler StartingPhrase;
        public event ContinuingPhraseHandler ContinuingPhrase;
        public event FinishingPhraseHandler FinishingPhrase;
        public event FinishingPageHandler FinishingPage;

        /// <summary>
        /// Create a recording session for a project starting from a given node.
        /// </summary>
        /// <param name="project">The project in which we are recording.</param>
        /// <param name="recorder">The audio recorder from the project.</param>
        /// <param name="channels">Number of channels of audio to record.</param>
        /// <param name="sampleRate">Sample rate of audio to record.</param>
        /// <param name="bitDepth">Bit depth of audio to record.</param>
        public RecordingSession(Project project, AudioRecorder recorder, int channels, int sampleRate, int bitDepth)
        {
            mProject = project;
            mRecorder = recorder;            
            mChannels = channels;
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            mSessionOffset = 0;
            mPhraseMarks = new List<double>();
            mSectionMarks = new List<int>();
            mAssetList = new List<AudioMediaAsset>();
            // set up event handlers
            Audio.AudioRecorder.Instance.StateChanged +=
                new StateChangedHandler(delegate(object sender, StateChangedEventArgs e) { });
            Audio.AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new UpdateVuMeterHandler(delegate(object sender, UpdateVuMeterEventArgs e) { });
            mRecordingUpdateTimer.Tick += new System.EventHandler(mRecordingUpdateTimer_tick);
            mRecordingUpdateTimer.Interval = 1000;
        }

        /// <summary>
        /// The audio recorder used by the recording session.
        /// </summary>
        public Audio.AudioRecorder AudioRecorder
        {
            get { return mRecorder; }
        }

        /// <summary>
        /// The list of recorded asset, in the order in which they were recorded during the session.
        /// </summary>
        public List<AudioMediaAsset> RecordedAssets
        {
            get { return mAssetList; }
        }

        /// <summary>
        /// Listen. This may happen at the start of the seession, or after pause was pressed when we were recording.
        /// Create a new asset to "record" in (it gets discarded anyway.)
        /// </summary>
        public void Listen()
        {
            if (mRecorder.State == AudioRecorderState.Idle)
            {
                AudioMediaAsset asset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mRecorder.StartListening(asset);
            }
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        public void Record()
        {
            if (mRecorder.State == AudioRecorderState.Idle)
            {
                mSessionOffset = mAssetList.Count;
                mPhraseMarks = new List<double>();
                mSectionMarks = new List<int>();
                mSessionAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mRecorder.StartRecording(mSessionAsset);
                StartingPhrase(this, new PhraseEventArgs(mSessionAsset, mSessionOffset, 0.0));
                mRecordingUpdateTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Stop recording altogether.
        /// </summary>
        public void Stop()
        {
            mRecordingUpdateTimer.Enabled = false;
            StoppedRecording();
        }

        /// <summary>
        /// Convenience function to stop recording.
        /// </summary>
        /// <returns>True if we were indeed recording.</returns>
        /// <remarks>May throw an exception.</remarks>
        private void StoppedRecording()
        {
            if (mRecorder.State != AudioRecorderState.Idle)
            {
                bool wasRecording = mRecorder.State == AudioRecorderState.Recording;
                mRecorder.StopRecording();
                if (wasRecording)
                {
                    FinishedPhrase();
                    // Split the session asset into smaller assets starting from the end
                    // (to keep the split times correct) until the second one
                    for (int i = mPhraseMarks.Count - 2; i >= 0; --i)
                    {
                        mAssetList.Insert(mSessionOffset, mSessionAsset.Manager.SplitAudioMediaAsset  ( mSessionAsset ,  mPhraseMarks[i]));
                    }
                    // The first asset is what remains of the session asset
                    mAssetList.Insert(mSessionOffset, mSessionAsset);
                }
            }
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                FinishedPhrase();
                StartingPhrase (this, new PhraseEventArgs(mSessionAsset, mSessionOffset + mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// The phrase that was just finished receives a page number as well (auto-generated.)
        /// </summary>
        public void MarkPage()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                FinishingPage(this, FinishedPhrase());
                StartingPhrase(this, new PhraseEventArgs(mSessionAsset, mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Finish recording of the current phrase.
        /// </summary>
        private PhraseEventArgs FinishedPhrase()
        {
            mPhraseMarks.Add(mRecorder.TimeOfAsset);
            int last = mPhraseMarks.Count - 1;
            double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            PhraseEventArgs e = new PhraseEventArgs(mSessionAsset, mSessionOffset + last, length);
            FinishingPhrase(this, e);
            return e;
        }

        private void mRecordingUpdateTimer_tick(object sender, EventArgs e)
        {
            double time = mRecorder.TimeOfAsset - (mPhraseMarks.Count > 0 ? mPhraseMarks[mPhraseMarks.Count - 1] : 0.0); 
            ContinuingPhrase(this, new PhraseEventArgs(mSessionAsset, mSessionOffset + mPhraseMarks.Count, time ));
        }
    }
}
