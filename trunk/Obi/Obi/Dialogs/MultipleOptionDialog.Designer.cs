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
            this.m_lbl_ChooseOption = new System.Windows.Forms.Label();
            this.m_rdb_SaveBookmarkAndProject = new System.Windows.Forms.RadioButton();
            this.m_rdb_SaveProjectOnly = new System.Windows.Forms.RadioButton();
            this.m_rdb_DiscardBoth = new System.Windows.Forms.RadioButton();
            this.m_btn_OK = new System.Windows.Forms.Button();
            this.m_btn_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_lbl_ChooseOption
            // 
            this.m_lbl_ChooseOption.AutoSize = true;
            this.m_lbl_ChooseOption.Location = new System.Drawing.Point(42, 22);
            this.m_lbl_ChooseOption.Name = "m_lbl_ChooseOption";
            this.m_lbl_ChooseOption.Size = new System.Drawing.Size(77, 13);
            this.m_lbl_ChooseOption.TabIndex = 0;
            this.m_lbl_ChooseOption.Text = "Choose Option";
            // 
            // m_rdb_SaveBookmarkAndProject
            // 
            this.m_rdb_SaveBookmarkAndProject.AutoSize = true;
            this.m_rdb_SaveBookmarkAndProject.Location = new System.Drawing.Point(45, 51);
            this.m_rdb_SaveBookmarkAndProject.Name = "m_rdb_SaveBookmarkAndProject";
            this.m_rdb_SaveBookmarkAndProject.Size = new System.Drawing.Size(158, 17);
            this.m_rdb_SaveBookmarkAndProject.TabIndex = 1;
            this.m_rdb_SaveBookmarkAndProject.TabStop = true;
            this.m_rdb_SaveBookmarkAndProject.Text = "Save Bookmark and Project";
            this.m_rdb_SaveBookmarkAndProject.UseVisualStyleBackColor = true;
            this.m_rdb_SaveBookmarkAndProject.CheckedChanged += new System.EventHandler(this.m_rdb_SaveBookmarkAndProject_CheckedChanged);
            // 
            // m_rdb_SaveProjectOnly
            // 
            this.m_rdb_SaveProjectOnly.AutoSize = true;
            this.m_rdb_SaveProjectOnly.Location = new System.Drawing.Point(45, 74);
            this.m_rdb_SaveProjectOnly.Name = "m_rdb_SaveProjectOnly";
            this.m_rdb_SaveProjectOnly.Size = new System.Drawing.Size(110, 17);
            this.m_rdb_SaveProjectOnly.TabIndex = 3;
            this.m_rdb_SaveProjectOnly.TabStop = true;
            this.m_rdb_SaveProjectOnly.Text = "Save Project Only";
            this.m_rdb_SaveProjectOnly.UseVisualStyleBackColor = true;
            this.m_rdb_SaveProjectOnly.CheckedChanged += new System.EventHandler(this.m_rdb_SaveProjectOnly_CheckedChanged);
            // 
            // m_rdb_DiscardBoth
            // 
            this.m_rdb_DiscardBoth.AutoSize = true;
            this.m_rdb_DiscardBoth.Location = new System.Drawing.Point(45, 97);
            this.m_rdb_DiscardBoth.Name = "m_rdb_DiscardBoth";
            this.m_rdb_DiscardBoth.Size = new System.Drawing.Size(85, 17);
            this.m_rdb_DiscardBoth.TabIndex = 4;
            this.m_rdb_DiscardBoth.TabStop = true;
            this.m_rdb_DiscardBoth.Text = "Discard both";
            this.m_rdb_DiscardBoth.UseVisualStyleBackColor = true;
            this.m_rdb_DiscardBoth.CheckedChanged += new System.EventHandler(this.m_rdb_DiscardBoth_CheckedChanged);
            // 
            // m_btn_OK
            // 
            this.m_btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btn_OK.Location = new System.Drawing.Point(44, 136);
            this.m_btn_OK.Name = "m_btn_OK";
            this.m_btn_OK.Size = new System.Drawing.Size(75, 23);
            this.m_btn_OK.TabIndex = 5;
            this.m_btn_OK.Text = "OK";
            this.m_btn_OK.UseVisualStyleBackColor = true;
            this.m_btn_OK.Click += new System.EventHandler(this.m_btn_OK_Click);
            // 
            // m_btn_Cancel
            // 
            this.m_btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btn_Cancel.Location = new System.Drawing.Point(160, 136);
            this.m_btn_Cancel.Name = "m_btn_Cancel";
            this.m_btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.m_btn_Cancel.TabIndex = 6;
            this.m_btn_Cancel.Text = "Cancel";
            this.m_btn_Cancel.UseVisualStyleBackColor = true;
            this.m_btn_Cancel.Click += new System.EventHandler(this.m_btn_Cancel_Click);
            // 
            // MultipleOptionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 175);
            this.Controls.Add(this.m_btn_Cancel);
            this.Controls.Add(this.m_btn_OK);
            this.Controls.Add(this.m_rdb_DiscardBoth);
            this.Controls.Add(this.m_rdb_SaveProjectOnly);
            this.Controls.Add(this.m_rdb_SaveBookmarkAndProject);
            this.Controls.Add(this.m_lbl_ChooseOption);
            this.Name = "MultipleOptionDialog";
            this.Text = "MultipleOptionDialog";
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
    }
}