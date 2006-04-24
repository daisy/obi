using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WaveBits
{
    public partial class WaveStrip : UserControl
    {
        private WaveFile mWaveFile = null;  // the wave file to display

        public WaveFile WaveFile
        {
            get { return mWaveFile; }
            set
            {
                mWaveFile = value;
                if (mWaveFile != null)
                {
                    label.Text = mWaveFile.Label;     // update the label accordingly
                    wavePanel.mWaveFile = mWaveFile;  // and the wave panel
                    wavePanel.Refresh();
                }
            }
        }

        public WaveStrip()
        {
            InitializeComponent();
        }
    }
}
