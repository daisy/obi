namespace Obi.Dialogs
{
    partial class MultipleOptionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultipleOptionDialog));
            this.m_lbl_ChooseOption = new System.Windows.Forms.Label();
            this.m_rdb_SaveBookmarkAndProject = new System.Windows.Forms.RadioButton();
            this.m_rdb_SaveProjectOnly = new System.Windows.Forms.RadioButton();
            this.m_rdb_DiscardBoth = new System.Windows.Forms.RadioButton();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.SuspendLayout();
            // 
            // m_lbl_ChooseOption
            // 
            resources.ApplyResources(this.m_lbl_ChooseOption, "m_lbl_ChooseOption");
            this.m_lbl_ChooseOption.Name = "m_lbl_ChooseOption";
            // 
            // m_rdb_SaveBookmarkAndProject
            // 
            resources.ApplyResources(this.m_rdb_SaveBookmarkAndProject, "m_rdb_SaveBookmarkAndProject");
            this.m_rdb_SaveBookmarkAndProject.Name = "m_rdb_SaveBookmarkAndProject";
            this.m_rdb_SaveBookmarkAndProject.TabStop = true;
            this.m_rdb_SaveBookmarkAndProject.UseVisualStyleBackColor = true;
            this.m_rdb_SaveBookmarkAndProject.CheckedChanged += new System.EventHandler(this.m_rdb_SaveBookmarkAndProject_CheckedChanged);
            // 
            // m_rdb_SaveProjectOnly
            // 
            resources.ApplyResources(this.m_rdb_SaveProjectOnly, "m_rdb_SaveProjectOnly");
            this.m_rdb_SaveProjectOnly.Name = "m_rdb_SaveProjectOnly";
            this.m_rdb_SaveProjectOnly.TabStop = true;
            this.m_rdb_SaveProjectOnly.UseVisualStyleBackColor = true;
            this.m_rdb_SaveProjectOnly.CheckedChanged += new System.EventHandler(this.m_rdb_SaveProjectOnly_CheckedChanged);
            // 
            // m_rdb_DiscardBoth
            // 
            resources.ApplyResources(this.m_rdb_DiscardBoth, "m_rdb_DiscardBoth");
            this.m_rdb_DiscardBoth.Name = "m_rdb_DiscardBoth";
            this.m_rdb_DiscardBoth.TabStop = true;
            this.m_rdb_DiscardBoth.UseVisualStyleBackColor = true;
            this.m_rdb_DiscardBoth.CheckedChanged += new System.EventHandler(this.m_rdb_DiscardBoth_CheckedChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btn_OK, "m_btn_OK");
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btn_Cancel, "m_btn_Cancel");
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // MultipleOptionDialog
            // 
            this.AcceptButton = this.m_btn_OK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btn_Cancel;
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_rdb_DiscardBoth);
            this.Controls.Add(this.m_rdb_SaveProjectOnly);
            this.Controls.Add(this.m_rdb_SaveBookmarkAndProject);
            this.Controls.Add(this.m_lbl_ChooseOption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MultipleOptionDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lbl_ChooseOption;
        private System.Windows.Forms.RadioButton m_rdb_SaveBookmarkAndProject;
        private System.Windows.Forms.RadioButton m_rdb_SaveProjectOnly;
        private System.Windows.Forms.RadioButton m_rdb_DiscardBoth;
        private System.Windows.Forms.Button m_btn_OK;
        private System.Windows.Forms.Button m_btn_Cancel;
        private System.Windows.Forms.HelpProvider helpProvider1;
    }
}