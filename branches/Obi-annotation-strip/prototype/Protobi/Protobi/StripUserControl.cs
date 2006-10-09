using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class StripUserControl : UserControl
    {
        protected Strip mController;  // controller for this user control
        protected Size mMinSize;      // minimum size of the strip (must be bigger than its contents)
        protected bool mSelected;     // the strip is currently selected

        protected bool resizing = false;  // persistent variables for resizing
        protected int from_x;             // the x coordinate of the point we started resizing from
        protected int old_width;          // the width before resizing

        protected GraphicsPath select_path;  // precomputed path for drawing the selection tab
        protected GraphicsPath size_path;    // and for the resizing tab

        public Size MinSize { get { return mMinSize; } }

        // When the label change, we should make sure that it fits in the strip by resizing the strip if necessary.
        // Note that there seems to be a bug when computing the width of a Japanese string.
        // Note also that inherited controls will probably recalculate their minimum length differently.
        public string Label
        {
            get { return label.Text; }
            set
            {
                label.Text = value;
                ContentsSizeChanged();
            }
        }

        // Set the selected property
        public bool Selected { set { mSelected = value; } }

        // Should not be used, except for the interface builder
        protected StripUserControl()
        {
            InitializeComponent();
            InitializeMembers();
        }

        /// <summary>
        /// Create a new user control.
        /// </summary>
        /// <param name="controller">The corresponding controller.</param>
        public StripUserControl(Strip controller)
        {
            InitializeComponent();
            InitializeMembers(controller);
        }

        // The non-graphical constructor. Do not override the constructor as the call to InitializeComponent will
        // initialize the base components!
        protected virtual void InitializeMembers()
        {
            MinimumSize = new Size(selectHandle.Width + sizeHandle.Width + label.Margin.Left + label.Margin.Right,
                label.Margin.Top + label.Height + label.Margin.Bottom);
            mMinSize = MinimumSize;
            mMinSize.Height *= 2;
            mController = null;
            select_path = LeftPath(selectHandle.Width);
            size_path = RightPath(sizeHandle.Width);
        }

        /// <summary>
        /// Override this!
        /// </summary>
        /// <param name="controller">The controller of the user control.</param>
        /// <param name="showLabel">Flag for showing the label or not.</param>
        protected void InitializeMembers(Strip controller)
        {
            InitializeMembers();
            mController = controller;
            label.Text = controller.Label;
            ContentsSizeChanged();
        }

        // Resize the control by dragging the size handle on the right. It cannot be narrower than its contents.

        protected virtual void sizeHandle_MouseDown(object sender, MouseEventArgs e)
        {
            resizing = true;
            from_x = e.X;
            old_width = Width;
        }

        protected virtual void sizeHandle_MouseUp(object sender, MouseEventArgs e)
        {
            if (old_width != Width)
            {
                ((WorkAreaForm)ParentForm).PushUndo(new ResizeStripCommand(mController, new Size(old_width, Height), Size));
            }
            resizing = false;
        }

        private void sizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                int diff = e.X - from_x;
                if (Width + diff < mMinSize.Width) diff = mMinSize.Width - Width;
                Width += diff;
            }
        }

        private void selectHandle_MouseClick(object sender, MouseEventArgs e)
        {
            mSelected = true;
            mController.Select();
        }

        /// <summary>
        /// Resize the strip so that all of its contents can be shown.
        /// </summary>
        public virtual void ContentsSizeChanged()
        {
            mMinSize.Width = MinimumSize.Width + label.Width;
            // Grow horizontally when necessary
            if (Width < mMinSize.Width) Width = mMinSize.Width;
            if (Height != mMinSize.Height)
            {
                Height = mMinSize.Height;
                select_path = LeftPath(selectHandle.Width);
                size_path = RightPath(sizeHandle.Width);
            }
        }

        // Painting the control

        private void selectHandle_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(Brushes.CornflowerBlue, select_path);
            e.Graphics.DrawPath(Pens.Black, select_path);
        }

        public virtual void StripUserControl_Paint(object sender, PaintEventArgs e)
        {
            int x = selectHandle.Width - 1;
            int w = Width - selectHandle.Width - sizeHandle.Width + 1;
            e.Graphics.FillRectangle(mSelected ? Brushes.LightBlue : Brushes.LightGoldenrodYellow, x, 0, w, Height - 1);
            e.Graphics.DrawRectangle(Pens.Black, x, 0, w, Height - 1);
        }
        
        private void sizeHandle_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.FillPath(Brushes.CornflowerBlue, size_path);
            e.Graphics.DrawPath(Pens.Black, size_path);
        }

        /// <summary>
        /// Compute the path of the rounded edges for the left handle.
        /// </summary>
        /// <param name="r">Radius of the corners/width of the handle.</param>
        /// <returns>The path to draw the handle.</returns>
        protected GraphicsPath LeftPath(int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 2 * r, 2 * r, 180, 90);
            path.AddLine(r - 1, 0, r - 1, Height - 1);
            path.AddArc(0, Height - 2 * r - 1, 2 * r, 2 * r, 90, 90);
            //path.AddLine(0, Height - 2 * r, 0, r);
            path.AddLine(0, Height - r - 1, 0, r - 1);
            return path;
        }

        /// <summary>
        /// Compute the path of the rounded edges for the right handle.
        /// </summary>
        /// <param name="r">Radius of the corners/width of the handle.</param>
        /// <returns>The path to draw the handle.</returns>
        protected GraphicsPath RightPath(int r)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLine(r - 1, Height - r - 1, r - 1, r - 1);
            path.AddArc(-r - 1, Height - 2 * r - 1, 2 * r, 2 * r, 0, 90);
            path.AddLine(0, 0, 0, Height - 1);
            path.AddArc(-r - 1, 0, 2 * r, 2 * r, 270, 90);
            return path;
        }
    }
}
