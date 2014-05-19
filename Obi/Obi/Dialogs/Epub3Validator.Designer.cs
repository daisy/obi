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
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_openFileDialogBrowse = new System.Windows.Forms.OpenFileDialog();
            this.m_epubCheckRichTextBox = new System.Windows.Forms.RichTextBox();
            this.m_lblEpubCompletionStatus = new System.Windows.Forms.TextBox();
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
            this.m_openFileDialogBrowse.FileName = "";
            // 
            // m_epubCheckRichTextBox
            // 
            resources.ApplyResources(this.m_epubCheckRichTextBox, "m_epubCheckRichTextBox");
            this.m_epubCheckRichTextBox.Name = "m_epubCheckRichTextBox";
            // 
            // m_lblEpubCompletionStatus
            // 
            resources.ApplyResources(this.m_lblEpubCompletionStatus, "m_lblEpubCompletionStatus");
            this.m_lblEpubCompletionStatus.Name = "m_lblEpubCompletionStatus";
            this.m_lblEpubCompletionStatus.ReadOnly = true;
            // 
            // Epub3Validator
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_lblEpubCompletionStatus);
            this.Controls.Add(this.m_epubCheckRichTextBox);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
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
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.OpenFileDialog m_openFileDialogBrowse;
        private System.Windows.Forms.RichTextBox m_epubCheckRichTextBox;
        private System.Windows.Forms.TextBox m_lblEpubCompletionStatus;

    }
}