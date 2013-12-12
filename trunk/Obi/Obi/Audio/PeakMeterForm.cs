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
        public bool flagFirstTimeInit = false;
        private int tempFormHeight = 0;

		public PeakMeterForm()
		{
			InitializeComponent();
            this.TopMost = true;
            mGraphicalPeakMeter.Dock = DockStyle.None;
            flagFirstTimeInit = true;
            this.chkOnTop.Location = new Point(this.chkOnTop.Location.X, mGraphicalPeakMeter.Height);
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
        public void GraphicalPeakMeterDocPropSet()
        {
            mGraphicalPeakMeter.Dock = DockStyle.None;
        }
        public void PeakMeterInit()
        {
            mGraphicalPeakMeter.Size = new Size(154, 580);
            this.chkOnTop.Location = new Point(this.chkOnTop.Location.X, mGraphicalPeakMeter.Height);
            //this.Size=new Size(170,640);
            //this.mGraphicalPeakMeter.Height = this.Height;
            //this.Height = this.Height + 10;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Peakmeter Size {0}", mGraphicalPeakMeter.Size);
            flagFirstTimeInit = false;


        }
        public void GraphicaPeakMeterSizeSet(Settings set)
        {
            mGraphicalPeakMeter.Size = set.GraphicalPeakMeterContolSize;
            this.chkOnTop.Location = new Point(this.chkOnTop.Location.X, mGraphicalPeakMeter.Height);
        }
        public void GraphicalPeakMeterSaveSettings(Settings set)
        {
            set.GraphicalPeakMeterContolSize = this.mGraphicalPeakMeter.Size;
        }

        private void chkOnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkOnTop.Checked)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }

        private void PeakMeterForm_ResizeBegin(object sender, EventArgs e)
        {
            tempFormHeight = this.Height;
            Console.WriteLine("Peakmeter resize begin event called");
        }

        private void PeakMeterForm_ResizeEnd(object sender, EventArgs e)
        {
            if (flagFirstTimeInit == false)
            {
                int diff = this.Height - tempFormHeight;
                Console.WriteLine("Difference is {0}", diff);
                if ((mGraphicalPeakMeter.Height + diff) > (chkOnTop.Height*2))
                {
                    this.mGraphicalPeakMeter.Height = mGraphicalPeakMeter.Height + diff;
                }
                else
                {
                   this.Height = tempFormHeight;
                }
                this.chkOnTop.Location = new Point(this.chkOnTop.Location.X, mGraphicalPeakMeter.Height);
            }
        }
	}
}