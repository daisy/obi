using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using urakawa.core;
using urakawa.media;
using VirtualAudioBackend;

namespace Obi
{
    /// <summary>
    /// An Obi project is an Urakawa project (core tree and metadata)
    /// It also knows where to save itself and has a simpler set of metadata.
    /// The core tree uses three channels:
    ///   1. an audio channel for audio media
    ///   2. a text channel for table of contents items (which will become NCX label in DTB)
    ///   3. an annotation channel for text annotation of other items in the book (e.g. phrases.)
    /// So we keep a handy pointer to those.
    /// TODO: we should subclass the nodes, so that our tree has a root (CoreNode), section nodes
    /// and phrase nodes.
    /// </summary>
    public class Project : urakawa.project.Project
    {
        private Channel mAudioChannel;       // handy pointer to the audio channel
        private Channel mTextChannel;        // handy pointer to the text channel 
        private Channel mAnnotationChannel;  // handy pointer to the annotation channel

        private AssetManager mAssManager;    // the asset manager
        private string mAssPath;             // the path to the asset manager directory

        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        private string mLastPath;            // last path to which the project was saved (see save as)
        private SimpleMetadata mMetadata;    // metadata for this project

        public static readonly string AudioChannel = "obi.audio";            // canonical name of the audio channel
        public static readonly string TextChannel = "obi.text";              // canonical name of the text channel
        public static readonly string AnnotationChannel = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Project.CommandCreatedHandler CommandCreated;   // a new command must be added to the command manager

        public event Events.Node.AddedSectionNodeHandler AddedSectionNode;  // a section node was added to the TOC
        public event Events.Node.DeletedNodeHandler DeletedNode;            // a node was deleted from the presentation
        public event Events.Node.RenamedNodeHandler RenamedNode;            // a node was renamed in the presentation
        public event Events.Node.MovedNodeHandler MovedNode;            // a node was moved in the presentation
        public event Events.Node.DecreasedNodeLevelHandler DecreasedNodeLevel; //a node's level was decreased in the presentation
        public event Events.Node.ImportedAssetHandler ImportedAsset;  // an asset was imported into the project
        public event Events.Node.MovedNodeHandler UndidMoveNode;    //a node was restored to its previous location

        public event Events.Node.MediaSetHandler MediaSet;  // a media object was set on a node
        
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
            getPresentation().setPropertyFactory(new AssetPropertyFactory(getPresentation()));
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
            mXUKPath = xukPath;
            mAssPath = GetAssetDirectory(xukPath);
            mAssManager = new AssetManager(mAssPath);
            mMetadata = CreateMetadata(title, id, userProfile);
            mAudioChannel = getPresentation().getChannelFactory().createChannel(AudioChannel);
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel(TextChannel);
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mAnnotationChannel = getPresentation().getChannelFactory().createChannel(AnnotationChannel);
            getPresentation().getChannelsManager().addChannel(mAnnotationChannel);

