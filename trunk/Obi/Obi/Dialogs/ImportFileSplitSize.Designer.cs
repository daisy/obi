namespace Obi.Dialogs
{
    partial class ImportFileSplitSize
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
            this.m_lblPhraseSize = new System.Windows.Forms.Label();
            this.m_txtPhraseSize = new System.Windows.Forms.TextBox();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lblPhraseSize
            // 
            this.m_lblPhraseSize.AutoSize = true;
            this.m_lblPhraseSize.Location = new System.Drawing.Point(0, 15);
            this.m_lblPhraseSize.Name = "m_lblPhraseSize";
            this.m_lblPhraseSize.Size = new System.Drawing.Size(116, 13);
            this.m_lblPhraseSize.TabIndex = 0;
            this.m_lblPhraseSize.Text = "Phrase &Size in minutes:";
            // 
            // m_txtPhraseSize
            // 
            this.m_txtPhraseSize.AccessibleName = "Phrase size in minutes:";
            this.m_txtPhraseSize.Location = new System.Drawing.Point(100, 15);
            this.m_txtPhraseSize.Name = "m_txtPhraseSize";
            this.m_txtPhraseSize.Size = new System.Drawing.Size(100, 20);
            this.m_txtPhraseSize.TabIndex = 1;
            // 
            // m_btnOk
            // 
            this.m_btnOk.Location = new System.Drawing.Point(100, 50);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 2;
            this.m_btnOk.Text = "&OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(180, 50);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 3;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // ImportFileSplitSize
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_txtPhraseSize);
            this.Controls.Add(this.m_lblPhraseSize);
            this.Name = "ImportFileSplitSize";
            this.Text = "Choose maximun phrase size";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblPhraseSize;
        private System.Windows.Forms.TextBox m_txtPhraseSize;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
    }
}