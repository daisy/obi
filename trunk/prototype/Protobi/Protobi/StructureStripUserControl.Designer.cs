namespace Protobi
{
    partial class StructureStripUserControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.headingLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // headingLabel
            // 
            this.headingLabel.AutoSize = true;
            this.headingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.headingLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.headingLabel.Location = new System.Drawing.Point(19, 20);
            this.headingLabel.Margin = new System.Windows.Forms.Padding(4);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.Padding = new System.Windows.Forms.Padding(4);
            this.headingLabel.Size = new System.Drawing.Size(54, 22);
            this.headingLabel.TabIndex = 2;
            this.headingLabel.TabStop = true;
            this.headingLabel.Text = "heading";
            this.headingLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.headingLabel_LinkClicked);
            // 
            // StructureStripUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Controls.Add(this.headingLabel);
            this.Name = "StructureStripUserControl";
            this.Controls.SetChildIndex(this.headingLabel, 0);
            this.Controls.SetChildIndex(this.label, 0);
            this.Controls.SetChildIndex(this.selectHandle, 0);
            this.Controls.SetChildIndex(this.sizeHandle, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel headingLabel;

    }
}
