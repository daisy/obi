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
        public bool HasCursor;             // selection is just a cursor position; if false, begin/end
        public double CursorTime;          // time position of the cursor (if true)
        public double SelectionBeginTime;  // begin time of selection
        public double SelectionEndTime;    // end time of selection

        public WaveformSelection(double time)
        {
            HasCursor = true;
            CursorTime = time;
        }

        public WaveformSelection(double from, double to)
        {
            HasCursor = false;
            SelectionBeginTime = from;
            SelectionEndTime = to;
        }

        public override bool Equals(object obj)
        {
            WaveformSelection s = obj as WaveformSelection;
            return s != null && HasCursor ? s.HasCursor && s.CursorTime == CursorTime :
                !s.HasCursor && s.SelectionBeginTime == SelectionBeginTime && s.SelectionEndTime == SelectionEndTime;
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
        public WaveformSelection Waveform;     // waveform selection for a block

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        public NodeSelection(ObiNode node, IControlWithSelection control, WaveformSelection waveform)
        {
            Node = node;
            Control = control;
            Waveform = waveform;
        }

        public NodeSelection(ObiNode node, IControlWithSelection control) : this(node, control, null) { }

        /// <summary>
        /// Stringify the selection for debug printing.
        /// </summary>
        public override string ToString() { return System.String.Format("{0} in {1}", Node, Control); }

        /// <summary>
        /// Two node selections are equal if they are the selection of the same node in the same control.
        /// </summary>
        public override bool Equals(object obj)
        {
            NodeSelection s = obj as NodeSelection;
            return s != null && s.Node == Node && s.Control == Control && s.Waveform == Waveform;
        }

        public SectionNode Section { get { return Node as SectionNode; } }
        public SectionNode SectionOf { get { return Node is PhraseNode ? Node.ParentAs<SectionNode>() : Node as SectionNode; } }
        public PhraseNode Phrase { get { return Node as PhraseNode; } }

        public bool CanPaste(Clipboard clipboard)
        {
            return clipboard == null ? false :
                Control is ProjectView.TOCView ? clipboard.Node is SectionNode :
                Control is ProjectView.StripsView ? (clipboard.Node is SectionNode && Node is SectionNode)
                                                    || clipboard.Node is PhraseNode :
                false;
        }


        /// <summary>
        /// Find the parent node for a new node to be added at the current selection.
        /// The new node can either be a SectionNode or an EmtpyNode.
        /// The rule is to add inside containers and after cursor/phrase block.
        /// </summary>
        public virtual ObiNode ParentForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ?
                (Node is SectionNode ? Node.ParentAs<ObiNode>() : Node.AncestorAs<SectionNode>()) :
                (Node is SectionNode ? Node : Node.ParentAs<ObiNode>());
        }

        public virtual int IndexForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ?
                (Node is SectionNode ? (Node.SectionChildCount + 1) : Node.AncestorAs<SectionNode>().SectionChildCount) :
                (Node is SectionNode ? Node.PhraseChildCount : (Node.Index + 1));
        }

        /// <summary>
        /// Get the parent for adding a new block depending on the context.
        /// </summary>
        public ObiNode ParentForNewBlock()
        {
            // TODO: handle cursor
            return Node is SectionNode ? Node : Node is EmptyNode ? Node.ParentAs<ObiNode>() : null;
        }

        /// <summary>
        /// Get the index at which to insert a new block depending on the context.
        /// </summary>
        /// <returns></returns>
        public int IndexForNewBlock()
        {
            // TODO: handle cursor
            return Node is SectionNode ? Node.PhraseChildCount : Node is EmptyNode ? (Node.Index + 1) : -1;
        }

    };

    /// <summary>
    /// Section dummy selected in the TOC view. The selected section is then the parent of the dummy, which is
    /// always the first child of its parent.
    /// </summary>
    public class DummySelection : NodeSelection
    {
        public DummySelection(ObiNode node, ProjectView.TOCView view) : base(node, view) { }

        public override bool Equals(object obj)
        {
            DummySelection s = obj as DummySelection;
            return s is DummySelection && base.Equals(obj);
        }

        public override ObiNode ParentForNewNode(ObiNode newNode) { return Node; }
        public override int IndexForNewNode(ObiNode newNode) { return 0; }
    }

    /// <summary>
    /// Text selected inside a strip or a section.
    /// </summary>
    public class TextSelection : NodeSelection
    {
        private string mText;

        public TextSelection(SectionNode node, IControlWithSelection control, string text)
            : base(node, control)
        {
            mText = text;
        }

        public string Text { get { return mText; } }

        public override bool Equals(object obj)
        {
            TextSelection s = obj as TextSelection;
            return s != null && s.Text == mText && base.Equals(obj);
        }
    }

    /// <summary>
    /// Cursor selection inside a strip. The actual node is always a section node.
    /// </summary>
    public class StripCursorSelection : NodeSelection
    {
        private int mIndex;  // cursor index in the strip

        public StripCursorSelection(SectionNode node, IControlWithSelection control, int index)
            : base(node, control)
        {
            mIndex = index;
        }

        public int Index { get { return mIndex; } }

        public override bool Equals(object obj)
        {
            StripCursorSelection s = obj as StripCursorSelection;
            return s != null && s.Index == mIndex && base.Equals(s);
        }

        public override ObiNode ParentForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ? Node.ParentAs<ObiNode>() : Node;
        }

        public override int IndexForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ? Node.Index + 1 : mIndex;
        }
    }

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