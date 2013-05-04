using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;
using AudioLib;


namespace Obi.ProjectView
{
    public partial class Waveform_Recording : UserControl
    {
        private ContentView m_ContentView = null;
        private ProjectView m_ProjectView = null;
        private float m_ZoomFactor;
        private AudioLib.VuMeter m_VUMeter;
        private System.Drawing.Graphics g;
        private Pen pen_ChannelMono;
        private Pen pen_Channel1;
        private Pen pen_Channel2;
        private Pen pen_HighContrastMono;
        private Pen pen_HighContrastChannel1;
        private Pen pen_HighContrastChannel2;
        private Pen pen_WaveformBaseLine;

        private Pen m_PenTimeGrid;
        private Pen m_PenPhrasePage;

        private Pen pen_HighlightedHighContrastMono;
        private Font myFont = new Font("Microsoft Sans Serif", 7);
        private EmptyNode m_ExistingPhrase = null;
        private int m_Counter = 0;
        private int m_LocalTime = 0;
        private ColorSettings m_ColorSettings;
        private ColorSettings m_ColorSettingsHC;
        private bool m_IsColorHighContrast = false;
        //  private Dictionary<int, short> m_PointMMinChannelMap = new Dictionary<int, short>();
        //  private Dictionary<int, short> m_PointMaxChannelMap = new Dictionary<int, short>();
        
        private List<int> listOfCurrentXLocation = new List<int>();
        private List<int> listOfCurrentMinChannel1 = new List<int>();
        private List<int> listOfCurrentMaxChannel1 = new List<int>();
        private List<int> listOfCurrentMinChannel2 = new List<int>();
        private List<int> listOfCurrentMaxChannel2 = new List<int>();
        private List<int> listOfSelctedPortion=new List<int>(); 
        private List<int> listOfEndSelection=new List<int>(); 
        private int m_X = 0;
        private int m_XCV = 0;
        private Dictionary<int, string> m_MainDictionary = new Dictionary<int, string>();
        private double timeOfAssetMilliseconds = 0;
        private bool m_IsMaximized = false;
        private bool m_IsToBeRepainted = false;
        private Dictionary<int, string> m_CurrentDictionary = new Dictionary<int, string>();
        private int m_OffsetLocation = 400;
        private int m_StaticRecordingLocation = 0;
        private int m_TotalPixelCount = 0;
        private int m_Pass = 0;
        private double m_InitialStaticTime = 0;
        private bool m_IsResizing = false;
        private int m_InitialOffsetTime = 0;
        private int m_InitialOffsetLocation = 0;
        private List<double> listOfPhrases = new List<double>();
        private double m_Time = 0;
        private int m_MouseLocX = 0;
        private int m_MouseButtonDownLoc = 0;
        private int m_MouseButtonUpLoc = 0;
        private bool m_IsPage = false;
        //     private int m_TempMouseBtnDownLoc = 0;
        private bool m_IsMouseBtnUp = false;
        private int m_TempMouseMoveLoc = 0;
        private int m_StartSelection = 0;
        private bool IsSelectionActive = false;
        private double m_NewPhraseTime = 0;
        private bool m_IsSelected = false;
        private double[] arrOfLocations = new double[2];
        List<double[]> listOfLocationArray = new List<double[]>();
        //   private int m_DeletedOffset = 0;
        private bool m_IsDeleted = false;
        private  const int m_TopMargin=30;
        private Dictionary<double, int> m_TimeToPixelMap = new Dictionary<double, int>(); // workaround to avoid minor mismatch in timing calculations
        private int m_OverlapPixelLength = 0;
        private int m_PixelOfRedLine = 0;//workaround to avoid pixels mismatch for deleting red line consistenly.
        private bool m_ResetCalled = false;
        private bool m_DeletedPartEnclosed = false;
        private int tempXLocation=0;
        private List<int> m_MouseMoveList = new List<int>();
        
        public Waveform_Recording()
        {
            InitializeComponent();
            m_IsColorHighContrast = SystemInformation.HighContrast;   
            m_ZoomFactor = 1.0f;
            this.Visible = false;
            this.Height = Convert.ToInt32(124 * m_ZoomFactor);
            g = this.CreateGraphics();
            if (m_ProjectView != null && m_ProjectView.TransportBar.RecordingPhrase != null)
                m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
            Location = new Point(-400, Location.Y);
            Size = new Size(10000, WaveformHeight);
            m_StaticRecordingLocation = -1;
            m_InitialOffsetTime = -1;
            m_MouseButtonUpLoc = 0;
            m_MouseButtonDownLoc = 0;
            m_PenTimeGrid = new Pen(m_IsColorHighContrast ? Color.LightGray : Color.Gray);
            m_PenPhrasePage = new Pen(m_IsColorHighContrast? Color.White:Color.Black);
            m_PenPhrasePage.Width = 2;
            m_OverlapPixelLength = 0;
        }

        private RecordingSession m_RecordingSession;
        public RecordingSession RecordingSession
        {
            set
            {
                if (value != null
                    && (m_RecordingSession == null || m_RecordingSession != value))
                {
                    m_RecordingSession = value;
                    m_RecordingSession.FinishingPhrase += new Obi.Events.Audio.Recorder.FinishingPhraseHandler(m_RecordingSession_FinishingPhrase);
                    m_RecordingSession.FinishingPage += new Obi.Events.Audio.Recorder.FinishingPageHandler(m_RecordingSession_FinishingPage);
                }
            }
        }


        public ContentView contentView
        {
            get { return m_ContentView; }
            set
            {
                m_ContentView = value;
                //     if (m_ContentView != null) m_ContentView.SizeChanged += new EventHandler(Waveform_Recording_Resize);
            }
        }

        public ProjectView projectView
        {
            get { return m_ProjectView; }
            set
            {
                m_ProjectView = value;
                if (m_ProjectView != null)
                {
                    m_IsColorHighContrast = SystemInformation.HighContrast;   
                    m_ProjectView.TransportBar.Recorder.PcmDataBufferAvailable += new AudioRecorder.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Recorder);
                    m_ProjectView.ObiForm.Resize += new EventHandler(ObiForm_Resize);
                    m_ProjectView.ObiForm.ResizeEnd += new EventHandler(ObiForm_ResizeEnd);
                    m_ProjectView.ObiForm.ResizeBegin += new EventHandler(ObiForm_ResizeBegin);
                    m_ProjectView.ObiForm.Activated += new EventHandler(ObiForm_Activated);
                    m_ProjectView.ObiForm.Deactivate += new EventHandler(ObiForm_Deactivate);
                    m_ContentView.Resize += new EventHandler(m_ContentView_Resize);
                    m_ColorSettings = m_ProjectView.ObiForm.Settings.ColorSettings;
                    m_ColorSettingsHC = m_ProjectView.ObiForm.Settings.ColorSettingsHC;
                    this.BackColor = m_ColorSettings.WaveformBackColor;
                    if (m_IsColorHighContrast)
                        UpdateColors(m_ColorSettingsHC);
                    else
                        UpdateColors(m_ColorSettings);
                }
            }
        }

        public AudioLib.VuMeter VUMeter
        {
            get { return m_VUMeter; }
            set { m_VUMeter = value; }
        }

        public float zoomFactor
        {
            get { return m_ZoomFactor; }
            set
            {
                m_ZoomFactor = value;
                ZoomWaveform();
            }
        }

        public int recordingTimeCursor
        {
            get { return m_ContentView.Width / 2 + 50; }
        }

