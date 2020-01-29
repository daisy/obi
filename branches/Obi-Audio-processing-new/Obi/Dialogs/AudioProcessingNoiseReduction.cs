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

        public bool IsFfmpegAfftdnNoiseReduction
        {
            get
            {
                return m_rbFfmpegAfftdnNoiseReduction.Checked;
            }
        }

        public bool IsFfmpegAnlmdnNoiseReduction
        {
            get
            {
                return m_rbFfmpegAnlmdnNoiseReduction.Checked;
            }
        }

        public bool IsFfmpegNoiseReduction
        {
            get
            {
                return m_rbFfmpegNoiseReduction.Checked;
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

        public decimal NoiseReductionInDb
        {
            get
            {
                return m_SetNoiseReduction.Value;
            }
        }

        public decimal NoiseFloorInDb
        {
            get
            {
                return m_SetNoiseFloor.Value;
            }
        }

        public decimal DenoisingStrength
        {
            get
            {
                return m_SetDenoisingStrength.Value;
            }
        }

        private void m_rbFfmpegAfftdnNoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rbFfmpegAfftdnNoiseReduction.Checked)
            {
                m_SetNoiseReduction.Enabled = true;
                m_SetNoiseFloor.Enabled = true;

                m_SetDenoisingStrength.Enabled = false;
                m_tb_HighPass.Enabled = false;
                m_tb_LowPass.Enabled = false;
            }
        }

        private void m_rbNAudioNoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rbNAudioNoiseReduction.Checked)
            {
                m_tb_HighPass.Enabled = true;
                m_tb_LowPass.Enabled = true;

                m_SetDenoisingStrength.Enabled = false;
                m_SetNoiseReduction.Enabled = false;
                m_SetNoiseFloor.Enabled = false;
            }
        }

        private void m_rbFfmpegNoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rbFfmpegNoiseReduction.Checked)
            {
                m_tb_HighPass.Enabled = true;
                m_tb_LowPass.Enabled = true;

                m_SetDenoisingStrength.Enabled = false;
                m_SetNoiseReduction.Enabled = false;
                m_SetNoiseFloor.Enabled = false;
            }
        }

        private void m_rbFfmpegAnlmdnNoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rbFfmpegAnlmdnNoiseReduction.Checked)
            {
                m_SetDenoisingStrength.Enabled = true;

                m_SetNoiseReduction.Enabled = false;
                m_SetNoiseFloor.Enabled = false;
                m_tb_HighPass.Enabled = false;
                m_tb_LowPass.Enabled = false;
            }
        }
    }
}
