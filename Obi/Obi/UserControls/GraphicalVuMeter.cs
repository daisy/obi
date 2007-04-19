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
    public partial class GraphicalVuMeter : UserControl
    {
        public GraphicalVuMeter()
        {
            InitializeComponent();
            
        }

        private Audio.VuMeter mVuMeter;
        private bool m_ResizeParentForm = false;

        // flag to indicate overload for overload light to display
        bool m_OverloadLightEnabled = false;

        // variable to count number of timer ticks to skip for an operation
        int BackPaintCount = 0;

        public Audio.VuMeter VuMeter
        {
            get
            {
                return mVuMeter;
            }
            set
            {
                mVuMeter = value;

                if (mVuMeter != null)
                {
                    mVuMeter.PeakOverload += new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
                    mVuMeter.UpdateForms += new Events.Audio.VuMeter.UpdateFormsHandler(CatchUpdateForms);
                    mVuMeter.ResetEvent += new Events.Audio.VuMeter.ResetHandler(CatchResetEvent);
                    setScaleFactor();
                    //tmRefresh.Enabled = false;
                }

            }
        }


        public bool ResizeParent
        {
            get
            {
                return m_ResizeParentForm;
            }
            set
            {
                m_ResizeParentForm = value;
            }
        }

        public void UnHookEvents ()
        {
            mVuMeter.PeakOverload -= new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
            mVuMeter.UpdateForms -= new Events.Audio.VuMeter.UpdateFormsHandler(CatchUpdateForms);
            mVuMeter.ResetEvent -= new Events.Audio.VuMeter.ResetHandler(CatchResetEvent);
        }

        // member variables

        internal int HighTop = 100;
        internal int HighBottom = 160;

        internal int NormalTop = 165;
        internal int NormalBottom = 285;

        internal int LowTop = 290;
        internal int LowBottom = 350;

        internal int LineWidth = 30;

        internal double ScaleFactor = 1;
        internal int LeftGraphX = 20;
        internal int RightGraphX = 75;

        private int GraphOriginY = 100;

        internal int BackGroundWidth = 0;
        internal int BackGroundTop = 0;
        internal int BackGroundBottom = 0;
        internal int BackGroundX = 0;

        internal int EraserLeft = 0;
        internal int EraserRight = 0;

        internal int PeakOverloadLightX = 0;
        internal int PeakOverloadLightY = 0;

        private int PeakOverloadLightWidth = 45;

        private void GraphicalVuMeter_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
            setScaleFactor();
            System.Drawing.Graphics objGraphics;
            objGraphics = this.CreateGraphics();

            Pen PenWhite = new Pen(Color.White);
            PenWhite.Width = 300;
            objGraphics.DrawLine(PenWhite, 0, 0, 0, 600);

            // enables the refresh timer for repainting graph at regular interval
            tmRefresh.Enabled = true;
        }


        // function to catch the update event from VuMeter class to update graph cordinates
        public void CatchUpdateForms(object sender, Events.Audio.VuMeter.UpdateFormsEventArgs Update)
        {
            VuMeter ob_VuMeterArg = sender as VuMeter;
            mVuMeter = ob_VuMeterArg;

            // Update erase left and erase right cordinates
            int ThresholdFactor = 12500 / (mVuMeter.UpperThreshold - mVuMeter.LowerThreshold);
            int DisplayAmpLeft = (mVuMeter.m_MeanValueLeft * ThresholdFactor) / 100;
            int DisplayAmpRight = (mVuMeter.m_MeanValueRight * ThresholdFactor) / 100;
            int Offset = 65 - ((mVuMeter.LowerThreshold * ThresholdFactor) / 100);
            DisplayAmpLeft = DisplayAmpLeft + Offset;
            DisplayAmpRight = DisplayAmpRight + Offset;

            EraserLeft = Convert.ToInt32(LowBottom - (ScaleFactor * DisplayAmpLeft));
            EraserRight = Convert.ToInt32(LowBottom - (ScaleFactor * DisplayAmpRight));
            //EraserLeft = 100+  mVuMeter.m_MeanValueLeft;

            tmRefresh.Start();
            //MessageBox.Show(EraserLeft.ToString());
        }
        
        private void tmRefresh_Tick(object sender, System.EventArgs e)
        {
            //System.Media.SystemS2ounds.Asterisk.Play();

            // paint form
            System.Drawing.Graphics objGraphics;
            objGraphics = this.CreateGraphics();

            // Paint Backgrounds
            Pen PenVackPaint = new Pen(Color.White);
            PenVackPaint.Width = 700;

            BackPaintCount++;
            if (BackPaintCount == 40)
            {


                objGraphics.DrawLine(PenVackPaint, 0, 0, 0, 600);
                BackPaintCount = 0;

            }
            // Paint two vertical graphs
            Pen PenHigh = new Pen(Color.Red);
            PenHigh.Width = LineWidth;

            Pen PenNormal = new Pen(Color.Green);
            PenNormal.Width = LineWidth;

            Pen PenLow = new Pen(Color.Yellow);
            PenLow.Width = LineWidth;

            Pen PenVackground = new Pen(Color.White);
            PenVackground.Width = LineWidth;

            Pen PenBackLight = new Pen(Color.White);
            PenBackLight.Width = PeakOverloadLightWidth;
            Pen PenOverloadLight = new Pen(Color.Red);
            PenOverloadLight.Width = PeakOverloadLightWidth;

            // following one line added for debugging
            //EraserLeft = EraserRight = 0;

            if (EraserLeft < HighTop)
            {
                EraserLeft = HighTop;
            }

            if (EraserRight < HighTop)
            {
                EraserRight = HighTop;
            }


            // For left channel painting
            if (EraserLeft < LowBottom && EraserLeft > LowTop)
            {
                objGraphics.DrawLine(PenLow, LeftGraphX, EraserLeft, LeftGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, LeftGraphX, LineWidth, EraserLeft, LowBottom);
            }
            else if (EraserLeft > NormalTop && EraserLeft <= LowTop)
            {
                objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, LeftGraphX, LineWidth, LowTop, LowBottom);
                objGraphics.DrawLine(PenNormal, LeftGraphX, EraserLeft, LeftGraphX, NormalBottom);
            }
            else if (EraserLeft >= HighTop && EraserLeft <= NormalTop)
            {
                objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, LeftGraphX, LineWidth, LowTop, LowBottom);
                objGraphics.DrawLine(PenNormal, LeftGraphX, NormalTop, LeftGraphX, NormalBottom);
                objGraphics.DrawLine(PenHigh, LeftGraphX, EraserLeft, LeftGraphX, HighBottom);
            }

            // for painting right channel
            if (EraserRight < LowBottom && EraserRight > LowTop)
            {
                objGraphics.DrawLine(PenLow, RightGraphX, EraserRight, RightGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, RightGraphX, LineWidth, EraserRight, LowBottom);
            }
            else if (EraserRight > NormalTop && EraserRight <= LowTop)
            {
                objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, RightGraphX, LineWidth, LowTop, LowBottom);
                objGraphics.DrawLine(PenNormal, RightGraphX, EraserRight, RightGraphX, NormalBottom);
            }
            else if (EraserRight >= HighTop && EraserRight <= NormalTop)
            {
                objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);
                DrawBoundary(objGraphics, Color.Orange, RightGraphX, LineWidth, LowTop, LowBottom);
                objGraphics.DrawLine(PenNormal, RightGraphX, NormalTop, RightGraphX, NormalBottom);
                objGraphics.DrawLine(PenHigh, RightGraphX, EraserRight, RightGraphX, HighBottom);
            }


            // Erase the unwanted line starting from top according to amplitude of each channel
            objGraphics.DrawLine(PenVackground, LeftGraphX, HighTop, LeftGraphX, EraserLeft);
            objGraphics.DrawLine(PenVackground, RightGraphX, HighTop, RightGraphX, EraserRight);

            EraserLeft = LowBottom;
            EraserRight = LowBottom;

            // paint the peak overload light

            if (m_OverloadLightEnabled== false)
            {
                objGraphics.DrawLine(PenBackLight, PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX, PeakOverloadLightY + PeakOverloadLightWidth);
            }
            else  // Paint the light red for warning
            {
                objGraphics.DrawLine(PenOverloadLight, PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX, PeakOverloadLightY + PeakOverloadLightWidth);

            }
            m_OverloadLightEnabled= false;
        }


        private void DrawBoundary(System.Drawing.Graphics objGraphics, Color BoundaryColor, int XCord, int Width, int TopY, int LowY)
        {
            Pen BoundaryPen = new Pen(BoundaryColor);
            int BoundaryWidth = Convert.ToInt32(6 * ScaleFactor);
            BoundaryPen.Width = BoundaryWidth;

            int LeftX = XCord - ((Width / 2) - (BoundaryWidth / 2));
            int RightX = XCord + ((Width / 2) - (BoundaryWidth / 2));

            objGraphics.DrawLine(BoundaryPen, LeftX, TopY, LeftX, LowY);
            objGraphics.DrawLine(BoundaryPen, RightX, TopY, RightX, LowY);
        }

        // catch the peak overload event triggered by VuMeter
        public void CatchPeakOverloadEvent(object sender, Events.Audio.VuMeter.PeakOverloadEventArgs ob_PeakOverload)
        {
            VuMeter ob_VuMeter = sender as VuMeter;
            if (ob_PeakOverload.Channel == 1)
            {
            }
            if (ob_PeakOverload.Channel == 2)
            {
            }

            m_OverloadLightEnabled= true;
        }

        internal void CatchResetEvent(object sender, Events.Audio.VuMeter.ResetEventArgs ob_VuMeterEvent)
        {

            System.Drawing.Graphics objGraphics;
            objGraphics = this.CreateGraphics();

            Pen PenVackPaint = new Pen(Color.White);
            PenVackPaint.Width = 300;

            objGraphics.DrawLine(PenVackPaint, 0, 0, 0, 600);

            Pen PenVackground = new Pen(Color.White);
            PenVackground.Width = LineWidth;
            objGraphics.DrawLine(PenVackground, PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX, PeakOverloadLightY + LineWidth + LineWidth);
            objGraphics.DrawLine(PenVackground, PeakOverloadLightX + LineWidth, PeakOverloadLightY, PeakOverloadLightX + LineWidth, PeakOverloadLightY + LineWidth + LineWidth);

            //SetTextBoxText(txtOverloadLeft, " ");   // avoid race condition - JQ
            //SetTextBoxText(txtOverloadRight, " ");  // JQ
            m_ResizeParentForm = false;
            setScaleFactor();
            m_ResizeParentForm = true;
        }

        private void setScaleFactor()
        {
            //ScaleFactor = 2 ;
            GraphOriginY = Convert.ToInt32(85 * ScaleFactor);

            HighTop = GraphOriginY;
            HighBottom = HighTop + Convert.ToInt32(60 * ScaleFactor);

            NormalTop = HighBottom + Convert.ToInt32(6 * ScaleFactor);
            NormalBottom = NormalTop + Convert.ToInt32(120 * ScaleFactor);

            LowTop = NormalBottom + Convert.ToInt32(6 * ScaleFactor);
            LowBottom = LowTop + Convert.ToInt32(60 * ScaleFactor);

            LineWidth = Convert.ToInt32(31 * ScaleFactor);

            LeftGraphX = Convert.ToInt32(25 * ScaleFactor);
            RightGraphX = Convert.ToInt32(76 * ScaleFactor);

            PeakOverloadLightX = Convert.ToInt32(60 * ScaleFactor);
            PeakOverloadLightY = 10;
            PeakOverloadLightWidth = Convert.ToInt32(45 * ScaleFactor);

            int width = Convert.ToInt32(110 * ScaleFactor);
            int height = Convert.ToInt32((LowBottom + 20));
            this.Size = new Size(width, height);

            if ( m_ResizeParentForm )
            this.ParentForm.Size = this.Size;
        }

        


        


    }
}
