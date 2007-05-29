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
        /// This is to respond to external renames.
        /// </summary>
        internal void SyncRenamedSectionNode(object sender, Events.Node.RenameSectionNodeEventArgs e)
        {
            TreeNode treeNode = FindTreeNodeWithoutLabel(e.Node);
            treeNode.Text = e.Label;
        }

        /// <summary>
        /// Add a section to the tree view. If we were the ones to request its addition, 
        /// also start editing its label right now.
        /// </summary>
        public void SyncAddedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            TreeNode newTreeNode = AddSingleSectionNode(e.Node);
            newTreeNode.ExpandAll();
            newTreeNode.EnsureVisible();
        }

        private TreeNode AddSingleSectionNode(SectionNode node)
        {
            TreeNode newTreeNode;
            string label = Project.GetTextMedia(node).getText();

            //            if (node.getParent().getParent() != null)
            if (node.getParent() != null)
            {
                if (node.getParent() is SectionNode)
                {
                    TreeNode relTreeNode = FindTreeNodeFromSectionNode((SectionNode)node.getParent());
                    newTreeNode = relTreeNode.Nodes.Insert(node.Index, node.GetHashCode().ToString(), label);
                }
                else //must be the root that is parent
                {
                    newTreeNode = mTocTree.Nodes.Insert(node.Index, node.GetHashCode().ToString(), label);
                }
            }
            else
            {
                newTreeNode = mTocTree.Nodes.Insert(node.Index, node.GetHashCode().ToString(), label);
            }
            newTreeNode.Tag = node;

            return newTreeNode;
        }

        private TreeNode AddSectionNode(SectionNode node)
        {
            TreeNode addedNode = AddSingleSectionNode(node);

            for (int i = 0; i < node.SectionChildCount; i++)
            {
                AddSectionNode(node.SectionChild(i));
            }

            return addedNode;
        }

        /// <summary>
        /// Remove a node from the tree view.
        /// This will remove the whole subtree.
        /// </summary>
        /// <param name="sender">The sender of this event notification</param>
        /// <param name="e"><see cref="e.Node"/> is the node to be removed.</param>
        internal void SyncDeletedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            SyncRemovedSectionNode(e.Node);
        }

        private void SyncRemovedSectionNode(SectionNode node)
        {
            TreeNode treeNode = FindTreeNodeFromSectionNode(node);

            // Avn:  if the root node of tree is to be deleted, it should be deselected to avoid not found exceptions 
            // important while using undoAdd Section  because current selection may not be on node to be deleted and this condition was not handled in ProjectPanel
            if (mTocTree.SelectedNode != null && mTocTree.SelectedNode.Nodes.Count == 0)
                mProjectPanel.CurrentSelection = null;
                
            mTocTree.SelectedNode = treeNode.PrevVisibleNode;
            if (mTocTree.SelectedNode != null) mTocTree.SelectedNode.EnsureVisible();
            treeNode.Remove();
        }


        internal void SyncMovedSectionNode(object sender, Events.Node.MovedSectionNodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromSectionNode(e.Node);
            TreeNode parent = null;
            if (e.Parent.GetType() == Type.GetType("Obi.SectionNode"))
            {
                parent = FindTreeNodeFromSectionNode((SectionNode)e.Parent);
            }
        
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

            siblings.Insert(e.Node.Index, clone);
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
        internal void SyncDecreasedSectionNodeLevel(object sender, Events.Node.SectionNodeEventArgs e)
        {
            TreeNode selected = FindTreeNodeFromSectionNode(e.Node);
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

        internal void SyncCutSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            SyncRemovedSectionNode(e.Node);
        }

        //md 20060810
        //e.Node is what was just pasted in
        internal void SyncPastedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
           //add a subtree
            TreeNode uncutNode = AddSectionNode(e.Node);
            uncutNode.ExpandAll();
            uncutNode.EnsureVisible();
            mTocTree.SelectedNode = uncutNode;
        }

        internal void SyncUndidPasteSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
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

        internal void ToggledNodeUsedState(object sender, Events.Node.ObiNodeEventArgs e)
        {
            if (e.Node is SectionNode)
            {
                TreeNode treeNode = FindTreeNodeFromSectionNode((SectionNode)e.Node);
                treeNode.ForeColor = e.Node.Used ? Colors.SectionUsed : Colors.SectionUnused;
            }
        }
    }
}
