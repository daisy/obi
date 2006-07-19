using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi
{
    /// <summary>
    /// An Obi project is an Urakawa project (core tree and metadata)
    /// The core tree uses three channels:
    ///   1. an audio channel for audio media
    ///   2. a text channel for table of contents items (which will become NCX label in DTB)
    ///   3. an annotation channel for text annotation of other items in the book (e.g. phrases.)
    /// </summary>
    public class Project : urakawa.project.Project
    {
        private Channel mAudioChannel;       // handy pointer to the audio channel
        private Channel mTextChannel;        // handy pointer to the text channel 
        private Channel mAnnotationChannel;  // handy pointer to the annotation channel

        private bool mUnsaved;               // saved flag
        private string mXUKPath;             // path to the project XUK file
        private SimpleMetadata mMetadata;    // metadata for this project

        public static readonly string AUDIO_CHANNEL = "obi.audio";            // canonical name of the audio channel
        public static readonly string TEXT_CHANNEL = "obi.text";              // canonical name of the text channel
        public static readonly string ANNOTATION_CHANNEL = "obi.annotation";  // canonical name of the annotation channel

        public event Events.Sync.AddedChildNodeHandler AddedChildNode;
        public event Events.Sync.AddedSiblingNodeHandler AddedSiblingNode;
        public event Events.Sync.DeletedNodeHandler DeletedNode;

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
        /// Create a new project.
        /// </summary>
        public Project(string xukPath, string title, string id, UserProfile userProfile): base()
        {
            mUnsaved = false;
            mXUKPath = xukPath;
            mMetadata = CreateMetadata(title, id, userProfile);
            mAudioChannel = getPresentation().getChannelFactory().createChannel(AUDIO_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel(TEXT_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mAnnotationChannel = getPresentation().getChannelFactory().createChannel(ANNOTATION_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAnnotationChannel);
        }

        /// <summary>
        /// Create a project from a XUK file.
        /// </summary>
        /// <param name="xukPath">The path of the XUK file.</param>
        public Project(string xukPath): base()
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
            }
            else
            {
                throw new Exception(String.Format(Localizer.Message("open_project_error_text"), xukPath)); 
            }
        }

        /// <summary>
        /// Find the channel with the given name. It must be unique.
        /// </summary>
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
        /// Create the metadata for the project.
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
            metaDate.setScheme("YYYY-MM-DD");
            AddMetadata("xuk:generator", "Obi+Urakawa toolkit; let's share the blame.");
            return metadata;
        }

        /// <summary>
        /// Update the metadata of the project given the simple metadata object.
        /// At the moment, the metadata is really updated only when the project is saved.
        /// Also, this method is very ugly.
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
        /// </summary>
        /// <param name="name">The name of the metadata property.</param>
        /// <param name="value">Its content, i.e. the value.</param>
        private urakawa.project.Metadata AddMetadata(string name, string value)
        {
            urakawa.project.Metadata meta = (urakawa.project.Metadata)getMetadataFactory().createMetadata("Metadata");
            meta.setName(name);
            meta.setContent(value);
            this.appendMetadata(meta);
            return meta;
        }



        /// <summary>
        /// Save the project to a XUK file.
        /// </summary>
        public void Save()
        {
            UpdateMetadata();
            saveXUK(new Uri(mXUKPath));
            mUnsaved = false;
        }

        /// <summary>
        /// Get a short name from a given title. Usable for XUK file and project directory filename.
        /// </summary>
        /// <param name="title">Complete title.</param>
        /// <returns>The short version.</returns>
        public static string ShortName(string title)
        {
            string shrtnm = System.Text.RegularExpressions.Regex.Replace(title, @"[^a-zA-Z0-9]+", "_");
            shrtnm = System.Text.RegularExpressions.Regex.Replace(shrtnm, "^_", "");
            shrtnm = System.Text.RegularExpressions.Regex.Replace(shrtnm, "_$", "");
            return shrtnm;
        }

        /// <summary>
        /// Simulate a modification of the project.
        /// </summary>
        public void Touch()
        {
            mUnsaved = true;
        }

        #region Undo and redo



        #endregion

        #region TOC event handlers

        /// <summary>
        /// Create a sibling section for a given section.
        /// </summary>
        public void CreateSiblingSection(object sender, Events.Node.AddSiblingSectionEventArgs e)
        {
            CoreNode sibling = CreateSectionNode();
            //UserControls.ICoreTreeView view = (UserControls.ICoreTreeView)sender;
            if (e.ContextNode == null)
            {
                getPresentation().getRootNode().appendChild(sibling);
                AddedChildNode(this, new Events.Sync.AddedChildNodeEventArgs(sender, sibling));
                //view.AddNewChildSection(sibling, null);
            }
            else
            {
                CoreNode parent = (CoreNode)e.ContextNode.getParent();
                parent.insert(sibling, parent.indexOf(e.ContextNode) + 1);
                AddedSiblingNode(this, new Events.Sync.AddedSiblingNodeEventArgs(sender, sibling, e.ContextNode));
                //view.AddNewSiblingSection(sibling, e.ContextNode);
            }
            //view.BeginEditingNodeLabel(sibling);
            mUnsaved = true;
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public void CreateChildSection(object sender, Events.Node.AddChildSectionEventArgs e)
        {
            CoreNode child = CreateSectionNode();
            CoreNode parent = e.ContextNode;
            if (parent == null) parent = getPresentation().getRootNode();
            parent.appendChild(child);
            UserControls.ICoreTreeView view = (UserControls.ICoreTreeView)sender;
            AddedChildNode(this, new Events.Sync.AddedChildNodeEventArgs(sender, child));
            //view.AddNewChildSection(child, e.ContextNode);
            //view.BeginEditingNodeLabel(child);
            mUnsaved = true;
        }

        /// <summary>
        /// Create a new section node with a default text label.
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
        /// <param name="node"></param>
        public void RemoveNode(object sender, Events.Node.DeleteSectionEventArgs e)
        {
            e.Node.detach();
            DeletedNode(this, new Events.Sync.DeletedNodeEventArgs(sender, e.Node));
            // ((UserControls.ICoreTreeView)sender).DeleteSectionNode(e.Node);
            mUnsaved = true;
        }

        /// <summary>
        /// Move a node up in the TOC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MoveNodeUp(object sender, Events.Node.MoveSectionUpEventArgs e)
        {
            //((UserControls.ICoreTreeView)sender).MoveCurrentSectionUp();
            //mUnsaved = true;
        }

        #endregion
    }
}