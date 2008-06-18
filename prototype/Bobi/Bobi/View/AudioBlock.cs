using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class AudioBlock : UserControl
    {
        private Size baseSize;      // base height
        private double zoom;        // zoom factor
        private double audioScale;  // audio zoom
        private AudioNode node;     // audio node
        private bool selected;      // selection flag

        private double selectionMs;  // selection time in milliseconds

        private static readonly float AUDIO_SCALE = 0.01f;  // scale of audio


        public AudioBlock()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selected = false;
            this.node = null;
            this.baseSize = Size;
            this.zoom = 1.0;
            this.audioScale = AUDIO_SCALE;
            this.cursorBar.BaseHeight = this.cursorBar.Height;
            this.selectionMs = -1.0;
        }

        public AudioBlock(AudioNode node): this()
        {
            this.node = node;
            this.node.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(node_changed);
            SetAudio(node.Audio);
            this.selected = false;
        }

        private void node_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.media.MediaEventArgs &&
                ((urakawa.events.media.MediaEventArgs)e).SourceMedia is urakawa.media.data.audio.ManagedAudioMedia)
            {
                SetAudio(((urakawa.media.data.audio.ManagedAudioMedia)((urakawa.events.media.MediaEventArgs)e).SourceMedia).getMediaData());
            }
        }

        /// <summary>
        /// Set the audio scale for the block
        /// </summary>
        public double AudioScale
        {
            set 
            {
                this.audioScale = value * AUDIO_SCALE;
                SetAudio(this.node.Audio);
            }
        }

        /// <summary>
        /// Update colors for this track and its children.
        /// </summary>
        public ColorSettings Colors
        {
            get { return ((Track)Parent.Parent).Colors; }
            set
            {
                BackColor = value.AudioBlockBackColor;
                cursorBar.BackColor = value.AudioBlockBackColor;
                waveformCanvas.BackColor = value.AudioBlockBackColor;
            }
        }

        /// <summary>
        /// Audio node for this block.
        /// </summary>
        public AudioNode Node { get { return this.node; } }

        /// <summary>
        /// Block selection.
        /// </summary>
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
                if (Parent is Track) Colors = ((Track)Parent).Colors;
            }
        }

        /// <summary>
        /// Zoom to fit the new height.
        /// </summary>
        public void SetHeight(int height)
        {
            height -= (Margin.Top + Margin.Bottom);
            this.zoom = (double)height / baseSize.Height;
            this.cursorBar.Zoom = this.zoom;
            Height = height;
            SetAudio(node.Audio);
        }

        public double Zoom
        {
            set
            {
                this.zoom = value;
                this.cursorBar.Zoom = this.zoom;
                Height = (int)Math.Round(baseSize.Height * this.zoom);
                SetAudio(node.Audio);
            }
        }


        private delegate void SetAudioDelegate(urakawa.media.data.audio.AudioMediaData audio);

        private void SetAudio(urakawa.media.data.audio.AudioMediaData audio)
        {
            if (InvokeRequired)
            {
                Invoke(new SetAudioDelegate(SetAudio), audio);
            }
            else
            {
                Width = WidthForAudio(audio);
                this.waveformCanvas.Audio = audio;
                if (audio != null && Parent != null && Parent.Parent is Track) ((Track)Parent.Parent).UpdateSize();
            }
        }

        private int WidthForAudio(urakawa.media.data.audio.AudioMediaData audio)
        {
            return (int)Math.Round(this.zoom * (audio == null ? this.baseSize.Width :
                this.audioScale * audio.getAudioDuration().getTimeDeltaAsMillisecondFloat()));
        }

        public int SelectionX
        {
            get
            {
                return this.node.Audio == null ? -1 :
                    (int)Math.Round(this.selectionMs / this.node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() * this.waveformCanvas.Width);
            }
            set
            {
                this.selectionMs = value * this.node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() / this.waveformCanvas.Width;
                this.cursorBar.Invalidate();
                this.waveformCanvas.Invalidate();
            }
        }
    }
}
