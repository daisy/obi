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
            this.mRenameBox = new System.Windows.Forms.TextBox();
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
            // mRenameBox
            // 
            this.mRenameBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mRenameBox.Location = new System.Drawing.Point(5, 3);
            this.mRenameBox.Name = "mRenameBox";
            this.mRenameBox.Size = new System.Drawing.Size(100, 12);
            this.mRenameBox.TabIndex = 2;
            this.mRenameBox.Visible = false;
            this.mRenameBox.Leave += new System.EventHandler(this.mRenameBox_Leave);
            this.mRenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mRenameBox_KeyDown);
            // 
            // AudioBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.mRenameBox);
            this.Controls.Add(this.mTimeLabel);
            this.Controls.Add(this.mAnnotationLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.Name = "AudioBlock";
            this.Click += new System.EventHandler(this.AudioBlock_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mAnnotationLabel;
        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.TextBox mRenameBox;
    }
}
