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
        private int selectionX;

        public CursorBar()
        {
            InitializeComponent();
            this.selectionX = -1;
        }

        public int BaseHeight { set { this.baseHeight = value; } }
        public double Zoom { set { Height = (int)Math.Round(this.baseHeight * value); Invalidate();  } }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            if (selectionX >= 0)
            {
                //int h = Height - 1;
                pe.Graphics.FillRectangle(Brushes.Green, new Rectangle(this.selectionX - Height / 2, 0, Height, Height));
                //int hh = Height / 2;
                //pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //pe.Graphics.DrawLine(Pens.Green, new Point(this.selectionX, 0), new Point(this.selectionX, h));
                //pe.Graphics.DrawLine(Pens.Green, new Point(this.selectionX, 0), new Point(this.selectionX + hh, hh));
                //pe.Graphics.DrawLine(Pens.Green, new Point(this.selectionX, h), new Point(this.selectionX + hh, hh));
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        public void SelectX(int x)
        {
            this.selectionX = x;
            Invalidate();
        }
    }
}
