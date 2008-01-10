using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using urakawa.property.channel;
using urakawa.media.data.audio;
using urakawa.publish;
using System.Xml;

namespace Obi
{
    public class Presentation: urakawa.Presentation
    {
        private bool mInitialized;                                   // initialization flag
        private Dictionary<string, List<EmptyNode>> mCustomClasses;  // custom classes and which nodes have them

        /// <summary>
        /// Create an uninitialized presentation.
        /// </summary>
        public Presentation() : base() 
        {
            mInitialized = false;
            mCustomClasses = new Dictionary<string, List<EmptyNode>>();
        }


        public static readonly string AUDIO_CHANNEL_NAME = "obi.audio";  // canonical name of the audio channel
        public static readonly string TEXT_CHANNEL_NAME = "obi.text";    // canonical name of the text channel
        public static readonly string PUBLISH_AUDIO_CHANNEL_NAME = "obi.publish.audio"; //canonical name of the published audio channel


        public event Commands.UndoRedoEventHandler CommandExecuted;     // triggered when a command was executed
        public event Commands.UndoRedoEventHandler CommandUnexecuted;   // triggered when a command was unexecuted
        public event NodeEventHandler<SectionNode> RenamedSectionNode;  // triggered after a section was renamed
        public event NodeEventHandler<ObiNode> UsedStatusChanged;       // triggered after a node used status changed
        public event CustomClassEventHandler CustomClassAddded;         // triggered after a custom class was added
        public event CustomClassEventHandler CustomClassRemoved;        // triggered after a custom class was removed
        public event MetadataEventHandler MetadataEntryAdded;           // triggered after a metadata entry was added
        public event MetadataEventHandler MetadataEntryDeleted;         // triggered after a metadata entry was deleted
        public event MetadataEventHandler MetadataEntryContentChanged;  // triggered after the content of a metadata entry changed
        public event MetadataEventHandler MetadataEntryNameChanged;     // triggered after the name of a metadata entry changed


        /// <summary>
        /// Add a new metadata entry (with event.)
        /// </summary>
        public void AddMetadata(urakawa.metadata.Metadata entry)
        {
            addMetadata(entry);
            if (MetadataEntryAdded != null) MetadataEntryAdded(this, new MetadataEventArgs(entry));
        }

        /// <summary>
        /// Delete a metadata entry (with event.)
        /// </summary>
        public void DeleteMetadata(urakawa.metadata.Metadata entry)
        {
            deleteMetadata(entry);
            if (MetadataEntryDeleted != null) MetadataEntryDeleted(this, new MetadataEventArgs(entry));
        }

        public void SetMetadataEntryContent(urakawa.metadata.Metadata entry, string content)
        {
            entry.setContent(content);
            if (MetadataEntryContentChanged != null) MetadataEntryContentChanged(this, new MetadataEventArgs(entry));
        }

        public void SetMetadataEntryName(urakawa.metadata.Metadata entry, string name)
        {
            entry.setName(name);
            if (MetadataEntryNameChanged != null) MetadataEntryNameChanged(this, new MetadataEventArgs(entry));
        }

        /// <summary>
        /// The media data manager for the project.
        /// </summary>
        public Audio.DataManager DataManager { get { return (Audio.DataManager)getMediaDataManager(); } }

        /// <summary>
        /// Root node of the presentation.
        /// </summary>
        public RootNode RootNode { get { return (RootNode)getRootNode(); } }


        /// <summary>
        /// Get the audio channel of the presentation
        /// </summary>
        public Channel AudioChannel { get { return GetSingleChannelByName(AUDIO_CHANNEL_NAME); } }

        /// <summary>
        /// First section node in the project, or null if there are no sections.
        /// </summary>
        public SectionNode FirstSection
        {
            get { return RootNode.SectionChildCount > 0 ? RootNode.SectionChild(0) : null; }
        }

        /// <summary>
        /// Get the last section node in the project or null if there are no sections.
        /// </summary>
        public SectionNode LastSection
        {
            get
            {
                SectionNode last = RootNode.SectionChildCount > 0 ? RootNode.SectionChild(-1) : null;
                while (last != null && last.SectionChildCount > 0) last = last.SectionChild(-1);
                return last;
            }
        }

        /// <summary>
        /// Get the text channel of the presentation.
        /// </summary>
        public Channel TextChannel { get { return GetSingleChannelByName(TEXT_CHANNEL_NAME); } }

        /// <summary>
        /// Get the title of the presentation from the metadata.
        /// </summary>
        public string Title { get { return GetFirstMetadataItem(Metadata.DC_TITLE).getContent(); } }

        /// <summary>
        /// Get a list of the custom classes that have been defined by the user for this presentation
        /// </summary>
        public Dictionary<string, List<EmptyNode>>.KeyCollection CustomClasses { get { return mCustomClasses.Keys; } }

        /// <summary>
        /// Get all the nodes of given custom class.
        /// </summary>
        public List<EmptyNode> NodesForCustomClass(string customClass)
        {
            return mCustomClasses.ContainsKey(customClass) ? mCustomClasses[customClass] : new List<EmptyNode>();
        }

