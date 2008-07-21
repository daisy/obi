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
        private AudioMediaData mAudio;       // audio data to draw
        private Bitmap mBitmap;              // cached bitmap of the waveform
        private Bitmap mBitmap_Highlighted;  // cached bitmap of the waveform (highlighted)
        private AudioRange mSelection;       // selection in the waveform
        private AudioRange mCursor;          // playback cursor (can be different from cursor)


        /// <summary>
        /// Create a waveform with no data to display yet.
        /// </summary>
        public Waveform()
        {
            InitializeComponent();
            DoubleBuffered = true;
            mAudio = null;
            mBitmap = null;
            mBitmap_Highlighted = null;
            mSelection = null;
            mCursor = null;
        }


        /// <summary>
        /// Clear bitmaps when redrawing.
        /// </summary>
        private void ClearBitmaps()
        {
            mBitmap = null;
            mBitmap_Highlighted = null;
            Invalidate();
        }

        /// <summary>
        /// Set the audio data to be displayed
        /// </summary>
        public AudioMediaData Media
        {
            set
            {
                if (mAudio != value)
                {
                    mAudio = value;
                    if (mAudio != null) RequestRendering();
                }
            }
        }

        /// <summary>
        /// Render the waveform graphically then display it. Return the background worker doing the job.
        /// </summary>
        public BackgroundWorker Render()
        {
            AudioBlock block = Parent as AudioBlock;
            if (block != null && Width > 0 && Height > 0)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.WorkerReportsProgress = false;
                worker.WorkerSupportsCancellation = true;
                worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                {
                    ColorSettings settings = block.ColorSettings;
                    if (!worker.CancellationPending) mBitmap = CreateBitmapHighlighted(block.ColorSettings, false);
                    if (!worker.CancellationPending) mBitmap_Highlighted = CreateBitmapHighlighted(block.ColorSettings, true);
                });
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs e)
                {
                    if (e.Cancelled)
                    {
                        mBitmap = null;
                        mBitmap_Highlighted = null;
                    }
                    else
                    {
                        block.ContentView.FinishedRendering();
                    }
                    Invalidate();
                });
                worker.RunWorkerAsync();
                return worker;
            }
            return null;
        }


        private delegate Bitmap CreateBitmapDelegate(ColorSettings settings, bool highlighted);

        // Create the bitmap for the waveform given the current color settings.
        // Use the highlight colors if the highlight flag is set, otherwise regular colors.
        private Bitmap CreateBitmapHighlighted(ColorSettings settings, bool highlighted)
        {
            //if (InvokeRequired)
            //{
            //    return (Bitmap)Invoke(new CreateBitmapDelegate(CreateBitmapHighlighted), new Object[] { settings, highlighted });
            //}
            //else
            //{
                Bitmap bitmap = new Bitmap(Width, Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(highlighted ? settings.WaveformHighlightedBackColor : settings.WaveformBackColor);
                g.DrawLine(highlighted ? settings.WaveformHighlightedPen : settings.WaveformBaseLinePen,
                    new Point(0, Height / 2), new Point(Width - 1, Height / 2));
                if (mAudio != null) DrawWaveform(g, settings, highlighted);
                return bitmap;
            //}
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

        // Draw the waveform in a given graphics. Use the color settings and draw a highlighted or lowlighted
        // version depending on the highlighted flag. Do nothing for bit depths different from 16.
        private void DrawWaveform(Graphics g, ColorSettings settings, bool highlighted)
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
                System.IO.Stream au = mAudio.getAudioData();
                for (int x = 0; x < Width; ++x)
                {
                    int read = au.Read(bytes, 0, bytesPerPixel);
                    Buffer.BlockCopy(bytes, 0, samples, 0, read);
                    DrawChannel(g, highlighted ? settings.WaveformHighlightedPen :
                        channels == 1 ? settings.WaveformMonoPen : settings.WaveformChannel1Pen,
                        samples, x, read, frameSize, 0, channels);
                    if (channels == 2) DrawChannel(g,
                        highlighted ? settings.WaveformHighlightedPen : settings.WaveformChannel2Pen,
                        samples, x, read, frameSize, 1, channels);
                }
                au.Close();
            }
        }

        // Repaint the waveform bitmap.
        protected override void OnPaint(PaintEventArgs pe)
        {
            ColorSettings settings = ColorSettings;
            if (settings != null)
            {
                AudioBlock block = Parent as AudioBlock;
                if (block != null)
                {
                    if (block.Highlighted)
                    {
                        if (mBitmap_Highlighted != null)
                        {
                            pe.Graphics.DrawImage(mBitmap_Highlighted, new Point(0, 0));
                        }
                        else
                        {
                            pe.Graphics.DrawString(Localizer.Message("rendering_waveform"), block.Font,
                                block.ColorSettings.WaveformHighlightedTextBrush, new PointF(0.0f, 0.0f));
                        }
                    }
                    else
                    {
                        if (mBitmap != null)
                        {
                            pe.Graphics.DrawImage(mBitmap, new Point(0, 0));
                        }
                        else
                        {
                            pe.Graphics.DrawString(Localizer.Message("rendering_waveform"), block.Font,
                                block.ColorSettings.WaveformTextBrush, new PointF(0.0f, 0.0f));
                        }
                    }
                    if (mSelection != null)
                    {
                        if (CheckCursor)
                        {
                            pe.Graphics.DrawLine(settings.WaveformSelectionPen,
                                new Point(SelectionPointPosition, 0), new Point(SelectionPointPosition, Height - 1));
                        }
                        else if (CheckRange)
                        {
                            pe.Graphics.FillRectangle(settings.WaveformSelectionBrush,
                                InitialSelectionPosition, 0, FinalSelectionPosition - InitialSelectionPosition, Height);
                        }
                    }
                    if (mCursor != null)
                    {
                        pe.Graphics.DrawLine(settings.WaveformCursorPen,
                            new Point(CursorPosition, 0), new Point(CursorPosition, Height - 1));
                        int w = block.Margin.Right;
                        Point[] points = new Point[3];
                        points[0] = new Point(CursorPosition, Height / 2 - w);
                        points[1] = new Point(CursorPosition + w, Height / 2);
                        points[2] = new Point(CursorPosition, Height / 2 + w);
                        pe.Graphics.FillPolygon(settings.WaveformCursorBrush, points);
                    }
                }
                base.OnPaint(pe);
            }
        }

        // Add self to the content view rendering list.
        private void RequestRendering()
        {
            Block block = Parent as Block;
            if (block != null)
            {
                ClearBitmaps();
                block.ContentView.RenderWaveform(this);
            }
        }

        // Request a new rendering when the size has changed.
        private void Waveform_SizeChanged(object sender, EventArgs e) { RequestRendering(); }






        /// <summary>
        /// Selection point (not range) position (in pixels).
        /// </summary>
        public int SelectionPointPosition
        {
            get { return mSelection == null ? -1 : XFromTime(mSelection.CursorTime); }
            set
            {
                if (value >= 0)
                {
                    mSelection = new AudioRange(TimeFromX(value));
                    Invalidate();
                }
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
                if (mSelection != null)
                {

                    int x = value < 0 ? 0 : value > Width ? Width : value;
                    double end = TimeFromX(x);
                    double start = mSelection == null ? end : mSelection.CursorTime;
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
        }

        /// <summary>
        /// Get the initial position of the selection (in pixels.)
        /// </summary>
        public int InitialSelectionPosition { get { return XFromTime(mSelection.SelectionBeginTime); } }

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


        // Get the color settings from above
        private ColorSettings ColorSettings
        {
            get { return Parent is AudioBlock ? ((AudioBlock)Parent).ColorSettings : null; }
        }

        // Get the position in pixels of the cursor (during playback only.)
        private int CursorPosition
        {
            get { return XFromTime(mCursor.CursorTime); }
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

        // Convert a time (in ms) to a pixel position.
        private int XFromTime(double time)
        {
            return (int)Math.Round(time / mAudio.getAudioDuration().getTimeDeltaAsMillisecondFloat() * Width);
        }
    }
}