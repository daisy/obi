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
            this.mSelectButton = new System.Windows.Forms.Button();
            this.m_lblParentDirectoryPath = new System.Windows.Forms.Label();
            this.mLocationTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSwitchToNewCheckBox = new System.Windows.Forms.CheckBox();
            this.m_ProjectNameTextBox = new System.Windows.Forms.TextBox();
            this.m_lbl_ProjectName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mSelectButton
            // 
            this.mSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSelectButton.Location = new System.Drawing.Point(455, 18);
            this.mSelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.Size = new System.Drawing.Size(100, 28);
            this.mSelectButton.TabIndex = 2;
            this.mSelectButton.Text = "&Select";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.mSelectButton_Click);
            // 
            // m_lblParentDirectoryPath
            // 
            this.m_lblParentDirectoryPath.AccessibleName = "Location";
            this.m_lblParentDirectoryPath.AutoSize = true;
            this.m_lblParentDirectoryPath.Location = new System.Drawing.Point(60, 24);
            this.m_lblParentDirectoryPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_lblParentDirectoryPath.Name = "m_lblParentDirectoryPath";
            this.m_lblParentDirectoryPath.Size = new System.Drawing.Size(62, 16);
            this.m_lblParentDirectoryPath.TabIndex = 0;
            this.m_lblParentDirectoryPath.Text = "&Location:";
            // 
            // mLocationTextBox
            // 
            this.mLocationTextBox.AccessibleName = global::Obi.messages.Executing__0_;
            this.mLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mLocationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLocationTextBox.Location = new System.Drawing.Point(130, 22);
            this.mLocationTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.mLocationTextBox.Name = "mLocationTextBox";
            this.mLocationTextBox.Size = new System.Drawing.Size(308, 22);
            this.mLocationTextBox.TabIndex = 1;
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mOKButton.Location = new System.Drawing.Point(151, 134);
            this.mOKButton.Margin = new System.Windows.Forms.Padding(4);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(100, 28);
            this.mOKButton.TabIndex = 6;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mCancelButton.Location = new System.Drawing.Point(282, 134);
            this.mCancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(100, 28);
            this.mCancelButton.TabIndex = 7;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mSwitchToNewCheckBox
            // 
            this.mSwitchToNewCheckBox.AutoSize = true;
            this.mSwitchToNewCheckBox.Checked = true;
            this.mSwitchToNewCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mSwitchToNewCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mSwitchToNewCheckBox.Location = new System.Drawing.Point(157, 97);
            this.mSwitchToNewCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.mSwitchToNewCheckBox.Name = "mSwitchToNewCheckBox";
            this.mSwitchToNewCheckBox.Size = new System.Drawing.Size(136, 20);
            this.mSwitchToNewCheckBox.TabIndex = 5;
            this.mSwitchToNewCheckBox.Text = "S&witch to new copy";
            this.mSwitchToNewCheckBox.UseVisualStyleBackColor = true;
            // 
            // m_ProjectNameTextBox
            // 
            this.m_ProjectNameTextBox.Location = new System.Drawing.Point(130, 57);
            this.m_ProjectNameTextBox.Name = "m_ProjectNameTextBox";
            this.m_ProjectNameTextBox.Size = new System.Drawing.Size(137, 22);
            this.m_ProjectNameTextBox.TabIndex = 4;
            this.m_ProjectNameTextBox.Text = "project.obi";
            // 
            // m_lbl_ProjectName
            // 
            this.m_lbl_ProjectName.AutoSize = true;
            this.m_lbl_ProjectName.Location = new System.Drawing.Point(12, 60);
            this.m_lbl_ProjectName.Name = "m_lbl_ProjectName";
            this.m_lbl_ProjectName.Size = new System.Drawing.Size(110, 16);
            this.m_lbl_ProjectName.TabIndex = 3;
            this.m_lbl_ProjectName.Text = "Pro&ject file name:";
            // 
            // SaveProjectAsDialog
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(594, 187);
            this.ControlBox = false;
            this.Controls.Add(this.m_lbl_ProjectName);
            this.Controls.Add(this.m_ProjectNameTextBox);
            this.Controls.Add(this.mSwitchToNewCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mLocationTextBox);
            this.Controls.Add(this.m_lblParentDirectoryPath);
            this.Controls.Add(this.mSelectButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SaveProjectAsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Save project as";
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
    }
}