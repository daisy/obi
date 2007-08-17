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
using urakawa.core.events;

namespace Obi.UserControls
{
    public partial class StripManagerPanel
    {
        /// <summary>
        /// A section was added to the tree, so a strip is added to the strip view.
        /// </summary>
        public void SyncAddedSectionNode(object sender, Events.Node.SectionNodeEventArgs e)
        {
                        AddStripFromNode(e.Node);
            if (sender == this)
            {
                mProjectPanel.CurrentSelection = new NodeSelection(e.Node, this);
                mSectionNodeMap[e.Node].Renaming = true;
            }
        }

internal          void AddStripFromNode(SectionNode node)
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
        private void AddStripsFromNodeSubtree(SectionNode node)
        {      
            AddStripFromNode(node);
            for (int i = 0; i < node.PhraseChildCount; i++)
            {
                SyncAddedPhraseNode(node.PhraseChild(i));
            }
            for (int i = 0; i < node.SectionChildCount; i++)
            {
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
            if (mProjectPanel.CurrentSelectedStrip == e.Node) mProjectPanel.CurrentSelection = null;
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
            e.Node.acceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is SectionNode)
                    {
                        SectionStrip strip = mSectionNodeMap[(SectionNode)n];
                        mFlowLayoutPanel.Controls.Remove(strip);
                        mSectionNodeMap.Remove((SectionNode)n);
                    }
                    return true;
                },
                delegate(urakawa.core.TreeNode n) { }
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
                mPhraseNodeMap[(PhraseNode)e.Node].RefreshDisplay();
            }
        }


        /// <summary>
        /// Select the previous section of the selected section in the strip manager.
        /// If a phrase is selected, select its parent strip.
        /// If nothing is selected, select the last strip in the project.
        /// </summary>
        public void PreviousSection()
        {
            SectionNode prev = null;
            if (mProjectPanel.Project != null)
            {
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    prev = mProjectPanel.CurrentSelectedAudioBlock.ParentSection;
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    prev = mProjectPanel.CurrentSelectedStrip.PreviousSection;
                }
                else
                {
                    prev = mProjectPanel.Project.LastSection;
                }
            }
            if (prev != null) mProjectPanel.CurrentSelection = new NodeSelection(prev, this);
        }

        /// <summary>
        /// Select the next section for the selected section in the strip manager.
        /// If a phrase is selected, select the next stripfor the parent strip.
        /// If nothing is selected, select the first strip of the project.
        /// </summary>
        public void NextSection()
        {
            SectionNode next = null;
            if (mProjectPanel.Project != null)
            {
                if (mProjectPanel.CurrentSelectedAudioBlock != null)
                {
                    next = mProjectPanel.CurrentSelectedAudioBlock.ParentSection.NextSection;
                }
                else if (mProjectPanel.CurrentSelectedStrip != null)
                {
                    next = mProjectPanel.CurrentSelectedStrip.NextSection;
                }
                else
                {
                    next = mProjectPanel.Project.FirstSection;
                }
            }
            if (next != null) mProjectPanel.CurrentSelection = new NodeSelection(next, this);
        }
    }
}
