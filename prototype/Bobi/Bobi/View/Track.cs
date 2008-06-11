using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class Track : Control
    {
        private Size baseSize;               // size at zoom factor 1
        private urakawa.core.TreeNode node;  // node for this track
        private bool selected;               // selected flag
        private double zoom;                 // zoom factor

        /// <summary>
        /// Create a new, empty track.
        /// </summary>
        public Track()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.baseSize = new Size(512, 144);
            Selected = false;
            Zoom = 1.0;
            this.node = null;
        }

        /// <summary>
        /// Create a new track for a node.
        /// </summary>
        /// <param name="node"></param>
        public Track(urakawa.core.TreeNode node): this()
        {
            this.node = node;
        }

        /// <summary>
        /// Base size (at zoom factor 1.)
        /// </summary>
        public Size BaseSize
        {
            get { return this.baseSize; }
            set
            {
                this.baseSize = value;
                Zoom = this.Zoom;  // resize the window using the current zoom factor
            }
        }

        /// <summary>
        /// Node for this track.
        /// </summary>
        public urakawa.core.TreeNode Node { get { return this.node; } }

        /// <summary>
        /// Get or set the selected flag for this track.
        /// </summary>
        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
                if (Parent is ProjectView) SetColorScheme(((ProjectView)Parent).ColorScheme);
                if (this.selected && Parent is ProjectView) ((ProjectView)Parent).ScrollControlIntoView(this);
            }
        }

        /// <summary>
        /// Set the colors for this track.
        /// </summary>
        public void SetColorScheme(ColorSettings scheme)
        {
            BackColor = this.selected ? scheme.TrackSelectedBackColor : scheme.TrackBackColor;
        }

        /// <summary>
        /// Zoom factor.
        /// </summary>
        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value;
                this.Size = new Size((int)Math.Round(baseSize.Width * value), (int)Math.Round(baseSize.Height * value));
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: Add custom paint code here
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        // Propagate selection upward
        private void SelectUp()
        {
            if (!this.selected)
            {
                ProjectView view = Parent as ProjectView;
                if (view != null)
                {
                    Selected = true;
                    view.SelectFromBelow(this.node);
                }
            }
        }

        private void Track_Click(object sender, EventArgs e) { SelectUp(); }

        private void Track_ParentChanged(object sender, EventArgs e)
        {
            if (Parent is ProjectView) SetColorScheme(((ProjectView)Parent).ColorScheme);
        }
    }
}
