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
            this.mTimeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTimeLabel.Location = new System.Drawing.Point(3, 0);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(26, 20);
            this.mTimeLabel.TabIndex = 0;
            this.mTimeLabel.Text = "0s";
            this.mTimeLabel.Click += new System.EventHandler(this.TimeLabel_Click);
            // 
            // Block
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.HotPink;
            this.Controls.Add(this.mTimeLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Name = "Block";
            this.Size = new System.Drawing.Size(104, 104);
            this.Click += new System.EventHandler(this.Block_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTimeLabel;
    }
}
