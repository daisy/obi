using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AudioLib;

namespace Obi.Audio
{
	public partial class PeakMeterForm : Form
	{
		public PeakMeterForm()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Property overridden to show peak meter without moving keyboard focus to it
                /// </summary>
        protected override bool ShowWithoutActivation
        {
            get            {                 return true;             }
        }

		public VuMeter SourceVuMeter
		{
			get
			{
				return mGraphicalPeakMeter.SourceVuMeter;
			}
			set
			{
				mGraphicalPeakMeter.SourceVuMeter = value;
			}
		}
	}
}