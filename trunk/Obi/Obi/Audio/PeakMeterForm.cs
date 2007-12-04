using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Audio
{
	public partial class PeakMeterForm : Form
	{
		public PeakMeterForm()
		{
			InitializeComponent();
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