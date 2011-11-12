namespace Obi.Audio
{
	partial class PeakMeterForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeakMeterForm));
            this.mGraphicalPeakMeter = new Obi.UserControls.GraphicalPeakMeter();
            this.SuspendLayout();
            // 
            // mGraphicalPeakMeter
            // 
            this.mGraphicalPeakMeter.BarPaddingToWidthRatio = 0.075F;
            this.mGraphicalPeakMeter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mGraphicalPeakMeter.FontToHeightRatio = 0.03F;
            this.mGraphicalPeakMeter.FontToWidthRatio = 0.075F;
            this.mGraphicalPeakMeter.Location = new System.Drawing.Point(0, 0);
            this.mGraphicalPeakMeter.MinimumSize = new System.Drawing.Size(100, 40);
            this.mGraphicalPeakMeter.Name = "mGraphicalPeakMeter";
            this.mGraphicalPeakMeter.Size = new System.Drawing.Size(154, 492);
            this.mGraphicalPeakMeter.SourceVuMeter = null;
            this.mGraphicalPeakMeter.TabIndex = 0;
            // 
            // PeakMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 492);
            this.Controls.Add(this.mGraphicalPeakMeter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PeakMeterForm";
            this.Text = "Peak Meter";
            this.ResumeLayout(false);

		}

		#endregion

		private Obi.UserControls.GraphicalPeakMeter mGraphicalPeakMeter;
	}
}