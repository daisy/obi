using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using urakawa.core;
using urakawa.media;
using Obi.Assets;

namespace Obi
{
    public partial class Project
    {
        public Events.UpdateTimeHandler UpdateTime;
        public Events.PhraseNodeHandler RemovedPageNumber;
        public Events.PhraseNodeHandler SetPageNumber;

        #region clip board (cut/copy/paste/delete)

        /// <summary>
        /// Cut a phrase node: delete it and store it in the clipboard (store the original node, not a copy.)
        /// Issue a command and modify the project.
        /// </summary>
        /// <param name="node">The phrase node to cut.</param>
        public void CutPhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                Commands.Strips.CutPhrase command = new Commands.Strips.CutPhrase(node);
                mClipboard.Phrase = node;
                RemovePhraseNodeAndAsset(node);
                Modified();
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a phrase node by storing a copy in the clipboard.
        /// Issue a command but do not mark the project as modified.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        public void CopyPhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                PhraseNode copy = node.copy(true);
                Commands.Strips.CopyPhrase command = new Commands.Strips.CopyPhrase(copy);
                mClipboard.Phrase = copy;
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        public void DeletePhraseNode(PhraseNode node)
        {
            if (node != null)
            {
                Commands.Strips.DeletePhrase command = RemovePhraseNodeAndAsset(node);
                Modified();
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Paste *before*.
        /// </summary>
        /// <param name="node">The phrase node to paste.</param>
        /// <param name="contextNode">If the context is a section, paste at the end of the section.
        /// If it is a phrase, paste *before* this phrase.</param>
        public void PastePhraseNode(PhraseNode node, ObiNode contextNode)
        {
            if (node != null && contextNode != null)
            {
                SectionNode parent;
                int index;
                if (contextNode is SectionNode)
                {
                    parent = (SectionNode)contextNode;
                    index = parent.PhraseChildCount;
                }
                else if (contextNode is PhraseNode)
                {
                    parent = ((PhraseNode)contextNode).ParentSection;
                    index = ((PhraseNode)contextNode).Index;
                }
                else
                {
                    throw new Exception(String.Format("Cannot paste with context node as {0}.", contextNode.GetType()));
                }
                PhraseNode copy = node.copy(true);
                AddPhraseNode(copy, parent, index);
                Modified();
                Commands.Strips.PastePhrase command = new Commands.Strips.PastePhrase(copy);
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Import an asset, create a node for it and add it at the given position in its section.
        /// The phrase is named after the imported file name.
        /// </summary>
        public void ImportAssetRequested(object sender, Events.Strip.ImportAssetEventArgs e)
        {
            AudioMediaAsset asset = mAssManager.ImportAudioMediaAsset(e.AssetPath);
            mAssManager.InsureRename(asset, Path.GetFileNameWithoutExtension(e.AssetPath));
            
            //create a phrase node and assign it an asset
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
        /// Merge a phrase node with the next one.
        /// </summary>
        /// <param name="node">The node to merge.</param>
        /// <param name="next">The next one to merge with.</param>
        public void MergeNodes__(PhraseNode node, PhraseNode nextNode)
        {
            Assets.AudioMediaAsset asset = node.Asset;
            Assets.AudioMediaAsset next = nextNode.Asset;
            // the command is created while the assets are not changed; there is time to copy the original asset before the
            // merge is done.
            Commands.Strips.MergePhrases command = new Commands.Strips.MergePhrases(node, nextNode);
            mAssManager.MergeAudioMediaAssets(asset, next);
            UpdateSeq(node);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, Project.AudioChannelName,
                GetMediaForChannel(node, Project.AudioChannelName)));
            DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, nextNode));
            nextNode.DetachFromParent();
            TouchedNode(this, new Events.Node.NodeEventArgs(this, node));
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        public enum Direction { Forward, Backward };

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
        internal bool DidSetMedia(object origin, PhraseNode node, string channel, IMedia media)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            IList channelsList = channelsProp.getListOfUsedChannels();
            for (int i = 0; i < channelsList.Count; i++)
            {
                IChannel ch = (IChannel)channelsList[i];
                if (ch.getName() == channel)
                {
                    Commands.Command command = null;
                   /* if (GetNodeType(node) == NodeType.Phrase && channel == Project.AnnotationChannelName)
                    {
                        // we are renaming a phrase node
                        //md no longer allowed to rename assets
                      /*  Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
                        string old = mAssManager.RenameAsset(asset, ((TextMedia)media).getText());
                        if (old == asset.Name) return false;
                        command = new Commands.Strips.RenamePhrase(this, node);*/
                  //  }

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
        /// Split phrase node in two.
        /// </summary>
        /// <param name="node">The original node that was split, with its split asset.</param>
        /// <param name="newAsset">The new asset to create a new phrase node from.</param>
        public void Split(PhraseNode node, Assets.AudioMediaAsset newAsset)
        {
            PhraseNode newNode = CreatePhraseNode(newAsset);
            node.ParentSection.AddChildPhraseAfter(newNode, node);
            UpdateSeq(node);
            // review this
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, Project.AudioChannelName,
                GetMediaForChannel(node, Project.AudioChannelName)));
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, newNode));
            Commands.Strips.SplitPhrase command = new Commands.Strips.SplitPhrase(node, newNode);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        public void ApplyPhraseDetection(PhraseNode node, long threshold, double length, double gap)
        {
            AudioMediaAsset originalAsset = node.Asset;
            List<AudioMediaAsset> assets = originalAsset.ApplyPhraseDetection(threshold, length, gap);
            if (assets.Count > 1)
            {
                List<PhraseNode> nodes = new List<PhraseNode>(assets.Count);
                assets.ForEach(delegate(AudioMediaAsset ass) { nodes.Add(CreatePhraseNode(ass)); });
                ReplaceNodeWithNodes(node, nodes);
                CommandCreated(this,
                    new Events.Project.CommandCreatedEventArgs(new Commands.Strips.ApplyPhraseDetection(this, node, nodes)));
                Modified();
            }
        }

        /// <summary>
        /// Replace a node with a list of nodes.
        /// </summary>
        /// <param name="mNode">The node to remove.</param>
        /// <param name="mPhraseNodes">The nodes to add instead.</param>
        internal void ReplaceNodeWithNodes(PhraseNode node, List<PhraseNode> nodes)
        {
            if (node.getParent().GetType() == Type.GetType("Obi.SectionNode"))
            {
                SectionNode parent = (SectionNode)node.getParent();
                int index = parent.indexOf(node);
                DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
                node.DetachFromParent();

                foreach (PhraseNode n in nodes)
                {
                    parent.AddChildPhrase(n, index);
                    AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, n));
                }
                Modified();
            }
            else
            { //TODO: will this ever come up?
            }
        }

