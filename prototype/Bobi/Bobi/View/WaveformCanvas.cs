using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using urakawa.media.data.audio;

namespace Bobi.View
{
    public partial class WaveformCanvas : Control
    {
        private AudioMediaData audio;  // audio data to draw
        private Bitmap bitmap;         // cached bitmap of the waveform

        // Pens and brushes should be owned by someone above
        private static readonly Pen Channel1Pen = new Pen(Color.FromArgb(128, 0, 0, 255));
        private static readonly Pen Channel2Pen = new Pen(Color.FromArgb(128, 255, 0, 255));
        private static readonly Pen SelectionPen = new Pen(Color.FromArgb(128, 0, 255, 255));
        private static readonly SolidBrush SelectionBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));
        private static readonly Pen CursorPen = new Pen(Color.FromArgb(128, 255, 128, 128));


        /// <summary>
        /// Create a waveform with no data to display yet.
        /// </summary>
        public WaveformCanvas()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.audio = null;
            this.bitmap = null;
        }

        /// <summary>
        /// Set the audio data to be displayed
        /// </summary>
        public AudioMediaData Audio
        {
            set
            {
                this.audio = value;
                UpdateWaveform();
            }
        }



        // Draw the waveform in a graphics
        // TODO: handle other bit depths than 16 bit.
        private void DrawWaveform(Graphics g)
        {
            PCMFormatInfo format = this.audio.getPCMFormat();
            if (format.getBitDepth() != 16) throw new Exception("Cannot deal with bitdepth others than 16.");
            ushort channels = format.getNumberOfChannels();
            ushort frameSize = format.getBlockAlign();
            int samplesPerPixel = (int)Math.Ceiling(this.audio.getPCMLength() / (float)frameSize / Width * channels);
            int bytesPerPixel = samplesPerPixel * frameSize / channels;
            byte[] bytes = new byte[bytesPerPixel];
            short[] samples = new short[samplesPerPixel];
            System.IO.Stream au = this.audio.getAudioData();
            for (int x = 0; x < Width; ++x)
            {
                int read = au.Read(bytes, 0, bytesPerPixel);
                Buffer.BlockCopy(bytes, 0, samples, 0, read);
                DrawChannel(g, Channel1Pen, samples, x, read, frameSize, 0, channels);
                if (channels == 2) DrawChannel(g, Channel2Pen, samples, x, read, frameSize, 1, channels);
            }
            au.Close();
        }

        // Draw one channel for a given x
        private void DrawChannel(Graphics g, Pen pen, short[] samples, int x, int read, int frameSize, int channel, int channels)
        {
            short min = short.MaxValue;
            short max = short.MinValue;
            for (int i = channel; i < (int)Math.Ceiling(read / (float)frameSize); i += channels)
            {
                if (samples[i] < min) min = samples[i];
                if (samples[i] > max) max = samples[i];
            }
            g.DrawLine(pen, new Point(x, Height - (int)Math.Round(((min - short.MinValue) * Height) / (float)ushort.MaxValue)),
                new Point(x, Height - (int)Math.Round(((max - short.MinValue) * Height) / (float)ushort.MaxValue)));
        }

        // Repaint the waveform bitmap.
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (this.bitmap != null) pe.Graphics.DrawImage(this.bitmap, new Point(0, 0));
            AudioBlock block = Parent as AudioBlock;
            if (block != null)
            {
                AudioSelection selection = block.Selection as AudioSelection;
                if (selection != null)
                {
                    int from = block.XForTime(selection.From);
                    if (selection.IsRange)
                    {
                        int to = block.XForTime(selection.To);
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush,
                            new Rectangle(from < to ? from : to, 0, from < to ? to - from : from - to, Height));
                        pe.Graphics.DrawLine(block.Colors.AudioSelectionPen, new Point(to, 0), new Point(to, Height - 1));
                    }
                    pe.Graphics.DrawLine(block.Colors.AudioSelectionPen, new Point(from, 0), new Point(from, Height - 1));
                }
                if (block.Playing)
                {
                    int at = block.XForTime(block.PlayingTime);
                    pe.Graphics.DrawLine(block.Colors.AudioPlaybackPen, new Point(at, 0), new Point(at, Height - 1));
                }
            }
            base.OnPaint(pe);
        }

        // Redraw the waveform to fit the available size.
        private void UpdateWaveform()
        {
            if (Width > 0 && Height > 0)
            {
                this.bitmap = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(this.bitmap);
                g.Clear(BackColor);
                g.DrawLine(Pens.BlueViolet, new Point(0, Height / 2), new Point(Width - 1, Height / 2));
                if (this.audio != null) DrawWaveform(g);
                Invalidate();
            }
        }

        private void Waveform_SizeChanged(object sender, EventArgs e) { UpdateWaveform(); }

        private void WaveformCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Parent is AudioBlock) ((AudioBlock)Parent).SelectFromXFromBelow(e.X);
        }

        private void WaveformCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Parent is AudioBlock) ((AudioBlock)Parent).SelectToXFromBelow(e.X);
        }

        private void WaveformCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) if (Parent is AudioBlock) ((AudioBlock)Parent).SelectToXFromBelow(e.X);
        }
    }
}