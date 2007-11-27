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

        private void Label_LabelEditedByUser(object sender, EventArgs e)
        {
            if (mLabel.Label != "")
            {
                // update the label for the node
                mParentView.RenameStrip(this);
                mParentView.Selection = new NodeSelection(mNode, mParentView);
            }
            else
            {
                // restore the previous label from the node
                mLabel.Label = mNode.Label;
            }
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, StripsView parent): this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
            mParentView = parent;
            UpdateColors();
        }


        /// <summary>
        /// Add a new block for a phrase node.
        /// </summary>
        public Block AddBlockForPhrase(PhraseNode phrase)
        {
            Block block = new Block(phrase, this);
            mBlocksPanel.Controls.Add(block);
            mBlocksPanel.Controls.SetChildIndex(block, phrase.Index);
            UpdateWidth();
            return block;
        }

        /// <summary>
        /// The label of the strip (i.e. the title of the section; editable.)
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set
            {
                if (value != null && value != "") mLabel.Label = value;
                int w = mLabel.Location.X + mLabel.MinimumSize.Width + mLabel.Margin.Right;
                if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
            }
        }

        /// <summary>
        /// Get the tab index of the last control in the strip
        /// </summary>
        public int LastTabIndex
        {
            get
            {
                int count = mBlocksPanel.Controls.Count;
                return count == 0 ? TabIndex : ((Block)mBlocksPanel.Controls[count - 1]).LastTabIndex;
            }
        }

        /// <summary>
        /// Update the tab index for the strip and all of its blocks.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            foreach (Control c in mBlocksPanel.Controls) index = ((Block)c).UpdateTabIndex(index);
            return index;
        }

        /// <summary>
        /// The section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the strip. This just tells the strip that it is selected.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value && mParentView.Selection.Text == null;
                UpdateColors();
            }
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors()
        {
            if (mNode != null)
            {
                mLabel.BackColor = mNode.Used ? Color.Thistle : Color.LightGray;    
                BackColor = mBlocksPanel.BackColor =
                    mSelected ? Color.Yellow :
                    mNode.Used ? Color.LightBlue : Color.LightGray;
            }
        }

        /// <summary>
        /// Select a block in the strip.
        /// </summary>
        public Block SelectedBlock { set { mParentView.SelectedNode = value.Node; } }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Editable = true;
            mParentView.Selection = new NodeSelection(mNode, mParentView, Label);
        }

        // Resize the strip according to the editable label, whose size can change.
        // TODO since there are really two possible heights, we should cache these values.
        private void mLabel_SizeChanged(object sender, EventArgs e)
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

        private void mLabel_EditableChanged(object sender, EventArgs e)
        {
            if (mLabel.Editable) mParentView.Selection = new NodeSelection(mNode, mParentView, mLabel.Label);
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

        public Block FindBlock(PhraseNode phrase)
        {
            foreach (Control c in mBlocksPanel.Controls)
            {
                if (c is Block && ((Block)c).Node == phrase) return (Block)c;
            }
            return null;
        }

        public void RemoveBlock(PhraseNode phrase)
        {
            mBlocksPanel.Controls.Remove(FindBlock(phrase));
            UpdateWidth();
        }

        private void UpdateWidth()
        {
            int w = 0;
            foreach (Control c in mBlocksPanel.Controls) w += c.Width + c.Margin.Right;
            if (mBlocksPanel.Controls.Count > 0) w -= mBlocksPanel.Controls[mBlocksPanel.Controls.Count - 1].Margin.Right;
            if (w > mBlocksPanel.Width) mBlocksPanel.Size = new Size(w, mBlocksPanel.Height);
            w += mBlocksPanel.Location.X + mBlocksPanel.Margin.Right;
            if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
        }

        private void Strip_Enter(object sender, EventArgs e)
        {
            if (mParentView.SelectedSection != mNode) mParentView.SelectedNode = mNode;
        }

        /// <summary>
        /// Return the block after the selected block or strip. In the case of a strip is the first block.
        /// Return null if this the last block, there are no blocks, or nothing was selected in the first place.
        /// </summary>
        public Block BlockAfter(ISelectableInStripView item)
        {
            int count = mBlocksPanel.Controls.Count;
            int index = item is Strip ? 0 :
                        item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) + 1 :
                                        count;
            return index < count ? (Block)mBlocksPanel.Controls[index] : null;
        }

        /// <summary>
        /// Return the block before the selected block or strip. In the case of a strip this is the last block.
        /// Return null if this the first block, there are no blocks, or nothing was selected in the first place.
        /// </summary>
        public Block BlockBefore(ISelectableInStripView item)
        {
            int index = item is Strip ? mBlocksPanel.Controls.Count :
                        item is Block ? mBlocksPanel.Controls.IndexOf((Control)item) :
                                        0;
            return index > 0 ? (Block)mBlocksPanel.Controls[index - 1] : null;
        }

        /// <summary>
        /// Return the last block in the strip, or null if empty.
        /// </summary>
        public Block LastBlock
        {
            get
            {
                return mBlocksPanel.Controls.Count > 0 ? (Block)mBlocksPanel.Controls[mBlocksPanel.Controls.Count - 1]:
                    null;
            }
        }

        /// <summary>
        /// Return the first block in the strip, or null if empty.
        /// </summary>
        public Block FirstBlock
        {
            get
            {
                return mBlocksPanel.Controls.Count > 0 ? (Block)mBlocksPanel.Controls[0] : null;
            }
        }

        public void SelectTimeInBlock(Block block, WaveformSelection waveformSelection)
        {
            mParentView.SelectionFromStrip = new NodeSelection(block.Node, mParentView, waveformSelection);
        }
    }
}
