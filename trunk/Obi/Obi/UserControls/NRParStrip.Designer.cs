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
            this.labelBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelBox
            // 
            this.labelBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBox.BackColor = System.Drawing.Color.White;
            this.labelBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelBox.Location = new System.Drawing.Point(3, 3);
            this.labelBox.Name = "labelBox";
            this.labelBox.Size = new System.Drawing.Size(252, 12);
            this.labelBox.TabIndex = 0;
            this.labelBox.TextChanged += new System.EventHandler(this.labelBox_TextChanged);
            // 
            // NRParStrip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.labelBox);
            this.Name = "NRParStrip";
            this.Size = new System.Drawing.Size(258, 60);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox labelBox;

    }
}
