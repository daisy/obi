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
        private void m_btnBrowseSecondAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = Localizer.Message("audio_file_filter");
            if (select_File.ShowDialog() != DialogResult.OK) return;
            else
            {
                m_txtSelectSecondAudioForMixing.Text = select_File.FileName;
            }
        }

        public string AudioForMixing
        {
            get
            {
                return m_txtSelectAudioForMixing.Text;
            }
        }

        public string SecondAudioForMixing
        {
            get
            {
                return m_txtSelectSecondAudioForMixing.Text;
            }
        }

        public decimal WeightOfAudio
        {
            get
            {
                return m_WeightOfSoundNumericUpDown.Value;
            }
        }
        public decimal WeightOfSecondAudio
        {
            get
            {
                return m_WeightOfSecondAudioSoundNumericUpDown.Value;
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

        public double DuartionOfMixingAudioAfterPhraseEnds
        {
            get
            {
                return (double)m_DurationOfMixingAudioNumericUpDown.Value * 1000;
            }
        }

        public bool IsDurationOfMixingAudioChecked
        {
            get
            {
                return m_cbDuationOfMixingAudio.Checked;
            }
        }

        public bool IsSecondAudioToMixSelected
        {
            get
            {
                return m_cblSelectSecondAudioForMixing.Checked;
            }
        }


        private void m_cbStreamDuration_CheckedChanged(object sender, EventArgs e)
        {
            if (m_cbStreamDuration.Checked)
            {
                m_cbDuationOfMixingAudio.Visible = m_DurationOfMixingAudioNumericUpDown.Visible = m_lblSecondsDurationOfAudioMixing.Visible = false;
            }
            else
            {
                m_cbDuationOfMixingAudio.Visible = m_DurationOfMixingAudioNumericUpDown.Visible = m_lblSecondsDurationOfAudioMixing.Visible = true;
            }
        }

        private void m_cbDuationOfMixingAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (m_cbDuationOfMixingAudio.Checked)
            {
                m_DurationOfMixingAudioNumericUpDown.Enabled = true;
            }
            else
            {
                m_DurationOfMixingAudioNumericUpDown.Enabled = false;
            }
        }

        private void m_cblSelectSecondAudioForMixing_CheckedChanged(object sender, EventArgs e)
        {
            if (m_cblSelectSecondAudioForMixing.Checked)
            {
                m_txtSelectSecondAudioForMixing.Enabled = true;
                m_btnBrowseSecondAudio.Enabled = true;
                m_lblWeightOfSecondAudioSound.Enabled = true;
                m_WeightOfSecondAudioSoundNumericUpDown.Enabled = true;
            }
            else
            {
                m_txtSelectSecondAudioForMixing.Enabled = false;
                m_btnBrowseSecondAudio.Enabled = false;
                m_lblWeightOfSecondAudioSound.Enabled = false;
                m_WeightOfSecondAudioSoundNumericUpDown.Enabled = false;
            }
        }

        private void AudioMixer_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool tempflag = false;
            if (m_txtSelectAudioForMixing.Text == string.Empty && m_txtSelectSecondAudioForMixing.Enabled == false)
            {
                MessageBox.Show(Localizer.Message("AudioMixingFileNotSelected"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                tempflag = true;
            }
            else if (m_txtSelectSecondAudioForMixing.Enabled == true && (m_txtSelectAudioForMixing.Text == string.Empty || m_txtSelectSecondAudioForMixing.Text == string.Empty))
            {
                if (m_txtSelectAudioForMixing.Text == string.Empty)
                {
                    MessageBox.Show(Localizer.Message("AudioMixingFileNotSelected"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (m_txtSelectAudioForMixing.Text != string.Empty && m_txtSelectSecondAudioForMixing.Text == string.Empty)
                {
                    MessageBox.Show(Localizer.Message("AudioMixingSecondFileNotSelected"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                tempflag = true;
            }
            if (tempflag)
            {
                e.Cancel = true;
            }

        }


    }
}
