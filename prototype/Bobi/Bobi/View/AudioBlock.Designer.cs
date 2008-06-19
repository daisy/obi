namespace Bobi.View
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
            this.cursorBar = new Bobi.View.CursorBar();
            this.waveformCanvas = new Bobi.View.WaveformCanvas();
            this.SuspendLayout();
            // 
            // cursorBar
            // 
            this.cursorBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cursorBar.Location = new System.Drawing.Point(0, 0);
            this.cursorBar.Margin = new System.Windows.Forms.Padding(0);
            this.cursorBar.Name = "cursorBar";
            this.cursorBar.Size = new System.Drawing.Size(115, 15);
            this.cursorBar.TabIndex = 1;
            this.cursorBar.Text = "cursorBar1";
            // 
            // waveformCanvas
            // 
            this.waveformCanvas.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.waveformCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.waveformCanvas.Location = new System.Drawing.Point(0, 15);
            this.waveformCanvas.Name = "waveformCanvas";
            this.waveformCanvas.Size = new System.Drawing.Size(115, 115);
            this.waveformCanvas.TabIndex = 0;
            this.waveformCanvas.Text = "waveformCanvas1";
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cursorBar);
            this.Controls.Add(this.waveformCanvas);
            this.Name = "AudioBlock";
            this.Size = new System.Drawing.Size(115, 130);
            this.ResumeLayout(false);

        }

        #endregion

        private WaveformCanvas waveformCanvas;
        private CursorBar cursorBar;
    }
}
