namespace AudioEngine.PPMeter
{
    partial class PPMeter
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
            this.mPPMBar1 = new AudioEngine.PPMeter.PPMBar();
            this.mPPMBar2 = new AudioEngine.PPMeter.PPMBar();
            this.SuspendLayout();
            // 
            // mPPMBar1
            // 
            this.mPPMBar1.FallbackSecondsPerDb = System.TimeSpan.Parse("00:00:00.0750000");
            this.mPPMBar1.ForeColor = System.Drawing.Color.Yellow;
            this.mPPMBar1.Location = new System.Drawing.Point(0, 20);
            this.mPPMBar1.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.mPPMBar1.Minimum = -72;
            this.mPPMBar1.Name = "mPPMBar1";
            this.mPPMBar1.Size = new System.Drawing.Size(340, 24);
            this.mPPMBar1.SpectrumEndColor = System.Drawing.Color.Red;
            this.mPPMBar1.TabIndex = 1;
            this.mPPMBar1.Text = "ppmBar1";
            this.mPPMBar1.Value = -72;
            // 
            // mPPMBar2
            // 
            this.mPPMBar2.FallbackSecondsPerDb = System.TimeSpan.Parse("00:00:00.0750000");
            this.mPPMBar2.ForeColor = System.Drawing.Color.Yellow;
            this.mPPMBar2.Location = new System.Drawing.Point(0, 43);
            this.mPPMBar2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.mPPMBar2.Minimum = -72;
            this.mPPMBar2.Name = "mPPMBar2";
            this.mPPMBar2.Size = new System.Drawing.Size(340, 24);
            this.mPPMBar2.SpectrumEndColor = System.Drawing.Color.Red;
            this.mPPMBar2.TabIndex = 0;
            this.mPPMBar2.Text = "ppmBar1";
            this.mPPMBar2.Value = -72;
            // 
            // PPMeter
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mPPMBar1);
            this.Controls.Add(this.mPPMBar2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "PPMeter";
            this.Size = new System.Drawing.Size(340, 65);
            this.ResumeLayout(false);

        }

        #endregion

        private PPMBar mPPMBar2;
        private PPMBar mPPMBar1;

    }
}
