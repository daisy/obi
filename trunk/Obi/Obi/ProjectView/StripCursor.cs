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
        private Strip mStrip;

        public StripCursor()
        {
            InitializeComponent();
            mHighlighted = false;
        }

        public StripCursor(Strip strip): this()
        {
            mStrip = strip;
        }


        private Strip ParentStrip
        {
            get
            {
                Control c = Parent;
                while (c != null && !(c is Strip)) c = c.Parent;
                return c as Strip;
            }
        }

        #region ISelectableInStripView Members

        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value;
                if (ParentStrip != null)
                {
                    BackColor = mHighlighted ? ParentStrip.ColorSettings.StripCursorSelectedBackColor : Parent.BackColor;
                    Invalidate();
                }
            }
        }


        public ObiNode ObiNode { get { return mStrip.Node; } }
        public Strip Strip { get { return mStrip; } }
        public NodeSelection SelectionFromView { set { Highlighted = value != null; } }

        #endregion
    }
}
