using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class ParStripUserControl : Protobi.StripUserControl
    {
        public ParStripUserControl()
        {
            InitializeComponent();
            InitializeMembers();
        }

        public ParStripUserControl(ParStrip controller)
        {
            InitializeComponent();
            InitializeMembers(controller);
        }

        public void AddStripUserControl(StripUserControl strip)
        {
            layoutPanel.Controls.Add(strip);
            ContentsSizeChanged();
        }

        /// <summary>
        /// Resize the strip so that all of its contents can be shown.
        /// </summary>
        public override void ContentsSizeChanged()
        {
            int w = label.Width;
            int h = 0;
            foreach (Control control in layoutPanel.Controls)
            {
                if (control.Width > w) w = control.Width;
                h += control.Height + control.Margin.Top + control.Margin.Bottom;
            }
            mMinSize.Width = w + MinimumSize.Width;
            mMinSize.Height = h + MinimumSize.Height;
            // Grow horizontally when necessary
            if (Width < mMinSize.Width) Width = mMinSize.Width;
            // Always make the height exactly fit
            if (Height != mMinSize.Height)
            {
                Height = mMinSize.Height;
                select_path = LeftPath(selectHandle.Width);
                size_path = RightPath(sizeHandle.Width);
            }
        }
    }
}