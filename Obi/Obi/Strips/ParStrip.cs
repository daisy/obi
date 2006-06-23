using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Strips
{
    public class ParStrip: Strip
    {
        private string mLabel;

        public delegate void LabelChangedHandler(object sender, LabelChangedEventArgs e);
        public event LabelChangedHandler LabelChanged;

        public string Label
        {
            get
            {
                return mLabel;
            }
            set
            {
                string old_label = mLabel;
                mLabel = value;
                LabelChanged(this, new LabelChangedEventArgs(old_label, mLabel, false));
            }
        }

        public ParStrip()
        {
            mLabel = null;
        }

        public ParStrip(string label)
        {
            mLabel = label;
        }

        public override string ToString()
        {
            return String.Format("<{0}> \"{1}\"", base.ToString(), mLabel);
        }
    }
}
