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
            AudioBlock block = Parent as AudioBlock;
            if (block != null)
            {
                AudioSelection selection = block.Selection as AudioSelection;
                if (selection != null)
                {
                    int from = block.XForTime(selection.From < selection.To ? selection.From : selection.To);
                    int to = block.XForTime(selection.To > selection.From ? selection.To : selection.From);
                    if (selection.IsRange)
                    {
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush, new Rectangle(from, 0, Height, Height));
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush, new Rectangle(to - Height + 1, 0, Height, Height));
                    }
                    else
                    {
                        pe.Graphics.FillRectangle(block.Colors.AudioSelectionBrush,
                            new Rectangle(block.XForTime(selection.At) - Height / 2, 0, Height, Height));
                    }
                }
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}
