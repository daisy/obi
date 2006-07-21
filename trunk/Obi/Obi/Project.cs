using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

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

        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        private string mLastPath;            // last path to which the project was saved (see save as)
        private SimpleMetadata mMetadata;    // metadata for this project

        public static readonly string AUDIO_CHANNEL = "obi.audio";            // canonical name of the audio channel
        public static readonly string TEXT_CHANNEL = "obi.text";              // canonical name of the text channel
        public static readonly string ANNOTATION_CHANNEL = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Project.StateChangedHandler StateChanged;       // the state of the project changed (modified, saved...)
        public event Events.Sync.AddedChildNodeHandler AddedChildNode;      // a new child node was added to the presentation
        public event Events.Sync.AddedSiblingNodeHandler AddedSiblingNode;  // a new sibling node was added to the presentation
        public event Events.Sync.DeletedNodeHandler DeletedNode;            // a node was deleted from the presentation
        public event Events.Sync.RenamedNodeHandler RenamedNode;            // a node was renamed in the presentation

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
            mUnsaved = false;
            mXUKPath = null;
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
            mMetadata = CreateMetadata(title, id, userProfile);
            mAudioChannel = getPresentation().getChannelFactory().createChannel(AUDIO_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel(TEXT_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mAnnotationChannel = getPresentation().getChannelFactory().createChannel(ANNOTATION_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAnnotationChannel);
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
                mAudioChannel = FindChannel(AUDIO_CHANNEL);
                mTextChannel = FindChannel(TEXT_CHANNEL);
                mAnnotationChannel = FindChannel(ANNOTATION_CHANNEL);
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
                        default:
                            break;
                    }
                }
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
            CoreNode sibling = CreateSectionNode();
            if (contextNode == null)
            {
                getPresentation().getRootNode().appendChild(sibling);
                AddedChildNode(this, new Events.Sync.AddedChildNodeEventArgs(origin, sibling));
            }
            else
            {
                CoreNode parent = (CoreNode)contextNode.getParent();
                parent.insert(sibling, parent.indexOf(contextNode) + 1);
                AddedSiblingNode(this, new Events.Sync.AddedSiblingNodeEventArgs(origin, sibling, contextNode));
            }
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public void CreateSiblingSectionRequested(object sender, Events.Node.AddSiblingSectionEventArgs e)
        {
            CreateSiblingSection(sender, e.ContextNode);
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public void CreateChildSection(object origin, CoreNode parent)
        {
            CoreNode child = CreateSectionNode();
            if (parent == null) parent = getPresentation().getRootNode();
            parent.appendChild(child);
            AddedChildNode(this, new Events.Sync.AddedChildNodeEventArgs(origin, child));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public void CreateChildSectionRequested(object sender, Events.Node.AddChildSectionEventArgs e)
        {
            CreateChildSection(sender, e.ContextNode);
        }

        /// <summary>
        /// Create a new section node with a default text label. The node is not attached to anything.
        /// TODO: this should be a constructor/factory method for our derived node class.
        /// </summary>
        /// <returns>The created node.</returns>
        private CoreNode CreateSectionNode()
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia text = (TextMedia)getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            text.setText(Localizer.Message("default_section_label"));
            prop.setMedia(mTextChannel, text);
            return node;
        }

        /// <summary>
        /// Remove a node from the core tree (simply detach it, and synchronize the views.)
        /// </summary>
        public void RemoveNode(object origin, CoreNode node)
        {
            node.detach();
            DeletedNode(this, new Events.Sync.DeletedNodeEventArgs(origin, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }
        
        public void RemoveNodeRequested(object sender, Events.Node.DeleteSectionEventArgs e)
        {
            RemoveNode(sender, e.Node);
        }

        /// <summary>
        /// Move a node up in the TOC.
        /// </summary>
        public void MoveNodeUp(object origin, CoreNode node)
        {
            // doesn't do much yet
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public void MoveNodeUpRequested(object sender, Events.Node.MoveSectionUpEventArgs e)
        {
            MoveNodeUp(sender, e.ContextNode);
        }

        /// <summary>
        /// Change the text label of a node.
        /// </summary>
        public void RenameNode(object origin, CoreNode node, string label)
        {
            GetTextMedia(node).setText(label);
            RenamedNode(this, new Events.Sync.RenamedNodeEventArgs(origin, node, label));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public void RenameNodeRequested(object sender, Events.Node.RenameSectionEventArgs e)
        {
            RenameNode(sender, e.Node, e.Label);
        }

        #endregion

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
                if (channelName == Project.TEXT_CHANNEL)
                {
                    textChannel = (Channel)channelsList[i];
                    return (TextMedia)channelsProp.getMedia(textChannel);
                }
            }
            return null;
        }
    }
}