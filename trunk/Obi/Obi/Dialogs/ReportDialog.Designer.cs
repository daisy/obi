namespace Obi.Dialogs
{
    partial class ReportDialog
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
            this.m_lbDetailsOfImportedFiles = new System.Windows.Forms.ListBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnDetails = new System.Windows.Forms.Button();
            this.m_lblReportDialog = new System.Windows.Forms.Label();
            this.m_txtBoxPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // m_lbDetailsOfImportedFiles
            // 
            this.m_lbDetailsOfImportedFiles.AccessibleName = "Details description";
            this.m_lbDetailsOfImportedFiles.FormattingEnabled = true;
            this.m_lbDetailsOfImportedFiles.Location = new System.Drawing.Point(25, 124);
            this.m_lbDetailsOfImportedFiles.Name = "m_lbDetailsOfImportedFiles";
            this.m_lbDetailsOfImportedFiles.Size = new System.Drawing.Size(400, 121);
            this.m_lbDetailsOfImportedFiles.TabIndex = 3;
            // 
            // m_btnOk
            // 
            this.m_btnOk.AccessibleName = "OK";
            this.m_btnOk.Location = new System.Drawing.Point(98, 70);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 1;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnDetails
            // 
            this.m_btnDetails.AccessibleName = "Show details";
            this.m_btnDetails.Enabled = false;
            this.m_btnDetails.Location = new System.Drawing.Point(263, 70);
            this.m_btnDetails.Name = "m_btnDetails";
            this.m_btnDetails.Size = new System.Drawing.Size(75, 23);
            this.m_btnDetails.TabIndex = 2;
            this.m_btnDetails.Text = "Show &details";
            this.m_btnDetails.UseVisualStyleBackColor = true;
            this.m_btnDetails.Click += new System.EventHandler(this.m_btnDetails_Click);
            // 
            // m_lblReportDialog
            // 
            this.m_lblReportDialog.AutoSize = true;
            this.m_lblReportDialog.Location = new System.Drawing.Point(63, 25);
            this.m_lblReportDialog.Name = "m_lblReportDialog";
            this.m_lblReportDialog.Size = new System.Drawing.Size(35, 13);
            this.m_lblReportDialog.TabIndex = 3;
            this.m_lblReportDialog.Text = "label1";
            this.m_lblReportDialog.Visible = false;
            // 
            // m_txtBoxPath
            // 
            this.m_txtBoxPath.AccessibleName = "Path";
            this.m_txtBoxPath.Location = new System.Drawing.Point(25, 25);
            this.m_txtBoxPath.Name = "m_txtBoxPath";
            this.m_txtBoxPath.ReadOnly = true;
            this.m_txtBoxPath.Size = new System.Drawing.Size(400, 20);
            this.m_txtBoxPath.TabIndex = 0;
            // 
            // ReportDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 118);
            this.Controls.Add(this.m_txtBoxPath);
            this.Controls.Add(this.m_lblReportDialog);
            this.Controls.Add(this.m_btnDetails);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_lbDetailsOfImportedFiles);
            this.Location = new System.Drawing.Point(600, 200);
            this.Name = "ReportDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_lbDetailsOfImportedFiles;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnDetails;
        private System.Windows.Forms.Label m_lblReportDialog;
        private System.Windows.Forms.TextBox m_txtBoxPath;
    }
}