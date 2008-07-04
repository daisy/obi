using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, IBlockContainer, ISearchable
    {
        protected EmptyNode mNode;                 // the corresponding node
        private bool mSelected;                    // selected flag
        private IBlockContainer mParentContainer;  // not necessarily a strip!
        private bool mEntering;                    // entering flag (for selection/deselection)


        // Used by the designer
        public Block() { InitializeComponent(); }

        /// <summary>
        /// Create a new empty block from an empty node.
        /// </summary>
        public Block(EmptyNode node, IBlockContainer parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            mSelected = false;
            mEntering = false;
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(Node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(Node_ChangedPageNumber);
            node.ChangedTo_DoStatus += new NodeEventHandler<EmptyNode>(Node_ChangedTo_DoStatus);
            UpdateColors();
            UpdateLabel();
        }


        public ColorSettings ColorSettings
        {
            get { return mParentContainer == null ? null : mParentContainer.ColorSettings; }
            set { UpdateColors(value); }
        }

        /// <summary>
        /// Get the tab index of the block.
        /// </summary>
        public int LastTabIndex { get { return TabIndex; } }

        /// <summary>
        /// The empty node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }

        /// <summary>
        /// The Obi node for this block.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public virtual bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                UpdateColors();
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public virtual NodeSelection SelectionFromView { set { Selected = value != null; } }

        /// <summary>
        /// The strip that contains this block.
        /// </summary>
        public Strip Strip
        {
            get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; }
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors() { UpdateColors(ColorSettings); }

        public void UpdateColors(ColorSettings settings)
        {
            if (mNode != null && settings != null)
            {
                BackColor =
                    mSelected ? settings.BlockSelectedBackColor :
                    mNode.Used ? settings.BlockBackColor : settings.BlockUnusedBackColor;
                ForeColor =
                    mSelected ? settings.BlockSelectedForeColor :
                    mNode.Used ? settings.BlockForeColor : settings.BlockUnusedForeColor;
            }
        }

        /// <summary>
        /// Update the tab index of the block with the new value and return the next index.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            return index + 1;
        }

        #region ISearchable Members

        public string ToMatch()
        {
            return mLabel.Text.ToLowerInvariant();
        }

        #endregion


        // Width of the label (including margins)
        protected int LabelFullWidth { get { return mLabel.Margin.Left + mLabel.Width + mLabel.Margin.Right; } }

        // Generate the label string for this block.
        // Since there is no content, the width is always that of the label's.
        public virtual void UpdateLabel()
        {
            UpdateLabelsText();
            Size = new Size(LabelFullWidth, Height);
        }


        private delegate void UpdateLabelsTextDelegate();

        public virtual void UpdateLabelsText()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateLabelsTextDelegate(UpdateLabelsText));
            }
            else
            {
                mLabel.Text = Node.BaseStringShort();
                mLabel.AccessibleName = Node.BaseString();
                AccessibleName = mLabel.AccessibleName;
            }
        }

        // Select/deselect on click
        private void Block_Click(object sender, EventArgs e) { ToggleSelection(); }

        // Select on tabbing
        protected void Block_Enter(object sender, EventArgs e)
        {
            if (!Strip.ParentView.Focusing)
            {
                mEntering = true;
                Strip.SelectedBlock = this;
            }
        }

        // Select when clickin the label too.
        private void Label_Click(object sender, EventArgs e) { ToggleSelection(); }

        // Update label when the page number changes
        private void Node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }

        // Update the label when the role of the node changes
        private void Node_ChangedKind(object sender, ChangedKindEventArgs e) { UpdateLabel(); }

        // update label when to do status changes
        private void Node_ChangedTo_DoStatus(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }
        // Toggle selection when clicking.
        private void ToggleSelection()
        {
            if (!mSelected || mEntering)
            {
                Strip.SelectedBlock = this;
            }
            else
            {
                Strip.UnselectInStrip();
            }
            mEntering = false;
        }
    }
}
