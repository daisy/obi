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
        private Font myFont = new Font("Microsoft Sans Serif", 7);
        private EmptyNode m_ExistingPhrase = null;
        private int m_Counter = 0;
        private int m_LocalTime = 0;
        private ColorSettings m_ColorSettings;
        private ColorSettings m_ColorSettingsHC;
        private bool m_IsColorHighContrast = false;
      //  private Dictionary<int, short> m_PointMMinChannelMap = new Dictionary<int, short>();
      //  private Dictionary<int, short> m_PointMaxChannelMap = new Dictionary<int, short>();
        private Pen pen = new Pen(Color.Black);
        private List<int> listOfXLocation = new List<int>();
        private List<int> listOfMinChannel1 = new List<int>();
        private List<int> listOfMaxChannel1 = new List<int>();
        private List<int> listOfMinChannel2 = new List<int>();
        private List<int> listOfMaxChannel2 = new List<int>();
       // private bool m_IsResized = false;
        private int m_X = 0;
        private int m_XCV = 0;
        private Dictionary<int, string> m_DictionaryEmpNode = new Dictionary<int, string>();
        private double timeOfAssetMilliseconds = 0;
        private bool m_IsMaximized = false;
        private bool m_IsToBeRepainted = false;
     //   private bool m_IsResize = false;
        
        public Waveform_Recording()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            this.Height = Convert.ToInt32(104 * m_ZoomFactor);
            g = this.CreateGraphics();
            if (m_ProjectView != null && m_ProjectView.TransportBar.RecordingPhrase != null)
                m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
            Location = new Point(0, Location.Y);
            Size = new Size(10000, Height);
            
        }

        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
        }

        public bool invertColor
        {
            get { return m_IsColorHighContrast; }
            set
            {
                m_IsColorHighContrast = value;
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
                    m_ProjectView.TransportBar.Recorder.PcmDataBufferAvailable += new AudioRecorder.PcmDataBufferAvailableHandler(OnPcmDataBufferAvailable_Recorder);
                    m_ColorSettings = m_ProjectView.ObiForm.Settings.ColorSettings;
                    m_ColorSettingsHC = m_ProjectView.ObiForm.Settings.ColorSettingsHC;
                    this.BackColor = m_ColorSettings.WaveformBackColor;
                    pen_ChannelMono = m_ColorSettings.WaveformMonoPen;
                    pen_Channel1 = m_ColorSettings.WaveformChannel1Pen;
                    pen_Channel2 = m_ColorSettings.WaveformChannel2Pen;
                    pen_WaveformBaseLine = m_ColorSettings.WaveformBaseLinePen;
                    m_ProjectView.TransportBar.PhraseCreatedEvent += new EventHandler(Phrase_Created_Event);
                    if (m_IsColorHighContrast)
                    {
                        pen_HighContrastMono = m_ColorSettingsHC.WaveformMonoPen;
                        pen_HighContrastChannel1 = m_ColorSettingsHC.WaveformChannel1Pen;
                        pen_HighContrastChannel2 = m_ColorSettingsHC.WaveformChannel2Pen;
                    }
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

        public void ZoomWaveform()
        {
            if (m_ContentView != null)
                this.Size = new Size(m_ContentView.Width, Convert.ToInt32(104 * m_ZoomFactor));
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            
            m_XCV = m_X + Location.X;
            int diff = m_XCV - (m_ContentView.Width / 2 + 50);
            int newXLocation = (m_X - (m_ContentView.Width / 2 + 50)) * -1;
            if (Math.Abs(diff) > 2)
            {
                this.Location = new Point(newXLocation, this.Location.Y);
                m_IsToBeRepainted = false;
            }
            if (newXLocation > 0)
            {
                m_IsToBeRepainted = false;
                Location = new Point(0, Location.Y);
            }
          
            
            if (m_VUMeter == null || m_ContentView == null) return;
            g = this.CreateGraphics();
            
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
            {
                if(!m_IsColorHighContrast)
                g.DrawLine(pen_ChannelMono, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                else
                g.DrawLine(pen_HighContrastMono, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                    new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));                
            }
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
            {
                if (!m_IsColorHighContrast)
                {
                    g.DrawLine(pen_Channel1, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    g.DrawLine(pen_Channel2, new Point(m_X, Height - (int)Math.Round(((minChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                           new Point(m_X, Height - (int)Math.Round(((maxChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                }
                else
                {
                    g.DrawLine(pen_HighContrastChannel1, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    g.DrawLine(pen_HighContrastChannel2, new Point(m_X, Height - (int)Math.Round(((minChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                           new Point(m_X, Height - (int)Math.Round(((maxChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                }
            }
            
            g.DrawLine(pen_WaveformBaseLine, 0, Height / 2, m_ContentView.Width, Height / 2);            
            
            g.DrawLine(pen_WaveformBaseLine, 0, Height / 2, Width, Height / 2);
            string text = "";
            Pen newPen = new Pen(SystemColors.Control);
            
            timeOfAssetMilliseconds =
                   (double)m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition) /
                   Time.TIME_UNIT;
            int timeInSeconds = Convert.ToInt32(timeOfAssetMilliseconds / 1000);
            m_Counter++;
            if (m_Counter == 10)
            {
                g.DrawLine(newPen, m_X, 0, m_X, Height);
                if (timeInSeconds % 10 == 0 && m_LocalTime != timeInSeconds)
                {
                    text = timeInSeconds.ToString();
                    g.DrawString(text, myFont, Brushes.Gray, m_X, Height - 12);
                    if (!m_DictionaryEmpNode.ContainsKey(m_X))
                        m_DictionaryEmpNode.Add(m_X, text);
                    m_LocalTime = timeInSeconds;
                }
                else
                {
                    if (!m_DictionaryEmpNode.ContainsKey(m_X))
                        m_DictionaryEmpNode.Add(m_X, "");
                }
           
                m_Counter = 0;
            }

            if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring && m_ExistingPhrase != m_ProjectView.TransportBar.RecordingPhrase)
                CreatePageorPhrase(m_X);
            if ((m_ContentView.Width - m_XCV) > 600)
            {              
            //    this.Location = new Point(m_ContentView.Location.X, Location.Y);
                  // m_IsMaximized = false;
            }            
             m_X++;
             listOfXLocation.Add(m_X); 
             listOfMinChannel1.Add((int)minChannel1);
             listOfMaxChannel1.Add((int)maxChannel1);
             listOfMinChannel2.Add((int)minChannel2);
             listOfMaxChannel2.Add((int)maxChannel2);
         }

        private void Waveform_Recording_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                timer1.Start();
                if(m_ContentView != null)
                m_X = m_ContentView.Width / 2 + 50;
                Location = new Point(-400, Location.Y);
            }
            else
            {
                Location = new Point(-400, Location.Y);
                timer1.Stop();
            }
            listOfMaxChannel1.Clear();
            listOfMinChannel1.Clear();
            listOfMinChannel2.Clear();
            listOfMaxChannel2.Clear();
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
int channels = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels ;
int channel = 0;
                int frameSize = m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.BlockAlign ;
                short [] samples = new short[e.PcmDataBufferLength];
                Buffer.BlockCopy(e.PcmDataBuffer,0,samples,0,e.PcmDataBufferLength ) ;

            //for (int i = 0 ; i < (int)Math.Ceiling(e.PcmDataBufferLength/ (float)frameSize); i += frameSize)
                for (int i = channel; i < (int)Math.Ceiling(e.PcmDataBufferLength/ (float)frameSize); i += channels)
            {
                
                                if (samples [i] < minChannel1) minChannel1 = samples [i];
                                if (samples[i] > maxChannel1) maxChannel1 = samples [i];
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
                m_AmpValue =  m_Amp[0] + (int) (m_Amp[1] *  Math.Pow(8, m_Amp.Length));
               // m_PointMinChannelMap.Add(x_Loc, minChannel1);
               // m_PointMaxChannelMap.Add(x_Loc, maxChannel1);
               
            }
        }

        private void Waveform_Recording_Resize(object sender, EventArgs e)
        {
            RepaintWaveform();           
        }

        private void RepaintWaveform()
        {
            Console.WriteLine("CLLIG TOO MANT TIMEW");
            int count = 0;
            int secondsMark = 0;
            int localCount = 0;
            Font myFont = new Font("Microsoft Sans Serif", 7);
            Pen newPen = new Pen(SystemColors.Control);
            
            if (m_IsMaximized)
            {
                m_IsMaximized = false;
                return;
            }
            m_IsMaximized = true;
            int counterMin = listOfMinChannel1.Count;
            int x = 0;
            int counterMax = listOfMaxChannel2.Count;

            if (m_ContentView != null)
                x = m_ContentView.Width / 2 + 50;
            if (counterMin == 0)
                return;
            if (counterMin < 5)
            { }
            else
            {
                timer1.Stop();
                for (int i = counterMin - 1; i >= 0; i--)
                {
                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
                    {
                        g.DrawLine(pen_Channel1, new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMinChannel1[i] - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMaxChannel1[i] - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    }

                    if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
                    {
                        g.DrawLine(pen_Channel1, new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMinChannel2[i] - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMaxChannel2[i] - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    }
                    count++;

                    if(m_DictionaryEmpNode.ContainsKey(listOfXLocation[i]))
                    {
                        g.DrawString(m_DictionaryEmpNode[listOfXLocation[i]], myFont, Brushes.Gray, listOfXLocation[i], 0);
                        g.DrawLine(pen, listOfXLocation[i], 0, listOfXLocation[i], Height);
                        if (m_DictionaryEmpNode[listOfXLocation[i]] == "")
                            g.DrawLine(newPen, listOfXLocation[i], 0, listOfXLocation[i], Height);
                        else if (m_DictionaryEmpNode[listOfXLocation[i]].EndsWith("0"))
                            g.DrawString(m_DictionaryEmpNode[listOfXLocation[i]], myFont, Brushes.Gray, listOfXLocation[i], Height - 15);
                    }

                  /*  if (this.Location.X < 0 && 
                        (this.Location.X * -1) < listOfXLocation[i])
                    {
                        Console.WriteLine("Breaking refresh loop at:" + i) ;
                        break;
                    }*/
                }
                m_IsMaximized = false;
                timer1.Start();
            }
        }

        public void Phrase_Created_Event(object sender, EventArgs e)
        {
            int lastItemInList = (int)(m_ProjectView.TransportBar.phDetectorPhraseTimingList[m_ProjectView.TransportBar.phDetectorPhraseTimingList.Count - 1] / 100);
            int location = lastItemInList + (m_ContentView.Width / 2 + 50);

            if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring)
            CreatePageorPhrase(location);
        }

        private void CreatePageorPhrase(int xLocation)
        {
            string text = "";
            if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Page)
                text = "Page" + m_ProjectView.TransportBar.RecordingPhrase.PageNumber.ToString();
            else if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Plain)
                text = "Phrase";
            g.DrawLine(pen, xLocation, 0, xLocation, Height);
            g.DrawString(text, myFont, Brushes.Black, xLocation, 0);
            m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
            if (!m_DictionaryEmpNode.ContainsKey(xLocation))
                m_DictionaryEmpNode.Add(xLocation, text);
           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(m_IsToBeRepainted)
            RepaintWaveform();
            m_IsToBeRepainted = true;
            m_IsMaximized = false;
        }               
    }
}



