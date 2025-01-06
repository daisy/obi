namespace Obi.Dialogs
{
    partial class AddAzureVoice
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
            this.m_AddAzureVoiceListBox = new System.Windows.Forms.ListBox();
            this.m_AddVoiceBtn = new System.Windows.Forms.Button();
            this.m_GenderCb = new System.Windows.Forms.ComboBox();
            this.m_GenderLbl = new System.Windows.Forms.Label();
            this.m_LanguageLbl = new System.Windows.Forms.Label();
            this.m_LanguageCb = new System.Windows.Forms.ComboBox();
            this.m_DialectLbl = new System.Windows.Forms.Label();
            this.m_DialectCb = new System.Windows.Forms.ComboBox();
            this.m_VoiceLbl = new System.Windows.Forms.Label();
            this.m_VoiceCb = new System.Windows.Forms.ComboBox();
            this.m_CancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_AddAzureVoiceListBox
            // 
            this.m_AddAzureVoiceListBox.AccessibleName = "Voices available in Azure";
            this.m_AddAzureVoiceListBox.FormattingEnabled = true;
            this.m_AddAzureVoiceListBox.ItemHeight = 20;
            this.m_AddAzureVoiceListBox.Location = new System.Drawing.Point(133, 49);
            this.m_AddAzureVoiceListBox.Name = "m_AddAzureVoiceListBox";
            this.m_AddAzureVoiceListBox.Size = new System.Drawing.Size(457, 504);
            this.m_AddAzureVoiceListBox.TabIndex = 0;
            // 
            // m_AddVoiceBtn
            // 
            this.m_AddVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddVoiceBtn.Location = new System.Drawing.Point(302, 599);
            this.m_AddVoiceBtn.Name = "m_AddVoiceBtn";
            this.m_AddVoiceBtn.Size = new System.Drawing.Size(94, 29);
            this.m_AddVoiceBtn.TabIndex = 1;
            this.m_AddVoiceBtn.Text = "&Add Voice";
            this.m_AddVoiceBtn.UseVisualStyleBackColor = true;
            this.m_AddVoiceBtn.Click += new System.EventHandler(this.m_AddVoiceBtn_Click);
            // 
            // m_GenderCb
            // 
            this.m_GenderCb.AccessibleName = "Select Gender";
            this.m_GenderCb.FormattingEnabled = true;
            this.m_GenderCb.Items.AddRange(new object[] {
            "All",
            "Male",
            "Female"});
            this.m_GenderCb.Location = new System.Drawing.Point(643, 102);
            this.m_GenderCb.Name = "m_GenderCb";
            this.m_GenderCb.Size = new System.Drawing.Size(175, 28);
            this.m_GenderCb.TabIndex = 3;
            this.m_GenderCb.SelectedIndexChanged += new System.EventHandler(this.m_GenderCb_SelectedIndexChanged);
            // 
            // m_GenderLbl
            // 
            this.m_GenderLbl.AutoSize = true;
            this.m_GenderLbl.Location = new System.Drawing.Point(643, 55);
            this.m_GenderLbl.Name = "m_GenderLbl";
            this.m_GenderLbl.Size = new System.Drawing.Size(57, 20);
            this.m_GenderLbl.TabIndex = 2;
            this.m_GenderLbl.Text = "Gender";
            // 
            // m_LanguageLbl
            // 
            this.m_LanguageLbl.AutoSize = true;
            this.m_LanguageLbl.Location = new System.Drawing.Point(643, 184);
            this.m_LanguageLbl.Name = "m_LanguageLbl";
            this.m_LanguageLbl.Size = new System.Drawing.Size(74, 20);
            this.m_LanguageLbl.TabIndex = 4;
            this.m_LanguageLbl.Text = "Language";
            // 
            // m_LanguageCb
            // 
            this.m_LanguageCb.AccessibleName = "Language";
            this.m_LanguageCb.FormattingEnabled = true;
            this.m_LanguageCb.Location = new System.Drawing.Point(643, 239);
            this.m_LanguageCb.Name = "m_LanguageCb";
            this.m_LanguageCb.Size = new System.Drawing.Size(175, 28);
            this.m_LanguageCb.TabIndex = 5;
            this.m_LanguageCb.SelectedIndexChanged += new System.EventHandler(this.m_LanguageCb_SelectedIndexChanged);
            // 
            // m_DialectLbl
            // 
            this.m_DialectLbl.AutoSize = true;
            this.m_DialectLbl.Location = new System.Drawing.Point(643, 339);
            this.m_DialectLbl.Name = "m_DialectLbl";
            this.m_DialectLbl.Size = new System.Drawing.Size(56, 20);
            this.m_DialectLbl.TabIndex = 6;
            this.m_DialectLbl.Text = "Dialect";
            // 
            // m_DialectCb
            // 
            this.m_DialectCb.AccessibleName = "Dialect";
            this.m_DialectCb.FormattingEnabled = true;
            this.m_DialectCb.Location = new System.Drawing.Point(643, 394);
            this.m_DialectCb.Name = "m_DialectCb";
            this.m_DialectCb.Size = new System.Drawing.Size(175, 28);
            this.m_DialectCb.TabIndex = 7;
            this.m_DialectCb.SelectedIndexChanged += new System.EventHandler(this.m_DialectCb_SelectedIndexChanged);
            // 
            // m_VoiceLbl
            // 
            this.m_VoiceLbl.AutoSize = true;
            this.m_VoiceLbl.Location = new System.Drawing.Point(643, 473);
            this.m_VoiceLbl.Name = "m_VoiceLbl";
            this.m_VoiceLbl.Size = new System.Drawing.Size(45, 20);
            this.m_VoiceLbl.TabIndex = 8;
            this.m_VoiceLbl.Text = "Voice";
            // 
            // m_VoiceCb
            // 
            this.m_VoiceCb.AccessibleName = "Voice";
            this.m_VoiceCb.FormattingEnabled = true;
            this.m_VoiceCb.Location = new System.Drawing.Point(643, 525);
            this.m_VoiceCb.Name = "m_VoiceCb";
            this.m_VoiceCb.Size = new System.Drawing.Size(175, 28);
            this.m_VoiceCb.TabIndex = 9;
            this.m_VoiceCb.SelectedIndexChanged += new System.EventHandler(this.m_VoiceCb_SelectedIndexChanged);
            // 
            // m_CancelBtn
            // 
            this.m_CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_CancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_CancelBtn.Location = new System.Drawing.Point(724, 599);
            this.m_CancelBtn.Name = "m_CancelBtn";
            this.m_CancelBtn.Size = new System.Drawing.Size(94, 29);
            this.m_CancelBtn.TabIndex = 10;
            this.m_CancelBtn.Text = "&Cancel";
            this.m_CancelBtn.UseVisualStyleBackColor = true;
            // 
            // AddAzureVoice
            // 
            this.AccessibleName = "Add Azure voice";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 659);
            this.Controls.Add(this.m_CancelBtn);
            this.Controls.Add(this.m_VoiceCb);
            this.Controls.Add(this.m_VoiceLbl);
            this.Controls.Add(this.m_DialectCb);
            this.Controls.Add(this.m_DialectLbl);
            this.Controls.Add(this.m_LanguageCb);
            this.Controls.Add(this.m_LanguageLbl);
            this.Controls.Add(this.m_GenderLbl);
            this.Controls.Add(this.m_GenderCb);
            this.Controls.Add(this.m_AddVoiceBtn);
            this.Controls.Add(this.m_AddAzureVoiceListBox);
            this.Name = "AddAzureVoice";
            this.Text = "Add Azure Voice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox m_AddAzureVoiceListBox;
        private System.Windows.Forms.Button m_AddVoiceBtn;
        private System.Windows.Forms.ComboBox m_GenderCb;
        private System.Windows.Forms.Label m_GenderLbl;
        private System.Windows.Forms.Label m_LanguageLbl;
        private System.Windows.Forms.ComboBox m_LanguageCb;
        private System.Windows.Forms.Label m_DialectLbl;
        private System.Windows.Forms.ComboBox m_DialectCb;
        private System.Windows.Forms.Label m_VoiceLbl;
        private System.Windows.Forms.ComboBox m_VoiceCb;
        private System.Windows.Forms.Button m_CancelBtn;
    }
}