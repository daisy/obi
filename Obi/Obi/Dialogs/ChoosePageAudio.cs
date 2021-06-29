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
    public partial class ChoosePageAudio : Form
    {
        private bool m_IsCancelButtonPressed = false;
        public ChoosePageAudio()
        {
            InitializeComponent();
        }

        public string RecordedAudioPath
        {
            get
            {
                return m_txtSelectRecordedAudio.Text;
            }
        }

        private void m_btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = Localizer.Message("audio_file_filter");
            if (select_File.ShowDialog() != DialogResult.OK) return;
            else
            {
                m_txtSelectRecordedAudio.Text = select_File.FileName;
            }
        }

        private void ChoosePageAudio_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || m_IsCancelButtonPressed)
                return;

            if (m_txtSelectRecordedAudio.Text == string.Empty)
            {
                MessageBox.Show(Localizer.Message("ChoosePageAudioFileNotSelected"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }

        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            m_IsCancelButtonPressed = true;
        }

    }
}
