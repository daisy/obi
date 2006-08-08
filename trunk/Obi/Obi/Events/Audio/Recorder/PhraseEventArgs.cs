using System;

namespace Obi.Events.Audio.Recorder
{
    /// <summary>
    /// A new phrase is being recorded. Time myst be 0.
    /// </summary>
    public delegate void StartingPhraseHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// The latest phrase being recorded is updated. Time is the current length of the phrase.
    /// </summary>
    public delegate void ContinuingPhraseHandler(object sender, PhraseEventArgs e);

    /// <summary>
    /// The latest phrase is done. Time is the total length of the phrase.
    /// </summary>
    public delegate void FinishingPhraseHandler(object sender, PhraseEventArgs e);
 
    /// <summary>
    /// A new phrase to be created.
    /// </summary>
    class PhraseEventArgs: EventArgs
    {
        private double mTime;  // time of the asset so far

        public double Time
        {
            get { return mTime; }
        }

        public PhraseEventArgs(double time)
        {
            mTime = time;
        }
    }
}
