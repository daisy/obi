using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Protobi
{
    public class StripManager
    {
        private StripManagerModel mModel;
        private FlowLayoutPanel mUserControl;

        public Strip Selected { get { return mModel.Selected; } }

        public StripManager(FlowLayoutPanel layout)
        {
            mUserControl = layout;
            mModel = new StripManagerModel();
        }

        public ParStrip AppendNewParStrip()
        {
            Console.WriteLine("Append new ParStrip >>> {0}", mModel.NextLabel);
            ParStrip strip = new ParStrip(this, mModel.NextLabel);
            mModel.Strips.Add(strip);
            mUserControl.Controls.Add(strip.UserControl);
            return strip;
        }

        /// <summary>
        /// Append an existing strip.
        /// </summary>
        /// <param name="stripController"></param>
        public void AppendStrip(Strip stripController)
        {
            mUserControl.Controls.Add(stripController.UserControl);
            mModel.Strips.Add(stripController);
        }

        public void RemoveLastStrip()
        {
            mModel.Strips[mModel.Strips.Count - 1].Selected = false;
            mModel.Strips[mModel.Strips.Count - 1].UserControl.Selected = false;
            mModel.Strips.RemoveAt(mModel.Strips.Count - 1);
            mUserControl.Controls.RemoveAt(mUserControl.Controls.Count - 1);
        }

        public void Select(Strip strip)
        {
            if (mModel.Selected != strip)
            {
                mModel.Selected = strip;
                mUserControl.Refresh();
                ((WorkAreaForm)mUserControl.Parent).EnableSelect();
            }
        }
    }

    /// <summary>
    /// The strip manager keeps track of container strips.
    /// </summary>
    public class StripManagerModel
    {
        List<Strip> mStrips;   // all the strips
        List<ParStrip> mPars;  // just the containers
        Strip mSelected;       // the currently selected strip

        public List<Strip> Strips { get { return mStrips; } }
        public string NextLabel { get { return String.Format(Localizer.GetString("strip_name"), mStrips.Count + 1); } }

        public Strip Selected
        {
            get
            {
                return mSelected;
            }
            set
            {
                if (mSelected != null) mSelected.Selected = false;
                mSelected = value;
            }
        }

        public StripManagerModel()
        {
            mStrips = new List<Strip>();
            mPars = new List<ParStrip>();
            mSelected = null;
        }
    }
}
