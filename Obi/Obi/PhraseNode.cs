using System;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.property.channel;

namespace Obi
{
    public class PhraseNode: EmptyNode
    {
        /// <summary>
        /// This event is sent when the audio
        /// </summary>
        public event NodeEventHandler<PhraseNode> NodeAudioChanged;

        public static new readonly string XUK_ELEMENT_NAME = "phrase";  // name of the element in the XUK file

        public PhraseNode(Presentation presentation): base(presentation) {}
        public PhraseNode(Presentation presentation, EmptyNode.Kind kind) : base(presentation, kind) {} 
        public PhraseNode(Presentation presentation, string custom) : base(presentation, Kind.Custom) {}


        /// <summary>
        /// The audio media data associated with this node.
        /// </summary>
        public ManagedAudioMedia Audio
        {
            get { return (ManagedAudioMedia)getProperty<ChannelsProperty>().getMedia(Presentation.AudioChannel); }
            set { getProperty<ChannelsProperty>().setMedia(Presentation.AudioChannel, value); }
        }

        /// <summary>
        /// If used, then is its own first used phrase.
        /// </summary>
        public override PhraseNode FirstUsedPhrase { get { return Used ? this : null; } }

        /// <summary>
        /// Allow only phrase nodes to be inserted.
        /// If the index is negative, count backward from the end (-1 is last.)
        /// </summary>
        public override void Insert(ObiNode node, int index)
        {
            if (!(node is PhraseNode)) throw new Exception("Only phrase nodes can be added as children of a phrase node.");
            if (index < 0) index += getChildCount();
            insert(node, index);
        }

        /// <summary>
        /// Next phrase for this phrase in its section. Null if this phrase is the last one.
        /// </summary>
        public PhraseNode NextPhraseInSection
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = Index;
                return index < parent.PhraseChildCount - 1 ? parent.PhraseChild(index + 1) : null;
            }
        }

        /// <summary>
        /// Previous phrase node in linear order in the whole project.
        /// Null if it is the first phrase in the project.
        /// </summary>
        public PhraseNode PreviousPhraseInProject
        {
            get
            {
                PhraseNode prev = PreviousPhraseInSection;
                if (prev == null)
                {
                    SectionNode prevSection;
                    for (prevSection = ParentAs<SectionNode>().PreviousSection;
                        prevSection != null && prevSection.PhraseChildCount == 0;
                        prevSection = prevSection.PreviousSection) { }
                    if (prevSection != null && prevSection.PhraseChildCount != 0) prev = prevSection.PhraseChild(-1);
                }
                return prev;
            }
        }

        /// <summary>
        /// Previous phrase for this phrase in its section. Null if this phrase is the first one.
        /// </summary>
        public PhraseNode PreviousPhraseInSection
        {
            get
            {
                SectionNode parent = (SectionNode)getParent();
                int index = Index;
                return index > 0 ? parent.PhraseChild(index - 1) : null;
            }
        }
        /// <summary>
        /// Split the audio of this phrase at the given position and notify that the 
        /// </summary>
        /// <returns>The other half of the split audio.</returns>
        public ManagedAudioMedia SplitAudio(urakawa.media.timing.Time splitPoint)
        {
            ManagedAudioMedia newAudio = Audio.split(splitPoint);
            if (NodeAudioChanged != null) NodeAudioChanged(this, new NodeEventArgs<PhraseNode>(this));
            return newAudio;
        }

        /// <summary>
        /// Merge the audio of this phrase with the audio of another phrase and notify that audio has changed.
        /// </summary>
        /// <param name="audio"></param>
        public void MergeAudioWith(ManagedAudioMedia audio)
        {
            Audio.mergeWith(audio);
            if (NodeAudioChanged != null) NodeAudioChanged(this, new NodeEventArgs<PhraseNode>(this));
        }

        public override string getXukLocalName()
        {
            return XUK_ELEMENT_NAME;
        }
    }
}