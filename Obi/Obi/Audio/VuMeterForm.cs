using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
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
            this.tmRefresh = new System.Windows.Forms.Timer(this.components);
            this.tmBeep = new System.Windows.Forms.Timer(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tmRefresh
            // 
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
            this.btnClose.Location = new System.Drawing.Point(270, 483);
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

		internal int HighTop =0 ;
		internal int HighBottom = 0 ;

		internal int NormalTop = 0 ;
		internal int NormalBottom = 0 ;

		internal int LowTop = 0 ;
		internal int LowBottom = 0 ;

		internal int LineWidth = 0 ;

		internal int ScaleFactor = 1 ;
		internal int LeftGraphX = 0 ;
		internal int RightGraphX = 0 ;

		internal int BackGroundWidth = 0 ;
		internal int BackGroundTop = 0 ;
		internal int BackGroundBottom = 0 ;
		internal int BackGroundX = 0;

		internal int EraserLeft = 0 ;
		internal int EraserRight = 0 ;

		internal int PeakOverloadLightX  =0 ;
		internal int PeakOverloadLightY  =0 ;
		

		private int AmplitudeLeft = 0 ;
		private int AmplitudeRight = 0 ;

		VuMeter ob_VuMeter ;



        private System.Windows.Forms.Timer tmRefresh;
        private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Timer tmBeep;
		

		// initialise the form and frame for graph display
		private void VuMeterForm_Load(object sender, System.EventArgs e)
		{
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			Pen PenWhite = new Pen(Color.White );
			PenWhite.Width = BackGroundWidth;
			objGraphics.DrawLine(PenWhite , BackGroundX , BackGroundTop , BackGroundX, BackGroundBottom ) ;		

			// enables the refresh timer for repainting graph at regular interval
			tmRefresh.Enabled = true ;
			
		}

		// load the beep file and plays it once
		void LoadBeep ()
		{
			Device BeepDevice  = new Device() ;
			BeepDevice.SetCooperativeLevel(this, CooperativeLevel.Normal );
			SecondaryBuffer BeepBuffer   ;
			try
			{
				BeepBuffer  =  new SecondaryBuffer("c:\\beep.wav", BeepDevice );
				BeepBuffer.Play(0, BufferPlayFlags.Default );		
			}		
			catch (Exception Ex)
			{
				MessageBox.Show (Ex.ToString ()) ;
			}


			
		}

		// function to catch the update event from VuMeter class to update graph cordinates
		public void CatchUpdateForms (object sender , Events.Audio.VuMeter.UpdateFormsEventArgs Update )
		{
			VuMeter ob_VuMeterArg  = sender as VuMeter ;
			ob_VuMeter = ob_VuMeterArg ;

			// Update cordinates
			HighTop =ob_VuMeter.Graph.HighTop ;
			HighBottom = ob_VuMeter.Graph.HighBottom ;

			NormalTop = ob_VuMeter.Graph.NormalTop ;
			NormalBottom = ob_VuMeter.Graph.NormalBottom ;

			LowTop = ob_VuMeter.Graph.LowTop ;
			LowBottom = ob_VuMeter.Graph.LowBottom ;

			LineWidth = ob_VuMeter.Graph.LineWidth ;

			LeftGraphX = ob_VuMeter.Graph.LeftGraphX ;
			RightGraphX = ob_VuMeter.Graph.RightGraphX ;

			BackGroundWidth = ob_VuMeter.Graph.BackGroundWidth ;
			BackGroundTop = ob_VuMeter.Graph.BackGroundTop ;
			BackGroundBottom = ob_VuMeter.Graph.BackGroundBottom ;

			EraserLeft = ob_VuMeter.Graph.EraserLeft ;
			EraserRight = ob_VuMeter.Graph.EraserRight ;

			PeakOverloadLightX =   ob_VuMeter.Graph.PeakOverloadLightX ;
			PeakOverloadLightY =   ob_VuMeter.Graph.PeakOverloadLightY ;


			AmplitudeLeft = ob_VuMeter.m_MeanValueLeft ;
			AmplitudeRight = ob_VuMeter.m_MeanValueRight ;


			tmRefresh.Enabled = true ;
			
		}

		private void tmRefresh_Tick(object sender, System.EventArgs e)
		{

			// paint form
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			// Paint Backgrounds
			Pen PenVackPaint= new Pen(Color.White);
			PenVackPaint.Width = 700 ;

			objGraphics.DrawLine(PenVackPaint , 0, 0, 0, 600);		

			// Paint two vertical graphs
			Pen PenHigh  = new Pen(Color.Red );
			PenHigh.Width = LineWidth ;

			Pen PenNormal = new Pen(Color.Green);
			PenNormal.Width = LineWidth ;

			Pen PenLow = new Pen(Color.Yellow );
			PenLow.Width = LineWidth ;

			Pen PenVackground = new Pen(Color.White);
			PenVackground.Width = LineWidth ;


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
            else if ( EraserLeft > HighTop  &&  EraserLeft <= NormalTop )
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
            else if (EraserRight > HighTop &&  EraserRight <= NormalTop )
            {
                objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);	
                objGraphics.DrawLine(PenNormal, RightGraphX, NormalTop, RightGraphX, NormalBottom);	
                objGraphics.DrawLine(PenHigh, RightGraphX, EraserRight , RightGraphX, HighBottom);		
            }

            /*
			objGraphics.DrawLine(PenHigh, LeftGraphX, HighTop , LeftGraphX, HighBottom);		
			objGraphics.DrawLine(PenHigh, RightGraphX, HighTop , RightGraphX, HighBottom);		

			objGraphics.DrawLine(PenNormal, LeftGraphX, NormalTop, LeftGraphX, NormalBottom);	
			objGraphics.DrawLine(PenNormal, RightGraphX, NormalTop, RightGraphX, NormalBottom);	

			objGraphics.DrawLine(PenLow, LeftGraphX, LowTop, LeftGraphX, LowBottom);	
			objGraphics.DrawLine(PenLow, RightGraphX, LowTop, RightGraphX, LowBottom);	
            */
	
			// Erase the unwanted line starting from top according to amplitude of each channel
			objGraphics.DrawLine(PenVackground , LeftGraphX, HighTop , LeftGraphX, EraserLeft );	
			objGraphics.DrawLine(PenVackground , RightGraphX, HighTop , RightGraphX, EraserRight );	
						
				
			// paint the peak overload light
			
			if ( BeepEnabled == false)
			{
				objGraphics.DrawLine(PenVackground , PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY + LineWidth + LineWidth);	
				objGraphics.DrawLine(PenVackground , PeakOverloadLightX + LineWidth, PeakOverloadLightY, PeakOverloadLightX + LineWidth , PeakOverloadLightY + LineWidth + LineWidth);	
                
			}
			else  // Paint the light red for warning
			{
				objGraphics.DrawLine(PenHigh, PeakOverloadLightX , PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY + LineWidth + LineWidth);
                objGraphics.DrawLine(PenHigh, PeakOverloadLightX + LineWidth , PeakOverloadLightY, PeakOverloadLightX + LineWidth, PeakOverloadLightY + LineWidth + LineWidth);	
			}
			
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
				//txtOverloadLeft.Text	 = ob_VuMeter.m_MeanValueLeft.ToString () ;
//				SetTextBoxText(txtOverloadLeft, ob_VuMeter.m_MeanValueLeft.ToString());  // JQ -- avoid race condition

			}
			
			if (ob_PeakOverload .Channel== 2)
			{
				//txtOverloadRight.Text = ob_VuMeter.m_MeanValueRight.ToString ()  ;
				//SetTextBoxText(txtOverloadRight, ob_VuMeter.m_MeanValueRight.ToString());
			}

			BeepEnabled =true  ;
