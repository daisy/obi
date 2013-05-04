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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportDialog));
            this.m_lbDetailsOfErrors = new System.Windows.Forms.ListBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnDetails = new System.Windows.Forms.Button();
            this.m_lblReportDialog = new System.Windows.Forms.Label();
            this.m_grpBox_lb_ErrorsList = new System.Windows.Forms.GroupBox();
            this.m_txtBoxPath = new System.Windows.Forms.TextBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_grpBox_lb_ErrorsList.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lbDetailsOfErrors
            // 
            resources.ApplyResources(this.m_lbDetailsOfErrors, "m_lbDetailsOfErrors");
            this.m_lbDetailsOfErrors.FormattingEnabled = true;
            this.m_lbDetailsOfErrors.Name = "m_lbDetailsOfErrors";
            // 
            // m_btnOk
            // 
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnDetails
            // 
            resources.ApplyResources(this.m_btnDetails, "m_btnDetails");
            this.m_btnDetails.Name = "m_btnDetails";
            this.m_btnDetails.UseVisualStyleBackColor = true;
            this.m_btnDetails.Click += new System.EventHandler(this.m_btnDetails_Click);
            // 
            // m_lblReportDialog
            // 
            resources.ApplyResources(this.m_lblReportDialog, "m_lblReportDialog");
            this.m_lblReportDialog.Name = "m_lblReportDialog";
            // 
            // m_grpBox_lb_ErrorsList
            // 
            this.m_grpBox_lb_ErrorsList.Controls.Add(this.m_lbDetailsOfErrors);
            resources.ApplyResources(this.m_grpBox_lb_ErrorsList, "m_grpBox_lb_ErrorsList");
            this.m_grpBox_lb_ErrorsList.Name = "m_grpBox_lb_ErrorsList";
            this.m_grpBox_lb_ErrorsList.TabStop = false;
            // 
            // m_txtBoxPath
            // 
            resources.ApplyResources(this.m_txtBoxPath, "m_txtBoxPath");
            this.m_txtBoxPath.Name = "m_txtBoxPath";
            this.m_txtBoxPath.ReadOnly = true;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // ReportDialog
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_txtBoxPath);
            this.Controls.Add(this.m_grpBox_lb_ErrorsList);
            this.Controls.Add(this.m_lblReportDialog);
            this.Controls.Add(this.m_btnDetails);
            this.Controls.Add(this.m_btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportDialog";
            this.m_grpBox_lb_ErrorsList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_lbDetailsOfErrors;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnDetails;
        private System.Windows.Forms.Label m_lblReportDialog;
        private System.Windows.Forms.GroupBox m_grpBox_lb_ErrorsList;
        private System.Windows.Forms.TextBox m_txtBoxPath;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}