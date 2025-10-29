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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddAzureVoice));
            m_AddAzureVoiceListBox = new System.Windows.Forms.ListBox();
            m_AddVoiceBtn = new System.Windows.Forms.Button();
            m_GenderCb = new System.Windows.Forms.ComboBox();
            m_GenderLbl = new System.Windows.Forms.Label();
            m_LanguageLbl = new System.Windows.Forms.Label();
            m_LanguageCb = new System.Windows.Forms.ComboBox();
            m_DialectLbl = new System.Windows.Forms.Label();
            m_DialectCb = new System.Windows.Forms.ComboBox();
            m_VoiceLbl = new System.Windows.Forms.Label();
            m_VoiceCb = new System.Windows.Forms.ComboBox();
            m_CancelBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // m_AddAzureVoiceListBox
            // 
            resources.ApplyResources(m_AddAzureVoiceListBox, "m_AddAzureVoiceListBox");
            m_AddAzureVoiceListBox.FormattingEnabled = true;
            m_AddAzureVoiceListBox.Name = "m_AddAzureVoiceListBox";
            // 
            // m_AddVoiceBtn
            // 
            resources.ApplyResources(m_AddVoiceBtn, "m_AddVoiceBtn");
            m_AddVoiceBtn.Name = "m_AddVoiceBtn";
            m_AddVoiceBtn.UseVisualStyleBackColor = true;
            m_AddVoiceBtn.Click += m_AddVoiceBtn_Click;
            // 
            // m_GenderCb
            // 
            resources.ApplyResources(m_GenderCb, "m_GenderCb");
            m_GenderCb.FormattingEnabled = true;
            m_GenderCb.Items.AddRange(new object[] { resources.GetString("m_GenderCb.Items"), resources.GetString("m_GenderCb.Items1"), resources.GetString("m_GenderCb.Items2") });
            m_GenderCb.Name = "m_GenderCb";
            m_GenderCb.SelectedIndexChanged += m_GenderCb_SelectedIndexChanged;
            // 
            // m_GenderLbl
            // 
            resources.ApplyResources(m_GenderLbl, "m_GenderLbl");
            m_GenderLbl.Name = "m_GenderLbl";
            // 
            // m_LanguageLbl
            // 
            resources.ApplyResources(m_LanguageLbl, "m_LanguageLbl");
            m_LanguageLbl.Name = "m_LanguageLbl";
            // 
            // m_LanguageCb
            // 
            resources.ApplyResources(m_LanguageCb, "m_LanguageCb");
            m_LanguageCb.FormattingEnabled = true;
            m_LanguageCb.Name = "m_LanguageCb";
            m_LanguageCb.SelectedIndexChanged += m_LanguageCb_SelectedIndexChanged;
            // 
            // m_DialectLbl
            // 
            resources.ApplyResources(m_DialectLbl, "m_DialectLbl");
            m_DialectLbl.Name = "m_DialectLbl";
            // 
            // m_DialectCb
            // 
            resources.ApplyResources(m_DialectCb, "m_DialectCb");
            m_DialectCb.FormattingEnabled = true;
            m_DialectCb.Name = "m_DialectCb";
            m_DialectCb.SelectedIndexChanged += m_DialectCb_SelectedIndexChanged;
            // 
            // m_VoiceLbl
            // 
            resources.ApplyResources(m_VoiceLbl, "m_VoiceLbl");
            m_VoiceLbl.Name = "m_VoiceLbl";
            // 
            // m_VoiceCb
            // 
            resources.ApplyResources(m_VoiceCb, "m_VoiceCb");
            m_VoiceCb.FormattingEnabled = true;
            m_VoiceCb.Name = "m_VoiceCb";
            m_VoiceCb.SelectedIndexChanged += m_VoiceCb_SelectedIndexChanged;
            // 
            // m_CancelBtn
            // 
            m_CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(m_CancelBtn, "m_CancelBtn");
            m_CancelBtn.Name = "m_CancelBtn";
            m_CancelBtn.UseVisualStyleBackColor = true;
            // 
            // AddAzureVoice
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(m_CancelBtn);
            Controls.Add(m_VoiceCb);
            Controls.Add(m_VoiceLbl);
            Controls.Add(m_DialectCb);
            Controls.Add(m_DialectLbl);
            Controls.Add(m_LanguageCb);
            Controls.Add(m_LanguageLbl);
            Controls.Add(m_GenderLbl);
            Controls.Add(m_GenderCb);
            Controls.Add(m_AddVoiceBtn);
            Controls.Add(m_AddAzureVoiceListBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddAzureVoice";
            ResumeLayout(false);
            PerformLayout();

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