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
            this.SuspendLayout();
            // 
            // m_RdOpenWebPage
            // 
            this.m_RdOpenWebPage.AutoSize = true;
            this.m_RdOpenWebPage.Checked = true;
            this.m_RdOpenWebPage.Location = new System.Drawing.Point(10, 0);
            this.m_RdOpenWebPage.Name = "m_RdOpenWebPage";
            this.m_RdOpenWebPage.Size = new System.Drawing.Size(179, 17);
            this.m_RdOpenWebPage.TabIndex = 0;
            this.m_RdOpenWebPage.TabStop = true;
            this.m_RdOpenWebPage.Text = "Open the new version web page";
            this.m_RdOpenWebPage.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindLater
            // 
            this.m_RdRemindLater.AutoSize = true;
            this.m_RdRemindLater.Location = new System.Drawing.Point(10, 20);
            this.m_RdRemindLater.Name = "m_RdRemindLater";
            this.m_RdRemindLater.Size = new System.Drawing.Size(113, 17);
            this.m_RdRemindLater.TabIndex = 1;
            this.m_RdRemindLater.TabStop = true;
            this.m_RdRemindLater.Text = "Remind again later";
            this.m_RdRemindLater.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindForNextVersion
            // 
            this.m_RdRemindForNextVersion.AutoSize = true;
            this.m_RdRemindForNextVersion.Location = new System.Drawing.Point(10, 40);
            this.m_RdRemindForNextVersion.Name = "m_RdRemindForNextVersion";
            this.m_RdRemindForNextVersion.Size = new System.Drawing.Size(158, 17);
            this.m_RdRemindForNextVersion.TabIndex = 2;
            this.m_RdRemindForNextVersion.TabStop = true;
            this.m_RdRemindForNextVersion.Text = "Remind only for next version";
            this.m_RdRemindForNextVersion.UseVisualStyleBackColor = true;
            // 
            // m_RdDisableCheckUpdates
            // 
            this.m_RdDisableCheckUpdates.AutoSize = true;
            this.m_RdDisableCheckUpdates.Location = new System.Drawing.Point(10, 60);
            this.m_RdDisableCheckUpdates.Name = "m_RdDisableCheckUpdates";
            this.m_RdDisableCheckUpdates.Size = new System.Drawing.Size(179, 17);
            this.m_RdDisableCheckUpdates.TabIndex = 3;
            this.m_RdDisableCheckUpdates.TabStop = true;
            this.m_RdDisableCheckUpdates.Text = "Disable new version notifications";
            this.m_RdDisableCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(10, 227);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 4;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Location = new System.Drawing.Point(114, 256);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 5;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // m_lblInfo
            // 
            this.m_lblInfo.AutoSize = true;
            this.m_lblInfo.Location = new System.Drawing.Point(0, 0);
            this.m_lblInfo.Name = "m_lblInfo";
            this.m_lblInfo.Size = new System.Drawing.Size(151, 13);
            this.m_lblInfo.TabIndex = 6;
            this.m_lblInfo.Text = "Please select from the options.";
            // 
            // CheckUpdates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.m_lblInfo);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_RdDisableCheckUpdates);
            this.Controls.Add(this.m_RdRemindForNextVersion);
            this.Controls.Add(this.m_RdRemindLater);
            this.Controls.Add(this.m_RdOpenWebPage);
            this.Name = "CheckUpdates";
            this.Text = "CheckUpdates";
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
    }
}