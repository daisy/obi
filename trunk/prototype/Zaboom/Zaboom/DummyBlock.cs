using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class DummyBlock : Control
    {
        private Boolean selected;

        private static readonly Pen SELECTION_PEN = new Pen(Color.Blue, 3.0f);

        public DummyBlock()
        {
            InitializeComponent();
            selected = false;
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            if (selected)
            {
                pe.Graphics.DrawRectangle(SELECTION_PEN,
                    SELECTION_PEN.Width / 2.0f,
                    SELECTION_PEN.Width / 2.0f,
                    Width - SELECTION_PEN.Width,
                    Height - SELECTION_PEN.Width);
            }
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void DummyBlock_Click(object sender, EventArgs e)
        {
            Selected = !Selected;
        }
    }
}
