using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UrakawaPrototype
{
    public partial class VuMeter : UserControl
    {
        public VuMeter()
        {
            InitializeComponent();
        }

        private void load(object sender, EventArgs e)
        {
            this.progressBar1.Value = 67;
            this.progressBar2.Value = 43;
            this.progressBar3.Value = 86;
        }
    }
}
