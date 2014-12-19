using System;
using urakawa.media.data;
using urakawa.media.data.audio ;

namespace Obi.Events.Audio.Recorder
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
        private double m_TimeFromBeginning  = -1;
        private bool m_IsPage = false;

        public PhraseEventArgs(ManagedAudioMedia audio, int index, double time)
        {
            mAudio = audio;
            mPhraseIndex = index;
            mTime = time;
            
        }

        public PhraseEventArgs(ManagedAudioMedia audio, int index, double time, double timeFromBeginning)
            : this(audio, index, time)
        {
            m_TimeFromBeginning = timeFromBeginning;
        }

        public PhraseEventArgs(ManagedAudioMedia audio, int index, double time, double timeFromBeginning, bool isPage)
            : this(audio, index, time, timeFromBeginning)
        {
            m_IsPage = isPage;
        }
        public ManagedAudioMedia Audio { get { return mAudio; } }
        public int PhraseIndex { get { return mPhraseIndex; } }
        public double Time { get { return mTime; } }
        public double TimeFromBeginning { get { return m_TimeFromBeginning; } }
        public bool IsPage { get { return m_IsPage; } }

    }
}