        /// <summary>
        /// Replace a list of contiguous nodes with a single one.
        /// </summary>
        /// <param name="mPhraseNodes">The nodes to remove.</param>
        /// <param name="mNode">The node to add instead.</param>
        internal void ReplaceNodesWithNode(List<PhraseNode> nodes, PhraseNode node)
        {
            if (nodes[0].getParent().GetType() == Type.GetType("Obi.SectionNode"))
            {
                SectionNode parent = (SectionNode)nodes[0].getParent();
                int index = nodes[0].Index;

                foreach (PhraseNode n in nodes)
                {
                    DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, n));
                    n.DetachFromParent();
                }
                parent.AddChildPhrase(node, index);
                AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
                Modified();
            }
            else
            {//TODO: when will this case arise?
            }
        }

        /// <summary>
        /// A new phrase being recorded.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void StartRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = CreatePhraseNode(e.Asset);
            parent.AddChildPhrase(phrase, index);
            UpdateSeq(phrase);
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, phrase));
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(phrase);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Update the time information of a phrase being recorded.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void ContinuingRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = parent.PhraseChild(index);
            UpdateTime(this, new Events.Strip.UpdateTimeEventArgs(this, phrase, e.Time));            
        }

        /// <summary>
        /// When a phrase has finished recording, update its media object.
        /// </summary>
        /// <param name="e">The phrase event originally sent by the recording session.</param>
        /// <param name="parent">Parent core node for the new phrase.</param>
        /// <param name="index">Base index in the parent for new phrases.</param>
        internal void FinishRecordingPhrase(Events.Audio.Recorder.PhraseEventArgs e, SectionNode parent, int index)
        {
            PhraseNode phrase = parent.PhraseChild(index + e.PhraseIndex);
            UpdateSeq(phrase);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, phrase, Project.AudioChannelName,
                GetMediaForChannel(phrase, Project.AudioChannelName)));
        }

        #endregion

        #region backend functions





        /// <summary>
        /// This function is called when undeleting a subtree
        /// the phrase nodes already exist under the section node, so they can't be re-added
        /// they just need to be rebuilt in the views
        /// </summary>
        /// <param name="node"></param>
        /// <param name="index"></param>
        //md 20060813
        public void ReconstructPhraseNodeInView(PhraseNode node)
        {
            //we might consider using a different event for this
            //i don't know who else will be listening in the future (more than only viewports?)
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
        }

        /// <summary>
        /// Delete a phrase node from the tree.
        /// </summary>
        /// <param name="node">The phrase node to delete.</param>
        public void RemovePhraseNode(PhraseNode node)
        {
            DeletedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
            node.DetachFromParent();
            Modified();
        }

        /// <summary>
        /// Delete a phrase node from the tree and remove its asset from the asset manager.
        /// </summary>
        /// <param name="node">The phrase node to delete.</param>
        /// <returns>A suitable command for shallow delete.</returns>
        public Commands.Strips.DeletePhrase RemovePhraseNodeAndAsset(PhraseNode node)
        {
            Commands.Strips.DeletePhrase command = new Commands.Strips.DeletePhrase(node);
            mAssManager.RemoveAsset(node.Asset);
            RemovePhraseNode(node);
            return command;
        }

        /// <summary>
        /// Get the next phrase in the section. If this is the last phrase, then return null. If the original node is null,
        /// return null as well.
        /// </summary>
        public static PhraseNode GetNextPhrase(PhraseNode node)
        {
            if (node != null)
            {
                if (node.getParent().GetType() == Type.GetType("Obi.SectionNode"))
                {
                    SectionNode parent = (SectionNode)node.getParent();
                    if (node.Index == parent.PhraseChildCount - 1) return null;
                    else return parent.PhraseChild(node.Index + 1);
                }
                else
                {//TODO: when will this case arise?
                }
            }
            return null;
        }

        public bool CanMovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            if (node != null)
            {
                int index = node.Index + (dir == PhraseNode.Direction.Forward ? 1 : -1);
                return index >= 0 && index < node.ParentSection.PhraseChildCount;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Move a phrase node in the given direction.
        /// </summary>
        public void MovePhraseNode(PhraseNode node, PhraseNode.Direction dir)
        {
            int index = node.Index + (dir == PhraseNode.Direction.Forward ? 1 : -1);
            if (index >= 0 && index < node.ParentSection.PhraseChildCount)
            {
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(new Commands.Strips.MovePhrase(node, index)));
                MovePhraseNodeTo(node, index);
                Modified();
            }
        }

        public void MovePhraseNodeTo(PhraseNode node, int index)
        {
            SectionNode parent = node.ParentSection;
            RemovePhraseNode(node);
            AddPhraseNode(node, parent, index);
        }

        /// <summary>
        /// Edit the annotation (i.e. label) of a phrase node.
        /// </summary>
        internal void EditAnnotationPhraseNode(PhraseNode node, string name)
        {
            TextMedia media = (TextMedia)GetMediaForChannel(node, Project.AnnotationChannelName);
            Assets.AudioMediaAsset asset = node.Asset;
            mAssManager.RenameAsset(asset, name);
            media.setText(asset.Name);
            MediaSet(this, new Events.Node.SetMediaEventArgs(this, node, Project.AnnotationChannelName, media));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Set the audio media asset for a phrase node.
        /// The Sequence media object is updated as well.
        /// </summary>
        internal void SetAudioMediaAsset(PhraseNode node, AudioMediaAsset asset)
        {
            node.Asset = asset;
            UpdateSeq(node);
        }

        /// <summary>
        /// Make a new sequence media object for the asset of this node.
        /// The sequence media object is simply a translation of the list of clips.
        /// </summary>
        //md change from private to internal so it could be used by CopyPhraseAssets
        internal void UpdateSeq(PhraseNode node)
        {
            Assets.AudioMediaAsset asset = node.Asset;
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

        /// <summary>
        /// Add an empty phrase node in a section at a given index.
        /// The phrase node gets the empty media asset.
        /// </summary>
        /// <param name="section">The section node to add to.</param>
        /// <param name="index">The index at which the new node is added.</param>
        public void AddEmptyPhraseNode(SectionNode section, int index)
        {
            PhraseNode node = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                as PhraseNode;
            node.Asset = AudioMediaAsset.Empty;
            AddPhraseNode(node, section, index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(node);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        /// <summary>
        /// Add an already existing phrase node.
        /// </summary>
        /// <param name="node">The phrase node to add.</param>
        /// <param name="parent">Its parent section.</param>
        /// <param name="index">Its position in the parent section (with regards to other phrases.)</param>
        public void AddPhraseNode(PhraseNode node, SectionNode parent, int index)
        {
            parent.AddChildPhrase(node, index);
            AddedPhraseNode(this, new Events.Node.PhraseNodeEventArgs(this, node));
            Modified();
        }

        /// <summary>
        /// Add an already existing phrase node and its asset.
        /// </summary>
        /// <param name="node">The phrase node to add.</param>
        /// <param name="parent">Its parent section.</param>
        /// <param name="index">Its position in the parent section (with regards to other phrases.)</param>
        public void AddPhraseNodeAndAsset(PhraseNode node, SectionNode parent, int index)
        {
            Assets.AudioMediaAsset asset = node.Asset;
            mAssManager.AddAsset(asset);
            AddPhraseNode(node, parent, index);
        }

        /// <summary>
        /// Add a new phrase with an asset created from a sound file.
        /// This creates a command and modifies the project.
        /// </summary>
        /// <param name="path">The path of the sound file to create the asset from.</param>
        /// <param name="section">The section node in which to add the phrase.</param>
        /// <param name="index">The index at which the phrase is added.</param>
        public void AddPhraseFromFile(string path, SectionNode section, int index)
        {
            AudioMediaAsset asset = mAssManager.ImportAudioMediaAsset(path);
            mAssManager.InsureRename(asset, Path.GetFileNameWithoutExtension(path));
            PhraseNode phrase = getPresentation().getCoreNodeFactory().createNode(PhraseNode.Name, ObiPropertyFactory.ObiNS)
                 as PhraseNode;
            phrase.Asset = asset;
            AddPhraseNode(phrase, section, index);
            Commands.Strips.AddPhrase command = new Commands.Strips.AddPhrase(phrase);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            Modified();
        }

        #region page numbers

        /// <summary>
        /// Set a page number on the given phrase.
        /// </summary>
        /// <param name="node">The phrase node to set a page on.</param>
        /// <returns>True if the operation actually was performed.</returns>
        public bool DidSetPageNumberOnPhrase(PhraseNode node)
        {
            if (node.PageProperty == null)
            {
                node.PageProperty = new PageProperty();
                node.PageProperty.PageNumber = 0;
                RenumberPages();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Set a page number on the given phrase and issue a command.
        /// </summary>
        /// <param name="node">The phrase node to set a page on.</param>
        public void SetPageNumberOnPhraseWithUndo(PhraseNode node)
        {
            if (DidSetPageNumberOnPhrase(node)) Modified(new Commands.Strips.SetPageNumber(node));
        }

        /// <summary>
        /// Remove a page number of the given phrase.
        /// </summary>
        /// <param name="node">The phrase to remove the page number from.</param>
        public bool DidRemovePageNumberFromPhrase(PhraseNode node)
        {
            if (node.PageProperty != null)
            {
                node.PageProperty = null;
                RemovedPageNumber(this, new Events.Node.PhraseNodeEventArgs(this, node));
                RenumberPages();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Remove a page number from the given phrase and issue a command.
        /// </summary>
        /// <param name="node">The phrase to remove the page number from.</param>
        public void RemovePageNumberFromPhraseWithUndo(PhraseNode node)
        {
            if (DidRemovePageNumberFromPhrase(node)) Modified(new Commands.Strips.RemovePageNumber(node));
        }

        /// <summary>
        /// Renumber pages in the book when new pages have been added/removed.
        /// </summary>
        public void RenumberPages()
        {
            int pageNumber = 1;
            RootNode.visitDepthFirst(
                delegate(ICoreNode n)
                {
                    PhraseNode visited = n as PhraseNode;
                    if (visited != null)
                    {
                        if (visited.PageProperty != null)
                        {
                            if (visited.PageProperty.PageNumber != pageNumber)
                            {
                                visited.PageProperty.PageNumber = pageNumber;
                                SetPageNumber(this, new Events.Node.PhraseNodeEventArgs(this, visited));
                            }
                            ++pageNumber;
                        }
                    }
                    return true;
                },
                delegate(ICoreNode n) { }
            );
        }

        #endregion
    }
}