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
        public Events.PhraseNodeHandler RemovedPageNumber;     // page number was removed
        public Events.PhraseNodeHandler SetPageNumber;         // page number was set
        public Events.PhraseNodeUpdateTimeHandler UpdateTime;  // time of the audio of a phrase node changed

        #region clip board

        private PhraseNode mPhraseClipBoard;

        /// <summary>
        /// Clip board that can store a phrase node.
        /// </summary>
        /// <remarks>In the future, this should become a list of phrase nodes (multiple selection.)</remarks>
        internal PhraseNode PhraseClipBoard
        {
            get { return mPhraseClipBoard; }
            set { mPhraseClipBoard = value; }
        }

        /// <summary>
        /// Cut a phrase node: delete it and store it in the clipboard.
        /// Issue a command and modify the project.
        /// </summary>
        internal void CutPhraseNode(object sender, PhraseNode node)
        {
            // create the command before storing the node in the clip board, otherwise the previous value is lost
            Commands.Strips.CutPhrase command = new Commands.Strips.CutPhrase(node);
            mPhraseClipBoard = node;
            DeletePhraseNodeAndAsset(node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Copy a phrase node: simply store it in the clipboard (paste will do the actual copying.)
        /// Issue a command. The project is not modified as a result.
        /// </summary>
        internal void CopyPhraseNode(object sender, PhraseNode node)
        {
            Commands.Strips.CopyPhrase command = new Commands.Strips.CopyPhrase(node);
            mPhraseClipBoard = node;
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        /// <summary>
        /// Paste a copy of the node in the clipboard after the given phrase node, or as the last phrase of the section node.
        /// The clipboard is unmodified.
        /// Issue a command and modify the project.
        /// </summary>
        internal void PastePhraseNode(object sender, ObiNode contextNode)
        {
            if (mPhraseClipBoard != null)
            {
                PhraseNode copy = (PhraseNode)mPhraseClipBoard.copy(true);
                SectionNode parent;
                int index;
                if (contextNode.GetType() == typeof(SectionNode))
                {
                    parent = (SectionNode)contextNode;
                    index = parent.PhraseChildCount;
                }
                else
                {
                    PhraseNode _contextNode = (PhraseNode)contextNode;
                    parent = _contextNode.ParentSection;
                    index = _contextNode.Index + 1;
                }
                AddPhraseNode(copy, parent, index);
                Commands.Strips.PastePhrase command = new Commands.Strips.PastePhrase(copy);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            }
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// Create a command object.
        /// </summary>
        public void DeletePhraseNodeRequested(object sender, PhraseNode node)
        {
            Commands.Strips.DeletePhrase command = new Commands.Strips.DeletePhrase(node);
            CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            DeletePhraseNodeAndAsset(node);
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
            PhraseNode node = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                as PhraseNode;
            node.Asset = asset;
            AddPhraseNode(node, e.SectionNode, e.Index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Merge two adjacent phrase nodes: merge their assets into the first node's asset and remove the second node.
        /// </summary>
        public void MergePhraseNodesRequested(object sender, PhraseNode n1, PhraseNode n2)
        {
            // the command is created while the assets are not changed; there is time to copy the original asset before the
            // merge is done.
            Commands.Strips.MergePhrases command = new Commands.Strips.MergePhrases(n1, n2);
            // n1's asset is merged in place with n2's so don't forget to update the seq media object.
            mAssManager.MergeAudioMediaAssets(n1.Asset, n2.Asset);
            n1.UpdateSeq();
            PhraseAudioSet(sender, n1);
            DeletedPhraseNode(sender, n2);
            n2.DetachFromParent();
            TouchedPhraseNode(sender, n1);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Move a phrase node in either direction. May not succeed if there is nowhere to move in that direction.
        /// </summary>
        private void MovePhraseNodeRequested(object sender, PhraseNode node, PhraseNode.Direction dir)
        {
            if (CanMovePhraseNode(node, dir))
            {
                MovePhraseNode(node, dir);
                Commands.Strips.MovePhrase command = new Commands.Strips.MovePhrase(node, dir);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }
        /// <summary>
        /// Move a phrase node forward. May not succeed if the node is last of its kind.
        /// </summary>
        public void MovePhraseNodeForwardRequested(object sender, PhraseNode node)
        {
            MovePhraseNodeRequested(sender, node, PhraseNode.Direction.Forward);
        }

        /// <summary>
        /// Move a phrase node forward.
        /// </summary>
        public void MovePhraseNodeBackwardRequested(object sender, PhraseNode node)
        {
            MovePhraseNodeRequested(sender, node, PhraseNode.Direction.Backward);
        }

        /// <summary>
        /// Try to set the media on a given channel of a node.
        /// Cancel the event the change could not be made (e.g. renaming a block.)
        /// </summary>
        public void SetMediaRequested(object sender, PhraseNode node, Events.Node.SetMediaEventArgs e)
        {
            if (!DidSetMedia(sender, node, e.Channel, e.Media)) e.Cancel = true;
        }

        /// <summary>
        /// Find the channel and set the media object.
        /// As this may fail, return true if the change was really made or false otherwise.
        /// Throw an exception if the channel could not be found.
        /// </summary>
        internal bool DidSetMedia(object origin, CoreNode node, string channel, IMedia media)
        {
            return false;
            /*
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                IChannel ch = (IChannel)channelsList[i];
                if (ch.getName() == channel)
                {
                    Commands.Command command = null;
                    if (GetNodeType(node) == NodeType.Phrase && channel == AnnotationChannelName)
                    {
                        // we are renaming a phrase node
                        Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
                        string old = mAssManager.RenameAsset(asset, ((TextMedia)media).getText());
                        if (old == asset.Name) return false;
                        command = new Commands.Strips.RenamePhrase(this, node);
                    }
                    channelsProp.setMedia(ch, media);
                    PhraseAudioSet(origin, node);
                    MediaSet(this, new Events.Node.SetMediaEventArgs(origin, node, channel, media));
                    if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
                    mUnsaved = true;
                    StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                    return true;
                }
            }
            // the channel was not found when it should have been...
            throw new Exception(String.Format(Localizer.Message("channel_not_found"), channel));
        
             */
        }

        /// <summary>
        /// Handle a request to split an audio block. The event contains the original node that was split and the new asset
        /// created from the split. A new sibling to the original node is to be added.
        /// </summary>
        public void SplitAudioBlockRequested(object sender, PhraseNode node, Assets.AudioMediaAsset asset)
        {
            PhraseNode newNode = (PhraseNode)
                getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS);
            newNode.Asset = asset;
            node.ParentSection.AddChildPhraseAfter(node, newNode);
            node.UpdateSeq();
            PhraseAudioSet(sender, node);
            AddedPhraseNode(sender, newNode);
            Commands.Strips.SplitPhrase command = new Commands.Strips.SplitPhrase(node, newNode);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        internal void StartRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, CoreNode parent, int index)
        {
            PhraseNode phrase = (PhraseNode)
                getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS);
            phrase.Asset = e.Asset;
            parent.insert(phrase, index + e.PhraseIndex);
            UpdateSeq(phrase);
            AddedPhraseNode(this, phrase);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(phrase);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        internal void ContinuingRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, CoreNode parent, int index)
        {
            PhraseNode phrase = ((SectionNode)parent).PhraseChild(index);
            UpdateTime(this, phrase, e.Time);
        }

        internal void FinishRecordingPhrase(object sender, Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = parent.PhraseChild(index + e.PhraseIndex);
            phrase.UpdateSeq();
            PhraseAudioSet(this, phrase);
        }

        #endregion

        #region backend functions

        /// <summary>
        /// Add an already existing phrase node.
        /// </summary>
        public void AddPhraseNode(PhraseNode node, SectionNode parent, int index)
        {
            parent.AddChildPhrase(node, index);
            AddedPhraseNode(this, node);
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Add an already existing phrase node and its asset.
        /// </summary>
        public void AddPhraseNodeAndAsset(PhraseNode node, SectionNode parent, int index)
        {
            mAssManager.AddAsset(node.Asset);
            AddPhraseNode(node, parent, index);    
        }

        /// <summary>
        /// This function is called when undeleting a subtree
        /// the phrase nodes already exist under the section node, so they can't be re-added
        /// they just need to be rebuilt in the views
        /// </summary>
        public void ReconstructPhraseNodeInView(PhraseNode node)
        {
            //we might consider using a different event for this
            //i don't know who else will be listening in the future (more than only viewports?)
            AddedPhraseNode(this, node);
        }

        /// <summary>
        /// Determine whether a node can be moved forward or backward in the list of phrase nodes.
        /// </summary>
        private bool CanMovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            return dir == PhraseNode.Direction.Forward ?
                node.Index < node.ParentSection.PhraseChildCount - 1 : node.Index > 0;
        }

        /// <summary>
        /// Delete a phrase node from the tree.
        /// </summary>
        public void DeletePhraseNode(PhraseNode node)
        {
            DeletedPhraseNode(this, node);
            node.DetachFromParent();
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        /// <param name="node">The phrase node to delete.</param>
        /// <returns>The delete command created by the deletion.</returns>
        public Commands.Strips.DeletePhrase DeletePhraseNodeAndAsset(PhraseNode node)
        {
            //md 20060814 added this command return value here so we have a record of it
            //for shallow-delete's undo
            //but note that it hasn't gone to the main command queue
            Commands.Strips.DeletePhrase command = null;
            if (node.getParent() != null) command = new Commands.Strips.DeletePhrase(node);
            mAssManager.RemoveAsset(node.Asset);
            DeletePhraseNode(node);
            return command;
        }

        /// <summary>
        /// Get the next phrase in the section. If this is the last phrase, then return null. If the original node is null,
        /// return null as well.
        /// </summary>
        public static PhraseNode GetNextPhrase(PhraseNode node)
        {
            return node != null && node.Index < node.ParentSection.PhraseChildCount - 1 ?
                node.ParentSection.PhraseChild(node.Index + 1) : null;
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
        public void MovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            int index = node.Index;
            SectionNode parent = node.ParentSection;
            DeletePhraseNode(node);
            AddPhraseNode(node, parent, index + dir == PhraseNode.Direction.Forward ? 1 : -1);
        }

        /// <summary>
        /// Edit the annotation (i.e. label) of a phrase node.
        /// </summary>
        internal void EditAnnotationPhraseNode(PhraseNode node, string name)
        {
            string oldName = mAssManager.RenameAsset(node.Asset, name);
            if (oldName != node.Asset.Name)
            {
                node.Annotation = name;
                PhraseAnnotationSet(this, node);
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
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

        /// <summary>
        /// Set the audio media asset for a phrase node. The annotation is changed to the asset's name.
        /// </summary>
        /// <param name="node">The phrase node on which to set the asset.</param>
        /// <param name="asset">The new asset for the node.</param>
        internal void SetAudioMediaAsset(PhraseNode node, AudioMediaAsset asset)
        {
            if (asset != node.Asset)
            {
                node.Asset = asset;
                node.UpdateSeq();
                PhraseAudioSet(this, node);
                node.Annotation = asset.Name;
                // fire an event here to make the annotation change?
            }
        }

        /// <summary>
        /// Make a new sequence media object for the asset of this node.
        /// The sequence media object is simply a translation of the list of clips.
        /// </summary>
        //md change from private to internal so it could be used by CopyPhraseAssets
        internal void UpdateSeq(CoreNode node)
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
        internal void TouchPhraseNode(PhraseNode node)
        {
            TouchedPhraseNode(this, node);
        }

        /// <summary>
        /// Set the page for a phrase node. Create a new property if it did not exist before, otherwise update the label.
        /// </summary>
        /// <remarks>PhraseNode should provide better abstraction of page numbers; we shouldn't have to deal with page
        /// properties here.</remarks>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="node">Phrase node on which to set the page number.</param>
        /// <param name="pageNumber">Page number to set.</param>
        internal void SetPageRequested(object sender, PhraseNode node, int pageNumber)
        {
            PageProperty pageProp = node.PageProperty;
            Commands.Strips.SetNewPageNumber command = null;
            if (pageProp == null)
            {
                // no previous page number for this phrase
                pageProp = (PageProperty)getPresentation().getPropertyFactory().createProperty(PageProperty.NodeName,
                    ObiPropertyFactory.ObiNS);
                pageProp.PageNumber = pageNumber;
                node.setProperty(pageProp);
                command = new Commands.Strips.SetNewPageNumber(node);
            }
            else
            {
                int prev = pageProp.PageNumber;
                if (pageNumber != prev)
                {
                    pageProp.PageNumber = pageNumber;
                    command = new Commands.Strips.SetPageNumber(node, prev);
                }
                else
                {
                    return;
                }
            }
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Remove a page for a phrase node.
        /// </summary>
        internal void RemovePageRequested(object sender, PhraseNode node)
        {
            Commands.Strips.RemovePageNumber command = new Commands.Strips.RemovePageNumber(node);
            RemovePage(sender, node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        /// <summary>
        /// Remove a page label from a phrase node (actually remove the property.)
        /// </summary>
        internal void RemovePage(object origin, PhraseNode node)
        {
            node.removeProperty(typeof(PageProperty));
            RemovedPageNumber(origin, node);
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));            
        }

        #endregion
    }
}