        public void ZoomWaveform()
        {
            if (m_ContentView != null)
                this.Size = new Size(m_ContentView.Width, Convert.ToInt32(104 * m_ZoomFactor));
        }
        public int WaveformHeight
        {
            get
            {
                {
                    return this.Height = this.Height - 20;
                }
            }
        }

        public bool invertColor
        {
            get { return m_IsColorHighContrast; }
            set
            {
                m_IsColorHighContrast = value;
                if (m_ProjectView == null)
                    return;
                if (m_ProjectView.ObiForm == null)
                    return;
                if (m_ProjectView.ObiForm.Settings == null)
                    return;
                if (m_IsColorHighContrast)
                    UpdateColors(m_ColorSettingsHC);
                else
                    UpdateColors(m_ColorSettings);
            }
        }


        public void UpdateColors(ColorSettings colorSettings)
        {
            m_ProjectView.ObiForm.ColorSettings.CreateBrushesAndPens();
            pen_ChannelMono = colorSettings.WaveformMonoPen;
            pen_Channel1 = colorSettings.WaveformChannel1Pen;
            pen_Channel2 = colorSettings.WaveformChannel2Pen;
            pen_WaveformBaseLine = colorSettings.WaveformBaseLinePen;
            pen_HighlightedHighContrastMono = m_ColorSettingsHC.WaveformHighlightedPen;
            m_PenTimeGrid = new Pen(m_IsColorHighContrast ? Color.LightGray : Color.Gray);
            m_PenPhrasePage = new Pen(m_IsColorHighContrast ? Color.White : Color.Black);
            m_PenPhrasePage.Width = 2;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawWaveForm();
        }

