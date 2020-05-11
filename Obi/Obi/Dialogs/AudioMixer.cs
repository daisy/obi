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
    public partial class AudioMixer : Form
    {
        public AudioMixer()
        {
            InitializeComponent();
        }

        private void m_btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = Localizer.Message("audio_file_filter");
            if (select_File.ShowDialog() != DialogResult.OK) return;
            else
            {
                m_txtSelectAudioForMixing.Text = select_File.FileName;
            }
        }

        public string AudioForMixing
        {
            get
            {
                return m_txtSelectAudioForMixing.Text;
            }
        }

        public decimal WeightOfAudio
        {
            get
            {
                return m_WeightOfSoundNumericUpDown.Value;
            }
        }

        public decimal DropoutTansition
        {
            get
            {
                return m_DropoutTransitionNumericUpDown.Value;
            }
        }

        public bool IsEndOfStreamDurationChecked
        {
            get
            {
                return m_cbStreamDuration.Checked;
            }
        }

    }
}
