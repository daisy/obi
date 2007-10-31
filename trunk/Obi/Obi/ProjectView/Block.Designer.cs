namespace Obi.ProjectView
{
    partial class Block
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
            this.mWaveform.Location = new System.Drawing.Point(3, 3);
            this.mWaveform.Name = "mWaveform";
            this.mWaveform.Size = new System.Drawing.Size(98, 98);
            this.mWaveform.TabIndex = 1;
            this.mWaveform.Text = "waveform1";
            this.mWaveform.Click += new System.EventHandler(this.mWaveform_Click);
            // 
            // Block
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.HotPink;
            this.Controls.Add(this.mWaveform);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Name = "Block";
            this.Size = new System.Drawing.Size(104, 104);
            this.Enter += new System.EventHandler(this.Block_Enter);
            this.Click += new System.EventHandler(this.Block_Click);
            this.ResumeLayout(false);

        }

        #endregion

        private Waveform mWaveform;
    }
}
