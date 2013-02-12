namespace Obi.Dialogs
{
    partial class SpecialPhraseList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecialPhraseList));
            this.m_grpSelectSpecialPhrases = new System.Windows.Forms.GroupBox();
            this.m_cb_SpecialPhrases = new System.Windows.Forms.ComboBox();
            this.m_btnFind = new System.Windows.Forms.Button();
            this.m_lbSpecialPhrasesList = new System.Windows.Forms.ListBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.mgrp_PlayPhraseInListBox = new System.Windows.Forms.GroupBox();
            this.m_BtnPause = new System.Windows.Forms.Button();
            this.m_BtnStop = new System.Windows.Forms.Button();
            this.m_BtnPlay = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_grpSelectSpecialPhrases.SuspendLayout();
            this.mgrp_PlayPhraseInListBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_grpSelectSpecialPhrases
            // 
            resources.ApplyResources(this.m_grpSelectSpecialPhrases, "m_grpSelectSpecialPhrases");
            this.m_grpSelectSpecialPhrases.Controls.Add(this.m_cb_SpecialPhrases);
            this.m_grpSelectSpecialPhrases.Controls.Add(this.m_btnFind);
            this.m_grpSelectSpecialPhrases.Name = "m_grpSelectSpecialPhrases";
            this.m_grpSelectSpecialPhrases.TabStop = false;
            // 
            // m_cb_SpecialPhrases
            // 
            this.m_cb_SpecialPhrases.FormattingEnabled = true;
            this.m_cb_SpecialPhrases.Items.AddRange(new object[] {
            resources.GetString("m_cb_SpecialPhrases.Items"),
            resources.GetString("m_cb_SpecialPhrases.Items1"),
            resources.GetString("m_cb_SpecialPhrases.Items2"),
            resources.GetString("m_cb_SpecialPhrases.Items3"),
            resources.GetString("m_cb_SpecialPhrases.Items4"),
            resources.GetString("m_cb_SpecialPhrases.Items5"),
            resources.GetString("m_cb_SpecialPhrases.Items6"),
            resources.GetString("m_cb_SpecialPhrases.Items7"),
            resources.GetString("m_cb_SpecialPhrases.Items8")});
            resources.ApplyResources(this.m_cb_SpecialPhrases, "m_cb_SpecialPhrases");
            this.m_cb_SpecialPhrases.Name = "m_cb_SpecialPhrases";
            this.m_cb_SpecialPhrases.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.m_cb_SpecialPhrases_PreviewKeyDown);
            // 
            // m_btnFind
            // 
            resources.ApplyResources(this.m_btnFind, "m_btnFind");
            this.m_btnFind.Name = "m_btnFind";
            this.m_btnFind.UseVisualStyleBackColor = true;
            this.m_btnFind.Click += new System.EventHandler(this.m_btnFind_Click);
            // 
            // m_lbSpecialPhrasesList
            // 
            this.m_lbSpecialPhrasesList.FormattingEnabled = true;
            resources.ApplyResources(this.m_lbSpecialPhrasesList, "m_lbSpecialPhrasesList");
            this.m_lbSpecialPhrasesList.Name = "m_lbSpecialPhrasesList";
            this.m_lbSpecialPhrasesList.SelectedIndexChanged += new System.EventHandler(this.m_lbSpecialPhrasesList_SelectedIndexChanged);
            // 
            // m_btnOK
            // 
            resources.ApplyResources(this.m_btnOK, "m_btnOK");
            this.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            // 
            // m_btnCancel
            // 
            resources.ApplyResources(this.m_btnCancel, "m_btnCancel");
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // mgrp_PlayPhraseInListBox
            // 
            this.mgrp_PlayPhraseInListBox.Controls.Add(this.m_BtnPause);
            this.mgrp_PlayPhraseInListBox.Controls.Add(this.m_BtnStop);
            this.mgrp_PlayPhraseInListBox.Controls.Add(this.m_BtnPlay);
            resources.ApplyResources(this.mgrp_PlayPhraseInListBox, "mgrp_PlayPhraseInListBox");
            this.mgrp_PlayPhraseInListBox.Name = "mgrp_PlayPhraseInListBox";
            this.mgrp_PlayPhraseInListBox.TabStop = false;
            // 
            // m_BtnPause
            // 
            resources.ApplyResources(this.m_BtnPause, "m_BtnPause");
            this.m_BtnPause.Name = "m_BtnPause";
            this.m_BtnPause.UseVisualStyleBackColor = true;
            this.m_BtnPause.Click += new System.EventHandler(this.m_BtnPause_Click);
            // 
            // m_BtnStop
            // 
            resources.ApplyResources(this.m_BtnStop, "m_BtnStop");
            this.m_BtnStop.Name = "m_BtnStop";
            this.m_BtnStop.UseVisualStyleBackColor = true;
            this.m_BtnStop.Click += new System.EventHandler(this.m_BtnStop_Click);
            // 
            // m_BtnPlay
            // 
            resources.ApplyResources(this.m_BtnPlay, "m_BtnPlay");
            this.m_BtnPlay.Name = "m_BtnPlay";
            this.m_BtnPlay.UseVisualStyleBackColor = true;
            this.m_BtnPlay.Click += new System.EventHandler(this.m_BtnPlay_Click);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // SpecialPhraseList
            // 
            this.AcceptButton = this.m_btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.Controls.Add(this.mgrp_PlayPhraseInListBox);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_lbSpecialPhrasesList);
            this.Controls.Add(this.m_grpSelectSpecialPhrases);
            this.MaximizeBox = false;
            this.Name = "SpecialPhraseList";
            this.m_grpSelectSpecialPhrases.ResumeLayout(false);
            this.mgrp_PlayPhraseInListBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox m_grpSelectSpecialPhrases;
        private System.Windows.Forms.ComboBox m_cb_SpecialPhrases;
        private System.Windows.Forms.Button m_btnFind;
        private System.Windows.Forms.ListBox m_lbSpecialPhrasesList;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.GroupBox mgrp_PlayPhraseInListBox;
        private System.Windows.Forms.Button m_BtnStop;
        private System.Windows.Forms.Button m_BtnPlay;
        private System.Windows.Forms.Button m_BtnPause;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}