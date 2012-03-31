namespace Obi.UserControls
{
	partial class GraphicalPeakMeter
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
        this.components = new System.ComponentModel.Container ();
        this.mPPMeter = new AudioEngine.PPMeter.PPMeter ();
        this.mUpdateGUITimer = new System.Windows.Forms.Timer ( this.components );
        this.SuspendLayout ();
        // 
        // mPPMeter
        // 
        this.mPPMeter.BackColor = System.Drawing.Color.Black;
        this.mPPMeter.BarOrientation = System.Windows.Forms.Orientation.Vertical;
        this.mPPMeter.BarPadding = 5;
        this.mPPMeter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mPPMeter.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mPPMeter.FallbackSecondsPerDb = System.TimeSpan.Parse ( "00:00:00.0750000" );
        this.mPPMeter.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
        this.mPPMeter.ForeColor = System.Drawing.Color.Yellow;
        this.mPPMeter.Location = new System.Drawing.Point ( 0, 0 );
        this.mPPMeter.Margin = new System.Windows.Forms.Padding ( 4, 2, 4, 2 );
        this.mPPMeter.Minimum = -35;
        this.mPPMeter.Name = "mPPMeter";
        this.mPPMeter.NumberOfChannels = 2;
        this.mPPMeter.ShowPeakOverloadIndicators = true;
        this.mPPMeter.Size = new System.Drawing.Size ( 127, 299 );
        this.mPPMeter.SpectrumEndColor = System.Drawing.Color.Red;
        this.mPPMeter.TabIndex = 0;
        this.mPPMeter.Resize += new System.EventHandler ( this.mPPMeter_Resize );
        this.mPPMeter.PeakOverloadIndicatorClicked += new System.EventHandler<AudioEngine.PPMeter.PeakOverloadIndicatorClickedEventArgs> ( this.mPPMeter_PeakOverloadIndicatorClicked );
        // 
        // mUpdateGUITimer
        // 
        this.mUpdateGUITimer.Interval = 50;
        this.mUpdateGUITimer.Tick += new System.EventHandler ( this.mUpdateGUITimer_Tick );
        // 
        // GraphicalPeakMeter
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add ( this.mPPMeter );
        this.MinimumSize = new System.Drawing.Size ( 100, 40 );
        this.Name = "GraphicalPeakMeter";
        this.Size = new System.Drawing.Size ( 127, 299 );
        this.ResumeLayout ( false );

		}

		#endregion

        private AudioEngine.PPMeter.PPMeter mPPMeter;
        private System.Windows.Forms.Timer mUpdateGUITimer;
	}
}
