namespace Obi.Events.Node
{
    public delegate void AddedSectionNodeHandler(object sender, IndexEventArgs<SectionNode> e);
    // public delegate void PastedSectionNodeHandler(object sender, IndexEventArgs e);

    /// <summary>
    /// A node event with an index field.
    /// </summary>
    public class IndexEventArgs<NodeType>: EventArgs<NodeType>
    {
        int mIndex;  // the index

        public int Index
        {
            get { return mIndex; }
        }

        public IndexEventArgs(NodeType node, int index)
            : base(node)
        {
            mIndex = index;
        }
    }
}
