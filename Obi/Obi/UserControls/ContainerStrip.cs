using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class ContainerStrip : Obi.UserControls.EmptyStrip
    {
        public ContainerStrip()
        {
            InitializeComponent();
            mStripPanel = mContainerPanel;
        }

        public void ObserveStrip(Strips.ParStrip strip)
        {
            mContainerPanel.ObserveStrip(strip);
        }
    }
}

