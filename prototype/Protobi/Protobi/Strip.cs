using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Protobi
{
    public class Strip
    {
        protected string mLabel;

        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        public Strip(string label)
        {
            mLabel = label;
        }
    }

    public class StripController
    {
        protected StripManagerController mManager;
        protected StripUserControl mUserControl;
        protected Strip mStrip;
        protected bool mSelected;

        public StripUserControl UserControl
        {
            get { return mUserControl; }
            set { mUserControl = value; }
        }

        public bool Selected
        {
            get { return mSelected; }
            set
            {
                mSelected = value;
                mUserControl.Selected = value;
            }
        }

        public string Label
        {
            get { return mStrip.Label; }
            set
            {
                mStrip.Label = value;
                mUserControl.Label = value;
            }
        }

        public Size MinimumSize { get { return mUserControl.MinimumSize; } }

        public StripController(StripManagerController manager, Strip strip)
        {
            mManager = manager;
            mStrip = strip;
            mUserControl = null;
            mSelected = false;
        }

        public void Select()
        {
            mSelected = true;
            mManager.Select(this);
        }
    }
}
