using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;
using urakawa.project;
using UrakawaApplicationBackend;

namespace Zaboom
{
    /// <summary>
    /// Zaboom project is a convenience class to manage the Urakawa project.
    /// </summary>
    class Project: urakawa.project.Project
    {
        private Channel mAudioChannel;  // the single audio channel
        private Channel mTextChannel;   // the single text channel (for labels)
        private List<Phrase> mPhrases;  // the phrases in order

        /// <summary>
        /// Create a new Zaboom project and set up the audio and text channels.
        /// </summary>
        public Project(): base()
        {
            mAudioChannel = this.getPresentation().getChannelFactory().createChannel("audio channel");
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = this.getPresentation().getChannelFactory().createChannel("text channel");
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mPhrases = new List<Phrase>();
        }

        /// <summary>
        /// Append a new metadata object to the project.
        /// </summary>
        /// <param name="name">The name of the metadata object.</param>
        /// <param name="content">The content of the metadata object.</param>
        public void AppendMetadata(string name, string content)
        {
            Metadata meta = (Metadata)this.getMetadataFactory().createMetadata("Metadata");
            meta.setName(name);
            meta.setContent(content);
            this.appendMetadata(meta);
        }

        /// <summary>
        /// Append a new audio asset to the root.
        /// Set the location and timing of the audio node to that of the asset.
        /// Set the text in the node to the label of the asset.
        /// The phrase is added to the list of phrases in that project.
        /// </summary>
        /// <param name="asset">The asset to append.</param>
        public void AppendPhrase(AudioMediaAsset asset)
        {
            CoreNode node = this.getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(PropertyType.CHANNEL);
            AudioMedia audio = (AudioMedia)this.getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
            audio.setLocation(new MediaLocation(asset.Path));
            audio.setClipEnd(new Time((long)Math.Round(asset.LengthInMilliseconds)));
            prop.setMedia(mAudioChannel, audio);
            TextMedia text = (TextMedia)this.getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            text.setText(asset.Name);
            prop.setMedia(mTextChannel, text);
            this.getPresentation().getRootNode().appendChild(node);
            mPhrases.Add(new Phrase(node, asset));
        }
    }
}
