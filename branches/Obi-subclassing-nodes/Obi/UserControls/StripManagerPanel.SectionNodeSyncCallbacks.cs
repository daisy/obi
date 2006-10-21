using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        /// <summary>
        /// Add a new empty strip.
        /// </summary>
        internal void SyncAddedEmptySectionNode(object sender, SectionNode node)
        {
            AddEmptyStripFromSectionNode(node, sender == this);
        }

        /// <summary>
        /// Add a single strip for a section node (which should not have any children.)
        /// The strip is added at the correct position since the node knows its position.
        /// If the rename parameter is true, also start renaming the strip right away.
        /// </summary>
        private void AddEmptyStripFromSectionNode(SectionNode node, bool rename)
        {
            SectionStrip strip = new SectionStrip();
            strip.Label = node.Label;
            strip.Manager = this;
            strip.Node = node;
            mSectionNodeMap[node] = strip;
            mFlowLayoutPanel.Controls.Add(strip);
            mFlowLayoutPanel.Controls.SetChildIndex(strip, node.Position);
            if (rename)
            {
                SelectedSectionNode = node;
                strip.StartRenaming();
            }
        }

        //md 20060811
        //recursive function to add strips for a node and its subtree
        //returns the position marker after the operation is completed
        //todo: this should probably be a visitor
        private void AddStripsFromSectionNode(SectionNode node)
        {
            AddEmptyStripFromSectionNode(node, false);
            for (int i = 0; i < node.getChildCount(); i++)
            {
                if (node.getChild(i).GetType() == typeof(SectionNode))
                {
                    AddStripsFromSectionNode(node.SectionChild(i));
                }
                else
                {
                    //todo: replace this with something cleaner ?  we are kind of falsely invoking an event handler
                    SyncAddedPhraseNode(this, node.PhraseChild(i));
                }
            }
        }

        internal void SyncRenamedSectionNode(object sender, Events.Node.Section.RenameEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            strip.Label = e.NewLabel;
        }

        /// <summary>
        /// When deleting a node from the tree, all descendants are deleted as well.
        /// </summary>
        internal void SyncDeletedNode(object sender, Events.Node.Section.EventArgs e)
        {
            if (e.Node != null)
            {
                Visitors.DescendantsVisitor visitor = new Visitors.DescendantsVisitor();
                e.Node.acceptDepthFirst(visitor);
                foreach (CoreNode node in visitor.Nodes)
                {
                    if (node.GetType() == typeof(SectionNode))
                    {
                        SectionStrip strip = mSectionNodeMap[(SectionNode)node];
                        mFlowLayoutPanel.Controls.Remove(strip);
                    }
                }
                //mg:
                //this.ReflowTabOrder(mSectionNodeMap[e.Node]);
            }
        }

        internal void SyncMovedNode(object sender, Events.Node.MovedNodeEventArgs e)
        {
            //md:
            ArrayList stripsToMove = new ArrayList();
            MakeFlatListOfStrips(e.Node, stripsToMove);

            SectionStrip parentNodeStrip = mSectionNodeMap[(SectionNode)e.Node];
            int currentPosition = mFlowLayoutPanel.Controls.GetChildIndex(parentNodeStrip);

            //if we are moving down
            if (currentPosition < e.Position)
            {
                //reverse the order, because we want to move the last strip first
                //otherwise the operation doesn't work correctly because strips
                //get swapped, and a sequence of moves will not preserve
                //every move that has happened in that sequence

                for (int i = stripsToMove.Count - 1; i >= 0; i--)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], e.Position + i);
                }
            }
            else
            {

                for (int i = 0; i < stripsToMove.Count; i++)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], e.Position + i);
                }
            }

            //mg:
            this.ReflowTabOrder(parentNodeStrip);
        }

        //md 20060811
        internal void SyncCutSectionNode(object sender, Events.Node.Section.EventArgs e)
        {
            SyncDeletedNode(sender, e);
        }

        //md 20060811
        internal void SyncUndidCutSectionNode(object sender, Events.Node.Section.AddedEventArgs e)
        {
            AddStripsFromSectionNode(e.Node);
        }

        //md 20060811
        //does nothing; just a placeholder
        internal void SyncCopiedSectionNode(object sender, Events.Node.NodeEventArgs e)
        {
        }

        //md 20060811
        //does nothing; just a placeholder
        internal void SyncUndidCopySectionNode(object sender, Events.Node.NodeEventArgs e)
        {
        }

        //md 20060811
        internal void SyncPastedSectionNode(object sender, Events.Node.Section.AddedEventArgs e)
        {
            AddStripsFromSectionNode(e.Node);
            
        }

        //md 20060811
        internal void SyncUndidPasteSectionNode(object sender, Events.Node.Section.EventArgs e)
        {
            SyncDeletedNode(sender, e);
        }

        //md: recursive function to enumerate the strips under a node (including the strip for the node itself)
        private void MakeFlatListOfStrips(CoreNode node, ArrayList strips)
        {
            if (node.GetType() == typeof(SectionNode))
            {
                SectionStrip strip = mSectionNodeMap[(SectionNode)node];
                strips.Add(strip);
                for (int i = 0; i < node.getChildCount(); i++)
                {
                    MakeFlatListOfStrips(node.getChild(i), strips);
                }
            }
        }

        //md 20060813
        internal void SyncShallowSwapNodes(object sender, Events.Node.ShallowSwappedSectionNodesEventArgs e)
        {
            SectionStrip strip1 = mSectionNodeMap[(SectionNode)e.Node];
            SectionStrip strip2 = mSectionNodeMap[(SectionNode)e.SwappedNode];

            mFlowLayoutPanel.Controls.SetChildIndex(strip1, e.SwappedNodePosition);
            mFlowLayoutPanel.Controls.SetChildIndex(strip2, e.NodePosition);

            if (e.SwappedNodePosition < e.NodePosition)
                this.ReflowTabOrder(strip1);
            else
                this.ReflowTabOrder(strip2);

        }

    }
}
