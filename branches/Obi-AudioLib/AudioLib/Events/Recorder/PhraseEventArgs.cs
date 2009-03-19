using System;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace AudioLib.Events.Audio.Recorder
{
    /// <summary>
    /// A new phrase is being recorded.
    /// </summary>
    public delegate void StartingPhraseHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// The latest phrase being recorded is updated.
    /// </summary>
    public delegate void ContinuingPhraseHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// The latest phrase is done.
    /// </summary>
    public delegate void FinishingPhraseHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// Same as finishing a phrase, but this one will be marked as a page.
    /// </summary>
    public delegate void FinishingPageHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// Same as starting a phrase, but a new section is created first.
    /// </summary>
    public delegate void StartingSectionHandler(object sender, EventArgs e);

    public class PhraseEventArgs: EventArgs
    {
        private ManagedAudioMedia mAudio;
        private int mPhraseIndex;
        private double mTime;

        public PhraseEventArgs(ManagedAudioMedia audio, int index, double time)
        {
            mAudio = audio;
            mPhraseIndex = index;
            mTime = time;
        }

        public ManagedAudioMedia Audio { get { return mAudio; } }
        public int PhraseIndex { get { return mPhraseIndex; } }
        public double Time { get { return mTime; } }
    }
}
