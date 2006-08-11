using System;
using System.IO;
using urakawa.core;
using urakawa.media;
using Obi.Assets;

namespace Obi
{
    public partial class Project
    {
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
        public void DeletePhraseNodeAndAsset(CoreNode node)
        {
            Assets.AudioMediaAsset asset = GetAudioMediaAsset(node);
            mAssManager.RemoveAsset(asset);
            DeletePhraseNode(node);
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

        #endregion
    }
}