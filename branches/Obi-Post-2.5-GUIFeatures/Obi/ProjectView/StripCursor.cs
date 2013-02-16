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
        private bool mHighlighted;  // highlight flag

        private static readonly double HeightToWidthRatio = 20;
        private int m_MouseDownLoc = 0;


        /// <summary>
        /// Constructor used by the designer.
        /// </summary>
        public StripCursor()
        {
            InitializeComponent();
            Highlighted = false;
        }


        /// <summary>
        /// Set the color settings of the cursor.
        /// </summary>
        public ColorSettings ColorSettings
        {
            set
            {
                BackColor = value.StripBackColor;
                if (Highlighted) Invalidate();
            }
        }

        /// <summary>
        /// Get or set the highlight status.
        /// </summary>
        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Get the node of the parent strip.
        /// </summary>
        public ObiNode ObiNode { get { return Strip.ObiNode; } }


        /// <summary>
        /// Get the parent strip.
        /// </summary>
        public Strip Strip
        {
            get { return Parent == null ? null : Parent.Parent as Strip; }
        }


        /// <summary>
        /// Set the accessible name for this cursor given its index.
        /// </summary>
        public void SetAccessibleNameForIndex(int index)
        {
            AccessibleName = string.Format(Localizer.Message("strip_cursor_accessible_name"), index, index + 1);
        }

        /// <summary>
        /// Set the height of the cursor to fit the parent layout given as the height parameter.
        /// </summary>
        public void SetHeight(int h)
        {
            Height = h - Margin.Vertical;
            Width = (int)Math.Round(h / HeightToWidthRatio);
        }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public void SetSelectionFromContentView(NodeSelection selection) { Highlighted = selection != null; }


        // Draw the selection shape.
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Highlighted)
            {
                if (Strip != null)
                {
                    Point[] points = new Point[4];
                    points[0] = new Point(0, 0);
                    points[1] = new Point(0, Height - 1);
                    points[2] = new Point(Width - 1, 0);
                    points[3] = new Point(Width - 1, Height - 1);
                    pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    pe.Graphics.FillPolygon(Strip.ColorSettings.BlockLayoutSelectedBrush, points);
                }                
            }
            else if (this.BackColor == SystemColors.Highlight && Strip != null &&  !Strip.Highlighted)
            {
                this.BackColor = Strip.BackColor;
                Parent.BackColor = Strip.BackColor;
            }
            base.OnPaint(pe);
        }

        // Select on click.
        private void StripCursor_Click(object sender, EventArgs e) { Strip.SetSelectedIndexFromStripCursor(this); }

        internal void UpdateColors()
        {
            BackColor = Parent.BackColor;
        }

        private void StripCursor_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseDownLoc = 1;
        }

        private void StripCursor_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_MouseDownLoc == 0)
                return;
            if (Strip != null)
                Strip.SetAnimationCursor(e.X, e.Y);
        }

        private void StripCursor_MouseUp(object sender, MouseEventArgs e)
        {
            m_MouseDownLoc = 0;
            Strip.SetAnimationCursor();
        }
    }
}
