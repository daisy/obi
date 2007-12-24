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
        private static readonly float AUDIO_SCALE = 0.01f;

        public AudioBlock(PhraseNode node, Strip strip)
            : base(node, strip)
        {
            this.InitializeComponent();
            SetWaveform((PhraseNode)Node);
            node.NodeAudioChanged += new NodeEventHandler<PhraseNode>(node_NodeAudioChanged);
        }


        public bool CanSelectInWaveform { get { return !((ObiForm)ParentForm).IsTransportActive; } }


        protected override void UpdateLabel()
        {
            base.UpdateLabel();
            mLabel.Text = String.Format("{0} ({1:0.00}s)",
                Node.NodeKind == EmptyNode.Kind.Plain ? Localizer.Message("audio_block") : mLabel.Text,
                ((PhraseNode)Node).Audio.getDuration().getTimeDeltaAsMillisecondFloat() / 1000);
            AccessibleName = mLabel.Text;
            int w = mLabel.Margin.Left + mLabel.Width + mLabel.Margin.Right;
            if (mWaveform == null || w > mWaveform.Margin.Left + mWaveform.Width + mWaveform.Margin.Right)
            {
                Size = new Size(w, Height);
                if (mWaveform != null) mWaveform.Width = w;
            }
        }

        private void SetWaveform(PhraseNode node)
        {
            mWaveform.AccessibleName = AccessibleName;
            long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
            int w = (int)Math.Round(time * AUDIO_SCALE);
            mWaveform.Width = w < mLabel.Width ? mLabel.Width : w;
            mWaveform.Media = node.Audio.getMediaData();
            int ww = mWaveform.Width + mWaveform.Margin.Right + mWaveform.Margin.Left;
            if (ww > mLabel.Margin.Left + mLabel.Width + mLabel.Margin.Right) Size = new Size(ww, Height);
        }
        
        private void node_NodeAudioChanged(object sender, NodeEventArgs<PhraseNode> e)
        {
            SetWaveform((PhraseNode)Node);
        }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public override bool Selected
        {
            set
            {
                if (!value) mWaveform.Deselect();
                base.Selected = value;
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

        private void mWaveform_Click(object sender, EventArgs e) { Strip.SelectTimeInBlock(this, mWaveform.Selection); }

        private void mWaveform_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mWaveform.CursorPosition = e.X;
                Strip.SelectTimeInBlock(this, mWaveform.Selection);
            }
        }

        private void mWaveform_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mWaveform.FinalSelectionPosition = e.X;
        }    

        private void mWaveform_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) mWaveform.FinalSelectionPosition = e.X;
        }

        public void SetCursorTime(double time)
        {
            mWaveform.Selection = new AudioRange(time);
            Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        public void UpdateCursorTime(double time) { mWaveform.CursorTime = time; }

        public void SelectAtCurrentTime() { Strip.SelectTimeInBlock(this, mWaveform.Selection); }
    }
}
