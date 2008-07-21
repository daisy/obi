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
            this.m_lblDirectoryPath.Location = new System.Drawing.Point(13, 19);
            this.m_lblDirectoryPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_lblDirectoryPath.Name = "m_lblDirectoryPath";
            this.m_lblDirectoryPath.Size = new System.Drawing.Size(94, 16);
            this.m_lblDirectoryPath.TabIndex = 0;
            this.m_lblDirectoryPath.Text = "&Directory path:";
            // 
            // m_txtDirectoryPath
            // 
            this.m_txtDirectoryPath.AccessibleName = "Directory path:";
            this.m_txtDirectoryPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtDirectoryPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_txtDirectoryPath.Location = new System.Drawing.Point(115, 17);
            this.m_txtDirectoryPath.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_txtDirectoryPath.Name = "m_txtDirectoryPath";
            this.m_txtDirectoryPath.Size = new System.Drawing.Size(324, 22);
            this.m_txtDirectoryPath.TabIndex = 1;
            // 
            // m_btnDirectoryBrowse
            // 
            this.m_btnDirectoryBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnDirectoryBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnDirectoryBrowse.Location = new System.Drawing.Point(447, 13);
            this.m_btnDirectoryBrowse.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_btnDirectoryBrowse.Name = "m_btnDirectoryBrowse";
            this.m_btnDirectoryBrowse.Size = new System.Drawing.Size(100, 28);
            this.m_btnDirectoryBrowse.TabIndex = 2;
            this.m_btnDirectoryBrowse.Text = "&Browse";
            this.m_btnDirectoryBrowse.UseVisualStyleBackColor = true;
            this.m_btnDirectoryBrowse.Click += new System.EventHandler(this.m_btnDirectoryBrowse_Click);
            // 
            // m_btnOk
            // 
            this.m_btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Location = new System.Drawing.Point(176, 87);
            this.m_btnOk.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(100, 28);
            this.m_btnOk.TabIndex = 3;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Location = new System.Drawing.Point(284, 87);
            this.m_btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(100, 28);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(560, 129);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnDirectoryBrowse);
            this.Controls.Add(this.m_txtDirectoryPath);
            this.Controls.Add(this.m_lblDirectoryPath);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectDirectoryPath";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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