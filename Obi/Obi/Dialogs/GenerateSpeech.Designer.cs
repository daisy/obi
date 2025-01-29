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
            this.m_TextToSpeechTb.AccessibleName = "Write text for speech generation";
            this.m_TextToSpeechTb.Location = new System.Drawing.Point(14, 27);
            this.m_TextToSpeechTb.Name = "m_TextToSpeechTb";
            this.m_TextToSpeechTb.Size = new System.Drawing.Size(815, 203);
            this.m_TextToSpeechTb.TabIndex = 0;
            this.m_TextToSpeechTb.Text = "";
            // 
            // m_PreviewBtn
            // 
            this.m_PreviewBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_PreviewBtn.Location = new System.Drawing.Point(17, 23);
            this.m_PreviewBtn.Name = "m_PreviewBtn";
            this.m_PreviewBtn.Size = new System.Drawing.Size(106, 35);
            this.m_PreviewBtn.TabIndex = 11;
            this.m_PreviewBtn.Text = "&Preview";
            this.m_PreviewBtn.UseVisualStyleBackColor = true;
            this.m_PreviewBtn.Click += new System.EventHandler(this.m_PreviewBtn_ClickAsync);
            // 
            // m_GenerateBtn
            // 
            this.m_GenerateBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_GenerateBtn.Location = new System.Drawing.Point(255, 23);
            this.m_GenerateBtn.Name = "m_GenerateBtn";
            this.m_GenerateBtn.Size = new System.Drawing.Size(166, 35);
            this.m_GenerateBtn.TabIndex = 13;
            this.m_GenerateBtn.Text = "&Generate Speech";
            this.m_GenerateBtn.UseVisualStyleBackColor = true;
            this.m_GenerateBtn.Click += new System.EventHandler(this.m_GenerateBtn_ClickAsync);
            // 
            // m_BuildInRbtn
            // 
            this.m_BuildInRbtn.AccessibleName = "Build in voices";
            this.m_BuildInRbtn.AutoSize = true;
            this.m_BuildInRbtn.Checked = true;
            this.m_BuildInRbtn.Location = new System.Drawing.Point(20, 26);
            this.m_BuildInRbtn.Name = "m_BuildInRbtn";
            this.m_BuildInRbtn.Size = new System.Drawing.Size(142, 24);
            this.m_BuildInRbtn.TabIndex = 4;
            this.m_BuildInRbtn.TabStop = true;
            this.m_BuildInRbtn.Text = "&System Voices";
            this.m_BuildInRbtn.UseVisualStyleBackColor = true;
            this.m_BuildInRbtn.CheckedChanged += new System.EventHandler(this.m_BuildInRbtn_CheckedChanged);
            // 
            // m_AzureRbtn
            // 
            this.m_AzureRbtn.AccessibleName = "Azure voices";
            this.m_AzureRbtn.AutoSize = true;
            this.m_AzureRbtn.Location = new System.Drawing.Point(169, 26);
            this.m_AzureRbtn.Name = "m_AzureRbtn";
            this.m_AzureRbtn.Size = new System.Drawing.Size(183, 24);
            this.m_AzureRbtn.TabIndex = 5;
            this.m_AzureRbtn.Text = "Online &Azure Voices";
            this.m_AzureRbtn.UseVisualStyleBackColor = true;
            this.m_AzureRbtn.CheckedChanged += new System.EventHandler(this.m_AzurRbtn_CheckedChanged);
            // 
            // m_VoiceSelectionCb
            // 
            this.m_VoiceSelectionCb.AccessibleName = "TTS voice selection";
            this.m_VoiceSelectionCb.FormattingEnabled = true;
            this.m_VoiceSelectionCb.ItemHeight = 20;
            this.m_VoiceSelectionCb.Location = new System.Drawing.Point(173, 256);
            this.m_VoiceSelectionCb.Name = "m_VoiceSelectionCb";
            this.m_VoiceSelectionCb.Size = new System.Drawing.Size(636, 28);
            this.m_VoiceSelectionCb.TabIndex = 7;
            // 
            // m_ClearBtn
            // 
            this.m_ClearBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_ClearBtn.Location = new System.Drawing.Point(136, 23);
            this.m_ClearBtn.Name = "m_ClearBtn";
            this.m_ClearBtn.Size = new System.Drawing.Size(106, 35);
            this.m_ClearBtn.TabIndex = 12;
            this.m_ClearBtn.Text = "&Clear";
            this.m_ClearBtn.UseVisualStyleBackColor = true;
            this.m_ClearBtn.Click += new System.EventHandler(this.m_ClearBtn_Click);
            // 
            // m_SpeedTb
            // 
            this.m_SpeedTb.AccessibleName = "Speech Rate";
            this.m_SpeedTb.Location = new System.Drawing.Point(173, 320);
            this.m_SpeedTb.Minimum = -10;
            this.m_SpeedTb.Name = "m_SpeedTb";
            this.m_SpeedTb.Size = new System.Drawing.Size(636, 56);
            this.m_SpeedTb.TabIndex = 9;
            // 
            // m_TTSVoiceLbl
            // 
            this.m_TTSVoiceLbl.AutoSize = true;
            this.m_TTSVoiceLbl.Location = new System.Drawing.Point(50, 259);
            this.m_TTSVoiceLbl.Name = "m_TTSVoiceLbl";
            this.m_TTSVoiceLbl.Size = new System.Drawing.Size(92, 20);
            this.m_TTSVoiceLbl.TabIndex = 6;
            this.m_TTSVoiceLbl.Text = "TTS Voice:";
            // 
            // m_SpeechRateLbl
            // 
            this.m_SpeechRateLbl.AutoSize = true;
            this.m_SpeechRateLbl.Location = new System.Drawing.Point(46, 320);
            this.m_SpeechRateLbl.Name = "m_SpeechRateLbl";
            this.m_SpeechRateLbl.Size = new System.Drawing.Size(110, 20);
            this.m_SpeechRateLbl.TabIndex = 8;
            this.m_SpeechRateLbl.Text = "Speech Rate:";
            // 
            // m_FontSmallerBtn
            // 
            this.m_FontSmallerBtn.AccessibleName = "Decrease text size";
            this.m_FontSmallerBtn.Enabled = false;
            this.m_FontSmallerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_FontSmallerBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.m_FontSmallerBtn.Location = new System.Drawing.Point(777, 216);
            this.m_FontSmallerBtn.Name = "m_FontSmallerBtn";
            this.m_FontSmallerBtn.Size = new System.Drawing.Size(61, 37);
            this.m_FontSmallerBtn.TabIndex = 99;
            this.m_FontSmallerBtn.Text = "-";
            this.m_FontSmallerBtn.UseVisualStyleBackColor = true;
            this.m_FontSmallerBtn.Visible = false;
            this.m_FontSmallerBtn.Click += new System.EventHandler(this.m_FontSmallerBtn_Click);
            // 
            // m_FontBiggerBtn
            // 
            this.m_FontBiggerBtn.AccessibleName = "Increase text size";
            this.m_FontBiggerBtn.Enabled = false;
            this.m_FontBiggerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_FontBiggerBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.m_FontBiggerBtn.Location = new System.Drawing.Point(777, 131);
            this.m_FontBiggerBtn.Name = "m_FontBiggerBtn";
            this.m_FontBiggerBtn.Size = new System.Drawing.Size(61, 38);
            this.m_FontBiggerBtn.TabIndex = 100;
            this.m_FontBiggerBtn.Text = "+";
            this.m_FontBiggerBtn.UseVisualStyleBackColor = true;
            this.m_FontBiggerBtn.Visible = false;
            this.m_FontBiggerBtn.Click += new System.EventHandler(this.m_FontBiggerBtn_Click);
            // 
            // m_AddAzureKeyBtn
            // 
            this.m_AddAzureKeyBtn.Enabled = false;
            this.m_AddAzureKeyBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddAzureKeyBtn.Location = new System.Drawing.Point(312, 30);
            this.m_AddAzureKeyBtn.Name = "m_AddAzureKeyBtn";
            this.m_AddAzureKeyBtn.Size = new System.Drawing.Size(135, 35);
            this.m_AddAzureKeyBtn.TabIndex = 17;
            this.m_AddAzureKeyBtn.Text = "Add &Key";
            this.m_AddAzureKeyBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureKeyBtn.Click += new System.EventHandler(this.m_AddAzureKeyBtn_Click);
            // 
            // m_AddAzureVoiceBtn
            // 
            this.m_AddAzureVoiceBtn.Enabled = false;
            this.m_AddAzureVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_AddAzureVoiceBtn.Location = new System.Drawing.Point(19, 30);
            this.m_AddAzureVoiceBtn.Name = "m_AddAzureVoiceBtn";
            this.m_AddAzureVoiceBtn.Size = new System.Drawing.Size(133, 35);
            this.m_AddAzureVoiceBtn.TabIndex = 15;
            this.m_AddAzureVoiceBtn.Text = "&Add Voice";
            this.m_AddAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_AddAzureVoiceBtn.Click += new System.EventHandler(this.m_AddAzureVoiceBtn_Click);
            // 
            // m_DeleteAzureVoiceBtn
            // 
            this.m_DeleteAzureVoiceBtn.Enabled = false;
            this.m_DeleteAzureVoiceBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_DeleteAzureVoiceBtn.Location = new System.Drawing.Point(168, 30);
            this.m_DeleteAzureVoiceBtn.Name = "m_DeleteAzureVoiceBtn";
            this.m_DeleteAzureVoiceBtn.Size = new System.Drawing.Size(126, 35);
            this.m_DeleteAzureVoiceBtn.TabIndex = 16;
            this.m_DeleteAzureVoiceBtn.Text = "&Delete Voice";
            this.m_DeleteAzureVoiceBtn.UseVisualStyleBackColor = true;
            this.m_DeleteAzureVoiceBtn.Click += new System.EventHandler(this.m_DeleteAzureVoiceBtn_Click);
            // 
            // m_CloseBtn
            // 
            this.m_CloseBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_CloseBtn.Location = new System.Drawing.Point(703, 513);
            this.m_CloseBtn.Name = "m_CloseBtn";
            this.m_CloseBtn.Size = new System.Drawing.Size(126, 35);
            this.m_CloseBtn.TabIndex = 18;
            this.m_CloseBtn.Text = "&Cancel";
            this.m_CloseBtn.UseVisualStyleBackColor = true;
            // 
            // m_VoiceTypeGroupBox
            // 
            this.m_VoiceTypeGroupBox.AccessibleName = "Group box to select system or azure voices";
            this.m_VoiceTypeGroupBox.Controls.Add(this.m_BuildInRbtn);
            this.m_VoiceTypeGroupBox.Controls.Add(this.m_AzureRbtn);
            this.m_VoiceTypeGroupBox.Location = new System.Drawing.Point(24, 398);
            this.m_VoiceTypeGroupBox.Name = "m_VoiceTypeGroupBox";
            this.m_VoiceTypeGroupBox.Size = new System.Drawing.Size(357, 70);
            this.m_VoiceTypeGroupBox.TabIndex = 3;
            this.m_VoiceTypeGroupBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_AddAzureVoiceBtn);
            this.groupBox1.Controls.Add(this.m_DeleteAzureVoiceBtn);
            this.groupBox1.Controls.Add(this.m_AddAzureKeyBtn);
            this.groupBox1.Location = new System.Drawing.Point(25, 483);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 77);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Azure";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleName = "Group Box to preview and generate speech";
            this.groupBox2.Controls.Add(this.m_PreviewBtn);
            this.groupBox2.Controls.Add(this.m_ClearBtn);
            this.groupBox2.Controls.Add(this.m_GenerateBtn);
            this.groupBox2.Location = new System.Drawing.Point(400, 398);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(429, 70);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            // 
            // GenerateSpeech
            // 
            this.AccessibleName = "Generate Audio";
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 576);
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
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "GenerateSpeech";
            this.Text = "Generate Audio";
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