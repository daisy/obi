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
            this.txtReplayGain.Text = "--noreplaygain";
            this.txtStereoToMono.Text = "-a";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}