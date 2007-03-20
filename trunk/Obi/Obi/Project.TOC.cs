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
        public event Events.SectionNodeHandler AddedSectionNode;           // a section node was added to the TOC
        public event Events.RenameSectionNodeHandler RenamedSectionNode;   // a node was renamed in the presentation
        public event Events.MovedSectionNodeHandler MovedSectionNode;      // a node was moved in the presentation
        public event Events.SectionNodeHandler DecreasedSectionNodeLevel;  // a node's level was decreased in the presentation
        public event Events.MovedSectionNodeHandler UndidMoveSectionNode;  // a node was restored to its previous location
        public event Events.SectionNodeHandler DeletedSectionNode;         // a node was deleted from the presentation
        public event Events.SectionNodeHandler PastedSectionNode;
        public event Events.SectionNodeHandler UndidPasteSectionNode;
        public event Events.SectionNodeHandler ToggledSectionUsedState;

        /// <summary>
        /// Cut the whole subtree for a section node.
        /// Actually the section node is just cut off the tree,
        /// but all phrases must be removed from the manager as
        /// well.
        /// </summary>
        /// <param name="node">The node to cut.</param>
        public void CutSectionNode(SectionNode node)
        {
            if (node != null)
            {
                SectionNode copy = node.copy(true);
                Commands.TOC.CutSectionNode command = new Commands.TOC.CutSectionNode(copy);
                command.AddCommand(RemoveSectionNode(node));
                mClipboard.Section = copy;
                Modified();
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a section node as a whole (i.e. the entire subtree.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        public void CopySectionNode(SectionNode node)
        {
            if (node != null)
            {
                SectionNode copy = node.copy(true);
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(copy)));
                mClipboard.Section = copy;
            }
        }

        /// <summary>
        /// Paste the clipboard node under the parent (or the root if no parent is given.)
        /// </summary>
        /// <param name="parent">Parent to paste under (root if null.)</param>
        public SectionNode PasteSectionNode(CoreNode parent)
        {
            if (mClipboard.Section != null)
            {
                if (parent == null) parent = RootNode;
                SectionNode pasted = PasteCopyOfSectionNode(mClipboard.Section, parent);
                CommandCreated(this,
                    new Events.Project.CommandCreatedEventArgs(new Commands.TOC.PasteSectionNode(pasted, parent)));
                return pasted;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Delete a node and its whole subtree from the core tree.
        /// </summary>
        public void DeleteSectionNode(SectionNode node)
        {
            if (node != null)
            {
                Commands.TOC.DeleteSectionNode command = RemoveSectionNode(node);
                Modified();
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Create a sibling section for a given section.
        /// The context node may be null if this is the first node that is added, in which case
        /// we add a new child to the root (and not a sibling.)
        /// </summary>
        /// <param name="contextNode">The sibling of the new node.</param>
        /// <returns>The new section node.</returns>
        public SectionNode CreateSiblingSectionNode(SectionNode contextNode)
        {
            CoreNode parent = (CoreNode)(contextNode == null ? RootNode : contextNode.getParent());
            SectionNode sibling = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            if (contextNode == null)
            {
                // first node ever
                parent.appendChild(sibling);
            }
            else
            {
                AddChildSectionBefore(sibling, contextNode, parent);
            }
            AddedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, sibling));
            Modified(new Commands.TOC.AddSectionNode(sibling));
            return sibling;
        }

        /// <summary>
        /// Create a new child section for a given section.
        /// If the context node is null, add to the root of the tree.
        /// </summary>
        /// <param name="parent">The parent section of the new section.</param>
        /// <returns>The created section.</returns>
        public SectionNode CreateChildSectionNode(CoreNode parent)
        {
            SectionNode child = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(SectionNode.Name, ObiPropertyFactory.ObiNS);
            if (parent == null)
            {
                RootNode.appendChild(child);
            }
            else
            {
                AppendChildSection(child, parent);
            }
            AddedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, child));
            Modified(new Commands.TOC.AddSectionNode(child));
            return child;
        }

        public void RenameSectionNodeWithCommand(SectionNode node, string label)
        {
            Commands.TOC.Rename command = new Commands.TOC.Rename(node, label);
            RenameSectionNode(node, label);
            CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        public void RenameSectionNode(SectionNode node, string label)
        {
            node.Label = label;
            RenamedSectionNode(this, new Obi.Events.Node.RenameSectionNodeEventArgs(this, node, label));
            Modified();
        }





        /// <summary>
        /// Readd a section node that was previously delete and restore all its contents.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        public void UndeleteSectionNode(SectionNode node, CoreNode parent, int index)
        {
            Visitors.UndeleteSubtree visitor = new Visitors.UndeleteSubtree(this, parent, index);
            node.acceptDepthFirst(visitor);
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
            if (node.getParent() != null) node.DetachFromParent();
            AddChildSection(node, parent, index);
            UndidMoveSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));
            Modified();
        }

        /// <summary>
        /// Undo increase level
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="node"></param>
        //added by marisa 01 aug 06
        public void UnincreaseSectionNodeLevel(SectionNode node, CoreNode parent, int index)
        {
            UndoMoveSectionNode(node, parent, index);
        }

        /// <summary>
        /// Increase the level of a node.
        /// </summary>
        /// <param name="node">The node to increase the level of.</param>
        /// <returns>Return an increase command (for use in shallow section commands.)</returns>
        public Commands.TOC.IncreaseSectionNodeLevel IncreaseSectionNodeLevel(SectionNode node)
        {
            Commands.TOC.IncreaseSectionNodeLevel command = new Commands.TOC.IncreaseSectionNodeLevel(node, (CoreNode)node.getParent());
            SectionNode newParent = node.ParentSection == null ?
                (SectionNode)((CoreNode)node.getParent()).getChild(node.Index - 1) :
                node.ParentSection.SectionChild(node.Index - 1);
            node.DetachFromParent();
            AppendChildSection(node, newParent);
            MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, newParent));
            Modified();
            return command;
        }

        /// <summary>
        /// Move a section node out (decrease its level.)
        /// </summary>
        /// <param name="node">The node to move out.</param>
        public void MoveSectionNodeOut(SectionNode node)
        {
            if (CanMoveSectionNodeOut(node))
            {
                Commands.TOC.DecreaseSectionNodeLevel command = DecreaseSectionNodeLevel(node);
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Decrease the level of a section node.
        /// </summary>
        /// <param name="node">The node getting it.</param>
        public Commands.TOC.DecreaseSectionNodeLevel DecreaseSectionNodeLevel(SectionNode node)
        {
            SectionNode parent = node.ParentSection;
            Commands.TOC.DecreaseSectionNodeLevel command = new Commands.TOC.DecreaseSectionNodeLevel(node, parent);
            // Make a list of the following siblings which will become children of the node
            List<SectionNode> newChildren = new List<SectionNode>();            
            for (int i = parent.SectionChildCount - 1; i > node.Index; --i)
            {
                newChildren.Insert(0, parent.SectionChild(i).DetachFromParent());
            }
            node.DetachFromParent();
            CoreNode newParent = (CoreNode)parent.getParent();
            AddChildSection(node, newParent, parent.Index + 1);
            newChildren.ForEach(delegate(SectionNode n) { AppendChildSection(n, node); });
            DecreasedSectionNodeLevel(this, new Events.Node.SectionNodeEventArgs(this, node));
            Modified();
            return command;
        }

        /// <summary>
        /// Undo decrease node level is a bit tricky because we might have to move some of the node's
        /// children out a level, but only if they were newly adopted after the decrease level action happened. 
        /// </summary>
        /// <param name="originalChildCount">number of children this node used to have before the decrease level
        /// action happened</param>
        public void UndecreaseSectionNodeLevel(SectionNode node, CoreNode parent, int originalChildCount)
        {
            List<SectionNode> unOriginalChildren = new List<SectionNode>();
            for (int i = node.SectionChildCount - 1; i >= originalChildCount; --i)
            {
                unOriginalChildren.Insert(0, node.SectionChild(i).DetachFromParent());
            }
            node.DetachFromParent();
            AppendChildSection(node, parent);
            MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, node, parent));
            foreach (SectionNode child in unOriginalChildren)
            {
                AppendChildSection(child, parent);
                MovedSectionNode(this, new Events.Node.MovedSectionNodeEventArgs(this, child, parent));
            }
        }

        /// <summary>
        /// Increase the level of a section node.
        /// </summary>
        public void MoveSectionNodeIn(SectionNode node)
        {
            if (CanMoveSectionNodeIn(node))
            {
                Commands.TOC.IncreaseSectionNodeLevel command = IncreaseSectionNodeLevel(node);
                CommandCreated(this, new Obi.Events.Project.CommandCreatedEventArgs(command));
            }
        }

        //this is almost the same as ShallowDeleteSectionNode
        public void DoShallowCutSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            mClipboard.Section = node.copy(false);
            Commands.TOC.ShallowCutSectionNode command = origin == this ?
                null : new Commands.TOC.ShallowCutSectionNode(node);
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                Commands.Command cmdDecrease = DecreaseSectionNodeLevel(node.SectionChild(i));
                if (command != null) command.AddCommand(cmdDecrease);
            }
            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                Commands.Command cmdDeletePhrase = RemovePhraseNodeAndAsset(node.PhraseChild(i));
                if (command != null) command.AddCommand(cmdDeletePhrase);
            }
            Commands.Command cmdRemove = RemoveSectionNode(node);
            if (command != null) command.AddCommand(cmdRemove);
            Modified();
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
        }

        //md 20061220
        public void UndoShallowCutSectionNode()
        {
            mClipboard.Section = null;
            //the rest of the undo operations for this command are found in 
            //Commands.TOC.ShallowCutSectionNode
        }

        //md 20061220
        internal void UndoShallowCopySectionNode(SectionNode node)
        {
            mClipboard.Section = null;

            //no event is raised (i.e. UndidShallowCopySectionNode) because no one cares

            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
     
        }

        //md 20060810
        public void UndoPasteSectionNode(SectionNode node)
        {
            node.DetachFromParent();
           
            UndidPasteSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));

        }

        //md 20060812
        //shallow delete a section node
        //see Commands.TOC.ShallowDeleteSectionNode if you're wondering how the "undo" works
        public void ShallowDeleteSectionNode(object origin, SectionNode node)
        {
            //we have to gather this data here, because it might be different at the end
            //however, we can't create the command here, because its data isn't ready yet
            //these lines only need to be executed if origin != this
       
            Commands.TOC.ShallowDeleteSectionNode command = null;
        
            if (origin != this)
            {  
                command = new Commands.TOC.ShallowDeleteSectionNode(node);
            }
               
            int numChildren = node.SectionChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                Commands.Command cmdDecrease = DecreaseSectionNodeLevel(node.SectionChild(i));
                command.AddCommand(cmdDecrease);
            }

            numChildren = node.PhraseChildCount;
            for (int i = numChildren - 1; i>=0; i--)
            {
                Commands.Command cmdDeletePhrase = RemovePhraseNodeAndAsset(node.PhraseChild(i));
                command.AddCommand(cmdDeletePhrase);
            }

            Commands.Command cmdRemove = this.RemoveSectionNode(node);
            command.AddCommand(cmdRemove);

          
            mUnsaved = true;
            StateChanged(this, new Events.Project.StateChangedEventArgs(Events.Project.StateChange.Modified));
            if (command != null) CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
             
        }

        /// <summary>
        /// True if the node can be moved in.
        /// </summary>
        public bool CanMoveSectionNodeIn(SectionNode node)
        {
            return node != null && node.Index > 0;
        }

        /// <summary>
        /// True if the node can be moved out.
        /// </summary>
        public bool CanMoveSectionNodeOut(SectionNode node)
        {
            return node != null && node.ParentSection != null;
        }

        //helper function which tests for parent being root
        //md 20061204
        internal void AddChildSection(SectionNode node, CoreNode parent, int index)
        {
            if (parent.Equals(getPresentation().getRootNode()))
            {
                parent.insert(node, index);
            }
            else if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                ((SectionNode)parent).AddChildSection(node, index);
            }          
        }

        //helper function which tests for parent being root
        //md 20061204
        internal void AppendChildSection(SectionNode node, CoreNode parent)
        {
            if (parent.Equals(getPresentation().getRootNode()))
            {
                parent.appendChild(node);
            }
            else if (parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                ((SectionNode)parent).AppendChildSection(node);
            }         
        }






        //helper function which tests for parent being root
        //md 20061204
        private void AddChildSectionBefore(SectionNode node, SectionNode contextNode, CoreNode parent)
        {
            if (parent is CoreNode)
            {
                parent.insertBefore(node, contextNode);
            }
            else if (parent is SectionNode)
            {
                ((SectionNode)parent).AddChildSectionBefore(node, contextNode);
            }
        }

        /// <summary>
        /// Paste a copy of a section node under a parent. Both must be defined.
        /// Project is modified but no command is issued.
        /// </summary>
        /// <param name="node">The node to paste.</param>
        /// <param name="parent">The parent under which to paste (append.)</param>
        /// <returns>The node actually pasted.</returns>
        public SectionNode PasteCopyOfSectionNode(SectionNode node, CoreNode parent)
        {
            SectionNode copy = node.copy(true);
            if (parent is SectionNode)
            {
                ((SectionNode)parent).AppendChildSection(copy);
            }
            else
            {
                parent.appendChild(copy);
            }
            PastedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, copy));
            Modified();
            return copy;
        }

        /// <summary>
        /// Add a section that had previously been added.
        /// </summary>
        /// <param name="node">The section node to (re)add.</param>
        /// <param name="parent">Its parent node.</param>
        /// <param name="index">Its index in the parent.</param>
        public void ReaddSectionNode(SectionNode node, CoreNode parent, int index)
        {
            AddChildSection(node, parent, index);
            AddedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
        }

        /// <summary>
        /// Remove a section node and all of its phrases and subsections from the project.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        /// <returns>The corresponding delete command.</returns>
        public Commands.TOC.DeleteSectionNode RemoveSectionNode(SectionNode node)
        {
            Commands.TOC.DeleteSectionNode command = new Commands.TOC.DeleteSectionNode(node);
            for (int i = node.PhraseChildCount - 1; i >= 0; --i)
            {
                command.AddCommand(RemovePhraseNodeAndAsset(node.PhraseChild(i)));
            }
            for (int i = node.SectionChildCount - 1; i >= 0; --i)
            {
                command.AddCommand(RemoveSectionNode(node.SectionChild(i)));
            }
            DeletedSectionNode(this, new Events.Node.SectionNodeEventArgs(this, node));
            node.DetachFromParent();
            return command;
        }

        /// <summary>
        /// Shallow cut of a section node, i.e. only the strip.
        /// Sub-sections are moved back one level.
        /// </summary>
        /// <param name="node">The node to cut.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCutSectionNode(SectionNode node)
        {
            if (node != null)
            {
                Commands.TOC.ShallowCutSectionNode command = new Commands.TOC.ShallowCutSectionNode(node);
                mClipboard.Section = node.copy(false);
                for (int i = node.SectionChildCount - 1; i >= 0; --i)
                {
                    command.AddCommand(DecreaseSectionNodeLevel(node.SectionChild(i)));
                }
                for (int i = node.PhraseChildCount - 1; i >= 0; --i)
                {
                    command.AddCommand(RemovePhraseNodeAndAsset(node.PhraseChild(i)));
                }
                command.AddCommand(RemoveSectionNode(node));
                Modified();
                CommandCreated(this, new Events.Project.CommandCreatedEventArgs(command));
            }
        }

        /// <summary>
        /// Copy a section node and its phrases, but not its subsections (i.e. copy one strip.)
        /// The project is unmodified, only the clipboard gets updated.
        /// </summary>
        /// <param name="node">The node to copy.</param>
        /// <param name="issueCommand">Issue a command if true.</param>
        public void ShallowCopySectionNode(SectionNode node, bool issueCommand)
        {
            if (node != null)
            {
                object data = mClipboard.Data;
                SectionNode copy = node.copy(false);
                for (int i = 0; i < node.PhraseChildCount; ++i)
                {
                    copy.AppendChildPhrase(node.PhraseChild(i).copy(true));
                }
                mClipboard.Section = copy;
                if (issueCommand)
                {
                    // CommandCreated(this,
                    //    new Events.Project.CommandCreatedEventArgs(new Commands.TOC.CopySectionNode(data, copy)));
                }
            }
        }

        /// <summary>
        /// Toggle the use state of a section node.
        /// </summary>
        internal void ToggleSectionUsedState(Events.Node.SectionNodeEventArgs e)
        {
            e.Node.Used = !e.Node.Used;
            ToggledSectionUsedState(this, e);
        }
    }
}
