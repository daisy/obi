using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using urakawa.media;

namespace Obi.UserControls
{
    public partial class TOCPanel
    {
        /// <summary>
        /// Change the label of the tree view node.
        /// This is in response to external renames (i.e. those not originating from within the tree view itself)
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the tree node being renamed.</param>
        internal void SyncRenamedSectionNode(object sender, Events.Node.Section.RenameEventArgs e)
        {
            if (e.Origin != this)
            {
                TreeNode treeNode = FindTreeNodeWithoutLabel(e.Node);
                treeNode.Text = e.NewLabel;
            }
        }

        /// <summary>
        /// Add a section to the tree view. If we were the ones to request its addition, 
        /// also start editing its label right now.
        ///
        /// The new heading has already been created as a <see cref="CoreNode"/>.  
        /// It is in its correct place in the core tree.  
        /// Now we need to add it as a <see cref="TreeNode"/> so 
        /// it shows up in the tree view. Internally, the new <see cref="TreeNode"/>
        /// is given the key of its <see cref="CoreNode"/> object's hash code.
        /// This makes it faster to find a <see cref="TreeNode"/> 
        /// based on a given <see cref="CoreNode"/>.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the new heading to add to the tree</param>
        internal void SyncAddedSectionNode(object sender, Events.Node.Section.AddedEventArgs e)
        {
            TreeNode newTreeNode = ShallowAddSectionNode(e.Node, e.Index);
            //start editing if the request to add a node happened in the tree view
            if (e.Origin.Equals(this))
            {
                newTreeNode.BeginEdit();
            }
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            mTocTree.SelectedNode = newTreeNode;
        }

        /// <summary>
        /// Add a new tree node for a section node. The operation is shallow, that is it adds only this node.
        /// </summary>
        /// <param name="node">The section node to add. It must not have children.</param>
        /// <param name="index">The index in the parent.</param>
        /// <returns>The created tree node.</returns>
        private TreeNode ShallowAddSectionNode(SectionNode node, int index)
        {
            TreeNode newTreeNode;
            if (node.getParent().getParent() != null)
            {
                TreeNode relTreeNode = FindTreeNodeFromSectionNode((SectionNode)node.getParent());
                newTreeNode = relTreeNode.Nodes.Insert(index, node.Id.ToString(), node.Label);
            }
            else
            {
                newTreeNode = mTocTree.Nodes.Insert(index, node.Id.ToString(), node.Label);
            }
            newTreeNode.Tag = node;
            return newTreeNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sectionIndex"></param>
        /// <returns></returns>
        private TreeNode DeepAddSectionNode(SectionNode node, int sectionIndex)
        {
            TreeNode newTreeNode = ShallowAddSectionNode(node, sectionIndex);
            for (int i = 0; i < node.SectionChildCount; i++)
            {
                DeepAddSectionNode(node.SectionChild(i), i);
            }
            return newTreeNode;
        }

        /// <summary>
        /// Remove a node from the tree view.
        /// This will remove the whole subtree.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the node to be removed.</param>
        internal void SyncDeletedSectionNode(object sender, Events.Node.Section.EventArgs e)
        {
            if (e.Node != null)
            {
                TreeNode treeNode = FindTreeNodeFromSectionNode(e.Node);
                mTocTree.SelectedNode = treeNode.PrevVisibleNode;
                if (mTocTree.SelectedNode != null)
                {
                    mTocTree.SelectedNode.EnsureVisible();
                }
                treeNode.Remove();
            }
        }


        internal void SyncMovedSectionNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromSectionNode((SectionNode)e.Node);

            TreeNode parent = Project.GetNodeType(e.Parent) == NodeType.Section ? FindTreeNodeFromSectionNode((SectionNode)e.Parent) : null;

            if (selected == null) return;

            TreeNode clone = (TreeNode)selected.Clone();

            selected.Remove();

            TreeNodeCollection siblings = null;
            if (parent == null)
            {
                siblings = mTocTree.Nodes;
            }
            else
            {
                siblings = parent.Nodes;
            }

            siblings.Insert(e.SectionNodeIndex, clone);
            clone.ExpandAll();
            clone.EnsureVisible();
            if (mTocTree.SelectedNode != null)
            {
                mTocTree.SelectedNode = clone;
            }
        }

