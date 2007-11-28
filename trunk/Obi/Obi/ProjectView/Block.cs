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
        private PhraseNode mNode;   // the corresponding node
        private Strip mStrip;       // the parent strip
        private bool mSelected;     // selected flag

        private static readonly float AUDIO_SCALE = 0.01f;

        public Block(PhraseNode node, Strip strip): this()
        {
            mNode = node;
            CustomKindLabel = node.CustomKind;
            mStrip = strip;
            mSelected = false;
            if (node.Audio != null)
            {
                long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
                mWaveform.Width = (int)Math.Round(time * AUDIO_SCALE);
                mWaveform.Media = node.Audio.getMediaData();
                mTimeLabel.Text = String.Format("{0:0.00}s",
                    node.Audio.getDuration().getTimeDeltaAsMillisecondFloat() / 1000);
                Size = new Size(mWaveform.Width + mWaveform.Margin.Right + mWaveform.Margin.Left, Height);
                node.CustomTypeChanged += new ChangedCustomTypeEventHandler(node_CustomTypeChanged);
            }
            else
            {
                mTimeLabel.Text = "0s";
                mWaveform.Visible = false;
            }
        }

        
        public Block() { InitializeComponent(); }

        void node_CustomTypeChanged(object sender, Events.Node.ChangedCustomTypeEventArgs e)
        {
            CustomKindLabel = e.CustomType;
        }

        public string CustomKindLabel
        {
            set
            {
                if (value == null)
                {
                    mCustomKindLabel.Text = "";
                    mCustomKindLabel.Visible = false;
                }
                else
                {
                    mCustomKindLabel.Text = value;
                    mCustomKindLabel.Visible = true;
                }
            }
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
                if (!mSelected) mWaveform.Deselect();
                BackColor = mSelected ? Color.Yellow : Color.HotPink;
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public NodeSelection SelectionFromView
        {
            set
            {
                if (value == null)
                {
                    Selected = false;
                }
                else
                {
                    mWaveform.Selection = value.Waveform;
                    Selected = true;
                }
            }
        }

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
        public Strip Strip { get { return mStrip; } }

        public void GiveFocus()
        {
            if (!Focused)
            {
                Enter -= new EventHandler(Block_Enter);
                mStrip.GiveFocus();
                Focus();
                Enter += new EventHandler(Block_Enter);
            }
        }

        // Select on click and tabbing
        private void Block_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void Block_Enter(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void mTimeLabel_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void mCustomKindLabel_Click(object sender, EventArgs e) { mStrip.SelectedBlock = this; }
        private void mWaveform_Click(object sender, EventArgs e) { mStrip.SelectTimeInBlock(this, mWaveform.Selection); }

        private void mWaveform_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mWaveform.CursorPosition = e.X;
        }

        private void mWaveform_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mWaveform.FinalSelectionPosition = e.X;
        }    

        private void mWaveform_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mWaveform.FinalSelectionPosition = e.X;
        }
    }
}
