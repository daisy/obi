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
        private bool playing;         // playing flag
        private double playingTime;   // playing position

        private static readonly float AUDIO_SCALE = 0.01f;  // scale of audio

        private delegate void SetAudioDelegate(urakawa.media.data.audio.AudioMediaData audio);  // delegate for Invoke()


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
            this.playing = false;
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
                cursorBar.BackColor = this.selection is NodeSelection ? value.AudioBlockSelectedBackColor : value.AudioBlockBackColor;
                waveformCanvas.BackColor = value.AudioBlockBackColor;
            }
        }

        /// <summary>
        /// Get the audio node for this block.
        /// </summary>
        public AudioNode Node { get { return this.node; } }

        public bool Playing
        {
            get { return this.playing; }
            set
            {
                this.playing = value;
                cursorBar.Invalidate();
                waveformCanvas.Invalidate();
            }
        }

        public double PlayingTime
        {
            get { return this.playingTime; }
            set
            {
                this.playing = true;
                this.playingTime = value;
                this.cursorBar.Invalidate();
                this.waveformCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Select the block from below (e.g. the cursor bar.)
        /// </summary>
        public void SelectFromBelow()
        {
            Track track = Parent != null && Parent.Parent is Track ? (Track)Parent.Parent : null;
            if (track.View != null)
            {
                Selection = new NodeSelection(track.View, this.node);
                track.SelectFromBelow(this.selection);
            }
        }

        public void SelectFromXFromBelow(int x)
        {
            Track track = Parent != null && Parent.Parent is Track ? (Track)Parent.Parent : null;
            if (track.View != null)
            {
                Selection = new AudioSelection(track.View, this.node, TimeForX(x));
                track.SelectFromBelow(this.selection);
            }
        }

        public void SelectToXFromBelow(int x)
        {
            Track track = Parent != null && Parent.Parent is Track ? (Track)Parent.Parent : null;
            if (track.View != null)
            {
                double to = TimeForX(x);
                if (this.selection is AudioSelection)
                {
                    double from = ((AudioSelection)this.selection).From;
                    if (from != to)
                    {
                        Selection = new AudioSelection(track.View, this.node, from, to);
                        track.SelectFromBelow(this.selection);
                    }
                }
            }
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
                if (Parent != null && Parent.Parent is Track) Colors = ((Track)Parent.Parent).Colors;
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
        /// Set the zoom factor for this block.
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


        // Update the display when the node has new audio.
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
                    this.selection.Deselect(null);
                    this.selection = null;
                }
                if (((urakawa.events.media.MediaEventArgs)e).SourceMedia is urakawa.media.data.audio.ManagedAudioMedia)
                {
                    SetAudio(((urakawa.media.data.audio.ManagedAudioMedia)((urakawa.events.media.MediaEventArgs)e).SourceMedia).getMediaData());
                }
            }
        }

        // Compute the necessary width for the amount of audio at the current audio scale.
        private int WidthForAudio(urakawa.media.data.audio.AudioMediaData audio)
        {
            return (int)Math.Round(this.zoom * (audio == null ? this.baseSize.Width :
                this.audioScale * audio.getAudioDuration().getTimeDeltaAsMillisecondFloat()));
        }
    }
}
