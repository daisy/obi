using System;

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

        public override int GetHashCode() { return base.GetHashCode(); }
    }

    /// <summary>
    /// Selection structure to tell where a node is selected.
    /// Node should never be null, the whole selection should be.
    /// </summary>
    public class NodeSelection
    {
        public ObiNode Node;                   // the selected node
        public IControlWithSelection Control;  // control in which it is selected

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        public NodeSelection(ObiNode node, IControlWithSelection control)
        {
            Node = node;
            Control = control;
        }

        /// <summary>
        /// Stringify the selection for debug printing.
        /// </summary>
        public override string ToString() { return String.Format("{0} in {1}", Node, Control); }

        /// <summary>
        /// Two node selections are equal if they are the selection of the same node in the same control.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType() &&
                ((NodeSelection)obj).Node == Node && ((NodeSelection)obj).Control == Control;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

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
                (Node is SectionNode ? (Node.Index + 1) : Node.AncestorAs<SectionNode>().SectionChildCount) :
                (Node is SectionNode ? Node.PhraseChildCount : (Node.Index + 1));
        }
    };

    /// <summary>
    /// Section dummy selected in the TOC view. 
    /// </summary>
    public class DummySelection : NodeSelection
    {
        private ProjectView.DummyNode mDummy;
        public ProjectView.DummyNode Dummy
        {
            set {mDummy = value;}
            get {return mDummy;}
        }
        public DummySelection(ObiNode node, ProjectView.TOCView view) : base(node, view) { }

        public override ObiNode ParentForNewNode(ObiNode newNode) 
        {
            //this was the old behavior, before mDummy was introduced.  keeping it so I don't break anything.
            if (mDummy == null) return Node;

            //the new node is getting pasted as an "oldest" sibling of the selection
            if (mDummy.Type == Obi.ProjectView.DummyNode.DummyType.BEFORE) return Node.ParentAs<ObiNode>();
            //the new node is getting pasted as a first child of the selection
            else if (mDummy.Type == Obi.ProjectView.DummyNode.DummyType.CHILD) return Node;
            //default: the new node is getting pasted after the selection
            else return Node.ParentAs<ObiNode>();
        }
        public override int IndexForNewNode(ObiNode newNode) 
        {
            //this was the old behavior, before mDummy was introduced.  keeping it so I don't break anything.
            if (mDummy == null) return 0;

            //the new node is getting pasted before the selection (because the selection was a first child)
            if (mDummy.Type == Obi.ProjectView.DummyNode.DummyType.BEFORE) return 0;
            //the new node is getting pasted as a first child of the selection
            else if (mDummy.Type == Obi.ProjectView.DummyNode.DummyType.CHILD) return 0;
            //default: the new node is getting pasted after the selection
            else return Node.Index + 1;
        }
        public override string ToString() { return String.Format("Dummy for {0}", base.ToString()); }
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
            return obj != null && obj.GetType() == GetType() && ((TextSelection)obj).Text == mText && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return String.Format("\"{0}\" in {1}", mText, base.ToString());
        }
    }

    /// <summary>
    /// Selection of audio within a block.
    /// </summary>
    public class AudioSelection : NodeSelection
    {
        private WaveformSelection mWaveformSeletion;

        public AudioSelection(PhraseNode node, IControlWithSelection control, WaveformSelection waveformSelection)
            : base(node, control)
        {
            mWaveformSeletion = waveformSelection;
        }

        public WaveformSelection WaveformSelection { get { return mWaveformSeletion; } }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType() &&
                ((AudioSelection)obj).WaveformSelection == mWaveformSeletion && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return String.Format("Audio in {0}", base.ToString());
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
            return obj != null && obj.GetType() == GetType() && ((StripCursorSelection)obj).Index == mIndex && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override ObiNode ParentForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ? Node.ParentAs<ObiNode>() : Node;
        }

        public override int IndexForNewNode(ObiNode newNode)
        {
            return newNode is SectionNode ? Node.Index + 1 : mIndex;
        }

        public override string ToString()
        {
            return String.Format("Index {0} in {1}", mIndex, base.ToString());
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

    public class AudioClipboard: Clipboard
    {
        private WaveformSelection mWaveformSelection;

        public AudioClipboard(AudioSelection selection)
            : base(selection.Node, true)
        {
            mWaveformSelection = selection.WaveformSelection;
            if (mWaveformSelection.HasCursor) throw new Exception("Expected actual audio selection.");
            if (!(Node is PhraseNode)) throw new Exception("Expected phrase node.");
        }

        public WaveformSelection WaveformSelection { get { return mWaveformSelection; } }
    }
}