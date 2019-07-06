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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mThresholdNumericBox = new System.Windows.Forms.NumericUpDown();
            this.mGapNumericBox = new System.Windows.Forms.NumericUpDown();
            this.mLeadingNumericBox = new System.Windows.Forms.NumericUpDown();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            ((System.ComponentModel.ISupportInitialize)(this.mThresholdNumericBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mGapNumericBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mLeadingNumericBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // mThresholdNumericBox
            // 
            resources.ApplyResources(this.mThresholdNumericBox, "mThresholdNumericBox");
            this.mThresholdNumericBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mThresholdNumericBox.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.mThresholdNumericBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.mThresholdNumericBox.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.mThresholdNumericBox.Name = "mThresholdNumericBox";
            // 
            // mGapNumericBox
            // 
            resources.ApplyResources(this.mGapNumericBox, "mGapNumericBox");
            this.mGapNumericBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mGapNumericBox.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.mGapNumericBox.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.mGapNumericBox.Name = "mGapNumericBox";
            this.mGapNumericBox.ValueChanged += new System.EventHandler(this.mGapNumericBox_ValueChanged);
            // 
            // mLeadingNumericBox
            // 
            resources.ApplyResources(this.mLeadingNumericBox, "mLeadingNumericBox");
            this.mLeadingNumericBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mLeadingNumericBox.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.mLeadingNumericBox.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mLeadingNumericBox.Name = "mLeadingNumericBox";
            this.mLeadingNumericBox.ValueChanged += new System.EventHandler(this.mLeadingNumericBox_ValueChanged);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SentenceDetection
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mLeadingNumericBox);
            this.Controls.Add(this.mGapNumericBox);
            this.Controls.Add(this.mThresholdNumericBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SentenceDetection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.mThresholdNumericBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mGapNumericBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mLeadingNumericBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown mThresholdNumericBox;
        private System.Windows.Forms.NumericUpDown mGapNumericBox;
        private System.Windows.Forms.NumericUpDown mLeadingNumericBox;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}