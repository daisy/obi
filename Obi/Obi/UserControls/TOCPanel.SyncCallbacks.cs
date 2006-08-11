using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media;
using System.Collections;


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
        internal void SyncRenamedSectionNode(object sender, Events.Node.RenameNodeEventArgs e)
        {
            if (e.Origin != this)
            {
                TreeNode treeNode = FindTreeNodeWithoutLabel(e.Node);
                treeNode.Text = e.Label;
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

        internal void SyncAddedSectionNode(object sender, Events.Node.AddedSectionNodeEventArgs e)
        {
            TreeNode newTreeNode;
            string label = Project.GetTextMedia(e.Node).getText();
            if (e.Node.getParent().getParent() != null)
            {
                TreeNode relTreeNode = FindTreeNodeFromCoreNode((urakawa.core.CoreNode)e.Node.getParent());
                newTreeNode = relTreeNode.Nodes.Insert(e.Index, e.Node.GetHashCode().ToString(), label);
            }
            else
            {
                newTreeNode = mTocTree.Nodes.Insert(e.Index, e.Node.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = e.Node;
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            mTocTree.SelectedNode = newTreeNode;
            //start editing if the request to add a node happened in the tree view
            if (e.Origin.Equals(this))
            {
                newTreeNode.BeginEdit();
            }
        }



        /// <summary>
        /// Remove a node from the tree view.
        /// This will remove the whole subtree.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the node to be removed.</param>
        internal void SyncDeletedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            if (e.Node != null)
            {
                TreeNode treeNode = FindTreeNodeFromCoreNode(e.Node);
                treeNode.Remove();
            }
        }

        /// <summary>
        /// This function deletes a node and promotes its children to be one level shallower
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SyncShallowDeletedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            System.Windows.Forms.TreeNode selected = FindTreeNodeFromCoreNode(e.Node);
            TreeNode newSelection = null;

            //save the first child as our new selection (for the end of this function)
            if (selected.Nodes.Count > 0)
            {
                newSelection = selected.Nodes[0];
            }

            foreach (TreeNode childnode in selected.Nodes)
            {
                ExecuteDecreaseNodeLevel(childnode);
            }

            selected.Remove();

            //make the currently selected node something reasonable
            if (newSelection != null)
            {
                mTocTree.SelectedNode = newSelection;
            }

        }

        internal void SyncMovedSectionNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);

            TreeNode parent = Project.GetNodeType(e.Parent) == NodeType.Section ? FindTreeNodeFromCoreNode(e.Parent) : null;

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

            siblings.Insert(e.Index, clone);
            clone.ExpandAll();
            clone.EnsureVisible();
            mTocTree.SelectedNode = clone;
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
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);
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

            int idx = 0;
            //make copies of our future children, and remove them from the tree
            foreach (TreeNode node in selectedNode.Parent.Nodes)
            {
                if (node.Index > selectedNode.Index)
                {
                    futureChildren.Add(node.Clone());
                    node.Remove();
                    idx++;
                }
            }

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
        internal void SyncCutSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);
            if (selected != null)
            {
                mClipboard = (TreeNode)selected.Clone();
                selected.Remove();
            }
        }

        //md 20060810
        internal void SyncUndidCutSectionNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            if (mClipboard == null) return;

            TreeNode parent = Project.GetNodeType(e.Parent) == NodeType.Section ? FindTreeNodeFromCoreNode(e.Parent) : null;

            TreeNodeCollection siblings = null;
            if (parent == null)
            {
                siblings = mTocTree.Nodes;
            }
            else
            {
                siblings = parent.Nodes;
            }

            TreeNode uncutNode = mClipboard;

            siblings.Insert(e.Index, uncutNode);
            uncutNode.ExpandAll();
            uncutNode.EnsureVisible();
            mTocTree.SelectedNode = uncutNode;

            mClipboard = null;
        }

        //md 20060810
        internal void SyncCopiedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromCoreNode(e.Node);

            if (selected != null)
            {
                mClipboard = (TreeNode)selected.Clone();
            }
        }

        //md 20060810
        internal void SyncUndidCopySectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            mClipboard = null;
        }

        //md 20060810
        //e.Node is what was just pasted in
        internal void SyncPastedSectionNode(object sender, Events.Node.AddedSectionNodeEventArgs e)
        {
            if (mClipboard == null) return;

            urakawa.core.CoreNode nodeParent = (urakawa.core.CoreNode)e.Node.getParent();

            TreeNode parent = Project.GetNodeType(nodeParent) == NodeType.Section ? FindTreeNodeFromCoreNode(nodeParent) : null;
            string label = Project.GetTextMedia(e.Node).getText();

            TreeNodeCollection siblings = null;
            if (parent == null)
            {
                siblings = mTocTree.Nodes;
            }
            else
            {
                siblings = parent.Nodes;
            }

            siblings.Insert(e.Index, mClipboard);

            //don't clear the clipboard, we can use it again
        }

        internal void SyncUndidPasteSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
            TreeNode pastedNode = FindTreeNodeFromCoreNode(e.Node);

            if (pastedNode != null)
            {
                //put it back on the clipboard
                mClipboard = (TreeNode)pastedNode.Clone();
                pastedNode.Remove();
            }

        }

    }
}
