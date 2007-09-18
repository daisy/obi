using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa;
using urakawa.core;
using urakawa.core.events;
using urakawa.media;
using urakawa.media.data;
using urakawa.property.channel;
using urakawa.undo;

namespace Obi
{
    public partial class Project : urakawa.Project
    {
        private int mPageCount;              // count the pages in the book
        private int mPhraseCount;            // total number of phrases in the project
        private UndoRedoManager mUndoMngr;   // undo/redo manager for this project
        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        
        private string mLastPath;            // last path to which the project was saved (see save as)
        
        // TODO remove mAudioChannel, mAnnotation and mClipboard as members.
        private Clipboard mClipboard;        // project-wide clipboard; should move to project panel

        public static readonly string CURRENT_XUK_VERSION = "obi-xuk-012";         // version of the Obi/XUK file
        public static readonly string AUDIO_CHANNEL_NAME = "obi.audio";            // canonical name of the audio channel
        public static readonly string TEXT_CHANNEL_NAME = "obi.text";              // canonical name of the text channel
        public static readonly string ANNOTATION_CHANNEL_NAME = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager
        public event Events.SetMediaHandler MediaSet;                       // a media object was set on a node
        public event Events.NodeEventHandler TouchedNode;                   // this node was somehow modified
        public event Events.ObiNodeHandler ToggledNodeUsedState;            // the used state of a node was toggled.
        public event Events.SectionNodeHeadingHandler HeadingChanged;       // the heading of a section changed.


        /// <summary>
        /// Create a new empty project.
        /// </summary>
        /// <param name="XUKPath">The path to the XUK file where the project is to be saved.</param>
        public Project(string XUKPath): base(CreatePresentation(XUKPath))
        {
            mClipboard = new Clipboard();  // TODO move this to project panel
            mPhraseCount = 0;
            mPageCount = 0;
            mUndoMngr = new UndoRedoManager();
            mUnsaved = true;
            mXUKPath = XUKPath;
            Presentation presentation = getPresentation();
            ((ObiNodeFactory)presentation.getTreeNodeFactory()).Project = this;
            presentation.setRootNode(presentation.getTreeNodeFactory().createNode(Obi.RootNode.XUK_ELEMENT_NAME,
                Program.OBI_NS));
            presentation.treeNodeAdded += new TreeNodeAddedEventHandler(presentation_treeNodeAdded);
            presentation.treeNodeRemoved += new TreeNodeRemovedEventHandler(presentation_treeNodeRemoved);
            AddChannel(ANNOTATION_CHANNEL_NAME);
            AddChannel(AUDIO_CHANNEL_NAME);
            AddChannel(TEXT_CHANNEL_NAME);
        }


        /// <summary>
        /// Get the annotation channel of the project.
        /// </summary>
        public Channel AnnotationChannel { get { return GetSingleChannelByName(ANNOTATION_CHANNEL_NAME); } }

        /// <summary>
        /// Get the audio channel of the project.
        /// </summary>
        public Channel AudioChannel { get { return GetSingleChannelByName(AUDIO_CHANNEL_NAME); } }

        /// <summary>
        /// Number of audio channels for the project audio.
        /// </summary>
        public int AudioChannels
        {
            get { return getPresentation().getMediaDataManager().getDefaultPCMFormat().getNumberOfChannels(); }
        }

        /// <summary>
        /// Bit depth for the project audio.
        /// </summary>
        public int BitDepth
        {
            get { return getPresentation().getMediaDataManager().getDefaultPCMFormat().getBitDepth(); }
        }

