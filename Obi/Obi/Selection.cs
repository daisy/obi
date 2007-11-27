namespace Obi
{
    /// <summary>
    /// Controls for which the selection is managed through the project
    /// view implement this interface (e.g. strip view, TOC view)
    /// </summary>
    public interface IControlWithSelection
    {
        NodeSelection Selection { get; set; }
    }

    /// <summary>
    /// Controls for which the selection can also be renamed.
    /// </summary>
    public interface IControlWithRenamableSelection : IControlWithSelection
    {
        void SelectAndRename(ObiNode node);
    }

    public class WaveformSelection
    {
        private bool mHasCursor;
        private bool mHasSelection;
        public double CursorTime;
        public double SelectionEndTime;

        public bool HasCursor
        {
            get { return mHasCursor; }
            set { mHasCursor = value; mHasSelection = !value; }
        }

        public bool HasSelection
        {
            get { return mHasSelection; }
            set { mHasSelection = value; mHasCursor = !value; }
        }

        public double SelectionBeginTime
        {
            get { return CursorTime; }
            set { CursorTime = value; }
        }

        public WaveformSelection(double time)
        {
            HasCursor = true;
            CursorTime = time;
        }

        public WaveformSelection(double from, double to)
        {
            HasSelection = true;
            CursorTime = from;
            SelectionEndTime = to;
        }
    }

    /// <summary>
    /// Selection structure to tell where a node is selected.
    /// Node should never be null, the whole selection should be.
    /// </summary>
    public class NodeSelection
    {
        public ObiNode Node;                   // the selected node
        public IControlWithSelection Control;  // control in which it is selected
        public bool IsDummy;                   // true if actually a dummy node is selected
        public string Text;                    // selected text within the node
        public WaveformSelection Waveform;     // waveform selection for a block

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        public NodeSelection(ObiNode node, IControlWithSelection control, bool isDummy, string text,
            WaveformSelection waveform)
        {
            Node = node;
            Control = control;
            IsDummy = isDummy;
            Text = text;
            Waveform = waveform;
        }

        public NodeSelection(ObiNode node, IControlWithSelection control) : this(node, control, false, null, null) { }
        public NodeSelection(ObiNode node, IControlWithSelection control, bool isDummy) : this(node, control, isDummy, null, null) { }
        public NodeSelection(ObiNode node, IControlWithSelection control, string text) : this(node, control, false, text, null) { }
        public NodeSelection(ObiNode node, IControlWithSelection control, WaveformSelection waveform) : this(node, control, false, null, waveform) { }

        /// <summary>
        /// Stringify the selection for debug printing.
        /// </summary>
        public override string ToString() { return System.String.Format("{0}{2} in {1}", Node, Control, IsDummy ? "*" : Text != null ? "#" : ""); }

        /// <summary>
        /// Two node selections are equal if they are the selection of the same node in the same control.
        /// </summary>
        public override bool Equals(object obj)
        {
            NodeSelection s = obj as NodeSelection;
            return s != null && s.Node == Node && s.Control == Control && s.IsDummy == IsDummy && s.Text == Text;
        }

        public SectionNode Section { get { return Node as SectionNode; } }
        public SectionNode SectionOnly { get { return Text == null ? Node as SectionNode : null; } }
        public SectionNode SectionOf { get { return Node is PhraseNode ? Node.ParentAs<SectionNode>() : Node as SectionNode; } }
        public PhraseNode Phrase { get { return Node as PhraseNode; } }
        public PhraseNode PhraseOnly { get { return Text == null ? Node as PhraseNode : null; } }

        public bool CanPaste(Clipboard clipboard)
        {
            return clipboard == null ? false :
                Control is ProjectView.TOCView ? clipboard.Node is SectionNode :
                Control is ProjectView.StripsView ? (clipboard.Node is SectionNode && Node is SectionNode)
                                                    || clipboard.Node is PhraseNode :
                false;
        }
    };

    public class Clipboard
    {
        private ObiNode mNode;
        private bool mDeep;

        public Clipboard(ObiNode node, bool deep)
        {
            mNode = node;
            mDeep = deep;
        }

        public ObiNode Copy { get { return (ObiNode)mNode.copy(mDeep, true); } }
        public bool Deep { get { return mDeep; } }
        public ObiNode Node { get { return mNode; } }
    }
}