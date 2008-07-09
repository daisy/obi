namespace Obi.PipelineInterface
{
    partial class ValidatorForm
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
            this.m_lblDTBFilePath = new System.Windows.Forms.Label();
            this.m_txtDTBFilePath = new System.Windows.Forms.TextBox();
            this.m_btnBrowseDTBFilePath = new System.Windows.Forms.Button();
            this.m_chkReportToFile = new System.Windows.Forms.CheckBox();
            this.m_lblErrorReportFilePath = new System.Windows.Forms.Label();
            this.m_txtErrorFilePath = new System.Windows.Forms.TextBox();
            this.m_btnBrowseErrorFile = new System.Windows.Forms.Button();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // m_lblDTBFilePath
            // 
            this.m_lblDTBFilePath.AutoSize = true;
            this.m_lblDTBFilePath.Location = new System.Drawing.Point(0, 10);
            this.m_lblDTBFilePath.Name = "m_lblDTBFilePath";
            this.m_lblDTBFilePath.Size = new System.Drawing.Size(163, 13);
            this.m_lblDTBFilePath.TabIndex = 0;
            this.m_lblDTBFilePath.Text = "DTB &File path (.opf/ncc.html file):";
            // 
            // m_txtDTBFilePath
            // 
            this.m_txtDTBFilePath.AccessibleName = "DTB File path (.opf/ncc.html):";
            this.m_txtDTBFilePath.Location = new System.Drawing.Point(120, 10);
            this.m_txtDTBFilePath.Name = "m_txtDTBFilePath";
            this.m_txtDTBFilePath.Size = new System.Drawing.Size(150, 20);
            this.m_txtDTBFilePath.TabIndex = 1;
            // 
            // m_btnBrowseDTBFilePath
            // 
            this.m_btnBrowseDTBFilePath.Location = new System.Drawing.Point(210, 40);
            this.m_btnBrowseDTBFilePath.Name = "m_btnBrowseDTBFilePath";
            this.m_btnBrowseDTBFilePath.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseDTBFilePath.TabIndex = 2;
            this.m_btnBrowseDTBFilePath.Text = "&Browse";
            this.m_btnBrowseDTBFilePath.UseVisualStyleBackColor = true;
            this.m_btnBrowseDTBFilePath.Click += new System.EventHandler(this.m_btnBrowseDTBFilePath_Click);
            // 
            // m_chkReportToFile
            // 
            this.m_chkReportToFile.AutoSize = true;
            this.m_chkReportToFile.Location = new System.Drawing.Point(0, 100);
            this.m_chkReportToFile.Name = "m_chkReportToFile";
            this.m_chkReportToFile.Size = new System.Drawing.Size(130, 17);
            this.m_chkReportToFile.TabIndex = 3;
            this.m_chkReportToFile.Text = "Create &error report file:";
            this.m_chkReportToFile.UseVisualStyleBackColor = true;
            this.m_chkReportToFile.CheckedChanged += new System.EventHandler(this.m_chkReportToFile_CheckedChanged);
            // 
            // m_lblErrorReportFilePath
            // 
            this.m_lblErrorReportFilePath.AutoSize = true;
            this.m_lblErrorReportFilePath.Enabled = false;
            this.m_lblErrorReportFilePath.Location = new System.Drawing.Point(0, 125);
            this.m_lblErrorReportFilePath.Name = "m_lblErrorReportFilePath";
            this.m_lblErrorReportFilePath.Size = new System.Drawing.Size(78, 13);
            this.m_lblErrorReportFilePath.TabIndex = 4;
            this.m_lblErrorReportFilePath.Text = "&Error report file:";
            // 
            // m_txtErrorFilePath
            // 
            this.m_txtErrorFilePath.AccessibleName = "Error report file path:";
            this.m_txtErrorFilePath.Enabled = false;
            this.m_txtErrorFilePath.Location = new System.Drawing.Point(120, 125);
            this.m_txtErrorFilePath.Name = "m_txtErrorFilePath";
            this.m_txtErrorFilePath.Size = new System.Drawing.Size(150, 20);
            this.m_txtErrorFilePath.TabIndex = 5;
            // 
            // m_btnBrowseErrorFile
            // 
            this.m_btnBrowseErrorFile.Enabled = false;
            this.m_btnBrowseErrorFile.Location = new System.Drawing.Point(210, 155);
            this.m_btnBrowseErrorFile.Name = "m_btnBrowseErrorFile";
            this.m_btnBrowseErrorFile.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseErrorFile.TabIndex = 6;
            this.m_btnBrowseErrorFile.Text = "Browse";
            this.m_btnBrowseErrorFile.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Location = new System.Drawing.Point(140, 225);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 7;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(220, 225);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ValidatorForm
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnBrowseErrorFile);
            this.Controls.Add(this.m_txtErrorFilePath);
            this.Controls.Add(this.m_lblErrorReportFilePath);
            this.Controls.Add(this.m_chkReportToFile);
            this.Controls.Add(this.m_btnBrowseDTBFilePath);
            this.Controls.Add(this.m_txtDTBFilePath);
            this.Controls.Add(this.m_lblDTBFilePath);
            this.Name = "ValidatorForm";
            this.Text = "DTB Validator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDTBFilePath;
        private System.Windows.Forms.TextBox m_txtDTBFilePath;
        private System.Windows.Forms.Button m_btnBrowseDTBFilePath;
        private System.Windows.Forms.CheckBox m_chkReportToFile;
        private System.Windows.Forms.Label m_lblErrorReportFilePath;
        private System.Windows.Forms.TextBox m_txtErrorFilePath;
        private System.Windows.Forms.Button m_btnBrowseErrorFile;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}