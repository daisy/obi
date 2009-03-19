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
        this.m_lblProjectDirectoryName = new System.Windows.Forms.Label ();
        this.mNewDirectoryTextBox = new System.Windows.Forms.TextBox ();
        this.mSelectButton = new System.Windows.Forms.Button ();
        this.m_lblParentDirectoryPath = new System.Windows.Forms.Label ();
        this.mLocationTextBox = new System.Windows.Forms.TextBox ();
        this.mOKButton = new System.Windows.Forms.Button ();
        this.mCancelButton = new System.Windows.Forms.Button ();
        this.mSwitchToNewCheckBox = new System.Windows.Forms.CheckBox ();
        this.SuspendLayout ();
        // 
        // m_lblProjectDirectoryName
        // 
        this.m_lblProjectDirectoryName.AutoSize = true;
        this.m_lblProjectDirectoryName.Location = new System.Drawing.Point ( 13, 21 );
        this.m_lblProjectDirectoryName.Margin = new System.Windows.Forms.Padding ( 4, 0, 4, 0 );
        this.m_lblProjectDirectoryName.Name = "m_lblProjectDirectoryName";
        this.m_lblProjectDirectoryName.Size = new System.Drawing.Size ( 137, 16 );
        this.m_lblProjectDirectoryName.TabIndex = 0;
        this.m_lblProjectDirectoryName.Text = "&New project directory:";
        // 
        // mNewDirectoryTextBox
        // 
        this.mNewDirectoryTextBox.AccessibleName = "New project directory";
        this.mNewDirectoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.mNewDirectoryTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mNewDirectoryTextBox.Location = new System.Drawing.Point ( 158, 19 );
        this.mNewDirectoryTextBox.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mNewDirectoryTextBox.Name = "mNewDirectoryTextBox";
        this.mNewDirectoryTextBox.Size = new System.Drawing.Size ( 320, 22 );
        this.mNewDirectoryTextBox.TabIndex = 1;
        this.mNewDirectoryTextBox.TextChanged += new System.EventHandler ( this.mNewDirectoryTextBox_TextChanged );
        // 
        // mSelectButton
        // 
        this.mSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.mSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.mSelectButton.Location = new System.Drawing.Point ( 493, 48 );
        this.mSelectButton.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mSelectButton.Name = "mSelectButton";
        this.mSelectButton.Size = new System.Drawing.Size ( 100, 28 );
        this.mSelectButton.TabIndex = 4;
        this.mSelectButton.Text = "&Select";
        this.mSelectButton.UseVisualStyleBackColor = true;
        this.mSelectButton.Click += new System.EventHandler ( this.mSelectButton_Click );
        // 
        // m_lblParentDirectoryPath
        // 
        this.m_lblParentDirectoryPath.AutoSize = true;
        this.m_lblParentDirectoryPath.Location = new System.Drawing.Point ( 88, 54 );
        this.m_lblParentDirectoryPath.Margin = new System.Windows.Forms.Padding ( 4, 0, 4, 0 );
        this.m_lblParentDirectoryPath.Name = "m_lblParentDirectoryPath";
        this.m_lblParentDirectoryPath.Size = new System.Drawing.Size ( 62, 16 );
        this.m_lblParentDirectoryPath.TabIndex = 2;
        this.m_lblParentDirectoryPath.Text = "&Location:";
        // 
        // mLocationTextBox
        // 
        this.mLocationTextBox.AccessibleName = "Location";
        this.mLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.mLocationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.mLocationTextBox.Location = new System.Drawing.Point ( 158, 52 );
        this.mLocationTextBox.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mLocationTextBox.Name = "mLocationTextBox";
        this.mLocationTextBox.Size = new System.Drawing.Size ( 320, 22 );
        this.mLocationTextBox.TabIndex = 3;
        // 
        // mOKButton
        // 
        this.mOKButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.mOKButton.Location = new System.Drawing.Point ( 195, 146 );
        this.mOKButton.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mOKButton.Name = "mOKButton";
        this.mOKButton.Size = new System.Drawing.Size ( 100, 28 );
        this.mOKButton.TabIndex = 6;
        this.mOKButton.Text = "&OK";
        this.mOKButton.UseVisualStyleBackColor = true;
        this.mOKButton.Click += new System.EventHandler ( this.mOKButton_Click );
        // 
        // mCancelButton
        // 
        this.mCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.mCancelButton.Location = new System.Drawing.Point ( 303, 146 );
        this.mCancelButton.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mCancelButton.Name = "mCancelButton";
        this.mCancelButton.Size = new System.Drawing.Size ( 100, 28 );
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
        this.mSwitchToNewCheckBox.Location = new System.Drawing.Point ( 158, 82 );
        this.mSwitchToNewCheckBox.Margin = new System.Windows.Forms.Padding ( 4 );
        this.mSwitchToNewCheckBox.Name = "mSwitchToNewCheckBox";
        this.mSwitchToNewCheckBox.Size = new System.Drawing.Size ( 136, 20 );
        this.mSwitchToNewCheckBox.TabIndex = 5;
        this.mSwitchToNewCheckBox.Text = "S&witch to new copy";
        this.mSwitchToNewCheckBox.UseVisualStyleBackColor = true;
        // 
        // SaveProjectAsDialog
        // 
        this.AcceptButton = this.mOKButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 8F, 16F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.mCancelButton;
        this.ClientSize = new System.Drawing.Size ( 606, 187 );
        this.ControlBox = false;
        this.Controls.Add ( this.mSwitchToNewCheckBox );
        this.Controls.Add ( this.mCancelButton );
        this.Controls.Add ( this.mOKButton );
        this.Controls.Add ( this.mLocationTextBox );
        this.Controls.Add ( this.m_lblParentDirectoryPath );
        this.Controls.Add ( this.mSelectButton );
        this.Controls.Add ( this.mNewDirectoryTextBox );
        this.Controls.Add ( this.m_lblProjectDirectoryName );
        this.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
        this.Margin = new System.Windows.Forms.Padding ( 4 );
        this.Name = "SaveProjectAsDialog";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.Text = "Save project as";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler ( this.SaveProjectAsDialog_FormClosing );
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Label m_lblProjectDirectoryName;
        private System.Windows.Forms.TextBox mNewDirectoryTextBox;
        private System.Windows.Forms.Button mSelectButton;
        private System.Windows.Forms.Label m_lblParentDirectoryPath;
        private System.Windows.Forms.TextBox mLocationTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mSwitchToNewCheckBox;
    }
}