namespace Obi.Dialogs
{
    partial class SentenceDetection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SentenceDetection));
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mThresholdBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mGapBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mLeadingSilenceBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // mThresholdBox
            // 
            this.mThresholdBox.AccessibleDescription = null;
            resources.ApplyResources(this.mThresholdBox, "mThresholdBox");
            this.mThresholdBox.BackgroundImage = null;
            this.mThresholdBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mThresholdBox.Font = null;
            this.mThresholdBox.Name = "mThresholdBox";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // mGapBox
            // 
            this.mGapBox.AccessibleDescription = null;
            resources.ApplyResources(this.mGapBox, "mGapBox");
            this.mGapBox.BackgroundImage = null;
            this.mGapBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mGapBox.Font = null;
            this.mGapBox.Name = "mGapBox";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // mLeadingSilenceBox
            // 
            this.mLeadingSilenceBox.AccessibleDescription = null;
            resources.ApplyResources(this.mLeadingSilenceBox, "mLeadingSilenceBox");
            this.mLeadingSilenceBox.BackgroundImage = null;
            this.mLeadingSilenceBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLeadingSilenceBox.Font = null;
            this.mLeadingSilenceBox.Name = "mLeadingSilenceBox";
            // 
            // SentenceDetection
            // 
            this.AcceptButton = this.mOKButton;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mLeadingSilenceBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mGapBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mThresholdBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SentenceDetection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mThresholdBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mGapBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mLeadingSilenceBox;
    }
}