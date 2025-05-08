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
            this.m_TextToSpeechTb = new System.Windows.Forms.RichTextBox();
            this.m_PreviewBtn = new System.Windows.Forms.Button();
            this.m_GenerateBtn = new System.Windows.Forms.Button();
            this.m_BuildInRbtn = new System.Windows.Forms.RadioButton();
            this.m_AzureRbtn = new System.Windows.Forms.RadioButton();
            this.m_VoiceSelectionCb = new System.Windows.Forms.ComboBox();
            this.m_ClearBtn = new System.Windows.Forms.Button();
            this.m_SpeedTb = new System.Windows.Forms.TrackBar();
            this.m_TTSVoiceLbl = new System.Windows.Forms.Label();
            this.m_SpeechRateLbl = new System.Windows.Forms.Label();
            this.m_FontSmallerBtn = new System.Windows.Forms.Button();
            this.m_FontBiggerBtn = new System.Windows.Forms.Button();
            this.m_AddAzureKeyBtn = new System.Windows.Forms.Button();
            this.m_AddAzureVoiceBtn = new System.Windows.Forms.Button();
            this.m_DeleteAzureVoiceBtn = new System.Windows.Forms.Button();
            this.m_CloseBtn = new System.Windows.Forms.Button();
            this.m_VoiceTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_SpeedTb)).BeginInit();
            this.m_VoiceTypeGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_TextToSpeechTb
            // 
            resources.ApplyResources(this.m_TextToSpeechTb, "m_TextToSpeechTb");
            this.m_TextToSpeechTb.BackColor = System.Drawing.SystemColors.Window;
            this.m_TextToSpeechTb.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.m_TextToSpeechTb.Name = "m_TextToSpeechTb";
            this.m_TextToSpeechTb.Enter += new System.EventHandler(this.m_TextToSpeechTb_Enter);
            this.m_TextToSpeechTb.Leave += new System.EventHandler(this.m_TextToSpeechTb_Leave);
            // 
            // m_PreviewBtn
            // 
            resources.ApplyResources(this.m_PreviewBtn, "m_PreviewBtn");
            this.m_PreviewBtn.Name = "m_PreviewBtn";
            this.m_PreviewBtn.UseVisualStyleBackColor = true;
            this.m_PreviewBtn.Click += new System.EventHandler(this.m_PreviewBtn_ClickAsync);
            // 
            // m_GenerateBtn
            // 
            resources.ApplyResources(this.m_GenerateBtn, "m_GenerateBtn");
            this.m_GenerateBtn.Name = "m_GenerateBtn";
            this.m_GenerateBtn.UseVisualStyleBackColor = true;
            this.m_GenerateBtn.Click += new System.EventHandler(this.m_GenerateBtn_ClickAsync);
            // 
            // m_BuildInRbtn
            // 
            resources.ApplyResources(this.m_BuildInRbtn, "m_BuildInRbtn");
            this.m_BuildInRbtn.Checked = true;
            this.m_BuildInRbtn.Name = "m_BuildInRbtn";
            this.m_BuildInRbtn.TabStop = true;
            this.m_BuildInRbtn.UseVisualStyleBackColor = true;
            this.m_BuildInRbtn.CheckedChanged += new System.EventHandler(this.m_BuildInRbtn_CheckedChanged);
            // 
            // m_AzureRbtn
            // 
            resources.ApplyResources(this.m_AzureRbtn, "m_AzureRbtn");
            this.m_AzureRbtn.Name = "m_AzureRbtn";
            this.m_AzureRbtn.UseVisualStyleBackColor = true;
            this.m_AzureRbtn.CheckedChanged += new System.EventHandler(this.m_AzurRbtn_CheckedChanged);
            // 
            // m_VoiceSelectionCb
            // 
            resources.ApplyResources(this.m_VoiceSelectionCb, "m_VoiceSelectionCb");
            this.m_VoiceSelectionCb.FormattingEnabled = true;
            this.m_VoiceSelectionCb.Name = "m_VoiceSelectionCb";
            // 
            // m_ClearBtn
            // 
            resources.ApplyResources(this.m_ClearBtn, "m_ClearBtn");
            this.m_ClearBtn.Name = "m_ClearBtn";
            this.m_ClearBtn.UseVisualStyleBackColor = true;
            this.m_ClearBtn.Click += new System.EventHandler(this.m_ClearBtn_Click);
            // 
            // m_SpeedTb
            // 
            resources.ApplyResources(this.m_SpeedTb, "m_SpeedTb");
            this.m_SpeedTb.Minimum = -10;
            this.m_SpeedTb.Name = "m_SpeedTb";
            // 
            // m_TTSVoiceLbl
            // 
            resources.ApplyResources(this.m_TTSVoiceLbl, "m_TTSVoiceLbl");
            this.m_TTSVoiceLbl.Name = "m_TTSVoiceLbl";
            // 
            // m_SpeechRateLbl
            // 
            resources.ApplyResources(this.m_SpeechRateLbl, "m_SpeechRateLbl");
            this.m_SpeechRateLbl.Name = "m_SpeechRateLbl";
            // 
            // m_FontSmallerBtn
            // 
            resources.ApplyResources(this.m_FontSmallerBtn, "m_FontSmallerBtn");
            this.m_FontSmallerBtn.Name = "m_FontSmallerBtn";
            this.m_FontSmallerBtn.UseVisualStyleBackColor = true;
            this.m_FontSmallerBtn.Click += new System.EventHandler(this.m_FontSmallerBtn_Click);
            // 
            // m_FontBiggerBtn
            // 
            resources.ApplyResources(this.m_FontBiggerBtn, "m_FontBiggerBtn");
            this.m_FontBiggerBtn.Name = "m_FontBiggerBtn";
            this.m_FontBiggerBtn.UseVisualStyleBackColor = true;
            this.m_FontBiggerBtn.Click += new System.EventHandler(this.m_FontBiggerBtn_Click);
            // 
            // m_AddAzureKeyBtn
            // 
            resources.ApplyResources(this.m_AddAzureKeyBtn, "m_AddAzureKeyBtn");
            this.m_AddAzureKeyBtn.Name = "m_AddAzureKeyBtn";
            this.m_AddAzureKeyBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureKeyBtn.Click += new System.EventHandler(this.m_AddAzureKeyBtn_Click);
            // 
            // m_AddAzureVoiceBtn
            // 
            resources.ApplyResources(this.m_AddAzureVoiceBtn, "m_AddAzureVoiceBtn");
            this.m_AddAzureVoiceBtn.Name = "m_AddAzureVoiceBtn";
            this.m_AddAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureVoiceBtn.Click += new System.EventHandler(this.m_AddAzureVoiceBtn_Click);
            // 
            // m_DeleteAzureVoiceBtn
            // 
            resources.ApplyResources(this.m_DeleteAzureVoiceBtn, "m_DeleteAzureVoiceBtn");
            this.m_DeleteAzureVoiceBtn.Name = "m_DeleteAzureVoiceBtn";
            this.m_DeleteAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_DeleteAzureVoiceBtn.Click += new System.EventHandler(this.m_DeleteAzureVoiceBtn_Click);
            // 
            // m_CloseBtn
            // 
            this.m_CloseBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_CloseBtn, "m_CloseBtn");
            this.m_CloseBtn.Name = "m_CloseBtn";
            this.m_CloseBtn.UseVisualStyleBackColor = true;
            // 
            // m_VoiceTypeGroupBox
            // 
            resources.ApplyResources(this.m_VoiceTypeGroupBox, "m_VoiceTypeGroupBox");
            this.m_VoiceTypeGroupBox.Controls.Add(this.m_BuildInRbtn);
            this.m_VoiceTypeGroupBox.Controls.Add(this.m_AzureRbtn);
            this.m_VoiceTypeGroupBox.Name = "m_VoiceTypeGroupBox";
            this.m_VoiceTypeGroupBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_AddAzureVoiceBtn);
            this.groupBox1.Controls.Add(this.m_DeleteAzureVoiceBtn);
            this.groupBox1.Controls.Add(this.m_AddAzureKeyBtn);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.m_PreviewBtn);
            this.groupBox2.Controls.Add(this.m_ClearBtn);
            this.groupBox2.Controls.Add(this.m_GenerateBtn);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // GenerateSpeech
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.m_VoiceTypeGroupBox);
            this.Controls.Add(this.m_CloseBtn);
            this.Controls.Add(this.m_FontSmallerBtn);
            this.Controls.Add(this.m_FontBiggerBtn);
            this.Controls.Add(this.m_SpeechRateLbl);
            this.Controls.Add(this.m_TTSVoiceLbl);
            this.Controls.Add(this.m_SpeedTb);
            this.Controls.Add(this.m_VoiceSelectionCb);
            this.Controls.Add(this.m_TextToSpeechTb);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerateSpeech";
            ((System.ComponentModel.ISupportInitialize)(this.m_SpeedTb)).EndInit();
            this.m_VoiceTypeGroupBox.ResumeLayout(false);
            this.m_VoiceTypeGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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