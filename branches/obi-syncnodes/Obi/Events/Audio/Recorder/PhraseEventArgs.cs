using System;

using Obi.Assets;

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
 
    class PhraseEventArgs: EventArgs
    {
        private AudioMediaAsset mAsset;
        private int mPhraseIndex;
        private double mTime;

        public AudioMediaAsset Asset
        {
            get { return mAsset; }
        }

        public int PhraseIndex
        {
            get { return mPhraseIndex; }
        }

        public double Time
        {
            get { return mTime; }
        }

        public PhraseEventArgs(AudioMediaAsset asset, int index, double time)
        {
            mAsset = asset;
            mPhraseIndex = index;
            mTime = time;
        }
    }
}
