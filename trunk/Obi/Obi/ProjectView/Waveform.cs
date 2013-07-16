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
        private Bitmap mBitmap;              // cached bitmap of the waveform
        private Bitmap mBitmap_Highlighted;  // cached bitmap of the waveform (highlighted)
        private AudioBlock mBlock;           // parent audio block
        private AudioRange mCursor;          // playback cursor (can be different from cursor)
        private bool mNeedsRendering;        // needs to render the waveform (when size, color or data changes.)
        private AudioRange mSelection;       // selection in the waveform
        private bool m_CancelRendering;
        private bool m_IsRenderingWaveform;
        private ToolTip newTooltip = new ToolTip();

        private System.Threading.Mutex mPaintMutex ; // for forcing mutual exclusion in on paint event

        /// <summary>
        /// Create a waveform with no data to display yet.
        /// </summary>
        public Waveform()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(Waveform_Disposed);
            DoubleBuffered = true;
            mBitmap = null;
            mBitmap_Highlighted = null;
            mBlock = null;
            mSelection = null;
            mNeedsRendering = false;
            mPaintMutex = new System.Threading.Mutex ();
        }


        // Audio media data of the parent block.
        private ManagedAudioMedia Audio { get { return ((PhraseNode)mBlock.Node).Audio; } }

        // Actual audio media data of the parent block.
        private AudioMediaData Media { get { return Audio.AudioMediaData; } }

        /// <summary>
        /// Set the parent audio block.
        /// </summary>
        public AudioBlock Block
        {
            set
            {
                //if (mBlock != value)
                {
                    if (mBlock != null)
                    {
                        //Audio.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Audio_changed);
                        
                    }
                    mBlock = value;
                    if (mBlock != null)
                    {
                        //
                        
                        RequestRendering();
                    }
                }
            }
        }

        // Request a new rendering when audio has changed.
        //private void Audio_changed(object sender, urakawa.events.DataModelChangedEventArgs e) { RequestRendering(); }
        //private void Audio_changed(object sender, NodeEventArgs<PhraseNode> e) { RequestRendering(); }

        public bool CancelRendering
        {
            get { return m_CancelRendering; }
            set { m_CancelRendering = value; }
        }

        public bool IsRenderingWaveform { get { return m_IsRenderingWaveform; } }

        private int Width_Expected
        {
            get
            {
                if (mBlock == null) return Width;
                if (Width == mBlock.MaxWaveformWidth) return mBlock.ComputeWaveformDefaultWidth();

                return Width;
            }
        }

        /// <summary>
        /// Render the waveform graphically then display it. Return the background worker doing the job.
        /// </summary>
        public BackgroundWorker Render()
        {
            if (mBlock != null &&  mBlock.Node != null && mBlock.Node.IsRooted)
            {
                if (mNeedsRendering)
                {
                    mNeedsRendering = false;
                    if (mBlock != null && Width > 0 && Height > 0)
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = false;
                        worker.WorkerSupportsCancellation = false;
                        worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                        {
                            m_IsRenderingWaveform = true;
                            ColorSettings settings = mBlock.ColorSettings;

                            if (mBlock.Strip.ContentView.Settings != null
                                    && mBlock.Strip.ContentView.Settings.OptimizeMemory)
                            {
                            System.Diagnostics.PerformanceCounter ramPerformanceCounter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
                            if (ramPerformanceCounter.NextValue() < 100)
                            {
                                //Console.WriteLine("RAM near overload " + ramPerformanceCounter.NextValue().ToString());

                                
                                    System.GC.GetTotalMemory(true);

                                    System.GC.WaitForFullGCComplete(500);

                                    float availableRAM = ramPerformanceCounter.NextValue();
                                    Console.WriteLine("RAM after collection " + availableRAM.ToString());
                                }
                                ramPerformanceCounter.Close();
                            }
                            m_CancelRendering = false;
                            mBitmap = CreateBitmap(mBlock.ColorSettings, false);
                            if (mBitmap == null)
                            {
                                System.Threading.Thread.Sleep(1000);
                                mBitmap = CreateBitmap(mBlock.ColorSettings, false);
                            }
                            if (mBitmap != null) mBitmap_Highlighted = CreateBitmap(mBlock.ColorSettings, true);
                            m_IsRenderingWaveform = false;
                        });
                        worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate(object sender, RunWorkerCompletedEventArgs e)
                        {
                            Invalidate();
                            mBlock.ContentView.FinishedRendering(this, mBitmap != null && mBitmap_Highlighted != null);
                        });
                        worker.RunWorkerAsync();
                        return worker;
                    }
                }
                return null;
            }
            else
            {
                if (mBitmap != null)
                {
                    mBitmap.Dispose();
                    mBitmap = null;
                }
                if (mBitmap_Highlighted  != null)
                {
                    mBitmap_Highlighted.Dispose();
                    mBitmap_Highlighted = null;
                }
                return null;
            }
        }


        // Clear bitmaps before redrawing.
        private void ClearBitmaps()
        {
            if (mBitmap != null)
            {
                mBitmap.Dispose();
                mBitmap = null;
            }
            if (ColorSettings == null)
            {
                mBitmap = null;
                mBitmap_Highlighted = null;
                Invalidate();
                return;
            }
            mBitmap = CreateBaseBitmap(ColorSettings, false);
            mBitmap_Highlighted = CreateBaseBitmap(ColorSettings, true);
            // Invalidate to immediately show the empty bitmaps
            Invalidate();
        }

        private Bitmap CreateBaseBitmap(ColorSettings settings, bool highlighted)
        {
            Bitmap bitmap = null;
            try
            {
                bitmap = new Bitmap(Width, Height);
                if (bitmap == null) return null;
                Graphics g = Graphics.FromImage(bitmap);
                g.Clear(highlighted ? settings.WaveformHighlightedBackColor : settings.WaveformBackColor);
                Pen linePen = (Pen) (highlighted ? (Pen)settings.WaveformHighlightedPen.Clone() : (Pen)settings.WaveformBaseLinePen.Clone() ) ;
                g.DrawLine(linePen,
                    new Point(0, Height / 2), new Point(Width - 1, Height / 2));
                linePen.Dispose();
                g.Dispose();
            }
            catch (Exception ex)
            { Console.WriteLine("Waveform exception : " + ex); }
            return bitmap;
        }

        // Create the bitmap for the waveform given the current color settings.
        // Use the highlight colors if the highlight flag is set, otherwise regular colors.
        private Bitmap CreateBitmap(ColorSettings settings, bool highlighted)
        {
            Bitmap bitmap = CreateBaseBitmap(settings, highlighted);
            if (bitmap == null) return null;
            try
            {
                Graphics g = Graphics.FromImage(bitmap);
                if(!m_CancelRendering && Audio != null) DrawWaveform(g, settings, highlighted);
                g.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Waveform exception : " + ex);}
                
            return bitmap;
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
            if (!Media.HasActualPcmData || Media.AudioDuration.AsLocalUnits <= 0)
            {
                return;
            }

            AudioLib.AudioLibPCMFormat format = Media.PCMFormat.Data;
            if (format.BitDepth == 16)
            {
                ushort channels = format.NumberOfChannels;
                ushort frameSize = format.BlockAlign;
                long pcmLength = Media.PCMFormat.Data.ConvertTimeToBytes(Media.AudioDuration.AsLocalUnits);
                int samplesPerPixel = (int)Math.Ceiling(pcmLength / (float)frameSize / Width_Expected * channels);
                int bytesPerPixel = samplesPerPixel * frameSize / channels;
                byte[] bytes = new byte[bytesPerPixel];
                short[] samples = new short[samplesPerPixel];
                System.IO.Stream au = Media.OpenPcmInputStream();
                try
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        if (m_CancelRendering) break;
                        int read = au.Read(bytes, 0, bytesPerPixel);
                        Buffer.BlockCopy(bytes, 0, samples, 0, read);
                        Pen channel1Pen = highlighted ? (Pen)settings.WaveformHighlightedPen.Clone() :
                            channels == 1 ? (Pen)settings.WaveformMonoPen.Clone() : (Pen)settings.WaveformChannel1Pen.Clone();
                        DrawChannel(g, channel1Pen,
                            samples, x, read, frameSize, 0, channels);
                        channel1Pen.Dispose();
                        if (channels == 2)
                        {
                            Pen channel2Pen = highlighted ? (Pen)settings.WaveformHighlightedPen.Clone() : (Pen)settings.WaveformChannel2Pen.Clone();
                            DrawChannel(g,
                            channel2Pen,
                            samples, x, read, frameSize, 1, channels);
                            channel2Pen.Dispose();
                        }

                    }
                    if (au != null) au.Close();
                    au = null;

                    if (mBlock.MaxWaveformWidth == Width && Width < Width_Expected)
                    {
                        DrawWaveformBreakMark(g, settings);
                    }
                }
                finally
                {
                    if (au != null) au.Close();
                    au = null;
                }
            }
        }

        public void DrawWaveformBreakMark(Graphics g, ColorSettings settings)
        {
            if (mBlock == null || Width < mBlock.MaxWaveformWidth) return;

            float percent = (8 * 100) / 200;
            int curveWidth = (int)((percent * Height) / 100);
            int width = Width - curveWidth;

                Pen penCurrent = new Pen(Color.Blue);
                penCurrent.Width = 2.0F;
                Point[] pt = { new Point(width - curveWidth,0), new Point(width - (2 * curveWidth), Height / 4), new Point(width - curveWidth, Height / 2), new Point(width - curveWidth, Height / 2), new Point(width, (3 * Height) / 4), new Point(width - curveWidth, Height) };
                g.DrawCurve(penCurrent, pt);
            
        }

        // Repaint the waveform bitmap.
        protected override void OnPaint(PaintEventArgs pe)
        {
            ColorSettings settings = ColorSettings;
            if (settings != null && mBlock != null)
            {
            mPaintMutex.WaitOne ();
                if (mBitmap != null)
                {
                    pe.Graphics.DrawImage(mBitmap, new Point(0, 0));
                }
                else
                {
                    pe.Graphics.DrawString(Localizer.Message("rendering_waveform"), mBlock.Font,
                        mBlock.ColorSettings.WaveformTextBrush, new PointF(0.0f, 0.0f));
                }
                int w = Height / 10;
                if (mSelection != null)
                {
                    if (CheckCursor)
                    {
                        pe.Graphics.DrawLine(settings.WaveformSelectionPen,
                            new Point(SelectionPointPosition, 0), new Point(SelectionPointPosition, Height - 1));
                        pe.Graphics.FillRectangle(settings.WaveformSelectionBrush,
                            new Rectangle(SelectionPointPosition - w / 2, 0, w, 2 * w));
                    }
                    else if (CheckRange)
                    {
                        if (mBitmap_Highlighted != null)
                        {
                            pe.Graphics.DrawImage(mBitmap_Highlighted, InitialSelectionPosition, 0,
                                new Rectangle(InitialSelectionPosition, 0, FinalSelectionPosition - InitialSelectionPosition,
                                Height), GraphicsUnit.Pixel);
                        }
                        else
                        {
                            pe.Graphics.FillRectangle(settings.WaveformSelectionBrush,
                                InitialSelectionPosition, 0, FinalSelectionPosition - InitialSelectionPosition, Height);
                        }
                    }
                }
                if (mCursor != null)
                {
                    pe.Graphics.DrawLine(settings.WaveformCursorPen,
                        new Point(CursorPosition, 0), new Point(CursorPosition, Height - 1));
                    Point[] points = new Point[3];
                    points[0] = new Point(CursorPosition, 0);
                    points[1] = new Point(CursorPosition + w, w);
                    points[2] = new Point(CursorPosition, 2 * w);
                    pe.Graphics.FillPolygon(settings.WaveformCursorBrush, points);
                }
            mPaintMutex.ReleaseMutex ();
            }
            base.OnPaint(pe);
        }

        // Add self to the content view rendering list.
        private void RequestRendering()
        {
            if (mBlock != null)
            {
                mNeedsRendering = true;
                ClearBitmaps();
                mBlock.ContentView.RenderWaveform(this, AudioBlock.NORMAL_PRIORITY);
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
                    if (mNeedsRendering) mBlock.PrioritizeRendering(AudioBlock.WAVEFORM_SELECTED_PRIORITY); 
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Set the cursor time and return the position inside the waveform.
        /// </summary>
        public int SetCursorTime(double time)
        {
            mCursor.CursorTime = time;
            Invalidate();
            return XFromTime(time);
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
        public void InitCursor(double time)
        {
            mCursor = new AudioRange(time);
            if (mNeedsRendering) mBlock.PrioritizeRendering(AudioBlock.WAVEFORM_SELECTED_PRIORITY);
        }

        /// <summary>
        /// Get or set the final position of the selection (in pixels.)
        /// Ignored if the transport bar is active.
        /// </summary>
        public int FinalSelectionPosition
        {
            get { return mSelection != null ? XFromTime(mSelection.SelectionEndTime) : 0; }
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
        public int CursorPosition
        {
            get { return mCursor != null ? XFromTime(mCursor.CursorTime) : -1; }
        }



        private bool CheckCursor
        {
            get
            {
                return mSelection.HasCursor &&
                    mSelection.CursorTime >= 0.0 && mSelection.CursorTime <= Media.AudioDuration.AsMilliseconds;
            }
        }

        private bool CheckRange
        {
            get
            {
                double d = Media.AudioDuration.AsMilliseconds;
                return !mSelection.HasCursor &&
                    mSelection.SelectionBeginTime >= 0.0 && mSelection.SelectionBeginTime <= d &&
                    mSelection.SelectionEndTime >= 0.0 && mSelection.SelectionEndTime <= d &&
                    mSelection.SelectionBeginTime < mSelection.SelectionEndTime;
            }
        }

        // Convert a pixel position into a time (in ms.)
        private double TimeFromX(int x)
        {
            return x * Media.AudioDuration.AsMilliseconds / Width_Expected;
        }

        // Convert a time (in ms) to a pixel position.
        private int XFromTime(double time)
        {
            return (int)Math.Round(time / Media.AudioDuration.AsMilliseconds * Width_Expected);
        }

        private void Waveform_Disposed(object sender, EventArgs e)
        {
            //if (Audio != null ) Audio.Changed -= new EventHandler<urakawa.events.DataModelChangedEventArgs>(Audio_changed);
            
            if (mBitmap != null) mBitmap.Dispose();
            mBitmap = null;
            if (mBitmap_Highlighted != null) mBitmap_Highlighted.Dispose();
            mBitmap_Highlighted = null;

        }

        private void Waveform_MouseHover(object sender, EventArgs e)
        {            
            if (Width < mBlock.MaxWaveformWidth)
                return;
            Point cursorPos = this.PointToClient(Cursor.Position);            
            if (cursorPos.X < this.Width - 100)
            {
                if (newTooltip.GetToolTip(this) == "")
                   return;                
                newTooltip.SetToolTip(this, "");
                return;
            }
            int time = (int)TimeFromX(mBlock.MaxWaveformWidth) / 1000;
            if (cursorPos.X <= this.Width && cursorPos.X >= this.Width - 100)
                newTooltip.SetToolTip(this, String.Format(Localizer.Message("waveform_is_truncated"), time));
        }



    }
}