        public void DrawWaveForm()
        {
            if (!this.Visible || this.Handle.ToInt32() == 0
               || m_ProjectView == null || m_ProjectView.TransportBar == null || m_ContentView == null || !m_ProjectView.TransportBar.IsRecorderActive) return;

            if (m_StaticRecordingLocation == -1)
            {
                Location = new Point(-m_OffsetLocation, Location.Y);
                //m_StaticRecordingLocation = recordingTimeCursor + Math.Abs( this.Location.X);
            }

            m_XCV = m_X + Location.X;
            int diff = m_XCV - recordingTimeCursor;
            int newXLocation = (m_X - recordingTimeCursor) * -1;
            if (Math.Abs(diff) > 2)
            {
                this.Location = new Point(newXLocation, this.Location.Y);
                m_IsToBeRepainted = false;
            }
            if (newXLocation > 0)
            {
                m_IsToBeRepainted = false;
                Location = new Point(-m_OffsetLocation, Location.Y);
            }

            int difference = 0;
            difference = this.Width - m_X - m_ContentView.Width;
            if (this.Location.X + Width < m_ContentView.Width + 50)
            {
                this.Location = new Point(-m_OffsetLocation, Location.Y);
                //  listOfXLocation.Clear();
                m_X = recordingTimeCursor + m_OffsetLocation;
                m_StaticRecordingLocation = m_X;
                ResetLists();
                g.FillRectangle(new System.Drawing.SolidBrush(this.BackColor), 0, 0, Width, WaveformHeight);
                ResetWaveform();
                m_Pass++;
            }

            if (m_VUMeter == null || m_ContentView == null) return;
            g = this.CreateGraphics();

            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
            {
                g.DrawLine(pen_ChannelMono, new Point(m_X, (WaveformHeight - (int)Math.Round(((minChannel1 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin),
                     new Point(m_X, (WaveformHeight - (int)Math.Round(((maxChannel1 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin));

            }
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
            {
                g.DrawLine(pen_Channel1, new Point(m_X, (WaveformHeight - (int)Math.Round(((minChannel1 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin),
                   new Point(m_X, (WaveformHeight - (int)Math.Round(((maxChannel1 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin));
                g.DrawLine(pen_Channel2, new Point(m_X, (WaveformHeight - (int)Math.Round(((minChannel2 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin),
                   new Point(m_X, (WaveformHeight - (int)Math.Round(((maxChannel2 - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)) + m_TopMargin));
            }

            g.DrawLine(pen_WaveformBaseLine, 0, (WaveformHeight / 2) + m_TopMargin, m_ContentView.Width, (WaveformHeight / 2) + m_TopMargin);

            g.DrawLine(pen_WaveformBaseLine, 0, (WaveformHeight / 2) + m_TopMargin, Width, (WaveformHeight / 2) + m_TopMargin);
            string text = "";


            timeOfAssetMilliseconds =
                   (double)m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                   Time.TIME_UNIT;
            int timeInSeconds = Convert.ToInt32(timeOfAssetMilliseconds / 1000);
            m_Counter++;

            if (ConvertPixelsToTime(m_X) < (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                   Time.TIME_UNIT))
                timer1.Interval = 90;
            else
                timer1.Interval = 100;

            if (m_Counter == 10)
            {
                if (m_InitialOffsetTime < 0)
                {
                    m_InitialStaticTime = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                       Time.TIME_UNIT;
                    m_StaticRecordingLocation = m_X;
                }

                g.DrawLine(m_PenTimeGrid, m_X, 0 + m_TopMargin, m_X, WaveformHeight + m_TopMargin);
                if (timeInSeconds % 10 == 0 && m_LocalTime != timeInSeconds)
                {
                    text = timeInSeconds.ToString();
                    g.DrawString(text, myFont, Brushes.Gray, m_X, 20);
                    if (!m_MainDictionary.ContainsKey(m_X))
                        m_MainDictionary.Add(m_X, text);
                    m_LocalTime = timeInSeconds;
                }
                else
                {
                    if (!m_MainDictionary.ContainsKey(m_X))
                        m_MainDictionary.Add(m_X, "");
                }

                m_Counter = 0;
            }

            /* if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring && m_ExistingPhrase != m_ProjectView.TransportBar.RecordingPhrase)
                 CreatePageorPhrase(m_X);
             if ((m_ContentView.Width - m_XCV) > 600)
             {              
             //    this.Location = new Point(m_ContentView.Location.X, Location.Y);
                   // m_IsMaximized = false;
             }
             */
            listOfCurrentXLocation.Add(m_X);
            m_X++;
            listOfCurrentMinChannel1.Add((int)minChannel1);
            listOfCurrentMaxChannel1.Add((int)maxChannel1);
            listOfCurrentMinChannel2.Add((int)minChannel2);
            listOfCurrentMaxChannel2.Add((int)maxChannel2);
            m_TotalPixelCount++;
        }

        private void ResetLists()
        {
            List<int> listOfMinChannel1Temp = new List<int>();
            List<int> listOfMinChannel2Temp = new List<int>();
            List<int> listOfMaxChannel1Temp = new List<int>();
            List<int> listOfMaxChannel2Temp = new List<int>();
            Dictionary<int, string> m_TempDictionary = new Dictionary<int, string>();
            int calculatedKey = 0;
            int CalculatedKeyForDelete = 0;
            int tempXLocation = 0;
            m_ResetCalled = true;

            listOfMinChannel1Temp = listOfCurrentMinChannel1;
            listOfMinChannel2Temp = listOfCurrentMinChannel2;
            listOfMaxChannel1Temp = listOfCurrentMaxChannel1;
            listOfMaxChannel2Temp = listOfCurrentMaxChannel2;
            m_TempDictionary = m_MainDictionary;
            tempXLocation = listOfCurrentXLocation[listOfCurrentXLocation.Count - 1];
            m_InitialStaticTime = -1;
            m_InitialOffsetLocation = -1;

            listOfCurrentMinChannel1 = new List<int>();
            listOfCurrentMinChannel2 = new List<int>();
            listOfCurrentMaxChannel1 = new List<int>();
            listOfCurrentMaxChannel2 = new List<int>();
            m_MainDictionary = new Dictionary<int, string>();
            m_InitialStaticTime = (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                        Time.TIME_UNIT) + m_InitialOffsetTime;
            m_OverlapPixelLength = recordingTimeCursor;
            for (int i = listOfMinChannel1Temp.Count - recordingTimeCursor; i <= listOfMinChannel1Temp.Count - 1; i++)
            {
                if (i >= 0)
                {
                    listOfCurrentMinChannel1.Add(listOfMinChannel1Temp[i]);
                    listOfCurrentMinChannel2.Add(listOfMinChannel2Temp[i]);
                    listOfCurrentMaxChannel1.Add(listOfMaxChannel1Temp[i]);
                    listOfCurrentMaxChannel2.Add(listOfMaxChannel2Temp[i]);
                }
            }

            foreach (KeyValuePair<int, string> pair in m_TempDictionary)
            {
                if (pair.Key > tempXLocation - recordingTimeCursor && pair.Key < tempXLocation - 1)
                {
                    calculatedKey = pair.Key - (tempXLocation - recordingTimeCursor);
                    m_MainDictionary.Add(calculatedKey + m_OffsetLocation, pair.Value);
                }
            }

            Dictionary<double, int> tempTimeToPixelMap = new Dictionary<double, int>();
            foreach (KeyValuePair<double, int> pair in m_TimeToPixelMap)
            {
                if (pair.Value > tempXLocation - recordingTimeCursor && pair.Value < tempXLocation - 1)
                {
                    CalculatedKeyForDelete = pair.Value - (tempXLocation - recordingTimeCursor);
                    tempTimeToPixelMap.Add(pair.Key, CalculatedKeyForDelete + m_OffsetLocation);
                }
            }
            //foreach (double t in m_TimeToPixelMap.Keys)
            //{
            //    int pixel =   CalculatePixels(t);
            //    if (pixel >= m_OffsetLocation) tempTimeToPixelMap.Add(t, pixel);
            //}
            m_TimeToPixelMap = tempTimeToPixelMap ;
        }

        private double ConvertPixelsToTime(int pixels)
        {
            double time = 0;
            time = m_InitialStaticTime + (pixels - m_StaticRecordingLocation) * 100;

            return time;
        }

        private int ConvertTimeToPixels(double time)
        {
            int pixels = 0;
            pixels = Convert.ToInt32(time * .01);
            return pixels;
        }

        private double ConvertPixelToTime(int pixels)
        {
            double time = 0;
            time = pixels / 0.01;
            return time;
        }

        private void Waveform_Recording_VisibleChanged(object sender, EventArgs e)
        {
            m_InitialStaticTime = 0;
            if (this.Visible == true)
            {
                timer1.Start();
                if (m_ContentView != null)
                    m_X = recordingTimeCursor + m_OffsetLocation;
                Location = new Point(-m_OffsetLocation, Location.Y);
                m_MainDictionary.Clear();
            }
            else
            {
                Location = new Point(-m_OffsetLocation, Location.Y);
                timer1.Stop();
            }
            listOfCurrentMaxChannel1.Clear();
            listOfCurrentMinChannel1.Clear();
            listOfCurrentMinChannel2.Clear();
            listOfCurrentMaxChannel2.Clear();
            m_StaticRecordingLocation = -1;
            m_OverlapPixelLength = 0;
            m_MouseButtonDownLoc = 0;
            m_MouseButtonUpLoc = 0;
            if (m_RecordingSession != null)
            {
              m_RecordingSession.DeletedItemList.Clear();
            }
        }

        private short[] m_Amp = new short[2];
        private int m_AmpValue = 0;
        short minChannel1;
        short maxChannel1;
        short minChannel2;
        short maxChannel2;

        public void OnPcmDataBufferAvailable_Recorder(object sender, AudioRecorder.PcmDataBufferAvailableEventArgs e)
        {
            if (e.PcmDataBuffer != null && e.PcmDataBuffer.Length > 1)
            {
                m_Amp[0] = e.PcmDataBuffer[0];
                //m_Amp[1] = e.PcmDataBuffer[1];

                minChannel1 = short.MaxValue;
                maxChannel1 = short.MinValue;
                int channels = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels;
                int channel = 0;
                int frameSize = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.BlockAlign;
                short[] samples = new short[e.PcmDataBufferLength];
                Buffer.BlockCopy(e.PcmDataBuffer, 0, samples, 0, e.PcmDataBufferLength);

                //for (int i = 0 ; i < (int)Math.Ceiling(e.PcmDataBufferLength/ (float)frameSize); i += frameSize)
                for (int i = channel; i < (int)Math.Ceiling(e.PcmDataBufferLength / (float)frameSize); i += channels)
                {

                    if (samples[i] < minChannel1) minChannel1 = samples[i];
                    if (samples[i] > maxChannel1) maxChannel1 = samples[i];
                }

                if (channels > 1)
                {
                    minChannel2 = short.MaxValue;
                    maxChannel2 = short.MinValue;
                    channel = 1;
                    for (int i = channel; i < (int)Math.Ceiling(e.PcmDataBufferLength / (float)frameSize); i += channels)
                    {
                        if (samples[i] < minChannel2) minChannel2 = samples[i];
                        if (samples[i] > maxChannel2) maxChannel2 = samples[i];
                    }
                }
                m_AmpValue = m_Amp[0] + (int)(m_Amp[1] * Math.Pow(8, m_Amp.Length));
                // m_PointMinChannelMap.Add(x_Loc, minChannel1);
                // m_PointMaxChannelMap.Add(x_Loc, maxChannel1);

            }
        }

        /*   private void Waveform_Recording_Resize(object sender, EventArgs e)
           {
               System.Media.SystemSounds.Asterisk.Play () ;

               RepaintWaveform();           
           }*/

        private void ObiForm_Resize(object sender, EventArgs e)
        {
            if (m_IsResizing)
                return;
            //m_CounterWaveform = listOfCurrentMinChannel1.Count;
            RepaintWaveform();
            m_IsResizing = false;
        }

        private void ObiForm_ResizeBegin(object sender, EventArgs e)
        {
            m_IsResizing = true;
        }

        private void ObiForm_ResizeEnd(object sender, EventArgs e)
        {
            //          m_CounterWaveform = listOfCurrentMinChannel1.Count;

            RepaintWaveform();
            m_IsResizing = false;
        }


        private void ObiForm_Deactivate(object sender, EventArgs e)
        {
            /*     if (this.Visible == false || !m_ProjectView.TransportBar.IsRecorderActive)
                     return;
            
                 Bitmap bmp = new Bitmap(this.Width, this.WaveformHeight);
                 Rectangle newRect = new Rectangle(0, 0, this.Width, this.WaveformHeight);

                 this.DrawToBitmap(bmp, newRect);

                 for (int i = recordingTimeCursor; i > 0; i--)
                 {
                     if(bmp.GetPixel(i, this.WaveformHeight / 2) == this.BackColor)
                     {
                    
                         for (int j = listOfCurrentMinChannel1.Count; j >= 0 ; j++)
                         {
                             g.DrawLine(pen_ChannelMono, new Point(i, WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[j] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                                 new Point(i, WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[j] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)));
                        
                         }
                     }
                 }*/
        }

        private void ObiForm_Activated(object sender, EventArgs e)
        {
        }

        private void m_ContentView_Resize(object sender, EventArgs e)
        {
            if (m_IsResizing)
                return;
            //            m_CounterWaveform = listOfCurrentMinChannel1.Count;

            RepaintWaveform();
            m_IsResizing = false;
        }

        private void RepaintWaveform()
        {
            Font myFont = new Font("Microsoft Sans Serif", 7);

            int xSize = SystemInformation.PrimaryMonitorSize.Width;
            int tempm_X = m_X;
            int counterWaveform = 0;
            g.FillRectangle(new System.Drawing.SolidBrush(this.BackColor), 0, 0, Width, Height);
            if (m_IsMaximized)
            {
                m_IsMaximized = false;
                return;
            }
            m_IsMaximized = true;
            int counterMin = listOfCurrentMinChannel1.Count;
            int x = 0;
            int counterMax = listOfCurrentMaxChannel2.Count;
            int countToRepaint = 0;
            counterWaveform = listOfCurrentMinChannel1.Count;

            if (m_ContentView != null)
                x = recordingTimeCursor;
            if (counterMin == 0)
                return;
            if (counterMin < 5)
            { }
            else
            {
                timer1.Stop();
                if (xSize > listOfCurrentMinChannel1.Count)
                    xSize = listOfCurrentMinChannel1.Count;

                /*if (counterWaveform < xSize)
                    countToRepaint = counterWaveform;
                else
                    countToRepaint = xSize;*/
                this.Location = new Point((m_X - recordingTimeCursor) * -1, Location.Y);
                foreach (double[] arr in m_RecordingSession.DeletedItemList)
                {
                    int beginPixel = m_TimeToPixelMap.ContainsKey (arr[0])? m_TimeToPixelMap[arr[0]] : CalculatePixels(arr[0]) ;
                    int endPixel = m_TimeToPixelMap.ContainsKey(arr[1])? m_TimeToPixelMap[arr[1]]: CalculatePixels(arr[1]);
                    g.FillRectangle(SystemBrushes.ControlDark, beginPixel, m_TopMargin,endPixel -beginPixel , this.WaveformHeight);
                    UpdateTimeToPixelDictionary(arr[0], beginPixel);
                    UpdateTimeToPixelDictionary (arr[1],endPixel) ;
                }
                for (int i = listOfCurrentMinChannel1.Count - 1; i >= listOfCurrentMinChannel1.Count - xSize; i--)
                {
                    if (tempm_X == m_MouseButtonUpLoc)
                    {
                        if (m_MouseButtonUpLoc < m_X)
                            g.FillRectangle(SystemBrushes.Highlight, m_MouseButtonDownLoc, m_TopMargin, m_MouseButtonUpLoc - m_MouseButtonDownLoc, this.WaveformHeight);
                        else
                            g.FillRectangle(SystemBrushes.Highlight, m_MouseButtonDownLoc, m_TopMargin, m_MouseButtonUpLoc - m_MouseButtonDownLoc, this.WaveformHeight);
                        pen_ChannelMono = new Pen(SystemColors.HighlightText);
                    }
                    if (tempm_X == m_MouseButtonDownLoc)
                        pen_ChannelMono =m_IsColorHighContrast? m_ColorSettingsHC.WaveformMonoPen: m_ColorSettings.WaveformMonoPen;

                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
                    {
                        g.DrawLine(pen_ChannelMono, new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                        new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                    }

                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
                    {
                        g.DrawLine(pen_Channel1, new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                        new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        g.DrawLine(pen_Channel2, new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                        new Point(tempm_X, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                    }
                    tempm_X--;
                    /*  if (this.Location.X < 0 && 
                          (this.Location.X * -1) < listOfXLocation[i])
                      {
                        
                     * .WriteLine("Breaking refresh loop at:" + i) ;
                          break;
                      }*/
                }

                foreach (KeyValuePair<int, string> pair in m_MainDictionary)
                {
                    g.DrawLine(m_PenTimeGrid, pair.Key, 0+m_TopMargin, pair.Key, WaveformHeight+m_TopMargin);
                    if (!pair.Value.EndsWith("0") || (pair.Value.StartsWith("P")))
                    {
                        g.DrawString(pair.Value, myFont, SystemBrushes.ControlDarkDark, pair.Key, 0);
                        if (pair.Value != "")
                            g.DrawLine(m_PenPhrasePage, pair.Key, 0+m_TopMargin, pair.Key, WaveformHeight+m_TopMargin);
                    }
                    else
                    {
                        g.DrawLine(m_PenTimeGrid, pair.Key, 0+m_TopMargin, pair.Key, WaveformHeight+m_TopMargin);
                        g.DrawString(pair.Value, myFont, Brushes.Gray, pair.Key, 20);
                    }
                }
                m_IsMaximized = false;

                timer1.Start();


            }
        }

        public void ResetWaveform()
        {
            Font myFont = new Font("Microsoft Sans Serif", 7);

            int xSize = SystemInformation.PrimaryMonitorSize.Width;
            int temp = m_X;
            if (m_IsMaximized)
            {
                m_IsMaximized = false;
                return;
            }
            m_IsMaximized = true;
            int counterMin = listOfCurrentMinChannel1.Count;
            int x = 0;
            int counterMax = listOfCurrentMaxChannel2.Count;
            int countToRepaint = 0;

            if (m_ContentView != null)
                x = recordingTimeCursor;
            if (counterMin == 0)
                return;
            if (counterMin < 5)
            { }
            else
            {
                timer1.Stop();
                /*if (m_CounterWaveform < xSize)
                    countToRepaint = m_CounterWaveform;
                  else
                    countToRepaint = xSize;*/

                for (int i = counterMin - 1; i >= 0; i--)
                {
                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
                    {
                        if (!m_IsColorHighContrast)
                        {
                            g.DrawLine(pen_ChannelMono, new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }
                        else
                        {
                            g.DrawLine(pen_HighContrastMono, new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }

                    }

                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
                    {
                        if (!m_IsColorHighContrast)
                        {
                            g.DrawLine(pen_Channel1, new Point(temp, WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                            g.DrawLine(pen_Channel2, new Point(temp, WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }
                        else
                        {
                            g.DrawLine(pen_HighContrastChannel1, new Point(temp, WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel1[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                            g.DrawLine(pen_HighContrastChannel2, new Point(temp, WaveformHeight - (int)Math.Round(((listOfCurrentMinChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                            new Point(temp, (WaveformHeight - (int)Math.Round(((listOfCurrentMaxChannel2[i] - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }
                    }

                    temp--;
                    /*  if (this.Location.X < 0 && 
                          (this.Location.X * -1) < listOfXLocation[i])
                      {
                          Console.WriteLine("Breaking refresh loop at:" + i) ;
                          break;
                      }*/
                }
              
               foreach (double[] arr in m_RecordingSession.DeletedItemList)
                {
                    if (m_TimeToPixelMap.ContainsKey(arr[0]) && m_TimeToPixelMap.ContainsKey(arr[1]))
                    {
                        int beginPixel = m_TimeToPixelMap[arr[0]];
                        int endPixel = m_TimeToPixelMap[arr[1]];
                        //Console.WriteLine("Begin Pixel Value is {0}",beginPixel);
                        //Console.WriteLine("End Pixel Value is {0}",endPixel);
                        g.FillRectangle(SystemBrushes.ControlDark, beginPixel, m_TopMargin, endPixel - beginPixel, this.WaveformHeight);
                        //UpdateTimeToPixelDictionary(arr[0], beginPixel);
                        //UpdateTimeToPixelDictionary(arr[1], endPixel);
                    }
                    else if (m_TimeToPixelMap.ContainsKey(arr[1]))
                    {
                        int beginPixel = tempXLocation - recordingTimeCursor;
                        int endPixel = m_TimeToPixelMap[arr[1]];
                        //Console.WriteLine("Begin Pixel Value is {0}",beginPixel);
                        //Console.WriteLine("End Pixel Value is {0}",endPixel);
                        g.FillRectangle(SystemBrushes.ControlDark, beginPixel, m_TopMargin, endPixel - beginPixel, this.WaveformHeight);
                    }
                }
                foreach (KeyValuePair<int, string> pair in m_MainDictionary)
                {
                    g.DrawLine(m_PenTimeGrid, pair.Key, 0+m_TopMargin, pair.Key, WaveformHeight+m_TopMargin);
                    if (!pair.Value.EndsWith("0") || (pair.Value.StartsWith("P")))
                    {
                        g.DrawString(pair.Value, myFont, SystemBrushes.ControlDarkDark, pair.Key, 0);
                        if (pair.Value != "")
                            g.DrawLine(m_PenPhrasePage, pair.Key, 0 + m_TopMargin, pair.Key, WaveformHeight + m_TopMargin);
                    }
                    else
                    {
                        g.DrawLine(m_PenTimeGrid, pair.Key, 0+m_TopMargin, pair.Key, WaveformHeight+m_TopMargin);
                        g.DrawString(pair.Value, myFont, Brushes.Gray, pair.Key, 20);
                    }
                }

                m_IsMaximized = false;
                timer1.Start();
            }
        }
        /*
        public void Phrase_Created_Event(object sender, EventArgs e)
        {
            int lastItemInList = (int)(m_ProjectView.TransportBar.phDetectorPhraseTimingList[m_ProjectView.TransportBar.phDetectorPhraseTimingList.Count - 1] / 100);
            int location = lastItemInList + recordingTimeCursor;

         //   if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring)
           // CreatePageorPhrase(location);
        }
        */
        private void CreatePageorPhrase(int xLocation)
        {
            string text = "";
            if (m_ProjectView.TransportBar.RecordingPhrase != null && m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Page)
                text = "Page" + m_ProjectView.TransportBar.RecordingPhrase.PageNumber.ToString();
            // else if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Plain)
            //   text = "Phrase";
            //g.DrawLine(pen, xLocation, 0, xLocation, WaveformHeight);
            g.DrawString(text, myFont, Brushes.Black, xLocation, 0);
            m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;

            if (!m_MainDictionary.ContainsKey(xLocation))
                m_MainDictionary.Add(xLocation, text);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (m_IsToBeRepainted)
                RepaintWaveform();

            m_IsToBeRepainted = true;
            m_IsMaximized = false;

        }

        private void m_RecordingSession_FinishingPhrase(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            int pixel = 0;
            string text = "Ph";
            Pen pen = new Pen(SystemColors.ControlDarkDark);

            if (m_TimeToPixelMap.ContainsKey(e.TimeFromBeginning))
            {
                pixel = m_TimeToPixelMap[e.TimeFromBeginning];
            }
            else
            {
                pixel = CalculatePixels(e.TimeFromBeginning);
                UpdateTimeToPixelDictionary(e.TimeFromBeginning, pixel);
            }
            g.DrawLine(m_PenPhrasePage, pixel, 0+m_TopMargin, pixel, WaveformHeight+m_TopMargin);
            g.DrawString("Ph", myFont, SystemBrushes.ControlDarkDark, pixel, 0);

            if (m_MainDictionary.ContainsKey(pixel))
            {
                m_MainDictionary[pixel] = text;
            }
            else
            {
                m_MainDictionary.Add(pixel, text);
            }
            //if (!m_MainDictionary.ContainsKey(pixel))
            //{
            //    m_MainDictionary.Add(pixel, "Ph");
            //    Console.WriteLine("Phrase Added Called: {0}", IndexOfPhrase);
            //    IndexOfPhrase++;
            //}
            
        }

        private void m_RecordingSession_FinishingPage(object sender, Obi.Events.Audio.Recorder.PhraseEventArgs e)
        {
            string text = "Pg";
            PhraseNode phrase = (PhraseNode)m_ProjectView.TransportBar.RecordingPhrase.ParentAs<SectionNode>().PhraseChild(e.PhraseIndex + m_ProjectView.TransportBar.RecordingInitPhraseIndex + 1);
            if (m_ProjectView.TransportBar.RecordingPhrase != null) text = text + " " + phrase.PageNumber.ToString();

            int pixel = 0;
            //   Pen pen = new Pen(SystemColors.ControlDarkDark);

            if (m_TimeToPixelMap.ContainsKey(e.TimeFromBeginning))
            {
                pixel = m_TimeToPixelMap[e.TimeFromBeginning];
            }
            else
            {
                pixel = CalculatePixels(e.TimeFromBeginning);
                UpdateTimeToPixelDictionary(e.TimeFromBeginning, pixel);
            }
            //if (IsInSelection(pixel))
            //    g.FillRectangle(new System.Drawing.SolidBrush(SystemColors.Highlight), pixel + 1, 0, 35, 10);
            //else
                g.FillRectangle(new System.Drawing.SolidBrush(this.BackColor), pixel + 1, 0, 35, 10);

            g.DrawString(text, myFont, SystemBrushes.ControlDarkDark, pixel, 0);
         //   g.DrawLine(blackPen, pixel, 0, pixel, WaveformHeight);
            if (m_MainDictionary.ContainsKey(pixel))
                m_MainDictionary[pixel] = text;
            else if (m_MainDictionary.ContainsKey(pixel + 1))
                m_MainDictionary[pixel + 1] = text;
            else if (m_MainDictionary.ContainsKey(pixel - 1))
                m_MainDictionary[pixel - 1] = text;
            else
                m_MainDictionary.Add(pixel, text);

        }

        private double CalculateTime(int pixel)
        {
            int PhraseMarkPixel = pixel;
            double calculatedTime = 0;

            double currentMarkTime = 0;
            currentMarkTime = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                        Time.TIME_UNIT;
            int currentm_X = m_X;
            int pixelGap = currentm_X - pixel;
            calculatedTime = ConvertPixelToTime(pixelGap);
            calculatedTime = currentMarkTime - calculatedTime;
            return calculatedTime;

        }
        private int CalculatePixels(double time)
        {
            double phraseMarkTime = time;
            int calculatedPixel = 0;
            double currentMarkTime = 0;
            int currentm_X;
            int pixelGap = 0;

            currentMarkTime = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
                        Time.TIME_UNIT;
            currentm_X = m_X;
            pixelGap = ConvertTimeToPixels(phraseMarkTime - currentMarkTime);
            if (pixelGap < 0)
                pixelGap = pixelGap * -1;
            if (phraseMarkTime < currentMarkTime)
                calculatedPixel = currentm_X - pixelGap;
            else
                calculatedPixel = currentm_X + pixelGap;
            return calculatedPixel;
        }

        private void Waveform_Recording_MouseDown(object sender, MouseEventArgs e)
        {
            m_DeletedPartEnclosed = false;
            if (!IsPhraseMarkAllowed(ConvertPixelsToTime(e.X)))
            {
                m_MouseButtonDownLoc = 0;
                m_MouseButtonUpLoc = 0;
                return;
            }

            if (IsValid(e.X))
                markPageToolStripMenuItem.Enabled = true;
            else
                markPageToolStripMenuItem.Enabled = false;
            // bool IsDeselected = false;



            if (e.Button == MouseButtons.Left)
            {

                if (m_MouseButtonDownLoc != m_MouseButtonUpLoc)
                {
                    if ((!IsValid(m_MouseButtonDownLoc) && m_MouseButtonDownLoc!= 0) 
                        || (!IsValid(m_MouseButtonUpLoc) && m_MouseButtonUpLoc != 0))
                    return;

                    listOfSelctedPortion.Sort();
                    listOfEndSelection.Sort();
                    if (listOfSelctedPortion.Count > 0)
                    {
                        int tempMouseDown = listOfSelctedPortion[0];
                        int tempMouseUp = listOfEndSelection[listOfEndSelection.Count - 1];
                        PaintWaveform(tempMouseDown, tempMouseUp, false);
                    }
                    else
                    {
                        PaintWaveform(m_MouseButtonDownLoc , m_MouseButtonUpLoc, false);
                    }
                    listOfSelctedPortion.Clear();
                    listOfEndSelection.Clear();
                   // PaintWaveform(m_MouseButtonDownLoc-30, m_MouseButtonUpLoc+30, false);

                    //    IsDeselected = true;
                    // SelectPortionOfWaveform(IsDeselected);
                }
                m_MouseButtonDownLoc = e.X;
                m_MouseButtonUpLoc = 0;
                m_TempMouseMoveLoc = m_MouseButtonDownLoc;
                
            }

            if (e.Button == MouseButtons.Right)
            {
                m_PixelOfRedLine = e.X;
                g.DrawLine(new Pen(Color.Red), new Point(e.X, 0+m_TopMargin), new Point(e.X, WaveformHeight+m_TopMargin));
            }
            IsSelectionActive = true;
            m_StartSelection = m_MouseButtonDownLoc - (recordingTimeCursor + m_OffsetLocation);
            m_Time = ConvertPixelsToTime(e.X);
            m_NewPhraseTime = m_Time;

            m_IsPage = false;
            //   IsDeselected = false;
            m_IsMouseBtnUp = false;
        }

        private void Waveform_Recording_MouseUp(object sender, MouseEventArgs e)
        {
            m_MouseMoveList.Clear();
            if (m_MouseButtonDownLoc == 0)
            {
                listOfSelctedPortion.Clear();
                listOfEndSelection.Clear();
                m_MouseButtonUpLoc = 0;
                return;
            }

            //if (m_DeletedPartEnclosed || !IsPhraseMarkAllowed(ConvertPixelsToTime(e.X)))
            //{
            //    m_DeletedPartEnclosed = false;
            //    return;
            //}

            if (e.Button == MouseButtons.Left)
                m_MouseButtonUpLoc = e.X;
            int swap = 0;
            IsSelectionActive = false;
            if (!IsPhraseMarkAllowed(ConvertPixelsToTime(e.X)))
                return;
            else
            {
                if (m_MouseButtonDownLoc == e.X || m_MouseButtonDownLoc == e.X + 1 || m_MouseButtonDownLoc == e.X - 1 || m_MouseButtonDownLoc==e.X+2 || m_MouseButtonDownLoc==e.X-2 || m_MouseButtonDownLoc==e.X+3 || m_MouseButtonDownLoc==e.X-3)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (e.X < (m_OffsetLocation + recordingTimeCursor) || e.X > m_X)
                        {
                            m_MouseButtonDownLoc = 0;
                            m_MouseButtonUpLoc = 0;
                        }
                        else
                            m_RecordingSession.UpdatePhraseTimeList(m_Time, m_IsPage);
                    }
                }

                else if ((m_MouseButtonUpLoc > m_X && m_MouseButtonDownLoc > m_X) || (m_MouseButtonUpLoc < (recordingTimeCursor + m_OffsetLocation) && m_MouseButtonDownLoc < (recordingTimeCursor + m_OffsetLocation)) && m_Pass==0)
                {
                    m_MouseButtonDownLoc = 0;
                    m_MouseButtonUpLoc = 0;
                }
                else if (m_MouseButtonUpLoc > m_X && m_MouseButtonDownLoc < m_X && m_MouseButtonDownLoc > (recordingTimeCursor + m_OffsetLocation))
                {
                    m_MouseButtonUpLoc = m_X;
                }
                else if (m_MouseButtonDownLoc < (recordingTimeCursor + m_OffsetLocation) && m_MouseButtonUpLoc > (recordingTimeCursor + m_OffsetLocation) && m_MouseButtonUpLoc < m_X)
                {
                    m_MouseButtonDownLoc = recordingTimeCursor + m_OffsetLocation;
                }
                else if (m_MouseButtonUpLoc > m_X && m_MouseButtonDownLoc < (recordingTimeCursor + m_OffsetLocation))
                {
                    m_MouseButtonDownLoc = recordingTimeCursor + m_OffsetLocation;
                    m_MouseButtonUpLoc = m_X;
                }
                /*     
                  if (e.X < m_X)
                         g.FillRectangle(SystemBrushes.Highlight, m_MouseButtonDownLoc, 0, e.X - m_MouseButtonDownLoc, this.WaveformHeight);
                     else
                         g.FillRectangle(SystemBrushes.Highlight, m_MouseButtonDownLoc, 0, m_X - m_MouseButtonDownLoc, this.WaveformHeight);
                     SelectPortionOfWaveform();
                 */

                m_IsMouseBtnUp = true;

                for (int i = m_MouseButtonDownLoc; i < m_MouseButtonUpLoc; i++)
                {
                    if (m_MainDictionary.ContainsKey(i))
                        DrawDictionary(i, true);
                }

                // m_NewPhraseTime = ConvertPixelsToTime(e.X);
                if (m_MouseButtonDownLoc > m_MouseButtonUpLoc)
                {
                    swap = m_MouseButtonUpLoc;
                    m_MouseButtonUpLoc = m_MouseButtonDownLoc;
                    m_MouseButtonDownLoc = swap;
                }
            }
        }

        /*      
          public void SelectPortionOfWaveform(bool IsDeselected)
              {
                  Pen pen_Waveform = null;
                  Pen newPen = null;
                  Pen blackPen = new Pen(SystemColors.ControlDarkDark);

                  if (!IsValid(m_MouseButtonDownLoc) || !IsValid(m_MouseButtonUpLoc))
                      return;
                  if (IsDeselected)
                  {
                      if(m_MouseButtonUpLoc < m_MouseButtonDownLoc)
                          g.FillRectangle(SystemBrushes.HighlightText, m_MouseButtonUpLoc, 0, m_MouseButtonDownLoc - m_MouseButtonUpLoc, this.WaveformHeight);
                      else
                          g.FillRectangle(SystemBrushes.HighlightText, m_MouseButtonDownLoc, 0, m_MouseButtonUpLoc - m_MouseButtonDownLoc, this.WaveformHeight);
                      pen_Waveform = pen_ChannelMono;
                      newPen = new Pen(Color.LightGray);
                  }
                  else
                  {
                      pen_Waveform = new Pen(SystemColors.HighlightText);
                      newPen = new Pen(SystemColors.ControlLight);
                  }
                  PaintWaveform(m_MouseButtonDownLoc, m_MouseButtonUpLoc, false);            
              }
          
         */

        private void addSectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ProjectView.AddStrip();
        }

        private void phraseIsTODOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            phraseIsTODOToolStripMenuItem.Checked = !phraseIsTODOToolStripMenuItem.Checked;
            m_ProjectView.ToggleTODOForPhrase();
        }

        private void markPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_IsPage = true;
            if (!IsPhraseMarkAllowed(m_Time))
                return;
            else
            {
                m_RecordingSession.UpdatePhraseTimeList(m_Time, m_IsPage);
                m_NewPhraseTime = -1;
            }

        }

        public bool IsPhraseMarkAllowed(double time)
        {
            bool IsAllowed = true;
            int count = 1;

            int noOfdeleteval = m_RecordingSession.DeletedItemList.Count;
            if (noOfdeleteval != 0)
                while (count <= m_RecordingSession.DeletedItemList.Count)
                {
                    int mid = (count + noOfdeleteval) / 2;
                    double[] array = m_RecordingSession.DeletedItemList[mid - 1];
                    if (time >= array[0])
                    {
                        if (time <= array[1] || time == array[0])
                        {
                            IsAllowed = false;
                            return IsAllowed;
                        }
                        else if (time > array[1])
                        {
                            count = mid + 1;
                        }

                    }
                    else
                    {
                        noOfdeleteval = mid;
                        if (mid == count)
                            return IsAllowed;
                    }

                }


            return IsAllowed;
        }

        private void Waveform_Recording_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            m_MouseMoveList.Add(e.X);
            if(m_MouseButtonDownLoc==0)
                return;
            
            if (m_DeletedPartEnclosed || !IsPhraseMarkAllowed(ConvertPixelsToTime(e.X)))
            {
                if (m_MouseButtonDownLoc < m_TempMouseMoveLoc)
                {
                    PaintWaveform(m_MouseButtonDownLoc, m_TempMouseMoveLoc, false);
                }
                else
                {
                    PaintWaveform(m_TempMouseMoveLoc, m_MouseButtonDownLoc, false);
                }
                m_MouseButtonDownLoc = 0;
                m_MouseButtonUpLoc = 0;
                m_TempMouseMoveLoc = 0;
                m_DeletedPartEnclosed = true;
               return;
            }


         


            if (m_RecordingSession.DeletedItemList.Count != 0)
            {
                double tempStartTime = ConvertPixelsToTime(m_MouseButtonDownLoc);
                double tempEndTime = ConvertPixelsToTime(e.X);
                for (int i = m_RecordingSession.DeletedItemList.Count - 1; i >= 0; i--)
                {
                    double[] arr = m_RecordingSession.DeletedItemList[i];

                    if (arr[1] < tempStartTime && arr[1] < tempEndTime)
                    {
                        break;

                    }
                    if (arr[0] > tempStartTime && arr[0] < tempEndTime ||
                       arr[0] < tempStartTime && arr[0] > tempEndTime ||
                       arr[1] > tempStartTime && arr[1] < tempEndTime ||
                       arr[1] < tempStartTime && arr[1] > tempEndTime)
                    {
                        Console.WriteLine("Destination Reached");
                        m_DeletedPartEnclosed = true;
                        return;
                    }
                }
            }

           
            if (!IsSelectionActive)
                return;
            if (m_MouseButtonUpLoc == 0)
                m_MouseButtonUpLoc = e.X;
            if (!m_IsMouseBtnUp && m_MouseButtonDownLoc > 0)
            {
                if (!IsValid(e.X))
                    return;
                else if (IsValid(e.X) && !IsValid(m_TempMouseMoveLoc) && m_MouseButtonDownLoc > 0)
                {
                    m_TempMouseMoveLoc = e.X;
                    m_MouseButtonDownLoc = e.X;
                }

                Pen backPen = new Pen(SystemColors.Highlight);
                Pen gridLinePen = new Pen(Color.LightGray);
                Pen pen_Waveform = pen_Waveform = new Pen(SystemColors.HighlightText);
                Pen blackPen = new Pen(SystemColors.ControlDarkDark);

                if (m_MouseButtonDownLoc < e.X)
                {
                    if (m_TempMouseMoveLoc > m_MouseButtonDownLoc && m_TempMouseMoveLoc < e.X)
                    {
                        m_IsSelected = true;
                        PaintWaveform(m_TempMouseMoveLoc, e.X, true);
                    }
                    else
                    {
                        m_IsSelected = false;
                        if (e.X < m_TempMouseMoveLoc)
                            PaintWaveform(e.X, m_TempMouseMoveLoc, false);
                        else
                            PaintWaveform(m_TempMouseMoveLoc, e.X, false);
                    }
                }
                else if (m_MouseButtonDownLoc > e.X)
                {
                    if (m_TempMouseMoveLoc < m_MouseButtonDownLoc && m_TempMouseMoveLoc > e.X)
                    {
                        m_IsSelected = true;
                        PaintWaveform(e.X, m_TempMouseMoveLoc, true);
                    }
                    else
                    {
                        m_IsSelected = false;
                        if (m_TempMouseMoveLoc < e.X)
                            PaintWaveform(m_TempMouseMoveLoc, e.X, false);
                        else
                            PaintWaveform(e.X, m_TempMouseMoveLoc, true);
                    }
                }
            }
            m_TempMouseMoveLoc = e.X;
        }

        public void PaintWaveform(int startSelection, int endSelection, bool IsSelected)
        {

            if (IsSelected)
            {
                listOfSelctedPortion.Add(startSelection);
                listOfEndSelection.Add(endSelection);
                if (startSelection < endSelection)
                    g.FillRectangle(SystemBrushes.Highlight, startSelection, 0 + m_TopMargin, endSelection - startSelection, this.WaveformHeight);
                else
                    g.FillRectangle(SystemBrushes.Highlight, startSelection, 0 + m_TopMargin, startSelection - endSelection, this.WaveformHeight);
            }
            else
            {
                if (startSelection < endSelection)
                    g.FillRectangle(Brushes.White, startSelection, 0 + m_TopMargin, endSelection - startSelection, this.WaveformHeight);
                else
                    g.FillRectangle(Brushes.White, startSelection, 0 + m_TopMargin, startSelection - endSelection, this.WaveformHeight);
            }

            for (int i = startSelection; i <= endSelection; i++)
            {
                if (IsValid(i))
                {
                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
                    {
                        if (IsSelected)
                            g.DrawLine(m_ColorSettings.WaveformHighlightedPen, new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                      new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        else
                            g.DrawLine(pen_ChannelMono, new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                      new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                    }

                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
                    {
                        if (IsSelected)
                        {
                            g.DrawLine(m_ColorSettings.WaveformHighlightedPen, new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                     new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                            g.DrawLine(m_ColorSettings.WaveformHighlightedPen, new Point(i, WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel2) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)),
                      new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel2) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }
                        else
                        {
                            g.DrawLine(pen_Channel1, new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue)+m_TopMargin)),
                     new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                            g.DrawLine(pen_Channel2, new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMinChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin),
                                                 new Point(i, (WaveformHeight - (int)Math.Round(((GetAmplitude(i, listOfCurrentMaxChannel1) - short.MinValue) * WaveformHeight) / (float)ushort.MaxValue))+m_TopMargin));
                        }
                    }
                }
                DrawDictionary(i, IsSelected);
            }
            for (int i = startSelection; i > startSelection - 50; i--)
            {
                if (m_MainDictionary.ContainsKey(i))
                    DrawDictionary(i, false);
            }
        }

        public void DrawDictionary(int index, bool isSelected)
        {
            Brush brushSel = null;
            Pen linePen = new Pen(Color.Black);
            linePen.Width = 2;
            if (isSelected)
                brushSel = SystemBrushes.ControlDarkDark;
            else
                brushSel = SystemBrushes.ControlDarkDark;

            

            if (m_MainDictionary.ContainsKey(index))
            {

                g.DrawLine(m_PenTimeGrid, index, 0+m_TopMargin, index, WaveformHeight+m_TopMargin);
                //if ((!m_MainDictionary[index].StartsWith("P")) || m_MainDictionary[index].EndsWith("0"))

                if (!m_MainDictionary[index].EndsWith("0") || (m_MainDictionary[index].StartsWith("P")))
                {
                    if (m_MainDictionary[index] != "")
                    {
                        g.DrawLine(linePen, index, 0 + m_TopMargin, index, WaveformHeight + m_TopMargin);
                        g.FillRectangle(new System.Drawing.SolidBrush(this.BackColor), index, 0, m_MainDictionary[index].Length, 10);
                        g.DrawString(m_MainDictionary[index], myFont, brushSel, index, 0);
                    }
                    
                }
                else
                {

                    g.FillRectangle(new System.Drawing.SolidBrush(this.BackColor), index, 20, m_MainDictionary[index].Length, 10);
                    g.DrawString(m_MainDictionary[index], myFont, Brushes.Gray, index, 20);
                    if (m_MainDictionary[index] != "")
                    {
                      g.DrawLine(m_PenTimeGrid, index, 0 + m_TopMargin, index, WaveformHeight + m_TopMargin);
                    }
                }
            }
        }

        public int GetAmplitude(int absLoc, List<int> listOfChannelAmplitudes)
        {
            int actualLoc = absLoc - (recordingTimeCursor + m_OffsetLocation - (m_Pass == 0? 0 : m_OverlapPixelLength ));
            if (actualLoc < 0 || actualLoc >= listOfChannelAmplitudes.Count) return 0;
            return listOfChannelAmplitudes[actualLoc];
        }

        public bool IsValid(int location_X)
        {
            if ((location_X >= (recordingTimeCursor + m_OffsetLocation) && location_X < m_X) || (m_Pass>0 && location_X < m_X))
                return true;
            else
                return false;
        }

        public bool IsInSelection(int location)
        {
            if (location > m_MouseButtonDownLoc && location < m_MouseButtonUpLoc)
                return true;
            else
                return false;
        }

        private void Waveform_Recording_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (m_NewPhraseTime > 0 && !m_IsPage)
            {
                if (IsInSelection(CalculatePixels(m_NewPhraseTime)))
                    PaintWaveform((CalculatePixels(m_NewPhraseTime) - 5), (CalculatePixels(m_NewPhraseTime)+5), true);
                else
                    PaintWaveform((CalculatePixels(m_NewPhraseTime) - 5), (CalculatePixels(m_NewPhraseTime)+5), false);
                m_NewPhraseTime = -1;
            }
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listOfSelctedPortion.Clear();
            listOfEndSelection.Clear();
            if (m_MouseButtonDownLoc == m_MouseButtonUpLoc)
            {
                PaintWaveform(m_MouseButtonUpLoc - 5, m_MouseButtonUpLoc + 5, false);
                return;
            }
            double beginTime = ConvertPixelsToTime(m_MouseButtonDownLoc) ;
            double endTime = ConvertPixelsToTime(m_MouseButtonUpLoc) ;
            m_RecordingSession.UpdateDeletedTimeList( beginTime,endTime );
            /*   listOfCurrentMinChannel1.RemoveRange(m_MouseButtonDownLoc - (recordingTimeCursor + m_OffsetLocation + m_DeletedOffset), (m_MouseButtonUpLoc - m_MouseButtonDownLoc));
               listOfCurrentMaxChannel1.RemoveRange(m_MouseButtonDownLoc - (recordingTimeCursor + m_OffsetLocation + m_DeletedOffset), (m_MouseButtonUpLoc - m_MouseButtonDownLoc));
               if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
               {
                   listOfCurrentMinChannel2.RemoveRange(m_MouseButtonDownLoc - (recordingTimeCursor + m_OffsetLocation + m_DeletedOffset), (m_MouseButtonUpLoc - m_MouseButtonDownLoc));
                   listOfCurrentMaxChannel2.RemoveRange(m_MouseButtonDownLoc - (recordingTimeCursor + m_OffsetLocation + m_DeletedOffset), (m_MouseButtonUpLoc - m_MouseButtonDownLoc));            
               }*/

            /*   foreach (KeyValuePair<int, string> pair in m_MainDictionary)
               {
                   if (pair.Key < m_MouseButtonDownLoc)
                       m_TempDictionary.Add(pair.Key + offset, pair.Value);
                   else if (pair.Key > m_MouseButtonUpLoc)
                   {
                       if (pair.Value != "")
                       {
                           val = int.Parse(pair.Value);
                           tempVal = val - (Convert.ToInt32(ConvertPixelsToTime(m_MouseButtonUpLoc) - ConvertPixelsToTime(m_MouseButtonDownLoc)) / 1000);
                           temp = tempVal % 10;
                           temp = tempVal - temp;
                           pairVal = temp.ToString();
                           Console.WriteLine("ALUE OF PAIR " + pairVal);
                           m_TempDictionary.Add(pair.Key - (temp * 10), pairVal);
                       }
                       else
                           m_TempDictionary.Add(pair.Key, pair.Value);
                   }           
                }
            m_DeletedOffset += m_MouseButtonUpLoc - m_MouseButtonDownLoc;*/

            m_MouseButtonDownLoc = 0;
            m_MouseButtonUpLoc = 0;
            m_TempMouseMoveLoc = 0;
            m_IsDeleted = true;
            RepaintWaveform();
            double RecorderTime = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition)) /
Time.TIME_UNIT;
            if ((RecorderTime - ConvertPixelsToTime((m_X))) > 100)
            {
                int noOfLoops = (int)((RecorderTime - ConvertPixelsToTime((m_X))) / 100);
                int i = 0;
                while (i < noOfLoops)
                {
                    DrawWaveForm();
                    i++;
                }
            }
        }

        private void UpdateTimeToPixelDictionary(double time, int pixel)
        {
            if (!m_TimeToPixelMap.ContainsKey(time))
            {
                m_TimeToPixelMap.Add(time, pixel);
            }
            else if (m_TimeToPixelMap[time] <= 0)
            {
                m_TimeToPixelMap[time] = pixel;
            }
            
        }

        private void deselectSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
           DeselectSelection();
        }

        public void DeselectSelection()
        {
            listOfSelctedPortion.Sort();
            listOfEndSelection.Sort();
            if (listOfSelctedPortion.Count > 0)
            {
                int tempMouseDown = listOfSelctedPortion[0];
                int tempMouseUp = listOfEndSelection[listOfEndSelection.Count - 1];
                PaintWaveform(tempMouseDown - 10, tempMouseUp + 10, false);
            }
            else
            {
                PaintWaveform(m_MouseButtonDownLoc - 10, m_MouseButtonUpLoc + 10, false);
            }
            listOfSelctedPortion.Clear();
            listOfEndSelection.Clear();            
            m_MouseButtonDownLoc = 0;
            m_MouseButtonUpLoc = 0;
            
        }
        }
}