namespace Obi.Dialogs
{
    partial class Epub3Validator
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
            this.m_lblSelectInputFile = new System.Windows.Forms.Label();
            this.m_lblInputEPUB = new System.Windows.Forms.Label();
            this.m_txtInputEPUB = new System.Windows.Forms.TextBox();
            this.m_btnBrowseInputOPF = new System.Windows.Forms.Button();
            this.m_lblSelectValidationFile = new System.Windows.Forms.Label();
            this.m_lblValidationReport = new System.Windows.Forms.Label();
            this.m_btnBrowseReport = new System.Windows.Forms.Button();
            this.m_txtValidationReport = new System.Windows.Forms.TextBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.m_lblEpubCompletionStatus = new System.Windows.Forms.Label();
            this.m_epubCheckRichTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // m_lblSelectInputFile
            // 
            this.m_lblSelectInputFile.AutoSize = true;
            this.m_lblSelectInputFile.Location = new System.Drawing.Point(21, 38);
            this.m_lblSelectInputFile.Name = "m_lblSelectInputFile";
            this.m_lblSelectInputFile.Size = new System.Drawing.Size(133, 15);
            this.m_lblSelectInputFile.TabIndex = 0;
            this.m_lblSelectInputFile.Text = "Select the  EPUB 3 File";
            // 
            // m_lblInputEPUB
            // 
            this.m_lblInputEPUB.AutoSize = true;
            this.m_lblInputEPUB.Location = new System.Drawing.Point(61, 71);
            this.m_lblInputEPUB.Name = "m_lblInputEPUB";
            this.m_lblInputEPUB.Size = new System.Drawing.Size(70, 15);
            this.m_lblInputEPUB.TabIndex = 1;
            this.m_lblInputEPUB.Text = "Input EPUB";
            // 
            // m_txtInputEPUB
            // 
            this.m_txtInputEPUB.Location = new System.Drawing.Point(137, 71);
            this.m_txtInputEPUB.Name = "m_txtInputEPUB";
            this.m_txtInputEPUB.Size = new System.Drawing.Size(258, 21);
            this.m_txtInputEPUB.TabIndex = 2;
            // 
            // m_btnBrowseInputOPF
            // 
            this.m_btnBrowseInputOPF.Location = new System.Drawing.Point(422, 68);
            this.m_btnBrowseInputOPF.Name = "m_btnBrowseInputOPF";
            this.m_btnBrowseInputOPF.Size = new System.Drawing.Size(79, 26);
            this.m_btnBrowseInputOPF.TabIndex = 3;
            this.m_btnBrowseInputOPF.Text = "Browse";
            this.m_btnBrowseInputOPF.UseVisualStyleBackColor = true;
            this.m_btnBrowseInputOPF.Click += new System.EventHandler(this.m_btnBrowseInputOPF_Click);
            // 
            // m_lblSelectValidationFile
            // 
            this.m_lblSelectValidationFile.AutoSize = true;
            this.m_lblSelectValidationFile.Location = new System.Drawing.Point(21, 123);
            this.m_lblSelectValidationFile.Name = "m_lblSelectValidationFile";
            this.m_lblSelectValidationFile.Size = new System.Drawing.Size(257, 15);
            this.m_lblSelectValidationFile.TabIndex = 4;
            this.m_lblSelectValidationFile.Text = "Select a file to store a Validation XML report in";
            // 
            // m_lblValidationReport
            // 
            this.m_lblValidationReport.AutoSize = true;
            this.m_lblValidationReport.Location = new System.Drawing.Point(21, 165);
            this.m_lblValidationReport.Name = "m_lblValidationReport";
            this.m_lblValidationReport.Size = new System.Drawing.Size(101, 15);
            this.m_lblValidationReport.TabIndex = 5;
            this.m_lblValidationReport.Text = "Validation Report";
            // 
            // m_btnBrowseReport
            // 
            this.m_btnBrowseReport.Location = new System.Drawing.Point(422, 162);
            this.m_btnBrowseReport.Name = "m_btnBrowseReport";
            this.m_btnBrowseReport.Size = new System.Drawing.Size(79, 26);
            this.m_btnBrowseReport.TabIndex = 7;
            this.m_btnBrowseReport.Text = "Browse";
            this.m_btnBrowseReport.UseVisualStyleBackColor = true;
            // 
            // m_txtValidationReport
            // 
            this.m_txtValidationReport.Location = new System.Drawing.Point(137, 165);
            this.m_txtValidationReport.Name = "m_txtValidationReport";
            this.m_txtValidationReport.Size = new System.Drawing.Size(258, 21);
            this.m_txtValidationReport.TabIndex = 6;
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Location = new System.Drawing.Point(160, 251);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 30);
            this.m_btnOk.TabIndex = 8;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Location = new System.Drawing.Point(271, 251);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 30);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_openFileDialogBrowse
            // 
            this.m_openFileDialogBrowse.FileName = "openFileDialog1";
            // 
            // m_lblEpubCompletionStatus
            // 
            this.m_lblEpubCompletionStatus.AutoSize = true;
            this.m_lblEpubCompletionStatus.Location = new System.Drawing.Point(21, 38);
            this.m_lblEpubCompletionStatus.Name = "m_lblEpubCompletionStatus";
            this.m_lblEpubCompletionStatus.Size = new System.Drawing.Size(139, 15);
            this.m_lblEpubCompletionStatus.TabIndex = 10;
            this.m_lblEpubCompletionStatus.Text = "Epub Completion Status";
            // 
            // m_epubCheckRichTextBox
            // 
            this.m_epubCheckRichTextBox.Location = new System.Drawing.Point(24, 68);
            this.m_epubCheckRichTextBox.Name = "m_epubCheckRichTextBox";
            this.m_epubCheckRichTextBox.Size = new System.Drawing.Size(454, 177);
            this.m_epubCheckRichTextBox.TabIndex = 11;
            this.m_epubCheckRichTextBox.Text = "Epub Completion Text";
            // 
            // Epub3Validator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 293);
            this.Controls.Add(this.m_epubCheckRichTextBox);
            this.Controls.Add(this.m_lblEpubCompletionStatus);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnBrowseReport);
            this.Controls.Add(this.m_txtValidationReport);
            this.Controls.Add(this.m_lblValidationReport);
            this.Controls.Add(this.m_lblSelectValidationFile);
            this.Controls.Add(this.m_btnBrowseInputOPF);
            this.Controls.Add(this.m_txtInputEPUB);
            this.Controls.Add(this.m_lblInputEPUB);
            this.Controls.Add(this.m_lblSelectInputFile);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Epub3Validator";
            this.Text = "Epub 3 Validator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblSelectInputFile;
        private System.Windows.Forms.Label m_lblInputEPUB;
        private System.Windows.Forms.TextBox m_txtInputEPUB;
        private System.Windows.Forms.Button m_btnBrowseInputOPF;
        private System.Windows.Forms.Label m_lblSelectValidationFile;
        private System.Windows.Forms.Label m_lblValidationReport;
        private System.Windows.Forms.Button m_btnBrowseReport;
        private System.Windows.Forms.TextBox m_txtValidationReport;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.OpenFileDialog m_openFileDialogBrowse;
        private System.Windows.Forms.Label m_lblEpubCompletionStatus;
        private System.Windows.Forms.RichTextBox m_epubCheckRichTextBox;

    }
}