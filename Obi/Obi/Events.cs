using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Obi
{
    public class ResizedEventArgs: EventArgs
    {
        private Size mBefore;
        private Size mAfter;

        public Size Before
        {
            get
            {
                return mBefore;
            }
        }

        public Size After
        {
            get
            {
                return mAfter;
            }
        }

        public ResizedEventArgs(Size before, Size after)
        {
            mBefore = before;
            mAfter = after;
        }
    }

    public class LabelChangedEventArgs : EventArgs
    {
        private string mBefore;
        private string mAfter;
        private bool mAutomatic;  // true if the size changed automatically, i.e. not directly from the user

        public string Before
        {
            get
            {
                return mBefore;
            }
        }

        public string After
        {
            get
            {
                return mAfter;
            }
        }

        public LabelChangedEventArgs(string before, string after, bool automatic)
        {
            mBefore = before;
            mAfter = after;
            mAutomatic = automatic;
        }
    }
}
