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

    public interface IControlWithRenamableSelection : IControlWithSelection
    {
        void SelectAndRename(ObiNode node);
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

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        /// <param name="node">The selected node.</param>
        /// <param name="control">The control in which it is selected.</param>
        public NodeSelection(ObiNode node, IControlWithSelection control, bool isDummy, string text)
        {
            Node = node;
            Control = control;
            IsDummy = isDummy;
            Text = text;
        }

        public NodeSelection(ObiNode node, IControlWithSelection control) : this(node, control, false, null) { }
        public NodeSelection(ObiNode node, IControlWithSelection control, bool isDummy) : this(node, control, isDummy, null) { }
        public NodeSelection(ObiNode node, IControlWithSelection control, string text) : this(node, control, false, text) { }

        /// <summary>
        /// Stringify the selection for debug printing.
        /// </summary>
        /// <returns>"{node type} in {control}"</returns>
        public override string ToString() { return System.String.Format("{0}{2} in {1}", Node, Control, IsDummy ? "*" : ""); }

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
                Control is ProjectView.StripsView ? clipboard.Node is SectionNode :
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