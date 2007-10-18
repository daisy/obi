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
        private PhraseNode mNode;  // the corresponding node
        private Strip mStrip;      // the parent strip
        private bool mSelected;    // selected flag

        public Block(PhraseNode node, Strip strip): this()
        {
            mNode = node;
            mStrip = strip;
            mSelected = false;
        }

        public Block() { InitializeComponent(); }


        /// <summary>
        /// The phrase node for this block.
        /// </summary>
        public PhraseNode Node { get { return mNode; } }
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                BackColor = mSelected ? Color.Yellow : Color.HotPink;
            }
        }

        private void Block_Click(object sender, EventArgs e)
        {
            mStrip.SelectedBlock = this;
        }
    }
}
