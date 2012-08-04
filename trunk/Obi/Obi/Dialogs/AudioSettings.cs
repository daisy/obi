using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioSettings : Form
    {
        public AudioSettings()
        {
            InitializeComponent();
            mcbAudioChannel.SelectedIndex = 0;
            mcbSampleRate.SelectedIndex = 0;
        }

        private void mbtnOK_Click(object sender, EventArgs e)
        {

        }

        private void mbtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
