using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa;

namespace Obi.ProjectView
{
    public partial class TOCView : TreeView, IControlWithRenamableSelection
    {
        private float mBaseFontSize;       // base font size (for scaling)
        private ProjectView mProjectView;  // the parent project view
        private NodeSelection mSelection;  // actual selection context


        /// <summary>
        /// Create an empty TOC view.
        /// </summary>
        public TOCView()
        {
            InitializeComponent();
            mBaseFontSize = Font.SizeInPoints;
            mSelection = null;
            mProjectView = null;
            this.Context_MergeWithNextMenuItem.Click += new System.EventHandler(this.Context_MergeWithNextMenuItem_Click);
            this.Context_MultipleOperationsMenuItem.Click +=new EventHandler(Context_MultipleOperationsMenuItem_Click);
            this.Context_AddEmptyPagesMenuItem.Click += new EventHandler(Context_AddEmptyPagesMenuItem_Click);
            this.Context_AddBlankPhraseMenuItem.Click += new EventHandler(Context_AddBlankPhraseMenuItem_Click);
        }

       



        /// <summary>
        /// Can add a section as long as the selection is not a text selection.
        /// </summary>
        public bool CanAddSection { get { return !(mSelection is TextSelection); } }

        /// <summary>
        /// Can add a subsection if there is a selection (not a text selection though.)
        /// </summary>
        public bool CanAddSubsection { get { return CanAddSection && mSelection != null; } }

        /// <summary>
        /// An actual section must be selected to be copied (i.e. not the text of the section.)
        /// </summary>
        public bool CanCopySection { get { return IsSectionSelected; } }

