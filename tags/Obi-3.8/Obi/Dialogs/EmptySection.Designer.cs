namespace Obi.Dialogs
{
    partial class EmptySection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmptySection));
            this.mContinueButton = new System.Windows.Forms.Button();
            this.mMessageLabel = new System.Windows.Forms.Label();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mKeepWarningCheckbox = new System.Windows.Forms.CheckBox();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // mContinueButton
            // 
            this.mContinueButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.mContinueButton, "mContinueButton");
            this.mContinueButton.Name = "mContinueButton";
            this.mContinueButton.UseVisualStyleBackColor = true;
            // 
            // mMessageLabel
            // 
            resources.ApplyResources(this.mMessageLabel, "mMessageLabel");
            this.mMessageLabel.Name = "mMessageLabel";
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mKeepWarningCheckbox
            // 
            resources.ApplyResources(this.mKeepWarningCheckbox, "mKeepWarningCheckbox");
            this.mKeepWarningCheckbox.Checked = true;
            this.mKeepWarningCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mKeepWarningCheckbox.Name = "mKeepWarningCheckbox";
            this.mKeepWarningCheckbox.UseVisualStyleBackColor = true;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // EmptySection
            // 
            this.AcceptButton = this.mContinueButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ControlBox = false;
            this.Controls.Add(this.mKeepWarningCheckbox);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mMessageLabel);
            this.Controls.Add(this.mContinueButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmptySection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mContinueButton;
        private System.Windows.Forms.Label mMessageLabel;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mKeepWarningCheckbox;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}