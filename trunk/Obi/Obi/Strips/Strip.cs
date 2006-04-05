using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Strips
{
    /// <summary>
    /// Super class for all strips.
    /// </summary>
    public abstract class Strip
    {
        private bool mSelected;

        public bool Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                mSelected = value;
            }
        }

        public Strip()
        {
            mSelected = false;
        }
    }
}
