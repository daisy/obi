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
        private Pen br_ChannelMono;
        private Pen br_Channel1;
        private Pen br_Channel2;
        private Pen br_HighContrastMono;
        private Pen br_HighContrastChannel1;
        private Pen br_HighContrastChannel2;
        private Pen br2;
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
        private bool m_IsResized = false;
        private int m_X = 0;
        private int m_XCV = 0;
        private int m_OldXLocation = 0;
        private Dictionary<int, string> m_DictionarySeconds = new Dictionary<int, string>();
        private Dictionary<int, string> m_DictionaryEmpNode = new Dictionary<int, string>();
        
        public Waveform_Recording()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            this.Height = Convert.ToInt32(104 * m_ZoomFactor);
            g = this.CreateGraphics();
            if (m_ProjectView != null && m_ProjectView.TransportBar.RecordingPhrase != null)
                m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
            Location = new Point(-400, Location.Y);
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
                    br_ChannelMono = m_ColorSettings.WaveformMonoPen;
                    br_Channel1 = m_ColorSettings.WaveformChannel1Pen;
                    br_Channel2 = m_ColorSettings.WaveformChannel2Pen;
                    br2 = m_ColorSettings.WaveformBaseLinePen;
                    if (m_IsColorHighContrast)
                    {
                        br_HighContrastMono = m_ColorSettingsHC.WaveformMonoPen;
                        br_HighContrastChannel1 = m_ColorSettingsHC.WaveformChannel1Pen;
                        br_HighContrastChannel2 = m_ColorSettingsHC.WaveformChannel2Pen;
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
           
           
          
       //     int m_X = m_ContentView.Width / 2 + 50;
                       
        //    x_Loc = m_ContentView.Width / 2 + 50 + (-1 * Location.X);
          
           /* xNewLoc =  x_Loc - (m_ContentView.Width / 2 + 50);
            Console.WriteLine("LOCA  " + m_OldXLocation + "NEW LOCA " + x_Loc);
            if (m_IsResized && Math.Abs(m_OldXLocation) - Math.Abs(x_Loc) > 100)
            {
                this.Location = new Point(xNewLoc, Location.Y);
              //  x_Loc = m_ContentView.Width / 2 + 50 + (-1 * Location.X);
            }*/

            m_XCV = m_X + Location.X;
            int diff = m_XCV - (m_ContentView.Width / 2 + 50);
            int newXLocation = (m_X - (m_ContentView.Width / 2 + 50)) * -1;
            
            if (Math.Abs(diff) > 2)
                this.Location = new Point(newXLocation, this.Location.Y);
            if (newXLocation > 0)
                Location = new Point(0, Location.Y);
          
            
            if (m_VUMeter == null || m_ContentView == null) return;
            g = this.CreateGraphics();
            //Console.WriteLine("height waveform " + (Height - (int)Math.Round(((min - short.MinValue) * Height) / (float)ushort.MaxValue)) + " : " + (Height - (int)Math.Round(((max - short.MinValue) * Height) / (float)ushort.MaxValue)));
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels == 1)
            {
                if(!m_IsColorHighContrast)
                g.DrawLine(br_ChannelMono, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                else
                g.DrawLine(br_HighContrastMono, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                    new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));                
            }
            if (m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.NumberOfChannels > 1)
            {
                if (!m_IsColorHighContrast)
                {
                    g.DrawLine(br_Channel1, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    g.DrawLine(br_Channel2, new Point(m_X, Height - (int)Math.Round(((minChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                           new Point(m_X, Height - (int)Math.Round(((maxChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                }
                else
                {
                    g.DrawLine(br_HighContrastChannel1, new Point(m_X, Height - (int)Math.Round(((minChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                        new Point(m_X, Height - (int)Math.Round(((maxChannel1 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    g.DrawLine(br_HighContrastChannel2, new Point(m_X, Height - (int)Math.Round(((minChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)),
                           new Point(m_X, Height - (int)Math.Round(((maxChannel2 - short.MinValue) * Height) / (float)ushort.MaxValue)));
                }
            }
            
            g.DrawLine(br2, 0, Height / 2, m_ContentView.Width, Height / 2);            
            
            g.DrawLine(br2, 0, Height / 2, Width, Height / 2);
            string text = "";
            Font myFont = new Font("Microsoft Sans Serif", 7);
            Pen newPen = new Pen(SystemColors.Control);
            
            double timeOfAssetMilliseconds =
                   (double)m_ProjectView.TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(m_ProjectView.TransportBar.Recorder.CurrentDurationBytePosition) /
                   Time.TIME_UNIT;
            int time = Convert.ToInt32(timeOfAssetMilliseconds / 1000);
            m_Counter++;
            if (m_Counter == 10)
            {
                g.DrawLine(newPen, m_X, 0, m_X, Height);
                m_Counter = 0;
            }

                if (time % 10 == 0 && m_LocalTime != time )
                {
                    text = time.ToString();
                    g.DrawString(text, myFont, Brushes.Gray, m_X, Height - 12);
                    m_DictionarySeconds.Add(m_X, text);
                    m_LocalTime = time;
                }
            

            if (m_ProjectView.TransportBar.CurrentState != TransportBar.State.Monitoring && m_ExistingPhrase != m_ProjectView.TransportBar.RecordingPhrase)
            {
                if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Page)
                    text = "Page" + m_ProjectView.TransportBar.RecordingPhrase.PageNumber.ToString();
                else if (m_ProjectView.TransportBar.RecordingPhrase.Role_ == EmptyNode.Role.Plain)
                    text = "Phrase";
                g.DrawLine(pen, m_X, 0, m_X, Height);
                g.DrawString(text, myFont, Brushes.Black, m_X, 0);
                m_ExistingPhrase = m_ProjectView.TransportBar.RecordingPhrase;
                m_DictionaryEmpNode.Add(m_X, text);
             //   m_DictionarySeconds.Add(m_X, text);
            }
            
        //     Location = new Point(Location.X - 1, Location.Y);
             
             this.Width = this.Width + 150;
             m_IsResized = true;
             m_X++;
             listOfXLocation.Add(m_X); 
             listOfMinChannel1.Add((int)minChannel1);
             listOfMaxChannel1.Add((int)maxChannel1);
             m_OldXLocation = m_X;             
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
            int count = 0;
            int secondsMark = 0;
            int localCount = 0;
            Font myFont = new Font("Microsoft Sans Serif", 7);
            Pen newPen = new Pen(SystemColors.Control);
            if (m_IsResized)
            {
                m_IsResized = false;
                return; 
            }
            int counterMin = listOfMinChannel1.Count;
            int x = 0;
            
            if(m_ContentView != null)
             x = m_ContentView.Width / 2 + 50;
            if (counterMin == 0)
                return;
            if (counterMin < 5)
            { }
            else
            {    
                System.Media.SystemSounds.Asterisk.Play();
                timer1.Stop();
                for (int i = counterMin - 1; i >= 0; i--)
                {
                    g.DrawLine(br_Channel1, new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMinChannel1[i] - short.MinValue) * Height) / (float)ushort.MaxValue)),
                    new Point(listOfXLocation[i], Height - (int)Math.Round(((listOfMaxChannel1[i] - short.MinValue) * Height) / (float)ushort.MaxValue)));
                    count++;

                    if (count == 10)
                    {
                        g.DrawLine(newPen, listOfXLocation[i], 0, listOfXLocation[i], Height);
                        count = 0;
                        secondsMark++;
                    }
                    if (secondsMark % 10 == 0 && localCount != secondsMark)
                    {
                        foreach (KeyValuePair<int, string> pair in m_DictionarySeconds)
                        {
                            if (m_DictionarySeconds.ContainsKey(pair.Key))
                            {
                                g.DrawString(pair.Value, myFont, Brushes.Gray, pair.Key, Height - 15);
                            }
                        }
                        foreach (KeyValuePair<int, string> pair in m_DictionaryEmpNode)
                        {
                            if (m_DictionaryEmpNode.ContainsKey(pair.Key))
                            {
                                g.DrawString(pair.Value, myFont, Brushes.Gray, pair.Key, 0);
                            }
                        }
                    }
                }
                timer1.Start();
            }
        }
    }
}