            // Give a custom property to the root node to make it a RootNode.
            NodeTypeProperty typeProp =
                (NodeTypeProperty)getPresentation().getPropertyFactory().createProperty("NodeTypeProperty");
            typeProp.Type = NodeType.RootNode;
            getPresentation().getRootNode().setProperty(typeProp);
            NodeTypeProperty rootType = (NodeTypeProperty)getPresentation().getRootNode().getProperty(typeof(NodeTypeProperty));

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
                        default:
                            break;
                    }
                }
                if (mAssPath == null)
                {
                    mAssPath = GetAssetDirectory(mXUKPath);
                    AddMetadata("obi:assetsdir", mAssPath);
                }
                mAssManager = new AssetManager(mAssPath);
                // Recreate the assets from the phrase nodes
                getPresentation().getRootNode().acceptDepthFirst(new Visitors.AssetVisitor());
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
            AddMetadata("xuk:generator", "Obi+Urakawa toolkit; let's share the blame.");
            AddMetadata("obi:assetsdir", mAssPath);
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
                urakawa.project.Metadata meta = (urakawa.project.Metadata)getMetadataFactory().createMetadata("Metadata");
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
        /// Save the project to a XUK file.
        /// </summary>
        /// <remarks>
        /// We probably need to catch exceptions here.
        /// </remarks>
        public void SaveAs(string path)
        {
            UpdateMetadata();
            saveXUK(new Uri(path));
            mUnsaved = false;
            mLastPath = path;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Saved));
        }

        /// <summary>
        /// Close the project. This doesn't do much except generate a Closed event.
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
            NodePositionVisitor visitor = new NodePositionVisitor(sibling);
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
            System.Diagnostics.Debug.WriteLine("create sibling request from " + sender.ToString() + " with hash: " + sender.GetHashCode());
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
            NodePositionVisitor visitor = new NodePositionVisitor(child);
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
        public void ReaddSection(CoreNode node, CoreNode parent, int index, int position, string originalLabel)
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
        /// Remove a node from the core tree (simply detach it, and synchronize the views.)
        /// </summary>
        public void RemoveNode(object origin, CoreNode node)
        {
            if (node != null)
            {
                Commands.TOC.DeleteSection command = null;
                if (origin != this)
                {
                    CoreNode parent = (CoreNode)node.getParent();
                    NodePositionVisitor visitor = new NodePositionVisitor(node);
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
                NodePositionVisitor visitor = new NodePositionVisitor(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionUp
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }
            
            bool succeeded = ExecuteMoveNodeUp(node);

            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                NodePositionVisitor visitor = new NodePositionVisitor(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        //a facade API function could do this for us
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

        /// <summary>
        /// Undo decrease level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UndoDecreaseSectionLevel(CoreNode node, CoreNode parent, int index, int position)
        {
        }

        public void MoveNodeDown(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionDown command = null;
          
            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                NodePositionVisitor visitor = new NodePositionVisitor(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionDown
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            //a facade API function could do this for us
            bool succeeded = ExecuteMoveNodeDown(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                NodePositionVisitor visitor = new NodePositionVisitor(node);
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
                NodePositionVisitor visitor = new NodePositionVisitor(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.IncreaseSectionLevel
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            //a facade API function could do this for us
            bool succeeded = ExecuteIncreaseNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                NodePositionVisitor visitor = new NodePositionVisitor(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                //IncreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (origin, node, newParent, newParent.indexOf(node), visitor.Position));

                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified)); 
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
           
        }

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
            //a facade API function could do this for us
            bool succeeded = ExecuteDecreaseNodeLevel(node);
            if (succeeded)
            {
                DecreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            }
        }

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

        // Find the channel and set the media object.
        // Throw an exception if the channel could not be found (JQ)
        private void SetMedia(object origin, CoreNode node, string channel, IMedia media)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                IChannel ch = (IChannel)channelsList[i];
                if (ch.getName() == channel)
                {
                    channelsProp.setMedia(ch, media);
                    MediaSet(this, new Events.Node.SetMediaEventArgs(origin, node, channel, media));
                    mUnsaved = true;
                    StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                    // make a command here
                    return;
                }
            }
            // the channel was not found when it should have been...
            throw new Exception(String.Format(Localizer.Message("channel_not_found"), channel));
        }

        /// <summary>
        /// Set the media on a given channel of a node.
        /// </summary>
        /// <remarks>JQ</remarks>
        public void SetMediaRequested(object sender, Events.Node.SetMediaEventArgs e)
        {
            SetMedia(sender, e.Node, e.Channel, e.Media);
        }

        /// <summary>
        /// Create a new phrase node and add it to the section node.
        /// </summary>
        /// <remarks>JQ</remarks>
        public void ImportPhraseRequested(object sender, Events.Strip.ImportAssetEventArgs e)
        {
            ArrayList list = new ArrayList(1);
            list.Add(new AudioClip(e.AssetPath));
            AudioMediaAsset asset = mAssManager.NewAudioMediaAsset(list);
            CoreNode node = CreatePhraseNode(asset);
            e.SectionNode.appendChild(node);
            ImportedAsset(this, new Events.Node.NodeEventArgs(sender, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }
        
        #endregion

        /// <summary>
        /// Create a new section node with a default text label. The node is not attached to anything.
        /// TODO: this should be a constructor/factory method for our derived node class.
        /// </summary>
        /// <returns>The created node.</returns>
        private CoreNode CreateSectionNode()
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia text = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            text.setText(Localizer.Message("default_section_label"));
            prop.setMedia(mTextChannel, text);
            NodeTypeProperty typeProp = (NodeTypeProperty)getPresentation().getPropertyFactory().createProperty("NodeTypeProperty");
            typeProp.Type = NodeType.SectionNode;
            node.setProperty(typeProp);
            return node;
        }

        /// <summary>
        /// Create a new phrase node from an asset.
        /// Add a default annotation with the name of the file (should be the original name, not the internal one.)
        /// Add a seq media object with the clips of the audio asset. Do not forget to set begin/end time explicitely.
        /// </summary>
        /// <param name="asset">The asset for the phrase.</param>
        /// <returns>The created node.</returns>
        private CoreNode CreatePhraseNode(AudioMediaAsset asset)
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia annotation = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            annotation.setText(asset.Name);
            prop.setMedia(mAnnotationChannel, annotation);
            SequenceMedia seq =
                (SequenceMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.EMPTY_SEQUENCE);
            AudioMedia audio = (AudioMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.AUDIO);
            for (int i = 0; i < asset.m_alClipList.Count; ++i)
            {
                AudioClip clip = asset.GetClip(i);
                audio.setLocation(new MediaLocation(clip.Path));
                audio.setClipBegin(new Time((long)Math.Round(clip.BeginTime)));
                audio.setClipEnd(new Time((long)Math.Round(clip.EndTime)));
                seq.appendItem(audio);
            }
            prop.setMedia(mAudioChannel, seq);
            AssetProperty assProp = (AssetProperty)getPresentation().getPropertyFactory().createProperty("AssetProperty");
            assProp.Asset = asset;
            node.setProperty(assProp);
            NodeTypeProperty typeProp = (NodeTypeProperty)getPresentation().getPropertyFactory().createProperty("NodeTypeProperty");
            typeProp.Type = NodeType.PhraseNode;
            node.setProperty(typeProp);
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
            NodeTypeProperty prop = (NodeTypeProperty)node.getProperty(typeof(NodeTypeProperty));
            if (prop != null)
            {
                return prop.Type;
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
        public static AudioMediaAsset GetAudioMediaAsset(CoreNode node)
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
    }
}