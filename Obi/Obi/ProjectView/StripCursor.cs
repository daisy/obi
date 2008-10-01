using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class StripCursor : Control, ISelectableInContentView
    {
        private bool mHighlighted;
        private SectionNode mSection;

        public StripCursor()
        {
            InitializeComponent();
        }

        public StripCursor(SectionNode section): this()
        {
            Highlighted = false;
            mSection = section;
        }

        public Strip Strip
        {
            get { return Parent == null ? null : Parent.Parent as Strip; }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Highlighted)
            {
                Strip strip = Strip;
                if (strip != null)
                {
                    Point[] points = new Point[4];
                    points[0] = new Point(0, 0);
                    points[1] = new Point(0, Height - 1);
                    points[2] = new Point(Width - 1, 0);
                    points[3] = new Point(Width - 1, Height - 1);
                    pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    pe.Graphics.FillPolygon(strip.ColorSettings.BlockLayoutSelectedBrush, points);
                }
            }
            base.OnPaint(pe);
        }

        #region ISelectableInContentView Members

        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value;
                Invalidate();
            }
        }

        public ObiNode ObiNode { get { return mSection; } }

        public void SetSelectionFromContentView(NodeSelection selection) { Highlighted = selection != null; }

        #endregion

        private static readonly double HeightToWidthRatio = 10.0;

        public void SetHeight(int h)
        {
            Height = h - Margin.Vertical;
            Width = (int)Math.Round(h / HeightToWidthRatio);
        }

        public ColorSettings ColorSettings
        {
            set 
            {
                BackColor = Color.Magenta;
                // BackColor = value.StripBackColor;
                if (Highlighted) Invalidate();
            }
        }

        private void StripCursor_Click(object sender, EventArgs e)
        {
            Strip.SetSelectedIndexFromStripCursor(this);
            System.Diagnostics.Debug.Print("*** <" + AccessibleName + ">");
        }

        /// <summary>
        /// Set the accessible name for this cursor given its index.
        /// </summary>
        public void SetAccessibleName(int index)
        {
            AccessibleName = string.Format(Localizer.Message("strip_cursor_accessible_name"), index, index + 1);
        }
    }
}
