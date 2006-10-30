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
        private int mChannels;            // number of channels of audio to record
        private int mSampleRate;          // sample rate of audio to record
        private int mBitDepth;            // bit depth of audio to record
        private CoreNode mCurrentPhrase;  // phrase currently being recorded
        private CoreNode mFirstPhrase;    // first phrase being recorded

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
        }

        /// <summary>
        /// Listen. This may happen at the start of the seession, or after pause was pressed when we were recording.
        /// Create a new asset to record in.
        /// </summary>
        public void Listen()
        {
            AudioMediaAsset asset = mProject.AssetManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            mRecorder.StartListening(asset);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Record()
        {
            AudioMediaAsset asset = Project.GetAudioMediaAsset(mCurrentPhrase);
            mRecorder.StartRecording(asset);
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
            mRecorder.StopRecording();
            return mFirstPhrase;
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// </summary>
        public void NextPhrase()
        {
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase in a new section.
        /// </summary>
        /// <remarks>Because splitting sections is not implemented yet, this is only possible if appending to a section,
        /// or we may skip the end of the current section.</remarks>
        public void NextSection()
        {
        }

        /// <summary>
        /// Finish the currently recording phrase and continue recording into a new phrase.
        /// The phrase that was just finished receives a page number as well (auto-generated.)
        /// </summary>
        public void MarkPage()
        {
            // somewhere we'll have something like:
            // mProject.AssignNumber(phrase)
        }
    }
}
