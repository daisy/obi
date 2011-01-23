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

        /// <summary>
        /// Create a phrase node.
        /// </summary>
        public PhraseNode(Presentation presentation): base(presentation) {}
        public PhraseNode(Presentation presentation, EmptyNode.Role kind) : base(presentation, kind) {} 
        public PhraseNode(Presentation presentation, string custom) : base(presentation, Role.Custom) {}


        /// <summary>
        /// The audio media data associated with this node.
        /// </summary>
        public ManagedAudioMedia Audio
        {
            get { return (ManagedAudioMedia)getProperty<ChannelsProperty>().getMedia(Presentation.AudioChannel); }
            set
            {
                getProperty<ChannelsProperty>().setMedia(Presentation.AudioChannel, value);
                if (NodeAudioChanged != null) NodeAudioChanged(this, new NodeEventArgs<PhraseNode>(this));
            }
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
        /// Merge the audio of this phrase with the audio of another phrase and notify that audio has changed.
        /// </summary>
        public void MergeAudioWith(ManagedAudioMedia audio)
        {
            Audio.mergeWith(audio);
            if (NodeAudioChanged != null) NodeAudioChanged(this, new NodeEventArgs<PhraseNode>(this));
        }

        /// <summary>
        /// Preceding phrase node in linear order in the whole project.
        /// Null if it is the first phrase in the project.
        /// </summary>
        public PhraseNode PrecedingPhraseInProject
        {
            get
            {
                ObiNode prev;
                for (prev = PrecedingNode; prev != null && !(prev is PhraseNode); prev = PrecedingNode) ;
                return prev as PhraseNode;
            }
        }

        /// <summary>
        /// Signal a change in the audio for this phrase (used during recording)
        /// </summary>
        public void SignalAudioChanged(object sender, ManagedAudioMedia media)
        {
            if (NodeAudioChanged != null) NodeAudioChanged(sender, new NodeEventArgs<PhraseNode>(this));
        }

        /// <summary>
        /// Split the audio of this phrase at the given position and notified that audio has changed.
        /// </summary>
        /// <returns>The half of the split audio after the split point.</returns>
        public ManagedAudioMedia SplitAudio(urakawa.media.timing.Time splitPoint)
        {
            ManagedAudioMedia newAudio = Audio.split(splitPoint);
            if (NodeAudioChanged != null) NodeAudioChanged(this, new NodeEventArgs<PhraseNode>(this));
            return newAudio;
        }

        /// <summary>
        /// We use our own element name for XUK output.
        /// </summary>
        public override string GetTypeNameFormatted()  //sdk2 : used uinstead of GetXukLocalName as this function is called by xukLocalName in IXukable
        {
            return XUK_ELEMENT_NAME;
        }

        public override ObiNode Detach()
        {
            if (Role_ == Role.Heading) AncestorAs<SectionNode>().UnsetHeading(this);
            return base.Detach();
        }

        public override string BaseString() { return base.BaseString(Audio.getDuration().getTimeDeltaAsMillisecondFloat()); }
        public override string BaseStringShort() { return base.BaseStringShort(Audio.getDuration().getTimeDeltaAsMillisecondFloat()); }
        public override double Duration { get { return Audio.getDuration().getTimeDeltaAsMillisecondFloat(); } }
    }
}