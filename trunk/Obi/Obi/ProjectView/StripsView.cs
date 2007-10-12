using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    public partial class StripsView : UserControl
    {
        private Project mProject;  // project for this view

        /// <summary>
        /// A new strips view that is unconnected to any project yet.
        /// </summary>
        public StripsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Connect the strips view to the project.
        /// TODO we may need to get rid of old events if we change projects.
        /// </summary>
        public Project Project
        {
            set
            {
                mProject = value;
                mProject.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(StripsView_treeNodeAdded);
                mProject.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(StripsView_treeNodeRemoved);
                mProject.RenamedSectionNode += new Obi.Events.RenameSectionNodeHandler(mProject_RenamedSectionNode);
            }
        }


        #region Event handlers

        // Handle resizing of the layout panel: all strips are resized to be at least as wide.
        private void mLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                c.MinimumSize = new Size(mLayoutPanel.Width, c.MinimumSize.Height);
            }
        }

        // Handle section nodes renamed from the project: change the label of the corresponding strip.
        private void mProject_RenamedSectionNode(object sender, Obi.Events.Node.RenameSectionNodeEventArgs e)
        {
            Strip strip = FindStrip(e.Node);
            strip.Label = e.Label;
        }

        // Handle addition of tree nodes: add a new strip for new section nodes.
        private void StripsView_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                SectionNode section = (SectionNode)e.getTreeNode();
                if (section.IsRooted)
                {
                    Strip strip = AddStripForSection(section);
                    mLayoutPanel.ScrollControlIntoView(strip);
                }
            }
        }

        private Strip AddStripForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) AddStripForSection(section.SectionChild(i));
            Strip strip = new Strip(section);
            strip.LabelEditedByUser += new EventHandler(delegate(object sender, EventArgs _e)
            {
                mProject.RenameSectionNode(section, strip.Label);
            });
            mLayoutPanel.Controls.Add(strip);
            mLayoutPanel.Controls.SetChildIndex(strip, section.Position);
            strip.MinimumSize = new Size(mLayoutPanel.Width, strip.MinimumSize.Height);
            return strip;
        }

        // Handle removal of tree nodes: remove a strip for a section node and all of its children.
        void StripsView_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                RemoveStripForSection((SectionNode)e.getTreeNode());
            }
        }

        private void RemoveStripForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripForSection(section.SectionChild(i));
            Strip strip = FindStrip(section);
            mLayoutPanel.Controls.Remove(strip);
        }

        #endregion


        #region Utility functions

        /// <summary>
        /// Find the strip for the given section node.
        /// The strip must be present so an exception is thrown on failure.
        /// </summary>
        private Strip FindStrip(SectionNode section)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                if (c is Strip && ((Strip)c).Node == section) return c as Strip;
            }
            throw new Exception(String.Format("Could not find strip for section node labeled `{0}'", section.Label));
        }

        #endregion

        /// <summary>
        /// Show the strip for this section node.
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (section != null) mLayoutPanel.ScrollControlIntoView(FindStrip(section));
        }
    }
}
