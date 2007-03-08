using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.media;

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
    public partial class Project : urakawa.project.Project
    {
        private Channel mAudioChannel;       // handy pointer to the audio channel
        private Channel mAnnotationChannel;  // handy pointer to the annotation channel

        private Assets.AssetManager mAssManager;  // the asset manager
        private string mAssPath;                  // the path to the asset manager directory

        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        private string mLastPath;            // last path to which the project was saved (see save as)
        private SimpleMetadata mMetadata;    // metadata for this project

        private Clipboard mClipboard;        // project-wide clipboard.
        //private PhraseNode mSilencePhrase;     // silence phrase used for phrase detection

        public static readonly string XUKVersion = "obi-xuk-009";                // version of the Obi/XUK file
        public static readonly string AudioChannelName = "obi.audio";            // canonical name of the audio channel
        public static readonly string TextChannelName = "obi.text";              // canonical name of the text channel
        public static readonly string AnnotationChannelName = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager
        public event Events.PhraseNodeHandler AddedPhraseNode;              // a phrase node was added to a strip
        public event Events.SetMediaHandler MediaSet;                       // a media object was set on a node
        public event Events.PhraseNodeHandler DeletedPhraseNode;            // deleted a phrase node 
        public event Events.NodeEventHandler TouchedNode;                   // this node was somehow modified
        public event Events.ObiNodeHandler ToggledNodeUsedState;            // the used state of a node was toggled.

        private int mPages;  // count the pages in the book

        public Channel AnnotationChannel
        {
            get { return mAnnotationChannel; }
        }

        public Channel TextChannel
        {
            get { return FindChannel(TextChannelName); }
        }

        /// <summary>
        /// Identify self as generator for this project.
        /// </summary>
        public string Generator
        {
            get
            {
                return String.Format("{0} v{1} with Urakawa SDK v0.5 (http://urakawa.sf.net)",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            }
        }

        /// <summary>
        /// This flag is set to true if the project contains modifications that have not been saved.
        /// </summary>
        public bool Unsaved
        {
            get { return mUnsaved; }
        }

        /// <summary>
        /// The metadata of the project.
        /// </summary>
        public SimpleMetadata Metadata
        {
            get { return mMetadata; }
        }

        /// <summary>
        /// The path to the XUK file for this project.
        /// </summary>
        public string XUKPath
        {
            get { return mXUKPath; }
        }

        /// <summary>
        /// Get the root node of the presentation as a CoreNode.
        /// </summary>
        public CoreNode RootNode
        {
            get { return (CoreNode)getPresentation().getRootNode(); }
        }

        /// <summary>
        /// Return the first section node in the project; or null if there are no sections.
        /// </summary>
        public SectionNode FirstSection
        {
            get
            {
                return RootNode.getChildCount() > 0 ? (SectionNode)RootNode.getChild(0) : null;
            }
        }

        /// <summary>
        /// Return the last section node in the project; or null if there are no sections.
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
        /// Last path under which the project was saved (different from the normal path when we save as)
        /// </summary>
        public string LastPath
        {
            get { return mLastPath; }
        }

        /// <summary>
        /// Get the asset manager for this project.
        /// </summary>
        internal Assets.AssetManager AssetManager
        {
            get { return mAssManager; }
        }

        /// <summary>
        /// The project-wide clipboard.
        /// </summary>
        public Clipboard Clipboard
        {
            get { return mClipboard; }
        }

        /// <summary>
        /// Get the number of pages in the book.
        /// </summary>
        public int Pages
        {
            get { return mPages; }
        }

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
            mPages = 0;
        }

        /// <summary>
        /// Convenience method for creating a new blank project. Actually create a presentation first so that we can use our own
        /// core node factory and custom property factory. Set up the channel manager as well.
        /// </summary>
        /// <returns>The newly created, blank project.</returns>
        public static Project BlankProject()
        {
            ObiNodeFactory nodeFactory = new ObiNodeFactory();
            Presentation presentation = new Presentation(nodeFactory, null, null, new ObiPropertyFactory(), null);
            ChannelFactory factory = presentation.getChannelFactory();
            ChannelsManager manager = presentation.getChannelsManager();
            Project project = new Project(presentation);
            nodeFactory.Project = project;
            return project;
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
            Presentation presentation = getPresentation();

            // The asset manager path is made relative to the XUK file's path; but we still use the absolute value that we
            // get from GetAssetDirectory to create/initialize the path of the asset manager.
            mXUKPath = xukPath;
            mAssPath = GetAssetDirectory(xukPath);
            mAssManager = new Assets.AssetManager(mAssPath);
            mAssPath = (new Uri(xukPath)).MakeRelativeUri(new Uri(mAssPath)).ToString();

            mAssPath = mAssPath.Replace("%20", " ");

            // Create metadata and channels factories
            ChannelFactory factory = presentation.getChannelFactory();
            ChannelsManager manager = presentation.getChannelsManager();
            //create three channels
            mAudioChannel = factory.createChannel(AudioChannelName);
            manager.addChannel(mAudioChannel);
            manager.addChannel(factory.createChannel(TextChannelName));
            mAnnotationChannel = factory.createChannel(AnnotationChannelName);
            manager.addChannel(mAnnotationChannel);
            mMetadata = CreateMetadata(title, id, userProfile);

            // Create a title section if necessary
            if (createTitle)
            {
                SectionNode node = (SectionNode)
                    presentation.getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
                node.Label = title;
                presentation.getRootNode().appendChild(node);
            }

            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            Save();
        }

        /// <summary>
        /// Get a suitable directory name to store the assets.
        /// The default name is the same as the XUK file with _assets instead of .xuk as a suffix.
        /// Mangle the name until a free name is found.
        /// </summary>
        private string GetAssetDirectory(string path)
        {
            string assetdir = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + "_assets";
            while (Directory.Exists(assetdir) || File.Exists(assetdir)) assetdir += "_";
            return assetdir;
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// Throw an exception if the file could not be read.
        /// </summary>
        /// <param name="xukPath">The path of the XUK file.</param>
        public void Open(string xukPath)
        {
            if (openXUK(new Uri(xukPath)))
            {
                mUnsaved = false;
                mXUKPath = xukPath;
                mAudioChannel = FindChannel(AudioChannelName);
                mAnnotationChannel = FindChannel(AnnotationChannelName);
                mMetadata = new SimpleMetadata();
                string xukversion = "none";
                foreach (object o in getMetadataList())
                {
                    urakawa.project.Metadata meta = (urakawa.project.Metadata)o;
                    if (meta.getName() == SimpleMetadata.MetaTitle)
                    {
                        mMetadata.Title = meta.getContent();
                    }
                    else if (meta.getName() == SimpleMetadata.MetaPublisher)
                    {
                        mMetadata.Publisher = meta.getContent();
                    }
                    else if (meta.getName() == SimpleMetadata.MetaIdentifier)
                    {
                        mMetadata.Identifier = meta.getContent();
                    }
                    else if (meta.getName() == SimpleMetadata.MetaLanguage)
                    {
                        mMetadata.Language = new System.Globalization.CultureInfo(meta.getContent());
                    }
                    else if (meta.getName() == SimpleMetadata.MetaNarrator)
                    {
                        mMetadata.Narrator = meta.getContent();
                    }
                    else if (meta.getName() == SimpleMetadata.MetaAssetsDir)
                    {
                        mAssPath = meta.getContent();
                    }
                    else if (meta.getName() == SimpleMetadata.MetaXUKVersion)
                    {
                        xukversion = meta.getContent();
                    }
                }
                if (xukversion != Project.XUKVersion)
                    throw new Exception(String.Format(Localizer.Message("xuk_version_mismatch"), Project.XUKVersion, xukversion));
                if (mAssPath == null) throw new Exception(Localizer.Message("missing_asset_path"));
                Uri absoluteAssPath = new Uri(new Uri(xukPath), mAssPath); 
                mAssManager = new Assets.AssetManager(absoluteAssPath.AbsolutePath);
                // Recreate the assets from the phrase nodes
                // string errMessages = ""; 
                Visitors.PhraseInitializer visitor = new Visitors.PhraseInitializer(mAssManager, delegate(string m) { });
                //     delegate(string message) { errMessages += message + "\n"; });
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                mPages = visitor.Pages;
                // if (errMessages != "")
                // {
                //     throw new Exception(String.Format(Localizer.Message("open_project_error_text") + "\n" + errMessages, xukPath));
                // }
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            }
            else
            {
                throw new Exception(String.Format(Localizer.Message("open_project_error_text"), xukPath)); 
            }
        }

        /// <summary>
        /// Find the channel with the given name. It must be unique.
        /// </summary>
        /// <remarks>This is a convenience method because the toolkit returns an array of channels.</remarks>
        /// <param name="name">Name of the channel that we are looking for.</param>
        /// <returns>The channel found.</returns>
        internal Channel FindChannel(string name)
        {
            IChannel[] channels = getPresentation().getChannelsManager().getChannelByName(name);
            if (channels.Length != 1)
            {
                throw new Exception(String.Format("Expected one channel named {0} in {1}, but got {2}.",
                    name, mXUKPath, channels.Length));
            }
            return (Channel)channels[0];
        }

        /// <summary>
        /// Create the XUK metadata for the project from the SimpleMetadata object.
        /// </summary>
        /// <returns>The simple metadata object for the project metadata.</returns>
        private SimpleMetadata CreateMetadata(string title, string id, UserProfile userProfile)
        {
            SimpleMetadata metadata = new SimpleMetadata(title, id, userProfile);
            AddMetadata(SimpleMetadata.MetaTitle, metadata.Title);
            AddMetadata(SimpleMetadata.MetaPublisher, metadata.Publisher);
            AddMetadata(SimpleMetadata.MetaIdentifier, metadata.Identifier);
            AddMetadata(SimpleMetadata.MetaLanguage, metadata.Language.ToString());
            AddMetadata(SimpleMetadata.MetaNarrator, metadata.Narrator);
            AddMetadata(SimpleMetadata.MetaGenerator, this.Generator);
            AddMetadata(SimpleMetadata.MetaAssetsDir, mAssPath);
            AddMetadata(SimpleMetadata.MetaXUKVersion, Project.XUKVersion);
            // urakawa.project.Metadata metaDate = AddMetadata("dc:Date", DateTime.Today.ToString("yyyy-MM-dd"));
            // if (metaDate != null) metaDate.setScheme("YYYY-MM-DD");
            return metadata;
        }

        /// <summary>
        /// Update the XUK metadata of the project given the simple metadata object.
        /// Also, this method is very ugly; hope the facade API makes this better.
        /// </summary>
        private void UpdateMetadata()
        {
            foreach (object o in getMetadataList())
            {
                urakawa.project.Metadata meta = (urakawa.project.Metadata)o;
                if (meta.getName() == SimpleMetadata.MetaTitle)
                {
                    meta.setContent(mMetadata.Title);
                }
                else if (meta.getName() == SimpleMetadata.MetaPublisher)
                {
                    meta.setContent(mMetadata.Publisher);
                }
                else if (meta.getName() == SimpleMetadata.MetaLanguage)
                {
                    meta.setContent(mMetadata.Language.ToString());
                }
                else if (meta.getName() == SimpleMetadata.MetaIdentifier)
                {
                    meta.setContent(mMetadata.Identifier);
                }
                else if (meta.getName() == SimpleMetadata.MetaNarrator)
                {
                    meta.setContent(mMetadata.Narrator);
                }
                else if (meta.getName() == SimpleMetadata.MetaAssetsDir)
                {
                    meta.setContent(mAssPath);
                }
            }
        }

        /// <summary>
        /// Convenience method to create a new metadata object with a name/value pair.
        /// Skip it if there is no value (the toolkit doesn't like it.)
        /// </summary>
        /// <param name="name">The name of the metadata property.</param>
        /// <param name="value">Its content, i.e. the value.</param>
        private urakawa.project.Metadata AddMetadata(string name, string value)
        {
            if (value != null)
            {
                urakawa.project.Metadata meta = (urakawa.project.Metadata)getMetadataFactory().createMetadata();
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
        /// Project was modified.
        /// </summary>
        public void Modified()
        {
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Project was modified and a command is issued.
        /// </summary>
        /// <param name="command">The command to issue.</param>
        public void Modified(Commands.Command command)
        {
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        /// <summary>
        /// Project was saved
        /// </summary>
        private void Saved()
        {
            mUnsaved = false;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Saved));            
        }

        /// <summary>
        /// Save the project to its XUK file.
        /// </summary>
        internal void Save()
        {
            UpdateMetadata();
            saveXUK(new Uri(mXUKPath));
            mLastPath = mXUKPath;
            Saved();
        }

        /// <summary>
        /// Save the project to a different name/XUK file.
        /// </summary>
        /// <remarks>
        /// We probably need to catch exceptions here.
        /// </remarks>
        public void SaveAs(string path)
        {
            string oldAssPath = mAssPath;
            mAssPath = GetAssetDirectory(path);
            Directory.CreateDirectory(mAssPath);
            foreach (string assetPath in mAssManager.Files.Keys)
            {
                File.Copy(assetPath, mAssPath + @"\" + Path.GetFileName(assetPath));
            }
            mAssPath = (new Uri(path)).MakeRelativeUri(new Uri(mAssPath)).ToString();
            UpdateMetadata();
            saveXUK(new Uri(path));
            mLastPath = path;
            mAssPath = oldAssPath;
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
        /// Close the project.
        /// </summary>
        public void Close()
        {
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Closed));
        }

        /// <summary>
        /// Get a safe name from a given title. Usable for XUK file and project directory filename.
        /// Disallowed characters for file paths are removed.
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
        /// Simulate a modification of the project.
        /// </summary>
        public void Touch()
        {
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(new Commands.Touch(this)));
            Modified();
        }

        /// <summary>
        /// Project has reverted to its initial state (e.g. after all commands have been undone.)
        /// </summary>
        internal void Reverted()
        {
            Saved();
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
            PhraseNode node = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                 as PhraseNode;
            node.Asset = asset;

            return node;
        }

        /// <summary>
        /// Get the text media of a core node. The result can then be used to get or set the text of a node.
        /// Original comments: A helper function to get the text from the given <see cref="CoreNode"/>.
        /// The text channel which contains the desired text will be named so that we know 
        /// what its purpose is (ie, "DefaultText" or "PrimaryText")
        /// @todo
        /// Otherwise we should use the default, only, or randomly first text channel found.
        /// </summary>
        /// <remarks>This replaces get/setCoreNodeText. E.g. getCoreNodeText(node) = GetTextMedia(node).getText()</remarks>
        /// <remarks>This is taken from TOCPanel, and should probably be a node method;
        /// we would subclass CoreNode fort his.</remarks>
        /// <param name="node">The node which text media we are interested in.</param>
        /// <returns>The text media found, or null if none.</returns>
        public static TextMedia GetTextMedia(CoreNode node)
        {
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel textChannel = Project.GetChannel(node, TextChannelName);
            return textChannel == null ? null : (TextMedia)prop.getMedia(textChannel);
        }

        public static Channel GetChannel(CoreNode node, string name)
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
        public static IMedia GetMediaForChannel(CoreNode node, string channel)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel foundChannel;
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();
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
        /// Temporary convenience for finding the first phrase, i.e. the silence phrase (so far.)
        /// </summary>
        /// <returns>The first phrase node or null.</returns>
        internal PhraseNode FindFirstPhrase()
        {
            PhraseNode first = null;
            getPresentation().getRootNode().visitDepthFirst
            (
                delegate(ICoreNode n)
                {
                    if (first != null) return false;
                    if (n.GetType() == System.Type.GetType("Obi.PhraseNode")) { first = (PhraseNode)n; System.Diagnostics.Debug.Print("bing!"); }
                    return true;
                },
                delegate(ICoreNode n) {}
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
                node.visitDepthFirst(
                    delegate(ICoreNode n)
                    {
                        ObiNode _n = (ObiNode)n;
                        if (_n.Used != used)
                        {
                            _n.Used = used;
                            ToggledNodeUsedState(this, new Events.Node.ObiNodeEventArgs(_n));
                        }
                        return true;
                    },
                    delegate(ICoreNode n) { }
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
        /// <param name="path"></param>
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
    }
}
