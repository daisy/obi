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
            AudioBlock block = Parent as AudioBlock;
            if (block != null)
            {
                if (block.SelectionX >= 0)
                {
                    pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush,
                        new Rectangle(block.SelectionX - Height / 2, 0, Height, Height));
                }
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
