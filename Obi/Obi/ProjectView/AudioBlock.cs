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

        public const int NORMAL_PRIORITY = 1;
        public const int STRIP_SELECTED_PRIORITY = 2;
        public const int BLOCK_SELECTED_PRIORITY = 3;
        public const int WAVEFORM_SELECTED_PRIORITY = 4;

        //@singleSection: height and width is increased by 20%, height is increased from properties of designer from 128 to 154

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
            if (node != null && mWaveform != null)
            {
                if (node.Audio.Duration.AsTimeSpan.TotalMilliseconds > 0.0)
                {
                    mWaveform.Visible = true;
                    mWaveform.BackColor = BackColor;
                    mWaveform.AccessibleName = AccessibleName;
                    mWaveform.Location = new Point(0, mLabel.Height + mLabel.Margin.Bottom);
                    mWaveform.Size = new Size(WaveformDefaultWidth,
                        Height - mLabel.Height - mLabel.Margin.Bottom - mWaveform.Margin.Vertical - BorderHeight);
                    mWaveform.Block = this;
                    Size = new Size(WaveformFullWidth, Height);
                }
                else
                {
                    mWaveform.Visible = false;
                }
            }
        }




        /// <summary>
        /// Update the playback cursor time, and return its (horizontal) position in the waveform.
        /// </summary>
        public int UpdateCursorTime(double time) { return mWaveform != null ? mWaveform.SetCursorTime(time): 0; }

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
                if (!value && mWaveform != null) mWaveform.Deselect();
                base.Highlighted = value &&  mWaveform != null  && mWaveform.Selection == null;
                if (base.Highlighted) PrioritizeRendering(BLOCK_SELECTED_PRIORITY);
            }
        }

        public void PrioritizeRendering(int priority)
        {
            if ( mWaveform != null )  ContentView.RenderWaveform(mWaveform, priority);
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public override void SetSelectionFromContentView(NodeSelection selection)
        {
            if (selection != null && mWaveform != null) mWaveform.Selection = selection is AudioSelection ? ((AudioSelection)selection).AudioRange : null;
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
            mRecordingLabel.Font = new Font(Font.FontFamily, zoom * mBaseFontSize);
            mRecordingLabel.Location = new Point(0, mLabel.Height + mLabel.Location.Y);
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

        public readonly int MaxWaveformWidth = 32700;
        // Width that the waveform has by default.
        protected int WaveformDefaultWidth
        {
            get
            {
                int w = ComputeWaveformDefaultWidth();
                // workaround to prevent visibility problem in block layout, waveform should remain below blocklayout width of 32768
                if (w > MaxWaveformWidth) 
                    {
                    w = MaxWaveformWidth;
                    }
                return w;
            }
        }

        public int ComputeWaveformDefaultWidth()
        {
            long time = (long)((PhraseNode)Node).Audio.Duration.AsTimeSpan.TotalMilliseconds;
            // originally, 1 second should has width of 10 pixels
            //int w =  time == 0.0 ? LabelFullWidth : (int)Math.Round(time * AudioScale);//@singleSection: original
            int w = time == 0.0 ? LabelFullWidth : (int)Math.Round(time * AudioScale * 1.2f);//@singleSection: updated
            return w;
        }

        // Clicking selects at that point (see mouse up/down)
        private void mWaveform_Click(object sender, EventArgs e)
        {
            if (CanSelectInWaveform && mWaveform.Selection != null)
            {
                Strip.ContentView.DisableScrolling();
                Strip.SetSelectedAudioInBlockFromBlock(this, mWaveform.Selection);
            }
        }

        // Track down the shift key for selection
        private void mWaveform_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShiftKeyPressed = true; }
        private void mWaveform_KeyUp(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.ShiftKey) mShiftKeyPressed = false; }

        // Double clicking on the waveform selects all.
        private void mWaveform_DoubleClick(object sender, EventArgs e)
        {
            if (CanSelectInWaveform)
            {
                mWaveform.Selection = new AudioRange(0.0, ((PhraseNode)mNode).Audio.Duration.AsTimeSpan.TotalMilliseconds);
                Strip.SetSelectedAudioInBlockFromBlock(this, mWaveform.Selection);
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
                Strip.ContentView.DisableScrolling();
                Strip.SetSelectedAudioInBlockFromBlock(this, mWaveform.Selection);
            }
        }

        // ... and commit it (select) on mouse up outside of the waveform (otherwise the click event is not registered ?!)
        private void mWaveform_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform)
            {
                if (e.X < 0 || e.X > mWaveform.Width) Strip.SetSelectedAudioInBlockFromBlock(this, mWaveform.Selection);
            }
        }    

        // Update the audio range selection when moving the mouse.
        private void mWaveform_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform)
            {
                mWaveform.FinalSelectionPosition = e.X;
            }
        }


        public void SelectAtCurrentTime() { Strip.SetSelectedAudioInBlockFromBlock(this, mWaveform.Selection); }

        public void InitCursor(double time)
        {
            if (mWaveform != null)
            {
                mWaveform.InitCursor(time);
                Strip.ContentView.ScrollControlIntoView(this);
            }
        }

        public void ClearCursor() { if ( mWaveform != null )  mWaveform.ClearCursor(); }


        public void GetLocationXForAudioCursorAndSelection(out int audioCursorPosition, out int selectionStartPosition, out int selectionEndPosition)
        {
            audioCursorPosition = selectionStartPosition = selectionEndPosition = -1;
            if (mWaveform != null)
            {
                audioCursorPosition = mWaveform.Location.X + mWaveform.CursorPosition;
                if (mWaveform.Selection != null)
                {
                    selectionStartPosition = mWaveform.Selection.HasCursor ? mWaveform.SelectionPointPosition + mWaveform.Location.X : mWaveform.InitialSelectionPosition + mWaveform.Location.X;
                    selectionEndPosition = mWaveform.Selection.HasCursor ? -1 : mWaveform.FinalSelectionPosition + mWaveform.Location.X;
                }
            }
            //Console.WriteLine(audioCursorPosition + " : " + selectionStartPosition + " : " + selectionEndPosition);
        }


        protected override void Block_Disposed(object sender, EventArgs e)
        {
            if (mNode != null  && mNode is PhraseNode)  ((PhraseNode) mNode).NodeAudioChanged -= new NodeEventHandler<PhraseNode>(node_NodeAudioChanged);
            if (mWaveform != null && !mWaveform.IsDisposed) mWaveform.Dispose();
            mWaveform = null;
            base.Block_Disposed(sender, e);
        }

    }
}
