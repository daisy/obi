namespace Obi.UserControls
{
    partial class SectionStrip
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
            this.mLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mLabel
            // 
            this.mLabel.AutoSize = true;
            this.mLabel.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.mLabel.Location = new System.Drawing.Point(3, 0);
            this.mLabel.Name = "mLabel";
            this.mLabel.Size = new System.Drawing.Size(32, 16);
            this.mLabel.TabIndex = 0;
            this.mLabel.Text = "xxx";
            // 
            // SectionStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.Controls.Add(this.mLabel);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.Name = "SectionStrip";
            this.Size = new System.Drawing.Size(295, 49);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label mLabel;
    }
}
