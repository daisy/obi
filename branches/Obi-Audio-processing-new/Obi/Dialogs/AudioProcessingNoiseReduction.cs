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
            int highpass = 200;
            int lowpass = 3000;
            m_tb_HighPass.Text = highpass.ToString() ;
            m_tb_LowPass.Text = lowpass.ToString();
        }

        public bool IsNAudioNoiseReduction
        {
            get
            {
                return m_rbNAudioNoiseReduction.Checked;
            }
        }

        public float HighPass
        {
            get
            {
                float highPassVal = 200;
                bool isFloat = float.TryParse(m_tb_HighPass.Text, out highPassVal);
                return highPassVal;
            }
        }

        public float LowPass
        {
            get
            {
                float LowPassVal = 3000;
                bool isFloat = float.TryParse(m_tb_LowPass.Text, out LowPassVal);
                return LowPassVal;
            }
        }
    }
}
