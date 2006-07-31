namespace Obi.UserControls
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
            this.mAnnotationLabel = new System.Windows.Forms.Label();
            this.mTimeLabel = new System.Windows.Forms.Label();
            this.mPlayLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // mAnnotationLabel
            // 
            this.mAnnotationLabel.AutoSize = true;
            this.mAnnotationLabel.Location = new System.Drawing.Point(3, 3);
            this.mAnnotationLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mAnnotationLabel.Name = "mAnnotationLabel";
            this.mAnnotationLabel.Size = new System.Drawing.Size(66, 12);
            this.mAnnotationLabel.TabIndex = 0;
            this.mAnnotationLabel.Text = "(annotation)";
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Location = new System.Drawing.Point(3, 21);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(35, 12);
            this.mTimeLabel.TabIndex = 1;
            this.mTimeLabel.Text = "(time)";
            // 
            // mPlayLabel
            // 
            this.mPlayLabel.AutoSize = true;
            this.mPlayLabel.Location = new System.Drawing.Point(3, 39);
            this.mPlayLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mPlayLabel.Name = "mPlayLabel";
            this.mPlayLabel.Size = new System.Drawing.Size(27, 12);
            this.mPlayLabel.TabIndex = 2;
            this.mPlayLabel.TabStop = true;
            this.mPlayLabel.Text = "Play";
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.mPlayLabel);
            this.Controls.Add(this.mTimeLabel);
            this.Controls.Add(this.mAnnotationLabel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AudioBlock";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mAnnotationLabel;
        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.LinkLabel mPlayLabel;
    }
}
