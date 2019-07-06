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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckUpdates));
            this.m_RdOpenWebPage = new System.Windows.Forms.RadioButton();
            this.m_RdRemindLater = new System.Windows.Forms.RadioButton();
            this.m_RdRemindForNextVersion = new System.Windows.Forms.RadioButton();
            this.m_RdDisableCheckUpdates = new System.Windows.Forms.RadioButton();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.mCheckUpdates_grpBox = new System.Windows.Forms.GroupBox();
            this.mInfoTxtBox = new System.Windows.Forms.TextBox();
            this.mCheckUpdates_grpBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_RdOpenWebPage
            // 
            resources.ApplyResources(this.m_RdOpenWebPage, "m_RdOpenWebPage");
            this.m_RdOpenWebPage.Checked = true;
            this.m_RdOpenWebPage.Name = "m_RdOpenWebPage";
            this.m_RdOpenWebPage.TabStop = true;
            this.m_RdOpenWebPage.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindLater
            // 
            resources.ApplyResources(this.m_RdRemindLater, "m_RdRemindLater");
            this.m_RdRemindLater.Name = "m_RdRemindLater";
            this.m_RdRemindLater.TabStop = true;
            this.m_RdRemindLater.UseVisualStyleBackColor = true;
            // 
            // m_RdRemindForNextVersion
            // 
            resources.ApplyResources(this.m_RdRemindForNextVersion, "m_RdRemindForNextVersion");
            this.m_RdRemindForNextVersion.Name = "m_RdRemindForNextVersion";
            this.m_RdRemindForNextVersion.TabStop = true;
            this.m_RdRemindForNextVersion.UseVisualStyleBackColor = true;
            // 
            // m_RdDisableCheckUpdates
            // 
            resources.ApplyResources(this.m_RdDisableCheckUpdates, "m_RdDisableCheckUpdates");
            this.m_RdDisableCheckUpdates.Name = "m_RdDisableCheckUpdates";
            this.m_RdDisableCheckUpdates.TabStop = true;
            this.m_RdDisableCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // m_btnOk
            // 
            resources.ApplyResources(this.m_btnOk, "m_btnOk");
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // mCheckUpdates_grpBox
            // 
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdOpenWebPage);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdRemindLater);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdRemindForNextVersion);
            this.mCheckUpdates_grpBox.Controls.Add(this.m_RdDisableCheckUpdates);
            resources.ApplyResources(this.mCheckUpdates_grpBox, "mCheckUpdates_grpBox");
            this.mCheckUpdates_grpBox.Name = "mCheckUpdates_grpBox";
            this.mCheckUpdates_grpBox.TabStop = false;
            // 
            // mInfoTxtBox
            // 
            resources.ApplyResources(this.mInfoTxtBox, "mInfoTxtBox");
            this.mInfoTxtBox.Name = "mInfoTxtBox";
            this.mInfoTxtBox.ReadOnly = true;
            // 
            // CheckUpdates
            // 
            this.AcceptButton = this.m_btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.mInfoTxtBox);
            this.Controls.Add(this.mCheckUpdates_grpBox);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.MaximizeBox = false;
            this.Name = "CheckUpdates";
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
        private System.Windows.Forms.GroupBox mCheckUpdates_grpBox;
        private System.Windows.Forms.TextBox mInfoTxtBox;
    }
}