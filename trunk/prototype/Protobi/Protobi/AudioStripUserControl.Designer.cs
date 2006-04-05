namespace Protobi
{
    partial class AudioStripUserControl
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
            this.wavePanel = new Protobi.WavePanel();
            this.SuspendLayout();
            // 
            // wavePanel
            // 
            this.wavePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wavePanel.BackColor = System.Drawing.Color.White;
            this.wavePanel.Location = new System.Drawing.Point(21, 20);
            this.wavePanel.Margin = new System.Windows.Forms.Padding(4);
            this.wavePanel.Name = "wavePanel";
            this.wavePanel.Size = new System.Drawing.Size(200, 72);
            this.wavePanel.TabIndex = 2;
            // 
            // AudioStripUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Controls.Add(this.wavePanel);
            this.Name = "AudioStripUserControl";
            this.Controls.SetChildIndex(this.wavePanel, 0);
            this.Controls.SetChildIndex(this.selectHandle, 0);
            this.Controls.SetChildIndex(this.sizeHandle, 0);
            this.Controls.SetChildIndex(this.label, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WavePanel wavePanel;

    }
}
