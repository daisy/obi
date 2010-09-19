using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using urakawa.command;
using urakawa.media.data.audio;
using urakawa.property.channel;
using urakawa.publish;
using urakawa.media.data;

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


        public event NodeEventHandler<SectionNode> RenamedSectionNode;  // triggered after a section was renamed
        public event NodeEventHandler<ObiNode> UsedStatusChanged;       // triggered after a node used status changed
        public event CustomClassEventHandler CustomClassAddded;         // triggered after a custom class was added
        public event CustomClassEventHandler CustomClassRemoved;        // triggered after a custom class was removed
        public event MetadataEventHandler MetadataEntryAdded;           // triggered after a metadata entry was added
        public event MetadataEventHandler MetadataEntryDeleted;         // triggered after a metadata entry was deleted
        public event MetadataEventHandler MetadataEntryContentChanged;  // triggered after the content of a metadata entry changed
        public event MetadataEventHandler MetadataEntryNameChanged;     // triggered after the name of a metadata entry changed

        public event EventHandler<urakawa.events.command.CommandEventArgs> BeforeCommandExecuted;


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
                mCustomClasses.Add(customClass, new List<EmptyNode>(1));
                if (CustomClassAddded != null) CustomClassAddded(this, new CustomClassEventArgs(customClass));
            }
            mCustomClasses[customClass].Add(node);
        }

        /// <summary>
        /// Create a new composite command with the given label.
        /// </summary>
        public CompositeCommand CreateCompositeCommand(string label)
        {
            CompositeCommand command = getCommandFactory().createCompositeCommand();
            command.setShortDescription(label);
            return command;
        }

        /// <summary>
        /// Execute a command, but warn first.
        /// </summary>
        public void Do(ICommand command)
        {
            if (command != null)
            {
                if (BeforeCommandExecuted != null)
                {
                    BeforeCommandExecuted(this, new urakawa.events.command.CommandEventArgs(command));
                }
                getUndoRedoManager().execute(command);
            }
        }

        /// <summary>
        /// Remove a custom class from the list.
        /// </summary>
        public void RemoveCustomClass(string customClass, EmptyNode node)
        {
            if (mCustomClasses.ContainsKey(customClass))
            {
                if (node != null) mCustomClasses[customClass].Remove(node);
                if (mCustomClasses[customClass].Count == 0
                    ||    ( mCustomClasses[customClass].Count  == 1  &&  mCustomClasses[customClass] [0] == null )) // condition appended because custom class without an empty node is initialised with null entry
                {
                    mCustomClasses.Remove(customClass);
                    if (CustomClassRemoved != null) CustomClassRemoved(this, new CustomClassEventArgs(customClass));
                }
            }
        }

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
        /// Creates list of phrases from file being imported. Split by the duration parameter, or not if 0.
        /// </summary>
        public List<PhraseNode> CreatePhraseNodeList(string path, double durationMs)
        {
            List<PhraseNode> PhraseList = new List<PhraseNode>();
            List<ManagedAudioMedia> MediaList = ImportAudioFromFile(path, durationMs);
            foreach (ManagedAudioMedia m in MediaList) PhraseList.Add(CreatePhraseNode(m));
            return PhraseList;
        }


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
            foreach (urakawa.metadata.Metadata entry in getListOfMetadata(name)) DeleteMetadata(entry);
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
                AddMetadata(meta);
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
            SetSingleMetadataItem ( Obi.Metadata.DC_CREATOR, Localizer.Message("Metadata_CreatorNameForInitializing"));//it is important for DAISY 2.02
            SetSingleMetadataItem(Obi.Metadata.DTB_NARRATOR, userProfile.Name);
            SetSingleMetadataItem ( Obi.Metadata.GENERATOR, DataModelFactory.Generator );
            SetSingleMetadataItem ( Obi.Metadata.OBI_XUK_VERSION, DataModelFactory.XUK_VERSION );
        }

        // Create a title section with a string as its title.
        private void CreateTitleSection(string title)
        {
            SectionNode node = CreateSectionNode();
            node.Label = title;
            RootNode.AppendChild(node);
        }

        // Access a channel which we know exist and is the only channel by this name.
        internal Channel GetSingleChannelByName(string name)
        {
            List<Channel> channels = getChannelsManager().getListOfChannels(name);
            if (channels.Count == 0) throw new Exception(String.Format("No channel named \"{0}\"", name));
            if (channels.Count > 1) throw new Exception(String.Format("Expected 1 channel for {0}, got {1}.",
                name, channels.Count));
            return channels[0];
        }

        // Create a media object from a sound file.
        private ManagedAudioMedia ImportAudioFromFile ( string path )
            {
            string dataProviderDirectory = ((urakawa.media.data.FileDataProviderManager)this.getDataProviderManager ()).getDataFileDirectoryFullPath ();

            if (!getMediaDataManager ().getEnforceSinglePCMFormat ())
                {
                Stream input = File.OpenRead ( path );
                PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader ( input );
                input.Close ();
                DataManager.setDefaultBitDepth ( info.getBitDepth () );
                DataManager.setDefaultNumberOfChannels ( info.getNumberOfChannels () );
                DataManager.setDefaultSampleRate ( info.getSampleRate () );
                DataManager.setEnforceSinglePCMFormat ( true );
                }

            AudioMediaData data = getMediaDataFactory ().createAudioMediaData ();

            if (path.StartsWith ( dataProviderDirectory ))
                {
                FileDataProvider dataProv = (FileDataProvider)this.getDataProviderFactory ().createDataProvider ( FileDataProviderFactory.AUDIO_WAV_MIME_TYPE );
                dataProv.InitByMovingExistingFile ( path );
                data.AppendPcmData ( dataProv );
                }
            else
                {
                data.appendAudioDataFromRiffWave ( path );
                }

            ManagedAudioMedia media = (ManagedAudioMedia)getMediaFactory ().createAudioMedia ();
            media.setMediaData ( data );
            return media;
            }

        // Create a list of ManagedAudioMedia from audio file being imported
        // Split by duration, unless 0 or less.
        private List<ManagedAudioMedia> ImportAudioFromFile(string path, double durationMs)
        {
            ManagedAudioMedia media = ImportAudioFromFile(path);
            double totalDuration = media.getDuration().getTimeDeltaAsMillisecondFloat();
            // if duration is 0 or less, just one phrase
            int phrases = durationMs <= 0.0 ? 1 : (int)Math.Floor(totalDuration / durationMs);
            double lastPhraseBegin = phrases * durationMs;
            double remaining = totalDuration - lastPhraseBegin;
            if (remaining < durationMs / 10.0)
            {
                lastPhraseBegin -= durationMs;
            }
            else
            {
                ++phrases;
            }
            List<ManagedAudioMedia> audioMediaList = new List<ManagedAudioMedia>(phrases);
            for (double time = lastPhraseBegin; time > 0.0; time -= durationMs)
            {
                audioMediaList.Insert(0, media.split(new urakawa.media.timing.Time(time)));
            }
            audioMediaList.Insert(0, media);
            return audioMediaList;
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

        /// <summary>
        /// Create a list of phrase nodes from a list of audio assets.
        /// </summary>
        public List <PhraseNode> CreatePhraseNodesFromAudioAssetList(List<ManagedAudioMedia> AssetList)
        {
            List<PhraseNode> PhraseList = new List<PhraseNode>();
            for (int i = 0; i < AssetList.Count; i++)
            {
                PhraseList.Add(CreatePhraseNode(AssetList[i]));
            }
            return PhraseList;
        }

        /// <summary>
        /// During recording of a phrase, its audio may/should be updated.
        /// </summary>
        public void UpdateAudioForPhrase(EmptyNode phrase, ManagedAudioMedia media)
        {
            ((PhraseNode)phrase).Audio = media;
        }

        /// <summary>
        /// Export the project as DAISY to an export directory.
        /// </summary>
        public void ExportToZ(string exportPath, string xukPath, Export.ExportFormat format ,int audioFileSectionLevel )
        {
            UpdatePublicationMetadata();
            TreeNodeTestDelegate nodeIsSection = delegate(urakawa.core.TreeNode node) { return node is SectionNode   &&   ((SectionNode)node).Level <= audioFileSectionLevel ; };
            TreeNodeTestDelegate nodeIsUnused = delegate(urakawa.core.TreeNode node) { return !((ObiNode)node).Used; };

            RemoveAllPublishChannels (); // remove any publish channel, in case they exist
            PublishManagedAudioVisitor visitor = new PublishManagedAudioVisitor(nodeIsSection, nodeIsUnused);
            Channel publishChannel = AddChannel(Presentation.PUBLISH_AUDIO_CHANNEL_NAME);
            visitor.setDestinationChannel(publishChannel);
            visitor.setSourceChannel(AudioChannel);
            visitor.setDestinationDirectory(new Uri(exportPath));
            RootNode.acceptDepthFirst(visitor);
            // TODO check that there is an audio file to write
            visitor.writeAndCloseCurrentAudioFile();

            if (format == Obi.Export.ExportFormat.DAISY3_0)
                {
                ConvertXukToZed ( exportPath, xukPath, XukString );
                }
            else
                {
                Export.DAISY202Export export202 = new Obi.Export.DAISY202Export ( this, exportPath );
                export202.CreateDAISY202Files ();
                }
            getChannelsManager().removeChannel(publishChannel);
        }
 
        // Update metadata before exporting
        private void UpdatePublicationMetadata()
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            urakawa.metadata.Metadata producedDate = GetFirstMetadataItem(Metadata.DTB_PRODUCED_DATE);
            urakawa.metadata.Metadata revision = GetFirstMetadataItem(Metadata.DTB_REVISION);
            urakawa.metadata.Metadata revisionDate = GetFirstMetadataItem(Metadata.DTB_REVISION_DATE);
            if (producedDate == null)
            {
                System.Diagnostics.Debug.Assert(revisionDate == null && revision == null);
                SetSingleMetadataItem(Metadata.DTB_PRODUCED_DATE, date);
            }
            else
            {
                if (revision != null)
                {
                    System.Diagnostics.Debug.Assert(revisionDate != null);
                    int rev = Int32.Parse(revision.getContent()) + 1;
                    SetMetadataEntryContent(revision, rev.ToString());
                    SetMetadataEntryContent(revisionDate, date);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(revisionDate == null);
                    SetSingleMetadataItem(Metadata.DTB_REVISION, "1");
                    SetSingleMetadataItem(Metadata.DTB_REVISION_DATE, date);
                }
            }
        }

        /// <summary>
        /// Remove all publish channels. It is useful for removing uncleared published channels that could not be removed due to export faliure.
        /// </summary>
        public void RemoveAllPublishChannels ()
            {
            List<Channel> channels = getChannelsManager ().getListOfChannels ( PUBLISH_AUDIO_CHANNEL_NAME );
            if (channels != null && channels.Count > 0)
                {
                for (int i = channels.Count - 1; i >= 0; i--)
                    {
                    getChannelsManager ().removeChannel ( channels[i] );
                    }
                }

            }

        /// <summary>
        /// XUK as a string.
        /// </summary>
        public string XukString
        {
            get
            {
                StringBuilder strBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                XmlWriter writer = XmlWriter.Create(strBuilder, settings);

                writer.WriteStartDocument();
                
                //This local XukString property should really only serialize the current context (i.e. Presentation):
                //          xukOut(writer, getRootUri(), null);
                //...but in order to remain compatible with the SaveXukAction mechanism (see code lines below),
                //we serialize the whole Project.
                //          urakawa.xuk.SaveXukAction action = new urakawa.xuk.SaveXukAction(null, getProject(), memstream);
                //          action.execute();
                //TODO: decide whether the whole Project XML needs to be in the result string, or just the Presentation
                
                getProject().xukOut(writer, getRootUri(), null);

                writer.WriteEndDocument();
                writer.Close();
                
                return strBuilder.ToString();
            }
        }

        // Convert the XUK output of the project to Z.
        private void ConvertXukToZed(string outputDir, string xukPath, string exported)
        {
            Export.Z z = new Export.Z();
            z.ExportPath = outputDir;
            z.ProjectPath = Path.GetDirectoryName(xukPath);
            z.ElapsedTimes = ElapsedTimes;
            System.Xml.XmlReader xuk = System.Xml.XmlReader.Create(new StringReader(exported));
            z.WriteFileset(xuk);
            // Write out the filtered file that is transformed for debugging.
            z.WriteFiltered(System.Xml.XmlReader.Create(new StringReader(exported)));
        }


        /// <summary>
        /// Get the page number for this node. If the node already has a page number, return it.
        /// If it has no page number, find the nearest preceding block with a page number and add one.
        /// </summary>
        public PageNumber PageNumberFor(ObiNode node)
        {
            if (node is EmptyNode && ((EmptyNode)node).Role_ == EmptyNode.Role.Page)
            {
                return ((EmptyNode)node).PageNumber;
            }
            else
            {
                ObiNode n = node.PrecedingNode;
                while (n != null && !(n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)) n = n.PrecedingNode;
                return n != null ? ((EmptyNode)n).PageNumber.NextPageNumber() : new PageNumber(1);
            }
        }

        /// <summary>
        /// Find the page number following the one for this node. If the node doesn't have a number,
        /// find the first preceding node that has one.
        /// </summary>
        public PageNumber PageNumberFollowing(ObiNode node)
        {
            while (node != null && !(node is EmptyNode && ((EmptyNode)node).Role_ == EmptyNode.Role.Page))
            {
                node = node.PrecedingNode;
            }
            return node != null ? ((EmptyNode)node).PageNumber.NextPageNumber() : new PageNumber(1);
        }

        /// <summary>
        /// Update the audio properties of the presentation, if possible. Return true on success.
        /// </summary>
        public bool UpdatePresentationAudioProperties(int channels, int bitDepth, int sampleRate)
        {
            try
            {
                DataManager.setDefaultNumberOfChannels((ushort)channels);
                DataManager.setDefaultBitDepth((ushort)bitDepth);
                DataManager.setDefaultSampleRate((uint)sampleRate);
                DataManager.setEnforceSinglePCMFormat(true);
                return true;
            }
            catch
            {
                return false;
            }
        }        

        // Get the elapsed time at the beginning of each section
        // (last section has the total time.)
        // Unused sections and phrases are skipped.
        private List<double> ElapsedTimes
        {
            get
            {
                List<double> times = new List<double>();
                double time = 0.0;
                RootNode.acceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                        if (n is ObiNode && !((ObiNode)n).Used) return false;
                        if (n is PhraseNode) time += ((PhraseNode)n).Duration;
                        else if (n is SectionNode && ((SectionNode)n).FirstUsedPhrase != null) times.Add(time);
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
                times.Add(time);
                return times;
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
