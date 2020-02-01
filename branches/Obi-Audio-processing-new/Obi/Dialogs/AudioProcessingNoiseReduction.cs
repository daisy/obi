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
        private bool m_IsSetNoiseLevelDoneFromNumericUpDownControl;
        private bool m_IsSetNoiseReductionDoneFromNumericUpDownControl;
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
                decimal SetNoiseFloorFromNoiseLevel = ((-20 - (-80)) * (m_SetNoiseLevelInPercent.Value / 100)) + (-80);
                return SetNoiseFloorFromNoiseLevel;
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
                m_SetNoiseReductionTrackBar.Enabled = true;
                m_SetNoiseLevelInPercent.Enabled = true;
                m_SetNoiseLevelTrackBar.Enabled = true;
                m_SelectPresetComboBox.Enabled = true;

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
                m_SetNoiseReductionTrackBar.Enabled = false;
                m_SetNoiseLevelInPercent.Enabled = false;
                m_SetNoiseLevelTrackBar.Enabled = false;
                m_SelectPresetComboBox.Enabled = false;
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
                m_SetNoiseReductionTrackBar.Enabled = false;
                m_SetNoiseLevelInPercent.Enabled = false;
                m_SetNoiseLevelTrackBar.Enabled = false;
                m_SelectPresetComboBox.Enabled = false;
            }
        }

        private void m_rbFfmpegAnlmdnNoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rbFfmpegAnlmdnNoiseReduction.Checked)
            {
                m_SetDenoisingStrength.Enabled = true;

                m_SetNoiseReduction.Enabled = false;
                m_SetNoiseReductionTrackBar.Enabled = false;
                m_SetNoiseLevelInPercent.Enabled = false;
                m_SetNoiseLevelTrackBar.Enabled = false;
                m_SelectPresetComboBox.Enabled = false;
                m_tb_HighPass.Enabled = false;
                m_tb_LowPass.Enabled = false;
            }
        }

        private void m_SetNoiseFloor_ValueChanged(object sender, EventArgs e)
        {
        }

        private void m_SetNoiseLevelInPercent_ValueChanged(object sender, EventArgs e)
        {

            m_IsSetNoiseLevelDoneFromNumericUpDownControl = true;
            m_SetNoiseLevelTrackBar.Value = (int)m_SetNoiseLevelInPercent.Value;
        }

        private void m_SetNoiseLevelTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if(!m_IsSetNoiseLevelDoneFromNumericUpDownControl)
            m_SetNoiseLevelInPercent.Value = m_SetNoiseLevelTrackBar.Value;
            m_IsSetNoiseLevelDoneFromNumericUpDownControl = false;
        }

        private void m_SetNoiseReductionTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (!m_IsSetNoiseReductionDoneFromNumericUpDownControl)
            {
                if (m_SetNoiseReductionTrackBar.Value != 0)
                    m_SetNoiseReduction.Value = m_SetNoiseReductionTrackBar.Value;
                else
                    m_SetNoiseReduction.Value = 0.01M;
            }
            m_IsSetNoiseReductionDoneFromNumericUpDownControl = false;
        }

        private void m_SetNoiseReduction_ValueChanged(object sender, EventArgs e)
        {
            m_IsSetNoiseReductionDoneFromNumericUpDownControl = true;
            m_SetNoiseReductionTrackBar.Value = (int)m_SetNoiseReduction.Value;
        }

        private void m_SelectPresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SelectPresetComboBox.SelectedIndex == 0)
            {
                m_SetNoiseLevelInPercent.Value = 100;
                m_SetNoiseReduction.Value = 50;
            }
            else if (m_SelectPresetComboBox.SelectedIndex == 1)
            {
                m_SetNoiseLevelInPercent.Value = 75;
                m_SetNoiseReduction.Value = 30;
            }
            else if (m_SelectPresetComboBox.SelectedIndex == 2)
            {
                m_SetNoiseLevelInPercent.Value = 50;
                m_SetNoiseReduction.Value = 10;
            }

        }
    }
}
