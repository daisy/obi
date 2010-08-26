namespace Obi.Dialogs
    {
    partial class GoToPageOrPhrase
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose ( bool disposing )
            {
            if (disposing && (components != null))
                {
                components.Dispose ();
                }
            base.Dispose ( disposing );
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
            {
                this.m_btnOk = new System.Windows.Forms.Button();
                this.m_radPage = new System.Windows.Forms.RadioButton();
                this.m_radPhrase = new System.Windows.Forms.RadioButton();
                this.m_grpSearchPagePhrase = new System.Windows.Forms.GroupBox();
                this.mPhraseIndexComboBox = new System.Windows.Forms.ComboBox();
                this.m_grpSearchPagePhrase.SuspendLayout();
                this.SuspendLayout();
                // 
                // label1
                // 
                this.label1.TabIndex = 3;
                // 
                // mNumberBox
                // 
                this.mNumberBox.TabIndex = 4;
                // 
                // mOKButton
                // 
                this.mOKButton.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
                this.mOKButton.TabIndex = 8;
                // 
                // mCancelButton
                // 
                this.mCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
                this.mCancelButton.TabIndex = 9;
                // 
                // mRenumber
                // 
                this.mRenumber.TabIndex = 7;
                // 
                // mNumberOfPagesBox
                // 
                this.mNumberOfPagesBox.TabIndex = 6;
                // 
                // label2
                // 
                this.label2.TabIndex = 5;
                // 
                // mPageKindComboBox
                // 
                this.mPageKindComboBox.Size = new System.Drawing.Size(184, 24);
                this.mPageKindComboBox.TabIndex = 6;
                // 
                // label3
                // 
                this.label3.TabIndex = 5;
                // 
                // m_btnOk
                // 
                this.m_btnOk.Location = new System.Drawing.Point(90, 167);
                this.m_btnOk.Name = "m_btnOk";
                this.m_btnOk.Size = new System.Drawing.Size(100, 28);
                this.m_btnOk.TabIndex = 7;
                this.m_btnOk.Text = "&OK";
                this.m_btnOk.UseVisualStyleBackColor = true;
                this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
                // 
                // m_radPage
                // 
                this.m_radPage.AutoSize = true;
                this.m_radPage.Location = new System.Drawing.Point(165, 13);
                this.m_radPage.Name = "m_radPage";
                this.m_radPage.Size = new System.Drawing.Size(107, 20);
                this.m_radPage.TabIndex = 1;
                this.m_radPage.TabStop = true;
                this.m_radPage.Text = "Page number";
                this.m_radPage.UseVisualStyleBackColor = true;
                this.m_radPage.CheckedChanged += new System.EventHandler(this.m_radPage_CheckedChanged);
                // 
                // m_radPhrase
                // 
                this.m_radPhrase.AutoSize = true;
                this.m_radPhrase.Location = new System.Drawing.Point(36, 11);
                this.m_radPhrase.Name = "m_radPhrase";
                this.m_radPhrase.Size = new System.Drawing.Size(104, 20);
                this.m_radPhrase.TabIndex = 1;
                this.m_radPhrase.TabStop = true;
                this.m_radPhrase.Text = "Phrase index";
                this.m_radPhrase.UseVisualStyleBackColor = true;
                this.m_radPhrase.CheckedChanged += new System.EventHandler(this.m_radPhrase_CheckedChanged);
                // 
                // m_grpSearchPagePhrase
                // 
                this.m_grpSearchPagePhrase.Controls.Add(this.m_radPhrase);
                this.m_grpSearchPagePhrase.Controls.Add(this.m_radPage);
                this.m_grpSearchPagePhrase.Location = new System.Drawing.Point(37, 12);
                this.m_grpSearchPagePhrase.Name = "m_grpSearchPagePhrase";
                this.m_grpSearchPagePhrase.Size = new System.Drawing.Size(298, 49);
                this.m_grpSearchPagePhrase.TabIndex = 0;
                this.m_grpSearchPagePhrase.TabStop = false;
                this.m_grpSearchPagePhrase.Text = "GoTo Page or Phrase";
                // 
                // mPhraseIndexComboBox
                // 
                this.mPhraseIndexComboBox.FormattingEnabled = true;
                this.mPhraseIndexComboBox.Location = new System.Drawing.Point(189, 130);
                this.mPhraseIndexComboBox.Name = "mPhraseIndexComboBox";
                this.mPhraseIndexComboBox.Size = new System.Drawing.Size(121, 24);
                this.mPhraseIndexComboBox.TabIndex = 10;
                this.mPhraseIndexComboBox.Visible = false;
                // 
                // GoToPageOrPhrase
                // 
                this.AcceptButton = this.m_btnOk;
                this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(389, 208);
                this.Controls.Add(this.m_btnOk);
                this.Controls.Add(this.m_grpSearchPagePhrase);
                this.Controls.Add(this.mPhraseIndexComboBox);
                this.Name = "GoToPageOrPhrase";
                this.Text = "GoToPageOrPhrase";
                this.Controls.SetChildIndex(this.mPhraseIndexComboBox, 0);
                this.Controls.SetChildIndex(this.m_grpSearchPagePhrase, 0);
                this.Controls.SetChildIndex(this.m_btnOk, 0);
                this.Controls.SetChildIndex(this.label1, 0);
                this.Controls.SetChildIndex(this.mNumberBox, 0);
                this.Controls.SetChildIndex(this.mOKButton, 0);
                this.Controls.SetChildIndex(this.mCancelButton, 0);
                this.Controls.SetChildIndex(this.mRenumber, 0);
                this.Controls.SetChildIndex(this.mNumberOfPagesBox, 0);
                this.Controls.SetChildIndex(this.label2, 0);
                this.Controls.SetChildIndex(this.mPageKindComboBox, 0);
                this.Controls.SetChildIndex(this.label3, 0);
                this.m_grpSearchPagePhrase.ResumeLayout(false);
                this.m_grpSearchPagePhrase.PerformLayout();
                this.ResumeLayout(false);
                this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.RadioButton m_radPage;
        private System.Windows.Forms.RadioButton m_radPhrase;
        private System.Windows.Forms.GroupBox m_grpSearchPagePhrase;
        private System.Windows.Forms.ComboBox mPhraseIndexComboBox;
        }
    }