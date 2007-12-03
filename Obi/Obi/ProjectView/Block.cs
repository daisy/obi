using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInStripView
    {
        private EmptyNode mNode;                          // the corresponding node
        private bool mSelected;                           // selected flag
        private ISelectableInStripView mParentContainer;  // not necessarily a strip!

        public Block(EmptyNode node, ISelectableInStripView parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            CustomClassLabel = node.CustomClass;
            mSelected = false;
            mTimeLabel.Text = "0s";
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(node_ChangedKind);
        }

        public Block() { InitializeComponent(); }

        void node_ChangedKind(object sender, ChangedKindEventArgs e)
        {
            CustomClassLabel = e.Node.NodeKind == EmptyNode.Kind.Custom ? e.Node.CustomClass : e.Node.NodeKind.ToString();
        }

        public string CustomClassLabel
        {
            set
            {
                if (value == null)
                {
                    mCustomClassLabel.Text = "";
                    mCustomClassLabel.Visible = false;
                }
                else
                {
                    mCustomClassLabel.Text = value;
                    mCustomClassLabel.Visible = true;
                }
            }
        }

        /// <summary>
        /// The phrase node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// The time label control
        /// </summary>
        public string TimeLabel
        {
            get { return mTimeLabel.Text; }
            set { mTimeLabel.Text = value; }
        }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public virtual bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                BackColor = mSelected ? Color.Yellow : Color.HotPink;
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public virtual NodeSelection SelectionFromView { set { Selected = value != null; } }

        /// <summary>
        /// Get the tab index of the block.
        /// </summary>
        public int LastTabIndex { get { return TabIndex; } }

        /// <summary>
        /// Update the tab index of the block with the new value and return the next index.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            return index;
        }

        /// <summary>
        /// The strip that contains this block.
        /// </summary>
        public Strip Strip { get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; } }

        // Select on click and tabbing
        private void Block_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Click on {0}", this);
            Strip.SelectedBlock = this;
        }

        protected void Block_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Enter {0}?", this);
            if (!Strip.ParentView.Focusing)
            {
                System.Diagnostics.Debug.Print("Yes.");
                Strip.SelectedBlock = this;
            }
            else
            {
                System.Diagnostics.Debug.Print("No.");
            }
        }
        private void mTimeLabel_Click(object sender, EventArgs e) { Strip.SelectedBlock = this; }
        private void mCustomKindLabel_Click(object sender, EventArgs e) { Strip.SelectedBlock = this; }
    }
}
