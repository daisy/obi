using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class WavePanel : UserControl
    {
        private static readonly Pen MONOPEN = new Pen(Color.FromArgb(192, 0, 0, 255), 1);
        private static readonly Pen STEREOPEN1 = new Pen(Color.FromArgb(128, 0, 0, 255), 1);
        private static readonly Pen STEREOPEN2 = new Pen(Color.FromArgb(128, 255, 0, 0), 1);
        private WaveFile mWaveFile = null;

        public WaveFile WaveFile
        {
            set
            {
                mWaveFile = value;
                Refresh();
            }
        }

        public WavePanel()
        {
            InitializeComponent();
        }

        private void WavePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.LightBlue, 0, (float)Math.Ceiling(Height / 2.0), Width, (float)Math.Ceiling(Height / 2.0));
            if (mWaveFile != null)
            {
                double spp = Math.Ceiling(mWaveFile.Samples / (double) Width);
                Pen pen1 = mWaveFile.Format.Channels == 1 ? MONOPEN : STEREOPEN1;
                Draw16Bits(e.Graphics, spp, pen1);
            }
        }

        private void Draw16Bits(Graphics g, double spp, Pen pen1)
        {
            int x = 0;       // current drawing position
            int sample = 0;  // sample counter
            while (sample < mWaveFile.Samples)
            {
                // Do the first channel
                short min = short.MaxValue;
                short max = short.MinValue;
                int samplesRead = 0;
                for (int s = sample; s < sample + spp && s < mWaveFile.Samples; s += mWaveFile.Format.Channels)
                {
                    short v = mWaveFile.Shorts[s];
                    if (v < min) min = v;
                    if (v > max) max = v;
                    ++samplesRead;
                }
                int ymin = Height - ((min - short.MinValue) * Height) / ushort.MaxValue;
                int ymax = Height - ((max - short.MinValue) * Height) / ushort.MaxValue;
                g.DrawLine(pen1, x, ymin, x, ymax);
                if (mWaveFile.Format.Channels == 2)
                {
                    // Do the second channel
                    min = short.MaxValue;
                    max = short.MinValue;
                    for (int s = sample + 1; s < sample + spp && s < mWaveFile.Samples; s += mWaveFile.Format.Channels)
                    {
                        short v = mWaveFile.Shorts[s];
                        if (v < min) min = v;
                        if (v > max) max = v;
                        ++samplesRead;
                    }
                    ymin = Height - ((min - short.MinValue) * Height) / ushort.MaxValue;
                    ymax = Height - ((max - short.MinValue) * Height) / ushort.MaxValue;
                    g.DrawLine(STEREOPEN2, x, ymin, x, ymax);
                }
                ++x;
                sample += samplesRead;
            }
        }

        private void WavePanel_SizeChanged(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
