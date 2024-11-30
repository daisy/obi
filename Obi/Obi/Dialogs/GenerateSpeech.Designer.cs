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
            this.m_TextToSpeechTb = new System.Windows.Forms.RichTextBox();
            this.m_PreviewBtn = new System.Windows.Forms.Button();
            this.m_StopBtn = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.m_SpeedTb)).BeginInit();
            this.SuspendLayout();
            // 
            // m_TextToSpeechTb
            // 
            this.m_TextToSpeechTb.AccessibleName = "Text to audio";
            this.m_TextToSpeechTb.Location = new System.Drawing.Point(12, 33);
            this.m_TextToSpeechTb.Name = "m_TextToSpeechTb";
            this.m_TextToSpeechTb.Size = new System.Drawing.Size(925, 422);
            this.m_TextToSpeechTb.TabIndex = 0;
            this.m_TextToSpeechTb.Text = "";
            // 
            // m_PreviewBtn
            // 
            this.m_PreviewBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_PreviewBtn.Location = new System.Drawing.Point(129, 668);
            this.m_PreviewBtn.Name = "m_PreviewBtn";
            this.m_PreviewBtn.Size = new System.Drawing.Size(94, 29);
            this.m_PreviewBtn.TabIndex = 9;
            this.m_PreviewBtn.Text = "&Preview";
            this.m_PreviewBtn.UseVisualStyleBackColor = true;
            this.m_PreviewBtn.Click += new System.EventHandler(this.m_PreviewBtn_ClickAsync);
            // 
            // m_StopBtn
            // 
            this.m_StopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_StopBtn.Location = new System.Drawing.Point(288, 668);
            this.m_StopBtn.Name = "m_StopBtn";
            this.m_StopBtn.Size = new System.Drawing.Size(94, 29);
            this.m_StopBtn.TabIndex = 10;
            this.m_StopBtn.Text = "&Stop";
            this.m_StopBtn.UseVisualStyleBackColor = true;
            this.m_StopBtn.Click += new System.EventHandler(this.m_StopBtn_Click);
            // 
            // m_GenerateBtn
            // 
            this.m_GenerateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_GenerateBtn.Location = new System.Drawing.Point(605, 668);
            this.m_GenerateBtn.Name = "m_GenerateBtn";
            this.m_GenerateBtn.Size = new System.Drawing.Size(94, 29);
            this.m_GenerateBtn.TabIndex = 12;
            this.m_GenerateBtn.Text = "&Generate";
            this.m_GenerateBtn.UseVisualStyleBackColor = true;
            this.m_GenerateBtn.Click += new System.EventHandler(this.m_GenerateBtn_ClickAsync);
            // 
            // m_BuildInRbtn
            // 
            this.m_BuildInRbtn.AccessibleName = "Build in voices";
            this.m_BuildInRbtn.AutoSize = true;
            this.m_BuildInRbtn.Checked = true;
            this.m_BuildInRbtn.Location = new System.Drawing.Point(85, 619);
            this.m_BuildInRbtn.Name = "m_BuildInRbtn";
            this.m_BuildInRbtn.Size = new System.Drawing.Size(80, 24);
            this.m_BuildInRbtn.TabIndex = 3;
            this.m_BuildInRbtn.TabStop = true;
            this.m_BuildInRbtn.Text = "&Build In";
            this.m_BuildInRbtn.UseVisualStyleBackColor = true;
            this.m_BuildInRbtn.CheckedChanged += new System.EventHandler(this.m_BuildInRbtn_CheckedChanged);
            // 
            // m_AzureRbtn
            // 
            this.m_AzureRbtn.AccessibleName = "Azure voices";
            this.m_AzureRbtn.AutoSize = true;
            this.m_AzureRbtn.Location = new System.Drawing.Point(288, 619);
            this.m_AzureRbtn.Name = "m_AzureRbtn";
            this.m_AzureRbtn.Size = new System.Drawing.Size(68, 24);
            this.m_AzureRbtn.TabIndex = 4;
            this.m_AzureRbtn.Text = "&Azure";
            this.m_AzureRbtn.UseVisualStyleBackColor = true;
            this.m_AzureRbtn.CheckedChanged += new System.EventHandler(this.m_AzurRbtn_CheckedChanged);
            // 
            // m_VoiceSelectionCb
            // 
            this.m_VoiceSelectionCb.AccessibleName = "TTS voice";
            this.m_VoiceSelectionCb.FormattingEnabled = true;
            this.m_VoiceSelectionCb.ItemHeight = 20;
            this.m_VoiceSelectionCb.Location = new System.Drawing.Point(154, 485);
            this.m_VoiceSelectionCb.Name = "m_VoiceSelectionCb";
            this.m_VoiceSelectionCb.Size = new System.Drawing.Size(783, 28);
            this.m_VoiceSelectionCb.TabIndex = 6;
            // 
            // m_ClearBtn
            // 
            this.m_ClearBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_ClearBtn.Location = new System.Drawing.Point(442, 668);
            this.m_ClearBtn.Name = "m_ClearBtn";
            this.m_ClearBtn.Size = new System.Drawing.Size(94, 29);
            this.m_ClearBtn.TabIndex = 11;
            this.m_ClearBtn.Text = "&Clear";
            this.m_ClearBtn.UseVisualStyleBackColor = true;
            this.m_ClearBtn.Click += new System.EventHandler(this.m_ClearBtn_Click);
            // 
            // m_SpeedTb
            // 
            this.m_SpeedTb.AccessibleName = "Speech Rate";
            this.m_SpeedTb.Location = new System.Drawing.Point(154, 549);
            this.m_SpeedTb.Minimum = -10;
            this.m_SpeedTb.Name = "m_SpeedTb";
            this.m_SpeedTb.Size = new System.Drawing.Size(783, 56);
            this.m_SpeedTb.TabIndex = 8;
            // 
            // m_TTSVoiceLbl
            // 
            this.m_TTSVoiceLbl.AutoSize = true;
            this.m_TTSVoiceLbl.Location = new System.Drawing.Point(44, 488);
            this.m_TTSVoiceLbl.Name = "m_TTSVoiceLbl";
            this.m_TTSVoiceLbl.Size = new System.Drawing.Size(76, 20);
            this.m_TTSVoiceLbl.TabIndex = 5;
            this.m_TTSVoiceLbl.Text = "TTS Voice:";
            // 
            // m_SpeechRateLbl
            // 
            this.m_SpeechRateLbl.AutoSize = true;
            this.m_SpeechRateLbl.Location = new System.Drawing.Point(41, 549);
            this.m_SpeechRateLbl.Name = "m_SpeechRateLbl";
            this.m_SpeechRateLbl.Size = new System.Drawing.Size(94, 20);
            this.m_SpeechRateLbl.TabIndex = 7;
            this.m_SpeechRateLbl.Text = "Speech Rate:";
            // 
            // m_FontSmallerBtn
            // 
            this.m_FontSmallerBtn.AccessibleName = "Decrease text size";
            this.m_FontSmallerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_FontSmallerBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.m_FontSmallerBtn.Location = new System.Drawing.Point(959, 245);
            this.m_FontSmallerBtn.Name = "m_FontSmallerBtn";
            this.m_FontSmallerBtn.Size = new System.Drawing.Size(54, 37);
            this.m_FontSmallerBtn.TabIndex = 2;
            this.m_FontSmallerBtn.Text = "-";
            this.m_FontSmallerBtn.UseVisualStyleBackColor = true;
            this.m_FontSmallerBtn.Click += new System.EventHandler(this.m_FontSmallerBtn_Click);
            // 
            // m_FontBiggerBtn
            // 
            this.m_FontBiggerBtn.AccessibleName = "Increase text size";
            this.m_FontBiggerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_FontBiggerBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.m_FontBiggerBtn.Location = new System.Drawing.Point(959, 160);
            this.m_FontBiggerBtn.Name = "m_FontBiggerBtn";
            this.m_FontBiggerBtn.Size = new System.Drawing.Size(54, 38);
            this.m_FontBiggerBtn.TabIndex = 1;
            this.m_FontBiggerBtn.Text = "+";
            this.m_FontBiggerBtn.UseVisualStyleBackColor = true;
            this.m_FontBiggerBtn.Click += new System.EventHandler(this.m_FontBiggerBtn_Click);
            // 
            // m_AddAzureKeyBtn
            // 
            this.m_AddAzureKeyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddAzureKeyBtn.Location = new System.Drawing.Point(851, 617);
            this.m_AddAzureKeyBtn.Name = "m_AddAzureKeyBtn";
            this.m_AddAzureKeyBtn.Size = new System.Drawing.Size(142, 29);
            this.m_AddAzureKeyBtn.TabIndex = 16;
            this.m_AddAzureKeyBtn.Text = "Add Azure &Key";
            this.m_AddAzureKeyBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureKeyBtn.Click += new System.EventHandler(this.m_AddAzureKeyBtn_Click);
            // 
            // m_AddAzureVoiceBtn
            // 
            this.m_AddAzureVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddAzureVoiceBtn.Location = new System.Drawing.Point(453, 617);
            this.m_AddAzureVoiceBtn.Name = "m_AddAzureVoiceBtn";
            this.m_AddAzureVoiceBtn.Size = new System.Drawing.Size(151, 29);
            this.m_AddAzureVoiceBtn.TabIndex = 14;
            this.m_AddAzureVoiceBtn.Text = "&Add Azure Voice";
            this.m_AddAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureVoiceBtn.Click += new System.EventHandler(this.m_AddAzureVoiceBtn_Click);
            // 
            // m_DeleteAzureVoiceBtn
            // 
            this.m_DeleteAzureVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_DeleteAzureVoiceBtn.Location = new System.Drawing.Point(635, 617);
            this.m_DeleteAzureVoiceBtn.Name = "m_DeleteAzureVoiceBtn";
            this.m_DeleteAzureVoiceBtn.Size = new System.Drawing.Size(175, 29);
            this.m_DeleteAzureVoiceBtn.TabIndex = 15;
            this.m_DeleteAzureVoiceBtn.Text = "&Delete Azure Voice";
            this.m_DeleteAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_DeleteAzureVoiceBtn.Click += new System.EventHandler(this.m_DeleteAzureVoiceBtn_Click);
            // 
            // m_CloseBtn
            // 
            this.m_CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_CloseBtn.Location = new System.Drawing.Point(755, 668);
            this.m_CloseBtn.Name = "m_CloseBtn";
            this.m_CloseBtn.Size = new System.Drawing.Size(94, 29);
            this.m_CloseBtn.TabIndex = 13;
            this.m_CloseBtn.Text = "&Close";
            this.m_CloseBtn.UseVisualStyleBackColor = true;
            this.m_CloseBtn.Click += new System.EventHandler(this.m_CloseBtn_Click);
            // 
            // GenerateSpeech
            // 
            this.AccessibleName = "Generate Audio";
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 732);
            this.Controls.Add(this.m_CloseBtn);
            this.Controls.Add(this.m_DeleteAzureVoiceBtn);
            this.Controls.Add(this.m_AddAzureVoiceBtn);
            this.Controls.Add(this.m_AddAzureKeyBtn);
            this.Controls.Add(this.m_FontSmallerBtn);
            this.Controls.Add(this.m_FontBiggerBtn);
            this.Controls.Add(this.m_SpeechRateLbl);
            this.Controls.Add(this.m_TTSVoiceLbl);
            this.Controls.Add(this.m_SpeedTb);
            this.Controls.Add(this.m_ClearBtn);
            this.Controls.Add(this.m_VoiceSelectionCb);
            this.Controls.Add(this.m_AzureRbtn);
            this.Controls.Add(this.m_BuildInRbtn);
            this.Controls.Add(this.m_GenerateBtn);
            this.Controls.Add(this.m_StopBtn);
            this.Controls.Add(this.m_PreviewBtn);
            this.Controls.Add(this.m_TextToSpeechTb);
            this.Name = "GenerateSpeech";
            this.Text = "Generate Audio";
            ((System.ComponentModel.ISupportInitialize)(this.m_SpeedTb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_TextToSpeechTb;
        private System.Windows.Forms.Button m_PreviewBtn;
        private System.Windows.Forms.Button m_StopBtn;
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
    }
}