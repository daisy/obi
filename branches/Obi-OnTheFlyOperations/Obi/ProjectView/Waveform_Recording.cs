using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Waveform_Recording : UserControl
    {
        private ContentView m_ContentView = null;
        private float m_ZoomFactor;
        private AudioLib.VuMeter m_VUMeter;
        private System.Drawing.Graphics g;
        private System.Drawing.Pen br1 = new System.Drawing.Pen(Color.White);
        Point point;
        private int x = 0;
        public Waveform_Recording()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            this.Height = Convert.ToInt32(104 * m_ZoomFactor);
            timer1.Start();
            point.X = this.Location.X;
        }

        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
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

        public void VuMeter_PeakMeterUpdated(object sender, AudioLib.VuMeter.PeakMeterUpdateEventArgs e)
        {
            
          /*  g = this.CreateGraphics();

            Point point = new Point((this.Location.X + this.Size.Height / 2), this.Location.Y + Convert.ToInt32(m_VUMeter.AverageAmplitudeDBValue));
            //g.DrawLine(br1, (this.Location.X + Convert(this.Size.Height/2)), this.Location.Y + Convert.ToInt32(m_VUMeter.AverageAmplitudeDBValue));
            g.DrawLine(br1, point.X, this.Location.Y, point.X, point.Y);
            */
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            g = this.CreateGraphics();
            int amp = 0;
            if (m_VUMeter != null)
                amp = Convert.ToInt32(m_VUMeter.AverageAmplitudeDBValue[0]) * (-1);
             g.DrawLine(br1, x, 0, x, amp);
             x++;                                     
        }
    }
}
