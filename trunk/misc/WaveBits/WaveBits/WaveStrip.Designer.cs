namespace WaveBits
{
    partial class WaveStrip
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
            this.selectPanel = new System.Windows.Forms.Panel();
            this.resizePanel = new System.Windows.Forms.Panel();
            this.label = new System.Windows.Forms.Label();
            this.wavePanel = new WaveBits.WavePanel();
            this.SuspendLayout();
            // 
            // selectPanel
            // 
            this.selectPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.selectPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.selectPanel.Location = new System.Drawing.Point(0, 0);
            this.selectPanel.Margin = new System.Windows.Forms.Padding(0);
            this.selectPanel.Name = "selectPanel";
            this.selectPanel.Size = new System.Drawing.Size(16, 150);
            this.selectPanel.TabIndex = 0;
            // 
            // resizePanel
            // 
            this.resizePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.resizePanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.resizePanel.Location = new System.Drawing.Point(134, 0);
            this.resizePanel.Margin = new System.Windows.Forms.Padding(0);
            this.resizePanel.Name = "resizePanel";
            this.resizePanel.Size = new System.Drawing.Size(16, 150);
            this.resizePanel.TabIndex = 1;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(20, 4);
            this.label.Margin = new System.Windows.Forms.Padding(4);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(41, 12);
            this.label.TabIndex = 1;
            this.label.Text = "No file.";
            // 
            // wavePanel
            // 
            this.wavePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wavePanel.AutoSize = true;
            this.wavePanel.BackColor = System.Drawing.Color.White;
            this.wavePanel.Location = new System.Drawing.Point(16, 20);
            this.wavePanel.Margin = new System.Windows.Forms.Padding(0);
            this.wavePanel.Name = "wavePanel";
            this.wavePanel.Size = new System.Drawing.Size(118, 130);
            this.wavePanel.TabIndex = 2;
            // 
            // WaveStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wavePanel);
            this.Controls.Add(this.label);
            this.Controls.Add(this.resizePanel);
            this.Controls.Add(this.selectPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "WaveStrip";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel selectPanel;
        private System.Windows.Forms.Panel resizePanel;
        private System.Windows.Forms.Label label;
        private WavePanel wavePanel;
    }
}
