namespace Obi.Dialogs
{
    partial class SelectDirectoryPath
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
            this.m_lblDirectoryPath = new System.Windows.Forms.Label();
            this.m_txtDirectoryPath = new System.Windows.Forms.TextBox();
            this.m_btnDirectoryBrowse = new System.Windows.Forms.Button();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // m_lblDirectoryPath
            // 
            this.m_lblDirectoryPath.AutoSize = true;
            this.m_lblDirectoryPath.Location = new System.Drawing.Point(0, 10);
            this.m_lblDirectoryPath.Name = "m_lblDirectoryPath";
            this.m_lblDirectoryPath.Size = new System.Drawing.Size(76, 13);
            this.m_lblDirectoryPath.TabIndex = 0;
            this.m_lblDirectoryPath.Text = "&Directory path:";
            // 
            // m_txtDirectoryPath
            // 
            this.m_txtDirectoryPath.AccessibleName = "Directory path:";
            this.m_txtDirectoryPath.Location = new System.Drawing.Point(100, 10);
            this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
            this.m_txtDirectoryPath.Size = new System.Drawing.Size(150, 20);
            this.m_txtDirectoryPath.TabIndex = 1;
            // 
            // m_btnDirectoryBrowse
            // 
            this.m_btnDirectoryBrowse.Location = new System.Drawing.Point(200, 40);
            this.m_btnDirectoryBrowse.Name = "m_btnDirectoryBrowse";
            this.m_btnDirectoryBrowse.Size = new System.Drawing.Size(75, 23);
            this.m_btnDirectoryBrowse.TabIndex = 2;
            this.m_btnDirectoryBrowse.Text = "&Browse";
            this.m_btnDirectoryBrowse.UseVisualStyleBackColor = true;
            this.m_btnDirectoryBrowse.Click += new System.EventHandler(this.m_btnDirectoryBrowse_Click);
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(130, 100);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 3;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(210, 100);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 4;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Folder path";
            // 
            // SelectDirectoryPath
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 123);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnDirectoryBrowse);
            this.Controls.Add(this.m_txtDirectoryPath);
            this.Controls.Add(this.m_lblDirectoryPath);
            this.Name = "SelectDirectoryPath";
            this.Text = "Select directory path";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDirectoryPath;
        private System.Windows.Forms.TextBox m_txtDirectoryPath;
        private System.Windows.Forms.Button m_btnDirectoryBrowse;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}