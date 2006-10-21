namespace Obi.Events.Node.Phrase
{
    public delegate void UpdateTimeHandler(object sender, UpdateTimeEventArgs e);

    class UpdateTimeEventArgs: EventArgs
    {
        private double mTime;

        public double Time
        {
            get { return mTime; }
        }

        public UpdateTimeEventArgs(PhraseNode node, double time)
            : base(null, node)
        {
            mTime = time;
        }
    }
}
