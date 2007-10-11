namespace Obi
{
    /// <summary>
    /// Controls for which the selection is managed through the project panel
    /// implement this interface (e.g. strip view, TOC view, transport bar...)
    /// </summary>
    public interface IControlWithSelection
    {
        NodeSelection Selection { get; set; }
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

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        /// <param name="node">The selected node.</param>
        /// <param name="control">The control in which it is selected.</param>
        public NodeSelection(ObiNode node, IControlWithSelection control, bool isDummy)
        {
            Node = node;
            Control = control;
            IsDummy = isDummy;
        }

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
            return s != null && s.Node == Node && s.Control == Control && s.IsDummy == IsDummy;
        }

        public SectionNode Section { get { return Node as SectionNode; } }
        public PhraseNode Phrase { get { return Node as PhraseNode; } }
    };
}