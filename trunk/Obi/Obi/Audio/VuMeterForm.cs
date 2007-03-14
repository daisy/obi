using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX ;
using Microsoft.DirectX.DirectSound ;

namespace Obi.Audio
{
	/// <summary>
	/// Summary description for VuMeterForm.
	/// </summary>
	public class VuMeterForm : System.Windows.Forms.Form
	{
        private VuMeter mVuMeter;  // the vu meter model for this form

        /// <summary>
        /// Return the VU meter object that it represents.
        /// </summary>
        public VuMeter VuMeter
        {
            get { return mVuMeter; }
        }
        
        private System.ComponentModel.IContainer components;

		public VuMeterForm(VuMeter vuMeter)
		{
			InitializeComponent();
			mVuMeter = vuMeter;
			mVuMeter.PeakOverload += new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
			mVuMeter.UpdateForms += new Events.Audio.VuMeter.UpdateFormsHandler(CatchUpdateForms);
			mVuMeter.ResetEvent += new Events.Audio.VuMeter.ResetHandler(CatchResetEvent);
//            setScaleFactor();
            //tmRefresh.Enabled = false;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VuMeterForm));
            this.tmRefresh = new System.Windows.Forms.Timer(this.components);
            this.tmBeep = new System.Windows.Forms.Timer(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tmRefresh
            // 
            this.tmRefresh.Enabled = true;
            this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
            // 
            // tmBeep
            // 
            this.tmBeep.Enabled = true;
            this.tmBeep.Interval = 1000;
            this.tmBeep.Tick += new System.EventHandler(this.tmBeep_Tick);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(0, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(60, 22);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // VuMeterForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(332, 566);
            this.Controls.Add(this.btnClose);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(600, 100);
            this.MaximizeBox = false;
            this.Name = "VuMeterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "VuMeter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VuMeterForm_FormClosing);
            this.Load += new System.EventHandler(this.VuMeterForm_Load);
            this.ResumeLayout(false);

		}
		#endregion

        // override property to show without focus
        private bool ShowWithoutFocusFlag  = true ;
        protected override bool ShowWithoutActivation
        {
            get
            {
                return ShowWithoutFocusFlag;
            }
        }


		internal int HighTop =100 ;
		internal int HighBottom = 160 ;

		internal int NormalTop = 165 ;
		internal int NormalBottom = 285 ;

		internal int LowTop = 290 ;
		internal int LowBottom = 350 ;

		internal int LineWidth = 30 ;

		internal double ScaleFactor = 1 ;
		internal int LeftGraphX = 20  ;
		internal int RightGraphX = 75 ;

        private int GraphOriginX  = 100;

		internal int BackGroundWidth = 0 ;
		internal int BackGroundTop = 0 ;
		internal int BackGroundBottom = 0 ;
		internal int BackGroundX = 0;

		internal int EraserLeft = 0 ;
		internal int EraserRight = 0 ;

		internal int PeakOverloadLightX  =0 ;
		internal int PeakOverloadLightY  =0 ;

        private int PeakOverloadLightWidth = 45;

		VuMeter ob_VuMeter ;



        private System.Windows.Forms.Timer tmRefresh;
        private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Timer tmBeep;
		
        public double MagnificationFactor
        {
            get
            {
                return ScaleFactor;
            }
            set
            {
                ScaleFactor = value;
//                setScaleFactor();
            }
    }


		// initialise the form and frame for graph display
		private void VuMeterForm_Load(object sender, System.EventArgs e)
        {
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			Pen PenWhite = new Pen(Color.White );
			PenWhite.Width = 300 ;
			objGraphics.DrawLine(PenWhite , 0, 0, 0, 600 ) ;
            this.BackColor = Color.White;
			// enables the refresh timer for repainting graph at regular interval
			tmRefresh.Enabled = true ;
			
		}

		// load the beep file and plays it once
		void LoadBeep ()
		{
            FileInfo BeepFile = new FileInfo("Beep.wav");
            if (BeepFile.Exists)
            {
                System.Media.SoundPlayer PlayBeep = new System.Media.SoundPlayer("Beep.wav");
                PlayBeep.Play();
            }
		}

        double percentReadingLeft = 0.0; // values = 0.0 - 1.0
        double percentReadingRight = 0.0;

        public void CatchUpdateForms(object sender, Events.Audio.VuMeter.UpdateFormsEventArgs Update)
		{
            VuMeter ob_VuMeter = (VuMeter)sender;
            percentReadingLeft = ob_VuMeter.m_MeanValueLeft > 0 ? (Math.Log10(ob_VuMeter.m_MeanValueLeft) - Math.Log10(1)) / (Math.Log10(int.MaxValue) - Math.Log10(1)) : 0;
            percentReadingRight = ob_VuMeter.m_MeanValueRight > 0 ? (Math.Log10(ob_VuMeter.m_MeanValueRight) - Math.Log10(1)) / (Math.Log10(int.MaxValue) - Math.Log10(1)) : 0;

            tmRefresh.Start();
        }

        private void tmRefresh_Tick(object sender, System.EventArgs e)
        {
            System.Drawing.Graphics objGraphics;
            objGraphics = this.CreateGraphics();

//            objGraphics.Clear(System.Drawing.Color.White);

            int borderwidth = this.ClientSize.Width/8;
            int spaceAtTop = 40;
            int graphWidth = (this.ClientSize.Width - 3 * borderwidth) / 2;
            int graphMaxHeight = this.ClientSize.Height - 3 * borderwidth;

            int leftSize = (int)(graphMaxHeight * percentReadingLeft);
            int rightSize = (int)(graphMaxHeight * percentReadingRight);

            Rectangle leftBar = new Rectangle(borderwidth, borderwidth * 2 + spaceAtTop + (graphMaxHeight - leftSize), graphWidth, leftSize);
            Rectangle rightBar = new Rectangle(borderwidth * 2 + graphWidth, borderwidth * 2 + spaceAtTop + (graphMaxHeight - rightSize), graphWidth, rightSize);
            Rectangle leftAntiBar = new Rectangle(borderwidth, borderwidth * 2 + spaceAtTop, graphWidth, (graphMaxHeight - leftSize));
            Rectangle rightAntiBar = new Rectangle(borderwidth * 2 + graphWidth, borderwidth * 2 + spaceAtTop, graphWidth, (graphMaxHeight - rightSize));

            objGraphics.FillRectangle(new SolidBrush(Color.White), leftAntiBar);
            objGraphics.FillRectangle(new SolidBrush(Color.White), rightAntiBar);
            objGraphics.FillRectangle(new SolidBrush(Color.Blue), leftBar);
            objGraphics.FillRectangle(new SolidBrush(Color.Blue), rightBar);

            #region old stufff
            /*
            //System.Media.SystemS2ounds.Asterisk.Play();
            
			// paint form
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			// Paint Backgrounds
			Pen PenVackPaint= new Pen(Color.White);
			PenVackPaint.Width = 700 ;

            BackPaintCount++;
            if ( BackPaintCount == 30 )
            {
                

                    objGraphics.DrawLine(PenVackPaint, 0, 0, 0, 600);
                    BackPaintCount = 0;
                
            }
			// Paint two vertical graphs
			Pen PenHigh  = new Pen(Color.Red );
			PenHigh.Width = LineWidth ;

			Pen PenNormal = new Pen(Color.Green);
			PenNormal.Width = LineWidth ;

			Pen PenLow = new Pen(Color.Yellow );
			PenLow.Width = LineWidth ;

			Pen PenVackground = new Pen(Color.White);
			PenVackground.Width = LineWidth ;

            Pen PenBackLight = new Pen(Color.White);
            PenBackLight.Width = PeakOverloadLightWidth;
            Pen PenOverloadLight = new Pen(Color.Red);
            PenOverloadLight.Width = PeakOverloadLightWidth;

            if ( EraserLeft < HighTop )
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
                objGraphics.DrawLine(PenLow, LeftGraphX, EraserLeft , LeftGraphX, LowBottom);	
            }
            else if( EraserLeft > NormalTop  &&  EraserLeft <= LowTop )
            {
                objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);	
                objGraphics.DrawLine(PenNormal, LeftGraphX, EraserLeft , LeftGraphX, NormalBottom);	
            }
            else if ( EraserLeft >= HighTop  &&  EraserLeft <= NormalTop )
            {
                objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);
                objGraphics.DrawLine(PenNormal, LeftGraphX, NormalTop, LeftGraphX, NormalBottom);
                objGraphics.DrawLine(PenHigh, LeftGraphX, EraserLeft , LeftGraphX, HighBottom);		
            }

            // for painting right channel
            if ( EraserRight < LowBottom && EraserRight > LowTop)
            {
                objGraphics.DrawLine(PenLow, RightGraphX, EraserRight , RightGraphX, LowBottom);	
            }
            else if (EraserRight > NormalTop  &&  EraserRight <= LowTop )
            {
                objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);	
                objGraphics.DrawLine(PenNormal, RightGraphX, EraserRight , RightGraphX, NormalBottom);	
            }
            else if (EraserRight >= HighTop &&  EraserRight <= NormalTop )
            {
                objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);	
                objGraphics.DrawLine(PenNormal, RightGraphX, NormalTop, RightGraphX, NormalBottom);	
                objGraphics.DrawLine(PenHigh, RightGraphX, EraserRight , RightGraphX, HighBottom);		
            }

	
			// Erase the unwanted line starting from top according to amplitude of each channel
			objGraphics.DrawLine(PenVackground , LeftGraphX, HighTop , LeftGraphX, EraserLeft );	
			objGraphics.DrawLine(PenVackground , RightGraphX, HighTop , RightGraphX, EraserRight );

            EraserLeft = LowBottom;
            EraserRight = LowBottom ;

			// paint the peak overload light
			
			if ( BeepEnabled == false)
			{
				objGraphics.DrawLine(PenBackLight , PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY  + PeakOverloadLightWidth );	
				
                
			}
			else  // Paint the light red for warning
			{
				objGraphics.DrawLine( PenOverloadLight , PeakOverloadLightX , PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY  + PeakOverloadLightWidth );
                LoadBeep();
			}
            */
            #endregion


        }
		
		/// <summary>
		/// Thread-safe way to set the text on a control.
		/// </summary>
		/// <remarks>Added by JQ</remarks>
		private void SetTextBoxText(TextBox box, string text)
		{
			if (InvokeRequired)
			{
				Invoke(new SetTextBoxTextCallback(SetTextBoxText), new object[] { box, text }); 
			}
			else
			{
				box.Text = text;
			}
		}

		private delegate void SetTextBoxTextCallback(TextBox box, string text);

		bool BeepEnabled = false ;
		// catch the peak overload event triggered by VuMeter
		public void CatchPeakOverloadEvent ( object sender , Events.Audio.VuMeter.PeakOverloadEventArgs ob_PeakOverload )
		{
			VuMeter ob_VuMeter  = sender as VuMeter ;
			if (ob_PeakOverload .Channel == 1)
			{
			}
			if (ob_PeakOverload .Channel== 2)
			{
			}

			BeepEnabled =true  ;
		}	

		// repeats the LoadBeep function to repeat beeps while there is overload
		private void tmBeep_Tick(object sender, System.EventArgs e)
		{
			if (BeepEnabled == true)
			BeepEnabled = false ;
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Visible = false  ;
		}

		internal void CatchResetEvent ( object sender , Events.Audio.VuMeter.ResetEventArgs ob_VuMeterEvent)
		{
            percentReadingLeft = 0.0;
            percentReadingRight = 0.0;            
		}

        private void VuMeterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }


