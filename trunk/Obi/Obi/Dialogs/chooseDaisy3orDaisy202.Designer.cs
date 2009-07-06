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
        this.m_radBtnDaisy3 = new System.Windows.Forms.RadioButton ();
        this.m_radBtnDaisy202 = new System.Windows.Forms.RadioButton ();
        this.m_OKBtn = new System.Windows.Forms.Button ();
        this.m_BtnCancel = new System.Windows.Forms.Button ();
        this.SuspendLayout ();
        // 
        // m_radBtnDaisy3
        // 
        this.m_radBtnDaisy3.AutoSize = true;
        this.m_radBtnDaisy3.Location = new System.Drawing.Point ( 36, 12 );
        this.m_radBtnDaisy3.Name = "m_radBtnDaisy3";
        this.m_radBtnDaisy3.Size = new System.Drawing.Size ( 63, 17 );
        this.m_radBtnDaisy3.TabIndex = 0;
        this.m_radBtnDaisy3.TabStop = true;
        this.m_radBtnDaisy3.Text = "DAISY3";
        this.m_radBtnDaisy3.UseVisualStyleBackColor = true;
        // 
        // m_radBtnDaisy202
        // 
        this.m_radBtnDaisy202.AutoSize = true;
        this.m_radBtnDaisy202.Location = new System.Drawing.Point ( 174, 12 );
        this.m_radBtnDaisy202.Name = "m_radBtnDaisy202";
        this.m_radBtnDaisy202.Size = new System.Drawing.Size ( 75, 17 );
        this.m_radBtnDaisy202.TabIndex = 1;
        this.m_radBtnDaisy202.TabStop = true;
        this.m_radBtnDaisy202.Text = "DAISY202";
        this.m_radBtnDaisy202.UseVisualStyleBackColor = true;
        // 
        // m_OKBtn
        // 
        this.m_OKBtn.Location = new System.Drawing.Point ( 63, 49 );
        this.m_OKBtn.Name = "m_OKBtn";
        this.m_OKBtn.Size = new System.Drawing.Size ( 75, 23 );
        this.m_OKBtn.TabIndex = 2;
        this.m_OKBtn.Text = "OK";
        this.m_OKBtn.UseVisualStyleBackColor = true;
        this.m_OKBtn.Click += new System.EventHandler ( this.m_OKBtn_Click );
        // 
        // m_BtnCancel
        // 
        this.m_BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.m_BtnCancel.Location = new System.Drawing.Point ( 153, 49 );
        this.m_BtnCancel.Name = "m_BtnCancel";
        this.m_BtnCancel.Size = new System.Drawing.Size ( 75, 23 );
        this.m_BtnCancel.TabIndex = 3;
        this.m_BtnCancel.Text = "Cancel";
        this.m_BtnCancel.UseVisualStyleBackColor = true;
        this.m_BtnCancel.Click += new System.EventHandler ( this.m_BtnCancel_Click );
        // 
        // chooseDaisy3orDaisy202
        // 
        this.AcceptButton = this.m_OKBtn;
        this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.m_BtnCancel;
        this.ClientSize = new System.Drawing.Size ( 295, 97 );
        this.Controls.Add ( this.m_BtnCancel );
        this.Controls.Add ( this.m_OKBtn );
        this.Controls.Add ( this.m_radBtnDaisy202 );
        this.Controls.Add ( this.m_radBtnDaisy3 );
        this.Name = "chooseDaisy3orDaisy202";
        this.Text = "chooseDaisy3orDaisy202";
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.RadioButton m_radBtnDaisy3;
        private System.Windows.Forms.RadioButton m_radBtnDaisy202;
        private System.Windows.Forms.Button m_OKBtn;
        private System.Windows.Forms.Button m_BtnCancel;
    }
}