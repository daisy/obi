namespace Obi.Dialogs
{
    partial class ExportAdvance
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.mChkReplayGain = new System.Windows.Forms.CheckBox();
            this.mChkResample = new System.Windows.Forms.CheckBox();
            this.mComboBoxStereoMode = new System.Windows.Forms.ComboBox();
            this.mLblStereoMode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.AccessibleName = "Ok";
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Location = new System.Drawing.Point(28, 225);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleName = "Cancel";
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(113, 225);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // mChkReplayGain
            // 
            this.mChkReplayGain.AccessibleName = "ReplayGain";
            this.mChkReplayGain.AutoSize = true;
            this.mChkReplayGain.Location = new System.Drawing.Point(38, 25);
            this.mChkReplayGain.Name = "mChkReplayGain";
            this.mChkReplayGain.Size = new System.Drawing.Size(81, 17);
            this.mChkReplayGain.TabIndex = 8;
            this.mChkReplayGain.Text = "ReplayGain";
            this.mChkReplayGain.UseVisualStyleBackColor = true;
            // 
            // mChkResample
            // 
            this.mChkResample.AccessibleName = "Resample";
            this.mChkResample.AutoSize = true;
            this.mChkResample.Location = new System.Drawing.Point(38, 60);
            this.mChkResample.Name = "mChkResample";
            this.mChkResample.Size = new System.Drawing.Size(73, 17);
            this.mChkResample.TabIndex = 9;
            this.mChkResample.Text = "Resample";
            this.mChkResample.UseVisualStyleBackColor = true;
            // 
            // mComboBoxStereoMode
            // 
            this.mComboBoxStereoMode.FormattingEnabled = true;
            this.mComboBoxStereoMode.Items.AddRange(new object[] {
            "s",
            "j",
            "f",
            "m"});
            this.mComboBoxStereoMode.Location = new System.Drawing.Point(113, 97);
            this.mComboBoxStereoMode.Name = "mComboBoxStereoMode";
            this.mComboBoxStereoMode.Size = new System.Drawing.Size(61, 21);
            this.mComboBoxStereoMode.TabIndex = 10;
            // 
            // mLblStereoMode
            // 
            this.mLblStereoMode.AccessibleName = "Stereo Mode";
            this.mLblStereoMode.AutoSize = true;
            this.mLblStereoMode.Location = new System.Drawing.Point(35, 100);
            this.mLblStereoMode.Name = "mLblStereoMode";
            this.mLblStereoMode.Size = new System.Drawing.Size(68, 13);
            this.mLblStereoMode.TabIndex = 11;
            this.mLblStereoMode.Text = "Stereo Mode";
            // 
            // ExportAdvance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 260);
            this.Controls.Add(this.mLblStereoMode);
            this.Controls.Add(this.mComboBoxStereoMode);
            this.Controls.Add(this.mChkResample);
            this.Controls.Add(this.mChkReplayGain);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "ExportAdvance";
            this.Text = "Export Advance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox mChkReplayGain;
        private System.Windows.Forms.CheckBox mChkResample;
        private System.Windows.Forms.ComboBox mComboBoxStereoMode;
        private System.Windows.Forms.Label mLblStereoMode;
    }
}