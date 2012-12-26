using System;
using urakawa.command;
using urakawa.media.data.audio.codec;
using urakawa.media.timing;

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

    /// <summary>
    /// Audio range within a block. May be just a cursor (flag HasCursor is set) or a range between two points.
    /// </summary>
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

        public override string ToString()
        {
            return HasCursor ?
                String.Format(Localizer.Message("duration_s_ms"), CursorTime / 1000.0) :
                String.Format("{0}-{1}",
                    String.Format(Localizer.Message("duration_s_ms"), SelectionBeginTime / 1000.0),
                    String.Format(Localizer.Message("duration_s_ms"), SelectionEndTime / 1000.0));
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

        /// <summary>
        /// Create a new selection object.
        /// </summary>
        public NodeSelection(ObiNode node, IControlWithSelection control)
        {
            Node = node;
            Control = control;
        }


        /// <summary>
        /// Get the empty node in context for that selection. In the case of a section node, a text selection,
        /// or a strip cursor with no following empty node, this will be null; in the case of an audio selection,
        /// this will be the node where the selection is.
        /// </summary>
        public virtual EmptyNode EmptyNodeForSelection { get { return Node as EmptyNode; } }


        /// <summary>
        /// Stringify the selection for showing in the status bar.
        /// </summary>
        public override string ToString()
        {
            return String.Format(Localizer.Message("node_selection_to_string"), Node != null? Node.ToString(): "", Control.ToString());
        }

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
                clipboard.Node is SectionNode ? Node is SectionNode : Control is ProjectView.ContentView;
        }

        /// <summary>
        /// Create the paste command for pasting whatever is in the clipboard in the current selection.
        /// </summary>
        public virtual urakawa.command.Command PasteCommand(ProjectView.ProjectView view, bool allowAutoSpecialRole)   //@AssociateNode
        {
            return view.Clipboard is AudioClipboard ? PasteCommandAudio(view) : PasteCommandNode(view, allowAutoSpecialRole);
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
        protected virtual urakawa.command.Command PasteCommandAudio(ProjectView.ProjectView view)
        {
            AudioClipboard c = (AudioClipboard)view.Clipboard;
            //urakawa.media.data.audio.ManagedAudioMedia media = ((PhraseNode)view.Clipboard.Node).Audio.copy(
            urakawa.media.data.audio.ManagedAudioMedia media = view.Presentation.MediaFactory.CreateManagedAudioMedia();

            WavAudioMediaData wavData = ((WavAudioMediaData)((PhraseNode)view.Clipboard.Node).Audio.AudioMediaData).Copy(
                new Time(Convert.ToInt64(c.AudioRange.SelectionBeginTime * Time.TIME_UNIT)),
                new Time(Convert.ToInt64(c.AudioRange.SelectionEndTime * Time.TIME_UNIT)));//sdk2
            media.AudioMediaData = wavData;

            CompositeCommand p = view.Presentation.CreateCompositeCommand(Localizer.Message("paste_audio"));

            if (Node is EmptyNode)
            {
                if (view.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
                {
                    Command pasteCommand = new Commands.Audio.Paste(view, view.TransportBar.SplitBeginTime);
                    p.ChildCommands.Insert(p.ChildCommands.Count, pasteCommand);
                    
                }
                else
                {
                    PhraseNode phrase = view.Presentation.CreatePhraseNode(media);
                    p.ChildCommands.Insert(p.ChildCommands.Count, new Commands.Node.AddNode(view, phrase, ParentForNewNode(phrase), IndexForNewNode(phrase)));
                    p.ChildCommands.Insert(p.ChildCommands.Count, Commands.Node.MergeAudio.GetMergeCommand(view, (EmptyNode)Node, phrase));
                    if (Node is PhraseNode)
                    {
                        p.ChildCommands.Insert(p.ChildCommands.Count, new Commands.UpdateSelection(view, new AudioSelection((PhraseNode)Node, Control, new AudioRange(Node.Duration, Node.Duration + phrase.Duration))));
                    }
                    else if (Node is EmptyNode)
                    {
                        p.ChildCommands.Insert(p.ChildCommands.Count, new Commands.UpdateSelection(view, new NodeSelection(phrase, Control)));
                    }
                }
            }
            else
            {
                PhraseNode phrase = view.Presentation.CreatePhraseNode(media);
                p.ChildCommands.Insert(p.ChildCommands.Count, new Commands.Node.AddNode(view, phrase, ParentForNewNode(phrase), IndexForNewNode(phrase)));
            }
            return p;
        }

        // Paste a node in or after another node.
        protected virtual urakawa.command.Command PasteCommandNode(ProjectView.ProjectView view, bool allowAutoSpecialRoleAssign)   //@AssociateNode
        {
            Commands.Node.Paste paste = new Commands.Node.Paste(view);
            paste.AllowRoleChangeAccordingToSurroundingSpecialNodes = allowAutoSpecialRoleAssign;   //@AssociateNode
            CompositeCommand p = view.Presentation.CreateCompositeCommand(paste.ShortDescription);
            p.ChildCommands.Insert(p.ChildCommands.Count, paste);
            if (paste.DeleteSelectedBlock)
            {
                Commands.Node.Delete delete = new Commands.Node.Delete(view, view.Selection.Node);
                delete.UpdateSelection = false;
                p.ChildCommands.Insert (p.ChildCommands.Count, delete);
            }
            // If the node to paste is a heading phrase, revert to a plain phrase.
            // We consider that being a heading is more of a section things (copying sections conserve the heading.)
            if (paste.Copy is PhraseNode && ((PhraseNode)paste.Copy).Role_ == EmptyNode.Role.Heading)
            {
                Commands.Node.AssignRole behead =
                    new Commands.Node.AssignRole(view, (EmptyNode)paste.Copy, EmptyNode.Role.Plain);
                behead.UpdateSelection = false;
                p.ChildCommands.Insert(p.ChildCommands.Count, behead);
            }
            if (paste.Copy.Used && !paste.CopyParent.Used) view.AppendMakeUnused(p, paste.Copy);
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

        public override EmptyNode EmptyNodeForSelection { get { return null; } }

        public override string ToString()
        {
            return String.Format(Localizer.Message("text_selection_to_string"), Node.ToString(), Control.ToString(), mText);
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
        /// If audio is selected, then audio can be pasted, or the audio from a phrase node.
        /// </summary>
        public override bool CanPaste(Clipboard clipboard)
        {
            return clipboard is AudioClipboard || 
                (clipboard != null && clipboard.Node is PhraseNode);
        }

        /// <summary>
        /// Create a paste command for this selection and the clipboard selection.
        /// </summary>
        public override urakawa.command.Command PasteCommand(Obi.ProjectView.ProjectView view, bool allowAutoSpecialRole)   //@AssociateNode
        {
            return new Commands.Audio.Paste(view);
        }

        public override string ToString()
        {
        string audioRangeString = AudioRange != null ? AudioRange.ToString () : " " ; // for precaution
            return String.Format(Localizer.Message("audio_selection_to_string"), audioRangeString , Node.ToString(), Control.ToString());
        }
    }

    /// <summary>
    /// Cursor selection inside a strip. The actual node is always a section node.
    /// </summary>
    public class StripIndexSelection : NodeSelection
    {
        private int mIndex;  // cursor index in the strip

        public StripIndexSelection(SectionNode node, IControlWithSelection control, int index)
            : base(node, control)
        {
            mIndex = index;
        }

        // Since we're in the strip, section nodes cannot be pasted.
        public override bool CanPaste(Clipboard clipboard) { return clipboard != null && !(clipboard.Node is SectionNode); }

        /// <summary>
        /// Return the block following the cursor, if any (i.e. if not the last cursor in the strip.)
        /// </summary>
        public override EmptyNode EmptyNodeForSelection
        {
            get
            {
                SectionNode section = (SectionNode)Node;
                return mIndex < section.PhraseChildCount ? section.PhraseChild(mIndex) : null;
            }
        }

        protected override urakawa.command.Command PasteCommandAudio(Obi.ProjectView.ProjectView view)
        {
            AudioClipboard c = (AudioClipboard)view.Clipboard;

            urakawa.media.data.audio.ManagedAudioMedia media = view.Presentation.MediaFactory.CreateManagedAudioMedia();

            WavAudioMediaData wavData = ((WavAudioMediaData)((PhraseNode)view.Clipboard.Node).Audio.AudioMediaData).Copy(
            new Time(Convert.ToInt64(c.AudioRange.SelectionBeginTime * Time.TIME_UNIT)),
            new Time(Convert.ToInt64(c.AudioRange.SelectionEndTime * Time.TIME_UNIT)));//sdk2

            media.AudioMediaData = wavData;

            PhraseNode phrase = view.Presentation.CreatePhraseNode(media);
            CompositeCommand p = view.Presentation.CreateCompositeCommand(Localizer.Message("paste_audio"));
            p.ChildCommands.Insert(p.ChildCommands.Count, new Commands.Node.AddNode(view, phrase, ParentForNewNode(phrase), IndexForNewNode(phrase)));
            if (!Node.Used) view.AppendMakeUnused(p, phrase);
            return p;
        }

        public int Index { get { return mIndex; } }

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
            return String.Format(Localizer.Message("strip_cursor_selection_to_string"), mIndex, Node.ToString(), Control.ToString());
        }
    }

    public class MetadataSelection : NodeSelection
    {
        private ProjectView.MetadataItemSelection mItem;

        //public MetadataSelection(RootNode node, ProjectView.MetadataView control, ProjectView.MetadataItemSelection item)
        public MetadataSelection(ObiNode node, ProjectView.MetadataView control, ProjectView.MetadataItemSelection item) //sdk2
            : base(node, control)
        {
            mItem = item;
        }

        public ProjectView.MetadataItemSelection Item { get { return mItem; } }
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