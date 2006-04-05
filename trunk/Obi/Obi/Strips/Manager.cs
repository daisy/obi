using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Strips
{
    class Manager
    {
        private List<ParStrip> mPars;
        private Strip mSelected;

        public Strip Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                if (mSelected != null && mSelected != value) mSelected.Selected = false;
                mSelected = value;
                mSelected.Selected = true;
            }
        }

        public Manager()
        {
            mPars = new List<ParStrip>();
            mSelected = null;
        }

        public ParStrip AddNewParStrip()
        {
            ParStrip par = new ParStrip(Localizer.Message("new_strip_label"));
            mPars.Add(par);
            return par;
        }
    }
}
