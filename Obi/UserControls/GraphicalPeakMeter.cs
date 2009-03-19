using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using AudioLib;
using Obi.Audio;

namespace Obi.UserControls
{
	/// <summary>
	/// Graphical Peak meter control based on the <see cref="AudioEngine.PPMeter.PPMeter"/> control
	/// </summary>
	public partial class GraphicalPeakMeter : UserControl
	{
        private double[] mPeakValues;
        private int mPeakArrayLength;
        private bool mSourcePeaksIsNull  ;
        private AudioLib.Events.VuMeter.PeakOverloadEventArgs mPeakOverloadObject  ;
        //private int m_DieOutCounter;

		public GraphicalPeakMeter()
		{
			InitializeComponent();
            mPeakValues = new double[2];
            mPeakArrayLength = 1;
            mSourcePeaksIsNull = true ;
            mPeakOverloadObject = null ;
            //m_DieOutCounter = 0;
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
                    if ( mUpdateGUITimer.Enabled )  mUpdateGUITimer.Stop ();
						mSourceVuMeter.UpdatePeakMeter -= new AudioLib.Events.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent -= new AudioLib.Events.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
						mSourceVuMeter.PeakOverload -= new AudioLib.Events.VuMeter.PeakOverloadHandler(SourceVuMeter_PeakOverload);
					}
					mSourceVuMeter = value;
					if (mSourceVuMeter != null)
					{
						mSourceVuMeter.UpdatePeakMeter += new AudioLib.Events.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent += new AudioLib.Events.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
						mSourceVuMeter.PeakOverload += new AudioLib.Events.VuMeter.PeakOverloadHandler(SourceVuMeter_PeakOverload);
                        mUpdateGUITimer.Start ();
                        					}
				}
			}
		}

		void SourceVuMeter_PeakOverload(object sender, AudioLib.Events.VuMeter.PeakOverloadEventArgs e)
		{
        mPeakOverloadObject = e;
			//mPPMeter.SetPeakOverloadCount(e.Channel - 1, mPPMeter.GetPeakOverloadCount(e.Channel - 1) + 1);
		}

        void SourceVuMeter_ResetEvent(object sender, AudioLib.Events.VuMeter.ResetEventArgs e)
        {
            if (this.InvokeRequired)
            {
                AudioLib.Events.VuMeter.ResetHandler d = new AudioLib.Events.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
                this.Invoke(d, sender, e);
            }
            else
            {
                for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                {
                    mPPMeter.SetValue(i, Double.NegativeInfinity);
                }
                mPPMeter.ForceFullFallback();
                mPeakOverloadObject = null;
            }
        }

        void SourceVuMeter_UpdatePeakMeter ( object sender, AudioLib.Events.VuMeter.UpdatePeakMeter e )
            {
            if (e.PeakValues == null) mSourcePeaksIsNull = true;
            else mSourcePeaksIsNull = false;

            if (mSourceVuMeter != null && e.PeakValues != null && e.PeakValues.Length > 0)
                {


                mPeakArrayLength = e.PeakValues.Length;
                for (int i = 0; i < e.PeakValues.Length; i++)
                    {
                    mPeakValues[i] = e.PeakValues[i];
                    }
                }
            }

        private void UpdateGui ()
            {
            if (this.InvokeRequired)
            {
                //AudioLib.Events.VuMeter.UpdatePeakMeterHandler d = new AudioLib.Events.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                //this.Invoke(d, sender, e);
            }
            else
            {

                if (mSourcePeaksIsNull == true )
                {
                    for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                    {
                        mPPMeter.SetValue(i, Double.NegativeInfinity);
                    }
                }
                else
                {
                    if (mPeakArrayLength != mPPMeter.NumberOfChannels) mPPMeter.NumberOfChannels = mPeakArrayLength;
                    for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
                    {
                        mPPMeter.SetValue(i, mPeakValues[i]);
                    }
                }
                // if peak overload, update GUI for it
            if (mPeakOverloadObject != null)
                {
                mPPMeter.SetPeakOverloadCount ( mPeakOverloadObject.Channel - 1, mPPMeter.GetPeakOverloadCount ( mPeakOverloadObject.Channel - 1 ) + 1 );
                mPeakOverloadObject = null;
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
                mPPMeter.BarPadding = (int)(BarPaddingToWidthRatio * mPPMeter.Width);
            }
		}

		private float mFontToHeightRatio = 0.03f;
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

		private float mFontToWidthRatio = 0.075f;
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

		private float mBarPaddingToWidthRatio = 0.075f;
		public float BarPaddingToWidthRatio
		{
			get
			{
				return mBarPaddingToWidthRatio;
			}
			set
			{
				mBarPaddingToWidthRatio = value;
				if (mBarPaddingToWidthRatio < 0.01f) mBarPaddingToWidthRatio = 0.01f;
				UpdateFontSizeAndBarPadding();
			}
		}

		private void mPPMeter_PeakOverloadIndicatorClicked(object sender, AudioEngine.PPMeter.PeakOverloadIndicatorClickedEventArgs e)
		{
			mPPMeter.SetPeakOverloadCount(e.ChannelNumber, 0);
		}

        private void mUpdateGUITimer_Tick ( object sender, EventArgs e )
            {
            UpdateGui ();
                                    }
	}
}
