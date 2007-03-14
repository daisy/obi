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
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(124, 257);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 25);
            this.mOKButton.TabIndex = 0;
            this.mOKButton.Text = "&OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Location = new System.Drawing.Point(205, 258);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 25);
            this.mCancelButton.TabIndex = 1;
            this.mCancelButton.Text = "&Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Silence threshold:";
            // 
            // mThresholdBox
            // 
            this.mThresholdBox.AccessibleName = "Silence threshold:";
            this.mThresholdBox.Location = new System.Drawing.Point(147, 7);
            this.mThresholdBox.Name = "mThresholdBox";
            this.mThresholdBox.Size = new System.Drawing.Size(100, 20);
            this.mThresholdBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "&Gap between sentences:";
            // 
            // mGapBox
            // 
            this.mGapBox.AccessibleName = "Gap between sentences:";
            this.mGapBox.Location = new System.Drawing.Point(147, 34);
            this.mGapBox.Name = "mGapBox";
            this.mGapBox.Size = new System.Drawing.Size(100, 20);
            this.mGapBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "&Leading silence:";
            // 
            // mLeadingSilenceBox
            // 
            this.mLeadingSilenceBox.AccessibleName = "Leading silence:";
            this.mLeadingSilenceBox.Location = new System.Drawing.Point(147, 61);
            this.mLeadingSilenceBox.Name = "mLeadingSilenceBox";
            this.mLeadingSilenceBox.Size = new System.Drawing.Size(100, 20);
            this.mLeadingSilenceBox.TabIndex = 7;
            // 
            // SentenceDetection
            // 
            this.AcceptButton = this.mOKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.ClientSize = new System.Drawing.Size(292, 296);
            this.Controls.Add(this.mLeadingSilenceBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mGapBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mThresholdBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SentenceDetection";
            this.Text = "Apply phrase detection";
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