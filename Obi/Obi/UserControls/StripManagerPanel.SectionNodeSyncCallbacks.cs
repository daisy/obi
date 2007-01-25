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
        internal void SyncAddedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            AddStripFromNode(e.Node);
        }

        private void AddStripFromNode(SectionNode node)
        {
            SectionStrip strip = new SectionStrip();
            strip.Label = Project.GetTextMedia(node).getText();
            strip.Manager = this;
            strip.Node = node;
            mSectionNodeMap[node] = strip;
            mFlowLayoutPanel.Controls.Add(strip);
            mFlowLayoutPanel.Controls.SetChildIndex(strip, node.Position);
         }

        //md 20060811
        //recursive function to add strips for a node and its subtree
        //returns the position marker after the operation is completed
        //todo: this should probably be a visitor
        private void AddStripsFromNodeSubtree(SectionNode node)
        {      
            AddStripFromNode(node);
         
            for (int i = 0; i < node.PhraseChildCount; i++)
            {
                //todo: replace this with something cleaner ?  we are kind of falsely invoking an event handler
                SyncAddedPhraseNode(this, new Obi.Events.Node.PhraseNodeEventArgs(this, node.PhraseChild(i)));
            }

            for (int i = 0; i < node.SectionChildCount; i++)
            {
                //then increment based on how many children were added
                AddStripsFromNodeSubtree(node.SectionChild(i));
            }
        }

        internal void SyncRenamedNode(object sender, Events.Node.RenameSectionNodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            strip.Label = e.Label;
        }

        internal void SyncDeletedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            SectionStrip strip = mSectionNodeMap[e.Node];
            mFlowLayoutPanel.Controls.Remove(strip);
            mSectionNodeMap.Remove(e.Node);
        }

        internal void SyncMovedSectionNode(object sender, Events.Node.MovedSectionNodeEventArgs e)
        {
            //md:
            List<SectionStrip> stripsToMove = new List<SectionStrip>();
            stripsToMove = MakeFlatListOfStrips(e.Node);

            SectionStrip parentNodeStrip = mSectionNodeMap[e.Node];
            int currentPosition = mFlowLayoutPanel.Controls.GetChildIndex(parentNodeStrip);

            //if we are moving down
            if (currentPosition < e.Node.Position)
            {
                //reverse the order, because we want to move the last strip first
                //otherwise the operation doesn't work correctly because strips
                //get swapped, and a sequence of moves will not preserve
                //every move that has happened in that sequence

                for (int i = stripsToMove.Count - 1; i >= 0; i--)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], e.Node.Position + i);
                }
            }
            else
            {

                for (int i = 0; i < stripsToMove.Count; i++)
                {
                    mFlowLayoutPanel.Controls.SetChildIndex
                        ((SectionStrip)stripsToMove[i], e.Node.Position + i);
                }
            }

            SetStripsFontSizes(parentNodeStrip);
        }

        //md 20061205
        //recursively set the font size on the given strip and its node's children
        private void SetStripsFontSizes(SectionStrip parentNodeStrip)
        {
            List<SectionStrip> strips = MakeFlatListOfStrips(parentNodeStrip.Node);

            foreach (SectionStrip strip in strips)
            {
                strip.SetStripFontSize();
            }
        }

        //md 20060811
        internal void SyncCutSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            e.Node.visitDepthFirst(
                delegate(ICoreNode n)
                {
                    if (n is SectionNode)
                    {
                        SectionStrip strip = mSectionNodeMap[(SectionNode)n];
                        mFlowLayoutPanel.Controls.Remove(strip);
                        mSectionNodeMap.Remove((SectionNode)n);
                    }
                    return true;
                },
                delegate(ICoreNode n) { }
            );
        }

        //md 20060811
        internal void SyncUndidCutSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            AddStripsFromNodeSubtree(e.Node);
        }

        

        //md 20060811
        internal void SyncPastedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            AddStripsFromNodeSubtree(e.Node);            
        }

        //md 20060811
        internal void SyncUndidPasteSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
            SyncDeletedSectionNode(sender, e);
        }

        //md 20061205
        internal void SyncDecreaseSectionNodeLevel(object sender, Events.Node.SectionNodeEventArgs e)
        {
            //adjust the fontsize when the section changes its level
            //note that the "increase section level" event is handled by MovedSectionNode
            SectionStrip strip = this.mSectionNodeMap[e.Node];
            SetStripsFontSizes(strip);
            
        }
        //md: recursive function to enumerate the strips under a node (including the strip for the node itself)
        private List<SectionStrip> MakeFlatListOfStrips(SectionNode node)
        {
            List<SectionStrip> strips = new List<SectionStrip>();
            SectionStrip strip = mSectionNodeMap[node];
            strips.Add(strip);

            for (int i = 0; i < node.SectionChildCount; i++)
            {
                strips.AddRange(MakeFlatListOfStrips(node.SectionChild(i)));
            }

            return strips;
        }

        internal void ToggledNodeUsedState(object sender, Events.Node.ObiNodeEventArgs e)
        {
            if (e.Node is SectionNode)
            {
                mSectionNodeMap[(SectionNode)e.Node].RefreshUsed();
            }
            else if (e.Node is PhraseNode)
            {
                mPhraseNodeMap[(PhraseNode)e.Node].RefreshUsed();
            }
        }
    }
}
