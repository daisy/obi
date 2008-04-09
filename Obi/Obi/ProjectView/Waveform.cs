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
        private AudioMediaData mAudio;  // audio data to draw
        private Bitmap mBitmap;         // cached bitmap of the waveform
        private AudioRange mSelection;  // selection in the waveform
        private AudioRange mCursor;     // playback cursor (can be different from cursor)

        // Pens and brushes should be owned by someone above
        private static readonly Pen Channel1Pen = new Pen(Color.FromArgb(128, 0, 0, 255));
        private static readonly Pen Channel2Pen = new Pen(Color.FromArgb(128, 255, 0, 255));
        private static readonly Pen SelectionPen = new Pen(Color.FromArgb(128, 0, 255, 255));
        private static readonly SolidBrush SelectionBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));
        private static readonly Pen CursorPen = new Pen(Color.FromArgb(128, 255, 128, 128));


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
            mCursor = null;
        }


        /// <summary>
        /// Selection point (not range) position (in pixels).
        /// </summary>
        public int SelectionPointPosition
        {
            get { return XFromTime(mSelection.CursorTime); }
            set
            {
                mSelection = new AudioRange(TimeFromX(value));
                Invalidate();
            }
        }

        /// <summary>
        /// Set the cursor time (during playback only.)
        /// </summary>
        public double CursorTime
        {
            set
            {
                mCursor.CursorTime = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Clear the current selection in the waveform.
        /// </summary>
        public void Deselect()
        {
            mSelection = null;
            Invalidate();
        }

        /// <summary>
        /// Playback cursor leaves the waveform.
        /// </summary>
        public void ClearCursor()
        {
            mCursor = null;
            Invalidate();
        }

        /// <summary>
        /// Create a new cursor when playback starts.
        /// </summary>
        public void InitCursor()
        {
            mCursor = new AudioRange(0.0);
        }

        /// <summary>
        /// Get or set the final position of the selection (in pixels.)
        /// Ignored if the transport bar is active.
        /// </summary>
        public int FinalSelectionPosition
        {
            get { return XFromTime(mSelection.SelectionEndTime); }
            set
            {
                int x = value < 0 ? 0 : value > Width ? Width : value;
                double start = mSelection.CursorTime;
                double end = TimeFromX(x);
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

        /// <summary>
        /// Get the initial position of the selection (in pixels.)
        /// </summary>
        public int InitialSelectionPosition { get { return XFromTime(mSelection.SelectionBeginTime); } }

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

        /// <summary>
        /// Get or set the audio range selection in the waveform.
        /// </summary>
        public AudioRange Selection
        {
            get { return mSelection; }
            set
            {
                mSelection = value;
                Invalidate();
            }
        }


        // Get the position in pixels of the cursor (during playback only.)
        private int CursorPosition
        {
            get { return XFromTime(mCursor.CursorTime); }
        }

        // Draw the waveform in a graphics
        // TODO: handle other bit depths than 16 bit.
        private void DrawWaveform(Graphics g)
        {
            PCMFormatInfo format = mAudio.getPCMFormat();
            if (format.getBitDepth() == 16)
            {
                ushort channels = format.getNumberOfChannels();
                ushort frameSize = format.getBlockAlign();
                int samplesPerPixel = (int)Math.Ceiling(mAudio.getPCMLength() / (float)frameSize / Width * channels);
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
                audio.Close();
            }
        }

        // Repaint the waveform bitmap.
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (mBitmap != null) pe.Graphics.DrawImage(mBitmap, new Point(0, 0));
            if (mSelection != null)
            {
                if (CheckCursor)
                {
                    pe.Graphics.DrawLine(SelectionPen, new Point(SelectionPointPosition, 0), new Point(SelectionPointPosition, Height - 1));
                }
                else if (CheckRange)
                {
                    pe.Graphics.FillRectangle(SelectionBrush, InitialSelectionPosition, 0, FinalSelectionPosition - InitialSelectionPosition, Height);
                }
            }
            if (mCursor != null)
            {
                pe.Graphics.DrawLine(CursorPen, new Point(CursorPosition, 0), new Point(CursorPosition, Height - 1));
            }
            base.OnPaint(pe);
        }

        private bool CheckCursor
        {
            get
            {
                return mSelection.HasCursor &&
                    mSelection.CursorTime >= 0.0 && mSelection.CursorTime <= mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat();
            }
        }

        private bool CheckRange
        {
            get
            {
                double d = mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat();
                return !mSelection.HasCursor &&
                    mSelection.SelectionBeginTime >= 0.0 && mSelection.SelectionBeginTime <= d &&
                    mSelection.SelectionEndTime >= 0.0 && mSelection.SelectionEndTime <= d &&
                    mSelection.SelectionBeginTime < mSelection.SelectionEndTime;
            }
        }

        // Convert a pixel position into a time (in ms.)
        private double TimeFromX(int x)
        {
            return x * mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat() / Width;
        }

        // Redraw the waveform to fit the available size.
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

        // Convert a time (in ms) to a pixel position.
        private int XFromTime(double time)
        {
            return (int)Math.Round(time / mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat() * Width);
        }

        private void Waveform_SizeChanged(object sender, EventArgs e) { UpdateWaveform(); }
    }
}