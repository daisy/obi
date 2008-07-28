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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // mPhraseSizeTextBox
            // 
            this.mPhraseSizeTextBox.AccessibleDescription = null;
            resources.ApplyResources(this.mPhraseSizeTextBox, "mPhraseSizeTextBox");
            this.mPhraseSizeTextBox.BackgroundImage = null;
            this.mPhraseSizeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPhraseSizeTextBox.Font = null;
            this.mPhraseSizeTextBox.Name = "mPhraseSizeTextBox";
            this.mPhraseSizeTextBox.TextChanged += new System.EventHandler(this.mPhraseSizeTextBox_TextChanged);
            // 
            // mOKButton
            // 
            this.mOKButton.AccessibleDescription = null;
            this.mOKButton.AccessibleName = null;
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.BackgroundImage = null;
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Font = null;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.AccessibleDescription = null;
            this.mCancelButton.AccessibleName = null;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.BackgroundImage = null;
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Font = null;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // ImportFileSplitSize
            // 
            this.AcceptButton = this.mOKButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mPhraseSizeTextBox);
            this.Controls.Add(this.label1);
            this.Icon = null;
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
    }
}