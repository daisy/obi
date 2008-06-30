using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInStripView
    {
        private SectionNode mNode;       // the section node for this strip
        private bool mSelected;          // selected flag
        private ContentView mParentView;  // parent strip view
        private bool mWrap;              // wrap contents
        private bool mEntering;          // entering flag
        private Mutex m_MutexUpdateThread;

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mNode = null;
            Selected = false;
            mWrap = false;
            mEntering = false;

            m_MutexUpdateThread = new Mutex();
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, ContentView parent)
            : this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
            mParentView = parent;
            UpdateColors();
            SetAccessibleName();
            AddCursor(0);
        }


        /// <summary>
        /// Add a new block for a phrase node.
        /// </summary>
        public Block AddBlockForNode(EmptyNode node)
        {
            Block block = node is PhraseNode ? new AudioBlock((PhraseNode)node, this) : new Block(node, this);
            block.Margin = new Padding(0, 0, 0, 0);
            mBlocksPanel.Controls.Add(block);
            mBlocksPanel.Controls.SetChildIndex(block, 1 + node.Index * 2);
            AddCursor(2 * (1 + node.Index));
            UpdateSize();
            return block;
        }

        /// <summary>
        /// Return the block after the selected block or strip. In the case of a strip it is the first block.
        /// Return null if this is the last block, there are no blocks, or nothing was selected in the first place.
        /// This is used for arrow navigation.
        /// </summary>
        public Block BlockAfter(ISelectableInStripView item)
        {
            int count = mBlocksPanel.Controls.Count;
            int index = item is Strip ? 1 :
                        item is StripCursor ? mBlocksPanel.Controls.IndexOf((Control)item) + 1 :
                        item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) + 2 : count;
            return index < count ? (Block)mBlocksPanel.Controls[index] : null;
        }

        /// <summary>
        /// Return the block before the selected block or strip. In the case of a strip this is the last block.
        /// Return null if this the first block, there are no blocks, or nothing was selected in the first place.
        /// </summary>
        public Block BlockBefore(ISelectableInStripView item)
        {
            int index = item is Strip ? mBlocksPanel.Controls.Count - 2 :
                        item is StripCursor ? mBlocksPanel.Controls.IndexOf((Control)item) - 1 :
                        item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) - 2 : 0;
            return index > 0 ? (Block)mBlocksPanel.Controls[index] : null;
        }

        /// <summary>
        /// Find the block for the corresponding node inside the strip.
        /// </summary>
        public Block FindBlock(EmptyNode node)
        {
            foreach (Control c in mBlocksPanel.Controls)
            {
                // this needs to be updated for container blocks
                if (c is Block && ((Block)c).Node == node) return (Block)c;
            }
            return null;
        }

        /// <summary>
        /// Find the strip cursor for the given selection.
        /// </summary>
        public StripCursor FindStripCursor(StripCursorSelection selection)
        {
            return (StripCursor)mBlocksPanel.Controls[selection.Index * 2];
        }

        /// <summary>
        /// Return the first block in the strip, or null if empty.
        /// </summary>
        public Block FirstBlock
        {
            get { return mBlocksPanel.Controls.Count > 1 ? (Block)mBlocksPanel.Controls[1] : null; }
        }

        /// <summary>
        /// Focus on the label.
        /// </summary>
        public void FocusStripLabel() { mLabel.Focus(); }

        /// <summary>
        /// The label of the strip where the title of the section can be edited.
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set
            {
                if (value != null && value != "")
                {
                    mLabel.Label = value;
                    SetAccessibleName();
                }
                int w = mLabel.MinimumSize.Width + mLabel.Margin.Left + mLabel.Margin.Right;
                if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
            }
        }

        /// <summary>
        /// Return the last block in the strip, or null if empty.
        /// </summary>
        public Block LastBlock
        {
            get
            {
                return mBlocksPanel.Controls.Count > 1 ? (Block)mBlocksPanel.Controls[mBlocksPanel.Controls.Count - 2] :
                    null;
            }
        }

        /// <summary>
        /// Get the tab index of the last control in the strip
        /// </summary>
        public int LastTabIndex
        {
            get
            {
                int last = mBlocksPanel.Controls.Count - 2; // index of the last block (if any; last control is a cursor)
                return last >= 0 ? ((Block)mBlocksPanel.Controls[last]).LastTabIndex : TabIndex;
            }
        }

        /// <summary>
        /// Get the section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        /// <summary>
        /// Get the (generic) node for this strip; used for selection.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Get the strips view to which this strip belongs.
        /// </summary>
        public ContentView ParentView { get { return mParentView; } }

        /// <summary>
        /// Remove the block for the given node.
        /// </summary>
        public void RemoveBlock(EmptyNode node)
        {
            Block block = FindBlock(node);
            if (block != null)
            {
                int index = mBlocksPanel.Controls.IndexOf(block);
                mBlocksPanel.Controls.RemoveAt(index + 1);         // remove the cursor after the block
                mBlocksPanel.Controls.RemoveAt(index);             // and the block itself
                UpdateSize();
            }
        }

        /// <summary>
        /// Set the selected flag for the strip. This just tells the strip that it is selected.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value && !(mParentView.Selection is TextSelection);
                UpdateColors();
            }
        }

        /// <summary>
        /// Select a block in the strip.
        /// </summary>
        public Block SelectedBlock { set { mParentView.SelectedNode = value.Node; } }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public NodeSelection SelectionFromView
        {
            set
            {
                if (value is StripCursorSelection)
                {
                    ((StripCursor)mBlocksPanel.Controls[((StripCursorSelection)value).Index * 2]).Selected = true;
                }
                else
                {
                    Selected = value != null;
                }
            }
        }

        /// <summary>
        /// Show the cursor at the current time in the waveform of the current playing block.
        /// </summary>
        public void SelectTimeInBlock(Block block, AudioRange audioRange)
        {
            mParentView.SelectionFromStrip = new AudioSelection((PhraseNode)block.Node, mParentView, audioRange);
        }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Editable = true;
            mParentView.SelectionFromStrip = new TextSelection(mNode, mParentView, Label);
            SetAccessibleName();
        }

        /// <summary>
        /// Get the index of the cursor to select after the given item (cursor, block or strip.)
        /// In the case of a strip this is the first cursor, and in the case of the last cursor position
        /// return -1.
        /// </summary>
        public int StripCursorAfter(ISelectableInStripView item)
        {
            int index = item is Strip ? 0 :
                        item is StripCursor ? mBlocksPanel.Controls.IndexOf((Control)item) + 2 :
                        item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) + 1 : -2;
            return index < mBlocksPanel.Controls.Count ? index / 2 : -1;
        }

        /// <summary>
        /// Get the index of the cursor to select before the given item (cursor, block or strip.)
        /// In the case of a strip this is the last cursor, and in the case of the first cursor position
        /// return a negative value.
        /// </summary>
        public int StripCursorBefore(ISelectableInStripView item)
        {
            return (item is Strip ? mBlocksPanel.Controls.Count - 1 :
                    item is StripCursor ? mBlocksPanel.Controls.IndexOf((Control)item) - 2 :
                    item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) - 1 : -2) / 2;
        }

        /// <summary>
        /// Clear the selection in the strip from its contents.
        /// </summary>
        public void UnselectInStrip()
        {
            mParentView.Selection = null;
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors()
        {
            if (mNode != null)
            {
                // TODO: get colors from profile
                mLabel.BackColor =
                BackColor = mBlocksPanel.BackColor =
                    mSelected ? Color.Yellow :
                    mNode.Used ? Color.LightSkyBlue : Color.LightGray;
            }
        }

        /// <summary>
        /// Update the tab index for the strip and all of its blocks.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            foreach (Control c in mBlocksPanel.Controls)
            {
                if (c is Block) index = ((Block)c).UpdateTabIndex(index);
            }
            return index;
        }

        /// <summary>
        /// Set the wrap parameter.
        /// </summary>
        public bool Wrap
        {
            set
            {
                mWrap = value;
                UpdateSize();
                if (mWrap)
                {
                    mParentView.SizeChanged += new EventHandler(mParentView_SizeChanged);
                }
                else
                {
                    mParentView.SizeChanged -= new EventHandler(mParentView_SizeChanged);
                }
            }
        }

        void mParentView_SizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }


        #region ISearchable Members

        public string ToMatch()
        {
            return Label.ToLowerInvariant();
        }

        #endregion

        // Add a cursor at the end of the strip
        private void AddCursor(int index)
        {
            StripCursor cursor = new StripCursor(this);
            cursor.BackColor = Color.SkyBlue;
            mBlocksPanel.Controls.Add(cursor);
            mBlocksPanel.Controls.SetChildIndex(cursor, index);
            cursor.Click += new EventHandler(delegate(object sender, EventArgs e)
                {
                    if (!cursor.Selected)
                    {
                        mParentView.SelectionFromStrip = new StripCursorSelection(mNode, mParentView,
                            mBlocksPanel.Controls.IndexOf((Control)cursor) / 2);
                    }
                    else
                    {
                        mParentView.Selection = null;
                    }
                }
            );
        }

        // Add content view label to the accessible name of the strip when entering.
        private void AddContentsViewLabel()
        {
            SetAccessibleName();
            if (mParentView.IsEnteringView)
            {
                mLabel.AccessibleName = string.Format("{0} {1}", Localizer.Message("content_view"), mLabel.AccessibleName);
                Thread TrimAccessibleName = new Thread(new ThreadStart(TrimContentsViewAccessibleLabel));
                TrimAccessibleName.Start();
            }
        }

        // Select the label when it is clicked (i.e. made editable) by the user.
        private void Label_EditableChanged(object sender, EventArgs e)
        {
            mParentView.SelectionFromStrip = mLabel.Editable ?
                new TextSelection(mNode, mParentView, mLabel.Label) :
                new NodeSelection(mNode, mParentView);
        }

        // Update the label of the node after the user edited it.
        private void Label_LabelEditedByUser(object sender, EventArgs e)
        {
            if (mLabel.Label != "")
            {
                // update the label for the node
                mParentView.RenameStrip(this);
                mParentView.SelectionFromStrip = new NodeSelection(mNode, mParentView);
            }
            else
            {
                // restore the previous label from the node
                mLabel.Label = mNode.Label;
            }
        }

        // Resize the strip according to the editable label, which size can change.
        // TODO since there are really two possible heights, we should cache these values.
        private void Label_SizeChanged(object sender, EventArgs e)
        {
            mBlocksPanel.Location = new Point(mBlocksPanel.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
            Size = new Size(Width,
                mBlocksPanel.Location.Y + mBlocksPanel.Height + mBlocksPanel.Margin.Bottom);
        }

        // Set verbose accessible name for the strip 
        private void SetAccessibleName() 
        {
                        mLabel.AccessibleName = string.Concat ( mNode.Used ? "" : Localizer.Message("unused"),
                mNode.Label,
                mNode.Duration== 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("time_in_seconds"), mNode.Duration / 1000.0),
                string.Format(Localizer.Message("section_level_to_string"), mNode.IsRooted ? mNode.Level: 0),
                mNode.PhraseChildCount == 0 ? "" :
                    mNode.PhraseChildCount== 1 ? Localizer.Message("section_one_phrase_to_string") :
                        string.Format(Localizer.Message("section_phrases_to_string"), mNode.PhraseChildCount));
                }

        // Toggle selection when clicking
        private void Strip_Click(object sender, EventArgs e)
        {
            ToggleSelection();
        }

        // Toggle selection
        private void ToggleSelection()
        {
            if (mSelected && !mEntering)
            {
                mParentView.SelectionFromStrip = null;
            }
            else
            {
                mParentView.SelectedNode = mNode;
            }
            mEntering = false;
        }

        // Select when tabbed into
        private void Strip_Enter(object sender, EventArgs e)
        {
            mEntering = true;
            AddContentsViewLabel();
            if (mParentView.SelectedSection != mNode && !mParentView.Focusing) mParentView.SelectedNode = mNode;
        }

        // Reset the accessible name after a short while.
        private void TrimContentsViewAccessibleLabel()
        {
            Thread.Sleep(750);
            SetAccessibleName();
        }

        // Update the size of the strip to use the available width of the view
        private void UpdateSize()
        {
            // System.Diagnostics.Debug.Print("<-> Update size");
            if (mWrap)
            {
                MinimumSize = new Size(ParentView.Width, MinimumSize.Height);
                Width = ParentView.Width;
                mBlocksPanel.AutoSize = true;
                mBlocksPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                mBlocksPanel.WrapContents = true;
                Height = mBlocksPanel.Location.Y + mBlocksPanel.Height + mBlocksPanel.Margin.Bottom;
            }
            else
            {
                mBlocksPanel.AutoSize = false;
                mBlocksPanel.WrapContents = false;
                // Compute the minimum width of the block panel
                int minBlockPanelWidth = 0;
                foreach (Control c in mBlocksPanel.Controls) minBlockPanelWidth += c.Width;
                MinimumSize = new Size(minBlockPanelWidth + mBlocksPanel.Margin.Left + mBlocksPanel.Margin.Right, MinimumSize.Height);
            }
        }

        private void Strip_SizeChanged(object sender, EventArgs e)
        {
            // System.Diagnostics.Debug.Print("<-> Resize strip to " + Size);
        }

        private void BlocksPanel_SizeChanged(object sender, EventArgs e)
        {
            // System.Diagnostics.Debug.Print("<---> Resize block panel to " + mBlocksPanel.Size);
        }
        

                public void UpdateBlockLabelsInStrip( object sender  , DoWorkEventArgs e)
        {
            m_MutexUpdateThread.WaitOne();
                        int BlocksCount = mBlocksPanel.Controls.Count;
            Control BlockControl = null ;
                    
                        for (int i = 0 ; i < BlocksCount; i++)
            {
                BlockControl = mBlocksPanel.Controls[i] ;
                if (BlockControl is Block )
                                    {
                                                                                ((Block)BlockControl).UpdateLabelsText();
                }
            }// end loop
                                             m_MutexUpdateThread.ReleaseMutex();
        }

    }
}
