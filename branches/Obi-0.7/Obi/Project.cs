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
        private Channel mTextChannel;        // handy pointer to the text channel 
        private Channel mAnnotationChannel;  // handy pointer to the annotation channel

        private Assets.AssetManager mAssManager;  // the asset manager
        private string mAssPath;                  // the path to the asset manager directory

        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        private string mLastPath;            // last path to which the project was saved (see save as)
        private SimpleMetadata mMetadata;    // metadata for this project

        private Clipboard mClipboard;        // project-wide clipboard.
        private CoreNode mSilencePhrase;     // silence phrase used for phrase detection

        public static readonly string XUKVersion = "obi-xuk-007";            // version of the Obi/XUK file
        public static readonly string AudioChannel = "obi.audio";            // canonical name of the audio channel
        public static readonly string TextChannel = "obi.text";              // canonical name of the text channel
        public static readonly string AnnotationChannel = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager
        public event Events.Node.AddedPhraseNodeHandler AddedPhraseNode;        // a phrase node was added to a strip
        public event Events.Node.MediaSetHandler MediaSet;                      // a media object was set on a node
        public event Events.Node.DeletedNodeHandler DeletedPhraseNode;          // deleted a phrase node 
        public event Events.Node.TouchedNodeHandler TouchedNode;  // this node was somehow modified

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
        /// Create an empty project. And I mean empty.
        /// </summary>
        /// <remarks>
        /// This is necessary because we need an instance of a project to set the event handler for the project state
        /// changes, which happen as soon as the project is created or opened.
        /// </remarks>
        public Project()
            : base()
        {
            mAssManager = null;
            mUnsaved = false;
            mXUKPath = null;
            mClipboard = new Clipboard();
            mSilencePhrase = null;
            // Use our own property factory so that we can create custom properties
            getPresentation().setPropertyFactory(new ObiPropertyFactory());
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
            mAssManager = new Assets.AssetManager(mAssPath);
            mAssPath = (new Uri(xukPath)).MakeRelativeUri(new Uri(mAssPath)).ToString();

            // Create metadata and channels
            mMetadata = CreateMetadata(title, id, userProfile);
            mAudioChannel = getPresentation().getChannelFactory().createChannel(AudioChannel);
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel(TextChannel);
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mAnnotationChannel = getPresentation().getChannelFactory().createChannel(AnnotationChannel);
            getPresentation().getChannelsManager().addChannel(mAnnotationChannel);

            // Give a custom property to the root node to make it a Root node.
            NodeInformationProperty typeProp =
                (NodeInformationProperty)getPresentation().getPropertyFactory().createProperty("NodeInformationProperty",
                ObiPropertyFactory.ObiNS);
            typeProp.NodeType = NodeType.Root;
            typeProp.NodeStatus = NodeStatus.NA;
            getPresentation().getRootNode().setProperty(typeProp);
            NodeInformationProperty rootType = (NodeInformationProperty)getPresentation().getRootNode().getProperty(typeof(NodeInformationProperty));

            if (createTitle)
            {
                CoreNode node = CreateSectionNode();
                GetTextMedia(node).setText(title);
                getPresentation().getRootNode().appendChild(node);
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
                mAudioChannel = FindChannel(AudioChannel);
                mTextChannel = FindChannel(TextChannel);
                mAnnotationChannel = FindChannel(AnnotationChannel);
                mMetadata = new SimpleMetadata();
                string xukversion = "none";
                foreach (object o in getMetadataList())
                {
                    urakawa.project.Metadata meta = (urakawa.project.Metadata)o;
                    switch (meta.getName())
                    {
                        case "dc:Title":
                            mMetadata.Title = meta.getContent();
                            break;
                        case "dc:Creator":
                            mMetadata.Author = meta.getContent();
                            break;
                        case "dc:Identifier":
                            mMetadata.Identifier = meta.getContent();
                            break;
                        case "dc:Language":
                            mMetadata.Language = new System.Globalization.CultureInfo(meta.getContent());
                            break;
                        case "dc:Publisher":
                            mMetadata.Publisher = meta.getContent();
                            break;
                        case "obi:assetsdir":
                            mAssPath = meta.getContent();
                            break;
                        case "obi:xukversion":
                            xukversion = meta.getContent();
                            break;
                        default:
                            break;
                    }
                }
                if (xukversion != Project.XUKVersion)
                    throw new Exception(String.Format(Localizer.Message("xuk_version_mismatch"), Project.XUKVersion, xukversion));
                if (mAssPath == null) throw new Exception(Localizer.Message("missing_asset_path"));
                Uri absoluteAssPath = new Uri(new Uri(xukPath), mAssPath); 
                mAssManager = new Assets.AssetManager(absoluteAssPath.AbsolutePath);
                // Recreate the assets from the phrase nodes
                string errMessages = ""; 
                Visitors.AssetCreator visitor = new Visitors.AssetCreator(mAssManager,
                    delegate(string message) { errMessages += message + "\n"; });
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                if (errMessages != "")
                    throw new Exception(String.Format(Localizer.Message("open_project_error_text") + "\n" + errMessages, xukPath)); 
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
        private Channel FindChannel(string name)
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
            AddMetadata("dc:Title", metadata.Title);
            AddMetadata("dc:Creator", metadata.Author);  // missing "role"
            AddMetadata("dc:Identifier", metadata.Identifier);
            AddMetadata("dc:Language", metadata.Language.ToString());
            AddMetadata("dc:Publisher", metadata.Publisher);
            urakawa.project.Metadata metaDate = AddMetadata("dc:Date", DateTime.Today.ToString("yyyy-MM-dd"));
            if (metaDate != null) metaDate.setScheme("YYYY-MM-DD");
            AddMetadata("xuk:generator", "Obi+Urakawa toolkit");
            AddMetadata("obi:assetsdir", mAssPath);
            AddMetadata("obi:xukversion", Project.XUKVersion);
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
                switch (meta.getName())
                {
                    case "dc:Title":
                        meta.setContent(mMetadata.Title);
                        break;
                    case "dc:Creator":
                        meta.setContent(mMetadata.Author);
                        break;
                    case "dc:Identifier":
                        meta.setContent(mMetadata.Identifier);
                        break;
                    case "dc:Language":
                        meta.setContent(mMetadata.Language.ToString());
                        break;
                    case "dc:Publisher":
                        meta.setContent(mMetadata.Publisher);
                        break;
                    case "dc:Date":
                        meta.setContent(DateTime.Today.ToString("yyyy-MM-dd"));
                        break;
                    case "obi:assetsdir":
                        meta.setContent(mAssPath);
                        break;
                    default:
                        break;
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
        /// Save the project to its XUK file.
        /// </summary>
        internal void Save()
        {
            UpdateMetadata();
            saveXUK(new Uri(mXUKPath));
            mUnsaved = false;
            mLastPath = mXUKPath;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Saved));
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
        /// We allow only digits, letters and underscores in the safe name and transform anything else
        /// to underscores. Note that the user is allowed to change this--this is only a suggestion.
        /// </summary>
        /// <param name="title">Complete title.</param>
        /// <returns>The safe version.</returns>
        public static string SafeName(string title)
        {
            string safeName = System.Text.RegularExpressions.Regex.Replace(title, @"[^a-zA-Z0-9]+", "_");
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
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Project has reverted to its initial state (e.g. after all commands have been undone.)
        /// </summary>
        internal void Reverted()
        {
            mUnsaved = false;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Saved));
        }

        /// <summary>
        /// Create a new phrase node from an asset.
        /// Add a seq media object with the clips of the audio asset. Do not forget to set begin/end time explicitely.
        /// Add a node information custom property as well.
        /// </summary>
        /// <param name="asset">The asset for the phrase.</param>
        /// <returns>The created node.</returns>
        private CoreNode CreatePhraseNode(Assets.AudioMediaAsset asset)
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            node.setProperty(prop);
            TextMedia annotation = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            
            //md i would rather set this to "" but the toolkit doesn't allow it
            //annotation.setText("-");
            //JQ I guess we'll just set it to nothing and be done with it
            prop.setMedia(mAnnotationChannel, annotation);
           
            AssetProperty assProp = (AssetProperty)getPresentation().getPropertyFactory().createProperty("AssetProperty",
                ObiPropertyFactory.ObiNS);
            assProp.Asset = asset;
            node.setProperty(assProp);
            NodeInformationProperty typeProp =
                (NodeInformationProperty)getPresentation().getPropertyFactory().createProperty("NodeInformationProperty",
                ObiPropertyFactory.ObiNS);
            typeProp.NodeType = NodeType.Phrase;
            typeProp.NodeStatus = NodeStatus.Used;
            node.setProperty(typeProp);
            UpdateSeq(node);
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
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            Channel textChannel;
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                string channelName = ((IChannel)channelsList[i]).getName();
                if (channelName == Project.TextChannel)
                {
                    textChannel = (Channel)channelsList[i];
                    return (TextMedia)channelsProp.getMedia(textChannel);
                }
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
        /// Get the type of a node depending on its position in the tree or its channels.
        /// </summary>
        /// <param name="node">The node for which we want the type.</param>
        public static NodeType GetNodeType(CoreNode node)
        {
            NodeInformationProperty prop = (NodeInformationProperty)node.getProperty(typeof(NodeInformationProperty));
            if (prop != null)
            {
                return prop.NodeType;
            }
            else
            {
                return NodeType.Vanilla;
            }
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
        /// Return the node's level
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        // md 20061005.  expect to replace soon with a toolkit or extension function.
        public int getNodeLevel(CoreNode node)
        {
            //if we are the root node
            if (node.getParent() == null)
            {
                return 0;
            }
            else
            {
                return getNodeLevel((CoreNode)node.getParent()) + 1;
            }
        }

        /// <summary>
        /// Temporary convenience for finding the first phrase, i.e. the silence phrase (so far.)
        /// </summary>
        /// <returns>The first phrase node or null.</returns>
        internal CoreNode FindFirstPhrase()
        {
            CoreNode first = null;
            getPresentation().getRootNode().visitDepthFirst
            (
                delegate(ICoreNode n)
                {
                    if (first != null) return false;
                    if (GetNodeType((CoreNode)n) == NodeType.Phrase) { first = (CoreNode)n; System.Diagnostics.Debug.Print("bing!"); }
                    return true;
                },
                delegate(ICoreNode n) {}
            );
            return first;
        }
    }
}
