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

        private CoreNode mClipboard;        //clipboard for cut-copy-paste

        public static readonly string XUKVersion = "obi-xuk-001";            // version of the Obi/XUK file
        public static readonly string AudioChannel = "obi.audio";            // canonical name of the audio channel
        public static readonly string TextChannel = "obi.text";              // canonical name of the text channel
        public static readonly string AnnotationChannel = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager

        public event Events.Node.AddedSectionNodeHandler AddedSectionNode;      // a section node was added to the TOC
        public event Events.Node.AddedPhraseNodeHandler AddedPhraseNode;        // a phrase node was added to a strip
        public event Events.Node.DeletedNodeHandler DeletedNode;                // a node was deleted from the presentation
        public event Events.Node.RenamedNodeHandler RenamedNode;                // a node was renamed in the presentation
        public event Events.Node.MovedNodeHandler MovedNode;                    // a node was moved in the presentation
        public event Events.Node.DecreasedNodeLevelHandler DecreasedNodeLevel;  // a node's level was decreased in the presentation
        public event Events.Node.ImportedAssetHandler ImportedAsset;            // an asset was imported into the project
        public event Events.Node.MovedNodeHandler UndidMoveNode;                // a node was restored to its previous location
        public event Events.Node.MediaSetHandler MediaSet;                      // a media object was set on a node
        public event Events.Node.DeletedNodeHandler DeletedPhraseNode;          // deleted a phrase node 
        public event Events.Node.BlockChangedTimeHandler BlockChangedTime;      // a block's time has changed        

        //md: toc clipboard stuff
        public event Events.Node.CutNodeHandler CutTOCNode;
        public event Events.Node.MovedNodeHandler UndidCutTOCNode;
        public event Events.Node.CopiedNodeHandler CopiedTOCNode;
        public event Events.Node.CopiedNodeHandler UndidCopyTOCNode;
        public event Events.Node.PastedSectionNodeHandler PastedTOCNode;
        public event Events.Node.UndidPasteNodeHandler UndidPasteTOCNode;
      

        /// <summary>
        /// This flag is set to true if the project contains modifications that have not been saved.
        /// </summary>
        public bool Unsaved
        {
            get
            {
                return mUnsaved;
            }
        }

        /// <summary>
        /// The metadata of the project.
        /// </summary>
        public SimpleMetadata Metadata
        {
            get
            {
                return mMetadata;
            }
        }

        /// <summary>
        /// The path to the XUK file for this project.
        /// </summary>
        public string XUKPath
        {
            get
            {
                return mXUKPath;
            }
        }

        /// <summary>
        /// For debug purposes--should go away.
        /// </summary>
        public CoreNode RootNode
        {
            get
            {
                return (CoreNode)getPresentation().getRootNode();
            }
        }

        /// <summary>
        /// Last path under which the project was saved (different from the normal path when we save as)
        /// </summary>
        public string LastPath
        {
            get
            {
                return mLastPath;
            }
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

            //md:
            mClipboard = null;

            // Use our own property factory so that we can create custom properties
            getPresentation().setPropertyFactory(new ObiPropertyFactory(getPresentation()));
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
            typeProp.NodeStatus = NodeStatus.Used;
            getPresentation().getRootNode().setProperty(typeProp);
            NodeInformationProperty rootType = (NodeInformationProperty)getPresentation().getRootNode().getProperty(typeof(NodeInformationProperty));

            if (createTitle)
            {
                CoreNode node = CreateSectionNode();
                GetTextMedia(node).setText(title);
                getPresentation().getRootNode().appendChild(node);
            }
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Opened));
            SaveAs(mXUKPath);
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
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        #region TOC event handlers

        // Here are the event handlers for request sent by the GUI when editing the TOC.
        // Every request is passed to a method that uses mostly the same arguments,
        // which can also be called directly by a command for undo/redo purposes.
        // When we are done, a synchronization event is sent back.
        // (As well as a state change event.)

        /// <summary>
        /// Create a sibling section for a given section.
        /// The context node may be null if this is the first node that is added, in which case
        /// we add a new child to the root (and not a sibling.)
        /// </summary>
        public void CreateSiblingSection(object origin, CoreNode contextNode)
        {
            CoreNode parent = (CoreNode)(contextNode == null ? getPresentation().getRootNode() : contextNode.getParent());
            CoreNode sibling = CreateSectionNode();
            if (contextNode == null)
            {
                parent.appendChild(sibling);
            }
            else
            {
                parent.insert(sibling, parent.indexOf(contextNode) + 1);
            }
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(sibling);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, sibling, parent.indexOf(sibling),
                visitor.Position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSection command = new Commands.TOC.AddSection(this, sibling, parent, parent.indexOf(sibling),
                visitor.Position);
           CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateSiblingSectionRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CreateSiblingSection(sender, e.Node);
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public void CreateChildSection(object origin, CoreNode parent)
        {
            CoreNode child = CreateSectionNode();
            if (parent == null) parent = getPresentation().getRootNode();
            parent.appendChild(child);
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(child);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, child, parent.indexOf(child), visitor.Position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSection command = new Commands.TOC.AddSection(this, child, parent, parent.indexOf(child),
                visitor.Position);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateChildSectionRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CreateChildSection(sender, e.Node);
        }

        /// <summary>
        /// Add a section that had previously been added.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="originalLabel"></param>
        public void AddExistingSection(CoreNode node, CoreNode parent, int index, int position, string originalLabel)
        {
            if (node.getParent() == null) parent.insert(node, index);
            if (originalLabel != null) Project.GetTextMedia(node).setText(originalLabel);
            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(this, node, index, position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Readd a section node that was previously delete and restore all its contents.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void UndeleteSectionNode(CoreNode node, CoreNode parent, int index, int position)
        {
            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent, index, position);
            node.acceptDepthFirst(visitor);
        }

        /// <summary>
        /// Remove a node from the core tree. It is detached from the tree and the
        /// </summary>
        public void RemoveNode(object origin, CoreNode node)
        {
            if (node != null)
            {
                Commands.TOC.DeleteSection command = null;
                if (origin != this)
                {
                    CoreNode parent = (CoreNode)node.getParent();
                    Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);
                    command = new Commands.TOC.DeleteSection(this, node, parent, parent.indexOf(node), visitor.Position);
                }
                node.detach();
                DeletedNode(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }
        
        public void RemoveNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            RemoveNode(sender, e.Node);
        }

        /// <summary>
        /// Move a node up in the TOC.
        /// </summary>
        public void MoveNodeUp(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionUp command = null;
            
            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionUp
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }
            
            bool succeeded = ExecuteMoveNodeUp(node);

            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// move the node up
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>a facade API function could do this for us</remarks>
        private bool ExecuteMoveNodeUp(CoreNode node)
        {
            CoreNode newParent = null;
            int newIndex = 0;

            int currentIndex = ((CoreNode)node.getParent()).indexOf(node);

            //if it is the first node in its list
            //change its level and move it to be the previous sibling of its parent
            if (currentIndex == 0)
            {
                //it will be a sibling of its parent (soon to be former parent)
                if (node.getParent().getParent() != null)
                {
                    newParent = (CoreNode)node.getParent().getParent();

                    newIndex = newParent.indexOf((CoreNode)node.getParent());
                    
                }
            }
            else
            {
                //keep our current parent
                newParent = (CoreNode)node.getParent();
                newIndex = currentIndex - 1;
            }

            if (newParent != null)
            {
                CoreNode movedNode = (CoreNode)node.detach();
                newParent.insert(movedNode, newIndex);
                return true;
            }
            else
            {
                return false;
            }
         }
        
        public void MoveNodeUpRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveNodeUp(sender, e.Node);
        }

        /// <summary>
        /// reposition the node at the index under its given parent
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void UndoMoveNode(CoreNode node, CoreNode parent, int index, int position)
        {
            if (node.getParent() != null) node.detach();
            parent.insert(node, index);
            
            UndidMoveNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Undo increase level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UndoIncreaseNodeLevel(CoreNode node, CoreNode parent, int index, int position)
        {
            UndoMoveNode(node, parent, index, position);
        }

        public void MoveNodeDown(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionDown command = null;
          
            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionDown
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            bool succeeded = ExecuteMoveNodeDown(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position));

                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Move a node down in the presentation. If it has a younger sibling, then they swap
        /// places.  If not, it changes level and becomes a younger sibling of its parent.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        ///<remarks>a facade API function could do this for us</remarks>
        private bool ExecuteMoveNodeDown(CoreNode node)
        {
            CoreNode newParent = null;
            int newIndex = 0;

            int currentIndex = ((CoreNode)node.getParent()).indexOf(node);

            //if it is the last node in its list
            //change its level and move it to be the next sibling of its parent
            if (currentIndex == node.getParent().getChildCount() - 1)
            {
                //it will be a sibling of its parent (soon to be former parent)
                if (node.getParent().getParent() != null)
                {
                    newParent = (CoreNode)node.getParent().getParent();
                    newIndex = newParent.indexOf((CoreNode)node.getParent()) + 1;
                }
            }
            else
            {
                //keep our current parent
                newParent = (CoreNode)node.getParent();
                newIndex = currentIndex + 1;
            }

            if (newParent != null)
            {
                CoreNode movedNode = (CoreNode)node.detach();
                newParent.insert(movedNode, newIndex);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MoveNodeDownRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveNodeDown(sender, e.Node);
        }

        public void IncreaseNodeLevel(object origin, CoreNode node)
        {
            Commands.TOC.IncreaseSectionLevel command = null;  

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.IncreaseSectionLevel
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            bool succeeded = ExecuteIncreaseNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                //IncreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (origin, node, newParent, newParent.indexOf(node), visitor.Position));

                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified)); 
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  // JQ
            }
           
        }

        /// <summary>
        /// Move the node "in"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>a facade API function could do this for us</remarks>
        private bool ExecuteIncreaseNodeLevel(CoreNode node)
        {
            int nodeIndex = ((CoreNode)node.getParent()).indexOf(node);

            //the node's level can be increased if it has an older sibling
            if (nodeIndex == 0)
            {
                return false;
            }

            CoreNode newParent = ((CoreNode)node.getParent()).getChild(nodeIndex - 1);

            if (newParent != null)
            {
                CoreNode movedNode = (CoreNode)node.detach();
                newParent.appendChild(movedNode);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void IncreaseNodeLevelRequested(object sender, Events.Node.NodeEventArgs e)
        {
            IncreaseNodeLevel(sender, e.Node);
        }

        public void DecreaseNodeLevel(object origin, CoreNode node)
        {
            Commands.TOC.DecreaseSectionLevel command = null;  

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.DecreaseSectionLevel
                    (this, node, parent, parent.indexOf(node), visitor.Position, node.getChildCount());
            }

            bool succeeded = ExecuteDecreaseNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                DecreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  
            }
        }

       /// <summary>
       /// move the node "out"
       /// </summary>
       /// <param name="node"></param>
       /// <returns></returns>
        ///<remarks>a facade API function could do this for us</remarks>
        private bool ExecuteDecreaseNodeLevel(CoreNode node)
        {
            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (node.getParent() == null ||
                node.getParent().Equals(node.getPresentation().getRootNode()))
            {
                return false;
            }

            ArrayList futureChildren = new ArrayList();
            int nodeIndex = ((CoreNode)node.getParent()).indexOf(node);

            int numChildren = node.getParent().getChildCount();

            //make copies of our future children, and remove them from the tree
            for (int i = numChildren-1; i>nodeIndex; i--)
            {
               futureChildren.Add(node.getParent().getChild(i).detach());
            }
            //since the list was built in backwards order, rearrange it
            futureChildren.Reverse();

            CoreNode newParent = (CoreNode)node.getParent().getParent();
            int newIndex = newParent.indexOf((CoreNode)node.getParent()) + 1;
            
            CoreNode clone = (CoreNode)node.detach();

            newParent.insert(clone, newIndex);
            
            foreach (object childnode in futureChildren)
            {
                clone.appendChild((CoreNode)childnode);
            }

            return true;

        }

        public void DecreaseNodeLevelRequested(object sender, Events.Node.NodeEventArgs e)
        {
            DecreaseNodeLevel(sender, e.Node);
        }

        /// <summary>
        /// undo decrease node level is a bit tricky because we might have to move some of the node's
        /// children out a level, but only if they were newly adopted after the decrease level action happened. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="childCount">number of children this node used to have before the decrease level action happened</param>
        //added by marisa
        public void UndoDecreaseSectionLevel(CoreNode node, CoreNode parent, int index, int position, int originalChildCount)
        {
            //error-checking
            if (node.getChildCount() < originalChildCount)
            {
                //this would be a pretty strange thing to have happen.
                //todo: throw an exception?  
                return;
            }

            //detach the non-original children (child nodes originalChildCount...n-1)
            ArrayList nonOriginalChildren = new ArrayList();
            int totalNumChildren = node.getChildCount();

            for (int i = totalNumChildren - 1; i >= originalChildCount; i--)
            {
                CoreNode child = (CoreNode)node.getChild(i);
                if (child != null)
                {
                    nonOriginalChildren.Add(child);
                    child.detach();
                }
            }

            //this array was built backwards, so reverse it
            nonOriginalChildren.Reverse();

            //insert the node back in its old location
            node.detach();
            parent.insert(node, index);

            MovedNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position));
            
            Visitors.SectionNodePosition visitor = null;
            
            //reattach the children
            for (int i = 0; i < nonOriginalChildren.Count; i++)
            {
                parent.appendChild((CoreNode)nonOriginalChildren[i]);
                //find the position for the first one
                if (i == 0)
                {
                    visitor = new Visitors.SectionNodePosition(node);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);
                }

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, (CoreNode)nonOriginalChildren[i], parent, 
                    parent.getChildCount() - 1, visitor.Position + i));
            }
        }

        /// <summary>
        /// Change the text label of a node.
        /// </summary>
        public void RenameNode(object origin, CoreNode node, string label)
        {
            TextMedia text = GetTextMedia(node);
            Commands.TOC.Rename command = origin == this ? null : new Commands.TOC.Rename(this, node, text.getText(), label);
            GetTextMedia(node).setText(label);
            RenamedNode(this, new Events.Node.RenameNodeEventArgs(origin, node, label));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameNodeRequested(object sender, Events.Node.RenameNodeEventArgs e)
        {
            RenameNode(sender, e.Node, e.Label);
        }

        //md 20060810
        public void CutTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            DoCutTOCNode(sender, e.Node);
        }

        //md 20060810
        public void DoCutTOCNode(object origin, CoreNode node)
        {
            if (node == null) return;
           
            CoreNode parent = (CoreNode)node.getParent();
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            //we need to save the state of the node before it is altered
            Commands.TOC.CutSection command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CutSection
                 (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            mClipboard = node;
            node.detach();

            CutTOCNode(this, new Events.Node.NodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  
        }

        //md 20060810
        public void UndoCutNode(CoreNode node, CoreNode parent, int index, int position)
        {
            if (node.getParent() != null) node.detach();
            parent.insert(node, index);

            UndidCutTOCNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        public void CopyTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CopyTOCNode(sender, e.Node);
        }

        //md 20060810
        public void CopyTOCNode(object origin, CoreNode node)
        {
            if (node == null) return;

            Commands.TOC.CopySection command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CopySection(this, node);
            }

            //the actual copy operation
            mClipboard = node.copy(true);

            CopiedTOCNode(this, new Events.Node.NodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  
        }

        //md 20060810
        public void UndoCopyTOCNode(CoreNode node)
        {
            mClipboard = null;

            UndidCopyTOCNode(this, new Events.Node.NodeEventArgs(this, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        public void PasteTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            PasteTOCNode(sender, e.Node);
        }

        //md 20060810
        //"paste" will paste the clipboard contents as the first child of the given node
        public void PasteTOCNode(object origin, CoreNode parent)
        {
            if (parent == null) return;

            Commands.TOC.PasteSection command = null;
            
            CoreNode pastedSection = mClipboard;

            //don't clear the clipboard, we can use it again

            if (origin != this)
            {
                command = new Commands.TOC.PasteSection(this, pastedSection, parent);
            }

            //the actual paste operation
            parent.insert(pastedSection, 0);

            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(pastedSection);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            PastedTOCNode(this, new Events.Node.AddedSectionNodeEventArgs
                (origin, pastedSection, parent.indexOf(pastedSection), visitor.Position));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  
        }

        //md 20060810
        public void UndoPasteTOCNode(CoreNode node)
        {
            mClipboard = node.copy(true);

            node.detach();

            UndidPasteTOCNode(this, new Events.Node.NodeEventArgs(this, mClipboard));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        
        }

        // Find the channel and set the media object.
        // As this may fail, return true if the change was really made or false otherwise.
        // Throw an exception if the channel could not be found (JQ)
        internal bool DidSetMedia(object origin, CoreNode node, string channel, IMedia media)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                IChannel ch = (IChannel)channelsList[i];
                if (ch.getName() == channel)
                {
                    Commands.Command command = null;
                    if (GetNodeType(node) == NodeType.Phrase && channel == AnnotationChannel)
                    {
                        // we are renaming a phrase node
                        Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
                        string old = mAssManager.RenameAsset(asset, ((TextMedia)media).getText());
                        if (old == asset.Name) return false;
                        command = new Commands.Strips.RenamePhrase(this, node);
                    }
                    channelsProp.setMedia(ch, media);
                    MediaSet(this, new Events.Node.SetMediaEventArgs(origin, node, channel, media));
                    if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
                    mUnsaved = true;
                    StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                    // make a command here
                    return true;
                }
            }
            // the channel was not found when it should have been...
            throw new Exception(String.Format(Localizer.Message("channel_not_found"), channel));
        }

        /// <summary>
        /// Set the media on a given channel of a node.
        /// Cancel the event the change could not be made (e.g. renaming a block.)
        /// </summary>
        /// <remarks>JQ</remarks>
        public void SetMediaRequested(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (!DidSetMedia(sender, e.Node, e.Channel, e.Media)) e.Cancel = true;
        }

        internal void RenameBlock(CoreNode node, string name)
        {
            TextMedia media = (TextMedia)GetMediaForChannel(node, AnnotationChannel);
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
            mAssManager.RenameAsset(asset, name);
            media.setText(asset.Name);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, AnnotationChannel, media));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Handle a request to split an asset. The event contains the original node that was split and the new asset
        /// created from the split. A new sibling to the original node is to be added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SplitAssetRequested(object sender, Events.Node.SplitNodeEventArgs e)
        {
            CoreNode newNode = CreatePhraseNode(e.NewAsset);
            CoreNode parent = (CoreNode)e.Node.getParent();
            int index = parent.indexOf(e.Node) + 1;
            parent.insert(newNode, index);
            UpdateSeq(e.Node);
            BlockChangedTime(this, new Events.Node.NodeEventArgs(e.Origin, e.Node));
            AddedPhraseNode(this, new Events.Node.AddedPhraseNodeEventArgs(e.Origin, newNode, index));
            Commands.Strips.SplitPhrase command = new Commands.Strips.SplitPhrase(this, e.Node, newNode);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Merge two nodes.
        /// </summary>
        public void MergeNodesRequested(object sender, Events.Node.MergeNodesEventArgs e)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(e.Node);
            Assets.AudioMediaAsset next = GetAudioMediaAsset(e.Next);
            Commands.Strips.MergePhrases command = new Commands.Strips.MergePhrases(this, e.Node, e.Next);
            mAssManager.MergeAudioMediaAssets(asset, next);
            UpdateSeq(e.Node);
            BlockChangedTime(this, new Events.Node.NodeEventArgs(e.Origin, e.Node));
            DeletedPhraseNode(this, new Events.Node.NodeEventArgs(e.Origin, e.Next));
            e.Next.detach();
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        #endregion

        /// <summary>
        /// Create a new section node with a default text label. The node is not attached to anything.
        /// Add a node information custom property as well.
        /// </summary>
        /// <returns>The created node.</returns>
        private CoreNode CreateSectionNode()
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia text = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            text.setText(Localizer.Message("default_section_label"));
            prop.setMedia(mTextChannel, text);
            NodeInformationProperty typeProp =
                (NodeInformationProperty)getPresentation().getPropertyFactory().createProperty("NodeInformationProperty",
                ObiPropertyFactory.ObiNS);
            typeProp.NodeType = NodeType.Section;
            typeProp.NodeStatus = NodeStatus.Used;
            node.setProperty(typeProp);
            return node;
        }

        /// <summary>
        /// Create a new phrase node from an asset.
        /// Add a default annotation with the name of the asset.
        /// Add a seq media object with the clips of the audio asset. Do not forget to set begin/end time explicitely.
        /// Add a node information custom property as well.
        /// </summary>
        /// <param name="asset">The asset for the phrase.</param>
        /// <returns>The created node.</returns>
        private CoreNode CreatePhraseNode(Assets.AudioMediaAsset asset)
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia annotation = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            annotation.setText(asset.Name);
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
        /// Make a new sequence media object for the asset of this node.
        /// The sequence media object is simply a translation of the list of clips.
        /// </summary>
        private void UpdateSeq(CoreNode node)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            SequenceMedia seq =
                (SequenceMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.EMPTY_SEQUENCE);
            foreach (Assets.AudioClip clip in asset.Clips)
            {
                AudioMedia audio = (AudioMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.AUDIO);
                UriBuilder builder = new UriBuilder();
                builder.Scheme = "file";
                builder.Path = clip.Path;
                Uri relUri = mAssManager.BaseURI.MakeRelativeUri(builder.Uri);
                audio.setLocation(new MediaLocation(relUri.ToString()));
                audio.setClipBegin(new Time((long)Math.Round(clip.BeginTime)));
                audio.setClipEnd(new Time((long)Math.Round(clip.EndTime)));
                seq.appendItem(audio);
            }
            prop.setMedia(mAudioChannel, seq);
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
        /// Get the actual audio media object for a phrase node.
        /// </summary>
        /// <param name="node">The node for which we want the asset.</param>
        /// <returns>The asset or null if it could not be found.</returns>
        public static Assets.AudioMediaAsset GetAudioMediaAsset(CoreNode node)
        {
            AssetProperty prop = (AssetProperty)node.getProperty(typeof(AssetProperty));
            if (prop != null)
            {
                return prop.Asset;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Debug function for easy recording
        /// </summary>
        /// <param name="settings">Settings for recording</param>
        internal void StartRecording(Settings settings)
        {
            Dialogs.Record dialog = new Dialogs.Record(settings.AudioChannels, settings.SampleRate, settings.BitDepth, mAssManager);
            dialog.ShowDialog();
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
    }
}