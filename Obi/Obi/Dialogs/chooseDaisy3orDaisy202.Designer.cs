namespace Obi.Dialogs
{
    partial class chooseDaisy3orDaisy202
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(chooseDaisy3orDaisy202));
            this.m_OKBtn = new System.Windows.Forms.Button();
            this.m_BtnCancel = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_cbWPAudioBook = new System.Windows.Forms.CheckBox();
            this.m_cbDaisy3 = new System.Windows.Forms.CheckBox();
            this.m_cbDaisy202 = new System.Windows.Forms.CheckBox();
            this.m_cbEpub3 = new System.Windows.Forms.CheckBox();
            this.m_cbXhtml = new System.Windows.Forms.CheckBox();
            this.m_chMegaVoice = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // m_OKBtn
            // 
            resources.ApplyResources(this.m_OKBtn, "m_OKBtn");
            this.m_OKBtn.Name = "m_OKBtn";
            this.m_OKBtn.UseVisualStyleBackColor = true;
            this.m_OKBtn.Click += new System.EventHandler(this.m_OKBtn_Click);
            // 
            // m_BtnCancel
            // 
            this.m_BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_BtnCancel, "m_BtnCancel");
            this.m_BtnCancel.Name = "m_BtnCancel";
            this.m_BtnCancel.UseVisualStyleBackColor = true;
            this.m_BtnCancel.Click += new System.EventHandler(this.m_BtnCancel_Click);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_cbWPAudioBook
            // 
            resources.ApplyResources(this.m_cbWPAudioBook, "m_cbWPAudioBook");
            this.m_cbWPAudioBook.Name = "m_cbWPAudioBook";
            this.helpProvider1.SetShowHelp(this.m_cbWPAudioBook, ((bool)(resources.GetObject("m_cbWPAudioBook.ShowHelp"))));
            this.m_cbWPAudioBook.UseVisualStyleBackColor = true;
            // 
            // m_cbDaisy3
            // 
            resources.ApplyResources(this.m_cbDaisy3, "m_cbDaisy3");
            this.m_cbDaisy3.Checked = true;
            this.m_cbDaisy3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_cbDaisy3.Name = "m_cbDaisy3";
            this.m_cbDaisy3.UseVisualStyleBackColor = true;
            this.m_cbDaisy3.CheckedChanged += new System.EventHandler(this.m_cbDaisy3_CheckedChanged);
            // 
            // m_cbDaisy202
            // 
            resources.ApplyResources(this.m_cbDaisy202, "m_cbDaisy202");
            this.m_cbDaisy202.Name = "m_cbDaisy202";
            this.m_cbDaisy202.UseVisualStyleBackColor = true;
            this.m_cbDaisy202.CheckedChanged += new System.EventHandler(this.m_cbDaisy202_CheckedChanged);
            // 
            // m_cbEpub3
            // 
            resources.ApplyResources(this.m_cbEpub3, "m_cbEpub3");
            this.m_cbEpub3.Name = "m_cbEpub3";
            this.m_cbEpub3.UseVisualStyleBackColor = true;
            // 
            // m_cbXhtml
            // 
            resources.ApplyResources(this.m_cbXhtml, "m_cbXhtml");
            this.m_cbXhtml.Name = "m_cbXhtml";
            this.m_cbXhtml.UseVisualStyleBackColor = true;
            // 
            // m_chMegaVoice
            // 
            resources.ApplyResources(this.m_chMegaVoice, "m_chMegaVoice");
            this.m_chMegaVoice.Name = "m_chMegaVoice";
            this.m_chMegaVoice.UseVisualStyleBackColor = true;
            // 
            // chooseDaisy3orDaisy202
            // 
            this.AcceptButton = this.m_OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_BtnCancel;
            this.Controls.Add(this.m_cbWPAudioBook);
            this.Controls.Add(this.m_chMegaVoice);
            this.Controls.Add(this.m_cbXhtml);
            this.Controls.Add(this.m_cbEpub3);
            this.Controls.Add(this.m_cbDaisy202);
            this.Controls.Add(this.m_cbDaisy3);
            this.Controls.Add(this.m_BtnCancel);
            this.Controls.Add(this.m_OKBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "chooseDaisy3orDaisy202";
            this.helpProvider1.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_OKBtn;
        private System.Windows.Forms.Button m_BtnCancel;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.CheckBox m_cbDaisy3;
        private System.Windows.Forms.CheckBox m_cbDaisy202;
        private System.Windows.Forms.CheckBox m_cbEpub3;
        private System.Windows.Forms.CheckBox m_cbXhtml;
        private System.Windows.Forms.CheckBox m_chMegaVoice;
        private System.Windows.Forms.CheckBox m_cbWPAudioBook;
    }
}