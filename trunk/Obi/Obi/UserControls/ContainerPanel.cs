using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class ContainerPanel : Obi.UserControls.StripPanel
    {

        public ContainerPanel()
        {
            InitializeComponent();
        }

        public void ObserveStrip(Strips.ParStrip strip)
        {
            strip.LabelChanged += new Strips.ParStrip.LabelChangedHandler(LabelChanged);
        }

        private void LabelChanged(object sender, LabelChangedEventArgs e)
        {
            mLabel.Text = e.After;
            MinimumSize = new Size(mLabel.Width, MinimumSize.Height);
            if (Width < MinimumSize.Width)
            {
                Size before = Size;
                Width = MinimumSize.Width;
                ((ContainerStrip)Parent).OnResized();
            }
        }
    }
}

