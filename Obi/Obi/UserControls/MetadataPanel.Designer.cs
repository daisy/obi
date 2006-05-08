namespace Obi.UserControls
{
    partial class MetadataPanel
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
            this.titleBox = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.statusBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // titleBox
            // 
            this.titleBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.titleBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.titleBox.Location = new System.Drawing.Point(6, 6);
            this.titleBox.Name = "titleBox";
            this.titleBox.ReadOnly = true;
            this.titleBox.Size = new System.Drawing.Size(100, 12);
            this.titleBox.TabIndex = 0;
            this.titleBox.Text = "Current book title";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(6, 42);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(213, 12);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click here to edit bibliographic metadata";
            // 
            // statusBox
            // 
            this.statusBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusBox.Location = new System.Drawing.Point(6, 24);
            this.statusBox.Name = "statusBox";
            this.statusBox.ReadOnly = true;
            this.statusBox.Size = new System.Drawing.Size(100, 12);
            this.statusBox.TabIndex = 1;
            this.statusBox.Text = "Metadata status";
            this.statusBox.Enter += new System.EventHandler(this.statusBox_Enter);
            // 
            // MetadataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.Controls.Add(this.statusBox);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.titleBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MetadataPanel";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(341, 63);
            this.Load += new System.EventHandler(this.MetadataPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox titleBox;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox statusBox;
    }
}
