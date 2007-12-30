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


        // Fill (current) width of the waveform.
        protected int WaveformFullWidth { get { return mWaveform == null ? 0 : mWaveform.Margin.Left + mWaveform.Width + mWaveform.Margin.Right; } }

        // Width that the waveform has by default.
        protected int WaveformDefaultWidth
        {
            get
            {
                long time = ((PhraseNode)Node).Audio.getDuration().getTimeDeltaAsMilliseconds();
                return (int)Math.Round(time * AUDIO_SCALE);
            }
        }

        protected override void UpdateLabel()
        {
            string name = mNode.NodeKind == EmptyNode.Kind.Plain ? Localizer.Message("empty_block") :
                mNode.NodeKind == EmptyNode.Kind.Page ? String.Format(Localizer.Message("page_number"), mNode.PageNumber) :
                String.Format(Localizer.Message("kind_block"),
                    mNode.NodeKind == EmptyNode.Kind.Custom ? mNode.CustomClass : mNode.NodeKind.ToString());
            mLabel.Text = String.Format("{0} ({1:0.00}s)",
                Node.NodeKind == EmptyNode.Kind.Plain ? Localizer.Message("audio_block") : name,
                ((PhraseNode)Node).Audio.getDuration().getTimeDeltaAsMillisecondFloat() / 1000);
            AccessibleName = mLabel.Text;
            if (LabelFullWidth > WaveformDefaultWidth)
            {
                if (mWaveform != null) mWaveform.Width = mLabel.Width;
                Size = new Size(LabelFullWidth, Height);
            }
        }

        private void SetWaveform(PhraseNode node)
        {
            mWaveform.AccessibleName = AccessibleName;
            int w = WaveformDefaultWidth;
            mWaveform.Width = w < mLabel.Width ? mLabel.Width : w;
            mWaveform.Media = node.Audio.getMediaData();
            Size = new Size(WaveformFullWidth, Height);
        }   
        
        private void node_NodeAudioChanged(object sender, NodeEventArgs<PhraseNode> e)
        {
            UpdateLabel();
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

        private void mWaveform_Click(object sender, EventArgs e)
        {
            if (CanSelectInWaveform) Strip.SelectTimeInBlock(this, mWaveform.Selection);
        }

        private void mWaveform_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (CanSelectInWaveform)
                {
                    mWaveform.CursorPosition = e.X;
                    Strip.SelectTimeInBlock(this, mWaveform.Selection);
                }
            }
        }

        private void mWaveform_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform) mWaveform.FinalSelectionPosition = e.X;
        }    

        private void mWaveform_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanSelectInWaveform) mWaveform.FinalSelectionPosition = e.X;
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
