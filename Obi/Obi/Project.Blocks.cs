using System;
using System.Collections;
using System.IO;

using urakawa.core;
using urakawa.media;
using Obi.Assets;

namespace Obi
{
    public partial class Project
    {
        public Events.Strip.UpdateTimeHandler UpdateTime;

        #region clip board

        private CoreNode mBlockClipBoard;  // clip board for block nodes (phrases or pages)

        internal CoreNode BlockClipBoard
        {
            get { return mBlockClipBoard; }
            set { mBlockClipBoard = value; }
        }

        /// <summary>
        /// Cut a phrase node: delete it and store it in the clipboard.
        /// Issue a command an modify the project.
        /// </summary>
        internal void CutPhraseNode(object sender, Events.Node.NodeEventArgs e)
        {
            // create the command before storing the node in the clip board, otherwise the previous value is lost
            Commands.Strips.CutPhrase command = new Commands.Strips.CutPhrase(this, e.Node, (CoreNode)e.Node.getParent(),
                GetPhraseIndex(e.Node));
            mBlockClipBoard = e.Node;
            DeletePhraseNodeAndAsset(e.Node);
            BlockClipBoard = e.Node;
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// Create a command object.
        /// </summary>
        public void DeletePhraseNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CoreNode parent = (CoreNode)e.Node.getParent();
            int index = parent.indexOf(e.Node);
            Commands.Strips.DeletePhrase command = new Commands.Strips.DeletePhrase(this, e.Node, parent, index);
            CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            DeletePhraseNodeAndAsset(e.Node);
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Import an asset, create a node for it and add it at the given position in its section.
        /// The phrase is named after the imported file name.
        /// </summary>
        public void ImportAssetRequested(object sender, Events.Strip.ImportAssetEventArgs e)
        {
            AudioMediaAsset asset = mAssManager.ImportAudioMediaAsset(e.AssetPath);
            mAssManager.InsureRename(asset, Path.GetFileNameWithoutExtension(e.AssetPath));
            CoreNode node = CreatePhraseNode(asset);
            AddPhraseNode(node, e.SectionNode, e.Index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(this, node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Merge two adjacent phrase nodes: merge their assets into the first node's asset and remove the second node.
        /// </summary>
        public void MergeNodesRequested(object sender, Events.Node.MergeNodesEventArgs e)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(e.Node);
            Assets.AudioMediaAsset next = GetAudioMediaAsset(e.Next);
            // the command is created while the assets are not changed; there is time to copy the original asset before the
            // merge is done.
            Commands.Strips.MergePhrases command = new Commands.Strips.MergePhrases(this, e.Node, e.Next);
            mAssManager.MergeAudioMediaAssets(asset, next);
            UpdateSeq(e.Node);
            MediaSet(this, new Events.Node.SetMediaEventArgs(e.Origin, e.Node, Project.AudioChannel,
                GetMediaForChannel(e.Node, Project.AudioChannel)));
            DeletedPhraseNode(this, new Events.Node.NodeEventArgs(e.Origin, e.Next));
            e.Next.detach();
            TouchedNode(this, new Events.Node.NodeEventArgs(e.Origin, e.Node));
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        public enum Direction { Forward, Backward };

        /// <summary>
        /// Move a phrase node in either direction. May not succeed if there is nowhere to move in that direction.
        /// </summary>
        private void MovePhraseNodeRequested(object sender, Events.Node.NodeEventArgs e, Direction dir)
        {
            if (CanMovePhraseNode(e.Node, dir))
            {
                MovePhraseNode(e.Node, dir);
                Commands.Strips.MovePhrase command = new Commands.Strips.MovePhrase(this, e.Node, dir);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }
        /// <summary>
        /// Move a phrase node forward. May not succeed if the node is last of its kind.
        /// </summary>
        public void MovePhraseNodeForwardRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MovePhraseNodeRequested(sender, e, Direction.Forward);
        }

        /// <summary>
        /// Move a phrase node forward.
        /// </summary>
        public void MovePhraseNodeBackwardRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MovePhraseNodeRequested(sender, e, Direction.Backward);
        }

        /// <summary>
        /// Try to set the media on a given channel of a node.
        /// Cancel the event the change could not be made (e.g. renaming a block.)
        /// </summary>
        public void SetMediaRequested(object sender, Events.Node.SetMediaEventArgs e)
        {
            if (!DidSetMedia(sender, e.Node, e.Channel, e.Media)) e.Cancel = true;
        }

        /// <summary>
        /// Find the channel and set the media object.
        /// As this may fail, return true if the change was really made or false otherwise.
        /// Throw an exception if the channel could not be found.
        /// </summary>
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
        /// Handle a request to split an audio block. The event contains the original node that was split and the new asset
        /// created from the split. A new sibling to the original node is to be added.
        /// </summary>
        public void SplitAudioBlockRequested(object sender, Events.Node.SplitNodeEventArgs e)
        {
            CoreNode newNode = CreatePhraseNode(e.NewAsset);
            CoreNode parent = (CoreNode)e.Node.getParent();
            int index = parent.indexOf(e.Node) + 1;
            parent.insert(newNode, index);
            UpdateSeq(e.Node);
            MediaSet(this, new Events.Node.SetMediaEventArgs(e.Origin, e.Node, AudioChannel,
                GetMediaForChannel(e.Node, AudioChannel)));
            AddedPhraseNode(this, new Events.Node.AddedPhraseNodeEventArgs(e.Origin, newNode, index));
            Commands.Strips.SplitPhrase command = new Commands.Strips.SplitPhrase(this, e.Node, newNode);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        internal void StartRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, CoreNode parent, int index)
        {
            CoreNode phrase = CreatePhraseNode(e.Asset);
            parent.insert(phrase, index + e.PhraseIndex);
            UpdateSeq(phrase);
            AddedPhraseNode(this, new Events.Node.AddedPhraseNodeEventArgs(this, phrase, index + e.PhraseIndex));
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(this, phrase);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        internal void ContinuingRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, CoreNode parent, int index)
        {
            CoreNode phrase = parent.getChild(index + e.PhraseIndex);
            UpdateTime(this, new Events.Strip.UpdateTimeEventArgs(phrase, e.Time));            
        }

        internal void FinishRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, CoreNode parent, int index)
        {
            CoreNode phrase = parent.getChild(index + e.PhraseIndex);
            UpdateSeq(phrase);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, phrase, AudioChannel,
                GetMediaForChannel(phrase, AudioChannel)));
        }

