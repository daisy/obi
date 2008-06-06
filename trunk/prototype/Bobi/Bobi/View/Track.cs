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
        private Size baseSize;  // size at zoom factor 1
        private double zoom;    // zoom factor

        public Track()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.baseSize = new Size(512, 144);
            this.BackColor = Color.AliceBlue;
            Zoom = 1.0;
        }

        [Browsable(true)]
        public Size BaseSize
        {
            get { return this.baseSize; }
            set
            {
                this.baseSize = value;
                Zoom = this.Zoom;  // resize the window using the current zoom factor
            }
        }

        [Browsable(true)]
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
    }
}
