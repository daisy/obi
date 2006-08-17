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
        public event Events.Node.MovedNodeHandler MovedSectionNode;                    // a node was moved in the presentation
        public event Events.Node.DecreasedSectionNodeLevelHandler DecreasedSectionNodeLevel;  // a node's level was decreased in the presentation
        public event Events.Node.MovedNodeHandler UndidMoveNode;                // a node was restored to its previous location
        public event Events.Node.DeletedNodeHandler DeletedNode;                // a node was deleted from the presentation
        //md: toc clipboard stuff
        public event Events.Node.CutSectionNodeHandler CutSectionNode;
        public event Events.Node.MovedNodeHandler UndidCutSectionNode;
        public event Events.Node.CopiedSectionNodeHandler CopiedSectionNode;
        public event Events.Node.CopiedSectionNodeHandler UndidCopySectionNode;
        public event Events.Node.PastedSectionNodeHandler PastedSectionNode;
        public event Events.Node.UndidPasteSectionNodeHandler UndidPasteSectionNode;

        //md: shallow-swapped event for move up/down linear
        public event Events.Node.ShallowSwappedSectionNodesHandler ShallowSwappedSectionNodes;

        private CoreNode mClipboard;        //clipboard for cut-copy-paste

        public CoreNode Clipboard
        {
            get
            {
                return mClipboard;
            }
        }
      
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
        public void CreateSiblingSectionNode(object origin, CoreNode contextNode)
        {
            //mdXXX
            if (contextNode != null)
            {
                NodeType nodeType;
                nodeType = Project.GetNodeType(contextNode);
                if (nodeType != NodeType.Section)
                {
                    throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
                }
            }
            //end mdXXX

            CoreNode parent = (CoreNode)(contextNode == null ? getPresentation().getRootNode() : contextNode.getParent());
            CoreNode sibling = CreateSectionNode();
            if (contextNode == null)
            {
                parent.appendChild(sibling);
            }
            else
            {
                parent.insertAfter(sibling, contextNode);
            }
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(sibling);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            int sectionIdx = GetSectionNodeIndex(sibling);

            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, sibling, parent.indexOf(sibling),
                visitor.Position, sectionIdx));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(this, sibling, parent, parent.indexOf(sibling),
                visitor.Position);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateSiblingSectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CreateSiblingSectionNode(sender, e.Node);
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public void CreateChildSectionNode(object origin, CoreNode parent)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(parent);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            CoreNode child = CreateSectionNode();
            if (parent == null) parent = getPresentation().getRootNode();
            parent.appendChild(child);
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(child);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            int sectionIdx = GetSectionNodeIndex(child);

            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(origin, child, parent.indexOf(child), visitor.Position, sectionIdx));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(this, child, parent, parent.indexOf(child),
                visitor.Position);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateChildSectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CreateChildSectionNode(sender, e.Node);
        }

        /// <summary>
        /// Add a section that had previously been added.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="originalLabel"></param>
        public void AddExistingSectionNode(CoreNode node, CoreNode parent, int index, int position, string originalLabel)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            if (node.getParent() == null) parent.insert(node, index);
            if (originalLabel != null) Project.GetTextMedia(node).setText(originalLabel);

            int sectionIdx = GetSectionNodeIndex(node);

            AddedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs(this, node, index, position, sectionIdx));
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
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent, index, position);
            node.acceptDepthFirst(visitor);
        }

        /// <summary>
        /// Remove a node from the core tree. It is detached from the tree and the
        /// </summary>
        //md
        //the command value is returned so it can be used in UndoShallowDelete's undo list
        public Commands.Command RemoveNode(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            Commands.TOC.DeleteSectionNode command = null;
            if (node != null)
            {
                //md: need this particular command to be created even if origin = this
                //because its undo fn is required by UndoShallowDelete
                //if (origin != this)
               // {
                    CoreNode parent = (CoreNode)node.getParent();
                    Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);
                    command = new Commands.TOC.DeleteSectionNode(this, node, parent, parent.indexOf(node), visitor.Position);
              //  }
                node.detach();
                DeletedNode(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                //md: added condition "origin != this" to accomodate the change made above
                if (command != null && origin != this) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }

            return command;
        }

        public void RemoveNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            RemoveNode(sender, e.Node);
        }

        /// <summary>
        /// Move a node up in the TOC.
        /// </summary>
        public void MoveSectionNodeUp(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

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

            bool succeeded = ExecuteMoveSectionNodeUp(node);

            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int sectionIdx = GetSectionNodeIndex(node);

                MovedSectionNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position, sectionIdx));
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
        private bool ExecuteMoveSectionNodeUp(CoreNode node)
        {
            CoreNode newParent = null;
            int newIndex = 0;

            int currentIndex = ((CoreNode)node.getParent()).indexOf(node);
            int currentSectionIndex = GetSectionNodeIndex(node);

            //if it is the first node in its list
            //change its level and move it to be the previous sibling of its parent
            if (currentSectionIndex == 0)
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
                CoreNode switchWithNode = GetPreviousSectionNodeSibling(node);
                newIndex = newParent.indexOf(switchWithNode);
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

        public void MoveSectionNodeUpRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveSectionNodeUp(sender, e.Node);
        }

        /// <summary>
        /// reposition the node at the index under its given parent
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void UndoMoveSectionNode(CoreNode node, CoreNode parent, int index, int position)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX


            if (node.getParent() != null) node.detach();
            parent.insert(node, index);
            int sectionIdx = GetSectionNodeIndex(node);

            UndidMoveNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position, sectionIdx));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Undo increase level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UndoIncreaseSectionNodeLevel(CoreNode node, CoreNode parent, int index, int position)
        {
            UndoMoveSectionNode(node, parent, index, position);
        }

        public void MoveSectionNodeDown(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

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

            bool succeeded = ExecuteMoveSectionNodeDown(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int sectionIdx = GetSectionNodeIndex(node);

                MovedSectionNode(this, new Events.Node.MovedNodeEventArgs
                    (this, node, newParent, newParent.indexOf(node), visitor.Position, sectionIdx));

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
        private bool ExecuteMoveSectionNodeDown(CoreNode node)
        {
            CoreNode newParent = null;
            int newIndex = 0;

            int currentIndex = ((CoreNode)node.getParent()).indexOf(node);
           
            int currentSectionIndex = GetSectionNodeIndex(node);
            
            CoreNode lastSectionNode = GetLastSectionNodeChild((CoreNode)node.getParent());

            //if it is the last node in its list, change its level
            if (lastSectionNode == node)
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

        public void MoveSectionNodeDownRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveSectionNodeDown(sender, e.Node);
        }

        public void IncreaseSectionNodeLevel(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

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

            bool succeeded = ExecuteIncreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int sectionIdx = GetSectionNodeIndex(node);

                //IncreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                MovedSectionNode(this, new Events.Node.MovedNodeEventArgs
                    (origin, node, newParent, newParent.indexOf(node), visitor.Position, sectionIdx));

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
        private bool ExecuteIncreaseSectionNodeLevel(CoreNode node)
        {
            CoreNode newParent = GetPreviousSectionNodeSibling(node);

            //CoreNode newParent = ((CoreNode)node.getParent()).getChild(nodeIndex - 1);

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

        //md 20060813
        //helper function
        private CoreNode GetPreviousSectionNodeSibling(CoreNode node)
        {
            CoreNode parent = (CoreNode)node.getParent();

            int idx = parent.indexOf(node);

            if (idx == 0)
                return null;

            for (int i = idx - 1; i >= 0; i--)
            {
                if (GetNodeType(parent.getChild(i)) == NodeType.Section)
                {
                    return parent.getChild(i);
                }
            }

            return null;
        }

        public void IncreaseSectionNodeLevelRequested(object sender, Events.Node.NodeEventArgs e)
        {
            IncreaseSectionNodeLevel(sender, e.Node);
        }

       //md
       //the command value is returned so it can be used in UndoShallowDelete's undo list
       public Commands.Command DecreaseSectionNodeLevel(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            Commands.TOC.DecreaseSectionNodeLevel command = null;

            //md: need this particular command to be created even if origin = this
            //because its undo fn is required by UndoShallowDelete
            //if (origin != this)
            //{
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition nodeVisitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(nodeVisitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.DecreaseSectionNodeLevel
                    (this, node, parent, parent.indexOf(node), nodeVisitor.Position, node.getChildCount());
            //}

            bool succeeded = ExecuteDecreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                DecreasedSectionNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                //md: added condition "origin != this" to accomodate the change made above
                if (command != null && origin != this) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }

            return command;
        }

        /// <summary>
        /// move the node "out"
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        ///<remarks>a facade API function could do this for us</remarks>
        private bool ExecuteDecreaseSectionNodeLevel(CoreNode node)
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

        public void DecreaseSectionNodeLevelRequested(object sender, Events.Node.NodeEventArgs e)
        {
            DecreaseSectionNodeLevel(sender, e.Node);
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
        public void UndoDecreaseSectionNodeLevel(CoreNode node, CoreNode parent, int index, int position, int originalChildCount)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

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

            if (GetNodeType(node) == NodeType.Section)
            {
                int sectionIdx = GetSectionNodeIndex(node);
                MovedSectionNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position, sectionIdx));
            }

            Visitors.SectionNodePosition visitor = null;

            //reattach the children
            for (int i = 0; i < nonOriginalChildren.Count; i++)
            {
                parent.appendChild((CoreNode)nonOriginalChildren[i]);

                if (GetNodeType((CoreNode)nonOriginalChildren[i]) == NodeType.Section)
                {                    
                    visitor = new Visitors.SectionNodePosition((CoreNode)nonOriginalChildren[i]);
                    getPresentation().getRootNode().acceptDepthFirst(visitor);

                    int childSectionIdx = GetSectionNodeIndex((CoreNode)nonOriginalChildren[i]);

                    MovedSectionNode(this, new Events.Node.MovedNodeEventArgs
                        (this, (CoreNode)nonOriginalChildren[i], parent,
                        parent.getChildCount() - 1, visitor.Position, childSectionIdx));
                }
            }
        }

        /// <summary>
        /// Change the text label of a node.
        /// </summary>
        public void RenameSectionNode(object origin, CoreNode node, string label)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            TextMedia text = GetTextMedia(node);
            Commands.TOC.Rename command = origin == this ? null : new Commands.TOC.Rename(this, node, text.getText(), label);
            GetTextMedia(node).setText(label);
            RenamedNode(this, new Events.Node.RenameNodeEventArgs(origin, node, label));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameSectionNodeRequested(object sender, Events.Node.RenameNodeEventArgs e)
        {
            RenameSectionNode(sender, e.Node, e.Label);
        }

        //md 20060810
        public void CutSectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            DoCutSectionNode(sender, e.Node);
        }

        //md 20060810
        public void DoCutSectionNode(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

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

           CutSectionNode(this, new Events.Node.NodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCutSectionNode(CoreNode node, CoreNode parent, int index, int position)
        {
            UndeleteSectionNode(node, parent, index, position);
            mClipboard = null;
           
            //mdXXX
        /*    NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }*/
            //end mdXXX

            /*
            if (node.getParent() != null) node.detach();
            parent.insert(node, index);

            int sectionIdx = GetSectionNodeIndex(node);

            UndidCutSectionNode(this, new Events.Node.MovedNodeEventArgs(this, node, parent, index, position, sectionIdx));
            mClipboard = null;
            
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
             */
        }

        //md 20060810
        public void CopySectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            CopySectionNode(sender, e.Node);
        }

        //md 20060810
        public void CopySectionNode(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            if (node == null) return;

            Commands.TOC.CopySectionNode command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CopySectionNode(this, node);
            }

            //the actual copy operation
            mClipboard = node.copy(true);

            CopiedSectionNode(this, new Events.Node.NodeEventArgs(origin, mClipboard));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCopySectionNode(CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            mClipboard = null;

           UndidCopySectionNode(this, new Events.Node.NodeEventArgs(this, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        public void PasteSectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            PasteSectionNode(sender, e.Node);
        }

        //md 20060810
        //"paste" will paste the clipboard contents as the first child of the given node
        public void PasteSectionNode(object origin, CoreNode parent)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(parent);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            if (parent == null) return;

            Commands.TOC.PasteSectionNode command = null;

            CoreNode pastedSection = mClipboard.copy(true);

            //don't clear the clipboard, we can use it again

            if (origin != this)
            {
                command = new Commands.TOC.PasteSectionNode(this, pastedSection, parent);
            }

            //the actual paste operation
            parent.insert(pastedSection, 0);

            //reconstruct the assets
            Obi.Visitors.CopyPhraseAssets assVisitor = new Obi.Visitors.CopyPhraseAssets(mAssManager, this);
            pastedSection.acceptDepthFirst(assVisitor);

            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(pastedSection);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            int sectionIdx = GetSectionNodeIndex(pastedSection);

            PastedSectionNode(this, new Events.Node.AddedSectionNodeEventArgs
                (origin, pastedSection, parent.indexOf(pastedSection), visitor.Position, sectionIdx));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoPasteSectionNode(CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            node.detach();

            UndidPasteSectionNode(this, new Events.Node.NodeEventArgs(this, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

        //md 20060812
        public void ShallowDeleteSectionNodeRequested(object sender, Events.Node.NodeEventArgs e)
        {
            ShallowDeleteSectionNode(sender, e.Node);
        }

        //md 20060812
        //shallow delete a section node
        //see Commands.TOC.ShallowDeleteSectionNode if you're wondering how the "undo" works
        public void ShallowDeleteSectionNode(object origin, CoreNode node)
        {
            //mdXXX
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }
            //end mdXXX

            Commands.TOC.ShallowDeleteSectionNode command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                command = new Commands.TOC.ShallowDeleteSectionNode
                    (this, node, parent, parent.indexOf(node), visitor.Position, node.getChildCount());
             }

            int numChildren = node.getChildCount();

            for (int i = numChildren - 1; i >= 0; i-- )
            {
                if (Project.GetNodeType(node.getChild(i)) == NodeType.Section)
                {
                    Commands.Command cmdDecrease = this.DecreaseSectionNodeLevel(this, node.getChild(i));
                    if (command != null) command.addSubCommand(cmdDecrease);
                }
                //phrase nodes should be removed
                else if (Project.GetNodeType(node.getChild(i)) == NodeType.Phrase)
                {
                    Commands.Command cmdDeletePhrase = DeletePhraseNodeAndAsset(node.getChild(i));
                    if (command != null) command.addSubCommand(cmdDeletePhrase);
                }
            }

            Commands.Command cmdRemove = this.RemoveNode(this, node);
            if (command != null) command.addSubCommand(cmdRemove);

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060812
        public void MoveSectionNodeDownLinearRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveSectionNodeDownLinear(sender, e.Node);
        }

        //md 20060812
        public void MoveSectionNodeDownLinear(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionNodeDownLinear command = null;

            CoreNode nodeToSwapWith = GetNextSectionNode(node, 0);

            if (nodeToSwapWith != null)
            {
                //System.Windows.Forms.MessageBox.Show(string.Format("Should be swapped with {0}", GetTextMedia(nodeToSwapWith).getText()));
            
                if (origin != this)
                {
                    command = new Commands.TOC.MoveSectionNodeDownLinear(this, node, nodeToSwapWith);
                }
                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int nodePos = visitor.Position;

                visitor = new Visitors.SectionNodePosition(nodeToSwapWith);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int swappedNodePos = visitor.Position;
                int nodeSectionIdx = GetSectionNodeIndex(node);
                int swapSectionIdx = GetSectionNodeIndex(nodeToSwapWith);

                ShallowSwapNodes(node, nodeToSwapWith);

                 //raise the event that the action was completed
                ShallowSwappedSectionNodes(this, new Obi.Events.Node.ShallowSwappedSectionNodesEventArgs(this, node, nodeToSwapWith, nodePos, swappedNodePos, nodeSectionIdx, swapSectionIdx));
           
            }

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));      
        }

        //md 20060812
        public void MoveSectionNodeUpLinearRequested(object sender, Events.Node.NodeEventArgs e)
        {
            MoveSectionNodeUpLinear(sender, e.Node);
        }

        //md 20060812
        public void MoveSectionNodeUpLinear(object origin, CoreNode node)
        {
            Commands.TOC.MoveSectionNodeUpLinear command = null;

            if (node.getParent() == null) return;
            int idx = ((CoreNode)node.getParent()).indexOf(node);

            CoreNode nodeToSwapWith = GetPreviousSectionNode(node, idx - 1);

            if (nodeToSwapWith != null)
            {
             //   System.Windows.Forms.MessageBox.Show(string.Format("Should be swapped with {0}", GetTextMedia(nodeToSwapWith).getText()));
                if (origin != this)
                {
                    command = new Commands.TOC.MoveSectionNodeUpLinear(this, node, nodeToSwapWith);
                }

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int nodePos = visitor.Position;

                visitor = new Visitors.SectionNodePosition(nodeToSwapWith);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                int swappedNodePos = visitor.Position;
                int nodeSectionIdx = GetSectionNodeIndex(node);
                int swapSectionIdx = GetSectionNodeIndex(nodeToSwapWith);

                ShallowSwapNodes(node, nodeToSwapWith);

                
                //raise the event that the action was completed
                ShallowSwappedSectionNodes(this, new Obi.Events.Node.ShallowSwappedSectionNodesEventArgs
                    (this, node, nodeToSwapWith, nodePos, swappedNodePos, nodeSectionIdx, swapSectionIdx));
            }
            
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

      

        /// <summary>
        /// Swap nodes but not their section-subtrees.  Each node retains its own phrases.
        /// </summary>
        /// <param name="firstNode"></param>
        /// <param name="secondNode"></param>
        /// <param name="dir">1: down; -1: up</param>
        /// <remarks>this could be cleaner.  one thing that would help is better shallow-operation support in the API.</remarks>
        //md 20060813
        internal void ShallowSwapNodes(CoreNode firstNode, CoreNode secondNode)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("Swapping {0} with {1}", GetTextMedia(firstNode).getText(), GetTextMedia(secondNode).getText()));
            ArrayList firstNodeSubSections = new ArrayList();
            ArrayList secondNodeSubSections = new ArrayList();

            CoreNode firstNodeParent = (CoreNode)firstNode.getParent();
            CoreNode secondNodeParent = (CoreNode)secondNode.getParent();

            int firstNodeIndex = firstNodeParent.indexOf(firstNode);
            int secondNodeIndex = secondNodeParent.indexOf(secondNode);

            if (HasSubSections(firstNode))
            {
                //remove all the subsections for the first node
                for (int i = firstNode.getChildCount() - 1; i >= 0; i--)
                {
                    if (GetNodeType(firstNode.getChild(i)) == NodeType.Section)
                    {
                        firstNodeSubSections.Add(firstNode.getChild(i));

                        //make sure they're not nested
                        if (firstNode.getChild(i) != secondNode)
                        {
                            firstNode.getChild(i).detach();
                        }
                    }

                }
            }

            if (HasSubSections(secondNode))
            {
                //remove all the subsections for the second node
                for (int i = secondNode.getChildCount() - 1; i >= 0; i--)
                {
                    if (GetNodeType(secondNode.getChild(i)) == NodeType.Section)
                    {
                        secondNodeSubSections.Add(secondNode.getChild(i));
                        //make sure they're not nested
                        if (secondNode.getChild(i) != firstNode)
                        {
                            secondNode.getChild(i).detach();
                        }
                    }
                }
            }

            //reverse the arrays (they were collected backwards)
            firstNodeSubSections.Reverse();
            secondNodeSubSections.Reverse();

            //detach the nodes
            firstNode.detach();
            secondNode.detach();

            //put the second node's former subsections onto the first node
            for (int i = 0; i < secondNodeSubSections.Count; i++)
            {
                if (((CoreNode)secondNodeSubSections[i]) == firstNode)
                {
                    firstNode.appendChild(secondNode);
                }
                else
                {
                    firstNode.appendChild((CoreNode)secondNodeSubSections[i]);
                }
            }

            //put the first node's former subsections on the second node
            for (int i = 0; i < firstNodeSubSections.Count; i++)
            {
                if (((CoreNode)firstNodeSubSections[i]) == secondNode)
                {
                    secondNode.appendChild(firstNode);
                }
                else
                {
                    secondNode.appendChild((CoreNode)firstNodeSubSections[i]);
                }
            }

        
            //check if the two nodes were siblings
            if (secondNodeParent == firstNodeParent)
            {
                //add the lower index first, because otherwise the order is wrong in the end
                if (firstNodeIndex < secondNodeIndex)
                {
                    //adjust the index to make sure it's in range
                    if (firstNodeIndex < 0) firstNodeIndex = 0;
                    else if (firstNodeIndex > firstNodeParent.getChildCount()) firstNodeIndex = firstNodeParent.getChildCount();

                    firstNodeParent.insert(secondNode, firstNodeIndex);

                    //adjust the index to make sure it's in range
                    if (secondNodeIndex < 0) secondNodeIndex = 0;
                    else if (secondNodeIndex > secondNodeParent.getChildCount()) secondNodeIndex = secondNodeParent.getChildCount();

                    firstNodeParent.insert(firstNode, secondNodeIndex);
                }
                else
                {
                    //adjust the index to make sure it's in range
                    if (secondNodeIndex < 0) secondNodeIndex = 0;
                    else if (secondNodeIndex > secondNodeParent.getChildCount()) secondNodeIndex = secondNodeParent.getChildCount();

                    firstNodeParent.insert(firstNode, secondNodeIndex);

                    //adjust the index to make sure it's in range
                    if (firstNodeIndex < 0) firstNodeIndex = 0;
                    else if (firstNodeIndex > firstNodeParent.getChildCount()) firstNodeIndex = firstNodeParent.getChildCount();

                    firstNodeParent.insert(secondNode, firstNodeIndex);
                }
            }//end if the nodes were siblings
            else //they were either nested or in different subtrees
            {
                //check if the second node was nested under the first node
                if (firstNodeParent != secondNode)
                {
                    //adjust the index to make sure it's in range
                    if (firstNodeIndex < 0) firstNodeIndex = 0;
                    else if (firstNodeIndex > firstNodeParent.getChildCount()) firstNodeIndex = firstNodeParent.getChildCount();


                    firstNodeParent.insert(secondNode, firstNodeIndex);
                }
                //check if the first node was nested under the second node
                if (secondNodeParent != firstNode)
                {
                    //adjust the index to make sure it's in range
                    if (secondNodeIndex < 0) secondNodeIndex = 0;
                    else if (secondNodeIndex > secondNodeParent.getChildCount()) secondNodeIndex = secondNodeParent.getChildCount();

                    secondNodeParent.insert(firstNode, secondNodeIndex);
                }
            }
        }

        internal void UndoShallowSwapNodes(CoreNode firstNode, CoreNode secondNode)
        {
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(firstNode);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            int node1Pos = visitor.Position;

            visitor = new Visitors.SectionNodePosition(secondNode);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            int node2Pos = visitor.Position;
            int nodeSectionIdx = GetSectionNodeIndex(firstNode);
            int swapSectionIdx = GetSectionNodeIndex(secondNode);

            ShallowSwapNodes(firstNode, secondNode);

            //raise the event that the action was completed
            ShallowSwappedSectionNodes(this, new Obi.Events.Node.ShallowSwappedSectionNodesEventArgs(this, firstNode, secondNode, node1Pos, node2Pos, nodeSectionIdx, swapSectionIdx));
            
        }

        //would have used a visitor for the next two functions (Get Next/Prev Section Node)
        //but visitors don't go in the prev. direction.
        //md 20060813
        private CoreNode GetNextSectionNode(CoreNode node, int startIdx)
        {
            //look at our children
            for (int i = startIdx; i < node.getChildCount(); i++)
            {
                if (GetNodeType(node.getChild(i)) == NodeType.Section)
                {
                    return node.getChild(i);
                }
            }

            if (node.getParent() == null) return null;
            
            //look at our siblings
            CoreNode parent = (CoreNode)node.getParent();
            return GetNextSectionNode(parent, parent.indexOf(node) + 1);
        }

        //md 20060813
        private CoreNode GetPreviousSectionNode(CoreNode node, int startIdx)
        {
            if (node.getParent() == null) return null;

            //look at our siblings
            CoreNode parent = (CoreNode)node.getParent();

            for (int i = startIdx; i >= 0; i-- )
            {
                CoreNode sibling = parent.getChild(i);

                bool hasSubSections = HasSubSections(sibling);

                if (GetNodeType(sibling) == NodeType.Section &&
                    hasSubSections == false)
                {
                    return sibling;
                }
                else if (hasSubSections == true)
                {
                    CoreNode siblingsChild = sibling.getChild(sibling.getChildCount() - 1);
                    return GetPreviousSectionNode(siblingsChild, sibling.getChildCount() - 1);
                }
                //else it's not the right type of node and we won't consider it
                
            }
            
            //if we can't go up any more levels, return null
            if (GetNodeType(parent) == NodeType.Root) return null;

            return parent;
        }
       
        //md 20060813
        //helper function
        private bool HasSubSections(CoreNode node)
        {
            for (int i = 0; i < node.getChildCount(); i++)
            {
                if (GetNodeType(node.getChild(i)) == NodeType.Section)
                {
                    return true;
                }
            }

            return false;
        }

        //md 20060813
        internal bool CanMoveSectionNodeUp(CoreNode node)
        {
            if (this.getPresentation().getRootNode().getChildCount() == 0)
            {
                return false;
            }

            //the only reason a node couldn't be moved down is if it is the first 
            //child of the root
        
            CoreNode firstRootChild = this.getPresentation().getRootNode().getChild(0);
            if (node == firstRootChild)
            {
                return false;
            }

            return true;
        }

        //md 20060813
        internal bool CanMoveSectionNodeDown(CoreNode node)
        {

            if (this.getPresentation().getRootNode().getChildCount() == 0)
            {
                return false;
            }
            //the only reason a node couldn't be moved down is if it is the last
            //child of the root

            int numRootChildren = this.getPresentation().getRootNode().getChildCount();
            CoreNode lastRootChild = this.getPresentation().getRootNode().getChild(numRootChildren - 1);
            if (node == lastRootChild)
            {
                return false;
            }

            return true;
            
        }

        //md 20060814
        //helper function
        private CoreNode GetLastSectionNodeInSubtree(CoreNode node)
        {
            CoreNode lastSectionNodeChild = GetLastSectionNodeChild(node);

            if (GetLastSectionNodeChild(node) == null)
            {
                //make sure it's a section node (and not the root)
                if (GetNodeType(node) == NodeType.Section)
                    return node;
                else
                    return null;
            }
            else
            {
                return GetLastSectionNodeInSubtree(lastSectionNodeChild);
            }
        }
        
        //md 20060814
        //helper function
        private CoreNode GetLastSectionNodeChild(CoreNode node)
        {
            for (int i = node.getChildCount() - 1; i>=0; i--)
            {
                if (GetNodeType(node.getChild(i)) == NodeType.Section)
                {
                    return node.getChild(i);
                }
            }

            return null;
        }

        //md 20060813
        internal bool CanMoveSectionNodeIn(CoreNode node)
        {
            CoreNode newParent = GetPreviousSectionNodeSibling(node);

            if (newParent == null)
                return false;
            else
                return true;
        }

        //md 20060813
        internal bool CanMoveSectionNodeOut(CoreNode node)
        {
            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (node.getParent() == null ||
                node.getParent().Equals(node.getPresentation().getRootNode()))
            {
                return false;
            }
           
            return true;
        }

        //md 20060813
        public static int GetSectionNodeIndex(CoreNode node)
        {
            int numSections = 0;
            CoreNode parent = (CoreNode)node.getParent();

            for (int i = 0; i < parent.getChildCount(); i++)
            {
                if (GetNodeType(parent.getChild(i)) == NodeType.Section)
                {
                    numSections++;
                }
                if (parent.getChild(i) == node)
                {
                    break;
                }
            }

            return numSections - 1;
        }
    }
}
