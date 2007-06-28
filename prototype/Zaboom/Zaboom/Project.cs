using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using urakawa;
using urakawa.core;
using urakawa.exception;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.utilities;
using urakawa.metadata;
using urakawa.properties.channel;

namespace Zaboom
{
    public class Project: urakawa.Project
    {
        private bool canSave;  // project has modifications to be saved
        private Uri path;      // path of the XUK file for this project

        public event StateChangedHandler StateChanged;  // sent when the project is modified, saved, open, or closed

        /// <summary>
        /// Create a blank project.
        /// </summary>
        /// <param name="path">Path of the XUK file for the project (will be created when saved.)</param>
        public Project(string path)
            : base(new Uri(Path.GetDirectoryName(path) + Path.DirectorySeparatorChar))
        {
            this.path = new Uri(path);
            this.canSave = true;
        }

        /// <summary>
        /// Create a new project with a given title.
        /// </summary>
        /// <param name="title">The title of the project.</param>
        /// <param name="path">Path of the XUK file for the project (will be created when saved.)</param>
        public Project(string title, string path)
            : this(path)
        {
            Title = title;
            Channel audio = getPresentation().getChannelFactory().createChannel(typeof(Channel).Name,
                urakawa.ToolkitSettings.XUK_NS);
            audio.setName(AUDIO_CHANNEL_NAME);
            getPresentation().getChannelsManager().addChannel(audio);
        }

        /// <summary>
        /// Add a tree node and mark the project as modified.
        /// </summary>
        /// <param name="node">The node to add.</param>
        /// <param name="parent">The parent node to add to.</param>
        /// <param name="index">The index at which the node is inserted.</param>
        public void AddTreeNode(TreeNode node, TreeNode parent, int index)
        {
            parent.insert(node, index);
            Modified();
        }

        /// <summary>
        /// The audio channel of the project.
        /// </summary>
        /// <exception cref="urakawa.exception.ChannelDoesNotExistException">Thrown when there is no channel by that name.</exception>
        /// <exception cref="TooManyChannelsException">Thrown when there are more than one channels with that name.</exception>
        public Channel AudioChannel { get { return GetSingleChannelByName(AUDIO_CHANNEL_NAME); } }

        /// <summary>
        /// Import an audio file to the project by creating a new node with audio from the file.
        /// The node is created but not actually added but a command is returned.
        /// </summary>
        /// <param name="path">Full path to the audio file to import.</param>
        /// <returns>The command for adding the node.</returns>
        public AddTreeNodeCommand ImportAudioFileCommand(string path)
        {
            Stream input = File.OpenRead(path);
            PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader(input);
            input.Close();
            getPresentation().getMediaDataManager().getDefaultPCMFormat().setBitDepth(info.getBitDepth());
            getPresentation().getMediaDataManager().getDefaultPCMFormat().setNumberOfChannels(info.getNumberOfChannels());
            getPresentation().getMediaDataManager().getDefaultPCMFormat().setSampleRate(info.getSampleRate());
            AudioMediaData data = (AudioMediaData)
                getPresentation().getMediaDataFactory().createMediaData(typeof(AudioMediaData));
            data.appendAudioDataFromRiffWave(path);
						ManagedAudioMedia media = (ManagedAudioMedia)
								getPresentation().getMediaFactory().createAudioMedia();
            media.setMediaData(data);
            Channel audio = GetSingleChannelByName(AUDIO_CHANNEL_NAME);
            ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            prop.setMedia(audio, media);
            TreeNode node = getPresentation().getTreeNodeFactory().createNode();
            node.setProperty(prop);
            TreeNode root = getPresentation().getRootNode();
            AddTreeNodeCommand command = new AddTreeNodeCommand(this, node, root, root.getChildCount());
            return command;
        }

        /// <summary>
        /// Open a XUK file into a blank project. The path to the file was given in the constructor.
        /// </summary>
        public void Open()
        {
            openXUK(this.path);
            canSave = false;
        }

        /// <summary>
        /// Remove the given node from the tree and mark the project as modified.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveTreeNode(TreeNode node)
        {
            node.getParent().removeChild(node);
            Modified();
        }

        /// <summary>
        /// Save the project to its XUK file. If the directory for the XUK file does not exist it is created.
        /// </summary>
        public void Save()
        {
            if (canSave)
            {
                string directory = Path.GetDirectoryName(path.LocalPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                saveXUK(path);
                canSave = false;
                if (StateChanged != null) StateChanged(this, new StateChangedEventArgs(StateChange.Saved));
            }
        }

        /// <summary>
        /// Get or set the title of the project. The corresponding metadata is updated accordingly.
        /// </summary>
        public string Title
        {
            get { return GetSingleMetadataContentFor(META_TITLE_NAME); }
            set
            {
                List<Metadata> list = getMetadataList(META_TITLE_NAME);
                Metadata metatitle = null;
                if (list.Count == 0)
                {
                    metatitle = getMetadataFactory().createMetadata();
                    metatitle.setName("zaboom:title");
                    appendMetadata(metatitle);
                }
                else if (list.Count == 1)
                {
                    metatitle = list[0];
                }
                else
                {
                    throw new Exception(String.Format("Expected 1 item for {0}, got {1}.", META_TITLE_NAME, list.Count));
                }
                metatitle.setContent(value);
            }
        }

        /// <summary>
        /// Title with an asterisk if there are unsaved changes.
        /// </summary>
        public string TitleSaved { get { return GetSingleMetadataContentFor(META_TITLE_NAME) + (canSave ? "*" : ""); } }

        #region utilities

        private static readonly string AUDIO_CHANNEL_NAME = "zaboom.audio";  // canonical audio channel name
        private static readonly string META_TITLE_NAME = "zaboom:title";     // metadata element name for title

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
            if (channels.Count == 0)
            {
                throw new ChannelDoesNotExistException(String.Format("No channel named \"{0}\"", name));
            }
            else if (channels.Count > 1)
            {
                throw new TooManyChannelsException(String.Format("Expected 1 channel for {0}, got {1}.", name, channels.Count));
            }
            return channels[0];
        }

        /// <summary>
        /// Get the content of the single metadata element by that name. 
        /// </summary>
        /// <param name="name">Name of the metadata element to access.</param>
        /// <returns>The content of the corresponding metadata element.</returns>
        private string GetSingleMetadataContentFor(string name)
        {
            List<Metadata> list = getMetadataList(name);
            if (list.Count != 1)
            {
                throw new Exception(String.Format("Expected 1 item for {0}, got {1}.", name, list.Count));
            }
            return list[0].getContent();
        }

        /// <summary>
        /// Send an event when the project was modified and update its canSave property.
        /// </summary>
        private void Modified()
        {
            canSave = true;
            if (StateChanged != null) StateChanged(this, new StateChangedEventArgs(StateChange.Modified));
        }

        #endregion
    }

    public enum StateChange { Closed, Modified, Opened, Saved };

    public class StateChangedEventArgs : EventArgs
    {
        public StateChange Change;
        public StateChangedEventArgs(StateChange change) { Change = change; }
    }

    public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);

    public class TooManyChannelsException : CheckedException
    {
        public TooManyChannelsException(string msg) : base(msg) { }
        public TooManyChannelsException(string msg, Exception e) : base(msg, e) { }
    }
}
