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
            this.mGraphicalPeakMeter.Location = new System.Drawing.Point(0, 0);
            this.mGraphicalPeakMeter.MinimumSize = new System.Drawing.Size(100, 40);
            this.mGraphicalPeakMeter.Name = "mGraphicalPeakMeter";
            this.mGraphicalPeakMeter.Size = new System.Drawing.Size(154, 580);
            this.mGraphicalPeakMeter.SourceVuMeter = null;
            this.mGraphicalPeakMeter.TabIndex = 0;
            // 
            // chkOnTop
            // 
            this.chkOnTop.AutoSize = true;
            this.chkOnTop.Checked = true;
            this.chkOnTop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnTop.Location = new System.Drawing.Point(0, 586);
            this.chkOnTop.Name = "chkOnTop";
            this.chkOnTop.Size = new System.Drawing.Size(90, 17);
            this.chkOnTop.TabIndex = 1;
            this.chkOnTop.Text = "Toujours au-dessus";
            this.chkOnTop.UseVisualStyleBackColor = true;
            this.chkOnTop.CheckedChanged += new System.EventHandler(this.chkOnTop_CheckedChanged);
            // 
            // PeakMeterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 602);
            this.Controls.Add(this.chkOnTop);
            this.Controls.Add(this.mGraphicalPeakMeter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PeakMeterForm";
            this.Text = "Vu-Mètres";
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