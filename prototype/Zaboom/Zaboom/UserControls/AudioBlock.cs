using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Zaboom.UserControls
{
    public partial class AudioBlock : UserControl
    {
        public AudioBlock()
        {
            InitializeComponent();
        }

        public WaveformPanel Waveform { get { return waveformPanel; } }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            // select
        }

        private void waveformPanel_SizeChanged(object sender, EventArgs e)
        {
            Width = waveformPanel.Width;
        }
    }
}
