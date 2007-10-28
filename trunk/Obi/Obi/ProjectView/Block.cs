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

        private static readonly float AUDIO_SCALE = 0.01f;

        public Block(PhraseNode node, Strip strip): this()
        {
            mNode = node;
            mStrip = strip;
            mSelected = false;
            long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
            mTimeLabel.Text = FormatTime(time);
            int width = (int)Math.Round(time * AUDIO_SCALE);
            Size = new Size(width < MinimumWidth ? MinimumWidth : width, Height);
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


        // Select on click
        private void Block_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void TimeLabel_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }

        // The block shouldn't be smaller than its label
        private int MinimumWidth
        {
            get { return mTimeLabel.Location.X + mTimeLabel.Width + mTimeLabel.Margin.Right; }
        }

        // Format the time string to display in the block.
        private string FormatTime(long milliseconds)
        {
            return milliseconds > 60000 ?
                String.Format("{0}:{1:00}", milliseconds / 60000, Math.Round((milliseconds % 60000) / 1000.0)) : 
                String.Format("{0:0.00}s", milliseconds / 1000.0);
        }
    }
}
