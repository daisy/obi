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
            this.mCustomKindLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTimeLabel
            // 
            this.mTimeLabel.AutoSize = true;
            this.mTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.mTimeLabel.Location = new System.Drawing.Point(3, 3);
            this.mTimeLabel.Margin = new System.Windows.Forms.Padding(3);
            this.mTimeLabel.Name = "mTimeLabel";
            this.mTimeLabel.Size = new System.Drawing.Size(32, 13);
            this.mTimeLabel.TabIndex = 2;
            this.mTimeLabel.Text = "(time)";
            this.mTimeLabel.Click += new System.EventHandler(this.mTimeLabel_Click);
            // 
            // mCustomKindLabel
            // 
            this.mCustomKindLabel.AutoSize = true;
            this.mCustomKindLabel.Location = new System.Drawing.Point(3, 19);
            this.mCustomKindLabel.Name = "mCustomKindLabel";
            this.mCustomKindLabel.Size = new System.Drawing.Size(70, 13);
            this.mCustomKindLabel.TabIndex = 3;
            this.mCustomKindLabel.Text = "(custom kind)";
            this.mCustomKindLabel.Click += new System.EventHandler(this.mCustomKindLabel_Click);
            // 
            // Block
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.HotPink;
            this.Controls.Add(this.mCustomKindLabel);
            this.Controls.Add(this.mTimeLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Name = "Block";
            this.Size = new System.Drawing.Size(104, 104);
            this.Enter += new System.EventHandler(this.Block_Enter);
            this.Click += new System.EventHandler(this.Block_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTimeLabel;
        private System.Windows.Forms.Label mCustomKindLabel;
    }
}
