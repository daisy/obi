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
        public Project(string xukPath, string title, string id, UserProfile userProfile)
        {
            mUnsaved = false;
            mXUKPath = xukPath;
            mMetadata = new SimpleMetadata(title, id, userProfile);
            mAudioChannel = getPresentation().getChannelFactory().createChannel(AUDIO_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel(TEXT_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            mAnnotationChannel = getPresentation().getChannelFactory().createChannel(ANNOTATION_CHANNEL);
            getPresentation().getChannelsManager().addChannel(mAnnotationChannel);
        }

        /// <summary>
        /// Save the project to a XUK file.
        /// </summary>
        public void Save()
        {
            saveXUK(new Uri(mXUKPath));
        }

        /// <summary>
        /// Get a short name from a given title. Usable for XUK file and project directory filename.
        /// </summary>
        /// <param name="title">Complete title.</param>
        /// <returns>The short version.</returns>
        public static string ShortName(string title)
        {
            return System.Text.RegularExpressions.Regex.Replace(title, @"[^a-zA-Z0-9_]", "_");
        }

        /// <summary>
        /// Simulate a modification of the project.
        /// </summary>
        public void Touch()
        {
            mUnsaved = true;
        }

        #region Event handlers

        /// <summary>
        /// Create a sibling section for a given section.
        /// </summary>
        public void CreateSiblingSection(object sender, Events.Node.AddSiblingSectionEventArgs e)
        {
            CoreNode sibling = CreateSectionNode();
            CoreNode parent = (CoreNode)e.ContextNode.getParent();
            parent.insert(sibling, parent.indexOf(e.ContextNode));
            UserControls.ICoreTreeView view = (UserControls.ICoreTreeView)sender;
            view.AddNewSiblingSection(sibling, e.ContextNode);
            view.BeginEditingNodeLabel(sibling);
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
            view.AddNewChildSection(child, e.ContextNode);
            view.BeginEditingNodeLabel(child);
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

        #endregion
    }
}