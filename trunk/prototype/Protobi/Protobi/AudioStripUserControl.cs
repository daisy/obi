using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class AudioStripUserControl : Protobi.SeqStripUserControl
    {
        private WaveFile mWaveFile;

        public WaveFile WaveFile
        {
            get { return mWaveFile; }
            set
            {
                mWaveFile = value;
                wavePanel.WaveFile = value;
            }
        }

        public AudioStripUserControl()
        {
            InitializeComponent();
        }

        public AudioStripUserControl(SeqStrip controller)
        {
            InitializeComponent();
            InitializeMembers(controller);
            mWaveFile = null;
            // KLUDGY
            MinimumSize = new Size(200, 120);
            Width = MinimumSize.Width;
            Height = MinimumSize.Height;
            ContentsSizeChanged();
        }
    }
}

