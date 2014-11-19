namespace Obi.Dialogs
{
    partial class DownloadFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadFile));
            this.m_linkToDownload = new System.Windows.Forms.LinkLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_linkToDownload
            // 
            this.m_linkToDownload.AccessibleName = "Download ffmpeg";
            this.m_linkToDownload.AutoSize = true;
            this.m_linkToDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_linkToDownload.Location = new System.Drawing.Point(97, 206);
            this.m_linkToDownload.Name = "m_linkToDownload";
            this.m_linkToDownload.Size = new System.Drawing.Size(149, 20);
            this.m_linkToDownload.TabIndex = 1;
            this.m_linkToDownload.TabStop = true;
            this.m_linkToDownload.Text = "Download ffmpeg";
            this.m_linkToDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkToDownload_LinkClicked);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(26, 31);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(364, 153);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // m_btnClose
            // 
            this.m_btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnClose.Location = new System.Drawing.Point(280, 213);
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.Size = new System.Drawing.Size(75, 23);
            this.m_btnClose.TabIndex = 2;
            this.m_btnClose.Text = "Close";
            this.m_btnClose.UseVisualStyleBackColor = true;
            this.m_btnClose.Click += new System.EventHandler(this.m_btnClose_Click);
            // 
            // DownloadFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(415, 262);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.m_linkToDownload);
            this.Name = "DownloadFile";
            this.Text = "DownloadFile";
            this.Load += new System.EventHandler(this.DownloadFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel m_linkToDownload;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button m_btnClose;
    }
}