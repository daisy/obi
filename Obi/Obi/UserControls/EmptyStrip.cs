using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.UserControls
{
    public partial class EmptyStrip : UserControl
    {
        protected StripPanel mStripPanel;

        private bool resize_resizing;    // resizing in progress
        private int resize_from;         // resizing started from this point
        private int resize_size_before;  // size before resizing

        public delegate void ResizedHandler(object sender, ResizedEventArgs e);
        public event ResizedHandler Resized;

        public EmptyStrip()
        {
            InitializeComponent();
            mStripPanel = new StripPanel();
        }

        // Shrink to minimum size
        private void mSizeHandle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (Width > MinimumSize.Width + mStripPanel.MinimumSize.Width)
            {
                resize_size_before = Width;
                Width = MinimumSize.Width + mStripPanel.MinimumSize.Width;
                OnResized();
            }
        }

        // Starting resizing
        private void mSizeHandle_MouseDown(object sender, MouseEventArgs e)
        {
            resize_size_before = Width;
            resize_from = e.X;
            resize_resizing = true;
        }

        // Ending resizing
        private void mSizeHandle_MouseUp(object sender, MouseEventArgs e)
        {
            if (resize_size_before != Width) OnResized();
            resize_resizing = false;
        }

        public void OnResized()
        {
            Size before = new Size(resize_size_before, Height);
            Resized(this, new ResizedEventArgs(before, Size));
        }

        // When resizing, make sure that we don't make the controller smaller that it can be.
        private void mSizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (resize_resizing)
            {
                int diff = e.X - resize_from;
                if (Width + diff < MinimumSize.Width + mStripPanel.MinimumSize.Width)
                    diff = MinimumSize.Width + mStripPanel.MinimumSize.Width - Width;
                Width += diff;
            }
        }
    }
}
