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
    public partial class StripsView : UserControl, IControlWithSelection
    {
        private ProjectView mView;     // parent project view
        private Strip mSelectedStrip;  // the current selection

        /// <summary>
        /// A new strips view.
        /// </summary>
        public StripsView(ProjectView view): this()
        {
            mView = view;
            mSelectedStrip = null;
        }

        // Used by the designer
        public StripsView() { InitializeComponent(); }


        /// <summary>
        /// Show the strip for this section node.
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (section != null) mLayoutPanel.ScrollControlIntoView(FindStrip(section));
        }

        /// <summary>
        /// Set a new project for this view.
        /// </summary>
        public void NewProject()
        {
            mView.Project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(StripsView_treeNodeAdded);
            mView.Project.getPresentation().treeNodeRemoved += new TreeNodeRemovedEventHandler(StripsView_treeNodeRemoved);
            mView.Project.RenamedSectionNode += new Obi.Events.RenameSectionNodeHandler(Project_RenamedSectionNode);
        }

        /// <summary>
        /// Set the selected section (null to deselect)
        /// </summary>
        public SectionNode SelectedSection
        {
            get { return mSelectedStrip == null ? null : mSelectedStrip.Node; }
            set { mView.Selection = new NodeSelection(value, this, false); }
        }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return new NodeSelection(mSelectedStrip.Node, this, false); }
            set
            {
                Strip s = value == null ? null : FindStrip(value.Node as SectionNode);
                if (s != mSelectedStrip)
                {
                    if (mSelectedStrip != null) mSelectedStrip.Selected = false;
                    if (s != null)
                    {
                        s.Selected = true;
                        mLayoutPanel.ScrollControlIntoView(s);
                        mView.MakeTreeNodeVisibleForSection(value.Node as SectionNode);
                    }
                    mSelectedStrip = s;
                }
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
        private void Project_RenamedSectionNode(object sender, Obi.Events.Node.RenameSectionNodeEventArgs e)
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

        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) AddStripForSection(section.SectionChild(i));
            Strip strip = new Strip(section, this);
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

        // Remove all strips for a section and its subsections
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
    }
}
