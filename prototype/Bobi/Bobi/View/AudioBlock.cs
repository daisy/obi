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
        private Size baseSize;        // base height
        private double zoom;          // zoom factor
        private double audioScale;    // audio zoom
        private AudioNode node;       // audio node
        private Selection selection;  // current selection (node or audio selection)

        private static readonly float AUDIO_SCALE = 0.01f;  // scale of audio


        /// <summary>
        /// Create an empty audio block. (Normally this is used by the designer.)
        /// </summary>
        public AudioBlock()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.node = null;
            this.selection = null;
            this.baseSize = Size;
            this.zoom = 1.0;
            this.audioScale = AUDIO_SCALE;
            this.cursorBar.BaseHeight = this.cursorBar.Height;
        }

        /// <summary>
        /// Create an audio block with an existing node.
        /// </summary>
        public AudioBlock(AudioNode node): this()
        {
            this.node = node;
            this.node.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(node_changed);
            SetAudio(node.Audio);
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
        /// Update colors for this block and its children.
        /// </summary>
        public ColorSettings Colors
        {
            get { return ((Track)Parent.Parent).Colors; }
            set
            {
                BackColor = Color.Orange; // value.AudioBlockBackColor;
                cursorBar.BackColor = value.AudioBlockBackColor;
                waveformCanvas.BackColor = value.AudioBlockBackColor;
            }
        }

        /// <summary>
        /// Get the audio node for this block.
        /// </summary>
        public AudioNode Node { get { return this.node; } }

        public void SelectAtX(int x)
        {
            ProjectView view = Parent != null && Parent.Parent is Track ? ((Track)Parent.Parent).View : null;
            if (view != null) Selection = new AudioSelection(view, this.node, TimeForX(x));
        }

        /// <summary>
        /// Selection (may be the block or a range.)
        /// </summary>
        public Selection Selection
        {
            get { return this.selection; }
            set
            {
                this.selection = value;
                if (Parent is Track) Colors = ((Track)Parent).Colors;
                this.cursorBar.Invalidate();
                this.waveformCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Get the time of a position.
        /// </summary>
        public double TimeForX(int x)
        {
            return x * this.node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() / this.waveformCanvas.Width;
        }

        /// <summary>
        /// Get the position of a time.
        /// </summary>
        public int XForTime(double time)
        {
            return (int)Math.Round(time / this.node.Audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() * this.waveformCanvas.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        public double Zoom
        {
            set
            {
                this.zoom = value;
                SuspendLayout();
                this.cursorBar.Zoom = this.zoom;
                Height = (int)Math.Round(baseSize.Height * this.zoom);
                this.waveformCanvas.Location = new Point(0, this.cursorBar.Height);
                this.waveformCanvas.Height = Height - this.cursorBar.Height;
                SetAudio(node.Audio);
                ResumeLayout();
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
                this.cursorBar.Invalidate();
                if (audio != null && Parent != null && Parent.Parent is Track) ((Track)Parent.Parent).UpdateSize();
            }
        }

        // Update the audio for the new node.
        private void node_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.media.MediaEventArgs)
            {
                if (this.selection != null)
                {
                    this.selection.Deselect();
                    this.selection = null;
                }
                if (((urakawa.events.media.MediaEventArgs)e).SourceMedia is urakawa.media.data.audio.ManagedAudioMedia)
                {
                    SetAudio(((urakawa.media.data.audio.ManagedAudioMedia)((urakawa.events.media.MediaEventArgs)e).SourceMedia).getMediaData());
                }
            }
        }

        private int WidthForAudio(urakawa.media.data.audio.AudioMediaData audio)
        {
            return (int)Math.Round(this.zoom * (audio == null ? this.baseSize.Width :
                this.audioScale * audio.getAudioDuration().getTimeDeltaAsMillisecondFloat()));
        }
    }
}
