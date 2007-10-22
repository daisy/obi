using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using urakawa.property.channel;
using urakawa.media.data.audio;

namespace Obi
{
    public class Presentation: urakawa.Presentation
    {
        public static readonly string TEXT_CHANNEL_NAME = "obi.text";  // canonical name of the text channel

        public event Commands.UndoRedoEventHandler CommandExecuted;    // triggered when a command was executed
        public event Commands.UndoRedoEventHandler CommandUnexecuted;  // triggered when a command was unexecuted
        public event SectionNodeEventHandler RenamedSectionNode;       // triggered after a section was renamed

        /// <summary>
        /// The media data manager for the project.
        /// </summary>
        public Audio.DataManager DataManager { get { return (Audio.DataManager)getMediaDataManager(); } }

        /// <summary>
        /// Root node of the presentation.
        /// </summary>
        public RootNode RootNode { get { return (RootNode)getRootNode(); } }

        /// <summary>
        /// Get the text channel of the presentation.
        /// </summary>
        public Channel TextChannel { get { return GetSingleChannelByName(TEXT_CHANNEL_NAME); } }

        /// <summary>
        /// Get the title of the presentation from the metadata.
        /// </summary>
        public string Title { get { return GetFirstMetadataItem(Metadata.DC_TITLE).getContent(); } }

        /// <summary>
        /// The Undo/redo manager for this presentation (with the correct type.)
        /// </summary>
        public Commands.UndoRedoManager UndoRedoManager { get { return (Commands.UndoRedoManager)getUndoRedoManager(); } }


