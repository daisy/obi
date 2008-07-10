namespace Obi.ProjectView
{
    partial class AudioBlock : Block
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
            this.mWaveform = new Obi.ProjectView.Waveform();
            this.SuspendLayout();
            // 
            // mWaveform
            // 
            this.mWaveform.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mWaveform.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mWaveform.Location = new System.Drawing.Point(-1, 22);
            this.mWaveform.Margin = new System.Windows.Forms.Padding(0);
            this.mWaveform.Name = "mWaveform";
            this.mWaveform.Selection = null;
            this.mWaveform.Size = new System.Drawing.Size(141, 107);
            this.mWaveform.TabIndex = 1;
            this.mWaveform.Text = "waveform1";
            this.mWaveform.DoubleClick += new System.EventHandler(this.mWaveform_DoubleClick);
            this.mWaveform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseMove);
            this.mWaveform.Click += new System.EventHandler(this.mWaveform_Click);
            this.mWaveform.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mWaveform_KeyUp);
            this.mWaveform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseDown);
            this.mWaveform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseUp);
            this.mWaveform.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mWaveform_KeyDown);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.mWaveform);
            this.Margin = new System.Windows.Forms.Padding(11, 0, 11, 0);
            this.Name = "AudioBlock";
            this.Size = new System.Drawing.Size(139, 128);
            this.Controls.SetChildIndex(this.mWaveform, 0);
            this.Controls.SetChildIndex(this.mLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Waveform mWaveform;
    }
}
