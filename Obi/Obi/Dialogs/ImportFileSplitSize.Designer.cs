namespace Obi.Dialogs
{
    partial class ImportFileSplitSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFileSplitSize));
            this.label1 = new System.Windows.Forms.Label();
            this.mPhraseSizeTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mSplitCheckBox = new System.Windows.Forms.CheckBox();
            this.mCreateAudioFilePerSectionCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mPhraseSizeTextBox
            // 
            resources.ApplyResources(this.mPhraseSizeTextBox, "mPhraseSizeTextBox");
            this.mPhraseSizeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPhraseSizeTextBox.Name = "mPhraseSizeTextBox";
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mSplitCheckBox
            // 
            resources.ApplyResources(this.mSplitCheckBox, "mSplitCheckBox");
            this.mSplitCheckBox.Name = "mSplitCheckBox";
            this.mSplitCheckBox.UseVisualStyleBackColor = true;
            this.mSplitCheckBox.CheckedChanged += new System.EventHandler(this.mSplitCheckBox_CheckedChanged);
            // 
            // mCreateAudioFilePerSectionCheckBox
            // 
            resources.ApplyResources(this.mCreateAudioFilePerSectionCheckBox, "mCreateAudioFilePerSectionCheckBox");
            this.mCreateAudioFilePerSectionCheckBox.Name = "mCreateAudioFilePerSectionCheckBox";
            this.mCreateAudioFilePerSectionCheckBox.UseVisualStyleBackColor = true;
            // 
            // ImportFileSplitSize
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mCreateAudioFilePerSectionCheckBox);
            this.Controls.Add(this.mSplitCheckBox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mPhraseSizeTextBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFileSplitSize";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportFileSplitSize_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mPhraseSizeTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mSplitCheckBox;
        private System.Windows.Forms.CheckBox mCreateAudioFilePerSectionCheckBox;
    }
}