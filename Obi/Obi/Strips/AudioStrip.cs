using System;
using System.Collections.Generic;
using System.Text;

using UrakawaApplicationBackend;
using urakawa.core;
using urakawa.media;

namespace Obi.Strips
{
    /// <summary>
    /// An audio strip is a strip of audio assets. Each asset is a phrase, and in turn is a core node.
    /// Note that unused data still corresponds to core nodes, except they are not in the tree.
    /// </summary>
    public class AudioStrip : Strip
    {
        private ParStrip mParent;  // parent par strip
        private List<Phrase> mPhrases;

        /// <summary>
        /// Constructor for a new audio strip.
        /// </summary>
        /// <param name="parent">The par strip that this audio strip belongs to.</param>
        public AudioStrip(ParStrip parent)
        {
            mParent = parent;
            mPhrases = new List<Phrase>();
        }

        /// <summary>
        /// Record some new audio, creating a new phrase and appending it at the end of the strip.
        /// </summary>
        public void Record()
        {
            Phrase ph = new Phrase(mParent.Presentation);
            mPhrases.Add(ph);
        }
    }

    /// <summary>
    /// A phrase encapsulates a node with audio data on it.
    /// </summary>
    public class Phrase
    {
        private CoreNode mNode;  // the core node

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
        }

        /// <summary>
        /// Create a phrase from an existing node.
        /// </summary>
        /// <param name="node"></param>
        public Phrase(CoreNode node)
        {
            mNode = node;
        }

        /// <summary>
        /// Create a phrase node without content.
        /// </summary>
        public Phrase(Presentation presentation)
        {
            mNode = presentation.getCoreNodeFactory().createNode();
        }

        /// <summary>
        /// Create a phrase node from a sequence media object.
        /// </summary>
        /// <param name="presentation">The presentation that the node will belong to.</param>
        /// <param name="media">The sequence media object to create the audio from.</param>
        public Phrase(Presentation presentation, SequenceMedia media)
        {
            mNode = presentation.getCoreNodeFactory().createNode();
            ChannelsProperty audioprop = (ChannelsProperty)presentation.getPropertyFactory().createChannelsProperty();
            ChannelsManager manager = (ChannelsManager)presentation.getChannelsManager();
            Channel audiochan = (Channel)((manager.getChannelByName(Project.AUDIO_CHANNEL)[0]));
            audioprop.setMedia(audiochan, media);
            mNode.setProperty(audioprop);
        }
    }
}
