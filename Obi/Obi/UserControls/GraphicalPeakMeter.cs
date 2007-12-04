using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Obi.Audio;

namespace Obi.UserControls
{
	/// <summary>
	/// Graphical Peak meter control based on the <see cref="AudioEngine.PPMeter.PPMeter"/> control
	/// </summary>
	public partial class GraphicalPeakMeter : UserControl
	{
		public GraphicalPeakMeter()
		{
			InitializeComponent();
		}

		private VuMeter mSourceVuMeter;

        protected override void OnHandleDestroyed(EventArgs e)
        {
            SourceVuMeter = null;
            Application.DoEvents();
            base.OnHandleDestroyed(e);
        }

		/// <summary>
		/// Gets or sets the <see cref="VuMeter"/> class providing the <see cref="VuMeter.UpdatePeakMeter"/> events
		/// updating the peak meter
		/// </summary>
		public VuMeter SourceVuMeter
		{
			get
			{
				return mSourceVuMeter;
			}
			set
			{
				if (mSourceVuMeter != value)
				{
					if (mSourceVuMeter != null)
					{
						mSourceVuMeter.UpdatePeakMeter -= new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent -= new Obi.Events.Audio.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
                    }
					mSourceVuMeter = value;
					if (mSourceVuMeter != null)
					{
						mSourceVuMeter.UpdatePeakMeter += new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent += new Obi.Events.Audio.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
					}
				}
			}
		}

        void SourceVuMeter_ResetEvent(object sender, Obi.Events.Audio.VuMeter.ResetEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Obi.Events.Audio.VuMeter.ResetHandler d = new Obi.Events.Audio.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
                this.Invoke(d, sender, e);
            }
            else
            {
                for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                {
                    mPPMeter.SetValue(i, Double.NegativeInfinity);
                }
                mPPMeter.ForceFullFallback();
            }
        }

        void SourceVuMeter_UpdatePeakMeter(object sender, Obi.Events.Audio.VuMeter.UpdatePeakMeter e)
		{
            if (this.InvokeRequired)
            {
                Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler d = new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                this.Invoke(d, sender, e);
            }
            else
            {

                if (e.PeakValues == null)
                {
                    for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                    {
                        mPPMeter.SetValue(i, Double.NegativeInfinity);
                    }
                }
                else
                {
                    if (e.PeakValues.Length != mPPMeter.NumberOfChannels) mPPMeter.NumberOfChannels = e.PeakValues.Length;
                    for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                    {
                        mPPMeter.SetValue(i, e.PeakValues[i]);
                    }
                }
            }
		}

		private void mPPMeter_Resize(object sender, EventArgs e)
		{
			UpdateFontSizeAndBarPadding();
		}

		private void UpdateFontSizeAndBarPadding()
		{
            if (this.InvokeRequired)
            {
                MethodInvoker d = new MethodInvoker(UpdateFontSizeAndBarPadding);
                this.Invoke(d);
            }
            else
            {
                float emSize = Math.Min(FontToHeightRatio * mPPMeter.Height, FontToWidthRatio * mPPMeter.Width);
                if (mPPMeter.Font.Size != emSize)
                {
                    mPPMeter.Font = new Font(mPPMeter.Font.FontFamily, emSize);
                }
                mPPMeter.BarPadding = (int)(BarPaddingToHeightRatio * mPPMeter.Height);
            }
		}

		private float mFontToHeightRatio = 0.075f;
		public float FontToHeightRatio
		{
			get
			{
				return mFontToHeightRatio;
			}
			set
			{
				mFontToHeightRatio = value;
				if (mFontToHeightRatio < 0.01f) mFontToHeightRatio = 0.01f;
				UpdateFontSizeAndBarPadding();
			}
		}

		private float mFontToWidthRatio = 0.03f;
		public float FontToWidthRatio
		{
			get
			{
				return mFontToWidthRatio;
			}
			set
			{
				mFontToWidthRatio = value;
				if (mFontToWidthRatio < 0.01f) mFontToWidthRatio = 0.01f;
				UpdateFontSizeAndBarPadding();
			}
		}

		private float mBarPaddingToHeightRatio = 0.075f;
		public float BarPaddingToHeightRatio
		{
			get
			{
				return mBarPaddingToHeightRatio;
			}
			set
			{
				mBarPaddingToHeightRatio = value;
				if (mBarPaddingToHeightRatio < 0.01f) mBarPaddingToHeightRatio = 0.01f;
				UpdateFontSizeAndBarPadding();
			}
		}
	}
}