        /// <summary>
        /// Add a custom class to the list.  Duplicates are filtered out.
        /// </summary>
        public void AddCustomClass(string customClass, EmptyNode node)
        {
            if (customClass == null || customClass == "") return;
            if (!mCustomClasses.ContainsKey(customClass))
            {
                if (node != null) mCustomClasses.Add(customClass, new List<EmptyNode>());
                if (CustomClassAddded != null) CustomClassAddded(this, new CustomClassEventArgs(customClass));
            }
            mCustomClasses[customClass].Add(node);
        }

        /// <summary>
        /// Create a new composite command with the given label.
        /// </summary>
        public urakawa.undo.CompositeCommand CreateCompositeCommand(string label)
        {
            urakawa.undo.CompositeCommand command = getCommandFactory().createCompositeCommand();
            command.setShortDescription(label);
            return command;
        }

        /// <summary>
        /// Remove a custom class from the list.
        /// </summary>
        public void RemoveCustomClass(string customClass, EmptyNode node)
        {
            if (mCustomClasses.ContainsKey(customClass))
            {
                if (node != null) mCustomClasses[customClass].Remove(node);
                if (mCustomClasses[customClass].Count == 0)
                {
                    mCustomClasses.Remove(customClass);
                    if (CustomClassRemoved != null) CustomClassRemoved(this, new CustomClassEventArgs(customClass));
                }
            }
        }

        /// <summary>
        /// The Undo/redo manager for this presentation (with the correct type.)
        /// </summary>
        public Commands.UndoRedoManager UndoRedoManager { get { return (Commands.UndoRedoManager)getUndoRedoManager(); } }