        /// <summary>
        /// Create a phrase node belonging to this presentation.
        /// </summary>
        public PhraseNode CreatePhraseNode()
        {
            return (PhraseNode)getTreeNodeFactory().createNode(PhraseNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
        }

        /// <summary>
        /// Convenience method to create a new plain phrase node from a file.
        /// </summary>
        public PhraseNode CreatePhraseNode(string path) { return CreatePhraseNode(ImportAudioFromFile(path)); }

        /// <summary>
        /// Create a section node belonging to this presentation.
        /// </summary>
        public SectionNode CreateSectionNode()
        {
            return (SectionNode)getTreeNodeFactory().createNode(SectionNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
        }

        /// <summary>
        /// Initialize the metadata of the presentation, and create a title section if necessary.
        /// </summary>
        public void Initialize(string title, bool createTitleSection, string id, UserProfile userProfile)
        {
            setRootNode(new RootNode(this));
            CreateMetadata(title, id, userProfile);
            AddChannel(TEXT_CHANNEL_NAME);
            if (createTitleSection) CreateTitleSection(title);
            UndoRedoManager.CommandExecuted += new Obi.Commands.UndoRedoEventHandler(undo_CommandExecuted);
            UndoRedoManager.CommandUnexecuted += new Obi.Commands.UndoRedoEventHandler(undo_CommandUnexecuted);
        }

        void undo_CommandExecuted(object sender, Obi.Commands.UndoRedoEventArgs e)
        {
            if (CommandExecuted != null) CommandExecuted(this, e);
        }

        void undo_CommandUnexecuted(object sender, Obi.Commands.UndoRedoEventArgs e)
        {
            if (CommandUnexecuted != null) CommandUnexecuted(this, e);
        }

        /// <summary>
        /// Get a single metadata item by name; return null if not found.
        /// </summary>
        public urakawa.metadata.Metadata GetFirstMetadataItem(string name)
        {
            IList list = getMetadataList(name);
            return list.Count > 0 ? (urakawa.metadata.Metadata)list[0] : null;
        }

        /// <summary>
        /// Use the Obi namespace URI!
        /// </summary>
        public override string getXukNamespaceUri() { return DataModelFactory.NS; }

        public void RenameSectionNode(SectionNode section, string label)
        {
            section.Label = label;
            if (RenamedSectionNode != null) RenamedSectionNode(this, new SectionNodeEventArgs(section));
        }

        /// <summary>
        /// Set a metadata and ensure that it is the only one; i.e. delete any other occurrence.
        /// </summary>
        public void SetSingleMetadataItem(string name, string content)
        {
            deleteMetadata(name);
            AddMetadata(name, content);
        }




        /// <summary>
        /// Add a new channel with the given name to the presentation's channel manager.
        /// </summary>
        private void AddChannel(string name)
        {
            Channel channel = getChannelFactory().createChannel();
            channel.setName(name);
            getChannelsManager().addChannel(channel);
        }

        /// <summary>
        /// Convenience method to create a new metadata object with a name/value pair.
        /// Skip it if there is no value (the toolkit doesn't like it.)
        /// </summary>
        private urakawa.metadata.Metadata AddMetadata(string name, string value)
        {
            if (value != null)
            {
                urakawa.metadata.Metadata meta = getMetadataFactory().createMetadata();
                meta.setName(name);
                meta.setContent(value);
                appendMetadata(meta);
                return meta;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Create the XUK metadata for the project from the project settings and the user profile.
        /// </summary>
        private void CreateMetadata(string title, string id, UserProfile userProfile)
        {
            SetSingleMetadataItem(Obi.Metadata.DC_TITLE, title);
            SetSingleMetadataItem(Obi.Metadata.DC_PUBLISHER, userProfile.Organization);
            SetSingleMetadataItem(Obi.Metadata.DC_IDENTIFIER, id);
            SetSingleMetadataItem(Obi.Metadata.DC_LANGUAGE, userProfile.Culture.ToString());
            SetSingleMetadataItem(Obi.Metadata.DTB_NARRATOR, userProfile.Name);
            SetSingleMetadataItem(Obi.Metadata.DTB_GENERATOR, DataModelFactory.Generator);
            SetSingleMetadataItem(Obi.Metadata.OBI_XUK_VERSION, DataModelFactory.XUK_VERSION);
        }

        // Create a new phrase node from an audio media.
        private PhraseNode CreatePhraseNode(urakawa.media.data.audio.ManagedAudioMedia audio)
        {
            PhraseNode node = CreatePhraseNode();
            node.Audio = audio;
            return node;
        }

        // Create a title section with a string as its title.
        private void CreateTitleSection(string title)
        {
            SectionNode node = CreateSectionNode();
            node.Label = title;
            RootNode.AppendChild(node);
        }

        /// <summary>
        /// Access a channel which we know exist and is the only channel by this name.
        /// </summary>
        /// <param name="name">The name of the channel (use the name constants.)</param>
        /// <returns>The channel for this name.</returns>
        /// <exception cref="urakawa.exception.ChannelDoesNotExistException">Thrown when there is no channel by that name.</exception>
        /// <exception cref="TooManyChannelsException">Thrown when there are more than one channels with that name.</exception>
        private Channel GetSingleChannelByName(string name)
        {
            List<Channel> channels = getChannelsManager().getListOfChannels(name);
            if (channels.Count == 0) throw new Exception(String.Format("No channel named \"{0}\"", name));
            if (channels.Count > 1) throw new Exception(String.Format("Expected 1 channel for {0}, got {1}.",
                name, channels.Count));
            return channels[0];
        }

        // Create a media object from a sound file.
        private ManagedAudioMedia ImportAudioFromFile(string path)
        {
            if (!DataManager.getEnforceSinglePCMFormat())
            {
                Stream input = File.OpenRead(path);
                PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader(input);
                input.Close();
                getMediaDataManager().getDefaultPCMFormat().setBitDepth(info.getBitDepth());
                getMediaDataManager().getDefaultPCMFormat().setNumberOfChannels(info.getNumberOfChannels());
                getMediaDataManager().getDefaultPCMFormat().setSampleRate(info.getSampleRate());
                DataManager.setEnforceSinglePCMFormat(true);
            }
            AudioMediaData data = (AudioMediaData)getMediaDataFactory().createMediaData(typeof(AudioMediaData));
            data.appendAudioDataFromRiffWave(path);
            ManagedAudioMedia media = (ManagedAudioMedia)getMediaFactory().createAudioMedia();
            media.setMediaData(data);
            return media;
        }
    }

    public class SectionNodeEventArgs : EventArgs
    {
        private SectionNode mNode;
        public SectionNodeEventArgs(SectionNode node) : base() { mNode = node; }
        public SectionNode Node { get { return mNode; } }
    }

    public delegate void SectionNodeEventHandler(object sender, SectionNodeEventArgs e);
}