        /// <summary>
        /// Close the project.
        /// </summary>
        public void Close()
        {
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Closed));
            }
        }

        /// <summary>
        /// The media data manager for the project.
        /// </summary>
        public Audio.DataManager DataManager { get { return (Audio.DataManager)getPresentation().getMediaDataManager(); } }

        /// <summary>
        /// Initialize a new project with metadata.
        /// </summary>
        /// <param name="XUKPath">The path to the XUK file where the project is to be saved.</param>
        /// <param name="title">The title of the project.</param>
        /// <param name="id">The identifier for the project.</param>
        /// <param name="userProfile">The user profile for the user creating the project.</param>
        /// <param name="createTitle">If true, create an initial title section.</param>
        public void Initialize(string title, string id, UserProfile userProfile, bool createTitle)
        {
            CreateMetadata(title, id, userProfile);
            // TODO remove this
            mClipboard = new Clipboard();
            if (createTitle) CreateTitleSection(title);
            if (StateChanged != null) StateChanged(this,
                new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            Save();
        }

        /// <summary>
        /// Delete a node from the tree.
        /// </summary>
        /// <param name="node">The node to delete.</param>
        public void DeleteNode(ObiNode node)
        {
            node.detach();
            Modified();
        }

        /// <summary>
        /// First section node in the project, or null if there are no sections.
        /// </summary>
        public SectionNode FirstSection
        {
            get { return RootNode.getChildCount() > 0 ? RootNode.SectionChild(0) : null; }
        }

        /// <summary>
        /// If there is audio in the projects, the audio settings should come from the project
        /// and not from the user settings.
        /// </summary>
        public bool HasAudioSettings
        {
            get { return mPhraseCount > 0; }
        }

        /// <summary>
        /// Get the last section node in the project or null if there are no sections.
        /// </summary>
        public SectionNode LastSection
        {
            get
            {
                SectionNode last = RootNode.getChildCount() > 0 ? RootNode.SectionChild(-1) : null;
                while (last != null && last.SectionChildCount > 0) last = last.SectionChild(-1);
                return last;
            }
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// Throw an exception if the file could not be read.
        /// </summary>
        /// <param name="xukPath">The path of the XUK file.</param>
        public void Open(string xukPath)
        {
            openXUK(new Uri(xukPath));
            mUnsaved = false;
            if (XukVersion != CURRENT_XUK_VERSION)
            {
                throw new Exception(String.Format(Localizer.Message("xuk_version_mismatch"),
                    CURRENT_XUK_VERSION, XukVersion));
            }
            mXUKPath = xukPath;
            // getPresentation().setBaseUri(new Uri(Path.GetDirectoryName(xukPath)));
            // TODO: make sure that phrases and pages are counted (should be!)
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            }
        }

        /// <summary>
        /// Get the number of pages in the book.
        /// </summary>
        public int Pages { get { return mPageCount; } }

        /// <summary>
        /// Project has reverted to its initial state (e.g. after all commands have been undone.)
        /// </summary>
        public void Reverted()
        {
            Saved();
        }

        /// <summary>
        /// Get the root node of the presentation as a TreeNode.
        /// </summary>
        public RootNode RootNode
        {
            get { return (RootNode)getPresentation().getRootNode(); }
        }

        /// <summary>
        /// Sample rate for the project audio.
        /// </summary>
        public int SampleRate
        {
            get { return (int)getPresentation().getMediaDataManager().getDefaultPCMFormat().getSampleRate(); }
        }

        /// <summary>
        /// Save the project to its XUK file.
        /// </summary>
        internal void Save()
        {
            bool enforce = DataManager.getEnforceSinglePCMFormat();
            DataManager.setEnforceSinglePCMFormat(true);  // TODO remove this kludge
            saveXUK(new Uri(mXUKPath));
            DataManager.setEnforceSinglePCMFormat(enforce);
            mLastPath = mXUKPath;
            Saved();
        }

        /// <summary>
        /// The project was saved.
        /// </summary>
        private void Saved()
        {
            mUnsaved = false;
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Saved));
            }
        }

        /// <summary>
        /// Get the text channel of the project.
        /// </summary>
        public Channel TextChannel { get { return GetSingleChannelByName(TEXT_CHANNEL_NAME); } }

        /// <summary>
        /// Simulate a modification of the project.
        /// </summary>
        public void Touch()
        {
            Modified(new Commands.Touch(this));
        }

        /// <summary>
        /// This flag is set to true if the project contains modifications that have not been saved.
        /// </summary>
        public bool Unsaved { get { return mUnsaved; } }

        /// <summary>
        /// The path to the XUK file for this project.
        /// </summary>
        public string XUKPath { get { return mXUKPath; } }


        #region utility functions

        /// <summary>
        /// Add a new channel with the given name to the presentation's channel manager.
        /// </summary>
        /// <param name="name">Name of the new channel.</param>
        private void AddChannel(string name)
        {
            Channel channel = getPresentation().getChannelFactory().createChannel();
            channel.setName(name);
            getPresentation().getChannelsManager().addChannel(channel);
        }

        /// <summary>
        /// Create a new phrase node.
        /// </summary>
        private PhraseNode CreatePhraseNode()
        {
            return (PhraseNode)
                getPresentation().getTreeNodeFactory().createNode(PhraseNode.XUK_ELEMENT_NAME, Program.OBI_NS);
        }

        /// <summary>
        /// Create a new phrase node from an audio media.
        /// </summary>
        private PhraseNode CreatePhraseNode( urakawa.media.data.audio.ManagedAudioMedia audio)
        {
            PhraseNode node = CreatePhraseNode();
            node.Audio = audio;
            return node;
        }

        /// <summary>
        /// Create a new empty presentation with custom node and property factories and audio data manager.
        /// </summary>
        /// <param name="XUKPath">Path of the XUK file for the project hosting this presentation.</param>
        /// <returns>The new presentation.</returns>
        private static Presentation CreatePresentation(string XUKPath)
        {
            return new Presentation(new Uri(Path.GetDirectoryName(XUKPath) + Path.DirectorySeparatorChar),
                new ObiNodeFactory(), new PropertyFactory(), 
                null, null, null,
                new Audio.DataManager(), null , null , null , null , null );
                        }

        /// <summary>
        /// Create a section node.
        /// </summary>
        private SectionNode CreateSectionNode()
        {
            return (SectionNode)
                getPresentation().getTreeNodeFactory().createNode(SectionNode.XUK_ELEMENT_NAME, Program.OBI_NS);
        }

        /// <summary>
        /// Create a title section.
        /// </summary>
        /// <param name="title">The title of this section.</param>
        private void CreateTitleSection(string title)
        {
            SectionNode node = CreateSectionNode();
            node.Label = title;
            RootNode.appendChild(node);
        }

        /// <summary>
        /// Get the generator string (Obi/Urakawa SDK) for the project.
        /// </summary>
        /// <remarks>Use the actual assembly name/version string for the toolkit (from 1.0)</remarks>
        private string Generator
        {
            get
            {
                return String.Format("{0} v{1} with {2} v{3} (http://urakawa.sf.net)",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Name,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Version);
            }
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
            List<Channel> channels = getPresentation().getChannelsManager().getListOfChannels(name);
            if (channels.Count == 0) throw new Exception(String.Format("No channel named \"{0}\"", name));
            if (channels.Count > 1) throw new Exception(String.Format("Expected 1 channel for {0}, got {1}.",
                name, channels.Count));
            return channels[0];
        }

        /// <summary>
        /// Project was modified.
        /// </summary>
        private void Modified()
        {
            mUnsaved = true;
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            }
        }

        /// <summary>
        /// Project was modified and a command is issued.
        /// </summary>
        /// <param name="command">The command to issue.</param>
        private void Modified(Commands.Command__OLD__ command)
        {
            if (CommandCreated != null)
            {
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
            Modified();
        }

        #endregion




        /// <summary>
        /// The project-wide clipboard.
        /// </summary>
        /// TODO this needs to go
        public Clipboard Clipboard
        {
            get { return mClipboard; }
        }





        /// <summary>
        /// Last path under which the project was saved (different from the normal path when we save as)
        /// </summary>
        /// TODO review save as
        public string LastPath
        {
            get { return mLastPath; }
        }












        /// <summary>
        /// Save the project to a different name/XUK file.
        /// </summary>
        /// <remarks>TO REVIEW</remarks>
        public void SaveAs(string path)
        {
            throw new Exception("DISABLED RIGHT NOW.");
        }

        /// <summary>
        /// Get the text media of a core node. The result can then be used to get or set the text of a node.
        /// Original comments: A helper function to get the text from the given <see cref="TreeNode"/>.
        /// The text channel which contains the desired text will be named so that we know 
        /// what its purpose is (ie, "DefaultText" or "PrimaryText")
        /// @todo
        /// Otherwise we should use the default, only, or randomly first text channel found.
        /// </summary>
        /// <remarks>This replaces get/setTreeNodeText. E.g. getTreeNodeText(node) = GetTextMedia(node).getText()</remarks>
        /// <remarks>This is taken from TOCPanel, and should probably be a node method;
        /// we would subclass TreeNode fort his.</remarks>
        /// <param name="node">The node which text media we are interested in.</param>
        /// <returns>The text media found, or null if none.</returns>
        public static TextMedia GetTextMedia(TreeNode node)
        {
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel textChannel = Project.GetChannel(node, TEXT_CHANNEL_NAME);
            return textChannel == null ? null : (TextMedia)prop.getMedia(textChannel);
        }

        public static Channel GetChannel(TreeNode node, string name)
        {
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channels = prop.getListOfUsedChannels();
            foreach (object o in channels)
            {
                if (((Channel)o).getName() == name) return (Channel)o;
            }
            return null;
        }


        /// <summary>
        /// Get the media object of a node for the first channel found wit the given name,
        /// or null if no such channel is found.
        /// </summary>
        /// <param name="node">The node for which we want a media object.</param>
        /// <param name="channel">The name of the channel that we are interested in.</param>
        /// <returns>The media object set on the first channel of that name, or null.</returns>
        public static IMedia GetMediaForChannel(TreeNode node, string channel)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel foundChannel;
            List<Channel> channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = (channelsList[i]).getName();
                if (channelName == channel)
                {
                    foundChannel = (Channel)channelsList[i];
                    return channelsProp.getMedia(foundChannel);
                }
            }
            return null;
        }

        /// <summary>
        /// Find the first phrase in the project.
        /// </summary>
        /// <returns>The first phrase node or null.</returns>
        public PhraseNode FindFirstPhrase()
        {
            PhraseNode first = null;
            getPresentation().getRootNode().acceptDepthFirst
            (
                delegate(TreeNode n)
                {
                    if (first != null) return false;
                    if (n is PhraseNode) first = (PhraseNode)n;
                    return true;
                },
                delegate(TreeNode n) {}
            );
            return first;
        }

        /// <summary>
        /// Toggle the "used" state of a node on behalf of a given view while issuing a command.
        /// </summary>
        /// <param name="node">The node to modify.</param>
        /// <param name="deep">If true, modify all descendants; otherwise, just phrase children.</param>
        internal void ToggleNodeUsedWithCommand(ObiNode node, bool deep)
        {
            if (node != null)
            {
                ToggleNodeUsed(node, deep);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(new Commands.Node.ToggleUsed(node, deep)));
            }
        }

        /// <summary>
        /// Toggle the "used" state of a node on behalf of a given view.
        /// </summary>
        /// <param name="node">The node to modify.</param>
        /// <param name="deep">If true, modify all descendants; otherwise, just phrase children.</param>
        public void ToggleNodeUsed(ObiNode node, bool deep)
        {
            bool used = !node.Used;
            if (deep)
            {
                // mark all nodes in the subtree.
                node.acceptDepthFirst(
                    delegate(TreeNode n)
                    {
                        ObiNode _n = (ObiNode)n;
                        if (_n.Used != used)
                        {
                            _n.Used = used;
                            ToggledNodeUsedState(this, new Events.Node.ObiNodeEventArgs(_n));
                        }
                        return true;
                    },
                    delegate(TreeNode n) { }
                );
            }
            else
            {
                // mark this node and its phrases if it is a section.
                node.Used = used;
                ToggledNodeUsedState(this, new Events.Node.ObiNodeEventArgs(node));
                SectionNode _n = node as SectionNode;
                if (_n != null)
                {
                    for (int i = 0; i < _n.PhraseChildCount; ++i)
                    {
                        PhraseNode ph = _n.PhraseChild(i);
                        if (ph.Used != used)
                        {
                            ph.Used = used;
                            ToggledNodeUsedState(this, new Events.Node.ObiNodeEventArgs(ph));
                        }
                    }
                }
            }
            Modified();
        }

        #region metadata

        /// <summary>
        /// Convenience method to create a new metadata object with a name/value pair.
        /// Skip it if there is no value (the toolkit doesn't like it.)
        /// </summary>
        /// <param name="name">The name of the metadata property.</param>
        /// <param name="value">Its content, i.e. the value.</param>
        private urakawa.metadata.Metadata AddMetadata(string name, string value)
        {
            if (value != null)
            {
                                urakawa.metadata.Metadata meta = getPresentation ().getMetadataFactory().createMetadata();
                meta.setName(name);
                meta.setContent(value);
                this.getPresentation ().appendMetadata(meta);
                return meta;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Test whether a metadata entry can be deleted (i.e. if it is not the last of its kind and is required.)
        /// </summary>
        public bool CanDeleteMetadata(MetadataEntryDescription entry)
        {
            return entry.Occurrence != MetadataOccurrence.Required || getPresentation ().getMetadataList(entry.Name).Count > 1;
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
            SetSingleMetadataItem(Obi.Metadata.DTB_GENERATOR, Generator);
            SetSingleMetadataItem(Obi.Metadata.OBI_XUK_VERSION, CURRENT_XUK_VERSION);
        }

        /// <summary>
        /// Get a single metadata item.
        /// </summary>
        /// <returns>The found metadata item, or null if not found.</returns>
        public urakawa.metadata.Metadata GetFirstMetadataItem(string name)
        {
            IList list = getPresentation ().getMetadataList(name);
            return list.Count > 0 ? list[0] as urakawa.metadata.Metadata : null;
        }

        /// <summary>
        /// Get a single metadata item.
        /// </summary>
        /// <returns>The found metadata item, or null if not found.</returns>
        public urakawa.metadata.Metadata GetSingleMetadataItem(string name)
        {
            IList list = getPresentation ().getMetadataList(name);
            if (list.Count > 1)
            {
                throw new Exception(String.Format("Expected a single metadata item for \"{0}\" but got {1}.",
                    name, list.Count));
            }
            return list.Count == 1 ? list[0] as urakawa.metadata.Metadata : null;
        }

        /// <summary>
        /// Set a metadata and ensure that it is the only one; i.e. delete any other occurrence.
        /// </summary>
        /// <param name="name">The name of the metadata item to set.</param>
        /// <param name="content">The content of the metadata item to set.</param>
        public void SetSingleMetadataItem(string name, string content)
        {
            getPresentation ().deleteMetadata(name);
            AddMetadata(name, content);
        }

        /// <summary>
        /// Shortcut to get the title of the project.
        /// </summary>
        public string Title
        {
            get { return GetFirstMetadataItem(Metadata.DC_TITLE).getContent(); }
        }

        /// <summary>
        /// Get or set the XUK version.
        /// </summary>
        public string XukVersion
        {
            get
            {
                urakawa.metadata.Metadata meta = GetSingleMetadataItem(Obi.Metadata.OBI_XUK_VERSION);
                return meta == null ? "" : meta.getContent();
            }
        }

        #endregion

        #region treeNode events

        /// <summary>
        /// Monitor the insertion of new nodes
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void presentation_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is PhraseNode)
            {
                PhraseNode phrase = (PhraseNode)e.getTreeNode();
                ++mPhraseCount;
                if (phrase.PageProperty != null) ++mPageCount;
                if (phrase.Audio != null && !DataManager.getEnforceSinglePCMFormat())
                {
                    DataManager.getDefaultPCMFormat().setSampleRate(phrase.Audio.getMediaData().getPCMFormat().getSampleRate());
                    DataManager.getDefaultPCMFormat().setNumberOfChannels(phrase.Audio.getMediaData().getPCMFormat().getNumberOfChannels());
                    DataManager.getDefaultPCMFormat().setBitDepth(phrase.Audio.getMediaData().getPCMFormat().getBitDepth());
                    DataManager.setEnforceSinglePCMFormat(true);
                }
                phrase.ParentSection.AddedPhraseNode(phrase);
            }
        }

        void presentation_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            if (e.getTreeNode() is PhraseNode)
            {
                                if (DataManager.getListOfMediaData ().Count == 0) DataManager.setEnforceSinglePCMFormat(false);
            }
        }

        #endregion


        /// <summary>
        /// Convenience method to create a new section node.
        /// </summary>
        public SectionNode NewSectionNode()
        {
            return getPresentation().getTreeNodeFactory().createNode(SectionNode.XUK_ELEMENT_NAME, Program.OBI_NS)
            as SectionNode;
        }
    }
}
