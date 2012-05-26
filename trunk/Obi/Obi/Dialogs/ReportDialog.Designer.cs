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
            this.m_lbDetailsOfErrors = new System.Windows.Forms.ListBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnDetails = new System.Windows.Forms.Button();
            this.m_lblReportDialog = new System.Windows.Forms.Label();
            this.m_grpBox_lb_ErrorsList = new System.Windows.Forms.GroupBox();
            this.m_txtBoxPath = new System.Windows.Forms.TextBox();
            this.m_grpBox_lb_ErrorsList.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_lbDetailsOfErrors
            // 
            this.m_lbDetailsOfErrors.AccessibleName = "Details description";
            this.m_lbDetailsOfErrors.FormattingEnabled = true;
            this.m_lbDetailsOfErrors.Location = new System.Drawing.Point(6, 16);
            this.m_lbDetailsOfErrors.Name = "m_lbDetailsOfErrors";
            this.m_lbDetailsOfErrors.Size = new System.Drawing.Size(400, 121);
            this.m_lbDetailsOfErrors.TabIndex = 4;
            // 
            // m_btnOk
            // 
            this.m_btnOk.AccessibleName = "OK";
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnOk.Location = new System.Drawing.Point(98, 71);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(100, 28);
            this.m_btnOk.TabIndex = 1;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnDetails
            // 
            this.m_btnDetails.AccessibleName = "Show details";
            this.m_btnDetails.Enabled = false;
            this.m_btnDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnDetails.Location = new System.Drawing.Point(264, 71);
            this.m_btnDetails.Name = "m_btnDetails";
            this.m_btnDetails.Size = new System.Drawing.Size(100, 28);
            this.m_btnDetails.TabIndex = 2;
            this.m_btnDetails.Text = "Show &details";
            this.m_btnDetails.UseVisualStyleBackColor = true;
            this.m_btnDetails.Click += new System.EventHandler(this.m_btnDetails_Click);
            // 
            // m_lblReportDialog
            // 
            this.m_lblReportDialog.AutoSize = true;
            this.m_lblReportDialog.Location = new System.Drawing.Point(28, 25);
            this.m_lblReportDialog.Name = "m_lblReportDialog";
            this.m_lblReportDialog.Size = new System.Drawing.Size(35, 13);
            this.m_lblReportDialog.TabIndex = 3;
            this.m_lblReportDialog.Text = "label1";
            this.m_lblReportDialog.Visible = false;
            // 
            // m_grpBox_lb_ErrorsList
            // 
            this.m_grpBox_lb_ErrorsList.Controls.Add(this.m_lbDetailsOfErrors);
            this.m_grpBox_lb_ErrorsList.Location = new System.Drawing.Point(13, 115);
            this.m_grpBox_lb_ErrorsList.Name = "m_grpBox_lb_ErrorsList";
            this.m_grpBox_lb_ErrorsList.Size = new System.Drawing.Size(412, 142);
            this.m_grpBox_lb_ErrorsList.TabIndex = 3;
            this.m_grpBox_lb_ErrorsList.TabStop = false;
            this.m_grpBox_lb_ErrorsList.Text = "Error l&ist";
            // 
            // m_txtBoxPath
            // 
            this.m_txtBoxPath.Location = new System.Drawing.Point(19, 25);
            this.m_txtBoxPath.Multiline = true;
            this.m_txtBoxPath.Name = "m_txtBoxPath";
            this.m_txtBoxPath.ReadOnly = true;
            this.m_txtBoxPath.Size = new System.Drawing.Size(400, 35);
            this.m_txtBoxPath.TabIndex = 0;
            // 
            // ReportDialog
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 112);
            this.Controls.Add(this.m_txtBoxPath);
            this.Controls.Add(this.m_grpBox_lb_ErrorsList);
            this.Controls.Add(this.m_lblReportDialog);
            this.Controls.Add(this.m_btnDetails);
            this.Controls.Add(this.m_btnOk);
            this.Location = new System.Drawing.Point(600, 200);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(453, 150);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(453, 150);
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
    }
}