using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi
{
    public class Project : urakawa.Project
    {
        private Uri path;          // path to .xuk file
        private int changes;       // changes since last save
        private bool initialized;  // flag set once the project is properly initialized


        /// <summary>
        /// Create a new empty project.
        /// </summary>
        public Project() : base()
        {
            this.path = null;
            this.changes = 0;
            setDataModelFactory(new DataModelFactory());
            setPresentation(getDataModelFactory().createPresentation(), 0);
            urakawa.property.channel.Channel audioChannel = getPresentation(0).getChannelFactory().createChannel();
            audioChannel.setName("bobi.audio");
            getPresentation(0).getChannelsManager().addChannel(audioChannel);
            SetUndoRedoEvents();
            this.initialized = true;
        }

        /// <summary>
        /// Create a new, uninitialized project ready for reading.
        /// </summary>
        public Project(Uri path) : base()
        {
            Path = path;
            this.changes = 0;
            this.initialized = false;
            setDataModelFactory(new DataModelFactory());
        }


        /// <summary>
        /// Create a new audio node by importing data from a file.
        /// </summary>
        public urakawa.core.TreeNode CreateAudioNode(string filename)
        {
            urakawa.Presentation p = getPresentation(0);
            urakawa.core.TreeNode node = p.getTreeNodeFactory().createNode(typeof(AudioNode).Name, DataModelFactory.NS);
            // Update the media data manager to accept this type of file
            if (!p.getMediaDataManager().getEnforceSinglePCMFormat())
            {
                System.IO.FileStream audioStream = System.IO.File.OpenRead(filename);
                urakawa.media.data.audio.PCMDataInfo info = urakawa.media.data.audio.PCMDataInfo.parseRiffWaveHeader(audioStream);
                audioStream.Close();
                p.getMediaDataManager().setDefaultPCMFormat(info);
                p.getMediaDataManager().setEnforceSinglePCMFormat(true);
            }
            // I'll go ahead and not comment this part
            urakawa.media.data.audio.AudioMediaData audio = p.getMediaDataFactory().createAudioMediaData();
            audio.appendAudioDataFromRiffWave(filename);
            urakawa.media.data.audio.ManagedAudioMedia media =
                (urakawa.media.data.audio.ManagedAudioMedia)p.getMediaFactory().createAudioMedia();
            media.setMediaData(audio);
            urakawa.property.channel.ChannelsProperty prop = p.getPropertyFactory().createChannelsProperty();
            prop.setMedia(FindChannel("bobi.audio"), media);
            node.addProperty(prop);
            return node;
        }

        /// <summary>
        /// True if there are changes since the last time the project was saved (or created.)
        /// </summary>
        public bool HasChanges { get { return this.changes != 0; } }

        /// <summary>
        /// True once the project has been properly initialized.
        /// </summary>
        public bool Initialized { get { return this.initialized; } }

        /// <summary>
        /// Get the number of tracks in the current project.
        /// </summary>
        public int NumberOfTracks { get { return getPresentation(0).getRootNode().getChildCount(); } }

        /// <summary>
        /// Open a XUK file at the set path.
        /// </summary>
        public void Open()
        {
            openXUK(Path);
            SetUndoRedoEvents();
            this.initialized = true;
        }

        public Uri Path
        {
            get { return this.path; }
            set
            {
                this.path = value;
                if (this.path != null && getNumberOfPresentations() > 0) getPresentation(0).setRootUri(this.path);
            }
        }

        /// <summary>
        /// Redo the last undone change.
        /// </summary>
        public void Redo()
        {
            urakawa.undo.UndoRedoManager redo = getPresentation(0).getUndoRedoManager();
            if (redo.canRedo()) redo.redo();
        }

        /// <summary>
        /// Save changes to the current path (if set.)
        /// </summary>
        public void Save()
        {
            if (Path != null)
            {
                saveXUK(Path);
                Changes(-this.changes);
            }
        }

        /// <summary>
        /// Undo the last change.
        /// </summary>
        public void Undo()
        {
            urakawa.undo.UndoRedoManager undo = getPresentation(0).getUndoRedoManager();
            if (undo.canUndo()) undo.undo();
        }


        // Keep track of the number of changes since open, and send an event when it changes.
        private void Changes(int n)
        {
            if (this.initialized)
            {
                this.changes += n;
                notifyChanged(new urakawa.events.DataModelChangedEventArgs(this));
            }
        }

        // Find a channel by name and return it. Return null when not found.
        public urakawa.property.channel.Channel FindChannel(string name)
        {
            urakawa.property.channel.Channel channel = null;
            foreach (urakawa.property.channel.Channel ch in getPresentation(0).getChannelsManager().getListOfChannels())
            {
                if (ch.getName() == name)
                {
                    channel = ch;
                    break;
                }
            }
            return channel;
        }

        // Send changes events when commands are executed or undone.
        private void Project_commandDone(object sender, urakawa.events.undo.DoneEventArgs e) { Changes(+1); }
        private void Project_commandReDone(object sender, urakawa.events.undo.ReDoneEventArgs e) { Changes(+1); }
        private void Project_commandUnDone(object sender, urakawa.events.undo.UnDoneEventArgs e) { Changes(-1); }

        // Set undo/redo events once the project is initialized.
        private void SetUndoRedoEvents()
        {
            getPresentation(0).getUndoRedoManager().commandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(Project_commandDone);
            getPresentation(0).getUndoRedoManager().commandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(Project_commandReDone);
            getPresentation(0).getUndoRedoManager().commandUnDone += new EventHandler<urakawa.events.undo.UnDoneEventArgs>(Project_commandUnDone);
        }
    }
}
