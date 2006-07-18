using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class StripManagerPanel : UserControl //, ICoreTreeView
    {
        public StripManagerPanel()
        {
            InitializeComponent();
        }

        public void Add(Strips.ParStrip strip)
        {
            NRParStrip nrpar = new NRParStrip();
            nrpar.Model = strip;
            mFlowLayoutPanel.Controls.Add(nrpar);
        }
    }
}
