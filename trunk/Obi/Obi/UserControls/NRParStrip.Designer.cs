namespace Obi.UserControls
{
    partial class NRParStrip
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
            this.mTitleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mTitleLabel
            // 
            this.mTitleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mTitleLabel.AutoSize = true;
            this.mTitleLabel.Location = new System.Drawing.Point(68, 24);
            this.mTitleLabel.Name = "mTitleLabel";
            this.mTitleLabel.Size = new System.Drawing.Size(123, 12);
            this.mTitleLabel.TabIndex = 0;
            this.mTitleLabel.Text = "Non-resizable par strip";
            this.mTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NRParStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mTitleLabel);
            this.Name = "NRParStrip";
            this.Size = new System.Drawing.Size(258, 60);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mTitleLabel;
    }
}
