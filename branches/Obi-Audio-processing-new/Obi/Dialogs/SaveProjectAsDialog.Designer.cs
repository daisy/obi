namespace Obi.Dialogs
{
    partial class SaveProjectAsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SaveProjectAsDialog));
            this.mSelectButton = new System.Windows.Forms.Button();
            this.m_lblParentDirectoryPath = new System.Windows.Forms.Label();
            this.mLocationTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSwitchToNewCheckBox = new System.Windows.Forms.CheckBox();
            this.m_ProjectNameTextBox = new System.Windows.Forms.TextBox();
            this.m_lbl_ProjectName = new System.Windows.Forms.Label();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // mSelectButton
            // 
            resources.ApplyResources(this.mSelectButton, "mSelectButton");
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.mSelectButton_Click);
            // 
            // m_lblParentDirectoryPath
            // 
            resources.ApplyResources(this.m_lblParentDirectoryPath, "m_lblParentDirectoryPath");
            this.m_lblParentDirectoryPath.Name = "m_lblParentDirectoryPath";
            // 
            // mLocationTextBox
            // 
            this.mLocationTextBox.AccessibleName = global::Obi.messages.phrase_extra_Plain;
            resources.ApplyResources(this.mLocationTextBox, "mLocationTextBox");
            this.mLocationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLocationTextBox.Name = "mLocationTextBox";
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mSwitchToNewCheckBox
            // 
            resources.ApplyResources(this.mSwitchToNewCheckBox, "mSwitchToNewCheckBox");
            this.mSwitchToNewCheckBox.Checked = true;
            this.mSwitchToNewCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mSwitchToNewCheckBox.Name = "mSwitchToNewCheckBox";
            this.mSwitchToNewCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_ProjectNameTextBox
            // 
            resources.ApplyResources(this.m_ProjectNameTextBox, "m_ProjectNameTextBox");
            this.m_ProjectNameTextBox.Name = "m_ProjectNameTextBox";
            // 
            // m_lbl_ProjectName
            // 
            resources.ApplyResources(this.m_lbl_ProjectName, "m_lbl_ProjectName");
            this.m_lbl_ProjectName.Name = "m_lbl_ProjectName";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SaveProjectAsDialog
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.m_lbl_ProjectName);
            this.Controls.Add(this.m_ProjectNameTextBox);
            this.Controls.Add(this.mSwitchToNewCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mLocationTextBox);
            this.Controls.Add(this.m_lblParentDirectoryPath);
            this.Controls.Add(this.mSelectButton);
            this.Name = "SaveProjectAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveProjectAsDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mSelectButton;
        private System.Windows.Forms.Label m_lblParentDirectoryPath;
        private System.Windows.Forms.TextBox mLocationTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mSwitchToNewCheckBox;
        private System.Windows.Forms.TextBox m_ProjectNameTextBox;
        private System.Windows.Forms.Label m_lbl_ProjectName;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}