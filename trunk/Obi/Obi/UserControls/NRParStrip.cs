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
                mTitleLabel.Text = mModel.Label;
            }
        }

        public NRParStrip()
        {
            InitializeComponent();
        }
    }
}
