using Obi.Audio;
using Obi.Assets;
using urakawa.core;
using System.Collections.Generic ;
using System;

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
        private int mChannels;            // number of channels of audio to record
        private int mSampleRate;          // sample rate of audio to record
        private int mBitDepth;            // bit depth of audio to record

        private AudioMediaAsset mAsset;
        private List<double> mPhraseMarks= new List<double> () ;
        private List <int> mSectionMarks = new List<int>();
        private List <int> mPageMarks = new List<int> () ;
        private List<AudioMediaAsset> mAssetList = new List<AudioMediaAsset>();

        // Record session events
        public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;
        public event Events.Audio.Recorder.FinishingPageHandler FinishingPage;
        public event Events.Audio.Recorder.StartingSectionHandler StartingSection;

        //list of audio assets corressponging to phrases created during recording
        public List<AudioMediaAsset> AssetsList
        {
            get { return mAssetList; }
        }

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
                //m_asset = Project.GetAudioMediaAsset(mCurrentPhrase);
                mAsset = null;  // create a new asset here
                mRecorder.StartRecording(mAsset);
                // please check the parameters for the event here...
                StartingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, 0, 0.0));
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

                mRecorder.StopRecording();

                // check location/values of this:
                FinishingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1,
                    mAsset.LengthInMilliseconds));


                if (Recording == true)
                {
                    if (mPhraseMarks.Count != 0)
                    {
                        
                        AudioMediaAsset asset;
                        for (int i = 0; i < mPhraseMarks.Count - 1; i++)
                        {
                            
                            asset = mAsset.GetChunk(mPhraseMarks[i], mPhraseMarks[i + 1]);
                            mProject.AssetManager.AddAsset( asset);
                            mAssetList.Add ( asset ) ;
                        }
                        asset =  mAsset.GetChunk(mPhraseMarks[mPhraseMarks.Count - 1], mAsset.LengthInMilliseconds  ) ;
                        mProject.AssetManager.AddAsset(asset);
                        mAssetList.Add(asset);
                    }
                    else
                    {
                        mAssetList.Add(mAsset);
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
                // check these:
                FinishingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1,
                    mAsset.LengthInMilliseconds));
                StartingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, 0.0));
            }
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase in a new section.
        /// </summary>
        /// <remarks>Because splitting sections is not implemented yet, this is only possible if appending to a section,
        /// or we may skip the end of the current section. But the recorder doesn't need to be aware of that, so it will
        /// happily send an event no matter what.</remarks>
        public void NextSection()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
            {
                mSectionMarks.Add(mPhraseMarks.Count - 1);
                // check these:
                FinishingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1,
                    mAsset.LengthInMilliseconds));
                StartingSection(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, 0.0));
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
                mPageMarks.Add(mPhraseMarks.Count - 1);
                // check these:
                FinishingPage(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count - 1,
                    mAsset.LengthInMilliseconds));
                StartingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAsset, mPhraseMarks.Count, 0.0));
            }
        }
    }
}
