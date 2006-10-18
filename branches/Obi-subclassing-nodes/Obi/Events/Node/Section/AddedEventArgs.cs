namespace Obi.Events.Node.Section
{
    public delegate void AddedSectionNodeHandler(object sender, AddedEventArgs e);
    public delegate void PastedSectionNodeHandler(object sender, AddedEventArgs e);

    public class AddedEventArgs: EventArgs
    {
        private int mIndex;     // index of the section in the list of child sections of the parent.

        public int Index
        {
            get { return mIndex; }
        }

        public AddedEventArgs(object origin, SectionNode node, int index)
            : base(origin, node)
        {
            mIndex = index;
        }
    }
}