        #endregion

        #region backend functions

        /// <summary>
        /// Add an already existing phrase node.
        /// </summary>
        public void AddPhraseNode(CoreNode node, CoreNode parent, int index)
        {
            parent.insert(node, index);
            AddedPhraseNode(this, new Events.Node.AddedPhraseNodeEventArgs(this, node, index));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Add an already existing phrase node and its asset.
        /// </summary>
        public void AddPhraseNodeAndAsset(CoreNode node, CoreNode parent, int index)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
            mAssManager.AddAsset(asset);
            AddPhraseNode(node, parent, index);    
        }

        /// <summary>
        /// Determine whether a node can be moved forward or backward in the list of phrase nodes.
        /// </summary>
        private bool CanMovePhraseNode(CoreNode node, Direction dir)
        {
            return dir == Direction.Forward ?
                GetPhraseIndex(node) < GetPhrasesCount((CoreNode)node.getParent()) - 1 :
                GetPhraseIndex(node) > 0;
        }

        /// <summary>
        /// Delete a phrase node from the tree.
        /// </summary>
        public void DeletePhraseNode(CoreNode node)
        {
            DeletedPhraseNode(this, new Events.Node.NodeEventArgs(this, node));
            node.detach();
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        public Commands.Command DeletePhraseNodeAndAsset(CoreNode node)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
            mAssManager.RemoveAsset(asset);
            DeletePhraseNode(node);

            //md 20060814 added this command here so we have a record of it
            //for shallow-delete's undo
            //but note that it hasn't gone to the command queue; that is only done
            //during DeletePhraseNodeRequested
            if (node.getParent() != null)
            {
                int index = ((CoreNode)node.getParent()).indexOf(node);
                Commands.Strips.DeletePhrase command = new Commands.Strips.DeletePhrase(this, node, (CoreNode)node.getParent(), index);
                return command;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the next phrase in the section. If this is the last phrase, then return null. If the original node is null,
        /// return null as well.
        /// </summary>
        public static CoreNode GetNextPhrase(CoreNode node)
        {
            if (node != null)
            {
                CoreNode parent = (CoreNode)node.getParent();
                for (int i = parent.indexOf(node) + 1; i < parent.getChildCount(); ++i)
                {
                    if (Project.GetNodeType(parent.getChild(i)) == NodeType.Phrase) return parent.getChild(i);
                }
            }
            return null;
        }

        /// <summary>
        /// Get the position of the phrase in the list of phrases for the section, i.e. not counting the other section nodes.
        /// </summary>
        // should be part of NodeInformationProperty
        public static int GetPhraseIndex(CoreNode node)
        {
            CoreNode parent = (CoreNode)node.getParent();
            int index = 0;
            for (int i = 0; i < parent.getChildCount(); ++i)
            {
                if (GetNodeType(parent.getChild(i)) == NodeType.Phrase)
                {
                    if (parent.getChild(i) == node) return index;
                    ++index;
                }
            }
            throw new Exception("Not a child of its parent?!");
        }

        /// <summary>
        /// Get the number of child phrases for a node.
        /// </summary>
        // should be part of NodeInformationProperty
        public static int GetPhrasesCount(CoreNode node)
        {
            int count = 0;
            for (int i = 0; i < node.getChildCount(); ++i)
            {
                if (GetNodeType(node.getChild(i)) == NodeType.Phrase) ++count;
            }
            return count;
        }

        /// <summary>
        /// Move a phrase node in the given direction.
        /// </summary>
        public void MovePhraseNode(CoreNode node, Direction dir)
        {
            int index = GetPhraseIndex(node);
            CoreNode parent = (CoreNode)node.getParent();
            DeletePhraseNode(node);
            AddPhraseNode(node, parent, dir == Direction.Forward ? index + 1 : index - 1);
        }

        /// <summary>
        /// Edit the annotation (i.e. label) of a phrase node.
        /// </summary>
        internal void EditAnnotationPhraseNode(CoreNode node, string name)
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

        internal void SetAudioMediaAsset(CoreNode node, AudioMediaAsset asset)
        {
            AssetProperty prop = (AssetProperty)node.getProperty(typeof(AssetProperty));
            if (prop != null)
            {
                prop.Asset = asset;
                UpdateSeq(node);
                MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, AudioChannel,
                    GetMediaForChannel(node, AudioChannel)));
            }
            else
            {
                throw new Exception("Cannot set an asset on a node lacking an asset property.");
            }
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
        /// Send a TouchedNode event.
        /// </summary>
        internal void TouchNode(CoreNode node)
        {
            TouchedNode(this, new Events.Node.NodeEventArgs(this, node));
        }

        #endregion
    }
}