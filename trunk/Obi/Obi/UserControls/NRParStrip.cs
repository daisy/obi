using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class NRParStrip : UserControl
    {
        private Strips.ParStrip mModel;  // the model for this strip

        public Strips.ParStrip Model
        {
            get
            {
                return mModel;
            }
            set
            {
                mModel = value;
                labelBox.Text = mModel.Label;
            }
        }

        public NRParStrip()
        {
            InitializeComponent();
        }

        private void labelBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
