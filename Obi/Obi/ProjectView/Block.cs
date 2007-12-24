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
            mSelected = false;
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(node_ChangedPageNumber);
            UpdateColors();
            UpdateLabel();
        }

        public Block() { InitializeComponent(); }


        // Generate the label string for this block.
        protected virtual void UpdateLabel()
        {
            string name = mNode.NodeKind == EmptyNode.Kind.Plain ? Localizer.Message("empty_block") :
                mNode.NodeKind == EmptyNode.Kind.Page ? String.Format(Localizer.Message("page_number"), mNode.PageNumber) :
                String.Format(Localizer.Message("kind_block"),
                    mNode.NodeKind == EmptyNode.Kind.Custom ? mNode.CustomClass : mNode.NodeKind.ToString());
            mLabel.Text = name;
            AccessibleName = name;
        }

        /*protected virtual void UpdateAccessibleName()
        {
            AccessibleName = mCustomClassLabel.Text + String.Format(Localizer.Message("audio_accessible_name"),
                mNode.Index + 1, mNode.ParentAs<ObiNode>().PhraseChildCount);
        }*/

        private void node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }
        private void node_ChangedKind(object sender, ChangedKindEventArgs e) { UpdateLabel(); }

        /// <summary>
        /// The phrase node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }
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
        private void mLabel_Click(object sender, EventArgs e) { Strip.SelectedBlock = this; }

        #region ISearchable Members

        public bool Matches(string search)
        {
            return FindInText.Match(this.mLabel.Text, search);
        }

        public void Replace(string search, string replace)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
