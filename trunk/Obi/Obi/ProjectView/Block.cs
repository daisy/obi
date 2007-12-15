using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInStripView, ISearchable
    {
        private EmptyNode mNode;                          // the corresponding node
        private bool mSelected;                           // selected flag
        private ISelectableInStripView mParentContainer;  // not necessarily a strip!

        public Block(EmptyNode node, ISelectableInStripView parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            CustomClassLabel = CustomClassLabelFromNode;
            mSelected = false;
            mTimeLabel.Text = "0s";
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(node_ChangedPageNumber);
            UpdateColors();
            if (parent != null) UpdateAccessibleName();
        }

        public Block() { InitializeComponent(); }


        protected virtual void UpdateAccessibleName()
        {
            AccessibleName = String.Format(Localizer.Message("audio_accessible_name"),
                mNode.Index + 1, mNode.ParentAs<ObiNode>().PhraseChildCount);
        }

        private string CustomClassLabelFromNode
        {
            get
            {
                return mNode.NodeKind == EmptyNode.Kind.Plain ? null :
                    mNode.NodeKind == EmptyNode.Kind.Custom ? mNode.CustomClass :
                    mNode.NodeKind == EmptyNode.Kind.Page ? String.Format(Localizer.Message("page_number"), mNode.PageNumber) :
                        Localizer.Message(mNode.NodeKind.ToString());
            }
        }

        // Update the class label with the new page number
        private void node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e)
        {
            CustomClassLabel = CustomClassLabelFromNode;
        }

        // Update the class label with the new kind of node
        private void node_ChangedKind(object sender, ChangedKindEventArgs e)
        {
            CustomClassLabel = CustomClassLabelFromNode;
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
            get { return mCustomClassLabel.Text; }
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
                // TODO Get colors from profile
                BackColor = mSelected ? Color.Yellow : mNode.Used ? Color.HotPink : Color.LightGray;
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
        public Strip Strip
        {
            get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; }

        }

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

        #region ISearchable Members

        public bool Matches(string search)
        {
            return FindInText.Match(this.CustomClassLabel, search);
        }

        public void Replace(string search, string replace)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
