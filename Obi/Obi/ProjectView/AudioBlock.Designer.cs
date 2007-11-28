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
            this.mWaveform.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.mWaveform.Location = new System.Drawing.Point(3, 3);
            this.mWaveform.Name = "mWaveform";
            this.mWaveform.Size = new System.Drawing.Size(98, 98);
            this.mWaveform.TabIndex = 1;
            this.mWaveform.Text = "waveform1";
            this.mWaveform.Click += new System.EventHandler(this.mWaveform_Click);
            this.mWaveform.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseDown);
            this.mWaveform.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseMove);
            this.mWaveform.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mWaveform_MouseUp);
            base.Controls.Add(this.mWaveform);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Waveform mWaveform;
    }
}
