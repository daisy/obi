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
        HelpProvider helpProvider1;

		public PeakMeterForm()
		{
			InitializeComponent();
            this.TopMost = true;
            helpProvider1 = new HelpProvider();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Graphical Peak Meter.htm");  
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