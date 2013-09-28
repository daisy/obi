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
            this.m_ChkReplayGain = new System.Windows.Forms.CheckBox();
            this.m_ChkResample = new System.Windows.Forms.CheckBox();
            this.m_ComboBoxStereoMode = new System.Windows.Forms.ComboBox();
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
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
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
            // m_ChkReplayGain
            // 
            this.m_ChkReplayGain.AccessibleName = "ReplayGain";
            this.m_ChkReplayGain.AutoSize = true;
            this.m_ChkReplayGain.Location = new System.Drawing.Point(38, 25);
            this.m_ChkReplayGain.Name = "m_ChkReplayGain";
            this.m_ChkReplayGain.Size = new System.Drawing.Size(81, 17);
            this.m_ChkReplayGain.TabIndex = 8;
            this.m_ChkReplayGain.Text = "ReplayGain";
            this.m_ChkReplayGain.UseVisualStyleBackColor = true;
            // 
            // m_ChkResample
            // 
            this.m_ChkResample.AccessibleName = "Resample";
            this.m_ChkResample.AutoSize = true;
            this.m_ChkResample.Checked = true;
            this.m_ChkResample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_ChkResample.Location = new System.Drawing.Point(38, 60);
            this.m_ChkResample.Name = "m_ChkResample";
            this.m_ChkResample.Size = new System.Drawing.Size(73, 17);
            this.m_ChkResample.TabIndex = 9;
            this.m_ChkResample.Text = "Resample";
            this.m_ChkResample.UseVisualStyleBackColor = true;
            // 
            // m_ComboBoxStereoMode
            // 
            this.m_ComboBoxStereoMode.FormattingEnabled = true;
            this.m_ComboBoxStereoMode.Items.AddRange(new object[] {
            "s",
            "j",
            "f",
            "m"});
            this.m_ComboBoxStereoMode.Location = new System.Drawing.Point(113, 97);
            this.m_ComboBoxStereoMode.Name = "m_ComboBoxStereoMode";
            this.m_ComboBoxStereoMode.Size = new System.Drawing.Size(61, 21);
            this.m_ComboBoxStereoMode.TabIndex = 10;
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
            this.Controls.Add(this.m_ComboBoxStereoMode);
            this.Controls.Add(this.m_ChkResample);
            this.Controls.Add(this.m_ChkReplayGain);
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
        private System.Windows.Forms.CheckBox m_ChkReplayGain;
        private System.Windows.Forms.CheckBox m_ChkResample;
        private System.Windows.Forms.ComboBox m_ComboBoxStereoMode;
        private System.Windows.Forms.Label mLblStereoMode;
    }
}