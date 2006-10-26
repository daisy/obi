using System.Collections.Generic;
using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    // Strip manager panel methods relating to callbacks for section strips
    public partial class StripManagerPanel
    {
        /// <summary>
        /// Add a new empty strip.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="node">The section node for which to add a strip.</param>
        internal void SyncAddedEmptySectionNode(object sender, SectionNode node)
        {
            AddEmptyStripFromSectionNode(node, sender == this);
        }

        /// <summary>
        /// Add a single strip for a section node (which should not have any children.)
        /// </summary>
        /// <param name="node">The section node for which to add a strip.</param>
        /// <param name="rename">If true, start renaming the strip.</param>
        private void AddEmptyStripFromSectionNode(SectionNode node, bool rename)
        {
            SectionStrip strip = new SectionStrip();
            strip.Label = node.Label;
            strip.Manager = this;
            strip.Node = node;
            mSectionNodeMap[node] = strip;
            mFlowLayoutPanel.Controls.Add(strip);
            mFlowLayoutPanel.Controls.SetChildIndex(strip, node.Position);
            System.Diagnostics.Debug.Print("+++ adding empty strip at position {0}.", node.Position);
            if (rename)
            {
                SelectedSectionNode = node;
                strip.StartRenaming();
            }
        }

        /// <summary>
        /// Add strips for a section node and all of its descendants.
        /// </summary>
        /// <param name="node">The section node (may have children) to add from.</param>
        private void AddStripsFromSectionNode(SectionNode node)
        {
            node.visitDepthFirst
            (
                delegate(ICoreNode n)
                {
                    if (n is SectionNode)
                    {
                        AddEmptyStripFromSectionNode((SectionNode)n, false);
                    }
                    else if (n is PhraseNode)
                    {
                        //todo: replace this with something cleaner ?  we are kind of falsely invoking an event handler
                        SyncAddedPhraseNode(this, (PhraseNode)n);
                    }
                    return true;
                },
                delegate(ICoreNode n) {}
            );
        }

        internal void SyncRenamedSectionNode(object sender, Events.Node.Section.RenameEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            strip.Label = e.NewLabel;
        }

        /// <summary>
        /// When deleting a node from the tree, all descendants are deleted as well.
        /// </summary>
        internal void SyncDeletedNode(object sender, SectionNode node)
        {
            if (node != null)
            {
                node.visitDepthFirst
                (
                    delegate(ICoreNode n)
                    {
                        if (n is SectionNode) mFlowLayoutPanel.Controls.Remove(mSectionNodeMap[(SectionNode)n]);
                        return true;
                    },
                    delegate(ICoreNode n) {}
                );
            }
        }

        internal void SyncMovedNode(object sender, SectionNode node, CoreNode parent)
        {
            List<SectionStrip> stripsToMove = new List<SectionStrip>();
            MakeFlatListOfStrips(node, stripsToMove);
            SectionStrip parentNodeStrip = mSectionNodeMap[(SectionNode)node];
            int currentPosition = mFlowLayoutPanel.Controls.GetChildIndex(parentNodeStrip);
            //if we are moving down
            if (currentPosition < node.Position)
            {
                //reverse the order, because we want to move the last strip first
                //otherwise the operation doesn't work correctly because strips
                //get swapped, and a sequence of moves will not preserve
                //every move that has happened in that sequence

                for (int i = stripsToMove.Count - 1; i >= 0; i--)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], node.Position + i);
                }
            }
            else
            {

                for (int i = 0; i < stripsToMove.Count; i++)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], node.Position + i);
                }
            }

            //mg:
            this.ReflowTabOrder(parentNodeStrip);
        }

        //md 20060811
        internal void SyncCutSectionNode(object sender, SectionNode node)
        {
            SyncDeletedNode(sender, node);
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
        internal void SyncUndidCopySectionNode(object sender, SectionNode node)
        {
        }

        internal void SyncPastedSectionNode(object sender, SectionNode node)
        {
            AddStripsFromSectionNode(node);
            
        }

        //md 20060811
        internal void SyncUndidPasteSectionNode(object sender, SectionNode node)
        {
            SyncDeletedNode(sender, node);
        }

        //md: recursive function to enumerate the strips under a node (including the strip for the node itself)
        private void MakeFlatListOfStrips(CoreNode node, List<SectionStrip> strips)
        {
            if (node is SectionNode)
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
