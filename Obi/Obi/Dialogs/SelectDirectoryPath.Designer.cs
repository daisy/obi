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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectDirectoryPath));
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
            this.m_lblDirectoryPath.AccessibleDescription = null;
            this.m_lblDirectoryPath.AccessibleName = null;
            resources.ApplyResources(this.m_lblDirectoryPath, "m_lblDirectoryPath");
            this.m_lblDirectoryPath.Font = null;
            this.m_lblDirectoryPath.Name = "m_lblDirectoryPath";
            // 
            // m_txtDirectoryPath
            // 
            this.m_txtDirectoryPath.AccessibleDescription = null;
            resources.ApplyResources(this.m_txtDirectoryPath, "m_txtDirectoryPath");
            this.m_txtDirectoryPath.BackgroundImage = null;
            this.m_txtDirectoryPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtDirectoryPath.Font = null;
            this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
            // 
            // m_btnDirectoryBrowse
            // 
            this.m_btnDirectoryBrowse.AccessibleDescription = null;
            this.m_btnDirectoryBrowse.AccessibleName = null;
            resources.ApplyResources(this.m_btnDirectoryBrowse, "m_btnDirectoryBrowse");
            this.m_btnDirectoryBrowse.BackgroundImage = null;
            this.m_btnDirectoryBrowse.Font = null;
            this.m_btnDirectoryBrowse.Name = "m_btnDirectoryBrowse";
            this.m_btnDirectoryBrowse.UseVisualStyleBackColor = true;
            this.m_btnDirectoryBrowse.Click += new System.EventHandler(this.m_btnDirectoryBrowse_Click);
            // 
            // m_btnOk
            // 
            this.m_btnOk.AccessibleDescription = null;
            this.m_btnOk.AccessibleName = null;
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.BackgroundImage = null;
            this.m_btnOk.Font = null;
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.AccessibleDescription = null;
            this.m_btnCancel.AccessibleName = null;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.BackgroundImage = null;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Font = null;
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // folderBrowserDialog1
            // 
            resources.ApplyResources(this.folderBrowserDialog1, "folderBrowserDialog1");
            // 
            // SelectDirectoryPath
            // 
            this.AcceptButton = this.m_btnOk;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnDirectoryBrowse);
            this.Controls.Add(this.m_txtDirectoryPath);
            this.Controls.Add(this.m_lblDirectoryPath);
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectDirectoryPath";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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