        /*
private void setScaleFactor()
        {
            //ScaleFactor = 2 ;
            GraphOriginX = Convert.ToInt32(100 * ScaleFactor);

            HighTop = GraphOriginX  ; 
        HighBottom = HighTop  + Convert.ToInt32 (  60 * ScaleFactor  ) ;

        NormalTop = HighBottom  + Convert.ToInt32 (   5 * ScaleFactor  ) ;
        NormalBottom = NormalTop + Convert.ToInt32 (  120 * ScaleFactor  ) ;

        LowTop = NormalBottom + Convert.ToInt32 (  5 * ScaleFactor  ) ;
        LowBottom = LowTop + Convert.ToInt32 ( 60 * ScaleFactor  ) ;

        LineWidth = Convert.ToInt32 (  30* ScaleFactor  ) ;

        LeftGraphX = Convert.ToInt32 ( 25 * ScaleFactor   ) ;
        RightGraphX = Convert.ToInt32 (  75    * ScaleFactor  ) ;

        PeakOverloadLightX = Convert.ToInt32 ( 65 * ScaleFactor);
    PeakOverloadLightY =  10 ;
    PeakOverloadLightWidth = Convert.ToInt32(45 * ScaleFactor);


        int width  = Convert.ToInt32 (  110 * ScaleFactor ) ;
        int height  = Convert.ToInt32 (   ( LowBottom + 20 )  ) ;
        this.Size = new Size( width , height );
        
    
        }
        */ 

		// end of class
	}
}
