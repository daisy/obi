using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.media.data.audio;

namespace Zaboom
{
    public partial class WaveformPanel : UserControl
    {
        private PCMDataInfo info;
        private double scale;

        public WaveformPanel()
        {
            InitializeComponent();
        }

        public WaveformPanel(PCMDataInfo info)
        {
            InitializeComponent();
            this.info = info;
            scale = Width / info.getDuration().getTimeDeltaAsMillisecondFloat();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(Pens.Red, new Point(0, Height / 2), new Point(Width, Height / 2));
        }
    }
}
