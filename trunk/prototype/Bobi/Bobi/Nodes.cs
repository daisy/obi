using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{

    public abstract class Node: urakawa.core.TreeNode
    {
        protected Node(urakawa.Presentation presentation): base()
        {
            setPresentation(presentation);
        }

        public override string getXukNamespaceUri() { return DataModelFactory.NS; }
        public override string getXukLocalName() { return GetType().Name; }
    }

    public class NodeFactory : urakawa.core.TreeNodeFactory
    {
        /// <summary>
        /// Create a new node given a QName.
        /// </summary>
        /// <param name="localName">the local part of the qname.</param>
        /// <param name="namespaceUri">the namespace URI of the qname.</param>
        /// <returns>A new node or null if the qname corresponds to no known node.</returns>
        public override urakawa.core.TreeNode createNode(string localName, string namespaceUri)
        {
            if (namespaceUri == DataModelFactory.NS)
            {
                if (localName == typeof(TrackNode).Name)
                {
                    return new TrackNode(getPresentation());
                }
                else if (localName == typeof(AudioNode).Name)
                {
                    return new AudioNode(getPresentation());
                }
            }
            return base.createNode(localName, namespaceUri);
        }

        public override string getXukNamespaceUri() { return DataModelFactory.NS; }
    }

    public class TrackNode : Node
    {
        public TrackNode(urakawa.Presentation presentation) : base(presentation) { }
    }

    public class AudioNode : Node
    {
        public AudioNode(urakawa.Presentation presentation) : base(presentation) { }

        public urakawa.media.data.audio.AudioMediaData Audio
        {
            get
            {
                urakawa.property.channel.ChannelsProperty prop = getProperty<urakawa.property.channel.ChannelsProperty>();
                if (prop != null)
                {
                    urakawa.media.data.audio.ManagedAudioMedia media = (urakawa.media.data.audio.ManagedAudioMedia)
                        prop.getMedia(((Project)getPresentation().getProject()).FindChannel("bobi.audio"));
                    return (urakawa.media.data.audio.AudioMediaData)media.getMediaData();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
