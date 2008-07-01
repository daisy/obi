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

        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                if (ParentStrip != null)
                {
                    BackColor = mSelected ? ParentStrip.ColorSettings.StripCursorSelectedBackColor : Parent.BackColor;
                    Invalidate();
                }
            }
        }


        public ObiNode ObiNode { get { return mStrip.Node; } }
        public Strip Strip { get { return mStrip; } }
        public NodeSelection SelectionFromView { set { Selected = value != null; } }

        #endregion
    }
}
