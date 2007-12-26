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

    public class AudioRange
    {
        public bool HasCursor;             // selection is just a cursor position; if false, begin/end
        public double CursorTime;          // time position of the cursor (if true)
        public double SelectionBeginTime;  // begin time of selection
        public double SelectionEndTime;    // end time of selection

        public AudioRange(double time)
        {
            HasCursor = true;
            CursorTime = time;
        }

        public AudioRange(double from, double to)
        {
            HasCursor = false;
            SelectionBeginTime = from;
            SelectionEndTime = to;
        }

        public override bool Equals(object obj)
        {
            AudioRange s = obj as AudioRange;
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
        public bool Soft;                      // soft selection flag

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        public NodeSelection(ObiNode node, IControlWithSelection control)
        {
            Node = node;
            Control = control;
            Soft = false;
        }

        /// <summary>
        /// Stringify the selection for debug printing.
        /// </summary>
        public override string ToString() { return String.Format("{2}{0} in {1}", Node, Control, Soft ? "~" : ""); }

        /// <summary>
        /// Two node selections are equal if they are the selection of the same node in the same control.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType() &&
                ((NodeSelection)obj).Soft == Soft && ((NodeSelection)obj).Node == Node && ((NodeSelection)obj).Control == Control;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public SectionNode Section { get { return Node as SectionNode; } }
        public SectionNode SectionOf { get { return Node is PhraseNode ? Node.ParentAs<SectionNode>() : Node as SectionNode; } }
        public PhraseNode Phrase { get { return Node as PhraseNode; } }

        /// <summary>
        /// What can be pasted where? Let's see.
        /// We'll look in the clipboard and look at what is in this selection.
        /// If we have nothing to paste, then we cannot paste.
        /// If we have a section node, then it can be pasted in a section or a strip.
        /// If we have an empty node, then it can be pasted in anything selected in the strips view. 
        /// </summary>
        public virtual bool CanPaste(Clipboard clipboard)
        {
            return clipboard == null ? false :
                clipboard.Node is SectionNode ? Node is SectionNode : Control is ProjectView.StripsView;
        }

        /// <summary>
        /// Create the paste command for pasting whatever is in the clipboard in the current selection.
        /// </summary>
        public virtual urakawa.undo.ICommand PasteCommand(ProjectView.ProjectView view)
        {
            return view.Clipboard is AudioClipboard ? PasteCommandAudio(view) : PasteCommandNode(view);
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



        // Create a new phrase node with the audio from the clipboard
        // and merge the selected node with this one.
        private urakawa.undo.ICommand PasteCommandAudio(ProjectView.ProjectView view)
        {
            AudioClipboard c = (AudioClipboard)view.Clipboard;
            urakawa.media.data.audio.ManagedAudioMedia media = ((PhraseNode)view.Clipboard.Node).Audio.copy(
                new urakawa.media.timing.Time(c.AudioRange.SelectionBeginTime),
                new urakawa.media.timing.Time(c.AudioRange.SelectionEndTime));
            PhraseNode phrase = view.Presentation.CreatePhraseNode(media);
            urakawa.undo.CompositeCommand p = view.Presentation.CreateCompositeCommand(Localizer.Message("paste_audio"));
            p.append(new Commands.Node.AddNode(view, phrase, ParentForNewNode(phrase), IndexForNewNode(phrase)));
            if (Node is PhraseNode)
            {
                p.append(new Commands.Node.MergeAudio(view, phrase));
            }
            else
            {
                phrase.CopyKind((EmptyNode)view.Selection.Node);
                phrase.Used = view.Selection.Node.Used;
                Commands.Node.Delete delete = new Commands.Node.Delete(view, view.Selection.Node);
                delete.UpdateSelection = false;
                p.append(delete);
            }
            return p;
        }

        // Paste a node in or after another node.
        private urakawa.undo.ICommand PasteCommandNode(ProjectView.ProjectView view)
        {
            Commands.Node.Paste paste = new Commands.Node.Paste(view);
            urakawa.undo.CompositeCommand p = view.Presentation.CreateCompositeCommand(paste.getShortDescription());
            p.append(paste);
            if (paste.DeleteSelectedBlock)
            {
                Commands.Node.Delete delete = new Commands.Node.Delete(view, view.Selection.Node);
                delete.UpdateSelection = false;
                p.append(delete);
            }
            if (paste.Copy.Used && !paste.CopyParent.Used)
            {
                paste.Copy.acceptDepthFirst(
                    delegate(urakawa.core.TreeNode node)
                    {
                        if (node is ObiNode && ((ObiNode)node).Used)
                        {
                            p.append(new Commands.Node.ToggleNodeUsed(view, (ObiNode)node));
                        }
                        return true;
                    }, delegate(urakawa.core.TreeNode node) { }
                );
            }
            return p;
        }
    };

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
        private AudioRange mAudioRange;

        public AudioSelection(PhraseNode node, IControlWithSelection control, AudioRange audioRange)
            : base(node, control)
        {
            mAudioRange = audioRange;
        }

        public AudioRange AudioRange { get { return mAudioRange; } }

        /// <summary>
        /// If audio is selected, then only audio can be pasted.
        /// </summary>
        public override bool CanPaste(Clipboard clipboard) { return clipboard is AudioClipboard; }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == GetType() &&
                ((AudioSelection)obj).AudioRange == mAudioRange && base.Equals(obj);
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

        /// <summary>
        /// Since we're in the strip, section nodes cannot be pasted.
        /// </summary>
        public override bool CanPaste(Clipboard clipboard) { return clipboard != null && !(clipboard.Node is SectionNode); } 

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

        public bool Deep { get { return mDeep; } }
        public ObiNode Node { get { return mNode; } }
    }

    public class AudioClipboard: Clipboard
    {
        private AudioRange mAudioRange;

        public AudioClipboard(PhraseNode node, AudioRange range)
            : base(node, true)
        {
            mAudioRange = range;
            if (mAudioRange.HasCursor) throw new Exception("Expected actual audio selection.");
            if (!(Node is PhraseNode)) throw new Exception("Expected phrase node.");
        }

        public AudioRange AudioRange { get { return mAudioRange; } }
    }
}