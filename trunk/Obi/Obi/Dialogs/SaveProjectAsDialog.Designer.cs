namespace Obi.Dialogs
{
    partial class SaveProjectAsDialog
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
            this.m_lblProjectDirectoryName = new System.Windows.Forms.Label();
            this.m_txtProjectDirectoryName = new System.Windows.Forms.TextBox();
            this.m_btnBrowseParentDirectory = new System.Windows.Forms.Button();
            this.m_lblParentDirectoryPath = new System.Windows.Forms.Label();
            this.m_txtParentDirectory = new System.Windows.Forms.TextBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.m_chkSavePrimaryDirectories = new System.Windows.Forms.CheckBox();
            this.m_chkActivateNewProject = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_lblProjectDirectoryName
            // 
            this.m_lblProjectDirectoryName.AutoSize = true;
            this.m_lblProjectDirectoryName.Location = new System.Drawing.Point(10, 10);
            this.m_lblProjectDirectoryName.Name = "m_lblProjectDirectoryName";
            this.m_lblProjectDirectoryName.Size = new System.Drawing.Size(85, 13);
            this.m_lblProjectDirectoryName.TabIndex = 0;
            this.m_lblProjectDirectoryName.Text = "Save Project as:";
            // 
            // m_txtProjectDirectoryName
            // 
            this.m_txtProjectDirectoryName.Location = new System.Drawing.Point(150, 8);
            this.m_txtProjectDirectoryName.Name = "m_txtProjectDirectoryName";
            this.m_txtProjectDirectoryName.Size = new System.Drawing.Size(100, 20);
            this.m_txtProjectDirectoryName.TabIndex = 1;
            // 
            // m_btnBrowseParentDirectory
            // 
            this.m_btnBrowseParentDirectory.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnBrowseParentDirectory.Location = new System.Drawing.Point(200, 80);
            this.m_btnBrowseParentDirectory.Name = "m_btnBrowseParentDirectory";
            this.m_btnBrowseParentDirectory.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseParentDirectory.TabIndex = 5;
            this.m_btnBrowseParentDirectory.Text = "&browse";
            this.m_btnBrowseParentDirectory.UseVisualStyleBackColor = true;
            this.m_btnBrowseParentDirectory.Click += new System.EventHandler(this.m_btnBrowseParentDirectory_Click);
            // 
            // m_lblParentDirectoryPath
            // 
            this.m_lblParentDirectoryPath.AutoSize = true;
            this.m_lblParentDirectoryPath.Location = new System.Drawing.Point(10, 50);
            this.m_lblParentDirectoryPath.Name = "m_lblParentDirectoryPath";
            this.m_lblParentDirectoryPath.Size = new System.Drawing.Size(123, 13);
            this.m_lblParentDirectoryPath.TabIndex = 3;
            this.m_lblParentDirectoryPath.Text = "Directory path for project";
            // 
            // m_txtParentDirectory
            // 
            this.m_txtParentDirectory.Location = new System.Drawing.Point(150, 50);
            this.m_txtParentDirectory.Name = "m_txtParentDirectory";
            this.m_txtParentDirectory.Size = new System.Drawing.Size(150, 20);
            this.m_txtParentDirectory.TabIndex = 4;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(130, 150);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 8;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(215, 150);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_chkSavePrimaryDirectories
            // 
            this.m_chkSavePrimaryDirectories.AutoSize = true;
            this.m_chkSavePrimaryDirectories.Location = new System.Drawing.Point(0, 115);
            this.m_chkSavePrimaryDirectories.Name = "m_chkSavePrimaryDirectories";
            this.m_chkSavePrimaryDirectories.Size = new System.Drawing.Size(160, 17);
            this.m_chkSavePrimaryDirectories.TabIndex = 6;
            this.m_chkSavePrimaryDirectories.Text = "Save &primary directories only";
            this.m_chkSavePrimaryDirectories.UseVisualStyleBackColor = true;
            // 
            // m_chkActivateNewProject
            // 
            this.m_chkActivateNewProject.AutoSize = true;
            this.m_chkActivateNewProject.Location = new System.Drawing.Point(0, 128);
            this.m_chkActivateNewProject.Name = "m_chkActivateNewProject";
            this.m_chkActivateNewProject.Size = new System.Drawing.Size(123, 17);
            this.m_chkActivateNewProject.TabIndex = 8;
            this.m_chkActivateNewProject.Text = "Activate &new project";
            this.m_chkActivateNewProject.UseVisualStyleBackColor = true;
            // 
            // SaveProjectAsDialog
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 173);
            this.Controls.Add(this.m_chkActivateNewProject);
            this.Controls.Add(this.m_chkSavePrimaryDirectories);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_txtParentDirectory);
            this.Controls.Add(this.m_lblParentDirectoryPath);
            this.Controls.Add(this.m_btnBrowseParentDirectory);
            this.Controls.Add(this.m_txtProjectDirectoryName);
            this.Controls.Add(this.m_lblProjectDirectoryName);
            this.Name = "SaveProjectAsDialog";
            this.Text = "Save project as";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblProjectDirectoryName;
        private System.Windows.Forms.TextBox m_txtProjectDirectoryName;
        private System.Windows.Forms.Button m_btnBrowseParentDirectory;
        private System.Windows.Forms.Label m_lblParentDirectoryPath;
        private System.Windows.Forms.TextBox m_txtParentDirectory;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox m_chkSavePrimaryDirectories;
        private System.Windows.Forms.CheckBox m_chkActivateNewProject;
    }
}