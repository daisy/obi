using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ExportAdvance : Form
    {
        public ExportAdvance()
        {
            InitializeComponent();
            m_comboBoxReplayGain.SelectedIndex = 0;
        }

        public bool OptionalReSample { get { return m_ChkResample.Checked; } }

        public string OptionalRePlayGain 
        { 
            get 
            {
                return m_comboBoxReplayGain.SelectedIndex == 0 ? null :
                    m_comboBoxReplayGain.SelectedIndex == 1 ? " --noreplaygain" :
                    m_comboBoxReplayGain.SelectedIndex == 2 ? " --replaygain-accurate" :
                    m_comboBoxReplayGain.SelectedIndex == 3 ? " --replaygain-fast": 
                    null;
            } 
        }

        public string OptionalChannelMode
        {
            get
            {
                return m_ComboBoxStereoMode.SelectedIndex == 0 ? "m" :
                    m_ComboBoxStereoMode.SelectedIndex == 1 ? "s" :
                    m_ComboBoxStereoMode.SelectedIndex == 2 ? "j" :
                    m_ComboBoxStereoMode.SelectedIndex == 3 ? "f" :
                    m_ComboBoxStereoMode.SelectedIndex == 4 ? "d" : null;
            }
        }

        public bool EnableAdvancedParameters { get { return this.DialogResult == DialogResult.OK; } }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

    }
}