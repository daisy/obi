using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInStripView
    {
        private SectionNode mNode;       // the section node for this strip
        private bool mSelected;          // selected flag
        private StripsView mParentView;  // parent strip view

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mLabel.FontSize = 18.0F;
            mNode = null;
            Selected = false;
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, StripsView parent)
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


        private void SetAccessibleName()
        {
            string Unused_Tag = "";

            if (mNode != null)
            {
                if (!mNode.Used)
                    Unused_Tag = Localizer.Message("Accessible_Label_Unused");

                mLabel.AccessibleName = this.Label + " - depth " + mNode.Level + Unused_Tag ;
            }
            else
                mLabel.AccessibleName = this.Label;
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
            UpdateWidth();
            AddCursor(2 * (1 + node.Index));
            return block;
        }

        /// <summary>
        /// Return the block after the selected block or strip. In the case of a strip is the first block.
        /// Return null if this the last block, there are no blocks, or nothing was selected in the first place.
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
        /// The label of the strip (i.e. the title of the section; editable.)
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
                int w = mLabel.Location.X + mLabel.MinimumSize.Width + mLabel.Margin.Right;
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
        /// The section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        /// <summary>
        /// The (generic) node for this strip; used for selection.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        public StripsView ParentView { get { return mParentView; } }

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
                UpdateWidth();
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
            return index < mBlocksPanel.Controls.Count ? index / 2: -1;
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
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors()
        {
            if (mNode != null)
            {
                // Get colors from profile
                mLabel.BackColor = mNode.Used ? Color.Thistle : Color.LightGray;
                BackColor = mBlocksPanel.BackColor =
                    mSelected ? Color.Yellow :
                    mNode.Used ? Color.LightBlue : Color.LightGray;
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

        #region ISearchable Members

        public bool Matches(string search)
        {
            return FindInText.Match(Label, search);
        }

        public void Replace(string search, string replace)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


        // Add a cursor at the end of the strip
        private void AddCursor(int index)
        {
            StripCursor cursor = new StripCursor(this);
            cursor.Size = new Size(12, mBlocksPanel.Height);
            mBlocksPanel.Controls.Add(cursor);
            mBlocksPanel.Controls.SetChildIndex(cursor, index);
            cursor.Click += new EventHandler(delegate(object sender, EventArgs e)
                {
                    mParentView.SelectionFromStrip = new StripCursorSelection(mNode, mParentView,
                        mBlocksPanel.Controls.IndexOf((Control)cursor) / 2);
                }
            );
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

        // Resize the strip according to the editable label, whose size can change.
        // TODO since there are really two possible heights, we should cache these values.
        private void Label_SizeChanged(object sender, EventArgs e)
        {
            mBlocksPanel.Location = new Point(mBlocksPanel.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
            Size = new Size(Width,
                mBlocksPanel.Location.Y + mBlocksPanel.Height + mBlocksPanel.Margin.Bottom);
        }

        // The user clicked on this strip, so select it if it wasn't already selected
        private void Strip_Click(object sender, EventArgs e)
        {
            if (!mSelected) mParentView.SelectedNode = mNode;
        }

        // Select when tabbed into
        private void Strip_Enter(object sender, EventArgs e)
        {
            if (mParentView.SelectedSection != mNode && !mParentView.Focusing) mParentView.SelectedNode = mNode;
        }

        // Update the width of the strip to use the available width of the view
        private void UpdateWidth()
        {
            int w = 0;
            foreach (Control c in mBlocksPanel.Controls) w += c.Width + c.Margin.Right;
            if (mBlocksPanel.Controls.Count > 0) w -= mBlocksPanel.Controls[mBlocksPanel.Controls.Count - 1].Margin.Right;
            if (w > mBlocksPanel.Width) mBlocksPanel.Size = new Size(w, mBlocksPanel.Height);
            w += mBlocksPanel.Location.X + mBlocksPanel.Margin.Right;
            if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
        }


        public void FocusStripLabel()
        {
            mLabel.Focus();
        }
    }
}
