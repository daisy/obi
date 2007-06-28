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
            this.waveformPanel = new WaveformPanel();
            this.SuspendLayout();
            // 
            // waveformPanel
            // 
            this.waveformPanel.BackColor = System.Drawing.Color.White;
            this.waveformPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.waveformPanel.Location = new System.Drawing.Point(0, 22);
            this.waveformPanel.Name = "waveformPanel";
            this.waveformPanel.PixelsPerSecond = 0;
            this.waveformPanel.Size = new System.Drawing.Size(150, 128);
            this.waveformPanel.TabIndex = 0;
            this.waveformPanel.SizeChanged += new System.EventHandler(this.waveformPanel_SizeChanged);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCoral;
            this.Controls.Add(this.waveformPanel);
            this.Name = "AudioBlock";
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private WaveformPanel waveformPanel;

    }
}
