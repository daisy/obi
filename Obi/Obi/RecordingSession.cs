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
        private AudioMediaAsset mAsset;                     // current phrase asset (?)
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
        public event StartingSectionHandler StartingSection;

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
                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                StartingPhrase(this, new PhraseEventArgs(mAsset, mSessionOffset, 0.0));
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
                        mAssetList[mSessionOffset + i] = mSessionAsset.Split(mPhraseMarks[i]);  
                    }
                    // The first asset is what remains of the session asset
                    mAssetList[mSessionOffset] = mSessionAsset;
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
                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                StartingPhrase (this, new PhraseEventArgs(mAsset, mSessionOffset + mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Finish recording of the current phrase.
        /// </summary>
        private void FinishedPhrase()
        {
            mPhraseMarks.Add(mSessionAsset.LengthInMilliseconds);
            mAssetList.Add(mAsset);
            int last = mPhraseMarks.Count - 1;
            double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
            PhraseEventArgs e = new PhraseEventArgs(mAsset, mSessionOffset + last, length);
            FinishingPhrase(this, e);
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase in a new section.
        /// </summary>
        /// <remarks>Because splitting sections is not implemented yet, this is only possible if appending to a section,
        /// or we may skip the end of the current section.</remarks>
        public void NextSection()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                mPhraseMarks.Add(mRecorder.CurrentTime);

                double mAssetLengthInMs  ;
                if ( mPhraseMarks.Count > 1 )
                mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
            else
                    mAssetLengthInMs = mPhraseMarks[0];

                Events.Audio.Recorder.PhraseEventArgs e =
                    new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);
                mAssetList.Add(mAsset);
                FinishingPhrase(this, e);
                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                e = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count , 0.0);
                StartingSection(this, e);
                StartingPhrase(this, e);
                mSectionMarks.Add(mPhraseMarks.Count - 1);
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
                mPhraseMarks.Add(mRecorder.CurrentTime);
                int last = mPhraseMarks.Count - 1;
                double length = mPhraseMarks[last] - (last == 0 ? 0.0 : mPhraseMarks[last - 1]);
                PhraseEventArgs e = new PhraseEventArgs(mAsset, last, length);
                mAssetList.Add(mAsset);
                FinishingPhrase(this, e);
                FinishingPage(this, e);
                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                StartingPhrase(this, new PhraseEventArgs(mAsset, mPhraseMarks.Count, 0.0));
            }
        }

        private void mRecordingUpdateTimer_tick(object sender, EventArgs e)
        {
            ContinuingPhrase(this, new PhraseEventArgs(mAsset, mSessionOffset + mPhraseMarks.Count, mRecorder.TimeOfAsset));
        }
    }
}