        /// <summary>
        /// Create a phrase node belonging to this presentation.
        /// </summary>
        public PhraseNode CreatePhraseNode()
        {
            PhraseNode node = (PhraseNode)getTreeNodeFactory().createNode(PhraseNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
            node.addProperty(getPropertyFactory().createChannelsProperty());
            return node;
        }

        /// <summary>
        /// Convenience method to create a new plain phrase node from a file.
        /// </summary>
        public PhraseNode CreatePhraseNode(string path) { return CreatePhraseNode(ImportAudioFromFile(path)); }

        /// <summary>
        /// Create a new phrase node from an audio media.
        /// </summary>
        public PhraseNode CreatePhraseNode(ManagedAudioMedia audio)
        {
            PhraseNode node = CreatePhraseNode();
            node.Audio = audio;
            return node;
        }


        /// <summary>
        /// Create a section node belonging to this presentation.
        /// </summary>
        public SectionNode CreateSectionNode()
        {
            SectionNode node = (SectionNode)getTreeNodeFactory().createNode(SectionNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
            urakawa.property.channel.ChannelsProperty channelsProperty = getPropertyFactory().createChannelsProperty();
            node.addProperty(channelsProperty);
            // Create the text media object for the label with a default label
            urakawa.media.ITextMedia labelMedia = getMediaFactory().createTextMedia();
            labelMedia.setText(Localizer.Message("default_section_label"));
            channelsProperty.setMedia(TextChannel, labelMedia);
            return node;
        }

        /// <summary>
        /// Initialize event handling for a new presentation.
        /// </summary>
        public void Initialize(Session session)
        {
            mInitialized = true;
            Uri uri = new Uri(session.Path);
            if (getRootUri() != uri) setRootUri(uri);
            UndoRedoManager.CommandExecuted += new Obi.Commands.UndoRedoEventHandler(undo_CommandExecuted);
            UndoRedoManager.CommandUnexecuted += new Obi.Commands.UndoRedoEventHandler(undo_CommandUnexecuted);
        }

        /// <summary>
        /// Initialize the metadata of the presentation, and create a title section if necessary.
        /// </summary>
        public void Initialize(Session session, string title, bool createTitleSection, string id, Settings settings)
        {
            Initialize(session);
            setRootNode(new RootNode(this));
            CreateMetadata(title, id, settings.UserProfile);
            AddChannel(AUDIO_CHANNEL_NAME);
            AddChannel(TEXT_CHANNEL_NAME);
            if (createTitleSection) CreateTitleSection(title);
            DataManager.setDefaultBitDepth((ushort)settings.BitDepth);
            DataManager.setDefaultNumberOfChannels((ushort)settings.AudioChannels);
            DataManager.setDefaultSampleRate((uint)settings.SampleRate);
            DataManager.setEnforceSinglePCMFormat(false);
        }

        /// <summary>
        /// True if the presentation was initialized.
        /// </summary>
        public bool Initialized { get { return mInitialized; } }

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
            List<urakawa.metadata.Metadata> list = getListOfMetadata(name);
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// Use the Obi namespace URI!
        /// </summary>
        public override string getXukNamespaceUri() { return DataModelFactory.NS; }

        public void RenameSectionNode(SectionNode section, string label)
        {
            section.Label = label;
            if (RenamedSectionNode != null) RenamedSectionNode(this, new NodeEventArgs<SectionNode>(section));
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
        /// Signal that the used status of a node has changed.
        /// </summary>
        public void SignalUsedStatusChanged(ObiNode node)
        {
            if (UsedStatusChanged != null) UsedStatusChanged(this, new NodeEventArgs<ObiNode>(node));
        }




        /// <summary>
        /// Add a new channel with the given name to the presentation's channel manager.
        /// </summary>
        private Channel AddChannel(string name)
        {
            Channel channel = getChannelFactory().createChannel();
            channel.setName(name);
            getChannelsManager().addChannel(channel);
            return channel;
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
                addMetadata(meta);
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

        // Create a title section with a string as its title.
        private void CreateTitleSection(string title)
        {
            SectionNode node = CreateSectionNode();
            node.Label = title;
            RootNode.AppendChild(node);
        }

        // Access a channel which we know exist and is the only channel by this name.
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
            if (!getMediaDataManager().getEnforceSinglePCMFormat())
            {
                Stream input = File.OpenRead(path);
                PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader(input);
                input.Close();
                DataManager.setDefaultBitDepth(info.getBitDepth());
                DataManager.setDefaultNumberOfChannels(info.getNumberOfChannels());
                DataManager.setDefaultSampleRate(info.getSampleRate());
                DataManager.setEnforceSinglePCMFormat(true);
            }
            AudioMediaData data = getMediaDataFactory().createAudioMediaData();
            data.appendAudioDataFromRiffWave(path);
            ManagedAudioMedia media = (ManagedAudioMedia)getMediaFactory().createAudioMedia();
            media.setMediaData(data);
            return media;
        }

        /// <summary>
        /// A new phrase is being recorded, so create and return it.
        /// </summary>
        public PhraseNode StartRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = CreatePhraseNode(e.Audio);
            parent.Insert(phrase, index);
            return phrase;
        }

        public List <PhraseNode> CreatePhraseNodesFromAudioAssetList(List<ManagedAudioMedia> AssetList)
        {
            List<PhraseNode> PhraseList = new List<PhraseNode> () ;

            for (int i = 0; i < AssetList.Count; i++)
            {
                PhraseList.Add ( CreatePhraseNode(AssetList[i]) ) ;
            }
            return PhraseList ;
        }

        /// <summary>
        /// During recording of a phrase, its audio may/should be updated.
        /// </summary>
        public void UpdateAudioForPhrase(EmptyNode phrase, ManagedAudioMedia media)
        {
            ((PhraseNode)phrase).Audio = media;
        }

        /// <summary>
        /// Export the project as DAISY to a directory.
        /// </summary>
        public void ExportToZed(Uri destinationDirectory)
        {
            TreeNodeTestDelegate nodeIsSection = delegate(urakawa.core.TreeNode node) { return node is SectionNode; };
            TreeNodeTestDelegate nodeIsUnused = delegate(urakawa.core.TreeNode node) { return !((ObiNode)node).Used; };
            PublishManagedAudioVisitor visitor = new PublishManagedAudioVisitor(nodeIsSection, nodeIsUnused);
            Channel publishChannel = AddChannel(Presentation.PUBLISH_AUDIO_CHANNEL_NAME);
            visitor.setDestinationChannel(publishChannel);
            visitor.setSourceChannel(AudioChannel);
            visitor.setDestinationDirectory(destinationDirectory);
            RootNode.acceptDepthFirst(visitor);
            visitor.writeCurrentAudioFile();
            XmlWriter writer = XmlWriter.Create(new System.Text.StringBuilder());
            getProject().saveXUK(writer, null);
            writer.Close();
            getChannelsManager().removeChannel(publishChannel);
        }

        /// <summary>
        /// Get the page number for this node. If the node already has a page number, return it.
        /// If it has no page number, find the nearest preceding block with a page number and add one.
        /// </summary>
        public int PageNumberFor(ObiNode node)
        {
            if (node is EmptyNode && ((EmptyNode)node).NodeKind == EmptyNode.Kind.Page)
            {
                return ((EmptyNode)node).PageNumber;
            }
            else
            {
                ObiNode n = node.PrecedingNode;
                while (n != null && !(n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)) n = n.PrecedingNode;
                return n != null ? ((EmptyNode)n).PageNumber + 1 : 1;
            }
        }
    }

    public class NodeEventArgs<T> : EventArgs
    {
        private T mNode;
        public NodeEventArgs(T node): base() { mNode = node; }
        public T Node { get { return mNode; } }
    }

    public delegate void NodeEventHandler<T>(object sender, NodeEventArgs<T> e);

    public class CustomClassEventArgs : EventArgs
    {
        private string mCustomClass;
        public CustomClassEventArgs(string customClass) : base() { mCustomClass = customClass; }
        public string CustomClass { get { return mCustomClass; } }
    }

    public delegate void CustomClassEventHandler(object sender, CustomClassEventArgs e);

    public class MetadataEventArgs : EventArgs
    {
        private urakawa.metadata.Metadata mEntry;
        public MetadataEventArgs(urakawa.metadata.Metadata entry) : base() { mEntry = entry; }
        public urakawa.metadata.Metadata Entry { get { return mEntry; } }
    }

    public delegate void MetadataEventHandler(object sender, MetadataEventArgs e);
}
