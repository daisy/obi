namespace Obi.Dialogs
{
    partial class CheckUpdates
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
            this.m_RdOpenWebPage = new System.Windows.Forms.RadioButton();
            this.m_RdRemindLater = new System.Windows.Forms.RadioButton();
            this.m_RdRemindForNextVersion = new System.Windows.Forms.RadioButton();
            this.m_RdDisableCheckUpdates = new System.Windows.Forms.RadioButton();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblInfo = new System.Windows.Forms.Label();
            this.mCheckUpdates_grpBox = new System.Windows.Forms.GroupBox();
            this.mCheckUpdates_grpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_RdOpenWebPage
            // 
            this.m_RdOpenWebPage.AutoSize = true;
            this.m_RdOpenWebPage.Checked = true;
            this.m_RdOpenWebPage.Location = new System.Drawing.Point(6, 19);
            this.m_RdOpenWebPage.Name = "m_RdOpenWebPage";
            this.m_RdOpenWebPage.Size = new System.Drawing.Size(179, 17);
            this.m_RdOpenWebPage.TabIndex = 2;
            this.m_RdOpenWebPage.TabStop = true;
            this.m_RdOpenWebPage.Text = "&Open the new version web page";
            this.m_RdOpenWebPage.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindLater
            // 
            this.m_RdRemindLater.AutoSize = true;
            this.m_RdRemindLater.Location = new System.Drawing.Point(6, 42);
            this.m_RdRemindLater.Name = "m_RdRemindLater";
            this.m_RdRemindLater.Size = new System.Drawing.Size(113, 17);
            this.m_RdRemindLater.TabIndex = 3;
            this.m_RdRemindLater.TabStop = true;
            this.m_RdRemindLater.Text = "Remind again &later";
            this.m_RdRemindLater.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindForNextVersion
            // 
            this.m_RdRemindForNextVersion.AutoSize = true;
            this.m_RdRemindForNextVersion.Location = new System.Drawing.Point(6, 65);
            this.m_RdRemindForNextVersion.Name = "m_RdRemindForNextVersion";
            this.m_RdRemindForNextVersion.Size = new System.Drawing.Size(158, 17);
            this.m_RdRemindForNextVersion.TabIndex = 4;
            this.m_RdRemindForNextVersion.TabStop = true;
            this.m_RdRemindForNextVersion.Text = "Remind only for &next version";
            this.m_RdRemindForNextVersion.UseVisualStyleBackColor = true;
            // 
            // m_RdDisableCheckUpdates
            // 
            this.m_RdDisableCheckUpdates.AutoSize = true;
            this.m_RdDisableCheckUpdates.Location = new System.Drawing.Point(6, 90);
            this.m_RdDisableCheckUpdates.Name = "m_RdDisableCheckUpdates";
            this.m_RdDisableCheckUpdates.Size = new System.Drawing.Size(179, 17);
            this.m_RdDisableCheckUpdates.TabIndex = 5;
            this.m_RdDisableCheckUpdates.TabStop = true;
            this.m_RdDisableCheckUpdates.Text = "&Disable new version notifications";
            this.m_RdDisableCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnOk.Location = new System.Drawing.Point(61, 171);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 6;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnCancel.Location = new System.Drawing.Point(267, 171);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 7;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_lblInfo
            // 
            this.m_lblInfo.AutoSize = true;
            this.m_lblInfo.Location = new System.Drawing.Point(12, 9);
            this.m_lblInfo.Name = "m_lblInfo";
            this.m_lblInfo.Size = new System.Drawing.Size(151, 13);
            this.m_lblInfo.TabIndex = 1;
            this.m_lblInfo.Text = "&Please select from the options.";
            // 
            // mCheckUpdates_grpBox
            // 
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdOpenWebPage);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdRemindLater);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdRemindForNextVersion);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdDisableCheckUpdates);
            this.mCheckUpdates_grpBox.Location = new System.Drawing.Point(15, 36);
            this.mCheckUpdates_grpBox.Name = "mCheckUpdates_grpBox";
            this.mCheckUpdates_grpBox.Size = new System.Drawing.Size(369, 123);
            this.mCheckUpdates_grpBox.TabIndex = 1;
            this.mCheckUpdates_grpBox.TabStop = false;
            // 
            // CheckUpdates
            // 
            this.AcceptButton = this.m_btnOk;
            this.AccessibleName = "Check Updates";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(410, 204);
            this.Controls.Add(this.mCheckUpdates_grpBox);
            this.Controls.Add(this.m_lblInfo);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.MaximizeBox = false;
            this.Name = "CheckUpdates";
            this.Text = "CheckUpdates";
            this.mCheckUpdates_grpBox.ResumeLayout(false);
            this.mCheckUpdates_grpBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_RdOpenWebPage;
        private System.Windows.Forms.RadioButton m_RdRemindLater;
        private System.Windows.Forms.RadioButton m_RdRemindForNextVersion;
        private System.Windows.Forms.RadioButton m_RdDisableCheckUpdates;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblInfo;
        private System.Windows.Forms.GroupBox mCheckUpdates_grpBox;
    }
}