using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    /// <summary>
    /// Flow layout panel for blocks inside a strip.
    /// </summary>
    public partial class BlockLayout : FlowLayoutPanel
    {
        public BlockLayout()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // Calling the base class OnPaint
            base.OnPaint(pe);
            StripIndexSelection selection = Selection as StripIndexSelection;
            if (selection != null)
            {
                int index = selection.Index;
                int w = index == 0 ? Controls[0].Margin.Left : Controls[index - 1].Margin.Right;
                Point[] points = new Point[4];
                int x = XForIndex(index);
                points[0] = new Point(x - w, 0);
                points[1] = new Point(x - w, Height - 1);
                points[2] = new Point(x + w - 1, 0);
                points[3] = new Point(x + w - 1, Height - 1);
                pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                pe.Graphics.FillPolygon(((Strip)Parent).ColorSettings.BlockLayoutSelectedBrush, points);
            }
        }

        private NodeSelection Selection
        {
            get { return Parent is Strip ? ((Strip)Parent).Selection : null; }
        }

        private int XForIndex(int index)
        {
            return index == 0 ? 0 :
                Controls[index - 1].Location.X + Controls[index - 1].Width + Controls[index - 1].Margin.Right;
        }

        // Get the index for an X position in the track layout
        public int IndexForX(int x)
        {
            int index = 0;
            foreach (Control c in Controls)
            {
                if (c.Location.X > x) return index;
                ++index;
            }
            return index;
        }

    }
}
