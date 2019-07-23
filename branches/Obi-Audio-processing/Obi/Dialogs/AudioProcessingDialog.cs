using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioProcessingDialog : Form
    {
        public AudioProcessingDialog(Settings settings)
        {
            InitializeComponent();
            m_cb_Process.SelectedIndex = 0;
            m_txt_info.Text = Localizer.Message("AudioProcessing_InfoText");
            m_InfoToolTip.SetToolTip(m_txt_info, m_txt_info.Text);

            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Audio/Audio processing.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig            
            }
        }

        public Obi.Audio.AudioFormatConverter.AudioProcessingKind AudioProcess
        {
            get
            {
                int index = m_cb_Process.SelectedIndex;
                return index == 0 ? Obi.Audio.AudioFormatConverter.AudioProcessingKind.Amplify :
                    index == 1 ? Audio.AudioFormatConverter.AudioProcessingKind.Normalize :
                    Audio.AudioFormatConverter.AudioProcessingKind.SoundTouch;
            }
        }

        //public Obi.Audio.AudioProcessing.AudioProcessingKind AudioProcessNaudio
        //{
        //    get
        //    {
        //        int index = m_cb_Process.SelectedIndex;
        //        return index == 0 ? Obi.Audio.AudioProcessing.AudioProcessingKind.Amplify :
        //            index == 1 ? Audio.AudioProcessing.AudioProcessingKind.FadeIn :
        //            index == 2 ? Audio.AudioProcessing.AudioProcessingKind.FadeOut :
        //            index == 3 ? Audio.AudioProcessing.AudioProcessingKind.Normalize :
        //            Audio.AudioProcessing.AudioProcessingKind.NoiseReduction;
        //    }
        //}


        public float AudioProcessingParameter
        {
            get
            {
                return (float) m_numericUpDown1.Value;
            }
        }

        public int NoiseReductionParameter
        {
            get
            {
                return (Int32.Parse(m_tb_NoiseReductionFreqency.Text));
            }
        }
        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_cb_Process_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = m_cb_Process.SelectedIndex;
            if (index == 1 || index == 2 || index == 3 || index == 4)
            {
                m_numericUpDown1.Visible = false;
                if (index == 4)
                {
                    m_tb_NoiseReductionFreqency.Visible = true;
                    m_tb_NoiseReductionFreqency.Location = m_numericUpDown1.Location;
                }
            }
            else
            {
                m_numericUpDown1.Visible = true;
                m_tb_NoiseReductionFreqency.Visible = false;
            }
        }

        private void m_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           if (m_numericUpDown1.Value > 0 && m_numericUpDown1.Value <= (decimal)0.25)
                m_AmplifyParameter.Value = -3;
            else if (m_numericUpDown1.Value > (decimal)0.25 && m_numericUpDown1.Value <= (decimal)0.50)
                m_AmplifyParameter.Value = -2;
            else if (m_numericUpDown1.Value > (decimal)0.50 && m_numericUpDown1.Value <= (decimal)0.75)
                m_AmplifyParameter.Value = -1;
            else if (m_numericUpDown1.Value > (decimal)0.75 && m_numericUpDown1.Value <= (decimal)1)
                m_AmplifyParameter.Value = 0;
            else if (m_numericUpDown1.Value > 1 && m_numericUpDown1.Value <= 2)
                m_AmplifyParameter.Value = 1;
            else if (m_numericUpDown1.Value > 2 && m_numericUpDown1.Value <= 3)
                m_AmplifyParameter.Value = 2;
            else if (m_numericUpDown1.Value > 3 && m_numericUpDown1.Value <= 4)
                m_AmplifyParameter.Value = 3;
        }

        private void m_AmplifyParameter_ValueChanged(object sender, EventArgs e)
        {
            float value = 0;
            if (m_AmplifyParameter.Value == 0)
            {
                value = 1;
            }
            else if (m_AmplifyParameter.Value == 1)
            {
                value = 2;
            }
            else if (m_AmplifyParameter.Value == 2)
            {
                value = 3;
            }
            else if (m_AmplifyParameter.Value == 3)
            {
                value = 4;
            }
            else if (m_AmplifyParameter.Value == -1)
            {
                value = (float)0.75;
            }
            else if (m_AmplifyParameter.Value == -2)
            {
                value = (float)0.50;
            }
            else if (m_AmplifyParameter.Value == -3)
            {
                value = (float)0.25;
            }
            m_numericUpDown1.Value = (decimal)value;
        }
    }
}