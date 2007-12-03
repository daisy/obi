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

        public AudioBlock(PhraseNode node, Strip strip): base(node, strip)
        {
            this.InitializeComponent();
            if (node.Audio != null)
            {
                SetWaveform((PhraseNode)Node);
            }
            else
            {
                TimeLabel = "0s";
                mWaveform.Visible = false;
            }
            node.NodeAudioChanged += new NodeEventHandler<PhraseNode>(node_NodeAudioChanged);
        }
      
        private void SetWaveform(PhraseNode node)
        {
             long time = node.Audio.getDuration().getTimeDeltaAsMilliseconds();
             mWaveform.Width = (int)Math.Round(time * AUDIO_SCALE);
             mWaveform.Media = node.Audio.getMediaData();
             TimeLabel = String.Format("{0:0.00}s",
             ((PhraseNode)Node).Audio.getDuration().getTimeDeltaAsMillisecondFloat() / 1000);
             Size = new Size(mWaveform.Width + mWaveform.Margin.Right + mWaveform.Margin.Left, Height);
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
                if (value != null) mWaveform.Selection = value.Waveform;
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
            mWaveform.Selection = new WaveformSelection(time);
            Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        public void UpdateCursorTime(double time)
        {
            if (time > mWaveform.Selection.CursorTime) mWaveform.CursorTime = time;
        }

        public void SelectAtCurrentTime() { Strip.SelectTimeInBlock(this, mWaveform.Selection); }
    }
}
