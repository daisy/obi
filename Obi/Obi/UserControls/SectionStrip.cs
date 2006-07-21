using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class SectionStrip : UserControl
    {
        public string Label
        {
            get
            {
                return mLabel.Text;
            }
            set
            {
                mLabel.Text = value;
            }
        }

        public SectionStrip()
        {
            InitializeComponent();
        }
    }
}
