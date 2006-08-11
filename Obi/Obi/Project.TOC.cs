using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi
{
    public partial class Project
    {
        public event Events.Node.AddedSectionNodeHandler AddedSectionNode;      // a section node was added to the TOC
        public event Events.Node.RenamedNodeHandler RenamedNode;                // a node was renamed in the presentation
        public event Events.Node.MovedNodeHandler MovedNode;                    // a node was moved in the presentation
        public event Events.Node.DecreasedSectionNodeLevelHandler DecreasedNodeLevel;  // a node's level was decreased in the presentation
        public event Events.Node.MovedNodeHandler UndidMoveNode;                // a node was restored to its previous location
        public event Events.Node.DeletedNodeHandler DeletedNode;                // a node was deleted from the presentation
        //md: toc clipboard stuff
        public event Events.Node.CutSectionNodeHandler CutTOCNode;
        public event Events.Node.MovedNodeHandler UndidCutTOCNode;
        public event Events.Node.CopiedSectionNodeHandler CopiedTOCNode;
        public event Events.Node.CopiedSectionNodeHandler UndidCopyTOCNode;
        public event Events.Node.PastedSectionNodeHandler PastedTOCNode;
        public event Events.Node.UndidPasteSectionNodeHandler UndidPasteTOCNode;

        private CoreNode mClipboard;        //clipboard for cut-copy-paste

      
        /// <summary>
        /// Create a new section node with a default text label. The node is not attached to anything.
        /// Add a node information custom property as well.
        /// </summary>
        /// <returns>The created node.</returns>
        private CoreNode CreateSectionNode()
        {
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty prop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            TextMedia text = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            text.setText(Localizer.Message("default_section_label"));
            prop.setMedia(mTextChannel, text);
            NodeInformationProperty typeProp =
                (NodeInformationProperty)getPresentation().getPropertyFactory().createProperty("NodeInformationProperty",
                ObiPropertyFactory.ObiNS);
            typeProp.NodeType = NodeType.Section;
            typeProp.NodeStatus = NodeStatus.Used;
            node.setProperty(typeProp);
            return node;
        }
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
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(sibling);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, sibling, parent.indexOf(sibling),
                visitor.Position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(this, sibling, parent, parent.indexOf(sibling),
                visitor.Position);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateSiblingSectionRequested(object sender, Events.Node.NodeEventArgs e)
        {
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
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(child);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, child, parent.indexOf(child), visitor.Position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(this, child, parent, parent.indexOf(child),
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
        public void AddExistingSection(CoreNode node, CoreNode parent, int index, int position, string originalLabel)
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
        /// Remove a node from the core tree. It is detached from the tree and the
        /// </summary>
        public void RemoveNode(object origin, CoreNode node)
        {
            if (node != null)
            {
                Commands.TOC.DeleteSectionNode command = null;
                if (origin != this)
                {
                    CoreNode parent = (CoreNode)node.getParent();
                    Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);
                    command = new Commands.TOC.DeleteSectionNode(this, node, parent, parent.indexOf(node), visitor.Position);
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
            Commands.TOC.MoveSectionNodeUp command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionNodeUp
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            bool succeeded = ExecuteMoveNodeUp(node);

            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// move the node up
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>a facade API function could do this for us</remarks>
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

        public void MoveNodeDown(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionNodeDown command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is moved
                command = new Commands.TOC.MoveSectionNodeDown
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            bool succeeded = ExecuteMoveNodeDown(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
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
        ///<remarks>a facade API function could do this for us</remarks>
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
            Commands.TOC.IncreaseSectionNodeLevel command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.IncreaseSectionNodeLevel
                    (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            bool succeeded = ExecuteIncreaseNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                //IncreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (origin, node, newParent, newParent.indexOf(node), visitor.Position));

                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));  // JQ
            }

        }

        /// <summary>
        /// Move the node "in"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <remarks>a facade API function could do this for us</remarks>
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
            Commands.TOC.DecreaseSectionNodeLevel command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.DecreaseSectionNodeLevel
                    (this, node, parent, parent.indexOf(node), visitor.Position, node.getChildCount());
            }

            bool succeeded = ExecuteDecreaseNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                DecreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// move the node "out"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        ///<remarks>a facade API function could do this for us</remarks>
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
            for (int i = numChildren - 1; i > nodeIndex; i--)
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
        /// undo decrease node level is a bit tricky because we might have to move some of the node's
        /// children out a level, but only if they were newly adopted after the decrease level action happened. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="childCount">number of children this node used to have before the decrease level action happened</param>
        //added by marisa
        public void UndoDecreaseSectionLevel(CoreNode node, CoreNode parent, int index, int position, int originalChildCount)
        {
            //error-checking
            if (node.getChildCount() < originalChildCount)
            {
                //this would be a pretty strange thing to have happen.
                //todo: throw an exception?  
                return;
            }

            //detach the non-original children (child nodes originalChildCount...n-1)
            ArrayList nonOriginalChildren = new ArrayList();
            int totalNumChildren = node.getChildCount();

            for (int i = totalNumChildren - 1; i >= originalChildCount; i--)
            {
                CoreNode child = (CoreNode)node.getChild(i);
                if (child != null)
                {
                    nonOriginalChildren.Add(child);
                    child.detach();
                }
            }

            //this array was built backwards, so reverse it
            nonOriginalChildren.Reverse();

            //insert the node back in its old location
            node.detach();
            parent.insert(node, index);

            MovedNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position));

            Visitors.SectionNodePosition visitor = null;

            //reattach the children
            for (int i = 0; i < nonOriginalChildren.Count; i++)
            {
                parent.appendChild((CoreNode)nonOriginalChildren[i]);
                //find the position for the first one
                if (i == 0)
                {
                    visitor = new Visitors.SectionNodePosition(node);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);
                }

                MovedNode(this, new Events.Node.MovedNodeEventArgs
                    (this, (CoreNode)nonOriginalChildren[i], parent,
                    parent.getChildCount() - 1, visitor.Position + i));
            }
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

        //md 20060810
        public void CutTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            DoCutTOCNode(sender, e.Node);
        }

        //md 20060810
        public void DoCutTOCNode(object origin, CoreNode node)
        {
            if (node == null) return;

            CoreNode parent = (CoreNode)node.getParent();
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            //we need to save the state of the node before it is altered
            Commands.TOC.CutSectionNode command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CutSectionNode
                 (this, node, parent, parent.indexOf(node), visitor.Position);
            }

            mClipboard = node;
            node.detach();

            CutTOCNode(this, new Events.Node.NodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCutNode(CoreNode node, CoreNode parent, int index, int position)
        {
            if (node.getParent() != null) node.detach();
            parent.insert(node, index);

            UndidCutTOCNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        public void CopyTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CopyTOCNode(sender, e.Node);
        }

        //md 20060810
        public void CopyTOCNode(object origin, CoreNode node)
        {
            if (node == null) return;

            Commands.TOC.CopySectionNode command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CopySectionNode(this, node);
            }

            //the actual copy operation
            mClipboard = node.copy(true);

            CopiedTOCNode(this, new Events.Node.NodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCopyTOCNode(CoreNode node)
        {
            mClipboard = null;

            UndidCopyTOCNode(this, new Events.Node.NodeEventArgs(this, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        public void PasteTOCNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            PasteTOCNode(sender, e.Node);
        }

        //md 20060810
        //"paste" will paste the clipboard contents as the first child of the given node
        public void PasteTOCNode(object origin, CoreNode parent)
        {
            if (parent == null) return;

            Commands.TOC.PasteSectionNode command = null;

            CoreNode pastedSection = mClipboard;

            //don't clear the clipboard, we can use it again

            if (origin != this)
            {
                command = new Commands.TOC.PasteSectionNode(this, pastedSection, parent);
            }

            //the actual paste operation
            parent.insert(pastedSection, 0);

            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(pastedSection);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            PastedTOCNode(this, new Events.Node.AddedSectionNodeEventArgs
                (origin, pastedSection, parent.indexOf(pastedSection), visitor.Position));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoPasteTOCNode(CoreNode node)
        {
            mClipboard = node.copy(true);

            node.detach();

            UndidPasteTOCNode(this, new Events.Node.NodeEventArgs(this, mClipboard));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

    }
}
