using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa;
using urakawa.core;
using urakawa.core.events;
using urakawa.media;
using urakawa.property.channel;
using Obi.Assets;

namespace Obi
{
    /// <summary>
    /// Error handler for cleaning up files (happens when a file cannot be deleted.)
    /// </summary>
    /// <param name="path">Path to the file that caused the error.</param>
    /// <param name="message">Error message.</param>
    public delegate void DeletingFileErrorHandler(string path, string message);

    /// <summary>
    /// An Obi project is an Urakawa project (core tree and metadata)
    /// It also knows where to save itself and has a simpler set of metadata.
    /// The core tree uses three channels:
    ///   1. an audio channel for audio media
    ///   2. a text channel for table of contents items (which will become NCX label in DTB)
    ///   3. an annotation channel for text annotation of other items in the book (e.g. phrases.)
    /// So we keep a handy pointer to those.
    /// </summary>
    public partial class Project : urakawa.Project
    {
        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file

        private AssetManager mAssManager;    // the asset manager
        private string mAssPath;             // the path to the asset manager directory
        private string mLastPath;            // last path to which the project was saved (see save as)
        private int mAudioChannels;          // project-wide number of channels for audio
        private int mSampleRate;             // project-wide sample rate for audio
        private int mBitDepth;               // project-wide bit depth for audio
        private int mPageCount;              // count the pages in the book
        private int mPhraseCount;            // total number of phrases in the project

        // TODO remove mAudioChannel, mAnnotation and mClipboard as members.
        private Channel mAudioChannel;       // handy pointer to the audio channel
        private Channel mAnnotationChannel;  // handy pointer to the annotation channel
        private Clipboard mClipboard;        // project-wide clipboard; should move to project panel

        public static readonly string CURRENT_XUK_VERSION = "obi-xuk-012";         // version of the Obi/XUK file
        public static readonly string AUDIO_CHANNEL_NAME = "obi.audio";            // canonical name of the audio channel
        public static readonly string TEXT_CHANNEL_NAME = "obi.text";              // canonical name of the text channel
        public static readonly string ANNOTATION_CHANNEL_NAME = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager
        public event Events.PhraseNodeHandler AddedPhraseNode;              // a phrase node was added to a strip
        public event Events.SetMediaHandler MediaSet;                       // a media object was set on a node
        public event Events.PhraseNodeHandler DeletedPhraseNode;            // deleted a phrase node 
        public event Events.NodeEventHandler TouchedNode;                   // this node was somehow modified
        public event Events.ObiNodeHandler ToggledNodeUsedState;            // the used state of a node was toggled.
        public event Events.SectionNodeHeadingHandler HeadingChanged;       // the heading of a section changed.


        /// <summary>
        /// Create a new empty project.
        /// </summary>
        /// <param name="XUKPath">The path to the XUK file where the project is to be saved.</param>
        public Project(string XUKPath): base(CreatePresentation(XUKPath), null)
        {
            mPhraseCount = 0;
            mPageCount = 0;
            mUnsaved = true;
            mXUKPath = XUKPath;
            Presentation presentation = getPresentation();
            ((ObiNodeFactory)presentation.getTreeNodeFactory()).Project = this;
            presentation.setRootNode(presentation.getTreeNodeFactory().createNode(Obi.RootNode.XUK_ELEMENT_NAME,
                Program.OBI_NS));
        }

