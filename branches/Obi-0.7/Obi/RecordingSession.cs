using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

using Obi.Audio;
using Obi.Assets;
using urakawa.core;



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
        private Thread UpdateDisplayThread;

        private AudioMediaAsset mAsset;
        private List<double> mPhraseMarks= new List<double> () ;
        private List <int> mSectionMarks = new List<int>();
        private List <int> mPageMarks = new List<int> () ;
        private List<AudioMediaAsset> mAssetList = new List<AudioMediaAsset>();
        
        //list of audio assets corressponging to phrases created during recording
        public List<AudioMediaAsset> AssetsList
        {
            get
            {
                return mAssetList;
            }
        }


        // Record session events
        public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;
        Obi.Events.Audio.Recorder.PhraseEventArgs mPhraseEventsArgs;
        public event Events.Audio.Recorder.FinishingPageHandler FinishingPage;
        public event Events.Audio.Recorder.StartingSectionHandler StartingSection;

        // events handler functions for AudioRecorder class
        private void AudioRecorder_StateChanged(object sender, Events.Audio.Recorder.StateChangedEventArgs state)
        {
        }

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
        public RecordingSession(Project project, AudioRecorder recorder, CoreNode node, int channels, int sampleRate, int bitDepth)
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

                mRecordingAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mRecorder.StartRecording(mRecordingAsset);

                mAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, 0, 0.0);
                StartingPhrase(this, mPhraseEventsArgs);

                UpdateDisplayThread = new Thread ( new ThreadStart ( UpdateDisplayPeriodically )) ;
            }
        }

        /// <summary>
        /// Stop recording altogether.
        /// </summary>
        public void Stop()
        {

            if (mRecorder.State != AudioRecorderState.Idle)
            {

                bool Recording = false;
                if (mRecorder.State == AudioRecorderState.Recording)
                    Recording = true;

                StopRecording();

                if (Recording == true)
                {
                    mPhraseMarks.Add( mRecordingAsset.LengthInMilliseconds );
                    mAssetList.Add(mAsset);
                    
                    double mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
                    mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);
                    FinishingPhrase(this, mPhraseEventsArgs);

                        AudioClip Clip = new AudioClip(mProject.AssetManager.AssetsDirectory, 0.0, mPhraseMarks[0]);
                        mAssetList[0].AddClip(Clip);
                        for (int i = 0; i < mPhraseMarks.Count - 1; i++)
                        {
                            Clip = new AudioClip(mProject.AssetManager.AssetsDirectory, mPhraseMarks [ i ] , mPhraseMarks[i + 1]);
                            mAssetList[i + 1 ].AddClip(Clip);
                    }
                }
                    }
             
            // here for loop is used to trigger events to make appropriate phrases, sections , pages which may be caught in project class
            // phrases, sections and page numbers were marked during recording session.
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                mPhraseMarks.Add(mRecorder.CurrentTime);
                
                double mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);

                mAssetList.Add(mAsset);
                FinishingPhrase(this, mPhraseEventsArgs);
                

                mAsset = new AudioMediaAsset ( mChannels , mBitDepth,  mSampleRate ) ;
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count ,  0.0)  ;
                StartingPhrase ( this , mPhraseEventsArgs ) ;

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
                double mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs );
                mAssetList.Add(mAsset);
                FinishingPhrase(this, mPhraseEventsArgs);

                mAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
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
                double mAssetLengthInMs = mPhraseMarks[mPhraseMarks.Count - 1] - mPhraseMarks[mPhraseMarks.Count - 2];
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1, mAssetLengthInMs);
                mAssetList.Add(mAsset);
                FinishingPhrase(this, mPhraseEventsArgs);

                FinishingPage(this, mPhraseEventsArgs);
                mPageMarks.Add(mPhraseMarks.Count - 1);

                mAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mPhraseEventsArgs = new Obi.Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count , 0.0);
                StartingPhrase(this, mPhraseEventsArgs);

            }
            // somewhere we'll have something like:
            // mProject.AssignNumber(phrase)
        }

        private void UpdateDisplayPeriodically ()
        {
            Thread.Sleep(1000);
            ContinuingPhrase(this, mPhraseEventsArgs);
        }

        private void StopRecording()
        {
            try
            {
                mRecorder.StopRecording();
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("recorder_error_text"), @Localizer.Message("recorder_error_caption" ))  ;
                
            }
        }


    } // end of class
}
