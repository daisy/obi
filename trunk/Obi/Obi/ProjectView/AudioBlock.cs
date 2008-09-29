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
        private bool mShiftKeyPressed;  // track the shift key


        /// <summary>
        /// Create a new audio block for a phrase node in a strip.
        /// </summary>
        public AudioBlock(PhraseNode node, Strip strip)
            : base(node, strip)
        {
            InitializeComponent();
            SetWaveform(Node as PhraseNode);
            node.NodeAudioChanged += new NodeEventHandler<PhraseNode>(node_NodeAudioChanged);
            mShiftKeyPressed = false;
        }


        /// <summary>
        /// Set cursor time during playback to show the current position.
        /// </summary>
        public void SetCursorTime(double time)
        {
            mWaveform.CursorTime = time;
            Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }


        // Audio of the block has changed: update the label and the width to accomodate the new audio.
        private void node_NodeAudioChanged(object sender, NodeEventArgs<PhraseNode> e)
        {
            SetWaveform(e.Node);
            UpdateLabel();
        }

        // Set the waveform from the audio on a phrase node.
        // Resize the block to fit both the whole waveform and its label.
        private void SetWaveform(PhraseNode node)
        {
            if (node != null)
            {
                mWaveform.BackColor = BackColor;
                mWaveform.AccessibleName = AccessibleName;
                mWaveform.Location = new Point(0, mLabel.Height + mLabel.Margin.Bottom);
                mWaveform.Size = new Size(WaveformDefaultWidth, Height - mLabel.Height - mLabel.Margin.Bottom);
                mWaveform.Block = this;
                Size = new Size(WaveformFullWidth, Height);
            }
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
                if (base.Highlighted) PrioritizeRendering(WaveformWithPriority.BLOCK_SELECTED_PRIORITY);
            }
        }

        public void PrioritizeRendering(int priority)
        {
            ContentView.RenderWaveform(new WaveformWithPriority(mWaveform, priority));
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public override void SetSelectionFromContentView(NodeSelection selection)
        {
            if (selection != null) mWaveform.Selection = selection is AudioSelection ? ((AudioSelection)selection).AudioRange : null;
            base.SetSelectionFromContentView(selection);
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
        protected override void UpdateLabel()
        {
            UpdateLabelsText();
        }

        public override void UpdateLabelsText()
        {
            base.UpdateLabelsText();
            if (mWaveform != null) mWaveform.AccessibleName = AccessibleName;
        }

        public Waveform Waveform { get { return mWaveform; } }

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


        // Clicking selects at that point (see mouse up/down)
        private void mWaveform_Click(object sender, EventArgs e)
        {
            if (CanSelectInWaveform) Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        // Track down the shift key for selection
        private void mWaveform_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShiftKeyPressed = true; }
        private void mWaveform_KeyUp(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShiftKeyPressed = false; }

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
                if (mShiftKeyPressed && mWaveform.Selection != null)
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
