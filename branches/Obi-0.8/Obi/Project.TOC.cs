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
        public event Events.SectionNodeHandler AddedSectionNode;      // a section node was added to the TOC
        public event Events.RenameSectionNodeHandler RenamedSectionNode;                // a node was renamed in the presentation
        public event Events.MovedSectionNodeHandler MovedSectionNode;                    // a node was moved in the presentation
        public event Events.SectionNodeHandler DecreasedSectionNodeLevel;  // a node's level was decreased in the presentation
       public event Events.MovedSectionNodeHandler UndidMoveSectionNode;                // a node was restored to its previous location
        public event Events.SectionNodeHandler DeletedSectionNode;                // a node was deleted from the presentation
        //md: toc clipboard stuff
        public event Events.SectionNodeHandler CutSectionNode;
        public event Events.SectionNodeHandler CopiedSectionNode;
        public event Events.SectionNodeHandler UndidCopySectionNode;
        public event Events.SectionNodeHandler PastedSectionNode;
        public event Events.SectionNodeHandler UndidPasteSectionNode;

        //md: shallow-swapped event for move up/down linear
       // public event Events.ShallowSwappedSectionNodesHandler ShallowSwappedSectionNodes;
      
        
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
        public void CreateSiblingSectionNode(object origin, SectionNode contextNode)
        {
            CoreNode parent = (CoreNode)(contextNode == null ? getPresentation().getRootNode() : contextNode.getParent());
            SectionNode sibling = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            if (contextNode == null)
            {
                // first node ever
                parent.appendChild(sibling);
            }
            else if (parent.GetType() == typeof(CoreNode))
            {
                // direct child of the root
                parent.insertAfter(sibling, contextNode);
            }
            else
            {
                // child of another section
                ((SectionNode)parent).AddChildSectionAfter(sibling, contextNode);
            }
            AddedSectionNode(origin, new Events.Node.SectionNodeEventArgs(origin, sibling));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(sibling);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void CreateSiblingSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            CreateSiblingSectionNode(sender, e.Node);
        }

        /// <summary>
        /// Create a new child section for a given section. If the context node is null, add to the root of the tree.
        /// </summary>
        public SectionNode CreateChildSectionNode(object origin, CoreNode parent)
        {
            SectionNode child = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            if (parent == null || parent == getPresentation().getRootNode())
            {
                getPresentation().getRootNode().appendChild(child);
            }
            else
            {
                ((SectionNode)parent).AppendChildSection(child);
            }
            AddedSectionNode(origin, new Events.Node.SectionNodeEventArgs(origin, child));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            Commands.TOC.AddSectionNode command = new Commands.TOC.AddSectionNode(child);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));

            return child;
        }

        public void CreateChildSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
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
        public void AddExistingSectionNode(SectionNode node, CoreNode parent, string originalLabel)
        {
           //shouldn't need to test for this: the toolkit does already if (node.getParent() == null) 
            if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                int index = ((SectionNode)parent).PhraseChildCount + node.Index;
                ((SectionNode)parent).AddChildSection(node, index);
            }
            else
            {
                //TODO!
                //will this case come up?
            }
            if (originalLabel != null) Project.GetTextMedia(node).setText(originalLabel);
 
            AddedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
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
        public void UndeleteSectionNode(SectionNode node, CoreNode parent)
        {
            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent);
            node.acceptDepthFirst(visitor);
        }

        /// <summary>
        /// Remove a node from the core tree. It is detached from the tree and the
        /// </summary>
        //md
        //the command value is returned so it can be used in UndoShallowDelete's undo list
        public Commands.Command RemoveSectionNode(object origin, SectionNode node)
        {
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
                    command = new Commands.TOC.DeleteSectionNode(node);
              //  }
                node.detach();
                DeletedSectionNode(this, new Events.Node.SectionNodeEventArgs(origin, node));
                mUnsaved = true;
                StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

                //md: added condition "origin != this" to accomodate the change made above
                if (command != null && origin != this) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }

            return command;
        }

        public void RemoveSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            RemoveSectionNode(sender, e.Node);
        }

        /// <summary>
        /// reposition the node at the index under its given parent
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void UndoMoveSectionNode(SectionNode node, CoreNode parent, int index)
        {
            if (node.getParent() != null) node.detach();
            parent.insert(node, index);
           
            UndidMoveSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        /// <summary>
        /// Undo increase level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UndoIncreaseSectionNodeLevel(SectionNode node, CoreNode parent, int index)
        {
            UndoMoveSectionNode(node, parent, index);
        }

        

        public void IncreaseSectionNodeLevel(object origin, SectionNode node)
        {

            Commands.TOC.IncreaseSectionNodeLevel command = null;

            if (origin != this)
            {
                CoreNode parent = (CoreNode)node.getParent();
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.IncreaseSectionNodeLevel(node, parent);
            }

            bool succeeded = ExecuteIncreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                int sectionIdx = node.Index;

                //IncreasedNodeLevel(this, new Events.Node.NodeEventArgs(origin, node));
                MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, newParent));

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
        private bool ExecuteIncreaseSectionNodeLevel(SectionNode node)
        {
            //can't increase section level if the node has no "older" siblings
            if (node.Index == 0)
            {
                return false;
            }
            else
            {
                CoreNode newParent; 
                //assumption: the root only has section node children, so we can do this
                if (node.getParent() == getPresentation().getRootNode())
                {
                    newParent = ((CoreNode)node.getParent()).getChild(node.Index - 1);
                    SectionNode movedNode = (SectionNode)node.detach();
                    newParent.appendChild(movedNode);
                }
                //else the node's parent is an ordinary section node
                else
                {
                    newParent = ((SectionNode)node).SectionChild(node.Index - 1);
                    SectionNode movedNode = (SectionNode)node.detach();
                    ((SectionNode)newParent).AppendChildSection(movedNode);
                }

                
                return true;
            }
        }

       
        public void IncreaseSectionNodeLevelRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            IncreaseSectionNodeLevel(sender, e.Node);
        }

       //md
       //the command value is returned so it can be used in UndoShallowDelete's undo list
       public Commands.Command DecreaseSectionNodeLevel(object origin, SectionNode node)
        {
            Commands.TOC.DecreaseSectionNodeLevel command = null;

            //md: need this particular command to be created even if origin = this
            //because its undo fn is required by UndoShallowDelete
            //if (origin != this)
            //{
                CoreNode parent = (CoreNode)node.getParent();
                Visitors.SectionNodePosition nodeVisitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(nodeVisitor);
                //we need to save the state of the node before it is altered
                command = new Commands.TOC.DecreaseSectionNodeLevel(node, parent);
                    
            //}

            bool succeeded = ExecuteDecreaseSectionNodeLevel(node);
            if (succeeded)
            {
                CoreNode newParent = (CoreNode)node.getParent();

                Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                DecreasedSectionNodeLevel(this, new Events.Node.SectionNodeEventArgs(origin, node));
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
        private bool ExecuteDecreaseSectionNodeLevel(SectionNode node)
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

        public void DecreaseSectionNodeLevelRequested(object sender, Events.Node.SectionNodeEventArgs e)
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
        public void UndoDecreaseSectionNodeLevel(SectionNode node, CoreNode parent, int originalChildCount)
        {
            //TODO: rewrite this!!!
           /*
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

            MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));

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

                    MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs
                        (this, (CoreNode)nonOriginalChildren[i], parent,
                        parent.getChildCount() - 1, visitor.Position, childSectionIdx));
                }
            }
            */
        }

        /// <summary>
        /// Change the text label of a node.
        /// </summary>
        public void RenameSectionNode(object origin, SectionNode node, string label)
        {
         
            TextMedia text = GetTextMedia(node);
            Commands.TOC.Rename command = origin == this ? null : new Commands.TOC.Rename(this, node, text.getText(), label);
            GetTextMedia(node).setText(label);
            RenamedSectionNode(this, new Events.Node.RenameSectionNodeEventArgs(origin, node, label));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameSectionNodeRequested(object sender, Events.Node.RenameSectionNodeEventArgs e)
        {
            RenameSectionNode(sender, e.Node, e.Label);
        }

        //md 20060810
        public void CutSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            DoCutSectionNode(sender, e.Node);
        }

        //md 20060810
        public void DoCutSectionNode(object origin, SectionNode node)
        {
            if (node == null) return;

            CoreNode parent = (CoreNode)node.getParent();
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(node);
            getPresentation().getRootNode().acceptDepthFirst(visitor);
            //we need to save the state of the node before it is altered
            Commands.TOC.CutSectionNode command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CutSectionNode(node);
            }

            mClipboard.Section = node;
            node.detach();

            CutSectionNode(this, new Events.Node.SectionNodeEventArgs(origin, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCutSectionNode(SectionNode node, CoreNode parent)
        {
            UndeleteSectionNode(node, parent);
            mClipboard.Section = null;
        }

        //md 20060810
        public void CopySectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            CopySectionNode(sender, e.Node);
        }

        //md 20060810
        public void CopySectionNode(object origin, SectionNode node)
        {
            if (node == null) return;

            Commands.TOC.CopySectionNode command = null;

            if (origin != this)
            {
                command = new Commands.TOC.CopySectionNode(node);
            }

            //the actual copy operation
            mClipboard.Section = node.copy(true);

            CopiedSectionNode(this, new Events.Node.SectionNodeEventArgs(origin, mClipboard.Section));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoCopySectionNode(SectionNode node)
        {
            mClipboard.Section = null;

           UndidCopySectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
        }

        //md 20060810
        // modified by JQ 20060818: can paste under the root node if we have deleted the first and only heading.
        public void PasteSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            if (e.Node != null)
            {
                PasteSectionNode(sender, e.Node);
            }
            else
            {
                PasteSectionNode(sender, getPresentation().getRootNode());
            }
        }

        //md 20060810
        //"paste" will paste the clipboard contents as the first child of the given node
        // modified by JQ 20060818: can paste under the root node if we have deleted the first and only heading.
        public void PasteSectionNode(object origin, CoreNode parent)
        {
            if (parent == null) return;

            Commands.TOC.PasteSectionNode command = null;

            SectionNode pastedSection = mClipboard.Section.copy(true);

            //don't clear the clipboard, we can use it again

            if (origin != this)
            {
                command = new Commands.TOC.PasteSectionNode(parent, pastedSection);
            }

            //the actual paste operation
            parent.insert(pastedSection, 0);

            //reconstruct the assets
            Obi.Visitors.CopyPhraseAssets assVisitor = new Obi.Visitors.CopyPhraseAssets(mAssManager, this);
            pastedSection.acceptDepthFirst(assVisitor);
           
            Visitors.SectionNodePosition visitor = new Visitors.SectionNodePosition(pastedSection);
            getPresentation().getRootNode().acceptDepthFirst(visitor);

            
            PastedSectionNode(this, new Events.Node.SectionNodeEventArgs(origin, pastedSection));

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20060810
        public void UndoPasteSectionNode(SectionNode node)
        {
            node.detach();
           
            UndidPasteSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

        //md 20060812
        public void ShallowDeleteSectionNodeRequested(object sender, Events.Node.SectionNodeEventArgs e)
        {
            ShallowDeleteSectionNode(sender, e.Node);
        }

        //md 20060812
        //shallow delete a section node
        //see Commands.TOC.ShallowDeleteSectionNode if you're wondering how the "undo" works
        public void ShallowDeleteSectionNode(object origin, SectionNode node)
        {
            //TODO: update this
            /*
            List<Commands.Command> commands = new List<Commands.Command>();

            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            //these lines only need to be executed if origin != this
            CoreNode parent = null;
            Visitors.SectionNodePosition visitor = null;
            int nodeIndex = 0;
            int nodeChildCount = 0;
            if (origin != this)
            {
                parent = (CoreNode)node.getParent();
                visitor = new Visitors.SectionNodePosition(node);
                getPresentation().getRootNode().acceptDepthFirst(visitor);

                nodeIndex = parent.indexOf(node);
                nodeChildCount = node.getChildCount();
            }
            
            NodeType nodeType;
            nodeType = Project.GetNodeType(node);
            if (nodeType != NodeType.Section)
            {
                throw new Exception(string.Format("Expected a SectionNode; got a {0}", nodeType.ToString()));
            }

            Commands.TOC.ShallowDeleteSectionNode command = null;
           
            int numChildren = node.getChildCount();

            for (int i = numChildren - 1; i >= 0; i-- )
            {
                if (Project.GetNodeType(node.getChild(i)) == NodeType.Section)
                {
                    Commands.Command cmdDecrease = this.DecreaseSectionNodeLevel(this, node.getChild(i));
                    commands.Add(cmdDecrease);
                }
                //phrase nodes should be removed
                else if (Project.GetNodeType(node.getChild(i)) == NodeType.Phrase)
                {
                    Commands.Command cmdDeletePhrase = DeletePhraseNodeAndAsset(node.getChild(i));
                    commands.Add(cmdDeletePhrase);
                }
            }

            Commands.Command cmdRemove = this.RemoveSectionNode(this, node);
            commands.Add(cmdRemove);

            //additional "null" tests added for completeness, since the logic was
            //separated out into this piece and the parent- and visitor-setting code above
            if (origin != this && parent != null && visitor != null)
            {
                command = new Commands.TOC.ShallowDeleteSectionNode
                    (this, node, parent, nodeIndex, 
                    visitor.Position, nodeChildCount, commands);
            }

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
             */
        }


      

        /// <summary>
        /// Swap nodes but not their section-subtrees.  Each node retains its own phrases.
        /// </summary>
        /// <param name="firstNode"></param>
        /// <param name="secondNode"></param>
        /// <param name="dir">1: down; -1: up</param>
        /// <remarks>this could be cleaner.  one thing that would help is better shallow-operation support in the API.</remarks>
        //md 20060813
        internal void ShallowSwapNodes(SectionNode firstNode, SectionNode secondNode)
        {
            //TODO: update this
            //TODO: do we need it since section up/down movement is gone?
            /*
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
             */
        }

        internal void UndoShallowSwapNodes(SectionNode firstNode, SectionNode secondNode)
        {
            //TODO: update this
            /*
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
            */
        }

        //would have used a visitor for the next two functions (Get Next/Prev Section Node)
        //but visitors don't go in the prev. direction.
        //md 20060813
        //TODO: remove this?  or modify to fit SectionNodes
        /*private CoreNode GetNextSectionNode(SectionNode node, int startIdx)
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
        private CoreNode GetPreviousSectionNode(SectionNode node, int startIdx)
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
        }*/
       
        //md 20060813
        //helper function
       /* private bool HasSubSections(SectionNode node)
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
        */
      

        //md 20060814
        //helper function
        private SectionNode GetLastSectionNodeInSubtree(SectionNode node)
        {
            SectionNode lastSectionNodeChild = node.SectionChild(node.SectionChildCount - 1);

            if (lastSectionNodeChild == null)
            {
                return node;
            }
            else
            {
                return GetLastSectionNodeInSubtree(lastSectionNodeChild);
            }
        }

        //md 20060813
        internal bool CanMoveSectionNodeIn(SectionNode node)
        {
            //if it's not the first section
            if (node.Index > 0)
                return false;
            else
                return true;
        }

        //md 20060813
        internal bool CanMoveSectionNodeOut(SectionNode node)
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
    }
}
