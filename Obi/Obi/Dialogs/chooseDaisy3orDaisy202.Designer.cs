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
            this.m_radBtnDaisy3 = new System.Windows.Forms.RadioButton();
            this.m_radBtnDaisy202 = new System.Windows.Forms.RadioButton();
            this.m_OKBtn = new System.Windows.Forms.Button();
            this.m_BtnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_radBtnDaisy3
            // 
            resources.ApplyResources(this.m_radBtnDaisy3, "m_radBtnDaisy3");
            this.m_radBtnDaisy3.Name = "m_radBtnDaisy3";
            this.m_radBtnDaisy3.TabStop = true;
            this.m_radBtnDaisy3.UseVisualStyleBackColor = true;
            // 
            // m_radBtnDaisy202
            // 
            resources.ApplyResources(this.m_radBtnDaisy202, "m_radBtnDaisy202");
            this.m_radBtnDaisy202.Name = "m_radBtnDaisy202";
            this.m_radBtnDaisy202.TabStop = true;
            this.m_radBtnDaisy202.UseVisualStyleBackColor = true;
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
            // chooseDaisy3orDaisy202
            // 
            this.AcceptButton = this.m_OKBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_BtnCancel;
            this.Controls.Add(this.m_BtnCancel);
            this.Controls.Add(this.m_OKBtn);
            this.Controls.Add(this.m_radBtnDaisy202);
            this.Controls.Add(this.m_radBtnDaisy3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "chooseDaisy3orDaisy202";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_radBtnDaisy3;
        private System.Windows.Forms.RadioButton m_radBtnDaisy202;
        private System.Windows.Forms.Button m_OKBtn;
        private System.Windows.Forms.Button m_BtnCancel;
    }
}