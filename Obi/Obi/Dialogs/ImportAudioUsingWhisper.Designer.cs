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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportAudioUsingWhisper));
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
            resources.ApplyResources(m_LogTxt, "m_LogTxt");
            m_LogTxt.Name = "m_LogTxt";
            m_LogTxt.ReadOnly = true;
            // 
            // m_ProgressBar
            // 
            resources.ApplyResources(m_ProgressBar, "m_ProgressBar");
            m_ProgressBar.Name = "m_ProgressBar";
            // 
            // m_btnCancel
            // 
            resources.ApplyResources(m_btnCancel, "m_btnCancel");
            m_btnCancel.Name = "m_btnCancel";
            m_btnCancel.UseVisualStyleBackColor = true;
            m_btnCancel.Click += m_btnCancel_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // m_ModelCb
            // 
            resources.ApplyResources(m_ModelCb, "m_ModelCb");
            m_ModelCb.FormattingEnabled = true;
            m_ModelCb.Name = "m_ModelCb";
            // 
            // m_btnStart
            // 
            resources.ApplyResources(m_btnStart, "m_btnStart");
            m_btnStart.Name = "m_btnStart";
            m_btnStart.UseVisualStyleBackColor = true;
            m_btnStart.Click += m_btnStart_Click;
            // 
            // lblBookLanguage
            // 
            resources.ApplyResources(lblBookLanguage, "lblBookLanguage");
            lblBookLanguage.Name = "lblBookLanguage";
            // 
            // m_BookLanguageCb
            // 
            resources.ApplyResources(m_BookLanguageCb, "m_BookLanguageCb");
            m_BookLanguageCb.FormattingEnabled = true;
            m_BookLanguageCb.Name = "m_BookLanguageCb";
            // 
            // ImportAudioUsingWhisper
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = m_btnCancel;
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