//			BeepEnabled = false;  // don't beep (JQ)
		}	

		// repeats the LoadBeep function to repeat beeps while there is overload
		private void tmBeep_Tick(object sender, System.EventArgs e)
		{
			if (BeepEnabled == true)
				//LoadBeep () ;

			BeepEnabled = false ;
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.Visible = false  ;
		}

		internal void CatchResetEvent ( object sender , Events.Audio.VuMeter.ResetEventArgs ob_VuMeterEvent)
		{
			System.Drawing.Graphics objGraphics;
			objGraphics = this.CreateGraphics();		

			

			
			Pen PenVackPaint= new Pen(Color.White);
			PenVackPaint.Width = 300 ;

			objGraphics.DrawLine(PenVackPaint , 0, 0, 0, 600);		

			Pen PenVackground = new Pen(Color.White);
			PenVackground.Width = LineWidth ;
			objGraphics.DrawLine(PenVackground , PeakOverloadLightX, PeakOverloadLightY, PeakOverloadLightX , PeakOverloadLightY + LineWidth + LineWidth);	
			objGraphics.DrawLine(PenVackground , PeakOverloadLightX + LineWidth, PeakOverloadLightY, PeakOverloadLightX + LineWidth , PeakOverloadLightY + LineWidth + LineWidth);	

			HighTop =0 ;
			HighBottom = 0 ;

			NormalTop = 0 ;
			NormalBottom = 0 ;

			LowTop = 0 ;
			LowBottom = 0 ;

			LineWidth = 0 ;
			AmplitudeLeft = 0 ;
			AmplitudeRight = 0 ;

			//txtOverloadLeft.Text = " " ;
			//txtOverloadRight.Text = " " ;
			//SetTextBoxText(txtOverloadLeft, " ");   // avoid race condition - JQ
			//SetTextBoxText(txtOverloadRight, " ");  // JQ
		}

        private void VuMeterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

		// end of class
	}
}
