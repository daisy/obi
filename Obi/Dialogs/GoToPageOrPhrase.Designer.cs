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
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToPageOrPhrase));
                this.m_btnOk = new System.Windows.Forms.Button();
                this.m_radPage = new System.Windows.Forms.RadioButton();
                this.m_radPhrase = new System.Windows.Forms.RadioButton();
                this.m_grpSearchPagePhrase = new System.Windows.Forms.GroupBox();
                this.m_radTime = new System.Windows.Forms.RadioButton();
                this.mPhraseIndexComboBox = new System.Windows.Forms.ComboBox();
                this.m_cb_TimeInPhraseOrSection = new System.Windows.Forms.ComboBox();
                this.m_txtBox_TimeInSeconds = new System.Windows.Forms.TextBox();
                this.m_lbl_Seconds = new System.Windows.Forms.Label();
                this.m_lbl_Time = new System.Windows.Forms.Label();
                this.helpProvider1 = new System.Windows.Forms.HelpProvider();
                this.label4 = new System.Windows.Forms.Label();
                this.m_grpSearchPagePhrase.SuspendLayout();
                this.SuspendLayout();
                // 
                // label1
                // 
                resources.ApplyResources(this.label1, "label1");
                // 
                // mNumberBox
                // 
                resources.ApplyResources(this.mNumberBox, "mNumberBox");
                // 
                // mOKButton
                // 
                resources.ApplyResources(this.mOKButton, "mOKButton");
                // 
                // mCancelButton
                // 
                resources.ApplyResources(this.mCancelButton, "mCancelButton");
                // 
                // mRenumber
                // 
                resources.ApplyResources(this.mRenumber, "mRenumber");
                // 
                // mNumberOfPagesBox
                // 
                resources.ApplyResources(this.mNumberOfPagesBox, "mNumberOfPagesBox");
                // 
                // label2
                // 
                resources.ApplyResources(this.label2, "label2");
                // 
                // mPageKindComboBox
                // 
                resources.ApplyResources(this.mPageKindComboBox, "mPageKindComboBox");
                // 
                // label3
                // 
                resources.ApplyResources(this.label3, "label3");
                // 
                // m_btnOk
                // 
                resources.ApplyResources(this.m_btnOk, "m_btnOk");
                this.m_btnOk.Name = "m_btnOk";
                this.m_btnOk.UseVisualStyleBackColor = true;
                this.m_btnOk.Click += new System.EventHandler(this.m_btnOk_Click);
                // 
                // m_radPage
                // 
                resources.ApplyResources(this.m_radPage, "m_radPage");
                this.m_radPage.Name = "m_radPage";
                this.m_radPage.TabStop = true;
                this.m_radPage.UseVisualStyleBackColor = true;
                this.m_radPage.CheckedChanged += new System.EventHandler(this.m_radPage_CheckedChanged);
                // 
                // m_radPhrase
                // 
                resources.ApplyResources(this.m_radPhrase, "m_radPhrase");
                this.m_radPhrase.Name = "m_radPhrase";
                this.m_radPhrase.TabStop = true;
                this.m_radPhrase.UseVisualStyleBackColor = true;
                this.m_radPhrase.CheckedChanged += new System.EventHandler(this.m_radPhrase_CheckedChanged);
                // 
                // m_grpSearchPagePhrase
                // 
                this.m_grpSearchPagePhrase.Controls.Add(this.m_radTime);
                this.m_grpSearchPagePhrase.Controls.Add(this.m_radPhrase);
                this.m_grpSearchPagePhrase.Controls.Add(this.m_radPage);
                resources.ApplyResources(this.m_grpSearchPagePhrase, "m_grpSearchPagePhrase");
                this.m_grpSearchPagePhrase.Name = "m_grpSearchPagePhrase";
                this.m_grpSearchPagePhrase.TabStop = false;
                // 
                // m_radTime
                // 
                resources.ApplyResources(this.m_radTime, "m_radTime");
                this.m_radTime.Name = "m_radTime";
                this.m_radTime.TabStop = true;
                this.m_radTime.UseVisualStyleBackColor = true;
                this.m_radTime.CheckedChanged += new System.EventHandler(this.m_radTime_CheckedChanged);
                // 
                // mPhraseIndexComboBox
                // 
                this.mPhraseIndexComboBox.FormattingEnabled = true;
                resources.ApplyResources(this.mPhraseIndexComboBox, "mPhraseIndexComboBox");
                this.mPhraseIndexComboBox.Name = "mPhraseIndexComboBox";
                // 
                // m_cb_TimeInPhraseOrSection
                // 
                this.m_cb_TimeInPhraseOrSection.FormattingEnabled = true;
                this.m_cb_TimeInPhraseOrSection.Items.AddRange(new object[] {
            resources.GetString("m_cb_TimeInPhraseOrSection.Items"),
            resources.GetString("m_cb_TimeInPhraseOrSection.Items1"),
            resources.GetString("m_cb_TimeInPhraseOrSection.Items2")});
                resources.ApplyResources(this.m_cb_TimeInPhraseOrSection, "m_cb_TimeInPhraseOrSection");
                this.m_cb_TimeInPhraseOrSection.Name = "m_cb_TimeInPhraseOrSection";
                // 
                // m_txtBox_TimeInSeconds
                // 
                resources.ApplyResources(this.m_txtBox_TimeInSeconds, "m_txtBox_TimeInSeconds");
                this.m_txtBox_TimeInSeconds.Name = "m_txtBox_TimeInSeconds";
                // 
                // m_lbl_Seconds
                // 
                resources.ApplyResources(this.m_lbl_Seconds, "m_lbl_Seconds");
                this.m_lbl_Seconds.Name = "m_lbl_Seconds";
                // 
                // m_lbl_Time
                // 
                resources.ApplyResources(this.m_lbl_Time, "m_lbl_Time");
                this.m_lbl_Time.Name = "m_lbl_Time";
                // 
                // helpProvider1
                // 
                resources.ApplyResources(this.helpProvider1, "helpProvider1");
                // 
                // label4
                // 
                resources.ApplyResources(this.label4, "label4");
                this.label4.Name = "label4";
                // 
                // GoToPageOrPhrase
                // 
                this.AcceptButton = this.m_btnOk;
                resources.ApplyResources(this, "$this");
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.Controls.Add(this.label4);
                this.Controls.Add(this.m_lbl_Seconds);
                this.Controls.Add(this.m_txtBox_TimeInSeconds);
                this.Controls.Add(this.m_btnOk);
                this.Controls.Add(this.mPhraseIndexComboBox);
                this.Controls.Add(this.m_cb_TimeInPhraseOrSection);
                this.Controls.Add(this.m_lbl_Time);
                this.Controls.Add(this.m_grpSearchPagePhrase);
                this.Name = "GoToPageOrPhrase";
                this.Controls.SetChildIndex(this.mPageKindComboBox, 0);
                this.Controls.SetChildIndex(this.m_grpSearchPagePhrase, 0);
                this.Controls.SetChildIndex(this.m_lbl_Time, 0);
                this.Controls.SetChildIndex(this.m_cb_TimeInPhraseOrSection, 0);
                this.Controls.SetChildIndex(this.mPhraseIndexComboBox, 0);
                this.Controls.SetChildIndex(this.m_btnOk, 0);
                this.Controls.SetChildIndex(this.mRenumber, 0);
                this.Controls.SetChildIndex(this.mNumberBox, 0);
                this.Controls.SetChildIndex(this.label1, 0);
                this.Controls.SetChildIndex(this.m_txtBox_TimeInSeconds, 0);
                this.Controls.SetChildIndex(this.m_lbl_Seconds, 0);
                this.Controls.SetChildIndex(this.label3, 0);
                this.Controls.SetChildIndex(this.label4, 0);
                this.Controls.SetChildIndex(this.mNumberOfPagesBox, 0);
                this.Controls.SetChildIndex(this.mOKButton, 0);
                this.Controls.SetChildIndex(this.mCancelButton, 0);
                this.Controls.SetChildIndex(this.label2, 0);
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
        private System.Windows.Forms.RadioButton m_radTime;
        private System.Windows.Forms.ComboBox m_cb_TimeInPhraseOrSection;
        private System.Windows.Forms.TextBox m_txtBox_TimeInSeconds;
        private System.Windows.Forms.Label m_lbl_Seconds;
        private System.Windows.Forms.Label m_lbl_Time;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label label4;
        }
    }