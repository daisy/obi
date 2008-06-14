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


        public AudioBlock()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.selected = false;
            this.node = null;
            this.baseSize = Size;
            this.zoom = 1.0;
            this.audioScale = 1.0;
        }

        public AudioBlock(AudioNode node): this()
        {
            this.node = node;
            this.node.changed += new EventHandler<urakawa.events.DataModelChangedEventArgs>(node_changed);
            this.waveformCanvas.Audio = node.Audio;
            this.selected = false;
        }

        private void node_changed(object sender, urakawa.events.DataModelChangedEventArgs e)
        {
            if (e is urakawa.events.media.MediaEventArgs &&
                ((urakawa.events.media.MediaEventArgs)e).SourceMedia is urakawa.media.data.audio.ManagedAudioMedia)
            {
                this.waveformCanvas.Audio = 
                    ((urakawa.media.data.audio.ManagedAudioMedia)((urakawa.events.media.MediaEventArgs)e).SourceMedia).getMediaData();
            }
        }


        /// <summary>
        /// Update colors for this track and its children.
        /// </summary>
        public ColorSettings Colors
        {
            set
            {
                BackColor = value.AudioBlockBackColor;
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
        public void Zoom(int height)
        {
            height -= (Margin.Top + Margin.Bottom);
            this.zoom = (double)height / baseSize.Height;
            Size = new Size((int)Math.Round(this.zoom * baseSize.Width), height);
        }
    }
}
