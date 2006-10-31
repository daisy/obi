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
        private CoreNode mCurrentPhrase;  // phrase currently being recorded
        private CoreNode mFirstPhrase;    // first phrase being recorded

        private AudioMediaAsset m_asset;
        private List<double> m_PhraseMarks= new List<double> () ;
        private List <int> m_SectionMarks = new List<int>();
        private List <int> m_PageMarks = new List<int> () ;
        private List<AudioMediaAsset> m_AssetList = new List<AudioMediaAsset>();
        
        //list of audio assets corressponging to phrases created during recording
        public List<AudioMediaAsset> AssetsList
        {
            get
            {
                return m_AssetList;
            }
        }


        // Record session events
        //public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        //public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        //public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;

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
            mCurrentPhrase = null;
            mFirstPhrase = null;
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
                m_asset = Project.GetAudioMediaAsset(mCurrentPhrase);
                mRecorder.StartRecording(m_asset);
            }
        }

        /// <summary>
        /// Stop recording altogether.
        /// When the recording session ends, we will select the first asset recorded so that the user can check
        /// what was recorded. In the future, when multiple selection is implemented we will select all of the
        /// newly recorded phrases.
        /// </summary>
        /// <returns>The first node that was recorded, or null if nothing was recorded.</returns>
        public CoreNode Stop()
        {

            if (mRecorder.State != AudioRecorderState.Idle)
            {

                bool Recording = false;
                if (mRecorder.State == AudioRecorderState.Recording)
                    Recording = true;

                mRecorder.StopRecording();


                if (Recording == true)
                {
                    if (m_PhraseMarks.Count != 0)
                    {
                        
                        AudioMediaAsset asset;
                        for (int i = 0; i < m_PhraseMarks.Count - 1; i++)
                        {
                            
                            asset = m_asset.GetChunk(m_PhraseMarks[i], m_PhraseMarks[i + 1]);
                            mProject.AssetManager.AddAsset( asset);
                            m_AssetList.Add ( asset ) ;
                        }
                        asset =  m_asset.GetChunk(m_PhraseMarks[m_PhraseMarks.Count - 1], m_asset.LengthInMilliseconds  ) ;
                        mProject.AssetManager.AddAsset(asset);
                        m_AssetList.Add(asset);
                    }
                    else
                    {
                        m_AssetList.Add(m_asset);
                    }
                    
                }
            }
            // here for loop is used to trigger events to make appropriate phrases, sections , pages which may be caught in project class
            // phrases, sections and page numbers were marked during recording session.


            mProject.SetAudioMediaAsset( mFirstPhrase , m_AssetList[0]);
            return mFirstPhrase;
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
            if ( mRecorder.State == AudioRecorderState.Recording )
                m_PhraseMarks.Add (  mRecorder.CurrentTime) ;
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase in a new section.
        /// </summary>
        /// <remarks>Because splitting sections is not implemented yet, this is only possible if appending to a section,
        /// or we may skip the end of the current section.</remarks>
        public void NextSection()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
                m_SectionMarks.Add( m_PhraseMarks.Count - 1 );
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// The phrase that was just finished receives a page number as well (auto-generated.)
        /// </summary>
        public void MarkPage()
        {
            if (mRecorder.State == AudioRecorderState.Recording)
                m_PageMarks.Add(m_PhraseMarks.Count - 1);
            // somewhere we'll have something like:
            // mProject.AssignNumber(phrase)
        }
    }
}
