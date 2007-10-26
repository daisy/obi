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

        private static readonly float AUDIO_SCALE = 0.02f;

        public Block(PhraseNode node, Strip strip): this()
        {
            mNode = node;
            mStrip = strip;
            mSelected = false;
            long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
            mTimeLabel.Text = FormatTime(time);
            int width = (int)Math.Round(time * AUDIO_SCALE);
            Size = new Size(width < mTimeLabel.Width ? mTimeLabel.Width : width, Height);
        }

        public Block() { InitializeComponent(); }

        private string FormatTime(long milliseconds)
        {
            return String.Format("{0:0.00}s", milliseconds / 1000.0);
        }


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
