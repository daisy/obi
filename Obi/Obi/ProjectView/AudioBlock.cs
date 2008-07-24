using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class AudioBlock : Block
    {
        private bool mShift;                                // track the shift key


        /// <summary>
        /// Create a new audio block for a phrase node in a strip.
        /// </summary>
        public AudioBlock(PhraseNode node, Strip strip)
            : base(node, strip)
        {
            InitializeComponent();
            SetWaveform((PhraseNode)Node);
            node.NodeAudioChanged += new NodeEventHandler<PhraseNode>(node_NodeAudioChanged);
            mShift = false;
        }


        public void SetCursorTime(double time)
        {
            mWaveform.CursorTime = time;
            Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        public void UpdateCursorTime(double time) { mWaveform.CursorTime = time; }

        /// <summary>
        /// True if selection in the waveform is enabled.
        /// </summary>
        public bool CanSelectInWaveform { get { return true; } }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public override bool Highlighted
        {
            get { return base.Highlighted; }
            set
            {
                if (!value) mWaveform.Deselect();
                base.Highlighted = value && mWaveform.Selection == null;
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public override NodeSelection SelectionFromView
        {
            set
            {
                if (value != null) mWaveform.Selection = value is AudioSelection ? ((AudioSelection)value).AudioRange : null;
                base.SelectionFromView = value;
            }
        }

        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                if (mWaveform != null) mWaveform.BackColor = value;
            }
        }


        // Set the waveform from the audio on a phrase node.
        // Resize the block to fit both the whole waveform and its label.
        private void SetWaveform(PhraseNode node) { SetWaveform(node, false); }
        private void SetWaveform(PhraseNode node, bool update)
        {
            if (node != null)
            {
                mWaveform.BackColor = BackColor;
                mWaveform.AccessibleName = AccessibleName;
                int w = WaveformDefaultWidth;
                mWaveform.Location = new Point(0, mLabel.Height + mLabel.Margin.Bottom);
                mWaveform.Size = new Size(w < mLabel.Width ? mLabel.Width : w, Height - mLabel.Height - mLabel.Margin.Bottom);
                if (update)
                {
                    mWaveform.UpdateMedia();
                }
                else
                {
                    mWaveform.Media = node.Audio.getMediaData();
                }
                Size = new Size(WaveformFullWidth, Height);
            }
        }

        public float AudioScale
        {
            get { return Strip == null ? 0.01f : Strip.AudioScale; }
            set { SetWaveform(mNode as PhraseNode); }
        }

        public override void SetZoomFactorAndHeight(float zoom, int height)
        {
            base.SetZoomFactorAndHeight(zoom, height);
            SetWaveform(mNode as PhraseNode);
        }

        // Update label and waveform when there is new information to display.
        public override void UpdateLabel()
        {
            UpdateLabelsText();
            if (LabelFullWidth > WaveformDefaultWidth)
            {
                if (mWaveform != null) mWaveform.Width = mLabel.Width;
                Size = new Size(LabelFullWidth, Height);
            }
        }

        public override void UpdateLabelsText()
        {
            mLabel.Text = Node.BaseStringShort();
            AccessibleName = Node.BaseString();
            if (mWaveform != null) mWaveform.AccessibleName = AccessibleName;
        }

        // Fill (current) width of the waveform.
        protected int WaveformFullWidth { get { return mWaveform == null ? 0 : mWaveform.Margin.Left + mWaveform.Width + mWaveform.Margin.Right; } }

        // Width that the waveform has by default.
        protected int WaveformDefaultWidth
        {
            get
            {
                long time = ((PhraseNode)Node).Audio.getDuration().getTimeDeltaAsMilliseconds();
                return (int)Math.Round(time * AudioScale);
            }
        }

        // Update the waveform when the audio of the phrase node has changed.
        private void node_NodeAudioChanged(object sender, NodeEventArgs<PhraseNode> e)
        {
            UpdateLabel();
            SetWaveform((PhraseNode)Node, true);
        }


        // Clicking selects at that point (see mouse up/down)
        private void mWaveform_Click(object sender, EventArgs e)
        {
            if (CanSelectInWaveform) Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        // Track down the shift key for selection
        private void mWaveform_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShift = true; }
        private void mWaveform_KeyUp(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShift = false; }

        // Double clicking on the waveform selects all.
        private void mWaveform_DoubleClick(object sender, EventArgs e)
        {
            if (CanSelectInWaveform)
            {
                mWaveform.Selection = new AudioRange(0.0, ((PhraseNode)mNode).Audio.getDuration().getTimeDeltaAsMillisecondFloat());
                Strip.SelectTimeInBlock(this, mWaveform.Selection);
            }
        }

        // Update the selection position on mouse down...
        private void mWaveform_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform)
            {
                if (mShift && mWaveform.Selection != null)
                {
                    int begin = mWaveform.SelectionPointPosition;
                    if (begin < e.X)
                    {
                        mWaveform.FinalSelectionPosition = e.X;
                    }
                    else
                    {
                        if (mWaveform.Selection.HasCursor)
                        {
                            mWaveform.SelectionPointPosition = e.X;
                            mWaveform.FinalSelectionPosition = begin;
                        }
                        else
                        {
                            int end = mWaveform.FinalSelectionPosition;
                            mWaveform.SelectionPointPosition = e.X;
                            mWaveform.FinalSelectionPosition = end;
                        }
                    }
                }
                else
                {
                    mWaveform.SelectionPointPosition = e.X;
                }
                Strip.SelectTimeInBlock(this, mWaveform.Selection);
            }
        }

        // ... and commit it (select) on mouse up outside of the waveform (otherwise the click event is not registered ?!)
        private void mWaveform_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform)
            {
                if (e.X < 0 || e.X > mWaveform.Width) Strip.SelectTimeInBlock(this, mWaveform.Selection);
            }
        }    

        // Update the audio range selection when moving the mouse.
        private void mWaveform_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform) mWaveform.FinalSelectionPosition = e.X;
        }


        public void SelectAtCurrentTime() { Strip.SelectTimeInBlock(this, mWaveform.Selection); }

        public void InitCursor() { mWaveform.InitCursor(); }
        public void ClearCursor() { mWaveform.ClearCursor(); }
    }
}
