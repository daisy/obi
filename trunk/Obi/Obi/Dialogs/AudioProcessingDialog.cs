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
        public AudioProcessingDialog()
        {
            InitializeComponent();
            m_cb_Process.SelectedIndex = 0;
            m_txt_info.Text = Localizer.Message("AudioProcessing_InfoText");
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
    }
}