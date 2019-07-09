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

        public Obi.Audio.AudioProcessing.AudioProcessingKind AudioProcessNaudio
        {
            get
            {
                int index = m_cb_Process.SelectedIndex;
                return index == 0 ? Obi.Audio.AudioProcessing.AudioProcessingKind.Amplify :
                    index == 1 ? Audio.AudioProcessing.AudioProcessingKind.FadeIn :
                    index == 2 ? Audio.AudioProcessing.AudioProcessingKind.FadeOut :
                    Audio.AudioProcessing.AudioProcessingKind.Normalize;
            }
        }


        public float AudioProcessingParameter
        {
            get
            {
                return (float) m_numericUpDown1.Value;
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
            if (index == 1 || index == 2 || index == 3)
            {
                m_numericUpDown1.Enabled = false;
            }
            else
            {
                m_numericUpDown1.Enabled = true;
            }
        }
    }
}