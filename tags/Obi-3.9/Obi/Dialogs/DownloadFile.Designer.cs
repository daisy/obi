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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.m_btnClose = new System.Windows.Forms.Button();
            this.m_btnDownload = new System.Windows.Forms.Button();
            this.m_btnFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // m_btnClose
            // 
            resources.ApplyResources(this.m_btnClose, "m_btnClose");
            this.m_btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnClose.Name = "m_btnClose";
            this.m_btnClose.UseVisualStyleBackColor = true;
            this.m_btnClose.Click += new System.EventHandler(this.m_btnClose_Click);
            // 
            // m_btnDownload
            // 
            resources.ApplyResources(this.m_btnDownload, "m_btnDownload");
            this.m_btnDownload.Name = "m_btnDownload";
            this.m_btnDownload.UseVisualStyleBackColor = true;
            this.m_btnDownload.Click += new System.EventHandler(this.m_btnDownload_Click);
            // 
            // m_btnFolder
            // 
            resources.ApplyResources(this.m_btnFolder, "m_btnFolder");
            this.m_btnFolder.Name = "m_btnFolder";
            this.m_btnFolder.UseVisualStyleBackColor = true;
            this.m_btnFolder.Click += new System.EventHandler(this.m_btnFolder_Click);
            // 
            // DownloadFile
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnClose;
            this.Controls.Add(this.m_btnFolder);
            this.Controls.Add(this.m_btnDownload);
            this.Controls.Add(this.m_btnClose);
            this.Controls.Add(this.richTextBox1);
            this.Name = "DownloadFile";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button m_btnClose;
        private System.Windows.Forms.Button m_btnDownload;
        private System.Windows.Forms.Button m_btnFolder;
    }
}