using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl
    {
        private SectionNode mNode;                    // the section node for this strip
        public event EventHandler LabelEditedByUser;  // raised when the user has edited the label

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mLabel.FontSize = 18.0F;
            mNode = null;
            mLabel.LabelEditedByUser += new EventHandler(delegate(object sender, EventArgs e)
            {
                if (LabelEditedByUser != null) LabelEditedByUser(this, e);
            });
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node): this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
        }


        /// <summary>
        /// The label of the strip (i.e. the title of the section; editable.)
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set { mLabel.Label = value; }
        }

        /// <summary>
        /// The section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        // Resize the strip according to the editable label, whose size can change.
        // TODO since there are really two possible heights, we should cache these values.
        private void mLabel_SizeChanged(object sender, EventArgs e)
        {
            mBlocksPanel.Location = new Point(mBlocksPanel.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom);
            Size = new Size(Width,
                mBlocksPanel.Location.Y + mBlocksPanel.Height + mBlocksPanel.Margin.Bottom);
        }
    }
}
