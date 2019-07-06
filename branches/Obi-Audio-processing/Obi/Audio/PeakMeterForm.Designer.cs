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
            this.chkOnTop = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // mGraphicalPeakMeter
            // 
            this.mGraphicalPeakMeter.BarPaddingToWidthRatio = 0.075F;
            this.mGraphicalPeakMeter.FontToHeightRatio = 0.03F;
            this.mGraphicalPeakMeter.FontToWidthRatio = 0.075F;
            resources.ApplyResources(this.mGraphicalPeakMeter, "mGraphicalPeakMeter");
            this.mGraphicalPeakMeter.MinimumSize = new System.Drawing.Size(100, 40);
            this.mGraphicalPeakMeter.Name = "mGraphicalPeakMeter";
            this.mGraphicalPeakMeter.SourceVuMeter = null;
            // 
            // chkOnTop
            // 
            resources.ApplyResources(this.chkOnTop, "chkOnTop");
            this.chkOnTop.Checked = true;
            this.chkOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnTop.Name = "chkOnTop";
            this.chkOnTop.UseVisualStyleBackColor = true;
            this.chkOnTop.CheckedChanged += new System.EventHandler(this.chkOnTop_CheckedChanged);
            // 
            // PeakMeterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkOnTop);
            this.Controls.Add(this.mGraphicalPeakMeter);
            this.MaximizeBox = false;
            this.Name = "PeakMeterForm";
            this.ResizeBegin += new System.EventHandler(this.PeakMeterForm_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.PeakMeterForm_ResizeEnd);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private Obi.UserControls.GraphicalPeakMeter mGraphicalPeakMeter;
        private System.Windows.Forms.CheckBox chkOnTop;
	}
}