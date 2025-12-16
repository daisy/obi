namespace Obi.Dialogs
{
    partial class GenerateSpeech
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateSpeech));
            m_TextToSpeechTb = new System.Windows.Forms.RichTextBox();
            m_PreviewBtn = new System.Windows.Forms.Button();
            m_GenerateBtn = new System.Windows.Forms.Button();
            m_BuildInRbtn = new System.Windows.Forms.RadioButton();
            m_AzureRbtn = new System.Windows.Forms.RadioButton();
            m_VoiceSelectionCb = new System.Windows.Forms.ComboBox();
            m_ClearBtn = new System.Windows.Forms.Button();
            m_SpeedTb = new System.Windows.Forms.TrackBar();
            m_TTSVoiceLbl = new System.Windows.Forms.Label();
            m_SpeechRateLbl = new System.Windows.Forms.Label();
            m_FontSmallerBtn = new System.Windows.Forms.Button();
            m_FontBiggerBtn = new System.Windows.Forms.Button();
            m_AddAzureKeyBtn = new System.Windows.Forms.Button();
            m_AddAzureVoiceBtn = new System.Windows.Forms.Button();
            m_DeleteAzureVoiceBtn = new System.Windows.Forms.Button();
            m_CloseBtn = new System.Windows.Forms.Button();
            m_VoiceTypeGroupBox = new System.Windows.Forms.GroupBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)m_SpeedTb).BeginInit();
            m_VoiceTypeGroupBox.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // m_TextToSpeechTb
            // 
            resources.ApplyResources(m_TextToSpeechTb, "m_TextToSpeechTb");
            m_TextToSpeechTb.BackColor = System.Drawing.SystemColors.Window;
            m_TextToSpeechTb.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            m_TextToSpeechTb.Name = "m_TextToSpeechTb";
            m_TextToSpeechTb.Enter += m_TextToSpeechTb_Enter;
            m_TextToSpeechTb.Leave += m_TextToSpeechTb_Leave;
            // 
            // m_PreviewBtn
            // 
            resources.ApplyResources(m_PreviewBtn, "m_PreviewBtn");
            m_PreviewBtn.Name = "m_PreviewBtn";
            m_PreviewBtn.UseVisualStyleBackColor = true;
            m_PreviewBtn.Click += m_PreviewBtn_ClickAsync;
            // 
            // m_GenerateBtn
            // 
            resources.ApplyResources(m_GenerateBtn, "m_GenerateBtn");
            m_GenerateBtn.Name = "m_GenerateBtn";
            m_GenerateBtn.UseVisualStyleBackColor = true;
            m_GenerateBtn.Click += m_GenerateBtn_ClickAsync;
            // 
            // m_BuildInRbtn
            // 
            resources.ApplyResources(m_BuildInRbtn, "m_BuildInRbtn");
            m_BuildInRbtn.Checked = true;
            m_BuildInRbtn.Name = "m_BuildInRbtn";
            m_BuildInRbtn.TabStop = true;
            m_BuildInRbtn.UseVisualStyleBackColor = true;
            m_BuildInRbtn.CheckedChanged += m_BuildInRbtn_CheckedChanged;
            // 
            // m_AzureRbtn
            // 
            resources.ApplyResources(m_AzureRbtn, "m_AzureRbtn");
            m_AzureRbtn.Name = "m_AzureRbtn";
            m_AzureRbtn.UseVisualStyleBackColor = true;
            m_AzureRbtn.CheckedChanged += m_AzurRbtn_CheckedChanged;
            // 
            // m_VoiceSelectionCb
            // 
            resources.ApplyResources(m_VoiceSelectionCb, "m_VoiceSelectionCb");
            m_VoiceSelectionCb.FormattingEnabled = true;
            m_VoiceSelectionCb.Name = "m_VoiceSelectionCb";
            // 
            // m_ClearBtn
            // 
            resources.ApplyResources(m_ClearBtn, "m_ClearBtn");
            m_ClearBtn.Name = "m_ClearBtn";
            m_ClearBtn.UseVisualStyleBackColor = true;
            m_ClearBtn.Click += m_ClearBtn_Click;
            // 
            // m_SpeedTb
            // 
            resources.ApplyResources(m_SpeedTb, "m_SpeedTb");
            m_SpeedTb.Minimum = -10;
            m_SpeedTb.Name = "m_SpeedTb";
            // 
            // m_TTSVoiceLbl
            // 
            resources.ApplyResources(m_TTSVoiceLbl, "m_TTSVoiceLbl");
            m_TTSVoiceLbl.Name = "m_TTSVoiceLbl";
            // 
            // m_SpeechRateLbl
            // 
            resources.ApplyResources(m_SpeechRateLbl, "m_SpeechRateLbl");
            m_SpeechRateLbl.Name = "m_SpeechRateLbl";
            // 
            // m_FontSmallerBtn
            // 
            resources.ApplyResources(m_FontSmallerBtn, "m_FontSmallerBtn");
            m_FontSmallerBtn.Name = "m_FontSmallerBtn";
            m_FontSmallerBtn.UseVisualStyleBackColor = true;
            m_FontSmallerBtn.Click += m_FontSmallerBtn_Click;
            // 
            // m_FontBiggerBtn
            // 
            resources.ApplyResources(m_FontBiggerBtn, "m_FontBiggerBtn");
            m_FontBiggerBtn.Name = "m_FontBiggerBtn";
            m_FontBiggerBtn.UseVisualStyleBackColor = true;
            m_FontBiggerBtn.Click += m_FontBiggerBtn_Click;
            // 
            // m_AddAzureKeyBtn
            // 
            resources.ApplyResources(m_AddAzureKeyBtn, "m_AddAzureKeyBtn");
            m_AddAzureKeyBtn.Name = "m_AddAzureKeyBtn";
            m_AddAzureKeyBtn.UseVisualStyleBackColor = true;
            m_AddAzureKeyBtn.Click += m_AddAzureKeyBtn_Click;
            // 
            // m_AddAzureVoiceBtn
            // 
            resources.ApplyResources(m_AddAzureVoiceBtn, "m_AddAzureVoiceBtn");
            m_AddAzureVoiceBtn.Name = "m_AddAzureVoiceBtn";
            m_AddAzureVoiceBtn.UseVisualStyleBackColor = true;
            m_AddAzureVoiceBtn.Click += m_AddAzureVoiceBtn_Click;
            // 
            // m_DeleteAzureVoiceBtn
            // 
            resources.ApplyResources(m_DeleteAzureVoiceBtn, "m_DeleteAzureVoiceBtn");
            m_DeleteAzureVoiceBtn.Name = "m_DeleteAzureVoiceBtn";
            m_DeleteAzureVoiceBtn.UseVisualStyleBackColor = true;
            m_DeleteAzureVoiceBtn.Click += m_DeleteAzureVoiceBtn_Click;
            // 
            // m_CloseBtn
            // 
            m_CloseBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(m_CloseBtn, "m_CloseBtn");
            m_CloseBtn.Name = "m_CloseBtn";
            m_CloseBtn.UseVisualStyleBackColor = true;
            // 
            // m_VoiceTypeGroupBox
            // 
            resources.ApplyResources(m_VoiceTypeGroupBox, "m_VoiceTypeGroupBox");
            m_VoiceTypeGroupBox.Controls.Add(m_BuildInRbtn);
            m_VoiceTypeGroupBox.Controls.Add(m_AzureRbtn);
            m_VoiceTypeGroupBox.Name = "m_VoiceTypeGroupBox";
            m_VoiceTypeGroupBox.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(m_AddAzureVoiceBtn);
            groupBox1.Controls.Add(m_DeleteAzureVoiceBtn);
            groupBox1.Controls.Add(m_AddAzureKeyBtn);
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(m_PreviewBtn);
            groupBox2.Controls.Add(m_ClearBtn);
            groupBox2.Controls.Add(m_GenerateBtn);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // GenerateSpeech
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_CloseBtn;
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(m_VoiceTypeGroupBox);
            Controls.Add(m_CloseBtn);
            Controls.Add(m_FontSmallerBtn);
            Controls.Add(m_FontBiggerBtn);
            Controls.Add(m_SpeechRateLbl);
            Controls.Add(m_TTSVoiceLbl);
            Controls.Add(m_SpeedTb);
            Controls.Add(m_VoiceSelectionCb);
            Controls.Add(m_TextToSpeechTb);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GenerateSpeech";
            ((System.ComponentModel.ISupportInitialize)m_SpeedTb).EndInit();
            m_VoiceTypeGroupBox.ResumeLayout(false);
            m_VoiceTypeGroupBox.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_TextToSpeechTb;
        private System.Windows.Forms.Button m_PreviewBtn;
        private System.Windows.Forms.Button m_GenerateBtn;
        private System.Windows.Forms.RadioButton m_BuildInRbtn;
        private System.Windows.Forms.RadioButton m_AzureRbtn;
        private System.Windows.Forms.ComboBox m_VoiceSelectionCb;
        private System.Windows.Forms.Button m_ClearBtn;
        private System.Windows.Forms.TrackBar m_SpeedTb;
        private System.Windows.Forms.Label m_TTSVoiceLbl;
        private System.Windows.Forms.Label m_SpeechRateLbl;
        private System.Windows.Forms.Button m_FontSmallerBtn;
        private System.Windows.Forms.Button m_FontBiggerBtn;
        private System.Windows.Forms.Button m_AddAzureKeyBtn;
        private System.Windows.Forms.Button m_AddAzureVoiceBtn;
        private System.Windows.Forms.Button m_DeleteAzureVoiceBtn;
        private System.Windows.Forms.Button m_CloseBtn;
        private System.Windows.Forms.GroupBox m_VoiceTypeGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}