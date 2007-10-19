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
    /// <summary>
    /// Common interface for selection of strips and blocks
    /// </summary>
    public interface ISelectableInStripView
    {
        bool Selected { get; set; }
        ObiNode ObiNode { get; }
    }

    public partial class StripsView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;                     // parent project view
        private ISelectableInStripView mSelectedItem;  // selected strip or block

        /// <summary>
        /// A new strips view.
        /// </summary>
        public StripsView(ProjectView view)
            : this()
        {
            mView = view;
            mSelectedItem = null;
        }

        // Used by the designer
        public StripsView() { InitializeComponent(); }


        public bool CanAddStrip { get { return mSelectedItem is Strip; } }
        public bool CanRenameStrip { get { return mSelectedItem is Strip; } }

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
        /// Rename a strip.
        /// </summary>
        public void RenameStrip(Strip strip)
        {
            mView.RenameSectionNode(strip.Node, strip.Label);
        }

        public PhraseNode SelectedPhrase
        {
            get { return mSelectedItem != null && mSelectedItem is Block ? ((Block)mSelectedItem).Node : null; }
            set { mView.Selection = new NodeSelection(value, this, false); }
        }

        /// <summary>
        /// Set the selected section (null to deselect)
        /// </summary>
        public SectionNode SelectedSection
        {
            get { return mSelectedItem != null && mSelectedItem is Strip ? ((Strip)mSelectedItem).Node : null; }
            set { mView.Selection = new NodeSelection(value, this, false); }
        }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelectedItem == null ? null : new NodeSelection(mSelectedItem.ObiNode, this, false); }
            set
            {
                ISelectableInStripView s = value == null ? null : FindSelectable(value.Node);
                if (s != mSelectedItem)
                {
                    if (mSelectedItem != null) mSelectedItem.Selected = false;
                    if (s != null)
                    {
                        s.Selected = true;
                        mLayoutPanel.ScrollControlIntoView((Control)s);
                        SectionNode section = value.Node is SectionNode ? (SectionNode)value.Node :
                            value.Node is PhraseNode ? ((PhraseNode)value.Node).ParentSection : null;
                        mView.MakeTreeNodeVisibleForSection(section);
                    }
                    mSelectedItem = s;
                }
            }
        }

        /// <summary>
        /// Show/hide strips under the one for which the section was collapsed or expanded.
        /// </summary>
        public void SetStripsVisibilityForSection(SectionNode section, bool visible)
        {
            for (int i = 0; i < section.SectionChildCount; ++i)
            {
                Strip s;
                SectionNode child = section.SectionChild(i);
                if ((s = FindStrip(child)) != null)
                {
                    s.Visible = visible;
                    if (mSelectedItem == s && !visible) mView.Selection = null;
                    SetStripsVisibilityForSection(section.SectionChild(i), visible);
                }
            }
        }

        /// <summary>
        /// Views are not synchronized anymore, so make sure that all strips are visible.
        /// </summary>
        public void UnsyncViews()
        {
            foreach (Control c in mLayoutPanel.Controls) c.Visible = true;
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
            else if (e.getTreeNode() is PhraseNode)
            {
                PhraseNode phrase = (PhraseNode)e.getTreeNode();
                Block block = FindStrip(phrase.ParentSection).AddBlockForPhrase(phrase);
                mLayoutPanel.ScrollControlIntoView(block);
            }
        }

        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection(SectionNode section)
        {
            Strip strip = new Strip(section, this);
            mLayoutPanel.Controls.Add(strip);
            mLayoutPanel.Controls.SetChildIndex(strip, section.Position);
            strip.MinimumSize = new Size(mLayoutPanel.Width, strip.MinimumSize.Height);
            for (int i = 0; i < section.SectionChildCount; ++i) AddStripForSection(section.SectionChild(i));
            return strip;
        }

        // Handle removal of tree nodes: remove a strip for a section node and all of its children.
        void StripsView_treeNodeRemoved(ITreeNodeChangedEventManager o, TreeNodeRemovedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                RemoveStripForSection((SectionNode)e.getTreeNode());
            }
            else if (e.getTreeNode() is PhraseNode)
            {
                FindStrip((SectionNode)e.getFormerParent()).RemoveBlock((PhraseNode)e.getTreeNode());
            }
        }

        // Remove all strips for a section and its subsections
        private void RemoveStripForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripForSection(section.SectionChild(i));
            Strip strip = FindStrip(section);
            mLayoutPanel.Controls.Remove(strip);
        }

        // Deselect everything when clicking the 
        private void mLayoutPanel_Click(object sender, EventArgs e)
        {
            mView.Selection = null;
        }

        #endregion


        #region Utility functions

        private Block FindBlock(PhraseNode node)
        {
            return FindStrip(node.ParentSection).FindBlock(node);
        }

        private ISelectableInStripView FindSelectable(ObiNode node)
        {
            return node is SectionNode ? (ISelectableInStripView)FindStrip((SectionNode)node) :
                node is PhraseNode ? (ISelectableInStripView)FindBlock((PhraseNode)node) : null;
        }

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
            //throw new Exception(String.Format("Could not find strip for section node labeled `{0}'", section.Label));
            return null;
        }

        #endregion

        #region IControlWithRenamableSelection Members

        public void SelectAndRename(ObiNode node)
        {
            DoToNewNode(node, delegate()
            {
                mView.Selection = new NodeSelection(node, this, false);
                FindStrip((SectionNode)node).StartRenaming();
            });
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the view.
        private void DoToNewNode(ObiNode node, DoToNewNodeDelegate f)
        {
            if (IsInView(node))
            {
                f();
            }
            else
            {
                TreeNodeAddedEventHandler h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e) { };
                h = delegate(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == node)
                    {
                        f();
                        mView.Project.getPresentation().treeNodeAdded -= h;
                    }
                };
                mView.Project.getPresentation().treeNodeAdded += h;
            }
        }

        private bool IsInView(ObiNode node)
        {
            return node is SectionNode && FindStrip((SectionNode)node) != null;
        }

        #endregion




        // temporary for search
        public FlowLayoutPanel LayoutPanel { get { return mLayoutPanel; } }

        /// <summary>
        /// Get all the searchable items (i.e. strips; later blocks) in the control
        /// </summary>
        public List<ISearchable> Searchables
        {
            get
            {
                List<ISearchable> l = new List<ISearchable>(mLayoutPanel.Controls.Count);
                foreach (Control c in mLayoutPanel.Controls) if (c is ISearchable) l.Add((ISearchable)c);
                return l;
            }
        }

        /// <summary>
        /// Get some information about the selected strip
        /// </summary>
        public void AboutSelectedStrip()
        {
            //Console.Out.WriteLine("[Strip at level {0}, position {1}, with label `{2}' (`{3}')]",
            //    mSelectedStrip.Node.Level, mSelectedStrip.Node.Position, mSelectedStrip.Label, mSelectedStrip.Node.Label);
        }
    }
}