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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportAdvance));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.m_ChkResample = new System.Windows.Forms.CheckBox();
            this.m_ComboBoxStereoMode = new System.Windows.Forms.ComboBox();
            this.mLblStereoMode = new System.Windows.Forms.Label();
            this.m_LblReplayGain = new System.Windows.Forms.Label();
            this.m_comboBoxReplayGain = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // m_ChkResample
            // 
            resources.ApplyResources(this.m_ChkResample, "m_ChkResample");
            this.m_ChkResample.Checked = true;
            this.m_ChkResample.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_ChkResample.Name = "m_ChkResample";
            this.m_ChkResample.UseVisualStyleBackColor = true;
            // 
            // m_ComboBoxStereoMode
            // 
            this.m_ComboBoxStereoMode.FormattingEnabled = true;
            this.m_ComboBoxStereoMode.Items.AddRange(new object[] {
            resources.GetString("m_ComboBoxStereoMode.Items"),
            resources.GetString("m_ComboBoxStereoMode.Items1"),
            resources.GetString("m_ComboBoxStereoMode.Items2"),
            resources.GetString("m_ComboBoxStereoMode.Items3"),
            resources.GetString("m_ComboBoxStereoMode.Items4")});
            resources.ApplyResources(this.m_ComboBoxStereoMode, "m_ComboBoxStereoMode");
            this.m_ComboBoxStereoMode.Name = "m_ComboBoxStereoMode";
            // 
            // mLblStereoMode
            // 
            resources.ApplyResources(this.mLblStereoMode, "mLblStereoMode");
            this.mLblStereoMode.Name = "mLblStereoMode";
            // 
            // m_LblReplayGain
            // 
            resources.ApplyResources(this.m_LblReplayGain, "m_LblReplayGain");
            this.m_LblReplayGain.Name = "m_LblReplayGain";
            // 
            // m_comboBoxReplayGain
            // 
            resources.ApplyResources(this.m_comboBoxReplayGain, "m_comboBoxReplayGain");
            this.m_comboBoxReplayGain.FormattingEnabled = true;
            this.m_comboBoxReplayGain.Items.AddRange(new object[] {
            resources.GetString("m_comboBoxReplayGain.Items"),
            resources.GetString("m_comboBoxReplayGain.Items1"),
            resources.GetString("m_comboBoxReplayGain.Items2"),
            resources.GetString("m_comboBoxReplayGain.Items3")});
            this.m_comboBoxReplayGain.Name = "m_comboBoxReplayGain";
            // 
            // ExportAdvance
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.m_comboBoxReplayGain);
            this.Controls.Add(this.m_LblReplayGain);
            this.Controls.Add(this.mLblStereoMode);
            this.Controls.Add(this.m_ComboBoxStereoMode);
            this.Controls.Add(this.m_ChkResample);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Name = "ExportAdvance";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox m_ChkResample;
        private System.Windows.Forms.ComboBox m_ComboBoxStereoMode;
        private System.Windows.Forms.Label mLblStereoMode;
        private System.Windows.Forms.Label m_LblReplayGain;
        private System.Windows.Forms.ComboBox m_comboBoxReplayGain;
    }
}