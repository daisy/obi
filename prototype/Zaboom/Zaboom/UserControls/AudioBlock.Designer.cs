namespace Zaboom.UserControls
{
    partial class AudioBlock
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
            this.infoPanel = new System.Windows.Forms.Panel();
            this.timeLabel = new System.Windows.Forms.Label();
            this.waveformPanel = new UserControls.WaveformPanel();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.infoPanel.BackColor = System.Drawing.Color.Pink;
            this.infoPanel.Controls.Add(this.timeLabel);
            this.infoPanel.Location = new System.Drawing.Point(3, 3);
            this.infoPanel.Margin = new System.Windows.Forms.Padding(0);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.infoPanel.Size = new System.Drawing.Size(23, 144);
            this.infoPanel.TabIndex = 1;
            this.infoPanel.Click += new System.EventHandler(this.click);
            this.infoPanel.SizeChanged += new System.EventHandler(this.ContentsSizeChanged);
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(0, 3);
            this.timeLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(17, 12);
            this.timeLabel.TabIndex = 0;
            this.timeLabel.Text = "0s";
            this.timeLabel.Click += new System.EventHandler(this.click);
            // 
            // waveformPanel
            // 
            this.waveformPanel.BackColor = System.Drawing.Color.White;
            this.waveformPanel.Location = new System.Drawing.Point(26, 3);
            this.waveformPanel.Margin = new System.Windows.Forms.Padding(0);
            this.waveformPanel.Name = "waveformPanel";
            this.waveformPanel.PixelsPerSecond = 0;
            this.waveformPanel.Size = new System.Drawing.Size(121, 144);
            this.waveformPanel.TabIndex = 1;
            this.waveformPanel.Click += new System.EventHandler(this.click);
            this.waveformPanel.SizeChanged += new System.EventHandler(this.ContentsSizeChanged);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.waveformPanel);
            this.Controls.Add(this.infoPanel);
            this.Name = "AudioBlock";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Click += new System.EventHandler(this.click);
            this.infoPanel.ResumeLayout(false);
            this.infoPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label timeLabel;
        private WaveformPanel waveformPanel;

    }
}
