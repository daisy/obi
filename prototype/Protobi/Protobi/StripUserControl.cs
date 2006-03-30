using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public partial class StripUserControl : UserControl
    {
        private StripController mController;  // controller for this user control
        protected Label mLabel;               // the label of the strip
        protected bool mShowLabel;            // indicates whether the label is shown
        private Size mMinSize;                // minimum size of the strip (must be bigger than its contents)
        private bool mSelected;               // the strip is currently selected

        protected int min_width;              // absolute minimum width, excluding contents (but margins and handles)
        private int min_height;               // absolute minimum height, excluding contents other than label
        private bool resizing = false;        // persistent variables for resizing
        private int from_x;                   // the x coordinate of the point we started resizing from
        private int old_width;                // the width before resizing

        public Size MinSize { get { return mMinSize; } }

        public StripController Controller
        {
            set
            {
                mController = value;
                mLabel.Text = mController.Label;
            }
        }

        // When the label change, we should make sure that it fits in the strip by resizing the strip if necessary.
        // Note that there seems to be a bug when computing the width of a Japanese string.
        // Note also that inherited controls will probably recalculate their minimum length differently.
        public string Label
        {
            get { return mLabel.Text; }
            set
            {
                mLabel.Text = value;
                if (mShowLabel) mMinSize.Width = min_width + mLabel.Width;
            }
        }

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
        /// <param name="showLabel">Flag for showing the label or not.</param>
        public StripUserControl(StripController controller, bool showLabel)
        {
            InitializeComponent();
            InitializeMembers(controller, showLabel);
        }

        // The non-graphical constructor. Do not override the constructor as the call to InitializeComponent will
        // initialize the base components!
        protected void InitializeMembers()
        {
            mLabel = new Label();
            mLabel.AutoSize = true;
            mLabel.Margin = new Padding(4);
            mLabel.Text = "(No controller)";
            min_width = selectHandle.Width + sizeHandle.Width + layoutPanel.Margin.Right + layoutPanel.Margin.Left +
                mLabel.Margin.Right + mLabel.Margin.Left;
            min_height = Height;
            mMinSize = new Size(min_width, Height);
            mController = null;
        }

        /// <summary>
        /// Override this!
        /// </summary>
        /// <param name="controller">The controller of the user control.</param>
        /// <param name="showLabel">Flag for showing the label or not.</param>
        protected void InitializeMembers(StripController controller, bool showLabel)
        {
            InitializeMembers();
            mController = controller;
            mLabel.Text = controller.Label;
            mShowLabel = showLabel;
            if (mShowLabel)
            {
                layoutPanel.Controls.Add(mLabel);
                mMinSize.Width = min_width + mLabel.Width;
            }
        }

        private void sizeHandle_MouseDown(object sender, MouseEventArgs e)
        {
            resizing = true;
            from_x = e.X;
            old_width = Width;
        }

        private void sizeHandle_MouseUp(object sender, MouseEventArgs e)
        {
            if (old_width != Width)
            {
                ((WorkAreaForm)ParentForm).PushUndo(new ResizeStripCommand(mController, new Size(old_width, Height),
                    new Size(Width, Height)));
            }
            resizing = false;
        }

        private void sizeHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                int diff = e.X - from_x;
                if (Width + diff < MinSize.Width) diff = MinSize.Width - Width;
                Width += diff;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = mSelected ? Color.LightBlue : Color.LightGoldenrodYellow;
            base.OnPaint(e);
        }

        private void selectHandle_MouseClick(object sender, MouseEventArgs e)
        {
            mSelected = true;
            mController.Select();
        }
    }
}
