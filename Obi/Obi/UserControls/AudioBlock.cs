using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using VirtualAudioBackend;
using urakawa.core;

namespace Obi.UserControls
{
    public partial class AudioBlock : UserControl
    {
        public string Annotation
        {
            set
            {
                mAnnotationLabel.Text = value;
            }
        }

        public string Time
        {
            set
            {
                mTimeLabel.Text = value;
            }
        }

        public AudioBlock()
        {
            InitializeComponent();
        }
    }
}
