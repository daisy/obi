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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Epub3Validator));
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
            resources.ApplyResources(this.m_lblSelectInputFile, "m_lblSelectInputFile");
            this.m_lblSelectInputFile.Name = "m_lblSelectInputFile";
            // 
            // m_lblInputEPUB
            // 
            resources.ApplyResources(this.m_lblInputEPUB, "m_lblInputEPUB");
            this.m_lblInputEPUB.Name = "m_lblInputEPUB";
            // 
            // m_txtInputEPUB
            // 
            resources.ApplyResources(this.m_txtInputEPUB, "m_txtInputEPUB");
            this.m_txtInputEPUB.Name = "m_txtInputEPUB";
            // 
            // m_btnBrowseInputOPF
            // 
            resources.ApplyResources(this.m_btnBrowseInputOPF, "m_btnBrowseInputOPF");
            this.m_btnBrowseInputOPF.Name = "m_btnBrowseInputOPF";
            this.m_btnBrowseInputOPF.UseVisualStyleBackColor = true;
            this.m_btnBrowseInputOPF.Click += new System.EventHandler(this.m_btnBrowseInputOPF_Click);
            // 
            // m_lblSelectValidationFile
            // 
            resources.ApplyResources(this.m_lblSelectValidationFile, "m_lblSelectValidationFile");
            this.m_lblSelectValidationFile.Name = "m_lblSelectValidationFile";
            // 
            // m_lblValidationReport
            // 
            resources.ApplyResources(this.m_lblValidationReport, "m_lblValidationReport");
            this.m_lblValidationReport.Name = "m_lblValidationReport";
            // 
            // m_btnBrowseReport
            // 
            resources.ApplyResources(this.m_btnBrowseReport, "m_btnBrowseReport");
            this.m_btnBrowseReport.Name = "m_btnBrowseReport";
            this.m_btnBrowseReport.UseVisualStyleBackColor = true;
            // 
            // m_txtValidationReport
            // 
            resources.ApplyResources(this.m_txtValidationReport, "m_txtValidationReport");
            this.m_txtValidationReport.Name = "m_txtValidationReport";
            // 
            // m_btnOk
            // 
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_openFileDialogBrowse
            // 
            this.m_openFileDialogBrowse.FileName = "openFileDialog1";
            // 
            // m_lblEpubCompletionStatus
            // 
            resources.ApplyResources(this.m_lblEpubCompletionStatus, "m_lblEpubCompletionStatus");
            this.m_lblEpubCompletionStatus.Name = "m_lblEpubCompletionStatus";
            // 
            // m_epubCheckRichTextBox
            // 
            resources.ApplyResources(this.m_epubCheckRichTextBox, "m_epubCheckRichTextBox");
            this.m_epubCheckRichTextBox.Name = "m_epubCheckRichTextBox";
            // 
            // Epub3Validator
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Name = "Epub3Validator";
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