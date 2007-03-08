using Obi.Audio;
using Obi.Assets;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace Obi
{
    /// <summary>
    /// A recording session during which new assets are recorded. Phrases, sections and pages are created.
    /// This replaces the Record dialog.
    /// </summary>
    public class RecordingSession
    {
        private Project mProject;         // project in which we are recording
        private AudioRecorder mRecorder;  // the actual recorder
        
        private AudioMediaAsset mRecordingAsset ;
        private int mChannels;            // number of channels of audio to record
        private int mSampleRate;          // sample rate of audio to record
        private int mBitDepth;            // bit depth of audio to record
        //private Thread UpdateDisplayThread ;

        private AudioMediaAsset mAsset;
        private List<double> mPhraseMarks ;
        private List<int> mSectionMarks;
        private List<int> mPageMarks;
        private List<AudioMediaAsset> mAssetList;
        private double mSecondsCount;
        private System.Windows.Forms.Timer  tmCommitTimer = new System.Windows.Forms.Timer () ;
        private System.Windows.Forms.Timer tmUpdateDisplay = new System.Windows.Forms.Timer();

        /// <summary>
        /// The audio recorder used by the recording session.
        /// </summary>
        public Audio.AudioRecorder AudioRecorder
        {
            get { return mRecorder; }
        }

        /*
        // Property to set Autocommit interval time in seconds
        public int CommitIntervalSeconds
        {
            get { return tmCommitTimer.Interval/ 1000 ; }
            set { tmCommitTimer.Interval = value * 1000; }
        }
        */

        // Record session events
        public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;
        public event Events.Audio.Recorder.FinishingPageHandler FinishingPage;
        public event Events.Audio.Recorder.StartingSectionHandler StartingSection;

        Obi.Events.Audio.Recorder.PhraseEventArgs mPhraseEventsArgs;  // JQ: should not be a class variable

        /// <summary>
        /// React to state change events from the recorder.
        /// </summary>
        private void AudioRecorder_StateChanged(object sender, Events.Audio.Recorder.StateChangedEventArgs state)
        {
        }

        /// <summary>
        /// React to update events from the VU meter.
        /// </summary>
        private void AudioRecorder_UpdateVuMeter(Object sender, Events.Audio.Recorder.UpdateVuMeterEventArgs update)
        {
        }

        /// <summary>
        /// Create a recording session for a project starting from a given node.
        /// </summary>
        /// <param name="project">The project in which we are recording.</param>
        /// <param name="recorder">The audio recorder from the project.</param>
        /// <param name="node">The phrase node after which we record, or the section in which we append.
        /// If null, then append a new section.</param>
        /// <param name="channels">Number of channels of audio to record.</param>
        /// <param name="sampleRate">Sample rate of audio to record.</param>
        /// <param name="bitDepth">Bit depth of audio to record.</param>
        public RecordingSession(Project project, AudioRecorder recorder, ObiNode node, int channels, int sampleRate, int bitDepth)
        {
            mProject = project;
            mRecorder = recorder;
            
            mChannels = channels;
            // note: should add a convenience method to asset manager to get preferred audio format
            // and use it for the recording session
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            // set up event handlers
            Audio.AudioRecorder.Instance.StateChanged += new Events.Audio.Recorder.StateChangedHandler(AudioRecorder_StateChanged);

            Audio.AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(AudioRecorder_UpdateVuMeter);

            // StartingPhrase += new Obi.Events.Audio.Recorder.StartingPhraseHandler(CatchEvents);
            // ContinuingPhrase += new Obi.Events.Audio.Recorder.ContinuingPhraseHandler(CatchEvents);
            //FinishingPhrase += new Obi.Events.Audio.Recorder.FinishingPhraseHandler(CatchEvents);
            //StartingSection += new Obi.Events.Audio.Recorder.StartingSectionHandler(CatchEvents);
            //FinishingPage += new Obi.Events.Audio.Recorder.FinishingPageHandler(CatchEvents);
            

            // initialise commit timer
            //tmCommitTimer.Tick += new System.EventHandler( tmCommitTimer_tick );  //  Avn : Disabled on 2 Dec 2006
            //tmCommitTimer.Interval = 300000;    //  Avn : Disabled on 2 Dec 2006
            tmUpdateDisplay.Tick += new System.EventHandler ( tmUpdateDisplay_tick );
            tmUpdateDisplay.Interval = 1000 ;
        }


        /// <summary>
        /// Listen. This may happen at the start of the seession, or after pause was pressed when we were recording.
        /// Create a new asset to record in.
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
        /// 
        /// </summary>
        public void Record()
        {
            if (mRecorder.State == AudioRecorderState.Idle)
            {
                //mAsset = Project.GetAudioMediaAsset(mCurrentPhrase);
                // initialise lists
                mPhraseMarks = new List<double>();
                mSectionMarks = new List<int>();
                mPageMarks = new List<int>();
                mAssetList = new List<AudioMediaAsset>();
                mSecondsCount = 0;
                mRecordingAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mRecorder.StartRecording(mRecordingAsset);
                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, 0, 0.0);
                StartingPhrase(this, mPhraseEventsArgs);
                //UpdateDisplayThread = new Thread(new ThreadStart(UpdateDisplayPeriodically));
                //System.Media.SystemSounds.Exclamation.Play();
                //tmCommitTimer.Enabled = true;  // avn: Disabled on 2 Dec 2006
                tmUpdateDisplay.Enabled = true;
            }
        }

        /// <summary>
        /// Stop recording altogether.
        /// </summary>
        public void Stop()
        {
            //tmCommitTimer.Enabled = false;    // avn: Disabled on 2 Dec 2006
            tmUpdateDisplay.Enabled = false;
            Obi.Events.Audio.Recorder.PhraseEventArgs e = StoppedRecording();
            if (e != null) FinishingPhrase(this, e);
        }

        /// <summary>
        /// When committin, do not send an event.
        /// </summary>
        private void StopForCommit()
        {
            //tmCommitTimer.Enabled = false;    // Avneesh : Should not be disabled as further commits will be disabled
            StoppedRecording();
            //mVuMeter.CloseVuMeterForm();  // Avneesh : Should not be closed to keep recording session smooth for user inspite of internal commits
        }

        /// <summary>
        /// Convenience function to stop recording.
        /// </summary>
        /// <returns>True if we were indeed recording.</returns>
        /// <remarks>May throw an exception.</remarks>
        private Obi.Events.Audio.Recorder.PhraseEventArgs StoppedRecording()
        {
            Obi.Events.Audio.Recorder.PhraseEventArgs e = null;
            if (mRecorder.State != AudioRecorderState.Idle)
            {
                bool Recording = mRecorder.State == AudioRecorderState.Recording;
                mRecorder.StopRecording();
                if (Recording)
                {
                    mPhraseMarks.Add(mRecordingAsset.LengthInMilliseconds);
                    mAssetList.Add(mAsset);
                    //double mAssetLengthInMs = mPhraseMarks[0];
                    double mAssetLengthInMs;
                    if (mPhraseMarks.Count > 1)
                        mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
                    else
                        mAssetLengthInMs = mPhraseMarks[0];

                    e = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);
                    AudioClip Clip = new AudioClip(mRecordingAsset.Clips[0].Path, 0.0, mPhraseMarks[0]);
                    mAssetList[0].AddClip(Clip);
                    // here for loop is used to trigger events to make appropriate phrases, sections , pages which may be caught in
                    // project class phrases, sections and page numbers were marked during recording session.
                    for (int i = 0; i < mPhraseMarks.Count - 1; i++)
                    {
                        Clip = new AudioClip(mRecordingAsset.Clips[0].Path, mPhraseMarks[i], mPhraseMarks[i + 1]);
                        mAssetList[i + 1].AddClip(Clip);
                    }
                }
            }
            // clear all the lists and assets
            mPhraseMarks = null;
            mPageMarks = mSectionMarks = null;
            mAsset = mRecordingAsset = null;
            //if (UpdateDisplayThread != null && UpdateDisplayThread.IsAlive) UpdateDisplayThread.Abort();
            return e;
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                mPhraseMarks.Add(mRecorder.CurrentTime);
                int last = mPhraseMarks.Count - 1;
                double length = mPhraseMarks.Count > 1 ? mPhraseMarks[last] - mPhraseMarks[last - 1] : mPhraseMarks[0];
                Events.Audio.Recorder.PhraseEventArgs e = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, last, length);
                mAssetList.Add(mAsset);
                FinishingPhrase(this, e);
                mAsset = mProject.AssetManager.NewAudioMediaAsset (mChannels , mBitDepth,  mSampleRate);
                e = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, 0.0);
                StartingPhrase (this, mPhraseEventsArgs);
            }
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

                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs );
                mAssetList.Add(mAsset);
                FinishingPhrase(this, mPhraseEventsArgs);

                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count , 0.0);
                
                StartingSection(this,mPhraseEventsArgs  );
                StartingPhrase(this, mPhraseEventsArgs);
                
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

                double mAssetLengthInMs  ;
                if ( mPhraseMarks.Count > 1 )
                mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
            else
                    mAssetLengthInMs = mPhraseMarks[0];

                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);
                mAssetList.Add(mAsset);
                FinishingPhrase(this, mPhraseEventsArgs);

                FinishingPage(this, mPhraseEventsArgs);
                mPageMarks.Add(mPhraseMarks.Count - 1);

                mAsset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count , 0.0);
                StartingPhrase(this, mPhraseEventsArgs);

            }
            // somewhere we'll have something like:
            // mProject.AssignNumber(phrase)
        }

        /// <summary>
        /// Throw a continuing phrase event every second.
        /// </summary>
        private void UpdateDisplayPeriodically()
        {
            Thread.Sleep(1000);
            mSecondsCount++;
            mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, mRecorder.CurrentTime);
            ContinuingPhrase(this, mPhraseEventsArgs);
        }

        private void tmUpdateDisplay_tick(object sender, EventArgs e)
        {
//            System.Media.SystemSounds.Exclamation.Play();
            mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, mRecorder.CurrentTime);
            ContinuingPhrase(this, mPhraseEventsArgs);
        }

        private void tmCommitTimer_tick ( object sender , EventArgs e )
        {
            StopForCommit();
            Record();
        }
    }
}