        /// <summary>
        /// Decrease the node level.
        /// When a node becomes shallower, it adopts its former younger siblings.
        /// We'll have to get feedback on how users like this behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncDecreasedSectionNodeLevel(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromSectionNode((SectionNode)e.Node);
            ExecuteDecreaseNodeLevel(selected);
        }

        //this logic was separated from the SyncXXX function because
        //we need to use it separately during a shallow delete
        internal void ExecuteDecreaseNodeLevel(TreeNode selectedNode)
        {
            //the only reason we can't decrease the level is if the node is already 
            //at the outermost level
            if (selectedNode.Parent == null)
            {
                return;
            }

            ArrayList futureChildren = new ArrayList();

           //make copies of our future children, and remove them from the tree
            for (int i = selectedNode.Parent.Nodes.Count - 1; i > selectedNode.Index; i--)
            {
                TreeNode node = selectedNode.Parent.Nodes[i];
                futureChildren.Add(node.Clone());
                //MessageBox.Show(String.Format("About to remove {0}", node.Text));
                node.Remove();
            }

            //we collected the nodes in reverse-order, so switch them around
            futureChildren.Reverse();
         
            TreeNodeCollection siblingCollection = null;

            //move it out a level
            if (selectedNode.Parent.Parent != null)
            {
                siblingCollection = selectedNode.Parent.Parent.Nodes;
            }
            else
            {
                siblingCollection = mTocTree.Nodes;
            }

            int newIndex = selectedNode.Parent.Index + 1;

            TreeNode clone = (TreeNode)selectedNode.Clone();
            selectedNode.Remove();

            siblingCollection.Insert(newIndex, clone);

            foreach (object node in futureChildren)
            {
                clone.Nodes.Add((TreeNode)node);
            }

            clone.ExpandAll();
            clone.EnsureVisible();
            mTocTree.SelectedNode = clone;
        }

        //md 20060810
        internal void SyncCutSectionNode(object sender, Events.Node.Section.EventArgs e)
        {
            SyncDeletedSectionNode(sender, e);
        }

        //md 20060810
        //does nothing; just a placeholder
        internal void SyncCopiedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {    
        }

        //md 20060810
        //does nothing; just a placeholder
        internal void SyncUndidCopySectionNode(object sender, Events.Node.NodeEventArgs e)
        {
        }

        //md 20060810
        //e.Node is what was just pasted in
        internal void SyncPastedSectionNode(object sender, Events.Node.Section.AddedEventArgs e)
        {
           //add a subtree
            TreeNode uncutNode = DeepAddSectionNode(e.Node, e.Node.SectionIndex);
            uncutNode.ExpandAll();
            uncutNode.EnsureVisible();
            mTocTree.SelectedNode = uncutNode;
        }

        internal void SyncUndidPasteSectionNode(object sender, Events.Node.Section.EventArgs e)
        {
            TreeNode pastedNode = FindTreeNodeFromSectionNode(e.Node);

            //focus on the previous node
            mTocTree.SelectedNode = pastedNode.PrevVisibleNode;
            if (mTocTree.SelectedNode != null)
            {
                mTocTree.SelectedNode.ExpandAll();
                mTocTree.SelectedNode.EnsureVisible();
            }
            if (pastedNode != null)
            {
                pastedNode.Remove();
            }

        }

        //md 2006 08 13
        //this is pretty lazy, but the alternative was pretty ugly and unstable
        //when there is better support for shallow operations in the core tree, we can 
        //use them in Project.ShallowSwap..() and synchronize the toc view at each step
        internal void SyncShallowSwapNodes(object sender, Events.Node.ShallowSwappedSectionNodesEventArgs e)
        {
            mTocTree.Nodes.Clear();

            SynchronizeWithCoreTree((urakawa.core.CoreNode)e.Node.getPresentation().getRootNode());
        
            //focus on the first swapped node
            mTocTree.SelectedNode = FindTreeNodeFromSectionNode((SectionNode)e.Node);
            if (mTocTree.SelectedNode != null)
            {
                mTocTree.SelectedNode.ExpandAll();
                mTocTree.SelectedNode.EnsureVisible();
            }
        }
    }
}
