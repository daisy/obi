using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioProcessingNoiseReduction : Form
    {
        public AudioProcessingNoiseReduction()
        {
            InitializeComponent();
        }

        public bool IsNAudioNoiseReduction
        {
            get
            {
                return m_rbNAudioNoiseReduction.Checked;
            }
        }

    }
}
