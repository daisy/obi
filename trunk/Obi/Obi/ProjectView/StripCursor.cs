using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class StripCursor : Control, ISelectableInStripView
    {
        private bool mSelected;
        private Strip mStrip;

        public StripCursor()
        {
            InitializeComponent();
            mSelected = false;
        }

        public StripCursor(Strip strip): this()
        {
            mStrip = strip;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (mSelected)
            {
                pe.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(0, 0, Width, Height));
            }
            base.OnPaint(pe);
        }

        #region ISelectableInStripView Members

        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                Invalidate();
            }
        }


        public ObiNode ObiNode { get { return mStrip.Node; } }
        public Strip Strip { get { return mStrip; } }
        public NodeSelection SelectionFromView { set { Selected = value != null; } }

        #endregion
    }
}
