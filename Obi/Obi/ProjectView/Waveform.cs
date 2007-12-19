using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using urakawa.media.data.audio;

namespace Obi.ProjectView
{
    public partial class Waveform : Control
    {
        private AudioMediaData mAudio;         // audio data to draw
        private Bitmap mBitmap;                // cached bitmap of the waveform
        private AudioRange mSelection;  // selection in the waveform

        private static readonly Pen Channel1Pen = new Pen(Color.FromArgb(128, 0, 0, 255));
        private static readonly Pen Channel2Pen = new Pen(Color.FromArgb(128, 255, 0, 255));
        private static readonly Pen CursorPen = new Pen(Color.FromArgb(128, 0, 255, 255));
        private static readonly SolidBrush SelectionBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));


        /// <summary>
        /// Create a waveform with no data to display yet.
        /// </summary>
        public Waveform()
        {
            InitializeComponent();
            DoubleBuffered = true;
            mAudio = null;
            mBitmap = null;
            mSelection = null;
        }

        /// <summary>
        /// Set the audio data to be displayed
        /// </summary>
        public AudioMediaData Media
        {
            set
            {
                mAudio = value;
                UpdateWaveform();
            }
        }

        public AudioRange Selection
        {
            get { return mSelection; }
            set { mSelection = value; }
        }

        public void Deselect()
        {
            mSelection = null;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (mBitmap != null) pe.Graphics.DrawImage(mBitmap, new Point(0, 0));
            if (mSelection != null)
            {
                if (mSelection.HasCursor)
                {
                    pe.Graphics.DrawLine(CursorPen, new Point(CursorPosition, 0), new Point(CursorPosition, Height - 1));
                }
                else
                {
                    pe.Graphics.FillRectangle(SelectionBrush, InitialSelectionPosition, 0, FinalSelectionPosition - InitialSelectionPosition, Height);
                }
            }
            base.OnPaint(pe);
        }

        public double CursorTime
        {
            set
            {
                mSelection.CursorTime = value;
                Invalidate();
            }
        }

        public int CursorPosition
        {
            get { return XFromTime(mSelection.CursorTime); }
            set
            {
                mSelection = new AudioRange(TimeFromX(value));
                Invalidate();
            }
        }

        public int InitialSelectionPosition { get { return XFromTime(mSelection.SelectionBeginTime); } }

        public int FinalSelectionPosition
        {
            get { return XFromTime(mSelection.SelectionEndTime); }
            set
            {
                double start = mSelection.CursorTime;
                double end = TimeFromX(value);
                if (start == end)
                {
                    mSelection.HasCursor = true;
                }
                else
                {
                    mSelection.HasCursor = false;
                    mSelection.SelectionBeginTime = Math.Min(start, end);
                    mSelection.SelectionEndTime = Math.Max(start, end);
                }
                Invalidate();
            }
        }

        private int XFromTime(double time)
        {
            return (int)Math.Round(time / mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat() * Width);
        }

        private double TimeFromX(int x)
        {
            return x * mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat() / Width;
        }

        private void Waveform_SizeChanged(object sender, EventArgs e) { UpdateWaveform(); }

        // Update the waveform bitmap with the new size
        private void UpdateWaveform()
        {
            if (Width > 0 && Height > 0)
            {
                mBitmap = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(mBitmap);
                g.Clear(Color.White);
                g.DrawLine(Pens.BlueViolet, new Point(0, Height / 2), new Point(Width - 1, Height / 2));
                if (mAudio != null) DrawWaveform(g);
                Invalidate();
            }
        }

        // Draw the waveform in a graphics
        private void DrawWaveform(Graphics g)
        {
            PCMFormatInfo format = mAudio.getPCMFormat();
            if (format.getBitDepth() == 16)
            {
                ushort channels = format.getNumberOfChannels();
                ushort frameSize = format.getBlockAlign();
                int samplesPerPixel = (int)Math.Ceiling(mAudio.getPCMLength() / (float)frameSize / Width);
                int bytesPerPixel = samplesPerPixel * frameSize / channels;
                byte[] bytes = new byte[bytesPerPixel];
                short[] samples = new short[samplesPerPixel];
                System.IO.Stream audio = mAudio.getAudioData();
                for (int x = 0; x < Width; ++x)
                {
                    int read = audio.Read(bytes, 0, bytesPerPixel);
                    Buffer.BlockCopy(bytes, 0, samples, 0, read);
                    short min = short.MaxValue;
                    short max = short.MinValue;
                    for (int i = 0; i < (int)Math.Ceiling(read / (float)frameSize); i += channels)
                    {
                        if (samples[i] < min) min = samples[i];
                        if (samples[i] > max) max = samples[i];
                    }
                    int ymin = Height - (int)Math.Round(((min - short.MinValue) * Height) / (float)ushort.MaxValue);
                    int ymax = Height - (int)Math.Round(((max - short.MinValue) * Height) / (float)ushort.MaxValue);
                    g.DrawLine(Channel1Pen, new Point(x, ymin), new Point(x, ymax));
                    if (channels == 2)
                    {
                        min = short.MaxValue;
                        max = short.MinValue;
                        for (int i = 1; i < (int)Math.Ceiling(read / (float)frameSize); i += channels)
                        {
                            if (samples[i] < min) min = samples[i];
                            if (samples[i] > max) max = samples[i];
                        }
                        ymin = Height - (int)Math.Round(((min - short.MinValue) * Height) / (float)ushort.MaxValue);
                        ymax = Height - (int)Math.Round(((max - short.MinValue) * Height) / (float)ushort.MaxValue);
                        g.DrawLine(Channel2Pen, new Point(x, ymin), new Point(x, ymax));
                    }
                }
            }
        }
    }
}