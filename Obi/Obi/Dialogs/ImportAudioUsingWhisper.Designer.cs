namespace Obi.Dialogs
{
    partial class ImportAudioUsingWhisper
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
            m_LogTxt = new System.Windows.Forms.TextBox();
            m_ProgressBar = new System.Windows.Forms.ProgressBar();
            m_btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            m_ModelCb = new System.Windows.Forms.ComboBox();
            m_btnStart = new System.Windows.Forms.Button();
            lblBookLanguage = new System.Windows.Forms.Label();
            m_BookLanguageCb = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // m_LogTxt
            // 
            m_LogTxt.Location = new System.Drawing.Point(32, 259);
            m_LogTxt.Multiline = true;
            m_LogTxt.Name = "m_LogTxt";
            m_LogTxt.ReadOnly = true;
            m_LogTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            m_LogTxt.Size = new System.Drawing.Size(911, 209);
            m_LogTxt.TabIndex = 6;
            // 
            // m_ProgressBar
            // 
            m_ProgressBar.AccessibleName = "Progress Bar";
            m_ProgressBar.Location = new System.Drawing.Point(32, 145);
            m_ProgressBar.Name = "m_ProgressBar";
            m_ProgressBar.Size = new System.Drawing.Size(911, 37);
            m_ProgressBar.TabIndex = 5;
            // 
            // m_btnCancel
            // 
            m_btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnCancel.Location = new System.Drawing.Point(491, 506);
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.Size = new System.Drawing.Size(168, 35);
            m_btnCancel.TabIndex = 8;
            m_btnCancel.Text = "&Cancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(32, 27);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(157, 20);
            label1.TabIndex = 1;
            label1.Text = "Select Whisper Model:";
            // 
            // m_ModelCb
            // 
            m_ModelCb.AccessibleName = "Select Whisper Model";
            m_ModelCb.FormattingEnabled = true;
            m_ModelCb.Location = new System.Drawing.Point(250, 24);
            m_ModelCb.Name = "m_ModelCb";
            m_ModelCb.Size = new System.Drawing.Size(226, 28);
            m_ModelCb.TabIndex = 2;
            // 
            // m_btnStart
            // 
            m_btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            m_btnStart.Location = new System.Drawing.Point(168, 506);
            m_btnStart.Name = "m_btnStart";
            m_btnStart.Size = new System.Drawing.Size(168, 35);
            m_btnStart.TabIndex = 7;
            m_btnStart.Text = "&Start";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // lblBookLanguage
            // 
            lblBookLanguage.AutoSize = true;
            lblBookLanguage.Location = new System.Drawing.Point(74, 70);
            lblBookLanguage.Name = "lblBookLanguage";
            lblBookLanguage.Size = new System.Drawing.Size(115, 20);
            lblBookLanguage.TabIndex = 3;
            lblBookLanguage.Text = "Book Language:";
            // 
            // m_BookLanguageCb
            // 
            m_BookLanguageCb.AccessibleName = "Select Book Language";
            m_BookLanguageCb.FormattingEnabled = true;
            m_BookLanguageCb.Location = new System.Drawing.Point(250, 70);
            m_BookLanguageCb.Name = "m_BookLanguageCb";
            m_BookLanguageCb.Size = new System.Drawing.Size(226, 28);
            m_BookLanguageCb.TabIndex = 4;
            // 
            // ImportAudioUsingWhisper
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_btnCancel;
            ClientSize = new System.Drawing.Size(965, 572);
            Controls.Add(m_BookLanguageCb);
            Controls.Add(lblBookLanguage);
            Controls.Add(m_btnStart);
            Controls.Add(m_ModelCb);
            Controls.Add(label1);
            Controls.Add(m_btnCancel);
            Controls.Add(m_LogTxt);
            Controls.Add(m_ProgressBar);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportAudioUsingWhisper";
            Text = "Import Audio Using Whisper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox m_LogTxt;
        private System.Windows.Forms.ProgressBar m_ProgressBar;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox m_ModelCb;
        private System.Windows.Forms.Button m_btnStart;
        private System.Windows.Forms.Label lblBookLanguage;
        private System.Windows.Forms.ComboBox m_BookLanguageCb;
    }
}