        /// <summary>
        /// Create a new project with initial metadata.
        /// </summary>
        /// <param name="XUKPath">The path to the XUK file where the project is to be saved.</param>
        /// <param name="title">The title of the project.</param>
        /// <param name="id">The identifier for the project.</param>
        /// <param name="userProfile">The user profile for the user creating the project.</param>
        /// <param name="createTitle">If true, create an initial title section.</param>
        public Project(string XUKPath, string title, string id, UserProfile userProfile, bool createTitle): this(XUKPath)
        {
            AddChannel(ANNOTATION_CHANNEL_NAME);
            AddChannel(AUDIO_CHANNEL_NAME);
            AddChannel(TEXT_CHANNEL_NAME);
            CreateMetadata(title, id, userProfile);
            if (createTitle) CreateTitleSection(title);
            if (StateChanged != null) StateChanged(this,
                new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            Save();
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
        /// Get the text channel of the project.
        /// </summary>
        public Channel TextChannel { get { return GetSingleChannelByName(TEXT_CHANNEL_NAME); } }

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
        /// Create a new empty presentation with custom node and property factories.
        /// </summary>
        /// <param name="XUKPath">Path of the XUK file for the project hosting this presentation.</param>
        /// <returns>The new presentation.</returns>
        private static Presentation CreatePresentation(string XUKPath)
        {
            return new Presentation(new Uri(Path.GetDirectoryName(XUKPath) + Path.DirectorySeparatorChar),
                new ObiNodeFactory(), new PropertyFactory(), null, null, null, null, null);
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
        /// Access a channel which we know exist and is the only channel by this name.
        /// </summary>
        /// <param name="name">The name of the channel (use the name constants.)</param>
        /// <returns>The channel for this name.</returns>
        /// <exception cref="urakawa.exception.ChannelDoesNotExistException">Thrown when there is no channel by that name.</exception>
        /// <exception cref="TooManyChannelsException">Thrown when there are more than one channels with that name.</exception>
        private Channel GetSingleChannelByName(string name)
        {
            List<Channel> channels = getPresentation().getChannelsManager().getChannelByName(name);
            if (channels.Count == 0) throw new Exception(String.Format("No channel named \"{0}\"", name));
            if (channels.Count > 1) throw new Exception(String.Format("Expected 1 channel for {0}, got {1}.",
                name, channels.Count));
            return channels[0];
        }

        #endregion






        /// <summary>
        /// Create a blank project from a seed presentation and using the default metadata factory.
        /// </summary>
        /// <param name="presentation">The presentation for this project.</param>
        private Project(Presentation presentation)
            : base(presentation, null)
        {
            mAssManager = null;
            mUnsaved = false;
            mXUKPath = null;
            mAudioChannel = null;
            mAnnotationChannel = null;
            mClipboard = new Clipboard();
            mPhraseCount = 0;
            mPageCount = 0;
            AddedPhraseNode += new Obi.Events.PhraseNodeHandler(Project_AddedPhraseNode);
            DeletedPhraseNode += new Obi.Events.PhraseNodeHandler(Project_DeletedPhraseNode);
        }

        /// <summary>
        /// Convenience method for creating a new blank project. Actually create a presentation first so that we can use our own
        /// core node factory and custom property factory. Set up the channel manager as well.
        /// </summary>
        /// <returns>The newly created, blank project.</returns>
        public static Project BlankProject(string xukpath)
        {
            /*ObiNodeFactory nodeFactory = new ObiNodeFactory();
            Presentation presentation = new Presentation(nodeFactory, null, null, new ObiPropertyFactory(), null);
            ChannelFactory factory = presentation.getChannelFactory();
            ChannelsManager manager = presentation.getChannelsManager();
            Project project = new Project(presentation);
            nodeFactory.Project = project;
            return project;*/

            string asspath = Path.Combine(Path.GetDirectoryName(xukpath), GetAssetDirectory(xukpath));
            ObiNodeFactory nodeFactory = new ObiNodeFactory();
            Presentation presentation = new Presentation(new Uri(asspath), nodeFactory, new ObiPropertyFactory(), null, null, null, null, null);
            ChannelFactory factory = (ChannelFactory)presentation.getChannelFactory();
            ChannelsManager manager = (ChannelsManager)presentation.getChannelsManager();
            Project project = new Project(presentation);
            nodeFactory.Project = project;
            return project;
        }

        /// <summary>
        /// Number of audio channels for the project audio.
        /// </summary>
        public int AudioChannels
        {
            get { return mAudioChannels; }
        }

        /// <summary>
        /// Get the asset manager for this project.
        /// </summary>
        public AssetManager AssetManager
        {
            get { return mAssManager; }
        }

        /// <summary>
        /// Bit depth for the project audio.
        /// </summary>
        public int BitDepth
        {
            get { return mBitDepth; }
        }

        /// <summary>
        /// The project-wide clipboard.
        /// </summary>
        public Clipboard Clipboard
        {
            get { return mClipboard; }
        }

        /// <summary>
        /// Get the first section node in the project or null if there are no sections.
        /// </summary>
        public SectionNode FirstSection
        {
            get { return RootNode.getChildCount() > 0 ? (SectionNode)RootNode.getChild(0) : null; }
        }

        /// <summary>
        /// Get the generator string (Obi/Urakawa SDK) for the project.
        /// </summary>
        /// <remarks>Use the actual assembly name/version string for the toolkit (from 1.0)</remarks>
        public string Generator
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
        /// If there is audio in the projects, the audio settings should come from the project
        /// and not from the user settings.
        /// </summary>
        public bool HasAudioSettings
        {
            get { return mPhraseCount > 0; }
        }

        /// <summary>
        /// Last path under which the project was saved (different from the normal path when we save as)
        /// </summary>
        public string LastPath
        {
            get { return mLastPath; }
        }

        /// <summary>
        /// Get the last section node in the project or null if there are no sections.
        /// </summary>
        public SectionNode LastSection
        {
            get
            {
                SectionNode last = null;
                if (RootNode.getChildCount() > 0)
                {
                    last = (SectionNode)RootNode.getChild(RootNode.getChildCount() - 1);
                    while (last.SectionChildCount > 0) last = last.SectionChild(-1);
                }
                return last;
            }
        }

        /// <summary>
        /// Get the number of pages in the book.
        /// </summary>
        public int Pages
        {
            get { return mPageCount; }
        }

        /// <summary>
        /// Get the root node of the presentation as a TreeNode.
        /// </summary>
        public TreeNode RootNode
        {
            get { return (TreeNode)getPresentation().getRootNode(); }
        }

        /// <summary>
        /// Sample rate for the project audio.
        /// </summary>
        public int SampleRate
        {
            get { return mSampleRate; }
        }


        /// <summary>
        /// Create an empty project with some initial metadata.
        /// We can also automatically create the first node.
        /// </summary>
        /// <param name="xukPath">The path of the XUK file.</param>
        /// <param name="title">The title of the project.</param>
        /// <param name="id">The id of the project.</param>
        /// <param name="userProfile">The profile of the user who created it to fill in the metadata blanks.</param>
        /// <param name="createTitle">Create a title section.</param>
        public void Create(string xukPath, string title, string id, UserProfile userProfile, bool createTitle)
        {
            // The asset manager path is made relative to the XUK file's path; but we still use the absolute value that we
            // get from GetAssetDirectory to create/initialize the path of the asset manager.
            mXUKPath = xukPath;
            mAssPath = GetAssetDirectory(xukPath);
            mAssManager = new Assets.AssetManager(Path.Combine(Path.GetDirectoryName(xukPath), mAssPath));
            // CreateChannels();
            CreateMetadata(title, id, userProfile);
            if (createTitle) CreateTitleSection(title);
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            }
            Save();
        }



        /// <summary>
        /// Create a new phrase node from an asset.
        /// Add a seq media object with the clips of the audio asset. Do not forget to set begin/end time explicitely.
        /// Add a node information custom property as well.
        /// </summary>
        /// <param name="asset">The asset for the phrase.</param>
        /// <returns>The created node.</returns>
        private PhraseNode CreatePhraseNode(Assets.AudioMediaAsset asset)
        {
            PhraseNode node = CreatePhraseNode();
            node.Asset = asset;
            return node;
        }

        /// <summary>
        /// Delets files that are not in use anymore.
        /// </summary>
        /// <param name="report">Report errors that occur while deleting files.</param>
        public void DeleteUnusedFiles(DeletingFileErrorHandler report)
        {
            foreach (string path in mAssManager.UnusedFilePaths())
            {
                System.Diagnostics.Debug.Print("Deleting unused file: {0}", path);
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception e)
                    {
                        report(path, e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Find the channel with the given name. It must be unique.
        /// </summary>
        /// <remarks>This is a convenience method because the toolkit returns an array of channels.</remarks>
        /// <param name="name">Name of the channel that we are looking for.</param>
        /// <returns>The channel found.</returns>
        public Channel FindChannel(string name)
        {
            List<Channel> channels = getPresentation().getChannelsManager().getChannelByName(name);
            if (channels.Count != 1)
            {
                throw new Exception(String.Format("Expected one channel named {0} in {1}, but got {2}.",
                    name, mXUKPath, channels.Count));
            }
            return channels[0];
        }

        /// <summary>
        /// Get a suitable directory name to store the assets.
        /// The default name is the same as the XUK file with _assets instead of .xuk as a suffix.
        /// Mangle the name until a free name is found.
        /// </summary>
        /// <returns>The relative paht to the directory chosen.</returns>
        private static string GetAssetDirectory(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string assetdir = Path.GetFileNameWithoutExtension(path) + "_assets";
            while (Directory.Exists(Path.Combine(dir, assetdir)) || File.Exists(Path.Combine(dir, assetdir))) assetdir += "_";
            return assetdir;
        }

        /// <summary>
        /// Project was modified.
        /// </summary>
        public void Modified()
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
        public void Modified(Commands.Command command)
        {
            if (CommandCreated != null)
            {
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
            Modified();
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
            ReadMetadata();
            mXUKPath = xukPath;
            mAudioChannel = FindChannel(AUDIO_CHANNEL_NAME);
            mAnnotationChannel = FindChannel(ANNOTATION_CHANNEL_NAME);
            if (mAssPath == null) throw new Exception(Localizer.Message("missing_asset_path"));
            Uri absoluteAssPath = new Uri(new Uri(xukPath), mAssPath);
            mAssManager = new Assets.AssetManager(absoluteAssPath.AbsolutePath);
            // TODO: count pages and phrases
            // mPhraseCount = visitor.Phrases;
            // mPageCount = visitor.Pages;
            if (StateChanged != null)
            {
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            }
        }

        /// <summary>
        /// Project has reverted to its initial state (e.g. after all commands have been undone.)
        /// </summary>
        public void Reverted()
        {
            Saved();
        }

        /// <summary>
        /// Get a safe name from a given title. Usable for XUK file and project directory filename.
        /// Disallowed characters for file paths are replaced by an underscore.
        /// Note that the user is allowed to change this--this is only a suggestion.
        /// </summary>
        /// <param name="title">Complete title.</param>
        /// <returns>The safe version.</returns>
        public static string SafeName(string title)
        {
            string invalid = "[";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                invalid += String.Format("\\x{0:x2}", (int)c);
            invalid += "]+";
            string safeName = System.Text.RegularExpressions.Regex.Replace(title, invalid, "_");
            safeName = System.Text.RegularExpressions.Regex.Replace(safeName, "^_", "");
            safeName = System.Text.RegularExpressions.Regex.Replace(safeName, "_$", "");
            return safeName;
        }

        /// <summary>
        /// Save the project to its XUK file.
        /// </summary>
        internal void Save()
        {
            saveXUK(new Uri(mXUKPath));
            mLastPath = mXUKPath;
            Saved();
        }

        /// <summary>
        /// Save the project to a different name/XUK file.
        /// </summary>
        /// <remarks>TO REVIEW</remarks>
        public void SaveAs(string path)
        {
            string oldAssPath = mAssPath;
            mAssPath = GetAssetDirectory(path);
            Directory.CreateDirectory(mAssPath);
            foreach (string assetPath in mAssManager.Files.Keys)
            {
                File.Copy(assetPath, mAssPath + Path.DirectorySeparatorChar + Path.GetFileName(assetPath));
            }
            mAssPath = (new Uri(path)).MakeRelativeUri(new Uri(mAssPath)).ToString();
            saveXUK(new Uri(path));
            mLastPath = path;
            mAssPath = oldAssPath;
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
        /// Simulate a modification of the project.
        /// </summary>
        public void Touch()
        {
            Modified(new Commands.Touch(this));
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
        /// Dump the asset manager to check what's going on.
        /// </summary>
        internal void DumpAssManager()
        {
            System.Diagnostics.Debug.Print("Managed assets:");
            foreach (string name in mAssManager.GetAssets(Assets.MediaType.Audio).Keys)
            {
                System.Diagnostics.Debug.Print("* {0}", name);
            }
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

        /// <summary>
        /// Changes the asset path to the next available directory.  Also create the directory.
        /// </summary>
        public string AssignNewAssetDirectory()
        {
            Uri absoluteAssPath = new Uri(new Uri(mXUKPath), mAssPath); 
            string newAssPath = GetAssetDirectory(absoluteAssPath.ToString());

            //make as a local path, as that's what everyone seems to want
            newAssPath = (new Uri(newAssPath)).LocalPath;
            
            //save the new asset path as a relative Uri
            mAssPath = (new Uri(mXUKPath)).MakeRelativeUri(new Uri(newAssPath)).ToString();
            
            //create and return the new asset path
            Directory.CreateDirectory(newAssPath);
            return newAssPath;
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
                urakawa.metadata.Metadata meta = getMetadataFactory().createMetadata();
                meta.setName(name);
                meta.setContent(value);
                this.appendMetadata(meta);
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
            return entry.Occurrence != MetadataOccurrence.Required || getMetadataList(entry.Name).Count > 1;
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
            SetSingleMetadataItem(Obi.Metadata.OBI_ASSETS_DIR, mAssPath);
            SetSingleMetadataItem(Obi.Metadata.OBI_XUK_VERSION, CURRENT_XUK_VERSION);
        }

        /// <summary>
        /// Get a single metadata item.
        /// </summary>
        /// <returns>The found metadata item, or null if not found.</returns>
        public urakawa.metadata.Metadata GetFirstMetadataItem(string name)
        {
            IList list = getMetadataList(name);
            return list.Count > 0 ? list[0] as urakawa.metadata.Metadata : null;
        }

        /// <summary>
        /// Get a single metadata item.
        /// </summary>
        /// <returns>The found metadata item, or null if not found.</returns>
        public urakawa.metadata.Metadata GetSingleMetadataItem(string name)
        {
            IList list = getMetadataList(name);
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
            deleteMetadata(name);
            AddMetadata(name, content);
        }

        /// <summary>
        /// Read the project metadata.
        /// </summary>
        private void ReadMetadata()
        {
            mAssPath = GetSingleMetadataItem(Metadata.OBI_ASSETS_DIR).getContent();
            urakawa.metadata.Metadata m;
            if ((m = GetSingleMetadataItem(Metadata.OBI_AUDIO_CHANNELS)) != null)
            {
                mAudioChannels = Int32.Parse(m.getContent());
            }
            if ((m = GetSingleMetadataItem(Metadata.OBI_BIT_DEPTH)) != null)
            {
                mBitDepth = Int32.Parse(m.getContent());
            }
            if ((m = GetSingleMetadataItem(Metadata.OBI_SAMPLE_RATE)) != null)
            {
                mSampleRate = Int32.Parse(m.getContent());
            }
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
    }
}