        /// <summary>
        /// True if there is a selected section and its level can be decreased (it must not be a top-level section.)
        /// </summary>
        public bool CanDecreaseLevel
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionOut.CanMoveNode((SectionNode)mSelection.Node) && !(Selection is TextSelection); }
        }

        /// <summary>
        /// True if there is a selected section and its level can be increased (it must not be the first child.)
        /// </summary>
        public bool CanIncreaseLevel
        {
            get { return IsSectionSelected && Commands.TOC.MoveSectionIn.CanMoveNode((SectionNode)mSelection.Node) && !(Selection is TextSelection); }
        }

        /// <summary>
        /// Can insert a section if there is a selection (not a text selection though.)
        /// </summary>
        public bool CanInsertSection { get { return CanAddSection && mSelection != null; } }

        /// <summary>
        /// True if the contents of the clipboard can be pasted after the selected section.
        /// </summary>
        public bool CanPaste(Clipboard clipboard)
        {
            return mSelection != null && !(mSelection is TextSelection) &&
                clipboard != null && clipboard.Node is SectionNode;
        }

        /// <summary>
        /// True if the contents of the clipboard can be pasted before the selected section.
        /// </summary>
        public bool CanPasteBefore(Clipboard clipboard)
        {
            return mSelection != null && clipboard != null && clipboard.Node is SectionNode;
        }

        /// <summary>
        /// True if the contents of the clipboard can be pasted inside the selected section.
        /// </summary>
        public bool CanPasteInside(Clipboard clipboard)
        {
            return mSelection != null && clipboard != null && clipboard.Node is SectionNode;
        }

        /// <summary>
        /// True if the selected node can be removed (deleted or cut)
        /// </summary>
        public bool CanRemoveSection { get { return IsSectionSelected   &&   !mProjectView.TransportBar.IsRecorderActive; } }

        /// <summary>
        /// True if the selected node can be renamed.
        /// </summary>
        public bool CanRenameSection { get { return IsSectionSelected && !mProjectView.TransportBar.IsRecorderActive; } }

        /// <summary>
        /// True if the used state of the selected section can be changed
        /// (a section is selected and its parent is used.)
        /// </summary>
        public bool CanSetSectionUsedStatus
        {
            get { return IsSectionSelected && mSelection.Node != null && mSelection.Node.Parent != null &&  mSelection.Node.ParentAs<ObiNode>().Used; }
        }

        /// <summary>
        /// Set the color settings from the parent view.
        /// </summary>
        public ColorSettings ColorSettings
        {
            set
            {
                BackColor = value.TOCViewBackColor;
                ForeColor = value.TOCViewForeColor;
                foreach (TreeNode n in Nodes) ChangeColorUsed(n, value);
            }
        }

        /// <summary>
        /// Set the parent project view.
        /// </summary>
        public ProjectView ProjectView { set { mProjectView = value; } }

        /// <summary>
        /// Get or set the current selection.
        /// Only the project view can set the selection.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (mSelection != value)
                {
                    if(mSelection != null && (value == null || value.Node != mSelection.Node)) PaintSelectedNode(false);

                    //if new selection is null and old selection is text selection, finish rename
                    if (value == null 
                        && mSelection != null && mSelection is TextSelection)
                    {
                        TreeNode n1 = FindTreeNode((SectionNode)mSelection.Node);
                        n1.EndEdit(false);
                        if(n1.ForeColor != SystemColors.ControlText )  n1.ForeColor = SystemColors.ControlText;
                    }
                    if (value != null && !this.ContainsFocus) this.Focus();
                    mSelection = value;
                    TreeNode n = value == null ? null : FindTreeNode((SectionNode)mSelection.Node); 
                    // Ignore the select event, since we were asked to change the selection;
                    // but allow the selection not coming from the user.
                    AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                    BeforeSelect -= new TreeViewCancelEventHandler(TOCTree_BeforeSelect);
                    SelectedNode = n;
                    if (n != null) mProjectView.MakeStripVisibleForSection(n.Tag as SectionNode);
                    AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
                    BeforeSelect += new TreeViewCancelEventHandler(TOCTree_BeforeSelect);

                    if (m_HighlightedSectionNodeWithoutSelection != null && (value == null ||  m_HighlightedSectionNodeWithoutSelection != value.Node)) RepaintHighlightNodeWithoutSelection();
                }
            }
        }

    private SectionNode m_HighlightedSectionNodeWithoutSelection = null;
    private SectionNode m_LastHighlightedSectionNodeWithoutSelection = null;
        /// <summary>
        /// highlights the section selected in content view, without moving keyboard focus to toc view.
        /// </summary>
    public SectionNode HighlightNodeWithoutSelection
        {
        get { return m_HighlightedSectionNodeWithoutSelection; }
        set
            {
            //first normalize previously highlighted node
            if (m_HighlightedSectionNodeWithoutSelection != null 
                && mProjectView.Presentation.RootNode != m_HighlightedSectionNodeWithoutSelection.Presentation.RootNode) 
                m_HighlightedSectionNodeWithoutSelection = null;
            if (m_HighlightedSectionNodeWithoutSelection != null && (value == null || m_HighlightedSectionNodeWithoutSelection != value))
            {
                TreeNode treeNodeForRemovingHighlight = FindTreeNodeWithoutLabel(m_HighlightedSectionNodeWithoutSelection);
                if (treeNodeForRemovingHighlight != null)
                {
                    treeNodeForRemovingHighlight.BackColor = System.Drawing.Color.Empty;
                    if (mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection && m_HighlightedSectionNodeWithoutSelection.Duration == 0.0) // @emptysectioncolor
                    {
                        EmptySectionBackColor(m_HighlightedSectionNodeWithoutSelection, treeNodeForRemovingHighlight);
                    }
                }
            }

            if (value != null && (m_HighlightedSectionNodeWithoutSelection == null || m_HighlightedSectionNodeWithoutSelection != value))
            {
                m_HighlightedSectionNodeWithoutSelection = value;
                TreeNode treeNodeToHighlight = FindTreeNodeWithoutLabel(m_HighlightedSectionNodeWithoutSelection);
                if (treeNodeToHighlight != null)
                {
                    treeNodeToHighlight.EnsureVisible();
                }
                //if (treeNodeToHighlight != null) treeNodeToHighlight.BackColor = System.Drawing.SystemColors.Control;
                if (treeNodeToHighlight != null) treeNodeToHighlight.BackColor = mProjectView.ColorSettings.HighlightedSectionNodeWithoutSelectionColor;
                else m_HighlightedSectionNodeWithoutSelection = null;
            }
            else
            {
                m_HighlightedSectionNodeWithoutSelection = value;
            }

            }
        }

        private void RepaintHighlightNodeWithoutSelection ()
        {
            if (mProjectView.Presentation != null &&  m_HighlightedSectionNodeWithoutSelection != null
                && mProjectView.Presentation.RootNode != m_HighlightedSectionNodeWithoutSelection.Presentation.RootNode)
                m_HighlightedSectionNodeWithoutSelection = null;
            if (m_HighlightedSectionNodeWithoutSelection != null )
            {
                                TreeNode treeNodeToHighlight = FindTreeNodeWithoutLabel(m_HighlightedSectionNodeWithoutSelection);
                if (treeNodeToHighlight != null) treeNodeToHighlight.BackColor =  mProjectView.ColorSettings.HighlightedSectionNodeWithoutSelectionColor;
            }
        }

        /// <summary>
        /// Set the zoom factor (normally, from the project view.)
        /// </summary>
        public float ZoomFactor
        {
            set { if (value != 0.0) Font = new Font(Font.FontFamily, mBaseFontSize * value); }
        }


        /// <summary>
        /// Make the tree node for this section visible.
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section) 
        {
            try
            {
                if (section != null && section.IsRooted) FindTreeNode(section).EnsureVisible();
            }
            catch (System.Exception ex)
            {
                mProjectView.WriteToLogFile(ex.ToString());
                MessageBox.Show(Localizer.Message("Operation_Cancelled") + "\n\n" + ex.ToString());
            }
        }

        /// <summary>
        /// Resynchronize strips and TOC views depending on which node is visible.
        /// </summary>
        public void ResyncViews() { foreach (TreeNode n in Nodes) SetStripsVisibilityForNode(n, true); }

        /// <summary>
        /// Select and start renaming a section node.
        /// </summary>
        public void SelectAndRename(ObiNode node)
        {
            SectionNode section = (SectionNode)node;
            DoToNewNode(section, delegate()
            {
                mProjectView.Selection = new TextSelection(section, this, section.Label);
                FindTreeNode(section).BeginEdit();
            });
        }

        /// <summary>
        /// Set a new presentation to be displayed.
        /// </summary>
        public void SetNewPresentation()
        {
            Nodes.Clear();
            CreateTreeNodeForSectionNode((ObiNode) mProjectView.Presentation.RootNode);
            mProjectView.Presentation.Changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(Presentation_changed);
            mProjectView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mProjectView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Stringification of the view.
        /// </summary>
        public override string ToString() { return Localizer.Message("toc_view_to_string"); }

        /// <summary>
        /// Update the context menu after selection has changed.
        /// </summary>
        public void UpdateContextMenu()
        {
            Context_AddSectionMenuItem.Enabled = CanAddSection;
            Context_AddSubsectionMenuItem.Enabled = CanAddSubsection && !mProjectView.TransportBar.IsRecorderActive;
            Context_InsertSectionMenuItem.Enabled = CanInsertSection && !mProjectView.TransportBar.IsRecorderActive;
            Context_RenameSectionMenuItem.Enabled = CanRenameSection;
            Context_DecreaseSectionLevelMenuItem.Enabled = CanDecreaseLevel && !mProjectView.TransportBar.IsRecorderActive;
            Context_IncreaseSectionLevelMenuItem.Enabled = CanIncreaseLevel && !mProjectView.TransportBar.IsRecorderActive;
            Context_SectionIsUsedMenuItem.Enabled = CanSetSectionUsedStatus;
            Context_SectionIsUsedMenuItem.CheckedChanged -= new EventHandler(Context_SectionIsUsedMenuItem_CheckedChanged);
            Context_SectionIsUsedMenuItem.Checked = mProjectView.CanMarkSectionUnused;
            Context_SectionIsUsedMenuItem.CheckedChanged += new EventHandler(Context_SectionIsUsedMenuItem_CheckedChanged);
            Context_CutMenuItem.Enabled = CanRemoveSection;
            Context_CopyMenuItem.Enabled = CanCopySection && !mProjectView.TransportBar.IsRecorderActive;
            Context_PasteMenuItem.Enabled = CanPaste(mProjectView.Clipboard);
            Context_PasteBeforeMenuItem.Enabled = CanPasteBefore(mProjectView.Clipboard);
            Context_PasteInsideMenuItem.Enabled = CanPasteInside(mProjectView.Clipboard);
            Context_DeleteMenuItem.Enabled = mProjectView.CanRemoveSection;
            Context_PropertiesMenuItem.Enabled = mProjectView.CanShowSectionPropertiesDialog;
           // Context_MergeSectionMenuItem.Enabled = mProjectView.CanMergeStripWithNext;
            Context_MergeWithNextMenuItem.Enabled = mProjectView.CanMergeStripWithNext;
            Context_MultipleOperationsMenuItem.Enabled = mProjectView.EnableMultiSectionOperation;
            Context_ShowContentsMenuItem.Enabled = mProjectView.Selection != null && !(mProjectView.Selection is TextSelection);
            Context_AddEmptyPagesMenuItem.Enabled = (mProjectView.CanAddEmptyBlock || (mProjectView.Selection != null && mProjectView.Selection.Node is SectionNode))
                                                          && !mProjectView.TransportBar.IsRecorderActive;
        }


        private delegate TreeNode NodeInvokation(ObiNode node);

        // Convenience method to add a new tree node for a section. Return the added tree node.
        private TreeNode AddSingleSectionNode(ObiNode node)
        {
            if (InvokeRequired)
            {
                return (TreeNode)Invoke(new NodeInvokation(AddSingleSectionNode), node);
            }
            else
            {
                TreeNode n = null;
                if (node is SectionNode && node.IsRooted)
                {
                    if (node.ParentAs<SectionNode>() != null)
                    {
                        TreeNode p = FindTreeNode(node.ParentAs<SectionNode>());
                        n = p.Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                    }
                    else
                    {
                        n = Nodes.Insert(node.Index, node.GetHashCode().ToString(), ((SectionNode)node).Label);
                    }
                    n.Tag = node;
                    ChangeColorUsed(n, mProjectView.ColorSettings);
                }
                return n;
            }
        }

        // Change the color of a node to reflect its used status
        private void ChangeColorUsed(TreeNode n, ColorSettings settings)
        {
            SectionNode section = n.Tag as SectionNode;
            if (section != null)
            {
                n.ForeColor = section.Used ? settings.TOCViewForeColor : settings.TOCViewUnusedColor;
                foreach (TreeNode n_ in n.Nodes) ChangeColorUsed(n_, settings);
            }
        }

        // Create a new tree node for a section node and all of its descendants
        private TreeNode CreateTreeNodeForSectionNode(ObiNode node)
        {
            if (InvokeRequired)
            {
                return (TreeNode)Invoke(new NodeInvokation(CreateTreeNodeForSectionNode), node);
            }
            else
            {
                TreeNode n = AddSingleSectionNode(node);
                if (n != null)
                {
                    n.EnsureVisible();
                    n.ExpandAll();
                    ChangeColorUsed(n, mProjectView.ColorSettings);
                    if (mProjectView.ObiForm.Settings != null && mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection
    && node is SectionNode && node.Duration == 0) // @emptysectioncolor
                    {
                        EmptySectionBackColor(node, n);
                    }
                }
                //if (n != null || node is RootNode)
                if (n != null || node == mProjectView.Presentation.RootNode)//sdk2
                {
                    for (int i = 0; i < node.SectionChildCount; ++i) CreateTreeNodeForSectionNode(node.SectionChild(i));
                }
                return n;
            }
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the tree; if it's not, set an event to look out for
        // its addition. This is used for naming new sections for instance.
        private void DoToNewNode(SectionNode section, DoToNewNodeDelegate f)
        {
            if (IsInTree(section))
            {
                f();
            }
            else
            {
                EventHandler<urakawa.events.DataModelChangedEventArgs> h =
                    delegate(object sender, urakawa.events.DataModelChangedEventArgs e) { };
                h = delegate(object sender, urakawa.events.DataModelChangedEventArgs e)
                {
                    if (e is ObjectAddedEventArgs<urakawa.core.TreeNode> &&
                        ((ObjectAddedEventArgs<urakawa.core.TreeNode>)e).m_AddedObject == section)
                    {
                        f();
                        mProjectView.Presentation.Changed -= h;
                    }
                };
                mProjectView.Presentation.Changed += h;
            }
        }

        // Find the tree node for a section node. The labels must also match.
        private TreeNode FindTreeNode(SectionNode section)
        {
            TreeNode n = FindTreeNodeWithoutLabel(section);
            if (n == null) throw new Exception(String.Format("Could not find tree node for section with label \"{0}\"",
                section.Label));
            if (n.Text != section.Label)
            {
                throw new Exception(
                    String.Format("Found tree node matching section node #{0} but labels mismatch (wanted \"{1}\" but got \"{2}\").",
                    section.GetHashCode(), section.Label, n.Text));
            }
            return n;
        }

        // Find a tree node for a section node, regardless of its label.
        // Return null when no node is found (do not throw an exception.)
        private TreeNode FindTreeNodeWithoutLabel(SectionNode section)
        {
            TreeNode[] nodes = Nodes.Find(section.GetHashCode().ToString(), true);
            foreach (TreeNode n in nodes) if (n.Tag == section) return n;
            return null;
        }

        // Check whether a node is in the tree view.
        private bool IsInTree(SectionNode section)
        {
            if (section != null)
            {
                TreeNode[] nodes = Nodes.Find(section.GetHashCode().ToString(), true);
                foreach (TreeNode n in nodes) if (n.Tag == section && n.Text == section.Label) return true;
            }
            return false;
        }

        // True if a section (not its text) is selected.
        private bool IsSectionSelected { get { return mSelection != null && mSelection is NodeSelection; } }

        // Tabbing inside the view
        protected override bool ProcessDialogKey(Keys KeyData)
        {
            if (this.ContainsFocus && (KeyData == Keys.Tab || KeyData == ((Keys)Keys.Shift | Keys.Tab)))
            {
                System.Media.SystemSounds.Beep.Play();
                return true;
            }
            return base.ProcessDialogKey(KeyData);
        }

        // Set the strips visibility for the given tree node according to expandednessity
        private void SetStripsVisibilityForNode(TreeNode node, bool visible)
        {
            mProjectView.SetStripVisibilityForSection((SectionNode)node.Tag, visible);
            foreach (TreeNode n in node.Nodes) SetStripsVisibilityForNode(n, visible && node.IsExpanded);
        }

        // Add new section nodes to the tree
        private void TreeNodeAdded(ObjectAddedEventArgs<urakawa.core.TreeNode> e)
        {
            if (e.m_AddedObject is SectionNode)
            {
                // ignore the selection of the new tree node
                AfterSelect -= new TreeViewEventHandler(TOCTree_AfterSelect);
                CreateTreeNodeForSectionNode((SectionNode)e.m_AddedObject);
                AfterSelect += new TreeViewEventHandler(TOCTree_AfterSelect);
            }
        }

        // Remove deleted section nodes from the tree
        void TreeNodeRemoved(ObjectRemovedEventArgs<urakawa.core.TreeNode> e)
        {
            if (e.m_RemovedObject is SectionNode) Nodes.Remove(FindTreeNode((SectionNode)e.m_RemovedObject));
        }


        // Add section context menu item
        private void Context_AddSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.AddSection(); }

        // Add subsection context menu item
        private void Context_AddSubsectionMenuItem_Click(object sender, EventArgs e) { mProjectView.AddSubSection(); }

        // Rename section menu item
        private void Context_RenameSectionMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(Selection != null);
            System.Diagnostics.Debug.Assert(Selection.Node is SectionNode);
            SelectAndRename(Selection.Node as SectionNode);
        }

        // Decrease section level context menu item
        private void Context_DecreaseSectionLevelMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.DecreaseSelectedSectionLevel();
        }

        // Increase section level context menu item
        private void Context_IncreaseSectionLevelMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.IncreaseSelectedSectionLevel();
        }

        // Section is used context menu item
        private void Context_SectionIsUsedMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mProjectView.SetSelectedNodeUsedStatus(Context_SectionIsUsedMenuItem.Checked);
        }

        // Cut context menu item
        private void Context_CutMenuItem_Click(object sender, EventArgs e) { mProjectView.Cut(); }

        // Copy context menu item
        private void Context_CopyMenuItem_Click(object sender, EventArgs e) { mProjectView.Copy(); }

        // Paste context menu item
        private void Context_PasteMenuItem_Click(object sender, EventArgs e) { mProjectView.Paste(); }

        // Paste before context menu item
        private void Context_PasteBeforeMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteBefore(); }

        // Paste inside context menu item
        private void Context_PasteInsideMenuItem_Click(object sender, EventArgs e) { mProjectView.PasteInside(); }

        // Delete context menu item
        private void Context_DeleteMenuItem_Click(object sender, EventArgs e) { mProjectView.Delete(); }

        // Insert section context menu item
        private void Context_InsertSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.InsertSection(); }

        // Properties context menu item
        private void Context_PropertiesMenuItem_Click(object sender, EventArgs e) { mProjectView.ShowSectionPropertiesDialog(); }

        private void Context_MergeSectionMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }

        private void Context_MergeWithNextMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }

        private void Context_MultipleOperationsMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeMultipleSections(); }


        private void Context_AddEmptyPagesMenuItem_Click(object sender, EventArgs e) 
        {
            mProjectView.AddEmptyPages();
        }

        private void Context_AddBlankPhraseMenuItem_Click(object sender, EventArgs e)
        {
            mProjectView.AddEmptyBlock();
        }
        
        

        private void Context_MergeSectionWithNextMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeStrips(); }

        private void Context_MergeMultipleSectionsMenuItem_Click(object sender, EventArgs e) { mProjectView.MergeMultipleSections(); }

        // Reflect changes in the presentation (added or deleted nodes)
        private void Presentation_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            urakawa.core.TreeNode changedNode = null;
            if (e is ObjectAddedEventArgs<urakawa.core.TreeNode>)
            {
                TreeNodeAdded((ObjectAddedEventArgs<urakawa.core.TreeNode>)e);
                changedNode = ((ObjectAddedEventArgs<urakawa.core.TreeNode>)e).m_AddedObject;
            }
            else if (e is ObjectRemovedEventArgs<urakawa.core.TreeNode>)
            {
                TreeNodeRemoved((ObjectRemovedEventArgs<urakawa.core.TreeNode>)e);
                changedNode = ((ObjectRemovedEventArgs<urakawa.core.TreeNode>)e).m_RemovedObject;
            }
            else if (mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection && e is urakawa.events.media.data.audio.AudioDataInsertedEventArgs)
            {
                TreeNode TreeNode = FindTreeNodeWithoutLabel((SectionNode)m_HighlightedSectionNodeWithoutSelection);
                if (TreeNode.BackColor == mProjectView.ObiForm.Settings.ColorSettings.EmptySectionBackgroundColor)
                {
                    TreeNode.BackColor = mProjectView.ColorSettings.HighlightedSectionNodeWithoutSelectionColor;
                    TreeNode.ForeColor = SystemColors.ControlText;
                }
            }
            if (changedNode != null && (changedNode is SectionNode || changedNode is EmptyNode) && ((ObiNode)changedNode).IsRooted) // @emptysectioncolor
            {
                PaintColorForEmptySection(changedNode is SectionNode ? (SectionNode)changedNode : ((EmptyNode)changedNode).ParentAs<SectionNode>(), false);
            }
        }

        // When a node was renamed, show the new name in the tree.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            TreeNode n = FindTreeNodeWithoutLabel(e.Node);
            n.Text = e.Node.Label;
        }

        // Node used status changed
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode && IsInTree((SectionNode)e.Node))
            {
                ChangeColorUsed(FindTreeNode((SectionNode)e.Node), mProjectView.ColorSettings);
            }
        }

        // When a node is collapsed, hide strips corresponding to the collapsed nodes.
        private void TOCTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // When a node is expanded, make the strips reappear
        private void TOCTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            SetStripsVisibilityForNode(e.Node, true);
        }

        // Rename the section after the text of the tree node has changed.
        // Cancel if the text is empty.
        private void TOCTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag != null && e.Label != null && e.Label.Trim ()  != "")
            {
                mProjectView.RenameSectionNode((SectionNode)e.Node.Tag, e.Label);
            }
            else
            {
                e.CancelEdit = true;
                mProjectView.Selection = new NodeSelection((SectionNode)e.Node.Tag, this);
            }
        }

        // Pass a new selection to the main view.
        // Do not act on reselection of the same item to avoid infinite loops.
        private void TOCTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            NodeSelection s = new NodeSelection((SectionNode)e.Node.Tag, this);
            if (s != mProjectView.Selection) mProjectView.Selection = s;
        }

        // Make a text selection in the view.
        private void TOCTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!(e.Node.Tag is SectionNode))
            {
                e.CancelEdit = true;
            }
            else if (mSelection != null)
            {
                mProjectView.Selection = new TextSelection((SectionNode)e.Node.Tag, this, e.Node.Text);
            }
        }

        // Filter out unwanted tree selections (not caused by the user clicking, expanding, collapsing or keyboarding.)
        private void TOCTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown) e.Cancel = true;
        }
        private void Context_ShowContentsMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectView.ShowSelectedSectionContents())
            {
                HighlightNodeWithoutSelection = mProjectView.GetSelectedPhraseSection;
            }
        }

        private void TOCView_DoubleClick(object sender, EventArgs e)
        {
            if (mProjectView.Selection != null)
            {
                if (mProjectView.ShowSelectedSectionContents())
                {
                    HighlightNodeWithoutSelection = mProjectView.GetSelectedPhraseSection;
                }

            }   
        }

        private void TOCView_Leave(object sender, EventArgs e)
        {
            PaintSelectedNode(true);
        }

        private void PaintSelectedNode(bool isSelected)
        {
            if (mProjectView.Presentation != null   &&    mSelection != null && mSelection.Node != null
                && mProjectView.Presentation.RootNode != mSelection.Node.Presentation.RootNode)
                mSelection = null;

            if (mSelection != null && !(mSelection is TextSelection))
            {
                TreeNode treeNodeToSelect = FindTreeNodeWithoutLabel((SectionNode)mSelection.Node);
                if (treeNodeToSelect != null)
                {
                    if (isSelected)
                    {
                        treeNodeToSelect.BackColor = SystemColors.Highlight;
                        treeNodeToSelect.ForeColor = SystemColors.HighlightText;
                    }
                    else
                    {
                        treeNodeToSelect.BackColor = Color.Empty;
                        treeNodeToSelect.ForeColor = SystemColors.ControlText;
                        if (mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection) // @emptysectioncolor
                        {
                            EmptySectionBackColor(mSelection.Node, treeNodeToSelect);
                        }

                    }

 
                }
                
            }
        }

        public void EmptySectionBackColor(ObiNode node, TreeNode treeNodeToSelect) // @emptysectioncolor
        {
            if (node == null || treeNodeToSelect == null) return;
            if (mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection && node.Duration == 0.0)
            {
                {
                    if (!SystemInformation.HighContrast)
                    {
                        treeNodeToSelect.BackColor = mProjectView.ObiForm.Settings.ColorSettings.EmptySectionBackgroundColor;
                    }
                    else
                    {
                        treeNodeToSelect.BackColor = mProjectView.ObiForm.Settings.ColorSettingsHC.EmptySectionBackgroundColor;
                    }
                    treeNodeToSelect.ForeColor = SystemColors.ControlText;
                }
            }
        }


        public void UpdateTOCBackColorForEmptySection(SectionNode node) // @emptysectioncolor
        {
            PaintColorForEmptySection(node, true);


            if (node.FollowingSection != null)
            {
                if (node.FollowingSection is SectionNode)
                {
                    UpdateTOCBackColorForEmptySection((SectionNode)node.FollowingSection);
                }
            }

        }

        private void PaintColorForEmptySection(SectionNode node, bool isIterating) // @emptysectioncolor
        {
            if (!mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection && !isIterating) return;
            if (mProjectView.ObiForm.Settings.Project_BackgroundColorForEmptySection && node.Duration == 0.0)
            {
                TreeNode treeNode = FindTreeNodeWithoutLabel((SectionNode)node);
                if (!SystemInformation.HighContrast)
                {
                    treeNode.BackColor = mProjectView.ObiForm.Settings.ColorSettings.EmptySectionBackgroundColor;
                }
                else
                {
                    treeNode.BackColor = mProjectView.ObiForm.Settings.ColorSettingsHC.EmptySectionBackgroundColor; ;
                }
                treeNode.ForeColor = SystemColors.ControlText;
                System.Media.SystemSounds.Asterisk.Play();
            }
            else if(m_HighlightedSectionNodeWithoutSelection!=(SectionNode)node)
            {
                TreeNode treeNode = FindTreeNodeWithoutLabel((SectionNode)node);

                treeNode.BackColor = Color.Empty;
                treeNode.ForeColor = SystemColors.ControlText;

            }
        }
    }
}
