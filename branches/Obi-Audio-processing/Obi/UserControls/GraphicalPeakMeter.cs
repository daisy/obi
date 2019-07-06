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
        private AudioLib.VuMeter.PeakOverloadEventArgs mPeakOverloadObject;
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


                    mSourceVuMeter.PeakMeterOverloaded -= new AudioLib.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
                    mSourceVuMeter.PeakMeterUpdated -= new AudioLib.VuMeter.PeakMeterUpdateHandler(CatchPeakMeterUpdateEvent);

						//mSourceVuMeter.UpdatePeakMeter -= new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent -= new AudioLib.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
						//mSourceVuMeter.PeakOverload -= new Obi.Events.Audio.VuMeter.PeakOverloadHandler(SourceVuMeter_PeakOverload);
					}
					mSourceVuMeter = value;
					if (mSourceVuMeter != null)
					{
                        mSourceVuMeter.PeakMeterOverloaded += new AudioLib.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
                        mSourceVuMeter.PeakMeterUpdated += new AudioLib.VuMeter.PeakMeterUpdateHandler(CatchPeakMeterUpdateEvent);

                        //mSourceVuMeter.UpdatePeakMeter += new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
                        mSourceVuMeter.ResetEvent += new AudioLib.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
                        //mSourceVuMeter.PeakOverload += new Obi.Events.Audio.VuMeter.PeakOverloadHandler(SourceVuMeter_PeakOverload);

                        mUpdateGUITimer.Start ();
                        					}
				}
			}
		}

        public void CatchPeakOverloadEvent(object sender, AudioLib.VuMeter.PeakOverloadEventArgs e)
		//void SourceVuMeter_PeakOverload(object sender, Obi.Events.Audio.VuMeter.PeakOverloadEventArgs e)
		{
        mPeakOverloadObject = e;
			//mPPMeter.SetPeakOverloadCount(e.Channel - 1, mPPMeter.GetPeakOverloadCount(e.Channel - 1) + 1);
		}

        private delegate void CatchResetEvent_Delegate();
        private void SourceVuMeter_ResetEvent(object sender, AudioLib.VuMeter.ResetEventArgs e) { CatchResetEvent(); }
        void CatchResetEvent()
        {
            if (this.InvokeRequired)
            {
                //Obi.Events.Audio.VuMeter.ResetHandler d = new Obi.Events.Audio.VuMeter.ResetHandler(SourceVuMeter_ResetEvent);
                //this.Invoke(d, sender, e);
                this.Invoke(new CatchResetEvent_Delegate(CatchResetEvent));
            }
            else
            {
            for (int i = 0; i < mPeakValues.Length; i++) mPeakValues[i] = double.NegativeInfinity;
            
            for (int i = 0; i < mPPMeter.NumberOfChannels; i++)
            {
                mPPMeter.SetValue(i, Double.NegativeInfinity);
            }
                mPPMeter.ForceFullFallback();
                mPeakOverloadObject = null;
            }
            
        }

        public void CatchPeakMeterUpdateEvent(object sender, AudioLib.VuMeter.PeakMeterUpdateEventArgs e)
        //void SourceVuMeter_UpdatePeakMeter ( object sender, Obi.Events.Audio.VuMeter.UpdatePeakMeter e )
            {
            
            //double channelValueLeft = 0;
            //double channelValueRight = 0;

            //if (e.PeakDb != null && e.PeakDb.Length > 0)
            //{
            //    channelValueLeft = e.PeakDb[0];

            //    if (e.PeakDb.Length > 1)
            //    {
            //        channelValueRight = e.PeakDb[1];
            //    }
            //    else
            //    {
            //        channelValueRight = channelValueLeft;
            //    }
                
            //    if ((channelValueLeft == Double.PositiveInfinity && e.PeakDb.Length > 0)
            //    || (channelValueLeft == Double.PositiveInfinity && e.PeakDb.Length > 1 && channelValueRight == Double.PositiveInfinity))
            //    {
            //        CatchResetEvent();
            //        return;
            //    }
            //}

            if (e.PeakDb == null) mSourcePeaksIsNull = true;
            else mSourcePeaksIsNull = false;

            if (mSourceVuMeter != null && e.PeakDb != null && e.PeakDb.Length > 0)
                {


                    mPeakArrayLength = e.PeakDb.Length;
                    for (int i = 0; i < e.PeakDb.Length; i++)
                    {
                        // AudioLib VuMeter is set to positive infinity by default so it is important to take care of it.
                        //mPeakValues[i] = e.PeakDb[i] == double.PositiveInfinity? double.NegativeInfinity: e.PeakDb[i];
                        mPeakValues[i] = e.PeakDb[i];
                    }
                }
            }

        private void UpdateGui ()
            {
            if (this.InvokeRequired)
            {
                //Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler d = new Obi.Events.Audio.VuMeter.UpdatePeakMeterHandler(SourceVuMeter_UpdatePeakMeter);
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
