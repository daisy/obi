using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class CursorBar : Control
    {
        private int baseHeight;

        public CursorBar()
        {
            InitializeComponent();
        }

        public int BaseHeight { set { this.baseHeight = value; } }
        public double Zoom { set { Height = (int)Math.Round(this.baseHeight * value); Invalidate();  } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pe.Graphics.DrawLine(Pens.Black, new Point(0, 0), new Point(0, Height - 1));
            pe.Graphics.DrawLine(Pens.Black, new Point(0, 0), new Point(Height / 2, Height / 2));
            pe.Graphics.DrawLine(Pens.Black, new Point(0, Height - 1), new Point(Height / 2, Height / 2));
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}
