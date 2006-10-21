namespace Obi.Events.Node.Phrase
{
    public delegate void AddedPhraseNodeHandler(object sender, AddedEventArgs e);
    public delegate void foo(object sender, AddedEventArgs e, PhraseNode n);

    public class AddedEventArgs : EventArgs
    {
        private int mIndex;     // index of the section in the list of child sections of the parent.

        public int Index
        {
            get { return mIndex; }
        }

        public AddedEventArgs(object origin, PhraseNode node, int index)
            : base(origin, node)
        {
            mIndex = index;
        }
    }
}