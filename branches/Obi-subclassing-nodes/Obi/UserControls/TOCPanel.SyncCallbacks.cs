using System.Collections.Generic;
using System.Windows.Forms;
using urakawa.media;

namespace Obi.UserControls
{
    // Callbacks for the TOCPanel
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
        internal void SyncAddedSectionNode(object sender, SectionNode node)
        {
            TreeNode newTreeNode = AddEmptySectionNode(node);
            //start editing if the request to add a node happened in the tree view
            if (sender == this)
            {
                newTreeNode.BeginEdit();
            }
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
            mTocTree.SelectedNode = newTreeNode;
        }

        /// <summary>
        /// Add a new tree node for a new, empty section node.
        /// </summary>
        /// <param name="node">The section node to add. It must not have children.</param>
        /// <param name="index">The index in the parent.</param>
        /// <returns>The created tree node.</returns>
        private TreeNode AddEmptySectionNode(SectionNode node)
        {
            TreeNode newTreeNode;
            if (node.ParentSection != null)
            {
                TreeNode relTreeNode = FindTreeNodeFromSectionNode(node.ParentSection);
                newTreeNode = relTreeNode.Nodes.Insert(node.Index, node.Id.ToString(), node.Label);
            }
            else
            {
                newTreeNode = mTocTree.Nodes.Insert(node.Index, node.Id.ToString(), node.Label);
            }
            newTreeNode.Tag = node;
            return newTreeNode;
        }

        private TreeNode AddSectionNode(SectionNode node)
        {
            TreeNode newTreeNode = AddEmptySectionNode(node);
            for (int i = 0; i < node.SectionChildCount; i++)
            {
                AddSectionNode(node.SectionChild(i));
            }
            return newTreeNode;
        }

        /// <summary>
        /// Remove a node from the tree view.
        /// This will remove the whole subtree.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="node">The root section to remove.</param>
        internal void SyncDeletedSectionNode(object sender, SectionNode node)
        {
            if (node != null)
            {
                TreeNode treeNode = FindTreeNodeFromSectionNode(node);
                mTocTree.SelectedNode = treeNode.PrevVisibleNode;
                if (mTocTree.SelectedNode != null)
                {
                    mTocTree.SelectedNode.EnsureVisible();
                }
                treeNode.Remove();
            }
        }

        internal void SyncMovedSectionNode(object sender, SectionNode node, urakawa.core.CoreNode parentNode)
        {
            TreeNode selected = FindTreeNodeFromSectionNode(node);
            if (selected != null)
            {
                TreeNode parent = parentNode is SectionNode ? FindTreeNodeFromSectionNode((SectionNode)parentNode) : null;
                TreeNode clone = (TreeNode)selected.Clone();
                selected.Remove();
                TreeNodeCollection siblings = parent == null ? mTocTree.Nodes : parent.Nodes;
                siblings.Insert(node.Index, clone);
                clone.ExpandAll();
                clone.EnsureVisible();
                if (mTocTree.SelectedNode != null) mTocTree.SelectedNode = clone;
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

            List<TreeNode> futureChildren = new List<TreeNode>();

           //make copies of our future children, and remove them from the tree
            for (int i = selectedNode.Parent.Nodes.Count - 1; i > selectedNode.Index; i--)
            {
                TreeNode node = selectedNode.Parent.Nodes[i];
                futureChildren.Add((TreeNode)node.Clone());
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
        internal void SyncCutSectionNode(object sender, SectionNode node)
        {
            SyncDeletedSectionNode(sender, node);
        }

        //md 20060810
        //does nothing; just a placeholder
        internal void SyncCopiedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {    
        }

        //md 20060810
        //does nothing; just a placeholder
        internal void SyncUndidCopySectionNode(object sender, SectionNode node)
        {
        }

        internal void SyncPastedSectionNode(object sender, SectionNode node)
        {
           //add a subtree
            TreeNode uncutNode = AddSectionNode(node);
            uncutNode.ExpandAll();
            uncutNode.EnsureVisible();
            mTocTree.SelectedNode = uncutNode;
        }

        internal void SyncUndidPasteSectionNode(object sender, SectionNode node)
        {
            TreeNode pastedNode = FindTreeNodeFromSectionNode(node);
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
